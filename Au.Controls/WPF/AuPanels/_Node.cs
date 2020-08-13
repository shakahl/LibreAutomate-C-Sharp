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
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Au;
using Au.Types;

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		/// <summary>
		/// Base of _Dragable (_Panel, _Tab) and _Stack.
		/// </summary>
		abstract class _Node
		{
			protected AuPanels _pm;
			_Node _parent;
			_Stack _stack;
			int _indexInStack;

			public _Node(AuPanels pm, _Node parent, XElement x) {
				_pm = pm;
				if (parent != null) {
					_parent = parent;
					_stack = parent is _Stack stack ? stack : parent._stack;
				}
			}

			public abstract FrameworkElement Elem { get; }
			public abstract void Save(XmlWriter x);

			public bool IsRoot => _parent == null;
			public virtual bool IsToolbar => false;
			public virtual bool IsDocument => false;

			public GridSplitter Splitter { get; set; }

			public int DefaultSplitterWidth => IsToolbar ? 1 : 5;

			/// <summary>
			/// Gets actual <see cref="Splitter"/> height in vertical stack or width in horizontal stack.
			/// </summary>
			public int SplitterWidthInStack {
				get {
					if (Splitter == null) return 0;
					return (_stack.IsVertical ? Splitter.ActualHeight : Splitter.ActualWidth).ToInt();
				}
			}

			/// <summary>
			/// Gets actual <see cref="Elem"/> height in vertical stack or width in horizontal stack.
			/// </summary>
			public double ElemWidthInStack {
				get {
					if (_parent is not _Stack) throw new InvalidOperationException();
					return _stack.IsVertical ? Elem.ActualHeight : Elem.ActualWidth;
				}
			}

			/// <summary>
			/// Gets actual row height/unit in vertical stack or column width/unit in horizontal stack. Also gets unit.
			/// </summary>
			public GridLength WidthInStack {
				get {
					if (_parent is not _Stack stack) throw new InvalidOperationException();
					var grid = stack.Elem as Grid;
					if (stack.IsVertical) {
						var d = grid.RowDefinitions[_indexInStack];
						return new GridLength(d.ActualHeight, d.Height.GridUnitType);
					} else {
						var d = grid.ColumnDefinitions[_indexInStack];
						return new GridLength(d.ActualWidth, d.Width.GridUnitType);
					}
				}
			}

			internal void LoadAttributesAndAddToStack_(XElement x, int indexInStack) {
				_indexInStack = indexInStack;
				var grid = _stack.Elem as Grid;
				bool isVertical = _stack.IsVertical;

				//set height in vertical stack or width in horizontal stack
				var k = _Util.GridLengthFromString(x);
				if (isVertical) {
					grid.RowDefinitions.Add(new RowDefinition { Height = k });
				} else {
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = k });
				}

				//splitter
				if (indexInStack > 0) {
					int whs = x.Attr("s", DefaultSplitterWidth);
					var v = new GridSplitter2 { ResizeBehavior = GridResizeBehavior.PreviousAndCurrent };
					if (isVertical) { //horz splitter
						v.ResizeDirection = GridResizeDirection.Rows;
						v.VerticalAlignment = VerticalAlignment.Top;
						v.HorizontalAlignment = HorizontalAlignment.Stretch;
						v.Height = whs;
						Elem.Margin = new Thickness(0, whs, 0, 0);
					} else { //vert splitter
						v.ResizeDirection = GridResizeDirection.Columns;
						v.HorizontalAlignment = HorizontalAlignment.Left;
						//default stretch
						v.Width = whs;
						Elem.Margin = new Thickness(whs, 0, 0, 0);
					}
					Splitter = v;
					if (isVertical) Grid.SetRow(v, indexInStack); else Grid.SetColumn(v, indexInStack);
					grid.Children.Add(v);
				}

				if (isVertical) Grid.SetRow(Elem, indexInStack); else Grid.SetColumn(Elem, indexInStack);
				grid.Children.Add(Elem);
			}

			/// <summary>
			/// Writes attributes w (width) and s (splitter width) if in stack.
			/// </summary>
			protected virtual void _SaveAttributes(XmlWriter x) {
				if (_parent is not _Stack) return;

				var w = WidthInStack;
				if (!w.IsAuto) x.WriteAttributeString("w", _Util.GridLengthToString(w));

				var s = SplitterWidthInStack;
				if (s > 0 && s != DefaultSplitterWidth) x.WriteAttributeString("s", s.ToString());
			}

		}
	}
}

