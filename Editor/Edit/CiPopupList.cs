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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using Microsoft.CodeAnalysis.Completion;

class CiPopupList : IMessageFilter
{
	_Window _w;
	_FastListBox _list;
	AuToolStrip _tb;
	int _nKindButtons;
	int _height;
	CiCompletion _compl;

	List<CiComplItem> _a;
	List<int> _aVisible; //indices of visible _a items
	Func<CiComplItem, Task<CompletionDescription>> _itemDescription;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public Form PopupWindow => _w;

	/// <summary>
	/// The child list control.
	/// </summary>
	public FastListBox ListControl => _list;

	public CiPopupList(CiCompletion compl)
	{
		_compl = compl;

		_w = new _Window(this);
		_w.ClientSize = Au.Util.ADpi.ScaleSize((300, 360));

		_list = new _FastListBox(this);
		_list.AccessibleName = _list.Name = "Codein_list";
		_list.Dock = DockStyle.Fill;
		_list.ItemClick += _list_ItemClick;

		_tb = new AuToolStrip();
		_list.AccessibleName = _list.Name = "Codein_listFilter";
		_tb.GripStyle = ToolStripGripStyle.Hidden;
		_tb.Renderer = new AuDockPanel.DockedToolStripRenderer();
		//_tb.LayoutStyle = ToolStripLayoutStyle.StackWithOverflow; //default
		_tb.Dock = DockStyle.Left;
		_tb.ItemClicked += _tb_ItemClicked;

		var kindNames = CodeInfo.ItemKindNames;
		_nKindButtons = kindNames.Length;
		for(int i = 0; i < kindNames.Length; i++) _AddButton(kindNames[i], CodeInfo.GetImage((CiItemKind)i), null);
		_tb.Items.Add(new ToolStripSeparator());
		for(int i = 0; i < s_groupNames.Length; i++) _AddButton(s_groupNames[i], null, i switch { 0 => "N", 1 => "T", _ => "K" });

		void _AddButton(string text, Image image, string imageText)
		{
			ToolStripItem t;
			if(imageText != null) {
				t = _tb.Items.Add(imageText);
				t.TextAlign = ContentAlignment.MiddleLeft;
			} else {
				t = _tb.Items.Add(image);
				t.DisplayStyle = ToolStripItemDisplayStyle.Image;
			}
			var b = t as ToolStripButton;
			b.Name = b.AccessibleName = b.ToolTipText = text;
			b.Margin = new Padding(1, 1, 0, 0);
		}

		_w.Controls.Add(_list);
		_w.Controls.Add(_tb);
	}

	static string[] s_groupNames = new string[] { "Group by namespace", "Group by type", "Group by kind" };

	private void _tb_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		var b = e.ClickedItem as ToolStripButton;
		bool check = !b.Checked; b.Checked = check;

		int index = _tb.Items.IndexOf(e.ClickedItem);
		if(index< _nKindButtons) {
			int kindsChecked = 0, kindsVisible = 0;
			for(int i = 0, n = _nKindButtons; i < n; i++) {
				var v = _tb.Items[i];
				if(v.Visible) {
					kindsVisible |= 1 << i;
					if((v as ToolStripButton).Checked) kindsChecked |= 1 << i;
				}
			}
			if(kindsChecked == 0) kindsChecked = kindsVisible;
			foreach(var v in _a) {
				if(0 != (kindsChecked & (1 << (int)v.kind))) v.hidden &= ~CiItemHiddenBy.Kind; else v.hidden |= CiItemHiddenBy.Kind;
			}
			UpdateVisibleItems();
		}
	}

	private void _list_ItemClick(object sender, FastListBox.ItemClickArgs e)
	{
		//Print(e.Index, e.DoubleClick);
		//_list.Focus();
		if(e.DoubleClick) _OnItemDClicked(e.Index); else _OnItemSelected(e.Index);
	}

	int _VisibleCount => _aVisible.Count;

	CiComplItem _VisibleItem(int index) => _a[_aVisible[index]];

	async void _OnItemSelected(int index)
	{
		if((uint)index >= _VisibleCount) return;
		var k = _VisibleItem(index);
		//var p1 = APerf.Create();
		//Program.MainForm.BeginInvoke(new Action(() => p1.NW()));
		var s = await _itemDescription(k);
		Print(s.Text);
	}

	void _OnItemDClicked(int index)
	{
		if((uint)index < _VisibleCount) _compl.OnCompletionListItemDClicked(_VisibleItem(index));
		Hide();
	}

	public void SetListItems(List<CiComplItem> a, Func<CiComplItem, Task<CompletionDescription>> itemDescription)
	{
		if(Empty(a)) {
			Hide();
			return;
		}

		_itemDescription = itemDescription;
		_a = a;
		UpdateVisibleItems();
		for(int i = 0, n = _nKindButtons; i < n; i++) (_tb.Items[i] as ToolStripButton).Checked = false;
	}

	public void UpdateVisibleItems()
	{
		int n1 = 0; foreach(var v in _a) if(v.hidden == 0) n1++;
		_aVisible = new List<int>(n1);
		for(int i = 0; i < _a.Count; i++) if(_a[i].hidden == 0) _aVisible.Add(i);
		_Sort();

		_list.AddItems(_VisibleCount, i => _VisibleItem(i).DisplayText, _TextHorzOffset, _DrawItem);

		int kinds = 0;
		foreach(var v in _a) if((v.hidden & ~CiItemHiddenBy.Kind) == 0) kinds |= 1 << (int)v.kind;
		int nVisible = 0;
		_tb.SuspendLayout();
		for(int i = 0, n = _nKindButtons; i < n; i++) if(_tb.Items[i].Visible = 0 != (kinds & (1 << i))) nVisible++;
		//_tb.Visible = nVisible > 1;
		_tb.ResumeLayout();
	}

	public void Show(SciCode doc, int position)
	{
		position = doc.ST.CountBytesFromChars(position);
		int x = doc.Call(Sci.SCI_POINTXFROMPOSITION, 0, position), y = doc.Call(Sci.SCI_POINTYFROMPOSITION, 0, position);
		var r = new Rectangle(x - _tb.Width - _TextHorzOffset - 6, y - 2, 0, doc.Call(Sci.SCI_TEXTHEIGHT, doc.ST.LineIndexFromPos(position)) + 4);

		_SetRect(doc, r);
		_w.ShowAt(doc);
	}

	public void Hide()
	{
		_a = null;
		_aVisible = null;
		_list.AddItems(0, null, 0, null);
		_w.Hide();
	}

	void _Sort()
	{
		_aVisible.Sort((i1, i2) => {

			CiComplItem c1 = _a[i1], c2 = _a[i2];
			if(c1.inheritanceLevel > c2.inheritanceLevel) return 1;
			if(c1.inheritanceLevel < c2.inheritanceLevel) return -1;

			return string.Compare(c1.DisplayText, c2.DisplayText, StringComparison.OrdinalIgnoreCase);
		});
	}

	void _SetRect(Control control, RECT anchor)
	{
		var ra = control.RectangleToScreen(anchor);
		var rs = Screen.FromRectangle(ra).WorkingArea;
		rs.Inflate(-1, -5);
		int heiAbove = ra.Top - rs.Top, heiBelow = rs.Bottom - ra.Bottom;
		int maxHeight = Math.Max(heiAbove, heiBelow); if(maxHeight < 200) maxHeight = 200;

		var r = _w.Bounds;
		int width = r.Width, height = r.Height;
		if(height > maxHeight) { _height = height; height = maxHeight; } else if(_height > 0 && _height <= maxHeight) { height = _height; _height = 0; }
		r.Height = height;

		r.X = ra.Left + width <= rs.Right ? ra.Left : rs.Right - width; r.X = Math.Max(r.X, rs.Left);
		bool down = height <= heiBelow || heiAbove <= heiBelow;
		r.Y = down ? ra.Bottom : ra.Top - height;
		_w.Bounds = r;
	}

	void _DrawItem(FastListBox.ItemDrawArgs e)
	{
		var g = e.Graphics;
		var ci = _VisibleItem(e.Index);
		var r = e.Bounds;
		//Print(e.Index, r);
		bool isObsolete = false;
		var sym = ci.FirstSymbol;
		if(sym != null) {
			//Print(ci.DisplayText, .Length);
			foreach(var v in sym.GetAttributes()) { //fast
				switch(v.AttributeClass.Name) {
				case "ObsoleteAttribute": isObsolete = true; break;
				}
				//Print(ci.DisplayText, v.AttributeClass.Name);
			}
		}

		//var p1 = APerf.Create();
		var image = ci.Image;
		if(image != null) g.DrawImage(image, r.X + 4, (r.Y + r.Bottom + 1) / 2 - image.Height / 2); //note: would be very slow, but is very fast if DoubleBuffered = true (the control sets it in ctor).
																									//p1.Next();

		int xText = _TextHorzOffset; //TODO: DPI scale image
		r.Width -= xText; r.X += xText;
		if(e.IsSelected) g.FillRectangle(SystemBrushes.Control, r);

		var s = ci.DisplayText;
		ADebug.PrintIf(!Empty(ci.ci.DisplayTextPrefix), s); //we don't support prefix; never seen.
		var font = _list.Font;
		if(isObsolete) font = new Font(font, FontStyle.Strikeout);

		var a = ci.hilite;
		if(a != null) {
			for(int i = 0, x = 0; i < a.Length; i++) {
				int j = i == 0 ? 0 : a[i - 1];
				x += _Measure(j, a[i] - j);
				j = a[i++];
				var z2 = _Measure(j, a[i] - j);
				g.FillRectangle(Brushes.GreenYellow, r.X + x, r.Y, z2 + 1, r.Height);
				x += z2;
			}

			int _Measure(int from, int len)
			{
				if(len == 0) return 0;
				return TextRenderer.MeasureText(g, s.Substring(from, len), font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
			}
		}

		TextRenderer.DrawText(g, ci.DisplayText, font, r, ci.inheritanceLevel > 0 ? Color.Gray : Color.Black, TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
		//p1.NW();

		if(isObsolete) font.Dispose();
	}

	static int _TextHorzOffset => Au.Util.ADpi.ScaleInt(24);

	bool IMessageFilter.PreFilterMessage(ref Message m)
	{
		switch(m.Msg) {
		case Api.WM_KEYDOWN:
			var k = (Keys)m.WParam;
			switch(k) {
			case Keys.Escape:
				Hide();
				return true;
			case Keys.Enter:
			case Keys.Tab:
				_OnItemDClicked(_list.SelectedIndex);
				return true;
				//	case Keys.Down:
				//	case Keys.Up:
				//	case Keys.PageDown:
				//	case Keys.PageUp:
				//		//m.HWnd = _c.Handle; break; //does not work, even with Send(Api.WM_SETFOCUS)
				//		if(k != Keys.PageDown && _c.SelectedNode == null) _c.SelectedNode = _c.CurrentNode;
				//		else _c.OnKeyDown2(new KeyEventArgs(k));
				//		return true;
				//	}
				//	break;
				//case Api.WM_KEYUP:
				//	switch((Keys)m.WParam) {
				//	case Keys.Down: case Keys.Up: case Keys.PageDown: case Keys.PageUp: return true;
			}
			break;
			//case Api.WM_SYSKEYDOWN: _Close(); break;
			//case Api.WM_LBUTTONDOWN:
			//case Api.WM_NCLBUTTONDOWN:
			//case Api.WM_RBUTTONDOWN:
			//case Api.WM_NCRBUTTONDOWN:
			//case Api.WM_MBUTTONDOWN:
			//case Api.WM_NCMBUTTONDOWN:
			//	if(((AWnd)m.HWnd).Window != (AWnd)_w) _Close(); //SHOULDDO: support owned windows of _w
			//	break;
		}
		return false;
	}

	class _Window : AuForm
	{
		CiPopupList _p;
		Control _owner;
		bool _showedOnce;

		public _Window(CiPopupList p)
		{
			_p = p;

			this.SuspendLayout();
			this.AutoScaleMode = AutoScaleMode.None;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Text = "Au.CiPopupList";
			this.MinimumSize = new Size(150, 150);
			this.ResumeLayout();
		}

		protected override CreateParams CreateParams {
			get {
				var p = base.CreateParams;
				p.Style = unchecked((int)(WS.POPUP | WS.THICKFRAME));
				p.ExStyle = (int)(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE);
				return p;

				//note: if WS_CLIPCHILDREN, often at startup briefly black until control finished painting
			}
		}

		/// <summary>
		/// 1. Prevents activating window when showing. 2. Allows to show ToolTip for inactive window.
		/// </summary>
		protected override bool ShowWithoutActivation => true;

		public void ShowAt(Control anchor)
		{
			var owner = anchor.TopLevelControl;
			bool changedOwner = false;
			if(_showedOnce) {
				changedOwner = owner != _owner;
				if(Visible) {
					if(!changedOwner) return;
					Visible = false;
				}
			}
			_owner = owner;

			Show(_owner);
			if(changedOwner) ((AWnd)this).ZorderAbove((AWnd)_owner);
			_showedOnce = true;
		}

		protected override void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m);

			switch(m.Msg) {
			case Api.WM_DESTROY:
				_OnVisibleChanged(false);
				break;
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)(((int)m.LParam >> 16 == Api.WM_LBUTTONDOWN) ? Api.MA_NOACTIVATE : Api.MA_NOACTIVATEANDEAT);
				return;
			case Api.WM_ACTIVATEAPP:
				//if(m.WParam == default) _p._Close();
				break;
			}

			base.WndProc(ref m);

			switch(m.Msg) {
			case Api.WM_SHOWWINDOW:
				_OnVisibleChanged(m.WParam != default);
				break;
			}
		}

		void _OnVisibleChanged(bool visible)
		{
			if(visible == _isVisible) return;
			if(visible) Application.AddMessageFilter(_p);
			else Application.RemoveMessageFilter(_p);
			_isVisible = visible;

			if(!visible) {
				//_p.ClosedAction?.Invoke(_p);
			}
		}
		bool _isVisible;
	}

	class _FastListBox : FastListBox
	{
		CiPopupList _p;

		public _FastListBox(CiPopupList p)
		{
			_p = p;

			this.SetStyle(ControlStyles.Selectable //prevent focusing control and activating window on click
				, false);

		}

	}
}
