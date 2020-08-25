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
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Au.Controls
{
	public partial class AuPanels
	{
		partial class _Node
		{
			[Flags]
			enum _DockState { Float = 1, Hide = 2 }

			bool _IsFloating => _float?.dockState.Has(_DockState.Float) ?? false;
			bool _IsHidden => _float?.dockState.Has(_DockState.Hide) ?? false;

			void _FloatableContextMenu(object sender, ContextMenuEventArgs e) {
				//AOutput.Write(sender, e.Source, e.OriginalSource);
				if (!_IsGoodMouseEvent(sender, e, out var target, out _)) return;
				e.Handled = true;
				var m = new AContextMenu();
				if (_IsFloating) _DockStateItem(0, "D-click, Alt+drag"); else _DockStateItem(_DockState.Float, "D-click, drag");
				_DockStateItem(_DockState.Hide, "M-click");
				using (m.Submenu("Caption At")) {
					_CaptionAtItem(Dock.Left);
					_CaptionAtItem(Dock.Top);
					_CaptionAtItem(Dock.Right);
					_CaptionAtItem(Dock.Bottom);
				}
				m.Separator();
				_ShowSubmenu(false);
				_ShowSubmenu(true);
				m.IsOpen = true;

				void _DockStateItem(_DockState state, string s2) {
					m[state.ToString()] = o => target._SetDockState(state);
					m.LastItem.InputGestureText = s2;
				}

				void _CaptionAtItem(Dock ca) {
					m[ca.ToString()] = o => _SetCaptionAt(ca);
					if (ca == _float.captionAt) m.LastItem.IsChecked = true;
				}

				void _ShowSubmenu(bool toolbar) {
					using (m.Submenu(toolbar ? "Show Toolbar" : "Show Panel")) {
						int n = 0;
						foreach (var p in _pm._aContent) {
							if (!p._IsHidden || p._IsToolbar != toolbar || p._IsDocument) continue;
							m[p.Name] = o => p._SetDockState(p._float.dockState & ~_DockState.Hide);
							n++;
						}
						if (n == 0) m.Add("All visible").IsEnabled = false;
					}
				}
			}

			protected private void _OnMouseDown(object sender, MouseButtonEventArgs e) {
				switch (e.ChangedButton) { case MouseButton.Middle: case MouseButton.Left: break; default: return; }
				if (!_IsGoodMouseEvent(sender, e, out var target, out var tabItem)) return;
				e.Handled = true;
				if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1) {
					if (tabItem != null) {
						tabItem.IsSelected = true;
					}
					POINT p = AMouse.XY;
					if (Api.DragDetect(_elem.Hwnd(), p)) {
						target._SetDockState(_DockState.Float);
						target._float.flo?.Drag(p);
					}
				} else {
					target._SetDockState(target._float.dockState ^ (e.ChangedButton == MouseButton.Middle ? _DockState.Hide : _DockState.Float));
				}
			}

			bool _IsGoodMouseEvent(object sender, RoutedEventArgs e, out _Node target, out TabItem tabItem) {
				target = null; tabItem = null;
				if (e.Source == sender) target = this;
				else if (e.Source is TabItem ti && ti.Parent == sender) target = _NodeFromTabItem(tabItem = ti);
				else return false;
				return true;
			}

			static _Node _NodeFromTabItem(TabItem ti) => ti.Tag as _Node;

			void _SetDockState(_DockState state) {
				//AOutput.Write(this, state);//TODO
				if (state == _float.dockState) return;
				var pa = Parent;

				var tabItem = pa._IsTab ? _elem.Parent as TabItem : null;

				var oldState = _float.dockState; _float.dockState = state;

				if (state.Has(_DockState.Hide)) {
					if (!_IsHidden) {

					}
				} else if (state.Has(_DockState.Float)) {
					//TODO: if hidden...

					_float.flo = new _Float(this);
					_float.flo.Show();
				} else {

				}

				if (oldState == 0) { //was docked; now hide or float
					if (pa._IsTab) pa._HideTabItem(tabItem);
					else _HideThisInStack();
				}
			}

			//this is any in stack. Hides this, and ancestors if need.
			void _HideThisInStack() {
				AOutput.Write(this);
				var g = Parent._stack.grid;
				if (_splitter != null && _splitter.Parent == g) g.Children.Remove(_splitter);
				if (_elem.Parent == g) g.Children.Remove(_elem);

				//if (_splitter != null) _splitter.Visibility = Visibility.Collapsed;
				var k = new _RowCol(this);
				////k.Visible = false;
				//if (_elem.Parent == g) _elem.Visibility = Visibility.Collapsed;
				k.SizeDef = new GridLength(0, GridUnitType.Auto);
			}

			//this is tab. Hides its tabitem. If need, hides this in stack, and ancestors if need.
			void _HideTabItem(TabItem ti) {
				var tc = _tab.tc;
				if (ti == tc.SelectedItem) {
					var a = tc.Items;
					TabItem tiSelect = null;
					int iSel = tc.SelectedIndex, n = a.Count;
					for (int i = iSel + 1; i < n; i++) if (a[i] is TabItem v && v.Visibility == Visibility.Visible) { tiSelect = v; break; }
					if (tiSelect == null) for (int i = iSel; --i >= 0;) if (a[i] is TabItem v && v.Visibility == Visibility.Visible) { tiSelect = v; break; }
					if (tiSelect != null) {
						tc.SelectedItem = tiSelect;
					} else {
						tc.Visibility = Visibility.Collapsed;
						_HideThisInStack();
					}

				}
				ti.Visibility = Visibility.Collapsed;
			}

		}
	}
}
