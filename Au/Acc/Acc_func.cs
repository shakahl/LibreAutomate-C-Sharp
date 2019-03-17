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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

#pragma warning disable CS0282 //VS bug: shows warning "There is no defined ordering between fields in multiple declarations of partial struct 'Acc'. To specify an ordering, all instance fields must be in the same declaration."

namespace Au
{
	public unsafe partial class Acc
	{
		/// <summary>
		/// Gets the container window or control of this accessible object.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns default(Wnd) if failed. Supports <see cref="WinError.Code"/>.
		/// All objects must support this property, but some have bugs and can fail (return default(Wnd)) or return a wrong window.
		/// </remarks>
		public Wnd WndContainer
		{
			get
			{
				LibThrowIfDisposed();
				_Hresult(_FuncId.container_window, _GetWnd(out var w));
				return w;
			}
		}

		int _GetWnd(out Wnd w)
		{
			int hr = Cpp.Cpp_AccGetInt(this, 'w', out var i);
			GC.KeepAlive(this);
			w = (Wnd)i;
			return hr;
		}

		/// <summary>
		/// Gets the top-level window that contains this accessible object.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn> and API <msdn>GetAncestor</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns default(Wnd) if failed. Supports <see cref="WinError.Code"/>.
		/// All objects must support this property, but some have bugs and can return default(Wnd).
		/// </remarks>
		public Wnd WndTopLevel => WndContainer.Window;
		//note: named not WndWindow, to avoid using accidentally instead of WndContainer.

		/// <summary>
		/// Gets location of this accessible object in screen.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetRect(out RECT)"/>.
		/// Returns empty rectangle if failed or this property is unavailable. Supports <see cref="WinError.Code"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public RECT Rect { get { GetRect(out var r); return r; } }

		/// <summary>
		/// Gets location of this accessible object in screen.
		/// Uses <msdn>IAccessible.accLocation</msdn>.
		/// </summary>
		/// <param name="r">Receives object rectangle in screen coordinates.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="WinError.Code"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public bool GetRect(out RECT r)
		{
			LibThrowIfDisposed();
			var hr = _Hresult(_FuncId.rectangle, Cpp.Cpp_AccGetRect(this, out r));
			GC.KeepAlive(this);
			return hr == 0;
		}

		/// <summary>
		/// Gets location of this accessible object in the client area of window w.
		/// Uses <msdn>IAccessible.accLocation</msdn> and <see cref="Wnd.MapScreenToClient(ref RECT)"/>.
		/// </summary>
		/// <param name="r">Receives object rectangle in w client area coordinates.</param>
		/// <param name="w">Window or control.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="WinError.Code"/>.
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
		/// Most objects have a standard role, as enum <see cref="AccROLE"/>. Some objects have a custom role, usually as string, for example in web pages in Firefox and Chrome.
		/// Returns 0 if role is string or if failed. Supports <see cref="WinError.Code"/>.
		/// All objects must support this property. If failed, probably the object is invalid, for example its window was closed.
		/// </remarks>
		public AccROLE RoleInt
		{
			get
			{
				LibThrowIfDisposed();
				if(_misc.role != 0) return (AccROLE)_misc.role; //SHOULDDO: use AccMiscFlags.RoleIsString
				_Hresult(_FuncId.role, _GetRole(out var role, out _, dontNeedStr: true));
				return role;
			}
		}

		/// <summary>
		/// Gets standard or custom role, as string.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </summary>
		/// <remarks>
		/// Most objects have a standard role, as enum <see cref="AccROLE"/>. Some objects have a custom role, usually as string, for example in web pages in Firefox and Chrome.
		/// For standard roles this function returns enum <see cref="AccROLE"/> member name. For string roles - the string. For unknown non-string roles - the int value like "0" or "500".
		/// Returns "" if failed. Supports <see cref="WinError.Code"/>.
		/// All objects must support this property. If failed, probably the object is invalid, for example its window was closed.
		/// </remarks>
		public string Role
		{
			get
			{
				LibThrowIfDisposed();
				var role = (AccROLE)_misc.role;
				if(role == 0) {
					if(0 != _Hresult(_FuncId.role, _GetRole(out role, out var roleStr, dontNeedStr: false))) return "";
					if(roleStr != null) return roleStr;
				}
				var a = s_roles; uint u = (uint)role;
				return (u < a.Length) ? a[u] : ((int)role).ToString();
			}
		}

		static readonly string[] s_roles = { "0", "TITLEBAR", "MENUBAR", "SCROLLBAR", "GRIP", "SOUND", "CURSOR", "CARET", "ALERT", "WINDOW", "CLIENT", "MENUPOPUP", "MENUITEM", "TOOLTIP", "APPLICATION", "DOCUMENT", "PANE", "CHART", "DIALOG", "BORDER", "GROUPING", "SEPARATOR", "TOOLBAR", "STATUSBAR", "TABLE", "COLUMNHEADER", "ROWHEADER", "COLUMN", "ROW", "CELL", "LINK", "HELPBALLOON", "CHARACTER", "LIST", "LISTITEM", "TREE", "TREEITEM", "PAGETAB", "PROPERTYPAGE", "INDICATOR", "IMAGE", "STATICTEXT", "TEXT", "BUTTON", "CHECKBOX", "RADIOBUTTON", "COMBOBOX", "DROPLIST", "PROGRESSBAR", "DIAL", "HOTKEYFIELD", "SLIDER", "SPINBUTTON", "DIAGRAM", "ANIMATION", "EQUATION", "BUTTONDROPDOWN", "BUTTONMENU", "BUTTONDROPDOWNGRID", "WHITESPACE", "PAGETABLIST", "CLOCK", "SPLITBUTTON", "IPADDRESS", "TREEBUTTON" };

		//Returns HRESULT.
		int _GetRole(out AccROLE roleInt, out string roleStr, bool dontNeedStr)
		{
			roleStr = null;
			if(_misc.role != 0) { roleInt = (AccROLE)_misc.role; return 0; }
			var hr = Cpp.Cpp_AccGetRole(this, out roleInt, out var b);
			GC.KeepAlive(this);
			if(hr == 0) {
				if(b.Is0) _misc.SetRole(roleInt);
				else if(dontNeedStr) b.Dispose();
				else roleStr = b.ToStringAndDispose();
			}
			return hr;
		}

		int _GetState(out AccSTATE state)
		{
			int hr = Cpp.Cpp_AccGetInt(this, 's', out int i);
			GC.KeepAlive(this);
			state = (AccSTATE)i;
			return hr;
		}

		/// <summary>
		/// Gets object state (flags).
		/// Uses <msdn>IAccessible.get_accState</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns 0 if failed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// if(a.State.Has_(AccSTATE.INVISIBLE)) Print("has state INVISIBLE");
		/// if(a.IsInvisible) Print("invisible");
		/// ]]></code>
		/// </example>
		public AccSTATE State
		{
			get
			{
				LibThrowIfDisposed();
				_Hresult(_FuncId.state, _GetState(out var state));
				return state;
			}
		}

		/// <summary> Calls <see cref="State"/> and returns true if has state CHECKED. </summary>
		public bool IsChecked => State.Has_(AccSTATE.CHECKED);

		/// <summary> Calls <see cref="State"/> and returns true if has state UNAVAILABLE. </summary>
		/// <remarks>Does not check whether this object is in a disabled parent/ancestor object.</remarks>
		public bool IsDisabled => State.Has_(AccSTATE.DISABLED);

		/// <summary> Calls <see cref="State"/> and returns true if has state FOCUSED. </summary>
		public bool IsFocused => State.Has_(AccSTATE.FOCUSED);

		/// <summary> Calls <see cref="State"/> and returns true if has state INVISIBLE and does not have state OFFSCREEN. </summary>
		/// <remarks>
		/// If the object has both INVISIBLE and OFFSCREEN states, it is either invisible or just offscreen, depending on application etc. Then this function works like Find and similar functions: for most objects returns false (is visible), but for objects that have these roles returns true (invisible): WINDOW, DOCUMENT, PROPERTYPAGE, GROUPING, ALERT, MENUPOPUP.
		/// Does not check whether this object is in an invisible parent/ancestor object.
		/// </remarks>
		public bool IsInvisible => LibIsInvisible(State);

		internal bool LibIsInvisible(AccSTATE state)
		{
			if(!state.Has_(AccSTATE.INVISIBLE)) return false;
			if(!state.Has_(AccSTATE.OFFSCREEN)) return true;
			switch(RoleInt) {
			case AccROLE.WINDOW:
			case AccROLE.DOCUMENT:
			case AccROLE.PROPERTYPAGE:
			case AccROLE.GROUPING:
			case AccROLE.ALERT:
			case AccROLE.MENUPOPUP:
				return true;
				//note: these roles must be the same as in _IsRoleToSkipIfInvisible in "acc find.cpp"
			}
			return false;
		}

		/// <summary> Calls <see cref="State"/> and returns true if has state OFFSCREEN. </summary>
		public bool IsOffscreen => State.Has_(AccSTATE.OFFSCREEN);

		/// <summary> Calls <see cref="State"/> and returns true if has state PROTECTED. </summary>
		/// <remarks>This state is used for password fields.</remarks>
		public bool IsPassword => State.Has_(AccSTATE.PROTECTED);

		/// <summary> Calls <see cref="State"/> and returns true if has state PRESSED. </summary>
		public bool IsPressed => State.Has_(AccSTATE.PRESSED);

		/// <summary> Calls <see cref="State"/> and returns true if has state READONLY. </summary>
		public bool IsReadonly => State.Has_(AccSTATE.READONLY);

		/// <summary> Calls <see cref="State"/> and returns true if has state SELECTED. </summary>
		public bool IsSelected => State.Has_(AccSTATE.SELECTED);

		/// <summary>
		/// Converts BSTR to string and disposes the BSTR.
		/// If hr is not 0, returns "" (never null).
		/// </summary>
		static string _BstrToString(int hr, BSTR b)
		{
			if(hr == 0) return b.ToStringAndDispose() ?? "";
			return "";
		}

		string _GetStringProp(char prop)
		{
			LibThrowIfDisposed();
			int hr = Cpp.Cpp_AccGetProp(this, prop, out var b);
			GC.KeepAlive(this);
			var s = _BstrToString(hr, b);
			_Hresult((_FuncId)prop, hr);
			return s;
		}

		/// <summary>
		/// Gets name.
		/// Uses <msdn>IAccessible.get_accName</msdn>.
		/// </summary>
		/// <remarks>
		/// Object name usually is its read-only text (eg button text, link text), or its adjacent read-only text (eg text label by this edit box). It usually does not change, therefore can be used to find or identify the object.
		/// Returns "" if name is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		public string Name
		{
			get => _GetStringProp('n');

			//note: bug of standard toolbar buttons: fails to get name if all these are true:
			//	This process is 64-bit and the target process is 32-bit.
			//	Button Name property comes from its tooltip.
			//	Found not inproc, eg with flag NotInProc.
		}

		/// <summary>
		/// Gets <see cref="Name"/> of window/control w.
		/// Returns null if w invalid. Returns "" if failed to get name.
		/// </summary>
		internal static string LibNameOfWindow(Wnd w)
		{
			if(!w.IsAlive) return null;
			var hr = Cpp.Cpp_AccFromWindow(1 | 2, w, 0, out _, out var b);
			return _BstrToString(hr, b);

			//speed: inproc ~10% faster. But first time slower, especially if process of different bitness.
		}

		/// <summary>
		/// Gets or sets value.
		/// Uses <msdn>IAccessible.get_accValue</msdn> or <msdn>IAccessible.put_accValue</msdn>.
		/// </summary>
		/// <exception cref="AuException">Failed to set value.</exception>
		/// <remarks>
		/// Object value usually is its editable text or some other value that can be changed at run time, therefore in most cases it cannot be used to find or identify the object reliably.
		/// The 'get' function returns "" if this property is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// Most objects don't support 'set'.
		/// </remarks>
		public string Value
		{
			get => _GetStringProp('v');
			set
			{
				LibThrowIfDisposed();
				AuException.ThrowIfHresultNot0(Cpp.Cpp_AccAction(this, 'v', value));
				GC.KeepAlive(this);
			}
		}

		/// <summary>
		/// Gets description.
		/// Uses <msdn>IAccessible.get_accDescription</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		public string Description
		{
			get => _GetStringProp('d');
		}

		/// <summary>
		/// Gets help text.
		/// Uses <msdn>IAccessible.get_accHelp</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		public string Help
		{
			get => _GetStringProp('h');
		}

		/// <summary>
		/// Gets UI Automation element AutomationId property.
		/// </summary>
		/// <remarks>
		/// Only objects found with flag <see cref="AFFlags.UIA"/> can have this property.
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		public string UiaId
		{
			get => _GetStringProp('u');
		}

		/// <summary>
		/// Gets keyboard shortcut.
		/// Uses <msdn>IAccessible.get_accKeyboardShortcut</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		public string KeyboardShortcut
		{
			get => _GetStringProp('k');
		}

		/// <summary>
		/// Gets default action.
		/// Uses <msdn>IAccessible.get_accDefaultAction</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="WinError.Code"/>.
		/// If this is a Java accessible object, returns all actions that can be used with <see cref="DoJavaAction"/>, like "action1, action2, action3", from which the first is considered default and is used by <see cref="DoAction"/>.
		/// </remarks>
		public string DefaultAction
		{
			get => _GetStringProp('a');
		}

		/// <summary>
		/// Performs the object's default action (see <see cref="DefaultAction"/>). Usually it is 'click', 'press' or similar.
		/// Uses <msdn>IAccessible.accDoDefaultAction</msdn>.
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Fails if the object does not have a default action. Then you can use <see cref="ExtensionMethods.MouseClick(Acc, Coord, Coord, MButton)" r=""/>, or try <see cref="VirtualClick"/>, <see cref="Select"/>, <see cref="Focus"/> and keyboard functions.
		/// The action can take long time, for example show a dialog. This function normally does not wait. It allows the caller to automate the dialog. If it waits, try <see cref="DoJavaAction"/> or one of the above functions (MouseClick etc).
		/// </remarks>
		public void DoAction()
		{
			LibThrowIfDisposed();
			var hr = Cpp.Cpp_AccAction(this, 'a');
			GC.KeepAlive(this);
			AuException.ThrowIfHresultNot0(hr);
			//_MinimalSleep(); //don't need. It does not make more reliable.
		}

		//void _MinimalSleep()
		//{
		//	Thread.Sleep(15);
		//	//if(0 == _iacc.GetWnd(out var w)) w.LibMinimalSleepIfOtherThread(); //better don't call GetWnd
		//}

		/// <summary>
		/// Posts mouse-left-click message to the container window, using coordinates of this object.
		/// </summary>
		/// <exception cref="AuException">Failed to get rectangle, or the object is invisible/offscreen.</exception>
		/// <remarks>
		/// Does not move the mouse.
		/// Does not wait until the target application finishes processing the message.
		/// Works not with all objects.
		/// Use (try) this function when the object does not support <see cref="DoAction"/>. When both don't work, use MouseClick.
		/// </remarks>
		public void VirtualClick()
		{
			_VirtualClick(false);
		}

		/// <summary>
		/// Posts mouse-right-click message to the container window, using coordinates of this object.
		/// </summary>
		/// <exception cref="AuException">Failed to get rectangle, or the object is invisible/offscreen.</exception>
		/// <remarks>
		/// Does not move the mouse.
		/// Does not wait until the target application finishes processing the message.
		/// Works not with all objects. When does not work, use MouseClick.
		/// </remarks>
		public void VirtualRightClick()
		{
			_VirtualClick(true);
		}

		void _VirtualClick(bool right)
		{
			var w = WndContainer;
			if(!GetRect(out var r, w)) throw new AuException(0, "*get rectangle");
			if(r.IsEmpty || State.HasAny_(AccSTATE.INVISIBLE | AccSTATE.OFFSCREEN)) throw new AuException(0, "Invisible or offscreen");
			//FUTURE: Chrome bug: OFFSCREEN is not updated after scrolling.

			var xy = Math_.MakeUint(r.CenterX, r.CenterY);
			uint b = 0; if(Keyb.IsCtrl) b |= Api.MK_CONTROL; if(Keyb.IsShift) b |= Api.MK_SHIFT;
			uint b1 = b | (right ? Api.MK_RBUTTON : Api.MK_LBUTTON);
			w.Post(right ? Api.WM_RBUTTONDOWN : Api.WM_LBUTTONDOWN, b1, xy);
			w.Post(Api.WM_MOUSEMOVE, b1, xy);
			w.Post(right ? Api.WM_RBUTTONUP : Api.WM_LBUTTONUP, b, xy);
			//_MinimalSleep(); //don't need. DoAction does not wait too.

			//never mind: support nonclient (WM_NCRBUTTONDOWN etc)
		}

		/// <summary>
		/// Performs one of actions supported by this Java accessible object.
		/// </summary>
		/// <param name="action">
		/// Action name. See <see cref="DefaultAction"/>.
		/// If null (default), performs default action (like <see cref="DoAction"/>) or posts Space key message. More info in Remarks.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Read more about Java accessible objects in <see cref="Acc"/> topic.
		/// 
		/// Problem: if the action opens a dialog, DoAction/DoJavaAction do not return until the dialog is closed (or fail after some time). The caller then waits and cannot automate the dialog. Also then this process cannot exit until the dialog is closed. If the action parameter is null and the object is focusable, this function tries a workaround: it makes the object (button etc) focused and posts Space key message, which should press the button; then this function does not wait.
		/// </remarks>
		public void DoJavaAction(string action = null)
		{
			//problem: if the button click action opens a modal dialog, doAccessibleActions waits until closed.
			//	Waits 8 s and then returns true. Somehow in QM2 returns false.
			//	During that time any JAB calls (probably from another thread) are blocked and fail. Tried various combinations.
			//	Also then releaseJavaObject does not return until the dialog closed. It even does not allow our process to exit. In QM2 the same.
			//	Previously (with old Java version?) then whole JAB crashed. Now doesn't. Or crashes only in some conditions that I now cannot reproduce.

			LibThrowIfDisposed();

			if(action == null
				&& 0 == _GetState(out var state)
				&& (state & (AccSTATE.FOCUSABLE | AccSTATE.SELECTABLE)) == AccSTATE.FOCUSABLE //must be only focusable. If SELECTABLE, probably don't need this workaround.
				&& 0 == _GetWnd(out var w)
				&& 0 == Cpp.Cpp_AccSelect(this, AccSELFLAG.TAKEFOCUS)
				&& 0 == _GetState(out state) && state.Has_(AccSTATE.FOCUSED) //avoid sending keys to another control
				) {
				GC.KeepAlive(this);
				w.Post(Api.WM_KEYDOWN, (byte)KKey.Space, 0);
				w.Post(Api.WM_KEYUP, (byte)KKey.Space, 0);
				//tested: works even if the window is inactive.
				w.LibMinimalSleepNoCheckThread();
				return;
			}

			var hr = Cpp.Cpp_AccAction(this, 'a', action);
			GC.KeepAlive(this);
			AuException.ThrowIfHresultNot0(hr);
			//_MinimalSleep(); //probably don't need, because JAB doAccessibleActions is sync, which is bad.
		}

		/// <summary>
		/// Calls <see cref="DoAction"/> or <paramref name="action"/> and waits until window name changes and web page name changes.
		/// </summary>
		/// <param name="secondsTimeout">
		/// <inheritdoc cref="WaitFor.Condition"/>
		/// Default 60 seconds.
		/// </param>
		/// <param name="action">If used, calls it instead of <see cref="DoAction"/>.</param>
		/// <returns><inheritdoc cref="WaitFor.Condition"/></returns>
		/// <exception cref="TimeoutException"><inheritdoc cref="WaitFor.Condition"/></exception>
		/// <exception cref="AuException">Failed. For example, when this object is invalid, or its top-level window does not contain a web page.</exception>
		/// <exception cref="WndException">The window was closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions thrown by <see cref="DoAction"/> or by the <paramref name="action"/> function.</exception>
		/// <remarks>
		/// This function is used to click a link in a web page and wait until current web page is gone. It prevents a following 'wait for object' function from finding a matching object in the old page, which would be bad.
		/// This function does not wait until the new page is completely loaded. There is no reliable/universal way for it. Instead, after calling it you can call a 'wait for object' function which waits for a known object that must be in the new page.
		/// This function cannot be used when the new page has the same title as current page. Then it waits until <paramref name="secondsTimeout"/> time or forever. The same if the action does not open a web page.
		/// </remarks>
		public bool DoActionAndWaitForNewWebPage(double secondsTimeout = 60, Action<Acc> action = null)
		{
			Wnd w = WndTopLevel; if(w.Is0) throw new AuException("*get window");
			Acc doc = Acc.Wait(-1, w, "web:"); if(doc == null) throw new AuException("*find web page");

			string wndName = w.LibNameTL, docName = doc.Name; Debug.Assert(!Empty(wndName) && !Empty(docName));
			bool wndOK = false, docOK = false;
			Acc.Finder f = null;

			if(action == null) DoAction(); else action(this);

			//wait until window name and document name both are changed. They can change in any order, especially in Chrome.
			var to = new WaitFor.Loop(secondsTimeout, 25);
			while(to.Sleep()) {
				w.ThrowIfInvalid();
				if(!wndOK) {
					var s = w.LibNameTL;
					if(s == null) continue; //probably invalid, will throw in next loop
					wndOK = s != wndName;
				}
				if(wndOK && !docOK) {
					if(f == null) f = new Acc.Finder("web:") { ResultGetProperty = 'n' };
					if(!f.Find(w)) continue; //eg in Firefox for some time there is no DOCUMENT
					var s = f.ResultProperty as string; if(s == null) continue;
					docOK = s != docName;
					//if(!docOK) Print("doc is late");
				}
				if(wndOK && docOK) {
					w.ThrowIfInvalid();
					return true;
				}
			}
			return false;
		}

#if false
		//This function is finished and normally works well.
		//However web browsers not always fire the event. For some pages never, or only when not cached.
		//Also, some pages are like never finish loading (the browser waiting animation does not stop spinning). Or finish when the wanted AO is there for long time, so why to wait. Or finish, then continue loading again...
		//Also, this function inevitably will stop working with some new web browser version with new bugs. Too unreliable.
		public bool DoActionAndWaitForWebPageLoaded(double secondsTimeout = 60, Action<Acc> action = null, Wnd w = default)
		{
			LibThrowIfDisposed();

			int timeout; bool throwTimeout = false;
			if(secondsTimeout == 0) timeout = Timeout.Infinite;
			else {
				if(secondsTimeout < 0) secondsTimeout = -secondsTimeout; else throwTimeout = true;
				timeout = checked((int)(secondsTimeout * 1000));
			}

			if(w.Is0) {
				w = WndContainer;
				if(w.Is0) throw new AuException("*get window");
			}

			var hookEvent = AccEVENT.IA2_DOCUMENT_LOAD_COMPLETE;
			int browser = w.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
			switch(browser) {
			case 0:
				Wnd ies = w.Child(null, Api.string_IES); if(ies.Is0) break;
				w = ies; goto case 1;
			case 1: hookEvent = AccEVENT.OBJECT_CREATE; break;
			}

			int tid = w.ThreadId; if(tid == 0) w.ThrowUseNative();
			AutoResetEvent eventNotify = null;
			int debugIndex = 0;

			Api.WINEVENTPROC hook = (IntPtr hWinEventHook, AccEVENT ev, Wnd hwnd, int idObject, int idChild, int idEventThread, int time) =>
			{
				if(eventNotify == null) { /*Print("null 1");*/ return; }
				if(ev == AccEVENT.OBJECT_CREATE && hwnd != w) return; //note: in Chrome hwnd is Chrome_RenderWidgetHostHWND
				int di = ++debugIndex;
				using(var a = Acc.FromEvent(hwnd, idObject, idChild)) {
					if(a == null) { /*Debug_.Print("Acc.FromEvent null");*/ return; } //often IE, but these are not useful objects
					if(eventNotify == null) { /*Print("null 2");*/ return; }
					if(ev == AccEVENT.IA2_DOCUMENT_LOAD_COMPLETE) { //Chrome, Firefox

						//filter out frame/iframe
						if(browser == 2) { //Firefox does not fire events for frame/iframe. But the Chrome code would work too.
						} else if(0 == a._iacc.get_accParent(out var a2)) { //bug in some Chrome versions: fails for main document
							using(a2) {
								if(eventNotify == null) { /*Print("null 3");*/ return; }
								bool isFrame;
								var hr = a2.GetRole(0, out var role, out var roleStr); if(hr != 0) Debug_.Print((uint)hr);
								if(eventNotify == null) { /*Print("null 4");*/ return; }
								if(hr != 0) isFrame = false;
								else if(roleStr != null) isFrame = roleStr.EndsWith_("frame", true);
								else isFrame = !(role == AccROLE.WINDOW || role == AccROLE.CLIENT);
								//Print(role, roleStr);
								if(isFrame) return;
								//browser    main        frame     iframe
								//Firefox    "browser"   "frame"   "iframe"
								//Chrome     WINDOW      "FRAME"   DOCUMENT
							}
						}
					} else { //IE (OBJECT_CREATE)
						if(a._elem != 0) return;
						if(0 != a._iacc.GetRole(0, out var role) || role != AccROLE.PANE) return;
						if(eventNotify == null) { /*Print("null 3");*/ return; }

						//filter out frame/iframe
						if(a.IsInvisible) return;
						if(eventNotify == null) { /*Print("null 4");*/ return; }
						using(var aCLIENT = _FromWindow(w, AccOBJID.CLIENT, noThrow: true)) {
							if(eventNotify == null) { /*Print("null 5");*/ return; }
							if(aCLIENT != null) {
								var URL1 = a.Value; Debug.Assert(URL1.Length > 0); //Print(URL1); //http:..., about:...
								aCLIENT.get_accName(0, out var URL2, 0); Debug.Assert(URL2.Length > 0);
								if(URL1 != URL2) return;
								if(eventNotify == null) { /*Print("null 6");*/ return; }
							} else Debug_.Print("aCLIENT null");
						}
					}

					//Print(di, ev, a);
					eventNotify.Set();
					eventNotify = null;
				}
			};

			var hh = Api.SetWinEventHook(hookEvent, hookEvent, default, hook, 0, tid, 0); if(hh == default) throw new AuException();
			try {
				eventNotify = new AutoResetEvent(false);
				if(action != null) action(this); else DoAction();
				if(eventNotify.WaitOne(timeout)) {
					//Thread.CurrentThread.Join(2000);
					return true;
				}
			}
			finally { Api.UnhookWinEvent(hh); eventNotify?.Dispose(); }
			GC.KeepAlive(hook);

			if(throwTimeout) throw new TimeoutException();
			return false;
		}
#endif

		/// <summary>
		/// Selects or deselects.
		/// </summary>
		/// <param name="how">Specifies whether to select, focus, add to selection etc. Can be two flags, for example <c>AccSELFLAG.TAKEFOCUS | AccSELFLAG.TAKESELECTION</c>.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <exception cref="WndException">Failed to activate the window (<see cref="Wnd.Activate"/>) or focus the control (<see cref="Wnd.Focus"/>).</exception>
		/// <remarks>
		/// Uses <msdn>IAccessible.accSelect</msdn>.
		/// Not all objects support it. Most objects support not all flags. It depends on object <see cref="AccSTATE">states</see> FOCUSABLE, SELECTABLE, MULTISELECTABLE, EXTSELECTABLE, DISABLED.
		/// Many object have bugs, especially with flag TAKEFOCUS. More bugs when the object found with flag <see cref="AFFlags.NotInProc"/>.
		/// </remarks>
		public void Select(AccSELFLAG how = AccSELFLAG.TAKESELECTION)
		{
			LibThrowIfDisposed();

			//Workaround for Windows controls bugs, part 1.
			Wnd w = default, wTL = default; bool focusingControl = false;
			if(how.Has_(AccSELFLAG.TAKEFOCUS) && 0 == _GetWnd(out w)) {
				if(!w.IsEnabled) throw new AuException("*set focus. Disabled"); //accSelect would not fail
				wTL = w.Window;
				wTL.Activate();
				if(focusingControl = (w != wTL)) w.Focus();
				if(IsFocused) how &= ~AccSELFLAG.TAKEFOCUS;
				if(how == 0) return;
			}

			for(int i = 0; i < 2; i++) {
				var hr = Cpp.Cpp_AccSelect(this, how);
				GC.KeepAlive(this);
				if(hr == 0) break;
				if(hr == 1) continue; //some objects return S_FALSE even if did what asked. Eg combobox (focuses the child Edit), slider. Or may need to retry, eg when trying to focus a listitem in a non-focused listbox.
				if(hr == Api.DISP_E_MEMBERNOTFOUND) throw new AuException("This object does not support this state");
				AuException.ThrowIfHresultNegative(hr);
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
			//		Good news: works well if the object found inproc, ie without flag NotInproc.
			//			But then need to focus the control, else does not work.
			//			Use the same workaround. It focuses the control.
			//	Works well with web browsers, WinForms.
			//	With WPF initially almost does not work. After using a navigation key (Tab etc) starts to work well.
			//tested: UIA.IElement.SetFocus:
			//	In most cases works well with standard controls, all web browsers, WinForms.
			//	With WPF same as Acc.
			//	Bug: If standard control is disabled, deactivates parent window and draws focus rectangle on the control.
		}

		/// <summary>
		/// Makes this object focused for keyboard input.
		/// </summary>
		/// <param name="andSelect">Add flag TAKESELECTION. Note: it is for selecting a list item, not for selecting text in a text box.</param>
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
		/// Returns empty array if there are no selected items of if failed. Supports <see cref="WinError.Code"/>.
		/// </summary>
		public Acc[] SelectedChildren
		{
			get
			{
				LibThrowIfDisposed();
				if(_elem != 0) { WinError.Clear(); return Array.Empty<Acc>(); }
				//return _iacc.get_accSelection();
				if(0 != _Hresult(_FuncId.selection, Cpp.Cpp_AccGetSelection(this, out var b)) || b.Is0) return Array.Empty<Acc>();
				GC.KeepAlive(this);
				var p = (Cpp.Cpp_Acc*)b.Ptr; int n = b.Length / sizeof(Cpp.Cpp_Acc);
				var r = new Acc[n];
				for(int i = 0; i < n; i++) r[i] = new Acc(p[i]);
				b.Dispose();
				return r;
			}
		}

		/// <summary>
		/// Gets the number of direct child objects.
		/// Uses <msdn>IAccessible.get_accChildCount</msdn>.
		/// </summary>
		public int ChildCount
		{
			get
			{
				LibThrowIfDisposed();
				if(_elem != 0) { WinError.Clear(); return 0; }
				_Hresult(_FuncId.child_count, Cpp.Cpp_AccGetInt(this, 'c', out int cc));
				GC.KeepAlive(this);
				return cc;
			}
		}

		/// <summary>
		/// Gets multiple properties.
		/// </summary>
		/// <param name="props">
		/// Which properties to get. Each character specifies a property:
		/// 'R' - <see cref="Role"/>.
		/// 'n' - <see cref="Name"/>.
		/// 'v' - <see cref="Value"/>.
		/// 'd' - <see cref="Description"/>.
		/// 'h' - <see cref="Help"/>.
		/// 'a' - <see cref="DefaultAction"/>.
		/// 'k' - <see cref="KeyboardShortcut"/>.
		/// 'u' - <see cref="UiaId"/>.
		/// 's' - <see cref="State"/>.
		/// 'r' - <see cref="Rect"/>.
		/// 'w' - <see cref="WndContainer"/>.
		/// 'o' - <see cref="Html"/> outer.
		/// 'i' - <see cref="Html"/> inner.
		/// '@' - <see cref="HtmlAttributes"/>.
		/// </param>
		/// <param name="result">Receives results.</param>
		/// <exception cref="ArgumentException">Unknown property character.</exception>
		/// <remarks>
		/// The returned variable contains values of properties specified in <paramref name="props"/>. When a property is empty or failed to get, the member variable is "", empty dictionary or default value of that type; never null.
		/// 
		/// Normally this function is faster than calling multiple property functions, because it makes single remote procedure call. But not if this accessible object was found with flag <see cref="AFFlags.NotInProc"/> etc.
		/// 
		/// Returns false if fails, for example when the object's window is closed. Supports <see cref="WinError.Code"/>.
		/// </remarks>
		public bool GetProperties(string props, out AccProperties result)
		{
			//SHOULDDO: use cached role.

			result = default;
			LibThrowIfDisposed();
			if(props.Length == 0) return true;
			int hr = Cpp.Cpp_AccGetProps(this, props, out var b);
			GC.KeepAlive(this);
			if(hr != 0) {
				if(hr == (int)Cpp.EError.InvalidParameter) throw new ArgumentException("Unknown property character.");
				WinError.Code = hr;
				return false;
			}
			using(b) {
				var offsets = (int*)b.Ptr;
				for(int i = 0; i < props.Length; i++) {
					int offs = offsets[i], len = ((i == props.Length - 1) ? b.Length : offsets[i + 1]) - offs;
					var p = b.Ptr + offs;
					switch(props[i]) {
					case 'r': result.Rect = len > 0 ? *(RECT*)p : default; break;
					case 's': result.State = len > 0 ? *(AccSTATE*)p : default; break;
					case 'w': result.WndContainer = len > 0 ? (Wnd)(*(int*)p) : default; break;
					case '@': result.HtmlAttributes = _AttributesToDictionary(p, len); break;
					default:
						var s = (len == 0) ? "" : new string(p, 0, len);
						switch(props[i]) {
						case 'R': result.Role = s; break;
						case 'n': result.Name = s; break;
						case 'v': result.Value = s; break;
						case 'd': result.Description = s; break;
						case 'h': result.Help = s; break;
						case 'a': result.DefaultAction = s; break;
						case 'k': result.KeyboardShortcut = s; break;
						case 'u': result.UiaId = s; break;
						case 'o': result.OuterHtml = s; break;
						case 'i': result.InnerHtml = s; break;
						}
						break;
					}
				}
			}
			return true;
		}

		static Dictionary<string, string> _AttributesToDictionary(char* p, int len)
		{
			var d = new Dictionary<string, string>();
			int ik = 0, iv = 0;
			for(int i = 0; i < len; i++) {
				var c = p[i];
				if(c == '\0' && iv > ik) {
					string sk = Util.StringCache.LibAdd(p + ik, iv - ik - 1);
					string sv = new string(p, iv, i - iv);
					d[sk] = sv;
					ik = i + 1;
				} else if(c == '=' && iv <= ik) {
					iv = i + 1;
				}
			}
			//Print(d);
			return d;
		}

		/// <summary>
		/// Gets an adjacent or related accessible object (AO) - next, child, parent, etc.
		/// Returns null if not found.
		/// </summary>
		/// <param name="navig">
		/// String consisting of one or more navigation direction strings separated by space, like "parent next child4 first".
		/// <list type="bullet">
		/// <item>"next" - next sibling AO in the same parent AO.</item>
		/// <item>"previous" - previous sibling AO in the same parent AO.</item>
		/// <item>"first" - first child AO.</item>
		/// <item>"last" - last child AO.</item>
		/// <item>"parent" - parent (container) AO.</item>
		/// <item>"child" - child AO by 1-based index. Example: "child3" (3-th child). Negative index means from end, for example -1 is the last child.</item>
		/// <item>"#N" - N is a numeric value to pass to <msdn>IAccessible.accNavigate</msdn> as navDir. Can be any standard or custom value supported by the AO.</item>
		/// </list>
		/// Can be only 2 letters, like "pr" for "previous".
		/// A string like "next3" or "next,3" is the same as "next next next". Except for "child".
		/// 
		/// For "next", "previous", "firstchild", "lastchild", "#N" is used <msdn>IAccessible.accNavigate</msdn>. Not all AOs support it. Some AOs skip invisible siblings. Instead you can use "parent childN" or "childN".
		/// For "parent" is used <msdn>IAccessible.get_accParent</msdn>. Few AOs don't support. Some AOs return a different parent than in the AO tree.
		/// For "child" is used API <msdn>AccessibleChildren</msdn>.
		/// </param>
		/// <param name="secondsToWait">Wait for the wanted AO max this number of seconds. If negative, waits forever.</param>
		/// <exception cref="ArgumentException">Invalid navig string.</exception>
		/// <example>
		/// <code><![CDATA[
		/// a = a.Navigate("parent next ch3", true);
		/// ]]></code>
		/// </example>
		public Acc Navigate(string navig, double secondsToWait = 0)
		{
			LibThrowIfDisposed();
			int hr; var ca = new Cpp.Cpp_Acc();
			if(secondsToWait == 0) {
				hr = Cpp.Cpp_AccNavigate(this, navig, out ca);
			} else {
				var to = new WaitFor.Loop(secondsToWait > 0 ? -secondsToWait : 0.0);
				do hr = Cpp.Cpp_AccNavigate(this, navig, out ca);
				while(hr != 0 && hr != (int)Cpp.EError.InvalidParameter && to.Sleep());
			}
			GC.KeepAlive(this);
			if(hr == (int)Cpp.EError.InvalidParameter) throw new ArgumentException("Invalid navig string.");
			WinError.Code = hr;
			return hr == 0 ? new Acc(ca) : null;

			//FUTURE: when fails, possibly this is disconnected etc. Retry Find with same Finder.
		}

		//rejected: public Acc Parent() (call get_accParent directly). Can use Navigate(), it's almost as fast. Useful mostly in programming, not in scripts.

		/// <summary>
		/// Gets HTML.
		/// </summary>
		/// <param name="outer">If true, gets outer HTML (with tag and attributes), else inner HTML.</param>
		/// <remarks>
		/// Returns "" if this is not a HTML element or if failed. Supports <see cref="WinError.Code"/>.
		/// Works with Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). This object must be found without flag NotInProc.
		/// If this is the root of web page (role DOCUMENT or PANE), gets web page body HTML.
		/// </remarks>
		public string Html(bool outer)
		{
			LibThrowIfDisposed();
			int hr = _Hresult(_FuncId.html, Cpp.Cpp_AccWeb(this, outer ? "'o" : "'i", out BSTR s));
			GC.KeepAlive(this);
			return _BstrToString(hr, s);
		}

		/// <summary>
		/// Gets a HTML attribute.
		/// </summary>
		/// <param name="name">Attribute name, for example "href", "id", "class". Full, case-sensitive.</param>
		/// <remarks>
		/// Returns "" if this is not a HTML element or does not have the specified attribute or failed. Supports <see cref="WinError.Code"/>.
		/// Works with Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). This object must be found without flag NotInProc.
		/// </remarks>
		/// <exception cref="ArgumentException">name is null/""/invalid.</exception>
		public string HtmlAttribute(string name)
		{
			LibThrowIfDisposed();
			if(Empty(name) || name[0] == '\'') throw new ArgumentException("Invalid name.");
			int hr = _Hresult(_FuncId.html, Cpp.Cpp_AccWeb(this, name, out BSTR s));
			GC.KeepAlive(this);
			return _BstrToString(hr, s);
		}

		/// <summary>
		/// Gets all HTML attributes.
		/// </summary>
		/// <remarks>
		/// Returns empty dictionary if this is not a HTML element or does not have attributes or failed. Supports <see cref="WinError.Code"/>.
		/// Works with Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). This object must be found without flag NotInProc.
		/// </remarks>
		public Dictionary<string, string> HtmlAttributes()
		{
			LibThrowIfDisposed();
			int hr = Cpp.Cpp_AccWeb(this, "'a", out BSTR s);
			GC.KeepAlive(this);
			_Hresult(_FuncId.html, hr);
			if(hr != 0) return new Dictionary<string, string>();
			using(s) return _AttributesToDictionary(s.Ptr, s.Length);
		}

		/// <summary>
		/// Scrolls this accessible object into view.
		/// </summary>
		/// <exception cref="AuException">Failed to scroll, or the object does not support scrolling.</exception>
		/// <remarks>
		/// This function works with these objects:
		/// <list type="bullet">
		/// <item>Web page objects in Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). With Find use role prefix "web:", "firefox:" or "chrome:", and don't use flag <see cref="AFFlags.NotInProc"/>.</item>
		/// <item>Objects in Edge browser, standard treeview and listview controls, some other. With Find use flag <see cref="AFFlags.UIA"/>.</item>
		/// </list>
		/// </remarks>
		public void ScrollTo()
		{
			LibThrowIfDisposed();

			int hr;
			if(_misc.flags.Has_(AccMiscFlags.UIA)) hr = Cpp.Cpp_AccAction(this, 's');
			else hr = Cpp.Cpp_AccWeb(this, "'s", out _);
			GC.KeepAlive(this);

			AuException.ThrowIfHresultNot0(hr, "*scroll");

			//tested: Chrome and Firefox don't support UI Automation scrolling (IUIAutomationScrollItemPattern). IE and Edge support.
		}
	}
}
