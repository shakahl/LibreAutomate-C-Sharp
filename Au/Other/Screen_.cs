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
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Extends <see cref="Screen"/>.
	/// </summary>
	public static class Screen_
	{
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

		/// <summary>
		/// Gets <see cref="Screen"/> object from screen index.
		/// </summary>
		/// <param name="index">
		/// Values greater than 0 are used for non-primary screens: 1 - first non-primary, 2 second...
		/// Also can be <see cref="Primary"/> (0), <see cref="OfMouse"/> (-1), <see cref="OfActiveWindow"/> (-2).
		/// </param>
		/// <param name="noThrow">Don't throw exception if index is invalid. Instead show warning and return null.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <remarks>
		/// Uses <see cref="Screen.AllScreens"/> to get screen indices. They may not match the indices that you can see in Control Panel.
		/// </remarks>
		public static Screen FromIndex(int index, bool noThrow = false)
		{
			//note: screen indices may be unexpectedly changed, breaking scripts that use them.
			//	Eg on my Win10 PC was always 1 = primary screen, but one day become 2 = primary screen.
			//	Using non-0 indices for non-primary screens mitigates this. Most multimonitor systems probably have only 2 monitors.
			//	CONSIDER: Need another way to identify screens.
			//		Eg allow to specify monitor index (or name) of each monitor (using eg a point). Let users add the code to the script template. Recorder must be able to get it from script.

			if(index > 0) {
				var a = Screen.AllScreens;
				for(int i = 0; i < a.Length; i++) {
					var s = a[i];
					if(s.Primary) continue;
					if(--index == 0) return s;
				}
				//SHOULDDO: ignore invisible pseudo-monitors associated with mirroring drivers.
				//	iScreen.AllScreens and EnumDisplayMonitors should include them,
				//	but in my recent tests with NetMeeting (noticed this long ago on an old OS version) and UltraVnc (wiki etc say) they didn't.
				//	Therefore I cannot test and add filtering. No problems if they are the last in the list. Never mind.
				//	Wiki about mirror drivers: https://en.wikipedia.org/wiki/Mirror_driver
			} else if(index == OfMouse) return Screen.FromPoint(Mouse.XY);
			else if(index == OfActiveWindow) return FromWindow(Wnd.WndActive);

			if(index != 0) {
				if(!noThrow) throw new ArgumentOutOfRangeException(null, "Invalid screen index.");
				PrintWarning("Invalid screen index.");
				return null;
			}
			return Screen.PrimaryScreen;

			//speed compared with the API monitor functions:
			//	First time several times slower, but then many times faster. It means that .NET uses caching.
			//	Tested: correctly updates after changing multi-monitor config.
		}

		/// <summary>
		/// Gets screen index that can be used with <see cref="FromIndex"/> (also with FromObject and some other functions of this library).
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

		/// <summary>
		/// Gets <see cref="Screen"/> object of the screen that contains the specified window (the biggest part of it) or is nearest to it.
		/// If w handle is 0 or invalid, gets the primary screen (<see cref="Screen.FromHandle"/> would return an invalid object if the window handle is invalid).
		/// </summary>
		public static Screen FromWindow(Wnd w)
		{
			if(!w.Is0) {
				Screen R = Screen.FromHandle((IntPtr)w);
				if(!R.Bounds.IsEmpty) return R;
			}
			return Screen.PrimaryScreen;
		}

		/// <summary>
		/// Gets <see cref="Screen"/> object by index or of the screen that contains (or is nearest to) the specified window, rectangle or point.
		/// </summary>
		/// <param name="screen">
		/// Depends on type:
		/// <list type="bullet">
		/// <item>null - gets primary screen.</item>
		/// <item><see cref="Screen"/> - returns it if valid. If invalid, gets primary screen.</item>
		/// <item>int - 1-based non-primary screen index (see <see cref="FromIndex"/>), or Screen_.Primary (0), Screen_.OfMouse (-1), Screen_.OfActiveWindow (-2).</item>
		/// <item><see cref="Wnd"/> - gets screen of this window or control (see <see cref="FromWindow"/>). If invalid, gets primary screen.</item>
		/// <item><see cref="Control"/> - gets screen of this .NET form or control (see <see cref="Screen.FromControl"/>). If invalid, gets primary screen.</item>
		/// <item><see cref="Point"/> - gets screen of this point (see <see cref="Screen.FromPoint"/>).</item>
		/// <item><see cref="Rectangle"/>, <see cref="RECT"/> - gets screen of this rectangle (see <see cref="Screen.FromRectangle"/>).</item>
		/// <item><see cref="Acc"/> - gets screen of this accessible object (see <see cref="Screen.FromRectangle"/>).</item>
		/// </list>
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <exception cref="ArgumentException">Bad object type (not one from the above list).</exception>
		/// <remarks>
		/// If something fails (except if invalid index), gets primary screen.
		/// Uses <see cref="Screen.AllScreens"/> to get screen indices. They may not match the indices that you can see in Control Panel.
		/// </remarks>
		public static Screen FromObject(object screen)
		{
			Screen R = null;
			switch(screen) {
			case null: break;
			case Screen scr: R = scr; break;
			case int index: return FromIndex(index);
			case Wnd w: return FromWindow(w);
			case Control c: R = Screen.FromControl(c); break;
			case Point p: return Screen.FromPoint(p);
			case Rectangle r1: return Screen.FromRectangle(r1);
			case RECT r2: return Screen.FromRectangle(r2);
			case Acc acc: return Screen.FromRectangle(acc.Rect);
		///// <item><see cref="UIA.IElement"/> - gets screen of this UI Automation element (see <see cref="Screen.FromRectangle"/>).</item>
			//case UIA.IElement e: return Screen.FromRectangle(e.BoundingRectangle);
			default: throw new ArgumentException("Bad object type.");
			}
			if(R != null && !R.Bounds.IsEmpty) return R;
			return Screen.PrimaryScreen;
		}

		/// <summary>
		/// Gets primary screen width.
		/// </summary>
		public static int Width => Api.GetSystemMetrics(Api.SM_CXSCREEN);
		/// <summary>
		/// Gets primary screen height.
		/// </summary>
		public static int Height => Api.GetSystemMetrics(Api.SM_CYSCREEN);
		//public static int Width => Screen.PrimaryScreen.Bounds.Width; //faster (gets cached value), but very slow first time, eg 15 ms

		/// <summary>
		/// Gets primary screen rectangle.
		/// </summary>
		public static RECT Rect => new RECT(0, 0, Width, Height, false);

		/// <summary>
		/// Gets screen rectangle.
		/// </summary>
		/// <param name="screen">Screen index etc, see <see cref="FromObject"/>. If null - primary screen.</param>
		/// <param name="workArea">Get work area rectangle.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		public static RECT GetRect(object screen = null, bool workArea = false)
		{
			RECT r;
			Screen scr = screen == null ? null : FromObject(screen);
			if(scr != null) r = workArea ? scr.WorkingArea : scr.Bounds;
			else r = workArea ? WorkArea : Rect;
			return r;
		}

		/// <summary>
		/// Gets primary screen work area.
		/// </summary>
		public static unsafe RECT WorkArea
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
		public static bool IsInAnyScreen(Point p)
		{
			return Api.MonitorFromPoint(p, 0) != default; //0 - MONITOR_DEFAULTTONULL
		}

		/// <summary>
		/// Returns true if rectangle r intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(RECT r)
		{
			return Api.MonitorFromRect(ref r, 0) != default;
		}

		/// <summary>
		/// Returns true if rectangle of window w intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(Wnd w)
		{
			return Api.MonitorFromWindow(w, 0) != default;
		}
	}
}
