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

using Au;
using Au.Types;
using Au.Util;
using Au.Controls;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Input;

class CiPopupXaml
{
	public enum UsedBy { PopupList, Signature, Info }

	KPopup _w;
	//FlowDocumentScrollViewer _c;
	FlowDocument _fd;
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

	//public FlowDocumentScrollViewer ViewerControl {
	//	get {
	//		_CreateOrGet();
	//		return _c;
	//	}
	//}

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
			_w = new KPopup(_usedBy == UsedBy.Info ? WS.POPUP | WS.BORDER : WS.POPUP | WS.THICKFRAME, shadow: _usedBy == UsedBy.Info) {
				Name = "Ci.Info",
				WindowName = _usedBy switch { UsedBy.PopupList => "Au list item info", UsedBy.Signature => "Au parameters info", _ => "Au quick info" },
				CloseHides = true
			};
			if (_usedBy == UsedBy.Info) {

			} else {
				//var owner = App.Wmain;
				//_w.MinimumSize = (150, 150);
				_w.Size = (
					//AScreen.Of(App.Wmain).WorkArea.Width / (_usedBy == UsedBy.PopupList ? 3 : 2),
					_usedBy == UsedBy.PopupList ? 360 : 600,
					_usedBy switch { UsedBy.PopupList => 360, UsedBy.Signature => 300, _ => 100 }
					);
				//TODO: save in settings
			}

			var c = CiXaml.CreateControl();
			_fd = c.Document;
			_w.Content = c;

			if (_onHiddenOrDestroyed != null) _w.Hidden += _onHiddenOrDestroyed;

			//TODO: on mouse up: if(IsVisible && !c.SelectedText.NE()) _w.Hwnd.ActivateLL(); //the user may want Ctrl+C
			//but no mouse up events, even PreviewX
		}
		return _w;
	}

	//SIZE _MeasureXaml() {
	//	if (html.NE()) return default;
	//	int sbWid = SystemInformation.VerticalScrollBarWidth;
	//	using var g = _c.CreateGraphics();
	//	var zf = HtmlRender.Measure(g, html, 0, _c.BaseCssData, imageLoad: OnLoadImage);
	//	int wid = (int)zf.Width;
	//	int waWid = AScreen.Of(App.Wmain).WorkArea.Width * 2 / 3 - sbWid;
	//	if (wid > waWid) { //remeasure, because HtmlRender.Measure returns maxWidth parameter if it is > text width
	//		zf = HtmlRender.Measure(g, html, waWid, _c.BaseCssData, imageLoad: OnLoadImage);
	//		wid = (int)zf.Width;
	//	}
	//	return new Size(wid + sbWid, (int)zf.Height + 3);
	//}

	/// <summary>
	/// Shows by anchorRect, owned by ownerControl.
	/// </summary>
	/// <param name="ownerControl"></param>
	/// <param name="anchorRect">Rectangle in screen coord. The popup window will be by it but not in it.</param>
	/// <param name="align"></param>
	/// <param name="hideIfOutside">Hide when mouse moved outside anchorRect unless is in the popup window.</param>
	public void Show(SciCode ownerControl, RECT anchorRect, Dock side, bool hideIfOutside = false) {
		_CreateOrGet();
		if(_updateXaml) _SetXaml();
		_w.ShowByRect(ownerControl, side, anchorRect);

		if (hideIfOutside) {
			ATimer.Every(100, t => {
				if (IsVisible) {
					var p = AMouse.XY;
					if (anchorRect.Contains(p) || _w.Hwnd.Rect.Contains(p) /*|| _w.Capture || _c.Capture*/) return; //TODO: Capture
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

	public void Hide() {
		if (!IsVisible) return;
		//AOutput.Write(new StackTrace());
		_w.Close();
		_xaml = null;
		_SetXaml();
	}

	public bool IsVisible => _w?.IsVisible ?? false;

	void _SetXaml() {
		_updateXaml = false;
		_fd?.Blocks.Clear();
		if (_xaml.NE()) return;
		_fd.Blocks.Add(CiXaml.Parse(_xaml));
		//if(_usedBy == UsedBy.Info) _w.Size = _MeasureXaml(_xaml);//TODO
	}
}
