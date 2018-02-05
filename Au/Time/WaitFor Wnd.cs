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

using Au.Types;
using static Au.NoClass;

namespace Au
{
	//TODO: move all to Wnd.
	//	WaitFor.WindowExists
	//	Wnd.Wait
	//	
	//	WaitFor.WindowActive
	//	Wnd.WaitActive
	//	
	//	WaitFor.WindowActive(w, 
	//	w.WaitActive
	//	
	//	WaitFor.WindowCondition(w, 
	//	w.WaitCondition


	/// <summary>
	/// Contains functions to wait for a condition, such as 'window exists'.
	/// </summary>
	/// <remarks>
	/// All functions have a secondsTimeout parameter. It is the maximal time to wait, seconds. If it is 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <see cref="TimeoutException"/>. If &lt;0, after -secondsTimeout time just stops waiting and returns default value (false, default(Wnd), etc).
	/// 
	/// While waiting, messages and events are not processed. For example, if used in a Form/Control event handler, the form would stop responding. Then need to use another thread, for example async/await/Task, like in the example.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Wnd w = WaitFor.WindowExists(10, "* Notepad");
	/// Print(w);
	/// ]]></code>
	/// Using in a Form/Control event handler.
	/// <code><![CDATA[
	/// var f = new Form();
	/// f.Click += async (unu, sed) =>
	///   {
	/// 	  Print("waiting for Notepad...");
	/// 	  Wnd w = await Task.Run(() => WaitFor.WindowExists(-10, "* Notepad"));
	/// 	  if(w.Is0) Print("timeout"); else Print(w);
	///   };
	/// f.ShowDialog();
	/// ]]></code>
	/// </example>
	//[DebuggerStepThrough]
	public static partial class WaitFor
	{
		/// <summary>
		/// Waits until the specified window exists and (optionally) is visible. Or the opposite, if <b>not</b> is true.
		/// Returns window handle. If secondsTimeout is negative, after -secondsTimeout time returns default(Wnd) (else exception).
		/// All undocumented parameters etc are the same as <see cref="Wnd.Find"/>.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns default(Wnd).
		/// </param>
		/// <param name="name"></param>
		/// <param name="className"></param>
		/// <param name="programEtc"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="not">
		/// Do the opposite - wait until no such windows exist.
		/// The return value is opposite too. If succeeded - default(Wnd). If timeout - a matching window (if no exception).
		/// If you have a window's Wnd variable, use <see cref="WindowClosed"/> or <see cref="WindowVisible"/> instead.
		/// </param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Wnd.Find"/>.</exception>
		/// <remarks>
		/// By default ignores invisible windows. Use flag HiddenToo if need.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = WaitFor.WindowExists(10, "* Notepad");
		/// Print(w);
		/// ]]></code>
		/// Using in a Form/Control event handler.
		/// <code><![CDATA[
		/// var f = new Form();
		/// f.Click += async (unu, sed) =>
		///   {
		/// 	  Print("waiting for Notepad...");
		/// 	  Wnd w = await Task.Run(() => WaitFor.WindowExists(-10, "* Notepad"));
		/// 	  if(w.Is0) Print("timeout"); else Print(w);
		///   };
		/// f.ShowDialog();
		/// ]]></code>
		/// </example>
		public static Wnd WindowExists(double secondsTimeout = 0.0,
			string name = null, string className = null, WFOwner programEtc = null,
			WFFlags flags = 0, Func<Wnd, bool> also = null, bool not = false)
		{
			var f = new Wnd.Finder(name, className, programEtc, flags, also);
			return WindowExists(f, secondsTimeout, not);

			//note: this and other similar functions can create much garbage. Then GC runs quite frequently. It runs when exceeds 2 MB (GC.GetTotalMemory).
			//	In this case, the garbage would be the name, classname and programname strings. Currently they are cached and don't create unnecessary garbage.
			//	Tried to measure how long a busy thread is suspended when GC runs. It seems about 250 mcs. Not bad. But the PC is fast, has CPU with 4 logical CPU. Need to test on a 1-CPU PC.
		}

		/// <summary>
		/// The same as <see cref="WindowExists(double, string, string, WFOwner, WFFlags, Func{Wnd, bool}, bool)"/>, just arguments are passed differently.
		/// </summary>
		/// <param name="f">Window properties etc.</param>
		/// <param name="secondsTimeout"></param>
		/// <param name="not"></param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		public static Wnd WindowExists(Wnd.Finder f, double secondsTimeout = 0.0, bool not = false)
		{
			var to = new LibTimeout(secondsTimeout);
			if(not) {
				Wnd w = default;
				for(;;) {
					if(!w.IsAlive || !f.IsMatch(w)) { //if first time, or closed (!IsAlive), or changed properties (!IsMatch)
						if(!f.Find()) return default;
						w = f.Result;
					}
					if(!to.Sleep()) return f.Result;
				}
			} else {
				for(;;) {
					if(f.Find()) return f.Result;
					if(!to.Sleep()) return default;
				}
			}

			//CONSIDER: overload with Wnd.ChildFinder>.
		}

		//Not often used. It's easy with await Task.Run. Anyway, need to provide an example of similar size.
		//public static async Task<Wnd> WindowExistsAsync(double secondsTimeout, string name)
		//{
		//	return await Task.Run(() => WindowExists(secondsTimeout, name));
		//}

		/// <summary>
		/// Waits until the specified window exists and is the active window. Or the opposite, if <b>not</b> is true.
		/// Returns window handle. If secondsTimeout is negative, after -secondsTimeout time returns default(Wnd) (else exception).
		/// All undocumented parameters etc are the same as <see cref="Wnd.Find"/>.
		/// If you have a window's Wnd, use <see cref="WindowActive(Wnd, double, bool)"/> instead.
		/// </summary>
		/// <param name="secondsTimeout"></param>
		/// <param name="name"></param>
		/// <param name="className"></param>
		/// <param name="programEtc"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="not">
		/// Do the opposite - wait until the window is not the active window or does not exist.
		/// The return value is always default(Wnd).
		/// </param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Wnd.Find"/>.</exception>
		/// <remarks>
		/// Hidden and minimized windows also can be active. This function by default waits until the window is active and visible. To ignore visibility, use flag <see cref="WFFlags.HiddenToo"/>. To wait until the window also is not minimized, use the 'also' parameter, like in the example.
		///
		/// More examples: see <see cref="WindowExists(double, string, string, WFOwner, WFFlags, Func{Wnd, bool}, bool)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = WaitFor.WindowActive(60, "*- Notepad", also: t => !t.IsMinimized);
		/// Print(w);
		/// ]]></code>
		/// </example>
		public static Wnd WindowActive(double secondsTimeout = 0.0,
			string name = null, string className = null, WFOwner programEtc = null,
			WFFlags flags = 0, Func<Wnd, bool> also = null, bool not = false)
		{
			var f = new Wnd.Finder(name, className, programEtc, flags, also);
			return WindowActive(f, secondsTimeout, not);
		}

		//TODO: need Wnd.Finder[]. It would wait for one of windows and returns 1-based index or 0 (like ScreenImage.Find). The same for WindowExists.
		//	public static int WindowActive(double secondsTimeout, bool not, params Wnd.Finder[] f)
		/// <summary>
		/// The same as <see cref="WindowActive(double, string, string, WFOwner, WFFlags, Func{Wnd, bool}, bool)"/>, just arguments are passed differently.
		/// </summary>
		/// <param name="f">Window properties etc.</param>
		/// <param name="secondsTimeout"></param>
		/// <param name="not"></param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		public static Wnd WindowActive(Wnd.Finder f, double secondsTimeout = 0.0, bool not = false)
		{
			var to = new LibTimeout(secondsTimeout);
			for(;;) {
				Wnd w = Wnd.WndActive;
				if(not) {
					if(!f.IsMatch(w)) break;
					if(!to.Sleep()) break; //could return w, but it can be default(Wnd). Better always return default(Wnd).
				} else {
					if(f.IsMatch(w)) return w;
					if(!to.Sleep()) break;
				}
			}
			return default;
		}

		/// <summary>
		/// Waits until the window is the active window. Or the opposite, if <b>not</b> is true.
		/// Returns true. If secondsTimeout is negative, after -secondsTimeout time returns false (else exception).
		/// Uses <see cref="Wnd.IsActive"/>.
		/// </summary>
		/// <param name="w">A window or control.</param>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <param name="not">Do the opposite - wait until the window is inactive or closed (no exception if closed/default(Wnd)/invalid).</param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="WndException">w is default(Wnd)/invalid or the window was closed while waiting.</exception>
		public static bool WindowActive(Wnd w, double secondsTimeout = 0.0, bool not = false)
		{
			if(not) return WindowCondition(w, t => !t.IsActive, secondsTimeout, true);
			return WindowCondition(w, t => t.IsActive, secondsTimeout);
		}

		/// <summary>
		/// Waits until the window is visible. Or the opposite, if <b>not</b> is true.
		/// Returns true. If secondsTimeout is negative, after -secondsTimeout time returns false (else exception).
		/// Uses <see cref="Wnd.IsVisibleEx"/>.
		/// For 'exists and is visible' use <see cref="WindowExists(double, string, string, WFOwner, WFFlags, Func{Wnd, bool}, bool)"/>. With default flags it waits for visible window and ignores invisible windows.
		/// </summary>
		/// <param name="w">A window or control.</param>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <param name="not">Do the opposite - wait until the window is invisible or closed (no exception if closed/default(Wnd)/invalid).</param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="WndException">w is default(Wnd)/invalid or the window was closed while waiting.</exception>
		public static bool WindowVisible(Wnd w, double secondsTimeout = 0.0, bool not = false)
		{
			if(not) return WindowCondition(w, t => !t.IsVisibleEx, secondsTimeout, true);
			return WindowCondition(w, t => t.IsVisibleEx, secondsTimeout);
		}

		/// <summary>
		/// Waits until the window is enabled (not disabled). Or the opposite, if <b>not</b> is true.
		/// Returns true. If secondsTimeout is negative, after -secondsTimeout time returns false (else exception).
		/// Uses <see cref="Wnd.IsEnabled"/>.
		/// </summary>
		/// <param name="w">A window or control.</param>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <param name="not">Do the opposite - wait until the window is disabled or closed (no exception if closed/default(Wnd)/invalid).</param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="WndException">w is default(Wnd)/invalid or the window was closed while waiting.</exception>
		public static bool WindowEnabled(Wnd w, double secondsTimeout = 0.0, bool not = false)
		{
			if(not) return WindowCondition(w, t => !t.IsEnabled, secondsTimeout, true);
			return WindowCondition(w, t => t.IsEnabled, secondsTimeout);
		}

		/// <summary>
		/// Waits until the window handle is invalid.
		/// Returns true. If secondsTimeout is negative, after -secondsTimeout time returns false (else exception).
		/// Uses <see cref="Wnd.IsAlive"/>.
		/// </summary>
		/// <param name="w">A window or control. Can be default(Wnd)/invalid.</param>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		public static bool WindowClosed(Wnd w, double secondsTimeout = 0.0)
		{
			return WindowCondition(w, t => !t.IsAlive, secondsTimeout, true);
		}

		/// <summary>
		/// Waits until the specified control (child window) is found in the window. Or the opposite, if <b>not</b> is true.
		/// Returns the control. If secondsTimeout is negative, after -secondsTimeout time returns default(Wnd) (else exception).
		/// Uses <see cref="Wnd.IsEnabled"/>.
		/// </summary>
		/// <param name="w">Direct or indirect parent window.</param>
		/// <param name="control">Control properties.</param>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns default(Wnd).
		/// </param>
		/// <param name="not">
		/// Do the opposite - wait until the control not found, or until the window closed (no exception if closed/default(Wnd)/invalid).
		/// The return value is opposite too. If succeeded - default(Wnd). If timeout - a matching control (if no exception).
		/// If you have a control's Wnd variable, use <see cref="WindowClosed"/> or <see cref="WindowVisible"/> instead.
		/// </param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="WndException">w is default(Wnd)/invalid or the window was closed while waiting.</exception>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("Options");
		/// var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		/// Wnd c = WaitFor.WindowChildExists(w, f);
		/// Print(c);
		/// ]]></code>
		/// </example>
		public static Wnd WindowChildExists(Wnd w, Wnd.ChildFinder control, double secondsTimeout = 0.0, bool not = false)
		{
			if(not) WindowCondition(w, t => !control.Find(t), secondsTimeout, true); //shoulddo: optimize. Never mind, rarely used. Another branch cannot be optimized.
			else WindowCondition(w, t => control.Find(t), secondsTimeout);
			return control.Result;
		}

		/// <summary>
		/// Waits for an user-defined condition of an existing window.
		/// Returns true. If secondsTimeout is negative, after -secondsTimeout time returns false (else exception).
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
		/// </param>
		/// <param name="w">An existing window. It is passed to the callback function.</param>
		/// <param name="condition">Callback function (eg lambda). It is called repeatedly, until returns true.</param>
		/// <param name="doNotThrowIfClosed">
		/// Do not throw exception if w is invalid or the window was closed while waiting.
		/// In such case the callback function must return false. If need, it can use <see cref="Wnd.IsAlive"/>, like in the example. Else the exception is thrown (with a small delay) to prevent infinite waiting.
		/// </param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired.</exception>
		/// <exception cref="WndException">w is invalid or the window was closed while waiting.</exception>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("* Notepad");
		/// 
		/// //wait max 30 s until window w is active. Exception on timeout or if closed.
		/// WaitFor.WindowCondition(w, t => t.IsActive); //the same as WaitFor.WindowActive(w, 30);
		/// Print("active");
		/// 
		/// //wait indefinitely until window w is minimized or closed
		/// WaitFor.WindowCondition(w, t => t.IsMinimized || !t.IsAlive, 0, true);
		/// if(!w.IsAlive) { Print("closed"); return; }
		/// Print("minimized");
		/// ]]></code>
		/// </example>
		public static bool WindowCondition(Wnd w, Func<Wnd, bool> condition, double secondsTimeout = 0.0, bool doNotThrowIfClosed = false)
		{
			bool wasInvalid = false;
			var to = new LibTimeout(secondsTimeout);
			for(;;) {
				if(!doNotThrowIfClosed) w.ThrowIfInvalid();
				if(condition(w)) return true;
				if(doNotThrowIfClosed) {
					if(wasInvalid) w.ThrowIfInvalid();
					wasInvalid = !w.IsAlive;
				}
				if(!to.Sleep()) return false;
			}
		}
	}
}
