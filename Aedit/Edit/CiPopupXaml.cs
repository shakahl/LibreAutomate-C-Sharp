using Au;
using Au.Types;
using Au.Util;
using Au.Controls;
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
//using System.Linq;
using System.Windows;
using System.Windows.Controls;

class CiPopupXaml
{
	public enum UsedBy { PopupList, Signature, Info }

	KPopup _w;
	FlowDocumentControl _c;
	EventHandler _onHiddenOrDestroyed;
	UsedBy _usedBy;
	string _xaml;
	bool _updateXaml;

	public CiPopupXaml(UsedBy usedy, EventHandler onHiddenOrDestroyed = null) {
		_usedBy = usedy;
		_onHiddenOrDestroyed = onHiddenOrDestroyed;
	}

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public KPopup PopupWindow => _CreateOrGet();

	/// <summary>
	/// Called when clicked a link with href prefix "^".
	/// </summary>
	public Action<CiPopupXaml, string> OnLinkClick { get; set; }

	/// <summary>
	/// XAML to show. Set before calling Show.
	/// </summary>
	public string Xaml {
		get => _xaml;
		set {
			if (value != _xaml) {
				_xaml = value;
				if (IsVisible) _SetXaml(); else _updateXaml = true;
			}
		}
	}

	KPopup _CreateOrGet() {
		if (_w == null) {
			bool ubInfo = _usedBy == UsedBy.Info;
			_w = new KPopup(ubInfo ? WS.POPUP | WS.BORDER : WS.POPUP | WS.THICKFRAME, shadow: ubInfo, sizeToContent: ubInfo ? SizeToContent.Height : default) {
				Name = "Ci.Info",
				WindowName = _usedBy switch { UsedBy.PopupList => "Au completion item info", UsedBy.Signature => "Au parameters info", _ => "Au quick info" },
				CloseHides = true
			};
			bool ubList = _usedBy == UsedBy.PopupList;
			_w.Size = (ubList ? 450 : 600, ubList ? 360 : 300);
			//rejected: save size in app settings (if not info). Popup list too.

			_c = CiXaml.CreateControl();
			_w.Content = _c;

			if (_onHiddenOrDestroyed != null) _w.Hidden += _onHiddenOrDestroyed;
			_c.LinkClicked += (sender, s) => {
				if (s.Starts('#')) return; //anchor
				if (s.Starts('^')) {
					OnLinkClick?.Invoke(this, s);
				} else if (s.Starts('|')) { //go to symbol source file/position or web page
					CiGoTo.LinkGoTo(s, _c);
				} else {
					AFile.TryRun(s);
				}
			};

			//never mind: on mouse up: if(IsVisible && !c.SelectedText.NE()) _w.Hwnd.ActivateLL(); //the user may want Ctrl+C
			//	but no mouse up events, even PreviewX.
			//	can use context menu instead.
		}
		return _w;
	}

	/// <summary>
	/// Shows by anchorRect, owned by ownerControl.
	/// </summary>
	/// <param name="ownerControl"></param>
	/// <param name="anchorRect">Rectangle in screen coord. The popup window will be by it but not in it.</param>
	/// <param name="align"></param>
	/// <param name="hideIfOutside">Hide when mouse moved outside anchorRect unless is in the popup window.</param>
	public void Show(SciCode ownerControl, RECT anchorRect, Dock side, bool hideIfOutside = false) {
		_CreateOrGet();
		if (_updateXaml) _SetXaml();
		_w.ShowByRect(ownerControl, side, anchorRect);

		if (hideIfOutside) {
			ATimer.Every(100, t => {
				if (IsVisible) {
					var p = AMouse.XY;
					if (anchorRect.Contains(p) || _w.Hwnd.Rect.Contains(p) || _c.IsMouseCaptureWithin) return;
					Hide();
				}
				t.Stop();
			});
		}
	}

	public void Show(SciCode ownerControl, int pos16, bool hideIfOutside = false, bool above = false) {
		var r = CiUtil.GetCaretRectFromPos(ownerControl, pos16, inScreen: true);
		r.Inflate(50, 0);
		Show(ownerControl, r, above ? Dock.Top : Dock.Bottom, hideIfOutside);
	}

	public bool Hide() {
		if (!IsVisible) return false;
		//AOutput.Write(new StackTrace());
		_w.Close();
		_xaml = null;
		_SetXaml();
		return true;
	}

	public bool IsVisible => _w?.IsVisible ?? false;

	void _SetXaml() {
		_updateXaml = false;
		var fd = _c?.Document;
		fd?.Blocks.Clear();
		if (_xaml.NE()) return;
		fd.Blocks.Add(CiXaml.Parse(_xaml));
	}
}
