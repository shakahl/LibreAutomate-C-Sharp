
namespace Au
{
	public partial struct wnd
	{
		/// <summary>
		/// Waits until window exists, is visible (optionally) and active (optionally).
		/// Returns window handle. On timeout returns default(wnd) if <i>secondsTimeout</i> is negative; else exception.
		/// Parameters etc are the same as <see cref="find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="active">The window must be the active window (<see cref="active"/>), and not minimized.</param>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="find"/>.</exception>
		/// <remarks>
		/// By default ignores invisible and cloaked windows. Use flags if need.
		/// If you have a window's wnd variable, to wait until it is active/visible/etc use <see cref="WaitForCondition"/> instead.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// wnd w = wnd.wait(10, false, "* Notepad");
		/// print.it(w);
		/// ]]></code>
		/// Using in a Form/Control event handler.
		/// <code><![CDATA[
		/// var f = new Form();
		/// f.Click += async (_, _) =>
		///   {
		/// 	  print.it("waiting for Notepad...");
		/// 	  wnd w = await Task.Run(() => wnd.wait(-10, false, "* Notepad"));
		/// 	  if(w.Is0) print.it("timeout"); else print.it(w);
		///   };
		/// f.ShowDialog();
		/// ]]></code>
		/// </example>
		public static wnd wait(double secondsTimeout, bool active,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
			[ParamString(PSFormat.wildex)] string name = null,
			[ParamString(PSFormat.wildex)] string cn = null,
			[ParamString(PSFormat.wildex)] WOwner of = default,
			WFlags flags = 0, Func<wnd, bool> also = null, WContains contains = default)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		{
			var f = new wndFinder(name, cn, of, flags, also, contains);
			var to = new wait.Loop(secondsTimeout);
			for (; ; ) {
				if (active) {
					wnd w = wnd.active;
					if (f.IsMatch(w) && !w.IsMinimized) return w;
				} else {
					if (f.Find()) return f.Result;
				}
				if (!to.Sleep()) return default;
			}
		}
		//SHOULDDO: if wait for active, also wait until released mouse buttons.

		/// <summary>
		/// Waits until any of specified windows exists, is visible (optionally) and active (optionally).
		/// Returns 1-based index and window handle. On timeout returns <c>(0, default(wnd))</c> if <i>secondsTimeout</i> is negative; else exception.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="active">The window must be the active window (<see cref="active"/>), and not minimized.</param>
		/// <param name="windows">One or more variables containing window properties. Can be strings, see <see cref="wndFinder.op_Implicit(string)"/>.</param>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <remarks>
		/// By default ignores invisible and cloaked windows. Use <b>wndFinder</b> flags if need.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var (i, w) = wnd.waitAny(10, true, "* Notepad", new wndFinder("* Word"));
		/// print.it(i, w);
		/// ]]></code>
		/// </example>
		public static (int index, wnd w) waitAny(double secondsTimeout, bool active, params wndFinder[] windows) {
			foreach (var f in windows) f.Result = default;
			WFCache cache = active && windows.Length > 1 ? new WFCache() : null;
			var to = new wait.Loop(secondsTimeout);
			for (; ; ) {
				if (active) {
					wnd w = wnd.active;
					for (int i = 0; i < windows.Length; i++) {
						if (windows[i].IsMatch(w, cache) && !w.IsMinimized) return (i + 1, w);
					}
				} else {
					for (int i = 0; i < windows.Length; i++) {
						var f = windows[i];
						if (f.Find()) return (i + 1, f.Result);
					}
					//FUTURE: optimization: get list of windows once (Lib.EnumWindows2).
					//	Problem: list filtering depends on wndFinder flags. Even if all finders have same flags, its easy to make bugs.
				}
				if (!to.Sleep()) return default;
			}
		}

		//rejected. Not useful. Use the non-static WaitForClosed.
		//		/// <summary>
		//		/// Waits until window does not exist.
		//		/// Parameters etc are the same as <see cref="Find"/>.
		//		/// </summary>
		//		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		//		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		//		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		//		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		//		/// <remarks>
		//		/// By default ignores invisible and cloaked windows. Use flags if need.
		//		/// If you have a window's wnd variable, to wait until it is closed use <see cref="WaitForClosed"/> instead.
		//		/// Examples: <see cref="Wait"/>.
		//		/// </remarks>
		//		public static bool waitNot(double secondsTimeout,
		//#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		//			[ParamString(PSFormat.wildex)] string name = null,
		//			[ParamString(PSFormat.wildex)] string cn = null,
		//			[ParamString(PSFormat.wildex)] WOwner of = default,
		//			WFlags flags = 0, Func<wnd, bool> also = null, WContents contains = default)
		//#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		//		{
		//			var f = new wndFinder(name, cn, of, flags, also, contains);
		//			return WaitNot(secondsTimeout, out _, f);
		//		}

		//		/// <summary>
		//		/// Waits until window does not exist.
		//		/// </summary>
		//		/// <param name="secondsTimeout"></param>
		//		/// <param name="wFound">On timeout receives the first found matching window that exists.</param>
		//		/// <param name="f">Window properties etc. Can be string, see <see cref="wndFinder.op_Implicit(string)"/>.</param>
		//		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		//		public static bool waitNot(double secondsTimeout, out wnd wFound, wndFinder f)
		//		{
		//			wFound = default;
		//			var to = new wait.Loop(secondsTimeout);
		//			wnd w = default;
		//			for(; ; ) {
		//				if(!w.IsAlive || !f.IsMatch(w)) { //if first time, or closed (!IsAlive), or changed properties (!IsMatch)
		//					if(!f.Find()) { wFound = default; return true; }
		//					wFound = w = f.Result;
		//				}
		//				if(!to.Sleep()) return false;
		//			}
		//		}

		//rejected. Cannot use implicit conversion string to wndFinder.
		//public static bool waitNot(double secondsTimeout, wndFinder f)
		//	=> WaitNot(secondsTimeout, out _, f);

		//Not often used. It's easy with await Task.Run. Anyway, need to provide an example of similar size.
		//public static async Task<wnd> waitAsync(double secondsTimeout, string name)
		//{
		//	return await Task.Run(() => wait(secondsTimeout, name));
		//}

		/// <summary>
		/// Waits for a user-defined state/condition of this window. For example active, visible, enabled, closed, contains control.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="condition">Callback function (eg lambda). It is called repeatedly, until returns true.</param>
		/// <param name="dontThrowIfClosed">
		/// Do not throw exception when the window handle is invalid or the window was closed while waiting.
		/// In such case the callback function must return false, like in the examples with <see cref="IsAlive"/>. Else exception is thrown (with a small delay) to prevent infinite waiting.
		/// </param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">The window handle is invalid or the window was closed while waiting.</exception>
		/// <example>
		/// <code><![CDATA[
		/// wnd w = wnd.find("* Notepad");
		/// 
		/// //wait max 30 s until window w is active. Exception on timeout or if closed.
		/// w.WaitForCondition(30, t => t.IsActive);
		/// print.it("active");
		/// 
		/// //wait max 30 s until window w is enabled. Exception on timeout or if closed.
		/// w.WaitForCondition(30, t => t.IsEnabled);
		/// print.it("enabled");
		/// 
		/// //wait until window w is closed
		/// w.WaitForCondition(0, t => !t.IsAlive, true); //same as w.WaitForClosed()
		/// print.it("closed");
		/// 
		/// //wait until window w is minimized or closed
		/// w.WaitForCondition(0, t => t.IsMinimized || !t.IsAlive, true);
		/// if(!w.IsAlive) { print.it("closed"); return; }
		/// print.it("minimized");
		/// 
		/// //wait until window w contains focused control classnamed "Edit"
		/// var c = new wndChildFinder(cn: "Edit");
		/// w.WaitForCondition(10, t => c.Find(t) && c.Result.IsFocused);
		/// print.it("control focused");
		/// ]]></code>
		/// </example>
		public bool WaitForCondition(double secondsTimeout, Func<wnd, bool> condition, bool dontThrowIfClosed = false) {
			bool wasInvalid = false;
			var to = new wait.Loop(secondsTimeout);
			for (; ; ) {
				if (!dontThrowIfClosed) ThrowIfInvalid();
				if (condition(this)) return true;
				if (dontThrowIfClosed) {
					if (wasInvalid) ThrowIfInvalid();
					wasInvalid = !IsAlive;
				}
				if (!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits until this window has the specified name.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="name">
		/// Window name. Usually it is the title bar text.
		/// String format: [](xref:wildcard_expression).
		/// </param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuWndException">The window handle is invalid or the window was closed while waiting.</exception>
		/// <exception cref="ArgumentException">Invalid wildcard expression.</exception>
		public bool WaitForName(double secondsTimeout, [ParamString(PSFormat.wildex)] string name) {
			wildex x = name; //ArgumentNullException
			return WaitForCondition(secondsTimeout, t => x.Match(t.Name));
		}

		/// <summary>
		/// Waits until this window is closed/destroyed or until its process ends.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="waitUntilProcessEnds">Wait until the process of this window ends.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuException">Failed to open process handle when <i>waitUntilProcessEnds</i> is true.</exception>
		/// <remarks>
		/// If the window is already closed, immediately returns true.
		/// </remarks>
		public bool WaitForClosed(double secondsTimeout, bool waitUntilProcessEnds = false) {
			if (!waitUntilProcessEnds) return WaitForCondition(secondsTimeout, t => !t.IsAlive, true);

			//SHOULDDO: if window of this thread or process...

			if (!IsAlive) return true;
			using var ph = Handle_.OpenProcess(this, Api.SYNCHRONIZE);
			if (ph.Is0) {
				var e = new AuException(0, "*open process handle"); //info: with SYNCHRONIZE can open process of higher IL
				if (!IsAlive) return true;
				throw e;
			}
			return 0 != Au.wait.forHandle(secondsTimeout, opt.wait.DoEvents ? WHFlags.DoEvents : 0, ph);
		}
	}
}
