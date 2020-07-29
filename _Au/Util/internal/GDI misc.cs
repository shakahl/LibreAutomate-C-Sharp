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
using System.Drawing;
using Forms = System.Windows.Forms;
//using System.Linq;

using Au;
using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Helps to get and release screen DC with the 'using(...){...}' pattern.
	/// Uses API GetDC and ReleaseDC.
	/// </summary>
	internal struct ScreenDC_ : IDisposable
	{
		IntPtr _dc;

		public ScreenDC_(int unused) => _dc = Api.GetDC(default);
		public static implicit operator IntPtr(ScreenDC_ dc) => dc._dc;
		public void Dispose() { Api.ReleaseDC(default, _dc); _dc = default; }
	}

	/// <summary>
	/// Helps to get and release window DC with the 'using(...){...}' pattern.
	/// Uses API GetDC and ReleaseDC.
	/// If w is default(AWnd), gets screen DC.
	/// </summary>
	internal struct WindowDC_ : IDisposable, IDeviceContext
	{
		IntPtr _dc;
		AWnd _w;

		public WindowDC_(IntPtr dc, AWnd w) { _dc = dc; _w = w; }

		public WindowDC_(AWnd w) => _dc = Api.GetDC(_w = w);

		public static implicit operator IntPtr(WindowDC_ dc) => dc._dc;

		public bool Is0 => _dc == default;

		public void Dispose() => ReleaseHdc();

		public IntPtr GetHdc() => _dc;

		public void ReleaseHdc() {
			Api.ReleaseDC(_w, _dc);
			_w = default; _dc = default;
		}
	}

	/// <summary>
	/// Helps to create and delete screen DC with the 'using(...){...}' pattern.
	/// Uses API CreateCompatibleDC and DeleteDC.
	/// </summary>
	internal struct CompatibleDC_ : IDisposable, IDeviceContext
	{
		IntPtr _dc;

		public CompatibleDC_(IntPtr dc) => _dc = Api.CreateCompatibleDC(dc);
		public static implicit operator IntPtr(CompatibleDC_ dc) => dc._dc;
		public bool Is0 => _dc == default;

		public void Dispose() => ReleaseHdc();

		public IntPtr GetHdc() => _dc;

		public void ReleaseHdc() {
			Api.DeleteDC(_dc);
			_dc = default;
		}
	}

	///// <summary>
	///// Misc GDI util.
	///// </summary>
	//internal static class Gdi_
	//{
	//	//rejected: now we use BufferedGraphics. Same speed. With BufferedGraphics no TextRenderer problems.
	//	///// <summary>
	//	///// Copies a .NET Bitmap to a native DC in a fast way.
	//	///// </summary>
	//	///// <remarks>
	//	///// Can be used for double-buffering: create Bitmap and Graphics from it, draw in that Graphics, then call this func.
	//	///// The bitmap should be PixelFormat.Format32bppArgb (normal), else slower etc. Must be top-down (normal).
	//	///// </remarks>
	//	//public static unsafe void CopyNetBitmapToDC(Bitmap b, IntPtr dc)
	//	//{
	//	//	var r = new Rectangle(0, 0, b.Width, b.Height);
	//	//	var d = b.LockBits(r, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
	//	//	try {
	//	//		Api.BITMAPINFOHEADER h = default;
	//	//		h.biSize = sizeof(Api.BITMAPINFOHEADER);
	//	//		h.biWidth = d.Width;
	//	//		h.biHeight = -d.Height;
	//	//		h.biPlanes = 1;
	//	//		h.biBitCount = 32;
	//	//		int k = Api.SetDIBitsToDevice(dc, 0, 0, d.Width, d.Height, 0, 0, 0, d.Height, (void*)d.Scan0, &h, 0);
	//	//		Debug.Assert(k > 0);
	//	//	}
	//	//	finally { b.UnlockBits(d); }

	//	//	//speed: 6-7 times faster than Graphics.FromHdc/DrawImageUnscaled. When testing, the dc was from BeginPaint.
	//	//}
	//	//public static unsafe void CopyNetBitmapToDC2(Bitmap b, IntPtr dc)
	//	//{
	//	//	using(var g = Graphics.FromHdc(dc)) {
	//	//		g.DrawImageUnscaled(b, 0, 0);
	//	//	}
	//	//}
	//}
}
