//This control was created for code info lists, because other controls are too slow etc.
//Tried several controls. Speed of adding 2117 items + drawing 25-32 visible items with text and image:
//Virtual owner-draw ListView (25 visible items): 15 ms. Without DoubleBuffer 22 ms. Often much slower.
//	Can make faster if parent control handles native custom draw notifications and uses native functions to draw image.
//	But I don't like it. Does much unnecessary work, eg redraws items several times, reflects notifications, etc.
//TreeViewAdv (28 visible items): 55 ms.
//Scintilla without images (32 visible lines): 7-8 ms. With images probably not possible because Scintilla allows max 32 markers. We need 46 or more.
//ListBox (28 visible items): 430 ms. Not suitable for big lists.
//This control: 5 ms.

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

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	class AuListControl : AuScrollableControl
	{
		int _count, _itemWidth, _itemHeight, _iSelected;
		Func<ZItemMeasureArgs, int> _measureFunc;
		Action<ZItemDrawArgs> _drawAction;
		Func<int, string> _accName;
		System.Collections.BitArray _measuredItems;

		public AuListControl()
		{
			this.BackColor = SystemColors.Window;
			this.DoubleBuffered = true;
			this.ResizeRedraw = true;
		}

		public void ZAddItems(int count, Func<ZItemMeasureArgs, int> measureFunc, Action<ZItemDrawArgs> drawAction, Func<int, string> accName)
		{
			if(count + _count == 0) return;
			_measureFunc = measureFunc;
			_drawAction = drawAction;
			_accName = accName;
			_iSelected = -1;
			_aAcc = null;
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

		public void ZReMeasure()
		{
			ZAddItems(_count, _measureFunc, _drawAction, _accName);
		}

		public int ZCount => _count;

		/// <summary>
		/// Index of the selected item, or -1.
		/// </summary>
		public int ZSelectedIndex => _iSelected;

		public void ZSelectIndex(int i, bool scrollCenter)
		{
			if((uint)i >= _count) i = -1;
			if(i == _iSelected) return;
			_iSelected = i;
			if(i < 0) {
				this.SetScrollPos(true, 0, true);
			} else {
				var k = this.GetScrollInfo(true);
				int min = k.Pos, max = k.Pos + k.Page - 1;
				if((i < min || i > max) && k.Page > 0) {
					if(scrollCenter && k.Page >= 5) i = Math.Max(i - k.Page / 5, 0);
					else if(i > max) i = Math.Max(i - k.Page + 1, 0);
					this.SetScrollPos(true, i, true);
				}
			}
			this.Invalidate();
			_OnSelectedIndexChanged(_iSelected);
		}

		void _OnSelectedIndexChanged(int i)
		{
			ZSelectedIndexChanged?.Invoke(i);
		}
		public event Action<int> ZSelectedIndexChanged;

		///// <summary>
		///// Item height, calculated by <see cref="AddItems"/>.
		///// </summary>
		//public int ItemHeight => _itemHeight;

		//Sets _itemWidth. If iTo<0, sets _itemHeight too. Uses/sets _measuredItems.
		void _MeasureItems(int iFrom, int iTo)
		{
			using var m = new ZItemMeasureArgs();
			if(iTo < 0) {
				m.index = 0;
				var z = m.MeasureText("A");
				_itemHeight = Math.Max(17, z.height + 2);
				_itemWidth = 0;
				iFrom = 0;
				iTo = Math.Min(_count, Size.Height / _itemHeight);
			}
			for(int i = iFrom; i < iTo; i++) {
				if(_measuredItems[i]) continue;
				m.index = i;
				int width = _measureFunc(m); //many times faster than TextRenderer.MeasureText
				_itemWidth = Math.Max(_itemWidth, width);
				_measuredItems[i] = true;
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
			//ADebug.MemorySetAnchor_();
			//var p1 = APerf.Create();

			var g = e.Graphics;
			int xScroll = this.GetScrollPos(false), yScroll = this.GetScrollPos(true) * _itemHeight;
			//Print("OnPaint", e.ClipRectangle, g.VisibleClipBounds, xScroll, yScroll);

			var clip = e.ClipRectangle;
			int yFrom = yScroll + clip.Y, yTo = yFrom + clip.Height;
			int iFrom = yFrom / _itemHeight, iTo = Math.Min(_count, yTo / _itemHeight);
			//Print(iFrom, iTo);

			using var args = new ZItemDrawArgs { graphics = g };
			for(int i = iFrom; i < iTo; i++) {
				args.index = i;
				args.bounds = new Rectangle(-xScroll, i * _itemHeight - yScroll, Math.Max(clip.Width, _itemWidth), _itemHeight);
				args.isSelected = i == _iSelected;
				_drawAction(args);
			}

			//p1.NW('P');
			//APerf.Next();
			//ADebug.MemoryPrint_();

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

		public class ZItemMeasureArgs : IDisposable
		{
			public int index;
			IntPtr _dc, _oldFont;

			public ZItemMeasureArgs()
			{
				_dc = Api.GetDC(default);
				_oldFont = Api.SelectObject(_dc, Util.NativeFont_.RegularCached);
			}

			public void Dispose()
			{
				Api.SelectObject(_dc, _oldFont);
				Api.ReleaseDC(default, _dc);
			}

			public unsafe SIZE MeasureText(string s, int from = 0, int len = -1)
			{
				if(len < 0) len = s.Lenn();
				if(len == 0) return default;
				fixed (char* p = s) {
					Api.GetTextExtentPoint32(_dc, p + from, len, out var z); //many times faster than TextRenderer.MeasureText
					return z;
				}
			}
		}

		public class ZItemDrawArgs : IDisposable
		{
			public Graphics graphics;
			public int index;
			public Rectangle bounds;
			public bool isSelected;
			IntPtr _dc, _oldFont;

			public ZItemDrawArgs()
			{
				_dc = Api.GetDC(default);
				_oldFont = Api.SelectObject(_dc, Util.NativeFont_.RegularCached);
			}

			public void Dispose()
			{
				Api.SelectObject(_dc, _oldFont);
				Api.ReleaseDC(default, _dc);
			}

			public unsafe int MeasureText(string s, int from, int len)
			{
				if(len == 0) return 0;
				fixed (char* p = s) {
					Api.GetTextExtentPoint32(_dc, p + from, len, out var z); //many times faster than TextRenderer.MeasureText
					return z.width;
				}
			}
			//public int MeasureText(string s, int from, int len, Font font)
			//{
			//	if(len == 0) return 0;
			//	return TextRenderer.MeasureText(Graphics, s.Substring(from, len), font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
			//}

		}

		public class ZItemClickArgs
		{
			public ZItemClickArgs(int index, bool doubleClick)
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
		public event EventHandler<ZItemClickArgs> ZItemClick;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			//Print(e.Location, e.Clicks);
			if(_count != 0) {
				int i = _ItemAtY(e.Y);
				if(i != _iSelected) {
					_iSelected = i;
					Invalidate(); //SHOULDDO: invalidate only part
					_OnSelectedIndexChanged(_iSelected);
				}
				if(i >= 0 && e.Button == MouseButtons.Left) ZItemClick?.Invoke(this, new ZItemClickArgs(i, e.Clicks == 2));
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

		/// <summary>
		/// Scrolls on pressed arrow and page up/down keys.
		/// </summary>
		public void ZKeyboardNavigation(Keys k)
		{
			int i = _iSelected;
			if(i >= 0) {
				switch(k) {
				case Keys.Down: i++; break;
				case Keys.Up: i--; break;
				case Keys.PageDown: i += this.GetScrollInfo(true).Page; break;
				case Keys.PageUp: i -= this.GetScrollInfo(true).Page; break;
				}
				i = AMath.MinMax(i, 0, _count - 1);
			} else if(_count > 0) {
				i = 0;
			}
			ZSelectIndex(i, scrollCenter: false);
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
		public int ZAccessibleCount { get; set; } = 1000;
		//see comments in TVAcc.cs in TreeList project.

		class _AccContainer : ControlAccessibleObject
		{
			AuListControl _c;

			public _AccContainer(AuListControl c) : base(c) => _c = c;

			public override AccessibleRole Role => AccessibleRole.List;

			public override int GetChildCount() => Math.Min(_c._count, _c.ZAccessibleCount);

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
			AuListControl _c;
			int _index;

			public _AccNode(AuListControl c, int index)
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

			public override string Name => _c._accName(_index);

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
				_OnSelectedIndexChanged(_iSelected);
			}
			ZItemClick?.Invoke(this, new ZItemClickArgs(i, true));
		}

		#endregion
	}
}
