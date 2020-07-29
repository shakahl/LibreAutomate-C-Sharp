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

using Au.Types;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Au.Util
{
	/// <summary>
	/// Functions for high-DPI screen support.
	/// </summary>
	/// <remarks>
	/// To find DPI % on Windows 10: Settings -> System -> Display -> Scale and layout -> Change the size. If not 100%, it means high DPI. On older Windows versions it is in Control Panel -> Display.
	/// Currently this class and this library don't support multiple screens that have different DPI. The Windows OS supports it since version 8.1.
	/// </remarks>
	public static class ADpi
	{
		/// <summary>
		/// Gets DPI of this process.
		/// </summary>
		/// <remarks>
		/// It is DPI of the primary screen when this process started. Used by API that aren't per-monitor DPI aware.
		/// </remarks>
		public static int OfThisProcess {
			get {
				if (_baseDPI == 0) {
					using (var dcs = new ScreenDC_(0)) _baseDPI = Api.GetDeviceCaps(dcs, 90); //LOGPIXELSY
				}
				return _baseDPI;
			}
		}
		static int _baseDPI;

		/// <summary>
		/// Gets DPI of a window.
		/// Returns <see cref="OfThisProcess"/> if fails or if Windows version is less than 8.1.
		/// </summary>
		/// <remarks>
		/// On Windows 10 1607 and later uses API <msdn>GetDpiForWindow</msdn>. On Windows 8.1 and early Windows 10 uses API <msdn>GetDpiForMonitor</msdn>; it's less reliable. Older Windows versions don't support multiple screens with different DPI, therefore returns the system DPI.
		/// </remarks>
		public static int OfWindow(AWnd w) {
			int R = 0;
			if (!w.Is0) {
				if (AVersion.MinWin10_1607) R = Api.GetDpiForWindow(w);
				else if (AVersion.MinWin8_1 && w.GetRect(out var k)) return OfScreen(AScreen.Of(k).Handle);
			}
			return R != 0 ? R : OfThisProcess;
		}//TODO: by default don't support Win8.1. Add param supportWin81.

		/// <summary>
		/// Returns <c>OfWindow(w.Hwnd())</c>.
		/// </summary>
		public static int OfWindow(System.Windows.Forms.Control w) => OfWindow(w.Hwnd());

		/// <summary>
		/// Returns <c>OfWindow(w.Hwnd())</c>.
		/// </summary>
		public static int OfWindow(System.Windows.DependencyObject w) => OfWindow(w.Hwnd());

		///// <summary>
		///// Let <see cref="OfWindow"/> and 
		///// </summary>
		//public static bool SupportPerMonitorDpiOnWin8_1 { get; set; }

		///// <summary>
		///// Gets DPI of a window (API <msdn>GetDpiForWindow</msdn>).
		///// Returns <see cref="OfThisProcess"/> if fails or if the API is unavailable (added in Windows 10 1607, year 2016).
		///// </summary>
		//public static int OfWindow(AWnd w) {
		//	int R = AVersion.MinWin10_1607 ? Api.GetDpiForWindow(w) : 0;
		//	return R != 0 ? R : OfThisProcess;
		//}

		/// <summary>
		/// Gets DPI of a screen.
		/// Returns <see cref="OfThisProcess"/> if fails or if Windows version is less than 8.1.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>GetDpiForMonitor</msdn>.
		/// </remarks>
		public static int OfScreen(IntPtr hmon) {
			return AVersion.MinWin8_1 && 0 == Api.GetDpiForMonitor(hmon, 0, out int R, out _) ? R : OfThisProcess;
		}

		///// <summary>
		///// Gets small icon size that depends on DPI of the primary screen.
		///// Width and Height are <see cref="OfThisProcess"/>/6, which is 16 if DPI is 96 (100%).
		///// </summary>
		//internal static SIZE SmallIconSize_ { get { var t = OfThisProcess / 6; return new SIZE(t, t); } } //same as AIcon.SizeSmall

		/// <summary>
		/// If <i>dpi</i> isn't 96 (100%), returns scaled i. Else returns i.
		/// </summary>
		public static int Scale(int i, int dpi) => AMath.MulDiv(i, dpi, 96);

		/// <summary>
		/// If DPI <see cref="OfWindow"/> isn't 96 (100%), returns scaled i. Else returns i.
		/// </summary>
		public static int Scale(int i, AWnd w) => AMath.MulDiv(i, OfWindow(w), 96);

		/// <summary>
		/// If DPI <see cref="OfThisProcess"/> isn't 96 (100%), returns scaled i. Else returns i.
		/// </summary>
		public static int Scale(int i) => AMath.MulDiv(i, OfThisProcess, 96);

		/// <summary>
		/// If <i>dpi</i> isn't 96 (100%), returns scaled z. Else returns z.
		/// </summary>
		public static SIZE ScaleSize(SIZE z, int dpi) {
			z.width = AMath.MulDiv(z.width, dpi, 96);
			z.height = AMath.MulDiv(z.height, dpi, 96);
			return z;
		}

		/// <summary>
		/// If DPI <see cref="OfWindow"/> isn't 96 (100%), returns scaled z. Else returns z.
		/// </summary>
		public static SIZE ScaleSize(SIZE z, AWnd w) => ScaleSize(z, OfWindow(w));

		/// <summary>
		/// If DPI <see cref="OfThisProcess"/> isn't 96 (100%), returns scaled z. Else returns z.
		/// </summary>
		public static SIZE ScaleSize(SIZE z) => ScaleSize(z, OfThisProcess);

		/// <summary>
		/// If <see cref="OfThisProcess"/> &gt; 96 (100%) and image resolution is different, returns scaled image.Size. Else returns image.Size.
		/// </summary>
		/// <param name="image"></param>
		public static SIZE ImageSize(Image image) {
			if (image == null) return default;
			SIZE z = image.Size;
			int dpi = OfThisProcess;
			if (dpi > 96) {
				z.width = AMath.MulDiv(z.width, dpi, (int)Math.Round(image.HorizontalResolution));
				z.height = AMath.MulDiv(z.height, dpi, (int)Math.Round(image.VerticalResolution));
			}
			return z;
		}

		/// <summary>
		/// If <see cref="OfThisProcess"/> &gt; 96 (100%) and image resolution is different, returns scaled copy of <i>image</i>. Else returns <i>image</i>.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="disposeOld">If performed scaling (it means created new image), dispose old image.</param>
		/// <remarks>
		/// Unlike <see cref="System.Windows.Forms.Control.ScaleBitmapLogicalToDevice"/>, returns same object if don't need scaling.
		/// </remarks>
		public static Image ScaleImage(Image image, bool disposeOld) {
			if (image != null) {
				int dpi = OfThisProcess;
				if (dpi > 96) {
					int xRes = (int)Math.Round(image.HorizontalResolution), yRes = (int)Math.Round(image.VerticalResolution);
					//AOutput.Write(xRes, yRes, dpi);
					if (xRes != dpi || yRes != dpi) {
						var z = image.Size;
						var r = _ScaleBitmap(image, AMath.MulDiv(z.Width, dpi, xRes), AMath.MulDiv(z.Height, dpi, yRes), z);
						if (disposeOld) image.Dispose();
						image = r;
					}
				}
			}
			return image;
		}

		//From .NET DpiHelper.ScaleBitmapToSize, which is used by Control.ScaleBitmapLogicalToDevice.
		private static Bitmap _ScaleBitmap(Image oldImage, int width, int height, Size oldSize) {
			//note: could simply return new Bitmap(oldImage, width, height). It uses similar code, but lower quality.

			var r = new Bitmap(width, height, oldImage.PixelFormat);

			Debug.Assert(r.HorizontalResolution == OfThisProcess); //if fails, need r.SetResolution

			using var graphics = Graphics.FromImage(r);
			var mode = InterpolationMode.HighQualityBicubic;
			//if(width % oldSize.Width == 0 && height % oldSize.Height == 0) mode = InterpolationMode.NearestNeighbor; //DpiHelper does it, but maybe it isn't a good idea
			graphics.InterpolationMode = mode;
			graphics.CompositingQuality = CompositingQuality.HighQuality;

			var sourceRect = new RectangleF(-0.5f, -0.5f, oldSize.Width, oldSize.Height);
			var destRect = new RectangleF(0, 0, width, height);

			graphics.DrawImage(oldImage, destRect, sourceRect, GraphicsUnit.Pixel);

			return r;
		}

		/// <summary>
		/// Can be used to temporarily change thread's DPI awareness context with API <msdn>SetThreadDpiAwarenessContext</msdn>.
		/// Does nothing if the API is unavailable (added in Windows 10 version 1607).
		/// </summary>
		/// <remarks>
		/// Programs that use this library should use manifest with dpiAwareness = PerMonitorV2 and dpiAware = True/PM. Then default DPI awareness context is DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// using var dac = new ADpi.AwarenessContext(ADpi.AwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE);
		/// ]]></code>
		/// </example>
		public struct AwarenessContext : IDisposable
		{
			LPARAM _dac;

			/// <summary>
			/// Calls API <msdn>SetThreadDpiAwarenessContext</msdn> if available.
			/// </summary>
			/// <param name="dpiContext">One of <b>DPI_AWARENESS_CONTEXT_X</b> constants, for example <see cref="DPI_AWARENESS_CONTEXT_UNAWARE"/>.</param>
			public AwarenessContext(LPARAM dpiContext) {
				_dac = AVersion.MinWin10_1607 ? Api.SetThreadDpiAwarenessContext(dpiContext) : default;
			}

			/// <summary>
			/// Restores previous DPI awareness context.
			/// </summary>
			public void Dispose() {
				if(_dac==default) return;
				Api.SetThreadDpiAwarenessContext(_dac);
				_dac = default;
			}

			///
			public const int DPI_AWARENESS_CONTEXT_UNAWARE = -1;
			///
			public const int DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2;
			///
			public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3;
			///
			public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;
			///
			public const int DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5;
		}
	}
}
