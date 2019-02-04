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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

#pragma warning disable 1591 //XML doc //TODO

namespace Au
{
	/// <summary>
	/// TODO
	/// </summary>
	public class AuToolbar : BaseMT
	{
		static AuToolbar()
		{
			Wnd.Misc.MyWindow.RegisterClass("AuToolbar");
		}

		Wnd.Misc.MyWindow _wClass;
		Wnd _w;
		ToolStrip_ _ts;

		/// <summary>
		/// Gets ToolStrip.
		/// </summary>
		public ToolStrip Ex => _ts;

		/// <summary>Infrastructure.</summary>
		protected override ToolStrip MainToolStrip => _ts;

		public AuToolbar()
		{
			_Init();
		}

		/// <inheritdoc cref="Add(string, Action{MTClickArgs}, object)"/>
		/// <example><code>
		/// var t = new AuToolbar();
		/// t["One"] = o => Print(o);
		/// t["Two", @"icon file path"] = o => { Print(o); AuDialog.Show(o.ToString()); };
		/// t.LastItem.ToolTipText = "tooltip";
		/// </code></example>
		public Action<MTClickArgs> this[string text, object icon = null] {
			set { Add(text, value, icon); }
		}

		/// <summary>
		/// Adds new button as <see cref="ToolStripButton"/>.
		/// Sets its text, icon and Click event handler. Other properties can be specified later. See example.
		/// Code <c>t.Add("text", o => Print(o));</c> is the same as <c>t["text"] = o => Print(o);</c> .
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onClick">Callback function. Called when the button clicked.</param>
		/// <param name="icon"><inheritdoc cref="AuMenu.Add(string, Action{MTClickArgs}, object)"/></param>
		/// <example><code>
		/// var m = new AuToolbar();
		/// t.Add("One", o => Print(o), @"icon file path");
		/// t.LastItem.ToolTipText = "tooltip";
		/// t.Add("Two", o => { Print(o.MenuItem.Checked); });
		/// </code></example>
		public ToolStripButton Add(string text, Action<MTClickArgs> onClick, object icon = null)
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
		/// <param name="icon"><inheritdoc cref="AuMenu.Add(string, Action{MTClickArgs}, object)"/></param>
		/// <param name="onClick">Callback function. Called when the item clicked. Not useful for most item types.</param>
		public void Add(ToolStripItem item, object icon = null, Action<MTClickArgs> onClick = null)
		{
			_Items.Add(item);
			_SetItemProp(true, item, onClick, icon);

			//Activate window when a child control clicked, or something may not work, eg cannot enter text in Edit control.
			if(item is ToolStripControlHost cb) //combo, edit, progress
				cb.GotFocus += _Item_GotFocus; //info: this is before MouseDown, which does not work well with combo box
		}

		void _Item_GotFocus(object sender, EventArgs e)
		{
			//Debug_.PrintFunc();
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

		ToolStripItemCollection _Items {
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
		public ToolStripButton LastButton => LastItem as ToolStripButton;

		void _Init()
		{
			Perf.Next();
			var exStyle = Native.WS_EX.TOOLWINDOW | Native.WS_EX.NOACTIVATE | Native.WS_EX.TOPMOST;
			var style = Native.WS.POPUP;
			//style |= Native.WS.CAPTION | Native.WS.SYSMENU;

			//RECT r = Screen.PrimaryScreen.WorkingArea; r.Inflate(-2, -2); r.top += 4;
			RECT r = (0, 0, 200, 200);

			_wClass = new Wnd.Misc.MyWindow(WndProc);
			if(!_wClass.Create("AuToolbar", null, style, exStyle, r.top, r.left, r.Width, r.Height)) throw new Win32Exception();
			Perf.Next();

			_ts = new ToolStrip_(this);
			_ts.SuspendLayout();

			_ts.SetBounds(r.top, r.left, r.Width, r.Height);

			//then caller will add buttons etc and call Visible=true, which calls _ts.ResumeLayout and _ts.CreateControl
		}

		public bool Visible {
			get => _w.IsVisible;
			set {
				_GetIconsAsync(_ts);
				if(!_ts.Created) {
					_ts.ResumeLayout();
					_ts.CreateControl();
					Perf.Next();
				}
				_w.ShowLL(value);
				_showTime = Time.PerfMilliseconds;
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
		public Wnd MainWnd => _w;


		protected virtual LPARAM WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
		{
			switch(message) {
			case Api.WM_NCCREATE:
				_w = w;
				break;
			}

			var R = _wClass.DefWndProc(w, message, wParam, lParam);

			switch(message) {
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


		class ToolStrip_ : ToolStrip, _IAuToolStrip
		{
			AuToolbar _parent;

			internal ToolStrip_(AuToolbar parent) { _parent = parent; }

			protected override CreateParams CreateParams {
				get {
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
			//	//Debug_.PrintFunc();
			//	if(!_parent._w.IsActive && CanFocus && !Focused) {
			//		//Print("focus");
			//		Focus();
			//		//problem: toolbar starts slower if then mouse is in it
			//	}
			//	base.OnMouseHover(e);
			//}
			//TODO: try similar for AuMenu

			//Solves both ToolStrip problems in inactive window.
			//But creates new problem: slower toolbar startup if then mouse is in it. Actually the same with other methods, because draws hot state.
			//This also creates another problem: at startup not hot button (until mouse-move) if mouse was there.
			//protected override void OnMouseEnter(EventArgs e)
			//{
			//	//Debug_.PrintFunc();
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
				//Debug_.PrintFunc();
				if(!_parent._w.IsActive && CanFocus && !Focused) {
					//Print("focus");
					long td = Time.PerfMilliseconds - _parent._showTime - 500;
					if(td < 0) { /*Debug_.Print("timer");*/ } //TODO: timer
					else Focus();
					//TODO: timer
				}
				base.OnMouseEnter(e);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				//var perf = Perf.StartNew();

				//ThreadPriority tp = 0;
				//if(!_paintedOnce) { //this could make the first paint faster if CPU is without hyperthreading
				//	tp = Thread.CurrentThread.Priority;
				//	Thread.CurrentThread.Priority = ThreadPriority.Highest;
				//}
				base.OnPaint(e);
				//if(!_paintedOnce) Thread.CurrentThread.Priority = tp;

				//perf.Next(); Print("------------------ paint", perf.ToString());

				_paintedOnce = true;
			}

			//ToolStrip _IAuToolStrip.ToolStrip => this;

			bool _paintedOnce;
			bool _IAuToolStrip.PaintedOnce => _paintedOnce;
		}

	}
}
