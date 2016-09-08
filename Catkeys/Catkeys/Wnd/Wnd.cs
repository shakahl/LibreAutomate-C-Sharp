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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using static System.Math;

using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	/// <summary>
	/// Contains native window handle (HWND in C/C++, IntPtr in C#).
	/// Has many member functions to manipulate the window, get properties etc. Also has static member functions, mostly to find/get windows.
	/// </summary>
	/// <remarks>
	/// What caused the decision to make it a struct, not a class:
	///		Advantages:
	///			1. Lightweight. Contains just IntPtr, which is 4 or 8 bytes.
	///			2. Easier to create overloaded functions that have a window parameter. If it was a class, then a null argument could be ambiguous if eg could also be a string etc.
	///			3. When a find-window function does not find the specified window, calling next function (without checking the return value) does not throw null-reference exception. Instead the function can throw a more specific exception or just return false etc.
	///			4. Anyway, the handle actually already is a reference (to a window object managed by the OS). We don't own the object; we usually don't need to destroy the window finally; it is more like a numeric window id.
	///			5. Probably it is not a 'bad practice' to have a struct with many member functions, because eg the .NET DateTime is a struct.
	///			6. Code where a window argument is Wnd0 is more clear. If if it would be null, then it is unclear how the function interprets it: as a 0 handle or as "don't use it". Now if we want a "don't use it" behavior, we'll use an overload.
	///			7. In my experience, it makes programming/scripting easier that if it would be a class. Because windows are not found so often (in automation scripts). A find-window function could throw a 'not found' exception, but it is not good (it's easier to check the return value than to use try/catch or throwing/nonthrowing overloads).
	///		Disadvantages:
	///			1. Cannot be a base class of other classes. Workaround: Use it as a public field or property of the other class (or struct); in some cases it can be even better, because Wnd has very many methods, and the non-inherited methods of that class would be difficult to find; now they are separated, and can be used like x.NewClassMethod() and x.w.WndMethod(); anyway, in most cases we'll need the the new window classes only for the functions that they add, not for Wnd functions, eg we would use a class ButtonWnd mostly only for button functions, not for general window functions.
	///			2. In some cases C# compiler does not allow to call a property-set function:
	///				When a property-get function (or a method) returns a Wnd, the C# compiler does not allow to set properties in the same statement, like w.Owner.Enabled=false. Workaround: Wnd t=w.Owner; t.Enabled=false. Another workaround: if the property would get a field of a very simple struct, make that field public instaed of using a property.
	///				When the variable is a foreach variable. Workaround: foreach(Wnd c in ...) { Wnd _c=c; _c.Enabled=flse; }
	/// What happens when a Wnd function fails:
	///		Some Wnd functions throw exceptions when failed, and the exceptions are listed in function help.
	///		Many other Wnd functions instead use ThreadError class, and it is documented in function help.
	///		When a Wnd function that supports ThreadError returns something that does not indicate success (eg 0 or false), you can call ThreadError.IsError or other ThreadError functions to get error info.
	/// Almost all Wnd functions can be used with windows of any process/thread.
	///		Some of them work slightly differently with windows of current thread.
	///		There are several functions that work only with windows of current thread, and it is documented in function help.
	/// </remarks>
	[Serializable]
	public partial struct Wnd :IWin32Window
	{
		IntPtr _h;

		#region constructors, operators, overrides, constants

		Wnd(IntPtr hwnd) { _h = hwnd; } //don't need public

		public static explicit operator Wnd(IntPtr hwnd) { return new Wnd(hwnd); } //Wnd=(Wnd)IntPtr //don't need implicit, it creates more problems than is useful
		public static explicit operator IntPtr(Wnd w) { return w._h; } //IntPtr=(IntPtr)Wnd //could be implicit, but then problems with operator ==
		public static explicit operator Wnd(LPARAM hwnd) { return new Wnd(hwnd); } //Wnd=(Wnd)LPARAM
		public static explicit operator LPARAM(Wnd w) { return w._h; } //LPARAM=(LPARAM)Wnd
		public static implicit operator Wnd(Misc.SpecHwnd value) { return new Wnd((IntPtr)value); } //Wnd=Wnd.SpecHwnd
		public static explicit operator Wnd(int hwnd) { return new Wnd((IntPtr)hwnd); } //Wnd=(Wnd)int
		public static explicit operator Wnd(Control c) { return new Wnd(c == null ? Zero : c.Handle); } //Wnd=Control //implicit would allow Wnd==null
		public static explicit operator Control(Wnd w) { return Control.FromHandle(w._h); } //Control=(Control)Wnd //works only if it is a Control handle, null if other handle

		//public Wnd(IWin32Window hwnd) { _h=hwnd==null ? Zero : hwnd.Handle; } //ok, but not useful when we cannot have the cast operator
		//public Wnd(string name) { _h=Find(name); } //if need class etc, use Find() instead. Not useful if we don't use the cast operator.
		//public static implicit operator Wnd(IWin32Window hwnd) { return new Wnd(hwnd); } //Wnd=IWin32Window //error: user-defined conversions to or from an interface are not allowed
		//public static implicit operator Wnd(string name) { OutFunc(); return new Wnd(name); } //Wnd=string. DON'T. In some cases compiler would pick wrong overload, creating strange bugs.

		public static bool operator ==(Wnd w1, Wnd w2) { return w1._h == w2._h; }
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

		public bool Equals(Wnd w)
		{
			return w._h == _h;
		}

		/// <summary>
		/// Returns true if w!=null and w.Value == this.
		/// </summary>
		public bool Equals(Wnd? w)
		{
			return w != null && w.Value == this;
		}

		public override bool Equals(object obj)
		{
			return obj is Wnd && this == (Wnd)obj;
		}

		public override int GetHashCode()
		{
			return (int)_h; //window handles are always 32-bit int, although in a 64-bit process stored in 64-bit variables
			//return _h.GetHashCode();
		}

		/// <summary>
		/// Formats string $"{handle} {ClassName} \"{Name}\"".
		/// </summary>
		public override string ToString()
		{
			if(Is0) return "0";
			var cn = ClassName;
			if(cn == null) return $"{_h} <invalid handle>";
			string s = Name; if(s != null) s = s.Limit_(250);
			return $"{_h} {cn} \"{s}\"";
		}

		/// <summary>
		/// Implements IWin32Window.
		/// </summary>
		public IntPtr Handle { get { return _h; } }

		//not useful. Required eg for SortedList, but better use Dictionary. We'll never need a sorted list of window handles.
		///// <summary>
		///// Implements IComparable<Wnd>.
		///// </summary>
		//public int CompareTo(Wnd other)
		//{
		//	return (int)_h-(int)other._h;
		//}

		#endregion

		#region send/post message

		static partial class _Api
		{
			[DllImport("user32.dll", SetLastError = true)]
			public static extern LPARAM SendMessage(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam);

			[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
			public static extern LPARAM SendMessageS(Wnd hWnd, uint msg, LPARAM wParam, string lParam);

			[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
			public static extern LPARAM SendMessageSB(Wnd hWnd, uint msg, LPARAM wParam, [Out] StringBuilder lParam);

			[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
			public static extern LPARAM SendMessageTimeout(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

			[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
			public static extern LPARAM SendMessageTimeoutS(Wnd hWnd, uint Msg, LPARAM wParam, string lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

			[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
			public static extern LPARAM SendMessageTimeoutSB(Wnd hWnd, uint Msg, LPARAM wParam, [Out] StringBuilder lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

			[DllImport("user32.dll", EntryPoint = "SendNotifyMessageW", SetLastError = true)]
			public static extern bool SendNotifyMessage(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);

			[DllImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
			public static extern bool PostMessage(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);
		}

		/// <summary>
		/// Calls API SendMessage().
		/// </summary>
		public LPARAM Send(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
		{
			return _Api.SendMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API SendMessage() where lParam is string.
		/// </summary>
		public LPARAM SendS(uint message, LPARAM wParam, string lParam)
		{
			return _Api.SendMessageS(this, message, wParam, lParam);
			//info: don't use overload, then eg ambiguous if null.
		}

		/// <summary>
		/// Calls API SendMessage() where lParam is StringBuilder.
		/// </summary>
		public LPARAM SendSB(uint message, LPARAM wParam, StringBuilder lParam)
		{
			return _Api.SendMessageSB(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API SendMessageTimeout().
		/// Supports ThreadError.
		/// </summary>
		public bool SendTimeout(int timeoutMS, out LPARAM result, uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM), uint smtoFlags = Api.SMTO_ABORTIFHUNG)
		{
			result = 0;
			return 0 != _Api.SendMessageTimeout(this, message, wParam, lParam, smtoFlags, (uint)timeoutMS, out result) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Calls API SendMessageTimeout().
		/// Use this overload when you don't need the return value.
		/// Supports ThreadError.
		/// </summary>
		public bool SendTimeout(int timeoutMS, uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM), uint smtoFlags = Api.SMTO_ABORTIFHUNG)
		{
			LPARAM result;
			return SendTimeout(timeoutMS, out result, message, wParam, lParam, smtoFlags);
		}

		/// <summary>
		/// Calls API SendMessageTimeout() where lParam is string.
		/// Supports ThreadError.
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, out LPARAM result, uint message, LPARAM wParam, string lParam, uint smtoFlags = Api.SMTO_ABORTIFHUNG)
		{
			result = 0;
			return 0 != _Api.SendMessageTimeoutS(this, message, wParam, lParam, smtoFlags, (uint)timeoutMS, out result) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Calls API SendMessageTimeout() where lParam is string.
		/// Use this overload when you don't need the return value.
		/// Supports ThreadError.
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, uint message, LPARAM wParam, string lParam, uint smtoFlags = Api.SMTO_ABORTIFHUNG)
		{
			LPARAM result;
			return SendTimeoutS(timeoutMS, out result, message, wParam, lParam, smtoFlags);
		}

		/// <summary>
		/// Calls API SendMessageTimeout() where lParam is StringBuilder.
		/// Supports ThreadError.
		/// </summary>
		public bool SendTimeoutSB(int timeoutMS, out LPARAM result, uint message, LPARAM wParam, StringBuilder lParam, uint smtoFlags = Api.SMTO_ABORTIFHUNG)
		{
			result = 0;
			return 0 != _Api.SendMessageTimeoutSB(this, message, wParam, lParam, smtoFlags, (uint)timeoutMS, out result) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Calls API SendNotifyMessage().
		/// </summary>
		public bool SendNotify(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
		{
			return _Api.SendNotifyMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API PostMessage().
		/// </summary>
		public bool Post(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
		{
			return _Api.PostMessage(this, message, wParam, lParam);
		}
		#endregion

		#region valid, visible, enabled, cloaked

		/// <summary>
		/// Returns true if the window handle is 0.
		/// Alternatively use operator ==, like <c>if(w==Wnd0)</c>
		/// </summary>
		public bool Is0 { get { return _h == Zero; } }

		/// <summary>
		/// Returns true if the window exists.
		/// Returns false if the window is closed or the handle is 0 or some other invalid or special value.
		/// Calls Is0 and Api.IsWindow().
		/// </summary>
		public bool IsValid { get { return !Is0 && Api.IsWindow(this); } }

		const string _errStr_Handle0 = "Window handle is 0. Possibly previous 'find window' function did not find a window.";
		const string _errStr_HandleInvalid = "Invalid window handle. Possibly the window is closed/destroyed.";

		/// <summary>
		/// Throws exception if this.Is0==true or this.IsValid==false.
		/// </summary>
		public void ValidateThrow()
		{
			if(Is0) throw new CatException(_errStr_Handle0);
			if(!IsValid) throw new CatException(_errStr_HandleInvalid);
		}

		/// <summary>
		/// If this.Is0==true or this.IsValid==false, sets thread error or throws exception, depending on the 'throwException' argument.
		/// </summary>
		public bool Validate(bool throwException)
		{
			if(Is0) {
				if(throwException) throw new CatException(_errStr_Handle0);
				return ThreadError.Set(_errStr_Handle0);
			}
			if(!IsValid) {
				if(throwException) throw new CatException(_errStr_HandleInvalid);
				return ThreadError.Set(_errStr_HandleInvalid);
			}
			return true;
		}

		/// <summary>
		/// Gets or sets the visible state.
		/// <para>
		/// The 'get' function calls Api.IsWindowVisible().
		/// Returns true if the window is visible. Returns false if is invisible or is a child of invisible parent or when fails (eg window closed).
		/// </para>
		/// <para>
		/// The 'set' function calls Api.ShowWindow(). Does not activate/deactivate/zorder. Supports ThreadError.
		/// </para>
		/// </summary>
		/// <seealso cref="Activate"/>
		/// <seealso cref="MinimizeAndHide"/>
		public bool Visible
		{
			get { return Api.IsWindowVisible(this); } //note: ThreadError here would not be very useful, better let it be as fast as possible.
			set { _Show(value); }
		}

		/// <summary>
		/// Shows this window if not visible.
		/// Does not activate/deactivate/zorder.
		/// Calls Api.ShowWindow(this, Api.SW_SHOWNA/SW_HIDE).
		/// Supports ThreadError.
		/// </summary>
		internal bool _Show(bool show)
		{
			if(show && Visible) return true; //note: don't use this when !show, because then would not hide if the parent window is currently hidden.
			return Api.ShowWindow(this, show ? Api.SW_SHOWNA : Api.SW_HIDE) || ThreadError.SetWinError();

			//speed when the window already is in the requested state:
			//	ShowWindow and SetWindowPos don't test it.
			//	ShowWindow(SW_SHOW/SW_HIDE) are quite fast (maybe 5 times slower than IsWindowVisible).
			//	Unfortunately SW_SHOW activates the window if it is top-level (depends on the foreground lock).
			//	SW_SHOWNA is MUCH slower.
			//	SetWindowPos is slightly faster than SW_SHOWNA. With SWP_NOSENDCHANGING faster, especially with windows of other threads, but not much.
			//speed when the window is not in the requested state:
			//	All similar.
		}

		/// <summary>
		/// Gets or sets the enabled state.
		/// <para>
		/// The 'get' function calls Api.IsWindowEnabled().
		/// Returns true if the window is enabled. Returns false if is disabled or is a child of disabled parent or when fails (eg window closed).
		/// </para>
		/// <para>
		/// The 'set' function enables or disables the window.
		/// Calls Api.EnableWindow().
		/// </para>
		/// </summary>
		public bool Enabled
		{
			get { return Api.IsWindowEnabled(this); }
			set { Api.EnableWindow(this, value); }
		}

		/// <summary>
		/// Gets the cloaked state.
		/// Returns 0 if not cloaked or if failed.
		/// Else returns flags: 1 cloaked by its application, 2 cloaked by Windows, 4 cloaked because its owner window is cloaked.
		/// On Windows 7 returns 0 because there is no "cloaked window" feature.
		/// </summary>
		public int IsCloakedGetState
		{
			get
			{
				if(WinVer < Win8) return 0;
				int cloaked = 0, hr = Api.DwmGetWindowAttribute(this, 14, out cloaked, 4); //DWMWA_CLOAKED
				return cloaked;
			}
		}
		/// <summary>
		/// Returns true if the window is cloaked.
		/// Returns false if not cloaked or if failed to get the cloaked state.
		/// On Windows 7 returns false because there is no "cloaked window" feature.
		/// </summary>
		public bool IsCloaked
		{
			get { return IsCloakedGetState != 0; }
		}

		#endregion

		#region state (minimized, maximized, normal)

		/// <summary>
		/// Returns true if minimized.
		/// Calls Api.IsIconic().
		/// Supports ThreadError.
		/// </summary>
		public bool IsMinimized
		{
			get { return Api.IsIconic(this) || ThreadError.SetWinError(); }
		}

		/// <summary>
		/// Returns true if maximized.
		/// Calls Api.IsZoomed().
		/// Supports ThreadError.
		/// </summary>
		public bool IsMaximized
		{
			get { return Api.IsZoomed(this) || ThreadError.SetWinError(); }
		}

		///// <summary>
		///// Returns true if not minimized/maximized.
		///// Calls Api.IsIconic() and Api.IsZoomed().
		///// Supports ThreadError.
		///// </summary>
		//public bool IsNotMinMax
		//{
		//	get { return !(IsMaximized || IsMinimized || ThreadError.IsError); }
		//}

		/// <summary>
		/// Can be used with Wnd.ShowMinimized() and similar functions.
		/// </summary>
		public enum HowMinMax
		{
			/// <summary>
			/// Use LikeUser or LikeProgrammer, depending on window style. In most cases LikeUser.
			/// </summary>
			Auto = 0,
			/// <summary>
			/// Send Api.WM_SYSCOMMAND, like when the user clicks the Minimize, Maximize or Restore button in the title bar.
			/// </summary>
			LikeUser = 1,
			/// <summary>
			/// Call Api.ShowWindow().
			/// </summary>
			LikeProgrammer = 2,
			/// <summary>
			/// Visually fast, without animation. Uses Api.SetWindowPlacement().
			/// </summary>
			NoAnimation = 3,
		}

		/// <summary>
		/// If not minimized, minimizes.
		/// Also unhides.
		/// Applies auto-delay, except when this window is of this thread.
		/// Calls ShowMinimized(HowMinMax.Auto, true, true).
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public void ShowMinimized()
		{
			ShowMinimized(HowMinMax.Auto, true, true);
		}

		/// <summary>
		/// If not minimized, minimizes.
		/// Also unhides.
		/// Returns true if successful or if the window was already in that state.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="how">How to minimize.</param>
		/// <param name="validateThrow">Throw exception (listed below) if fails or the window is invalid.</param>
		/// <param name="applyAutodelay">Apply auto-delay, except when this window is of this thread.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public bool ShowMinimized(HowMinMax how, bool validateThrow = false, bool applyAutodelay = false)
		{
			return _MinMaxRes(Api.SW_MINIMIZE, how, validateThrow, applyAutodelay);
		}

		/// <summary>
		/// If not maximized, maximizes.
		/// Also unhides.
		/// Applies auto-delay, except when this window is of this thread.
		/// Calls ShowMaximized(HowMinMax.Auto, true, true).
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public void ShowMaximized()
		{
			ShowMaximized(HowMinMax.Auto, true, true);
		}

		/// <summary>
		/// If not minimized, minimizes.
		/// Also unhides.
		/// Returns true if successful or if the window was already in that state.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="how">How to maximize.</param>
		/// <param name="validateThrow">Throw exception (listed below) if fails or the window is invalid.</param>
		/// <param name="applyAutodelay">Apply auto-delay, except when this window is of this thread.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public bool ShowMaximized(HowMinMax how, bool validateThrow = false, bool applyAutodelay = false)
		{
			return _MinMaxRes(Api.SW_SHOWMAXIMIZED, how, validateThrow, applyAutodelay);
		}

		/// <summary>
		/// If maximized or minimized, makes normal (not min/max).
		/// Also unhides.
		/// Applies auto-delay, except when this window is of this thread.
		/// Calls ShowNotMinMax(HowMinMax.Auto, true, true).
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public void ShowNotMinMax()
		{
			ShowNotMinMax(HowMinMax.Auto, true, true);
		}

		/// <summary>
		/// If maximized or minimized, makes normal (not min/max).
		/// Also unhides.
		/// Returns true if successful or if the window was already in that state.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="how">How to make normal.</param>
		/// <param name="validateThrow">Throw exception (listed below) if fails or the window is invalid.</param>
		/// <param name="applyAutodelay">Apply auto-delay, except when this window is of this thread.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public bool ShowNotMinMax(HowMinMax how, bool validateThrow = false, bool applyAutodelay = false)
		{
			return _MinMaxRes(Api.SW_SHOWNORMAL, how, validateThrow, applyAutodelay);
		}

		/// <summary>
		/// If minimized, restores previous non-minimized state (maximized or normal).
		/// Also unhides.
		/// Applies auto-delay, except when this window is of this thread.
		/// Calls ShowNotMinimized(HowMinMax.Auto, true, true).
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public void ShowNotMinimized()
		{
			ShowNotMinimized(HowMinMax.Auto, true, true);
		}

		/// <summary>
		/// If minimized, restores previous non-minimized state (maximized or normal).
		/// Also unhides.
		/// Returns true if successful or if the window was already in that state.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="how">How to restore.</param>
		/// <param name="validateThrow">Throw exception (listed below) if fails or the window is invalid.</param>
		/// <param name="applyAutodelay">Apply auto-delay, except when this window is of this thread.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails.
		/// </exception>
		public bool ShowNotMinimized(HowMinMax how, bool validateThrow = false, bool applyAutodelay = false)
		{
			return _MinMaxRes(Api.SW_RESTORE, how, validateThrow, applyAutodelay);
		}

		/// <summary>
		/// Sets window min/max/normal/restore state.
		/// Also unhides.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="state">Must be Api.SW_MINIMIZE, Api.SW_RESTORE (restores to normal/max if minimized), Api.SW_SHOWNORMAL or Api.SW_SHOWMAXIMIZED.</param>
		/// <param name="how">Use WM_SYSCOMMAND, Api.ShowWindow() (animated) or Api.SetWindowPlacement() (no animation).</param>
		bool _MinMaxRes(int state, HowMinMax how, bool validateThrow, bool applyAutodelay)
		{
			Debug.Assert(state == Api.SW_MINIMIZE || state == Api.SW_RESTORE || state == Api.SW_SHOWNORMAL || state == Api.SW_SHOWMAXIMIZED);

			if(validateThrow) ValidateThrow();

			bool ok = false, fast = false, wasMinimized = IsMinimized;
			uint cmd = 0;

			switch(state) {
			case Api.SW_MINIMIZE:
				ok = wasMinimized;
				break;
			case Api.SW_RESTORE:
				ok = !wasMinimized;
				break;
			case Api.SW_SHOWNORMAL:
				ok = !wasMinimized && !IsMaximized; //info: if invalid handle, _Show() will return false, don't need to check here.
				break;
			case Api.SW_SHOWMAXIMIZED:
				ok = IsMaximized;
				break;
			}

			if(ok) {
				if(Visible) return true;
				ok = _Show(true) || ThreadError.SetWinError();
			} else {
				g1:
				switch(how) {
				case HowMinMax.NoAnimation:
					fast = true;
					break;
				case HowMinMax.Auto: //Send WM_SYSCOMMAND if has minimize/maximize button. Else call Api.ShowWindow.
					uint style = Style;
					switch(state) {
					case Api.SW_MINIMIZE: if((style & Api.WS_MINIMIZEBOX) != 0) cmd = Api.SC_MINIMIZE; break;
					case Api.SW_SHOWMAXIMIZED: if((style & Api.WS_MAXIMIZEBOX) != 0) cmd = Api.SC_MAXIMIZE; break;
					default: if((style & (Api.WS_MAXIMIZEBOX | Api.WS_MAXIMIZEBOX)) != 0) cmd = Api.SC_RESTORE; break;
					}
					break;
				case HowMinMax.LikeUser:
					switch(state) {
					case Api.SW_MINIMIZE: cmd = Api.SC_MINIMIZE; break;
					case Api.SW_SHOWMAXIMIZED: cmd = Api.SC_MAXIMIZE; break;
					default: cmd = Api.SC_RESTORE; break;
					}
					break;
				}

				if(cmd != 0) {
					ok = SendTimeout(10000, Api.WM_SYSCOMMAND, cmd);
					//if was minimized, now can be maximized, need to restore if SW_SHOWNORMAL
					if(ok && state == Api.SW_SHOWNORMAL && IsMaximized) ok = SendTimeout(10000, Api.WM_SYSCOMMAND, cmd);
				} else {
					if(fast) {
						Api.WINDOWPLACEMENT p;
						if(!GetWindowPlacement(out p)) return false;

						switch(state) {
						case Api.SW_MINIMIZE:
							if(p.showCmd == Api.SW_SHOWMAXIMIZED) p.flags |= Api.WPF_RESTORETOMAXIMIZED; else p.flags &= ~Api.WPF_RESTORETOMAXIMIZED; //Windows forgets to remove the flag
							break;
						case Api.SW_RESTORE:
							if((p.flags & Api.WPF_RESTORETOMAXIMIZED) != 0) state = Api.SW_SHOWMAXIMIZED; //without this would make normal
							break;
						}

						//if(wasMinimized) p.flags|=Api.WPF_ASYNCWINDOWPLACEMENT; //fixes Windows bug: if window of another thread, deactivates currently active window and does not activate this window. However then animates window. If we set this while the window is not minimized, it would set blinking caret in inactive window. 
						p.showCmd = state;

						ok = SetWindowPlacement(ref p);
					} else {
						ok = Api.ShowWindow(this, state) || ThreadError.SetWinError();
					}

					if(!ok && ThreadError.WinErrorCode == Api.ERROR_ACCESS_DENIED) { how = HowMinMax.LikeUser; goto g1; } //UAC blocks the API but not the message
				}

				if(ok) _SetStateActivateWait(state, wasMinimized);
			}

			if(ok) { if(applyAutodelay) Time.AutoDelay(this); } else if(validateThrow) throw new CatException();

			return ok;
		}

		void _SetStateActivateWait(int state, bool wasMinimized)
		{
			if(IsOfThisThread) return;
			if(wasMinimized) ActivateRaw(); //fix Windows bug: if window of another thread, deactivates currently active window and does not activate this window
			else if(state == Api.SW_MINIMIZE) Misc.WaitForAnActiveWindow();
		}

		/// <summary>
		/// Initializes wp and calls Api.GetWindowPlacement.
		/// Supports ThreadError.
		/// </summary>
		public bool GetWindowPlacement(out Api.WINDOWPLACEMENT wp)
		{
			wp = new Api.WINDOWPLACEMENT(); wp.length = Api.SizeOf(wp);
			return Api.GetWindowPlacement(this, ref wp) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Sets wp.length and calls Api.SetWindowPlacement.
		/// Supports ThreadError.
		/// </summary>
		public bool SetWindowPlacement(ref Api.WINDOWPLACEMENT wp)
		{
			wp.length = Api.SizeOf(wp);
			return Api.SetWindowPlacement(this, ref wp) || ThreadError.SetWinError();
		}

		//Rarely used.
		///// <summary>
		///// Quickly minimizes (without animation) and hides. Activates another window.
		///// Supports ThreadError.
		///// </summary>
		//public bool MinimizeAndHide()
		//{
		//	if(IsMinimized && !Visible) return true; //because ShowMinimized() unhides
		//	return ShowMinimized(HowMinMax.NoAnimation) && _Show(false);
		//}

		#endregion

		#region activate, focus

		/// <summary>
		/// Gets the foreground window.
		/// Calls Api.GetForegroundWindow.
		/// </summary>
		public static Wnd ActiveWindow { get { return Api.GetForegroundWindow(); } }

		/// <summary>
		/// Returns true if this window is the foreground window.
		/// </summary>
		public bool IsActive { get { return !Is0 && this == Api.GetForegroundWindow(); } }

		/// <summary>
		/// Activates this window (brings to the foreground).
		/// Also unhides, restores minimized etc, to ensure that the window is ready to receive sent keys, mouse clicks ect.
		/// Applies auto-delay, except when this window is of this thread.
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails to activate (unlikely).
		/// </exception>
		/// <remarks>
		/// Activating a window usually also uncloaks it, for example switches to its virtual desktop on Windows 10.
		/// Fails (throws exception) if cannot activate this window, except:
		///		1. If this is a control, activates its top-level parent window.
		///		2. If this is Wnd.Get.DesktopWindow, just deactivates the currently active window.
		///		3. When the target application instead activates another window of the same thread.
		/// This overload just calls Activate(HowActivate.CanThrow).
		/// </remarks>
		/// <seealso cref="ActivateRaw"/>
		/// <seealso cref="IsActive"/>
		/// <seealso cref="Wnd.ActiveWindow"/>
		/// <seealso cref="Wnd.SwitchActiveWindow"/>
		/// <seealso cref="Activate(_ActivateFlags)"/>
		public void Activate()
		{
			Activate(HowActivate.CanThrow);
		}

		/// <summary>
		/// Flags for the Wnd.Activate() overload that accepts flags.
		/// </summary>
		[Flags]
		public enum HowActivate
		{
			/// <summary>
			/// Throw exception if invalid window or if fails to activate. Without this flag then sets thread error and returns false.
			/// </summary>
			CanThrow = 1,
			/// <summary>
			/// Don't activate if has WS_EX_NOACTIVATE style or is toolwindow without caption, unless cloaked. Then returns true.
			/// </summary>
			IgnoreIfNoActivateStyleEtc = 2,
			/// <summary>
			/// Don't check is this a control (child window). Without this flag activates control's top-level parent window.
			/// </summary>
			DontCheckIsControl = 4,
			/// <summary>
			/// Don't apply auto-delay.
			/// Anyway, always waits at least 20 ms. You can instead use ActivateRaw(), it is as fast as possible.
			/// </summary>
			Faster = 8,
		}

		/// <summary>
		/// Activates this window (brings to the foreground).
		/// Also unhides, restores minimized etc, to ensure that the window is ready to receive sent keys, mouse clicks ect.
		/// Everything is the same as with other overload, except that this overload has a 'flags' parameter and by default returns false if failed (does not throw exception).
		/// </summary>
		public bool Activate(HowActivate flags)
		{
			bool canThrow = flags.HasFlag(HowActivate.CanThrow);
			if(!Validate(canThrow)) return false;
			if(!flags.HasFlag(HowActivate.DontCheckIsControl) && IsChildWindow) return ToplevelParentOrThis.Activate(flags | HowActivate.DontCheckIsControl);

			bool ofThisThread = IsOfThisThread;

			if(IsMinimized) {
				ShowNotMinimized();
				if(!ofThisThread) WaitMS(100); //need minimum 20 for Excel
			}
			if(!Visible) Visible = true;

			bool R = IsActive, noAct = false;

			if(!R) {
				if(flags.HasFlag(HowActivate.IgnoreIfNoActivateStyleEtc)) {
					uint est = ExStyle;
					if((est & Api.WS_EX_NOACTIVATE) != 0) noAct = true;
					else if((est & (Api.WS_EX_TOOLWINDOW | Api.WS_EX_APPWINDOW)) == Api.WS_EX_TOOLWINDOW) noAct = !HasStyle(Api.WS_CAPTION);
					if(noAct && !IsCloaked) return true; //if cloaked, need to activate to uncloak
				}

				for(int i = 0; i < 3; i++) {
					bool ok = ActivateRaw();

					if(!ofThisThread) {
						int speed = flags.HasFlag(HowActivate.Faster) ? 0 : Script.Speed;
						for(int j = 0; j < 5; j++) {
							//Out(ActiveWindow);
							WaitMS(speed / 5 + 2);
							//Perf.First();
							SendTimeout(200, 0);
							//Perf.NW();
						}
					}

					if(ok) {
						Wnd f = ActiveWindow;
						if(f == this) R = true;
						else if(this == Get.DesktopWindow) R = f.Is0;
						else {
							uint tid = ThreadId; if(tid == 0) break;
							if(f.ThreadId == tid) {
								R = Api.SetForegroundWindow(Get.DesktopWindow) && ActivateRaw() && ActiveWindow.ThreadId == tid;
								if(R) WaitMS(20);
								//Excel creates a minimized/offscreen window for each workbook opened in that excel process.
								//These windows just add taskbar buttons. Also it allows to find and activate workbooks.
								//When you activate such window, Excel instead activates its main window, where it displays all workbooks.
								//For this reason we would fail (not always, because this may be temporarily active).
								//Same with PowerPoint. Other Office apps no.
							}
						}
						if(R) break;
					}

					if(noAct) break;
				}
			}

			if(R && !ofThisThread && IsCloaked) {
				R = false;
				for(int i = 0; i < 40; i++) { WaitMS(50); if(R = !IsCloaked) break; } //when switching Win10 desktops, uncloaks after ~20 ms
				if(R) WaitMS(800); //need minimum 600 for pixel() and wait C, because of animation while switching Win10 virtual desktops.
			}

			if(R) return R;
			return ThreadError.ThrowOrSet(canThrow, "Failed to activate window.");
		}

		/// <summary>
		/// Low-level version of Activate().
		/// Just calls AllowActivate(), Api.SetForegroundWindow() and makes sure that it actually worked, but does not check whether it activated exactly this window.
		/// No exceptions, no auto-delay, does not unhide, does not restore minimized, does not check is it a top-level window or control, etc.
		/// Returns false if fails (unlikely).
		/// </summary>
		public bool ActivateRaw()
		{
			if(IsActive) return true;

			bool canAct = AllowActivate(true);

			if(!Api.SetForegroundWindow(this)) {
				if(!canAct || !IsValid) return false;
				//It happens when foreground process called LockSetForegroundWindow.
				//Although AllowSetForegroundWindow returns true, SetForegroundWindow fails.
				//It happens only before this process sends keys. Eg after first _AllowActivate_SendKey this never happens again.
				//If it has higher IL (and this process is User), also need _AllowActivate_MinRes.
				_AllowActivate_SendKey(true);
				if(!Api.SetForegroundWindow(this)) {
					_AllowActivate_MinRes();
					if(!Api.SetForegroundWindow(this)) return false;
				}
			}

			//Sometimes after SetForegroundWindow there is no active window for several ms. Not if the window is of this thread.
			if(this == Get.DesktopWindow) return ActiveWindow.Is0;
			return Misc.WaitForAnActiveWindow();
		}

		/// <summary>
		/// Activates another main window, like with Alt+Tab.
		/// Returns the window. Returns Wnd0 if there is no such window or if fails to activate (unlikely).
		/// Uses Wnd.Get.NextMainWindow() and Activate().
		/// </summary>
		/// <param name="skipMinimized">Don't activate minimized windows.</param>
		public static Wnd SwitchActiveWindow(bool skipMinimized = false)
		{
			Wnd wa = ActiveWindow;
			Wnd w = Get.NextMainWindow(wa, retryFromTop: true, skipMinimized: skipMinimized, likeAltTab: true);
			if(w.Is0 || w == wa || !w.Activate(HowActivate.DontCheckIsControl)) return Wnd0; //does not throw
			return w;

			//TODO: when all minimized (eg after ShowDesktop), activates wrong window.
		}

		public static partial class Misc
		{
			/// <summary>
			/// Waits while there is no active window.
			/// It sometimes happens after switching the active window (and it is not of this thread), very briefly until Windows makes the new window active after making the old window inactive.
			/// Don't need to call this after Activate(), ActivateRaw() and other functions of this library that activate windows.
			/// Waits max about 200 ms, then returns false if there is no active window.
			/// </summary>
			public static bool WaitForAnActiveWindow()
			{
				for(int i = 2; i < 20; i++) {
					if(!ActiveWindow.Is0) return true;
					WaitMS(i);
				}
				return false;
			}
		}

		public static partial class Misc
		{
			/// <summary>
			/// Temporarily enables this process to activate windows with Api.SetForegroundWindow().
			/// Returns false if fails (unlikely).
			/// In some cases you may need this function because Windows often disables SetForegroundWindow() to not allow applications to activate their windows while the user is working (using keyboard/mouse) with the currently active window. Then SetForegroundWindow() just makes the window's taskbar button flash which indicates that the windows wants attention. More info in SetForegroundWindow() help in MSDN.
			/// Usually you will not call Api.SetForegroundWindow() directly. It is called by some other functions, for example some API dialog functions.
			/// Don't need to call this function to enable Activate(), ActivateRaw() and other functions of this library that activate windows.
			/// </summary>
			public static bool AllowActivate()
			{
				return Wnd.AllowActivate(false);
			}
		}

		internal static bool AllowActivate(bool goingToActivateAWindow)
		{
			if(_AllowActivate_AllowSetFore()) return true; //not locked, or already successfully called ASF_Key

			_AllowActivate_SendKey(true);
			if(_AllowActivate_AllowSetFore()) return true;
			//First time fails if the foreground window is of higher IL. Then sending keys does not work.

			//_AllowActivate_MinRes() makes no active window...
			if(goingToActivateAWindow) {
				_AllowActivate_MinRes();
				return _AllowActivate_AllowSetFore();
			}

			var wFore = ActiveWindow; bool retry = false;
			g1: _AllowActivate_MinRes();
			if(!_AllowActivate_AllowSetFore()) return false;
			if(!wFore.Is0 && !retry) {
				Api.SetForegroundWindow(wFore);
				if(!_AllowActivate_AllowSetFore()) { retry = true; goto g1; } //didn't notice this but let's be safer
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
		static void _AllowActivate_SendKey(bool debugOut)
		{
			if(debugOut) OutDebug("AllowActivate: need key");

			var x = new Api.INPUTKEY(0, 128, Api.IKFlag.Up);
			Api.SendInputKey(ref x);
			//info: works without waiting.
		}

		/// <summary>
		/// Creates a temporary minimized window and restores it. It activates the window and allows us to activate.
		/// Then sets 'no active window' to prevent auto-activating another window when destroying the temporary window.
		/// </summary>
		static void _AllowActivate_MinRes()
		{
			OutDebug("AllowActivate: need min/res");

			Wnd t = Api.CreateWindowEx(Api.WS_EX_TOOLWINDOW, "#32770", null, Api.WS_POPUP | Api.WS_MINIMIZE | Api.WS_VISIBLE, 0, 0, 0, 0, Wnd0, 0, Zero, 0);
			//info: When restoring, the window must be visible, or may not work.
			try {
				var wp = new Api.WINDOWPLACEMENT(); wp.showCmd = Api.SW_RESTORE;
				t.SetWindowPlacement(ref wp); //activates t; fast (no animation)
				_AllowActivate_SendKey(false); //makes so that later our process can always activate
				_AllowActivate_AllowSetFore();
				Api.SetForegroundWindow(Get.DesktopWindow); //set no foreground window, or may activate the higher IL window (maybe does not activate, but winevents hook gets events, in random order). Other way would be to destroy our window later, but more difficult to implement.

			}
			finally { Api.DestroyWindow(t); }
		}

		/// <summary>
		/// Calls Api.AllowSetForegroundWindow(Api.GetCurrentProcessId()).
		/// </summary>
		static bool _AllowActivate_AllowSetFore() { return Api.AllowSetForegroundWindow(Api.GetCurrentProcessId()); }

		//Too unreliable.
		///// <summary>
		///// Calls Api.LockSetForegroundWindow(), which temporarily prevents other applications from activating windows easily with SetForegroundWindow().
		///// If Api.LockSetForegroundWindow() fails, calls AllowActivate() and retries.
		///// </summary>
		///// <param name="on">Lock or unlock.</param>
		//public static bool LockActiveWindow(bool on)
		//{
		//	uint f = on ? Api.LSFW_LOCK : Api.LSFW_UNLOCK;
		//	if(Api.LockSetForegroundWindow(f)) return true;
		//	return AllowActivate() && Api.LockSetForegroundWindow(f);
		//}

		/// <summary>
		/// Makes this control the focused (receiving keyboard input) control.
		/// Also activetes its top-level parent window.
		/// This control can belong to any process/thread. To focus controls of this thread usually it's better to use FocusControlOfThisThread(); it is lightweight, no exceptions.
		/// Applies auto-delay, except when this control is of this thread.
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails to activate parent window.
		/// 3. When fails to set focus, for example because of UAC.
		/// </exception>
		/// <seealso cref="Wnd.FocusedControl"/>
		public void FocusControl()
		{
			ValidateThrow();
			Wnd wTL = ToplevelParentOrThis;
			if(wTL != Api.GetForegroundWindow()) wTL.Activate(HowActivate.CanThrow | HowActivate.DontCheckIsControl);

			uint th1 = Api.GetCurrentThreadId(), th2 = ThreadId;
			if(th1 == th2) {
				Api.SetFocus(this);
				return;
			}

			bool ok = false;
			if(Api.AttachThreadInput(th1, th2, true))
				try {
					int i, speed = Script.Speed;
					for(i = 0; i < 50; i++) {
						Api.SetFocus(this);
						if(this == FocusedControl) { ok = true; break; }
						WaitMS(speed / 20 + 5);
					}
				}
				finally { Api.AttachThreadInput(th1, th2, false); }

			if(!ok) throw new CatException("Failed to set focus.");

			Time.AutoDelay(this);

			//note: don't use accSelect because on Win7 it simply calls SetForegroundWindow, which deactivates parent.
		}

		/// <summary>
		/// Makes this control the focused (receiving keyboard input) control.
		/// This control must belong to this thread, else nothing happens.
		/// Its top-level parent window should be the active window, else nothing happens.
		/// Calls Api.SetFocus.
		/// </summary>
		public void FocusControlOfThisThread()
		{
			Api.SetFocus(this);
		}

		/// <summary>
		/// Gets the focused (receiving keyboard input) control of this thread.
		/// Returns Wnd0 if the top-level parent window is not the active window.
		/// Calls Api.GetFocus().
		/// </summary>
		public static Wnd FocusedControlOfThisThread { get { return Api.GetFocus(); } }

		/// <summary>
		/// Gets the focused (receiving keyboard input) control of the currently active process/thread.
		/// Uses Api.GetGUIThreadInfo().
		/// </summary>
		public static Wnd FocusedControl
		{
			get
			{
				Api.GUITHREADINFO g;
				Misc.GetGUIThreadInfo(out g);
				return g.hwndFocus;
			}
		}

		#endregion

		#region rect

		/// <summary>
		/// Gets window rectangle (position and dimensions).
		/// Calls Api.GetWindowRect() and returns its return value.
		/// Supports ThreadError.
		/// When you don't need the bool return value, you can instead use the Rect property, which calls this function.
		/// </summary>
		/// <param name="r">A variable that receives the rectangle in screen coordinates. Will be empty if failed.</param>
		/// <seealso cref="GetClientRect"/>
		/// <seealso cref="RectInClientOf"/>
		bool GetWindowRect(out RECT r)
		{
			if(Api.GetWindowRect(this, out r)) return true;
			ThreadError.SetWinError();
			r.SetEmpty();
			return false;
		}

		/// <summary>
		/// Gets or sets window rectangle.
		/// <para>
		/// The 'get' function calls GetWindowRect(). Uses primary screen coordinates.
		/// Returns empty rectangle if fails (eg window closed).
		/// Use GetWindowRect() instead when you need a bool return value.
		/// </para>
		/// <para>
		/// The 'set' function for controls uses coordinates in direct parent's client area. For top-level windows - in primary screen. Uses MoveRaw().
		/// </para>
		/// Supports ThreadError. Sets thread error if failed, clears if succeeded.
		/// </summary>
		public RECT Rect
		{
			get
			{
				ThreadError.Clear();
				RECT r; GetWindowRect(out r);
				return r;
			}
			set { _Move(value.left, value.top, value.Width, value.Height); }
		}

		/// <summary>
		/// Gets or sets horizontal position.
		/// The 'set' function for controls uses coordinates in direct parent's client area. In other cases are use primary screen coordinates.
		/// Supports ThreadError.
		/// </summary>
		public int X
		{
			get { return Rect.left; }
			set { MoveRaw(value, null); }
		}
		/// <summary>
		/// Gets or sets vertical position.
		/// The 'set' function for controls uses coordinates in direct parent's client area. In other cases are use primary screen coordinates.
		/// Supports ThreadError.
		/// </summary>
		public int Y
		{
			get { return Rect.top; }
			set { MoveRaw(null, value); }
		}
		/// <summary>
		/// Gets or sets width.
		/// Supports ThreadError.
		/// </summary>
		public int Width
		{
			get { return Rect.Width; }
			set { ResizeRaw(value, null); }
		}
		/// <summary>
		/// Gets or sets height.
		/// Supports ThreadError.
		/// </summary>
		public int Height
		{
			get { return Rect.Height; }
			set { ResizeRaw(null, value); }
		}

		/// <summary>
		/// Gets client area rectangle.
		/// Calls Api.GetClientRect() and returns its return value.
		/// Supports ThreadError.
		/// When you don't need the bool return value, you can instead use the ClientRect property, which calls this function.
		/// This method is the same as GetClientSize, just the parameter type is different.
		/// </summary>
		/// <param name="r">A variable that receives the rectangle. Will be empty if failed.</param>
		bool GetClientRect(out RECT r)
		{
			if(Api.GetClientRect(this, out r)) return true;
			ThreadError.SetWinError();
			r.SetEmpty();
			return false;
		}

		/// <summary>
		/// Gets client area width and height.
		/// Calls Api.GetClientRect() and returns its return value.
		/// Supports ThreadError.
		/// When you don't need the bool return value, you can instead use the ClientSize property, which calls this function.
		/// This method is the same as GetClientRect, just the parameter type is different.
		/// </summary>
		/// <param name="z">A variable that receives width and height. Will be empty if failed.</param>
		bool GetClientSize(out SIZE z)
		{
			RECT r;
			if(Api.GetClientRect(this, out r)) { z = new SIZE(r.right, r.bottom); return true; }
			ThreadError.SetWinError();
			z = new SIZE();
			return false;
		}

		/// <summary>
		/// Gets or sets client area rectangle (width and height).
		/// The left and top fields are always 0. The right and bottom fields are the width and height of the client area.
		/// <para>
		/// The 'get' function returns empty rectangle if fails (eg window closed).
		/// Use GetClientRect() instead when you need a bool return value.
		/// </para>
		/// <para>
		/// The 'set' function calculates and sets window rectangle from the specified client area rectangle. Uses MoveRaw().
		/// </para>
		/// Supports ThreadError. Sets thread error if failed, clears if succeeded.
		/// This property is the same as ClientSize, just its type is different.
		/// </summary>
		public RECT ClientRect
		{
			get
			{
				ThreadError.Clear();
				RECT r; GetClientRect(out r);
				return r;
			}
			set { _SetClientSize(value.Width, value.Height); }
		}
		/// <summary>
		/// Gets or sets client area width and height.
		/// The 'set' function calculates and sets window rectangle from the specified client area width and height.
		/// The 'get' function returns empty SIZE value if fails (eg window closed).
		/// Supports ThreadError. Sets thread error if failed, clears if succeeded.
		/// Use GetClientSize() instead when you need a bool return value.
		/// This property is the same as ClientRect, just its type is different.
		/// </summary>
		public SIZE ClientSize
		{
			get
			{
				ThreadError.Clear();
				SIZE z; GetClientSize(out z);
				return z;
			}
			set { _SetClientSize(value.cx, value.cy); }
		}
		/// <summary>
		/// Gets or sets client area width.
		/// The 'set' function calculates and sets window width from the specified client area width.
		/// Supports ThreadError.
		/// </summary>
		public int ClientWidth
		{
			get { return ClientSize.cx; }
			set { _SetClientSize(value, null); }
		}
		/// <summary>
		/// Gets or sets client area height.
		/// The 'set' function calculates and sets window height from the specified client area height.
		/// Supports ThreadError.
		/// </summary>
		public int ClientHeight
		{
			get { return ClientSize.cy; }
			set { _SetClientSize(null, value); }
		}

		bool _SetClientSize(int? width, int? height)
		{
			Api.WINDOWINFO u; if(!GetWindowInfo(out u)) return false;

			int W = width != null ? width.Value : u.rcClient.Width; W += u.rcWindow.Width - u.rcClient.Width;
			int H = height != null ? height.Value : u.rcClient.Height; H += u.rcWindow.Height - u.rcClient.Height;

			return _Resize(W, H);
		}

		/// <summary>
		/// Calls Api.GetWindowInfo().
		/// All info in MSDN, WINDOWINFO topic.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="wi">A Api.WINDOWINFO variable that receives window/client rectangles, styles etc. This function clears it and sets cbSize before calling Api.GetWindowInfo().</param>
		public bool GetWindowInfo(out Api.WINDOWINFO wi)
		{
			wi = new Api.WINDOWINFO(); wi.cbSize = Api.SizeOf(wi);
			return Api.GetWindowInfo(this, ref wi) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Gets window rectangle and client area rectangle, both in screen coordinates.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="rWindow">Receives window rectangle.</param>
		/// <param name="rClient">Receives client area rectangle.</param>
		public bool GetWindowAndClientRectInScreen(out RECT rWindow, out RECT rClient)
		{
			ThreadError.Clear(); //because this func is used by some property-set functions
			Api.WINDOWINFO u;
			if(GetWindowInfo(out u)) { rWindow = u.rcWindow; rClient = u.rcClient; return true; }
			rWindow = new RECT(); rClient = new RECT();
			return false;
		}

		//don't use this because: rarely used; can use GetWindowAndClientRectInScreen; the 'set' function is unclear how it should work.
		///// <summary>
		///// Gets or sets client area rectangle in primary screen coordinates.
		///// <para>
		///// The 'get' function returns empty rectangle if fails (eg window closed).
		///// Use GetWindowAndClientRectInScreen() instead when you need a bool return value.
		///// </para>
		///// <para>
		///// The 'set' function calculates and sets window rectangle from the specified client area rectangle. Uses MoveRaw().
		///// The 'set' function for controls use parent coordinates, not screen coordinates.
		///// </para>
		///// Supports ThreadError. Sets thread error if failed, clears if succeeded.
		///// </summary>
		//public RECT ClientRectInScreen
		//{
		//	get
		//	{
		//		RECT rw, rc;
		//		GetWindowAndClientRectInScreen(out rw, out rc);
		//		return rc;
		//	}
		//	set
		//	{
		//		RECT rw, rc;
		//		if(!GetWindowAndClientRectInScreen(out rw, out rc)) return;
		//		int bL = rc.left - rw.left, bT = rc.top - rw.top, bR = rw.right - rc.right, bB = rw.bottom - rc.bottom;
		//		_Move(value.left - bL, value.top - bT, value.Width + bL + bR, value.Height + bT + bB);
		//	}
		//}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the client area of window w.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public unsafe bool MapClientToClientOf(Wnd w, ref RECT r)
		{
			fixed (void* t = &r) { return _MapWindowPoints(this, w, t, 2); }
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the client area of window w.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public unsafe bool MapClientToClientOf(Wnd w, ref POINT p)
		{
			fixed (void* t = &p) { return _MapWindowPoints(this, w, t, 1); }
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the primary screen.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public bool MapClientToScreen(ref RECT r)
		{
			return MapClientToClientOf(Wnd0, ref r);
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the primary screen.
		/// Calls Api.ClientToScreen().
		/// Supports ThreadError.
		/// </summary>
		public bool MapClientToScreen(ref POINT p)
		{
			Api.SetLastError(0);
			return Api.ClientToScreen(this, ref p) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Converts coordinates relative to the primary screen to coordinates relative to the client area of this window.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public unsafe bool MapScreenToClient(ref RECT r)
		{
			fixed (void* t = &r) { return _MapWindowPoints(Wnd0, this, t, 2); }
		}

		/// <summary>
		/// Converts coordinates relative to the primary screen to coordinates relative to the client area of this window.
		/// Calls Api.ScreenToClient().
		/// Supports ThreadError.
		/// </summary>
		public unsafe bool MapScreenToClient(ref POINT p)
		{
			Api.SetLastError(0);
			return Api.ScreenToClient(this, ref p) || ThreadError.SetWinError();
		}

		static unsafe bool _MapWindowPoints(Wnd wFrom, Wnd wTo, void* t, uint cPoints)
		{
			Api.SetLastError(0);
			if(Api.MapWindowPoints(wFrom, wTo, t, cPoints) != 0) return true;
			return ThreadError.SetIfWinError() == 0;
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the top-left corner of this window.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public bool MapClientToWindow(ref POINT p)
		{
			RECT rw, rc;
			if(!GetWindowAndClientRectInScreen(out rw, out rc)) return false;
			p.x += rc.left - rw.left; p.y += rc.top - rw.top;
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the client area of this window to coordinates relative to the top-left corner of this window.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public bool MapClientToWindow(ref RECT r)
		{
			RECT rw, rc;
			if(!GetWindowAndClientRectInScreen(out rw, out rc)) return false;
			r.Offset(rc.left - rw.left, rc.top - rw.top);
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to coordinates relative to the client area of this window.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public bool MapWindowToClient(ref POINT p)
		{
			RECT rw, rc;
			if(!GetWindowAndClientRectInScreen(out rw, out rc)) return false;
			p.x += rw.left - rc.left; p.y += rw.top - rc.top;
			return true;
		}

		/// <summary>
		/// Converts coordinates relative to the top-left corner of this window to coordinates relative to the client area of this window.
		/// Calls Api.MapWindowPoints().
		/// Supports ThreadError.
		/// </summary>
		public bool MapWindowToClient(ref RECT r)
		{
			RECT rw, rc;
			if(!GetWindowAndClientRectInScreen(out rw, out rc)) return false;
			r.Offset(rw.left - rc.left, rw.top - rc.top);
			return true;
		}

		/// <summary>
		/// Gets rectangle of this window (usually control) relative to the client area of another window (usually the parent window).
		/// Supports ThreadError.
		/// </summary>
		/// <param name="w">The returned rectangle will be relative to the client area of window w. If w is Wnd0, gets rectangle in screen.</param>
		public bool GetRectInClientOf(Wnd w, out RECT r)
		{
			if(w.Is0) return GetWindowRect(out r);
			return GetWindowRect(out r) && w.MapScreenToClient(ref r);
		}

		/// <summary>
		/// Gets or sets child window rectangle in parent window's client area.
		/// <para>
		/// The 'get' function returns empty rectangle if fails (eg window closed).
		/// Use GetRectInClientOf() instead when you need a bool return value.
		/// </para>
		/// <para>
		/// The 'set' function uses MoveRaw() and is the same as Rect.
		/// </para>
		/// Supports ThreadError. Sets thread error if failed, clears if succeeded.
		/// </summary>
		public RECT RectInParent
		{
			get
			{
				ThreadError.Clear();
				RECT r; GetRectInClientOf(DirectParent, out r);
				return r;
			}
			set { _Move(value.left, value.top, value.Width, value.Height); }
		}

		/// <summary>
		/// Gets rectangle of normal (restored) window even if it is minimized or maximized.
		/// Returns false if fails, eg if the window is closed.
		/// Supports ThreadError.
		/// </summary>
		public bool GetNormalStateRect(out RECT r)
		{
			Api.WINDOWPLACEMENT p;
			bool ok = GetWindowPlacement(out p);
			r = p.rcNormalPosition;
			return ok;
		}

		/// <summary>
		/// Returns mouse pointer position relative to the client area of this window.
		/// </summary>
		public POINT MouseClientXY { get { POINT p; Api.GetCursorPos(out p); MapScreenToClient(ref p); return p; } }

		#endregion

		#region move, resize, SetWindowPos

		/// <summary>
		/// Calls Api.SetWindowPos.
		/// All info in MSDN, SetWindowPos topic.
		/// Supports ThreadError.
		/// </summary>
		public bool SetWindowPos(uint swpFlags, int x = 0, int y = 0, int cx = 0, int cy = 0, Wnd wndInsertAfter = default(Wnd))
		{
			return Api.SetWindowPos(this, wndInsertAfter, x, y, cx, cy, swpFlags) || ThreadError.SetWinError();
		}

		/// <summary>
		/// Moves and resizes.
		/// Calls Api.SetWindowPos with flags Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|swpFlagsToAdd.
		/// Supports ThreadError.
		/// </summary>
		internal bool _Move(int x, int y, int width, int height, uint swpFlagsToAdd = 0)
		{
			ThreadError.Clear(); //because this func is used by some property-set functions
			return SetWindowPos(Api.SWP_NOZORDER | Api.SWP_NOOWNERZORDER | Api.SWP_NOACTIVATE | swpFlagsToAdd, x, y, width, height);
		}

		/// <summary>
		/// Moves.
		/// Calls Api.SetWindowPos with flags Api.SWP_NOSIZE|Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|swpFlagsToAdd.
		/// Supports ThreadError.
		/// </summary>
		internal bool _Move(int x, int y, uint swpFlagsToAdd = 0)
		{
			return _Move(x, y, 0, 0, Api.SWP_NOSIZE | swpFlagsToAdd);
		}

		/// <summary>
		/// Resizes.
		/// Calls Api.SetWindowPos with flags Api.SWP_NOMOVE|Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|swpFlagsToAdd.
		/// Supports ThreadError.
		/// </summary>
		internal bool _Resize(int width, int height, uint swpFlagsToAdd = 0)
		{
			return _Move(0, 0, width, height, Api.SWP_NOMOVE | swpFlagsToAdd);
		}

		/// <summary>
		/// Moves and/or resizes.
		/// This is a lower-level function than Move() (which can throw exceptions, applies auto-delay, supports fractional/workarea coordinates, restores if min/max, does not support Api.SWP_x flags).
		/// For top-level windows use primary screen coordinates. For controls - direct parent client coordinates.
		/// Calls Api.SetWindowPos().
		/// More info in MSDN, SetWindowPos topic.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="x">Left. Can be null to not move in X axis.</param>
		/// <param name="y">Top. Can be null to not move in Y axis.</param>
		/// <param name="width">Width. Can be null to not change width.</param>
		/// <param name="height">Height. Can be null to not change height.</param>
		/// <param name="swpFlagsToAdd">One or more Api.SWP_x flags, except Api.SWP_NOMOVE|Api.SWP_NOSIZE|Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE.</param>
		public bool MoveRaw(int? x, int? y, int? width, int? height, uint swpFlagsToAdd = 0)
		{
			int L = 0, T = 0, W = 0, H = 0;

			uint f = swpFlagsToAdd, getRect = 0;
			if(x == null && y == null) f |= Api.SWP_NOMOVE;
			else {
				if(x != null) L = x.Value; else getRect |= 1;
				if(y != null) T = y.Value; else getRect |= 2;
			}

			if(width == null && height == null) f |= Api.SWP_NOSIZE;
			else {
				if(width != null) W = width.Value; else getRect |= 4;
				if(height != null) H = height.Value; else getRect |= 8;
			}

			if(getRect != 0) {
				RECT r; if(!GetRectInClientOf(DirectParent, out r)) return false;
				if((getRect & 1) != 0) L = r.left;
				if((getRect & 2) != 0) T = r.top;
				if((getRect & 4) != 0) W = r.Width;
				if((getRect & 8) != 0) H = r.Height;
			}

			return _Move(L, T, W, H, f);
		}

		/// <summary>
		/// Moves.
		/// This is a lower-level function than Move() (which can throw exceptions, applies auto-delay, supports fractional/workarea coordinates, restores if min/max).
		/// For top-level windows use primary screen coordinates. For controls - direct parent client coordinates.
		/// Calls MoveRaw(x, y, null, null, 0).
		/// Supports ThreadError.
		/// </summary>
		/// <param name="x">Left. Can be null to not move in X axis.</param>
		/// <param name="y">Top. Can be null to not move in Y axis.</param>
		public bool MoveRaw(int? x, int? y)
		{
			return MoveRaw(x, y, null, null);
		}

		/// <summary>
		/// Resizes.
		/// This is a lower-level function than Resize() (which can throw exceptions, applies auto-delay, supports fractional coordinates, restores if min/max).
		/// Calls MoveRaw(null, null, width, height, 0).
		/// Supports ThreadError.
		/// </summary>
		/// <param name="width">Width. Can be null to not change width.</param>
		/// <param name="height">Height. Can be null to not change height.</param>
		public bool ResizeRaw(int? width, int? height)
		{
			return MoveRaw(null, null, width, height);
		}

		/// <summary>
		/// Moves and/or resizes.
		/// For top-level windows use primary screen coordinates. For controls - direct parent client coordinates.
		/// Applies auto-delay, except when this window is of this thread.
		/// </summary>
		/// <param name="x">Left. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not move in X axis.</param>
		/// <param name="y">Top. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not move in Y axis.</param>
		/// <param name="width">Width. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not change width.</param>
		/// <param name="height">Height. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not change height.</param>
		/// <param name="workArea">If false, the coordinates are relative to the primary screen, else to its work area. Not used when this is a child window.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized. If don't need it, use MoveRaw().
		/// </remarks>
		public void Move(Coord x, Coord y, Coord width, Coord height, bool workArea = false)
		{
			ValidateThrow();

			Wnd w = DirectParent;
			POINT xy, wh;
			if(!w.Is0) {
				xy = Coord.GetNormalizedInWindowClientArea(x, y, w);
				wh = Coord.GetNormalizedInWindowClientArea(width, height, w);
			} else {
				xy = Coord.GetNormalizedInScreen(x, y, workArea);
				wh = Coord.GetNormalizedInScreen(width, height, workArea, true);
			}

			uint f = 0, getRect = 0;
			if(x == null && y == null) f |= Api.SWP_NOMOVE; else if(x == null) getRect |= 1; else if(y == null) getRect |= 2;
			if(width == null && height == null) f |= Api.SWP_NOSIZE; else if(width == null) getRect |= 4; else if(height == null) getRect |= 8;

			if(getRect != 0) {
				RECT r; if(!GetRectInClientOf(w, out r)) ThreadError.ThrowIfError();
				if((getRect & 1) != 0) xy.x = r.left;
				if((getRect & 2) != 0) xy.y = r.top;
				if((getRect & 4) != 0) wh.x = r.Width;
				if((getRect & 8) != 0) wh.y = r.Height;
			}

			//restore min/max, except if child or hidden
			if(w.Is0 && (IsMinimized || IsMaximized) && Visible) {
				ShowNotMinMax(HowMinMax.NoAnimation);
				//info: '&& Visible' because ShowNotMinMax unhides
			}

			if(!_Move(xy.x, xy.y, wh.x, wh.y, f)) ThreadError.ThrowIfError();

			Time.AutoDelay(this);
		}


		/// <summary>
		/// Moves.
		/// For top-level windows use primary screen coordinates. For controls - direct parent client coordinates.
		/// Applies auto-delay, except when this window is of this thread.
		/// Calls Move(x, y, null, null, workArea).
		/// </summary>
		/// <param name="x">Left. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not move in X axis.</param>
		/// <param name="y">Top. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not move in Y axis.</param>
		/// <param name="workArea">If false, the coordinates are relative to the primary screen, else to its work area. Not used when this is a child window.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized. If don't need it, use MoveRaw().
		/// </remarks>
		public void Move(Coord x, Coord y, bool workArea = false)
		{
			Move(x, y, null, null, workArea);
		}

		/// <summary>
		/// Resizes.
		/// Applies auto-delay, except when this window is of this thread.
		/// Calls Move(null, null, width, height, workArea).
		/// </summary>
		/// <param name="width">Width. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not change width.</param>
		/// <param name="height">Height. Can be int (pixels) or float (fraction of screen or work area or direct parent client area) or null to not change height.</param>
		/// <param name="workArea">If false, fractional width/height are part of the primary screen, else of its work area. Not used when this is a child window.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <remarks>
		/// Also restores the visible top-level window if it is minimized or maximized. If don't need it, use ResizeRaw().
		/// </remarks>
		public void Resize(Coord width, Coord height, bool workArea = false)
		{
			Move(null, null, width, height, workArea);
		}

		#endregion

		#region MoveInScreen_EnsureInScreen

		static bool _MoveInScreen(bool useWindow, Wnd w, ref RECT r, object screen, bool bWorkArea, bool bJustMoveIntoScreen, bool bCanLimitSize, bool bRawXY)
		{
			int x = 0, y = 0, x0 = r.left, y0 = r.top, wid, hei, xmax, ymax;

			if(bJustMoveIntoScreen) { //only make sure that whole w or r (which must be relative to the primary screen) is in its (or nearest) screen
				if(screen == null) {
					if(useWindow) screen = w;
					else screen = r;
				}
				bRawXY = false;
			} // else //move w or r to r.left r.top in selected screen, etc

			Screen scr = Screen_.FromObject(screen, w);
			RECT rs = bWorkArea ? scr.WorkingArea : scr.Bounds;

			if(!bJustMoveIntoScreen) { x = r.left + rs.left; y = r.top + rs.top; }

			if(useWindow) {
				if(!w.GetNormalStateRect(out r)) return false;
			}

			if(!bRawXY) {
				wid = r.right - r.left; hei = r.bottom - r.top;
				xmax = rs.right - wid; ymax = rs.bottom - hei;
				if(bJustMoveIntoScreen) {
					x = r.left;
					y = r.top;
				} else {
					if(x0 < 0) x = xmax + x0; else if(x0 == 0) x = (rs.left + rs.right - wid) / 2; else x = rs.left + x0;
					if(y0 < 0) y = ymax + y0; else if(y0 == 0) y = (rs.top + rs.bottom - hei) / 2; else y = rs.top + y0;
				}

				x = Max(Min(x, xmax), rs.left);
				y = Max(Min(y, ymax), rs.top);
			}

			r.Offset(x - r.left, y - r.top);

			if(bCanLimitSize) {
				if(r.right > rs.right) r.right = rs.right;
				if(r.bottom > rs.bottom) r.bottom = rs.bottom;
			}

			if(useWindow) { //move window
				Api.WINDOWPLACEMENT wp;
				if(!w.GetWindowPlacement(out wp)) return false;
				bool moveMaxWindowToOtherMonitor = wp.showCmd == Api.SW_SHOWMAXIMIZED && !scr.Equals(Screen_.FromWindow(w));
				if(r == wp.rcNormalPosition && !moveMaxWindowToOtherMonitor) return true;

				Wnd hto = Wnd0; bool visible = w.Visible;
				try {
					//Windows bug: before a dialog is first time shown, may fail to move if it has an owner window. Depends on coordinates and on don't know what.
					//There are several workarounds. The best of them - temporarily set owner window 0.
					if(!visible) {
						hto = w.Owner;
						if(!hto.Is0) w.Owner = Wnd0;
					}

					wp.rcNormalPosition = r;
					wp.showCmd = visible ? Api.SW_SHOWNA : Api.SW_HIDE;
					if(!w.SetWindowPlacement(ref wp)) return false;

					if(moveMaxWindowToOtherMonitor) {
						//I found this way of moving max window to other screen by experimenting.
						//When moved to screen's coordinates and sized to screen's work area size, Windows adjusts window pos to be correct, ie border is outside screen, but invisible in adjacent screen.
						//Must call SetWindowPos twice, or it may refuse to move at all.
						//Another way - use SetWindowPlacement to temporarily restore, move to other screen, then maximize. But it unhides hidden window.
						rs = scr.WorkingArea;
						if(!(w._Move(rs.left, rs.top) && w._Resize(rs.Width, rs.Height))) return false;
					}
				}
				finally {
					if(!hto.Is0) w.Owner = hto;
				}

				Time.AutoDelay(w);
			}

			return true;
		}

		/// <summary>
		/// Moves this window to coordinates x y in specified screen, and ensures that entire window is in screen.
		/// By default, 0 and negative x y are interpreted as: 0 - screen center, ˂0 - relative to the right or bottom edge of the screen.
		/// </summary>
		/// <param name="x">X coordinate in the specified screen.</param>
		/// <param name="y">Y coordinate in the specified screen.</param>
		/// <param name="screen">Move to this screen. If null, use screen of this window. The same as with Screen_.FromObject().</param>
		/// <param name="workArea">Use the work area, not whole screen.</param>
		/// <param name="limitSize">If window is bigger than screen, resize it.</param>
		/// <param name="rawXY">Don't interpret 0 and negative x y in a special way. They are relative to the top-left corner of the screen.</param>
		/// <remarks>
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately, except when moving maximized to another screen.
		/// Returns false if fails. Supports ThreadError.
		/// Applies auto-delay, except when this window is of this thread.
		/// </remarks>
		/// <seealso cref="Wnd.Misc.RectMoveInScreen"/>
		public bool MoveInScreen(int x, int y, object screen = null, bool workArea = true, bool limitSize = false, bool rawXY = false)
		{
			var r = new RECT(x, y, x, y, false);
			return _MoveInScreen(true, this, ref r, screen, workArea, false, limitSize, rawXY);
		}

		/// <summary>
		/// Moves this window if need, to ensure that entire window is in screen.
		/// </summary>
		/// <param name="screen">Move to this screen. If null, use screen of this window. The same as with Screen_.FromObject().</param>
		/// <param name="workArea">Use the work area, not whole screen.</param>
		/// <param name="limitSize">If window is bigger than screen, resize it.</param>
		/// <remarks>
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately.
		/// Returns false if fails. Supports ThreadError.
		/// Applies auto-delay, except when this window is of this thread.
		/// </remarks>
		/// <seealso cref="Wnd.Misc.RectEnsureInScreen"/>
		public bool EnsureInScreen(object screen = null, bool workArea = true, bool limitSize = false)
		{
			var r = new RECT();
			return _MoveInScreen(true, this, ref r, screen, workArea, true, limitSize, false);
		}

		/// <summary>
		/// Moves this window to the center of the screen.
		/// Calls MoveInScreen(0, 0, screen, workArea). Also makes not minimized/maximized/hidden.
		/// </summary>
		/// <param name="screen">Move to this screen. If null, use screen of this window. The same as with Screen_.FromObject().</param>
		/// <param name="workArea">Use the work area, not whole screen.</param>
		/// <remarks>
		/// Returns false if fails. Supports ThreadError.
		/// Applies auto-delay, except when this window is of this thread.
		/// </remarks>
		/// <seealso cref="Wnd.Misc.RectMoveInScreen"/>
		public bool MoveToScreenCenter(object screen = null, bool workArea = true)
		{
			return ShowNotMinMax(HowMinMax.NoAnimation) && MoveInScreen(0, 0, screen, workArea);
		}

		public static partial class Misc
		{
			/// <summary>
			/// Adjusts a rectangle so that it can be used to create a new window that will be entirely in specified screen.
			/// By default, 0 and negative x y (r.left r.top) are interpreted as: 0 - screen center, ˂0 - relative to the right or bottom edge of the screen.
			/// </summary>
			/// <param name="r">The rectangle. Initially must contain rectangle coordinates relative to the specified screen. The function replaces them with normal coordinates relative to the primary screen.</param>
			/// <param name="screen">Use this screen. If null, use the primary screen. The same as with Screen_.FromObject().</param>
			/// <param name="workArea">Use the work area, not whole screen.</param>
			/// <param name="limitSize">If rectangle is bigger than screen, resize it.</param>
			/// <param name="rawXY">Don't interpret 0 and negative x y in a special way. They are relative to the top-left corner of the screen.</param>
			public static void RectMoveInScreen(ref RECT r, object screen = null, bool workArea = true, bool limitSize = false, bool rawXY = false)
			{
				_MoveInScreen(false, Wnd0, ref r, screen, workArea, false, limitSize, rawXY);
			}

			/// <summary>
			/// Adjusts a rectangle so that it can be used to create a new window that will be entirely in screen.
			/// Similar to RectMoveInScreen(), but initial r rectangle coordinates are relative to the primary screen and are not interpreted in a special way.
			/// The function adjusts rectangle coordinates to ensure that whole rectangle is in screen.
			/// </summary>
			/// <param name="r">The rectangle.</param>
			/// <param name="screen">Use this screen. If null, use screen of the rectangle. The same as with Screen_.FromObject().</param>
			/// <param name="workArea">Use the work area, not whole screen.</param>
			/// <param name="limitSize">If rectangle is bigger than screen, resize it.</param>
			public static void RectEnsureInScreen(ref RECT r, object screen = null, bool workArea = true, bool limitSize = false)
			{
				_MoveInScreen(false, Wnd0, ref r, screen, workArea, true, limitSize, false);
			}
		}

		/// <summary>
		/// Gets Screen object of the screen that contains this window (the biggest part of it) or is nearest to it.
		/// If this window handle is Wnd0 or invalid, gets the primary screen.
		/// The same as Screen_.FromWindow().
		/// </summary>
		public Screen Screen
		{
			get { return Screen_.FromWindow(this); }
		}

		#endregion

		#region Zorder
		const uint _SWP_ZORDER = Api.SWP_NOMOVE | Api.SWP_NOSIZE | Api.SWP_NOACTIVATE;

		/// <summary>
		/// Places this window after another window in the Z order. If it is a control - after another control.
		/// Also can make this window topmost or non-topmost, depending on where w is in the Z order.
		/// Supports ThreadError.
		/// If w is Wnd0, calls ZorderTop().
		/// </summary>
		public bool ZorderAfter(Wnd w)
		{
			if(w.Is0) return ZorderTop();
			if(!w.IsTopmost) ZorderNotopmost(); //see comments below
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, w);
		}
		/// <summary>
		/// Places this window before another window in the Z order. If it is a control - before another control.
		/// Also can make this window topmost or non-topmost, depending on where w is in the Z order.
		/// Supports ThreadError.
		/// If w is Wnd0, calls ZorderTop().
		/// </summary>
		public bool ZorderBefore(Wnd w)
		{
			return ZorderAfter(Get.PreviousSibling(w));
		}
		/// <summary>
		/// Places this window or control at the top of its Z order.
		/// If the window was topmost, it will be at the top of topmost windows, else at the top of non-topmost windows (after topmost windows).
		/// Does not activate.
		/// Supports ThreadError.
		/// </summary>
		public bool ZorderTop()
		{
			Wnd wa = Misc.SpecHwnd.Top;
			if(!IsChildWindow) {
				//SWP does not work if this window is inactive, unless wndInsertAfter is used
				if(IsTopmost) {
					//insert this after the first window, then insert the first window after this
					wa = Get.FirstToplevel();
					return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, wa) && wa.SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, this);
				} else {
					//insert after the last topmost window. This window does not become topmost.
					for(wa = this; !wa.Is0;) {
						wa = Api.GetWindow(wa, Api.GW_HWNDPREV);
						if(wa.IsTopmost) break;
					}
				}
			}
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, wa);
		}
		/// <summary>
		/// Places this window or control at the bottom of its Z order.
		/// If the window was topmost, makes it and its owner window non-topmost.
		/// Supports ThreadError.
		/// </summary>
		public bool ZorderBottom()
		{
			ZorderNotopmost(); //see comments below
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Misc.SpecHwnd.Bottom);
		}
		/// <summary>
		/// Makes this window topmost (always on top of non-topmost windows in the Z order).
		/// Does not activate.
		/// If this window has an owner wndow, the owner does not become topmost.
		/// Supports ThreadError.
		/// </summary>
		public bool ZorderTopmost()
		{
			return SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Misc.SpecHwnd.Topmost);
		}
		/// <summary>
		/// Makes this window non-topmost.
		/// If this window has an owner wndow, makes the owner window non-topmost too.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="afterActiveWindow">Also place this window after the active nontopmost window in the Z order, unless the active window is its owner.</param>
		public bool ZorderNotopmost(bool afterActiveWindow = false)
		{
			if(!IsTopmost) return true;

			for(int i = 0; i < 4; i++) {
				if(!SetWindowPos(_SWP_ZORDER, 0, 0, 0, 0, Misc.SpecHwnd.NoTopmost)) return false;
				if(i == 0 && !IsTopmost) break;
				//Out("retry");
			}

			//place this after the active window
			if(afterActiveWindow) {
				Wnd wa = ActiveWindow;
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
		public bool IsTopmost { get { return HasExStyle(Api.WS_EX_TOPMOST); } }

		/// <summary>
		/// Returns true if this window is above window w in the Z order.
		/// </summary>
		public bool IsZorderedBefore(Wnd w)
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
		/// It is a combination of Api.WS_x flags, documented in MSDN, "Window Styles" topic.
		/// See also: HasStyle().
		/// Supports ThreadError.
		/// </summary>
		public uint Style
		{
			get { return GetWindowLong(Api.GWL_STYLE); }
		}

		/// <summary>
		/// Gets window extended style.
		/// It is a combination of Api.WS_EX_x flags, documented in MSDN, "Extended Window Styles" topic.
		/// See also: HasExStyle().
		/// Supports ThreadError.
		/// </summary>
		public uint ExStyle
		{
			get { return GetWindowLong(Api.GWL_EXSTYLE); }
		}

		/// <summary>
		/// Returns true if the window has all specified style flags (Api.WS_x, documented in MSDN, "Window Styles" topic).
		/// Supports ThreadError.
		/// </summary>
		public bool HasStyle(uint style)
		{
			return (Style & style) == style;
		}

		/// <summary>
		/// Returns true if the window has all specified extended style flags (Api.WS_EX_x, documented in MSDN, "Extended Window Styles" topic).
		/// Supports ThreadError.
		/// </summary>
		public bool HasExStyle(uint exStyle)
		{
			return (ExStyle & exStyle) == exStyle;
		}

		/// <summary>
		/// Changes window style.
		/// SetStyle sets style as specified. Adding/removing some style bits is easier with SetStyleAdd/SetStyleRemove.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="style">One or more WS_x flags. Documented in MSDN, "Window Styles" topic.</param>
		/// <param name="updateNC">Update non-client area (frame, title bar).</param>
		/// <param name="updateClient">Update client area.</param>
		public bool SetStyle(uint style, bool updateNC = false, bool updateClient = false) { return _SetStyle(style, 0, false, updateNC, updateClient); }
		public bool SetStyleAdd(uint style, bool updateNC = false, bool updateClient = false) { return _SetStyle(style, 1, false, updateNC, updateClient); }
		public bool SetStyleRemove(uint style, bool updateNC = false, bool updateClient = false) { return _SetStyle(style, 2, false, updateNC, updateClient); }

		/// <summary>
		/// Changes window extended style.
		/// SetExStyle sets extended style as specified. Adding/removing some extended style bits is easier with SetExStyleAdd/SetExStyleRemove.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="style">One or more WS_x flags. Documented in MSDN, "Extended Window Styles" topic.</param>
		/// <param name="updateNC">Update non-client area (frame, title bar).</param>
		/// <param name="updateClient">Update client area.</param>
		public bool SetExStyle(uint style, bool updateNC = false, bool updateClient = false) { return _SetStyle(style, 0, true, updateNC, updateClient); }
		public bool SetExStyleAdd(uint style, bool updateNC = false, bool updateClient = false) { return _SetStyle(style, 1, true, updateNC, updateClient); }
		public bool SetExStyleRemove(uint style, bool updateNC = false, bool updateClient = false) { return _SetStyle(style, 2, true, updateNC, updateClient); }

		bool _SetStyle(uint style, uint how, bool exStyle = false, bool updateNC = false, bool updateClient = false)
		{
			int gwl = exStyle ? Api.GWL_EXSTYLE : Api.GWL_STYLE;
			if(how != 0) {
				uint pstyle = GetWindowLong(gwl);
				if(how == 1) style |= pstyle;
				else if(how == 2) style = pstyle & ~style;
				else style = pstyle ^ style;
			}

			if(SetWindowLong(gwl, (int)style) == 0 && ThreadError.IsError) return false;

			if(updateNC) SetWindowPos(Api.SWP_FRAMECHANGED | Api.SWP_NOMOVE | Api.SWP_NOSIZE | Api.SWP_NOZORDER | Api.SWP_NOOWNERZORDER | Api.SWP_NOACTIVATE);
			if(updateClient) Api.InvalidateRect(this, Zero, true);

			return true;
		}

		#endregion

		#region window/class long, control id, prop

		static partial class _Api
		{
			[DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
			internal static extern int GetWindowLong32(Wnd hWnd, int nIndex);

			[DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
			internal static extern LPARAM GetWindowLong64(Wnd hWnd, int nIndex);

			[DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
			internal static extern int SetWindowLong32(Wnd hWnd, int nIndex, int dwNewLong);

			[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
			internal static extern LPARAM SetWindowLong64(Wnd hWnd, int nIndex, LPARAM dwNewLong);

			[DllImport("user32.dll", EntryPoint = "GetClassLongW", SetLastError = true)]
			internal static extern int GetClassLong32(Wnd hWnd, int nIndex);

			[DllImport("user32.dll", EntryPoint = "GetClassLongPtrW", SetLastError = true)]
			internal static extern LPARAM GetClassLong64(Wnd hWnd, int nIndex);
		}

		/// <summary>
		/// Calls API GetWindowLong if this process is 32-bit, GetWindowLongPtr if 64-bit.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="index">One of Api.GWL_x or Api.DWL_x.</param>
		/// <remarks>
		/// All GWL_ values are the same in 32-bit and 64-bit process. Some DWL_ values are different, therefore they are defined as properties, not as constants.
		/// </remarks>
		public LPARAM GetWindowLong(int index)
		{
			Api.SetLastError(0);
			LPARAM R;
			if(IntPtr.Size == 8) R = _Api.GetWindowLong64(this, index); else R = _Api.GetWindowLong32(this, index);
			if(R == 0) ThreadError.SetWinError();
			return R;
		}

		/// <summary>
		/// Calls API SetWindowLong if this process is 32-bit, SetWindowLongPtr if 64-bit.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="index">One of Api.GWL_x or Api.DWL_x.</param>
		/// <remarks>
		/// All GWL_ values are the same in 32-bit and 64-bit process. Some DWL_ values are different, therefore they are defined as properties, not as constants.
		/// </remarks>
		public LPARAM SetWindowLong(int index, LPARAM newValue)
		{
			Api.SetLastError(0);
			LPARAM R;
			if(IntPtr.Size == 8) R = _Api.SetWindowLong64(this, index, newValue); else R = _Api.SetWindowLong32(this, index, newValue);
			if(R == 0) ThreadError.SetWinError();
			return R;
		}

		/// <summary>
		/// Calls API GetClassLong if current process is 32-bit, GetClassLongPtr if 64-bit.
		/// Supports ThreadError.
		/// </summary>
		/// <param name="index">One of Api.GCL_x or Api.GCW_ATOM.</param>
		/// <remarks>
		/// All GCL_/GCW_ values are the same in 32-bit and 64-bit process.
		/// </remarks>
		public LPARAM GetClassLong(int index)
		{
			Api.SetLastError(0);
			LPARAM R;
			if(IntPtr.Size == 8) R = _Api.GetClassLong64(this, index); else R = _Api.GetClassLong32(this, index);
			if(R == 0) ThreadError.SetWinError();
			return R;
		}

		/// <summary>
		/// Gets or sets id of this control.
		/// <para>The 'get' function calls Api.GetDlgCtrlID.</para>
		/// <para>The 'set' function calls SetWindowLong(Api.GWL_ID).</para>
		/// Supports ThreadError.
		/// </summary>
		public int ControlId
		{
			get { int R = Api.GetDlgCtrlID(this); if(R == 0) ThreadError.SetWinError(); return R; }
			set { SetWindowLong(Api.GWL_ID, value); }
		}

		/// <summary>
		/// Calls Api.GetProp() and returns its return value.
		/// More info in MSDN, GetProp topic.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		public LPARAM GetProp(string name)
		{
			return Api.GetProp(this, name);
		}
		/// <summary>
		/// Calls Api.GetProp() and returns its return value.
		/// More info in MSDN, GetProp topic.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		public LPARAM GetProp(ushort atom)
		{
			return Api.GetProp(this, atom);
			//note: cannot use ThreadError because GetLastError returns 0 when using atom that exists somewhere else.
		}

		/// <summary>
		/// Calls Api.SetProp() and returns its return value.
		/// More info in MSDN, SetProp topic.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		/// <param name="value">Property value. Can be a handle or an integer value.</param>
		public bool SetProp(string name, LPARAM value)
		{
			return Api.SetProp(this, name, value);
		}
		/// <summary>
		/// Calls Api.SetProp() and returns its return value.
		/// More info in MSDN, SetProp topic.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		/// <param name="value">Property value. Can be a handle or an integer value.</param>
		public bool SetProp(ushort atom, LPARAM value)
		{
			return Api.SetProp(this, atom, value);
		}

		/// <summary>
		/// Calls Api.RemoveProp() and returns its return value.
		/// More info in MSDN, RemoveProp topic.
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		public LPARAM RemoveProp(string name)
		{
			return Api.RemoveProp(this, name);
		}
		/// <summary>
		/// Calls Api.RemoveProp() and returns its return value.
		/// More info in MSDN, RemoveProp topic.
		/// </summary>
		/// <param name="atom">Property name atom in the global atom table.</param>
		public LPARAM RemoveProp(ushort atom)
		{
			return Api.RemoveProp(this, atom);
		}

		/// <summary>
		/// Gets list of window properties (see SetProp, GetProp, RemoveProp).
		/// Calls Api.EnumPropsEx().
		/// </summary>
		public Dictionary<string, LPARAM> GetPropList()
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

		#region thread, process, isUnicode, is 64-bit, is hung/ghost, is console, UAC

		/// <summary>
		/// Calls Api.GetWindowThreadProcessId().
		/// Supports ThreadError.
		/// </summary>
		public uint GetThreadAndProcessId(out uint processId)
		{
			processId = 0;
			uint R = Api.GetWindowThreadProcessId(this, out processId);
			if(R == 0) ThreadError.SetWinError();
			return R;
		}
		/// <summary>
		/// Calls Api.GetWindowThreadProcessId() and returns thread id.
		/// Supports ThreadError.
		/// </summary>
		public uint ThreadId { get { uint pid; return GetThreadAndProcessId(out pid); } }
		/// <summary>
		/// Calls Api.GetWindowThreadProcessId() and returns process id.
		/// Supports ThreadError.
		/// </summary>
		public uint ProcessId { get { uint pid = 0; GetThreadAndProcessId(out pid); return pid; } }
		/// <summary>
		/// Returns true if this window belongs to the current thread.
		/// Supports ThreadError.
		/// </summary>
		public bool IsOfThisThread { get { return Api.GetCurrentThreadId() == ThreadId; } }
		/// <summary>
		/// Returns true if this window belongs to the current process.
		/// Supports ThreadError.
		/// </summary>
		public bool IsOfThisProcess { get { return Api.GetCurrentProcessId() == ProcessId; } }

		/// <summary>
		/// Returns true if the window is a Unicode window.
		/// Returns false if the window is an ANSI window or if fails (eg the handle is invalid).
		/// </summary>
		public bool IsUnicode { get { return Api.IsWindowUnicode(this); } }

		/// <summary>
		/// Returns true if the window is of a 64-bit process.
		/// Returns false if the window is of a 32-bit process or if fails (eg the handle is invalid).
		/// Supports ThreadError.
		/// If you know that the window belongs to current process, instead use Environment.Is64BitProcess or IntPtr.Size==8.
		/// See also: Environment.Is64BitOperatingSystem.
		/// </summary>
		public bool Is64Bit
		{
			get
			{
				if(Environment.Is64BitOperatingSystem) {
					uint pid = ProcessId; if(pid == 0) return false;
					IntPtr ph = Zero; int is32bit;
					try {
						ph = Api.OpenProcess(Api.PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
						if(ph == Zero || !Api.IsWow64Process(ph, out is32bit)) return ThreadError.SetWinError();
						if(is32bit == 0) return true;
					}
					finally {
						if(ph != Zero) Api.CloseHandle(ph);
					}
				}
				ThreadError.Clear();
				return false;

				//info: don't use Process.GetProcessById, it does not have an desiredAccess parameter and fails with higher IL processes.
			}
		}

		/// <summary>
		/// Returns true if thread of this window is considered hung (not responding).
		/// Calls Api.IsHungAppWindow().
		/// </summary>
		public bool IsHung { get { return Api.IsHungAppWindow(this); } }

		/// <summary>
		/// Returns true if the window is a ghost window that the system creates over a hung (not responding) window to allow the user to minimally interact with it.
		/// </summary>
		public bool IsHungGhost
		{
			get
			{
				//Class is "Ghost", exe is "DWM" (even if no Aero), text sometimes ends with "(Not Responding)".
				//IsHungWindow returns true for ghost window, although it is not actually hung. It is the fastest.
				return IsHung && ClassNameIs("Ghost") && ProcessName.Equals_("DWM", true);
			}
		}

		/// <summary>
		/// Returns true if this is a console window (class name "ConsoleWindowClass").
		/// </summary>
		public bool IsConsole { get { return ClassNameIs("ConsoleWindowClass"); } }

		/// <summary>
		/// Returns true if UAC would not allow to automate the window.
		/// It happens when current process has lower UAC integrity level and is not uiAccess, unless UAC is turned off.
		/// </summary>
		public bool IsUacAccessDenied
		{
			get
			{
				//with this is faster when don't need other code, but possibly less reliable, and without this is fast enough
				//if(Process_.UacInfo.IsUacDisabled) return false;
				//var t = Process_.UacInfo.ThisProcess;
				//if(t.IsUIAccess) return false;

				Api.SetLastError(0);
				Api.RemoveProp(this, 0);
				return Marshal.GetLastWin32Error() == Api.ERROR_ACCESS_DENIED; //documented
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
		//	get { return UacIntegrityLevel > Process_.UacInfo.ThisProcess.IntegrityLevel; }
		//}

		#endregion

		#region text, class, isofclass, process name

		static partial class _Api
		{
			[DllImport("user32.dll", EntryPoint = "GetClassNameW", SetLastError = true)]
			public static extern unsafe int GetClassName(Wnd hWnd, [Out] char* lpClassName, int nMaxCount);

			[DllImport("user32.dll", EntryPoint = "InternalGetWindowText", SetLastError = true)]
			public static extern unsafe int InternalGetWindowText(Wnd hWnd, [Out] char* pString, int cchMaxCount);

			[DllImport("user32.dll", EntryPoint = "InternalGetWindowText", SetLastError = true)]
			public static extern int InternalGetWindowTextSB(Wnd hWnd, [Out] StringBuilder pString, int cchMaxCount);
		}

		/// <summary>
		/// Gets class name.
		/// Returns null if fails, eg if the window is closed.
		/// Supports ThreadError.
		/// </summary>
		public unsafe string ClassName
		{
			get
			{
				const int stackSize = 260;
				var b = stackalloc char[stackSize];
				int n = _Api.GetClassName(this, b, stackSize);
				if(n > 0) return new string(b, 0, n);
				ThreadError.SetWinError();
				return null;
				//speed: 320. It is 50% slower than QM2 str.getwinclass (with conversion to UTF8 and free()); slightly faster than with char[260] or fixed char[260]; with StringBuilder 70% slower; with Marshal.AllocHGlobal 30% slower.
				//Calling through delegate (Marshal.GetDelegateForFunctionPointer) does not make faster.
			}
		}

		/// <summary>
		/// Gets or sets window name or control text.
		/// The 'get' function returns "" if the text is empty. Returns null if fails, eg if the window is closed.
		/// </summary>
		/// <remarks>
		/// Each top-level window and control has a text property, although the text can be empty. This function gets or sets that text.
		/// If this is a top-level window, it is the title bar text.
		/// If control, it is the text displayed in the control, eg button text, static text, edit control text. However most other controls don't have text that we can get with this function, even if they display text.
		/// The 'get' function calls Api.InternalGetWindowText(). If it returns empty text and this is a control, calls GetControlText() with default parameter values. The later function is slow; to avoid it use GetControlName() instead of Name.
		/// The 'set' function calls SetControlText(), which sends WM_SETTEXT message with 5000 ms timeout; on timeout does nothing.
		/// Note: it is not the .NET Control.Name.
		/// Supports ThreadError.
		/// </remarks>
		public string Name
		{
			get { return _GetName(true); }
			set { SetControlText(value); }
		}

		/// <summary>
		/// Gets control name.
		/// Returns "" if it is empty. Returns null if fails, eg if the window is destroyed or hung.
		/// </summary>
		/// <param name="removeMnemonic">Remove '&amp;' characters that are used to underline next character when using the keyboard to select controls.</param>
		/// <remarks>
		/// Similar to the Name property, and in most cases will get the same value.
		/// Unlike Name, does not call the slow function GetControlText() when Api.InternalGetWindowText() returns empty text.
		/// Can be used with top-level windows too.
		/// Supports ThreadError.
		/// </remarks>
		public string GetControlName(bool removeMnemonic = false)
		{
			string R = _GetName(false);
			if(removeMnemonic && !Empty(R)) Util.Misc.StringRemoveMnemonicUnderlineAmpersand(ref R);
			return R;
		}

		unsafe string _GetName(bool getControlTextIfEmpty)
		{
			const int stackSize = 1024;

			var b = stackalloc char[stackSize];
			Api.SetLastError(0);
			int nt = _Api.InternalGetWindowText(this, b, stackSize);
			if(nt < 1) {
				if(ThreadError.SetIfWinError() != 0) return null;
				if(getControlTextIfEmpty && (Style & Api.WS_CHILD) != 0) return GetControlText();
				return "";
			}
			if(nt < stackSize - 1) return new string(b, 0, nt);

			var sb = new StringBuilder();
			for(int na = stackSize; na <= int.MaxValue / 4;) {
				na *= 2;
				sb.Capacity = na;
				Api.SetLastError(0);
				nt = _Api.InternalGetWindowTextSB(this, sb, na);
				if(nt < na - 1) {
					if(nt > 0) return sb.ToString();
					return (ThreadError.SetIfWinError() == 0) ? "" : null;
				}
			}

			return null;

			//speed: 320. Faster than QM2 str.getwintext (with conversion to UTF8 and free()).
		}

		/// <summary>
		/// Gets control text.
		/// Returns "" if it is empty. Returns null if fails, eg if the control is destroyed or its thread is hung.
		/// </summary>
		/// <param name="timeoutMS">Fail if the window is of another thread and does not respond in timeoutMS ms (default 5000). Does not wait if the thread is already known as hung.</param>
		/// <remarks>
		/// Similar to the Name property, and in most cases will get the same value.
		/// Unlike Name, does not try to use the fast API function InternalGetWindowText(), which in some cases can get unexpected value.
		/// Sends message WM_GETTEXT. It is much slower than InternalGetWindowText(), especially when the window is of another thread.
		/// Can be used with top-level windows too.
		/// Supports ThreadError.
		/// </remarks>
		public unsafe string GetControlText(int timeoutMS = 5000) //TODO: consider: Option.sendMessageTimeoutMS=1000 (min 500, 0 = no timeout). Then let these be properties: ControlText (get, set), ControlName.
		{
			const int stackSize = 1024;
			LPARAM na, nt;
			bool ofThisThread = IsOfThisThread;

			//At first try to get text without knowing length.
			//For most controls it makes faster (60 -> 40 mcs), and only for big-text controls slower (85 mcs).

			var b = stackalloc char[stackSize];

			if(ofThisThread) nt = Send(Api.WM_GETTEXT, stackSize, b); //info: here cannot fail, because IsOfThisThread returns false if cannot get thread id.
			else if(!SendTimeout(timeoutMS, out nt, Api.WM_GETTEXT, stackSize, b)) return null;

			if(nt < stackSize - 1) {
				if(nt < 1) return "";
				nt = Util.Misc.CharPtrLength(b, nt); //some buggy controls return incorrect nt, even bigger than text length, or can contain '\0'
				return new string(b, 0, nt);
			}

			//Get length, alloc StrinBuilder...

			if(ofThisThread) nt = Send(Api.WM_GETTEXTLENGTH);
			else if(!SendTimeout(timeoutMS, out nt, Api.WM_GETTEXTLENGTH)) return null;
			if(nt < 1) return "";

			var sb = new StringBuilder(na = nt + 1);

			if(ofThisThread) nt = SendSB(Api.WM_GETTEXT, na, sb);
			else if(!SendTimeoutSB(timeoutMS, out nt, Api.WM_GETTEXT, na, sb)) return null;

			return sb.ToString(0, Min(nt, sb.Length)); //info: sb.Length is Min(na, text.IndexOf('\0'))

			//speed:
			//	If of same thread: same speed as getwindowtext. With sendtimeout 6 times slower.
			//	If of other thread: many times slower.

			//note: don't use Marshal.AllocX/PtrToStringUni/FreeX or HeapAlloc etc. Don't know why, it makes 2 times slower (50 -> 100 mcs), unless we don't free or don't use wm_gettextlength. Initially even crashed.
		}

		/// <summary>
		/// Sets control text.
		/// Returns false if fails, eg if the window is destroyed or its thread is hung.
		/// </summary>
		/// <param name="text">Text. Can be null, it is the same as "".</param>
		/// <param name="timeoutMS">Fail if the window is of another thread and does not respond in timeoutMS ms (default 5000). Does not wait if the thread is already known as hung.</param>
		/// <remarks>
		/// Sends message WM_SETTEXT.
		/// More info: see GetControlText().
		/// Can be used with top-level windows too.
		/// You can instead use the Name property, which calls this function with default timeout.
		/// Supports ThreadError.
		/// </remarks>
		public bool SetControlText(string text, int timeoutMS = 5000)
		{
			return SendTimeoutS(timeoutMS, Api.WM_SETTEXT, 0, text ?? "");
		}

		/// <summary>
		/// Returns true if the class name of this window matches className.
		/// String by default is interpreted as wildcard, case-insensitive.
		/// Supports ThreadError.
		/// </summary>
		public bool ClassNameIs(WildStringI className)
		{
			return className.Match(ClassName);
		}

		/// <summary>
		/// If the class name of this window matches one of strings in classNames, returns 1-based index of the string. Else returns 0.
		/// Strings by default are interpreted as wildcard, case-insensitive.
		/// Supports ThreadError.
		/// </summary>
		public int ClassNameIs(params WildStringI[] classNames)
		{
			string cn = ClassName; if(cn == null) return 0;
			for(int i = 0; i < classNames.Length; i++) if(classNames[i].Match(cn)) return i + 1;
			ThreadError.Clear();
			return 0;
		}

		/// <summary>
		/// Gets process executable file name without ".exe".
		/// Return null if fails.
		/// See also: ProcessId, Process_.GetProcessesByName.
		/// </summary>
		public string ProcessName { get { return Process_.GetProcessName(ProcessId); } }

		/// <summary>
		/// Gets process executable file full path.
		/// Return null if fails.
		/// See also: ProcessId, Process_.GetProcessesByName.
		/// </summary>
		public string ProcessPath { get { return Process_.GetProcessName(ProcessId, true); } }

		#endregion

		#region close, destroy

		/// <summary>
		/// Tries to close the window.
		/// The window may refuse to be closed. For example, it may be hung, or hide itself instead, or display a "Save?" message box, or simply need more time until it is finally destroyed.
		/// This function waits a while until the window is destroyed or hidden or disabled, but does not wait indefinitely. Also applies auto-delay.
		/// If finally the window is destroyed (or was already destroyed when calling this function), sets this=Wnd0 and returns true.
		/// Else returns false (although in many cases the window is already hidden and will be destroyed soon, and another window is activated).
		/// If the window is of this thread, just sends (not posts) Api.WM_CLOSE or Api.WM_SYSCOMMAND and does not wait.
		/// </summary>
		/// <param name="closeLikeUser">
		/// If true, uses Api.WM_SYSCOMMAND, like when the user clicks the Close button in the title bar.
		/// If false, uses Api.WM_CLOSE, which is more often used in programming code.
		/// If null, uses Api.WM_SYSCOMMAND or Api.WM_CLOSE depending on window style.
		/// Most windows can be closed with any of these messages, but some respond properly only to one of them.
		/// </param>
		/// <seealso cref="Destroy"/>
		public bool Close(bool? closeLikeUser = null)
		{
			if(IsValid) {
				bool useSysCmd;
				if(closeLikeUser != null) useSysCmd = closeLikeUser.Value;
				else {
					uint style = Style;
					useSysCmd = (style & (Api.WS_SYSMENU | Api.WS_VISIBLE | Api.WS_CHILD | Api.WS_POPUP)) == (Api.WS_SYSMENU | Api.WS_VISIBLE);
				}

				if(IsOfThisThread) {
					if(useSysCmd) Send(Api.WM_SYSCOMMAND, Api.SC_CLOSE); else Send(Api.WM_CLOSE);
				} else {
					//note: Don't use SendX messages because of possible anomalies. Some windows then are
					//hidden but not closed, and even next app instance does not start (eg Firefox). Possible
					//problems with windows that have toolbars. Possible crashes and other anomalies.

					//Some windows cannot be properly closed using only WM_CLOSE. For example, VS 7 window closed, but the app not closed.
					//Some windows (eg IE7), sometimes ignore SC_CLOSE if it is posted soon after starting the program.
					//When the user closes a window, Windows sends SC_CLOSE. DefWindowProc on SC_CLOSE sends WM_CLOSE, except if disabled.
					//We post SC_CLOSE to most nonpopup windows. However it may not work with some windows.
					//Not true: Therefore we also post WM_CLOSE. However then it is possible that the window will receive WM_CLOSE two times. For example, if on first WM_CLOSE (sent by Windows in response to SC_CLOSE) the app displays a "Save?" message box, then on second WM_CLOSE it may display another message box, or crash, etc. Therefore we wait a while, and dont use WM_CLOSE if the window is disabled or closed or hidden during that time.

					bool ok = useSysCmd ? Post(Api.WM_SYSCOMMAND, Api.SC_CLOSE) : Post(Api.WM_CLOSE);
					if(!ok) {
						//Out(Marshal.GetLastWin32Error()); //0 when UAC access denied
						if(useSysCmd || !Post(Api.WM_SYSCOMMAND, Api.SC_CLOSE)) goto g1; //UAC blocks WM_CLOSE but not WM_SYSCOMMAND
					}

					for(int i = 2, n = useSysCmd ? 100 : 10; i < n; i++) {
						WaitMS(Min(i, 10));
						if(!Visible || !Enabled) goto g1; //destroyed, hidden or disabled (most likely because displays a modal dialog box, eg "Save?")
						if(i > n / 2) {
							if(!SendTimeout(200, 0)) {
								if(!IsValid || IsHung) goto g1;
							}
						}
					}
					//if(useSysCmd) Post(Api.WM_CLOSE); //too dirty, better fail
					g1:
					Time.AutoDelay();
					Misc.WaitForAnActiveWindow();
				}
				if(IsValid) return false;
			}
			_h = Zero;
			return true;
		}

		/// <summary>
		/// Closes all matching windows.
		/// Calls Wnd.FindAll(). All parameters etc are the same. Then calls Close() for each found window.
		/// </summary>
		public static void CloseAll(
			WildString name, WildStringI className = null, WildStringI program = null,
			WinFlag flags = 0, WinProp prop = null, Action<WndCallbackArgs> f = null
			)
		{
			foreach(Wnd w in FindAll(name, className, program, flags, prop, f)) w.Close();
		}

		/// <summary>
		/// Destroys the window, which must be of this thread and should not be a modal dialog.
		/// Calls Api.DestroyWindow(). If it succeeds, sets this=Wnd0 and returns true. Else returns false.
		/// </summary>
		/// <exception cref="CatkeysException">If the window is not of this thread.</exception>
		/// <seealso cref="Close"/>
		public bool Destroy()
		{
			if(Api.DestroyWindow(this)) { _h = Zero; return true; }
			uint tid = ThreadId;
			if(tid != 0 && tid != Api.GetCurrentThreadId()) throw new CatException("Window of other thread. Use Close.");
			return false;
		}

		#endregion

	}

}
