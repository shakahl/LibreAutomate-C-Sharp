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
using System.Globalization;

namespace Au.Controls
{
	public partial class AuPanels
	{
		partial class _Node
		{
			[Flags]
			enum _DockState { Hide = 1, Float = 2, }

			void _CaptionContextMenu(object sender, ContextMenuEventArgs e) {
				//AOutput.Write(sender, e.Source, e.OriginalSource);
				if (!_IsGoodMouseEvent(sender, e, out var target)) return;
				e.Handled = true;
				if (_IsDocumentHost && _panel.isDocumentPlaceholder) return;
				var m = new AContextMenu();
				_DockStateItem(_DockState.Hide, target._IsDocumentHost ? "Close" : "Hide", "M-click");

				if (_state.Has(_DockState.Float)) _DockStateItem(0, "Dock", "D-click, Alt+drag");
				else _DockStateItem(_DockState.Float, "Float", "D-click, drag");

				using (m.Submenu("Caption At")) {
					_CaptionAtItem(Dock.Left);
					_CaptionAtItem(Dock.Top);
					_CaptionAtItem(Dock.Right);
					_CaptionAtItem(Dock.Bottom);
				}
				target._ContextMenu_Move(m);
				_ShowSubmenus();
				m.IsOpen = true;

				void _DockStateItem(_DockState state, string s1, string s2) {
					m[s1] = o => target._SetDockState(state);
					m.LastItem.InputGestureText = s2;
				}

				void _CaptionAtItem(Dock ca) {
					m[ca.ToString()] = o => _SetCaptionAt(ca);
					if (ca == _captionAt) m.LastItem.IsChecked = true;
				}

				void _ShowSubmenus() {
					var a = new List<_Node>();
					foreach (var v in RootAncestor.Descendants()) {
						if (v._IsStack || !v._state.Has(_DockState.Hide)) continue;
						if (v._IsTab && v.Children().All(o => o._state.Has(_DockState.Hide))) continue;
						a.Add(v);
					}
					if (a.Count == 0) return;
					m.Separator();
					a.Sort((x, y) => {
						if (x._IsToolbarHost && !y._IsToolbarHost) return -1;
						if (y._IsToolbarHost && !x._IsToolbarHost) return 1;
						return string.Compare(x.ToString(), y.ToString(), true);
					});
					using (m.Submenu("Show")) {
						int i = 0;
						foreach (var v in a) {
							if (i > 0 && a[i - 1]._IsToolbarHost != a[i]._IsToolbarHost) m.Separator();
							i++;
							m[v.ToString()] = o => v._SetDockState(v._state & ~_DockState.Hide);
						}
#if DEBUG
						if (a.Count > 1) {
							m.Separator();
							m["Show All (debug)"] = _ => {
								foreach (var v in a) v._SetDockState(v._state & ~_DockState.Hide);
							};
						}
#endif
					}
				}
			}

			protected private void _OnMouseDown(object sender, MouseButtonEventArgs e) {
				switch (e.ChangedButton) { case MouseButton.Left: case MouseButton.Middle: break; default: return; }
				if (!_IsGoodMouseEvent(sender, e, out var target)) return;
				if (target._IsStack) {
					if (!(e.ChangedButton == MouseButton.Left && e.ClickCount == 1 && Keyboard.Modifiers == ModifierKeys.Alt)) return;
					if (target.Parent == null) { e.Handled = true; return; }
				}
				e.Handled = true;
				if (e.ChangedButton == MouseButton.Left) {
					if (e.ClickCount == 1) {
						e.Handled = false; //if tab item, let select it
						ATimer.After(1, _ => { //Dispatcher.InvokeAsync does not work
							POINT p = AMouse.XY;
							if (Api.DragDetect(_elem.Hwnd(), p)) {
								target._SetDockState(_DockState.Float, onDrag: true);
								target._floatWindow?.Drag(p);
							}
						});
					} else if (e.ClickCount == 2) {
						target._SetDockState(target._state ^ _DockState.Float);
					}
				} else {
					//TODO: if document, close. Or let the app on event close it. The same from menu. Let _SetDockState do it.
					target._SetDockState(_DockState.Hide);
				}
			}

			bool _IsGoodMouseEvent(object sender, RoutedEventArgs e, out _Node target) {
				target = null;
				if (e.Source == sender) target = sender == _splitter ? Parent : this;
				else if (e.Source is TabItem ti && ti.Parent == sender) target = _NodeFromTabItem(ti);
				else return false;
				return true;
			}

			void _SetDockState(_DockState state, bool onDrag = false) {
				if (state == _DockState.Hide) state |= _state & _DockState.Float;
				//AOutput.Write(this, state);//TODO
				if (state == _state) return;

				var oldState = _state;
				_state = state;

				if (oldState == _DockState.Float) {
					_floatWindow?.Close();
					//_floatWindow sets _floatWindow=null when closing
				} else if (state == _DockState.Float) {
					_floatWindow = new _Floating(this, onDrag);
					//ctor uses docked rect
				}

				if (state == 0) { //dock; was hidden or floating
					_AddRemoveCaptionAndBorder();
					if (_ParentIsTab) _ShowHideInTab(true);
					else _ShowHideInStack(true);
				} else {
					if (oldState == 0) { //was docked; now hide or float
						if (_ParentIsTab) _ShowHideInTab(false);
						else _ShowHideInStack(false);
					}

					_AddRemoveCaptionAndBorder();

					if (state == _DockState.Float) {
						_floatWindow.Content = _elem;
						_floatWindow.ShowIfOwnerVisible();
					}
				}

				if ((state ^ oldState).Has(_DockState.Hide)) VisibleChanged?.Invoke(oldState.Has(_DockState.Hide));
			}
			//TODO: redraw grid and float. WPF sometimes does not draw correctly.

			void _ContextMenu_Move(AContextMenu m) {
				using (m.Submenu("Move To")) {
					string sThis = ToString();
					foreach (var target in RootAncestor.Descendants(andSelf: true)) {
						if (target._ParentIsTab) continue;
						if (target.Ancestors(andSelf: true).Contains(this)) continue;
						if (_IsDocumentHost && !_panel.isDocumentPlaceholder) { //TODO: test, maybe add more filters
							if (!target._IsDocumentHost) continue;
						}
						if (target._IsDocumentHost && !target._panel.isDocumentPlaceholder) { //TODO: test, maybe add more filters
							if (!_IsDocumentHost) continue;
						}
						string sTarget = target.ToString();
						using (m.Submenu(new string(' ', target.Level * 4) + sTarget + (target._state switch { 0 => null, _DockState.Float => " (floating)", _ => " (hidden)" }))) {
							if (target._IsStack || (target._IsTab && _IsContent)) {
								int i = 0;
								foreach (var u in target.Children()) {
									m[i++ == 0 ? "First" : ($"Before '{u.ToString()}'")] = o => _MoveTo(u, _HowToMove.BeforeTarget);
								}
								m["Last"] = o => _MoveTo(target.LastChild, _HowToMove.AfterTarget);
								m.Separator();
							}
							if (target._ParentIsStack /*|| (v._ParentIsTab && _IsContent)*/) {
								m[$"Before '{sTarget}'"] = o => _MoveTo(target, _HowToMove.BeforeTarget);
								m[$"After '{sTarget}'"] = o => _MoveTo(target, _HowToMove.AfterTarget);
								m.Separator();
							}
							if (target._IsContent && _IsContent) {
								m.Add($"Create tabs and add '{sThis}' as:").IsEnabled = false;
								m[$"- First tab (before '{sTarget}')"] = o => _MoveTo(target, _HowToMove.FirstInNewTab);
								m[$"- Last tab (after '{sTarget}')"] = o => _MoveTo(target, _HowToMove.LastInNewTab);
								m.Separator();
							}
							m.Add($"Create stack and add '{sThis}' at:").IsEnabled = false;
							m["- Left"] = o => _MoveTo(target, _HowToMove.NewStack, Dock.Left);
							m["- Right"] = o => _MoveTo(target, _HowToMove.NewStack, Dock.Right);
							m["- Top"] = o => _MoveTo(target, _HowToMove.NewStack, Dock.Top);
							m["- Bottom"] = o => _MoveTo(target, _HowToMove.NewStack, Dock.Bottom);
						}
					}
				}
				//#if DEBUG
				//				m["Debug test move"] = o => _MoveTo(RootAncestor, _HowToMove.NewStack, Dock.Bottom);
				//#endif
			}

			enum _HowToMove
			{
				BeforeTarget, //before target in parent stack or tab
				AfterTarget, //after target in parent stack or tab
				FirstInNewTab, //create new tab in place of target; add this (first) and target to it
				LastInNewTab, //create new tab in place of target; add target and this (last) to it
				NewStack, //create new stack in place of target; add target and this to it; use dock to set orientation of new stack and index ot this and target
			}

			void _MoveTo(_Node target, _HowToMove how, Dock dock = default) {
				if (target == this) return;
				if (_state != 0) _SetDockState(0);
				if (target._state != 0 && how >= _HowToMove.FirstInNewTab) target._SetDockState(0);

				bool after = how == _HowToMove.AfterTarget;
				var targetParent = target.Parent;
				var oldParent = Parent;
				_RemoveFromParent();

				switch (how) {
				case _HowToMove.NewStack:
					targetParent = new _Node(target, isTab: false, verticalStack: dock == Dock.Top || dock == Dock.Bottom);
					target._AddToStack(moving: false, c_defaultSplitterSize);
					after = dock == Dock.Right || dock == Dock.Bottom;
					break;
				case _HowToMove.FirstInNewTab:
				case _HowToMove.LastInNewTab:
					var sp = target._splitter;
					targetParent = new _Node(target, isTab: true);
					target._AddToTab(moving: false);
					after = how == _HowToMove.LastInNewTab;
					break;
					//default: //before/after target in parent stack or tab
				}

				target.AddSibling(this, after);
				_index = target._index + (after ? 1 : 0);
				if (targetParent._IsTab) {
					_AddToTab(moving: true);
					_AddRemoveCaptionAndBorder();
					targetParent._tab.tc.SelectedIndex = _index;
				} else {
					if (_dockedSize.IsAuto && !_IsToolbarsNode) _dockedSize = new GridLength(100, GridUnitType.Star);
					_AddToStack(moving: true, c_defaultSplitterSize);
				}

#if false //debug print
				int i = 0;
				foreach (var v in Parent.Children()) {
					AOutput.Write(i++, v._index, v);
					if (Parent._IsStack) {
						if (v._splitter != null) AOutput.Write("splitter", _RC(v._splitter));
						AOutput.Write("elem    ", _RC(v._elem), v._dockedSize, v._SizeDef);
						int _RC(FrameworkElement e) => Parent._stack.isVertical ? Grid.GetRow(e) : Grid.GetColumn(e);
					}
				}
#endif

				int n = oldParent.Count;
				if (n == 0) {
					oldParent._RemoveFromParent();
				} else if (n == 1) {
					oldParent.FirstChild._MoveTo(oldParent, _HowToMove.BeforeTarget);
				} else if (oldParent._IsTab && Parent != oldParent) {
					oldParent._VerticalTabHeader(onMove: true);
				}

				//TODO: if fixed size>0.8 of stack size, make star.

				//TODO: support only document placeholder(s). For real documents use nested AuPanels or simple TabControl.
			}

			void _RemoveFromParent() {
				if (Parent._IsStack) {
					_RemoveGridRowCol(_elem);
					_RemoveSplitter();
					if (_index == 0) Next?._RemoveSplitter();
				} else {
					var ti = _elem.Parent as TabItem;
					ti.Content = null;
					Parent._tab.tc.Items.Remove(ti);
				}
				_ShiftSiblingIndices(-1);
				Remove();
			}
		}
	}
}
