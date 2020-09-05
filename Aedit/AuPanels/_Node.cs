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
using Au.Controls.WPF;

namespace Au.Controls
{
	public partial class AuPanels
	{
		partial class _Node : ATreeBase<_Node>, IPanel
		{
			readonly AuPanels _pm;
			readonly _StackFields _stack;
			readonly _TabFields _tab;
			readonly _PanelFields _panel;
			readonly FrameworkElement _elem; //_stack.grid or _tab.tc or _panel.panel
			GridSplitter2 _splitter; //null if in tab or first in stack
			readonly PanelType _ptype;
			int _index; //index in parent stack or tab. Grid row/column index is _index*2, because at _index*2-1 is splitter if _index>0.
			GridLength _dockedSize;
			Dock _captionAt;
			_DockState _state, _savedDockState;
			_Floating _floatWindow;
			string _floatSavedRect;
			//_Flags _flags;

			static readonly Brush s_toolbarCaptionBrush = SystemColors.ControlBrush;
			const int c_minSize = 4;
			const int c_defaultSplitterSize = 4;

			class _StackFields
			{
				public Grid grid;
				public bool isVertical;
			}

			class _TabFields
			{
				public TabControl tc;
				public bool isVerticalHeader;
			}

			class _PanelFields
			{
				public _DockPanelWithBorder panel;
				public FrameworkElement content; //app sets it = any control
				public FrameworkElement caption; //TextBlock if panel not in tab, Rectangle if toolbar/document not in tab, else null
				public string name; //used by the indexer to find it, also as caption/tabitem text
				public TabItem ti;
				public bool isDocumentPlaceholder;
			}

			[Flags]
			enum _Flags { Splitter_ResizeNearest = 1 }

			/// <summary>
			/// Used to create root node when loading from XML.
			/// </summary>
			public _Node(AuPanels pm, XElement x) : this(pm, x, null, 0) { }

			/// <summary>
			/// Used to create nodes when loading from XML.
			/// </summary>
			_Node(AuPanels pm, XElement x, _Node parent, int index) {
				_pm = pm;
				_index = index;

				string tag = x.Name.LocalName;

				if (parent == null) { //the root XML element
					if (tag != "stack") throw new ArgumentException("XML root element must be 'stack'");
					_pm._rootStack = this;
				} else {
					parent.AddChild(this);
				}

				switch (tag) {
				case "stack":
					_stack = new _StackFields { isVertical = x.Attr("o") == "v" };
					_elem = _stack.grid = new Grid();
					break;
				case "tab":
					_tab = new();
					_elem = _tab.tc = new TabControl();
					break;
				case "panel":
				case "toolbar":
				case "document":
					_panel = new();
					_elem = _panel.panel = new();
					_ptype = tag[0] switch { 'p' => PanelType.Panel, 't' => PanelType.Toolbar, _ => PanelType.Document };
					_panel.name = x.Attr("name") ?? throw new ArgumentException("XML element without 'name'");
					if (_IsDocumentHost) _panel.isDocumentPlaceholder = true;
					_pm._dictContent.Add(_panel.name, this);
					break;
				default: throw new ArgumentException("unknown XML tag");
				}
				_elem.Tag = this;

				if (parent != null) {
					if (!_IsDocumentHost) {
						_savedDockState = (_DockState)(x.Attr("state", 0) & 3);
						_floatSavedRect = x.Attr("floatRect");
					}
					if (!_IsStack) x.Attr(out _captionAt, "captionAt");

					if (_ParentIsStack) {
						_dockedSize = _Util.GridLengthFromString(x.Attr("z")); //height in vertical stack or width in horizontal stack
						_AddToStack(moving: false, _index == 0 ? 0 : x.Attr("s", c_defaultSplitterSize));

						var flags = (_Flags)x.Attr("flags", 0);
						if (flags.Has(_Flags.Splitter_ResizeNearest) && _splitter != null) _splitter.ResizeNearest = true;

						if (_IsTab) _InitTabControl();
					} else {
						_AddToTab(moving: false);
					}
				}

				if (!_IsContent) { //stack or tab
					if (_ParentIsTab) throw new ArgumentException(tag + " in 'tab'");
					int i = 0;
					foreach (var e in x.Elements()) {
						new _Node(_pm, e, this, i++);
					}
					//Debug.Assert(i >= (_IsTab ? 1 : 2)); //see _AutoUpdateXml
					Debug.Assert(i >= 1); //see _AutoUpdateXml

					if (_IsTab) {
						_tab.tc.SelectedIndex = Math.Clamp(x.Attr("active", 0), 0, i - 1);
					}
				}

				//AOutput.Write(new string('\t', parent?.Level ?? 0) + _ntype, _ptype, Name, _indexInParent);

				if (parent == null) { //the root XML element
					int nVisible = 0; _Node firstHidden = null;
					foreach (var v in Descendants()) {
						//if (v._IsStack) continue;
						if (!v._IsStack) if (v._IsVisibleReally(true)) nVisible++; else firstHidden ??= v;
						if (v._savedDockState != 0) _SetState(v);
					}
					if (nVisible == 0 && firstHidden != null) { //if all non-stack hidden, unhide one, else user cannot unhide any because there are no captions to show the context menu
						firstHidden._savedDockState = 0;
						_SetState(firstHidden);
					}

					void _SetState(_Node node) {
						if (node._savedDockState != _DockState.Float) node._SetDockState(node._savedDockState);
						else _stack.grid.Dispatcher.InvokeAsync(() => node._SetDockState(node._savedDockState));
					}

					//ATimer.After(1000, _ => _Test(5));
					////ATimer.After(5000, _ => _Test(0));
					//void _Test(int margin) {
					//	foreach (var v in Descendants(true)) {
					//		//if (v._IsStack) v._stack.grid.Background = (v.Level & 3) switch { 1 => Brushes.CornflowerBlue, 2 => Brushes.Khaki, 3 => Brushes.YellowGreen, _ => Brushes.Peru };
					//		if (v._IsStack) v._stack.grid.Background = (v.Level & 3) switch { 0 => Brushes.CornflowerBlue, 1 => Brushes.Khaki, 2 => Brushes.YellowGreen, _ => Brushes.LightYellow };
					//		if(v!=this) v._elem.Margin = new Thickness(margin);
					//		if (v._splitter != null) v._splitter.Visibility = Visibility.Collapsed;
					//		//if (!v._IsStack) continue;
					//		//v._stack.grid.Background = (v.Level & 3) switch { 1 => Brushes.CornflowerBlue, 2 => Brushes.Khaki, 3 => Brushes.YellowGreen, _ => Brushes.Peru };
					//		//v._elem.Margin = new Thickness(margin);
					//	}
					//}

					//_stack.grid.PreviewMouseMove += _RootGrid_PreviewMouseMove;
				}
			}

			/// <summary>
			/// Used when moving a node, when need to create new parent (this) stack or tab for it and target.
			/// </summary>
			_Node(_Node target, bool isTab, bool verticalStack = false) {
				_pm = target._pm;

				bool targetIsRoot = !isTab && target.Parent == null;
				if (targetIsRoot) {
					_pm._rootStack = this;
				} else {
					_index = target._index;
					target.AddSibling(this, after: false);
					target.Remove();
					target._index = 0;
				}
				AddChild(target);

				if (isTab) {
					_tab = new();
					_elem = _tab.tc = new TabControl();
				} else {
					_stack = new _StackFields { isVertical = verticalStack };
					_elem = _stack.grid = new Grid();
				}
				_elem.Tag = this;

				if (targetIsRoot) {
					target._dockedSize = new GridLength(verticalStack ? target._stack.grid.ActualHeight : target._stack.grid.ActualWidth, GridUnitType.Star);
					_pm._setContainer(_stack.grid);
				} else {
					_ReplaceInStack(target);
					if (isTab) {
						_captionAt = target._captionAt;
						_InitTabControl();
					}
				}
			}

			public void Save(XmlWriter x) {
				x.WriteStartElement(_IsStack ? "stack" : (_IsTab ? "tab" : (_IsToolbarHost ? "toolbar" : (_IsDocumentHost ? "document" : "panel"))));

				if (_IsStack) x.WriteAttributeString("o", _stack.isVertical ? "v" : "h");

				if (Parent != null) {
					if (_IsContent) x.WriteAttributeString("name", _panel.name);

					if (_ParentIsStack) {
						if (!_dockedSize.IsAuto) {
							if (_IsDockedInStack) _dockedSize = _SizeDef; //update _dockedSize
							x.WriteAttributeString("z", _Util.GridLengthToString(_dockedSize));//TODO: update when splitters moved
						}

						var z = _SplitterSize;
						if (z > 0 && z != c_defaultSplitterSize) x.WriteAttributeString("s", z.ToString());

						if (_splitter != null && _splitter.ResizeNearest) x.WriteAttributeString("flags", ((int)_Flags.Splitter_ResizeNearest).ToString());
					}

					if (_IsTab) {
						int i = _tab.tc.SelectedIndex;
						if (i > 0) x.WriteAttributeString("active", i.ToString());
					}

					if (_captionAt != 0) x.WriteAttributeString("captionAt", _captionAt.ToString());
					if (!_IsDocumentHost) {
						if (_state != 0) x.WriteAttributeString("state", ((int)_state).ToString());
						_floatWindow?.Save();
						if (_floatSavedRect != null) x.WriteAttributeString("floatRect", _floatSavedRect);
					}
				}

				if (!_IsContent) foreach (var v in Children()) v.Save(x);

				x.WriteEndElement();
			}

			bool _IsStack => _stack != null;
			bool _IsTab => _tab != null;
			bool _IsContent => _panel != null; //name could be _IsPanel, but then can be confused with _IsPanelPanel
			bool _IsPanelHost => _ptype == PanelType.Panel;
			bool _IsToolbarHost => _ptype == PanelType.Toolbar;
			bool _IsDocumentHost => _ptype == PanelType.Document;
			//bool _IsPanelOrToolbarHost => _ptype == _PType.Panel || _ptype == _PType.Toolbar;
			bool _ParentIsStack => Parent?._IsStack ?? false;
			bool _ParentIsTab => Parent?._IsTab ?? false;

			/// <summary>
			/// true if this is toolbar or this is stack/tab containing only toolbars.
			/// </summary>
			bool _IsToolbarsNode => Descendants(andSelf: true).All(o => o._IsToolbarHost);

			/// <summary>
			/// Gets name of panel/toolbar/document. Else exception.
			/// </summary>
			public string Name => _panel.name;

			/// <summary>
			/// Gets the UI element of this node. It is Grid if this is stack, or TabControl if tab, else DockPanel.
			/// </summary>
			public FrameworkElement Elem => _elem;

			public override string ToString() {
				string s;
				if (_IsContent) {
					s = Name;
					if (_IsToolbarHost) s = "tb " + Name;
					return s;
				}
				if (Parent == null) return "Root stack";
				var a = Descendants().Where(o => o._IsContent).ToArray();
				s = a.Length switch
				{
					0 => "",
					1 => a[0].ToString(),
					2 => a[0].ToString() + ", " + a[1].ToString(),
					_ => a[0].ToString() + " ... " + a[^1].ToString()
				};
				return (_IsTab ? "Tabs {" : "Stack {") + s + "}";
			}

			//string _ToStringWithoutTB() {
			//	var s = ToString();
			//	if (_IsToolbarHost) s = s[3..];
			//	return s;
			//}

			#region IPanel

			/// <summary>
			/// Gets or sets content control of panel/toolbar/document. Else exception.
			/// </summary>
			public FrameworkElement Content {
				get => _panel.content;
				set {
					if (_panel.content != null) _panel.panel.Children.Remove(_panel.content);
					_panel.panel.Children.Add(_panel.content = value);
				}
			}

			/// <summary>
			/// true if visible, either floating or docked.
			/// The 'get' function returns true even if inactive tab item. The 'set' function makes tab item active.
			/// </summary>
			public bool Visible {
				get => _IsVisibleReally();
				set => _SetDockState(value ? _state & ~_DockState.Hide : _DockState.Hide); //TODO: activate tab item
			}

			/// <summary>
			/// Returns true if this node does not have hidden state and is not docked in hidden tab.
			/// </summary>
			bool _IsVisibleReally(bool useSavedState = false) {
				var state = useSavedState ? _savedDockState : _state;
				if (state.Has(_DockState.Hide)) return false;
				if (state == 0 && _ParentIsTab) return Parent._IsVisibleReally(useSavedState);
				return true;
			}

			/// <summary>
			/// true if floating and visible.
			/// false if docked or hidden.
			/// </summary>
			public bool Floating {
				get => _state == _DockState.Float;
				set => _SetDockState(value ? _DockState.Float : _state & ~_DockState.Float);
			}
			//TODO: test how when hiding tab wnen its child is floating

			public event Action<bool> VisibleChanged;

			#endregion

			static readonly Style
				s_styleL = WPF.XamlResources.Dictionary["TabItemVerticalLeft"] as Style,
				s_styleR = WPF.XamlResources.Dictionary["TabItemVerticalRight"] as Style;

			void _SetCaptionAt(Dock ca, bool firstTime = false) {
				Dock old = firstTime ? Dock.Top : _captionAt;
				_captionAt = ca;
				if (_IsTab) {
					var tc = _tab.tc;
					if (ca == tc.TabStripPlacement) return;
					if (_tab.isVerticalHeader) {
						_tab.isVerticalHeader = false;
						foreach (var v in tc.Items.Cast<TabItem>()) v.Style = null;
					}
					tc.TabStripPlacement = ca;
					_VerticalTabHeader();
				} else if (_panel.caption != null) {
					DockPanel.SetDock(_panel.caption, ca);
					if (ca == old || !_IsPanelHost) return;
					if (ca == Dock.Top || ca == Dock.Bottom) {
						if (old == Dock.Left || old == Dock.Right) _panel.caption.LayoutTransform = null;
					} else {
						_panel.caption.LayoutTransform = new RotateTransform(ca == Dock.Left ? 270d : 90d);
					}
				}
			}

			void _AddRemoveCaptionAndBorder() {
				if (!_IsContent) return;
				if (_ParentIsTab && !_state.Has(_DockState.Float)) {
					if (_panel.caption != null) {
						_panel.panel.Children.Remove(_panel.caption);
						_panel.caption = null;

						_panel.panel.BorderThickness = default;
					}
				} else {
					if (_panel.caption == null) {
						if (_IsPanelHost) {
							_panel.caption = new TextBlock {
								Text = Name,
								TextAlignment = TextAlignment.Center,
								Padding = new Thickness(2, 1, 2, 3),
								Margin = new Thickness(-0.3), //workaround for: some captions sometimes look smaller by 1 pixel, unless window background color is similar.
								Background = _pm.CaptionBrush,
								TextTrimming = TextTrimming.CharacterEllipsis
							};
						} else if (_IsToolbarHost) {
							var c = new Rectangle {
								MinHeight = 5,
								MinWidth = 5,
								Fill = s_toolbarCaptionBrush,
								//note: without Fill there are no events
							};
							c.MouseEnter += (_, _) => c.Fill = _pm.CaptionBrush;
							c.MouseLeave += (_, _) => c.Fill = s_toolbarCaptionBrush;
							_panel.caption = c;
						} else { //document
							var c = new Rectangle {
								MinHeight = 5,
								MinWidth = 5,
								Fill = _pm.CaptionBrush,
							};
							_panel.caption = c;

							_panel.panel.LastChildFill = false;
							bool hasDoc = false;
							c.SizeChanged += (_, e) => {
								bool has = _panel.panel.Children.Count > 1;
								if (has != hasDoc) _panel.panel.LastChildFill = hasDoc = has;
							};
						}
						_panel.panel.Children.Insert(0, _panel.caption);
						_SetCaptionAt(_captionAt, true);
						_panel.caption.ContextMenuOpening += _CaptionContextMenu;
						_panel.caption.MouseDown += _OnMouseDown;

						if (_pm.BorderBrush != null && !_IsToolbarHost) {
							_panel.panel.BorderBrush = _pm.BorderBrush;
							_panel.panel.BorderThickness = new Thickness(1);
						}
					}
				}
			}
		}
	}
}

