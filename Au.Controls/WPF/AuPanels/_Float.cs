using Au.Types;
using Au.Util;
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
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		class _Float : Window
		{
			_Dragable _d;

			public _Float(_Dragable d) {
				_d = d;

				var parentTab = _d._parent as _Tab;
				var c = _d.Elem;
				var c2 = parentTab?.Elem ?? c;
				var rect = c2.RectToScreen_();
				var style = WS.THICKFRAME | WS.CLIPCHILDREN | WS.CLIPSIBLINGS | WS.POPUP;
				var estyle = WS2.TOOLWINDOW | WS2.WINDOWEDGE; if (_d.IsToolbar) estyle |= WS2.NOACTIVATE;
				ADpi.AdjustWindowRectEx(c2, ref rect, style, estyle);
				base.Loaded += (_, _) => {
					var w = this.Hwnd();
					w.SetStyle(style);
					if (_d.IsToolbar) w.SetExStyle(estyle);
					w.MoveLL(rect);
				};

				if (parentTab != null) {
					var pa = c.Parent as TabItem;
					pa.Content = null;
					var panel = _d as _Panel;
					panel.AddRemoveCaption();
				} else {
					var pa = c.Parent as Grid;
					pa.Children.Remove(c);
				}
				base.Content = c;

				base.WindowStartupLocation = WindowStartupLocation.Manual;
				base.WindowStyle = WindowStyle.ToolWindow;
				base.ShowInTaskbar = false;
				base.ShowActivated = false;
				base.Owner = _d._pm.ContainerWindow;
			}

			public _DockTarget Drag(POINT p) {

				bool canDock = false;
				_DockTarget target = null;
				var w = this.Hwnd();
				RECT r = w.Rect;
				POINT offs = (p.x - r.left, p.y - r.top);
				bool ok = ADragDrop.SimpleDragDrop(w, MButtons.Left, d =>
				{
					if (d.Msg.message != Api.WM_MOUSEMOVE) return;

					p = AMouse.XY;
					w.MoveLL(p.x - offs.x, p.y - offs.y);

					if (!canDock && AKeys.UI.IsAlt) {
						canDock = true;
						//w.SetTransparency(true, 128);
						//_dockIndic = new _DockIndicator(_manager, this);
						//_dockIndic.Show(this);
					}
					//if (canDock) _dockIndic.OnFloatMoved(_manager.PointToClient(p));
				});

				//if (canDock) {
				//	w.SetTransparency(false);
				//	_dockIndic.Close();
				//	if (ok) {
				//		target = _dockIndic.OnFloatDropped();
				//	}
				//	_dockIndic = null;
				//}

				return target;
			}
		}

		class _DockTarget
		{
			public _Node node;
			public Dock dock;
			//public bool tabAfter;
			public TabItem tabItem;
		}


	}
}
