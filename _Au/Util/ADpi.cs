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

namespace Au.Util
{
	/// <summary>
	/// Functions for high-DPI screen support.
	/// </summary>
	/// <remarks>
	/// High DPI is when in Windows Settings is set display text size other than 100%.
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
		public static SIZE ImageSize(System.Drawing.Image image)
		{
			if(image == null) return default;
			SIZE r = image.Size;
			int dpi = BaseDPI;
			if(dpi > 96) {
				r.width = AMath.MulDiv(r.width, dpi, (int)Math.Round(image.HorizontalResolution));
				r.height = AMath.MulDiv(r.height, dpi, (int)Math.Round(image.VerticalResolution));
			}
			return r;
		}

		//TEST: Win10 API GetDpiForWindow, GetSystemDpiForProcess, GetSystemMetricsForDpi.
		//	Win8.1 LogicalToPhysicalPointForPerMonitorDPI, PhysicalToLogicalPointForPerMonitorDPI.
	}
}
