using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

#pragma warning disable 1591 //XML doc //TODO

namespace Catkeys
{
	/// <summary>
	/// TODO
	/// </summary>
	public class CatBar :CatMenu_CatBar_Base
	{
		static Wnd.Misc.WindowClass _WndClass = Wnd.Misc.WindowClass.Register("CatBar", _WndProc, IntPtr.Size, Api.CS_GLOBALCLASS);

		Wnd _w;
		ToolStrip_ _ts;

		/// <summary>
		/// Gets ToolStrip.
		/// </summary>
		public ToolStrip Ex { get => _ts; }

		//Our base uses this.
		protected override ToolStrip MainToolStrip { get => _ts; }

		public CatBar()
		{
			_Init();
		}

		/// <summary>
		/// Adds new button as ToolStripButton.
		/// Sets its text, icon and Click event handler delegate. Other properties can be specified later. See example.
		/// Code <c>t.Add("text", o => Print(o));</c> is the same as <c>t["text"] = o => Print(o);</c> .
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">Can be:
		/// string - icon file, as with <see cref="Icons.GetFileIconHandle"/>.
		/// string - image name (key) in the ImageList (<see cref="ToolStripItem.ImageKey"/>).
		/// int - image index in the ImageList (<see cref="ToolStripItem.ImageIndex"/>).
		/// IntPtr - unmanaged icon handle (the function makes its own copy).
		/// Icon, Image, Folders.FolderPath.
		/// </param>
		/// <example><code>
		/// var t = new CatBar();
		/// t["One"] = o => Print(o);
		/// t["Two", @"icon file path"] = o => { Print(o); TaskDialog.Show(o.ToString()); };
		/// t.LastItem.ToolTipText = "tooltip";
		/// </code></example>
		public Action<ClickEventData> this[string text, object icon = null]
		{
			set { Add(text, value, icon); }
		}

		/// <summary>
		/// Adds new button as ToolStripButton.
		/// Sets its text, icon and Click event handler delegate. Other properties can be specified later. See example.
		/// Code <c>t.Add("text", o => Print(o));</c> is the same as <c>t["text"] = o => Print(o);</c> .
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onClick">Lambda etc function to be called when the button clicked.</param>
		/// <param name="icon">Can be:
		/// string - icon file, as with <see cref="Icons.GetFileIconHandle"/>.
		/// string - image name (key) in the ImageList (<see cref="ToolStripItem.ImageKey"/>).
		/// int - image index in the ImageList (<see cref="ToolStripItem.ImageIndex"/>).
		/// IntPtr - unmanaged icon handle (the function makes its own copy).
		/// Icon, Image, Folders.FolderPath.
		/// </param>
		/// <example><code>
		/// var m = new CatBar();
		/// t.Add("One", o => Print(o), @"icon file path");
		/// t.LastItem.ToolTipText = "tooltip";
		/// t.Add("Two", o => { Print(o.MenuItem.Checked); });
		/// </code></example>
		public ToolStripButton Add(string text, Action<ClickEventData> onClick, object icon = null)
		{
			var item = new ToolStripButton(text);
			_Items.Add(item);
			_SetItemProp(true, item, onClick, icon);
			return item;
		}

		/// <summary>
		/// Adds item of any supported type, for example ToolStripLabel, ToolStripTextBox, ToolStripComboBox, ToolStripProgressBar.
		/// Supports types derived from ToolStripItem.
		/// </summary>
		/// <param name="item">An already created item of any supported type.</param>
		/// <param name="icon">The same as with other overload.</param>
		/// <param name="onClick">Lambda etc function to be called when the item clicked. Not useful for most item types.</param>
		public void Add(ToolStripItem item, object icon = null, Action<ClickEventData> onClick = null)
		{
			_Items.Add(item);
			_SetItemProp(true, item, onClick, icon);

			//Activate window when a child control clicked, or something may not work, eg cannot enter text in Edit control.
			var cb = item as ToolStripControlHost; //combo, edit, progress
			if(cb != null) cb.GotFocus += _Item_GotFocus; //info: this is before MouseDown, which does not work well with combo box
		}

		void _Item_GotFocus(object sender, EventArgs e)
		{
			//PrintFunc();
			//_w.ActivateLL();
			Api.SetForegroundWindow(_w); //does not fail, probably after a mouse click this process is allowed to activate windows, even if the click did not activate because of the window style
		}

		/// <summary>
		/// Adds separator.
		/// </summary>
		public ToolStripSeparator Separator()
		{
			var item = new ToolStripSeparator();
			_Items.Add(item);
			LastItem = item;
			return item;
		}

		ToolStripItemCollection _Items
		{
			get => _ts.Items;
		}

		/// <summary>
		/// Gets the last added item as ToolStripButton.
		/// Returns null if it is not a ToolStripButton.
		/// The item can be added with m.Add(...) and m[...]=.
		/// </summary>
		/// <remarks>
		/// You can instead use LastItem, which gets ToolStripItem, which is the base class of all supported item types; cast it to a derived type if need.
		/// </remarks>
		public ToolStripButton LastButton { get => LastItem as ToolStripButton; }

		void _Init()
		{
			Perf.Next();
			uint exStyle = Native.WS_EX_TOOLWINDOW | Native.WS_EX_NOACTIVATE | Native.WS_EX_TOPMOST;
			uint style = Native.WS_POPUP;
			//style |= Native.WS_CAPTION | Native.WS_SYSMENU;

			RECT r = Screen.PrimaryScreen.WorkingArea;
			r.Inflate(-2, -2); r.top += 4;

			Api.CreateWindowEx(exStyle, _WndClass.Name, null, style, r.top, r.left, r.Width, r.Height, Wnd0, 0, Zero, (IntPtr)GCHandle.Alloc(this));
			if(_w.Is0) throw new Win32Exception();
			Perf.Next();

			_ts = new ToolStrip_(this);
			_ts.SuspendLayout();

			_ts.SetBounds(r.top, r.left, r.Width, r.Height);

			//then caller will add buttons etc and call Visible=true, which calls _ts.ResumeLayout and _ts.CreateControl
		}

		public bool Visible
		{
			get => _w.IsVisible;
			set
			{
				_GetIconsAsync(_ts);
				if(!_ts.Created) {
					_ts.ResumeLayout();
					_ts.CreateControl();
					Perf.Next();
				}
				_w.ShowLL(value);
				_showTime = Time.Milliseconds;
			}
		}

		long _showTime;

		public void Close()
		{
			if(_w.IsAlive) _w.Close();
		}

		/// <summary>
		/// Gets the main toolbar window.
		/// </summary>
		public Wnd MainWnd { get => _w; }

#pragma warning disable 649
		struct _CREATESTRUCT
		{
			public IntPtr lpCreateParams;
			public IntPtr hInstance;
			public IntPtr hMenu;
			public Wnd hwndParent;
			public int cy;
			public int cx;
			public int y;
			public int x;
			public int style;
			public IntPtr lpszName;
			public IntPtr lpszClass;
			public uint dwExStyle;
		}
#pragma warning restore 649

		//[ThreadStatic] static Dictionary<Wnd, CatBar> _objects; //adds 3.3 ms at startup
		//[ThreadStatic] static SortedList<Wnd, CatBar> _objects; //adds 3.6 ms at startup
		[ThreadStatic] static System.Collections.Hashtable _objects; //adds 0.35 ms at startup

		//TODO: test Hashtable retrieving speed. Maybe even does not work because need boxing.

		static unsafe LPARAM _WndProc(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
		{
			//Perf.First();
			CatBar x;
			if(msg == Api.WM_NCCREATE) {
				_CREATESTRUCT* cs = (_CREATESTRUCT*)lParam;
				var g = (GCHandle)cs->lpCreateParams;
				x = g.Target as CatBar;
				g.Free();
				x._w = w;
				//Perf.Next();
				if(_objects == null) _objects = new System.Collections.Hashtable();
				_objects.Add(w, x);
			} else {
				//if(!_objects.TryGetValue(w, out x)) return Api.DefWindowProc(w, msg, wParam, lParam);
				x = _objects?[w] as CatBar; if(x == null) return Api.DefWindowProc(w, msg, wParam, lParam);
			}
			//Perf.NW();
			//Print(x.MainWnd);

			LPARAM R = x._WndProc(msg, wParam, lParam);

			if(msg == Api.WM_NCDESTROY) {
				_objects?.Remove(w);
			}
			return R;
		}

		//static unsafe LPARAM _WndProc(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
		//{
		//	Perf.First();
		//	IntPtr ipThis;
		//	if(msg == Api.WM_NCCREATE) {
		//		_CREATESTRUCT* cs = (_CREATESTRUCT*)lParam;
		//		w.SetWindowLong(0, ipThis = cs->lpCreateParams);
		//	} else {
		//		ipThis = w.GetWindowLong(0);
		//		if(ipThis == Zero) return Api.DefWindowProc(w, msg, wParam, lParam);
		//	}
		//	Perf.Next();
		//	var gch = (GCHandle)ipThis;
		//	var x = gch.TargetPath as CatBar;
		//	if(msg == Api.WM_NCCREATE) x._w = w;
		//	Perf.NW();
		//	//Print(x.MainWnd);

		//	LPARAM R = x._WndProc(msg, wParam, lParam);

		//	if(msg == Api.WM_NCDESTROY) {
		//		w.SetWindowLong(0, 0);
		//		gch.Free();
		//	}
		//	return R;
		//}

		LPARAM _WndProc(uint msg, LPARAM wParam, LPARAM lParam)
		{

			LPARAM R = Api.DefWindowProc(_w, msg, wParam, lParam);

			switch(msg) {
			case Api.WM_NCDESTROY:
				//_mlTb.Stop();
				break;
			case Api.WM_PAINT:
				//Print("painted");
				//if(!_focusedOnce) {
				//	Perf.NW();
				//	_focusedOnce = true;
				//	if(!_ts.Focused) _ts.Focus(); //solves problem when in native window: the first button-click does not work. This takes several milliseconds therefore is after painting. But here it is much slower and can create problems, eg flickering, more slow startup of next toolbar.
				//}
				break;
			}
			return R;
		}



		class ToolStrip_ :ToolStrip, _ICatToolStrip
		{
			CatBar _parent;

			internal ToolStrip_(CatBar parent) { _parent = parent; }

			protected override CreateParams CreateParams
			{
				get
				{
					//Print("CreateParams");
					var p = base.CreateParams;
					if(_parent != null) { //this prop is called 3 times, first time before ctor
						p.Parent = _parent.MainWnd.Handle;

						//TODO: window name should be not exactly as script name.
						//	Because need to prevent finding it instead of eg its owner window when Wnd.Find is used with partial name and without class name.
						//	Even if ucase like QM 2, because Wnd.Find is case-insensitive.
						//	Maybe "s.c.r.i.p.t".
					}
					return p;
				}
			}

			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);

				//if(_AsyncIcons != null && _AsyncIcons.Count > 0) _AsyncIcons.GetAllAsync(_AsyncCallback, _cm.ImageScalingSize.Width, 0, true);
			}

			////Solves ToolStrip problem in inactive window: first time need to click 2 times.
			//protected override void WndProc(ref Message m)
			//{
			//	base.WndProc(ref m);

			//	if(m.Msg == (int)Api.WM_MOUSEACTIVATE) {
			//		//Print(m.Result);
			//		m.Result = (IntPtr)1;
			//		//Was MA_ACTIVATEANDEAT, return MA_ACTIVATE. It does not activate the window if it has the no-activate style.
			//		//If it does not have the style, returning MA_NOACTIVATE causes anomalies, eg showing tooltip activates another window...
			//	}
			//}

			////Solves ToolStrip problem in inactive window: no tooltips.
			//protected override void OnMouseHover(EventArgs e)
			//{
			//	//PrintFunc();
			//	if(!_parent._w.IsActive && CanFocus && !Focused) {
			//		//Print("focus");
			//		Focus();
			//		//problem: toolbar starts slower if then mouse is in it
			//	}
			//	base.OnMouseHover(e);
			//}
			//TODO: try similar for CatMenu

			//Solves both ToolStrip problems in inactive window.
			//But creates new problem: slower toolbar startup if then mouse is in it. Actually the same with other methods, because draws hot state.
			//This also creates another problem: at startup not hot button (until mouse-move) if mouse was there.
			//protected override void OnMouseEnter(EventArgs e)
			//{
			//	//PrintFunc();
			//	if(!_parent._w.IsActive && CanFocus && !Focused) {
			//		//Print("focus");
			//		Print(_parent._showTime);
			//		Focus();
			//	}
			//	base.OnMouseEnter(e);
			//}

			//Solves both ToolStrip problems in inactive window.
			//But creates new problem: slower toolbar startup if then mouse is in it. Actually the same with other methods, because draws hot state.
			//This also creates another problem: at startup not hot button (until mouse-move) if mouse was there.
			protected override void OnMouseEnter(EventArgs e)
			{
				//PrintFunc();
				if(!_parent._w.IsActive && CanFocus && !Focused) {
					//Print("focus");
					long td = Time.Milliseconds - _parent._showTime - 500;
					if(td < 0) { /*DebugPrint("timer");*/ } //TODO: timer
					else Focus();
					//TODO: Time.SetTimer(interval, delegate void TimerHandler(Timer t))
				}
				base.OnMouseEnter(e);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				//var perf = new Perf.Inst(true);

				//ThreadPriority tp = 0;
				//if(!_paintedOnce) { //this could make the first paint faster if CPU is without hyperthreading
				//	tp = Thread.CurrentThread.Priority;
				//	Thread.CurrentThread.Priority = ThreadPriority.Highest;
				//}
				base.OnPaint(e);
				//if(!_paintedOnce) Thread.CurrentThread.Priority = tp;

				//perf.Next(); PrintList("------------------ paint", perf.Times);

				_paintedOnce = true;
			}

			//ToolStrip _ICatToolStrip.ToolStrip { get => this; }

			bool _paintedOnce;
			bool _ICatToolStrip.PaintedOnce { get => _paintedOnce; }
		}

	}
}
