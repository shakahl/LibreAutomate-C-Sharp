﻿using System;
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

using Au.Types;
using static Au.NoClass;

#pragma warning disable 282 //intellisense bug: it thinks that Wnd has multiple fields.

namespace Au
{
	/// <summary>
	/// A variable of Wnd type represents a window or control. It is a window handle, also known as HWND.
	/// </summary>
	/// <remarks>
	/// Wnd functions can be used with windows and controls of any process/thread. Also can be used with .NET form/control and WPF window class variables, like <c>Wnd w=(Wnd)form; w.Method(...);</c> or <c>((Wnd)form).Method(...);</c>.
	/// 
	/// There are two main types of windows - top-level windows and controls. Controls are child windows of top-level windows.
	/// 
	/// More functions are in the nested classes - <see cref="Misc"/>, <see cref="Misc.Desktop"/> etc. They are used mostly in programming, rarely in automation scripts.
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
	/// Many functions fail if the window's process has a higher <see cref="Process_.UacInfo">UAC</see> integrity level (aministrator, uiAccess) than this process, unless this process has uiAccess level. Especially the functions that change window properties. Some functions that still work: <b>Activate</b>, <b>ActivateLL</b>, <b>ShowMinimized</b>, <b>ShowNotMinimized</b>, <b>ShowNotMinMax</b>, <b>Close</b>.
	/// 
	/// The Wnd type can be used with native Windows API functions without casting. Use Wnd for the parameter type in the declaration, like <c>[DllImport(...)] static extern bool NativeFunction(Wnd hWnd, ...)</c>.
	/// 
	/// See also: MSDN article <msdn>Window Features</msdn>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Wnd w = Wnd.Find("* - Notepad");
	/// if(w.Is0) { Print("window not found"); return; }
	/// w.Activate();
	/// Wnd c = w.Child(className: "Button");
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
		///		<item>Probably it is not a "bad practice" to have a struct with many member functions, because eg the .NET DateTime is struct.</item>
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

		#region constructors, operators, overrides

#pragma warning disable 1591 //XML doc
		Wnd(void* hwnd) { _h = hwnd; }
		Wnd(IntPtr hwnd) { _h = (void*)hwnd; }

		//note: don't need implicit conversions. It creates more problems than is useful.

		public static explicit operator Wnd(IntPtr hwnd) => new Wnd(hwnd);
		public static explicit operator IntPtr(Wnd w) => w.Handle;
		public static explicit operator Wnd(LPARAM hwnd) => new Wnd((void*)hwnd);
		public static explicit operator LPARAM(Wnd w) => w._h;
		public static implicit operator Wnd(Native.HWND hwnd) => new Wnd((void*)(int)hwnd);

		/// <summary>
		/// Gets the window handle as Wnd from a System.Windows.Forms.Control (or Form etc) variable.
		/// Returns default(Wnd) if w is null or the handle is still not created.
		/// Should be called in c thread. Calls <see cref="System.Windows.Forms.Control.IsHandleCreated"/> and <see cref="System.Windows.Forms.Control.Handle"/>.
		/// </summary>
		public static explicit operator Wnd(System.Windows.Forms.Control c) => new Wnd(c == null || !c.IsHandleCreated ? default : c.Handle);

		/// <summary>
		/// Gets the window handle as Wnd from a System.Windows.Window variable (WPF window).
		/// Returns default(Wnd) if w is null or the handle is still not created.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static explicit operator Wnd(System.Windows.Window w) => new Wnd(w == null ? default : new System.Windows.Interop.WindowInteropHelper(w).Handle);

		/// <summary>Compares window handles.</summary>
		public static bool operator ==(Wnd w1, Wnd w2) => w1._h == w2._h;
		/// <summary>Compares window handles.</summary>
		public static bool operator !=(Wnd w1, Wnd w2) => w1._h != w2._h;

		//Prevent accidental usage Wnd==null. The C# compiler allows it without a warning. As a side effect, the above also disables Wnd==Wnd?.
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator ==(Wnd w1, Wnd? w2) => false;
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator !=(Wnd w1, Wnd? w2) => true;
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator ==(Wnd? w1, Wnd w2) => false;
		[Obsolete("Replace Wnd==Wnd? with Wnd.Equals(Wnd?). Replace Wnd==null with Wnd.Is0.", true)]
		public static bool operator !=(Wnd? w1, Wnd w2) => true;
#pragma warning restore 1591 //XML doc

		//rejected. Use OrThrow.
		///// <summary>
		///// If x is not default(Wnd), returns x, else throws <see cref="NotFoundException"/>.
		///// Alternatively you can use <see cref="ExtensionMethods.OrThrow(Wnd)" r=""/>.
		///// </summary>
		///// <exception cref="NotFoundException">x is default(Wnd).</exception>
		///// <example><inheritdoc cref="ExtensionMethods.OrThrow(Wnd)"/></example>
		//public static Wnd operator +(Wnd x) => !x.Is0 ? x : throw new NotFoundException("Not found (Wnd).");

		/// <summary>
		/// Returns true if w == this.
		/// </summary>
		public bool Equals(Wnd w) => w == this; //IEquatable<Wnd>.Equals, to avoid boxing with eg Dictionary<Wnd, T2>

		/// <summary>
		/// Returns true if w != null and w.Value == this.
		/// </summary>
		public bool Equals(Wnd? w)
		{
			return w != null && w.GetValueOrDefault() == this;
		}

		/// <summary>
		/// Returns true if obj is Wnd and contains the same window handle.
		/// </summary>
		public override bool Equals(object obj)
		{
			//return obj is Wnd w && this == w; //compiler creates slow and big code if 'is ValueType variable'
			return obj is Wnd && this == (Wnd)obj;
		}

		///
		public override int GetHashCode() => (int)_h;
		//window handles are always 32-bit int, although in a 64-bit process stored in 64-bit variables.
		//IntPtr.GetHashCode also returns this.

		/// <summary>
		/// Gets window handle as IntPtr.
		/// Code <c>w.Handle</c> is the same as <c>(IntPtr)w</c> .
		/// </summary>
		public IntPtr Handle => new IntPtr(_h);

		/// <summary>
		/// Formats string $"{handle}  {ClassName}  \"{Name}\"  {ProgramName}  {Rect}".
		/// </summary>
		public override string ToString()
		{
			if(Is0) return "0";
			var cn = ClassName;
			var sh = Handle.ToString();
			if(cn == null) return sh + " <invalid handle>";
			string s = Name;
			if(s != null) s = s.Escape_(limit: 250);
			return $"{sh}  {cn}  \"{s}\"  {ProgramName}  {Rect.ToString()}";
		}

		#endregion

		#region send/post message

		/// <summary>
		/// Calls API <msdn>SendMessage</msdn>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM Send(int message, LPARAM wParam = default, LPARAM lParam = default)
		{
			Debug.Assert(!Is0);
			return Api.SendMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API <msdn>SendMessage</msdn> where lParam is string.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM SendS(int message, LPARAM wParam, string lParam)
		{
			Debug.Assert(!Is0);
			fixed (char* p = lParam)
				return Api.SendMessage(this, message, wParam, p);
			//info: don't use overload, then eg ambiguous if null.
		}

		/// <summary>
		/// Calls API <msdn>SendMessage</msdn> where lParam is char[].
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM SendS(int message, LPARAM wParam, char[] lParam)
		{
			Debug.Assert(!Is0);
			fixed (char* p = lParam)
				return Api.SendMessage(this, message, wParam, p);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn>.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		public bool SendTimeout(int timeoutMS, int message, LPARAM wParam = default, LPARAM lParam = default, Native.SMTO flags = Native.SMTO.ABORTIFHUNG)
		{
			Debug.Assert(!Is0);
			return 0 != Api.SendMessageTimeout(this, message, wParam, lParam, flags, timeoutMS, out LPARAM R);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn> and gets the result of the message processing.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		public bool SendTimeout(int timeoutMS, out LPARAM result, int message, LPARAM wParam = default, LPARAM lParam = default, Native.SMTO flags = Native.SMTO.ABORTIFHUNG)
		{
			Debug.Assert(!Is0);
			result = 0;
			return 0 != Api.SendMessageTimeout(this, message, wParam, lParam, flags, timeoutMS, out result);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn> where lParam is string.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, out LPARAM result, int message, LPARAM wParam, string lParam, Native.SMTO flags = Native.SMTO.ABORTIFHUNG)
		{
			Debug.Assert(!Is0);
			result = 0;
			fixed (char* p = lParam)
				return 0 != Api.SendMessageTimeout(this, message, wParam, p, flags, timeoutMS, out result);
		}

		/// <summary>
		/// Calls API <msdn>SendMessageTimeout</msdn> where lParam is char[].
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, out LPARAM result, int message, LPARAM wParam, char[] lParam, Native.SMTO flags = Native.SMTO.ABORTIFHUNG)
		{
			Debug.Assert(!Is0);
			result = 0;
			fixed (char* p = lParam)
				return 0 != Api.SendMessageTimeout(this, message, wParam, p, flags, timeoutMS, out result);
		}

		/// <summary>
		/// Calls API <msdn>SendNotifyMessage</msdn>.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		public bool SendNotify(int message, LPARAM wParam = default, LPARAM lParam = default)
		{
			Debug.Assert(!Is0);
			return Api.SendNotifyMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API <msdn>PostMessage</msdn>.
		/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <seealso cref="Misc.PostThreadMessage(int, LPARAM, LPARAM)"/>
		public bool Post(int message, LPARAM wParam = default, LPARAM lParam = default)
		{
			Debug.Assert(!Is0);
			return Api.PostMessage(this, message, wParam, lParam);
		}

		public static partial class Misc
		{
			/// <summary>
			/// Posts a message to the message queue of this thread.
			/// Calls API <msdn>PostMessage</msdn> with default(Wnd). 
			/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
			/// </summary>
			public static bool PostThreadMessage(int message, LPARAM wParam = default, LPARAM lParam = default)
			{
				return Api.PostMessage(default, message, wParam, lParam);
			}

			/// <summary>
			/// Posts a message to the message queue of the specified thread.
			/// Calls API <msdn>PostThreadMessage</msdn>. 
			/// Returns its return value (false if failed). Supports <see cref="Native.GetError"/>.
			/// </summary>
			public static bool PostThreadMessage(int threadId, int message, LPARAM wParam = default, LPARAM lParam = default)
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
		/// Throws <see cref="WndException"/> that uses mainMessage and the specified Windows API error code.
		/// Also the message depends on whether the window handle is 0/invalid.
		/// </summary>
		/// <exception cref="WndException"></exception>
		public void ThrowUseNative(int errorCode, string mainMessage)
		{
			throw new WndException(this, errorCode, mainMessage);
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
		/// Returns true if the <see cref="Wnd">window handle</see> is 0.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("Window*");
		/// if(w.Is0) { Print("window not found"); return; }
		/// ]]></code>
		/// </example>
		/// <seealso cref="IsAlive"/>
		public bool Is0 => _h == null;

		/// <summary>
		/// Returns true if the <see cref="Wnd">window handle</see> identifies an existing window.
		/// Returns false if the handle is 0 or invalid.
		/// Invalid non-0 handle usually means that the window is closed/destroyed.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="Is0"/> and API <msdn>IsWindow</msdn>.
		/// Although a Wnd variable holds a window handle, which is like a reference to a window, it does not prevent closing that window and making the handle invalid. After closing the window, the OS can even assign the same handle value to a new window, although normally it can happen only after long time.
		/// <note>Use this carefully with windows of other applications or threads. The window can be closed at any moment, even when your thread is still in this function.</note>
		/// </remarks>
		public bool IsAlive => !Is0 && Api.IsWindow(this);

		#endregion

		#region visible, enabled, cloaked

		/// <summary>
		/// Returns true if the window is visible.
		/// Returns false if is invisible or is a child of invisible parent.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>IsWindowVisible</msdn>. Does not call <see cref="IsCloaked"/>.
		/// 
		/// Even when this function returns true, the window may be actually invisible. It can be cloaked, on an inactive Windows 10 virtual desktop (cloaked), inactive Windows Store app (cloaked), transparent, zero-size, minimized, off-screen, covered by other windows or can have zero-size window region.
		/// </remarks>
		/// <seealso cref="IsVisibleEx"/>
		/// <seealso cref="IsCloaked"/>
		/// <seealso cref="IsVisibleAndNotCloaked"/>
		/// <seealso cref="Show"/>
		/// <seealso cref="Activate()"/>
		public bool IsVisible => Api.IsWindowVisible(this);

		/// <summary>
		/// Returns true if the window is visible.
		/// Returns false if is invisible or is a child of invisible parent.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <remarks>
		/// Returns false if API <msdn>IsWindowVisible</msdn> returns false.
		/// Also returns false if <see cref="IsCloaked"/> returns true, but only for some popup windows that usually are useless and could cause problems if considered visible.
		/// Else returns true.
		/// 
		/// Even when this function returns true, the window may be actually invisible. It can be cloaked (excepth the above case), on an inactive Windows 10 virtual desktop (cloaked), inactive Windows Store app (cloaked), transparent, zero-size, minimized, off-screen, covered by other windows or can have zero-size window region.
		/// </remarks>
		/// <seealso cref="IsVisible"/>
		/// <seealso cref="IsCloaked"/>
		/// <seealso cref="IsVisibleAndNotCloaked"/>
		/// <seealso cref="Show"/>
		/// <seealso cref="Activate()"/>
		public bool IsVisibleEx
		{
			get
			{
				if(!Api.IsWindowVisible(this)) return false;

				var style = Style;
				if((style & (Native.WS.POPUP | Native.WS.CHILD)) == Native.WS.POPUP) {
					if((style & Native.WS.CAPTION) != Native.WS.CAPTION) return !IsCloaked;

					//is it a ghost ApplicationFrameWindow, like closed Calculator on Win10?
					if(Ver.MinWin10 && HasExStyle(Native.WS_EX.NOREDIRECTIONBITMAP) && IsCloaked && ClassNameIs("ApplicationFrameWindow")) {
						var isGhost = default == Api.FindWindowEx(this, default, "Windows.UI.Core.CoreWindow", null);
						//Print(isGhost, this);
						return !isGhost;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Returns true if <see cref="IsVisible"/> returns true and <see cref="IsCloaked"/> returns false.
		/// </summary>
		/// <seealso cref="IsVisibleEx"/>
		public bool IsVisibleAndNotCloaked => IsVisible && !IsCloaked;

		/// <summary>
		/// Shows (if hidden) or hides this window.
		/// Does not activate/deactivate/zorder.
		/// With windows of current thread usually it's better to use <see cref="ShowLL"/>.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>ShowWindow</msdn> with SW_SHOWNA or SW_HIDE.
		/// </remarks>
		/// <exception cref="WndException"/>
		public void Show(bool show)
		{
			if(!ShowLL(show)) ThrowUseNative(show ? "*show*" : "*hide*");
			LibMinimalSleepIfOtherThread();
		}

		/// <summary>
		/// Shows (if hidden) or hides this window.
		/// Does not activate/deactivate/zorder.
		/// </summary>
		/// <remarks>
		/// This library has two similar functions - <see cref="Show"/> and <b>ShowLL</b>. <b>Show</b> is better to use in automation scripts, with windows of any process/thread. <b>ShowLL</b> usually is better to use in programming, with windows of current thread.
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
				int cloaked = 0;
				int hr = Api.DwmGetWindowAttribute(this, Api.DWMWA.CLOAKED, &cloaked, 4);
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

			LibMinimalSleepIfOtherThread();
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
		/// Sets WINDOWPLACEMENT <b>length</b> field and calls API <msdn>SetWindowPlacement</msdn>.
		/// </summary>
		/// <exception cref="WndException">Failed. Throws, only if errStr!=null, else returns false.</exception>
		internal bool LibSetWindowPlacement(ref Api.WINDOWPLACEMENT wp, string errStr = null)
		{
			//initially this was public, but probably don't need.

			wp.length = Api.SizeOf(wp);
			if(Api.SetWindowPlacement(this, wp)) return true;
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

				var wFore = Active; bool retry = false;
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

				var x = new Api.INPUTK(0, 128, Api.KEYEVENTF_KEYUP);
				Api.SendInput(&x);
				//info: works without waiting.
			}

			/// <summary>
			/// Creates a temporary minimized window and restores it. It activates the window and allows us to activate.
			/// Then sets 'no active window' to prevent auto-activating another window when destroying the temporary window.
			/// </summary>
			static void _EnableActivate_MinRes()
			{
				Debug_.Print("EnableActivate: need min/res");

				Wnd t = Misc.CreateWindow("#32770", null, Native.WS.POPUP | Native.WS.MINIMIZE | Native.WS.VISIBLE, Native.WS_EX.TOOLWINDOW);
				//info: When restoring, the window must be visible, or may not work.
				try {
					var wp = new Api.WINDOWPLACEMENT { showCmd = Api.SW_RESTORE };
					t.LibSetWindowPlacement(ref wp); //activates t; fast (no animation)
					_EnableActivate_SendKey(false); //makes so that later our process can always activate
					_EnableActivate_AllowSetFore();
					Api.SetForegroundWindow(GetWnd.Root); //set no foreground window, or may activate the higher IL window (maybe does not activate, but winevents hook gets events, in random order). Other way would be to destroy our window later, but more difficult to implement.
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
					if(w == GetWnd.Root) return Active.Is0;
					//CONSIDER: if GetForegroundWindow is not w, send WM_NULL. Info: https://blogs.msdn.microsoft.com/oldnewthing/20161118-00/?p=94745
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
				/// Don't call Get.Window (ie caller ensures it's a top-level window, not control).
				/// </summary>
				NoGetWindow = 2,

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
			if(!flags.Has_(Lib.ActivateFlags.NoThrowIfInvalid)) ThrowIfInvalid();
			if(flags.Has_(Lib.ActivateFlags.NoGetWindow)) Debug.Assert(!IsChild);
			else {
				var w = Window;
				if(w != this) {
					return w.LibActivate((flags | Lib.ActivateFlags.NoGetWindow) & ~Lib.ActivateFlags.NoThrowIfInvalid);
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
					var est = ExStyle;
					if((est & Native.WS_EX.NOACTIVATE) != 0) noAct = true;
					else if((est & (Native.WS_EX.TOOLWINDOW | Native.WS_EX.APPWINDOW)) == Native.WS_EX.TOOLWINDOW) noAct = !HasStyle(Native.WS.CAPTION);
					if(noAct && !IsCloaked) {
						ZorderTop(); //in most cases does not work, but try anyway, it just calls the API. It seems works if the window is topmost.
						return false; //if cloaked, need to activate to uncloak
					}
				}

				for(int i = 0; i < 3; i++) {
					bool ok = ActivateLL();

					if(!ofThisThread) {
						LibMinimalSleepNoCheckThread();
						LibMinimalSleepNoCheckThread();
					}

					if(ok) {
						Wnd f = Active;
						if(f == this) R = true;
						else if(this == GetWnd.Root) R = f.Is0; //activating GetDesktopWindow makes "no active window"
						else { //forgive if the target app instead activated another window of same thread
							int tid = ThreadId; if(tid == 0) break;
							if(f.ThreadId == tid) {
								//at first try to recognize such known windows, to avoid the hard way
								if(isMinimized || (f.Owner == this && Rect.IsEmpty)) {
									R = true;
								} else {
									R = Api.SetForegroundWindow(GetWnd.Root) && ActivateLL() && Active.ThreadId == tid;
									if(R && !ofThisThread) {
										LibMinimalSleepNoCheckThread();
										R = Active.ThreadId == tid;
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

			if(R && !ofThisThread && this != GetWnd.Root) {
				//If we activate a window that is on an inactive Win10 desktop, its desktop becomes active.
				//Windows on inactive desktops are cloaked. They are uncloaked after ~15 ms.
				if(IsCloaked) {
					R = false;
					for(int i = 0; i < 50; i++) { Thread.Sleep(30); if(R = !IsCloaked) break; }
					if(R) {
						if(forScreenCapture) Thread.Sleep(800); //need minimum 600 for 'find image' functions, because of animation while switching Win10 desktops.
						LibMinimalSleepNoCheckThread();
						R = IsActive;
						if(!R && ActivateLL()) {
							LibMinimalSleepNoCheckThread();
							R = IsActive;
						}
					}
				}
			}

			if(!R) ThrowNoNative("*activate*");
			if(forScreenCapture) LibMinimalSleepIfOtherThread();

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
		/// <item>If this is <see cref="GetWnd.Root"/>, just deactivates the currently active window.</item>
		/// <item>When the target application instead activates another window of the same thread.</item>
		/// </list>
		/// </remarks>
		/// <exception cref="WndException"/>
		/// <seealso cref="ActivateLL"/>
		/// <seealso cref="IsActive"/>
		/// <seealso cref="Active"/>
		/// <seealso cref="SwitchActiveWindow"/>
		public void Activate()
		{
			LibActivate(0);
		}
		//CONSIDER: if fails to activate:
		//TaskDialogEx("Failed to activate window", w.ToString(), footer: The script will continue if you activate the window in {x} s.", timeout: 10);

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
					if(!Active.Is0) return true;
					Thread.Sleep(15); //SHOULDDO: SleepDoEvents, or WaitForCallback
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
		/// Sets the keyboard input focus to this control.
		/// Also activetes its top-level parent window (see <see cref="Activate()"/>).
		/// </summary>
		/// <remarks>
		/// The control can belong to any process/thread. With controls of this thread you can use the more lightweight function <see cref="ThisThread.Focus"/>.
		/// Works not with all windows. For example, does not work with Windows Store apps. Then use <see cref="Acc.Focus"/>.
		/// Can instead focus a child control. For example, if this is a ComboBox, it will focus its child Edit control. Then does not throw exception.
		/// This can be control or top-level window. Top-level windows also can have focus.
		/// </remarks>
		/// <exception cref="WndException">
		/// Invalid handle; disabled; failed to set focus; failed to activate parent window.
		/// Fails to set focus when the target process is admin or uiAccess and this process isn't (see <see cref="Process_.UacInfo">UAC</see>).
		/// </exception>
		/// <seealso cref="Focused"/>
		/// <seealso cref="IsFocused"/>
		/// <seealso cref="Acc.Focus"/>
		public void Focus()
		{
			ThrowIfInvalid();
			Wnd wTL = Window;
			if(!wTL.IsActive) wTL.LibActivate(Lib.ActivateFlags.NoGetWindow);

			int tid = ThreadId;
			if(tid == Api.GetCurrentThreadId()) {
				if(!ThisThread.Focus(this)) {
					if(!IsEnabled) goto gDisabled;
					goto gFailed;
				}
				return;
			}

			if(IsFocused) return;
			if(!IsEnabled) goto gDisabled;

			bool ok = false;
			using(new Util.LibAttachThreadInput(tid, out bool atiOK)) {
				if(atiOK) { //FUTURE: if fails, try Acc.Focus or UIA. AttachThreadInput is unreliable.
					for(int i = 0; i < 5; i++) {
						if(i > 0) Thread.Sleep(30);
						Native.ClearError();
						if(ThisThread.Focus(this)) {
							Wnd f = Focused;
							if(f == this || f.IsChildOf(this)) { ok = true; break; }
						}
					}
				}
			}
			if(!ok) goto gFailed;

			LibMinimalSleepNoCheckThread();
			return;
			gDisabled: //SetFocus fails if disabled
			ThrowIfInvalid();
			ThrowNoNative("*set focus. Disabled");
			gFailed:
			ThrowUseNative("*set focus");
		}

		/// <summary>
		/// Gets the control or window that has the keyboard input focus.
		/// </summary>
		/// <remarks>
		/// The control/window can belong to any process/thread. With controls/windows of this thread you can use the more lightweight function <see cref="ThisThread.Focused"/>.
		/// Calls API <msdn>GetGUIThreadInfo</msdn>.
		/// </remarks>
		/// <seealso cref="Focus"/>
		/// <seealso cref="IsFocused"/>
		public static Wnd Focused
		{
			get
			{
				Misc.GetGUIThreadInfo(out var g);
				return g.hwndFocus;
			}
		}
		//FUTURE: need functions that wait eg max 1 s until a window is focused or active.
		//	Example: after activating a window using the taskbar, there is no active window for 100 ms or more.
		//	Example: after opening a common file dialog, the Edit control is focused after 200 ms, until that there is no focus.
		// For 'active' we already have Misc.WaitForAnActiveWindow.

		/// <summary>
		/// Returns true if this is the control or window that has the keyboard input focus.
		/// </summary>
		/// <remarks>
		/// This control/window can belong to any process/thread. With controls/windows of this thread you can use the more lightweight function <see cref="ThisThread.IsFocused"/>.
		/// Calls <see cref="Focused"/>.
		/// </remarks>
		/// <seealso cref="Focus"/>
		public bool IsFocused => !this.Is0 && this == Focused;

		/// <summary>
		/// Functions that can be used only with windows/controls of this thread.
		/// </summary>
		public static class ThisThread
		{
			/// <summary>
			/// Calls API <msdn>SetFocus</msdn>. It sets the keyboard input focus to the specified control or window, which must be of this thread.
			/// Returns false if fails. Supports <see cref="Native.GetError"/>.
			/// </summary>
			/// <remarks>
			/// Fails if the control/window belongs to another thread or is invalid or disabled.
			/// Can instead focus a child control. For example, if ComboBox, will focus its child Edit control. Then returns true.
			/// </remarks>
			public static bool Focus(Wnd w)
			{
				if(w.Is0) { Api.SetLastError(Api.ERROR_INVALID_WINDOW_HANDLE); return false; }
				var f = Api.GetFocus(); if(f == w) return true;
				if(!Api.SetFocus(w).Is0) return true;
				if(f.Is0) return !Api.GetFocus().Is0;
				return false;
			}

			/// <summary>
			/// Gets the focused control or window of this thread.
			/// </summary>
			/// <remarks>
			/// Calls API <msdn>GetFocus</msdn>.
			/// </remarks>
			public static Wnd Focused => Api.GetFocus();

			/// <summary>
			/// Returns true if w is the focused control or window of this thread.
			/// </summary>
			/// <remarks>
			/// Calls API <msdn>GetFocus</msdn>.
			/// </remarks>
			public static bool IsFocused(Wnd w) => !w.Is0 && w == Api.GetFocus();

			/// <summary>
			/// Gets the active window of this thread.
			/// Calls API <msdn>GetActiveWindow</msdn>.
			/// </summary>
			public static Wnd Active => Api.GetActiveWindow();
		}

		#endregion

		#region rect

		/// <summary>
		/// Gets rectangle (position and size) in screen coordinates.
		/// </summary>
		/// <param name="r">Receives the rectangle. Will be default(RECT) if failed.</param>
		/// <param name="withoutExtendedFrame">Don't include the transparent part of window border. For it is used API <msdn>DwmGetWindowAttribute</msdn>(DWMWA_EXTENDED_FRAME_BOUNDS); it is less reliable.</param>
		/// <remarks>
		/// The same as the <see cref="Rect"/> property.
		/// Calls API <msdn>GetWindowRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public bool GetRect(out RECT r, bool withoutExtendedFrame = false)
		{
			if(withoutExtendedFrame) {
				RECT t;
				if(0 == Api.DwmGetWindowAttribute(this, Api.DWMWA.EXTENDED_FRAME_BOUNDS, &t, 16)) {
					r = t;
					return true;
				}
			}

			if(Api.GetWindowRect(this, out r)) return true;
			r = default;
			return false;
		}

		/// <summary>
		/// Gets width and height.
		/// </summary>
		/// <param name="z">Receives width and height. Will be default(SIZE) if failed.</param>
		/// <remarks>
		/// The same as the <see cref="Size"/> property.
		/// Calls API <msdn>GetWindowRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public bool GetSize(out SIZE z)
		{
			if(Api.GetWindowRect(this, out RECT r)) { z = new SIZE(r.Width, r.Height); return true; }
			z = default;
			return false;
		}

		/// <summary>
		/// Gets rectangle (position and size) in screen coordinates.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetRect"/>. Returns default(RECT) if fails (eg window closed).
		/// </remarks>
		public RECT Rect
		{
			get
			{
				GetRect(out RECT r);
				return r;
			}
		}

		/// <summary>
		/// Gets width and height.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetSize"/>. Returns default(SIZE) if fails (eg window closed).
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public SIZE Size
		{
			get
			{
				GetSize(out SIZE z);
				return z;
			}
		}

		/// <summary>
		/// Gets horizontal position in screen coordinates.
		/// </summary>
		/// <remarks>Calls <see cref="GetRect"/>.</remarks>
		public int X
		{
			get => Rect.left;
		}

		/// <summary>
		/// Gets vertical position in screen coordinates.
		/// </summary>
		/// <remarks>Calls <see cref="GetRect"/>.</remarks>
		public int Y
		{
			get => Rect.top;
		}

		/// <summary>
		/// Gets width.
		/// </summary>
		/// <remarks>Calls <see cref="GetRect"/>.</remarks>
		public int Width
		{
			get => Rect.Width;
		}

		/// <summary>
		/// Gets height.
		/// </summary>
		/// <remarks>Calls <see cref="GetRect"/>.</remarks>
		public int Height
		{
			get => Rect.Height;
		}

		/// <summary>
		/// Gets client area rectangle.
		/// </summary>
		/// <param name="r">Receives the rectangle. Will be default(RECT) if failed.</param>
		/// <param name="inScreen">
		/// Get rectangle in screen coordinates; the same as <see cref="GetWindowAndClientRectInScreen"/>.
		/// If false (default), calls API <msdn>GetClientRect</msdn>; the same as <see cref="ClientRect"/> or <see cref="GetClientSize"/>.</param>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public bool GetClientRect(out RECT r, bool inScreen = false)
		{
			if(inScreen) return GetWindowAndClientRectInScreen(out _, out r);
			if(Api.GetClientRect(this, out r)) return true;
			r = default;
			return false;
		}

		/// <summary>
		/// Gets client area width and height.
		/// </summary>
		/// <param name="z">Receives width and height. Will be default(RECT) if failed.</param>
		/// <remarks>
		/// The same as the <see cref="ClientSize"/> property.
		/// The same as <see cref="GetClientRect"/>, just the parameter type is different.
		/// Calls API <msdn>GetClientRect</msdn> and returns its return value.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public bool GetClientSize(out SIZE z)
		{
			if(Api.GetClientRect(this, out RECT r)) { z = new SIZE(r.right, r.bottom); return true; }
			z = default;
			return false;
		}

		/// <summary>
		/// Gets client area rectangle (width and height).
		/// </summary>
		/// <remarks>
		/// The same as <see cref="ClientSize"/>, just the return type is different.
		/// The left and top fields are always 0. The right and bottom fields are the width and height of the client area.
		/// Calls <see cref="GetClientRect"/>. Returns default(RECT) if fails (eg window closed).
		/// </remarks>
		public RECT ClientRect
		{
			get
			{
				GetClientRect(out RECT r);
				return r;
			}
		}

		/// <summary>
		/// Gets client area rectangle (width and height) in screen.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetClientRect"/>. Returns default(RECT) if fails (eg window closed).
		/// </remarks>
		public RECT ClientRectInScreen
		{
			get
			{
				GetClientRect(out RECT r, true);
				return r;
			}
		}

		/// <summary>
		/// Gets client area width and height.
		/// </summary>
		/// <remarks>
		/// The same as <see cref="ClientRect"/>, just the return type is different.
		/// Calls <see cref="GetClientSize"/>. Returns default(SIZE) if fails (eg window closed).
		/// </remarks>
		public SIZE ClientSize
		{
			get
			{
				GetClientSize(out SIZE z);
				return z;
			}
		}

		/// <summary>
		/// Gets client area width.
		/// </summary>
		/// <remarks>Calls <see cref="GetClientSize"/>.</remarks>
		public int ClientWidth
		{
			get => ClientSize.width;
		}

		/// <summary>
		/// Gets client area height.
		/// </summary>
		/// <remarks>Calls <see cref="GetClientSize"/>.</remarks>
		public int ClientHeight
		{
			get => ClientSize.height;
		}

		/// <summary>
		/// Calculates and sets window rectangle from the specified client area rectangle.
		/// Calls <see cref="ResizeLL"/>.
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

			wi = default; wi.cbSize = Api.SizeOf(wi);
			return Api.GetWindowInfo(this, ref wi);
		}

		/// <summary>
		/// Gets window rectangle and client area rectangle, both in screen coordinates.
		/// </summary>
		/// <param name="rWindow">Receives window rectangle.</param>
		/// <param name="rClient">Receives client area rectangle.</param>
		/// <remarks>Calls API <msdn>GetWindowInfo</msdn>. Supports <see cref="Native.GetError"/>.</remarks>
		public bool GetWindowAndClientRectInScreen(out RECT rWindow, out RECT rClient)
		{
			if(LibGetWindowInfo(out var u)) {
				rWindow = u.rcWindow;
				rClient = u.rcClient;
				return true;
			}
			rWindow = default;
			rClient = default;
			return false;
		}

		//rejected: because now GetClientRect has 'inScreen' parameter.
		///// <summary>
		///// Gets client area rectangle in screen coordinates.
		///// </summary>
		///// <remarks>
		///// Calls <see cref="GetWindowAndClientRectInScreen"/>. Returns default(RECT) if fails (eg window closed).
		///// </remarks>
		//public RECT ClientRectInScreen
		//{
		//	get
		//	{
		//		GetWindowAndClientRectInScreen(out var rw, out var rc);
		//		return rc;
		//	}
		//}

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
		public bool MapClientToClientOf(Wnd w, ref POINT p)
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
		public bool MapClientToScreen(ref POINT p)
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
		public bool MapScreenToClient(ref POINT p)
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
		public bool MapClientToWindow(ref POINT p)
		{
			if(!GetWindowAndClientRectInScreen(out var rw, out var rc)) return false;
			p.x += rc.left - rw.left; p.y += rc.top - rw.top;
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
		public bool MapWindowToClient(ref POINT p)
		{
			if(!GetWindowAndClientRectInScreen(out var rw, out var rc)) return false;
			p.x += rw.left - rc.left; p.y += rw.top - rc.top;
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
		public bool MapWindowToScreen(ref POINT p)
		{
			if(!GetRect(out var rw)) return false;
			p.x += rw.left; p.y += rw.top;
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to screen coordinates.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool MapWindowToScreen(ref RECT r)
		{
			if(!GetRect(out var rw)) return false;
			r.Offset(rw.left, rw.top);
			return true;
		}

		/// <summary>
		/// Gets rectangle of this window (usually control) relative to the client area of another window (usually the parent).
		/// </summary>
		/// <param name="w">The returned rectangle will be relative to the client area of window w. If w is default(Wnd), gets rectangle in screen.</param>
		/// <param name="r">Receives the rectangle.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <seealso cref="RectInDirectParent"/>
		public bool GetRectInClientOf(Wnd w, out RECT r)
		{
			if(w.Is0) return GetRect(out r);
			return GetRect(out r) && w.MapScreenToClient(ref r);
		}

		/// <summary>
		/// Gets child window rectangle in the client area of the direct parent window.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetWnd.DirectParent"/> and <see cref="GetRectInClientOf"/>. Returns default(RECT) if fails (eg window closed).
		/// </remarks>
		public RECT RectInDirectParent => GetRectInClientOf(Get.DirectParent, out var r) ? r : default;

		/// <summary>
		/// Gets child window rectangle in the client area of the top-level parent window.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="Window"/> and <see cref="GetRectInClientOf"/>. Returns default(RECT) if fails (eg window closed).
		/// </remarks>
		public RECT RectInWindow => GetRectInClientOf(Window, out var r) ? r : default;

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
		public POINT MouseClientXY
		{
			get
			{
				Api.GetCursorPos(out var p);
				if(!MapScreenToClient(ref p)) p = default;
				return p;
			}
		}

		/// <summary>
		/// Returns true if this window (its rectangle) contains the specified point.
		/// </summary>
		/// <param name="x">X coordinate in screen. Not used if default(Coord).</param>
		/// <param name="y">Y coordinate in screen. Not used if default(Coord).</param>
		public bool ContainsScreenXY(Coord x, Coord y)
		{
			POINT p = Coord.Normalize(x, y);
			if(!GetRect(out RECT r)) return false;
			if(!r.Contains(x.IsEmpty ? r.left : p.x, y.IsEmpty ? r.top : p.y)) return false;
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
		/// <param name="x">X coordinate. Not used if default(Coord).</param>
		/// <param name="y">Y coordinate. Not used if default(Coord).</param>
		public bool ContainsWindowXY(Wnd parent, Coord x, Coord y)
		{
			if(!parent.IsAlive) return false;
			POINT p = Coord.NormalizeInWindow(x, y, parent);
			if(!GetRectInClientOf(parent, out RECT r)) return false;
			if(!r.Contains(x.IsEmpty ? r.left : p.x, y.IsEmpty ? r.top : p.y)) return false;
			return true;
		}

		/// <summary>
		/// This overload calls <see cref="ContainsWindowXY(Wnd, Coord, Coord)"/>(Window, x, y).
		/// </summary>
		public bool ContainsWindowXY(Coord x, Coord y)
		{
			return ContainsWindowXY(Window, x, y);
		}

		#endregion

		#region move, resize, SetWindowPos

		/// <summary>
		/// Calls API <msdn>SetWindowPos</msdn>.
		/// </summary>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>.
		/// For <paramref name="wndInsertAfter"/> you can use enum <b>Native.HWND</b> members: <b>TOP</b>, <b>BOTTOM</b>, <b>TOPMOST</b>, <b>NOTOPMOST</b>.
		/// </remarks>
		public bool SetWindowPos(Native.SWP swpFlags, int x = 0, int y = 0, int cx = 0, int cy = 0, Wnd wndInsertAfter = default)
		{
			return Api.SetWindowPos(this, wndInsertAfter, x, y, cx, cy, swpFlags);
		}

		/// <summary>
		/// Moves and resizes.
		/// </summary>
		/// <remarks>
		/// See also <see cref="Move(Coord, Coord, Coord, Coord, bool, Screen_)"/>. It is better to use in automation scripts, with windows of any process/thread. It throws exceptions, supports optional/reverse/fractional/workarea coordinates, restores if min/max, does not support SWP flags.
		/// This function is low-level, it just calls API <msdn>SetWindowPos</msdn> with flags NOZORDER|NOOWNERZORDER|NOACTIVATE|swpFlagsToAdd. It is better to use in programming, with windows of current thread.
		/// Supports <see cref="Native.GetError"/>.
		/// 
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// </remarks>
		/// <seealso cref="SetWindowPos"/>
		public bool MoveLL(int x, int y, int width, int height, Native.SWP swpFlagsToAdd = 0)
		{
			return SetWindowPos(Native.SWP.NOZORDER | Native.SWP.NOOWNERZORDER | Native.SWP.NOACTIVATE | swpFlagsToAdd, x, y, width, height);
		}

		/// <summary>
		/// Moves.
		/// </summary>
		/// <remarks>
		/// See also <see cref="Move(Coord, Coord, bool, Screen_)"/>. It is better to use in automation scripts, with windows of any process/thread. It throws exceptions, supports optional/reverse/fractional/workarea coordinates, restores if min/max.
		/// This function is low-level, it just calls API <msdn>SetWindowPos</msdn> with flags NOSIZE|NOZORDER|NOOWNERZORDER|NOACTIVATE. It is better to use in programming, with windows of current thread.
		/// Supports <see cref="Native.GetError"/>.
		/// 
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// </remarks>
		/// <seealso cref="SetWindowPos"/>
		public bool MoveLL(int x, int y)
		{
			return MoveLL(x, y, 0, 0, Native.SWP.NOSIZE);
		}

		/// <summary>
		/// Resizes.
		/// </summary>
		/// <remarks>
		/// See also <see cref="Resize(Coord, Coord, bool, Screen_)"/>. It is better to use in automation scripts, with windows of any process/thread. It throws exceptions, supports optional/reverse/fractional/workarea coordinates, restores if min/max.
		/// This function is low-level, it just calls API <msdn>SetWindowPos</msdn> with flags NOMOVE|NOZORDER|NOOWNERZORDER|NOACTIVATE. It is better to use in programming, with windows of current thread.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		/// <seealso cref="SetWindowPos"/>
		public bool ResizeLL(int width, int height)
		{
			return MoveLL(0, 0, width, height, Native.SWP.NOMOVE);
		}

		/// <summary>
		/// Moves and/or resizes.
		/// </summary>
		/// <param name="x">Left. If default(Coord), does not move in X axis.</param>
		/// <param name="y">Top. If default(Coord), does not move in Y axis.</param>
		/// <param name="width">Width. If default(Coord), does not change width.</param>
		/// <param name="height">Height. If default(Coord), does not change height.</param>
		/// <param name="workArea"><i>x y width height</i> are relative to the work area. Not used when this is a child window.</param>
		/// <param name="screen"><i>x y width height</i> are relative to this screen or its work area. Default - primary. Not used when this is a child window.</param>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized.
		/// For top-level windows use screen coordinates. For controls - direct parent client area coordinates.
		/// With windows of current thread usually it's better to use <see cref="MoveLL(int, int, int, int, Native.SWP)"/>.
		/// </remarks>
		/// <exception cref="WndException"/>
		public void Move(Coord x, Coord y, Coord width, Coord height, bool workArea = false, Screen_ screen = default)
		{
			ThrowIfInvalid();

			Wnd w = Get.DirectParent;
			POINT xy, wh;
			if(!w.Is0) {
				xy = Coord.NormalizeInWindow(x, y, w);
				wh = Coord.NormalizeInWindow(width, height, w);
			} else {
				xy = Coord.Normalize(x, y, workArea, screen);
				wh = Coord.Normalize(width, height, workArea, screen, widthHeight: true);
			}

			Native.SWP f = 0; uint getRect = 0;
			if(x.IsEmpty && y.IsEmpty) f |= Native.SWP.NOMOVE; else if(x.IsEmpty) getRect |= 1; else if(y.IsEmpty) getRect |= 2;
			if(width.IsEmpty && height.IsEmpty) f |= Native.SWP.NOSIZE; else if(width.IsEmpty) getRect |= 4; else if(height.IsEmpty) getRect |= 8;

			if(getRect != 0) {
				if(!GetRectInClientOf(w, out RECT r)) ThrowUseNative("*move/resize*");
				if((getRect & 1) != 0) xy.x = r.left;
				if((getRect & 2) != 0) xy.y = r.top;
				if((getRect & 4) != 0) wh.x = r.Width;
				if((getRect & 8) != 0) wh.y = r.Height;
			}

			//restore min/max, except if child or hidden
			if(w.Is0 && (IsMinimized || IsMaximized) && IsVisible) {
				ShowNotMinMax(true);
				//info: '&& IsVisible' because ShowNotMinMax unhides
			}

			if(!MoveLL(xy.x, xy.y, wh.x, wh.y, f)) ThrowUseNative("*move/resize*");

			LibMinimalSleepIfOtherThread();
		}

		/// <summary>
		/// Moves.
		/// </summary>
		/// <param name="x">Left. If default(Coord), does not move in X axis.</param>
		/// <param name="y">Top. If default(Coord), does not move in Y axis.</param>
		/// <param name="workArea"><i>x y</i> are relative to the work area. Not used when this is a child window.</param>
		/// <param name="screen"><i>x y</i> are relative to this screen or its work area. Default - primary. Not used when this is a child window.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized.
		/// For top-level windows use screen coordinates. For controls - direct parent client coordinates.
		/// With windows of current thread usually it's better to use <see cref="MoveLL(int, int)"/>.
		/// </remarks>
		public void Move(Coord x, Coord y, bool workArea = false, Screen_ screen = default)
		{
			Move(x, y, default, default, workArea, screen);
		}

		/// <summary>
		/// Resizes.
		/// </summary>
		/// <param name="width">Width. If default(Coord), does not change width.</param>
		/// <param name="height">Height. If default(Coord), does not change height.</param>
		/// <param name="workArea">For <see cref="Coord.Fraction"/> etc use width/height of the work area. Not used when this is a child window.</param>
		/// <param name="screen">For <b>Coord.Fraction</b> etc use width/height of this screen. Default - primary. Not used when this is a child window.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized.
		/// With windows of current thread usually it's better to use <see cref="ResizeLL(int, int)"/>.
		/// </remarks>
		public void Resize(Coord width, Coord height, bool workArea = false, Screen_ screen = default)
		{
			Move(default, default, width, height, workArea, screen);
		}

		#endregion

		#region MoveInScreen, EnsureInScreen, Screen

		internal static partial class Lib
		{
			/// <summary>
			/// Used directly by MoveInScreen, EnsureInScreen, RECT.MoveInScreen, RECT.EnsureInScreen. With inRect used by RECT.MoveInRect.
			/// </summary>
			internal static void MoveInScreen(bool bEnsureMethod,
			Coord left, Coord top, bool useWindow, Wnd w, ref RECT r,
			Screen_ screen, bool bWorkArea, bool bEnsureInScreen, RECT? inRect = default)
			{
				RECT rs;
				System.Windows.Forms.Screen scr;
				if(inRect.HasValue) {
					Debug.Assert(!useWindow);
					rs = inRect.GetValueOrDefault();
					scr = null;
				} else {
					if(!screen.IsNull) scr = screen.GetScreen();
					else if(useWindow) scr = Screen_.ScreenFromWindow(w);
					else if(bEnsureMethod) scr = System.Windows.Forms.Screen.FromRectangle(r);
					else scr = System.Windows.Forms.Screen.PrimaryScreen;

					rs = bWorkArea ? scr.WorkingArea : scr.Bounds;

					if(useWindow) {
						if(!w.GetRectNotMinMax(out r)) w.ThrowUseNative("*move*");
					}
				}

				int x, y, wid = r.Width, hei = r.Height;
				if(bEnsureMethod) {
					Debug.Assert(bEnsureInScreen == true && left.IsEmpty && top.IsEmpty); //left/top unused
					x = r.left;
					y = r.top;
				} else {
					if(left.IsEmpty) left = Coord.Center;
					if(top.IsEmpty) top = Coord.Center;
					var p = Coord.NormalizeInRect(left, top, rs);
					x = p.x; y = p.y;
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
					bool moveMaxWindowToOtherMonitor = wp.showCmd == Api.SW_SHOWMAXIMIZED && !scr.Equals(Screen_.ScreenFromWindow(w));
					if(r == wp.rcNormalPosition && !moveMaxWindowToOtherMonitor) return;

					Wnd hto = default; bool visible = w.IsVisible;
					try {
						//Windows bug: before a dialog is first time shown, may fail to move if it has an owner window. Depends on coordinates and on don't know what.
						//There are several workarounds. The best of them - temporarily set owner window 0.
						if(!visible) {
							hto = w.Owner;
							if(!hto.Is0) w.Owner = default;
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
						if(!hto.Is0) w.Owner = hto;
					}

					w.LibMinimalSleepIfOtherThread();
				}
			}
		}

		/// <summary>
		/// Moves this window to coordinates x y in specified screen, and ensures that entire window is in screen.
		/// </summary>
		/// <param name="x">X coordinate in the specified screen. If default(Coord) - screen center. You also can use <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="y">Y coordinate in the specified screen. If default(Coord) - screen center. You also can use <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="screen">Move to this screen (see <see cref="Screen_"/>). If default, uses screen of this window.</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <param name="ensureInScreen">If part of window is not in screen, move and/or resize it so that entire window would be in screen. Default true.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately, except when moving maximized to another screen.
		/// </remarks>
		/// <seealso cref="RECT.MoveInScreen"/>
		public void MoveInScreen(Coord x, Coord y, Screen_ screen = default, bool workArea = true, bool ensureInScreen = true)
		{
			RECT r = default;
			Lib.MoveInScreen(false, x, y, true, this, ref r, screen, workArea, ensureInScreen);
		}

		/// <summary>
		/// Moves this window if need, to ensure that entire window is in screen.
		/// </summary>
		/// <param name="screen">Move to this screen (see <see cref="Screen_"/>). If default, uses screen of this window.</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <exception cref="WndException"/>
		/// <remarks>
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately.
		/// </remarks>
		/// <seealso cref="RECT.EnsureInScreen"/>
		public void EnsureInScreen(Screen_ screen = default, bool workArea = true)
		{
			RECT r = default;
			Lib.MoveInScreen(true, default, default, true, this, ref r, screen, workArea, true);
		}

		/// <summary>
		/// Moves this window to the center of the screen.
		/// </summary>
		/// <param name="screen">Move to this screen (see <see cref="Screen_"/>). If default, uses screen of this window.</param>
		/// <exception cref="WndException"/>
		/// <remarks>Calls <c>ShowNotMinMax(true)</c> and <c>MoveInScreen(default, default, screen, true)</c>.</remarks>
		/// <seealso cref="RECT.MoveInScreen"/>
		public void MoveToScreenCenter(Screen_ screen = default)
		{
			ShowNotMinMax(true);
			MoveInScreen(default, default, screen, true);
		}

		/// <summary>
		/// Gets <see cref="System.Windows.Forms.Screen"/> object of the screen that contains this window (the biggest part of it) or is nearest to it.
		/// If this window handle is default(Wnd) or invalid, gets the primary screen.
		/// Calls <see cref="Screen_.ScreenFromWindow"/>.
		/// </summary>
		public System.Windows.Forms.Screen Screen
		{
			get => Screen_.ScreenFromWindow(this);
		}

		#endregion

		#region Zorder
		const Native.SWP _SWP_ZORDER = Native.SWP.NOMOVE | Native.SWP.NOSIZE | Native.SWP.NOACTIVATE;

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
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, before ? w.Get.Previous() : w);
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
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Native.HWND.TOP);

			//CONSIDER: find a workaround. Eg in some cases this works: set HWND_TOPMOST then HWND_NOTOPMOST.
		}
		//This version tries a workaround, but on latest Windows it does not work.
		//Better don't use workarounds here.
		//public bool ZorderTop()
		//{
		//	Wnd wa = Native.HWND.TOP;
		//	if(!IsChild) {
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
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Native.HWND.BOTTOM);
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
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Native.HWND.TOPMOST);
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
				if(!SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Native.HWND.NOTOPMOST)) return false;
				if(i == 0 && !IsTopmost) break;
				//Print("retry");
			}

			//place this after the active window
			if(afterActiveWindow) {
				Wnd wa = Active;
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
		public bool IsTopmost => HasExStyle(Native.WS_EX.TOPMOST);

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
		/// <value>One or more <see cref="Native.WS"/> flags and/or class-specific style flags. Reference: <msdn>window styles</msdn>.</value>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <seealso cref="HasStyle"/>
		/// <seealso cref="SetStyle"/>
		public Native.WS Style
		{
			get => (Native.WS)(uint)GetWindowLong(Native.GWL.STYLE);
		}

		/// <summary>
		/// Gets window extended style.
		/// </summary>
		/// <value>One or more <see cref="Native.WS_EX"/> flags. Reference: <msdn>extended window styles</msdn>.</value>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		/// <seealso cref="HasExStyle"/>
		/// <seealso cref="SetExStyle"/>
		public Native.WS_EX ExStyle
		{
			get => (Native.WS_EX)(uint)GetWindowLong(Native.GWL.EXSTYLE);
		}

		/// <summary>
		/// Returns true if the window has all specified style flags (see <see cref="Style"/>).
		/// </summary>
		/// <param name="style">One or more styles.</param>
		/// <param name="any">
		/// Return true if has any (not necessary all) of the specified styles.
		/// Note: don't use <see cref="Native.WS.CAPTION"/>, because it consists of two other styles - BORDER and DLGFRAME.
		/// </param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool HasStyle(Native.WS style, bool any = false)
		{
			var k = Style & style;
			return any ? k != 0 : k == style;
		}

		/// <summary>
		/// Returns true if the window has all specified extended style flags (see <see cref="ExStyle"/>).
		/// </summary>
		/// <param name="exStyle">One or more extended styles.</param>
		/// <param name="any">Return true if has any (not necessary all) of the specified styles.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool HasExStyle(Native.WS_EX exStyle, bool any = false)
		{
			var k = ExStyle & exStyle;
			return any ? k != 0 : k == exStyle;
		}

		/// <summary>
		/// Changes window style.
		/// </summary>
		/// <param name="style">One or more <see cref="Native.WS"/> flags and/or class-specific style flags. Reference: <msdn>window styles</msdn>.</param>
		/// <param name="how"></param>
		/// <param name="updateNC">Update non-client area (frame, title bar).</param>
		/// <param name="updateClient">Update client area.</param>
		/// <exception cref="WndException"/>
		/// <seealso cref="Style"/>
		public void SetStyle(Native.WS style, SetAddRemove how = SetAddRemove.Set, bool updateNC = false, bool updateClient = false)
		{
			_SetStyle((int)style, how, false, updateNC, updateClient);
		}

		/// <summary>
		/// Changes window extended style.
		/// </summary>
		/// <param name="style">One or more <see cref="Native.WS_EX"/> flags. Reference: <msdn>extended window styles</msdn>.</param>
		/// <param name="how"></param>
		/// <param name="updateNC">Update non-client area (frame, title bar).</param>
		/// <param name="updateClient">Update client area.</param>
		/// <exception cref="WndException"/>
		/// <seealso cref="ExStyle"/>
		public void SetExStyle(Native.WS_EX style, SetAddRemove how = SetAddRemove.Set, bool updateNC = false, bool updateClient = false)
		{
			_SetStyle((int)style, how, true, updateNC, updateClient);
		}

		void _SetStyle(int style, SetAddRemove how, bool exStyle = false, bool updateNC = false, bool updateClient = false)
		{
			var gwl = exStyle ? Native.GWL.EXSTYLE : Native.GWL.STYLE;
			if(how != SetAddRemove.Set) {
				int pstyle = (int)GetWindowLong(gwl);
				if(how == SetAddRemove.Add) style |= pstyle;
				else if(how == SetAddRemove.Remove) style = pstyle & ~style;
				else if(how == SetAddRemove.Xor) style = pstyle ^ style;
			}

			SetWindowLong(gwl, style);

			if(updateNC) SetWindowPos(Native.SWP.FRAMECHANGED | Native.SWP.NOMOVE | Native.SWP.NOSIZE | Native.SWP.NOZORDER | Native.SWP.NOOWNERZORDER | Native.SWP.NOACTIVATE);
			if(updateClient) Api.InvalidateRect(this, default, true);
		}

		/// <summary>
		/// Returns true if has Native.WS.POPUP style.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsPopupWindow => HasStyle(Native.WS.POPUP);

		/// <summary>
		/// Returns true if has Native.WS_EX.TOOLWINDOW style.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsToolWindow => HasExStyle(Native.WS_EX.TOOLWINDOW);

		/// <summary>
		/// Returns true if has Native.WS.THICKFRAME style.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsResizable => HasStyle(Native.WS.THICKFRAME);

		#endregion

		#region window/class long, control id, prop

		/// <summary>
		/// Calls API <msdn>GetWindowLong</msdn> if this process is 32-bit, <msdn>GetWindowLongPtr</msdn> if 64-bit.
		/// </summary>
		/// <remarks>
		/// For index can be used constants from <see cref="Native.GWL"/>.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public LPARAM GetWindowLong(int index)
		{
			LPARAM R;
			if(IntPtr.Size == 8) R = Api.GetWindowLong64(this, index); else R = Api.GetWindowLong32(this, index);
			return R;
		}

		/// <summary>
		/// Calls API <msdn>SetWindowLong</msdn> if this process is 32-bit, <msdn>SetWindowLongPtr</msdn> if 64-bit.
		/// </summary>
		/// <remarks>
		/// For index can be used constants from <see cref="Native.GWL"/>.
		/// </remarks>
		/// <exception cref="WndException"/>
		public LPARAM SetWindowLong(int index, LPARAM newValue)
		{
			Native.ClearError();
			LPARAM R;
			if(IntPtr.Size == 8) R = Api.SetWindowLong64(this, index, newValue); else R = Api.SetWindowLong32(this, index, (int)newValue);
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
			set { SetWindowLong(Native.GWL.ID, value); }
		}

		/// <summary>
		/// Returns an object that manages window properties using API <msdn>SetProp</msdn> and co.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// var w = Wnd.Find("* Explorer");
		/// w.Prop.Set("example", 5);
		/// Print(w.Prop["example"]);
		/// Print(w.Prop); //shows all w properties
		/// w.Prop.Remove("example"); //you should always remove window properties if don't want to see unrelated applications crashing after some time. And don't use many unique property names.
		/// ]]></code>
		/// </example>
		public WProp Prop => new WProp(this);

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
		/// Gets native thread id of this window. Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// Returns 0 if fails. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <remarks>
		/// It is not the same as <see cref="Thread.ManagedThreadId"/>.
		/// </remarks>
		public int ThreadId => GetThreadProcessId(out var pid);

		/// <summary>
		/// Gets native process id of this window. Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// Returns 0 if fails. Supports <see cref="Native.GetError"/>.
		/// </summary>
		public int ProcessId { get { GetThreadProcessId(out var pid); return pid; } }

		/// <summary>
		/// Returns true if this window belongs to the current thread, false if to another thread.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// </summary>
		public bool IsOfThisThread => Api.GetCurrentThreadId() == ThreadId;

		/// <summary>
		/// Returns true if this window belongs to the current process, false if to another process.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>GetWindowThreadProcessId</msdn>.
		/// </summary>
		public bool IsOfThisProcess => Api.GetCurrentProcessId() == ProcessId;

		/// <summary>
		/// Returns true if the window is a Unicode window, false if ANSI.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>IsWindowUnicode</msdn>.
		/// </summary>
		public bool IsUnicode => Api.IsWindowUnicode(this);

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
					using(var ph = Util.LibKernelHandle.OpenProcess(this)) {
						if(ph.Is0 || !Api.IsWow64Process(ph, out var is32bit)) return false;
						if(!is32bit) return true;
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
		public bool IsHung => Api.IsHungAppWindow(this);

		/// <summary>
		/// Returns true if the window is a ghost window that the system creates over a hung (not responding) window to allow the user to minimally interact with it.
		/// </summary>
		public bool IsHungGhost
		{
			get => IsHung && ClassNameIs("Ghost.exe") && ProgramName.Equals_("DWM.exe", true);
			//Class is "Ghost", exe is "DWM" (even if no Aero), text sometimes ends with "(Not Responding)".
			//IsHungWindow returns true for ghost window, although it is not actually hung. It is the fastest.
		}

		/// <summary>
		/// Returns true if this is a console window (class name "ConsoleWindowClass").
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsConsole => ClassNameIs("ConsoleWindowClass");

		internal void LibUacCheckAndThrow(string prefix = null)
		{
			if(!Is0 && IsUacAccessDenied) {
				if(prefix == null) prefix = "Failed. The"; else if(prefix.EndsWith_(".")) prefix += " The"; //this is to support prefix used by Mouse.Move: "The active"
				throw new AuException(Api.ERROR_ACCESS_DENIED, prefix + " window's process has a higher UAC integrity level (admin or uiAccess) than this process.");
			}
		}

		/// <summary>
		/// Returns true if <see cref="Process_.UacInfo">UAC</see> would not allow to automate the window.
		/// It happens when current process has lower UAC integrity level and is not uiAccess, unless UAC is turned off.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsUacAccessDenied
		{
			get
			{
				Native.ClearError();
				Api.RemoveProp(this, 0);
				return Native.GetError() == Api.ERROR_ACCESS_DENIED; //documented
			}
		}

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
				return Util.StringCache.LibAdd(b, n);
			}
		}

		/// <summary>
		/// Returns true if the class name of this window matches className. Else returns false.
		/// Also returns false when fails (probably window closed or 0 handle). Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="className">Class name. Case-insensitive <see cref="String_.Like_(string, string, bool)">wildcard</see>. Cannot be null.</param>
		public bool ClassNameIs(string className) => ClassName.Like_(className, true);

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
		/// Gets name.
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
		public string Name => GetText(false, true);

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
		public string ControlText => GetText(true, false);

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
		/// Removes only if this is a control (has style Native.WS.CHILD).
		/// Calls <see cref="Util.StringMisc.RemoveUnderlineAmpersand"/>.
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
				&& HasStyle(Native.WS.CHILD)
				) R = Util.StringMisc.RemoveUnderlineAmpersand(R);

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
					if(nr > 0) return Util.StringCache.LibAdd(b, nr);
					if(Native.GetError() != 0) return null;
					if(useSlowIfEmpty && HasStyle(Native.WS.CHILD)) return _GetTextSlow();
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
			int n = (int)ln; if(n < 1) return "";

			var b = Util.Buffers.LibChar(n);
			fixed (char* p = b.A) {
				if(!SendTimeout(30000, out ln, Api.WM_GETTEXT, n + 1, p)) return null;
				if(ln < 1) return "";
				b.A[n] = '\0';
				n = Util.LibCharPtr.Length(p, n); //info: some controls return incorrect ln, eg including '\0'
				return Util.StringCache.LibAdd(b, n);
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

		//rejected: faster but not better than NameAcc. Or must be improved.
		//	Instead, let Child find label control, and then navigate with Get.Right() etc.
		///// <summary>
		///// Gets <see cref="Name"/> of previous (in Z order) sibling control.
		///// Returns null if there is no such control.
		///// </summary>
		///// <remarks>
		///// Can be used to identify controls that have no name but have a named control (label) at the left or above. For example Edit And ComboBox controls in dialogs.
		///// In such cases this function works like <see cref="NameAcc"/>, but is faster and supports any sibling control type.
		///// </remarks>
		//public string NameLabel
		//{
		//	get
		//	{
		//		for(var w = this; ;) {
		//			var p = w.Get.DirectParent;
		//			if(p.Is0) return null;
		//			w = w.Get.Previous(); if(!w.Is0) return w.Name;
		//			w = p;
		//		}
		//		//_todo: if the label control is not at the left/above, find topmost control at the left/above
		//	}
		//}
#if false //artifacts of xml doc from various places. Maybe it will come back some day. The code is commented out there.
		//from Wnd.Child comments:
		/// <item>
		/// "***label " - use <see cref="NameLabel"/>.
		/// Useful when the control itself does not have a name but an adjacent control is used as its name. Examples - Edit controls in dialogs.
		/// </item>
#endif

		/// <summary>
		/// Gets <see cref="Acc.Name"/> of the accessible object (role WINDOW) of this window or control.
		/// Returns "" if the object has no name or failed to get it. Returns null if invalid window handle.
		/// </summary>
		public string NameAcc => Acc.LibNameOfWindow(this);

		/// <summary>
		/// Gets Control.Name property of a .NET Windows Forms control.
		/// Returns null if it is not a Windows Forms control or if fails.
		/// <note>Use this with controls of other processes. Don't use with your controls, when you have a Control object.</note>
		/// <note>This is slow when getting names of multiple controls in a window. Instead create a <see cref="Misc.WinFormsControlNames"/> instance and call its <see cref="Misc.WinFormsControlNames.GetControlName"/> method for each control.</note>
		/// </summary>
		/// <seealso cref="Misc.WinFormsControlNames.IsWinFormsControl"/>
		public string NameWinForms => Misc.WinFormsControlNames.GetSingleControlName(this);

		/// <summary>
		/// Gets filename of process executable file, like "notepad.exe".
		/// Return null if fails.
		/// Calls <see cref="ProcessId"/> and <see cref="Process_.GetName"/>.
		/// </summary>
		public string ProgramName => Process_.GetName(ProcessId);

		/// <summary>
		/// Gets full path of process executable file.
		/// Return null if fails.
		/// Calls <see cref="ProcessId"/> and <see cref="Process_.GetName"/>.
		/// </summary>
		public string ProgramFilePath => Process_.GetName(ProcessId, true);

		/// <summary>
		/// Gets description of process executable file.
		/// Return null if fails.
		/// Calls <see cref="ProcessId"/> and <see cref="Process_.GetDescription"/>.
		/// </summary>
		public string ProgramDescription => Process_.GetDescription(ProcessId);

		#endregion

		#region close, destroy

		/// <summary>
		/// Closes the window.
		/// Returns true if successfuly closed or if it was already closed (the handle is 0 or invalid) or if <paramref name="noWait"/>==true.
		/// </summary>
		/// <param name="noWait">
		/// If true, does not wait until the window is closed.
		/// If false, waits about 1 s (depends on window type etc) until the window is destroyed or disabled.
		/// </param>
		/// <param name="useXButton">
		/// If false (default), uses API message <msdn>WM_CLOSE</msdn>.
		/// If true, uses API message <msdn>WM_SYSCOMMAND SC_CLOSE</msdn>, like when the user clicks the X button in the title bar.
		/// Most windows can be closed with any of these messages, but some respond properly only to one of them. For example, some applications on WM_CLOSE don't exit, although the main window is closed. Some applications don't respond to WM_SYSCOMMAND if it is posted soon after opening the window, for example Internet Explorer.
		/// </param>
		/// <remarks>
		/// The window may refuse to be closed. For example, it may be hung, or hide itself instead, or display a "Save?" message box, or is a dialog without X button, or just need more time to close it.
		/// If the window is of this thread, just calls <see cref="Send"/> or <see cref="Post"/> (if <paramref name="noWait"/>==true) and returns true.
		/// </remarks>
		/// <seealso cref="WaitForClosed"/>
		/// <example>
		/// <code><![CDATA[
		/// //close all Notepad windows
		/// Wnd.FindAll("* Notepad", "Notepad").ForEach(t => t.Close());
		/// ]]></code>
		/// </example>
		public bool Close(bool noWait = false, bool useXButton = false)
		{
			if(!IsAlive) return true;

			int msg = Api.WM_CLOSE; uint wparam = 0; if(useXButton) { msg = Api.WM_SYSCOMMAND; wparam = Api.SC_CLOSE; }

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

			//if(!noWait.HasValue) noWait = Thread_.IsUI; //rejected
			if(noWait) return true;

			if(ok) {
				for(int i = 0; i < 100; i++) {
					Thread.Sleep(15);
					if(!IsEnabled) break; //destroyed or has an owned modal dialog box, eg "Save?"

					//Wait less if hidden, eg has a tray icon.
					//Also if a popup, eg a Yes/No message box (disabled X button).
					//Also if child.
					//if(!IsVisible && !_IsBusy(2)) i += 4; //unreliable, too dirty
					if(!IsVisible || (Style & (Native.WS.POPUP | Native.WS.CHILD)) != 0) i += 2;

					if(i >= 50) {
						if(!SendTimeout(200, 0)) {
							if(!IsAlive || IsHung) break;
						}
					}
				}
			}
			LibMinimalSleepNoCheckThread();
			Misc.WaitForAnActiveWindow();

			return !IsAlive;
		}

		//bool _IsBusy(int milliseconds)
		//{
		//	//Need to measure time. Cannot use just 2 ms timeout and ST return value because of the system timer default period 15.6 ms etc.
		//	var t = Time.Microseconds;
		//	SendTimeout(5 + milliseconds, 0, flags: 0);
		//	var d = Time.Microseconds - t;
		//	//Print(d);
		//	return (d >= milliseconds * 1000L);
		//}

		//Rarely used. It is easy, and there is example in Close() help: Wnd.FindAll("* Notepad", "Notepad").ForEach(t => t.Close());
		///// <summary>
		///// Closes all matching windows.
		///// Calls <see cref="FindAll"/>. All parameters etc are the same. Then calls <see cref="Close"/> for each found window.
		///// Returns the number of found windows.
		///// </summary>
		//public static int CloseAll(
		//	string name, string className = null, WFEtc programEtc = default,
		//	WFFlags flags = 0, Func<Wnd, bool> f = null, object contains = null
		//	)
		//{
		//	var a = FindAll(name, className, programEtc, flags, f, contains);
		//	foreach(Wnd w in a) w.Close();
		//	return a.Count;
		//}

		#endregion

	}

}

namespace Au.Types
{
	/// <summary>
	/// Sets, gets, removes and lists window properties using API <msdn>SetProp</msdn> and co.
	/// </summary>
	public struct WProp
	{
		Wnd _w;

		internal WProp(Wnd w) => _w = w;

		/// <summary>
		/// Gets a window property.
		/// Calls API <msdn>GetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM this[string name] => Api.GetProp(_w, name);

		/// <summary>
		/// Gets a window property.
		/// Calls API <msdn>GetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		/// <remarks>
		/// This overload uses atom instead of string. I's about 3 times faster. See API <msdn>GlobalAddAtom</msdn>, <msdn>GlobalDeleteAtom</msdn>.
		/// </remarks>
		public LPARAM this[ushort atom] => Api.GetProp(_w, atom);

		/// <summary>
		/// Sets a window property.
		/// Calls API <msdn>SetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Property value.</param>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>.
		/// 
		/// Later call <see cref="Remove(string)"/> to remove the property. If you use many unique property names and don't remove the properties, the property name strings can fill the global atom table which is of a fixed size (about 48000) and which is used by all processes for various purposes.
		/// </remarks>
		public bool Set(string name, LPARAM value)
		{
			return Api.SetProp(_w, name, value);
		}

		/// <summary>
		/// Sets a window property.
		/// Calls API <msdn>SetProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		/// <param name="value">Property value.</param>
		/// <remarks>
		/// This overload uses atom instead of string. I's about 3 times faster. See API <msdn>GlobalAddAtom</msdn>, <msdn>GlobalDeleteAtom</msdn>.
		/// </remarks>
		public bool Set(ushort atom, LPARAM value)
		{
			return Api.SetProp(_w, atom, value);
		}

		/// <summary>
		/// Removes a window property.
		/// Calls API <msdn>RemoveProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public LPARAM Remove(string name)
		{
			return Api.RemoveProp(_w, name);
		}

		/// <summary>
		/// Removes a window property.
		/// Calls API <msdn>RemoveProp</msdn> and returns its return value.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		public LPARAM Remove(ushort atom)
		{
			return Api.RemoveProp(_w, atom);
		}

		/// <summary>
		/// Gets list of window properties.
		/// Uses API <msdn>EnumPropsEx</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns 0-length list if fails. Fails if invalid window or access denied (<see cref="Process_.UacInfo">UAC</see>). Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public Dictionary<string, LPARAM> GetList()
		{
			var a = new Dictionary<string, LPARAM>();
			Api.EnumPropsEx(_w, (w, name, data, p) =>
			{
				string s;
				if((long)name < 0x10000) s = "#" + (int)name; else s = Marshal.PtrToStringUni(name);
				a.Add(s, data);
				return true;
			}, default);
			return a;
		}

		/// <summary>
		/// Calls <see cref="GetList"/> and converts to string.
		/// </summary>
		public override string ToString()
		{
			return string.Join("\r\n", GetList());
		}
	}
}
