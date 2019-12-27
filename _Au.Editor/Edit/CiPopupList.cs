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

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

class CiPopupList
{
	_Window _w;
	_FastListBox _list;
	Au.Util.AToolStrip _tb;
	int _nKindButtons;
	int _height;

	SciCode _doc;
	CiCompletion _compl;
	List<CiComplItem> _a;
	List<int> _aVisible; //indices of visible _a items
	List<string> _groups;
	ToolStripButton _groupButton;
	bool _groupsEnabled;
	Bitmap _imgStatic, _imgAbstract;
	CiPopupHtml _popupHtml;
	ATimer _popupTimer;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public InactiveWindow PopupWindow => _w;

	///// <summary>
	///// The child list control.
	///// </summary>
	//public AuListControl ListControl => _list;

	public CiPopupList(CiCompletion compl)
	{
		_compl = compl;

		_w = new _Window(this);
		_w.SuspendLayout();
		_w.Size = Au.Util.ADpi.ScaleSize((300, 360));

		_list = new _FastListBox();
		_list.SuspendLayout();
		_list.AccessibleName = _list.Name = "Codein_list";
		_list.Dock = DockStyle.Fill;
		_list.BackColor = Color.White;
		_list.ZItemClick += _list_ItemClick;
		_list.ZSelectedIndexChanged += _list_SelectedIndexChanged;

		_tb = new Au.Util.AToolStrip();
		_tb.SuspendLayout();
		_tb.AccessibleName = _tb.Name = "Codein_listFilter";
		_tb.Renderer = new AuDockPanel.ZDockedToolStripRenderer();
		_tb.GripStyle = ToolStripGripStyle.Hidden;
		_tb.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
		int tbWidth = 0;

		var kindNames = CiUtil.ItemKindNames;
		_nKindButtons = kindNames.Length;
		for(int i = 0; i < kindNames.Length; i++) _AddButton(kindNames[i], CiUtil.GetKindImage((CiItemKind)i));
		_tb.Items.Add(new ToolStripSeparator());
		_groupButton = _AddButton("Group by namespace or inheritance", EdResources.GetImageNoCacheDpi(nameof(Au.Editor.Resources.Resources.ciGroupBy)));
		if(Program.Settings.ci_complGroup) _groupButton.Checked = true;

		ToolStripButton _AddButton(string text, Image image)
		{
			var b = _tb.Items.Add(image) as ToolStripButton;
			b.DisplayStyle = ToolStripItemDisplayStyle.Image;
			b.ImageScaling = ToolStripItemImageScaling.None;
			b.Name = b.AccessibleName = b.ToolTipText = text;
			b.Margin = new Padding(1, 1, 0, 0);
			if(tbWidth == 0) tbWidth = 4 + b.ContentRectangle.Left * 2 + image.Width;
			return b;
		}

		_tb.Width = tbWidth; _tb.AutoSize = false; //does not work well with autosize, eg width too small when high DPI
		_tb.Dock = DockStyle.Left;
		_tb.ItemClicked += _tb_ItemClicked;

		_w.Controls.Add(_list);
		_w.Controls.Add(_tb);
		_tb.ResumeLayout();
		_list.ResumeLayout();
		_w.ResumeLayout(true);

		_imgStatic = EdResources.GetImageNoCacheDpi(nameof(Au.Editor.Resources.Resources.ciOverlayStatic));
		_imgAbstract = EdResources.GetImageNoCacheDpi(nameof(Au.Editor.Resources.Resources.ciOverlayAbstract));

		_popupHtml = new CiPopupHtml(CiPopupHtml.UsedBy.PopupList);
		_popupTimer = new ATimer(_ShowPopupHtml);
	}

	private void _tb_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		var b = e.ClickedItem as ToolStripButton;
		bool check = !b.Checked; b.Checked = check;

		int index = _tb.Items.IndexOf(e.ClickedItem);
		if(index < _nKindButtons) {
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
		} else if(e.ClickedItem == _groupButton) {
			_groupsEnabled = check && _groups != null;
			_Sort();
			this.SelectedItem = null;
			_list.ZReMeasure();
		}
	}

	private void _list_ItemClick(object sender, AuListControl.ZItemClickArgs e)
	{
		if(e.DoubleClick) {
			_compl.Commit(_doc, _VisibleItem(e.Index));
			Hide();
		}
	}

	private void _list_SelectedIndexChanged(int index)
	{
		if((uint)index < _VisibleCount) {
			_popupTimer.After(300, _VisibleItem(index));
			_popupHtml.Html = null;
		} else {
			_popupHtml.Hide();
			_popupTimer.Stop();
		}
	}

	void _ShowPopupHtml(ATimer t)
	{
		var ci = t.Tag as CiComplItem;
		var html = _compl.GetDescriptionHtml(ci, 0);
		if(html == null) return;
		_popupHtml.Html = html;
		_popupHtml.OnLinkClick = (ph, e) => ph.Html = _compl.GetDescriptionHtml(ci, e.Link.ToInt(1));
		_popupHtml.Show(Panels.Editor.ZActiveDoc, _w.Bounds);
	}

	int _VisibleCount => _aVisible.Count;

	CiComplItem _VisibleItem(int index) => _a[_aVisible[index]];

	public void SetListItems(List<CiComplItem> a, List<string> groups)
	{
		if(Empty(a)) {
			Hide();
			return;
		}

		_a = a;
		_groups = groups;
		_groupsEnabled = _groups != null && _groupButton.Checked;

		for(int i = 0, n = _nKindButtons; i < n; i++) (_tb.Items[i] as ToolStripButton).Checked = false;
		_groupButton.Visible = _groups != null;
		UpdateVisibleItems();
	}

	public void UpdateVisibleItems()
	{
		int n1 = 0; foreach(var v in _a) if(v.hidden == 0) n1++;
		_aVisible = new List<int>(n1);
		for(int i = 0; i < _a.Count; i++) if(_a[i].hidden == 0) _aVisible.Add(i);
		_Sort();

		_list.ZAddItems(_VisibleCount,
			o => _TextHorzOffset + o.MeasureText(_VisibleItem(o.index).DisplayText).width + o.MeasureText(_GreenSuffix(o.index)).width,
			_DrawItem,
			i => _VisibleItem(i).DisplayText);
		_compl.SelectBestMatch(_aVisible.Select(i => _a[i].ci)); //pass items sorted like in the visible list
		_list.Invalidate();

		int kinds = 0;
		foreach(var v in _a) if((v.hidden & ~CiItemHiddenBy.Kind) == 0) kinds |= 1 << (int)v.kind;
		_tb.SuspendLayout();
		for(int i = 0, n = _nKindButtons; i < n; i++) _tb.Items[i].Visible = 0 != (kinds & (1 << i));
		_tb.ResumeLayout();
	}

	public void Show(SciCode doc, int position)
	{
		_doc = doc;

		var r = CiUtil.GetCaretRectFromPos(doc, position);
		r.X -= _tb.Width + _TextHorzOffset + 6;

		_SetRect(r);
		_w.ZShow(doc);
	}

	public void Hide()
	{
		_a = null;
		_aVisible = null;
		_groups = null;
		_list.ZAddItems(0, null, null, null);
		_w.Hide();
		_popupHtml.Hide();
		_popupTimer.Stop();
	}

	public CiComplItem SelectedItem {
		get {
			int i = _list.ZSelectedIndex;
			return i >= 0 ? _VisibleItem(i) : null;
		}
		set {
			int i = -1;
			if(value != null)
				for(int j = 0; j < _aVisible.Count; j++) if(_VisibleItem(j) == value) { i = j; break; }
			if(_list.ZSelectedIndex == i) return;
			_list.ZSelectIndex(i, scrollCenter: true);
		}
	}

	void _Sort()
	{
		_aVisible.Sort((i1, i2) => {
			CiComplItem c1 = _a[i1], c2 = _a[i2];

			int diff = c1.moveDown - c2.moveDown;
			if(diff != 0) return diff;

			if(_groupsEnabled) {
				diff = c1.group - c2.group;
				if(diff != 0) return diff;
				if(Empty(_groups[c1.group])) {
					diff = c1.kind - c2.kind;
					if(diff != 0) return diff;
				}
			}

			return i1 - i2; //the list is already sorted by name
		});
	}

	void _SetRect(RECT anchor)
	{
		var ra = _doc.RectangleToScreen(anchor);
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

	void _DrawItem(AuListControl.ZItemDrawArgs e)
	{
		//var p1 = APerf.Create();
		var g = e.graphics;
		var ci = _VisibleItem(e.index);
		var r = e.bounds;
		int xText = _TextHorzOffset;
		var sym = ci.FirstSymbol;

		//draw images: kind, access, static/abstract
		var imageKind = ci.KindImage;
		if(imageKind != null) {
			int imgX = r.X + Au.Util.ADpi.ScaleInt(18);
			int imgY = (r.Y + r.Bottom + 1) / 2 - imageKind.Height / 2;
			int overlayX = r.X + Au.Util.ADpi.ScaleInt(4);
			int overlayY = imgY + Au.Util.ADpi.ScaleInt(4);
			g.DrawImage(imageKind, imgX, imgY, imageKind.Width, imageKind.Height); //note: would be very slow, but is very fast if DoubleBuffered = true (the control sets it in ctor).
			var imageAccess = ci.AccessImage;
			if(imageAccess != null) g.DrawImage(imageAccess, overlayX, overlayY, imageAccess.Width, imageAccess.Height);
			if(sym != null) {
				Bitmap b = null;
				if(sym.IsStatic && ci.kind != CiItemKind.Constant && ci.kind != CiItemKind.EnumMember && ci.kind != CiItemKind.Namespace) b = _imgStatic;
				else if(ci.kind == CiItemKind.Class && sym.IsAbstract) b = _imgAbstract;
				if(b != null) g.DrawImage(b, overlayX, overlayY, b.Width, b.Height);
			}
		}
		//p1.Next();

		//draw selection
		r.Width -= xText; r.X += xText;
		if(e.isSelected) g.FillRectangle(s_selectedBrush, r);

		//draw text
		var s = ci.DisplayText;
		ADebug.PrintIf(!Empty(ci.ci.DisplayTextPrefix), s); //we don't support prefix; never seen.
		int xEndOfText = 0;
		using(var tr = new GdiTextRenderer(g)) {
			ColorInt color = ci.moveDown.HasAny(CiItemMoveDownBy.Name | CiItemMoveDownBy.FilterText) ? 0x808080 : 0;
			tr.MoveTo(r.X, r.Y);
			if(ci.hilite != 0) {
				ulong h = ci.hilite;
				for(int normalFrom = 0, boldFrom, boldTo, to = s.Length; normalFrom < to; normalFrom = boldTo) {
					for(boldFrom = normalFrom; boldFrom < to && 0 == (h & 1); boldFrom++) h >>= 1;
					tr.DrawText(s, color, normalFrom, boldFrom);
					if(boldFrom == to) break;
					for(boldTo = boldFrom; boldTo < to && 0 != (h & 1); boldTo++) h >>= 1;
					tr.FontBold(); tr.DrawText(s, color, boldFrom, boldTo); tr.FontNormal();
				}
			} else {
				tr.DrawText(s, color);
			}

			if(ci.moveDown.Has(CiItemMoveDownBy.Obsolete)) xEndOfText = tr.GetCurrentPosition().x;

			string green = _GreenSuffix(e.index);
			if(green != null) tr.DrawText(green, 0x40A000);
		}

		//draw red line over obsolete items
		if(ci.moveDown.Has(CiItemMoveDownBy.Obsolete)) {
			int vCenter = r.Y + r.Height / 2;
			g.DrawLine(Pens.OrangeRed, r.X, vCenter, xEndOfText, vCenter);
		}
		//p1.NW('i');

		//draw group separator
		if(e.index > 0) {
			var cip = _VisibleItem(e.index - 1);
			if(cip.moveDown != ci.moveDown || (_groupsEnabled && cip.group != ci.group)) {
				g.DrawLine(Pens.YellowGreen, 0, r.Y, r.Right, r.Y);
			}
		}
	}

	static Brush s_selectedBrush = new SolidBrush((Color)(ColorInt)0xc4d5ff);

	static int _TextHorzOffset => Au.Util.ADpi.ScaleInt(38);

	string _GreenSuffix(int visibleIndex)
	{
		var ci = _VisibleItem(visibleIndex);
		var r = ci.ci.InlineDescription;
		if(Empty(r) && _groupsEnabled && (visibleIndex == 0 || _VisibleItem(visibleIndex - 1).group != ci.group)) r = _groups[ci.group];
		return Empty(r) ? null : "    //" + r;
	}

	public bool OnCmdKey(Keys keyData)
	{
		if(_w.Visible) {
			switch(keyData) {
			case Keys.Escape:
				Hide();
				return true;
			case Keys.Down:
			case Keys.Up:
			case Keys.PageDown:
			case Keys.PageUp:
				_list.ZKeyboardNavigation(keyData);
				return true;
			}
		}
		return false;
	}

	class _Window : InactiveWindow
	{
		CiPopupList _p;

		public _Window(CiPopupList p)
		{
			_p = p;

			this.Name = "Ci.PopupList";
			this.Text = "Au autocompletion list";
			this.MinimumSize = Au.Util.ADpi.ScaleSize((150, 150));
		}

		protected override unsafe void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_NCMOUSEMOVE);
			switch(m.Msg) {
			case Api.WM_DESTROY:
				Program.Settings.ci_complGroup = _p._groupButton.Checked;
				break;
			}

			base.WndProc(ref m);
		}
	}

	class _FastListBox : AuListControl
	{
		//CiPopupList _p;

		public _FastListBox(/*CiPopupList p*/)
		{
			//_p = p;
			this.SetStyle(ControlStyles.Selectable, false); //prevent focusing control and activating window on click
		}
	}
}
