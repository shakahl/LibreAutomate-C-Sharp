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
using System.Linq;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	//[DebuggerStepThrough]
	public sealed partial class GDockPanels :Control
	{
		//readonly Control _parent;
		//List<GNode> _nodes;
		List<GSplit> _aSplit;
		List<GTab> _aTab;
		List<GPanel> _aPanel;
		_PainTools _paintTools;
		//internal readonly List<Control> Panels;
		//internal readonly DockOverlay Overlay;
		//internal readonly DocIndicator Indicator;
		GSplit _firstSplit;
		int _captionWidth;
		const int _splitterWidth = 4;

		//AddControl() adds these to a dictionary which is later passed to GPanel ctors.
		class _ControlToAdd
		{
			internal Control c;
			internal int ii; //image index
			internal Image img;
		}
		//Data to pass to GPanel etc ctors.
		class _InitData
		{
			internal GDockPanels manager;
			internal Dictionary<string, _ControlToAdd> controls = new Dictionary<string, _ControlToAdd>();
			internal ImageList imageList;
		}
		_InitData _initData = new _InitData();

		public void AddControl(Control control, int imageIndex)
		{
			_initData.controls.Add(control.Name, new _ControlToAdd() { c = control, ii = imageIndex });
		}

		public void AddControl(Control control, Image image = null)
		{
			_initData.controls.Add(control.Name, new _ControlToAdd() { c = control, img = image });
		}

		public void Create(string xmlFile, ImageList imageList, Control toolStripParentForDebug)
		{
			#region debug
			var pFiles = _initData.controls["Files"].c;
			var pOutput = _initData.controls["Output"].c;
			var pFind = _initData.controls["Find"].c;
			var pOpen = _initData.controls["Open"].c;
			var pRunning = _initData.controls["Running"].c;
			var pCode = _initData.controls["Code"].c;
			var debugTS = new ToolStrip();
			debugTS.Dock = DockStyle.None;
			debugTS.Location = new Point(600, 25);
			debugTS.Items.Add("Files", null, (unu, sed) => { _DebugHideShowPanel(pFiles); });
			debugTS.Items.Add("Output", null, (unu, sed) => { _DebugHideShowPanel(pOutput); });
			debugTS.Items.Add("Find", null, (unu, sed) => { _DebugHideShowPanel(pFind); });
			debugTS.Items.Add("Open", null, (unu, sed) => { _DebugHideShowPanel(pOpen); });
			debugTS.Items.Add("Running", null, (unu, sed) => { _DebugHideShowPanel(pRunning); });
			debugTS.Items.Add("Code", null, (unu, sed) => { _DebugHideShowPanel(pCode); });
			toolStripParentForDebug.Controls.Add(debugTS);
			#endregion

			//_nodes = new List<GNode>();
			_aSplit = new List<GSplit>();
			_aTab = new List<GTab>();
			_aPanel = new List<GPanel>();
			_paintTools = new _PainTools(this);

			_initData.manager = this;
			_initData.imageList = imageList;

			var xml = new XmlDocument();
			xml.Load(xmlFile);
			//XmlElement xDoc = xml.DocumentElement;
			XmlElement xFirstSplit = xml.SelectSingleNode("panels/split") as XmlElement;

			_firstSplit = new GSplit(_initData, xFirstSplit, null);

			//var g = this.CreateGraphics();
			//_captionWidth = this.Font.GetHeight(g);
			//g.Dispose();
			//Out(_captionWidth);
			_captionWidth = this.Font.Height + 4; //almost same as GetHeight() and GetHeight(g). //TODO: test with high DPI
			_captionWidth = Math.Max(_captionWidth, 18); //for icon

			SuspendLayout();
			//Out(GetStyle(ControlStyles.DoubleBuffer));
			this.SetStyle(ControlStyles.ContainerControl | ControlStyles.ResizeRedraw | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true); //default: UserPaint, AllPaintingInWmPaint; not OptimizedDoubleBuffer, DoubleBuffer, Opaque. Opaque prevents erasing background, which prevents flickering when moving a splitter.
			this.Dock = DockStyle.Fill;
			foreach(var c in _initData.controls.Values) this.Controls.Add(c.c);
			ResumeLayout();
			//Out("ctor ended");

			_initData.controls = null;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_paintTools?.Dispose();
		}

		protected override void WndProc(ref Message m)
		{
			if(_WndProcBefore_Common(this, ref m)) return;

			base.WndProc(ref m);

			//switch((uint)m.Msg) {
			////case Api.WM_:
			////	break;
			//}
		}

		//The X_Common functions are called by GDockPanels and by GFloat.
		//Would be better to use a common base class for them, but it is difficult etc because GDockPanels is a control and GFloat is a top-level form.
		bool _WndProcBefore_Common(Control parent, ref Message m)
		{
			switch((uint)m.Msg) {
			case Api.WM_SETCURSOR:
				if(m.WParam == parent.Handle && _OnSetCursor(parent, m.LParam)) {
					m.Result = (IntPtr)1;
					return true;
				}
				break;
			}
			return false;
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			//OutList("OnClientSizeChanged", IsHandleCreated, ClientRectangle);
			var r = this.ClientRectangle;
			if(!r.IsEmpty) _firstSplit.UpdateLayout(r); //else eg minimized

			base.OnClientSizeChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			_OnPaint_Common(e);
			_firstSplit.Paint(e.Graphics);

			base.OnPaint(e);

			//note: don't use e.ClipRectangle. Now it always is whole client area, because of ResizeRedraw=true (cannot work without it).
		}

		void _OnPaint_Common(PaintEventArgs e)
		{
			e.Graphics.Clear(Color.FromArgb(unchecked((int)0xFF8e9bbc))); //draw borders
		}

		#region mouse

		bool _OnSetCursor(Control parent, LPARAM lParam)
		{
			bool R = false, hilite = false;
			if((((uint)lParam) & 0xFFFF) == Api.HTCLIENT && !parent.Capture) {
				var p = ((Wnd)parent).MouseClientXY;
				_HitTestData ht;
				switch(_HitTest(parent, p.x, p.y, out ht)) {
				case _HitTestResult.Splitter:
					//info: works better than Cursor=x in OnMouseMove.
					var c = ht.gs.IsVerticalSplit ? Cursors.VSplit : Cursors.HSplit;
					_Api.SetCursor(c.Handle);
					R = true;
					break;
				case _HitTestResult.Caption:
					//OutList(ht.gt!=null, ht.gp?.Content.Name);
					var gt = ht.ParentTab;
					if(gt != null && ht.gp != gt.ActiveTab) {
						hilite = true;
						if(_hilitedTabButton != ht.gp) {
							_UnhiliteTabButton();
							_hilitedTabButton = ht.gp;
							_hilitedTabButton.InvalidateCaption();
							parent.MouseLeave += (unu, sed) => _UnhiliteTabButton();
						}
					}
					break;
				}
			}
			if(!hilite) _UnhiliteTabButton();
			return R;
		}

		GPanel _hilitedTabButton;

		void _UnhiliteTabButton()
		{
			if(_hilitedTabButton == null) return;
			_hilitedTabButton.InvalidateCaption();
			_hilitedTabButton = null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_OnMouseDown_Common(this, e);
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_OnMouseUp_Common(this, e);
			base.OnMouseUp(e);
		}

		static void _AssertParent(Control parent)
		{
			Debug.Assert(parent is GDockPanels || parent is GFloat);
		}

		void _OnMouseDown_Common(Control parent, MouseEventArgs e)
		{
			_AssertParent(parent);
			_HitTestData ht;
			switch(_HitTest(parent, e.X, e.Y, out ht)) {
			case _HitTestResult.Splitter:
				//OutList("splitter");
				if(e.Button == MouseButtons.Left) {
					parent.Cursor = ht.gs.IsVerticalSplit ? Cursors.VSplit : Cursors.HSplit;
					ht.gs.DragSplitter();
					parent.ResetCursor();
				}
				break;

			case _HitTestResult.Caption:
				//OutList("caption");
				ht.ParentTab?.OnMouseDownTabButton(ht.gp, e.Button);

				if(e.Button == MouseButtons.Left) {

				} else if(e.Button == MouseButtons.Right) {

				}
				break;
			}
		}

		void _OnMouseUp_Common(Control parent, MouseEventArgs e)
		{
			_AssertParent(parent);
			if(e.Button == MouseButtons.Right) {
				_HitTestData ht;
				if(_HitTest(parent, e.X, e.Y, out ht) == _HitTestResult.Caption) {
					var gc = ht.gt ?? ht.gp as GContentNode;
					gc.ShowContextMenu(e.Location);
				}
			}
		}

		enum _HitTestResult
		{
			None, //a content control or border or outside
			Splitter, //splitter
			Caption, //panel caption, tab caption or tab button
		};
		struct _HitTestData
		{
			internal GSplit gs; //not null when _HitTestResult.Splitter
			internal GTab gt; //not null when _HitTestResult.Caption and cursor is on tab caption and not button
			internal GPanel gp; //not null when _HitTestResult.Caption and cursor is on panel caption or tab button

			/// <summary>
			/// If hit test on a tabbed GPanel, returns its parent GTab, else null.
			/// </summary>
			internal GTab ParentTab { get { return gp?.ParentTab; } }

			/// <summary>
			/// If hit test on a GContentNode, returns it, else null.
			/// </summary>
			internal GContentNode ContentNode { get { return gt ?? gp as GContentNode; } }
		}

		/// <summary>
		/// If hit-test on a splitter, sets ht.gs and returns _HitTestResult.Splitter.
		/// Else if on a GPanel caption or GPanel tab button, sets gp and returns _HitTestResult.Caption.
		/// Else (if on a GTab caption but not a  GPanel tab button) sets gt and returns _HitTestResult.Caption.
		/// </summary>
		_HitTestResult _HitTest(Control parent, int x, int y, out _HitTestData ht)
		{
			_AssertParent(parent);
			ht = new _HitTestData();
			if(parent == this && (ht.gs = _aSplit.Find(v => v.HitTestSplitter(x, y))) != null) return _HitTestResult.Splitter;
			if((ht.gp = _aPanel.Find(v => v.HitTestCaption(parent, x, y))) != null) return _HitTestResult.Caption;
			if((ht.gt = _aTab.Find(v => v.HitTestCaption(parent, x, y))) != null) return _HitTestResult.Caption;
			return _HitTestResult.None;
		}

		#endregion mouse

		enum GDockState { Docked, Floating, Hidden, AutoHide };

		[DebuggerStepThrough]
		GPanel _FindControlPanel(Control control)
		{
			return _aPanel.Find(gp => gp.Content == control);
		}

		void _DebugHideShowPanel(Control c)
		{
			var gp = _FindControlPanel(c);
			if(gp.IsHidden) ShowPanel(c); else HidePanel(c);
		}

		public void HidePanel(Control control)
		{
			_FindControlPanel(control).SetDockState(GDockState.Hidden);
		}

		public void ShowPanel(Control control)
		{
			_FindControlPanel(control).SetDockState(GDockState.Docked);
		}

		public void Test()
		{

		}
	}



	public interface IPanelParent
	{

	}
}
