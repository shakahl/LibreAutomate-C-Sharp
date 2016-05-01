using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
using System.Windows.Forms;
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
	/// Wnd functions don't throw error when failed, unless explicitly specified that the function throws error. Instead they call SetLastError(0) before calling API functions that set a non-zero error code if failed. When a function returns something that does not indicate success (eg 0 or false or void), you can call Marshal.GetLastWin32Error(), it will return the error code if the function failed, or 0 if not failed.
	/// </remarks>
	public partial struct Wnd :IWin32Window
	{
		IntPtr _h;

		#region constructors, operators, overrides, constants

		Wnd(IntPtr hwnd) { _h = hwnd; } //don't need public

		public static explicit operator Wnd(IntPtr hwnd) { return new Wnd(hwnd); } //Wnd=(Wnd)IntPtr //don't need implicit, it creates more problems than is useful
		public static explicit operator IntPtr(Wnd w) { return w._h; } //IntPtr=(IntPtr)Wnd //could be implicit, but then problems with operator ==
		public static explicit operator Wnd(LPARAM hwnd) { return new Wnd(hwnd); } //Wnd=(Wnd)LPARAM
		public static explicit operator LPARAM(Wnd w) { return w._h; } //LPARAM=(LPARAM)Wnd
		public static implicit operator Wnd(Spec value) { return new Wnd((IntPtr)value); } //Wnd=Wnd.Spec
		public static explicit operator Wnd(int hwnd) { return new Wnd((IntPtr)hwnd); } //Wnd=(Wnd)int
		public static explicit operator Wnd(Control c) { return new Wnd(c == null ? Zero : c.Handle); } //Wnd=Control //implicit would allow Wnd==null
																										//public static explicit operator Control(Wnd w) { return Control.FromHandle(w._h); } //Control=(Control)Wnd. TODO: test, maybe does not work

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
			return _h.GetHashCode();
		}

		/// <summary>
		/// Implements IWin32Window.Handle.
		/// </summary>
		public IntPtr Handle { get { return _h; } }

		/// <summary>
		/// Can be used with Windows API as special window handle value that is implicitly converted to Wnd.
		/// Example: <c>SetWindowPos(w, Wnd.Spec.Topmost, ...); //HWND_TOPMOST</c>
		/// </summary>
		public enum Spec
		{
			Zero = 0, //or Wnd0
			Top = 0, //SetWindowPos(HWND_TOP)
			Bottom = 1, //SetWindowPos(HWND_BOTTOM)
			Topmost = -1, //SetWindowPos(HWND_TOPMOST)
			NoTopmost = -2, //SetWindowPos(HWND_NOTOPMOST)
			Message = -3, //CreateWindowEx(HWND_MESSAGE)
			Broadcast = 0xffff //SendMessage(HWND_BROADCAST)
		}

		#endregion

		#region Send message

		static partial class _Api
		{
			[DllImport("user32.dll", SetLastError = true)]
			public static extern LPARAM SendMessage(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam);

			[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
			public static extern LPARAM SendMessageS(Wnd hWnd, uint msg, LPARAM wParam, string lParam);

			[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
			public static extern LPARAM SendMessageSB(Wnd hWnd, uint msg, LPARAM wParam, [Out] StringBuilder lParam);

			public const int SMTO_BLOCK = 1;
			public const int SMTO_ABORTIFHUNG = 2;
			public const int SMTO_NOTIMEOUTIFNOTHUNG = 8;

			[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
			public static extern LPARAM SendMessageTimeout(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

			[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
			public static extern LPARAM SendMessageTimeoutS(Wnd hWnd, uint Msg, LPARAM wParam, string lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

			[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
			public static extern LPARAM SendMessageTimeoutSB(Wnd hWnd, uint Msg, LPARAM wParam, [Out] StringBuilder lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

			[DllImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
			public static extern bool PostMessage(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);
		}

		/// <summary>
		/// Calls API SendMessage.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public LPARAM Send(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
		{
			ResetLastError();
			return _Api.SendMessage(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API SendMessage where lParam is string.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public LPARAM SendS(uint message, LPARAM wParam, string lParam)
		{
			ResetLastError();
			return _Api.SendMessageS(this, message, wParam, lParam);
			//info: don't use overload, then eg ambiguous if null.
		}

		/// <summary>
		/// Calls API SendMessage where lParam is StringBuilder.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public LPARAM SendSB(uint message, LPARAM wParam, StringBuilder lParam)
		{
			ResetLastError();
			return _Api.SendMessageSB(this, message, wParam, lParam);
		}

		/// <summary>
		/// Calls API SendMessageTimeout.
		/// Uses flag SMTO_ABORTIFHUNG. If block==true, adds SMTO_BLOCK.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool SendTimeout(int timeoutMS, out LPARAM result, uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM), bool block = false)
		{
			result = Zero;
			uint fl = _Api.SMTO_ABORTIFHUNG; if(block) fl |= _Api.SMTO_BLOCK;
			ResetLastError();
			return 0 != _Api.SendMessageTimeout(this, message, wParam, lParam, fl, (uint)timeoutMS, out result);
		}

		/// <summary>
		/// Calls API SendMessageTimeout.
		/// Use this overload when you don't need the return value.
		/// Uses flag SMTO_ABORTIFHUNG. If block==true, adds SMTO_BLOCK.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool SendTimeout(int timeoutMS, uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM), bool block = false)
		{
			LPARAM result;
			ResetLastError();
			return SendTimeout(timeoutMS, out result, message, wParam, lParam, block);
		}

		/// <summary>
		/// Calls API SendMessageTimeout where lParam is string.
		/// Uses flag SMTO_ABORTIFHUNG. If block==true, adds SMTO_BLOCK.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool SendTimeoutS(int timeoutMS, out LPARAM result, uint message, LPARAM wParam, string lParam, bool block = false)
		{
			result = Zero;
			uint fl = _Api.SMTO_ABORTIFHUNG; if(block) fl |= _Api.SMTO_BLOCK;
			ResetLastError();
			return 0 != _Api.SendMessageTimeoutS(this, message, wParam, lParam, fl, (uint)timeoutMS, out result);
		}

		/// <summary>
		/// Calls API SendMessageTimeout where lParam is StringBuilder.
		/// Uses flag SMTO_ABORTIFHUNG. If block==true, adds SMTO_BLOCK.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool SendTimeoutSB(int timeoutMS, out LPARAM result, uint message, LPARAM wParam, StringBuilder lParam, bool block = false)
		{
			result = Zero;
			uint fl = _Api.SMTO_ABORTIFHUNG; if(block) fl |= _Api.SMTO_BLOCK;
			ResetLastError();
			return 0 != _Api.SendMessageTimeoutSB(this, message, wParam, lParam, fl, (uint)timeoutMS, out result);
		}

		/// <summary>
		/// Calls API PostMessage.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool Post(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
		{
			ResetLastError();
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
		/// Returns false if the window is destroyed or the handle is 0 or some other invalid or special value.
		/// Calls Is0 and Api.IsWindow().
		/// </summary>
		public bool IsValid { get { return !Is0 && Api.IsWindow(this); } }

		/// <summary>
		/// Throws exception if this.Is0==true or this.IsValid==false.
		/// </summary>
		public void Validate()
		{
			if(Is0) throw new CatkeysException("Window handle is 0. Possibly previous 'find window' function did not find a window with the specified name etc.");
			if(!IsValid) throw new CatkeysException("Invalid window handle. Possibly the window is closed/destroyed.");
		}

		/// <summary>
		/// Gets or sets the visible state.
		/// The 'get' function returns true if the window is visible; returns false if invisible, destroyed or the handle is 0. Calls Api.IsWindowVisible.
		/// The 'set' function shows or hides the window, without [de]activating it or changing the Z order. Calls Api.ShowWindow(SW_SHOWNA/SW_HIDE).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool Visible
		{
			get { ResetLastError(); return Api.IsWindowVisible(this); }
			//set { if(value != Visible) Api.ShowWindow(this, value ? Api.SW_SHOWNA : Api.SW_HIDE); } //would not hide if the parent window is currently hidden
			set { ResetLastError(); Api.ShowWindow(this, value ? Api.SW_SHOWNA : Api.SW_HIDE); }
		}

		/// <summary>
		/// Private Visible without last error management.
		/// </summary>
		bool _Visible
		{
			get { return Api.IsWindowVisible(this); }
			set { Api.ShowWindow(this, value ? Api.SW_SHOWNA : Api.SW_HIDE); }
		}

		/// <summary>
		/// Gets or sets the enabled state.
		/// The 'get' function returns true if the window is enabled; returns false if disabled, destroyed or the handle is 0. Calls Api.IsWindowEnabled.
		/// The 'set' function enables or disables the window, without [de]activating it or changing the Z order. Calls Api.EnableWindow.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool Enabled
		{
			get { ResetLastError(); return Api.IsWindowEnabled(this); }
			set { ResetLastError(); Api.EnableWindow(this, value); }
		}

		/// <summary>
		/// Gets the cloaked state.
		/// Returns 0 if not cloaked or if failed (eg the window is destroyed or Wnd0).
		/// Else returns flags: 1 cloaked by its application, 2 cloaked by Windows, 4 cloaked because its owner window is cloaked.
		/// On Windows 7 returns 0 because there is no "cloaked windows" feature.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public int CloakedState
		{
			get
			{
				if(WinVer < Win8_0) return 0;
				int cloaked = 0, hr = Api.DwmGetWindowAttribute(this, 14, out cloaked, 4); //DWMWA_CLOAKED
				Api.SetLastError((uint)hr);
				return cloaked;
			}
		}
		/// <summary>
		/// Returns true if the window is cloaked.
		/// Returns false if not cloaked or if failed to get the cloaked state (eg the window is destroyed or Wnd0).
		/// On Windows 7 returns false because there is no "cloaked windows" feature.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool IsCloaked
		{
			get { return CloakedState != 0; }
		}

		#endregion

		#region state (minimized, maximized, normal)

		/// <summary>
		/// Gets or sets minimized state.
		/// The 'get' function calls Api.IsIconic.
		/// The 'set' function is like Minimize() if true, and like RestoreMinimized() if false. Unlike these methods, it is visually fast, without animation. Calls Api.SetWindowPlacement.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool StateMinimized
		{
			get { ResetLastError(); return Api.IsIconic(this); }
			set { _SetStateFast(value ? Api.SW_MINIMIZE : Api.SW_RESTORE); }
		}

		/// <summary>
		/// Gets or sets maximized state.
		/// The 'get' function calls Api.IsZoomed.
		/// The 'set' function is like Maximize(), but visually fast, without animation. Calls Api.SetWindowPlacement.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool StateMaximized
		{
			get { ResetLastError(); return Api.IsZoomed(this); }
			set { _SetStateFast(value ? Api.SW_SHOWMAXIMIZED : Api.SW_SHOWNORMAL); }
		}

		/// <summary>
		/// Gets or sets normal (not minimized or maximized) state.
		/// The 'get' function calls Api.IsIconic and Api.IsZoomed.
		/// The 'set' function is like RestoreToNormal(), but visually fast, without animation. Calls Api.SetWindowPlacement.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool StateNormal
		{
			get { return !(StateMaximized || StateMinimized) && Marshal.GetLastWin32Error() == 0; }
			set { _SetStateFast(value ? Api.SW_SHOWNORMAL : Api.SW_SHOWMAXIMIZED); }
		}

		/// <summary>
		/// Sets window min/max/normal/restore state with Api.ShowWindow.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="state">Must be Api.SW_MINIMIZE, Api.SW_RESTORE (restores to normal/max if minimized), Api.SW_SHOWNORMAL or Api.SW_SHOWMAXIMIZED.</param>
		bool _SetState(int state)
		{
			Debug.Assert(state == Api.SW_MINIMIZE || state == Api.SW_RESTORE || state == Api.SW_SHOWNORMAL || state == Api.SW_SHOWMAXIMIZED);

			bool wasMinimized = StateMinimized;

			switch(state) {
			case Api.SW_MINIMIZE:
				if(wasMinimized) goto gr;
				break;
			case Api.SW_RESTORE:
				if(!wasMinimized) goto gr; //TODO: && !IsThreadError
				break;
			case Api.SW_SHOWNORMAL:
				if(StateNormal) goto gr;
				break;
			case Api.SW_SHOWMAXIMIZED:
				if(StateMaximized) goto gr;
				break;
			}

			ResetLastError();
			if(!Api.ShowWindow(this, state)) return false;

			_SetStateActivateWait(state, wasMinimized);

			return true;
			gr:
			Visible = true;
			return true;
		}

		/// <summary>
		/// Sets window min/max/normal/restore state with Api.SetWindowPlacement, without animation.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="state">Must be Api.SW_MINIMIZE, Api.SW_RESTORE (restores to normal/max if minimized), Api.SW_SHOWNORMAL or Api.SW_SHOWMAXIMIZED.</param>
		bool _SetStateFast(int state)
		{
			Debug.Assert(state == Api.SW_MINIMIZE || state == Api.SW_RESTORE || state == Api.SW_SHOWNORMAL || state == Api.SW_SHOWMAXIMIZED);

			Api.WINDOWPLACEMENT p;
			if(!GetWindowPlacement(out p)) return false;
			bool wasMinimized = (p.showCmd == Api.SW_SHOWMINIMIZED);

			switch(state) {
			case Api.SW_MINIMIZE:
				if(wasMinimized) goto gr;
				if(p.showCmd == Api.SW_SHOWMAXIMIZED) p.flags |= Api.WPF_RESTORETOMAXIMIZED; else p.flags &= ~Api.WPF_RESTORETOMAXIMIZED; //Windows forgets to remove the flag
				break;
			case Api.SW_RESTORE:
				if(!wasMinimized) goto gr;
				if((p.flags & Api.WPF_RESTORETOMAXIMIZED) != 0) state = Api.SW_SHOWMAXIMIZED; //without this would make normal
				break;
			case Api.SW_SHOWNORMAL:
				if(p.showCmd == Api.SW_SHOWNORMAL) goto gr;
				break;
			case Api.SW_SHOWMAXIMIZED:
				if(p.showCmd == Api.SW_SHOWMAXIMIZED) goto gr;
				break;
			}
			//if(wasMinimized) p.flags|=Api.WPF_ASYNCWINDOWPLACEMENT; //fixes Windows bug: if window of another thread, deactivates currently active window and does not activate this window. However then animates window. If we set this while the window is not minimized, it would set blinking caret in inactive window. 
			p.showCmd = state;
			if(!SetWindowPlacement(ref p)) return false;

			_SetStateActivateWait(state, wasMinimized);

			ResetLastError();
			return true;
			gr:
			Visible = true;
			return true;
		}

		/// <summary>
		/// Initializes wp and calls Api.GetWindowPlacement.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool GetWindowPlacement(out Api.WINDOWPLACEMENT wp)
		{
			wp = new Api.WINDOWPLACEMENT(); wp.length = Api.SizeOf(wp);
			ResetLastError();
			return Api.GetWindowPlacement(this, ref wp);
		}

		/// <summary>
		/// Sets wp.length and calls Api.SetWindowPlacement.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool SetWindowPlacement(ref Api.WINDOWPLACEMENT wp)
		{
			wp.length = Api.SizeOf(wp);
			ResetLastError();
			return Api.SetWindowPlacement(this, ref wp);
		}

		void _SetStateActivateWait(int state, bool wasMinimized)
		{
			if(IsOfThisThread) return;
			if(wasMinimized) ActivateRaw(); //fix Windows bug: if window of another thread, deactivates currently active window and does not activate this window
			else if(state == Api.SW_MINIMIZE) WaitForAnActiveWindow();
		}

		/// <summary>
		/// If not minimized, minimizes.
		/// Also unhides.
		/// </summary>
		/// <param name="useSysCmd">If true, uses Api.WM_SYSCOMMAND. If false, uses Api.ShowWindow(). If omitted/null, uses Api.WM_SYSCOMMAND or Api.ShowWindow(), depending on window style.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <seealso cref="StateMinimized"/>
		public void Minimize(bool? useSysCmd = null)
		{
			if(!Visible) Visible = true;
			if(StateMinimized) return;
			_MinMaxRes(Api.SW_MINIMIZE, useSysCmd);
		}

		/// <summary>
		/// If not maximized, maximizes.
		/// Also unhides.
		/// </summary>
		/// <param name="useSysCmd">If true, uses Api.WM_SYSCOMMAND. If false, uses Api.ShowWindow(). If omitted/null, uses Api.WM_SYSCOMMAND or Api.ShowWindow(), depending on window style.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <seealso cref="StateMaximized"/>
		public void Maximize(bool? useSysCmd = null)
		{
			if(!Visible) Visible = true;
			if(StateMaximized) return;
			_MinMaxRes(Api.SW_SHOWMAXIMIZED, useSysCmd);
		}

		/// <summary>
		/// If maximized or minimized, makes normal (not min/max).
		/// Also unhides.
		/// </summary>
		/// <param name="useSysCmd">If true, uses Api.WM_SYSCOMMAND. If false, uses Api.ShowWindow(). If omitted/null, uses Api.WM_SYSCOMMAND or Api.ShowWindow(), depending on window style.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <seealso cref="StateNormal"/>
		public void RestoreToNormal(bool? useSysCmd = null)
		{
			if(!Visible) Visible = true;
			if(StateNormal) return;
			_MinMaxRes(Api.SW_SHOWNORMAL, useSysCmd);
		}

		/// <summary>
		/// If minimized, restores previous non-minimized state (maximized or normal).
		/// Also unhides.
		/// </summary>
		/// <param name="useSysCmd">If true, uses Api.WM_SYSCOMMAND. If false, uses Api.ShowWindow(). If omitted/null, uses Api.WM_SYSCOMMAND or Api.ShowWindow(), depending on window style.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails (unlikely).
		/// </exception>
		/// <seealso cref="StateMinimized"/>
		public void RestoreMinimized(bool? useSysCmd = null)
		{
			if(!Visible) Visible = true;
			if(!StateMinimized) return;
			_MinMaxRes(Api.SW_RESTORE, useSysCmd);
		}

		void _MinMaxRes(int state, bool? useSysCmd)
		{
			Validate();

			//Send WM_SYSCOMMAND if has minimize/maximize button. Else call Api.ShowWindow.
			uint cmd = 0;
			if(useSysCmd == null) {
				uint style = Style;
				switch(state) {
				case Api.SW_MINIMIZE: if((style & Api.WS_MINIMIZEBOX) != 0) cmd = Api.SC_MINIMIZE; break;
				case Api.SW_SHOWMAXIMIZED: if((style & Api.WS_MAXIMIZEBOX) != 0) cmd = Api.SC_MAXIMIZE; break;
				default: if((style & (Api.WS_MAXIMIZEBOX | Api.WS_MAXIMIZEBOX)) != 0) cmd = Api.SC_RESTORE; break;
				}
			} else if(useSysCmd.Value) {
				switch(state) {
				case Api.SW_MINIMIZE: cmd = Api.SC_MINIMIZE; break;
				case Api.SW_SHOWMAXIMIZED: cmd = Api.SC_MAXIMIZE; break;
				default: cmd = Api.SC_RESTORE; break;
				}
			}
			//Out(cmd);

			bool ok;
			if(cmd != 0) {
				ok = SendTimeout(10000, Api.WM_SYSCOMMAND, cmd);
				//if was minimized, now can be maximized, need to restore if SW_SHOWNORMAL
				if(ok && state == Api.SW_SHOWNORMAL && StateMaximized) ok = SendTimeout(10000, Api.WM_SYSCOMMAND, cmd);
				//it seems that don't need _SetStateActivateWait here like for ShowWindow.
			} else ok = _SetState(state);

			if(!ok) throw new CatkeysException("Failed to minimize, maximize or restore window.");
			//TODO: auto-delay
		}

		/// <summary>
		/// Quickly minimizes (without animation), hides and activates another window.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public void MinimizeAndHide()
		{
			StateMinimized = true;
			Visible = false;
		}

		#endregion

		#region activation, focus

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
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails to activate (unlikely).
		/// </exception>
		/// <remarks>
		/// Applies auto-delay.
		/// Activating a window usually also uncloaks it, for example switches to its virtual desktop on Windows 10.
		/// Fails (throws exception) if cannot activate this window, except:
		///		1. If this is a control, calls FocusControl(), which activates its top-level parent and sets focus to this control.
		///		2. If this is Wnd.Get.DesktopWindow, just deactivates the currently active window.
		///		3. When the target application instead activates another window of the same thread.
		/// </remarks>
		/// <seealso cref="ActivateRaw"/>
		/// <seealso cref="IsActive"/>
		/// <seealso cref="Wnd.ActiveWindow"/>
		public void Activate()
		{
			if(IsControl) { FocusControl(); return; }
			_Activate(_ActivateFlag.ValidateThrow);
		}

		[Flags]
		internal enum _ActivateFlag
		{
			ValidateThrow = 1, //call Validate(). Throw if fails to activate.
			IgnoreIfNoActivateStyleEtc = 2, //don't activate if WS_EX_NOACTIVATE or toolwindow without caption, unless cloaked. Then return true.
		}

		internal bool _Activate(_ActivateFlag flags)
		{
			bool validateThrow = flags.HasFlag(_ActivateFlag.ValidateThrow);
			if(validateThrow) Validate(); else if(!IsValid) return false;

			bool ofThisThread = IsOfThisThread;

			if(StateMinimized) {
				RestoreMinimized();
				if(!ofThisThread) WaitMS(200); //need minimum 20 for Excel
			}
			if(!Visible) Visible = true;

			bool R = IsActive, noAct = false;

			if(!R) {
				if(flags.HasFlag(_ActivateFlag.IgnoreIfNoActivateStyleEtc)) {
					uint est = ExStyle;
					if((est & Api.WS_EX_NOACTIVATE) != 0) noAct = true;
					else if((est & (Api.WS_EX_TOOLWINDOW | Api.WS_EX_APPWINDOW)) == Api.WS_EX_TOOLWINDOW) noAct = !HasStyle(Api.WS_CAPTION);
					if(noAct && !IsCloaked) return true; //if cloaked, need to activate to uncloak
				}

				for(int i = 0; i < 3; i++) {
					bool ok = ActivateRaw();

					if(!ofThisThread) {
						int speed = get_speed();
						for(int j = 0; j < 5; j++) {
							//Out(ActiveWindow);
							WaitMS(speed / 5 + 2);
							//Speed.First();
							SendTimeout(200, 0);
							//Speed.NextWrite();
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
								//Excel creates a minimized/offscreen window for each workbook opened in that excel process.
								//These windows just add taskbar buttons. Also it allows to find and activate workbooks.
								//When you activate such window, Excel instead activates its main window, where it displays all workbooks.
								//For this reason we would fail (not always, because this may be temporarily active).
								//Same with PowerPoint. Other Office apps no.
							}
						}
						if(R) break;
					}
				}
			}

			if(R && !ofThisThread && IsCloaked) {
				R = false;
				for(int i = 0; i < 40; i++) { WaitMS(50); if(R = !IsCloaked) break; } //when switching Win10 desktops, uncloaks after ~20 ms
				if(R) WaitMS(800); //need minimum 600 for pixel() and wait C, because of animation while switching Win10 virtual desktops.
			}

			if(R || noAct) return true;
			if(validateThrow) throw new CatkeysException("Failed to activate window.");
			return false;
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

			bool canAct = AllowActivate();

			if(!Api.SetForegroundWindow(this)) {
				if(!canAct || !IsValid) return false;
				//It happens when foreground process called LockSetForegroundWindow.
				//Although AllowSetForegroundWindow returns true, SetForegroundWindow fails.
				//It happens only before this process sends keys. Eg after first _Act.SendKey this never happens again.
				//If it has higher IL (and this process is User), also need _Act.MinRes.
				_Act.SendKey();
				if(!Api.SetForegroundWindow(this)) {
					_Act.MinRes();
					if(!Api.SetForegroundWindow(this)) return false;
				}
			}

			//Sometimes after SetForegroundWindow there is no active window for several ms. Not if the window is of this thread.
			if(this == Get.DesktopWindow) return ActiveWindow.Is0;
			return WaitForAnActiveWindow();
        }

		/// <summary>
		/// Waits while there is no active window.
		/// It sometimes happens after switching the active window (and it is not of this thread), very briefly until Windows makes the new window active after making the old window inactive.
		/// Don't need to call this after Activate(), ActivateRaw() and most other functions of this library that activate windows.
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

		/// <summary>
		/// Temporarily enables this process to activate windows with Api.SetForegroundWindow().
		/// Returns false if fails (unlikely).
		/// In some cases you may need this function because Windows often disables SetForegroundWindow() to not allow applications to activate their windows while the user is working (using keyboard/mouse) with the currently active window. Then SetForegroundWindow() just makes the window's taskbar button flash which indicates that the windows wants attention. More info in SetForegroundWindow() help in MSDN.
		/// Usually you will not call Api.SetForegroundWindow() directly. It is called by some other functions, for example some API dialog functions.
		/// Don't need to call this function to enable Activate(), ActivateRaw() and most other functions of this library that activate windows.
		/// </summary>
		public static bool AllowActivate()
		{
			if(_Act.AllowSetFore()) return true; //not locked, or already successfully called ASF_Key

			_Act.SendKey();
			if(_Act.AllowSetFore()) return true;
			//First time fails if the foreground window is of higher IL. Then sending keys does not work.

			_Act.MinRes();
			return _Act.AllowSetFore();

			//Other possible methods:
			//1. Instead of key can use attachthreadinput. But it is less reliable, eg works first time only, and does not allow our process to activate later easily. Does not work if foreground window is higher IL.
			//2. Call allowsetforegroundwindow from a hook from the foreground process. Too dirty. Need 2 native dlls (32/64-bit). Cannot inject if higher IL.
		}
		//TODO: test Show.TaskDialog etc, maybe need Wnd.AllowActivate(). Make sure that foreground lock enabled, because VS disables it.

		//Util functions for AllowActivate etc.
		static class _Act
		{
			/// <summary>
			/// Sends a key (VK_0 up). It allows to activate now.
			/// Later this process can always activate easily (without key etc). It works even with higher IL windows.
			/// Don't know why is this behavior. Tested on all OS from XP to 10.
			/// Does not work if the foreground process has higher UAC IL.
			/// </summary>
			internal static void SendKey()
			{
				OutDebug("_Act.Allow: need key");

				var x = new Api.INPUTKEY(0, 128, Api.IKFlag.Up);
				Api.SendInputKey(ref x);
				//info: works without waiting.
			}

			/// <summary>
			/// Creates a temporary minimized window and restores it. It activates the window and allows us to activate.
			/// Then sets 'no active window' to prevent auto-activating another window when destroying the temporary window.
			/// </summary>
			internal static void MinRes()
			{
				OutDebug("_Act.Allow: need min/res");

				Wnd t = Api.CreateWindowEx(Api.WS_EX_TOOLWINDOW, "#32770", null, Api.WS_POPUP | Api.WS_MINIMIZE | Api.WS_VISIBLE, 0, 0, 0, 0, Wnd0, 0, Zero, 0);
				//info: When restoring, the window must be visible, or may not work.
				try {
					var wp = new Api.WINDOWPLACEMENT(); wp.showCmd = Api.SW_RESTORE;
					t.SetWindowPlacement(ref wp); //activates t; fast (no animation)
					SendKey(); //makes so that later our process can always activate
					AllowSetFore();
					Api.SetForegroundWindow(Get.DesktopWindow); //set no foreground window, or may activate the higher IL window (maybe does not activate, but winevents hook gets events, in random order). Other way would be to destroy our window later, but more difficult to implement.

				} finally { Api.DestroyWindow(t); }
			}

			/// <summary>
			/// Calls Api.AllowSetForegroundWindow(Api.GetCurrentProcessId()).
			/// </summary>
			internal static bool AllowSetFore() { return Api.AllowSetForegroundWindow(Api.GetCurrentProcessId()); }
		}

		/// <summary>
		/// Calls Api.LockSetForegroundWindow(), which temporarily prevents other applications from activating windows easily with SetForegroundWindow().
		/// If Api.LockSetForegroundWindow() fails, calls AllowActivate() and retries.
		/// </summary>
		/// <param name="on">Lock or unlock.</param>
		public static bool LockActiveWindow(bool on)
		{
			uint f = on ? Api.LSFW_LOCK : Api.LSFW_UNLOCK;
			if(Api.LockSetForegroundWindow(f)) return true;
			return AllowActivate() && Api.LockSetForegroundWindow(f);
		}

		static int get_speed() { return 100; } //TODO

		/// <summary>
		/// Makes this control the focused (receiving keyboard input) control.
		/// Also activetes its top-level parent window.
		/// This control can belong to any process/thread. To focus controls of this thread usually it's better to use FocusControlOfThisThread(); it is lightweight, no exceptions.
		/// </summary>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid (not found, closed, etc).
		/// 2. When fails to activate parent window.
		/// 3. When fails to set focus, for example because of UAC.
		/// </exception>
		/// <seealso cref="Wnd.FocusedControl"/>
		public void FocusControl()
		{
			Debug.Assert(!IsOfThisThread);
			Validate();
			Wnd wTL = ToplevelParentOrThis;
			if(wTL != Api.GetForegroundWindow()) wTL._Activate(_ActivateFlag.ValidateThrow);

			uint th1 = Api.GetCurrentThreadId(), th2 = ThreadId;
			if(th1 == th2) {
				Api.SetFocus(this);
				return;
			}

			bool ok = false;
			if(Api.AttachThreadInput(th1, th2, true))
				try {
					int i, speed = get_speed();
					for(i = 0; i < 50; i++) {
						Api.SetFocus(this);
						if(this == FocusedControl) { ok = true; break; }
						WaitMS(speed / 20 + 5);
					}
				} finally { Api.AttachThreadInput(th1, th2, false); }

			if(!ok) throw new CatkeysException("Failed to set focus.");

			//TODO: auto-delay.

			//note: don't use accSelect because on Win7 it simply calls SetForegroundWindow, which like deactivates parent.
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
				var g = new Api.GUITHREADINFO(); g.cbSize = Api.SizeOf(g);
				Api.GetGUIThreadInfo(0, ref g);
				return g.hwndFocus;
			}
		}

		#endregion

		#region Move_Resize_SetPos
		/// <summary>
		/// Calls Api.SetWindowPos.
		/// All info in MSDN, SetWindowPos topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool SetPos(uint swpFlags, int x = 0, int y = 0, int cx = 0, int cy = 0, Wnd wndInsertAfter = default(Wnd))
		{
			ResetLastError();
			return Api.SetWindowPos(this, wndInsertAfter, x, y, cx, cy, swpFlags);
		}

		/// <summary>
		/// Moves and resizes.
		/// Calls Api.SetWindowPos with flags Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|swpFlagsToAdd.
		/// More info in MSDN, SetWindowPos topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool MoveResize(int x, int y, int width, int height, uint swpFlagsToAdd = 0)
		{
			return SetPos(Api.SWP_NOZORDER | Api.SWP_NOOWNERZORDER | Api.SWP_NOACTIVATE | swpFlagsToAdd, x, y, width, height);
		}

		/// <summary>
		/// Moves and resizes.
		/// Calls Api.SetWindowPos with flags Api.SWP_NOSIZE|Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|swpFlagsToAdd.
		/// More info in MSDN, SetWindowPos topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool Move(int x, int y, uint swpFlagsToAdd = 0)
		{
			return MoveResize(x, y, 0, 0, Api.SWP_NOSIZE | swpFlagsToAdd);
		}

		/// <summary>
		/// Moves and resizes.
		/// Calls Api.SetWindowPos with flags Api.SWP_NOMOVE|Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|swpFlagsToAdd.
		/// More info in MSDN, SetWindowPos topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool Resize(int width, int height, uint swpFlagsToAdd = 0)
		{
			return MoveResize(0, 0, width, height, Api.SWP_NOMOVE | swpFlagsToAdd);
		}

		//TODO: MoveToVirtualDesktop().

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
				if(!w.GetRectNormal(out r)) return false;
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
							//Windows bug: before a dialog is first time shown, may fail to move if it has an owner window. Depends on coordinates and on don't know what.
							//There are several workarounds. The best of them - temporarily set owner window 0.
				Wnd hto = Wnd0; bool visible = w.Visible;
				try {
					if(!visible) {
						hto = (Wnd)w.GetWindowLong(Api.GWL_HWNDPARENT);
						if(!hto.Is0) w.SetWindowLong(Api.GWL_HWNDPARENT, Zero);
					}

					Api.WINDOWPLACEMENT wp;
					if(!w.GetWindowPlacement(out wp)) return false;
					bool isMax = wp.showCmd == Api.SW_SHOWMAXIMIZED;
					//if(r == wp.rcNormalPosition && !isMax) return true;
					wp.rcNormalPosition = r;
					wp.showCmd = visible ? Api.SW_SHOWNA : Api.SW_HIDE;
					if(!w.SetWindowPlacement(ref wp)) return false;

					if(isMax && !scr.Equals(Screen_.FromWindow(w))) {
						//I found this way of moving max window to other screen by experimenting.
						//When moved to screen's coordinates and sized to screen's work area size, Windows adjusts window pos to be correct, ie border is outside screen, but invisible in adjacent screen.
						//Must call SetWindowPos twice, or it may refuse to move at all.
						//Another way - use SetWindowPlacement to temporarily restore, move to other screen, then maximize. But it unhides hidden window.
						rs = scr.WorkingArea;
						return w.Move(rs.left, rs.top) && w.Resize(rs.Width, rs.Height);
					}
				} finally {
					if(!hto.Is0) w.SetWindowLong(Api.GWL_HWNDPARENT, (LPARAM)hto);
				}
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
		/// If the window is maximized, minimized or hidden, it will have the new position and size when restored, not immediately.
		/// See also: Wnd.RectMoveInScreen.
		/// Returns false if fails. Supports Marshal.GetLastWin32Error().
		/// </remarks>
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
		/// See also: Wnd.RectEnsureInScreen.
		/// Returns false if fails. Supports Marshal.GetLastWin32Error().
		/// </remarks>
		public bool EnsureInScreen(object screen = null, bool workArea = true, bool limitSize = false)
		{
			var r = new RECT();
			return _MoveInScreen(true, this, ref r, screen, workArea, true, limitSize, false);
		}

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

		#endregion

		#region Zorder
		const uint _SWP_ZORDER = Api.SWP_NOMOVE | Api.SWP_NOSIZE | Api.SWP_NOACTIVATE | Api.SWP_NOOWNERZORDER;

		/// <summary>
		/// Places this window after another window in the Z order. If it is a control - after another control.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool ZorderAfter(Wnd anoterWindow)
		{
			return SetPos(_SWP_ZORDER, 0, 0, 0, 0, anoterWindow);
		}
		/// <summary>
		/// Places this window before another window in the Z order. If it is a control - before another control.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool ZorderBefore(Wnd anoterWindow)
		{
			return SetPos(_SWP_ZORDER, 0, 0, 0, 0, Get.PreviousSibling(anoterWindow));
		}
		/// <summary>
		/// Places this window or control at the top of its Z order.
		/// If the window was topmost, it will be at the top of topmost windows, else at the top of non-topmost windows (after topmost windows).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool ZorderTop()
		{
			return SetPos(_SWP_ZORDER, 0, 0, 0, 0, Spec.Top);
		}
		/// <summary>
		/// Places this window or control at the bottom of its Z order.
		/// If the window was topmost, makes it non-topmost. //TODO: maybe better don't do it; add parameter makeNonTopmost=false.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool ZorderBottom()
		{
			if(HasExStyle(Api.WS_EX_TOPMOST)) ZorderNotopmost();
			return SetPos(_SWP_ZORDER, 0, 0, 0, 0, Spec.Bottom);
		}
		/// <summary>
		/// Makes this window topmost (always on top of non-topmost windows in the Z order).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool ZorderTopmost()
		{
			return SetPos(_SWP_ZORDER, 0, 0, 0, 0, Spec.Topmost);
		}
		/// <summary>
		/// Makes this window non-topmost.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool ZorderNotopmost()
		{
			for(int i = 0; i < 4; i++) {
				if(!SetPos(_SWP_ZORDER, 0, 0, 0, 0, Spec.NoTopmost)) return false;
				if(i == 0 && !HasExStyle(Api.WS_EX_TOPMOST)) break;
			}
			return true;
		}
		// Windows 8/10 bug: cannot make a topmost uiAccess window non-topmost(with HWND_NOTOPMOST, HWND_BOTTOM or non-topmost hwndInsertAfter).
		//   Workaround: call SWP 2 times.With some Windows updates also need SWP_NOOWNERZORDER.
		//   Problems with SWP_NOOWNERZORDER:
		//      1. If used with non-uiAccess windows, then later HWND_TOPMOST does not work.Solution: call SWP first time without this flag.
		//      2. Does not make owned windows non-topmost.Solution: finally call SWP without this flag.
		//      3. Does not make owner window non-topmost.Never mind, it is rare, and a solution is dirty.
		//   The bug and workarounds are undocumented.
		// More problems with topmost uiAccess windows:
		//   Sometimes inserting a uiAccess hwnd after a window does not work, sometimes works...
		//   Problems with HWND_BOTTOM and owned windows.
		//   And so on.
		// On Windows XP/7/8 HWND_BOTTOM moves a topmost window to the bottom of ALL windows, as documented.
		//   But on Windows 10 - to the top of non-topmost windows; 2-nd SWP moves to the right place, but 3-th SWP moves uiAccess windows back :), 4-th makes correct (owned windows too).

		#endregion

		#region style, exStyle
		/// <summary>
		/// Gets window style.
		/// It is a combination of Api.WS_x flags, documented in MSDN, "Window Styles" topic.
		/// See also: HasStyle().
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public uint Style
		{
			get { return Api.GetWindowLong(this, Api.GWL_STYLE); }
		}

		/// <summary>
		/// Gets window extended style.
		/// It is a combination of Api.WS_EX_x flags, documented in MSDN, "Extended Window Styles" topic.
		/// See also: HasExStyle().
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public uint ExStyle
		{
			get { return Api.GetWindowLong(this, Api.GWL_EXSTYLE); }
		}

		/// <summary>
		/// Returns true if the window has all specified style flags (Api.WS_x, documented in MSDN, "Window Styles" topic).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool HasStyle(uint style)
		{
			return (Style & style) == style;
		}

		/// <summary>
		/// Returns true if the window has all specified extended style flags (Api.WS_EX_x, documented in MSDN, "Extended Window Styles" topic).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool HasExStyle(uint exStyle)
		{
			return (ExStyle & exStyle) == exStyle;
		}

		/// <summary>
		/// Changes window style.
		/// SetStyle sets style as specified. Adding/removing some style bits is easier with SetStyleAdd/SetStyleRemove.
		/// Supports Marshal.GetLastWin32Error().
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
		/// Supports Marshal.GetLastWin32Error().
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
				uint pstyle = Api.GetWindowLong(this, gwl);
				if(how == 1) style |= pstyle;
				else if(how == 2) style = pstyle & ~style;
				else style = pstyle ^ style;
			}

			if(Api.SetWindowLong(this, gwl, (int)style) == 0 && Marshal.GetLastWin32Error() != 0) return false;

			if(updateNC) SetPos(Api.SWP_FRAMECHANGED | Api.SWP_NOMOVE | Api.SWP_NOSIZE | Api.SWP_NOZORDER | Api.SWP_NOOWNERZORDER | Api.SWP_NOACTIVATE);
			if(updateClient) Api.InvalidateRect(this, Zero, true);

			return true;
		}
		#endregion

		#region window/class long, prop, control id

		/// <summary>
		/// Gets or sets id of this control.
		/// The 'get' function calls Api.GetDlgCtrlID.
		/// The 'set' function calls Api.SetWindowLong(Api.GWL_ID).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public int ControlId
		{
			get { ResetLastError(); return Api.GetDlgCtrlID(this); }
			set { SetWindowLong(Api.GWL_ID, value); }
		}

		/// <summary>
		/// Calls Api.GetWindowLong(), which calls GetWindowLong if current process is 32-bit, GetWindowLongPtr if 64-bit.
		/// </summary>
		/// <param name="index">One of Api.GWL_x or Api.DWL_x.</param>
		/// <remarks>
		/// All GWL_ values are the same in 32-bit and 64-bit process. Some DWL_ values are different, therefore they are defined as properties, not as constants.
		/// Supports Marshal.GetLastWin32Error().
		/// </remarks>
		public LPARAM GetWindowLong(int index)
		{
			return Api.GetWindowLong(this, index);
		}

		/// <summary>
		/// Calls Api.SetWindowLong(), which calls SetWindowLong if current process is 32-bit, SetWindowLongPtr if 64-bit.
		/// </summary>
		/// <param name="index">One of Api.GWL_x or Api.DWL_x.</param>
		/// <remarks>
		/// All GWL_ values are the same in 32-bit and 64-bit process. Some DWL_ values are different, therefore they are defined as properties, not as constants.
		/// Supports Marshal.GetLastWin32Error().
		/// </remarks>
		public LPARAM SetWindowLong(int index, LPARAM newValue)
		{
			return Api.SetWindowLong(this, index, newValue);
		}

		/// <summary>
		/// Calls Api.GetClassLong(), which calls GetClassLong if current process is 32-bit, GetClassLongPtr if 64-bit.
		/// </summary>
		/// <param name="index">One of Api.GCL_x or Api.GCW_ATOM.</param>
		/// <remarks>
		/// All GCL_/GCW_ values are the same in 32-bit and 64-bit process.
		/// Supports Marshal.GetLastWin32Error().
		/// </remarks>
		public LPARAM GetClassLong(int index)
		{
			return Api.GetClassLong(this, index);
		}

		/// <summary>
		/// Gets atom of a window class.
		/// To get class atom when you have a window w, use w.GetClassLong(Api.GCW_ATOM).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="className">Class name.</param>
		/// <param name="hInstance">Native module handle of the exe or dll that registered the class. Don't use if it is a global class.</param>
		internal static ushort GetClassAtom(string className, IntPtr hInstance = default(IntPtr))
		{
			var x = new Api.WNDCLASSEX_for_GetClassInfoEx();
			ResetLastError();
			return Api.GetClassInfoEx(hInstance, className, out x);
		}

		/// <summary>
		/// Calls Api.GetProp() and returns its return value.
		/// More info in MSDN, GetProp topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		public LPARAM GetProp(string name)
		{
			ResetLastError();
			return Api.GetProp(this, name);
		}
		public LPARAM GetProp(ushort atom)
		{
			ResetLastError();
			return Api.GetProp(this, atom);
		}

		/// <summary>
		/// Calls Api.SetProp() and returns its return value.
		/// More info in MSDN, SetProp topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		public bool SetProp(string name, LPARAM value)
		{
			ResetLastError();
			return Api.SetProp(this, name, value);
		}
		public bool SetProp(ushort atom, LPARAM value)
		{
			ResetLastError();
			return Api.SetProp(this, atom, value);
		}

		/// <summary>
		/// Calls Api.RemoveProp() and returns its return value.
		/// More info in MSDN, RemoveProp topic.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="name">Property name. Other overload allows to use global atom instead, which is faster.</param>
		public LPARAM RemoveProp(string name)
		{
			ResetLastError();
			return Api.RemoveProp(this, name);
		}
		public LPARAM RemoveProp(ushort atom)
		{
			ResetLastError();
			return Api.RemoveProp(this, atom);
		}

		#endregion

		#region rect

		/// <summary>
		/// Gets window rectangle relative to the primary screen.
		/// If fails (eg window closed), returns empty rectangle.
		/// Supports Marshal.GetLastWin32Error().
		/// See also: X, Y, Width, Height, ClientRect, ClientWidth, ClientHeight.
		/// </summary>
		public RECT Rect { get { var r = new RECT(); ResetLastError(); Api.GetWindowRect(this, out r); return r; } }
		/// <summary>
		/// Returns Rect.left.
		/// </summary>
		public int X { get { return Rect.left; } }
		/// <summary>
		/// Returns Rect.top.
		/// </summary>
		public int Y { get { return Rect.top; } }
		/// <summary>
		/// Returns Rect.Width.
		/// </summary>
		public int Width { get { return Rect.Width; } }
		/// <summary>
		/// Returns Rect.Height.
		/// </summary>
		public int Height { get { return Rect.Height; } }

		/// <summary>
		/// Gets client area rectangle.
		/// If fails (eg window closed), returns empty rectangle.
		/// The left and top fields are always 0. The right and bottom fields are the width and height of the client area.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public RECT ClientRect { get { var r = new RECT(); ResetLastError(); Api.GetClientRect(this, out r); return r; } }
		public int ClientWidth { get { return ClientRect.Width; } }
		public int ClientHeight { get { return ClientRect.Height; } }

		/// <summary>
		/// Gets rectangle of this window (usually control) relative to the client area of another window (usually the parent window).
		/// If fails (eg window closed), returns empty rectangle.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="w">The returned rectangle will be relative to the client area of window w.</param>
		public RECT RectInClientOf(Wnd w)
		{
			var r = Rect;
			if(r.IsEmpty && Marshal.GetLastWin32Error() != 0) return r;
			Api.MapWindowPoints(Wnd0, w, ref r, 2);
			return r;
		}

		/// <summary>
		/// Gets rectangle of normal (restored) window even if it is minimized or maximized.
		/// Returns false if fails, eg if the window is closed.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public bool GetRectNormal(out RECT r)
		{
			Api.WINDOWPLACEMENT p;
			bool ok = GetWindowPlacement(out p);
			r = p.rcNormalPosition;
			return ok;
		}

		#endregion

		#region thread, process, isUnicode, is64-bit, UAC

		/// <summary>
		/// Calls Api.GetWindowThreadProcessId().
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public uint GetThreadAndProcessId(out uint processId) { ResetLastError(); processId = 0; return Api.GetWindowThreadProcessId(this, out processId); }
		/// <summary>
		/// Calls Api.GetWindowThreadProcessId() and returns thread id.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public uint ThreadId { get { uint pid; return GetThreadAndProcessId(out pid); } }
		/// <summary>
		/// Calls Api.GetWindowThreadProcessId() and returns process id.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public uint ProcessId { get { uint pid = 0; GetThreadAndProcessId(out pid); return pid; } }
		/// <summary>
		/// Returns true if this window belongs to the current thread.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public bool IsOfThisThread { get { return Api.GetCurrentThreadId() == ThreadId; } }
		/// <summary>
		/// Returns true if this window belongs to the current process.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public bool IsOfThisProcess { get { return Api.GetCurrentProcessId() == ProcessId; } }

		/// <summary>
		/// Returns true if the window is a Unicode window.
		/// Returns false if the window is an ANSI window or if fails (eg the handle is invalid).
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public bool IsUnicode { get { ResetLastError(); return Api.IsWindowUnicode(this); } }

		/// <summary>
		/// Returns true if the window is of a 64-bit process.
		/// Returns false if the window is of a 32-bit process or if fails (eg the handle is invalid).
		/// Supports Marshal.GetLastWin32Error).
		/// If you know that the window belongs to current process, instead use Environment.Is64BitProcess or IntPtr.Size==8.
		/// See also: Environment.Is64BitOperatingSystem.
		/// </summary>
		public bool Is64Bit
		{
			get
			{
				if(!Environment.Is64BitOperatingSystem) { ResetLastError(); return false; }
				uint pid = ProcessId; if(pid == 0) return false;
				IntPtr ph = Zero;
				try {
					ph = Api.OpenProcess(Api.PROCESS_QUERY_LIMITED_INFORMATION, false, pid); if(ph == Zero) return false;
					int is32bit;
					return Api.IsWow64Process(ph, out is32bit) && is32bit == 0;
				} finally {
					if(ph != Zero) Api.CloseHandle(ph);
				}
				//info: don't use Process.GetProcessById, it does not have an desiredAccess parameter and fails with higher IL processes.
			}
		}

		/// <summary>
		/// Returns true if UAC would not allow to access/automate the window.
		/// If UAC is turned off, returns false.
		/// If current process has uiAccess rights, returns false.
		/// If cannot get window's process UAC info, returns true.
		/// If window's process is a Windows store app on Windows 8 (not on 10), returns true.
		/// If current process has lower UAC integrity level, returns true.
		/// Else returns false.
		/// </summary>
		public bool IsUacAccessDenied
		{
			get
			{
				if(UacInfo.IsUacDisabled) return false;
				var t = UacInfo.ThisProcess;
				if(t.IsUIAccess) return false;
				var u = UacInfo.GetOfProcess(ProcessId); if(u == null) return true;
				if(WinVer >= Win8_0 && WinVer <= Win8_1 && u.IsAppContainer) return true;
				if(t.IntegrityLevel < u.IntegrityLevel) return true;
				return false;
				//TODO: test
			}
		}

		//These are not useful. Use IsAccessDenied or class UacInfo.
		///// <summary>
		///// Gets UAC integrity level of window's process.
		///// Returns IL.Unknown if fails.
		///// This function considers UIAccess equal to High.
		///// See also: class UacInfo.
		///// </summary>
		//public UacInfo.IL UacIntegrityLevel
		//{
		//	get { var p = UacInfo.GetOfProcess(ProcessId); return p == null ? UacInfo.IL.Unknown : p.IntegrityLevel; }
		//}

		///// <summary>
		///// Returns true if window's process has higher UAC integrity level (IL) than current process.
		///// Returns true if fails to open process handle, which usually means that the process has higher integrity level.
		///// This function considers UIAccess equal to High.
		///// See also: class UacInfo.
		///// </summary>
		//public bool UacIntegrityLevelIsHigher
		//{
		//	get { return UacIntegrityLevel > UacInfo.ThisProcess.IntegrityLevel; }
		//}

		#endregion

		#region text, class, isofclass, process name, tostring

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
		/// Returns null if fails, eg if the window is destroyed.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public unsafe string ClassName
		{
			get
			{
				const int stackSize = 260;
				var b = stackalloc char[stackSize];
				ResetLastError();
				int n = _Api.GetClassName(this, b, stackSize);
				if(n > 0) return new string(b, 0, n);
				return null;
				//speed: 320. It is 50% slower than QM2 str.getwinclass (with conversion to UTF8 and free()); slightly faster than with char[260] or fixed char[260]; with StringBuilder 70% slower; with Marshal.AllocHGlobal 30% slower.
				//Calling through delegate (Marshal.GetDelegateForFunctionPointer) does not make faster.
			}
		}

		/// <summary>
		/// Gets or sets window name or control text.
		/// The 'get' function returns "" if the text is empty. Returns null if fails, eg if the window is destroyed.
		/// </summary>
		/// <remarks>
		/// Each top-level window and control has a text property, although the text can be empty. This function gets or sets that text.
		/// If this is a top-level window, it is the title bar text.
		/// If control, it is the text displayed in the control, eg button text, static text, edit control text. However most other controls don't have text that we can get with this function, even if they display text.
		/// The 'get' function calls Api.InternalGetWindowText(). If it returns empty text and this is a control, calls GetControlText() with default parameter values. The later function is slow; to avoid it use GetControlName() instead of Name.
		/// The 'set' function calls SetControlText(), which sends WM_SETTEXT message with 5000 ms timeout; on timeout does nothing.
		/// Note: it is not the .NET Control.Name.
		/// Supports Marshal.GetLastWin32Error.
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
		/// <remarks>
		/// Similar to the Name property, and in most cases will get the same value.
		/// Unlike Name, does not call the slow function GetControlText() when Api.InternalGetWindowText() returns empty text.
		/// Can be used with top-level windows too.
		/// Supports Marshal.GetLastWin32Error).
		/// </remarks>
		public string GetControlName()
		{
			return _GetName(false);
			//TODO: consider: bool param to remove &.
		}

		unsafe string _GetName(bool getControlTextIfEmpty)
		{
			const int stackSize = 1024;

			var b = stackalloc char[stackSize];
			ResetLastError();
			int nt = _Api.InternalGetWindowText(this, b, stackSize);
			if(nt < 1) {
				if(Marshal.GetLastWin32Error() != 0) return null;
				if(getControlTextIfEmpty && (Style & Api.WS_CHILD) != 0) return GetControlText();
				return "";
			}
			if(nt < stackSize - 1) return new string(b, 0, nt);

			var sb = new StringBuilder();
			for(int na = stackSize; na <= int.MaxValue / 4;) {
				na *= 2;
				sb.Capacity = na;
				ResetLastError();
				nt = _Api.InternalGetWindowTextSB(this, sb, na);
				if(nt < na - 1) {
					if(nt > 0) return sb.ToString();
					return (Marshal.GetLastWin32Error() == 0) ? "" : null;
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
		/// Supports Marshal.GetLastWin32Error).
		/// </remarks>
		public unsafe string GetControlText(int timeoutMS = 5000) //TODO: consider: Option.sendMessageTimeoutMS=1000 (min 500, 0 = no timeout). Then let these be properties: ControlText (get, set), ControlName.
		{
			const int stackSize = 1024;
			LPARAM na, nt;
			bool ofThisThread = IsOfThisThread;

			//At first try to get text without knowing length.
			//For most controls it makes faster (60 -> 40 mcs), and only for big-text controls slower (85 mcs).

			var b = stackalloc char[stackSize];

			if(ofThisThread) nt = Send(Api.WM_GETTEXT, stackSize, b);
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
		/// Supports Marshal.GetLastWin32Error).
		/// </remarks>
		public bool SetControlText(string text, int timeoutMS = 5000)
		{
			LPARAM res;
			return SendTimeoutS(timeoutMS, out res, Api.WM_SETTEXT, 0, text ?? "");
		}

		/// <summary>
		/// Returns true if the class name of this window matches className.
		/// String by default is interpreted as wildcard, case-insensitive.
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public bool ClassNameIs(WildStringI className)
		{
			return className.Match(ClassName);
		}

		/// <summary>
		/// If the class name of this window matches one of strings in classNames, returns 1-based index of the string. Else returns 0.
		/// classNames can be a string array or List, or it can be |-delimited string list like "Class1|Class2|Class3".
		/// By default the class names in the list are interpreted as wildcard, case-insensitive (uses String.Like_()).
		/// Supports Marshal.GetLastWin32Error).
		/// </summary>
		public int ClassNameIsAny(StringList classNames)
		{
			string cn = ClassName; if(cn == null) return 0;
			return cn.Like_(true, classNames.Arr);
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

		/// <summary>
		/// Formats string $"{handle} {ClassName} \"{Name}\"".
		/// </summary>
		public override string ToString()
		{
			if(Is0) return "0";
			string s = Name.Limit_(250);
			return $"{_h} {ClassName} \"{s}\"";
		}

		#endregion

		#region close, destroy

		/// <summary>
		/// Closes the window.
		/// See also: Destroy().
		/// </summary>
		public bool Close()
		{
			if(!IsValid) return true;
			//TODO: implement.
			return true;
		}

		/// <summary>
		/// Destroys the window.
		/// Calls Api.DestroyWindow() and returns its return value.
		/// Exception if the window is not of this thread.
		/// See also: Close().
		/// </summary>
		public bool Destroy()
		{
			if(!IsValid) return false;
			if(!IsOfThisThread) throw new CatkeysException("Cannot destroy windows of other threads. Try Close.");
			return Api.DestroyWindow(this);
		}

		#endregion

		#region misc

		/// <summary>
		/// Gets or sets native font handle.
		/// Sends message Api.WM_GETFONT or Api.WM_SETFONT (redraws).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public IntPtr Font
		{
			get { return Send(Api.WM_GETFONT); }
			set { Send(Api.WM_SETFONT, value, true); }
		}

		//TODO: Flash()

		/// <summary>
		/// Stops flashing taskbar button.
		/// </summary>
		public void FlashStop()
		{
			var fi = new Api.FLASHWINFO(); fi.cbSize = Api.SizeOf(fi); fi.hwnd = this;
			Api.FlashWindowEx(ref fi);
			//tested. FlashWindow is easier but does not work for taskbar button, only for caption when no taskbar button.
		}

		#endregion

		#region util

		static void _SetLastErrorInvalidHandle()
		{
			Api.SetLastError(1400); //Invalid window handle
		}

		//This can be used, but not much simpler than calling ATI directly and using try/finally.
		//internal struct _AttachThreadInput :IDisposable
		//{
		//	uint _tid;

		//	public bool Attach(uint tid)
		//	{
		//		if(!Api.AttachThreadInput(Api.GetCurrentThreadId(), _tid, true)) return false;
		//		_tid = tid; return true;
		//	}

		//	public bool Attach(Wnd w) { return Attach(w.ThreadId); }

		//	public void Dispose() { if(_tid != 0) Api.AttachThreadInput(Api.GetCurrentThreadId(), _tid, false); }
		//}

		#endregion
	}

}
