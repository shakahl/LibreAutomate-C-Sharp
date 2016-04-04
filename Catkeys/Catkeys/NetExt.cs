//Small extension classes for .NET classes. Except those that have own files, eg String_.
//Naming:
//	Class name: related .NET class name with _ suffix.
//	Extension method name: related .NET method name with _ suffix. Or new name with _ suffix.
//	Static method name: any name without _ suffix.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
#if dont_use
	////[DebuggerStepThrough]
	//public static class Int32_
	//{
	//	//This does not work because int is a value type and therefore the x parameter is a copy. Better don't need this func.
	//	//public static void LimitMinMax_(this int x, int min, int max)
	//	//{
	//	//	if(x>max) x=max;
	//	//	if(x<min) x=min;
	//	//}

	//	////Allows code like this: foreach(int u in 5.Times()) Out(u);
	//	////However the code is longer than for(int u=0; u<5; u++) Out(u);
	//	//public static IEnumerable<int> Times(this int nTimes)
	//	//{
	//	//	for(int i = 0; i<nTimes; i++) yield return i;
	//	//}
	//}

	/// <summary>
	/// Enum extension methods.
	/// </summary>
	[DebuggerStepThrough]
	public static class Enum_
	{
		/// <summary>
		/// Returns true if this has one or more flags specified in flagOrFlags.
		/// It is different from HasFlag, which returns true if this has ALL specified flags.
		/// Speed: 0.1 mcs. It is several times slower than HasFlag, and 100 times slower than operators & !=0.
		/// </summary>
		public static bool HasAny(this Enum t, Enum flagOrFlags)
		{
			return (Convert.ToUInt64(t) & Convert.ToUInt64(flagOrFlags)) !=0;
			//info: cannot apply operator & to Enum, or cast to uint, or use unsafe pointer.
		}
		////same speed:
		//public static bool Has<T>(this T t, T flagOrFlags) where T: struct
		//{
		//	//return ((uint)t & (uint)flagOrFlags) !=0; //error
		//	return (Convert.ToUInt64(t) & Convert.ToUInt64(flagOrFlags)) !=0;
		//}

		public static void SetFlag(ref Enum t, Enum flag, bool set)
		{
			ulong _t = Convert.ToUInt64(t), _f = Convert.ToUInt64(flag);
			if(set) _t|=_f; else _t&=~_f;
			t=(Enum)(object)_t; //can do it better?
			//info: Cannot make this a true extension method.
			//	If we use 'this Enum t', t is a copy of the actual variable, because Enum is a value type.
			//	That is why we need ref.
			//Speed not tested.
		}
	}
#endif

	public static class Screen_
	{
		public const int Primary = 0;
		public const int OfMouse = -1;
		public const int OfActiveWindow = -2;

		/// <summary>
		/// Gets Screen object from 1-based screen index.
		/// index also can be one of constants defined in this class: Primary (0), OfMouse, OfActiveWindow.
		/// If index is invalid, gets the primary screen.
		/// </summary>
		/// <remarks>
		/// As screen index is used index in the array returned by Screen.AllScreens + 1. It is not the screen index that you can see in Control Panel.
		/// </remarks>
		public static Screen FromIndex(int index)
		{
			if(index>0) {
				var a = Screen.AllScreens;
				if(--index<a.Length) return a[index];
				//SHOULDDO: ignore invisible pseudo-monitors associated with mirroring drivers.
				//	iScreen.AllScreens and EnumDisplayMonitors should include them,
				//	but in my recent tests with NetMeeting (noticed this long ago on an old OS version) and UltraVnc (wiki etc say) they didn't.
				//	Therefore I cannot test and add filtering. No problems if they are the last in the list. Never mind.
				//	Wiki about mirror drivers: https://en.wikipedia.org/wiki/Mirror_driver
			} else if(index==OfMouse) return Screen.FromPoint(Mouse.XY);
			else if(index==OfActiveWindow) return FromWindow(Wnd.ActiveWindow);

			return Screen.PrimaryScreen;

			//speed compared with the API monitor functions:
			//	First time several times slower, but then many times faster. It means that .NET uses caching.
			//	Tested: correctly updates after changing multi-monitor config.
		}

		/// <summary>
		/// Gets Screen object that contains (or is nearest to) the specified window.
		/// If w is 0 or closed/invalid, gets primary screen (Screen.FromHandle would return an invalid object if the window handle is invalid).
		/// </summary>
		public static Screen FromWindow(Wnd w)
		{
			if(w.Is0) return Screen.PrimaryScreen;
			Screen r = Screen.FromHandle((IntPtr)w);
			if(r.Bounds.IsEmpty) return Screen.PrimaryScreen;
			return r;
		}

		/// <summary>
		/// Gets Screen object that contains (or is nearest to) the specified object, which can be (depending on variable type):
		///		int: 1-based screen index, or Screen_.Primary (0), Screen_.OfMouse, Screen_.OfActiveWindow.
		///		Wnd: a window. If invalid, gets primary screen.
		///		POINT, Point: a point.
		///		RECT, Rectangle: a rectangular area.
		///		Screen: a Screen object. If invalid, gets primary screen.
		///		null: if w used, gets its screen, else gets primary screen.
		///		Other type: throws exception.
		/// </summary>
		/// <remarks>
		/// If something fails, gets primary screen.
		/// As screen index is used index in the array returned by Screen.AllScreens + 1. It is not the screen index that you can see in Control Panel.
		/// </remarks>
		public static Screen FromObject(object screen, Wnd w=default(Wnd))
		{
			if(screen==null) return FromWindow(w); //screen of w, or primary if w is Wnd0 or invalid
			if(screen is int) return FromIndex((int)screen);
			if(screen is Wnd) return FromWindow((Wnd)screen);
			if(screen is POINT) return Screen.FromPoint((POINT)screen);
			if(screen is RECT) return Screen.FromRectangle((RECT)screen);
			if(screen is System.Drawing.Point) return Screen.FromPoint((System.Drawing.Point)screen);
			if(screen is System.Drawing.Rectangle) return Screen.FromRectangle((System.Drawing.Rectangle)screen);
			if(screen is Screen) return ((Screen)screen).Bounds.IsEmpty ? Screen.PrimaryScreen : (Screen)screen;

			throw new ArgumentException("screen object type mismatch");
		}
	}
}
