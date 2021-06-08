using Au.Types;
using System;

namespace Au.More
{
	/// <summary>
	/// Wraps buffered paint API <msdn>BeginBufferedPaint</msdn> etc.
	/// </summary>
	public struct BufferedPaint : IDisposable
	{
		/// <summary>
		/// Calls API <msdn>BufferedPaintInit</msdn>.
		/// </summary>
		public static void Init() { Api.BufferedPaintInit(); } //fast

		/// <summary>
		/// Calls API <msdn>BufferedPaintUnInit</msdn>.
		/// </summary>
		public static void Uninit() { Api.BufferedPaintUnInit(); }

		wnd _w;
		IntPtr _dcn, _dcb;
		bool _wmPaint;
		Api.PAINTSTRUCT _ps;
		IntPtr _hb;
		RECT _r;

		/// <summary>
		/// Gets window DC.
		/// </summary>
		public IntPtr NonBufferedDC => _dcn;

		/// <summary>
		/// Gets the buffered DC. Returns <see cref="NonBufferedDC"/> if API <msdn>BeginBufferedPaint</msdn> failed.
		/// </summary>
		public IntPtr DC => _dcb;

		/// <summary>
		/// Gets client area rectangle.
		/// </summary>
		public RECT ClientRect => _r;

		/// <summary>
		/// Gets bounding rectangle of the update region in client area rectangle.
		/// </summary>
		public RECT UpdateRect {
			get {
				if (_wmPaint) return _ps.rcPaint;
				Api.GetUpdateRect(_w, out var r, false);
				return r;
			}
		}

		/// <summary>
		/// Gets nonbuffered DC with API <msdn>BeginPaint</msdn> or <msdn>GetDC</msdn>. Then gets buffered DC with API <msdn>BeginBufferedPaint</msdn> for entire client area.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="wmPaint">Use API <b>BeginPaint</b>/<b>EndPaint</b>. If false, uses <b>GetDC</b>/<b>ReleaseDC</b>.</param>
		public unsafe BufferedPaint(wnd w, bool wmPaint) {
			_w = w;
			if (_wmPaint = wmPaint) {
				_dcn = Api.BeginPaint(w, out _ps);
			} else {
				_ps = default;
				_dcn = Api.GetDC(_w);
			}

			_r = _w.ClientRect;
			Api.BP_PAINTPARAMS pp = new() { cbSize = sizeof(Api.BP_PAINTPARAMS) };
			//var ru = wmPaint ? _ps.rcPaint : _r; //the buffer bitmap is smaller when rcPaint smaller, but in most cases don't need to change painting code, although GetViewportOrgEx etc get 0 offsets of the buffer DC. However problem with brush alignment.
			_hb = Api.BeginBufferedPaint(_dcn, _r, Api.BP_BUFFERFORMAT.BPBF_TOPDOWNDIB, ref pp, out _dcb); //BPBF_COMPATIBLEBITMAP slower //tested: works with 16 and 8 bit colors too
			Debug_.PrintIf(_hb == default && !_r.NoArea, "BeginBufferedPaint");
			if (_hb == default) _dcb = _dcn;
		}

		/// <summary>
		/// Calls API <msdn>EndBufferedPaint</msdn> and <msdn>EndPaint</msdn> or <msdn>ReleaseDC</msdn>.
		/// </summary>
		public void Dispose() {
			if (_dcn == default) return;
			if (_hb != default) Api.EndBufferedPaint(_hb, true);
			if (_wmPaint) Api.EndPaint(_w, _ps); else Api.ReleaseDC(_w, _dcn);
			_dcn = default;
		}
	}
}
