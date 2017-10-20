using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Extends the .NET class Screen.
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
				Output.Warning("Invalid screen index.");
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
		/// <exception cref="CatException">Failed (probably the Screen object is invalid).</exception>
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
			throw new CatException();
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
		/// <item>null: primary screen.</item>
		/// <item>Screen: a <see cref="Screen"/> object. If invalid, gets primary screen.</item>
		/// <item>int: 1-based non-primary screen index (see <see cref="FromIndex"/>), or Screen_.Primary (0), Screen_.OfMouse (-1), Screen_.OfActiveWindow (-2).</item>
		/// <item>Wnd: a window (see <see cref="FromWindow"/>). If invalid, gets primary screen.</item>
		/// <item>Control: a .NET form or control (see <see cref="Screen.FromControl"/>). If invalid, gets primary screen.</item>
		/// <item>Point: a point (see <see cref="Screen.FromPoint"/>).</item>
		/// <item>Rectangle, RECT: a rectangle (see <see cref="Screen.FromRectangle"/>).</item>
		/// </list>
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <exception cref="ArgumentException">Bad object type (not one from the above list).</exception>
		/// <remarks>
		/// If something fails (except if invalid index), gets primary screen.
		/// As screen index is used index in the array returned by <see cref="Screen.AllScreens"/> + 1. It is not the screen index that you can see in Control Panel.
		/// </remarks>
		public static Screen FromObject(object screen)
		{
			//CONSIDER: don't use this. Now it is used to get 'screen' parameters of object type.
			//	Such parameters are unsafe and unclear to read the code.
			//		Eg instead of Mouse.Move(x, y, false, 1) better to write Mouse.Move(x, y, false, Screen_.FromIndex(1)).
			//		Although can write Mouse.Move(x, y, screen: 1), but most people will not use it.
			//	But in some cases better object.
			//		Eg then can use Screen_.FromMouse etc for TaskDialog.Options.DefaultScreen. If it was a Screen object, even if retrieved from index, it can become invalid after changing display settings.

			Screen R = null;
			switch(screen) {
			case null: break;
			case Screen s: R = s; break;
			case int index: return FromIndex(index);
			case Wnd wnd: return FromWindow(wnd);
			case Control c: R = Screen.FromControl(c); break;
			case Point p: return Screen.FromPoint(p);
			case Rectangle rr: return Screen.FromRectangle(rr);
			case RECT r: return Screen.FromRectangle(r);
			default: throw new ArgumentException("Bad object type.");
			}
			if(R != null && !R.Bounds.IsEmpty) return R;
			return Screen.PrimaryScreen;
		}

		/// <summary>
		/// Gets primary screen width.
		/// </summary>
		public static int Width { get => Api.GetSystemMetrics(Api.SM_CXSCREEN); }
		/// <summary>
		/// Gets primary screen height.
		/// </summary>
		public static int Height { get => Api.GetSystemMetrics(Api.SM_CYSCREEN); }
		//public static int Width { get => Screen.PrimaryScreen.Bounds.Width; } //faster (gets cached value), but very slow first time, eg 15 ms

		/// <summary>
		/// Gets primary screen rectangle.
		/// </summary>
		public static RECT Rect => new RECT(0, 0, Width, Height, false);

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
			return Api.MonitorFromPoint(p, 0) != Zero; //0 - MONITOR_DEFAULTTONULL
		}

		/// <summary>
		/// Returns true if rectangle r intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(RECT r)
		{
			return Api.MonitorFromRect(ref r, 0) != Zero;
		}

		/// <summary>
		/// Returns true if rectangle of window w intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(Wnd w)
		{
			return Api.MonitorFromWindow(w, 0) != Zero;
		}
	}
}
