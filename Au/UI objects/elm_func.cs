namespace Au
{
	public unsafe partial class elm
	{
		/// <summary>
		/// Gets the container window or control of this UI element.
		/// </summary>
		/// <remarks>
		/// Returns default(wnd) if failed. Supports <see cref="lastError"/>.
		/// All UI elements must support this property, but some have bugs and can fail or return a wrong window.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn>.
		/// </remarks>
		public wnd WndContainer {
			get {
				ThrowIfDisposed_();
				_Hresult(_FuncId.container_window, _GetWnd(out var w));
				return w;
			}
		}

		int _GetWnd(out wnd w) {
			int hr = Cpp.Cpp_AccGetInt(this, 'w', out var i);
			GC.KeepAlive(this);
			w = (wnd)i;
			return hr;
		}

		/// <summary>
		/// Gets the top-level window that contains this UI element.
		/// </summary>
		/// <remarks>
		/// Returns default(wnd) if failed. Supports <see cref="lastError"/>.
		/// All UI elements must support this property, but some have bugs and can return default(wnd).
		/// Uses API <msdn>WindowFromAccessibleObject</msdn> and API <msdn>GetAncestor</msdn>.
		/// </remarks>
		public wnd WndTopLevel => WndContainer.Window;
		//note: named not WndWindow, to avoid using accidentally instead of WndContainer.

		/// <summary>
		/// Gets location of this UI element in screen.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="GetRect(out RECT)"/>.
		/// Returns empty rectangle if failed or this property is unavailable. Supports <see cref="lastError"/>.
		/// Most but not all UI elements support this property.
		/// </remarks>
		public RECT Rect { get { GetRect(out var r); return r; } }

		/// <summary>
		/// Gets location of this UI element in screen.
		/// </summary>
		/// <param name="r">Receives rectangle in screen coordinates.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="lastError"/>.
		/// Most but not all UI elements support this property.
		/// Uses <msdn>IAccessible.accLocation</msdn>.
		/// </remarks>
		public bool GetRect(out RECT r) {
			ThrowIfDisposed_();
			var hr = _Hresult(_FuncId.rectangle, Cpp.Cpp_AccGetRect(this, out r));
			GC.KeepAlive(this);
			return hr == 0;
		}

		/// <summary>
		/// Gets location of this UI element in the client area of window w.
		/// </summary>
		/// <param name="r">Receives rectangle in w client area coordinates.</param>
		/// <param name="w">Window or control.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="lastError"/>.
		/// Most but not all UI elements support this property.
		/// Uses <msdn>IAccessible.accLocation</msdn> and <see cref="wnd.MapScreenToClient(ref RECT)"/>.
		/// </remarks>
		public bool GetRect(out RECT r, wnd w) {
			return GetRect(out r) && w.MapScreenToClient(ref r);
		}

		/// <summary>
		/// Gets standard non-string role, as enum ERole.
		/// </summary>
		/// <remarks>
		/// Most UI elements have a standard role, as enum <see cref="ERole"/>. Some UI elements have a custom role, usually as string, for example in web pages in Firefox and Chrome.
		/// Returns 0 if role is string or if failed. Supports <see cref="lastError"/>.
		/// All UI elements must support this property. If failed, probably the elm is invalid, for example the window has been closed.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </remarks>
		public ERole RoleInt {
			get {
				ThrowIfDisposed_();
				if (_misc.role != 0) return (ERole)_misc.role; //SHOULDDO: use EMiscFlags.RoleIsString
				_Hresult(_FuncId.role, _GetRole(out var role, out _, dontNeedStr: true));
				return role;
			}
		}

		/// <summary>
		/// Gets standard or custom role, as string.
		/// </summary>
		/// <remarks>
		/// Most UI elements have a standard role, defined in enum <see cref="ERole"/>. Some UI elements have a custom role, usually as string, for example in web pages in Firefox and Chrome.
		/// For standard roles this function returns enum <see cref="ERole"/> member name. For string roles - the string. For unknown non-string roles - the int value like "0" or "500".
		/// Returns "" if failed. Supports <see cref="lastError"/>.
		/// All UI elements must support this property. If failed, probably the elm is invalid, for example the window has been closed.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </remarks>
		public string Role {
			get {
				ThrowIfDisposed_();
				var role = (ERole)_misc.role;
				if (role == 0) {
					if (0 != _Hresult(_FuncId.role, _GetRole(out role, out var roleStr, dontNeedStr: false))) return "";
					if (roleStr != null) return roleStr;
				}
				var a = s_roles; uint u = (uint)role;
				return (u < a.Length) ? a[u] : ((int)role).ToString();
			}
		}

		static readonly string[] s_roles = { "0", "TITLEBAR", "MENUBAR", "SCROLLBAR", "GRIP", "SOUND", "CURSOR", "CARET", "ALERT", "WINDOW", "CLIENT", "MENUPOPUP", "MENUITEM", "TOOLTIP", "APPLICATION", "DOCUMENT", "PANE", "CHART", "DIALOG", "BORDER", "GROUPING", "SEPARATOR", "TOOLBAR", "STATUSBAR", "TABLE", "COLUMNHEADER", "ROWHEADER", "COLUMN", "ROW", "CELL", "LINK", "HELPBALLOON", "CHARACTER", "LIST", "LISTITEM", "TREE", "TREEITEM", "PAGETAB", "PROPERTYPAGE", "INDICATOR", "IMAGE", "STATICTEXT", "TEXT", "BUTTON", "CHECKBOX", "RADIOBUTTON", "COMBOBOX", "DROPLIST", "PROGRESSBAR", "DIAL", "HOTKEYFIELD", "SLIDER", "SPINBUTTON", "DIAGRAM", "ANIMATION", "EQUATION", "BUTTONDROPDOWN", "BUTTONMENU", "BUTTONDROPDOWNGRID", "WHITESPACE", "PAGETABLIST", "CLOCK", "SPLITBUTTON", "IPADDRESS", "TREEBUTTON" };

		//Returns HRESULT.
		int _GetRole(out ERole roleInt, out string roleStr, bool dontNeedStr) {
			roleStr = null;
			if (_misc.role != 0) { roleInt = (ERole)_misc.role; return 0; }
			var hr = Cpp.Cpp_AccGetRole(this, out roleInt, out var b);
			GC.KeepAlive(this);
			if (hr == 0) {
				if (b.Is0) _misc.SetRole(roleInt);
				else if (dontNeedStr) b.Dispose();
				else roleStr = b.ToStringAndDispose();
			}
			return hr;
		}

		int _GetState(out EState state) {
			int hr = Cpp.Cpp_AccGetInt(this, 's', out int i);
			GC.KeepAlive(this);
			state = (EState)i;
			return hr;
		}

		/// <summary>
		/// Gets UI element state (flags).
		/// </summary>
		/// <remarks>
		/// Returns 0 if failed. Supports <see cref="lastError"/>.
		/// Uses <msdn>IAccessible.get_accState</msdn>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// if(a.State.Has(EState.INVISIBLE)) print.it("has state INVISIBLE");
		/// if(a.IsInvisible) print.it("invisible");
		/// ]]></code>
		/// </example>
		public EState State {
			get {
				ThrowIfDisposed_();
				_Hresult(_FuncId.state, _GetState(out var state));
				return state;
			}
		}

		/// <summary> Calls <see cref="State"/> and returns true if has state CHECKED. </summary>
		public bool IsChecked => State.Has(EState.CHECKED);

		/// <summary> Calls <see cref="State"/> and returns true if has state UNAVAILABLE. </summary>
		/// <remarks>Does not check whether this UI element is in a disabled parent/ancestor UI element.</remarks>
		public bool IsDisabled => State.Has(EState.DISABLED);

		/// <summary> Calls <see cref="State"/> and returns true if has state FOCUSED. </summary>
		public bool IsFocused => State.Has(EState.FOCUSED);

		/// <summary> Calls <see cref="State"/> and returns true if has state INVISIBLE and does not have state OFFSCREEN. </summary>
		/// <remarks>
		/// If the UI element has both INVISIBLE and OFFSCREEN states, it is either invisible or just offscreen, depending on application etc. Then this function works like Find and similar functions: for most UI elements returns false (is visible), but for UI elements that have these roles returns true (invisible): WINDOW, DOCUMENT, PROPERTYPAGE, GROUPING, ALERT, MENUPOPUP.
		/// Does not check whether this UI element is in an invisible parent/ancestor UI element.
		/// </remarks>
		public bool IsInvisible => IsInvisible_(State);

		internal bool IsInvisible_(EState state) {
			if (!state.Has(EState.INVISIBLE)) return false;
			if (!state.Has(EState.OFFSCREEN)) return true;
			switch (RoleInt) {
			case ERole.WINDOW:
			case ERole.DOCUMENT:
			case ERole.PROPERTYPAGE:
			case ERole.GROUPING:
			case ERole.ALERT:
			case ERole.MENUPOPUP:
				return true;
				//note: these roles must be the same as in _IsRoleToSkipIfInvisible in "acc find.cpp"
			}
			return false;
		}

		/// <summary> Calls <see cref="State"/> and returns true if has state OFFSCREEN. </summary>
		public bool IsOffscreen => State.Has(EState.OFFSCREEN);

		/// <summary> Calls <see cref="State"/> and returns true if has state PROTECTED. </summary>
		/// <remarks>This state is used for password fields.</remarks>
		public bool IsPassword => State.Has(EState.PROTECTED);

		/// <summary> Calls <see cref="State"/> and returns true if has state PRESSED. </summary>
		public bool IsPressed => State.Has(EState.PRESSED);

		/// <summary> Calls <see cref="State"/> and returns true if has state READONLY. </summary>
		public bool IsReadonly => State.Has(EState.READONLY);

		/// <summary> Calls <see cref="State"/> and returns true if has state SELECTED. </summary>
		public bool IsSelected => State.Has(EState.SELECTED);

		/// <summary>
		/// Converts BSTR to string and disposes the BSTR.
		/// If hr is not 0, returns "" (never null).
		/// </summary>
		static string _BstrToString(int hr, BSTR b) {
			if (hr == 0) return b.ToStringAndDispose() ?? "";
			return "";
		}

		string _GetStringProp(char prop) {
			ThrowIfDisposed_();
			int hr = Cpp.Cpp_AccGetProp(this, prop, out var b);
			GC.KeepAlive(this);
			var s = _BstrToString(hr, b);
			_Hresult((_FuncId)prop, hr);
			return s;
		}

		/// <summary>
		/// Gets name.
		/// </summary>
		/// <remarks>
		/// UI element name usually is its read-only text (eg button text, link text), or its adjacent read-only text (eg text label by this edit box). It usually does not change, therefore can be used to find or identify the UI element.
		/// Returns "" if name is unavailable or if failed. Supports <see cref="lastError"/>.
		/// Uses <msdn>IAccessible.get_accName</msdn>.
		/// </remarks>
		public string Name {
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
		internal static string NameOfWindow_(wnd w) {
			if (!w.IsAlive) return null;
			var hr = Cpp.Cpp_AccFromWindow(1 | 2, w, 0, out _, out var b);
			return _BstrToString(hr, b);

			//speed: inproc ~10% faster. But first time slower, especially if process of different bitness.
		}

		/// <summary>
		/// Gets or sets value.
		/// </summary>
		/// <exception cref="AuException">Failed to set value.</exception>
		/// <remarks>
		/// Usually it is editable text or some other value that can be changed at run time, therefore in most cases it cannot be used to find or identify the UI element reliably.
		/// The 'get' function returns "" if this property is unavailable or if failed. Supports <see cref="lastError"/>.
		/// Most UI elements don't support 'set'.
		/// Uses <msdn>IAccessible.get_accValue</msdn> or <msdn>IAccessible.put_accValue</msdn>.
		/// </remarks>
		public string Value {
			get => _GetStringProp('v');
			set {
				ThrowIfDisposed_();
				AuException.ThrowIfHresultNot0(Cpp.Cpp_AccAction(this, 'v', value));
				GC.KeepAlive(this);
			}
		}

		/// <summary>
		/// Gets description.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="lastError"/>.
		/// Uses <msdn>IAccessible.get_accDescription</msdn>.
		/// </remarks>
		public string Description {
			get => _GetStringProp('d');
		}

		/// <summary>
		/// Gets help text.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="lastError"/>.
		/// Uses <msdn>IAccessible.get_accHelp</msdn>.
		/// </remarks>
		public string Help {
			get => _GetStringProp('h');
		}

		/// <summary>
		/// Gets UI Automation element AutomationId property.
		/// </summary>
		/// <remarks>
		/// Only UI elements found with flag <see cref="EFFlags.UIA"/> can have this property.
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="lastError"/>.
		/// </remarks>
		public string UiaId {
			get => _GetStringProp('u');
		}

		/// <summary>
		/// Gets keyboard shortcut.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="lastError"/>.
		/// Uses <msdn>IAccessible.get_accKeyboardShortcut</msdn>.
		/// </remarks>
		public string KeyboardShortcut {
			get => _GetStringProp('k');
		}

		/// <summary>
		/// Gets default action.
		/// See <see cref="Invoke"/>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="lastError"/>.
		/// If this is a Java UI element, returns all actions that can be used with <see cref="JavaInvoke"/>, like "action1, action2, action3", from which the first is considered default and is used by <see cref="Invoke"/>.
		/// Uses <msdn>IAccessible.get_accDefaultAction</msdn>.
		/// </remarks>
		public string DefaultAction {
			get => _GetStringProp('a');
		}

		/// <summary>
		/// Performs the UI element's default action (see <see cref="DefaultAction"/>). Usually it is 'click', 'press' or similar.
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Fails if the UI element does not have a default action. Then you can use <see cref="ExtAu.MouseClick(elm, Coord, Coord, MButton)"/>, or try <see cref="VirtualClick"/>, <see cref="Select"/>, <see cref="Focus"/> and keyboard functions.
		/// The action can take long time, for example show a dialog. This function normally does not wait. It allows the caller to automate the dialog. If it waits, try <see cref="JavaInvoke"/> or one of the above functions (MouseClick etc).
		/// Uses <msdn>IAccessible.accDoDefaultAction</msdn>.
		/// </remarks>
		public void Invoke() { //sorry, I did not find a better name. Alternatives: DoAction (weird), DoDefaultAction (too long), Execute.
			ThrowIfDisposed_();
			var hr = Cpp.Cpp_AccAction(this, 'a');
			GC.KeepAlive(this);
			AuException.ThrowIfHresultNot0(hr);
			//_MinimalSleep(); //don't need. It does not make more reliable.
		}

		//void _MinimalSleep()
		//{
		//	Thread.Sleep(15);
		//	//if(0 == _iacc.GetWnd(out var w)) w.MinimalSleepIfOtherThread_(); //better don't call getwnd
		//}

		/// <summary>
		/// Posts mouse-click messages to the container window, using coordinates of this element.
		/// </summary>
		/// <param name="button">Can specify the left (default), right or middle button. Also flag for double-click, press or release.</param>
		/// <exception cref="AuException">Failed to get rectangle, or the element is invisible/offscreen.</exception>
		/// <exception cref="NotSupportedException">Unsupported button specified.</exception>
		/// <remarks>
		/// Does not move the mouse.
		/// Does not wait until the target application finishes processing the message.
		/// Works not with all elements.
		/// Try this function when <see cref="Invoke"/> does not work and you don't want to use <c>MouseClick</c>.
		/// </remarks>
		public void VirtualClick(MButton button = MButton.Left) {
			var w = WndContainer;
			if (!GetRect(out var r, w)) throw new AuException(0, "*get rectangle");
			if (r.NoArea || State.HasAny(EState.INVISIBLE | EState.OFFSCREEN)) throw new AuException(0, "Invisible or offscreen");
			//FUTURE: Chrome bug: OFFSCREEN not updated after scrolling.

			MButton mask = MButton.Down | MButton.Up | MButton.DoubleClick, b = button & ~mask, dud = button & mask;
			if (b == 0) b = MButton.Left;
			int m = b switch {
				MButton.Left => Api.WM_LBUTTONDOWN,
				MButton.Right => Api.WM_RBUTTONDOWN,
				MButton.Middle => Api.WM_MBUTTONDOWN,
				_ => throw new ArgumentException("supported buttons: left, right, middle")
			};
			if (dud is not (0 or MButton.Down or MButton.Up or MButton.DoubleClick)) throw new ArgumentException();

			nint xy = Math2.MakeLparam(r.CenterX, r.CenterY);
			nint p = 0; if (keys.isCtrl) p |= Api.MK_CONTROL; if (keys.isShift) p |= Api.MK_SHIFT;
			nint p1 = p; if (dud != MButton.Up) p1 |= b switch { MButton.Left => Api.MK_LBUTTON, MButton.Right => Api.MK_RBUTTON, _ => Api.MK_MBUTTON };
			if (dud != MButton.Up) w.Post(m, p1, xy);
			if (dud != MButton.Down) {
				w.Post(Api.WM_MOUSEMOVE, p1, xy);
				w.Post(m + 1, p, xy);
			}
			if (dud == MButton.DoubleClick) {
				w.Post(m + 2, p1, xy);
				w.Post(m + 1, p, xy);
			}
			//_MinimalSleep(); //don't need. Invoke() does not wait too.

			//never mind: support nonclient (WM_NCRBUTTONDOWN etc)
		}

		/// <summary>
		/// Performs one of actions supported by this Java UI element.
		/// </summary>
		/// <param name="action">
		/// Action name. See <see cref="DefaultAction"/>.
		/// If null (default), performs default action (like <see cref="Invoke"/>) or posts Space key message. More info in Remarks.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Read more about Java UI elements in <see cref="elm"/> topic.
		/// 
		/// Problem: if the action opens a dialog, Invoke/JavaInvoke do not return until the dialog is closed (or fail after some time). The caller then waits and cannot automate the dialog. Also then this process cannot exit until the dialog is closed. If the action parameter is null and the UI element is focusable, this function tries a workaround: it makes the UI element (button etc) focused and posts Space key message, which should press the button; then this function does not wait.
		/// </remarks>
		public void JavaInvoke(string action = null) {
			//problem: if the button click action opens a modal dialog, doAccessibleActions waits until closed.
			//	Waits 8 s and then returns true. Somehow in QM2 returns false.
			//	During that time any JAB calls (probably from another thread) are blocked and fail. Tried various combinations.
			//	Also then releaseJavaObject does not return until the dialog closed. It even does not allow our process to exit. In QM2 the same.
			//	Previously (with old Java version?) then whole JAB crashed. Now doesn't. Or crashes only in some conditions that I now cannot reproduce.

			ThrowIfDisposed_();

			if (action == null
				&& 0 == _GetState(out var state)
				&& (state & (EState.FOCUSABLE | EState.SELECTABLE)) == EState.FOCUSABLE //must be only focusable. If SELECTABLE, probably don't need this workaround.
				&& 0 == _GetWnd(out var w)
				&& 0 == Cpp.Cpp_AccSelect(this, ESelect.TAKEFOCUS)
				&& 0 == _GetState(out state) && state.Has(EState.FOCUSED) //avoid sending keys to another control
				) {
				GC.KeepAlive(this);
				w.Post(Api.WM_KEYDOWN, (byte)KKey.Space, 0);
				w.Post(Api.WM_KEYUP, (byte)KKey.Space, 0);
				//tested: works even if the window is inactive.
				w.MinimalSleepNoCheckThread_();
				return;
			}

			var hr = Cpp.Cpp_AccAction(this, 'a', action);
			GC.KeepAlive(this);
			AuException.ThrowIfHresultNot0(hr);
			//_MinimalSleep(); //probably don't need, because JAB doAccessibleActions is sync, which is bad.
		}

		/// <summary>
		/// Calls <see cref="Invoke"/> or <i>action</i> and waits until changes the web page (window name and page name).
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).
		/// Default 60 seconds.
		/// </param>
		/// <param name="action">If used, calls it instead of <see cref="Invoke"/>.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="AuException">Failed. For example, when this UI element is invalid, or its top-level window does not contain a web page.</exception>
		/// <exception cref="AuWndException">The window was closed while waiting.</exception>
		/// <exception cref="Exception">Exceptions thrown by <see cref="Invoke"/> or by the <i>action</i> function.</exception>
		/// <remarks>
		/// This function is used to click a link in a web page and wait until current web page is gone. It prevents a following 'wait for UI element' function from finding a matching UI element in the old page, which would be bad.
		/// Does not wait until the new page is completely loaded. There is no reliable/universal way for it. Instead, after calling it you can call a 'wait for UI element' function which waits for a known UI element that must be in the new page.
		/// This function cannot be used when the new page has the same title as current page. Then it waits until <i>secondsTimeout</i> time or forever. The same if the invoked action does not open a web page.
		/// </remarks>
		public bool WebInvoke(double secondsTimeout = 60d, Action<elm> action = null) {
			wnd w = WndTopLevel; if (w.Is0) throw new AuException("*get window");
			elm doc = w.Elm["web:"].Find(-1) ?? throw new AuException("*find web page");

			string wndName = w.NameTL_, docName = doc.Name; Debug.Assert(!wndName.NE() && !docName.NE());
			bool wndOK = false, docOK = false;
			elmFinder f = null;

			if (action == null) Invoke(); else action(this);

			//wait until window name and document name both are changed. They can change in any order.
			var to = new wait.Loop(secondsTimeout, new OWait(period: 25));
			while (to.Sleep()) {
				w.ThrowIfInvalid();
				if (!wndOK) {
					var s = w.NameTL_;
					if (s == null) continue; //probably invalid, will throw in next loop
					wndOK = s != wndName;
				}
				if (wndOK && !docOK) {
					f ??= new elmFinder("web:") { ResultGetProperty = 'n' };
					if (!w.HasElm(f)) continue; //eg in Firefox for some time there is no DOCUMENT
					if (f.ResultProperty is not string s) continue;
					docOK = s != docName;
					//if(!docOK) print.it("doc is late");
				}
				if (wndOK && docOK) {
					w.ThrowIfInvalid();
					return true;
				}
			}
			return false;
		}

#if false
		//This function is finished and normally works well.
		//However web browsers not always fire the event. For some pages never, or only when not cached.
		//Also, some pages are like never finish loading (the browser waiting animation does not stop spinning). Or finish when the wanted UI element is there for long time, so why to wait. Or finish, then continue loading again...
		//Also, this function inevitably will stop working with some new web browser version with new bugs. Too unreliable.
		public bool InvokeAndWaitForWebPageLoaded(double secondsTimeout = 60, Action<elm> action = null, wnd w = default)
		{
			ThrowIfDisposed_();

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

			var hookEvent = EEvent.IA2_DOCUMENT_LOAD_COMPLETE;
			int browser = w.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
			switch(browser) {
			case 0:
				wnd ies = w.Child(null, Api.string_IES); if(ies.Is0) break;
				w = ies; goto case 1;
			case 1: hookEvent = EEvent.OBJECT_CREATE; break;
			}

			int tid = w.ThreadId; if(tid == 0) w.ThrowUseNative();
			AutoResetEvent eventNotify = null;
			int debugIndex = 0;

			Api.WINEVENTPROC hook = (IntPtr hWinEventHook, EEvent ev, wnd hwnd, int idObject, int idChild, int idEventThread, int time) =>
			{
				if(eventNotify == null) { /*print.it("null 1");*/ return; }
				if(ev == EEvent.OBJECT_CREATE && hwnd != w) return; //note: in Chrome hwnd is Chrome_RenderWidgetHostHWND
				int di = ++debugIndex;
				using(var a = elm.fromEvent(hwnd, idObject, idChild)) {
					if(a == null) { /*Debug_.Print("elm.fromEvent null");*/ return; } //often IE, but these are not useful UI elements
					if(eventNotify == null) { /*print.it("null 2");*/ return; }
					if(ev == EEvent.IA2_DOCUMENT_LOAD_COMPLETE) { //Chrome, Firefox

						//filter out frame/iframe
						if(browser == 2) { //Firefox does not fire events for frame/iframe. But the Chrome code would work too.
						} else if(0 == a._iacc.get_accParent(out var a2)) { //bug in some Chrome versions: fails for main document
							using(a2) {
								if(eventNotify == null) { /*print.it("null 3");*/ return; }
								bool isFrame;
								var hr = a2.GetRole(0, out var role, out var roleStr); if(hr != 0) Debug_.Print((uint)hr);
								if(eventNotify == null) { /*print.it("null 4");*/ return; }
								if(hr != 0) isFrame = false;
								else if(roleStr != null) isFrame = roleStr.Ends("frame", true);
								else isFrame = !(role == ERole.WINDOW || role == ERole.CLIENT);
								//print.it(role, roleStr);
								if(isFrame) return;
								//browser    main        frame     iframe
								//Firefox    "browser"   "frame"   "iframe"
								//Chrome     WINDOW      "FRAME"   DOCUMENT
							}
						}
					} else { //IE (OBJECT_CREATE)
						if(a._elem != 0) return;
						if(0 != a._iacc.GetRole(0, out var role) || role != ERole.PANE) return;
						if(eventNotify == null) { /*print.it("null 3");*/ return; }

						//filter out frame/iframe
						if(a.IsInvisible) return;
						if(eventNotify == null) { /*print.it("null 4");*/ return; }
						using(var aCLIENT = _FromWindow(w, EObjid.CLIENT, noThrow: true)) {
							if(eventNotify == null) { /*print.it("null 5");*/ return; }
							if(aCLIENT != null) {
								var URL1 = a.Value; Debug.Assert(URL1.Length > 0); //print.it(URL1); //http:..., about:...
								aCLIENT.get_accName(0, out var URL2, 0); Debug.Assert(URL2.Length > 0);
								if(URL1 != URL2) return;
								if(eventNotify == null) { /*print.it("null 6");*/ return; }
							} else Debug_.Print("aCLIENT null");
						}
					}

					//print.it(di, ev, a);
					eventNotify.Set();
					eventNotify = null;
				}
			};

			var hh = Api.SetWinEventHook(hookEvent, hookEvent, default, hook, 0, tid, 0); if(hh == default) throw new AuException();
			try {
				eventNotify = new AutoResetEvent(false);
				if(action != null) action(this); else Invoke();
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
		/// <param name="how">Specifies whether to select, focus, add to selection etc. Can be two flags, for example <c>ESelect.TAKEFOCUS | ESelect.TAKESELECTION</c>.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <exception cref="AuWndException">Failed to activate the window (<see cref="wnd.Activate"/>) or focus the control (<see cref="wnd.Focus"/>).</exception>
		/// <remarks>
		/// Uses <msdn>IAccessible.accSelect</msdn>.
		/// Not all UI elements support it. Most UI elements support not all flags. It depends on <see cref="EState"/> FOCUSABLE, SELECTABLE, MULTISELECTABLE, EXTSELECTABLE, DISABLED.
		/// Many UI elements have bugs, especially with flag TAKEFOCUS. More bugs when the UI element has been found with flag <see cref="EFFlags.NotInProc"/>.
		/// </remarks>
		public void Select(ESelect how = ESelect.TAKESELECTION) {
			ThrowIfDisposed_();

			//Workaround for Windows controls bugs, part 1.
			wnd w = default, wTL = default; bool focusingControl = false;
			if (how.Has(ESelect.TAKEFOCUS) && 0 == _GetWnd(out w)) {
				//if(!w.IsEnabled(true)) throw new AuException("*set focus. Disabled"); //accSelect would not fail //rejected. In some cases the UI element may be focusable although window disabled, eg KTreeView.
				wTL = w.Window;
				wTL.Activate();
				if (focusingControl = (w != wTL))
					if (w.IsEnabled()) //see above. Would be exception if disabled.
						w.Focus();
				if (IsFocused) how &= ~ESelect.TAKEFOCUS;
				if (how == 0) return;
			}

			for (int i = 0; i < 2; i++) {
				var hr = Cpp.Cpp_AccSelect(this, how);
				GC.KeepAlive(this);
				if (hr == 0) break;
				if (hr == 1) continue; //some UI elements return S_FALSE even if did what asked. Eg combobox (focuses the child Edit), slider. Or may need to retry, eg when trying to focus a listitem in a non-focused listbox.
				if (hr == Api.DISP_E_MEMBERNOTFOUND) throw new AuException("This UI element does not support this state");
				AuException.ThrowIfHresultNegative(hr);
			}

			if (!w.Is0) w.MinimalSleepIfOtherThread_(); //sleep only when focusing. Assume selection is sync. Also need for the bug, because the control may be activated a millisecond later.

			//Workaround for Windows controls bugs, part 2.
			if (focusingControl && w.IsActive) {
				//Debug_.Print("activated control");
				wTL.Activate();
			}

			//tested: IAccessible.accSelect(TAKEFOCUS):
			//	Most Windows controls have this bug: activates the control with SetForegroundWindow, which deactivates the top-level window.
			//		Especially if the control is already focused.
			//		If not already focused, fails if eg listbox item. But then works well with eg buttons.
			//		MSDN: If IAccessible::accSelect is called with the SELFLAG_TAKEFOCUS flag on a child UI element that has an HWND, the flag takes effect only if the UI element's parent has the focus.
			//		Tested, does not help: LockSetForegroundWindow, AttachThreadInput.
			//		Good news: works well if the UI element found inproc, ie without flag NotInproc.
			//			But then need to focus the control, else does not work.
			//			Use the same workaround. It focuses the control.
			//	Works well with web browsers, WinForms.
			//	With WPF initially almost does not work. After using a navigation key (Tab etc) starts to work well.
			//tested: UIA.IElement.SetFocus:
			//	In most cases works well with standard controls, all web browsers, WinForms.
			//	With WPF same as elm.
			//	Bug: If standard control is disabled, deactivates parent window and draws focus rectangle on the control.
		}

		/// <summary>
		/// Makes this UI element focused for keyboard input.
		/// </summary>
		/// <param name="andSelect">Add flag TAKESELECTION. Note: it is for selecting a list item, not for selecting text in a text box.</param>
		/// <remarks>
		/// Calls <see cref="Select"/> with flag TAKEFOCUS and optionally TAKESELECTION.
		/// Not all UI elements support this action and not all work correctly. More info in Select documentation.
		/// </remarks>
		public void Focus(bool andSelect = false) {
			var how = ESelect.TAKEFOCUS;
			if (andSelect) how |= ESelect.TAKESELECTION;
			Select(how);
		}

		/// <summary>
		/// Gets selected direct child items.
		/// Returns empty array if there are no selected items of if failed. Supports <see cref="lastError"/>.
		/// </summary>
		public elm[] SelectedChildren {
			get {
				ThrowIfDisposed_();
				if (_elem != 0) { lastError.clear(); return Array.Empty<elm>(); }
				//return _iacc.get_accSelection();
				if (0 != _Hresult(_FuncId.selection, Cpp.Cpp_AccGetSelection(this, out var b)) || b.Is0) return Array.Empty<elm>();
				GC.KeepAlive(this);
				var p = (Cpp.Cpp_Acc*)b.Ptr; int n = b.Length / sizeof(Cpp.Cpp_Acc);
				var r = new elm[n];
				for (int i = 0; i < n; i++) r[i] = new elm(p[i]);
				b.Dispose();
				return r;
			}
		}

		/// <summary>
		/// Gets the number of direct child UI elements.
		/// </summary>
		/// <remarks>
		/// Uses <msdn>IAccessible.get_accChildCount</msdn>.
		/// </remarks>
		public int ChildCount {
			get {
				ThrowIfDisposed_();
				if (_elem != 0) { lastError.clear(); return 0; }
				_Hresult(_FuncId.child_count, Cpp.Cpp_AccGetInt(this, 'c', out int cc));
				GC.KeepAlive(this);
				return cc;
			}
		}

		/// <summary>
		/// Gets multiple properties.
		/// </summary>
		/// <param name="props">
		/// String that specifies properties to get, for example "nv" for name and value.
		/// - R - <see cref="Role"/>.
		/// - n - <see cref="Name"/>.
		/// - v - <see cref="Value"/>.
		/// - d - <see cref="Description"/>.
		/// - h - <see cref="Help"/>.
		/// - a - <see cref="DefaultAction"/>.
		/// - k - <see cref="KeyboardShortcut"/>.
		/// - u - <see cref="UiaId"/>.
		/// - s - <see cref="State"/>.
		/// - r - <see cref="Rect"/>.
		/// - w - <see cref="WndContainer"/>.
		/// - o - <see cref="Html"/> outer.
		/// - i - <see cref="Html"/> inner.
		/// - @ - <see cref="HtmlAttributes"/>.
		/// </param>
		/// <param name="result">Receives results.</param>
		/// <exception cref="ArgumentException">Unknown property character.</exception>
		/// <remarks>
		/// The returned variable contains values of properties specified in <i>props</i>. When a property is empty or failed to get, the member variable is "", empty dictionary or default value of that type; never null.
		/// 
		/// Normally this function is faster than calling multiple property functions, because it makes single remote procedure call. But not if this UI element was found with flag <see cref="EFFlags.NotInProc"/> etc.
		/// 
		/// Returns false if fails, for example when the UI element's window is closed. Supports <see cref="lastError"/>.
		/// </remarks>
		public bool GetProperties(string props, out EProperties result) {
			//SHOULDDO: use cached role. Or not, because now can help to catch bugs where the cached role is incorrect.

			result = default;
			ThrowIfDisposed_();
			if (props.Length == 0) return true;
			int hr = Cpp.Cpp_AccGetProps(this, props, out var b);
			GC.KeepAlive(this);
			if (hr != 0) {
				if (hr == (int)Cpp.EError.InvalidParameter) throw new ArgumentException("Unknown property character.");
				lastError.code = hr;
				return false;
			}
			using (b) {
				var offsets = (int*)b.Ptr;
				for (int i = 0; i < props.Length; i++) {
					int offs = offsets[i], len = ((i == props.Length - 1) ? b.Length : offsets[i + 1]) - offs;
					var p = b.Ptr + offs;
					switch (props[i]) {
					case 'r': result.Rect = len > 0 ? *(RECT*)p : default; break;
					case 's': result.State = len > 0 ? *(EState*)p : default; break;
					case 'w': result.WndContainer = len > 0 ? (wnd)(*(int*)p) : default; break;
					case '@': result.HtmlAttributes = AttributesToDictionary_(p, len); break;
					default:
						var s = (len == 0) ? "" : new string(p, 0, len);
						switch (props[i]) {
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

		internal static Dictionary<string, string> AttributesToDictionary_(char* p, int len) {
			var d = new Dictionary<string, string>();
			int ik = 0, iv = 0;
			for (int i = 0; i < len; i++) {
				var c = p[i];
				if (c == '\0' && iv > ik) {
					string sk = new string(p, ik, iv - ik - 1);
					string sv = new string(p, iv, i - iv);
					d[sk] = sv;
					ik = i + 1;
				} else if (c == '=' && iv <= ik) {
					iv = i + 1;
				}
			}
			//print.it(d);
			return d;
		}

		/// <summary>
		/// Gets an adjacent or related UI element - next, child, parent, etc.
		/// Returns null if not found.
		/// </summary>
		/// <param name="navig">
		/// String consisting of one or more navigation direction strings separated by space, like <c>"parent next child4 first"</c>.
		/// - <c>"next"</c> - next sibling UI element in the same parent UI element.
		/// - <c>"previous"</c> - previous sibling UI element in the same parent UI element.
		/// - <c>"first"</c> - first child UI element.
		/// - <c>"last"</c> - last child UI element.
		/// - <c>"parent"</c> - parent (container) UI element.
		/// - <c>"child"</c> - child UI element by 1-based index. Example: <c>"child3"</c> (3-th child). Negative index means from end, for example -1 is the last child.
		/// - <c>"#N"</c> - custom. More info in Remarks.
		/// </param>
		/// <param name="waitS">Wait for the wanted UI element max this number of seconds. If negative, waits forever.</param>
		/// <exception cref="ArgumentException">Invalid <i>navig</i> string.</exception>
		/// <remarks>
		/// Can be only 2 letters, like <c>"pr"</c> for <c>"previous"</c>.
		/// A string like <c>"next3"</c> or <c>"next,3"</c> is the same as <c>"next next next"</c>. Except for <c>"child"</c>.
		/// Use string like <c>"#1000"</c> to specify a custom <i>navDir</i> value to pass to <msdn>IAccessible.accNavigate</msdn>. Can be any standard or custom value supported by the UI element.
		/// 
		/// For <c>"next"</c>, <c>"previous"</c>, <c>"firstchild"</c>, <c>"lastchild"</c> and <c>"#N"</c> is used <msdn>IAccessible.accNavigate</msdn>. Not all UI elements support it. Some UI elements skip invisible siblings. Instead you can use <c>"parent childN"</c> or <c>"childN"</c>.
		/// For <c>"parent"</c> is used <msdn>IAccessible.get_accParent</msdn>. Few UI elements don't support. Some UI elements return a different parent than in the tree of UI elements.
		/// For <c>"child"</c> is used API <msdn>AccessibleChildren</msdn>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// a = a.Navigate("parent next ch3", true);
		/// ]]></code>
		/// </example>
		public elm Navigate(string navig, double waitS = 0) {
			ThrowIfDisposed_();
			int hr; Cpp.Cpp_Acc ca;
			if (waitS == 0) {
				hr = Cpp.Cpp_AccNavigate(this, navig, out ca);
			} else {
				var to = new wait.Loop(waitS > 0 ? -waitS : 0.0);
				do hr = Cpp.Cpp_AccNavigate(this, navig, out ca);
				while (hr != 0 && hr != (int)Cpp.EError.InvalidParameter && to.Sleep());
			}
			GC.KeepAlive(this);
			if (hr == (int)Cpp.EError.InvalidParameter) throw new ArgumentException("Invalid navig string.");
			lastError.code = hr;
			return hr == 0 ? new elm(ca) : null;

			//FUTURE: when fails, possibly this is disconnected etc. Retry find with same elmFinder.
		}

		/// <summary>
		/// Gets parent element. Same as <see cref="Navigate"/> with argument "pa".
		/// Returns null if fails.
		/// </summary>
		public elm Parent => Navigate("pa");
		//rejected: public elm Parent => call get_accParent directly. Can use Navigate(), it's almost as fast. Useful mostly in programming, not in scripts.

		/// <summary>
		/// Gets HTML.
		/// </summary>
		/// <param name="outer">If true, gets outer HTML (with tag and attributes), else inner HTML.</param>
		/// <remarks>
		/// Returns "" if this is not a HTML element or if failed. Supports <see cref="lastError"/>.
		/// Works with Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). This UI element must be found without flag NotInProc.
		/// If this is the root of web page (role DOCUMENT or PANE), gets web page body HTML.
		/// </remarks>
		public string Html(bool outer) {
			ThrowIfDisposed_();
			int hr = _Hresult(_FuncId.html, Cpp.Cpp_AccWeb(this, outer ? "'o" : "'i", out BSTR s));
			GC.KeepAlive(this);
			return _BstrToString(hr, s);
		}

		/// <summary>
		/// Gets a HTML attribute.
		/// </summary>
		/// <param name="name">Attribute name, for example <c>"href"</c>, <c>"id"</c>, <c>"class"</c>. Full, case-sensitive.</param>
		/// <remarks>
		/// Returns "" if this is not a HTML element or does not have the specified attribute or failed. Supports <see cref="lastError"/>.
		/// Works with Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). This UI element must be found without flag NotInProc.
		/// </remarks>
		/// <exception cref="ArgumentException">name is null/""/invalid.</exception>
		public string HtmlAttribute(string name) {
			ThrowIfDisposed_();
			if (name.NE() || name[0] == '\'') throw new ArgumentException("Invalid name.");
			int hr = _Hresult(_FuncId.html, Cpp.Cpp_AccWeb(this, name, out BSTR s));
			GC.KeepAlive(this);
			return _BstrToString(hr, s);
		}

		/// <summary>
		/// Gets all HTML attributes.
		/// </summary>
		/// <remarks>
		/// Returns empty dictionary if this is not a HTML element or does not have attributes or failed. Supports <see cref="lastError"/>.
		/// Works with Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, web browser controls...). This UI element must be found without flag NotInProc.
		/// </remarks>
		public Dictionary<string, string> HtmlAttributes() {
			ThrowIfDisposed_();
			int hr = Cpp.Cpp_AccWeb(this, "'a", out BSTR s);
			GC.KeepAlive(this);
			_Hresult(_FuncId.html, hr);
			if (hr != 0) return new Dictionary<string, string>();
			using (s) return AttributesToDictionary_(s.Ptr, s.Length);
		}

		/// <summary>
		/// Scrolls this UI element into view.
		/// </summary>
		/// <exception cref="AuException">Failed to scroll, or the UI element does not support scrolling.</exception>
		/// <remarks>
		/// This function works with these UI elements:
		/// - Web page elements in Firefox, Chrome, Internet Explorer and apps that use their code (Thunderbird, Opera, Edge, web browser controls...). With Find use role prefix "web:", "firefox:" or "chrome:", and don't use flag <see cref="EFFlags.NotInProc"/>.
		/// - Standard treeview and listview controls, some other. With <b>Find</b> use flag <see cref="EFFlags.UIA"/>.
		/// </remarks>
		public void ScrollTo() {
			ThrowIfDisposed_();

			int hr;
			if (_misc.flags.Has(EMiscFlags.UIA)) hr = Cpp.Cpp_AccAction(this, 's');
			else hr = Cpp.Cpp_AccWeb(this, "'s", out _);
			GC.KeepAlive(this);

			AuException.ThrowIfHresultNot0(hr, "*scroll");

			//tested: Chrome and Firefox don't support UI Automation scrolling (IUIAutomationScrollItemPattern).
		}
	}
}
