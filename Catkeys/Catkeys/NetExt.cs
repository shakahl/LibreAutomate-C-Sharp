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
using Microsoft.Win32;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
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
		/// Speed: 0.1 mcs. It is several times slower than HasFlag, and 100 times slower than operators.
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
			if(index > 0) {
				var a = Screen.AllScreens;
				if(--index < a.Length) return a[index];
				//SHOULDDO: ignore invisible pseudo-monitors associated with mirroring drivers.
				//	iScreen.AllScreens and EnumDisplayMonitors should include them,
				//	but in my recent tests with NetMeeting (noticed this long ago on an old OS version) and UltraVnc (wiki etc say) they didn't.
				//	Therefore I cannot test and add filtering. No problems if they are the last in the list. Never mind.
				//	Wiki about mirror drivers: https://en.wikipedia.org/wiki/Mirror_driver
			} else if(index == OfMouse) return Screen.FromPoint(Mouse.XY);
			else if(index == OfActiveWindow) return FromWindow(Wnd.ActiveWindow);

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
		public static Screen FromObject(object screen, Wnd w = default(Wnd))
		{
			if(screen == null) return FromWindow(w); //screen of w, or primary if w is Wnd0 or invalid
			if(screen is int) return FromIndex((int)screen);
			if(screen is Wnd) return FromWindow((Wnd)screen);
			if(screen is POINT) return Screen.FromPoint((POINT)screen);
			if(screen is RECT) return Screen.FromRectangle((RECT)screen);
			if(screen is System.Drawing.Point) return Screen.FromPoint((System.Drawing.Point)screen);
			if(screen is System.Drawing.Rectangle) return Screen.FromRectangle((System.Drawing.Rectangle)screen);
			if(screen is Screen) return ((Screen)screen).Bounds.IsEmpty ? Screen.PrimaryScreen : (Screen)screen;

			throw new ArgumentException("screen object type mismatch");
		}

		/// <summary>
		/// Gets primary screen width.
		/// </summary>
		public static int Width { get { return Api.GetSystemMetrics(Api.SM_CXSCREEN); } }
		/// <summary>
		/// Gets primary screen height.
		/// </summary>
		public static int Height { get { return Api.GetSystemMetrics(Api.SM_CYSCREEN); } }
		//public static int Width { get { return Screen.PrimaryScreen.Bounds.Width; } } //faster (gets cached value), but very slow first time, eg 15 ms

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
	}

	/// <summary>
	/// Registry functions.
	/// Unlike Microsoft.Win32.Registry, does not throw exception when fails. Instead uses ThreadError.
	/// Also has methods not supported by Microsoft.Win32.Registry, for example set/get struct variables easily.
	/// </summary>
	public static class Registry_
	{
		public const string CatkeysKey = @"Software\Catkeys\User";

		/// <summary>
		/// Parses registry key string and returns hive as RegistryKey.
		/// If key starts with "HKEY_", removes hive name from it and returns that hive. For example, if key is @"HKEY_LOCAL_MACHINE\Software\Test", sets key=@"Software\Test" and returns Registry.LocalMachine.
		/// Else if key is null or @"\", sets key=Registry_.CatkeysKey (@"Software\Catkeys\User") and returns Registry.CurrentUser.
		/// Else if key starts with @"\", prepends Registry_.CatkeysKey (@"Software\Catkeys\User") and returns Registry.CurrentUser.
		/// Else just returns Registry.CurrentUser.
		/// Valid hive names: "HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE", "HKEY_CLASSES_ROOT", "HKEY_USERS", "HKEY_PERFORMANCE_DATA" or "HKEY_CURRENT_CONFIG".
		/// </summary>
		/// <param name="key">Registry key. Can start with a hive name.</param>
		/// <exception cref="ArgumentException">When key starts with "HKEY_" but it is an invalid hive name.</exception>
		public static RegistryKey ParseKeyString(ref string key)
		{
			if(key == null) key = @"\";
			if(key.StartsWith_(@"\")) {
				key = (key.Length == 1) ? CatkeysKey : CatkeysKey + key;
				return Registry.CurrentUser;
			}
			if(!key.StartsWith_("HKEY_")) return Registry.CurrentUser;

			RegistryKey R = null;
			int i = key.IndexOf('\\');
			string s = i < 0 ? key : key.Remove(i);
			switch(s) {
			case "HKEY_CURRENT_USER": R = Registry.CurrentUser; break;
			case "HKEY_LOCAL_MACHINE": R = Registry.LocalMachine; break;
			case "HKEY_CLASSES_ROOT": R = Registry.ClassesRoot; break;
			case "HKEY_USERS": R = Registry.Users; break;
			case "HKEY_CURRENT_CONFIG": R = Registry.CurrentConfig; break;
			case "HKEY_PERFORMANCE_DATA": R = Registry.PerformanceData; break;
			//case "HKEY_DYN_DATA": R = Registry.DynData; break; //9x
			default: throw new ArgumentException("Invalid \"HKEY_x\".");
			}
			key = i < 0 ? "" : key.Substring(i + 1);
			return R;
		}

		static RegistryKey _Open(string key, bool create)
		{
			RegistryKey hive = ParseKeyString(ref key);
			RegistryKey k = create ? hive.CreateSubKey(key) : hive.OpenSubKey(key);
			if(k == null) {
				string sc = create ? " or create" : "";
				throw new CatkeysException($"Failed to open{sc} registry key \"{hive.Name}\\{key}\".");
			}
			return k;
		}

		/// <summary>
		/// Sets value of REG_DWORD type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool SetInt(int data, string valueName, string key = null)
		{
			try {
				using(var k = _Open(key, true)) {
					k.SetValue(valueName, data, RegistryValueKind.DWord);
					return true;
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Gets value of REG_DWORD type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool GetInt(out int data, string valueName, string key = null)
		{
			data = 0;
			try {
				using(var k = _Open(key, false)) {
					object t = k.GetValue(valueName);
					if(t != null) { data = (int)t; return true; }
					ThreadError.Clear();
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Sets value of REG_QWORD type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool SetLong(long data, string valueName, string key = null)
		{
			try {
				using(var k = _Open(key, true)) {
					k.SetValue(valueName, data, RegistryValueKind.QWord);
					return true;
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Gets value of REG_QWORD type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool GetLong(out long data, string valueName, string key = null)
		{
			data = 0;
			try {
				using(var k = _Open(key, false)) {
					object t = k.GetValue(valueName);
					if(t != null) { data = (long)t; return true; }
					ThreadError.Clear();
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Sets string value of REG_SZ or REG_EXPAND_SZ type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool SetString(string data, string valueName, string key = null, bool REG_EXPAND_SZ = false)
		{
			try {
				using(var k = _Open(key, true)) {
					k.SetValue(valueName, data, REG_EXPAND_SZ ? RegistryValueKind.ExpandString : RegistryValueKind.String);
					return true;
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;

			//info: cannot set multistring with this.
		}

		/// <summary>
		/// Gets string value of REG_SZ or REG_EXPAND_SZ type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool GetString(out string data, string valueName, string key = null)
		{
			data = null;
			try {
				using(var k = _Open(key, false)) {
					object t = k.GetValue(valueName);
					if(t != null) { data = (string)t; return true; }
					ThreadError.Clear();
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Sets string value of REG_MULTI_SZ type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool SetStringArray(string[] data, string valueName, string key = null)
		{
			try {
				using(var k = _Open(key, true)) {
					k.SetValue(valueName, data, RegistryValueKind.MultiString);
					return true;
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;

			//info: cannot set multistring with this.
		}

		/// <summary>
		/// Gets string value of REG_MULTI_SZ type.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// If valueName does not exist, clears thread error and returns false.
		/// </summary>
		/// <param name="data">Receives data.</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool GetStringArray(out string[] data, string valueName, string key = null)
		{
			data = null;
			try {
				using(var k = _Open(key, false)) {
					object t = k.GetValue(valueName);
					if(t != null) { data = (string[])t; return true; }
					ThreadError.Clear();
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Sets binary value of REG_BINARY type.
		/// Creates key and value if don't exist.
		/// Supports ThreadError.
		/// If fails, sets thread error and returns false.
		/// </summary>
		/// <param name="data">Data. For example a struct variable (unsafe address).</param>
		/// <param name="size">Data size. For example, Marshal.SizeOf(variable) or Marshal.SizeOf(typeof(DATA)).</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe bool SetBinary(void* data, int size, string valueName, string key = null)
		{
			try {
				using(var k = _Open(key, true)) {
					return SetBinary(data, size, valueName, k);
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return false;
		}

		/// <summary>
		/// Use this overload when you have an open registry key.
		/// Everything is the same as with other overload.
		/// </summary>
		public static unsafe bool SetBinary(void* data, int size, string valueName, RegistryKey key)
		{
			IntPtr h = key.Handle.DangerousGetHandle();
			int e = Api.RegSetValueEx(h, valueName, 0, RegistryValueKind.Binary, data, size);
			return e == 0 || ThreadError.Set(e);
		}

		/// <summary>
		/// Gets binary data.
		/// Returns registry data size that the function copied into the 'data' memory. It can be equal or less than the 'size' argument. If registry data is bigger than 'size', gets only 'size' part of it.
		/// If valueName does not exist, returns 0.
		/// If fails, returns -1.
		/// Supports ThreadError.
		/// Registry data can be of any type.
		/// </summary>
		/// <param name="data">Receives data. For example a struct variable (unsafe address).</param>
		/// <param name="size">Max data size to get. For example, Marshal.SizeOf(variable) or Marshal.SizeOf(typeof(DATA)).</param>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="ParseKeyString"/></param>
		public static unsafe int GetBinary(void* data, int size, string valueName, string key = null)
		{
			try {
				using(var k = _Open(key, false)) {
					return GetBinary(data, size, valueName, k);
				}
			} catch(Exception e) { ThreadError.SetException(e); }
			return -1;
		}

		/// <summary>
		/// Use this overload when you have an open registry key.
		/// Everything is the same as with other overload.
		/// </summary>
		public static unsafe int GetBinary(void* data, int size, string valueName, RegistryKey key)
		{
			IntPtr h = key.Handle.DangerousGetHandle();
			RegistryValueKind kind; int z = size;
			int e = Api.RegQueryValueEx(h, valueName, Zero, out kind, data, ref z);
			if(e == 0) { ThreadError.Clear(); return z; }
			ThreadError.Set(e);
			if(e == Api.ERROR_FILE_NOT_FOUND) return 0;
			return -1;
		}
	}
}
