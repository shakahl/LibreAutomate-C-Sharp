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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;

namespace Au.Controls
{
	/// <summary>
	/// Allows to set/get scroll info and receive scroll notifications.
	/// This class exists because .NET ScrollableControl does not work when AutoScroll is false.
	/// </summary>
	public class AuScrollableControl : Control
	{
		public class ScrollInfo
		{
			public bool IsVertical { get; private set; }
			public ScrollEventType EventType { get; private set; }
			public int Min { get; private set; }
			public int Max { get; private set; }
			public int Page { get; private set; }
			public int Pos { get; private set; }
			//public int TrackPos { get; private set; }

			internal void Set(bool vert, ScrollEventType et, ref Api.SCROLLINFO k)
			{
				IsVertical = vert;
				EventType = et;
				Min = k.nMin;
				Max = k.nMax;
				Page = k.nPage;
				Pos = k.nPos;
				//TrackPos = k.nTrackPos;
			}
		}

		ScrollInfo _e;
		Api.SCROLLINFO _h, _v;

		public AuScrollableControl()
		{
			_h.cbSize = _v.cbSize = Api.SizeOf(_h);
			_h.nMax = _v.nMax = 100;
			_e = new ScrollInfo();
		}

		void _GetScrollInfo(bool vertical, out Api.SCROLLINFO k)
		{
			if(IsHandleCreated) {
				k = default; unsafe { k.cbSize = sizeof(Api.SCROLLINFO); }
				k.fMask = Api.SIF_TRACKPOS | Api.SIF_POS | Api.SIF_PAGE | Api.SIF_RANGE;
				Api.GetScrollInfo((AWnd)this, vertical, ref k);
			} else if(vertical) k = _v;
			else k = _h;
		}

		public ScrollInfo GetScrollInfo(bool vertical)
		{
			_GetScrollInfo(vertical, out var k);
			var r = new ScrollInfo();
			r.Set(vertical, ScrollEventType.ThumbPosition, ref k);
			return r;
		}

		public int GetScrollPos(bool vertical)
		{
			Api.SCROLLINFO k = default; unsafe { k.cbSize = sizeof(Api.SCROLLINFO); }
			k.fMask = Api.SIF_POS;
			Api.GetScrollInfo((AWnd)this, vertical, ref k);
			return k.nPos;
		}

		public void SetScrollPos(bool vertical, int pos, bool notify)
		{
			_GetScrollInfo(vertical, out var k);
			pos = Math.Clamp(pos, k.nMin, k.nMax);
			if(pos == k.nPos) return;
			k.nPos = pos;

			ref var x = ref (vertical ? ref _v : ref _h);
			x.fMask |= Api.SIF_POS;
			x.nPos = pos;
			if(IsHandleCreated) _SetScrollInfo();
			if(notify) OnScroll(GetScrollInfo(vertical));
		}

		public void SetScrollInfo(bool vertical, int max, int page, int? pos = null, bool notify = false)
		{
			ref var x = ref (vertical ? ref _v : ref _h);
			x.fMask |= Api.SIF_PAGE | Api.SIF_RANGE;
			x.nMax = max; x.nPage = page;
			if(pos.HasValue) {
				x.fMask |= Api.SIF_POS;
				x.nPos = pos.GetValueOrDefault();
			} else if(0 != (x.fMask & Api.SIF_POS)) { //can be when was no handle
				if(x.nPos > max) x.nPos = max;
			}
			if(IsHandleCreated) _SetScrollInfo();

			if(notify) OnScroll(GetScrollInfo(vertical));
		}

		void _SetScrollInfo()
		{
			if(_v.fMask != 0) {
				Api.SetScrollInfo((AWnd)this, true, _v, true);
				_v.fMask = 0;
			}
			if(_h.fMask != 0) {
				Api.SetScrollInfo((AWnd)this, false, _h, true);
				_h.fMask = 0;
			}
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_HSCROLL:
			case Api.WM_VSCROLL:
				_OnScroll(m.Msg == Api.WM_VSCROLL, (ScrollEventType)AMath.LoUshort(m.WParam));
				break;
			case Api.WM_CREATE:
				_SetScrollInfo();
				break;
			}
			base.WndProc(ref m);
		}

		void _OnScroll(bool vert, ScrollEventType code)
		{
			if(code == ScrollEventType.EndScroll) return;
			Api.SCROLLINFO k = default; unsafe { k.cbSize = sizeof(Api.SCROLLINFO); }
			k.fMask = Api.SIF_TRACKPOS | Api.SIF_POS | Api.SIF_PAGE | Api.SIF_RANGE;
			Api.GetScrollInfo((AWnd)this, vert, ref k);
			//AOutput.Write(k.nPos, k.nTrackPos, k.nPage, k.nMax);
			int i = k.nPos;
			switch(code) {
			case ScrollEventType.ThumbTrack: i = k.nTrackPos; break;
			case ScrollEventType.SmallDecrement: i--; break;
			case ScrollEventType.SmallIncrement: i++; break;
			case ScrollEventType.LargeDecrement: i -= k.nPage; break;
			case ScrollEventType.LargeIncrement: i += k.nPage; break;
			case ScrollEventType.First: i = k.nMin; break;
			case ScrollEventType.Last: i = k.nMax; break;
			}
			int max = k.nMax - k.nPage + 1;
			if(i > max) i = max;
			if(i < k.nMin) i = k.nMin;
			k.nPos = i;
			k.fMask = Api.SIF_POS;
			Api.SetScrollInfo((AWnd)this, vert, k, true);

			//if(vert) _v = k; else _h = k;

			_e.Set(vert, code, ref k);
			OnScroll(_e);
		}

		public event EventHandler<ScrollInfo> Scroll;
		protected virtual void OnScroll(ScrollInfo e)
		{
			Scroll?.Invoke(this, e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			//_search?.EndSearch();
			int lines = e.Delta / 120 * SystemInformation.MouseWheelScrollLines;

			var k = GetScrollInfo(true);
			int newValue = k.Pos - lines;
			newValue = Math.Min(k.Max - k.Page + 1, newValue);
			newValue = Math.Min(k.Max, newValue);
			var pos = Math.Max(0, newValue);
			SetScrollPos(true, pos, true);

			base.OnMouseWheel(e);
		}

		protected Size CalcClientAreaSizeWithScrollbars(int contentWidth, int contentHeight)
		{
			//calc current client size + scrollbars
			var z = this.ClientSize;
			if(IsHandleCreated) {
				var w = (AWnd)this;
				Api.SCROLLBARINFO sbi = default; unsafe { sbi.cbSize = sizeof(Api.SCROLLBARINFO); }
				Api.GetScrollBarInfo(w, AccOBJID.VSCROLL, ref sbi); z.Width += sbi.rcScrollBar.Width;
				Api.GetScrollBarInfo(w, AccOBJID.HSCROLL, ref sbi); z.Height += sbi.rcScrollBar.Height;
			}

			//subtract scrollbars if will be added
			bool needV = false, needH = false;
			for(int i = 0; i < 2; i++) {
				if(!needV && contentHeight > z.Height) {
					int k = SystemInformation.VerticalScrollBarWidth;
					if(needV = k <= z.Width && z.Height > 0) z.Width -= k;
				}
				if(!needH && contentWidth >= z.Width) {
					int k = SystemInformation.HorizontalScrollBarHeight;
					if(needH = k < z.Height && (z.Width > 0 || needV)) z.Height -= k;
				}
			}

			return z;
		}

		protected void SetListScrollbars(int count, int itemHeight, int itemWidth, bool resetPos)
		{
			if(_inSetScroll) return; _inSetScroll = true;
			try {
				if(count <= 0 || itemHeight <= 0) {
					this.SetScrollInfo(true, 0, 0, 0);
					this.SetScrollInfo(false, 0, 0, 0);
				} else {
					//var p1 = APerf.Create();
					var z = CalcClientAreaSizeWithScrollbars(itemWidth, count * itemHeight);
					int? pos = resetPos ? (int?)0 : null;
					this.SetScrollInfo(true, count - 1, z.Height / itemHeight, pos);
					this.SetScrollInfo(false, itemWidth, z.Width, pos);
					//p1.Next();
					if(resetPos) {
						//workaround for Windows or .NET bug: sometimes then mouse behaves incorrectly: on scrollbar generates client messages (eg WM_MOUSEMOVE instead of WM_NCMOUSEMOVE). This workaround makes this func almost 2 times slower, but I don't know a better way.
						((AWnd)this).SetWindowPos(Native.SWP.FRAMECHANGED | Native.SWP.NOMOVE | Native.SWP.NOSIZE | Native.SWP.NOACTIVATE | Native.SWP.NOZORDER | Native.SWP.NOSENDCHANGING);

						Invalidate();
					}
					//p1.NW('S');
					ADebug.PrintIf(z != ClientSize && Visible, $"calc={z}  now={ClientSize}");
				}
			}
			finally { _inSetScroll = false; }
		}
		bool _inSetScroll;
	}
}
