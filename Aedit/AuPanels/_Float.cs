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

namespace Au.Controls
{
	public partial class AuPanels
	{
		partial class _Node
		{
			class _Float : Window
			{
				_Node _node;

				public _Float(_Node node) {
					_node = node;

					var pa = _node.Parent;
					var c = _node.Elem;
					var c2 = pa._IsTab ? pa.Elem : c;
					var rect = c2.RectToScreen_();
					var style = WS.THICKFRAME | WS.POPUP | WS.CLIPCHILDREN;
					var estyle = WS2.TOOLWINDOW | WS2.WINDOWEDGE; if (_node._IsToolbar) estyle |= WS2.NOACTIVATE;
					ADpi.AdjustWindowRectEx(c2, ref rect, style, estyle);
					base.Loaded += (_, _) => {
						var w = this.Hwnd();
						w.SetStyle(style);
						if (_node._IsToolbar) w.SetExStyle(estyle);
						w.MoveLL(rect);
					};

					if (pa._IsTab) {
						var ti = c.Parent as TabItem;
						ti.Content = null;
						_node._AddRemoveCaption();
					} else {
						var g = c.Parent as Grid;
						g.Children.Remove(c);
					}
					base.Content = c;

					base.WindowStartupLocation = WindowStartupLocation.Manual;
					base.WindowStyle = WindowStyle.ToolWindow;
					base.ShowInTaskbar = false;
					base.ShowActivated = false;
					base.Owner = _node._pm._ContainerWindow;

					//AWnd.More.SavedRect.Restore(this)
				}

				protected override void OnInitialized(EventArgs e) {
					AOutput.Write("OnInitialized", this.Hwnd());
					base.OnInitialized(e);
				}

				protected override void OnSourceInitialized(EventArgs e) {
					AOutput.Write("OnSourceInitialized", this.Hwnd());
					base.OnSourceInitialized(e);
				}

				public _DockTarget Drag(POINT p) {

					bool canDock = false;
					_DockTarget target = null;
					var w = this.Hwnd();
					RECT r = w.Rect;
					POINT offs = (p.x - r.left, p.y - r.top);
					bool ok = ADragDrop.SimpleDragDrop(w, MButtons.Left, d => {
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
}
