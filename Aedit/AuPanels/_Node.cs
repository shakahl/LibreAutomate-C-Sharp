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

namespace Au.Controls
{
	public partial class AuPanels
	{
		partial class _Node : Util.ATreeBase<_Node>, IPanel
		{
			readonly AuPanels _pm;
			readonly _StackFields _stack;
			readonly _TabFields _tab;
			readonly _PanelFields _panel;
			readonly _FloatableFields _float; //_tab, _panel or null
			readonly FrameworkElement _elem; //_stack.grid or _tab.tc or _panel.panel
			GridSplitter _splitter; //null if in tab or first in stack
			readonly _PType _ptype;
			int _indexInParent;
			double _dockedSize;
			GridUnitType _dockedUnit;

			class _StackFields
			{
				public Grid grid;
				public bool isVertical;
			}

			class _TabFields : _FloatableFields
			{
				public TabControl tc;
				public bool isVerticalHeader;
			}

			class _PanelFields : _FloatableFields
			{
				public DockPanel panel;
				public FrameworkElement content; //app sets it = any control
				public FrameworkElement caption; //TextBlock if panel not in tab, Rectangle if toolbar not in tab, else null
				public string name; //used by the indexer to find it, also as caption/tabitem text
				public TabItem ti;
			}

			abstract class _FloatableFields
			{
				public Dock captionAt;
				public _DockState dockState;
				public _Float flo;
				public RECT rect;
				public string savedRect;
			}

			public _Node(AuPanels pm, XElement x) : this(pm, x, null, 0) { }

			public _Node(AuPanels pm, XElement x, _Node parent, int index) {
				_pm = pm;
				_indexInParent = index;

				string tag = x.Name.LocalName;
				Grid parentGrid = null;
				TabControl parentTab = null;

				if (parent == null) { //the root XML element
					if (tag != "stack") throw new ArgumentException("XML root element must be 'stack'");
					_pm._rootStack = this;
				} else {
					parent.AddChild(this);
					if (parent._IsTab) {
						parentTab = parent._tab.tc;
					} else {
						parentGrid = parent._stack.grid;
					}
				}

				switch (tag) {
				case "stack":
					_stack = new _StackFields { isVertical = x.Attr("o") == "v" };
					_elem = _stack.grid = new Grid();
					break;
				case "tab":
					_float = _tab = new();
					_elem = _tab.tc = new TabControl();
					break;
				case "panel":
				case "toolbar":
				case "document":
					_panel = new();
					_elem = _panel.panel = new DockPanel();
					_ptype = tag[0] switch { 'p' => _PType.Panel, 't' => _PType.Toolbar, _ => _PType.Document };
					if (_IsDocument) {
						_panel.name = "Document";
					} else {
						_panel.name = x.Attr("name") ?? throw new ArgumentException("XML element without 'name'");
						_float = _panel;
					}
					_pm._aContent.Add(this);
					break;
				default: throw new ArgumentException("unknown XML tag");
				}

				if (parentGrid != null) {
					bool inVerticalStack = parent._stack.isVertical;

					//set height in vertical stack or width in horizontal stack
					var k = _Util.GridLengthFromString(x.Attr("w"));
					if (inVerticalStack) {
						parentGrid.RowDefinitions.Add(new RowDefinition { Height = k });
					} else {
						parentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = k });
					}

					//splitter
					if (_indexInParent > 0) {
						_splitter = new GridSplitter2 { ResizeBehavior = GridResizeBehavior.PreviousAndCurrent };
						if (inVerticalStack) { //horz splitter
							_splitter.ResizeDirection = GridResizeDirection.Rows;
							_splitter.VerticalAlignment = VerticalAlignment.Top;
							_splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
						} else { //vert splitter
							_splitter.ResizeDirection = GridResizeDirection.Columns;
							_splitter.HorizontalAlignment = HorizontalAlignment.Left;
							//default stretch
						}
						_SplitterSize = x.Attr("s", c_defaultSplitterWidth);
						if (inVerticalStack) Grid.SetRow(_splitter, _indexInParent); else Grid.SetColumn(_splitter, _indexInParent);
						_splitter.ContextMenuOpening += _SplitterContextMenu;
						parentGrid.Children.Add(_splitter);
					}

					if (inVerticalStack) Grid.SetRow(_elem, _indexInParent); else Grid.SetColumn(_elem, _indexInParent);
					parentGrid.Children.Add(_elem);

					if (_CanFloat) {
						x.Attr(out _float.captionAt, "captionAt");
						//_dockState = (_DockState)(x.Attr("state", 0) & 3); //TODO
						if (!_IsTab) _AddRemoveCaption();
						_float.savedRect = x.Attr("floatRect");
					}

					if (_IsTab) {
						var tc = _tab.tc;
						tc.Padding = default;
						tc.TabStripPlacement = _float.captionAt;
						tc.SizeChanged += (_, e) => {
							switch (tc.TabStripPlacement) { case Dock.Top: case Dock.Bottom: return; }
							bool bigger = e.NewSize.Height > e.PreviousSize.Height;
							if (bigger != _tab.isVerticalHeader) _VerticalTabHeader(e.NewSize.Height);
						};
						tc.ContextMenuOpening += _FloatableContextMenu;
						tc.PreviewMouseDown += _OnMouseDown;
					}
				} else if (parentTab != null) {
					var ti = new TabItem { Header = _panel.name, Content = _elem, Tag = this };
					_panel.ti = ti;
					parentTab.Items.Add(ti);
				}

				if (!_IsContent) {
					if (parentTab != null) throw new ArgumentException(tag + " in 'tab'");
					int i = 0;
					foreach (var e in x.Elements()) {
						new _Node(_pm, e, this, i++);
					}
					Debug.Assert(i >= 2); //see _AutoUpdateXml

					if (_IsTab) {
						_tab.tc.SelectedIndex = Math.Clamp(x.Attr("active", 0), 0, i - 1);
					}
				}

				//AOutput.Write(new string('\t', parent?.Level ?? 0) + _ntype, _ptype, Name, _indexInParent);
			}

			public void Save(XmlWriter x) {
				x.WriteStartElement(_IsStack ? "stack" : (_IsTab ? "tab" : (_IsPanel ? "panel" : (_IsToolbar ? "toolbar" : "document"))));

				if (_IsStack) x.WriteAttributeString("o", _stack.isVertical ? "v" : "h");

				if (Parent?._IsStack ?? false) {
					var k = new _RowCol(this);
					var w = k.SizeDef;
					if (!w.IsAuto) {
						//if (k.Visible) //TODO
						w = new GridLength(k.SizeNow, w.GridUnitType);
						x.WriteAttributeString("w", _Util.GridLengthToString(w));
					}

					var s = _SplitterSize;
					if (s > 0 && s != c_defaultSplitterWidth) x.WriteAttributeString("s", s.ToString());
				}

				if (_IsPanel) {
					if (!_IsDocument) x.WriteAttributeString("name", _panel.name);
				} else if (_IsTab) {
					int i = _tab.tc.SelectedIndex;
					if (i > 0) x.WriteAttributeString("active", i.ToString());
				}

				if (_CanFloat) {
					if (_float.captionAt != default) x.WriteAttributeString("captionAt", _float.captionAt.ToString());
					if (_float.dockState != default) x.WriteAttributeString("state", ((int)_float.dockState).ToString());
					//TODO: if _IsFloating, update _float.savedRect now.
					if (_float.savedRect != null) x.WriteAttributeString("floatRect", _float.savedRect);
				}

				if (!_IsPanel) foreach (var v in Children()) v.Save(x);

				x.WriteEndElement();
			}

			/// <summary>_Node type.</summary>
			enum _NType { Stack, Tab, Content }

			/// <summary>_Node panel type.</summary>
			enum _PType { None, Panel, Toolbar, Document }

			bool _IsStack => _stack != null;
			bool _IsTab => _tab != null;
			bool _IsContent => _panel != null;
			bool _IsPanel => _ptype == _PType.Panel;
			bool _IsToolbar => _ptype == _PType.Toolbar;
			bool _IsDocument => _ptype == _PType.Document;
			bool _IsPanelOrToolbar => _ptype == _PType.Panel || _ptype == _PType.Toolbar;
			bool _CanFloat => _float != null;

			/// <summary>
			/// Gets name of panel/toolbar/document. Else exception.
			/// </summary>
			public string Name => _panel.name;

			/// <summary>
			/// Gets the UI element of this node. It is Grid if this is stack, or TabControl if tab, else DockPanel.
			/// </summary>
			public FrameworkElement Elem => _elem;

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

			const int c_defaultSplitterWidth = 5;

			/// <summary>
			/// Gets or sets actual height of <see cref="_splitter"/> in vertical stack or width in horizontal stack.
			/// </summary>
			int _SplitterSize {
				get {
					if (_splitter == null) return 0;
					return (Parent._stack.isVertical ? _splitter.ActualHeight : _splitter.ActualWidth).ToInt();
				}
				set {
					if (_splitter == null) return;
					double d = value;
					if (Parent._stack.isVertical) {
						_splitter.Height = d;
						_elem.Margin = new Thickness(0, d, 0, 0);
					} else {
						_splitter.Width = d;
						_elem.Margin = new Thickness(d, 0, 0, 0);
					}
				}
			}

			static Style
				s_styleL = WPF.XamlResources.Dictionary["TabItemVerticalLeft"] as Style,
				s_styleR = WPF.XamlResources.Dictionary["TabItemVerticalRight"] as Style;

			void _VerticalTabHeader(double height) {
				var tc = _tab.tc;
				var tabs = tc.Items.Cast<TabItem>();
				bool vert2 = _tab.isVerticalHeader ? tabs.Sum(o => o.ActualHeight) <= height - 15 : tabs.Sum(o => o.ActualWidth) <= height;
				if (vert2 == _tab.isVerticalHeader) return;
				_tab.isVerticalHeader = vert2;
				var dock = tc.TabStripPlacement;
				foreach (var v in tabs) v.Style = vert2 ? (dock == Dock.Left ? s_styleL : s_styleR) : null;
			}

			void _SetCaptionAt(Dock ca, bool firstTime = false) {
				Dock old = firstTime ? Dock.Top : _float.captionAt;
				_float.captionAt = ca;
				if (_IsTab) {
					var tc = _tab.tc;
					if (ca == tc.TabStripPlacement) return;
					bool sides = ca == Dock.Left || ca == Dock.Right;
					if (_tab.isVerticalHeader) {
						_tab.isVerticalHeader = false;
						foreach (var v in tc.Items.Cast<TabItem>()) v.Style = null;
					}
					tc.TabStripPlacement = ca;
					if (sides) _VerticalTabHeader(tc.ActualHeight);
				} else if (_panel.caption != null) {
					DockPanel.SetDock(_panel.caption, ca);
					if (ca == old || !_IsPanel) return;
					if (ca == Dock.Top || ca == Dock.Bottom) {
						if (old == Dock.Left || old == Dock.Right) _panel.caption.LayoutTransform = null;
					} else {
						_panel.caption.LayoutTransform = new RotateTransform(ca == Dock.Left ? 270d : 90d);
					}
				}
			}

			void _AddRemoveCaption() {
				Debug.Assert(_IsPanelOrToolbar);
				if (Parent._IsTab && !_IsFloating) {
					if (_panel.caption != null) {
						_panel.panel.Children.Remove(_panel.caption);
						_panel.caption = null;
					}
				} else {
					if (_panel.caption == null) {
						if (_IsToolbar) {
							var c = new Rectangle {
								MinHeight = 8,
								MinWidth = 8,
								Fill = Brushes.Transparent //note: without Fill there are no events
							};
							c.MouseEnter += (_, _) => c.Fill = s_captionBrush;
							c.MouseLeave += (_, _) => c.Fill = Brushes.Transparent;
							_panel.caption = c;
						} else {
							_panel.caption = new TextBlock {
								Text = Name,
								TextAlignment = TextAlignment.Center,
								Padding = new Thickness(2, 1, 2, 2),
								Background = s_captionBrush,
								TextTrimming = TextTrimming.CharacterEllipsis
							};
						}
						_panel.panel.Children.Insert(0, _panel.caption);
						_SetCaptionAt(_float.captionAt, true);
						_panel.caption.ContextMenuOpening += _FloatableContextMenu;
						_panel.caption.MouseDown += _OnMouseDown;
					}
				}
			}
			static readonly Brush s_captionBrush = Brushes.LightSteelBlue;

			void _SplitterContextMenu(object sender, ContextMenuEventArgs e) {
				e.Handled = true;
				var parentStack = Parent._stack;
				var m = new AContextMenu();
				using (m.Submenu("Splitter size")) {
					int z = _SplitterSize;
					for (int i = 1; i <= 10; i++) {
						m[i.ToString()] = o => _SplitterSize = (o.Item.Header as string).ToInt();
						if (i == z) m.LastItem.IsChecked = true;
					}
				}
				_UnitSubmenu(true);
				_UnitSubmenu(false);
				m.IsOpen = true;

				void _UnitSubmenu(bool before) {
					var node = before ? Previous : this;
					var unitNow = node._SizeDef.GridUnitType;
					bool disableNonstar = unitNow == GridUnitType.Star && Parent.Children().Count(o => o._SizeDef.GridUnitType == GridUnitType.Star) < 2;
#if true
					m.Separator();
					m.Add(parentStack.isVertical ? (before ? "- Height Above -" : "- Height Below -") : (before ? "- Width Left -" : "- Width Right -")).IsEnabled = false;
#else
					using (m.Submenu(parentStack.isVertical ? (before ? "Height above" : "Height below") : (before ? "Width left" : "Width right")))
#endif
					{
						_UnitItem("Variable", GridUnitType.Star);
						_UnitItem("Fixed", GridUnitType.Pixel);
						_UnitItem("Auto", GridUnitType.Auto);
					}

					void _UnitItem(string text, GridUnitType unit) {
						m[text] = o => _SetUnit(before, unit);
						if (unit == unitNow) m.LastItem.IsChecked = true;
						if (disableNonstar && unit != GridUnitType.Star) m.LastItem.IsEnabled = false;
					}
				}

				void _SetUnit(bool before, GridUnitType unit) {
					var grid = parentStack.grid;
					int i = _indexInParent; if (before) i--;
					if (parentStack.isVertical) {
						var a = grid.RowDefinitions;
						a[i].Height = new GridLength(a[i].ActualHeight, unit);
						if (unit == GridUnitType.Star) //update other stars, else the splitter may jump
							for (int j = 0; j < a.Count; j++)
								if (j != i && a[j].Height.GridUnitType == GridUnitType.Star)
									a[j].Height = new GridLength(a[j].ActualHeight, unit);
					} else {
						var a = grid.ColumnDefinitions;
						a[i].Width = new GridLength(a[i].ActualWidth, unit);
						if (unit == GridUnitType.Star)
							for (int j = 0; j < a.Count; j++)
								if (j != i && a[j].Width.GridUnitType == GridUnitType.Star)
									a[j].Width = new GridLength(a[j].ActualWidth, unit);
					}
				}
			}

			#region row/col, size

			/// <summary>
			/// Gets actual height of _elem in vertical stack or width in horizontal stack.
			/// </summary>
			double _ElemSize {
				get {
					var stack = Parent._stack;
					return stack.isVertical ? _elem.ActualHeight : _elem.ActualWidth;
				}
			}

			/// <summary>
			/// Gets defined row height/unit in vertical stack or column width/unit in horizontal stack.
			/// </summary>
			GridLength _SizeDef {
				get => new _RowCol(this).SizeDef;
				set => new _RowCol(this) { SizeDef = value };
			}

			/// <summary>
			/// Gets actual row height in vertical stack or column width in horizontal stack.
			/// </summary>
			double _SizeNow => new _RowCol(this).SizeNow;

			/// <summary>
			/// Gets minimal row height in vertical stack or column width in horizontal stack.
			/// </summary>
			double _SizeMin {
				get => new _RowCol(this).SizeMin;
				set => new _RowCol(this) { SizeMin = value };
			}

			/// <summary>
			/// Gets maximal row height in vertical stack or column width in horizontal stack.
			/// </summary>
			double _SizeMax {
				get => new _RowCol(this).SizeMax;
				set => new _RowCol(this) { SizeMax = value };
			}

			struct _RowCol
			{
				DefinitionBase _d;

				public _RowCol(_Node node) {
					var stack = node.Parent._stack;
					if (stack.isVertical) _d = stack.grid.RowDefinitions[node._indexInParent];
					else _d = stack.grid.ColumnDefinitions[node._indexInParent];
				}

				public object Def => _d;

				public GridLength SizeDef {
					get => (_d is RowDefinition rd) ? rd.Height : (_d as ColumnDefinition).Width;
					set { if (_d is RowDefinition rd) rd.Height = value; else (_d as ColumnDefinition).Width = value; }
				}

				public double SizeNow => (_d is RowDefinition rd) ? rd.ActualHeight : (_d as ColumnDefinition).ActualWidth;

				public double SizeMin {
					get => (_d is RowDefinition rd) ? rd.MinHeight : (_d as ColumnDefinition).MinWidth;
					set { if (_d is RowDefinition rd) rd.MinHeight = value; else (_d as ColumnDefinition).MinWidth = value; }
				}

				public double SizeMax {
					get => (_d is RowDefinition rd) ? rd.MaxHeight : (_d as ColumnDefinition).MaxWidth;
					set { if (_d is RowDefinition rd) rd.MaxHeight = value; else (_d as ColumnDefinition).MaxWidth = value; }
				}

				//public bool Visible {
				//	get => SizeMax != 0;
				//	set { SizeDef = new GridLength(SizeNow, SizeDef.GridUnitType); SizeMax = 0; }
				//}
			}

			#endregion
		}
	}
}

