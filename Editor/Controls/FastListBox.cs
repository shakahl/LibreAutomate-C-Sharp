//This control was created for code info lists, because other controls are too slow etc.
//Tried several controls. Speed of adding 2117 items + drawing 25-32 visible items with text and image:
//Virtual owner-draw ListView (25 visible items): 15 ms. Without DoubleBuffer 22 ms. Often much slower.
//	Can make faster if parent control handles native custom draw notifications and uses native functions to draw image.
//	But I don't like it. Does much unnecessary work, eg redraws items several times, reflects notifications, etc.
//TreeViewAdv (28 visible items): 55 ms.
//Scintilla without images (32 visible lines): 7-8 ms. With images probably not possible because Scintilla allows max 32 markers. We need 46 or more.
//ListBox (28 visible items): 430 ms. Not suitable for big lists.
//This control: 5 ms (4-10).
//When AddItems called every 100 ms (timer) with same list, process CPU is 6, 14, 4, ?, 3 %. Not including getting the completion items.

//TODO: move to TreeList

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	class FastListBox : AuScrollableControl
	{
		int _count, _itemWidth, _itemHeight, _widthPlus, _iSelected;
		Func<int, string> _itemText;
		Action<ItemDrawArgs> _drawFunc;
		System.Collections.BitArray _measuredItems;

		//bool _debugPaintedOnce;

		public FastListBox()
		{
			this.BackColor = SystemColors.Window;
			this.DoubleBuffered = true;
			this.ResizeRedraw = true;
		}

		public void AddItems(int count, Func<int, string> itemText, int widthPlus, Action<ItemDrawArgs> drawFunc)
		{
			//_debugPaintedOnce = false;

			//if(!IsHandleCreated) throw new InvalidOperationException("Must be handle created");
			if(count + _count == 0) return;
			_itemText = itemText;
			_drawFunc = drawFunc;
			_iSelected = -1;
			_aAcc = null;
			_widthPlus = widthPlus;
			_itemWidth = _itemHeight = 0;
			_measuredItems = null;
			_count = count;
			if(count > 0) {
				_measuredItems = new System.Collections.BitArray(count);
				_MeasureItems(0, -1); //now measure only visible items, else slow if many items. When scrolled, will measure others and set AutoScrollMinSize if need again, which will add or update horizontal scrollbar.
									  //APerf.Next('w');
			}

			_SetScroll(resetPos: true);
		}

		/// <summary>
		/// Index of the selected item, or -1.
		/// </summary>
		public int SelectedIndex {
			get => _iSelected;
			set {
				if(value!= _iSelected) {
					_iSelected = value;
					this.SetScrollPos(true, value, true);
				}
			}
		}

		/// <summary>
		/// Item height, calculated by <see cref="AddItems"/>.
		/// </summary>
		public int ItemHeight => _itemHeight;

		//Sets _itemWidth. If iTo<0, sets _itemHeight too. Uses/sets _measuredItems.
		void _MeasureItems(int iFrom, int iTo)
		{
			var dc = new Au.Util.LibWindowDC((AWnd)this); //never mind: if no handle, gets screen DC; it's the same.
			var oldFont = Api.SelectObject(dc, Au.Util.LibNativeFont.RegularCached);
			try {
				if(iTo < 0) {
					Debug.Assert(this.Font.Equals(Util.AFonts.Regular));
					var t = _itemText(0);
					Api.GetTextExtentPoint32(dc, t, t.Length, out var z);
					_itemHeight = Math.Max(17, z.height + 2);
					_itemWidth = 0;
					iFrom = 0;
					iTo = Math.Min(_count, Size.Height / _itemHeight);
				}
				for(int i = iFrom; i < iTo; i++) {
					if(_measuredItems[i]) continue;
					var t = _itemText(i);
					Api.GetTextExtentPoint32(dc, t, t.Length, out var z); //many times faster than TextRenderer.MeasureText
					_itemWidth = Math.Max(_itemWidth, z.width + _widthPlus);
					_measuredItems[i] = true;
				}
			}
			finally {
				Api.SelectObject(dc, oldFont);
				dc.Dispose();
			}
		}

		void _SetScroll(bool resetPos = false)
		{
			SetListScrollbars(_count, _itemHeight, _itemWidth, resetPos);
		}

		protected override void OnScroll(ScrollInfo e)
		{
			if(e.IsVertical) {
				int iFrom = e.Pos, iTo = Math.Min(_count, iFrom + e.Page);
				//Print(iFrom, iTo);
				bool measure = false;
				for(int i = iFrom; i < iTo; i++) if(measure = !_measuredItems[i]) break;
				if(measure) {
					//Print("measure", iFrom, iTo);
					int oldWidth = _itemWidth;
					_MeasureItems(iFrom, iTo);
					if(_itemWidth > oldWidth) {
						//Print("wider", oldWidth, _itemWidth);
						_SetScroll();
					}
				}
			}
			this.Invalidate(); //SHOULDDO: invalidate only part
			base.OnScroll(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if(_count == 0) return;

			//if(!_debugPaintedOnce) _debugPaintedOnce = true; else APerf.First();
			var g = e.Graphics;
			int xScroll = this.GetScrollPos(false), yScroll = this.GetScrollPos(true) * _itemHeight;
			if(xScroll + yScroll != 0) g.TranslateTransform(-xScroll, -yScroll);
			//Print("OnPaint", e.ClipRectangle, g.VisibleClipBounds, xScroll, yScroll);

			var clip = e.ClipRectangle;
			int yFrom = yScroll + clip.Y, yTo = yFrom + clip.Height;
			int iFrom = yFrom / _itemHeight, iTo = Math.Min(_count, yTo / _itemHeight);
			//Print(iFrom, iTo);

			var args = new ItemDrawArgs { Graphics = g };
			for(int i = iFrom; i < iTo; i++) {
				args.Index = i;
				args.Bounds = new Rectangle(0, i * _itemHeight, Math.Max(clip.Width, _itemWidth), _itemHeight);
				args.IsSelected = i == _iSelected;
				_drawFunc(args);
			}

			//APerf.Next();

			//base.OnPaint(e);
		}

		//protected override void WndProc(ref Message m)
		//{
		//	base.WndProc(ref m);
		//	if(m.Msg == Api.WM_PAINT) APerf.NW();
		//}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			if(_count > 0 && IsHandleCreated) _SetScroll();
			base.OnClientSizeChanged(e);
		}

		public class ItemDrawArgs
		{
			public Graphics Graphics;
			public int Index;
			public Rectangle Bounds;
			public bool IsSelected;
		}

		public class ItemClickArgs
		{
			public ItemClickArgs(int index, bool doubleClick)
			{
				Index = index;
				DoubleClick = doubleClick;
			}
			public int Index;
			public bool DoubleClick;
		}

		/// <summary>
		/// When an item left-clicked or double-clicked.
		/// </summary>
		public event EventHandler<ItemClickArgs> ItemClick;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			//Print(e.Location, e.Clicks);
			if(_count != 0) {
				int i = _ItemAtY(e.Y);
				if(i != _iSelected) {
					_iSelected = i;
					Invalidate(); //SHOULDDO: invalidate only part
				}
				if(i >= 0 && e.Button == MouseButtons.Left) ItemClick?.Invoke(this, new ItemClickArgs(i, e.Clicks == 2));
			}
			base.OnMouseDown(e);
		}

		//protected override void WndProc(ref Message m)
		//{
		//	AWnd.More.PrintMsg(m, Api.WM_GETTEXTLENGTH, Api.WM_GETTEXT);
		//	base.WndProc(ref m);
		//}

		/// <summary>
		/// Gets index of item at y coordinate in client area.
		/// Returns -1 if y is not in an item.
		/// </summary>
		/// <param name="y"></param>
		int _ItemAtY(int y)
		{
			if(_count == 0) return -1;
			int i = y / _itemHeight + this.GetScrollPos(true);
			if(i >= _count) i = -1;
			return i;
		}

		/// <summary>
		/// Gets bounds of one item or range, where it is visible in client area. Can be offscreen.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="i2">If -1, gets bounds of i. Else from i to (including) i2. Can be less than i.</param>
		Rectangle _ItemBounds(int i, int i2 = -1)
		{
			if(i2 == i) i2 = -1;
			if(i2 >= 0 && i2 < i) AMath.Swap(ref i, ref i2);
			return new Rectangle(0, (i + this.GetScrollPos(true)) * _itemHeight, ClientSize.Width, i2 < i ? _itemHeight : _itemHeight * (i2 - i + 1));
		}

		#region IAccessible

		_AccContainer _accObj;
		_AccNode[] _aAcc;

		protected override AccessibleObject CreateAccessibilityInstance() => _accObj ??= new _AccContainer(this);

		/// <summary>
		/// Max count of accessible objects that can be created for items.
		/// Default 1000.
		/// </summary>
		/// <remarks>
		/// Controls with large number of visible items consume much memory for accessible objects, because of very inefficient accessibility implementation of .NET. For example 120 MB of physical memory for 10000 items. Luckily accessible objects are created only when/if some accessibility/automation/etc app wants to use them.
		/// This property limits the number of accessible objects when some app wants to get all objects, but not when wants to get object from point or the focused/selected object.
		/// </remarks>
		public int AccessibleCount { get; set; } = 1000;
		//see comments in TVAcc.cs in TreeList project.

		class _AccContainer : ControlAccessibleObject
		{
			FastListBox _c;

			public _AccContainer(FastListBox c) : base(c) => _c = c;

			public override AccessibleRole Role => AccessibleRole.List;

			public override int GetChildCount() => Math.Min(_c._count, _c.AccessibleCount);

			public override AccessibleObject GetChild(int index) => _c._ItemAcc(index);

			public override AccessibleObject HitTest(int x, int y)
			{
				var p = _c.PointToClient(new Point(x, y));
				if(!_c.ClientRectangle.Contains(p)) return null;
				int i = _c._ItemAtY(p.Y);
				if(i < 0) return this;
				return _c._ItemAcc(i);
			}

			public override AccessibleObject GetSelected() => _c._ItemAcc(_c._iSelected);

			public override AccessibleObject GetFocused() => _c.Focused ? GetSelected() : null;
		}

		_AccNode _ItemAcc(int i)
		{
			if((uint)i >= _count) return null;
			_aAcc ??= new _AccNode[_count];
			return _aAcc[i] ??= new _AccNode(this, i);
		}

		class _AccNode : AccessibleObject
		{
			FastListBox _c;
			int _index;

			public _AccNode(FastListBox c, int index)
			{
				_c = c;
				_index = index;
			}

			public override AccessibleRole Role => AccessibleRole.ListItem;

			public override Rectangle Bounds => _c.RectangleToScreen(_c._ItemBounds(_index));

			public override AccessibleObject HitTest(int x, int y)
			{
				//Print("node.HitTest");
				if(this.Bounds.Contains(x, y)) return this;
				return null;
			}

			public override int GetChildCount() => 0;

			public override AccessibleObject Parent => _c._accObj;

			public override AccessibleObject Navigate(AccessibleNavigation navdir)
			{
				switch(navdir) {
				case AccessibleNavigation.Next: return _c._ItemAcc(_index + 1);
				case AccessibleNavigation.Previous: return _c._ItemAcc(_index - 1);
				}
				return base.Navigate(navdir);
			}

			public override string Name => _c._itemText(_index);

			public override AccessibleStates State {
				get {
					var r = AccessibleStates.Selectable | AccessibleStates.Focusable;
					if(_index == _c._iSelected) {
						r |= AccessibleStates.Selected;
						if(_c.Focused) r |= AccessibleStates.Focused;
					}
					//if(_IsOffscreen()) r |= AccessibleStates.Offscreen;
					return r;
				}
			}

			public override string DefaultAction => "Double Click";

			public override void DoDefaultAction() => _c._DoItemAction(_index);
		}

		void _DoItemAction(int i)
		{
			if(i != _iSelected) {
				Invalidate(_ItemBounds(i, _iSelected));
				_iSelected = i;
			}
			ItemClick?.Invoke(this, new ItemClickArgs(i, true));
		}

		#endregion

		//FUTURE: keyboard navigation. Currently don't need because the control is never focused.
	}
}
