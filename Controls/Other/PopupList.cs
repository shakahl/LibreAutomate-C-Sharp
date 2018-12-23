using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Drawing;
using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
//using Au.Controls;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections;

namespace Au.Controls
{
	public class PopupList : IMessageFilter
	{
		_Window _w;
		_Control _c;

		/// <summary>
		/// List items.
		/// </summary>
		/// <remarks>
		/// Must be set before calling <b>Show</b>.
		/// Item type can be string or other type. Its <b>ToString</b> is called to get display text. Items can be of different types.
		/// Item type can implement <see cref="IPopupListItem"/> interface if need checkboxes, colors, etc.
		/// </remarks>
		public object[] Items { get; set; }

		/// <summary>
		/// The top-level popup window.
		/// </summary>
		public Form PopupWindow {
			get {
				if(_w == null || _w.IsDisposed) {
					_w = new _Window(this);
					_c = new _Control(this);
					_w.Controls.Add(_c);
					_c.NodeMouseClick += _c_NodeMouseClick;
				}
				return _w;
			}
		}

		/// <summary>
		/// If false (default), destroys/disposes the popup window when it is hidden. Then next <b>Show</b> would create new window.
		/// If true, does not destroy. Then next <b>Show</b> reuses the same window. To destroy the window: <c>x.PopupWindow.Close();</c>.
		/// </summary>
		public bool MultiShow { get; set; }

		/// <summary>
		/// When the popup window hidden.
		/// If an item selected, the <b>ResultX</b> properties contain selection info, else null/-1/false.
		/// </summary>
		public event Action<PopupList> Closed;

		/// <summary>
		/// When an item selected.
		/// The event handler can use the <b>ResultX</b> properties to get selection info.
		/// The popup window is already hidden.
		/// </summary>
		public event Action<PopupList> Selected;

		/// <summary>
		/// Gets the selected item.
		/// Call this from <see cref="Selected"/> event handler.
		/// </summary>
		public object ResultItem { get; private set; }

		/// <summary>
		/// Gets the selected index.
		/// Call this from <see cref="Selected"/> event handler.
		/// </summary>
		public int ResultIndex {
			get {
				if(ResultItem != null && Items != null) {
					var a = Items; var r = ResultItem;
					for(int i = 0; i < a.Length; i++) if(a[i] == r) return i;
				}
				return -1;
			}
		}

		/// <summary>
		/// True if the selection was made with the keyboard (Enter), false if clicked.
		/// Call this from <b>OnSelected</b> callback function.
		/// </summary>
		public bool ResultWasKey { get; private set; }

		/// <summary>
		/// Don't make the popup list wider than anchor.
		/// If false, makes wider if strings are long.
		/// </summary>
		public bool FixedWidth { get; set; }

		/// <summary>
		/// Show the popup window with roll animation, like combo box.
		/// Uses default combo box animation time. Ignored if !<see cref="SystemInformation.IsComboBoxAnimationEnabled"/>.
		/// </summary>
		public bool ComboBoxAnimation { get; set; }
		//note: also tried menu animation as an option, but then does not draw control border.

		/// <summary>
		/// Don't hide the popup window when a window of another program is activated.
		/// </summary>
		public bool DoNotCloseWhenAppDeactivated { get; set; }

		/// <summary>
		/// Shows the popup list below or above the anchor control.
		/// </summary>
		/// <param name="anchor">Control. Its top-level parent window will own the popup window.</param>
		/// <exception cref="ArgumentException">anchor is null or its handle is not created.</exception>
		/// <exception cref="InvalidOperationException">Exceptions of <see cref="Form.Show(IWin32Window)"/>.</exception>
		public void Show(Control anchor)
		{
			if(!(anchor?.IsHandleCreated ?? false)) throw new ArgumentException();
			_Show(anchor, ((Wnd)anchor).Rect);
		}

		/// <summary>
		/// Shows the popup list below or above the anchor rectangle relative to control.
		/// </summary>
		/// <param name="control">Control or form. The top-level window will own the popup window.</param>
		/// <param name="anchor">Rectangle in control's client area.</param>
		/// <exception cref="ArgumentException">control is null or its handle is not created.</exception>
		/// <exception cref="InvalidOperationException">Exceptions of <see cref="Form.Show(IWin32Window)"/>.</exception>
		public void Show(Control control, Rectangle anchor)
		{
			if(!(control?.IsHandleCreated ?? false)) throw new ArgumentException();
			_Show(control, control.RectangleToScreen(anchor));
		}

		/// <summary>
		/// Shows the popup list below or above the anchor rectangle.
		/// </summary>
		/// <param name="anchor">Rectangle in screen.</param>
		/// <remarks>
		/// The popup window is top-most.
		/// </remarks>
		public void Show(Rectangle anchor)
		{
			_Show(null, anchor);
		}

		void _Show(Control anchor, Rectangle ra)
		{
			ResultItem = null;
			ResultWasKey = false;

			int n = Items?.Length ?? 0;
			if(n == 0) {
				Closed?.Invoke(this);
				return;
			}

			_ = PopupWindow; //auto-creates _w and _c objects if null or disposed

			if(MultiShow && _c.IsHandleCreated) {
				if(_c.Root.TryGetFirstChild(out var n0)) _c.SelectedNode = n0; //make CurrentNode=the first node and reset vertical scrollbar. Never mind: does not reset horizontal scrollbar.
				_c.ClearSelection();
			}

			//never mind: could skip most of this code if items not changed. Then need to clone Items etc. Too difficult if IPopupListItem.

			bool hasCheckboxes = false, hasIcons = false;
			foreach(var v in Items) {
				if(v is IPopupListItem e) {
					if(!hasCheckboxes && e.CheckType != default) hasCheckboxes = true;
					if(!hasIcons && e.Icon != null) hasIcons = true;
					if(hasCheckboxes && hasIcons) break;
				}
			}

			var screen = Screen.FromRectangle(ra);
			var rs = screen.WorkingArea; if(ra.Width > 0 && (ra.Right <= rs.Left || ra.Left >= rs.Right)) rs = screen.Bounds;
			rs.Inflate(-1, -5);
			int heiSB = SystemInformation.HorizontalScrollBarHeight;
			int heiAbove = ra.Top - rs.Top - heiSB, heiBelow = rs.Bottom - ra.Bottom - heiSB;
			int maxHeight = Math.Max(heiAbove, heiBelow), maxWidth = FixedWidth ? ra.Width : rs.Width - 100;
			if(maxHeight < 200) maxHeight = 200;
			bool hasVertSB = false;

			int width = 0, height = 2;
			Font fontNormal = _w.Font;
			foreach(var v in Items) {
				var e = v as IPopupListItem;
				var font = (e?.BoldFont ?? false) ? _w.FontBold : fontNormal;
				var z = TextRenderer.MeasureText(v.ToString(), font);
				z.Width += _c._ccText.LeftMargin;
				z.Height += 2; if(z.Height < 17) z.Height = 17;
				if(e != null) {
					if(e.CheckType != default) z.Width += NodeCheckBox.ImageSize + _c._ccCheck.LeftMargin;
					var im = e.Icon;
					if(im != null) {
						var iz = Util.Dpi.ImageSize(im);
						z.Width += iz.width + _c._ccIcon.LeftMargin;
						if(z.Height < iz.height) z.Height = iz.height;
					}
				}
				//Print(z);
				if(height + z.Height <= maxHeight) height += z.Height; else hasVertSB = true;
				if(z.Width > width) width = z.Width;
			}
			width += 5;
			if(hasVertSB) width += SystemInformation.VerticalScrollBarWidth;
			if(width < ra.Width) width = ra.Width; else if(width > maxWidth) { width = maxWidth; height += heiSB; }

			var r = new Rectangle(0, 0, width, height);
			_c.Bounds = r;

			r.X = ra.Left + width <= rs.Right ? ra.Left : rs.Right - width; r.X = Math.Max(r.X, rs.Left);
			bool down = height <= heiBelow || heiAbove <= heiBelow;
			r.Y = down ? ra.Bottom : ra.Top - height;
			_w.Bounds = r;

			_c.Model = null;
			_c.Model = _c;

			_w.ShowPopup(anchor, !down);
		}

		bool IMessageFilter.PreFilterMessage(ref Message m)
		{
			//Wnd.Misc.PrintMsg(m, 0x118, Api.WM_TIMER, Api.WM_PAINT, Api.WM_MOUSEMOVE, Api.WM_NCMOUSEMOVE, 0x60, 0xc341, 0x2a2);

			switch(m.Msg) {
			case Api.WM_KEYDOWN:
				var k = (Keys)m.WParam;
				switch(k) {
				case Keys.Escape:
					_Close();
					return true;
				case Keys.Enter:
				case Keys.Tab:
				case Keys.Space:
					return _KeyEnterTabSpace(k);
				case Keys.Down:
				case Keys.Up:
				case Keys.PageDown:
				case Keys.PageUp:
					//m.HWnd = _c.Handle; break; //does not work, even with Send(Api.WM_SETFOCUS)
					if(k != Keys.PageDown && _c.SelectedNode == null) _c.SelectedNode = _c.CurrentNode;
					else _c.OnKeyDown2(new KeyEventArgs(k));
					return true;
				}
				break;
			case Api.WM_KEYUP:
				switch((Keys)m.WParam) {
				case Keys.Down: case Keys.Up: case Keys.PageDown: case Keys.PageUp: return true;
				}
				break;
			case Api.WM_SYSKEYDOWN: _Close(); break;
			case Api.WM_LBUTTONDOWN:
			case Api.WM_NCLBUTTONDOWN:
			case Api.WM_RBUTTONDOWN:
			case Api.WM_NCRBUTTONDOWN:
			case Api.WM_MBUTTONDOWN:
			case Api.WM_NCMBUTTONDOWN:
				if(((Wnd)m.HWnd).Window != (Wnd)_w) _Close(); //TODO: support owned windows of _w
				break;
			}
			return false;

			//tested: somehow wheel scrolling is automatically correct.
		}

		void _Close()
		{
			_w.Hide();
		}

		void _Selected(TreeNodeAdv node, bool isKey)
		{
			ResultItem = node.Tag;
			ResultWasKey = isKey;
			_Close();
			Selected?.Invoke(this);
		}

		//returns true to steal the key
		bool _KeyEnterTabSpace(Keys key)
		{
			bool isEnter = key == Keys.Enter, isSpace = key == Keys.Space;
			var node = _c.SelectedNode;
			if(node == null) {
				if(isEnter) return true;
				if(!isSpace) _Close();
				return false;
			}
			var o = node.Tag;
			if(o is IPopupListItem x) {
				if(x.Disabled) return !isSpace;
				if(x.CheckType != default) {
					if(isSpace) {
						if(Keyb.IsCtrl || Keyb.IsShift) return false;
						_ToggleCheckbox(node, x);
						return true;
					}
					if(x.CheckType == PLCheckType.Row) return !isSpace;
				}
			}
			if(isSpace) return false;
			_Selected(node, true);
			return isEnter;
		}

		private void _c_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
		{
			var node = e.Node;
			if(node.Tag is IPopupListItem x) {
				if(x.Disabled) return;
				switch(x.CheckType) {
				case PLCheckType.Row:
				case PLCheckType.Box when e.Control is NodeCheckBox:
					_ToggleCheckbox(node, x);
					return;
				}
			}
			_Selected(node, false);
		}

		void _ToggleCheckbox(TreeNodeAdv node, IPopupListItem x)
		{
			int group = x.Group;
			bool check = !x.Checked;
			if(!check && group < 0) return;
			x.Checked = check;
			_c.UpdateNode(node);
			if(group != 0 && check && Items != null) {
				foreach(var v in Items) {
					if(v != x && v is IPopupListItem y && y.Group == group && y.Checked) {
						y.Checked = false;
						var n2 = _c.FindNodeByTag(v);
						if(n2 != null) _c.UpdateNode(n2);
					}
				}
			}
		}

		class _Window : Form
		{
			PopupList _p;
			Control _owner;
			Font _font, _fontBold;
			bool _showedOnce;
			bool _up;

			public Font FontBold => _fontBold ?? (_fontBold = new Font(Font, FontStyle.Bold)); //clone not _font, because caller may change Font

			public _Window(PopupList p)
			{
				_p = p;
				_font = Util.SystemFonts_.Regular;

				this.SuspendLayout();
				this.AutoScaleMode = AutoScaleMode.None;
				this.Font = _font;
				this.StartPosition = FormStartPosition.Manual;
				this.FormBorderStyle = FormBorderStyle.None;
				this.Text = "Au.PopupList";

				//SetStyle(ControlStyles.Opaque, true);

				this.ResumeLayout();
			}

			protected override void Dispose(bool disposing)
			{
				if(disposing) {
					_font.Dispose();
					_fontBold?.Dispose();
				}
				base.Dispose(disposing);
			}

			public void ShowPopup(Control anchor, bool up)
			{
				var owner = anchor?.TopLevelControl;
				bool changedOwner = false;
				if(_showedOnce) {
					changedOwner = owner != _owner;
					if(Visible) {
						if(!changedOwner) return;
						Visible = false;
					}
				}
				_owner = owner;
				_up = up;

				if(_owner != null) {
					Show(_owner);
					if(changedOwner) ((Wnd)this).ZorderAbove((Wnd)_owner);
				} else {
					Show(); //note: not the same as Show(null)
					if(changedOwner) ((Wnd)this).ZorderTopmost();
				}
				_showedOnce = true;
			}

			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					p.Style = unchecked((int)(Native.WS.POPUP));
					var es = Native.WS_EX.TOOLWINDOW | Native.WS_EX.NOACTIVATE;
					if(_p != null && _owner == null) es |= Native.WS_EX.TOPMOST;
					p.ExStyle = (int)es;
					p.ClassStyle |= (int)Api.CS_DROPSHADOW;
					return p;

					//note: if WS_CLIPCHILDREN, often at startup briefly black until control finished painting
				}
			}

			/// <summary>
			/// 1. Prevents activating window when showing. 2. Allows to show ToolTip for inactive window.
			/// </summary>
			protected override bool ShowWithoutActivation => true;

			protected override void WndProc(ref Message m)
			{
				//Wnd.Misc.PrintMsg(m);

				switch(m.Msg) {
				case Api.WM_DESTROY:
					_OnVisibleChanged(false, true);
					break;
				case Api.WM_MOUSEACTIVATE:
					m.Result = (IntPtr)(((int)m.LParam >> 16 == Api.WM_LBUTTONDOWN) ? Api.MA_NOACTIVATE : Api.MA_NOACTIVATEANDEAT);
					return;
				case Api.WM_ACTIVATEAPP:
					if(m.WParam == default && !_p.DoNotCloseWhenAppDeactivated) _p._Close();
					break;
				}

				base.WndProc(ref m);

				switch(m.Msg) {
				case Api.WM_SHOWWINDOW:
					_OnVisibleChanged(m.WParam != default, false);
					break;
				}
			}

			void _OnVisibleChanged(bool visible, bool onDestroy)
			{
				if(visible == _isVisible) return;
				if(visible) Application.AddMessageFilter(_p);
				else Application.RemoveMessageFilter(_p);
				_isVisible = visible;

				if(!visible) {
					_p.Closed?.Invoke(_p);
					if(!onDestroy && !_p.MultiShow) _p._w.Close();
				} else {
					//show with standard combobox animation
					if(_p.ComboBoxAnimation && SystemInformation.IsComboBoxAnimationEnabled) {
						Api.SetCursor(Cursors.Arrow.Handle);
						Api.AnimateWindow((Wnd)this, 0, Api.AnimationFlags.Slide | (_up ? Api.AnimationFlags.VerticalNegative : Api.AnimationFlags.VerticalPositive));
					}
				}
			}
			bool _isVisible;
		}

		class _Control : TreeViewAdv, ITreeModel
		{
			PopupList _p;
			internal NodeCheckBox _ccCheck;
			internal NodeIcon _ccIcon;
			internal NodeTextBox _ccText;
			ToolTip _tooltip;

			public _Control(PopupList p)
			{
				_p = p;

				this.BorderStyle = BorderStyle.FixedSingle;
				this.SetStyle(ControlStyles.Selectable //prevent focusing control and activating window on click
					, false);

				this.ShowNodeToolTips = true;
				this.FullRowSelect = true;
				this.ShowPlusMinus = false; this.ShowLines = false;
				this.GridLineStyle = GridLineStyle.Horizontal;
				//this.AutoRowHeight=false;

				_ccCheck = new NodeCheckBox();
				_ccCheck.LeftMargin = 4;
				_ccCheck.IsVisibleValueNeeded = o => o.Tag is IPopupListItem e && e.CheckType != default;
				_ccCheck.ValueNeeded = o => o.Tag is IPopupListItem e && e.Checked ? CheckState.Checked : default;
				NodeControls.Add(_ccCheck);

				_ccIcon = new NodeIcon();
				_ccIcon.LeftMargin = 4;
				_ccIcon.ScaleMode = ImageScaleMode.ScaleUp;
				_ccIcon.DpiStretch = true;
				_ccIcon.IsVisibleValueNeeded = o => o.Tag is IPopupListItem e && e.Icon != null;
				_ccIcon.ValueNeeded = o => o.Tag is IPopupListItem e ? e.Icon : null;
				NodeControls.Add(_ccIcon);

				_ccText = new NodeTextBox();
				//_ccText.LeftMargin = 0; //default 1
				_ccText.Trimming = StringTrimming.EllipsisCharacter;
				_ccText.ValueNeeded = node => node.Tag;
				_ccText.FontNeeded = node => (node.Tag is IPopupListItem x && x.BoldFont) ? _p._w.FontBold : null;
				_ccText.DrawText += _ccText_DrawText;
				_ccText.NeedDrawTextEvent = node => node.Tag is IPopupListItem x && (x.TextColor != default || (x.Disabled && x.BackColor == default));
				NodeControls.Add(_ccText);

#if false //test column header and multiple columns
				UseColumns = true;
				var _columnName = new TreeColumn("Name", 50);
				this.Columns.Add(_columnName);
				_ccText.ParentColumn = _columnName;

				//var _cc2 = new NodeTextBox();
				//_cc2.ValueNeeded = node => "2";
				//NodeControls.Add(_cc2);
				//var _col2 = new TreeColumn("Two", 50);
				//this.Columns.Add(_col2);
				//_cc2.ParentColumn = _col2;
#endif
			}

			#region drawing

			protected override void OnRowDraw(PaintEventArgs e, TreeNodeAdv node, ref DrawContext context, int row, Rectangle rowRect)
			{
				//if(context.DrawSelection == DrawSelectionMode.Inactive) context.DrawSelection = DrawSelectionMode.Active;

				if(node.Tag is IPopupListItem x) {
					if(x.BackColor != default) {
						var r = new Rectangle(OffsetX, rowRect.Y, ClientSize.Width, rowRect.Height);
						using(var brush = new SolidBrush((Color)x.BackColor)) e.Graphics.FillRectangle(brush, r);
					}
				}

				base.OnRowDraw(e, node, ref context, row, rowRect);
			}

			private void _ccText_DrawText(object sender, DrawEventArgs e)
			{
				var node = e.Node;
				if(node.Tag is IPopupListItem x) {
					if(x.TextColor != default && e.Context.DrawSelection == DrawSelectionMode.None) e.TextColor = (Color)x.TextColor;
					else if(x.Disabled && x.BackColor == default) e.TextColor = SystemColors.GrayText;
				}
			}

			#endregion

			#region mouse move-selection and tooltip

			protected override void OnMouseMove(MouseEventArgs e)
			{
				var n = this.GetNodeAt(e.Location);
				bool nodeChanged = n != _mouseNode; _mouseNode = n;
				string tooltipText = null;
				if(n != null) {
					if(n.Tag is IPopupListItem x) {
						tooltipText = x.TooltipText;
						if(x.Disabled) n = null;
					}
					SelectedNode = n;
				}
				if(nodeChanged) _SetTooltip(tooltipText);
				base.OnMouseMove(e);
			}
			TreeNodeAdv _mouseNode;

			void _SetTooltip(string text)
			{
				if(!Empty(text)) {
					if(_tooltip == null) _tooltip = new ToolTip { IsBalloon = true, AutoPopDelay = 30000, ShowAlways = true };
					_tooltip.Active = false;
					_tooltip.SetToolTip(this, text);
					_tooltip.Active = true;
				} else
					_tooltip?.SetToolTip(this, null);

				//info: it seems the tooltip window is automatically destroyed when this control destroyed.
			}

			protected override void OnMouseLeave(EventArgs e)
			{
				_mouseNode = null;
				base.OnMouseLeave(e);
			}

			#endregion

			#region ITreeModel

			public IEnumerable GetChildren(object nodeTag)
			{
				return _p.Items;
			}

			public bool IsLeaf(object nodeTag)
			{
				return true;
			}

#pragma warning disable 67
			public event EventHandler<TreeModelEventArgs> NodesChanged;
			public event EventHandler<TreeModelEventArgs> NodesInserted;
			public event EventHandler<TreeModelEventArgs> NodesRemoved;
			public event EventHandler<TreePathEventArgs> StructureChanged;
#pragma warning restore 67

			#endregion
		}
	}

	/// <summary>
	/// Can be used for <see cref="PopupList.Items"/> when need row colors, icon, checkbox, tooltip, etc.
	/// </summary>
	/// <remarks>
	/// Most of properties in this interface have only 'get' function, but your class that implement it also can add 'set' functions. Example: <c>public bool Disabled { get; set; }</c>. Only the <b>Checked</b> property has 'set' function, and the popup list calls it when the user changes checkbox state.
	/// </remarks>
	public interface IPopupListItem
	{
		/// <summary>
		/// Balloon tooltip text.
		/// </summary>
		string TooltipText { get; }

		/// <summary>
		/// Row icon.
		/// </summary>
		Image Icon { get; }

		/// <summary>
		/// Row background color.
		/// </summary>
		ColorInt BackColor { get; }

		/// <summary>
		/// Text color.
		/// </summary>
		ColorInt TextColor { get; }

		/// <summary>
		/// Adds checkbox.
		/// If <b>Row</b>, the row is used to toggle checkbox and not to select the item.
		/// If <b>Box</b>, the row is used to toggle checkbox or select the item, depending where you click.
		/// In any case the checkbox also can be toggled with the Space key.
		/// </summary>
		PLCheckType CheckType { get; }

		/// <summary>
		/// The checkbox is checked.
		/// Changed when the user changes checkbox state.
		/// </summary>
		bool Checked { get; set; }

		/// <summary>
		/// The user cannot select, check or uncheck the row.
		/// Also its text is gray, unless is used <see cref="TextColor"/> or <see cref="BackColor"/>.
		/// </summary>
		bool Disabled { get; }

		/// <summary>
		/// Draw item text using bold font.
		/// </summary>
		bool BoldFont { get; }

		/// <summary>
		/// Checkbox group id.
		/// If not 0, when the user makes the checkbox checked, other items that have the same group id are automatically unchecked.
		/// If negative, the user cannot uncheck the checked item.
		/// </summary>
		short Group { get; }
	}

	/// <summary>
	/// <see cref="IPopupListItem.CheckType"/>
	/// </summary>
	public enum PLCheckType : byte
	{
		/// <summary>
		/// No checkbox.
		/// </summary>
		None,

		/// <summary>
		/// The row is used to toggle checkbox and not to select the item.
		/// </summary>
		Row,

		/// <summary>
		/// The row is used to toggle checkbox or select the item, depending where you click.
		/// </summary>
		Box,
	}
}
