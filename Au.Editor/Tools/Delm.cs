#define THREAD

using System.Windows;
using System.Windows.Controls;
using Au.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Linq;

//SHOULDDO: like in QM2, option to capture smallest object at that point.
//SHOULDDO: if checked 'state', activate window before test. Else different FOCUSED etc.
//SHOULDDO: capture image to display in editor.
//SHOULDDO: sometimes VS 2022 hangs when capturing (noticed 2 times on Ctrl+Shift+E).

//FUTURE: tabs: | Window | Element | + |
//	The + button adds "find in element". Or add checkboxes in tree.
//	Then code: var w = wnd.find(...); var e = w.Elm[...][...].Find();
//	Or use "add to path", like now in Duiimage "add to array".

//TODO: if does not find in intermediate element, suggest to specify skip -1. Also in intermediate element let it be default textbox value (but unchecked).

namespace Au.Tools
{
	class Delm : KDialogWindow
	{
		elm _elm;
		wnd _wnd, _con;
		bool _useCon;
		string _wndName;

		KSciInfoBox _info;
		Button _bTest, _bTestAction, _bOK, _bInsert, _bSett;
		Label _speed;
		ComboBox _cbAction;
		KCheckBox _cCapture;
		ScrollViewer _scroller;
		Border _attr;
		KSciCodeBoxWnd _code;
		KTreeView _tree;

		KCheckTextBox roleA, nameA, uiaidA, idA, classA, valueA, descriptionA, actionA, keyA, helpA, elemA, stateA, rectA, alsoA, skipA, navigA, waitA, notinA, maxccA, levelA;
		KCheckBox controlC, exceptionA, hiddenTooA, reverseA, uiaA, notInprocA, clientAreaA, menuTooA;

		public Delm(POINT? p = null) {
			if (p != null) _ElmFromPoint(p.Value, out _elm, capturing: true, ctor: true); //will be processed in OnLoad

			Title = "Find UI element";

			var b = new wpfBuilder(this).WinSize((600, 450..), (600, 450..)).Columns(-1, 0);
			b.R.Add(out _info).Height(60);
			b.R.StartGrid().Columns(0, 0, 0, -1);
			b.R.AddButton(out _bTest, "_Test", _ => _Test()).Size(70, 21).Align("L").Disabled().Tooltip("Executes the 'find' code now.\nShows rectangle of the found UI element.\nIgnores options: wait, Exception, Action.");
			b.And(30).AddButton(out _bTestAction, "TA", _ => _Test(testAction: true)).Disabled().Tooltip("Test action. Finds the UI element and calls the selected action function.");
			b.AddOkCancel(out _bOK, out _, out _).Margin("T0");
			b.AddButton(out _bInsert, "_Insert", _ => _Insert(false)).Size(70, 21).Disabled().Tooltip("Insert code and don't close.");
			b.Add(out _cCapture, "_Capture").Align("R", "C").Tooltip("Enables hotkeys F3 and Ctrl+F3. Shows UI element rectangles when moving the mouse.");
			b.R.Add(out _speed).Tooltip("The search time (window + element). Red if not found.")
			.Add(out _cbAction).Items("Set variable|Invoke|WebInvoke|JavaInvoke|MouseClick|MouseClick(*2)|MouseClick(right)|MouseMove|VirtualClick|VirtualClick(*2)|VirtualClick(right)|Focus|Select|ScrollTo")
			.And(21).AddButton(out _bSett, "...", _ => _Options());
			exceptionA = b.xAddCheck("Exception if not found", noNewRow: true, check: true);
			b.End();
			b.R.AddSeparator(false);
			_bOK.IsEnabled = false;
			b.OkApply += _bOK_Click;
			_InitActionCombo();

			//elm properties, other parameters, search settings
			b.Row(184);
			_scroller = b.xStartPropertyGrid("L2 T3 R2 B1"); _scroller.Visibility = Visibility.Hidden;
			b.Columns(-2, 0, -1);
			//elm properties
			b.Row(0);
			b.StartGrid(); //left side
			roleA = b.xAddCheckText("role");
			nameA = b.xAddCheckText("name");
			uiaidA = b.xAddCheckText("uiaid");
			idA = b.xAddCheckText("id");
			classA = b.xAddCheckText("class");
			valueA = b.xAddCheckText("value");
			descriptionA = b.xAddCheckText("description");
			actionA = b.xAddCheckText("action");
			keyA = b.xAddCheckText("key");
			helpA = b.xAddCheckText("help");
			b.R.Add(out _attr); //HTML attributes will be added with another builder
			elemA = b.xAddCheckText("elem");
			stateA = b.xAddCheckText("state");
			rectA = b.xAddCheckText("rect");
			b.End();
			b.xAddSplitterV(span: 4, thickness: 12);
			//other parameters
			b.StartGrid(); //right side
			controlC = b.xAddCheck("Control");
			b.xAddButton("Window/control...", _bWnd_Click);
			alsoA = b.xAddCheckText("also", "o=>true");
			skipA = b.xAddCheckText("skip");
			navigA = b.xAddCheckText("navig");
			waitA = b.xAddCheckText("wait", "1", check: true);
			//search settings
			hiddenTooA = b.xAddCheck("Find hidden too");
			reverseA = b.xAddCheck("Reverse order");
			uiaA = b.xAddCheck("UI Automation");
			notInprocA = b.xAddCheck("Not in-process");
			clientAreaA = b.xAddCheck("Only client area");
			menuTooA = b.xAddCheck("Can be in menu");
			notinA = b.xAddCheckText("Not in");
			maxccA = b.xAddCheckText("maxcc");
			levelA = b.xAddCheckText("level");
			b.End();
			b.xEndPropertyGrid();
			b.R.AddSeparator(false);

			//code
			b.Row(64).xAddInBorder(out _code, "B");

			//tree
			b.xAddSplitterH(span: -1);
			b.Row(-1).StartGrid().Columns(-1, 0, -1);
			b.Row(-1).xAddInBorder(out _tree, "T");
			b.End();

			b.End();

			_InitTree();

			b.WinProperties(
				topmost: true,
				showActivated: _elm != null ? false : null //eg if captured a popup menu item, activating this window closes the menu and we cannot get properties
				);

			WndSavedRect.Restore(this, App.Settings.tools_Delm_wndPos, o => App.Settings.tools_Delm_wndPos = o);
		}

		static Delm() {
			TUtil.OnAnyCheckTextBoxValueChanged<Delm>((d, o) => d._AnyCheckTextBoxValueChanged(o));
		}

		public static void Dialog(POINT? p = null) {
#if THREAD
			if (Environment.CurrentManagedThreadId != 1) { //cannot simply pass an iaccessible to other thread
				_Show(p, false);
			} else {
				run.thread(() => _Show(p, true)); //don't allow main thread to hang when something is slow when working with UI elements
			}

			static void _Show(POINT? p, bool dialog) {
				try { //unhandled exception kills process if in nonmain thread
					var d = new Delm(p);
					if (dialog) d.ShowDialog(); else d.Show();
				}
				catch (Exception e1) { print.it(e1); }
			}
#else
			new Delm(e).Show();
#endif
		}

		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);

			if (_elm != null) _SetElm(false);
			_InitInfo();
			_cCapture.IsChecked = true;
		}

		protected override void OnClosing(CancelEventArgs e) {
			_cCapture.IsChecked = false;

			base.OnClosing(e);
		}

		void _SetElm(bool captured) {
			wnd c = _elm.WndContainer, w = c.Window;
			if (w.Is0) return;
			if (captured && w.IsCloaked) {
				//workaround for old pre-Chromium Edge. w is a cloaked windowsuicorecorewindow of other process. There are many such cloaked windows, and wnd.find often finds wrong window.
				c = wnd.fromMouse();
				w = c.Window;
				if (w.Is0) return;
			}

			string wndName = w.NameTL_;
			bool sameWnd = captured && w == _wnd && wndName == _wndName;
			_wndName = wndName;

			bool useCon = _useCon && captured && sameWnd && c == _con;
			//if control is in other thread, search in control by default, elso slow because cannot use inproc. Except for known windows.
			if (!useCon) useCon = c != w && c.ThreadId != w.ThreadId && 0 == c.ClassNameIs(Api.string_IES, "Windows.UI.Core.CoreWindow");

			_SetWndCon(w, c, useCon);

			if (!_FillPropertiesTreeAndCode(captured, sameWnd)) return;

			_bTest.IsEnabled = true; _bTestAction.IsEnabled = true; _bOK.IsEnabled = true; _bInsert.IsEnabled = true;

			//bool quickOK = !captured && App.Settings.tools_Delm_flags.Has(EOptions.QuickOK);
			//if (App.Settings.tools_Delm_flags.HasAny(EOptions.AutoTest | EOptions.QuickOK)) timerm.after(1, _ => _bTest_Click(quickOK));
			if (App.Settings.tools_Delm_flags.HasAny(EOptions.AutoTest | EOptions.AutoTestAction | EOptions.QuickInsert))
				timerm.after(1, _ => _Test(true, testAction: App.Settings.tools_Delm_flags.Has(EOptions.AutoTestAction), testActionNoActivate: true));
		}

		void _SetWndCon(wnd w, wnd con, bool useCon = false) {
			_wnd = w;
			_con = con == w ? default : con;
			_useCon = useCon && !_con.Is0;
			_noeventValueChanged = true;
			controlC.IsChecked = _useCon; controlC.IsEnabled = !_con.Is0;
			_noeventValueChanged = false;
		}

		bool _FillPropertiesTreeAndCode(bool captured = false, bool sameWnd = false) {
			//_nodeCaptured = null;

			//perf.first();
			bool sameTree = sameWnd && _TrySelectInSameTree();
			////perf.next();

			if (!sameTree) _ClearTree();
			if (!_FillProperties(out var p)) return false;
			if (!sameTree) {
				Mouse.SetCursor(Cursors.Wait);
				_FillTree(p);
				Mouse.SetCursor(Cursors.Arrow);
			}
			_FormatCode();

			if (captured && p.Role == "CLIENT" && _wnd.ClassNameIs("SunAwt*") && !_elm.MiscFlags.Has(EMiscFlags.Java) && !osVersion.is32BitOS)
				_info.zText = c_infoJava;

			//_nodeCaptured = _tree.SelectedItem as _TreeItem; //print.it(_nodeCaptured?.e);
			//perf.nw();
			return true;
		}

		bool _FillProperties(out EProperties p) {
			_attr.Child = null;

			if (!_elm.GetProperties("Rnuvdakh@srw", out p)) {
				_info.InfoError("Failed to get UI element properties", lastError.message);
				_scroller.Visibility = Visibility.Hidden;
				return false;
			}
			_scroller.Visibility = Visibility.Visible;

			_noeventValueChanged = true;

			bool isWeb = _IsVisibleWebPage(_elm, out var browser, _con.Is0 ? _wnd : _con);
			_isWebIE = isWeb && browser == _BrowserEnum.IE;

			var role = p.Role; if (isWeb) role = "web:" + role;
			_SetHideIfEmpty(roleA, role, check: true, escape: false);
			//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
			bool noName = !_SetHideIfEmpty(nameA, p.Name, check: true, escape: true, dontHide: true);
			if (_SetHideIfEmpty(uiaidA, p.UiaId, check: noName, escape: true)) noName = false;

			//control
			bool isClassId = !isWeb && !_con.Is0 && !_useCon;
			idA.Visible = isClassId;
			classA.Visible = isClassId;
			if (isClassId) {
				string sId = TUtil.GetUsefulControlId(_con, _wnd, out int id) ? id.ToString() : _con.NameWinforms;
				bool hasId = _SetHideIfEmpty(idA, sId, check: true, escape: false);
				_Set(classA, TUtil.StripWndClassName(_con.ClassName, true), check: !hasId);
			}

			_SetHideIfEmpty(valueA, p.Value, check: false, escape: true);
			if (_SetHideIfEmpty(descriptionA, p.Description, check: noName, escape: true)) noName = false;
			_SetHideIfEmpty(actionA, p.DefaultAction, check: false, escape: true);
			if (_SetHideIfEmpty(keyA, p.KeyboardShortcut, check: noName, escape: true)) noName = false;
			if (_SetHideIfEmpty(helpA, p.Help, check: noName, escape: true)) noName = false;
			if (p.HtmlAttributes.Count > 0) {
				var b = new wpfBuilder(_attr).Columns((0, ..100), -1).Options(modifyPadding: false, margin: new());
				foreach (var attr in p.HtmlAttributes) {
					string na = attr.Key, va = attr.Value;
					bool check = noName && (na == "id" || na == "name") && va.Length > 0;
					var k = b.xAddCheckText("@" + na, TUtil.EscapeWildex(va));
					if (check) { k.c.IsChecked = true; noName = false; }
					var info = TUtil.CommonInfos.AppendWildexInfo(TUtil.CommonInfos.PrependName(na, "HTML attribute."));
					_info.AddElem(k.c, info);
					_info.AddElem(k.t, info);
				}
				b.End();
			}
			int elem = _elm.SimpleElementId; if (elem != 0) _Set(elemA, elem.ToS()); else elemA.Visible = false;
			_Set(stateA, p.State.ToString());
			_Set(rectA, $"{{W={p.Rect.Width} H={p.Rect.Height}}}");

			alsoA.c.IsChecked = false;
			skipA.c.IsChecked = false;
			navigA.c.IsChecked = false;
			if (isWeb && !_waitAutoCheckedOnce) waitA.c.IsChecked = _waitAutoCheckedOnce = true;
			uiaA.IsChecked = _elm.MiscFlags.Has(EMiscFlags.UIA);

			_noeventValueChanged = false;
			return true;

			void _Set(KCheckTextBox ct, string value, bool check = false) {
				ct.t.Text = value;
				ct.c.IsChecked = check;
			}

			bool _SetHideIfEmpty(KCheckTextBox ct, string value, bool check, bool escape, bool dontHide = false) {
				bool empty = value.NE();
				ct.Visible = !empty || dontHide;
				if (empty) check = false;
				else if (escape) value = TUtil.EscapeWildex(value);
				ct.t.Text = value;
				ct.c.IsChecked = check;
				return !empty;
			}
		}

		private void _bWnd_Click(WBButtonClickArgs e) {
			var wPrev = _WndSearchIn;
			bool captCheck = _cCapture.IsChecked;
			var r = _code.ZShowWndTool(this, _wnd, _con, !_useCon);
			if (captCheck) _cCapture.IsChecked = true;
			if (!r.ok) return;
			_SetWndCon(r.w, r.con, r.useCon);
			if (_WndSearchIn != wPrev) _FillPropertiesTreeAndCode();
		}

		//when checked/unchecked any checkbox, and when text changed of any textbox
		void _AnyCheckTextBoxValueChanged(object source) {
			if (source == _cCapture) {
				_cCapture_CheckedChanged();
			} else if (!_noeventValueChanged) {
				_noeventValueChanged = true;
				if (source is KCheckBox c) {
					if (c == controlC) {
						_useCon = c.IsChecked;
						_FillPropertiesTreeAndCode();
						_noeventValueChanged = false;
						return;
					} else if (c == uiaA) {
						_uiaUserChecked = c.IsChecked;
						_ClearTree();
						_cCapture.IsChecked = true;
						TUtil.InfoTooltip(ref _ttRecapture, c, "Please capture the UI element again.");
					}
				} else if (source is TextBox t && t.Tag is KCheckTextBox k) {
					k.CheckIfTextNotEmpty();
				}
				_noeventValueChanged = false;

				_FormatCode();
			}
		}
		bool _noeventValueChanged = true;
		KPopup _ttRecapture;

		(string code, string wndVar) _FormatCode(bool forTest = false, bool testAction = false) {
			if (!_scroller.IsVisible) return default; //failed to get UI element props

			var (wndCode, wndVar) = _code.ZGetWndFindCode(_wnd, _useCon ? _con : default);

			bool isCall = (!forTest || testAction) && _cbAction.SelectedIndex > 0;

			var b = new StringBuilder();
			b.AppendLine(wndCode);
			if (!(forTest | isCall)) b.Append("var e = ");
			b.Append(wndVar).Append(".Elm[");

			roleA.GetText(out var role, emptyToo: true);
			b.AppendStringArg(role, noComma: true);

			bool isName = nameA.GetText(out var name, emptyToo: true);
			if (isName) b.AppendStringArg(name);

			int nProp = 0, propStart = 0;
			void _AppendProp(KCheckTextBox k, bool emptyToo = false) {
				if (!k.GetText(out var va, emptyToo)) return;
				if (k == levelA && va == "0 1000") return;
				if (k == maxccA && va == "10000") return;
				if (nProp++ == 0) {
					b.Append(", ");
					if (!isName) b.Append("prop: ");
					propStart = b.Length;
					b.Append("new(");
				} else b.Append(", ");
				int j = b.Length;
				b.Append('\"').Append(k.c.Content as string).Append('=');
				if (!va.NE()) {
					if (nProp == 1 && va.Contains('|')) nProp++;
					if (TUtil.IsVerbatim(va, out int prefixLen)) {
						b.Insert(j, va.Remove(prefixLen++));
						b.Append(va, prefixLen, va.Length - prefixLen);
						return;
					} else {
						va = va.Escape();
						b.Append(va);
					}
				}
				b.Append('\"');
			}

			_AppendProp(uiaidA, true);
			_AppendProp(idA, true);
			_AppendProp(classA);
			_AppendProp(valueA, true);
			_AppendProp(descriptionA, true);
			_AppendProp(actionA, true);
			_AppendProp(keyA, true);
			_AppendProp(helpA, true);
			if (_attr.Child is Grid g) foreach (var c in g.Children.OfType<KCheckBox>()) _AppendProp(c.Tag as KCheckTextBox, true);
			_AppendProp(elemA);
			_AppendProp(stateA);
			_AppendProp(rectA);
			_AppendProp(notinA);
			_AppendProp(maxccA);
			_AppendProp(levelA);
			if (nProp > 0) {
				if (nProp == 1) b.Remove(propStart, 4); //new(
				else b.Append(')');
			}

			b.AppendFlags((isName && nProp > 0) ? null : "flags", nameof(EFFlags),
				(hiddenTooA, nameof(EFFlags.HiddenToo)),
				(reverseA, nameof(EFFlags.Reverse)),
				(uiaA, nameof(EFFlags.UIA)),
				(notInprocA, nameof(EFFlags.NotInProc)),
				(clientAreaA, nameof(EFFlags.ClientArea)),
				(menuTooA, nameof(EFFlags.MenuToo))
				);

			if (alsoA.GetText(out var also)) b.AppendOtherArg(also, "also");
			if (skipA.GetText(out var skip)) b.AppendOtherArg(skip, "skip");
			if (navigA.GetText(out var navig)) b.AppendStringArg(navig, "navig");

			b.Append("].Find(");
			bool orThrow = !forTest && exceptionA.IsChecked;
			if (!forTest && waitA.GetText(out var waitTime, emptyToo: true)) b.AppendWaitTime(waitTime, orThrow); else if (orThrow) b.Append('0');
			b.Append(')');

			if (isCall) {
				if (!forTest) b.Append("\r\n\t");
				b.Append(orThrow ? null : "?").Append('.');
				var s = _cbAction.SelectedValue.ToString();
				int j = s.IndexOf('(');
				if (j < 0) b.Append(s).Append("()");
				else b.Append(s, 0, j).Append("(button: MButton.").Append(s.Ends("(*2)") ? "DoubleClick)" : "Right)");
			}
			b.Append(';');
			if (!(forTest | orThrow | isCall)) b.AppendLine().Append("if(a == null) { print.it(\"not found\"); }");

			var R = b.ToString();

			if (!forTest) _code.ZSetText(R, wndCode.Length);

			return (R, wndVar);
		}

		#region capture

		TUtil.CaptureWindowEtcWithHotkey _capt;

		void _cCapture_CheckedChanged() {
			_capt ??= new TUtil.CaptureWindowEtcWithHotkey(_cCapture, _Capture, () => _ElmFromMouse(out elm e, capturing: false) ? e.Rect : default);
			_capt.Capturing = _cCapture.IsChecked;
		}

		void _Capture() {
			_ttRecapture?.Close();
			_info.zText = c_dialogInfo; //clear error info

			if (!_ElmFromMouse(out elm e, capturing: true)) return;

			_elm = e;
			_SetElm(true);
			var w = this.Hwnd();
			if (w.IsMinimized) {
				w.ShowNotMinMax();
				w.ActivateL();
			}
		}

		bool _ElmFromMouse(out elm e, bool capturing) => _ElmFromPoint(mouse.xy, out e, capturing);

		bool _ElmFromPoint(POINT p, out elm e, bool capturing, bool ctor = false) {
			var flags = EXYFlags.PreferLink | EXYFlags.NoThrow;
			if (!ctor) {
				if (_uiaUserChecked && uiaA.IsChecked) flags |= EXYFlags.UIA;
				if (notInprocA.IsChecked) flags |= EXYFlags.NotInProc;
			}
			e = elm.fromXY(p, flags);
			if (ctor /*&& e != null && e.WndContainer.ClassName.Starts("Chrome_")*/) {
				//workaround for: Chrome may give wrong element at first. Eg youtube right list -> "x months ago".
				//	Possibly this can be useful with some other apps too.
				//	If not ctor, capturing works well because this func is called every ? ms to display element rectangles.
				//print.it(e);
				100.ms();
				e = elm.fromXY(p, flags);
				//print.it(e);
			}
			if (e == null && !flags.Has(EXYFlags.UIA)) e = elm.fromXY(p, flags | EXYFlags.UIA); //eg treeview in HtmlHelp 2

			if (capturing && !uacInfo.isAdmin && wnd.fromMouse().UacAccessDenied) print.warning("The target process is admin and this process isn't. May fail to get correct UI element or get its properties and call its functions.", -1); //not _info.InfoError, it's unreliable here, even with timer
			if (e == null) return false;

			if (capturing && !e.MiscFlags.HasAny(EMiscFlags.UIA | EMiscFlags.Java)) {
				//If e probably does not support Invoke or Focus, show menu with e and the first ancestor that supports it.
				//	In some cases use the ancestor without a menu (if LINK or BUTTON etc).
				//	This code is similar to the C++ code in _FromPoint_GetLink, but covers more cases.
				if (!_IsInteractive(e, true, out bool stop1) && !stop1) {
					bool found = false;
					string tt1 = "This element probably does not support Invoke or Focus",
						tt2 = "This parent element probably supports Invoke";
					elm ep = e.Parent, ep0 = ep;
					for (int i = 20; i-- > 0 && ep != null; ep = ep.Parent) {
						if ((found = _IsInteractive(ep, false, out bool stop2)) || stop2) break;
						//print.it(ep.ChildCount);
						i -= ep.ChildCount; //sometimes the subtree is <div><div><div>..., 1-2 children at every level
					}
					if (!found && ep != null) { //detect when e is not a child of its parent, eg Chrome tab button
												//var ee = ep0.Find(e.Role, prop: $"level=0\0maxcc=9\0rect={e.Rect}", flags: EFFlags.MenuToo); //TODO: test, remove
						var ee = ep0.Elm[e.Role, prop: $"level=0\0maxcc=9\0rect={e.Rect}", flags: EFFlags.MenuToo].Find();
						//print.it(ee);
						if (found = ee == null) {
							ep = ep0;
							tt1 = "Possibly disconnected from the tree";
							tt2 = "Parent element";
						}
						Debug_.PrintIf(found, "e is not a child of its parent: " + e);
					}
					if (found && ep.Name.NE() && !e.Name.NE()) found = false;
					if (found && ep.WndContainer.Is0) found = false; //see bug comment in C++
					if (found) {
						bool use = _IsLinkOrButton(ep.RoleInt)
							//|| role ERole.MENUITEM //will need EFFlags.MenuToo. But then cannot capture child elements in some menu items, and cannot select in tree because it closes the menu.
							;
						if (!use) use = _Menu(e, ep, tt1, tt2);
						if (use) e = ep;
					}
				} else if (!stop1) {
					//is in COMBOBOX?
					if (e.RoleInt is ERole.TEXT or ERole.BUTTON && e.Parent is elm ep) {
						var r1 = ep.RoleInt;
						if (r1 == ERole.COMBOBOX || (r1 == ERole.WINDOW && ((ep = ep.Parent)?.RoleInt ?? 0) == ERole.COMBOBOX)) {
							if (_Menu(e, ep, null, null)) e = ep;
						}
					}
				}

				static bool _IsInteractive(elm e, bool orig, out bool stop) {
					stop = false;
					if (e.SimpleElementId != 0) return true;
					var role = e.RoleInt;
					if (_IsLinkOrButton(role)) return true;
					if (stop = role is ERole.WINDOW or ERole.DOCUMENT or ERole.PROPERTYPAGE or ERole.PAGETABLIST
						or ERole.TABLE or ERole.LIST or ERole.TREE
						//or ERole.CLIENT or ERole.PANE //no, sometimes used for "no role" elements
						or ERole.DIALOG //action="OK"
						) return false;
					if (stop = role == ERole.PAGETAB) if (e.ChildCount > 2) return false; /*PAGETAB can be used either as button (eg in web browsers) or as button + page with all controls (eg WPF tab control)*/
					//if static text or image, even if has action, it may just throw "not implemented". Noticed in Firefox somewhere.
					if (role is ERole.STATICTEXT or ERole.IMAGE
						or ERole.DIAGRAM //IMAGE in Firefox
						or ERole.GROUPING //often has action, but rarely useful
						) return false;
					var state = e.State;
					if (state.Has(EState.FOCUSABLE)) return true; //eg editable TEXT. It could be eg in a WPF EXPANDER with action. Not in 'if (orig)' because eg TEXT may have children.
					if (orig) {
						if (role == ERole.TEXT) return state.Has(EState.DISABLED); //probably TEXT used instead of STATICTEXT, eg in Firefox
					} else if (state.Has(EState.INVISIBLE)) return false;
					return e.DefaultAction is not (null or "" or "click ancestor" /*Chrome*/ or "Collapse" /*WPF expander*/);
				}

				static bool _IsLinkOrButton(ERole role) => role is ERole.LINK
						or ERole.BUTTON or ERole.BUTTONMENU or ERole.BUTTONDROPDOWN or ERole.BUTTONDROPDOWNGRID
						or ERole.CHECKBOX or ERole.RADIOBUTTON;

				bool _Menu(elm e, elm ep, string tt1, string tt2) {
					var m = new popupMenu();
					string r1 = e.Role, r2 = ep.Role;
					if (r1.NE() || r2.NE()) return false;
					m.Add(1, "&" + r1).Tooltip = tt1;
					int i = r2.FindNot(r1[..1]); if (i >= 0) r2 = r2.Insert(i, "&");
					m.Add(2, r2).Tooltip = tt2;
					return 2 == m.Show(MSFlags.Underline | MSFlags.AlignRectBottomTop | MSFlags.AlignCenterH, excludeRect: e.Rect, owner: this);
				}
			}
			return true;
		}
		bool _uiaUserChecked; //to prevent capturing with EXYFlags.UIA when the checkbox was checked automatically (not by the user)
		bool _waitAutoCheckedOnce; //if user unchecks, don't check next time

		#endregion

		#region util

		wnd _WndSearchIn => _useCon ? _con : _wnd;

		//Returns true if e is in visible web page in one of 3 browsers, and UIA unchecked.
		//browser - receives nonzero if container's class is like in one of browsers: 1 IES, 2 FF, 3 Chrome. Even if returns false.
		bool _IsVisibleWebPage(elm e, out _BrowserEnum browser, wnd wContainer = default) {
			if (this.uiaA.True()) { browser = 0; return false; }
			if (wContainer.Is0) wContainer = e.WndContainer;
			browser = (_BrowserEnum)wContainer.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
			if (browser == 0) return false;
			if (browser == _BrowserEnum.IE) return true;
			elm eDoc = null;
			do {
				if (e.RoleInt == ERole.DOCUMENT) eDoc = e;
				e = e.Parent;
			} while (e != null);
			if (eDoc == null || eDoc.IsInvisible) return false;
			return true;
		}
		enum _BrowserEnum { IE = 1, FF, Chrome }

		#endregion

		#region OK, Test

		/// <summary>
		/// When OK clicked, contains C# code. Else null.
		/// </summary>
		public string ZResultCode { get; private set; }

		private void _bOK_Click(WBButtonClickArgs e) {
			if (!_Insert(true)) { e.Cancel = true; return; }
		}

		bool _Insert(bool ok) {
			var code = _code.zText;
			if (code == "") code = null;
			if (ok) ZResultCode = code;
			if (code == null) return false;
			InsertCode.Statements(code);
			return true;
		}

		private void _Test(bool captured = false, bool testAction = false, bool testActionNoActivate = false) {
			_info.zText = c_dialogInfo; //clear error info

			if (testAction) testAction = _cbAction.SelectedIndex > 0;
			bool testActWin = false;
			if (testAction && !testActionNoActivate) {
				switch (popupMenu.showSimple("1 Test action|2 Activate window and test action||0 Cancel", owner: this)) {
				case 1: break;
				case 2: testActWin = true; break;
				default: return;
				}
			}

			var (code, wndVar) = _FormatCode(true, testAction); if (code == null) return;
			var r = TUtil.RunTestFindObject(code, wndVar, _WndSearchIn, _bTest, _speed, _info, o => (o as elm).Rect, testActWin);
			if (r.obj is not elm e) return;
			//if (quickOK && r.speed < 1_000_000 && e.Rect == _elm.Rect) {
			//	//timerm.after(1000, _ => _bOK.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)));
			//	return;
			//}
			//TODO: auto test action: don't call if finds at other location.

			if (r.speed >= 20_000 && !notInprocA.IsChecked && !uiaA.IsChecked) {
				if (!e.MiscFlags.Has(EMiscFlags.InProc) && _wnd.ClassNameIs("Mozilla*")) {
					//need full path. Run("firefox.exe") fails if firefox is not properly installed.
					string ffInfo = c_infoFirefox, ffPath = _wnd.ProgramPath;
					if (ffPath != null) ffInfo = ffInfo.Replace("firefox.exe", ffPath);
					_info.zText = ffInfo;
				}
			}

			if (captured && App.Settings.tools_Delm_flags.Has(EOptions.QuickInsert)) {
				_quickMenu?.Close();
				var rect = e.Rect;
				timerm.after(100, _ => {
					_quickMenu = new();
					_quickMenu["&Insert"] = _ => _Insert(false);
					_quickMenu["&OK"] = _ => _bOK.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
					_quickMenu.Separator();
					_quickMenu["Test &action"] = _ => _Test(true, true, true); //captured=true, to show this menu again, because then the user probably wants to insert
																			   //_quickMenu.Add("Cancel"); //the menu will be closed on click anywhere or on next capturing hotkey
					_quickMenu.Show(MSFlags.Underline | MSFlags.AlignRectBottomTop | MSFlags.AlignCenterH, excludeRect: rect, owner: this);
					_quickMenu = null;
				});
			}
		}

		popupMenu _quickMenu;

		#endregion

		#region tree

		_TreeItem _treeRoot;
		//_TreeItem _nodeCaptured;
		bool _isWebIE; //_FillProperties sets it; then _FillTree uses it.

		void _InitTree() {
			_tree.SingleClickActivate = true;
			_tree.ItemActivated += (_, e) => {
				_elm = (e.Item as _TreeItem).e;
				_SetWndCon(_wnd, _elm.WndContainer, _useCon);
				if (!_FillProperties(out var p)) return;
				_FormatCode();
				TUtil.ShowOsdRect(p.Rect);
			};

			_tree.ItemClick += (_, e) => {
				if (e.MouseButton == MouseButton.Right) {
					var m = new popupMenu();
					m["Navigate to this element from the selected"] = _ => _NavigPath(e.Item as _TreeItem);
					m.Show(owner: this);
				}
			};
		}

		void _ClearTree() {
			_tree.SetItems(null);
			_treeRoot = null;
		}

		(_TreeItem xRoot, _TreeItem xSelect) _CreateTreeModel(wnd w, in EProperties p, bool skipWINDOW) {
			_TreeItem xRoot = new(this), xSelect = null;
			var stack = new Stack<_TreeItem>(); stack.Push(xRoot);
			int level = 0;

			EFFlags flags = Enum_.EFFlags_Mark | EFFlags.HiddenToo | EFFlags.MenuToo;
			if (uiaA.IsChecked) flags |= EFFlags.UIA;
			var us = (uint)p.State;
			var prop = $"rect={p.Rect}\0state=0x{us:X},!0x{~us:X}";
			if (skipWINDOW) prop += $"\0 notin=WINDOW";
			//print.it(prop.Replace('\0', ';'));
			var role = p.Role; if (role.Length == 0) role = null;
			try {
				w.Elm[role, "**tc " + p.Name, prop, flags, also: o => {
					//var x = new _ElmNode(o.Role);
					_TreeItem x = new(this);
					int lev = o.Level;
					if (lev != level) {
						if (lev > level) {
							Debug.Assert(lev - level == 1);
							stack.Push(stack.Peek().LastChild);
						} else {
							while (level-- > lev) stack.Pop();
						}
						level = lev;
					}
					x.e = o;
					if (o.MiscFlags.Has(Enum_.EMiscFlags_Marked)) {
						//print.it(o);
						if (xSelect == null) xSelect = x;
					}
					stack.Peek().AddChild(x);
					return false;
				}
				].Exists();
			}
			catch (Exception ex) {
				_info.InfoError("Failed to get UI element tree.", ex.Message);
				return default;
			}
			return (xRoot, xSelect);
		}

		void _FillTree(in EProperties p) {
			_treeRoot = null;

			var w = _WndSearchIn;
			if (_isWebIE && !_useCon && !_con.Is0) w = _con; //if IE, don't display whole tree. Could be very slow, because cannot use in-proc for web pages (and there may be many tabs with large pages), because its control is in other thread.
			var (xRoot, xSelect) = _CreateTreeModel(w, in p, false);
			if (xRoot == null) return;

			if (xSelect == null && w.IsAlive) {
				//IAccessible of some controls are not connected to the parent.
				//	Noticed this in Office 2003 Word Options dialog and in Dreamweaver.
				//	Also, WndContainer then may get the top-level window. Eg in Word.
				//	Workaround: enum child controls and look for _elm in one them. Then add "class" row if need.
				Debug_.Print("broken IAccessible branch");
				foreach (var c in w.Get.Children(onlyVisible: true)) {
					var m = _CreateTreeModel(c, in p, true);
					if (m.xSelect != null) {
						//m.xRoot.a = elm.fromWindow(c, flags: EWFlags.NoThrow);
						//if(m.xRoot.a != null) model.xRoot.Add(m.xRoot);
						//else model.xRoot = m.xRoot;
						(xRoot, xSelect) = m;
						if (!classA.Visible) {
							classA.t.Text = TUtil.StripWndClassName(c.ClassName, true);
							classA.c.IsChecked = true;
							classA.Visible = true;
						}
						break;
					}
				}
			}

			_tree.SetItems(xRoot.Children());
			_treeRoot = xRoot;
			if (xSelect != null) _SelectTreeItem(xSelect);
		}

		void _SelectTreeItem(_TreeItem x) {
			_tree.EnsureVisible(x);
			_tree.SelectSingle(x, true);
		}

		//Tries to find and select _elm in current tree when captured from same window.
		//	Eg maybe the user captured because wants to use navigate; then wants to see both elements.
		//	Usually faster than recreating tree, but in some cases can be slower. Slower when fails to find.
		bool _TrySelectInSameTree() {
			if (_treeRoot == null) return false;
			//if(keys.isScrollLock) return false;
			int elem = _elm.SimpleElementId;
			var ri = _elm.RoleInt;
			if (!_elm.GetProperties(ri == 0 ? "Rrn" : "rn", out var p)) return false;
			string rs = ri == 0 ? p.Role : null;
			//print.it(elem, ri, rs, p.Rect, _elm);
			foreach (var v in _treeRoot.Descendants()) {
				var e = v.e;
				if (e.SimpleElementId != elem) continue;
				if (e.RoleInt != ri) continue;
				if (rs != null && e.Role != rs) continue;
				if (e.Rect != p.Rect) continue;
				if (e.Name != p.Name) continue;
				_SelectTreeItem(v);
				return true;
			}
			Debug_.Print("recreating tree of same window");
			return false;

			//Other ways to compare elm:
			//IAccIdentity. Unavailable in web pages.
			//IUIAutomationElement. Very slow ElementFromIAccessible. In Firefox can be 30 ms.
		}

		class _TreeItem : TreeBase<_TreeItem>, ITreeViewItem
		{
			readonly Delm _dlg;
			public elm e;
			string _displayText;
			bool _isExpanded;
			bool _isFailed;
			bool _isInvisible;

			public _TreeItem(Delm dlg) {
				_dlg = dlg;
			}

			#region ITreeViewItem

			string ITreeViewItem.DisplayText {
				get {
					if (_displayText == null) {
						bool isWINDOW = e.RoleInt == ERole.WINDOW;
						string props = isWINDOW ? "Rnsw" : "Rns";
						if (!e.GetProperties(props, out var p)) {
							_isFailed = true;
							return _displayText = "Failed: " + lastError.message;
						}

						if (isWINDOW) {
							using (new StringBuilder_(out var b)) {
								b.Append(p.Role).Append("  (").Append(p.WndContainer.ClassName).Append(")");
								if (p.Name.Length > 0) b.Append("  \"").Append(p.Name).Append('\"');
								_displayText = b.ToString();
							}
						} else if (p.Name.Length == 0) _displayText = p.Role;
						else _displayText = p.Role + " \"" + p.Name.Escape(limit: 250) + "\"";

						_isInvisible = e.IsInvisible_(p.State);
					}
					return _displayText;
				}
			}

			void ITreeViewItem.SetIsExpanded(bool yes) { _isExpanded = yes; }

			bool ITreeViewItem.IsExpanded => _isExpanded;

			IEnumerable<ITreeViewItem> ITreeViewItem.Items => base.Children();

			bool ITreeViewItem.IsFolder => _IsFolder;
			bool _IsFolder => base.HasChildren;

			string ITreeViewItem.ImageSource => _isExpanded ? @"resources/images/expanddown_16x.xaml" : (_IsFolder ? @"resources/images/expandright_16x.xaml" : null);

			int ITreeViewItem.TextColor => _isFailed ? 0xff : (_isInvisible ? Api.GetSysColor(Api.COLOR_GRAYTEXT) : Api.GetSysColor(Api.COLOR_WINDOWTEXT));

			int ITreeViewItem.BorderColor {
				get {
					//if (this == _dlg._nodeCaptured) return 0x80C0FF;

					return -1;
				}
			}

			#endregion
		}

		#endregion

		#region misc

		void _Options() {
			var m = new popupMenu();
			//m.Add("Tool settings", disable: true).FontBold = true;
			var cRA = m.AddCheck("Remember action", App.Settings.tools_Delm_flags.Has(EOptions.ActionRemember));
			var cAT = m.AddCheck("Auto test", App.Settings.tools_Delm_flags.Has(EOptions.AutoTest));
			var cATA = m.AddCheck("Auto test action", App.Settings.tools_Delm_flags.Has(EOptions.AutoTestAction));
			var cQI = m.AddCheck("Quick insert", App.Settings.tools_Delm_flags.Has(EOptions.QuickInsert));
			//var cQO = m.AddCheck("Quick OK", App.Settings.tools_Delm_flags.Has(EOptions.QuickOK));
			m.Show(owner: this);
			App.Settings.tools_Delm_flags.SetFlag(EOptions.ActionRemember, cRA.IsChecked);
			App.Settings.tools_Delm_flags.SetFlag(EOptions.AutoTest, cAT.IsChecked);
			App.Settings.tools_Delm_flags.SetFlag(EOptions.AutoTestAction, cATA.IsChecked);
			App.Settings.tools_Delm_flags.SetFlag(EOptions.QuickInsert, cQI.IsChecked);
			//App.Settings.tools_Delm_flags.SetFlag(EOptions.QuickOK, cQO.IsChecked);
		}

		[Flags]
		public enum EOptions
		{
			ActionMask = 15,
			ActionRemember = 16,
			AutoTest = 32,
			AutoTestAction = 64,
			QuickInsert = 128,
			//QuickOK = 256,
		}

		void _InitActionCombo() {
			if (App.Settings.tools_Delm_flags.Has(EOptions.ActionRemember))
				_cbAction.SelectedIndex = Math.Clamp((int)(App.Settings.tools_Delm_flags & EOptions.ActionMask), 0, _cbAction.Items.Count - 1);
			_cbAction.SelectionChanged += (o, _) => {
				_AnyCheckTextBoxValueChanged(o);
				App.Settings.tools_Delm_flags = (App.Settings.tools_Delm_flags & ~EOptions.ActionMask) | ((EOptions)_cbAction.SelectedIndex & EOptions.ActionMask);
			};
		}

		//Builds navig path from the selected tree node to *to*. Sets control text and checkbox.
		void _NavigPath(_TreeItem to) {
			if (_tree.SelectedItem is not _TreeItem from) return;
			//print.it(from.e); print.it(to.e);
			var a = new List<(string s, int n)>();
			if (from.Parent == to.Parent) {
				_AppendNePr(from, to);
			} else {
				var a1 = from.AncestorsFromRoot(andSelf: true);
				int i = Array.IndexOf(a1, to);
				if (i >= 0) { //to is ancestor of from
					_AppendPa();
				} else {
					//find common ancestor
					var a2 = to.AncestorsFromRoot(andSelf: true);
					for (i = Math.Min(a1.Length, a2.Length); --i >= 0;) if (a1[i] == a2[i]) break;

					if (++i < a1.Length) {
						_AppendPa();
						_AppendNePr(a1[i], a2[i]);
					} else i--;

					while (++i < a2.Length) {
						var v = a2[i];
						var p = a2[i].Parent;
						if (v == p.FirstChild) {
							if (a.Count > 0 && a[^1].s == "fi") a[^1] = ("fi", a[^1].n + 1);
							else a.Add(("fi", 1));
						} else if (v == p.LastChild) {
							if (a.Count > 0 && a[^1].s == "la") a[^1] = ("la", a[^1].n + 1);
							else a.Add(("la", 1));
						} else {
							a.Add(("ch", v.Index + 1));
						}
					}
				}

				void _AppendPa() => a.Add(("pa", a1.Length - i - 1));
			}

			void _AppendNePr(_TreeItem from, _TreeItem to) {
				if (to == from) return;
				int n = to.Index - from.Index;
				a.Add((n > 0 ? "ne" : "pr", Math.Abs(n)));
			}

			var b = new StringBuilder();
			foreach (var (s, n) in a) {
				if (b.Length > 0) b.Append(' ');
				b.Append(s);
				if (n > 1) b.Append(n);
			}
			var navig = b.ToString();

			navigA.Set(navig.Length > 0, navig);
		}

		public static class Java
		{
			/// <summary>
			/// Calls <see cref="EnableDisableJab"/>(null) and shows results in task dialog.
			/// </summary>
			/// <param name="owner"></param>
			public static void EnableDisableJabUI(AnyWnd owner) {
				var (ok, results) = EnableDisableJab(null);
				if (results != null) dialog.show("Results", results, icon: ok ? DIcon.Info : DIcon.Error, owner: owner, flags: DFlags.CenterOwner);
			}

			/// <summary>
			/// Enables or disables Java Access Bridge for current user.
			/// Returns: ok = false if failed or cancelled. results = null if cancelled.
			/// </summary>
			/// <param name="enable">If null, shows enable/disable dialog.</param>
			public static (bool ok, string results) EnableDisableJab(bool? enable/*, bool allUsers*/) {
				if (enable == null) {
					switch (dialog.showList("1 Enable|2 Disable|Cancel", "Java Access Bridge")) {
					case 1: enable = true; break;
					case 2: enable = false; break;
					default: return (false, null);
					}
				}
				bool en = enable.GetValueOrDefault();

				if (!GetJavaPath(out var dir)) return (false, "Cannot find Java " + (osVersion.is32BitProcess ? "32" : "64") + "-bit. Make sure it is installed.");

				//if(!allUsers) {
				string jabswitch = dir + @"\bin\jabswitch.exe", sout = null;
				if (!filesystem.exists(jabswitch).isFile) return (false, "Cannot find jabswitch.exe.");
				try {
					run.console(out sout, jabswitch, en ? "-enable" : "-disable");
					sout = sout?.Trim();
				}
				catch (Exception ex) {
					return (false, ex.ToStringWithoutStack());
				}
				//} else {
				//never mind
				//}

				sout += "\r\nRestart Java apps to apply the new settings.";

				string dll64 = folders.SystemX64 + "WindowsAccessBridge-64.dll", dll32 = folders.SystemX86 + "WindowsAccessBridge-32.dll";
				if (!filesystem.exists(dll64).isFile) sout += "\r\n\r\nWarning: dll not found: " + dll64 + ".  64-bit apps will not be able to use UI elements of Java apps. Install 64-bit Java too.";
				if (!filesystem.exists(dll32).isFile) sout += "\r\n\r\nNote: dll not found: " + dll32 + ".  32-bit apps will not be able to use UI elements of Java apps. Install 32-bit Java too.";

				return (true, sout);

				//tested: the checkbox in CP does not disable JAB. Works only enabling.
				//	Tested on Win 10 (installed Java 64 and 32) and 7 (installed Java 64).
				//	Tested 64-bit and 32-bit processes.
				//	\lib\accessibility.properties is not modified, ie not enabled for all users.
				//	This function works.
			}

			/// <summary>
			/// Gets Java folder path of Java of same 32/64 bitness as this process.
			/// </summary>
			public static bool GetJavaPath(out string path) {
				path = null;
				string rk = @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Runtime Environment";
				if (Microsoft.Win32.Registry.GetValue(rk, "CurrentVersion", null) is not string ver) return false;
				path = Microsoft.Win32.Registry.GetValue(rk + @"\" + ver, "JavaHome", null) as string;
				return path != null;
			}
		}

		#endregion

		#region info

		TUtil.CommonInfos _commonInfos;
		void _InitInfo() {
			_commonInfos = new TUtil.CommonInfos(_info);

			_info.zText = c_dialogInfo;
			_info.AddElem(this, c_dialogInfo);
			_info.ZTags.AddLinkTag("+jab", _ => Java.EnableDisableJabUI(this));

			_info.InfoCT(roleA,
@"Role. Prefix <b>web:<> means 'in web page'.
Read more in <help>elmFinder[]<> help.");
			_info.InfoCT(nameA, "Name.", true);
			_info.InfoCT(uiaidA, "UIA AutomationId.", true);
			_info.InfoCT(idA, "Control id. Will search only in controls that have it.");
			_info.InfoCT(classA, "Control class name. Will search only in controls that have it.", true);
			_info.InfoCT(valueA, "Value.", true);
			_info.InfoCT(descriptionA, "Description.", true);
			_info.InfoCT(actionA, "Default action.", true);
			_info.InfoCT(keyA, "Keyboard shortcut.", true);
			_info.InfoCT(helpA, "Help.", true);
			_info.InfoCT(elemA,
@"Simple element id.");
			_info.InfoCT(stateA,
@"State. List of <help Au.Types.EState>states<> this UI element must have and/or not have.
Example: CHECKED, !DISABLED
Note: state can change. Use only states you need. Remove others from the list.");
			_info.InfoCT(rectA,
@"Rectangle. Can be specified width (W) and/or height (H).
Example: {W=100 H=20}");

			_info.InfoC(controlC,
@"Find first matching control and search in it, not in all matching controls.
To change window or/and control name etc, click 'Window/control...' or edit it in the code field.");
			_info.InfoCT(alsoA,
@"<help>elmFinder[]<> <i>also<> " + TUtil.CommonInfos.c_alsoParameter);
			_info.InfoCT(skipA,
@"0-based index of matching UI element.
For example, if 1, gets the second matching UI element.
-1 means any matching intermediate element.");
			_info.InfoCT(navigA,
@"Get another UI element using this path from the found UI element. See <help>elm.Navigate<>.
One or several words: <u><i>parent<> <i>child<> <i>first<> <i>last<> <i>next<> <i>previous<><>. Or 2 letters, like <i>ne<>.
Example: pa ne2 ch3. The 2 means 2 times (ne ne). The 3 means 3-rd child; -3 would be 3-rd from end.
Tool: in the tree view right click that element...");
			_info.InfoCT(waitA,
@"The wait timeout, seconds.
The function waits max this time interval. On timeout throws exception if 'Exception...' checked, else returns null. If empty, uses 8e88 (infinite).");
			_info.InfoCT(notinA,
@"Don't search in UI elements that have these roles. Can make faster.
Example: LIST,TREE,TITLEBAR,SCROLLBAR");
			_info.InfoCT(maxccA, "Don't search in UI elements that have more direct children. Default 10000.");
			_info.InfoCT(levelA,
@"0-based level of the UI element in the tree of UI elements. Or min and max levels. Default 0 1000.
Relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc).");

			_info.InfoC(exceptionA,
@"Throw exception if not found.
If unchecked, returns null.");
			_info.Info(_cbAction, "Action",
@"Set an elm variable, or call a single function without a variable.
With the variable later you can add code to call one or more functions. Examples e.Invoke(); var s = e.Name.");
			_info.Info(_bSett, "...",
@"Saved settings of this tool dialog.
<i>Remember action<> - when opening, select the last used action.
<i>Auto test<> - automatically click Test when captured.
<i>Auto test action<> - automatically click TA when captured.
<i>Quick insert<> - when captured, test and show popup menu.");

			_info.InfoC(hiddenTooA, "Flag <help>Au.Types.EFFlags<>.HiddenToo.");
			_info.InfoC(reverseA, "Flag <help>Au.Types.EFFlags<>.Reverse (search bottom to top).");
			_info.InfoC(uiaA,
@"Flag <help>Au.Types.EFFlags<>.UIA (use UI Automation API instead of IAccessible).
The capturing tool checks/unchecks this automatically when need.");
			_info.InfoC(notInprocA, "Flag <help>Au.Types.EFFlags<>.NotInProc.");
			_info.InfoC(clientAreaA, "Flag <help>Au.Types.EFFlags<>.ClientArea.");
			_info.InfoC(menuTooA,
@"Flag <help>Au.Types.EFFlags<>.MenuToo.
Check this if the UI element is in a menu and its role is not MENUITEM or MENUPOPUP.");

			_info.Info(_tree, "Tree view",
@"All UI elements in the window.");

			//SHOULDDO: now no info for HwndHost
			//			_Info(_code, "Code",
			//@"Created code to find the UI element.
			//Some parts can be edited directly.");
		}

		const string c_dialogInfo =
@"This dialog creates code to find <help elm>UI element<> in <help wnd.find>window<> (to click etc).
1. Move the mouse to a UI element. Press key <b>F3<> or <b>Ctrl+F3<>.
2. Click the Test button. It finds and shows the UI element.
3. If need, change some fields or select another element.
4. Click OK, it inserts C# code in editor. Or copy/paste.
5. In editor add code to use the UI element. <help elm>Examples<>. If need, rename variables, delete duplicate wnd.find lines, replace part of window name with *, etc.

How to find UI elements that don't have a name or other property with unique constant value? Capture another UI element near it, and use <b>navig<> to get it. Or try <b>skip<>.

If F3 does not work when the target window is active, probably its process is admin and this process isn't. Ctrl+F3 works, but cannot get UI element properties.";
		const string c_infoFirefox = @"To make much faster in Firefox, disable its multiprocess feature. More info in <help>elm<> help. Or use Chrome instead.";
		const string c_infoJava = @"If there are no UI elements in this window, need to <+jab>enable<> Java Access Bridge etc. More info in <help>elm<> help.";

		#endregion
	}
}
