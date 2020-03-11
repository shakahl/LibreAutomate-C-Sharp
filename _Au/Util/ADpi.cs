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
		public static SIZE SmallIconSize { get { var t = BaseDPI / 6; return new SIZE(t, t); } }

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96, returns stretched i.
		/// Else returns i.
		/// </summary>
		/// <param name="i"></param>
		public static int ScaleInt(int i)
		{
			long dpi = BaseDPI;
			if(dpi > 96) i = (int)(i * dpi / 96);
			return i;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96, returns scaled (stretched) z.
		/// Else returns z.
		/// Note: for images use <see cref="ImageSize"/>.
		/// </summary>
		/// <param name="z"></param>
		public static SIZE ScaleSize(SIZE z)
		{
			int dpi = BaseDPI;
			if(dpi > 96) {
				z.width = (int)((long)z.width * dpi / 96);
				z.height = (int)((long)z.height * dpi / 96);
			}
			return z;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96 and image resolution is different, returns scaled (stretched) image.Size.
		/// Else returns image.Size.
		/// </summary>
		/// <param name="image"></param>
		public static SIZE ImageSize(System.Drawing.Image image)
		{
			if(image == null) return default;
			SIZE r = image.Size;
			int dpi = BaseDPI;
			if(dpi > 96) {
				r.width = (int)((long)r.width * dpi / (int)Math.Round(image.HorizontalResolution));
				r.height = (int)((long)r.height * dpi / (int)Math.Round(image.VerticalResolution));
			}
			return r;
		}

		//TEST: Win10 API GetDpiForWindow, GetSystemDpiForProcess, GetSystemMetricsForDpi.
		//	Win8.1 LogicalToPhysicalPointForPerMonitorDPI, PhysicalToLogicalPointForPerMonitorDPI.
	}
}
