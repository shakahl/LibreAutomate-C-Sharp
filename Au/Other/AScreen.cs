//#define CACHE_ALLSCREENS

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
using System.Windows.Forms;
//using System.Linq;

using Au;
using Au.Types;

namespace Au
{
	/// <summary>
	/// Gets screens from index/window/point/etc and their properties such as bounds.
	/// </summary>
	/// <remarks>
	/// A computer can have one or more screens (aka display devices, monitors) attached that extend desktop space. One of them is the <i>primary</i> screen; its top-left coordinate is 0 0.
	/// To show or find a window or some object in a particular screen, need to identify the screen somehow. At Windows API level each screen has a unique int identifier, known as screen handle or HMONITOR. But it is a random variable value and therefore cannot be specified directly in script. In script instead is used screen index or some object on that screen (window, point, rectangle).
	/// 
	/// There are two types (both are struct):
	/// - an <b>AScreen.ScreenHandle</b> variable contains a screen handle, aka HMONITOR. Its functions are used to quickly get properties of that screen, eg bounds.
	/// - an <b>AScreen</b> variable contains a value that is used to get screen handle. It can be screen index, window, point, etc, and even a callback function.
	/// 
	/// <b>AScreen</b> is used mostly in two ways:
	/// - the static functions are used to get a screen handle as <see cref="ScreenHandle"/> using screen index, window, point, etc. Then the <b>ScreenHandle</b> gets screen properties. Example: <c>RECT r = AScreen.Of(AWnd.Active).Bounds; //get rectangle of screen that contains the active window</c>
	/// - if a function parameter or some property etc is of <b>AScreen</b> type, callers can specify screen index, window, point, etc. Index can be passed directly; for other types use code like <c>new AScreen(window)</c>. The <b>AScreen</b> variable holds the specified value. When the function needs screen properties, it calls <see cref="GetScreenHandle"/> to get <see cref="ScreenHandle"/> corresponding that value at that time.
	/// 
	/// Why need two types? Why <b>AScreen</b> doesn't just get and store screen handle immediately when the variable is created. Because a handle cannot be reliably used for a long time. Reasons:
	/// - screen handles may change when changing the configuration of multiple screens.
	/// - the window etc that identifies the screen later may move to another screen.
	/// </remarks>
	public struct AScreen
	{
		readonly object _o;

		/// <summary>
		/// Gets the value as object.
		/// </summary>
		public object Value => _o;

		/// <summary>
		/// true if this variable does not have a value.
		/// </summary>
		public bool IsNull => _o == null;

		/// <summary>
		/// Creates variable that holds screen index. Later will be called <see cref="Index(int)"/>.
		/// Alternatively use implicit conversion from int, for example <c>1</c> instead of <c>new AScreen(1)</c>.
		/// </summary>
		/// <param name="screenIndex">0 primary screen, 1 first nonprimary, and so on. Does not match those in Windows Settings.</param>
		public AScreen(int screenIndex) => _o = screenIndex;

		/// <summary>
		/// Creates variable that holds screen index. Later will be called <see cref="Index(int)"/>.
		/// </summary>
		/// <param name="screenIndex">0 primary screen, 1 first nonprimary, and so on. Does not match those in Windows Settings.</param>
		public static implicit operator AScreen(int screenIndex) => new AScreen(screenIndex);

		/// <summary>
		/// Creates variable that holds <see cref="POINT"/>. Later will be called <see cref="Of(POINT, SODefault)"/>.
		/// </summary>
		public AScreen(POINT p) => _o = p;

		/// <summary>
		/// Creates variable that holds <see cref="RECT"/>. Later will be called <see cref="Of(RECT, SODefault)"/>.
		/// </summary>
		public AScreen(RECT r) => _o = r;

		/// <summary>
		/// Creates variable that holds window handle. Later will be called <see cref="Of(AWnd, SODefault)"/>.
		/// </summary>
		public AScreen(AWnd w) => _o = w;

		/// <summary>
		/// Creates variable that holds <see cref="Control"/>. Later will be called <see cref="Of(Control, SODefault)"/>.
		/// </summary>
		public AScreen(Control w) => _o = w;

		/// <summary>
		/// Creates variable that holds a WPF window/popup/control. Later will be called <see cref="Of(System.Windows.DependencyObject, SODefault)"/>.
		/// </summary>
		public AScreen(System.Windows.DependencyObject w) => _o = new object[] { w };

		/// <summary>
		/// Creates variable that calls your function to get screen when need.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// new AScreen(() => AScreen.Of(AMouse.XY));
		/// new AScreen(() => AScreen.Of(AWnd.Active));
		/// ]]></code>
		/// </example>
		public AScreen(Func<ScreenHandle> f) => _o = f;

		/// <summary>
		/// Creates variable that holds <see cref="ScreenHandle"/>.
		/// </summary>
		public AScreen(ScreenHandle screen) => _o = screen;

		//no
		///// <summary>
		///// Creates variable that holds <see cref="ScreenHandle"/>.
		///// </summary>
		//public static implicit operator AScreen(ScreenHandle screen) => new AScreen(screen);

		/// <summary>
		/// Gets the primary screen.
		/// </summary>
		public static ScreenHandle Primary => new ScreenHandle(MonitorFromWindow(default, SODefault.Primary)); //fast

		/// <summary>
		/// Gets <b>AScreen</b> variable that later will get the screen from the mouse cursor position at that time.
		/// </summary>
		public static AScreen OfMouse { get; } = new AScreen(() => Of(AMouse.XY));

		/// <summary>
		/// Gets <b>AScreen</b> variable that later will get the screen of the active window at that time.
		/// </summary>
		public static AScreen OfActiveWindow { get; } = new AScreen(() => Of(AWnd.Active));

#if CACHE_ALLSCREENS
		//This version uses a cached array that is updated like in Screen class code, on SystemEvents.DisplaySettingsChanging (wm_displaychanged).
		//Index functions are faster, but less reliable when changing display settings, because the array is updated with a delay.
		//In other version index functions are fast enough. Faster than EnumWindows which is used much more often.

		/// <summary>
		/// Gets screen at the specified index of the <see cref="AllScreens"/> array.
		/// </summary>
		/// <param name="index">0-based screen index. Index 0 is the primary screen. If index too big, gets the primary screen.</param>
		public static ScreenHandle Index(int index)
		{
			if(index < 0) throw new ArgumentOutOfRangeException();
			if(index > 0) {
				var a = AllScreens;
				if(index < a.Length) return a[index];
				//AWarning.Write("Invalid screen index.");
			}
			return Primary;
		}

		/// <summary>
		/// Gets all screens.
		/// </summary>
		/// <remarks>
		/// To get screens is used API <msdn>EnumDisplayMonitors</msdn>. Its results are cached in a static array. The array is auto-updated after changing display settings, on <see cref="SystemEvents.DisplaySettingsChanging"/> event. This property returns that array. Don't modify it.
		/// The order of array elements is different than in Windows Settings (Control Panel). The primary screen is always at index 0.
		/// </remarks>
		public static ScreenHandle[] AllScreens => s_screens ??= _GetScreens();
		static ScreenHandle[] s_screens;
		static bool s_haveEvent;

		static ScreenHandle[] _GetScreens()
		{
			if(!s_haveEvent) {
				s_haveEvent = true;
				//ThreadPool.QueueUserWorkItem(_ => SystemEvents.DisplaySettingsChanging += (_, _) => s_screens = null); //slower if used first time. Task.Run much slower.
				new Thread(() => SystemEvents.DisplaySettingsChanging += (_, _) => s_screens = null).Start();
				//async because very slow first time, eg 30 ms if cold CPU. Unless SystemEvents was already used by some code.
			}
			var a = new List<ScreenHandle> { Primary };
			EnumDisplayMonitors(default, default, (hmon, _1, _2, _3) => { //not very fast
				if(hmon != a[0].Handle) a.Add(new ScreenHandle(hmon));
				return true;
			}, default);
			return a.ToArray();
		}
#else
		/// <summary>
		/// Gets all screens.
		/// </summary>
		/// <remarks>
		/// The order of array elements may be different than in Windows Settings (Control Panel). The primary screen is always at index 0.
		/// The array is not cached. Each time calls API <msdn>EnumDisplayMonitors</msdn>.
		/// </remarks>
		public static ScreenHandle[] AllScreens => _AllScreens().ToArray();

		static List<ScreenHandle> _AllScreens()
		{
			t_a ??= new List<ScreenHandle>();
			t_a.Clear();
			t_a.Add(Primary); //2 mcs with cold CPU
			EnumDisplayMonitors(default, default, (hmon, _1, _2, param) => { //50-100 mcs with cold CPU
				if(hmon != t_a[0].Handle) t_a.Add(new ScreenHandle(hmon));
				return true;
			}, default);
			return t_a;
		}
		[ThreadStatic] static List<ScreenHandle> t_a;

		/// <summary>
		/// Gets screen at the specified index of the <see cref="AllScreens"/> array.
		/// </summary>
		/// <param name="index">0-based screen index. Index 0 is the primary screen. If index too big, gets the primary screen.</param>
		public static ScreenHandle Index(int index)
		{
			if(index < 0) throw new ArgumentOutOfRangeException();
			if(index > 0) {
				var a = _AllScreens();
				if(index < a.Count) return a[index];
				//AWarning.Write("Invalid screen index.");
			}
			return Primary;
		}
#endif

		/// <summary>
		/// Gets screen containing the biggest part of the specified window or nearest to it.
		/// </summary>
		/// <param name="w">Window or control. Can be default(AWnd) or invalid.</param>
		/// <param name="defaultScreen"></param>
		public static ScreenHandle Of(AWnd w, SODefault defaultScreen = SODefault.Nearest)
			=> new ScreenHandle(MonitorFromWindow(w, defaultScreen));

		/// <summary>
		/// Gets screen containing the biggest part of the specified control or form or nearest to it.
		/// </summary>
		/// <param name="c">Control or form. Cannot be null. The handle can be created or not.</param>
		/// <param name="defaultScreen"></param>
		public static ScreenHandle Of(Control c, SODefault defaultScreen = SODefault.Nearest)
			=> Of(c.Hwnd(), defaultScreen);

		/// <summary>
		/// Gets screen containing the biggest part of the specified WPF window/popup/control or nearest to it.
		/// </summary>
		/// <param name="w">WPF window, popup or a child object.</param>
		/// <param name="defaultScreen"></param>
		public static ScreenHandle Of(System.Windows.DependencyObject w, SODefault defaultScreen = SODefault.Nearest)
			=> Of(w.Hwnd(), defaultScreen);

		/// <summary>
		/// Gets screen containing the specified coordinate or nearest to it.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="defaultScreen"></param>
		public static ScreenHandle Of(POINT p, SODefault defaultScreen = SODefault.Nearest)
			=> new ScreenHandle(MonitorFromPoint(p, defaultScreen));

		/// <summary>
		/// Gets screen containing the biggest part of the specified rectangle or nearest to it.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="defaultScreen"></param>
		public static ScreenHandle Of(RECT r, SODefault defaultScreen = SODefault.Nearest)
			=> new ScreenHandle(MonitorFromRect(r, defaultScreen));

		/// <summary>
		/// Returns true if point p is in some screen.
		/// </summary>
		public static bool IsInAnyScreen(POINT p) => MonitorFromPoint(p, SODefault.Zero) != default;

		/// <summary>
		/// Returns true if rectangle r intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(RECT r) => MonitorFromRect(r, SODefault.Zero) != default;

		/// <summary>
		/// Returns true if rectangle of window w intersects with some screen.
		/// </summary>
		public static bool IsInAnyScreen(AWnd w) => MonitorFromWindow(w, SODefault.Zero) != default;

		//no
		///// <summary>Converts to <see cref="ScreenHandle"/> (calls <see cref="GetScreenHandle"/>).</summary>
		//public static implicit operator ScreenHandle(AScreen screen) => screen.GetScreenHandle();

		/// <summary>
		/// Gets <see cref="ScreenHandle"/> from the value stored in this variable (screen index, window, etc).
		/// If the value is null, gets the primary screen.
		/// </summary>
		public ScreenHandle GetScreenHandle() => _o switch
		{
			null => Primary,
			int k => Index(k),
			POINT k => Of(k),
			RECT k => Of(k),
			ScreenHandle k => k,
			Func<ScreenHandle> k => k(),
			_ => Of(AWnd.Internal_.FromObject(_o)),
		};

		/// <summary>
		/// Represents a screen device. Gets its rectangle etc.
		/// </summary>
		public unsafe struct ScreenHandle : IEquatable<ScreenHandle>
		{
			//This type is similar to the .NET class Screen. We don't use it mostly because some functions are too slow.

			readonly IntPtr _h;

			/// <param name="handle">A screen handle (HMONITOR).</param>
			public ScreenHandle(IntPtr handle) => _h = handle;

			/// <summary>
			/// The screen handle (HMONITOR).
			/// </summary>
			public IntPtr Handle => _h;

			/// <summary>
			/// True if the screen handle is 0.
			/// </summary>
			public bool Is0 => _h == default;

			///
			public static implicit operator IntPtr(ScreenHandle screen) => screen._h;

#if CACHE_ALLSCREENS
			/// <summary>
			/// Gets index of this screen in the <see cref="AllScreens"/> array.
			/// </summary>
			/// <remarks>
			/// Returns -1 if this screen is not in the array. It can happen after changing display settings, when this variable is older or newer than the array. Also if the screen handle is 0.
			/// </remarks>
			public int Index {
				get {
					var a = AllScreens;
					for(int i = 0; i < a.Length; i++) if(a[i]._h == _h) return i;
					return -1;
				}
			}
#else
			/// <summary>
			/// Gets index of this screen in the <see cref="AllScreens"/> array.
			/// </summary>
			/// <remarks>
			/// Returns -1 if this screen is not in the array. It can happen after changing display settings, but is rare. Also if the screen handle is 0.
			/// </remarks>
			public int Index {
				get {
					var a = _AllScreens();
					for(int i = 0; i < a.Count; i++) if(a[i]._h == _h) return i;
					return -1;
				}
			}
#endif

			/// <summary>
			/// Gets screen rectangle and other info.
			/// </summary>
			/// <returns>
			/// Tuple containing:
			/// - bounds - rectangle of the screen.
			/// - workArea - rectangle of the work area of the screen.
			/// - isPrimary - true if the info is of the primary screen.
			/// - isAlive - true if the screen handle is valid. If invalid (eg 0), the function gets info of the primary screen.
			/// </returns>
			public (RECT bounds, RECT workArea, bool isPrimary, bool isAlive) GetInfo()
			{
				var m = new MONITORINFO { cbSize = sizeof(MONITORINFO) };
				for(int i = _h != default ? 0 : 1; i < 10; i++) {
					if(GetMonitorInfo(i == 0 ? _h : Primary._h, ref m)) //fast
						return (m.rcMonitor, m.rcWork, 0 != (m.dwFlags & 1), i == 0);
				}
				return default;
			}

			/// <summary>
			/// Calls <see cref="GetInfo"/> and returns rectangle of the screen or its work area.
			/// </summary>
			/// <param name="workArea">Get work area rectangle.</param>
			public RECT GetRect(bool workArea = false)
			{
				var v = GetInfo();
				return workArea ? v.workArea : v.bounds;
			}

			/// <summary>
			/// Calls <see cref="GetInfo"/> and returns rectangle of the screen.
			/// </summary>
			public RECT Bounds => GetRect();

			/// <summary>
			/// Calls <see cref="GetInfo"/> and returns rectangle of the screen work area.
			/// </summary>
			public RECT WorkArea => GetRect(true);

			/// <summary>
			/// Gets DPI of this screen. See <see cref="ADpi.OfScreen"/>.
			/// </summary>
			public int Dpi => ADpi.OfScreen(_h);

			/// <summary>
			/// True if the screen handle is valid.
			/// </summary>
			public bool IsAlive {
				get {
					if(_h == default) return false;
					var m = new MONITORINFO { cbSize = sizeof(MONITORINFO) };
					return GetMonitorInfo(_h, ref m);
				}
			}

			///
			public override string ToString() => _h.ToString() + " " + Bounds.ToString();

			///
			public override int GetHashCode() => (int)_h;

			///
			public bool Equals(ScreenHandle other) => other._h == _h;

			///
			public static bool operator ==(ScreenHandle a, ScreenHandle b) => a._h == b._h;
			///
			public static bool operator !=(ScreenHandle a, ScreenHandle b) => a._h != b._h;

			//rejected. GetHashCode gets hmonitor but is undocumented. Rarely used.
			///// <summary>Converts from <see cref="Screen"/>.</summary>
			//public static implicit operator ScreenHandle(Screen screen) => new ScreenHandle((IntPtr)screen.GetHashCode());

			///// <summary>Converts to <see cref="Screen"/>. Returns null if fails.</summary>
			//public static implicit operator Screen(ScreenHandle screen) => Screen.AllScreens.FirstOrDefault(o => o.GetHashCode() == (int)screen._h);
		}

		[DllImport("user32.dll")]
		static extern IntPtr MonitorFromPoint(POINT pt, SODefault dwFlags);

		[DllImport("user32.dll")]
		static extern IntPtr MonitorFromRect(in RECT lprc, SODefault dwFlags);

		[DllImport("user32.dll")]
		static extern IntPtr MonitorFromWindow(AWnd hwnd, SODefault dwFlags);

		struct MONITORINFO
		{
			public int cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
		}

		[DllImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
		static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		delegate bool MONITORENUMPROC(IntPtr hmon, IntPtr hdc, IntPtr r, LPARAM dwData);

		[DllImport("user32.dll")]
		static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MONITORENUMPROC lpfnEnum, LPARAM dwData);
	}
}

namespace Au.Types
{
	/// <summary>
	/// Used with <see cref="AScreen.Of"/> to specify what screen to use if the function fails, for example if the window/point/etc is not in a screen or if the window handle is invalid.
	/// </summary>
	public enum SODefault
	{
		/// <summary>0 (<see cref="AScreen.ScreenHandle.Is0"/> will return true).</summary>
		Zero, //MONITOR_DEFAULTTONULL

		/// <summary>The primary screen.</summary>
		Primary, //MONITOR_DEFAULTTOPRIMARY

		/// <summary>The nearest screen. If window handle is invalid - the primary screen.</summary>
		Nearest, //MONITOR_DEFAULTTONEAREST
	}
}
