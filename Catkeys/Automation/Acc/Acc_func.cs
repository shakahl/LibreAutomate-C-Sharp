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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	public unsafe partial class Acc
	{
		/// <summary>
		/// Gets the container window or control of this accessible object.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns default(Wnd) if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property, but some have bugs and can fail (return default(Wnd)) or return a wrong window.
		/// </remarks>
		public Wnd WndContainer
		{
			get
			{
				if(_Disposed) throw new ObjectDisposedException(nameof(Acc));
				_Hresult(_FuncId.container_window, _iacc.GetWnd(out var w));
				return w;
			}
		}

		/// <summary>
		/// Gets the top-level window that contains this accessible object.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn> and API <msdn>GetAncestor</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns default(Wnd) if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property, but some have bugs and can return default(Wnd).
		/// </remarks>
		public Wnd WndTopLevel { get => WndContainer.WndWindow; }
		//note: named not WndWindow, to avoid using accidentally instead of WndContainer.

		/// <summary>
		/// Gets location of this accessible object in screen.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetRect(out RECT)"/>.
		/// Returns empty rectangle if failed or this property is unavailable. Supports <see cref="Native.GetError"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public RECT Rect { get { GetRect(out var r); return r; } }

		/// <summary>
		/// Gets location of this accessible object in screen.
		/// Uses <msdn>IAccessible.accLocation</msdn>.
		/// </summary>
		/// <param name="r">Receives object rectangle in screen coordinates.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="Native.GetError"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public bool GetRect(out RECT r)
		{
			return 0 == _Hresult(_FuncId.rectangle, _iacc.accLocation(_elem, out r));
		}

		/// <summary>
		/// Gets location of this accessible object in the client area of window w.
		/// Uses <msdn>IAccessible.accLocation</msdn> and <see cref="Wnd.MapScreenToClient(ref RECT)"/>.
		/// </summary>
		/// <param name="r">Receives object rectangle in w client area coordinates.</param>
		/// <param name="w">Window or control.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="Native.GetError"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public bool GetRect(out RECT r, Wnd w)
		{
			return GetRect(out r) && w.MapScreenToClient(ref r);
		}

		/// <summary>
		/// Gets standard non-string role, as enum AccROLE.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </summary>
		/// <remarks>
		/// Most objects have a standard role, as enum AccROLE. Some objects have a custom role, normally as string.
		/// Returns 0 if object's role is string.
		/// Returns 0 if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property. If failed, probably the object is invalid, for example its window was closed.
		/// </remarks>
		public AccROLE RoleEnum
		{
			get
			{
				if(_role == 0) _Hresult(_FuncId.role, _iacc.GetRole(_elem, out _role));
				return _role;
			}
		}

		/// <summary>
		/// Gets standard or custom role, as string.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </summary>
		/// <remarks>
		/// Most objects have a standard role, as enum <see cref="AccROLE"/>. Some objects have a custom role, normally as string, for example in web pages in Firefox and Chrome.
		/// For standard roles this function returns enum <see cref="AccROLE"/> member name. For string roles - the string. For unknown non-string roles - the int value like "0" or "500".
		/// Returns "" if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property. If failed, probably the object is invalid, for example its window was closed.
		/// </remarks>
		public string RoleString
		{
			get
			{
				_Hresult(_FuncId.role, _iacc.GetRoleString(_elem, out string s, ref _role));
				return s ?? "";
			}
		}

		/// <summary>
		/// Gets object state (flags).
		/// Uses <msdn>IAccessible.get_accState</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns 0 if failed. Supports <see cref="Native.GetError"/>.
		/// Getting state often is slower than getting role, name or rectangle. But usually not much slower.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// if(a.State.Has_(AccSTATE.INVISIBLE)) Print("invisible");
		/// if(a.IsInvisible) Print("invisible"); //the same as above
		/// ]]></code>
		/// </example>
		public AccSTATE State
		{
			get
			{
				_Hresult(_FuncId.state, _iacc.get_accState(_elem, out var state));
				return state;
			}
		}

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.BUSY. Can be used with DOCUMENT object of web browsers. </summary>
		public bool IsBusy { get => State.Has_(AccSTATE.BUSY); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.CHECKED. </summary>
		public bool IsChecked { get => State.Has_(AccSTATE.CHECKED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.UNAVAILABLE. </summary>
		public bool IsDisabled { get => State.Has_(AccSTATE.DISABLED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.FOCUSED. </summary>
		public bool IsFocused { get => State.Has_(AccSTATE.FOCUSED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.INVISIBLE. </summary>
		public bool IsInvisible { get => State.Has_(AccSTATE.INVISIBLE); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.OFFSCREEN. </summary>
		public bool IsOffscreen { get => State.Has_(AccSTATE.OFFSCREEN); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.PROTECTED. </summary>
		public bool IsPassword { get => State.Has_(AccSTATE.PROTECTED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.PRESSED. </summary>
		public bool IsPressed { get => State.Has_(AccSTATE.PRESSED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.READONLY. </summary>
		public bool IsReadonly { get => State.Has_(AccSTATE.READONLY); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.SELECTED. See also <see cref="IsFocused"/>. </summary>
		public bool IsSelected { get => State.Has_(AccSTATE.SELECTED); }

		/// <summary>
		/// Gets or sets name.
		/// Uses <msdn>IAccessible.get_accName</msdn> or <msdn>IAccessible.put_accName</msdn>.
		/// </summary>
		/// <exception cref="CatException">Failed to set name.</exception>
		/// <remarks>
		/// Object name usually is its read-only text (eg button text or link text), or its adjacent read-only text (eg text label by this edit box). It usually does not change, therefore can be used to find or identify the object.
		/// Most objects don't support 'set'.
		/// The 'get' function returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Name
		{
			get
			{
				_Hresult(_FuncId.name, _iacc.get_accName(_elem, out var s, _role));
				return s;
			}
			set
			{
				CatException.ThrowIfHresultNot0(_iacc.put_accName(_elem, value));
			}
		}

		/// <summary>
		/// Gets accessible name of window/control w.
		/// Returns null if w invalid. Returns "" if failed to get name.
		/// </summary>
		internal static string LibNameOfWindow(Wnd w)
		{
			if(!w.IsAlive) return null;
			using(var a = _FromWindow(w, noThrow: true)) {
				if(!a.Is0 && 0 == a.get_accName(0, out var s, AccROLE.WINDOW))
					return s;
			}
			return "";
		}

		/// <summary>
		/// Gets or sets value.
		/// Uses <msdn>IAccessible.get_accValue</msdn> or <msdn>IAccessible.put_accValue</msdn>.
		/// </summary>
		/// <exception cref="CatException">Failed to set value.</exception>
		/// <remarks>
		/// Object value usually is its editable text or some other value that can be changed at run time, therefore in most cases it cannot be used to find or identify the object reliably.
		/// Most objects don't support 'set'.
		/// The 'get' function returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Value
		{
			get
			{
				_Hresult(_FuncId.value, _iacc.get_accValue(_elem, out var s));
				return s;
			}
			set
			{
				CatException.ThrowIfHresultNot0(_iacc.put_accValue(_elem, value));
			}
		}

		/// <summary>
		/// Gets description.
		/// Uses <msdn>IAccessible.get_accDescription</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Description
		{
			get
			{
				_Hresult(_FuncId.description, _iacc.get_accDescription(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets help text.
		/// Uses <msdn>IAccessible.get_accHelp</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Help
		{
			get
			{
				_Hresult(_FuncId.help_text, _iacc.get_accHelp(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets keyboard shortcut.
		/// Uses <msdn>IAccessible.get_accKeyboardShortcut</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string KeyboardShortcut
		{
			get
			{
				_Hresult(_FuncId.keyboard_shortcut, _iacc.get_accKeyboardShortcut(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets default action.
		/// Uses <msdn>IAccessible.get_accDefaultAction</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// This function may have problems with Java windows. Then instead use <see cref="DoJavaAction"/>.
		/// </remarks>
		public string DefaultAction
		{
			get
			{
				_Hresult(_FuncId.default_action, _iacc.get_accDefaultAction(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets list of actions that can be used with <see cref="DoJavaAction"/>.
		/// </summary>
		public string JavaActions
		{
			get
			{
				_Hresult(_FuncId.default_action, _iacc.get_accDefaultAction(-1000_000, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets another accessible object that is somehow related to this object (child, parent, next, etc). Allows to specify multiple relations.
		/// </summary>
		/// <param name="navig">
		/// String consisting of one or more navigation direction strings separated by space, like "parent next child4 first".
		/// Navigation string reference and more info: <see cref="AccNAVDIR"/>.
		/// A string can end with a number, like "child4" or "child,4". It is the n parameter of <see cref="Navigate(AccNAVDIR, int, bool)"/>.
		/// A custom direction can be specified like "#0x1009".
		/// </param>
		/// <param name="disposeThis">Dispose this Acc variable (release the COM object).</param>
		/// <exception cref="ArgumentException">Invalid navig string.</exception>
		public Acc Navigate(string navig, bool disposeThis = false)
		{
			if(_Disposed) throw new ObjectDisposedException(nameof(Acc));
			var na = _Acc.NavdirN.Parse(navig); //ArgumentException
			var a = new _Acc(_iacc, _elem);
			if(disposeThis) _Dispose(doNotRelease: true); else _iacc.AddRef();
			if(0 != _Hresult(_FuncId.navigate, a.Navigate(na))) return null;
			return new Acc(a.a, a.elem);

			//rejected: child role, like "child(LISTITEM)2". Usually can be just child2, or use Find.
		}

		/// <summary>
		/// Gets another accessible object that is somehow related to this object (child, parent, next, etc).
		/// Returns null if not found. You can use <see cref="ExtensionMethods.OrThrow{T}"/> (see example).
		/// </summary>
		/// <param name="navDir">
		/// Navigation direction (relation). For example <c>AccNAVDIR.PARENT</c>.
		/// Custom directions also are supported. For example <c>(AccNAVDIR)0x1009</c>.
		/// More info is in <see cref="AccNAVDIR"/> documentation.
		/// </param>
		/// <param name="n">
		/// With navDir AccNAVDIR.CHILD it is 1-based child index. Negative index means from end, for example -1 is the last child. Cannot be 0.
		/// With all other navDir - how many times to get object in the specified direction. For example PARENT 2 will get parent's parent. Cannot be less than 1.
		/// </param>
		/// <param name="disposeThis">Dispose this Acc variable (release the COM object).</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid n.</exception>
		public Acc Navigate(AccNAVDIR navDir, int n = 1, bool disposeThis = false)
		{
			if(_Disposed) throw new ObjectDisposedException(nameof(Acc));
			if(n == 0 || (n < 0 && navDir != AccNAVDIR.CHILD)) throw new ArgumentOutOfRangeException();
			var a = new _Acc(_iacc, _elem);
			if(disposeThis) _Dispose(doNotRelease: true); else _iacc.AddRef();
			if(0 != _Hresult(_FuncId.navigate, a.Navigate(navDir, n))) return null;
			return new Acc(a.a, a.elem);
		}

		//rejected: public Acc Parent(bool disposeThis = false) (call get_accParent directly). Can use Navigate(), it's almost as fast. Useful mostly in programming, not in scripts.
		//rejected: public Acc Child(int childIndex, bool disposeThis = false) (call get_accChild). Many objects don't implement get_accChild. Can use Navigate, which calls AccessibleChildren. Rarely used, unlike Parent.

		/// <summary>
		/// Asks the accessible object to perform its default action (see <see cref="DefaultAction"/>), for example 'click'.
		/// Uses <msdn>IAccessible.accDoDefaultAction</msdn>.
		/// </summary>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// Fails if the object does not have a default action. Then you can use <see cref="ExtensionMethods.MouseClick(Acc, Coord, Coord, MButton)"/> or try <see cref="Select"/>, <see cref="Focus"/>.
		/// If the action takes long time, for example shows a dialog, this function normally just starts the action and returns immediately. It allows the caller to automate the dialog. If it does not return immediately, you can use another thread to automate the dialog.
		/// </remarks>
		public void DoDefaultAction()
		{
			var hr = _iacc.accDoDefaultAction(_elem);
			if(hr != 0) {
				if(hr == Api.DISP_E_MEMBERNOTFOUND || DefaultAction.Length == 0) throw new CatException("This object does not have a default action");
				CatException.ThrowIfHresultNot0(hr);
				//CatException.ThrowIfHresultNegative(hr);
				//Debug_.PrintIf(hr != 0, "failed");
			}
			_MinimalSleep();
		}

		void _MinimalSleep()
		{
			Thread.Sleep(15);
			//if(0 == _iacc.GetWnd(out var w)) w.LibMinimalSleepIfOtherThread(); //better don't call GetWnd
		}

		/// <summary>
		/// Asks the Java accessible object to perform one of its actions.
		/// </summary>
		/// <param name="action">Action name. See <see cref="JavaActions"/>. If null (default), posts Space key message to the object.</param>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// Supports accessible objects of Java windows only if the Java Access Bridge (JAB) is enabled. You can enable/disable it in Control Panel -> Ease of Access Center -> Use the computer without a display. Or use jabswitch.exe.
		/// JAB has this bug: if it is a button click action that opens a modal dialog, DoDefaultAction/DoJavaAction do not return until the dialog is closed. It waits max 8 s. During that time the caller must wait and cannot automate the dialog. Also then this process cannot exit until the dialog is closed. If the action parameter is null and the object is focusable, this function tries a workaround: it makes the object (button etc) focused and posts Space key message, which should press the button, and it does not wait.
		/// </remarks>
		public void DoJavaAction(string action = null)
		{
			//problem: if the button click action opens a modal dialog, doAccessibleActions waits until closed.
			//	Waits 8 s and then returns true. Somehow in QM2 returns false.
			//	During that time any JAB calls (probably from another thread) are blocked and fail. Tried various combinations.
			//	Also then releaseJavaObject does not return until the dialog closed. It even does not allow our process to exit. In QM2 the same.
			//	Previously (with old Java version?) then whole JAB crashed. Now doesn't. Or crashes only in some conditions that I now cannot reproduce.

			if(action == null
				&& 0 == _iacc.get_accState(_elem, out var state)
				&& (state & (AccSTATE.FOCUSABLE | AccSTATE.SELECTABLE)) == AccSTATE.FOCUSABLE //must be only focusable. If SELECTABLE, probably don't need this workaround.
				&& 0 == _iacc.GetWnd(out var w)
				&& 0 == _iacc.accSelect(AccSELFLAG.TAKEFOCUS, _elem)
				&& 0 == _iacc.get_accState(_elem, out state) && state.Has_(AccSTATE.FOCUSED) //avoid sending keys to another control
				) {
				w.Post(Api.WM_KEYDOWN, Api.VK_SPACE, 0);
				w.Post(Api.WM_KEYUP, Api.VK_SPACE, 0);
				//tested: works even if the window is inactive.
				w.LibMinimalSleepNoCheckThread();
				return;
			}

			var hr = _iacc.accDoDefaultAction(action);
			CatException.ThrowIfHresultNot0(hr);
			//_MinimalSleep(); //probably don't need, because JAB doAccessibleActions is sync, which is bad.
		}

		/// <summary>
		/// Selects or deselects.
		/// </summary>
		/// <param name="how">Specifies whether to select, focus, add to selection etc. Can be two flags, for example <c>AccSELFLAG.TAKEFOCUS | AccSELFLAG.TAKESELECTION</c>.</param>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// Uses <msdn>IAccessible.accSelect</msdn>.
		/// Not all objects support it. Most objects support not all flags. It depends on object <see cref="AccSTATE">states</see> FOCUSABLE, SELECTABLE, MULTISELECTABLE, EXTSELECTABLE, DISABLED.
		/// Many object have bugs, especially with flag TAKEFOCUS. For example standard Windows controls often deactivate the window etc.
		/// </remarks>
		public void Select(AccSELFLAG how = AccSELFLAG.TAKESELECTION)
		{
			//Workaround for Windows controls bugs, part 1.
			Wnd w = default, wTL = default; bool focusingControl = false;
			if(how.Has_(AccSELFLAG.TAKEFOCUS) && 0 == _iacc.GetWnd(out w)) {
				if(!w.IsEnabled) throw new CatException("*set focus. Disabled"); //accSelect would not fail
				wTL = w.WndWindow;
				wTL.Activate();
				if(focusingControl = (w != wTL)) w.Focus();
				if(IsFocused) how &= ~AccSELFLAG.TAKEFOCUS;
				if(how == 0) return;
			}

			for(int i = 0; i < 2; i++) {
				var hr = _iacc.accSelect(how, _elem);
				if(hr == 0) break;
				if(hr == 1) continue; //some objects return S_FALSE even if did what asked. Eg combobox (focuses the child Edit), slider. Or may need to retry, eg when trying to focus a listitem in a non-focused listbox.
				if(hr == Api.DISP_E_MEMBERNOTFOUND) throw new CatException("This object does not support this state");
				CatException.ThrowIfHresultNegative(hr);
			}

			if(!w.Is0) w.LibMinimalSleepIfOtherThread(); //sleep only when focusing. Assume selection is sync. Also need for the bug, because the control may be activated a millisecond later.

			//Workaround for Windows controls bugs, part 2.
			if(focusingControl && w.IsActive) {
				//Debug_.Print("activated control");
				wTL.Activate();
			}

			//tested: IAccessible.accSelect(TAKEFOCUS):
			//	Most Windows controls have this bug: activates the control with SetForegroundWindow, which deactivates the top-level window.
			//		Especially if the control is already focused.
			//		If not already focused, fails if eg listbox item. But then works well with eg buttons.
			//		MSDN: If IAccessible::accSelect is called with the SELFLAG_TAKEFOCUS flag on a child object that has an HWND, the flag takes effect only if the object's parent has the focus.
			//		Tested, does not help: LockSetForegroundWindow, AttachThreadInput.
			//	Works well with web browsers, WinForms.
			//	With WPF initially almost does not work. After using a navigation key (Tab etc) starts to work well.
			//tested: IUIAutomationElement.SetFocus:
			//	In most cases works well with standard controls, all web browsers, WinForms.
			//	With WPF same as Acc.
			//	Bug: If standard control is disabled, deactivates parent window and draws focus rectangle on the control.
		}

		/// <summary>
		/// Makes this object focused for keyboard input.
		/// </summary>
		/// <param name="andSelect">Focus and select. Adds flag TAKESELECTION. Note: it is for selecting a list item, not for selecting text in a text box.</param>
		/// <remarks>
		/// Calls <see cref="Select"/> with flag TAKEFOCUS and optionally TAKESELECTION.
		/// Not all objects support this action and not all work correctly. More info in Select documentation.
		/// </remarks>
		public void Focus(bool andSelect = false)
		{
			var how = AccSELFLAG.TAKEFOCUS;
			if(andSelect) how |= AccSELFLAG.TAKESELECTION;
			Select(how);
		}

		/// <summary>
		/// Gets selected direct child items.
		/// Returns empty array if there are no selected items of if failed.
		/// </summary>
		public Acc[] SelectedChildren
		{
			get
			{
				if(_Disposed) throw new ObjectDisposedException(nameof(Acc));
				if(_elem != 0) return new Acc[0];
				return _iacc.get_accSelection();
			}
		}
	}
}
