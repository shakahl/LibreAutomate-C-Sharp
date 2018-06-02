using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	public partial struct Wnd
	{
		/// <summary>
		/// Waits until window exists, is visible (optionally) and active (optionally).
		/// Returns window handle. On timeout returns default(Wnd) if <paramref name="secondsTimeout"/> is negative; else exception.
		/// Parameters etc are the same as <see cref="Find"/>.
		/// </summary>
		/// <param name="active">The window must be the active window (<see cref="WndActive"/>), and not minimized.</param>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits infinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns default(Wnd).
		/// </param>
		/// <param name="name"></param>
		/// <param name="className"></param>
		/// <param name="programEtc"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="contains"></param>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <remarks>
		/// By default ignores invisible windows. Use flag <see cref="WFFlags.HiddenToo"/> if need.
		/// If you have a window's Wnd variable, to wait until it is active/visible/etc use <see cref="WaitForCondition"/> instead.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Wait(false, 10, "* Notepad");
		/// Print(w);
		/// ]]></code>
		/// Using in a Form/Control event handler.
		/// <code><![CDATA[
		/// var f = new Form();
		/// f.Click += async (unu, sed) =>
		///   {
		/// 	  Print("waiting for Notepad...");
		/// 	  Wnd w = await Task.Run(() => Wnd.Wait(false, -10, "* Notepad"));
		/// 	  if(w.Is0) Print("timeout"); else Print(w);
		///   };
		/// f.ShowDialog();
		/// ]]></code>
		/// </example>
		public static Wnd Wait(bool active, double secondsTimeout = 0.0,
			string name = null, string className = null, string programEtc = null,
			WFFlags flags = 0, Func<Wnd, bool> also = null, object contains = null)
		{
			var f = new Finder(name, className, programEtc, flags, also, contains);
			var to = new WaitFor.Loop(secondsTimeout);
			for(; ; ) {
				if(active) {
					Wnd w = WndActive;
					if(f.IsMatch(w) && !w.IsMinimized) return w;
				} else {
					if(f.Find()) return f.Result;
				}
				if(!to.Sleep()) return default;
			}
		}

		/// <summary>
		/// Waits until any of specified windows exists, is visible (optionally) and active (optionally).
		/// Returns window handle. On timeout returns default(Wnd) if <paramref name="secondsTimeout"/> is negative; else exception.
		/// </summary>
		/// <param name="active">The window must be the active window (<see cref="WndActive"/>), and not minimized.</param>
		/// <param name="secondsTimeout"><inheritdoc cref="Wait"/></param>
		/// <param name="windows">One or more variables containing window properties.</param>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <remarks>
		/// By default ignores invisible windows. Use flag <see cref="WFFlags.HiddenToo"/> if need.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.WaitAny(true, 10, new Wnd.Finder("* Notepad"), new Wnd.Finder("* Word"));
		/// Print(w);
		/// ]]></code>
		/// </example>
		public static Wnd WaitAny(bool active, double secondsTimeout, params Finder[] windows)
		{
			foreach(var f in windows) f.Result = default;
			var to = new WaitFor.Loop(secondsTimeout);
			for(; ; ) {
				if(active) {
					Wnd w = WndActive;
					foreach(var f in windows) if(f.IsMatch(w) && !w.IsMinimized) return w;
				} else {
					foreach(var f in windows) if(f.Find()) return f.Result;
					//FUTURE: optimization: get list of windows once (Lib.EnumWindows2).
					//	Problem: list filtering depends on Finder flags. Even if all finders have same flags, its easy to make bugs.
				}
				if(!to.Sleep()) return default;
			}
		}

		/// <summary>
		/// Waits until window does not exist.
		/// Returns default(Wnd). On timeout returns window handle if <paramref name="secondsTimeout"/> is negative; else exception.
		/// All undocumented parameters etc are the same as <see cref="Find"/>.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits infinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns window handle.
		/// </param>
		/// <param name="name"></param>
		/// <param name="className"></param>
		/// <param name="programEtc"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="contains"></param>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <remarks>
		/// By default ignores invisible windows. Use flag <see cref="WFFlags.HiddenToo"/> if need.
		/// If you have a window's Wnd variable, to wait until it is closed/etc use <see cref="WaitForCondition"/> instead.
		/// Examples: <see cref="Wait"/>.
		/// </remarks>
		public static Wnd WaitNot(double secondsTimeout = 0.0,
			string name = null, string className = null, string programEtc = null,
			WFFlags flags = 0, Func<Wnd, bool> also = null, object contains = null)
		{
			var f = new Finder(name, className, programEtc, flags, also, contains);
			return WaitNot(secondsTimeout, f);
		}

		/// <summary>
		/// The same as <see cref="WaitNot(double, string, string, string, WFFlags, Func{Wnd, bool}, object)"/>. Arguments are passed differently.
		/// </summary>
		/// <param name="secondsTimeout"></param>
		/// <param name="f">Window properties etc.</param>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		public static Wnd WaitNot(double secondsTimeout, Finder f)
		{
			var to = new WaitFor.Loop(secondsTimeout);
			Wnd w = default;
			for(; ; ) {
				if(!w.IsAlive || !f.IsMatch(w)) { //if first time, or closed (!IsAlive), or changed properties (!IsMatch)
					if(!f.Find()) return default;
					w = f.Result;
				}
				if(!to.Sleep()) return f.Result;
			}
		}

		//Not often used. It's easy with await Task.Run. Anyway, need to provide an example of similar size.
		//public static async Task<Wnd> WaitAsync(double secondsTimeout, string name)
		//{
		//	return await Task.Run(() => Wait(secondsTimeout, name));
		//}

		/// <summary>
		/// Waits for an user-defined state/condition of this window. For example active, visible, enabled, closed, contains control.
		/// </summary>
		/// <param name="secondsTimeout"><inheritdoc cref="WaitFor.Condition"/></param>
		/// <param name="condition">Callback function (eg lambda). It is called repeatedly, until returns true.</param>
		/// <param name="doNotThrowIfClosed">
		/// Do not throw exception when the window handle is invalid or the window was closed while waiting.
		/// In such case the callback function must return false, like in the examples with <see cref="IsAlive"/>. Else exception is thrown (with a small delay) to prevent infinite waiting.
		/// </param>
		/// <returns><inheritdoc cref="WaitFor.Condition"/></returns>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="WndException">The window handle is invalid or the window was closed while waiting.</exception>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("* Notepad");
		/// 
		/// //wait max 30 s until window w is active. Exception on timeout or if closed.
		/// w.WaitForCondition(30, t => t.IsActive);
		/// Print("active");
		/// 
		/// //wait max 30 s until window w is enabled. Exception on timeout or if closed.
		/// w.WaitForCondition(30, t => t.IsEnabled);
		/// Print("enabled");
		/// 
		/// //wait until window w is closed
		/// w.WaitForCondition(0, t => !t.IsAlive, true); //same as w.WaitForClosed()
		/// Print("closed");
		/// 
		/// //wait until window w is minimized or closed
		/// w.WaitForCondition(0, t => t.IsMinimized || !t.IsAlive, true);
		/// if(!w.IsAlive) { Print("closed"); return; }
		/// Print("minimized");
		/// 
		/// //wait until window w contains focused control classnamed "Edit"
		/// var c = new Wnd.ChildFinder(className: "Edit");
		/// w.WaitForCondition(10, t => c.Find(t) && c.Result.IsFocused);
		/// Print("control focused");
		/// ]]></code>
		/// </example>
		public bool WaitForCondition(double secondsTimeout, Func<Wnd, bool> condition, bool doNotThrowIfClosed = false)
		{
			bool wasInvalid = false;
			var to = new WaitFor.Loop(secondsTimeout);
			for(; ; ) {
				if(!doNotThrowIfClosed) ThrowIfInvalid();
				if(condition(this)) return true;
				if(doNotThrowIfClosed) {
					if(wasInvalid) ThrowIfInvalid();
					wasInvalid = !IsAlive;
				}
				if(!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits until this window has the specified name.
		/// </summary>
		/// <param name="secondsTimeout"><inheritdoc cref="WaitFor.Condition"/></param>
		/// <param name="name">
		/// Window name. Usually it is the title bar text.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// </param>
		/// <returns><inheritdoc cref="WaitFor.Condition"/></returns>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="WndException">The window handle is invalid or the window was closed while waiting.</exception>
		/// <exception cref="ArgumentException">Invalid wildcard expression.</exception>
		public bool WaitForName(double secondsTimeout, string name)
		{
			Wildex x = name; //ArgumentNullException
			return WaitForCondition(secondsTimeout, t => x.Match(t.Name));
		}

		/// <summary>
		/// Waits until this window is closed/destroyed or until its process ends.
		/// </summary>
		/// <param name="secondsTimeout"><inheritdoc cref="WaitFor.Condition"/></param>
		/// <param name="waitUntilProcessEnds">Wait until the process of this window ends.</param>
		/// <returns><inheritdoc cref="WaitFor.Condition"/></returns>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="AuException">Failed to open process handle when <paramref name="waitUntilProcessEnds"/> is true.</exception>
		/// <remarks>
		/// If the window is already closed, immediately returns true.
		/// </remarks>
		public bool WaitForClosed(double secondsTimeout = 0d, bool waitUntilProcessEnds = false)
		{
			if(!waitUntilProcessEnds) return WaitForCondition(secondsTimeout, t => !t.IsAlive, true);

			if(!IsAlive) return true;
			using(var ph = Process_.LibProcessHandle.FromWnd(this, Api.SYNCHRONIZE)) {
				if(ph.Is0) {
					var e = new AuException(0, "*open process handle"); //info: with SYNCHRONIZE can open process of higher IL
					if(!IsAlive) return true;
					throw e;
				}
				return 0 != WaitFor.Handle(secondsTimeout, Opt.WaitFor.DoEvents ? WHFlags.DoEvents : 0, ph.Handle);
			}
		}
	}
}
