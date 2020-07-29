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
using System.Drawing;
using System.Collections.Concurrent;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Creates and manages native font handle.
	/// </summary>
	internal sealed class NativeFont_ : IDisposable
	{
		public IntPtr Handle { get; private set; }
		public int HeightOnScreen { get; private set; }

		public NativeFont_(IntPtr handle) { Handle = handle; }

		public static implicit operator IntPtr(NativeFont_ f) => f?.Handle ?? default;

		~NativeFont_() { _Dispose(); }
		public void Dispose() { _Dispose(); GC.SuppressFinalize(this); }
		void _Dispose() {
			if (Handle != default) { Api.DeleteObject(Handle); Handle = default; }
		}

		public NativeFont_(string name, int height, FontStyle style = default, bool calculateHeightOnScreen = false) {
			using var dcs = new ScreenDC_(0);
			int h2 = -AMath.MulDiv(height, Api.GetDeviceCaps(dcs, 90), 72); //LOGPIXELSY=90
			Handle = Api.CreateFont(h2,
				cWeight: style.Has(FontStyle.Bold) ? 700 : 0, //FW_BOLD
				bItalic: style.Has(FontStyle.Italic) ? 1 : 0,
				bUnderline: style.Has(FontStyle.Underline) ? 1 : 0,
				bStrikeOut: style.Has(FontStyle.Strikeout) ? 1 : 0,
				iCharSet: 1,
				pszFaceName: name);
			if (calculateHeightOnScreen) {
				using var dcMem = new CompatibleDC_(dcs);
				var of = Api.SelectObject(dcMem, Handle);
				Api.GetTextExtentPoint32(dcMem, "A", 1, out var z);
				HeightOnScreen = z.height;
				Api.SelectObject(dcMem, of);
			}
		}

		//static unsafe NativeFont_ _Create(bool bold, bool italic) {
		//	Api.NONCLIENTMETRICS m = default; m.cbSize = sizeof(Api.NONCLIENTMETRICS);
		//	Api.SystemParametersInfo(Api.SPI_GETNONCLIENTMETRICS, m.cbSize, &m, 0);
		//	if (bold) m.lfMessageFont.lfWeight = 700;
		//	if (italic) m.lfMessageFont.lfItalic = 1;
		//	return new NativeFont_(Api.CreateFontIndirect(m.lfMessageFont));
		//}

		static unsafe NativeFont_ _Create(int dpi, bool bold, bool italic) {
			Api.NONCLIENTMETRICS m = default; m.cbSize = sizeof(Api.NONCLIENTMETRICS);
			if (dpi != 0 && AVersion.MinWin10_1607) Api.SystemParametersInfoForDpi(Api.SPI_GETNONCLIENTMETRICS, m.cbSize, &m, 0, dpi);
			else Api.SystemParametersInfo(Api.SPI_GETNONCLIENTMETRICS, m.cbSize, &m, 0);
			if (bold) m.lfMessageFont.lfWeight = 700;
			if (italic) m.lfMessageFont.lfItalic = 1;
			return new NativeFont_(Api.CreateFontIndirect(m.lfMessageFont));
		}

		//flags: loword dpi, 0x10000 bold, 0x20000 italic, 0x100000 Verdana 9
		static unsafe NativeFont_ _CreateCached(int flags) {
			int dpi = flags & 0xffff;
			if (0 != (flags & 0x100000)) {
				int size = dpi == 0 ? 9 : AMath.MulDiv(9, dpi, 96);
				var style = (flags & 0x30000) switch { 0x10000 => FontStyle.Bold, 0x20000 => FontStyle.Italic, 0x30000 => FontStyle.Bold | FontStyle.Italic, _ => default };
				return new NativeFont_("Verdana", size, style, calculateHeightOnScreen: true);
			}
			return _Create(dpi, 0 != (flags & 0x10000), 0 != (flags & 0x20000));
		}

		static readonly ConcurrentDictionary<int, NativeFont_> _d = new ConcurrentDictionary<int, NativeFont_>();

		/// <summary>
		/// Cached standard font used by most windows and controls.
		/// On Windows 10 it is "Segoe UI" 9 by default.
		/// </summary>
		internal static NativeFont_ RegularCached(int dpi) => _d.GetOrAdd(dpi, i => _CreateCached(i));

		/// <summary>
		/// Cached standard bold font used by most windows and controls.
		/// </summary>
		internal static NativeFont_ BoldCached(int dpi) => _d.GetOrAdd(dpi | 0x10000, i => _CreateCached(i));

		/// <summary>
		/// Cached font "Verdana" 9.
		/// Used eg by ADialog for input Edit control.
		/// </summary>
		internal static NativeFont_ Verdana9Cached(int dpi) => _d.GetOrAdd(dpi | 0x100000, i => _CreateCached(i));
	}

	//currently not used
	///// <summary>
	///// Provides various versions of standard UI font.
	///// Per-monitor DPI-aware.
	///// </summary>
	///// <remarks>
	///// The properties return non-cached <b>Font</b> objects. It's safe to dispose them. It's OK to not dispose (GC will do; GDI+ fonts don't use much unmanaged memory).
	///// </remarks>
	//internal static class Fonts_
	//{
	//	//info: we don't return cached Font objects, because we cannot protect them from disposing. The Font class is sealed.
	//	//	SystemFonts too, always creates new object.
	//	//	But eg Brushes and SystemBrushes use cached object. It is not protected from disposing (would be exception later).

	//	/// <summary>
	//	/// Standard font used by most windows and controls.
	//	/// On Windows 10 it is "Segoe UI" 9 by default.
	//	/// </summary>
	//	public static Font Regular(int dpi) => Font.FromHfont(NativeFont_.RegularCached(dpi));

	//	/// <summary>
	//	/// Bold version of <see cref="Regular"/> font.
	//	/// </summary>
	//	public static Font Bold(int dpi) => Font.FromHfont(NativeFont_.BoldCached(dpi));
	//}
}
