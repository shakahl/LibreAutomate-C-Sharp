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

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		/// <summary>
		/// Base of _Dragable (_Panel, _Tab) and _Stack.
		/// </summary>
		abstract class _Node
		{
			internal readonly AuPanels _pm;
			internal _Node _parent;
			internal _Stack _stack;
			internal int _indexInStack;

			public _Node(AuPanels pm, _Node parent) {
				_pm = pm;
				if (parent != null) {
					_parent = parent;
					_stack = parent is _Stack stack ? stack : parent._stack;
				}
			}

			/// <summary>
			/// Grid if _Stack, DockPanel if _Panel, TabControl if _Tab.
			/// </summary>
			public abstract FrameworkElement Elem { get; }
			public abstract void Save(XmlWriter x);

			public bool IsRoot => _parent == null;
			public virtual bool IsToolbar => false;
			public virtual bool IsDocument => false;
			/// <summary>
			/// Not stack/toolbar/document.
			/// </summary>
			public virtual bool IsPanel => false;

			public GridSplitter Splitter { get; set; }

			protected const int c_defaultSplitterWidth = 5;

			/// <summary>
			/// Gets or sets actual height of <see cref="Splitter"/> in vertical stack or width in horizontal stack.
			/// </summary>
			public int SplitterSize {
				get {
					if (Splitter == null) return 0;
					return (_stack.IsVertical ? Splitter.ActualHeight : Splitter.ActualWidth).ToInt();
				}
				private set {
					if (Splitter == null) return;
					double d = value;
					if (_stack.IsVertical) {
						Splitter.Height = d;
						Elem.Margin = new Thickness(0, d, 0, 0);
					} else {
						Splitter.Width = d;
						Elem.Margin = new Thickness(d, 0, 0, 0);
					}
				}
			}

			/// <summary>
			/// Gets actual height of <see cref="Elem"/> in vertical stack or width in horizontal stack.
			/// </summary>
			public double ElemSize {
				get {
					if (_parent is not _Stack) throw new InvalidOperationException();
					return _stack.IsVertical ? Elem.ActualHeight : Elem.ActualWidth;
				}
			}

			/// <summary>
			/// Gets defined row height in vertical stack or column width in horizontal stack. Also gets unit.
			/// </summary>
			public GridLength SizeDef {
				get => new _RowCol(this).SizeDef;
				set => new _RowCol(this) { SizeDef = value };
			}

			/// <summary>
			/// Gets actual row height in vertical stack or column width in horizontal stack.
			/// </summary>
			public double SizeNow => new _RowCol(this).SizeNow;

			/// <summary>
			/// Gets minimal row height in vertical stack or column width in horizontal stack.
			/// </summary>
			public double SizeMin {
				get => new _RowCol(this).SizeMin;
				set => new _RowCol(this) { SizeMin = value };
			}

			/// <summary>
			/// Gets maximal row height in vertical stack or column width in horizontal stack.
			/// </summary>
			public double SizeMax {
				get => new _RowCol(this).SizeMax;
				set => new _RowCol(this) { SizeMax = value };
			}

			internal void LoadAttributesAndAddToStack_(XElement x, int indexInStack) {
				_indexInStack = indexInStack;
				var grid = _stack.Elem as Grid;
				bool isVertical = _stack.IsVertical;

				//set height in vertical stack or width in horizontal stack
				var k = _Util.GridLengthFromString(x.Attr("w"));
				if (isVertical) {
					grid.RowDefinitions.Add(new RowDefinition { Height = k });
				} else {
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = k });
				}

				//splitter
				if (_indexInStack > 0) {
					var v = new GridSplitter2 { ResizeBehavior = GridResizeBehavior.PreviousAndCurrent };
					if (isVertical) { //horz splitter
						v.ResizeDirection = GridResizeDirection.Rows;
						v.VerticalAlignment = VerticalAlignment.Top;
						v.HorizontalAlignment = HorizontalAlignment.Stretch;
					} else { //vert splitter
						v.ResizeDirection = GridResizeDirection.Columns;
						v.HorizontalAlignment = HorizontalAlignment.Left;
						//default stretch
					}
					Splitter = v;
					SplitterSize = x.Attr("s", c_defaultSplitterWidth);
					if (isVertical) Grid.SetRow(v, _indexInStack); else Grid.SetColumn(v, _indexInStack);
					v.ContextMenuOpening += _SplitterContextMenu;
					grid.Children.Add(v);
				}

				if (isVertical) Grid.SetRow(Elem, _indexInStack); else Grid.SetColumn(Elem, _indexInStack);
				grid.Children.Add(Elem);
			}

			/// <summary>
			/// Writes attributes w (width) and s (splitter width) if in stack.
			/// </summary>
			protected virtual void _SaveAttributes(XmlWriter x) {
				if (_parent is not _Stack) return;

				var k = new _RowCol(this);
				var w = k.SizeDef;
				if (!w.IsAuto) {
					if (k.Visible) w = new GridLength(k.SizeNow, w.GridUnitType); //hiding: Height=ActualHeight and MaxHeight=0
					x.WriteAttributeString("w", _Util.GridLengthToString(w));
				}

				var s = SplitterSize;
				if (s > 0 && s != c_defaultSplitterWidth) x.WriteAttributeString("s", s.ToString());
			}

			void _SplitterContextMenu(object sender, ContextMenuEventArgs e) {
				e.Handled = true;
				var m = new AContextMenu();
				using (m.Submenu("Splitter size")) {
					int z = SplitterSize;
					for (int i = 1; i <= 10; i++) {
						m[i.ToString()] = o => SplitterSize = (o.Item.Header as string).ToInt();
						if (i == z) m.LastItem.IsChecked = true;
					}
				}
				_UnitSubmenu(true);
				_UnitSubmenu(false);
				m.IsOpen = true;

				void _UnitSubmenu(bool before) {
					int i = _indexInStack; if (before) i--;
					var node = _stack.Nodes[i];
					var unitNow = node.SizeDef.GridUnitType;
					bool disableNonstar = unitNow == GridUnitType.Star && _stack.Nodes.Count(o => o.SizeDef.GridUnitType == GridUnitType.Star) < 2;
#if true
					m.Separator();
					m.Add(_stack.IsVertical ? (before ? "- Height Above -" : "- Height Below -") : (before ? "- Width Left -" : "- Width Right -")).IsEnabled = false;
#else
					using (m.Submenu(_stack.IsVertical ? (before ? "Height above" : "Height below") : (before ? "Width left" : "Width right")))
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
					var grid = _stack.Elem as Grid;
					int i = _indexInStack; if (before) i--;
					if (_stack.IsVertical) {
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
		}

		struct _RowCol
		{
			DefinitionBase _d;

			public _RowCol(_Node node) {
				if (node._parent is not _Stack stack) throw new InvalidOperationException();
				var grid = stack.Elem as Grid;
				if (stack.IsVertical) _d = grid.RowDefinitions[node._indexInStack];
				else _d = grid.ColumnDefinitions[node._indexInStack];
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

			public bool Visible {
				get => SizeMax != 0;
				set { SizeDef = new GridLength(SizeNow, SizeDef.GridUnitType); SizeMax = 0; }
			}
		}
	}
}

