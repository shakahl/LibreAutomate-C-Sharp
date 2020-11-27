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
using System.Linq;

using Au;
using Au.Types;
using Au.Util;
using Au.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

class CiPopupList
{
	Popup _popup;
	AuTreeView _list;
	ToolBar _tb;
	int _nKindButtons;
	int _height;
	int _dpi;

	SciCode _doc;
	CiCompletion _compl;
	List<CiComplItem> _a;
	List<int> _aVisible; //indices of visible _a items
	List<string> _groups;
	CheckBox _groupButton;
	bool _groupsEnabled;
	CiPopupHtml _popupHtml;
	ATimer _popupTimer;

	///// <summary>
	///// The top-level popup window.
	///// </summary>
	//public Popup PopupWindow => _popup;

	///// <summary>
	///// The child list control.
	///// </summary>
	//public AuTreeView ListControl => _list;

	public CiPopupList(CiCompletion compl, SciCode docForDpi) {
		_compl = compl;
		//_dpi = ADpi.OfWindow(docForDpi);//TODO: remove docForDpi param

		_list = new AuTreeView { ImageCache = App.ImageCache, ItemMarginLeft = 20, HotTrack = true, CustomDraw = new _CustomDraw(this) };
		//_list.AccessibleName = _list.Name = "Codein_list";
		//_list.ZItemClick += _list_ItemClick;
		//_list.ZSelectedIndexChanged += _list_SelectedIndexChanged;

		_tb = new ToolBar();
		//_tb.AccessibleName = _tb.Name = "Codein_listFilter";

		var kindNames = CiUtil.ItemKindNames;
		_nKindButtons = kindNames.Length;
		for (int i = 0; i < kindNames.Length; i++) _AddButton(kindNames[i], CiComplItem.ImageResource((CiItemKind)i));
		_tb.Items.Add(new Separator());
		_groupButton = _AddButton("Group by namespace or inheritance", "resources/ci/groupby.xaml");
		if (App.Settings.ci_complGroup) _groupButton.IsChecked = true;

		CheckBox _AddButton(string text, string image) {
			var b = new CheckBox { ToolTip = text, Content = AResources.GetWpfImageElement(image) };
			_tb.Items.Add(b);
			//b.AccessibleName = text;
			//b.Margin = new Padding(1, 1, 0, 0);
			return b;
		}

		//_tb.ItemClicked += _tb_ItemClicked;

		var ttray = new ToolBarTray { Orientation = Orientation.Vertical, IsLocked = true };
		ttray.ToolBars.Add(_tb);
		var panel = new DockPanel { Background = SystemColors.WindowBrush, Width = 300, Height = 360 };
		DockPanel.SetDock(_tb, Dock.Left);
		panel.Children.Add(ttray);
		panel.Children.Add(_list);
		_popup = new Popup { Child = new Border { Child = panel, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) } };

		_popupHtml = new CiPopupHtml(CiPopupHtml.UsedBy.PopupList);
		_popupTimer = new ATimer(_ShowPopupHtml);
	}

	//private void _tb_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
	//	var b = e.ClickedItem as ToolStripButton;
	//	bool check = !b.Checked; b.Checked = check;

	//	int index = _tb.Items.IndexOf(e.ClickedItem);
	//	if (index < _nKindButtons) {
	//		int kindsChecked = 0, kindsVisible = 0;
	//		for (int i = 0, n = _nKindButtons; i < n; i++) {
	//			var v = _tb.Items[i];
	//			if (v.Visible) {
	//				kindsVisible |= 1 << i;
	//				if ((v as ToolStripButton).Checked) kindsChecked |= 1 << i;
	//			}
	//		}
	//		if (kindsChecked == 0) kindsChecked = kindsVisible;
	//		foreach (var v in _a) {
	//			if (0 != (kindsChecked & (1 << (int)v.kind))) v.hidden &= ~CiItemHiddenBy.Kind; else v.hidden |= CiItemHiddenBy.Kind;
	//		}
	//		UpdateVisibleItems();
	//	} else if (e.ClickedItem == _groupButton) {
	//		_groupsEnabled = check && _groups != null;
	//		_Sort();
	//		this.SelectedItem = null;
	//		_list.ZReMeasure();
	//	}
	//}

	private void _list_ItemClick(object sender, AuListControl.ZItemClickArgs e) {
		if (e.DoubleClick) {
			_compl.Commit(_doc, _VisibleItem(e.Index));
			Hide();
		}
	}

	private void _list_SelectedIndexChanged(int index) {
		if ((uint)index < _VisibleCount) {
			_popupTimer.After(300, _VisibleItem(index));
			_popupHtml.Html = null;
		} else {
			_popupHtml.Hide();
			_popupTimer.Stop();
		}
	}

	void _ShowPopupHtml(ATimer t) {
		var ci = t.Tag as CiComplItem;
		var html = _compl.GetDescriptionHtml(ci, 0);
		if (html == null) return;
		_popupHtml.Html = html;
		_popupHtml.OnLinkClick = (ph, e) => ph.Html = _compl.GetDescriptionHtml(ci, e.Link.ToInt(1));
		_popupHtml.Show(Panels.Editor.ZActiveDoc, _popup.Hwnd().Rect);
	}

	int _VisibleCount => _aVisible.Count;

	CiComplItem _VisibleItem(int index) => _a[_aVisible[index]];

	public void UpdateVisibleItems() {
		int n1 = 0; foreach (var v in _a) if (v.hidden == 0) n1++;
		_aVisible = new List<int>(n1);
		for (int i = 0; i < _a.Count; i++) if (_a[i].hidden == 0) _aVisible.Add(i);
		_Sort();

		//_list.SetItems(_VisibleCount,
		//	o => _TextHorzOffset + o.MeasureText(_VisibleItem(o.index).DisplayText).width + o.MeasureText(_GreenSuffix(o.index)).width,
		//	_DrawItem,
		//	i => _VisibleItem(i).DisplayText,
		//	_dpi);
		_list.SetItems(_a, false);//TODO
		_compl.SelectBestMatch(_aVisible.Select(i => _a[i].ci)); //pass items sorted like in the visible list

		int kinds = 0;
		foreach (var v in _a) if ((v.hidden & ~CiItemHiddenBy.Kind) == 0) kinds |= 1 << (int)v.kind;
		for (int i = 0, n = _nKindButtons; i < n; i++) (_tb.Items[i] as CheckBox).Visibility = 0 != (kinds & (1 << i)) ? Visibility.Visible : Visibility.Collapsed;
	}

	public void Show(SciCode doc, int position, List<CiComplItem> a, List<string> groups) {
		if (a.NE_()) {
			Hide();
			return;
		}

		_a = a;
		_groups = groups;
		_groupsEnabled = _groups != null && _groupButton.IsCheck();
		_doc = doc;
		_dpi = ADpi.OfWindow(_doc);

		for (int i = 0, n = _nKindButtons; i < n; i++) (_tb.Items[i] as CheckBox).IsChecked = false;
		_groupButton.Visibility = _groups != null ? Visibility.Visible : Visibility.Collapsed;
		UpdateVisibleItems();

		var r = CiUtil.GetCaretRectFromPos(_doc, position);
		r.left -= _Scale(50);

		//_SetRect(r);
		_popup.PlacementTarget = _doc;
		_popup.PlacementRectangle = ADpi.Unscale(r, _dpi);
		_popup.Placement = PlacementMode.Bottom;
		_popup.IsOpen = true;
	}

	public void Hide() {
		_a = null;
		_aVisible = null;
		_groups = null;
		_list.SetItems(null, false);
		_popup.IsOpen = false;
		_popupHtml.Hide();
		_popupTimer.Stop();
	}

	public CiComplItem SelectedItem {
		get {
			int i = _list.SelectedIndex;
			return i >= 0 ? _VisibleItem(i) : null;
		}
		set {
			int i = -1;
			if (value != null)
				for (int j = 0; j < _aVisible.Count; j++) if (_VisibleItem(j) == value) { i = j; break; }
			if (_list.SelectedIndex == i) return;
			//_list.Select(i, scrollCenter: true);//TODO
			_list.SelectSingle(i, andFocus: true);
		}
	}

	void _Sort() {
		_aVisible.Sort((i1, i2) => {
			CiComplItem c1 = _a[i1], c2 = _a[i2];

			int diff = c1.moveDown - c2.moveDown;
			if (diff != 0) return diff;

			if (_groupsEnabled) {
				diff = c1.group - c2.group;
				if (diff != 0) return diff;
				if (_groups[c1.group].NE()) {
					diff = c1.kind - c2.kind;
					if (diff != 0) return diff;
				}
			}

			return i1 - i2; //the list is already sorted by name
		});
	}

	void _SetRect(RECT anchor) {
		//TODO
		//var ra = _doc.RectangleToScreen(anchor);
		//var rs = AScreen.Of(ra).WorkArea;
		//rs.Inflate(-1, -5);
		//int heiAbove = ra.Top - rs.top, heiBelow = rs.bottom - ra.Bottom;
		//int maxHeight = Math.Max(heiAbove, heiBelow); if (maxHeight < 200) maxHeight = 200;

		//var r = _w.Bounds;
		//int width = r.Width, height = r.Height;
		//if (height > maxHeight) { _height = height; height = maxHeight; } else if (_height > 0 && _height <= maxHeight) { height = _height; _height = 0; }
		//r.Height = height;

		//r.X = ra.Left + width <= rs.right ? ra.Left : rs.right - width; r.X = Math.Max(r.X, rs.left);
		//bool down = height <= heiBelow || heiAbove <= heiBelow;
		//r.Y = down ? ra.Bottom : ra.Top - height;
		//_w.Bounds = r;
	}

	//void _DrawItem(AuListControl.ZItemDrawArgs e) {
	//	//var p1 = APerf.Create();
	//	var g = e.graphics;
	//	var ci = _VisibleItem(e.index);
	//	var r = e.bounds;
	//	int xText = _TextHorzOffset;
	//	var sym = ci.FirstSymbol;

	//	//draw images: kind, access, static/abstract
	//	var imageKind = CiUtil.GetKindImage(ci.kind, _dpi);
	//	if (imageKind != null) {
	//		int imgX = r.X + _Scale(18);
	//		int imgY = (r.Y + r.Bottom + 1) / 2 - imageKind.Height / 2;
	//		int overlayX = r.X + _Scale(4);
	//		int overlayY = imgY + _Scale(4);
	//		g.DrawImage(imageKind, imgX, imgY, imageKind.Width, imageKind.Height); //note: would be very slow, but is very fast if DoubleBuffered = true (the control sets it in ctor).
	//		var imageAccess = CiUtil.GetAccessImage(ci.access, _dpi);
	//		if (imageAccess != null) g.DrawImage(imageAccess, overlayX, overlayY, imageAccess.Width, imageAccess.Height);
	//		if (sym != null) {
	//			Bitmap b = null;
	//			if (sym.IsStatic && ci.kind != CiItemKind.Constant && ci.kind != CiItemKind.EnumMember && ci.kind != CiItemKind.Namespace) b = _imgStatic;
	//			else if (ci.kind == CiItemKind.Class && sym.IsAbstract) b = _imgAbstract;
	//			if (b != null) g.DrawImage(b, overlayX, overlayY, b.Width, b.Height);
	//		}
	//	}
	//	//p1.Next();

	//	//draw selection
	//	r.Width -= xText; r.X += xText;
	//	if (e.isSelected) g.FillRectangle(s_selectedBrush, r);

	//	//draw text
	//	var s = ci.DisplayText;
	//	ADebug.PrintIf(!ci.ci.DisplayTextPrefix.NE(), s); //we don't support prefix; never seen.
	//	int xEndOfText = 0;
	//	var tr = new GdiTextRenderer(g.GetHdc(), _dpi);
	//	try {
	//		int color = ci.moveDown.HasAny(CiItemMoveDownBy.Name | CiItemMoveDownBy.FilterText) ? 0x808080 : 0;
	//		tr.MoveTo(r.X, r.Y);
	//		if (ci.hilite != 0) {
	//			ulong h = ci.hilite;
	//			for (int normalFrom = 0, boldFrom, boldTo, to = s.Length; normalFrom < to; normalFrom = boldTo) {
	//				for (boldFrom = normalFrom; boldFrom < to && 0 == (h & 1); boldFrom++) h >>= 1;
	//				tr.DrawText(s, color, normalFrom..boldFrom);
	//				if (boldFrom == to) break;
	//				for (boldTo = boldFrom; boldTo < to && 0 != (h & 1); boldTo++) h >>= 1;
	//				tr.FontBold(); tr.DrawText(s, color, boldFrom..boldTo); tr.FontNormal();
	//			}
	//		} else {
	//			tr.DrawText(s, color);
	//		}

	//		if (ci.moveDown.Has(CiItemMoveDownBy.Obsolete)) xEndOfText = tr.GetCurrentPosition().x;

	//		string green = _GreenSuffix(e.index);
	//		if (green != null) tr.DrawText(green, 0x00A040);
	//	}
	//	finally { g.ReleaseHdc(); tr.Dispose(); }

	//	//draw red line over obsolete items
	//	if (ci.moveDown.Has(CiItemMoveDownBy.Obsolete)) {
	//		int vCenter = r.Y + r.Height / 2;
	//		g.DrawLine(Pens.OrangeRed, r.X, vCenter, xEndOfText, vCenter);
	//	}
	//	//p1.NW('i');

	//	//draw group separator
	//	if (e.index > 0) {
	//		var cip = _VisibleItem(e.index - 1);
	//		if (cip.moveDown != ci.moveDown || (_groupsEnabled && cip.group != ci.group)) {
	//			g.DrawLine(Pens.YellowGreen, 0, r.Y, r.Right, r.Y);
	//		}
	//	}
	//}

	//static Brush s_selectedBrush = new SolidBrush((Color)(ColorInt)0xc4d5ff);

	class _CustomDraw : ITVCustomDraw
	{
		CiPopupList _p;
		TVDrawInfo _cd;
		GdiTextRenderer _tr;

		public _CustomDraw(CiPopupList list) {
			_p = list;
		}

		public void Begin(TVDrawInfo cd, GdiTextRenderer tr) {
			_cd = cd;
			_tr = tr;
		}

		//public bool DrawBackground() {
		//}

		//public bool DrawCheckbox() {
		//}

		//public bool DrawImage(System.Drawing.Bitmap image) {
		//}

		public void DrawMarginLeft() {
			var cxy = _cd.imageRect.Width;
			var r = new System.Drawing.Rectangle(_cd.imageRect.left-cxy, _cd.imageRect.top + cxy/4, cxy, cxy);
			var g = _cd.graphics;
			var ci = _cd.item as CiComplItem;
			var s = ci.AccessImageSource;
			if (s != null) g.DrawImage(App.ImageCache.Get(s, _cd.dpi, true), r);
			var sym = ci.FirstSymbol;
			if (sym != null) {
				s = null;
				if (sym.IsStatic && ci.kind != CiItemKind.Constant && ci.kind != CiItemKind.EnumMember && ci.kind != CiItemKind.Namespace) s = "resource:resources/ci/overlaystatic.xaml";
				else if (ci.kind == CiItemKind.Class && sym.IsAbstract) s = "resource:resources/ci/overlayabstract.xaml";
				if (s != null) g.DrawImage(App.ImageCache.Get(s, _cd.dpi, true), r);
			}
		}

		//public void DrawMarginRight() {
		//}

		public bool DrawText() {


			return false;
		}

		public void End() {
		}
	}

	int _Scale(int i) => AMath.MulDiv(i, _dpi, 96);

	int _TextHorzOffset => _Scale(38);

	string _GreenSuffix(int visibleIndex) {
		var ci = _VisibleItem(visibleIndex);
		var r = ci.ci.InlineDescription;
		if (r.NE() && _groupsEnabled && (visibleIndex == 0 || _VisibleItem(visibleIndex - 1).group != ci.group)) r = _groups[ci.group];
		return r.NE() ? null : "    //" + r;
	}

	public bool OnCmdKey(KKey key) {
		if (_popup.IsOpen) {
			switch (key) {
			case KKey.Escape:
				Hide();
				return true;
			case KKey.Down:
			case KKey.Up:
			case KKey.PageDown:
			case KKey.PageUp:
				//_list.ZKeyboardNavigation(key);//TODO
				return true;
			}
		}
		return false;
	}

	//class _Window : InactiveWindow
	//{
	//	CiPopupList _p;

	//	public _Window(CiPopupList p) {
	//		_p = p;

	//		this.Name = "Ci.PopupList";
	//		this.Text = "Au autocompletion list";
	//	}

	//	protected override unsafe void WndProc(ref Message m) {
	//		//AWnd.More.PrintMsg(m, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_NCMOUSEMOVE);
	//		switch (m.Msg) {
	//		case Api.WM_DESTROY:
	//			App.Settings.ci_complGroup = _p._groupButton.Checked;
	//			break;
	//			//case Api.WM_DPICHANGED:
	//			//	AOutput.Write("WM_DPICHANGED");
	//			//	break;
	//		}

	//		base.WndProc(ref m);
	//	}

	//	protected override void OnDpiChanged(DpiChangedEventArgs e) {
	//		e.Cancel = true;
	//		base.OnDpiChanged(e);
	//	}
	//}

	//class _FastListBox : AuListControl
	//{
	//	//CiPopupList _p;

	//	public _FastListBox(/*CiPopupList p*/) {
	//		//_p = p;
	//		this.SetStyle(ControlStyles.Selectable, false); //prevent focusing control and activating window on click
	//	}
	//}
}
