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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	/// <summary>
	/// Allows to set/get scroll info and receive scroll notifications.
	/// This class exists because .NET ScrollableControl does not work when AutoScroll is false.
	/// </summary>
	public class ScrollableControl_ :Control
	{
		internal const uint SIF_RANGE = 0x1;
		internal const uint SIF_PAGE = 0x2;
		internal const uint SIF_POS = 0x4;
		internal const uint SIF_TRACKPOS = 0x10;

		internal struct SCROLLINFO
		{
			public uint cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public int nPage;
			public int nPos;
			public int nTrackPos;
		}

		[DllImport("user32.dll")]
		internal static extern int SetScrollInfo(Wnd hwnd, bool vert, [In] ref SCROLLINFO lpsi, bool redraw);

		[DllImport("user32.dll")]
		internal static extern bool GetScrollInfo(Wnd hwnd, bool vert, ref SCROLLINFO lpsi);

		public class ScrollInfo
		{
			public bool IsVertical { get; private set; }
			public ScrollEventType EventType { get; private set; }
			public int Min { get; private set; }
			public int Max { get; private set; }
			public int Page { get; private set; }
			public int Pos { get; private set; }
			//public int TrackPos { get; private set; }

			internal void Set(bool vert, ScrollEventType et, ref SCROLLINFO k)
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
		SCROLLINFO _h, _v;

		public ScrollableControl_()
		{
			_h.cbSize = _v.cbSize = Api.SizeOf(_h);
			_h.nMax = _v.nMax = 100;
			_e = new ScrollInfo();
		}

		void _GetScrollInfo(bool vertical, out SCROLLINFO k)
		{
			if(IsHandleCreated) {
				k = new SCROLLINFO(); k.cbSize = Api.SizeOf(k);
				k.fMask = SIF_TRACKPOS | SIF_POS | SIF_PAGE | SIF_RANGE;
				GetScrollInfo((Wnd)this, vertical, ref k);
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
			var k = new SCROLLINFO(); k.cbSize = Api.SizeOf(k);
			k.fMask = SIF_POS;
			GetScrollInfo((Wnd)this, vertical, ref k);
			return k.nPos;
		}

		public void SetScrollPos(bool vertical, int pos, bool notify)
		{
			_GetScrollInfo(vertical, out var k);
			pos = Math_.MinMax(pos, k.nMin, k.nMax);
			if(pos == k.nPos) return;
			k.nPos = pos;

			if(vertical) {
				_v.fMask |= SIF_POS;
				_v.nPos = pos;
			} else {
				_h.fMask |= SIF_POS;
				_h.nPos = pos;
			}
			if(IsHandleCreated) _SetScrollInfo();
			if(notify) OnScroll(GetScrollInfo(vertical));
		}

		public void SetScrollInfo(bool vertical, int max, int page, bool notify)
		{
			if(vertical) {
				_v.fMask |= SIF_PAGE | SIF_RANGE;
				_v.nMax = max; _v.nPage = page;
			} else {
				_h.fMask |= SIF_PAGE | SIF_RANGE;
				_h.nMax = max; _h.nPage = page;
			}
			if(IsHandleCreated) _SetScrollInfo();

			if(notify) OnScroll(GetScrollInfo(vertical));
		}

		void _SetScrollInfo()
		{
			if(_h.fMask != 0) {
				SetScrollInfo((Wnd)this, false, ref _h, true);
				_h.fMask = 0;
			}
			if(_v.fMask != 0) {
				SetScrollInfo((Wnd)this, true, ref _v, true);
				_v.fMask = 0;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			_SetScrollInfo();
			base.OnHandleCreated(e);
		}

		protected override void WndProc(ref Message m)
		{
			uint msg = (uint)m.Msg;
			switch(msg) {
			case Api.WM_HSCROLL:
			case Api.WM_VSCROLL:
				_OnScroll(msg == Api.WM_VSCROLL, (ScrollEventType)Math_.LoUshort(m.WParam));
				break;
			}
			base.WndProc(ref m);
		}

		void _OnScroll(bool vert, ScrollEventType code)
		{
			if(code == ScrollEventType.EndScroll) return;
			var k = new SCROLLINFO(); k.cbSize = Api.SizeOf(k);
			k.fMask = SIF_TRACKPOS | SIF_POS | SIF_PAGE | SIF_RANGE;
			GetScrollInfo((Wnd)this, vert, ref k);
			//Print(k.nPos, k.nTrackPos, k.nPage, k.nMax);
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
			k.fMask = SIF_POS;
			SetScrollInfo((Wnd)this, vert, ref k, true);

			//if(vert) _v = k; else _h = k;

			_e.Set(vert, code, ref k);
			OnScroll(_e);
		}

		public event EventHandler<ScrollInfo> Scroll;
		protected virtual void OnScroll(ScrollInfo e)
		{
			Scroll?.Invoke(this, e);
		}
	}
}
