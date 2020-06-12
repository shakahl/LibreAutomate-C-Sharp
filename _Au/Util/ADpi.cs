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
		/// Gets DPI of the primary screen.
		/// </summary>
		/// <remarks>
		/// On newer Windows versions, users can change DPI without logoff-logon. This function gets the setting that was after logon.
		/// </remarks>
		public static int BaseDPI {
			get {
				if(_baseDPI == 0) {
					using(var dcs = new ScreenDC_(0)) _baseDPI = Api.GetDeviceCaps(dcs, 90); //LOGPIXELSY
				}
				return _baseDPI;
			}
		}
		static int _baseDPI;

		/// <summary>
		/// Gets small icon size that depends on DPI of the primary screen.
		/// Width and Height are <see cref="BaseDPI"/>/6, which is 16 if DPI is 96 (100%).
		/// </summary>
		internal static SIZE SmallIconSize_ { get { var t = BaseDPI / 6; return new SIZE(t, t); } } //same as AIcon.SizeSmall

		/// <summary>
		/// If <see cref="BaseDPI"/> isn't 96 (100%), returns scaled i. Else returns i.
		/// </summary>
		/// <param name="i"></param>
		public static int ScaleInt(int i) => AMath.MulDiv(i, BaseDPI, 96);

		/// <summary>
		/// If <see cref="BaseDPI"/> isn't 96 (100%), returns scaled z. Else returns z.
		/// Note: for images use <see cref="ImageSize"/>.
		/// </summary>
		/// <param name="z"></param>
		public static SIZE ScaleSize(SIZE z)
		{
			int dpi = BaseDPI;
			z.width = AMath.MulDiv(z.width, dpi, 96);
			z.height = AMath.MulDiv(z.height, dpi, 96);
			return z;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> &gt; 96 (100%) and image resolution is different, returns scaled image.Size. Else returns image.Size.
		/// </summary>
		/// <param name="image"></param>
		public static SIZE ImageSize(Image image)
		{
			if(image == null) return default;
			SIZE z = image.Size;
			int dpi = BaseDPI;
			if(dpi > 96) {
				z.width = AMath.MulDiv(z.width, dpi, (int)Math.Round(image.HorizontalResolution));
				z.height = AMath.MulDiv(z.height, dpi, (int)Math.Round(image.VerticalResolution));
			}
			return z;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> &gt; 96 (100%) and image resolution is different, returns scaled copy of <i>image</i>. Else returns <i>image</i>.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="disposeOld">If performed scaling (it means created new image), dispose old image.</param>
		/// <remarks>
		/// Unlike <see cref="System.Windows.Forms.Control.ScaleBitmapLogicalToDevice"/>, returns same object if don't need scaling.
		/// </remarks>
		public static Image ScaleImage(Image image, bool disposeOld)
		{
			if(image != null) {
				int dpi = BaseDPI;
				if(dpi > 96) {
					int xRes = (int)Math.Round(image.HorizontalResolution), yRes = (int)Math.Round(image.VerticalResolution);
					//AOutput.Write(xRes, yRes, dpi);
					if(xRes != dpi || yRes != dpi) {
						var z = image.Size;
						var r = _ScaleBitmap(image, AMath.MulDiv(z.Width, dpi, xRes), AMath.MulDiv(z.Height, dpi, yRes), z);
						if(disposeOld) image.Dispose();
						image = r;
					}
				}
			}
			return image;
		}

		//From .NET DpiHelper.ScaleBitmapToSize, which is used by Control.ScaleBitmapLogicalToDevice.
		private static Bitmap _ScaleBitmap(Image oldImage, int width, int height, Size oldSize)
		{
			//note: could simply return new Bitmap(oldImage, width, height). It uses similar code, but lower quality.

			var r = new Bitmap(width, height, oldImage.PixelFormat);

			Debug.Assert(r.HorizontalResolution == BaseDPI); //if fails, need r.SetResolution

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

		//TEST: Win10 API GetDpiForWindow, GetSystemDpiForProcess, GetSystemMetricsForDpi.
		//	Win8.1 LogicalToPhysicalPointForPerMonitorDPI, PhysicalToLogicalPointForPerMonitorDPI.
	}
}
