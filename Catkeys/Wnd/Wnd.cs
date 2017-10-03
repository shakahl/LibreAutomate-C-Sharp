using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

using Catkeys.Types;
using static Catkeys.NoClass;

#pragma warning disable 282 //intellisense bug: it thinks that Wnd has multiple fields.

namespace Catkeys
{
	/// <summary>
	/// A variable of Wnd type represents a native window or control. It contains native window handle, also known as HWND.
	/// Wnd functions can be used with windows and controls of any process/thread.
	/// Wnd functions also can be used with .NET Form and Control instances, like <c>Wnd w=(Wnd)netControl; w.Method(...);</c> or <c>((Wnd)netControl).Method(...);</c>.
	/// </summary>
	/// <remarks>
	/// There are two main types of windows - top-level windows and controls. Controls are child windows of top-level windows.
	/// 
	/// More functions are in the nested classes - Wnd.Misc, Wnd.Misc.Desktop etc. They are used rarely, mostly in programming, not in automation scripts.
	/// 
	/// What happens when a Wnd function fails:
	/// <list type="bullet">
	/// <item>Functions that get window properties don't throw exceptions. They return false/0/null/empty. Most of them support <see cref="Native.GetError"/>, and it is mentioned in function documentation.</item>
	/// <item>Many functions that change window properties throw exception. Exceptions are listed in function documentation. Almost all these functions throw only <see cref="WndException"/>.</item>
	/// <item>Other functions that change window properties return false. They are more often used in programming than in automation scripts.</item>
	/// <item>When a 'find' function does not find the window or control, it returns default(Wnd) (window handle 0). Then <see cref="Is0"/> will return true.</item>
	/// <item>If a function does not follow these rules, it is mentioned in function documentation.</item>
	/// </list>
	/// 
	/// Many functions fail if the window's process has higher <see cref="Process_.UacInfo">UAC integrity level</see> (aministrator, uiAccess) than this process, unless this process has uiAccess level. Especially the functions that change window properties. Some functions that still work: Activate, ActivateLL, ShowMinimized, ShowNotMinimized, ShowNotMinMax, Close.
	/// 
	/// The Wnd type can be used with native Windows API functions wihout casting. Use Wnd for the parameter type in the declaration, like <c>[DllImport(...)] static extern bool NativeFunction(Wnd hWnd, ...)</c>.
	/// 
	/// See also: MSDN article <msdn>Window Features</msdn>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Wnd w = Wnd.Find("* - Notepad");
	/// if(w.Is0) { Print("window not found"); return; }
	/// w.Activate();
	/// Wnd c = w.Child(className: "Edit");
	/// Print(c.Name);
	/// ]]></code>
	/// </example>
	[Serializable]
	public unsafe partial struct Wnd :IEquatable<Wnd>
	{
#if false
		/// Why Wnd is struct, not class:
		///		Advantages:
		///		<list type="number">
		///		<item>Lightweight. Contains just void*, which is 4 or 8 bytes.</item>
		///		<item>Easier to create overloaded functions that have a window parameter. If it was a class, then a null argument could be ambiguous if eg could also be a string etc.</item>
		///		<item>When a find-window function does not find the window, calling next function (without checking the return value) does not throw null-reference exception. Instead the function can throw a more specific exception or just return false etc.</item>
		///		<item>The handle actually already is a reference (to a window object managed by the OS). We don't own the object; we usually don't need to destroy the window finally; it is more like a numeric window id.</item>
		///		<item>Code where a window argument is default(Wnd) is more clear. If it would be null, then it is unclear how the function interprets it: as a 0 handle or as "don't use it". Now if we want a "don't use it" behavior, we'll use an overload.</item>
		///		<item>In my experience, it makes programming/scripting easier that if it would be a class. Because windows are not found so often (in automation scripts). A find-window function could throw a 'not found' exception, but it is not good (it's easier to check the return value than to use try/catch or throwing/nonthrowing overloads).</item>
		///		<item>Probably it is not a "bad practice" to have a struct with many member functions, because eg the .NET DateTime is a struct.</item>
		///		</list>
		///		Disadvantages:
		///		<list type="number">
		///		<item>Cannot be a base class of other classes. Workaround: Use it as a public field or property of the other class (or struct); in some cases it can be even better, because Wnd has very many methods, and the non-inherited methods of that class would be difficult to find; now they are separated, and can be used like x.NewClassMethod() and x.w.WndMethod(); anyway, in most cases we'll need the new window classes only for the functions that they add, not for Wnd functions, eg we would use a class ButtonWnd mostly only for button functions, not for general window functions.</item>
		///		<item>In some cases C# does not allow to call a property-set function. Wnd has few such functions, maybe none.</item>
		///		</list>
		///		
		//note: don't use :IWin32Window, because it loads System.Windows.Forms.dll always when Wnd used.
#endif

		void* _h;

		#region constructors, operators, overrides, constants

#pragma warning disable 1591 //XML doc
		Wnd(void* hwnd) { _h = hwnd; }
		Wnd(IntPtr hwnd) { _h = (void*)hwnd; }

		public static explicit operator Wnd(IntPtr hwnd) { return new Wnd(hwnd); } //Wnd=(Wnd)IntPtr //don't need implicit, it creates more problems than is useful
		public static explicit operator IntPtr(Wnd w) { return w.Handle; } //IntPtr=(IntPtr)Wnd //could be implicit, but then problems with operator ==
		public static explicit operator Wnd(LPARAM hwnd) { return new Wnd((void*)hwnd); } //Wnd=(Wnd)LPARAM
		public static explicit operator LPARAM(Wnd w) { return w._h; } //LPARAM=(LPARAM)Wnd

		/// <summary>
		/// Gets the window handle as Wnd from a System.Windows.Forms.Control (or Form etc) variable.
		/// The same as the extension method Wnd_().
		/// If the handle is still not created, the Control auto-creates it (hidden window).
		/// </summary>
		public static explicit operator Wnd(Control c) { return new Wnd(c == null ? Zero : c.Handle); } //Wnd=(Wnd)Control //implicit would allow Wnd==null

		/// <summary>
		/// If this window is a System.Windows.Forms.Control (or Form etc), gets the Control variable, else returns null.
		/// </summary>
		public static explicit operator Control(Wnd w) { return Control.FromHandle(w.Handle); } //Control=(Control)Wnd

		/// <summary>Compares window handles.</summary>
		public static bool operator ==(Wnd w1, Wnd w2) { return w1._h == w2._h; }
		/// <summary>Compares window handles.</summary>
		public static bool operator !=(Wnd w1, Wnd w2) { return w1._h != w2._h; }

		//Prevent accidental usage Wnd==null. The C# compiler allows it without a warning. As a side effect, the above also disables Wnd==Wnd?.
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator ==(Wnd w1, Wnd? w2) { return false; }
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator !=(Wnd w1, Wnd? w2) { return true; }
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator ==(Wnd? w1, Wnd w2) { return false; }
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator !=(Wnd? w1, Wnd w2) { return true; }
#pragma warning restore 1591 //XML doc

		/// <summary>
		/// Returns true if w == this.
		/// </summary>
		public bool Equals(Wnd w) //IEquatable<Wnd>.Equals, to avoid boxing with eg Dictionary<Wnd, T2>
		{
			return w == this;

			//TODO: LPARAM etc must support IEquatable too.
		}

		/// <summary>
		/// Returns true if w != null and w.Value == this.
		/// </summary>
		public bool Equals(Wnd? w)
		{
			return w != null && w.GetValueOrDefault() == this;
		}

		/// <summary>
		/// Returns true if obj is this Wnd.
		/// </summary>
		public override bool Equals(object obj)
		{
			//return obj is Wnd w && this == w; //compiler creates slow and big code if 'is ValueType variable'
			return obj is Wnd && this == (Wnd)obj;
		}

		///
		public override int GetHashCode()
		{
			return (int)_h;
			//window handles are always 32-bit int, although in a 64-bit process stored in 64-bit variables.
			//IntPtr.GetHashCode also returns this.
		}

		/// <summary>
		/// Gets window handle as IntPtr.
		/// Code <c>w.Handle</c> is the same as <c>(IntPtr)w</c> .
		/// </summary>
		public IntPtr Handle { get => new IntPtr(_h); }

		/// <summary>
		/// Formats string $"{handle}  {ClassName}  \"{Name}\"  {ProcessName}  {Rect}".
		/// </summary>
		public override string ToString()
		{
			if(Is0) return "0";
			var cn = ClassName;
			var sh = Handle.ToString();
			if(cn == null) return sh + " <invalid handle>";
			string s = Name;
			if(s != null) s = s.Limit_(250).Escape_();
			return $"{sh}  {cn}  \"{s}\"  {ProcessName}  {Rect.ToString()}";
		}

		#endregion

		#region send/post message

		/// <summary>
		/// Calls API <msdn>SendMessage</msdn>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM Send(uint message, LPARAM wParam = default, LPARAM lParam = default)
		{
			return Api.SendMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API <msdn>SendMessage</msdn> where lParam is string.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM SendS(uint message, LPARAM wParam, string lParam)
		{
			fixed (char* p = lParam)
				return Api.SendMessage(this, message, wParam, p);
			//info: don't use overload, then eg ambiguous if null.
		}

		/// <summary>
		/// Calls API <msdn>SendMessage</msdn> where lParam is char[].
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM SendS(uint message, LPARAM wParam, char[] lParam)
		{
			fixed (char* p = lParam)
				return Api.SendMessage(this, message, wParam, p);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn>.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// flags can be Native.SMTO_.
		/// </summary>
		public bool SendTimeout(int timeoutMS, uint message, LPARAM wParam = default, LPARAM lParam = default, uint flags = Native.SMTO_ABORTIFHUNG)
		{
			return 0 != Api.SendMessageTimeout(this, message, wParam, lParam, flags, (uint)timeoutMS, out LPARAM R);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn> and gets the result of the message processing.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// flags can be Native.SMTO_.
		/// </summary>
		public bool SendTimeout(int timeoutMS, out LPARAM result, uint message, LPARAM wParam = default, LPARAM lParam = default, uint flags = Native.SMTO_ABORTIFHUNG)
		{
			result = 0;
			return 0 != Api.SendMessageTimeout(this, message, wParam, lParam, flags, (uint)timeoutMS, out result);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn> where lParam is string.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// flags can be Native.SMTO_.
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, out LPARAM result, uint message, LPARAM wParam, string lParam, uint flags = Native.SMTO_ABORTIFHUNG)
		{
			result = 0;
			fixed (char* p = lParam)
				return 0 != Api.SendMessageTimeout(this, message, wParam, p, flags, (uint)timeoutMS, out result);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn> where lParam is char[].
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// flags can be Native.SMTO_.
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, out LPARAM result, uint message, LPARAM wParam, char[] lParam, uint flags = Native.SMTO_ABORTIFHUNG)
		{
			result = 0;
			fixed (char* p = lParam)
				return 0 != Api.SendMessageTimeout(this, message, wParam, p, flags, (uint)timeoutMS, out result);
		}

		/// <summary>
		/// Calls API <msdn>SendNotifyMessage</msdn>.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		public bool SendNotify(uint message, LPARAM wParam = default, LPARAM lParam = default)
		{
			return Api.SendNotifyMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API <msdn>PostMessage</msdn>.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <seealso cref="Misc.PostThreadMessage(uint, LPARAM, LPARAM)"/>
		public bool Post(uint message, LPARAM wParam = default, LPARAM lParam = default)
		{
			return Api.PostMessage(this, message, wParam, lParam);
		}

		public static partial class Misc
		{
			/// <summary>
			/// Posts a message to the message queue of this thread.
			/// Calls API <msdn>PostMessage</msdn> with default(Wnd). 
			/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
			/// </summary>
			public static bool PostThreadMessage(uint message, LPARAM wParam = default, LPARAM lParam = default)
			{
				return Api.PostMessage(default, message, wParam, lParam);
			}

			/// <summary>
			/// Posts a message to the message queue of the specified thread.
			/// Calls API <msdn>PostThreadMessage</msdn>. 
			/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
			/// </summary>
			public static bool PostThreadMessage(uint threadId, uint message, LPARAM wParam = default, LPARAM lParam = default)
			{
				return Api.PostThreadMessage(threadId, message, wParam, lParam);
			}
		}

		#endregion

		#region throw, valid

		/// <summary>
		/// If <see cref="Is0"/>, throws <see cref="WndException"/>.
		/// </summary>
		/// <exception cref="WndException"></exception>
		public void ThrowIf0()
		{
			if(_h == null) throw new WndException(this, Api.ERROR_INVALID_WINDOW_HANDLE);
		}

		/// <summary>
		/// If <see cref="Is0"/> or !<see cref="IsAlive"/>, throws <see cref="WndException"/>.
		/// </summary>
		/// <exception cref="WndException"></exception>
		public void ThrowIfInvalid()
		{
			if(_h == null || !Api.IsWindow(this)) throw new WndException(this, Api.ERROR_INVALID_WINDOW_HANDLE);
		}

		/// <summary>
		/// Throws <see cref="WndException"/> that uses the last Windows API error (code and message).
		/// Also the message depends on whether the window handle is 0/invalid.
		/// </summary>
		/// <exception cref="WndException"></exception>
		public void ThrowUseNative()
		{
			throw new WndException(this, 0);
		}

		/// <summary>
		/// Throws <see cref="WndException"/> that uses mainMessage and the last Windows API error (code and message).
		/// Also the message depends on whether the window handle is 0/invalid.
		/// </summary>
		/// <exception cref="WndException"></exception>
		public void ThrowUseNative(string mainMessage)
		{
			throw new WndException(this, 0, mainMessage);
		}

		/// <summary>
		/// Throws <see cref="WndException"/> that uses mainMessage and does not use the last Windows API error.
		/// Also the message depends on whether the window handle is 0/invalid.
		/// </summary>
		/// <exception cref="WndException"></exception>
		public void ThrowNoNative(string mainMessage)
		{
			throw new WndException(this, mainMessage);
		}

		/// <summary>
		/// Returns true if the <see cref="Wnd">handle</see> is 0.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("Window*");
		/// if(w.Is0) { Print("window not found"); return; }
		/// ]]></code>
		/// </example>
		/// <seealso cref="IsAlive"/>
		public bool Is0 { get => _h == null; }

		/// <summary>
		/// Returns true if the <see cref="Wnd">handle</see> identifies an existing window.
		/// Returns false if the handle is 0 or invalid.
		/// Invalid non-0 handle usually means that the window is closed/destroyed.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="Is0"/> and API <msdn>IsWindow</msdn>.
		/// Although a Wnd variable holds a window handle, which is like a reference to a window, it does not prevent closing that window and making the handle invalid. After closing the window, the OS can even assign the same handle value to a new window, although normally it can happen only after long time.
		/// <note>Use this carefully with windows of other applications or threads. The window can be closed at any moment, even when your thread is still in this function.</note>
		/// </remarks>
		public bool IsAlive { get => !Is0 && Api.IsWindow(this); }

		#endregion

		#region visible, enabled, cloaked

		/// <summary>
		/// Gets the visible state.
		/// Returns true if the window is visible.
		/// Returns false if is invisible or is a child of invisible parent.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>IsWindowVisible</msdn>.
		/// <note>This function is unaware about <see cref="IsCloaked">cloaked</see> windows. If need, use code <c>if(w.IsVisible &amp;&amp; !w.IsCloaked)</c>.</note>
		/// <note>This function is unaware about transparent windows, zero-size windows, zero-window-region windows, off-screen windows and windows that are completely covered by other windows.</note>
		/// </remarks>
		/// <seealso cref="IsCloaked"/>
		/// <seealso cref="Show"/>
		/// <seealso cref="Activate()"/>
		public bool IsVisible
		{
			get => Api.IsWindowVisible(this);
		}

		/// <summary>
		/// Shows (if hidden) or hides this window.
		/// Does not activate/deactivate/zorder.
		/// With windows of current thread usually it's better to use <see cref="ShowLL">ShowLL</see>.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>ShowWindow</msdn> with SW_SHOWNA or SW_HIDE.
		/// </remarks>
		/// <exception cref="WndException"/>
		public void Show(bool show)
		{
			if(!ShowLL(show)) ThrowUseNative(show ? "*show*" : "*hide*");
			_MinimalWaitIfOtherThread();
		}

		/// <summary>
		/// Shows (if hidden) or hides this window.
		/// Does not activate/deactivate/zorder.
		/// </summary>
		/// <remarks>
		/// This library has two similar functions - <see cref="Show">Show</see> and <b>ShowLL</b>. <b>Show</b> is better to use in automation scripts, with windows of any process/thread. <b>ShowLL</b> usually is better to use in programming, with windows of current thread.
		/// <b>ShowLL</b> is more low-level. Does not throw exception when fails, and does not add a delay; <b>Show</b> adds a small delay when the window is of other thread.
		/// 
		/// Calls API <msdn>ShowWindow</msdn> with SW_SHOWNA or SW_HIDE.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public bool ShowLL(bool show)
		{
			//Is it already visible?
			//This check makes MUCH faster when already visible (read comments below).
			//But don't check when !show. Then would not hide if the parent is hidden, unless we instead check WS_VISIBLE style (not tested). And it does not make much faster.
			if(show && IsVisible) return true;
			Native.ClearError();
			Api.ShowWindow(this, show ? Api.SW_SHOWNA : Api.SW_HIDE);
			return Native.GetError() == 0;

			//speed when the window already is in the requested state:
			//	ShowWindow and SetWindowPos don't test it.
			//	ShowWindow(SW_SHOW/SW_HIDE) are quite fast (maybe 5 times slower than IsWindowVisible).
			//	But SW_SHOW activates the window if it is top-level (depends on the foreground lock), and we don't want it.
			//	SW_SHOWNA is MUCH slower.
			//	SetWindowPos is slightly faster than SW_SHOWNA. With SWP_NOSENDCHANGING faster, especially with windows of other threads, but not much.
			//speed when the window is not in the requested state:
			//	All similar.
		}

		/// <summary>
		/// Gets the enabled state.
		/// Returns true if the window is enabled.
		/// Returns false if is disabled or is a child of disabled parent.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>IsWindowEnabled</msdn>.
		/// </summary>
		public bool IsEnabled
		{
			get => Api.IsWindowEnabled(this);
		}

		/// <summary>
		/// Enables or disables the window.
		/// Calls API <msdn>EnableWindow</msdn>.
		/// </summary>
		/// <param name="enable">Enable or disable.</param>
		/// <exception cref="WndException"/>
		public void Enable(bool enable)
		{
			Native.ClearError();
			Api.EnableWindow(this, enable);
			if(Native.GetError() != 0) ThrowUseNative("*enable/disable*");
		}

		/// <summary>
		/// Gets the cloaked state.
		/// Returns 0 if not cloaked or if failed.
		/// Else returns flags: 1 cloaked by its application, 2 cloaked by Windows, 4 cloaked because its owner window is cloaked.
		/// On Windows 7 returns 0 because there is no "cloaked window" feature.
		/// </summary>
		/// <seealso cref="IsCloaked"/>
		public int IsCloakedGetState
		{
			get
			{
				if(!Ver.MinWin8) return 0;
				int hr = Api.DwmGetWindowAttribute(this, 14, out int cloaked, 4); //DWMWA_CLOAKED
				return cloaked;
			}
		}
		/// <summary>
		/// Returns true if the window is cloaked.
		/// Returns false if not cloaked or if failed.
		/// On Windows 7 returns false because there is no "cloaked window" feature.
		/// Windows 10 uses window cloaking mostly to hide windows on inactive desktops. Windows 8 - mostly to hide Metro app windows.
		/// </summary>
		/// <seealso cref="IsCloakedGetState"/>
		public bool IsCloaked
		{
			get => IsCloakedGetState != 0;
		}

		#endregion

		#region minimized, maximized

		/// <summary>
		/// Returns true if minimized, false if not.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>IsIconic</msdn>.
		/// </summary>
		public bool IsMinimized
		{
			get => Api.IsIconic(this);
		}

		/// <summary>
		/// Returns true if maximized, false if not.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>IsZoomed</msdn>.
		/// </summary>
		public bool IsMaximized
		{
			get => Api.IsZoomed(this);
		}

		/// <summary>
		/// If not minimized, minimizes.
		/// Also unhides.
		/// </summary>
		/// <param name="noAnimation">Visually fast, without animation.</param>
		/// <exception cref="WndException">The API call failed. No exception if the window did not obey.</exception>
		public void ShowMinimized(bool noAnimation = false)
		{
			_MinMaxRes(Api.SW_MINIMIZE, noAnimation);
		}

		/// <summary>
		/// If not minimized, minimizes.
		/// Also unhides.
		/// </summary>
		/// <param name="noAnimation">Visually fast, without animation.</param>
		/// <exception cref="WndException">The API call failed. No exception if the window did not obey.</exception>
		public void ShowMaximized(bool noAnimation = false)
		{
			_MinMaxRes(Api.SW_SHOWMAXIMIZED, noAnimation);
		}

		/// <summary>
		/// If maximized or minimized, makes normal (not min/max).
		/// Also unhides.
		/// </summary>
		/// <param name="noAnimation">Visually fast, without animation.</param>
		/// <exception cref="WndException">The API call failed. No exception if the window did not obey.</exception>
		public void ShowNotMinMax(bool noAnimation = false)
		{
			_MinMaxRes(Api.SW_SHOWNORMAL, noAnimation);
		}

		/// <summary>
		/// If minimized, restores previous non-minimized state (maximized or normal).
		/// Also unhides.
		/// </summary>
		/// <param name="noAnimation">Visually fast, without animation.</param>
		/// <exception cref="WndException">The API call failed. No exception if the window did not obey.</exception>
		public void ShowNotMinimized(bool noAnimation = false)
		{
			_MinMaxRes(Api.SW_RESTORE, noAnimation);
		}

		/// <summary>
		/// Sets window min/max/normal/restore state.
		/// Also unhides.
		/// </summary>
		/// <param name="state">Must be SW_MINIMIZE, SW_RESTORE (restores to normal/max if minimized), SW_SHOWNORMAL or SW_SHOWMAXIMIZED.</param>
		/// <param name="noAnimation">Use SetWindowPlacement (no animation).</param>
		/// <exception cref="WndException"/>
		void _MinMaxRes(int state, bool noAnimation)
		{
			Debug.Assert(state == Api.SW_MINIMIZE || state == Api.SW_RESTORE || state == Api.SW_SHOWNORMAL || state == Api.SW_SHOWMAXIMIZED);
			ThrowIfInvalid();

			bool ok = false, wasMinimized = IsMinimized;

			switch(state) {
			case Api.SW_MINIMIZE:
				ok = wasMinimized;
				break;
			case Api.SW_RESTORE:
				ok = !wasMinimized;
				break;
			case Api.SW_SHOWNORMAL:
				ok = !wasMinimized && !IsMaximized; //info: if invalid handle, Show() will return false, don't need to check here.
				break;
			case Api.SW_SHOWMAXIMIZED:
				ok = IsMaximized;
				break;
			}

			if(ok) {
				if(IsVisible) return;
				Show(true);
			} else {
				if(!noAnimation) {
					Native.ClearError();
					Api.ShowWindow(this, state);
					ok = 0 == Native.GetError();
				} else if(ok = LibGetWindowPlacement(out var p)) {
					int state2 = state;
					switch(state) {
					case Api.SW_MINIMIZE:
						if(p.showCmd == Api.SW_SHOWMAXIMIZED) p.flags |= Api.WPF_RESTORETOMAXIMIZED; else p.flags &= ~Api.WPF_RESTORETOMAXIMIZED; //Windows forgets to remove the flag
						break;
					case Api.SW_RESTORE:
						if((p.showCmd == Api.SW_SHOWMINIMIZED) && (p.flags & Api.WPF_RESTORETOMAXIMIZED) != 0) state2 = Api.SW_SHOWMAXIMIZED; //without this would make normal
						break;
					}

					//if(wasMinimized) p.flags|=Api.WPF_ASYNCWINDOWPLACEMENT; //fixes Windows bug: if window of another thread, deactivates currently active window and does not activate this window. However then animates window. If we set this while the window is not minimized, it would set blinking caret in inactive window. Instead we use another workaround, see below.
					p.showCmd = state2;
					ok = LibSetWindowPlacement(ref p);
				}

				if(!ok) {
					if(Native.GetError() == Api.ERROR_ACCESS_DENIED) {
						//UAC blocks the API but not WM_SYSCOMMAND.
						//However does not allow to maximize with WM_SYSCOMMAND.
						uint cmd;
						switch(state) {
						case Api.SW_MINIMIZE: cmd = Api.SC_MINIMIZE; break;
						case Api.SW_SHOWMAXIMIZED: cmd = Api.SC_MAXIMIZE; break;
						default: cmd = Api.SC_RESTORE; break;
						}
						ok = Send(Api.WM_SYSCOMMAND, cmd);
						//if was minimized, now can be maximized, need to restore if SW_SHOWNORMAL
						if(ok && state == Api.SW_SHOWNORMAL && IsMaximized) ok = Send(Api.WM_SYSCOMMAND, cmd);
					}

					if(!ok) ThrowUseNative("*minimize/maximize/restore*");
				}

				if(!IsOfThisThread) {
					if(wasMinimized) ActivateLL(); //fix Windows bug: if window of another thread, deactivates currently active window and does not activate this window
					else if(state == Api.SW_MINIMIZE) Misc.WaitForAnActiveWindow();
				}
			}

			_MinimalWaitIfOtherThread();
		}

		/// <summary>
		/// Initializes a WINDOWPLACEMENT struct and calls API <msdn>GetWindowPlacement</msdn>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <exception cref="WndException">Failed. Throws, only if errStr!=null, else returns false.</exception>
		internal bool LibGetWindowPlacement(out Api.WINDOWPLACEMENT wp, string errStr = null)
		{
			//initially this was public, but probably don't need.

			wp = new Api.WINDOWPLACEMENT(); wp.length = Api.SizeOf(wp);
			if(Api.GetWindowPlacement(this, ref wp)) return true;
			if(errStr != null) ThrowUseNative(errStr);
			return false;
		}

		/// <summary>
		/// Sets WINDOWPLACEMENT length field and calls API <msdn>SetWindowPlacement</msdn>.
		/// </summary>
		/// <exception cref="WndException">Failed. Throws, only if errStr!=null, else returns false.</exception>
		internal bool LibSetWindowPlacement(ref Api.WINDOWPLACEMENT wp, string errStr = null)
		{
			//initially this was public, but probably don't need.

			wp.length = Api.SizeOf(wp);
			if(Api.SetWindowPlacement(this, ref wp)) return true;
			if(errStr != null) ThrowUseNative(errStr);
			return false;
		}

		#endregion

		#region activate, focus

		internal static partial class Lib
		{
			/// <summary>
			/// No exceptions.
			/// </summary>
			internal static bool EnableActivate(bool goingToActivateAWindow)
			{
				if(_EnableActivate_AllowSetFore()) return true; //not locked, or already successfully called ASF_Key

				_EnableActivate_SendKey(true);
				if(_EnableActivate_AllowSetFore()) return true;
				//First time fails if the foreground window is of higher IL. Then sending keys does not work.

				//_EnableActivate_MinRes() makes no active window...
				if(goingToActivateAWindow) {
					_EnableActivate_MinRes();
					return _EnableActivate_AllowSetFore();
				}

				var wFore = WndActive; bool retry = false;
				g1: _EnableActivate_MinRes();
				if(!_EnableActivate_AllowSetFore()) return false;
				if(!wFore.Is0 && !retry) {
					Api.SetForegroundWindow(wFore);
					if(!_EnableActivate_AllowSetFore()) { retry = true; goto g1; } //didn't notice this but let's be safer
				}
				return true;

				//Other possible methods:
				//1. Instead of key can use attachthreadinput. But it is less reliable, eg works first time only, and does not allow our process to activate later easily. Does not work if foreground window is higher IL.
				//2. Call allowsetforegroundwindow from a hook from the foreground process (or from the shell process, not tested). Too dirty. Need 2 native dlls (32/64-bit). Cannot inject if higher IL.

				//tested: cannot disable the foreground lock timeout with SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT) when this process cannot activate windows.
			}

			/// <summary>
			/// Sends a key (VK_0 up). It allows to activate now.
			/// Later this process can always activate easily (without key etc). It works even with higher IL windows.
			/// Don't know why is this behavior. Tested on all OS from XP to 10.
			/// Does not work if the foreground process has higher UAC IL.
			/// </summary>
			static void _EnableActivate_SendKey(bool debugOut)
			{
				if(debugOut) Debug_.Print("EnableActivate: need key");

				var x = new Api.INPUTKEY(0, 128, Api.IKFlag.Up);
				Api.SendInput_Key(ref x);
				//info: works without waiting.
			}

			/// <summary>
			/// Creates a temporary minimized window and restores it. It activates the window and allows us to activate.
			/// Then sets 'no active window' to prevent auto-activating another window when destroying the temporary window.
			/// </summary>
			static void _EnableActivate_MinRes()
			{
				Debug_.Print("EnableActivate: need min/res");

				Wnd t = Misc.CreateWindow(Native.WS_EX_TOOLWINDOW, "#32770", null, Native.WS_POPUP | Native.WS_MINIMIZE | Native.WS_VISIBLE);
				//info: When restoring, the window must be visible, or may not work.
				try {
					var wp = new Api.WINDOWPLACEMENT { showCmd = Api.SW_RESTORE };
					t.LibSetWindowPlacement(ref wp); //activates t; fast (no animation)
					_EnableActivate_SendKey(false); //makes so that later our process can always activate
					_EnableActivate_AllowSetFore();
					Api.SetForegroundWindow(Misc.WndRoot); //set no foreground window, or may activate the higher IL window (maybe does not activate, but winevents hook gets events, in random order). Other way would be to destroy our window later, but more difficult to implement.
				}
				finally { Api.DestroyWindow(t); }

				//Another way:
				//	t.Send(Api.WM_SETHOTKEY, ...);
				//	Api.SendInputKey(...)
				//Works but need more testing.
				//This does not work: t.Send(WM_SYSCOMMAND, SC_HOTKEY);
			}

			/// <summary>
			/// Calls Api.AllowSetForegroundWindow(Api.GetCurrentProcessId()).
			/// </summary>
			static bool _EnableActivate_AllowSetFore()
			{
				return Api.AllowSetForegroundWindow(Api.GetCurrentProcessId());
			}

			internal static bool ActivateLL(Wnd w)
			{
				if(w.LibIsActiveOrNoActiveAndThisIsWndRoot) return true;

				try {
					bool canAct = EnableActivate(true);

					if(!Api.SetForegroundWindow(w)) {
						if(!canAct || !w.IsAlive) return false;
						//It happens when foreground process called LockSetForegroundWindow.
						//Although AllowSetForegroundWindow returns true, SetForegroundWindow fails.
						//It happens only before this process sends keys. Eg after first _EnableActivate_SendKey this never happens again.
						//If it has higher IL (and this process is User), also need _EnableActivate_MinRes.
						_EnableActivate_SendKey(true);
						if(!Api.SetForegroundWindow(w)) {
							_EnableActivate_MinRes();
							if(!Api.SetForegroundWindow(w)) return false;
						}
					}

					//Sometimes after SetForegroundWindow there is no active window for several ms. Not if the window is of this thread.
					if(w == Misc.WndRoot) return WndActive.Is0;
					return Misc.WaitForAnActiveWindow();
				}
				//catch(WndException) { return false; }
				catch { return false; }
			}

			[Flags]
			internal enum ActivateFlags
			{
				/// <summary>
				/// Don't call ThrowIfInvalid at the very start (ie called ensures it is valid).
				/// </summary>
				NoThrowIfInvalid = 1,

				/// <summary>
				/// Don't call WndWindow (ie caller ensures it's a top-level window, not control).
				/// </summary>
				NoGetWndWindow = 2,

				/// <summary>
				/// Don't activate if has WS_EX_NOACTIVATE style or is toolwindow without caption, unless cloaked.
				/// Then just calls ZorderTop(), which in most cases does not work (inactive window).
				/// </summary>
				IgnoreIfNoActivateStyleEtc = 4,

				/// <summary>
				/// Wait for window animations to end. Eg when switching Win10 desktops.
				/// </summary>
				ForScreenCapture = 8,
			}
		}

		/// <summary>
		/// Activates this window (brings to the foreground).
		/// The same as <see cref="Activate()"/>, but has some options.
		/// Returns false if does not activate because of flag IgnoreIfNoActivateStyleEtc.
		/// </summary>
		/// <exception cref="WndException"/>
		internal bool LibActivate(Lib.ActivateFlags flags)
		{
			//CONSIDER: use Options.Relaxed

			if(0 != (flags & Lib.ActivateFlags.NoThrowIfInvalid)) ThrowIfInvalid();
			if(0 != (flags & Lib.ActivateFlags.NoGetWndWindow)) Debug.Assert(!IsChildWindow);
			else {
				var w = WndWindow;
				if(w != this) {
					return w.LibActivate((flags | Lib.ActivateFlags.NoGetWndWindow) & ~Lib.ActivateFlags.NoThrowIfInvalid);
				}
			}

			bool R = false, noAct = false, isMinimized = false, ofThisThread = IsOfThisThread;
			bool forScreenCapture = 0 != (flags & Lib.ActivateFlags.ForScreenCapture);

			if(IsMinimized) {
				ShowNotMinimized(true);
				isMinimized = IsMinimized;
				if(forScreenCapture && !isMinimized && !ofThisThread) Thread.Sleep(250); //although we use noAnimation, in some cases still restores with animation
			}
			if(!IsVisible) Show(true);

			R = LibIsActiveOrNoActiveAndThisIsWndRoot;
			if(!R) {
				if(0 != (flags & Lib.ActivateFlags.IgnoreIfNoActivateStyleEtc)) {
					uint est = ExStyle;
					if((est & Native.WS_EX_NOACTIVATE) != 0) noAct = true;
					else if((est & (Native.WS_EX_TOOLWINDOW | Native.WS_EX_APPWINDOW)) == Native.WS_EX_TOOLWINDOW) noAct = !HasStyle(Native.WS_CAPTION);
					if(noAct && !IsCloaked) {
						ZorderTop(); //in most cases does not work, but try anyway, it just calls the API. It seems works if the window is topmost.
						return false; //if cloaked, need to activate to uncloak
					}
				}

				for(int i = 0; i < 3; i++) {
					bool ok = ActivateLL();

					if(!ofThisThread) {
						_MinimalWaitNoCheckThread();
						_MinimalWaitNoCheckThread();
					}

					if(ok) {
						Wnd f = WndActive;
						if(f == this) R = true;
						else if(this == Misc.WndRoot) R = f.Is0; //activating GetDesktopWindow makes "no active window"
						else { //forgive if the target app instead activated another window of same thread
							int tid = ThreadId; if(tid == 0) break;
							if(f.ThreadId == tid) {
								//at first try to recognize such known windows, to avoid the hard way
								if(isMinimized || (f.WndOwner == this && Rect.IsEmpty)) {
									R = true;
								} else {
									R = Api.SetForegroundWindow(Misc.WndRoot) && ActivateLL() && WndActive.ThreadId == tid;
									if(R && !ofThisThread) {
										_MinimalWaitNoCheckThread();
										R = WndActive.ThreadId == tid;
									}
								}
								//Example 1:
								//Excel creates a minimized window for each workbook opened in that excel process.
								//These windows just add taskbar buttons. Also it allows to find and activate workbooks.
								//When you activate such window, Excel instead activates its main window, where it displays all workbooks.
								//For this reason we would fail (not always, because this may be temporarily active).
								//Same with PowerPoint. Other Office apps no.
								//Example 2:
								//Inno Setup, SQLite Expert. They have a zero-size owner window that just adds taskbar button.
							}
						}
						if(R) break;
					}

					if(noAct) break;
					Thread.Sleep(30);
				}
			}

			if(R && !ofThisThread && this != Misc.WndRoot) {
				//If we activate a window that is on an inactive Win10 desktop, its desktop becomes active.
				//Windows on inactive desktops are cloaked. They are uncloaked after ~15 ms.
				if(IsCloaked) {
					R = false;
					for(int i = 0; i < 50; i++) { Thread.Sleep(30); if(R = !IsCloaked) break; }
					if(R) {
						if(forScreenCapture) Thread.Sleep(800); //need minimum 600 for 'find image' functions, because of animation while switching Win10 desktops.
						_MinimalWaitNoCheckThread();
						R = IsActive;
						if(!R && ActivateLL()) {
							_MinimalWaitNoCheckThread();
							R = IsActive;
						}
					}
				}
			}

			if(!R) ThrowNoNative("*activate*");
			if(forScreenCapture) _MinimalWaitIfOtherThread();

			return true;

			//tested: if the window is hung, activates the ghost window and fails (exception). It's OK.
		}

		/// <summary>
		/// Activates this window (brings to the foreground).
		/// Also unhides, restores minimized etc, to ensure that the window is ready to receive sent keys, mouse clicks ect.
		/// </summary>
		/// <remarks>
		/// Activating a window usually also uncloaks it, for example switches to its virtual desktop on Windows 10.
		/// Fails (throws exception) if cannot activate this window, except:
		/// <list type="number">
		/// <item>If this is a control, activates its top-level parent window.</item>
		/// <item>If this is <see cref="Misc.WndRoot"/>, just deactivates the currently active window.</item>
		/// <item>When the target application instead activates another window of the same thread.</item>
		/// </list>
		/// </remarks>
		/// <exception cref="WndException"/>
		/// <seealso cref="ActivateLL"/>
		/// <seealso cref="IsActive"/>
		/// <seealso cref="WndActive"/>
		/// <seealso cref="Misc.SwitchActiveWindow"/>
		public void Activate()
		{
			LibActivate(0);
		}

		/// <summary>
		/// Low-level version of <see cref="Activate()"/>.
		/// Just calls <see cref="Misc.EnableActivate"/>, API <msdn>SetForegroundWindow</msdn> and makes sure that it actually worked, but does not check whether it activated exactly this window.
		/// No exceptions, does not unhide, does not restore minimized, does not check is it a top-level window or control, etc.
		/// Returns false if fails.
		/// </summary>
		public bool ActivateLL()
		{
			return Lib.ActivateLL(this);
		}

		public static partial class Misc
		{
			/// <summary>
			/// Waits while there is no active window.
			/// It sometimes happens after closing, minimizing or switching the active window, briefly until another window becomes active.
			/// Waits max 500 ms, then returns false if there is no active window.
			/// Processes Windows messages that are in the message queue of this thread.
			/// Don't need to call this after calling functions of this library.
			/// </summary>
			public static bool WaitForAnActiveWindow()
			{
				for(int i = 0; i < 32; i++) {
					Time.DoEvents();
					if(!WndActive.Is0) return true;
					Thread.Sleep(15);
				}
				return false;
				//Call this after showing a dialog API.
				//	In a thread that does not process messages, after closing a dialog may be not updated key states.
				//	Processing remaining unprocessed messages fixes it.
			}

			/// <summary>
			/// Temporarily enables this process to activate windows with API <msdn>SetForegroundWindow</msdn>.
			/// Returns false if fails (unlikely).
			/// In some cases you may need this function because Windows often disables SetForegroundWindow to not allow applications to activate their windows while the user is working (using keyboard/mouse) with the currently active window. Then SetForegroundWindow just makes the window's taskbar button flash which indicates that the windows wants attention. More info: <msdn>SetForegroundWindow</msdn>.
			/// Usually you don't call SetForegroundWindow directly. It is called by some other functions, for example Form.Show.
			/// Don't need to call this function before calling Wnd.Activate and other functions of this library that activate windows.
			/// </summary>
			public static bool EnableActivate()
			{
				return Lib.EnableActivate(false);
			}
		}

		//Too unreliable.
		///// <summary>
		///// Calls API LockSetForegroundWindow, which temporarily prevents other applications from activating windows easily with SetForegroundWindow().
		///// If LockSetForegroundWindow() fails, calls EnableActivate() and retries.
		///// </summary>
		///// <param name="on">Lock or unlock.</param>
		//public static bool LockActiveWindow(bool on)
		//{
		//	uint f = on ? Api.LSFW_LOCK : Api.LSFW_UNLOCK;
		//	if(Api.LockSetForegroundWindow(f)) return true;
		//	return EnableActivate() && Api.LockSetForegroundWindow(f);
		//}

		/// <summary>
		/// Sets the keyboard input focus to this control or window.
		/// Also activetes its top-level parent window (see <see cref="Activate()"/>).
		/// Can belong to any process/thread. With controls of this thread you can use the more lightweight function <see cref="FocusLL"/>.
		/// </summary>
		/// <remarks>
		/// Can instead focus a child control. For example, if this is a ComboBox, it will focus its child Edit control. Then does not throw exception.
		/// </remarks>
		/// <exception cref="WndException">
		/// Invalid handle; disabled; failed to set focus; failed to activate parent window.
		/// Fails to set focus when the target process is admin and this process is normal.
		/// </exception>
		/// <seealso cref="WndFocused"/>
		/// <seealso cref="IsFocused"/>
		public void Focus()
		{
			ThrowIfInvalid();
			Wnd wTL = WndWindow;
			if(!wTL.IsActive) wTL.LibActivate(Lib.ActivateFlags.NoGetWndWindow);

			int th1 = Api.GetCurrentThreadId(), th2 = ThreadId;
			if(th1 == th2) {
				Api.SetFocus(this);
				if(this != WndFocusedLL) ThrowUseNative("*set focus");
				return;
			}

			if(IsFocused) return;
			if(!IsEnabled) { //SetFocus would fail
				ThrowIfInvalid();
				ThrowNoNative("*set focus. Disabled");
			}

			bool ok = false;
			if(Api.AttachThreadInput(th1, th2, true))
				try {
					for(int i = 0; i < 5; i++) {
						Native.ClearError();
						Api.SetFocus(this);
						Wnd f = WndFocused;
						if(f == this || f.IsChildOf(this)) { ok = true; break; }
						//Print(i);
						Thread.Sleep(30);
					}
				}
				finally { Api.AttachThreadInput(th1, th2, false); }

			if(!ok) ThrowUseNative("*set focus");

			_MinimalWaitNoCheckThread();

			//note: don't use accSelect, it has bugs
		}

		/// <summary>
		/// Sets the keyboard input focus to this control or window.
		/// Does nothing if it belongs to another thread or its top-level parent window isn't the active window.
		/// Calls API <msdn>SetFocus</msdn>.
		/// </summary>
		/// <seealso cref="WndFocusedLL"/>
		public void FocusLL()
		{
			Api.SetFocus(this);
		}

		/// <summary>
		/// Gets the control or window that has the keyboard input focus.
		/// It can belong to any process/thread. With controls of this thread you can use the more lightweight function <see cref="WndFocusedLL"/>.
		/// Calls API <msdn>GetGUIThreadInfo</msdn>.
		/// </summary>
		/// <seealso cref="Focus"/>
		/// <seealso cref="IsFocused"/>
		public static Wnd WndFocused
		{
			get
			{
				Misc.GetGUIThreadInfo(out var g);
				return g.hwndFocus;
			}
		}

		/// <summary>
		/// Gets the control or window of this thread that has the keyboard input focus.
		/// Calls API <msdn>GetFocus</msdn>.
		/// </summary>
		/// <seealso cref="FocusLL"/>
		public static Wnd WndFocusedLL { get => Api.GetFocus(); }

		/// <summary>
		/// Returns true if this is the control or window that has the keyboard input focus.
		/// Can belong to any process/thread. With controls of this thread you can use the more lightweight function <see cref="WndFocusedLL"/>.
		/// Calls <see cref="WndFocused"/>.
		/// </summary>
		/// <seealso cref="Focus"/>
		public bool IsFocused { get => !this.Is0 && this == WndFocused; }

		#endregion

		#region rect

		/// <summary>
		/// Gets rectangle (position and size) in screen coordinates.
		/// The same as the Rect property.
		/// </summary>
		/// <param name="r">Receives the rectangle. Will be empty if failed.</param>
		/// <remarks>
		/// Calls API <msdn>GetWindowRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		bool _GetRect(out RECT r)
		{
			if(Api.GetWindowRect(this, out r)) return true;
			r.Set0();
			return false;
		}

		/// <summary>
		/// Gets width and height.
		/// The same as the Size property.
		/// </summary>
		/// <param name="z">Receives width and height. Will be empty if failed.</param>
		/// <remarks>
		/// Calls API <msdn>GetWindowRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		bool _GetSize(out Size z)
		{
			if(Api.GetWindowRect(this, out RECT r)) { z = new Size(r.Width, r.Height); return true; }
			z = new Size();
			return false;
		}

		/// <summary>
		/// Gets rectangle (position and size) in screen coordinates.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>GetWindowRect</msdn>. Returns empty RECT if fails (eg window closed).
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public RECT Rect
		{
			get
			{
				_GetRect(out RECT r);
				return r;
			}
		}
		//TODO: Need a function to get the visible part of window rect, without the transparent border on Win10. There is API, but don't remember.

		/// <summary>
		/// Gets width and height.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>GetWindowRect</msdn>. Returns empty Size if fails (eg window closed).
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public Size Size
		{
			get
			{
				_GetSize(out Size z);
				return z;
			}
		}

		/// <summary>
		/// Gets horizontal position in screen coordinates.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public int X
		{
			get => Rect.left;
		}
		/// <summary>
		/// Gets vertical position in screen coordinates.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public int Y
		{
			get => Rect.top;
		}
		/// <summary>
		/// Gets width.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public int Width
		{
			get => Rect.Width;
		}
		/// <summary>
		/// Gets height.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public int Height
		{
			get => Rect.Height;
		}

		/// <summary>
		/// Gets client area rectangle.
		/// The same as the ClientRect property.
		/// The same as _GetClientSize, just the parameter type is different.
		/// </summary>
		/// <param name="r">Receives the rectangle. Will be empty if failed.</param>
		/// <remarks>
		/// Calls API <msdn>GetClientRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		bool _GetClientRect(out RECT r)
		{
			if(Api.GetClientRect(this, out r)) return true;
			r.Set0();
			return false;
		}

		/// <summary>
		/// Gets client area width and height.
		/// The same as the ClientSize property.
		/// The same as _GetClientRect, just the parameter type is different.
		/// </summary>
		/// <param name="z">Receives width and height. Will be empty if failed.</param>
		/// <remarks>
		/// Calls API <msdn>GetClientRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		bool _GetClientSize(out Size z)
		{
			if(Api.GetClientRect(this, out RECT r)) { z = new Size(r.right, r.bottom); return true; }
			z = new Size();
			return false;
		}

		/// <summary>
		/// Gets client area rectangle (width and height).
		/// The same as <see cref="ClientSize"/>, just the return type is different.
		/// </summary>
		/// <remarks>
		/// The left and top fields are always 0. The right and bottom fields are the width and height of the client area.
		/// Calls <msdn>GetClientRect</msdn>. Returns empty rectangle if fails (eg window closed).
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public RECT ClientRect
		{
			get
			{
				//TODO: Native.ClearError. In other places too. Also let _GetRect etc be public.
				_GetClientRect(out RECT r);
				return r;
			}
		}
		/// <summary>
		/// Gets client area width and height.
		/// The same as <see cref="ClientRect"/>, just the return type is different.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>GetClientRect</msdn>. Returns empty Size value if fails (eg window closed).
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public Size ClientSize
		{
			get
			{
				_GetClientSize(out Size z);
				return z;
			}
		}
		/// <summary>
		/// Gets client area width.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public int ClientWidth
		{
			get => ClientSize.Width;
		}
		/// <summary>
		/// Gets client area height.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public int ClientHeight
		{
			get => ClientSize.Height;
		}

		/// <summary>
		/// Calculates and sets window rectangle from the specified client area rectangle.
		/// Calls <see cref="ResizeLL">ResizeLL</see>.
		/// </summary>
		/// <param name="width">Width. Use null to not change.</param>
		/// <param name="height">Height. Use null to not change.</param>
		/// <exception cref="WndException"/>
		public void SetClientSize(int? width, int? height)
		{
			if(LibGetWindowInfo(out var u)) {
				int W = width != null ? width.GetValueOrDefault() : u.rcClient.Width; W += u.rcWindow.Width - u.rcClient.Width;
				int H = height != null ? height.GetValueOrDefault() : u.rcClient.Height; H += u.rcWindow.Height - u.rcClient.Height;

				if(ResizeLL(W, H)) return;
			}

			ThrowUseNative();
		}

		/// <summary>
		/// Calls API <msdn>GetWindowInfo</msdn>.
		/// </summary>
		/// <param name="wi">Receives window/client rectangles, styles etc.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		internal bool LibGetWindowInfo(out Api.WINDOWINFO wi)
		{
			//initially this was public, but probably don't need.

			wi = new Api.WINDOWINFO(); wi.cbSize = Api.SizeOf(wi);
			return Api.GetWindowInfo(this, ref wi);
		}

		/// <summary>
		/// Gets window rectangle and client area rectangle, both in screen coordinates.
		/// </summary>
		/// <param name="rWindow">Receives window rectangle.</param>
		/// <param name="rClient">Receives client area rectangle.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool GetWindowAndClientRectInScreen(out RECT rWindow, out RECT rClient)
		{
			if(LibGetWindowInfo(out var u)) { rWindow = u.rcWindow; rClient = u.rcClient; return true; }
			rWindow = new RECT(); rClient = new RECT();
			return false;
		}

		/// <summary>
		/// Gets client area rectangle in screen coordinates.
		/// Returns empty rectangle if fails (eg window closed).
		/// Use <see cref="GetWindowAndClientRectInScreen">GetWindowAndClientRectInScreen</see> instead when you need a bool return value.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public RECT ClientRectInScreen
		{
			get
			{
				GetWindowAndClientRectInScreen(out var rw, out var rc);
				return rc;
			}
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the client area of window w.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapClientToClientOf(Wnd w, ref RECT r)
		{
			fixed (void* t = &r) { return _MapWindowPoints(this, w, t, 2); }
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the client area of window w.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapClientToClientOf(Wnd w, ref Point p)
		{
			fixed (void* t = &p) { return _MapWindowPoints(this, w, t, 1); }
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the screen.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapClientToScreen(ref RECT r)
		{
			return MapClientToClientOf(default, ref r);
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the screen.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapClientToScreen(ref Point p)
		{
			return Api.ClientToScreen(this, ref p);
		}

		/// <summary>
		/// Converts coordinates relative to the screen to coordinates relative to the client area of this window.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapScreenToClient(ref RECT r)
		{
			fixed (void* t = &r) { return _MapWindowPoints(default, this, t, 2); }
		}

		/// <summary>
		/// Converts coordinates relative to the screen to coordinates relative to the client area of this window.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapScreenToClient(ref Point p)
		{
			return Api.ScreenToClient(this, ref p);
		}

		static bool _MapWindowPoints(Wnd wFrom, Wnd wTo, void* t, int cPoints)
		{
			Native.ClearError();
			if(Api.MapWindowPoints(wFrom, wTo, t, cPoints) != 0) return true;
			return Native.GetError() == 0;
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the top-left corner of this window.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapClientToWindow(ref Point p)
		{
			if(!GetWindowAndClientRectInScreen(out var rw, out var rc)) return false;
			p.X += rc.left - rw.left; p.Y += rc.top - rw.top;
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the top-left corner of this window.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapClientToWindow(ref RECT r)
		{
			if(!GetWindowAndClientRectInScreen(out var rw, out var rc)) return false;
			r.Offset(rc.left - rw.left, rc.top - rw.top);
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to coordinates relative to the client area of this window.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapWindowToClient(ref Point p)
		{
			if(!GetWindowAndClientRectInScreen(out var rw, out var rc)) return false;
			p.X += rw.left - rc.left; p.Y += rw.top - rc.top;
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to coordinates relative to the client area of this window.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapWindowToClient(ref RECT r)
		{
			if(!GetWindowAndClientRectInScreen(out var rw, out var rc)) return false;
			r.Offset(rw.left - rc.left, rw.top - rc.top);
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to screen coordinates.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapWindowToScreen(ref Point p)
		{
			if(!_GetRect(out var rw)) return false;
			p.X += rw.left; p.Y += rw.top;
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to screen coordinates.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapWindowToScreen(ref RECT r)
		{
			if(!_GetRect(out var rw)) return false;
			r.Offset(rw.left, rw.top);
			return true;
		}

		/// <summary>
		/// Gets rectangle of this window (usually control) relative to the client area of another window (usually the parent).
		/// </summary>
		/// <param name="w">The returned rectangle will be relative to the client area of window w. If w is default(Wnd), gets rectangle in screen.</param>
		/// <param name="r">Receives the rectangle.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <seealso cref="RectInParent"/>
		public bool GetRectInClientOf(Wnd w, out RECT r)
		{
			if(w.Is0) return _GetRect(out r);
			return _GetRect(out r) && w.MapScreenToClient(ref r);
		}

		/// <summary>
		/// Gets or sets child window rectangle in parent window's client area.
		/// Calls <see cref="GetRectInClientOf">GetRectInClientOf</see>. Returns empty rectangle if fails (eg window closed).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public RECT RectInParent
		{
			get
			{
				if(!GetRectInClientOf(WndDirectParent, out var r)) r = default;
				return r;
			}
		}

		/// <summary>
		/// Gets rectangle of normal (restored) window even if currently it is minimized or maximized.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool GetRectNotMinMax(out RECT r)
		{
			if(!LibGetWindowPlacement(out var p)) { r = default; return false; }
			r = p.rcNormalPosition;
			return true;
		}

		/// <summary>
		/// Returns mouse pointer position relative to the client area of this window.
		/// </summary>
		public Point MouseClientXY
		{
			get
			{
				Api.GetCursorPos(out var p);
				if(!MapScreenToClient(ref p)) p = default;
				return p;
			}
		}

		/// <summary>
		/// Returns true if this window (its rectangle) contains the specified point in primary screen coordinates.
		/// </summary>
		/// <param name="x">X coordinate in screen. Not used if null.</param>
		/// <param name="y">Y coordinate in screen. Not used if null.</param>
		/// <param name="workArea">The coordinates are relative to the work area.</param>
		/// <param name="screen">Screen of x y. If null, primary screen. See <see cref="Screen_.FromObject"/>.</param>
		public bool ContainsScreenXY(Coord x, Coord y, bool workArea = false, object screen = null)
		{
			Point p = Coord.Normalize(x, y, workArea, screen);
			if(!_GetRect(out RECT r)) return false;
			if(!r.Contains(x.IsNull ? r.left : p.X, y.IsNull ? r.top : p.Y)) return false;
			return true;

			//note: we don't use name ContainsXY and 2 overloads, mostly because of possible incorrect usage. Also now easier to read the code.
		}

		/// <summary>
		/// Returns true if this control (its rectangle) contains the specified point in parent window.
		/// </summary>
		/// <param name="parent">
		/// Direct or indirect parent window. The coordinates are relative to its client area.
		/// Actually this and parent can be any windows or controls, the function does not check whether this is a child of parent.
		/// </param>
		/// <param name="x">X coordinate. Not used if null.</param>
		/// <param name="y">Y coordinate. Not used if null.</param>
		public bool ContainsWindowXY(Wnd parent, Coord x, Coord y)
		{
			if(!parent.IsAlive) return false;
			Point p = Coord.NormalizeInWindow(x, y, parent);
			if(!GetRectInClientOf(parent, out RECT r)) return false;
			if(!r.Contains(x.IsNull ? r.left : p.X, y.IsNull ? r.top : p.Y)) return false;
			return true;
		}

		/// <summary>
		/// This overload calls <see cref="ContainsWindowXY(Wnd, Coord, Coord)">ContainsWindowXY</see>(WndWindow, x, y).
		/// </summary>
		public bool ContainsWindowXY(Coord x, Coord y)
		{
			return ContainsWindowXY(WndWindow, x, y);
		}

		#endregion

		#region move, resize, SetWindowPos

		/// <summary>
		/// Calls API <msdn>SetWindowPos</msdn>.
		/// </summary>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>.
		/// For swpFlags you can use Native.SWP_ constants.
		/// </remarks>
		public bool SetWindowPos(uint swpFlags, int x = 0, int y = 0, int cx = 0, int cy = 0, Wnd wndInsertAfter = default)
		{
			return Api.SetWindowPos(this, wndInsertAfter, x, y, cx, cy, swpFlags);
		}

		/// <summary>
		/// Moves and resizes.
		/// </summary>
		/// <remarks>
		/// See also <see cref="Move(Coord, Coord, Coord, Coord, bool)"/>. It is better to use in automation scripts, with windows of any process/thread. It throws exceptions, supports optional/reverse/fractional/workarea coordinates, restores if min/max, does not support SWP_ flags.
		/// This function is low-level, it just calls API <msdn>SetWindowPos</msdn> with flags SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOACTIVATE|swpFlagsToAdd. It is better to use in programming, with windows of current thread.
		/// Supports <see cref="Native.GetError"/>.
		/// 
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// </remarks>
		/// <seealso cref="SetWindowPos"/>
		public bool MoveLL(int x, int y, int width, int height, uint swpFlagsToAdd = 0)
		{
			return SetWindowPos(Native.SWP_NOZORDER | Native.SWP_NOOWNERZORDER | Native.SWP_NOACTIVATE | swpFlagsToAdd, x, y, width, height);
		}

		/// <summary>
		/// Moves.
		/// </summary>
		/// <remarks>
		/// See also <see cref="Move(Coord, Coord, bool)"/>. It is better to use in automation scripts, with windows of any process/thread. It throws exceptions, supports optional/reverse/fractional/workarea coordinates, restores if min/max.
		/// This function is low-level, it just calls API <msdn>SetWindowPos</msdn> with flags SWP_NOSIZE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOACTIVATE. It is better to use in programming, with windows of current thread.
		/// Supports <see cref="Native.GetError"/>.
		/// 
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// </remarks>
		/// <seealso cref="SetWindowPos"/>
		public bool MoveLL(int x, int y)
		{
			return MoveLL(x, y, 0, 0, Native.SWP_NOSIZE);
		}

		/// <summary>
		/// Resizes.
		/// </summary>
		/// <remarks>
		/// See also <see cref="Resize(Coord, Coord, bool)"/>. It is better to use in automation scripts, with windows of any process/thread. It throws exceptions, supports optional/reverse/fractional/workarea coordinates, restores if min/max.
		/// This function is low-level, it just calls API <msdn>SetWindowPos</msdn> with flags SWP_NOMOVE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOACTIVATE. It is better to use in programming, with windows of current thread.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		/// <seealso cref="SetWindowPos"/>
		public bool ResizeLL(int width, int height)
		{
			return MoveLL(0, 0, width, height, Native.SWP_NOMOVE);
		}

		/// <summary>
		/// Moves and/or resizes.
		/// With windows of current thread usually it's better to use <see cref="MoveLL(int, int, int, int, uint)"/>.
		/// </summary>
		/// <param name="x">Left. If null, does not move in X axis.</param>
		/// <param name="y">Top. If null, does not move in Y axis.</param>
		/// <param name="width">Width. If null, does not change width.</param>
		/// <param name="height">Height. If null, does not change height.</param>
		/// <param name="workArea">If false, the coordinates are relative to the primary screen, else to its work area. Not used when this is a child window.</param>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized.
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// </remarks>
		/// <exception cref="WndException"/>
		public void Move(Coord x, Coord y, Coord width, Coord height, bool workArea = false)
		{
			ThrowIfInvalid();

			Wnd w = WndDirectParent;
			Point xy, wh;
			if(!w.Is0) {
				xy = Coord.NormalizeInWindow(x, y, w);
				wh = Coord.NormalizeInWindow(width, height, w);
			} else {
				xy = Coord.Normalize(x, y, workArea);
				wh = Coord.Normalize(width, height, workArea, widthHeight: true);
			}

			uint f = 0, getRect = 0;
			if(x.IsNull && y.IsNull) f |= Native.SWP_NOMOVE; else if(x.IsNull) getRect |= 1; else if(y.IsNull) getRect |= 2;
			if(width.IsNull && height.IsNull) f |= Native.SWP_NOSIZE; else if(width.IsNull) getRect |= 4; else if(height.IsNull) getRect |= 8;

			if(getRect != 0) {
				if(!GetRectInClientOf(w, out RECT r)) ThrowUseNative("*move/resize*");
				if((getRect & 1) != 0) xy.X = r.left;
				if((getRect & 2) != 0) xy.Y = r.top;
				if((getRect & 4) != 0) wh.X = r.Width;
				if((getRect & 8) != 0) wh.Y = r.Height;
			}

			//restore min/max, except if child or hidden
			if(w.Is0 && (IsMinimized || IsMaximized) && IsVisible) {
				ShowNotMinMax(true);
				//info: '&& IsVisible' because ShowNotMinMax unhides
			}

			if(!MoveLL(xy.X, xy.Y, wh.X, wh.Y, f)) ThrowUseNative("*move/resize*");

			_MinimalWaitIfOtherThread();
		}

		/// <summary>
		/// Moves.
		/// Calls Move(x, y, null, null, workArea).
		/// With windows of current thread usually it's better to use <see cref="MoveLL(int, int)"/>.
		/// </summary>
		/// <param name="x">Left. If null, does not move in X axis.</param>
		/// <param name="y">Top. If null, does not move in Y axis.</param>
		/// <param name="workArea">If false, the coordinates are relative to the primary screen, else to its work area. Not used when this is a child window.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized.
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// </remarks>
		public void Move(Coord x, Coord y, bool workArea = false)
		{
			Move(x, y, null, null, workArea);
		}

		/// <summary>
		/// Resizes.
		/// Calls Move(null, null, width, height, workArea).
		/// With windows of current thread usually it's better to use <see cref="ResizeLL(int, int)"/>.
		/// </summary>
		/// <param name="width">Width. If null, does not change width.</param>
		/// <param name="height">Height. If null, does not change height.</param>
		/// <param name="workArea">If false, reverse and fractional width/height are relative to the primary screen, else to its work area. Not used when this is a child window.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized.
		/// </remarks>
		public void Resize(Coord width, Coord height, bool workArea = false)
		{
			Move(null, null, width, height, workArea);
		}

		#endregion

		#region MoveInScreen, EnsureInScreen, Screen

		internal static partial class Lib
		{
			/// <summary>
			/// Used directly by MoveInScreen, EnsureInScreen, RECT.MoveInScreen, RECT.EnsureInScreen.
			/// </summary>
			internal static void MoveInScreen(bool bEnsureMethod,
			Coord left, Coord top, bool useWindow, Wnd w, ref RECT r,
			object screen, bool bWorkArea, bool bEnsureInScreen)
			{
				Screen scr;
				if(screen == null) {
					if(useWindow) scr = Screen_.FromWindow(w);
					else scr = bEnsureMethod ? Screen.FromRectangle(r) : Screen.PrimaryScreen;
				} else scr = Screen_.FromObject(screen);

				RECT rs = bWorkArea ? scr.WorkingArea : scr.Bounds;

				if(useWindow) {
					if(!w.GetRectNotMinMax(out r)) w.ThrowUseNative("*move*");
				}

				int x, y, wid = r.Width, hei = r.Height;
				if(bEnsureMethod) {
					Debug.Assert(bEnsureInScreen == true && left.IsNull && top.IsNull); //left/top unused
					x = r.left;
					y = r.top;
				} else {
					if(left.IsNull) left = Coord.Center;
					if(top.IsNull) top = Coord.Center;
					var p = Coord.NormalizeInRect(left, top, rs);
					x = p.X; y = p.Y;
					switch(left.Type) { case Coord.CoordType.Reverse: x -= wid; break; case Coord.CoordType.Fraction: x -= (int)(wid * left.FractionValue); break; }
					switch(top.Type) { case Coord.CoordType.Reverse: y -= hei; break; case Coord.CoordType.Fraction: y -= (int)(hei * top.FractionValue); break; }
				}

				if(bEnsureInScreen) {
					x = Math.Max(Math.Min(x, rs.right - wid), rs.left);
					y = Math.Max(Math.Min(y, rs.bottom - hei), rs.top);
					if(r.Width > rs.Width) r.Width = rs.Width;
					if(r.Height > rs.Height) r.Height = rs.Height;
				}

				r.Offset(x - r.left, y - r.top);

				if(useWindow) { //move window
					w.LibGetWindowPlacement(out var wp, "*move*");
					bool moveMaxWindowToOtherMonitor = wp.showCmd == Api.SW_SHOWMAXIMIZED && !scr.Equals(Screen_.FromWindow(w));
					if(r == wp.rcNormalPosition && !moveMaxWindowToOtherMonitor) return;

					Wnd hto = default; bool visible = w.IsVisible;
					try {
						//Windows bug: before a dialog is first time shown, may fail to move if it has an owner window. Depends on coordinates and on don't know what.
						//There are several workarounds. The best of them - temporarily set owner window 0.
						if(!visible) {
							hto = w.WndOwner;
							if(!hto.Is0) w.WndOwner = default;
						}

						wp.rcNormalPosition = r;
						wp.showCmd = visible ? Api.SW_SHOWNA : Api.SW_HIDE;
						w.LibSetWindowPlacement(ref wp, "*move*");

						if(moveMaxWindowToOtherMonitor) {
							//I found this way of moving max window to other screen by experimenting.
							//When moved to screen's coordinates and sized to screen's work area size, OS adjusts window pos to be correct, ie border is outside screen, but invisible in adjacent screen.
							//Must call SetWindowPos twice, or it may refuse to move at all.
							//Another way - use SetWindowPlacement to temporarily restore, move to other screen, then maximize. But it unhides hidden window.
							rs = scr.WorkingArea;
							if(!w.MoveLL(rs.left, rs.top) || !w.ResizeLL(rs.Width, rs.Height)) w.ThrowUseNative("*move*");
						}
					}
					finally {
						if(!hto.Is0) w.WndOwner = hto;
					}

					w._MinimalWaitIfOtherThread();
				}
			}
		}

		/// <summary>
		/// Moves this window to coordinates x y in specified screen, and ensures that entire window is in screen.
		/// </summary>
		/// <param name="x">X coordinate in the specified screen. If null - screen center. You also can use Coord.Reverse etc.</param>
		/// <param name="y">Y coordinate in the specified screen. If null - screen center. You also can use Coord.Reverse etc.</param>
		/// <param name="screen">Move to this screen (see <see cref="Screen_.FromObject"/>). If null (default), use screen of this window.</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <param name="ensureInScreen">If part of window is not in screen, move and/or resize it so that entire window would be in screen. Default true.</param>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <remarks>
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately, except when moving maximized to another screen.
		/// </remarks>
		/// <seealso cref="RECT.MoveInScreen"/>
		public void MoveInScreen(Coord x, Coord y, object screen = null, bool workArea = true, bool ensureInScreen = true)
		{
			var r = new RECT();
			Lib.MoveInScreen(false, x, y, true, this, ref r, screen, workArea, ensureInScreen);
		}

		/// <summary>
		/// Moves this window if need, to ensure that entire window is in screen.
		/// </summary>
		/// <param name="screen">
		/// Move to this screen (see <see cref="Screen_.FromObject"/>). If null (default), uses screen of this window.
		/// If screen index is invalid, shows warning, no exception. Then uses screen of this window.
		/// </param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately.
		/// </remarks>
		/// <seealso cref="RECT.EnsureInScreen"/>
		public void EnsureInScreen(object screen = null, bool workArea = true)
		{
			var r = new RECT();
			if(screen is int) screen = Screen_.FromIndex((int)screen, noThrow: true); //returns null if invalid
			Lib.MoveInScreen(true, null, null, true, this, ref r, screen, workArea, true);
		}

		/// <summary>
		/// Moves this window to the center of the screen.
		/// Calls ShowNotMinMax(true) and MoveInScreen(null, null, screen, true, true).
		/// </summary>
		/// <param name="screen">Move to this screen (see <see cref="Screen_.FromObject"/>). If null (default), uses screen of this window.</param>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <seealso cref="RECT.MoveInScreen"/>
		public void MoveToScreenCenter(object screen = null)
		{
			ShowNotMinMax(true);
			MoveInScreen(null, null, screen, true);
		}

		/// <summary>
		/// Gets <see cref="System.Windows.Forms.Screen"/> object of the screen that contains this window (the biggest part of it) or is nearest to it.
		/// If this window handle is default(Wnd) or invalid, gets the primary screen.
		/// Calls <see cref="Screen_.FromWindow"/>.
		/// </summary>
		public Screen Screen
		{
			get => Screen_.FromWindow(this);
		}

		#endregion

		#region Zorder
		const uint _SWP_ZORDER = Native.SWP_NOMOVE | Native.SWP_NOSIZE | Native.SWP_NOACTIVATE;

		/// <summary>
		/// Places this window before window w in the Z order.
		/// Also can make this window topmost or non-topmost, depending on where w is in the Z order.
		/// This window and w can be both top-level windows or both controls of same parent.
		/// May not work with top-level windows when it would move an inactive window above the active window.
		/// If w is default(Wnd), calls <see cref="ZorderBottom"/>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool ZorderAbove(Wnd w)
		{
			return _ZorderAfterBefore(w, true);
		}

		/// <summary>
		/// Places this window after window w in the Z order.
		/// Also can make this window topmost or non-topmost, depending on where w is in the Z order.
		/// This window and w can be both top-level windows or both controls of same parent.
		/// May not work with top-level windows when it would move an inactive window above the active window.
		/// If w is default(Wnd), calls <see cref="ZorderTop"/>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool ZorderBelow(Wnd w)
		{
			return _ZorderAfterBefore(w, false);
		}

		bool _ZorderAfterBefore(Wnd w, bool before)
		{
			if(w.Is0) return before ? ZorderBottom() : ZorderTop();
			if(w == this) return true;
			if(IsTopmost && !w.IsTopmost) ZorderNoTopmost(); //see comments below
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, before ? w.WndPrev : w);
		}

		/// <summary>
		/// Places this window or control at the top of the Z order.
		/// If the window was topmost, it will be at the top of topmost windows, else at the top of non-topmost windows (after topmost windows).
		/// Does not activate.
		/// In most cases does not work with top-level inactive windows, although returns true; instead use <see cref="ActivateLL"/>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool ZorderTop()
		{
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, SpecHwnd.HWND_TOP);

			//CONSIDER: find a workaround. Eg in some cases this works: set HWND_TOPMOST then HWND_NOTOPMOST.
		}
		//This version tries a workaround, but on latest Windows it does not work.
		//Better don't use workarounds here.
		//public bool ZorderTop()
		//{
		//	Wnd wa = SpecHwnd.Top;
		//	if(!IsChildWindow) {
		//		//SWP does not work if this window is inactive, unless wndInsertAfter is used.
		//		//Workaround: insert this after the first window, then insert the first window after this.
		//		wa = Api.GetTopWindow(IsTopmost ? default : this);
		//		return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, wa) && wa.SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, this);
		//	}
		//	return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, wa);
		//}

		/// <summary>
		/// Places this window or control at the bottom of the Z order.
		/// If the window was topmost, makes it and its owner window non-topmost.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool ZorderBottom()
		{
			ZorderNoTopmost(); //see comments below
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, SpecHwnd.HWND_BOTTOM);
		}

		/// <summary>
		/// Makes this window topmost (always on top of non-topmost windows in the Z order).
		/// Does not activate.
		/// If this window has an owner window, the owner does not become topmost.
		/// This cannot be a control.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool ZorderTopmost()
		{
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, SpecHwnd.HWND_TOPMOST);
		}

		/// <summary>
		/// Makes this window non-topmost.
		/// If this window has an owner window, makes the owner window non-topmost too.
		/// This cannot be a control.
		/// </summary>
		/// <param name="afterActiveWindow">Also place this window after the active nontopmost window in the Z order, unless the active window is its owner.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool ZorderNoTopmost(bool afterActiveWindow = false)
		{
			if(!IsTopmost) return true;

			for(int i = 0; i < 4; i++) {
				if(!SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, SpecHwnd.HWND_NOTOPMOST)) return false;
				if(i == 0 && !IsTopmost) break;
				//Print("retry");
			}

			//place this after the active window
			if(afterActiveWindow) {
				Wnd wa = WndActive;
				if(wa != this && !wa.IsTopmost) SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, wa);
			}

			return true;
		}
		// Windows 8/10 bug: cannot make a topmost uiAccess window non-topmost (with HWND_NOTOPMOST, HWND_BOTTOM or non-topmost hwndInsertAfter).
		//   Workaround: call SWP 2 times. With some Windows updates also need SWP_NOOWNERZORDER.
		//   Problems with SWP_NOOWNERZORDER:
		//      1. If used with non-uiAccess windows, then later HWND_TOPMOST does not work. Solution: call SWP first time without this flag.
		//      2. Does not make owned windows non-topmost. Solution: finally call SWP without this flag.
		//      3. Does not make owner window non-topmost. Never mind, it is rare, and a solution is dirty.
		//   The bug and workarounds are undocumented.
		//   Now I cannot reproduce on Win10, maybe it is fixed in current Win10 version. Tested and can reproduce on Win8.1.
		// More problems with topmost uiAccess windows:
		//   Sometimes inserting a uiAccess hwnd after a window does not work, sometimes works...
		//   Problems with HWND_BOTTOM and owned windows.
		//   And so on.
		// On Windows XP/7/8 HWND_BOTTOM moves a topmost window to the bottom of ALL windows, as documented.
		//   But on Windows 10 - to the top of non-topmost windows; 2-nd SWP moves to the right place, but 3-th SWP moves uiAccess windows back :), 4-th makes correct (owned windows too).
		//      It seems it is fixed in current Win10 version.

		/// <summary>
		/// Returns true if this is a topmost (always-on-top) window.
		/// </summary>
		public bool IsTopmost { get => HasExStyle(Native.WS_EX_TOPMOST); }

		/// <summary>
		/// Returns true if this window is above window w in the Z order.
		/// </summary>
		public bool ZorderIsBefore(Wnd w)
		{
			if(w.Is0) return false;
			for(Wnd t = this; !t.Is0;) {
				t = Api.GetWindow(t, Api.GW_HWNDNEXT);
				if(t == w) return true;
			}
			return false;
		}

		#endregion

		#region style, exStyle
		/// <summary>
		/// Gets window style.
		/// </summary>
		/// <value>One or more Native.WS_ flags (not WS_EX_) and/or class-specific style flags. Reference: <msdn>window styles</msdn>.</value>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <seealso cref="HasStyle"/>
		/// <seealso cref="SetStyle"/>
		public uint Style
		{
			get => GetWindowLong(Native.GWL_STYLE);
		}

		/// <summary>
		/// Gets window extended style.
		/// </summary>
		/// <value>One or more Native.WS_EX_ flags. Reference: <msdn>extended window styles</msdn>.</value>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <seealso cref="HasExStyle"/>
		/// <seealso cref="SetExStyle"/>
		public uint ExStyle
		{
			get => GetWindowLong(Native.GWL_EXSTYLE);
		}

		/// <summary>
		/// Returns true if the window has all specified style flags (see <see cref="Style"/>).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool HasStyle(uint style)
		{
			return (Style & style) == style;
		}

		/// <summary>
		/// Returns true if the window has all specified extended style flags (see <see cref="ExStyle"/>).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool HasExStyle(uint exStyle)
		{
			return (ExStyle & exStyle) == exStyle;
		}

		/// <summary>
		/// Changes window style.
		/// </summary>
		/// <param name="style">One or more Native.WS_ flags (not WS_EX_) and/or class-specific style flags. Reference: <msdn>window styles</msdn>.</param>
		/// <param name="how"></param>
		/// <param name="updateNC">Update non-client area (frame, title bar).</param>
		/// <param name="updateClient">Update client area.</param>
		/// <exception cref="WndException"/>
		/// <seealso cref="Style"/>
		public void SetStyle(uint style, SetAddRemove how = SetAddRemove.Set, bool updateNC = false, bool updateClient = false)
		{
			_SetStyle(style, how, false, updateNC, updateClient);
		}

		/// <summary>
		/// Changes window extended style.
		/// </summary>
		/// <param name="style">One or more Native.WS_EX_ flags. Reference: <msdn>extended window styles</msdn>.</param>
		/// <param name="how"></param>
		/// <param name="updateNC">Update non-client area (frame, title bar).</param>
		/// <param name="updateClient">Update client area.</param>
		/// <exception cref="WndException"/>
		/// <seealso cref="ExStyle"/>
		public void SetExStyle(uint style, SetAddRemove how = SetAddRemove.Set, bool updateNC = false, bool updateClient = false)
		{
			_SetStyle(style, how, true, updateNC, updateClient);
		}

		void _SetStyle(uint style, SetAddRemove how, bool exStyle = false, bool updateNC = false, bool updateClient = false)
		{
			var gwl = exStyle ? Native.GWL_EXSTYLE : Native.GWL_STYLE;
			if(how != SetAddRemove.Set) {
				uint pstyle = GetWindowLong(gwl);
				if(how == SetAddRemove.Add) style |= pstyle;
				else if(how == SetAddRemove.Remove) style = pstyle & ~style;
				else if(how == SetAddRemove.Xor) style = pstyle ^ style;
			}

			SetWindowLong(gwl, (int)style);

			if(updateNC) SetWindowPos(Native.SWP_FRAMECHANGED | Native.SWP_NOMOVE | Native.SWP_NOSIZE | Native.SWP_NOZORDER | Native.SWP_NOOWNERZORDER | Native.SWP_NOACTIVATE);
			if(updateClient) Api.InvalidateRect(this, Zero, true);
		}

		/// <summary>
		/// Returns true if has Native.WS_POPUP style.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsPopupWindow { get => HasStyle(Native.WS_POPUP); }

		#endregion

		#region window/class long, control id, prop

		/// <summary>
		/// Calls API GetWindowLong if this process is 32-bit, GetWindowLongPtr if 64-bit.
		/// </summary>
		/// <remarks>
		/// All Native.GWL_ values are the same in 32-bit and 64-bit process. Some Native.DWL_ values are different, use Native.DWLP_ instead.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public LPARAM GetWindowLong(int index)
		{
			LPARAM R;
			if(IntPtr.Size == 8) R = Api.GetWindowLong64(this, index); else R = Api.GetWindowLong32(this, index);
			return R;
		}

		/// <summary>
		/// Calls API <msdn>SetWindowLong</msdn> if this process is 32-bit, SetWindowLongPtr if 64-bit.
		/// </summary>
		/// <remarks>
		/// All Native.GWL_ values are the same in 32-bit and 64-bit process. Some Native.DWL_ values are different, use Native.DWLP_ instead.
		/// </remarks>
		/// <exception cref="WndException"/>
		public LPARAM SetWindowLong(int index, LPARAM newValue)
		{
			Native.ClearError();
			LPARAM R;
			if(IntPtr.Size == 8) R = Api.SetWindowLong64(this, index, newValue); else R = Api.SetWindowLong32(this, index, newValue);
			if(R == 0 && Native.GetError() != 0) ThrowUseNative();
			return R;
		}

		/// <summary>
		/// Gets or sets id of this control.
		/// The 'get' function supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <exception cref="WndException">Failed (only 'set' function).</exception>
		public int ControlId
		{
			get => Api.GetDlgCtrlID(this);
			set { SetWindowLong(Native.GWL_ID, value); }
		}

		/// <summary>
		/// Calls API <msdn>GetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM PropGet(string name)
		{
			return Api.GetProp(this, name);
		}
		/// <summary>
		/// Calls API <msdn>GetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		public LPARAM PropGet(ushort atom)
		{
			return Api.GetProp(this, atom);
			//note: cannot use GetLastError, it returns 0 when using atom that exists somewhere else.
		}

		/// <summary>
		/// Calls API <msdn>SetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		/// <param name="value">Property value. Can be a handle or an integer value.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool PropSet(string name, LPARAM value)
		{
			return Api.SetProp(this, name, value);
		}
		/// <summary>
		/// Calls API <msdn>SetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		/// <param name="value">Property value. Can be a handle or an integer value.</param>
		public bool PropSet(ushort atom, LPARAM value)
		{
			return Api.SetProp(this, atom, value);
		}

		/// <summary>
		/// Calls API <msdn>RemoveProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM PropRemove(string name)
		{
			return Api.RemoveProp(this, name);
		}
		/// <summary>
		/// Calls API <msdn>RemoveProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		public LPARAM PropRemove(ushort atom)
		{
			return Api.RemoveProp(this, atom);
		}

		/// <summary>
		/// Gets list of window properties (see <see cref="PropGet(string)">PropGet</see>, PropSet, PropRemove).
		/// Calls API <msdn>EnumPropsEx</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns 0-length list if fails. Fails if invalid window or access denied (UAC). Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public Dictionary<string, LPARAM> PropList()
		{
			var a = new Dictionary<string, LPARAM>();
			Api.EnumPropsEx(this, (w, name, data, p) =>
			{
				string s;
				if((long)name < 0x10000) s = "#" + (int)name; else s = Marshal.PtrToStringUni(name);
				a.Add(s, data);
				return true;
			}, Zero);
			return a;
		}

		#endregion

		#region thread, process, is Unicode, is 64-bit, is hung/ghost, is console, UAC

		/// <summary>
		/// Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// Returns thread id and also gets process id.
		/// Returns 0 if fails. Supports <see cref="Native.GetError"/>.
		/// <note>It is native thread id, not Thread.ManagedThreadId.</note>
		/// </summary>
		public int GetThreadProcessId(out int processId)
		{
			processId = 0;
			return Api.GetWindowThreadProcessId(this, out processId);
		}
		/// <summary>
		/// Calls API <msdn>GetWindowThreadProcessId</msdn> and returns thread id.
		/// Returns 0 if fails. Supports <see cref="Native.GetError"/>.
		/// <note>It is native thread id, not Thread.ManagedThreadId.</note>
		/// </summary>
		public int ThreadId { get => GetThreadProcessId(out var pid); }
		/// <summary>
		/// Calls API <msdn>GetWindowThreadProcessId</msdn> and returns process id.
		/// Returns 0 if fails. Supports <see cref="Native.GetError"/>.
		/// </summary>
		public int ProcessId { get { GetThreadProcessId(out var pid); return pid; } }
		/// <summary>
		/// Returns true if this window belongs to the current thread, false if to another thread.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// </summary>
		public bool IsOfThisThread { get => Api.GetCurrentThreadId() == ThreadId; }
		/// <summary>
		/// Returns true if this window belongs to the current process, false if to another process.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// </summary>
		public bool IsOfThisProcess { get => Api.GetCurrentProcessId() == ProcessId; }

		/// <summary>
		/// Returns true if the window is a Unicode window, false if ANSI.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>IsWindowUnicode</msdn>.
		/// </summary>
		public bool IsUnicode { get => Api.IsWindowUnicode(this); }

		/// <summary>
		/// Returns true if the window is of a 64-bit process, false if of a 32-bit process.
		/// Also returns false if fails. Supports <see cref="Native.GetError"/>.
		/// If <see cref="Ver.Is64BitOS"/> is true, calls API <msdn>GetWindowThreadProcessId</msdn>, <msdn>OpenProcess</msdn> and <msdn>IsWow64Process</msdn>.
		/// <note>If you know that the window belongs to current process, instead use <see cref="Environment.Is64BitProcess"/> or <c>IntPtr.Size==8</c>. This function is much slower.</note>
		/// </summary>
		public bool Is64Bit
		{
			get
			{
				if(Ver.Is64BitOS) {
					int pid = ProcessId; if(pid == 0) return false;
					IntPtr ph = Zero;
					try {
						ph = Api.OpenProcess(Api.PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
						if(ph == Zero || !Api.IsWow64Process(ph, out var is32bit)) return false;
						if(!is32bit) return true;
					}
					finally {
						if(ph != Zero) Api.CloseHandle(ph);
					}
				}
				Native.ClearError();
				return false;

				//info: don't use Process.GetProcessById, it does not have an desiredAccess parameter and fails with higher IL processes.
			}
		}

		/// <summary>
		/// Returns true if thread of this window is considered hung (not responding).
		/// Calls API <msdn>IsHungAppWindow</msdn>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsHung { get => Api.IsHungAppWindow(this); }

		/// <summary>
		/// Returns true if the window is a ghost window that the system creates over a hung (not responding) window to allow the user to minimally interact with it.
		/// </summary>
		public bool IsHungGhost
		{
			get => IsHung && ClassNameIs("Ghost") && ProcessName.Equals_("DWM", true);
			//Class is "Ghost", exe is "DWM" (even if no Aero), text sometimes ends with "(Not Responding)".
			//IsHungWindow returns true for ghost window, although it is not actually hung. It is the fastest.
		}

		/// <summary>
		/// Returns true if this is a console window (class name "ConsoleWindowClass").
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsConsole { get => ClassNameIs("ConsoleWindowClass"); }

		/// <summary>
		/// Returns true if UAC would not allow to automate the window.
		/// It happens when current process has lower UAC integrity level and is not uiAccess, unless UAC is turned off.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsUacAccessDenied
		{
			get
			{
				//with this is faster when don't need other code, but possibly less reliable, and without this is fast enough
				//if(Process_.UacInfo.IsUacDisabled) return false;
				//var t = Process_.UacInfo.ThisProcess;
				//if(t.IsUIAccess) return false;

				Native.ClearError();
				Api.RemoveProp(this, 0);
				return Native.GetError() == Api.ERROR_ACCESS_DENIED; //documented
			}
		}
		//public bool IsUacAccessDenied
		//{
		//	get
		//	{
		//		if(Process_.UacInfo.IsUacDisabled) return false;
		//		var t = Process_.UacInfo.ThisProcess;
		//		if(t.IsUIAccess) return false;
		//		var u = Process_.UacInfo.GetOfProcess(ProcessId); if(u == null) return true;
		//		if(t.IntegrityLevel < u.IntegrityLevel) return true;
		//		return false;
		//	}
		//}

		//These are not useful. Use IsAccessDenied or class Process_.UacInfo.
		///// <summary>
		///// Gets UAC integrity level of window's process.
		///// Returns IL.Unknown if fails.
		///// This function considers UIAccess equal to High.
		///// See also: class Process_.UacInfo.
		///// </summary>
		//public Process_.UacInfo.IL UacIntegrityLevel
		//{
		//	get { var p = Process_.UacInfo.GetOfProcess(ProcessId); return p == null ? UacInfo.IL.Unknown : p.IntegrityLevel; }
		//}

		///// <summary>
		///// Returns true if window's process has higher UAC integrity level (IL) than current process.
		///// Returns true if fails to open process handle, which usually means that the process has higher integrity level.
		///// This function considers UIAccess equal to High.
		///// See also: class Process_.UacInfo.
		///// </summary>
		//public bool UacIntegrityLevelIsHigher
		//{
		//	get => UacIntegrityLevel > Process_.UacInfo.ThisProcess.IntegrityLevel;
		//}

		#endregion

		#region text, class, program

		/// <summary>
		/// Gets class name.
		/// Returns null if fails, eg if the window is closed. Supports <see cref="Native.GetError"/>.
		/// </summary>
		public string ClassName
		{
			get
			{
				const int stackSize = 260;
				var b = stackalloc char[stackSize]; //tested: same speed with Util.Buffers
				int n = Api.GetClassName(this, b, stackSize);
				if(n == 0) return null;
				return _String(b, n);
			}
		}

		/// <summary>
		/// Returns true if the class name of this window matches className. Else returns false.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="className">Class name. Case-insensitive <see cref="String_.Like_(string, string, bool)">wildcard</see>. Cannot be null.</param>
		public bool ClassNameIs(string className)
		{
			return ClassName.Like_(className, true);
		}

		/// <summary>
		/// If the class name of this window matches one of strings in classNames, returns 1-based index of the string. Else returns 0.
		/// Also returns 0 if fails to get class name (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="classNames">Class names. Case-insensitive <see cref="String_.Like_(string, string, bool)">wildcard</see>. The array and strings cannot be null.</param>
		public int ClassNameIs(params string[] classNames)
		{
			string cn = ClassName; if(cn == null) return 0;
			return cn.Like_(true, classNames);
		}

		/// <summary>
		/// Gets window or control name.
		/// Returns "" if no name. Returns null if fails, eg if the window is closed. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <remarks>
		/// <note>It is not the .NET Control.Name property. To get it you can use <see cref="NameWinForms"/>.</note>
		/// Top-level window name usually its title bar text.
		/// Control name usually is its text that does not change, for example button or static (label) control text.
		/// Unlike <see cref="ControlText"/>, this function usually does not get variable text, for example Edit control editable text, ComboBox control selected item text, status bar text.
		/// Calls <see cref="GetText"/>(false, true).
		/// </remarks>
		/// <seealso cref="SetText"/>
		/// <seealso cref="ControlText"/>
		/// <seealso cref="NameAcc"/>
		/// <seealso cref="NameWinForms"/>
		public string Name
		{
			get => GetText(false, true);
		}

		/// <summary>
		/// Gets control text.
		/// Returns "" if no text. Returns null if fails, eg if the window is closed. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <remarks>
		/// Unlike <see cref="Name"/>, this function prefers variable text, for example Edit control editable text, ComboBox control selected item text, status bar text.
		/// For controls that cannot have such text (eg button, static), it usually gets the same text as <b>Name</b>. For example button and static (label) controls.
		/// Much slower than <b>Name</b>. Fails if the window is hung.
		/// Calls <see cref="GetText"/>(true, false).
		/// </remarks>
		/// <seealso cref="SetText"/>
		/// <seealso cref="Name"/>
		public string ControlText
		{
			get => GetText(true, false);
		}

		/// <summary>
		/// Gets window/control name or control text.
		/// Returns "" if it is empty.
		/// Returns null if fails, eg if the window is closed. Supports <see cref="Native.GetError"/>.
		/// This is a low-level function. You can instead use <see cref="Name"/> and <see cref="ControlText"/>.
		/// </summary>
		/// <param name="getText">
		/// false - use API function <msdn>InternalGetWindowText</msdn>. It is fast and usually does not get variable text. This is used by <see cref="Name"/>.
		/// true - use API message <msdn>WM_GETTEXT</msdn>. It is slow and prefers variable text. This is used by <see cref="ControlText"/>. Fails if the window is hung.
		/// null - try InternalGetWindowText. If it gets "", and this is a control, then try WM_GETTEXT.
		/// </param>
		/// <param name="removeUnderlineAmpersand">
		/// Remove the invisible '&amp;' characters that are used to underline keyboard shortcuts with the Alt key.
		/// Removes only if this is a control (has style Native.WS_CHILD).
		/// Calls <see cref="Misc.StringRemoveUnderlineAmpersand"/>.
		/// </param>
		/// <seealso cref="SetText"/>
		/// <seealso cref="NameAcc"/>
		/// <seealso cref="NameWinForms"/>
		public string GetText(bool? getText = null, bool removeUnderlineAmpersand = true)
		{
			string R = null;

			if(getText == null) R = _GetTextFast(true);
			else if(getText.GetValueOrDefault()) R = _GetTextSlow();
			else R = _GetTextFast(false);

			if(removeUnderlineAmpersand
				&& !Empty(R)
				//&& R.IndexOf('&') >= 0 //slower than HasStyle if the string is longer than 20
				&& HasStyle(Native.WS_CHILD)
				) R = Misc.StringRemoveUnderlineAmpersand(R);

			return R;
		}

		/// <summary>
		/// Gets text.
		/// Returns "" if it is empty.
		/// Returns null if fails, eg if the control is destroyed or its thread is hung. Supports <see cref="Native.GetError"/>.
		/// Calls API InternalGetWindowText. If it fails, and getControlTextIfEmpty==true, and this is a control, calls _GetTextSlow, which uses WM_GETTEXT.
		/// </summary>
		string _GetTextFast(bool useSlowIfEmpty)
		{
			if(Is0) return null;
			for(int na = 300; ; na *= 2) {
				var b = Util.Buffers.LibChar(ref na);
				Native.ClearError();
				int nr = Api.InternalGetWindowText(this, b, na);
				if(nr < na - 1) {
					if(nr > 0) return _String(b, nr);
					if(Native.GetError() != 0) return null;
					if(useSlowIfEmpty && HasStyle(Native.WS_CHILD)) return _GetTextSlow();
					return "";
				}
			}
		}

		/// <summary>
		/// Gets text.
		/// Returns "" if it is empty.
		/// Returns null if fails, eg if the control is destroyed or its thread is hung. Supports <see cref="Native.GetError"/>.
		/// Uses WM_GETTEXT.
		/// </summary>
		string _GetTextSlow()
		{
			if(!SendTimeout(5000, out LPARAM ln, Api.WM_GETTEXTLENGTH)) return null;
			int n = ln; if(n < 1) return "";

			var b = Util.Buffers.LibChar(n);
			fixed (char* p = b.A) {
				if(!SendTimeout(30000, out ln, Api.WM_GETTEXT, n + 1, p)) return null;
				if(ln < 1) return "";
				b.A[n] = '\0';
				n = Util.LibCharPtr.Length(p, n); //info: some controls return incorrect ln, eg including '\0'
				return _String(b, n);
			}

			//note: cannot do this optimization:
			//	At first allocate stack memory and send WM_GETTEXT without WM_GETTEXTLENGTH. Then use WM_GETTEXTLENGTH/WM_GETTEXT if returned size is buffer length - 1.
			//	It works with most controls, but some controls return 0 if buffer is too small. Eg SysLink. The WM_GETTEXT documentation does not say what should happen when buffer is too small.
			//	The speed is important for Wnd.Child().
			//	It is ~30% faster when all controls have text, but not much if called for many controls that don't have text (then we don't use WM_GETTEXT).
		}

		/// <summary>
		/// Sets window/control name or control text.
		/// </summary>
		/// <param name="text">Text. Can be null, it is the same as "".</param>
		/// <remarks>
		/// Uses API <msdn>WM_SETTEXT</msdn>.
		/// Top-level window name usually its title bar text.
		/// For variable-text controls (edit, combo box, status bar, ...) this usually is the text that <see cref="ControlText"/> would get.
		/// For other controls (button, static, ...) and top-level windows this usually is the text that <see cref="Name"/> would get.
		/// </remarks>
		/// <exception cref="WndException">Failed, for example the window is closed.</exception>
		/// <seealso cref="GetText"/>
		/// <seealso cref="Name"/>
		/// <seealso cref="ControlText"/>
		public void SetText(string text)
		{
			if(!SendTimeoutS(30000, out var _, Api.WM_SETTEXT, 0, text ?? "", 0)) ThrowUseNative();
		}

		/// <summary>
		/// Gets MSAA IAccessible.Name property.
		/// </summary>
		public string NameAcc { get => _GetNameAcc(); }

		string _GetNameAcc()
		{
			if(!IsAlive) return null;
			try {
				return Acc.FromWindow(this).Name;
			}
			catch(CatException) { }
			return null;
		}

		/// <summary>
		/// Gets Control.Name property of a .NET Windows Forms control.
		/// Returns null if it is not a Windows Forms control or if fails.
		/// <note>Use this with controls of other processes. Don't use with your controls, when you have a Control object.</note>
		/// <note>This is slow when getting names of multiple controls in a window. Instead create a <see cref="Misc.WinFormsControlNames"/> instance and call its <see cref="Misc.WinFormsControlNames.GetControlName"/> method for each control.</note>
		/// </summary>
		/// <seealso cref="Misc.WinFormsControlNames.IsWinFormsControl"/>
		public string NameWinForms { get => Misc.WinFormsControlNames.GetSingleControlName(this); }

		/// <summary>
		/// Gets filename (without ".exe") of process executable file.
		/// Return null if fails.
		/// Calls <see cref="ProcessId"/> and <see cref="Process_.GetProcessName">Process_.GetProcessName</see>.
		/// </summary>
		public string ProcessName { get => Process_.GetProcessName(ProcessId); }

		/// <summary>
		/// Gets full path of process executable file.
		/// Return null if fails.
		/// Calls <see cref="ProcessId"/> and <see cref="Process_.GetProcessName">Process_.GetProcessName</see>.
		/// </summary>
		public string ProcessPath { get => Process_.GetProcessName(ProcessId, true); }

		#endregion

		#region close, destroy

		/// <summary>
		/// Closes the window.
		/// Returns true if successfuly closed or if it was already closed (the handle is 0 or invalid) or if noWait==true.
		/// </summary>
		/// <param name="noWait">
		/// If false (default), waits a while until the window is destroyed or disabled. But does not wait indefinitely.
		/// If true, does not wait.
		/// </param>
		/// <param name="useXButton">
		/// If false (default), uses API message <msdn>WM_CLOSE</msdn>.
		/// If true, uses API message <msdn>WM_SYSCOMMAND SC_CLOSE</msdn>, like when the user clicks the X button in the title bar.
		/// Most windows can be closed with any of these messages, but some respond properly only to one of them. For example, some applications on WM_CLOSE don't exit, although the main window is closed. Some applications don't respond to WM_SYSCOMMAND if it is posted soon after opening the window, for example Internet Explorer.
		/// </param>
		/// <remarks>
		/// The window may refuse to be closed. For example, it may be hung, or hide itself instead, or display a "Save?" message box, or is a dialog without X button, or just need more time to close it.
		/// If the window is of this thread, just calls <see cref="Send">Send</see> (if noWait==false) or <see cref="Post">Post</see> (if noWait==true) and returns true.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// //close all Notepad windows
		/// Wnd.FindAll("* Notepad", "Notepad").ForEach(t => t.Close());
		/// ]]></code>
		/// </example>
		public bool Close(bool noWait = false, bool useXButton = false)
		{
			//CONSIDER: add parameter killProcessAfterTimeMS. If non-0, kills process if the window still exists after that time. The default wait is not inclused in this timeout.

			if(!IsAlive) return true;

			uint msg = Api.WM_CLOSE, wparam = 0; if(useXButton) { msg = Api.WM_SYSCOMMAND; wparam = Api.SC_CLOSE; }

			if(IsOfThisThread) {
				if(noWait) Post(msg, wparam);
				else Send(msg, wparam);
				return true;
			}

			//Some windows cannot be properly closed using only WM_CLOSE. For example, VS 7 window closed, but the process not.
			//Some windows (eg IE), ignore SC_CLOSE if it is posted soon after starting the program. Only if using Post, not if SendNotify.
			//When the user closes a window, Windows sends SC_CLOSE. DefWindowProc on SC_CLOSE sends WM_CLOSE, except if disabled.

			//Other apps use either WM_CLOSE or SC_CLOSE.
			//	Task Manager and taskbar send (not post) SC_CLOSE.
			//	Process Explorer and taskkill post WM_CLOSE.

			//For our purposes WM_CLOSE is probably better.
			//	QM2 used SC_CLOSE or WM_CLOSE depending on window style.

			//note: Don't use SendX messages because of possible crashes and other anomalies.
			//	Eg it can hide (not close) some windows, and next app instance does not start (eg Firefox).
			//	Imagine if the target window's app is in SendMessage which dispatches foreign messages, and after it returns, the window is already died.

			bool ok = Post(msg, wparam);
			if(!ok) {
				//Print(Native.GetError()); //0 when UAC access denied
				if(!useXButton) ok = Post(Api.WM_SYSCOMMAND, Api.SC_CLOSE); //UAC blocks WM_CLOSE but not WM_SYSCOMMAND
			}
			if(noWait) return true;

			if(ok) {
				for(int i = 0; i < 100; i++) {
					Thread.Sleep(15);
					if(!IsEnabled) break; //destroyed or has an owned modal dialog box, eg "Save?"

					//Wait less if hidden, eg has a tray icon.
					//Also if a popup, eg a Yes/No message box (disabled X button).
					//Also if child.
					//if(!IsVisible && !_IsBusy(2)) i += 4; //unreliable, too dirty
					if(!IsVisible || (Style & (Native.WS_POPUP | Native.WS_CHILD)) != 0) i += 2;

					if(i >= 50) {
						if(!SendTimeout(200, 0)) {
							if(!IsAlive || IsHung) break;
						}
					}
				}
			}
			_MinimalWaitNoCheckThread();
			Misc.WaitForAnActiveWindow();

			return !IsAlive;
		}

		//bool _IsBusy(int timeMS)
		//{
		//	//Need to measure time. Cannot use just 2 ms timeout and ST return value because of the system timer default period 15.6 ms etc.
		//	var t = Time.Microseconds;
		//	SendTimeout(5 + timeMS, 0, flags: 0);
		//	var d = Time.Microseconds - t;
		//	//Print(d);
		//	return (d >= timeMS * 1000L);
		//}

		//Rarely used. It is easy, and there is example in Close() help: Wnd.FindAll("* Notepad", "Notepad").ForEach(t => t.Close());
		///// <summary>
		///// Closes all matching windows.
		///// Calls <see cref="FindAll">FindAll</see>. All parameters etc are the same. Then calls <see cref="Close">Close</see> for each found window.
		///// Returns the number of found windows.
		///// </summary>
		//public static int CloseAll(
		//	string name, string className = null, object programEtc = null,
		//	WFFlags flags = 0, Func<Wnd, bool> f = null
		//	)
		//{
		//	var a = FindAll(name, className, programEtc, flags, f);
		//	foreach(Wnd w in a) w.Close();
		//	return a.Count;
		//}

		#endregion

	}

}
