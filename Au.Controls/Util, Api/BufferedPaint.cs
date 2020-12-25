using Au.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Au.Controls
{
	/// <summary>
	/// Wraps buffered paint API BeginBufferedPaint etc.
	/// </summary>
	public struct BufferedPaint : IDisposable
	{
		/// <summary>
		/// Calls API BufferedPaintInit.
		/// </summary>
		public static void Init() { KApi.BufferedPaintInit(); } //fast

		/// <summary>
		/// Calls API BufferedPaintUnInit.
		/// </summary>
		public static void Uninit() { KApi.BufferedPaintUnInit(); }

		AWnd _w;
		nint _dcn, _dcb;
		bool _wmPaint;
		Api.PAINTSTRUCT _ps;
		nint _hb;
		RECT _r;

		public nint NonBufferedDC => _dcn;

		/// <summary>
		/// Gets the buffered DC. Returns <see cref="NonBufferedDC"/> if API BeginBufferedPaint failed.
		/// </summary>
		public nint DC => _dcb;

		/// <summary>
		/// Gets buffered bitmap rectangle (== client rectangle).
		/// </summary>
		public RECT Rect => _r;

		/// <summary>
		/// Gets nonbuffered DC with API BeginPaint or GetDC. Then gets buffered DC with API BeginBufferedPaint for entire client area.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="wmPaint">Use API BeginPaint/EndPaint. If false, uses GetDC/ReleaseDC.</param>
		public unsafe BufferedPaint(AWnd w, bool wmPaint) {
			_w = w;
			if (_wmPaint = wmPaint) {
				_dcn = Api.BeginPaint(w, out _ps);
			} else {
				_ps = default;
				_dcn = Api.GetDC(_w);
			}

			_r = _w.ClientRect; //never mind: should draw only the invalidated rect (ps.rcPaint)
			KApi.BP_PAINTPARAMS pp = default; pp.cbSize = sizeof(KApi.BP_PAINTPARAMS);
			_hb = KApi.BeginBufferedPaint(_dcn, _r, KApi.BP_BUFFERFORMAT.BPBF_TOPDOWNDIB, ref pp, out _dcb); //BPBF_COMPATIBLEBITMAP slower //tested: works with 16 and 8 bit colors too
			ADebug.PrintIf(_hb == default, "BeginBufferedPaint");
			if (_hb == default) _dcb = _dcn;
		}

		/// <summary>
		/// Calls API EndBufferedPaint and EndPaint or ReleaseDC.
		/// </summary>
		public void Dispose() {
			if (_dcn == default) return;
			if (_hb != default) KApi.EndBufferedPaint(_hb, true);
			if (_wmPaint) Api.EndPaint(_w, _ps); else Api.ReleaseDC(_w, _dcn);
			_dcn = default;
		}
	}
}
