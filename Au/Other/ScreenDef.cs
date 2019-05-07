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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Used to specify a screen (aka display, monitor) using index, window, control, <see cref="Acc"/>, point, rectangle, <see cref="Screen"/>, <see cref="OfActiveWindow"/> or <see cref="OfMouse"/>.
	/// </summary>
	/// <remarks>
	/// Used mostly for function parameters. The caller can specify screen index (int), window (Wnd etc), <see cref="Screen"/> object, etc. There are implicit conversion operators from these types. The <b>ScreenDef</b> variable holds the specified value. When the function needs screen properties, it calls <see cref="GetScreen"/> to get <see cref="Screen"/> object corresponding that value at that time.
	/// </remarks>
	public struct ScreenDef
	{
		object _o;
		ScreenDef(object o) => _o = o;

		/// <summary>
		/// Creates variable that holds <see cref="Screen"/> object. If invalid, will be used the primary screen.
		/// </summary>
		public static implicit operator ScreenDef(Screen screen) => new ScreenDef(screen);

		/// <summary>
		/// Creates variable that holds screen index. Later will be called <see cref="ScreenFromIndex"/>.
		/// </summary>
		/// <param name="screenIndex">See <see cref="ScreenFromIndex"/>.</param>
		public static implicit operator ScreenDef(int screenIndex) => new ScreenDef(screenIndex);

		/// <summary>
		/// Creates variable that holds window handle. Later will be called <see cref="ScreenFromWindow"/>. If invalid, will be used the primary screen.
		/// </summary>
		public static implicit operator ScreenDef(Wnd w) => new ScreenDef(w);

		/// <summary>
		/// Creates variable that holds <see cref="Control"/>. Later will be called <see cref="Screen.FromControl"/>.
		/// </summary>
		public static implicit operator ScreenDef(Control w) => new ScreenDef(w);

		/// <summary>
		/// Creates variable that holds <see cref="System.Windows.Window"/>. Later will be called <see cref="ScreenFromWindow"/>.
		/// </summary>
		public static implicit operator ScreenDef(System.Windows.Window w) => new ScreenDef(w);

		/// <summary>
		/// Creates variable that holds <see cref="POINT"/>. Later will be called <see cref="Screen.FromPoint"/>.
		/// </summary>
		public static implicit operator ScreenDef(POINT p) => new ScreenDef(p);

		/// <summary>
		/// Creates variable that holds <see cref="RECT"/>. Later will be called <see cref="Screen.FromRectangle"/>.
		/// </summary>
		public static implicit operator ScreenDef(RECT r) => new ScreenDef(r);

		/// <summary>
		/// Creates variable that holds <see cref="Acc"/>. Later will be called <see cref="Screen.FromRectangle"/>.
		/// </summary>
		public static implicit operator ScreenDef(Acc a) => new ScreenDef(a);

		/// <summary>
		/// Screen index of the primary screen. Value 0.
		/// Values greater than 0 are used for non-primary screens: 1 - first non-primary, 2 second...
		/// </summary>
		public const int Primary = 0;
		/// <summary>
		/// Special screen index to specify the screen of the mouse pointer. Value -1.
		/// </summary>
		public const int OfMouse = -1;
		/// <summary>
		/// Special screen index to specify the screen of the active window. Value -2.
		/// </summary>
		public const int OfActiveWindow = -2;

		//TODO: use enum for index, like now TMScreen (mouse triggers).
		/// <summary>
		/// Gets <see cref="Screen"/> object from screen index.
		/// </summary>
		/// <param name="screenIndex">
		/// Values greater than 0 are used for non-primary screens: 1 - first non-primary, 2 second...
		/// Also can be <see cref="Primary"/> (0), <see cref="OfMouse"/> (-1), <see cref="OfActiveWindow"/> (-2).
		/// If invalid, prints warning and gets the primary screen.
		/// </param>
		/// <remarks>
		/// Uses <see cref="Screen.AllScreens"/> to get screen indices. They may not match the indices that you can see in Windows Settings.
		/// </remarks>
		public static Screen ScreenFromIndex(int screenIndex)
		{
			//note: screen indices may be unexpectedly changed, breaking scripts that use them.
			//	Eg on my Win10 PC was always 1 = primary screen, but one day became 2 = primary screen.
			//	Using non-0 indices for non-primary screens mitigates this. Most multimonitor systems probably have only 2 monitors.
			//	CONSIDER: Need another way to identify screens.
			//		Eg allow to specify monitor index (or name) of each monitor (using eg a point). Let users add the code to the script template. Recorder must be able to get it from script.

			if(screenIndex > 0) {
				var a = Screen.AllScreens;
				for(int i = 0; i < a.Length; i++) {
					var s = a[i];
					if(s.Primary) continue;
					if(--screenIndex == 0) return s;
				}
				//SHOULDDO: ignore invisible pseudo-monitors associated with mirroring drivers.
				//	iScreen.AllScreens and EnumDisplayMonitors should include them,
				//	but in my recent tests with NetMeeting (noticed this long ago on an old OS version) and UltraVnc (wiki etc say) they didn't.
				//	Therefore I cannot test and add filtering. No problems if they are the last in the list. Never mind.
				//	Wiki about mirror drivers: https://en.wikipedia.org/wiki/Mirror_driver
			} else if(screenIndex == OfMouse) return Screen.FromPoint(Mouse.XY);
			else if(screenIndex == OfActiveWindow) return ScreenFromWindow(Wnd.Active);

			if(screenIndex != 0) PrintWarning("Invalid screen index.");
			return Screen.PrimaryScreen;

			//speed compared with the API monitor functions:
			//	First time several times slower, but then many times faster. .NET uses caching.
			//	Tested: correctly updates after changing multi-monitor config.
		}

		/// <summary>
		/// Gets <see cref="Screen"/> object of the screen that contains the specified window (the biggest part of it) or is nearest to it.
		/// If w handle is 0 or invalid, gets the primary screen (<see cref="Screen.FromHandle"/> would return an invalid object if the window handle is invalid).
		/// </summary>
		public static Screen ScreenFromWindow(Wnd w)
		{
			if(!w.Is0) {
				Screen R = Screen.FromHandle((IntPtr)w);
				if(!R.Bounds.IsEmpty) return R;
			}
			return Screen.PrimaryScreen;
		}

		/// <summary>
		/// Gets <see cref="Screen"/> object from the value stored in this variable (screen index, window handle, etc).
		/// If the value is null or invalid or fails, gets the primary screen.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="Screen.AllScreens"/> to get screen indices. They may not match the indices that you can see in Windows Settings.
		/// </remarks>
		public Screen GetScreen()
		{
			Screen R = null;
			switch(_o) {
			case null: break;
			case Screen scr: R = scr; break;
			case int index: return ScreenFromIndex(index);
			case Wnd w: return ScreenFromWindow(w);
			case Control c: R = Screen.FromControl(c); break;
			case POINT p: return Screen.FromPoint(p);
			case RECT r2: return Screen.FromRectangle(r2);
			case Acc acc: return Screen.FromRectangle(acc.Rect);
			///// <item><see cref="UIA.IElement"/> - gets screen of this UI Automation element (see <see cref="Screen.FromRectangle"/>).</item>
			//case UIA.IElement e: return Screen.FromRectangle(e.BoundingRectangle);
			default: return ScreenFromWindow(_Wpf());
			}
			if(R != null && !R.Bounds.IsEmpty) return R;
			return Screen.PrimaryScreen;
		}

		[MethodImpl(MethodImplOptions.NoInlining)] //prevents loading WPF dlls when don't need
		Wnd _Wpf() => (Wnd)(System.Windows.Window)_o;

		/// <summary>
		/// Gets the value as object.
		/// </summary>
		public object Value => _o;

		/// <summary>
		/// true if this variable does not have a value.
		/// </summary>
		public bool IsNull => _o == null;

		/// <summary>
		/// Gets primary screen width.
		/// </summary>
		public static int PrimaryWidth => Api.GetSystemMetrics(Api.SM_CXSCREEN);
		//public static int PrimaryWidth => Screen.PrimaryScreen.Bounds.Width; //faster (gets cached value), but very slow first time, eg 15 ms

		/// <summary>
		/// Gets primary screen height.
		/// </summary>
		public static int PrimaryHeight => Api.GetSystemMetrics(Api.SM_CYSCREEN);

		/// <summary>
		/// Gets primary screen rectangle.
		/// </summary>
		public static RECT PrimaryRect => (0, 0, PrimaryWidth, PrimaryHeight, false);

		/// <summary>
		/// Gets screen rectangle.
		/// </summary>
		/// <param name="screen">Screen index etc. If default - primary screen.</param>
		/// <param name="workArea">Get work area rectangle.</param>
		public static RECT GetRect(ScreenDef screen = default, bool workArea = false)
		{
			RECT r;
			if(screen.IsNull) {
				r = workArea ? PrimaryWorkArea : PrimaryRect;
			} else {
				var x = screen.GetScreen();
				r = workArea ? x.WorkingArea : x.Bounds;
			}
			return r;
		}

		/// <summary>
		/// Gets primary screen work area.
		/// </summary>
		public static unsafe RECT PrimaryWorkArea
		{
			get
			{
				RECT r;
				Api.SystemParametersInfo(Api.SPI_GETWORKAREA, 0, (void*)&r, 0);
				return r;
			}
		}

		/// <summary>
		/// Returns true if point p is in some screen.
		/// </summary>
		public static bool IsInAnyScreen(POINT p)
		{
			return Api.MonitorFromPoint(p, Api.MONITOR_DEFAULTTONULL) != default;
		}

		/// <summary>
		/// Returns true if rectangle r intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(RECT r)
		{
			return Api.MonitorFromRect(r, 0) != default;
		}

		/// <summary>
		/// Returns true if rectangle of window w intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(Wnd w)
		{
			return Api.MonitorFromWindow(w, 0) != default;
		}
	}

	public static partial class ExtOther
	{
		/// <summary>
		/// Gets screen index that can be used with <see cref="ScreenDef"/> and functions that use it.
		/// Primary screen is 0. Values greater than 0 are used for non-primary screens: 1 - first non-primary, 2 second...
		/// </summary>
		/// <param name="t"></param>
		/// <exception cref="AuException">Failed (probably the Screen object is invalid).</exception>
		public static int GetIndex_(this Screen t)
		{
			if(t.Primary) return 0;
			var a = Screen.AllScreens;
			for(int j = 0; j < 2; j++) {
				for(int i = 0, index = 0; i < a.Length; i++) {
					var s = a[i];
					if(s.Primary) continue;
					index++;
					bool found = (j == 0) ? (s == t) : (s.GetHashCode() == t.GetHashCode());
					if(found) return index;
					//GetHashCode is HMONITOR, but it is undocumented.
					//When changed display settings, AllScreens objects are replaced with new objects. HMONITOR in most cases are the same. 
				}
			}
			throw new AuException();
		}
	}
}
