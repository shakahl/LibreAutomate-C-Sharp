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
using System.Windows.Interop;
using System.Windows.Automation;
using System.Windows.Input;

class CiPopupList
{
	KPopup _popup;
	DockPanel _panel;
	AuTreeView _tv;
	StackPanel _tb;
	int _dpi;

	SciCode _doc;
	CiCompletion _compl;
	List<CiComplItem> _a;
	List<CiComplItem> _av;
	CheckBox[] _kindButtons;
	CheckBox _groupButton;
	bool _groupsEnabled;
	List<string> _groups;
	CiPopupHtml _popupHtml;
	ATimer _popupTimer;

	public KPopup PopupWindow => _popup;

	///// <summary>
	///// The child list control.
	///// </summary>
	//public AuTreeView Control => _tv;

	public CiPopupList(CiCompletion compl) {
		_compl = compl;

		_tv = new AuTreeView { ImageCache = App.ImageCache, ItemMarginLeft = 20, HotTrack = true, CustomDraw = new _CustomDraw(this) };
		_tv.ItemActivated += _tv_ItemActivated;
		_tv.SelectedSingle += _tv_SelectedSingle;

		_tb = new StackPanel { Background = SystemColors.ControlBrush };

		var cstyle = Application.Current.FindResource(ToolBar.CheckBoxStyleKey) as Style;
		var bstyle = Application.Current.FindResource(ToolBar.ButtonStyleKey) as Style;

		var kindNames = CiUtil.ItemKindNames;
		_kindButtons = new CheckBox[kindNames.Length];
		for (int i = 0; i < kindNames.Length; i++) {
			_AddButton(_kindButtons[i] = new CheckBox(), kindNames[i], CiComplItem.ImageResource((CiItemKind)i));
			_kindButtons[i].Click += _KindButton_Click;
		}

		_tb.Children.Add(new Separator());

		_AddButton(_groupButton = new CheckBox(), "Group by namespace or inheritance", "resources/ci/groupby.xaml");
		_groupButton.Click += _GroupButton_Click;
		if (App.Settings.ci_complGroup) _groupButton.IsChecked = true;

		var options = new Button();
		options.Click += _Options_Click;
		_AddButton(options, "Options", "resources/images/settingsgroup_16x.xaml");

		void _AddButton(ButtonBase b, string text, string image) {
			b.Style = (b is CheckBox) ? cstyle : bstyle;
			b.Content = AResources.GetWpfImageElement(image);
			b.ToolTip = text;
			AutomationProperties.SetName(b, text);
			b.Focusable = false; //would close popup
			_tb.Children.Add(b);
		}

		_panel = new DockPanel { Background = SystemColors.WindowBrush };
		DockPanel.SetDock(_tb, Dock.Left);
		_panel.Children.Add(_tb);
		_panel.Children.Add(_tv);
		_popup = new KPopup { Size = (240, 360), Content = _panel };

		_popupHtml = new CiPopupHtml(CiPopupHtml.UsedBy.PopupList);
		_popupTimer = new ATimer(_ShowPopupHtml);
	}

	private void _KindButton_Click(object sender, RoutedEventArgs e) {
		int kindsChecked = 0, kindsVisible = 0;
		for (int i = 0; i < _kindButtons.Length; i++) {
			var v = _kindButtons[i];
			if (v.IsVisible) {
				kindsVisible |= 1 << i;
				if (v.IsCheck()) kindsChecked |= 1 << i;
			}
		}
		if (kindsChecked == 0) kindsChecked = kindsVisible;
		foreach (var v in _a) {
			if (0 != (kindsChecked & (1 << (int)v.kind))) v.hidden &= ~CiItemHiddenBy.Kind; else v.hidden |= CiItemHiddenBy.Kind;
		}
		UpdateVisibleItems();
	}

	private void _GroupButton_Click(object sender, RoutedEventArgs e) {
		_groupsEnabled = (sender as CheckBox).IsCheck() && _groups != null;
		_SortAndSetControlItems();
		this.SelectedItem = null;
		_tv.Redraw(true);
		App.Settings.ci_complGroup = _groupButton.IsCheck();
	}

	private void _Options_Click(object sender, RoutedEventArgs e) {
		var m = new AWpfMenu();
		//m[""] = o => ;
		m.PlacementTarget = sender as Button;
		m.IsOpen = true;
	}

	private void _tv_ItemActivated(object sender, TVItemEventArgs e) {
		_compl.Commit(_doc, _av[e.Index]);
		Hide();
	}

	private void _tv_SelectedSingle(object sender, int index) {
		if ((uint)index < _av.Count) {
			_popupTimer.After(300, _av[index]);
			_popupHtml.Html = null;
		} else {
			_popupHtml.Hide();
			_popupTimer.Stop();
		}
	}

	void _ShowPopupHtml(ATimer t) {
		var ci = t.Tag as CiComplItem;
		var html = _compl.GetDescriptionHtml(ci, 0);
		AOutput.Write(html);
		if (html == null) return;
		_popupHtml.Html = html;
		_popupHtml.OnLinkClick = (ph, e) => ph.Html = _compl.GetDescriptionHtml(ci, e.Link.ToInt(1));
		_popupHtml.Show(Panels.Editor.ZActiveDoc, _popup.Hwnd.Rect);
	}

	void _SortAndSetControlItems() {
		_av.Sort((c1, c2) => {
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

			return string.Compare(c1.ci.DisplayText, c2.ci.DisplayText, StringComparison.OrdinalIgnoreCase);
		});

		CiComplItem prev = null;
		foreach (var v in _av) {
			var group = _groupsEnabled && (prev == null || v.group != prev.group) ? _groups[v.group] : null;
			v.SetDisplayText(group);
			prev = v;
		}

		_tv.SetItems(_av, false);
	}

	public void UpdateVisibleItems() {
		int n1 = 0; foreach (var v in _a) if (v.hidden == 0) n1++;
		_av = new(n1);
		foreach (var v in _a) if (v.hidden == 0) _av.Add(v);
		_SortAndSetControlItems();

		_compl.SelectBestMatch(_av.Select(o => o.ci)); //pass items sorted like in the visible list

		int kinds = 0;
		foreach (var v in _a) if ((v.hidden & ~CiItemHiddenBy.Kind) == 0) kinds |= 1 << (int)v.kind;
		for (int i = 0; i < _kindButtons.Length; i++) _kindButtons[i].Visibility = 0 != (kinds & (1 << i)) ? Visibility.Visible : Visibility.Collapsed;
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

		foreach (var v in _kindButtons) v.IsChecked = false;
		_groupButton.Visibility = _groups != null ? Visibility.Visible : Visibility.Collapsed;
		UpdateVisibleItems();

		var r = CiUtil.GetCaretRectFromPos(_doc, position, inScreen: true);
		r.left -= _Scale(50);

		_popup.ShowByRect(_doc, Dock.Bottom, r);
	}

	public void Hide() {
		if (_a == null) return;
		_tv.SetItems(null, false);
		_popup.Hide();
		_popupHtml.Hide();
		_popupTimer.Stop();
		_a = null;
		_av = null;
		_groups = null;
	}

	public CiComplItem SelectedItem {
		get {
			int i = _tv.SelectedIndex;
			return i >= 0 ? _av[i] : null;
		}
		set {
			int i = value == null ? -1 : _av.IndexOf(value);
			if (_tv.SelectedIndex == i) return;
			if (i >= 0) _tv.SelectSingle(i, andFocus: true); else _tv.UnselectAll();
			//SHOULDDO: scroll center
		}
	}

	public bool OnCmdKey(KKey key) {
		if (_popup.IsVisible) {
			switch (key) {
			case KKey.Escape:
				Hide();
				return true;
			case KKey.Down:
			case KKey.Up:
			case KKey.PageDown:
			case KKey.PageUp:
			//case KKey.Home:
			//case KKey.End:
				_tv.ProcessKey(AKeys.More.KKeyToWpf(key));
				return true;
			}
		}
		return false;
	}

	int _Scale(int i) => AMath.MulDiv(i, _dpi, 96);

	//void _SetRect(RECT anchor) {
	//	var ra = _doc.RectangleToScreen(anchor);
	//	var rs = AScreen.Of(ra).WorkArea;
	//	rs.Inflate(-1, -5);
	//	int heiAbove = ra.Top - rs.top, heiBelow = rs.bottom - ra.Bottom;
	//	int maxHeight = Math.Max(heiAbove, heiBelow); if (maxHeight < 200) maxHeight = 200;

	//	var r = _w.Bounds;
	//	int width = r.Width, height = r.Height;
	//	if (height > maxHeight) { _height = height; height = maxHeight; } else if (_height > 0 && _height <= maxHeight) { height = _height; _height = 0; }
	//	r.Height = height;

	//	r.X = ra.Left + width <= rs.right ? ra.Left : rs.right - width; r.X = Math.Max(r.X, rs.left);
	//	bool down = height <= heiBelow || heiAbove <= heiBelow;
	//	r.Y = down ? ra.Bottom : ra.Top - height;
	//	_w.Bounds = r;
	//}

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

		public bool DrawText() {
			var ci = _cd.item as CiComplItem;

			var s = ci.DisplayText;
			Range black, green;
			if (ci.commentOffset == 0) { black = ..s.Length; green = default; } else { black = ..ci.commentOffset; green = ci.commentOffset..; }

			ADebug.PrintIf(!ci.ci.DisplayTextPrefix.NE(), s); //we don't support prefix; never seen.
			int xEndOfText = 0;
			int color = ci.moveDown.HasAny(CiItemMoveDownBy.Name | CiItemMoveDownBy.FilterText) ? 0x808080 : 0;
			_tr.MoveTo(_cd.xText, _cd.yText);
			if (ci.hilite != 0) {
				ulong h = ci.hilite;
				for (int normalFrom = 0, boldFrom, boldTo, to = black.End.Value; normalFrom < to; normalFrom = boldTo) {
					for (boldFrom = normalFrom; boldFrom < to && 0 == (h & 1); boldFrom++) h >>= 1;
					_tr.DrawText(s, color, normalFrom..boldFrom);
					if (boldFrom == to) break;
					for (boldTo = boldFrom; boldTo < to && 0 != (h & 1); boldTo++) h >>= 1;
					_tr.FontBold(); _tr.DrawText(s, color, boldFrom..boldTo); _tr.FontNormal();
				}
			} else {
				_tr.DrawText(s, color, black);
			}

			if (ci.moveDown.Has(CiItemMoveDownBy.Obsolete)) xEndOfText = _tr.GetCurrentPosition().x;

			if (ci.commentOffset > 0) _tr.DrawText(s, 0x00A040, green);

			//draw red line over obsolete items
			if (ci.moveDown.Has(CiItemMoveDownBy.Obsolete)) {
				int vCenter = _cd.rect.top + _cd.rect.Height / 2;
				_cd.graphics.DrawLine(System.Drawing.Pens.OrangeRed, _cd.xText, vCenter, xEndOfText, vCenter);
			}

			return true;
		}

		public void DrawMarginLeft() {
			var ci = _cd.item as CiComplItem;
			var g = _cd.graphics;
			var r = _cd.rect;

			//draw images: access, static/abstract
			var cxy = _cd.imageRect.Width;
			var ri = new System.Drawing.Rectangle(_cd.imageRect.left - cxy, _cd.imageRect.top + cxy / 4, cxy, cxy);
			var s = ci.AccessImageSource;
			if (s != null) g.DrawImage(App.ImageCache.Get(s, _cd.dpi, true), ri);
			var sym = ci.FirstSymbol;
			if (sym != null) {
				s = null;
				if (sym.IsStatic && ci.kind != CiItemKind.Constant && ci.kind != CiItemKind.EnumMember && ci.kind != CiItemKind.Namespace) s = "resource:resources/ci/overlaystatic.xaml";
				else if (ci.kind == CiItemKind.Class && sym.IsAbstract) s = "resource:resources/ci/overlayabstract.xaml";
				if (s != null) g.DrawImage(App.ImageCache.Get(s, _cd.dpi, true), ri);
			}

			//draw group separator
			if (_cd.index > 0) {
				var cip = _p._av[_cd.index - 1];
				if (cip.moveDown != ci.moveDown || (_p._groupsEnabled && cip.group != ci.group)) {
					_cd.graphics.DrawLine(System.Drawing.Pens.YellowGreen, 0, r.top + 1, r.right, r.top + 1);
				}
			}
		}

		//public void DrawMarginRight() {
		//}

		//public void End() {
		//}
	}
}
