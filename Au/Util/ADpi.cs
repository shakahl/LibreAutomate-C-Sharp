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
	/// To find DPI % on Windows 10: Settings -> System -> Display -> Scale and layout. If not 100%, it means high DPI. On older Windows versions it is in Control Panel -> Display.
	/// 
	/// This program must be per-monitor-DPI-aware. Alse the results are undefined.
	/// </remarks>
	public static class ADpi
	{
		/// <summary>
		/// Gets DPI of the primary screen at the time this process started.
		/// </summary>
		public static int System {
			get {
				if (_systemDPI == 0) {
					using var dcs = new ScreenDC_();
					_systemDPI = Api.GetDeviceCaps(dcs, 90); //LOGPIXELSY
				}
				return _systemDPI;
			}
		}
		static int _systemDPI;

		/// <summary>
		/// Gets DPI of a window.
		/// </summary>
		/// <param name="w">Top-level window or control. Can belong to any process.</param>
		/// <param name="ofScreen">
		/// If true, the function gets DPI for which the window is (or should be) scaled, either by the program or by the OS; it depends on window's current screen DPI.
		/// If false (default), gets DPI used internally by the window's program; it does not depend on whether the window is OS-scaled; it never changes for that window instance.
		/// </param>
		/// <remarks>
		/// If <i>ofScreen</i> is false (default), the result depends on the DPI awareness of the window:
		/// - per-monitor-DPI-aware - usually DPI of windows's screen.
		/// - system aware - system DPI (DPI of primary screen).
		/// - unaware - 96.
		/// 
		/// To get window's internal DPI, on Windows 10 1607 and later uses API <msdn>GetDpiForWindow</msdn>. On Windows 8.1 and early Windows 10, if <see cref="SupportPMOnWin8_1"/> is true, uses API <msdn>GetDpiForMonitor</msdn>. Older Windows versions don't support per-monitor DPI.
		/// 
		/// Returns the system DPI (<see cref="System"/>) if fails or if not supported on this Windows version (see <see cref="SupportPMOnWin8_1"/>).
		/// 
		/// On Windows 7 and 8.0 always returns the system DPI, because of lack of Windows API; it is always correct only if <i>ofScreen</i> is true.
		/// </remarks>
		public static int OfWindow(AWnd w, bool ofScreen = false) {
			if (!AVersion.MinWin8_1) return System;
			int R = 0;
			if (!w.Is0) {
				if (ofScreen) {
					//	if(Api.GetSystemMetrics(SM_CMONITORS)<2) return AScreen.Primary.Dpi; //slow
					switch (WindowDpiAwareness(w)) {
					case Awareness.PerMonitor: break;
					case Awareness.Invalid: return System;
					default: return _OfScreen(w);
					}

					//Good: when a non-pm-dpi-aware window is in 2 screens, MonitorFromWindow(w) gets screen that matches window's DPI scaling.
					//	It can be different than MonitorFromRect(w.Rect). Tested on Win10; never mind Win8.1.

					//Windows bug: if a non-pm-dpi-aware program is set pm-dpi-aware (in file Properties dialog), after changing screen DPI the API still returns old value.
					//	Works correctly when window DPI changes when moving between screens.
					//	Never mind. This is rare*rare.
				}
				if (AVersion.MinWin10_1607) R = Api.GetDpiForWindow(w);
				else if (ofScreen || SupportPMOnWin8_1) return _OfScreen(w);
			}
			return R != 0 ? R : System;

			static int _OfScreen(AWnd w) {
				var hm = Api.MonitorFromWindow(w.Window, SODefault.Nearest);
				return 0 == Api.GetDpiForMonitor(hm, 0, out int R, out _) ? R : System;
			}
		}

		/// <summary>
		/// Returns <c>OfWindow(w.Hwnd())</c>.
		/// </summary>
		public static int OfWindow(System.Windows.Forms.Control w) => OfWindow(w.Hwnd());

		/// <summary>
		/// Returns <c>OfWindow(w.Hwnd())</c>.
		/// </summary>
		public static int OfWindow(System.Windows.DependencyObject w) => OfWindow(w.Hwnd());

		/// <summary>
		/// Gets DPI of a screen.
		/// Returns <see cref="System"/> if fails or if not supported on this Windows version (see <see cref="SupportPMOnWin8_1"/>).
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>GetDpiForMonitor</msdn>.
		/// </remarks>
		/// <seealso cref="AScreen.Of"/>
		public static int OfScreen(IntPtr hMonitor) {
			return
				(SupportPMOnWin8_1 ? AVersion.MinWin8_1 : AVersion.MinWin10_1607) && 0 == Api.GetDpiForMonitor(hMonitor, 0, out int R, out _)
				? R
				: System;
		}

		/// <summary>
		/// If true, <see cref="OfScreen"/> and <see cref="OfWindow"/> will support per-monitor DPI on Windows 8.1 and later.
		/// If false (default) - on Windows 10 1607 and later; on 8.1 will always return <see cref="System"/>.
		/// </summary>
		/// <remarks>
		/// This is a per-thread property. You can change/restore it before/after calling these functions.
		/// Before Windows 10 1607 it is difficult to support per-monitor DPI because of lack of API and OS/.NET support.
		/// </remarks>
		[field: ThreadStatic]
		public static bool SupportPMOnWin8_1 { get; set; }

		///// <summary>
		///// Gets small icon size that depends on DPI of the primary screen.
		///// Width and Height are <see cref="OfThisProcess"/>/6, which is 16 if DPI is 96 (100%).
		///// </summary>
		//internal static SIZE SmallIconSize_ { get { var t = OfThisProcess / 6; return new SIZE(t, t); } } //same as AIcon.SizeSmall

		/// <summary>
		/// Scales <b>int</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static int Scale(int i, DpiOf dpiOf) => AMath.MulDiv(i, dpiOf, 96);

		//no. Eg also would be used for uint, long... Or name eg ScaleD. Or add double extension method.
		///// <summary>
		///// Scales <b>int</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		///// </summary>
		//public static int Scale(double i, DpiOf dpiOf) => (i*(int)dpiOf/96).ToInt();

		/// <summary>
		/// Unscales <b>int</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static double Unscale(int i, DpiOf dpiOf) => i * (96d / dpiOf);
		//Unscaling sometimes useful with WPF. Unscale to double, not int, else result often incorrect.

		/// <summary>
		/// Scales <b>SIZE</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static SIZE Scale(SIZE z, DpiOf dpiOf) {
			int dpi = dpiOf;
			z.width = AMath.MulDiv(z.width, dpi, 96);
			z.height = AMath.MulDiv(z.height, dpi, 96);
			return z;
		}

		/// <summary>
		/// Scales <b>System.Windows.Size</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static SIZE Scale(System.Windows.Size z, DpiOf dpiOf) {
			double f = (int)dpiOf / 96d;
			z.Width *= f;
			z.Height *= f;
			return new(z.Width.ToInt(), z.Height.ToInt());
		}

		/// <summary>
		/// Unscales <b>SIZE</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static System.Windows.Size Unscale(SIZE z, DpiOf dpiOf) {
			double f = 96d / dpiOf;
			return new(z.width * f, z.height * f);
		}

		/// <summary>
		/// Scales <b>RECT</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static RECT Scale(RECT r, DpiOf dpiOf) {
			int dpi = dpiOf;
			r.left = AMath.MulDiv(r.left, dpi, 96);
			r.top = AMath.MulDiv(r.top, dpi, 96);
			r.right = AMath.MulDiv(r.right, dpi, 96);
			r.bottom = AMath.MulDiv(r.bottom, dpi, 96);
			return r;
		}

		/// <summary>
		/// Unscales <b>RECT</b> if <i>dpiOf.Dpi</i> isn't 96 (100%).
		/// </summary>
		public static System.Windows.Rect Unscale(RECT r, DpiOf dpiOf) {
			double f = 96d / dpiOf;
			return new(r.left * f, r.top * f, r.Width * f, r.Height * f);
		}

		/// <summary>
		/// Calls API <msdn>GetSystemMetricsForDpi</msdn> if available, else <msdn>GetSystemMetrics</msdn>.
		/// </summary>
		public static int GetSystemMetrics(int nIndex, DpiOf dpiOf)
			=> AVersion.MinWin10_1607
			? Api.GetSystemMetricsForDpi(nIndex, dpiOf)
			: Api.GetSystemMetrics(nIndex);

		/// <summary>
		/// Calls API <msdn>SystemParametersInfoForDpi</msdn> if available, else <msdn>SystemParametersInfo</msdn>.
		/// Use only with <i>uiAction</i> = SPI_GETICONTITLELOGFONT, SPI_GETICONMETRICS, SPI_GETNONCLIENTMETRICS.
		/// </summary>
		public static bool SystemParametersInfo(uint uiAction, int uiParam, LPARAM pvParam, DpiOf dpiOf)
			=> AVersion.MinWin10_1607
			? Api.SystemParametersInfoForDpi(uiAction, uiParam, pvParam, 0, dpiOf)
			: Api.SystemParametersInfo(uiAction, uiParam, pvParam);

		/// <summary>
		/// Calls API <msdn>AdjustWindowRectExForDpi</msdn> if available, else <msdn>AdjustWindowRectEx</msdn>.
		/// </summary>
		/// <remarks>
		/// Also adds scrollbar width or/and height if need.
		/// </remarks>
		public static bool AdjustWindowRectEx(DpiOf dpiOf, ref RECT r, WS style, WS2 exStyle, bool hasMenu = false) {
			int dpi = dpiOf;
			bool ok=AVersion.MinWin10_1607
				? Api.AdjustWindowRectExForDpi(ref r, style, hasMenu, exStyle, dpi)
				: Api.AdjustWindowRectEx(ref r, style, hasMenu, exStyle);
			if (ok) {
				if (style.Has(WS.VSCROLL)) r.Width += ScrollbarV_(dpi);
				if (style.Has(WS.HSCROLL)) r.Width += ScrollbarH_(dpi);
			}
			return ok;
		}

		internal static int ScrollbarV_(int dpi) => GetSystemMetrics(Api.SM_CXVSCROLL, dpi);
		internal static int ScrollbarH_(int dpi) => GetSystemMetrics(Api.SM_CYHSCROLL, dpi);

		/// <summary>
		/// DPI awareness of a window or process.
		/// </summary>
		public enum Awareness //== DPI_AWARENESS == PROCESS_DPI_AWARENESS
		{
			///
			Invalid = -1,
			///
			Unaware,
			///
			System,
			///
			PerMonitor
		}

		/// <summary>
		/// Gets DPI awareness of a window.
		/// </summary>
		/// <remarks>
		/// On Windows 7 and 8.0 always returns <b>System</b>, because of lack of Windows API.
		/// </remarks>
		public static Awareness WindowDpiAwareness(AWnd w) {
			if (AVersion.MinWin10_1607) {
				var c = Api.GetWindowDpiAwarenessContext(w);
				return Api.GetAwarenessFromDpiAwarenessContext(c);
			} else if (AVersion.MinWin8_1) {
				using var hp = Handle_.OpenProcess(w);
				return (!hp.Is0 && 0 == Api.GetProcessDpiAwareness(hp, out var a)) ? a : Awareness.Invalid;
			} else {
				return _IsWindowScaledWin7(w) switch { true => Awareness.Unaware, false => Awareness.System, _ => Awareness.Invalid };
			}
		}

		static bool? _IsWindowScaledWin7(AWnd w) {
#if DPI_WIN7
			if(ADpi.OfThisProcess==96) return w.IsAlive ? false : null;
	
			bool retry=false;
			g1:
			//test a point in hwnd most far from screen 0 0. Never mind: cannot detect DPI-scaled window if the point is 0 0 or near. 
			if(!w.GetRect(out var r)) return null;
			POINT p=new(Math.Abs(r.left)>Math.Abs(r.right) ? r.left : r.right, Math.Abs(r.top)>Math.Abs(r.bottom) ? r.top : r.bottom), pp=p;
			if(LogicalToPhysicalPoint(w, ref p)) return p!=pp;
			//tested: the logical/physical conversion API work with hidden and off-screen top-level windows too. Minimized windows are off-screen.
	
			//if control, retry with top-level window. LTPP fails if control is zero-size or not in window rect.
			if(!retry) {
				retry=true;
				var tl=w.Window;
				if(tl.Is0) return null;
				if(tl!=w) { w=tl; goto g1; }
			}
			//maybe the window moved/resized while testing. Then LTPP fails if the point is not in the window.
			if(!w.GetRect(out var rr) || rr==r) return null;
			goto g1;
#else
			return w.IsAlive ? false : null;
#endif
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
			public AwarenessContext(nint dpiContext) {
				_dac = AVersion.MinWin10_1607 ? Api.SetThreadDpiAwarenessContext(dpiContext) : default;
			}

			/// <summary>
			/// Restores previous DPI awareness context.
			/// </summary>
			public void Dispose() {
				if (_dac == default) return;
				Api.SetThreadDpiAwarenessContext(_dac);
				_dac = default;
			}

			///
			public const nint DPI_AWARENESS_CONTEXT_UNAWARE = -1;
			///
			public const nint DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2;
			///
			public const nint DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3;
			///
			public const nint DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;
			///
			public const nint DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5;
		}
	}
}

namespace Au.Types
{
	using Au.Util;

	/// <summary>
	/// Used for <i>DPI</i> parameter of functions.
	/// Has implicit conversions from int (DPI), AWnd (DPI of window), IntPtr (DPI of screen handle), POINT (DPI of screen containing point), RECT (DPI of screen containing rectangle), forms Control, WPF DependencyObject. The conversion operators set the <see cref="Dpi"/> property and the function can use it.
	/// </summary>
	public struct DpiOf
	{
		int _dpi;

		///
		public DpiOf(int dpi) { _dpi = dpi; }

		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public DpiOf(AWnd w) {
			w.ThrowIfInvalid();
			_dpi = ADpi.OfWindow(w);
		}

		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public DpiOf(System.Windows.Forms.Control c) : this(c?.Hwnd() ?? throw new ArgumentNullException()) { }

		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public DpiOf(System.Windows.DependencyObject c) : this(c?.Hwnd() ?? throw new ArgumentNullException()) { }

		///
		public DpiOf(IntPtr hMonitor) { _dpi = ADpi.OfScreen(hMonitor); }

		///
		public DpiOf(POINT screenOf) : this(AScreen.Of(screenOf).Handle) { }

		///
		public DpiOf(RECT screenOf) : this(AScreen.Of(screenOf).Handle) { }

		///
		public static implicit operator DpiOf(int dpi) => new(dpi);

		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public static implicit operator DpiOf(AWnd w) => new(w);

		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public static implicit operator DpiOf(System.Windows.Forms.Control c) => new(c);

		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public static implicit operator DpiOf(System.Windows.DependencyObject c) => new(c);

		///
		public static implicit operator DpiOf(IntPtr hMonitor) => new(hMonitor);

		///
		public static implicit operator DpiOf(POINT screenOf) => new(screenOf);

		///
		public static implicit operator DpiOf(RECT screenOf) => new(screenOf);

		///
		public int Dpi => _dpi;

		///
		public static implicit operator int(DpiOf d) => d._dpi;
	}
}