using Au.Types;
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
using System.Windows.Input;

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		/// <summary>
		/// Base of _Panel and _Tab.
		/// Has caption and can float.
		/// </summary>
		abstract class _Dragable : _Node
		{
			Dock _captionAt;
			_DockState _dockState;
			_Float _float;

			protected static readonly Brush s_captionBrush = Brushes.LightSteelBlue;

			public _Dragable(AuPanels pm, _Node parent, XElement x) : base(pm, parent) {
				x.Attr(out _captionAt, "captionAt");
				//_dockState = (_DockState)(x.Attr("state", 0) & 3); //TODO
			}

			public virtual Dock CaptionAt {
				get => _captionAt;
				set => _captionAt = value;
			}

			public _DockState DockState => _dockState;
			public bool IsFloating => _dockState.Has(_DockState.Float);
			public bool IsHidden => _dockState.Has(_DockState.Hide);


			/// <summary>
			/// Writes attributes if need.
			/// </summary>
			protected override void _SaveAttributes(XmlWriter x) {
				base._SaveAttributes(x);
				if (_captionAt != default) x.WriteAttributeString("captionAt", _captionAt.ToString());
				if (_dockState != default) x.WriteAttributeString("state", ((int)_dockState).ToString());
			}

			bool _IsGoodMouseEvent(object sender, RoutedEventArgs e, out _Dragable target, out TabItem tabItem) {
				target = null; tabItem = null;
				if (e.Source == sender) target = this;
				else if (e.Source is TabItem ti && ti.Parent == sender) target = _Tab.PanelFromTabItem(tabItem = ti);
				else return false;
				return true;
			}

			protected private void _ContextMenu(object sender, ContextMenuEventArgs e) {
				//AOutput.Write(sender, e.Source, e.OriginalSource);
				if (!_IsGoodMouseEvent(sender, e, out var target, out _)) return;
				e.Handled = true;
				var m = new AContextMenu();
				if (IsFloating) _DockStateItem(0, "D-click, Alt+drag"); else _DockStateItem(_DockState.Float, "D-click, drag");
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
					m[ca.ToString()] = o => CaptionAt = ca;
					if (ca == _captionAt) m.LastItem.IsChecked = true;
				}

				void _ShowSubmenu(bool toolbar) {
					using (m.Submenu(toolbar ? "Show Toolbar" : "Show Panel")) {
						int n = 0;
						foreach (var p in _pm._aPanel) {
							if (!p.IsHidden || p.IsToolbar != toolbar || p.IsDocument) continue;
							m[p.Name] = o => p._SetDockState(p.DockState & ~_DockState.Hide);
							n++;
						}
						if (n == 0) m.Add("All visible").IsEnabled = false;
					}
				}
			}

			void _SetDockState(_DockState state) {
				AOutput.Write(this, state);//TODO
				if (state == _dockState) return;
				_OnSetDockState(true, state, _dockState);

				var tabItem = _parent is _Tab ? this.Elem.Parent as TabItem : null;

				var oldState = _dockState; _dockState = state;

				if (state.Has(_DockState.Hide)) {
					if (!IsHidden) {

					}
				} else if (state.Has(_DockState.Float)) {
					//TODO: if hidden...

					_float = new _Float(this);
					_float.Show();
				} else {

				}

				if (oldState == 0) { //was docked; now hide or float
					if (_parent is _Tab tab) tab.HideTabItem(tabItem);
					else _stack.HideNode(this);
				}
				//_OnSetDockState(false, state, oldState);
			}

			protected abstract void _OnSetDockState(bool before, _DockState state, _DockState oldState);

			protected private void _OnMouseDown(object sender, MouseButtonEventArgs e) {
				switch (e.ChangedButton) { case MouseButton.Middle: case MouseButton.Left: break; default: return; }
				if (!_IsGoodMouseEvent(sender, e, out var target, out var tabItem)) return;
				e.Handled = true;
				if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1) {
					if (tabItem != null) {
						tabItem.IsSelected = true;
					}
					POINT p = AMouse.XY;
					if (Api.DragDetect(Elem.Hwnd(), p)) {
						target._SetDockState(_DockState.Float);
						target._float?.Drag(p);
					}
				} else {
					target._SetDockState(target._dockState ^ (e.ChangedButton == MouseButton.Middle ? _DockState.Hide : _DockState.Float));
				}
			}
		}

		[Flags]
		enum _DockState { Float = 1, Hide = 2 }
	}
}
