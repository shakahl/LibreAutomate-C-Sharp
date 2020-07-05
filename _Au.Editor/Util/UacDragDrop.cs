using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;

class UacDragDrop
{
	public class AdminProcess
	{
		AHookAcc _hook; //SYSTEM_CAPTURESTART
		ATimer _timer; //tracks mouse etc
		bool _isDragMode; //is in drag-drop
		bool _isProcess2; //is our non-admin process started
		int _endCounter; //delays ending drag mode

		static AdminProcess s_inst;

		//Called with on=true when main form becomes visible.
		//Called with on=false when hidden or closed.
		public static void Enable(bool on)
		{
			if(on == (s_inst != null)) return;
			if(on) {
				if(AUac.OfThisProcess.Elevation != UacElevation.Full) return;
				s_inst = new AdminProcess();
			} else {
				s_inst._Dispose();
				s_inst = null;
			}
		}

		AdminProcess()
		{
			_timer = new ATimer(_ => _Timer());

			//use hook to detect when drag-drop started
			_hook = new AHookAcc(AccEVENT.SYSTEM_CAPTURESTART, 0, d => {
				_EndedDragMode();
				if(0 == d.wnd.ClassNameIs("CLIPBRDWNDCLASS", "DragWindow")) return;
				_StartedDragMode();
			}, flags: AccHookFlags.SKIPOWNPROCESS);

			//note: we don't use SYSTEM_CAPTUREEND. It's too early and sometimes missing.
			//tested: no EVENT_SYSTEM_DRAGDROPSTART events.
			//info: classname "DragWindow" is of Windows Store.
			//rejected: int pid = d.wnd.ProcessId; using(var u = AUac.OfProcess(pid)) { if(u == null || u.Elevation == UacElevation.Full) return; }
		}

		void _Dispose()
		{
			_hook.Dispose();
			_EndedDragMode();
		}

		void _StartedDragMode()
		{
			//AOutput.Write("START");
			_isDragMode = true;
			_endCounter = 0;
			_wTransparent = default;
			_wWindow = default;
			_wTargetControl = default;
			_data = null;
			_timer.Every(30);
		}

		void _EndedDragMode()
		{
			if(!_isDragMode) return;
			//AOutput.Write("END");
			_isDragMode = _isProcess2 = false;
			_timer.Stop();
			if(!_wTransparent.Is0) _wTransparent.Post(Api.WM_USER); //let it close self
		}

		//Every 30 ms while in drag mode.
		void _Timer()
		{
			if(!_isDragMode) return;

			//when mouse released, end drag mode with ~100 ms delay
			if(!AMouse.IsPressed(MButtons.Left | MButtons.Right)) { //calls GetKeyStateAsync. GetKeyState somehow unreliable when in drag mode.
				if(++_endCounter == 4) _EndedDragMode();
				return;
			}
			_endCounter = 0;
			//end drag mode if _wTransparent died, eg did not find useful data on drag enter
			if(!_wTransparent.Is0 && !_wTransparent.IsAlive) {
				_EndedDragMode();
				return;
			}

			var w = AWnd.FromMouse(WXYFlags.NeedWindow);
			if(!_isProcess2) {
				if(!w.IsOfThisProcess) return;
				//AOutput.Write("drag");
				_isProcess2 = true;
				_wWindow = w;
				new Au.Util.ProcessStarter_("Au.Editor.exe", "/dd " + CommandLine.MsgWnd.Handle.ToString()).StartUserIL();
				//new Au.Util.ProcessStarter_("Au.Editor.exe", $"/dd {CommandLine.MsgWnd.Handle.ToString()} {ATime.PerfMilliseconds}").StartUserIL(); //test process startup speed
			} else if(w != _wTransparent) {
				_wWindow = w;
				_SetTransparentSizeZorder();
			}
		}

		AWnd _wTransparent; //our transparent non-admin window
		AWnd _wWindow; //current our top-level admin window covered by _wTransparent

		//Called in admin process. Non-admin process may not be able to zorder its window above admin windows.
		void _SetTransparentSizeZorder()
		{
			if(_wWindow.IsOfThisProcess) {
				var r = _wWindow.Rect;
				_wTransparent.MoveLL(r.left, r.top, r.Width, r.Height);
				_wTransparent.ZorderAbove(_wWindow);
			} else {
				_wTransparent.MoveLL(0, 0, 0, 0);
			}
		}

		//SendMessage from _wTransparent on WM_CREATE.
		public static void OnTransparentWindowCreated(AWnd wTransparent)
		{
			var x = s_inst;
			if(x?._isDragMode ?? false) {
				x._wTransparent = wTransparent;
				x._SetTransparentSizeZorder();
			} else wTransparent.Post(Api.WM_USER); //let it close self
		}

		public static DragDropEffects OnDragEvent(int event_, int keyState, byte[] b = null) => s_inst?._OnDragEvent(event_, keyState, b) ?? 0;
		DragDropEffects _OnDragEvent(int event_, int keyState, byte[] b)
		{
			if(!_isDragMode) return 0;
			DDEvent ev = (DDEvent)event_;
			//if(ev != DDEvent.Over) AOutput.Write(ev);
			if(ev == DDEvent.Enter) {
				var a = Au.Util.Serializer_.Deserialize(b);
				_allowedEffects = (DragDropEffects)(int)a[0];
				_keyState = a[1];
				_data = new DataObject();
				var t = new _DDData { files = a[2], shell = a[3], text = a[4], linkName = a[5] };
				if(t.files != null) _data.SetData("FileDrop", t.files);
				if(t.shell != null) _data.SetData("Shell IDList Array", t.shell);
				if(t.text != null) _data.SetData("UnicodeText", t.text);
				if(t.linkName != null) _data.SetData("FileGroupDescriptorW", t.linkName);
			} else {
				_keyState = keyState;
			}

			DragDropEffects ef = 0;
			if(ev != DDEvent.Leave) {
				var p = AMouse.XY;
				var w = _wWindow.ChildFromXY(p, WXYCFlags.ScreenXY);
				if(w.Is0) w = _wWindow;
				if(w != _wTargetControl && !_wTargetControl.Is0) {
					_InvokeDDHandler(_wTargetControl, DDEvent.Leave);
					_wTargetControl = default;
				}
				if(!w.Is0 && w.IsOfThisProcess && w.IsEnabled(true)) {
					if(ev != 0 && _wTargetControl.Is0) {
						if(ev == DDEvent.Over) ev = 0;
						else _InvokeDDHandler(w, DDEvent.Enter, p);
					}
					ef = _InvokeDDHandler(_wTargetControl = w, ev, p);
				}
			} else if(!_wTargetControl.Is0) {
				_InvokeDDHandler(_wTargetControl, ev);
			}

			if(ev >= DDEvent.Drop) { _wTargetControl = default; }

			return ef;
		}

		//data etc sent by _wTransparent on OnDragEnter. Then used here on enter/over/drop.
		DataObject _data;
		DragDropEffects _allowedEffects;
		int _keyState; //set on enter, then updated on over/drop

		AWnd _wTargetControl; //control or form from mouse

		//If the control alows drop, calls its OnDragEnter/Over/Drop/Leave through reflection.
		DragDropEffects _InvokeDDHandler(AWnd w, DDEvent ev, POINT p = default)
		{
			//AOutput.Write(ev, w);
			var c = Control.FromHandle(w.Handle); //is it thread-safe? It seems yes.
			if(c == null || !c.AllowDrop) return 0; //thread-safe

			if(_miEnter == null) {
				var t = typeof(Control);
				var bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
				_miEnter = t.GetMethod("OnDragEnter", bf);
				_miOver = t.GetMethod("OnDragOver", bf);
				_miDrop = t.GetMethod("OnDragDrop", bf);
				_miLeave = t.GetMethod("OnDragLeave", bf);
			}

			if(ev == DDEvent.Leave) {
				_Invoke(_miLeave, EventArgs.Empty);
				return 0;
			}

			var e = new DragEventArgs(_data, _keyState, p.x, p.y, _allowedEffects, DragDropEffects.None);
			MethodInfo mi;
			switch(ev) {
			case DDEvent.Enter: mi = _miEnter; break;
			case DDEvent.Over: mi = _miOver; break;
			default: mi = _miDrop; break;
			}
			_Invoke(mi, e);
			return e.Effect;

			void _Invoke(MethodInfo method, object arg)
			{
				if(c.InvokeRequired) {
					c.Invoke(new MethodInvoker(() => method.Invoke(c, new object[] { arg })));
				} else {
					method.Invoke(c, new object[] { arg });
				}
			}
		}
		MethodInfo _miEnter, _miOver, _miDrop, _miLeave; //MethodInfo cache
	}

	//Drag-drop events.
	public enum DDEvent { Enter, Over, Drop, Leave }

	//A form in non-admin process that accepts drag-drop events and relays to the admin process.
	//Covers our admin window. Almost transparent.
	public class NonAdminProcess : Form
	{
		//Called from Main() in non-admin process when command line starts with /dd.
		public static void MainDD(string[] args)
		{
			//AOutput.Write("NonAdminProcess");
			var msgWnd = (AWnd)args[1].ToInt();

			var f = new NonAdminProcess(msgWnd);
			f.ShowDialog();
			f.Dispose();
			//AOutput.Write("exit");
		}

		AWnd _msgWnd; //message-only IPC window in admin process

		NonAdminProcess(AWnd msgWnd)
		{
			_msgWnd = msgWnd;
			Text = "Au.DropTarget";
			AllowDrop = true;
			AutoScaleMode = AutoScaleMode.None;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;
			Opacity = 0.05;
			//Opacity = 0.7;
			Bounds = default; //on WM_CREATE we'll send message to the admin process, let it set size and Z order
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams k = base.CreateParams;
				k.ExStyle = (int)(WS2.LAYERED | WS2.NOACTIVATE | WS2.TOOLWINDOW | WS2.TOPMOST);
				k.Style = unchecked((int)(WS.POPUP | WS.DISABLED));
				return k;
			}
		}

		protected override bool ShowWithoutActivation => true;

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_CREATE:
				_msgWnd.Send(Api.WM_USER, 10, m.HWnd);
				Api.SetTimer((AWnd)m.HWnd, 1, 1000, null);
				break;
			case Api.WM_TIMER:
				if(!_msgWnd.IsAlive) Hide();
				break;
			case Api.WM_USER: //admin posts it when ended drag mode
				Hide();
				//AOutput.Write("WM_USER");
				break;
			}
			base.WndProc(ref m);

			//test process startup speed. About 300 ms. Ngened slower.
			//if(m.Msg == Api.WM_SHOWWINDOW && m.WParam != default) AOutput.Write(ATime.PerfMilliseconds - Environment.GetCommandLineArgs()[3].ToInt64());
		}

		bool _enteredOnce;

		protected override void OnDragEnter(DragEventArgs e)
		{
			if(_enteredOnce) {
				OnDragOver(e);
				return;
			}
			_enteredOnce = true;

			_DDData r = default;
			if(r.GetData(e.Data)) {
				var b = Au.Util.Serializer_.Serialize((int)e.AllowedEffect, e.KeyState, r.files, r.shell, r.text, r.linkName);
				e.Effect = (DragDropEffects)(int)AWnd.More.CopyDataStruct.SendBytes(_msgWnd, 110, b);
			} else {
				Hide();
			}
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			e.Effect = (DragDropEffects)(int)_msgWnd.Send(Api.WM_USER, (int)DDEvent.Over, e.KeyState);
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			Hide();
			e.Effect = (DragDropEffects)(int)_msgWnd.Send(Api.WM_USER, (int)DDEvent.Drop, e.KeyState);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			_msgWnd.Send(Api.WM_USER, (int)DDEvent.Leave);
		}
	}

	struct _DDData
	{
		public string[] files;
		public byte[] shell, linkName;
		public string text;

		public bool GetData(IDataObject d)
		{
			//foreach(var v in d.GetFormats()) AOutput.Write(v, d.GetData(v, false)?.GetType()); AOutput.Write("--");
			try {
				if(d.GetDataPresent(DataFormats.FileDrop, false)) { //files
					files = d.GetData(DataFormats.FileDrop, false) as string[]; if(files == null) return false;
				} else if(d.GetDataPresent("Shell IDList Array", false)) { //any shell objects
					if(!(d.GetData("Shell IDList Array", false) is MemoryStream m)) return false;
					shell = m.ToArray();
				} else if(d.GetDataPresent("UnicodeText", false)) { //text or URL
					text = d.GetData("UnicodeText", false) as string; if(text == null) return false;
					if(d.GetDataPresent("FileGroupDescriptorW", true)) { //link text. All browsers.
						if(!(d.GetData("FileGroupDescriptorW", false) is MemoryStream m)) return false;
						linkName = m.ToArray();
					}
				} else return false;
				return true;
			}
			catch(Exception ex) { ADebug.Print(ex); return false; }

			//note: MemoryStream.GetBuffer says "UnauthorizedAccessException: MemoryStream's internal buffer cannot be accessed"
			//info: if from IE, GetData returns null for all formats.
		}
	}

}
