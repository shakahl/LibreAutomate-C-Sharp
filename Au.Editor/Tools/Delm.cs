﻿using System.Windows;
using System.Windows.Controls;
using Au.Controls;
using System.Windows.Input;
using System.Linq;

//SHOULDDO: if checked 'state', activate window before test. Else different FOCUSED etc.

//CONSIDER: here and in Dwnd: UI for "contains image".
//	Also then need to optimize "elm containing image". Now with image finder in 'also' gets pixels multiple times.

//PROBLEM: hangs when trying to get a property while the target window does not respond. Only MSAA, not UIA.
//	Eg when the Windows search UI hidden after capturing. Then its process is suspended?

//note: don't use access keys (_ in control names). It presses the button without Alt, eg when the user forgets to activate editor and starts typing.

//TEST: CoCancelCall/CoTestCancel.

//SHOULDDO: auto detect when should use Invoke and when WebInvoke, and suggest to switch.
//	If now Invoke, and role web:, and after executing action soon changed window title, suggest WebInvoke.
//	If now WebInvoke, and role not web:, and after executing action does not change window title for eg 5 s, suggest to stop waiting and use Invoke.
//	Or just add pseudo action Auto. If "web:", use WebInvoke, else if has action, use Invoke, else MouseClick.

namespace Au.Tools;

class Delm : KDialogWindow
{
	public static void Dialog(POINT? p = null)
		=> TUtil.ShowDialogInNonmainThread(() => new Delm(p));

	elm _elm;
	wnd _wnd, _con;
	bool _useCon;
	bool _wndNoActivate;
	string _wndName;
	string _screenshot;

	KSciInfoBox _info;
	Button _bTest, _bInsert, _bSettings, _bWindow;
	ComboBox _cbAction;
	KCheckBox _cCapture, _cAutoTestAction, _cAutoInsert, _cControl, _cException, _cScroll;
	KCheckTextBox _wait, _xy;
	Panel _topRow3;
	_PropPage _page, _commonPage;
	Border _pageBorder;
	KSciCodeBoxWnd _code;
	KTreeView _tree;

	partial class _PropPage
	{
		Delm _dlg;

		public KCheckTextBox roleA, nameA, uiaidA, idA, classA, valueA, descriptionA, actionA, keyA, helpA, elemA, stateA, rectA, alsoA, skipA, navigA, notinA, maxccA, levelA;
		public KCheckBox inPath, hiddenTooA, reverseA, uiaA, notInprocA, clientAreaA, menuTooA;
		public Border htmlAttr;
		public Grid panel;
		public _TreeItem ti;

		//elm properties, other parameters, search settings
		public _PropPage(Delm dlg) {
			//print.it("_PropPage");
			_dlg = dlg;

			var b = new wpfBuilder().Height(180).Columns(-2, 0, -1);
			panel = b.Panel as Grid;
			//elm properties (left side)
			b.R.xStartPropertyGrid("L2 TRB").Height = panel.Height;
			roleA = b.xAddCheckText("role");
			nameA = b.xAddCheckText("name");
			uiaidA = b.xAddCheckText("uiaid"); //note: these must be == prop property names
			idA = b.xAddCheckText("id");
			classA = b.xAddCheckText("class");
			valueA = b.xAddCheckText("value");
			descriptionA = b.xAddCheckText("desc");
			actionA = b.xAddCheckText("action");
			keyA = b.xAddCheckText("key");
			helpA = b.xAddCheckText("help");
			b.R.Add(out htmlAttr); //HTML attributes will be added with another builder
			elemA = b.xAddCheckText("item");
			stateA = b.xAddCheckText("state");
			rectA = b.xAddCheckText("rect");
			b.xEndPropertyGrid();
			b.SpanRows(3);
			b.xAddSplitterV(span: 4, thickness: 12);
			//right side
			b.xStartPropertyGrid("R2 LTB").Height = 140;
			//other parameters
			alsoA = b.xAddCheckText("also", "o=>true");
			skipA = b.xAddCheckText("skip");
			navigA = b.xAddCheckText("navig");
			//search settings
			b.xAddCheck(out hiddenTooA, "Find hidden too");
			b.xAddCheck(out reverseA, "Reverse order");
			b.xAddCheck(out uiaA, "UI Automation");
			b.xAddCheck(out notInprocA, "Not in-process");
			b.xAddCheck(out clientAreaA, "Only client area");
			b.xAddCheck(out menuTooA, "Can be in menu");
			notinA = b.xAddCheckText("notin"); //note: these must be == prop property names
			maxccA = b.xAddCheckText("maxcc");
			levelA = b.xAddCheckText("level");
			b.xEndPropertyGrid();
			b.R.Skip(2).AddSeparator(vertical: false).Margin("T9 B9");
			b.Row(-1).Skip(2).Add(out inPath, "Add to path").Margin("1");
			b.End();

			_InitInfo();
		}
	}

	public Delm(POINT? p = null) {
		Title = "Find UI element";

		var b = new wpfBuilder(this).WinSize((500, 440..), (600, 500..)).Columns(-1, 0);
		b.R.Add(out _info).Height(60);
		b.R.StartGrid().Columns(76, 76, 130, 0, 70, -1);
		//row 1
		b.R.StartStack();
		_cCapture = b.xAddCheckIcon("*Unicons.Capture #FF4040", $"Enable capturing (hotkey {App.Settings.delm.hk_capture}, also {App.Settings.delm.hk_insert} to insert) and show UI element rectangles");
		_cAutoTestAction = b.xAddCheckIcon("*Material.CursorDefaultClickOutline #FF4040", "Auto test action when captured.\r\nIf no action selected, will show menu.");
		_cAutoInsert = b.xAddCheckIcon("*VaadinIcons.Insert #9F5300", "Auto insert code when captured");
		b.AddSeparator(true);
		b.xAddButtonIcon("*Ionicons.UndoiOS #9F5300", _ => App.Dispatcher.InvokeAsync(() => Menus.Edit.Undo()), "Undo in editor");
		b.xAddButtonIcon("*Material.SquareEditOutline #008EEE", _ => App.Hmain.ActivateL(true), "Activate editor window");
		b.xAddButtonIcon("*FontAwesome.WindowMaximizeRegular #008EEE", _ => _wnd.ActivateL(true), "Activate captured window");
		b.AddSeparator(true);
		b.xAddButtonIcon("*Material.CursorDefaultClickOutline #000000", _ => _Test(testAction: true), "Test action");
		b.xAddButtonIcon("*Material.CursorDefaultClickOutline #008EEE", _ => _Test(testAction: true, actWin: true), "Activate window and test action");
		b.AddSeparator(true);
		_bSettings = b.xAddButtonIcon("*EvaIcons.Options2 #99BF00", _ => _ToolSettings(), "Tool settings");
		b.End();
		//row 2
		b.R.AddButton(out _bTest, "Test", _ => _Test()).Tooltip("Executes the 'find' part of the code now and shows the rectangle.\r\nRight-click for more options.");
		_bTest.ContextMenuOpening += _bTest_ContextMenuOpening;
		b.AddButton(out _bInsert, "Insert", _ => _Insert(hotkey: false)).Tooltip($"Insert code in editor.\nHotkey {App.Settings.delm.hk_insert} (while capturing).");
		b.Add(out _cbAction).Tooltip("Action. Call this function when found.");
		_xy = b.xAddCheckText("x, y", noR: true, check: _Opt.Has(_EOptions.MouseXY)); b.Span(1).Height(18); _xy.t.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
		b.Add(out _cScroll, "Scroll").Checked(_Opt.Has(_EOptions.MouseScroll));
		//row 3
		b.R.StartStack(); _topRow3 = b.Panel;
		b.AddButton(out _bWindow, "Window...", _bWnd_Click).Width(70);
		b.xAddCheck(out _cControl, "Control").Margin("R30");
		_wait = b.xAddCheckText("Wait", App.Settings.delm.wait ?? "1", check: !_Opt.Has(_EOptions.NoWait)); b.Width(48);
		b.xAddCheck(out _cException, "Fail if not found").Checked();
		b.End();

		b.End();
		_ActionInit();
		_EnableDisableTopControls(false);

		b.R.AddSeparator(false);
		_commonPage = new(this);
		b.R.Add(out _pageBorder).Margin("LR").Height(_commonPage.panel.Height);
		b.R.AddSeparator(false);

		//code
		b.Row(80).xAddInBorder(out _code, "B");

		//tree
		b.xAddSplitterH(span: -1);
		b.Row(-1).StartGrid().Columns(-1, 0, -1);
		b.Row(-1).xAddInBorder(out _tree, "T");
		b.End();

		b.End();

		_InitTree();

		if (p != null) _elm = _ElmFromPoint(p.Value, ctor: true); //will call _SetElm in OnSourceInitialized

		b.WinProperties(
			topmost: true,
			showActivated: _elm != null ? false : null //eg if captured a popup menu item, activating this window closes the menu and we cannot get properties
			);

		WndSavedRect.Restore(this, App.Settings.delm.wndPos, o => App.Settings.delm.wndPos = o);
	}

	static Delm() {
		TUtil.OnAnyCheckTextBoxValueChanged<Delm>((d, o) => d._AnyCheckTextBoxValueChanged(o), comboToo: true);
	}

	protected override void OnSourceInitialized(EventArgs e) {
		base.OnSourceInitialized(e);

		if (_elm != null) _SetElm(false);
		_InitInfo();
		_InitCapturingWithHotkey();
	}

	protected override void OnClosing(CancelEventArgs e) {
		_cCapture.IsChecked = false;
		base.OnClosing(e);
	}

	void _SetElm(bool captured) {
		wnd c = _GetWndContainer(), w = c.Window;
		if (w.Is0) return;
		//if (captured && w.IsCloaked) {
		//	//workaround for old pre-Chromium Edge. w is a cloaked windowsuicorecorewindow of other process. There are many such cloaked windows, and wnd.find often finds wrong window.
		//	c = wnd.fromMouse();
		//	w = c.Window;
		//	if (w.Is0) return;
		//}

		string wndName = w.NameTL_;
		bool sameWnd = captured && w == _wnd && wndName == _wndName;
		_wndName = wndName;

		bool useCon = _useCon && captured && sameWnd && c == _con;
		//search in control by default in these cases:
		//	1. It is in other thread. Then would be slow because cannot use inproc. Except in known windows.
		//	2. If Java. Can find in any case if control class name specified, but now cannot get tree of entire window.
		if (!useCon && c != w) {
			if (c.ThreadId != w.ThreadId) useCon = 0 == c.ClassNameIs(Api.string_IES, "Windows.UI.Core.CoreWindow");
			else useCon = w.ClassNameIs("SunAwt*") && c.ClassNameIs("SunAwt*");
		}

		_SetWndCon(w, c, useCon);

		if (!_FillPropertiesTreeAndCode(true, sameWnd)) return;

		if (_IsAutoTest) timer.after(1, _ => _Test(captured: true, testAction: _cAutoTestAction.IsChecked));
	}

	void _SetWndCon(wnd w, wnd con, bool useCon = false) {
		_wnd = w;
		_con = con == w ? default : con;
		_useCon = useCon && !_con.Is0;
		_wndNoActivate = w.IsNoActivateStyle_();
		using var nevc = new _NoeventValueChanged(this);
		_cControl.IsChecked = _useCon; _cControl.IsEnabled = !_con.Is0;
	}

	//Called when: 1. _SetElm. 2. The Window button changed window. 3. The Control checkbox checked/unchecked.
	bool _FillPropertiesTreeAndCode(bool setElm = false, bool sameWnd = false) {
		//_nodeCaptured = null;

		//perf.first();
		_TreeItem ti = null;
		bool sameTree = sameWnd && _TrySelectInSameTree(out ti);
		//perf.next();

		if (!sameTree) _ClearTree();
		else if (ti != null && _PathSetPageWhenTreeItemSelected(ti)) return true;

		if (!_FillProperties(out var p)) return false;
		if (!sameTree) {
			Mouse.SetCursor(Cursors.Wait);
			_FillTree(p);
			Mouse.SetCursor(Cursors.Arrow);
		}
		_FormatCode();

		if (p.Role == "CLIENT" && _wnd.ClassNameIs("SunAwt*") && !_elm.MiscFlags.Has(EMiscFlags.Java) && !osVersion.is32BitOS) {
			timer.after(50, _ => {
				if (_info.ZElemsSuspended) { //eg showing test result
					string s1 = c_infoJava, s2 = _info.zText; if (!s2.NE() && !s2.Ends('\n')) s1 = "\r\n" + s1;
					_info.zAppendText(s1, false, false);
				} else _info.zText = c_infoJava;
			});
		}
		//_info.zText = c_infoJava;

		//_nodeCaptured = _tree.SelectedItem as _TreeItem; //print.it(_nodeCaptured?.e);
		//perf.nw();
		return true;
	}

	//Called when: 1. Captured, or window/control changed (_FillPropertiesTreeAndCode). 2. A tree item clicked.
	bool _FillProperties(out EProperties p) {
		var (propOK, pr, browser) = _RunElmTask(2000, (_elm, _con.Is0 ? _wnd : _con), static m => {
			if (!m.Item1.GetProperties("Rnuvdakh@srw", out var pr)) return default;
			return (true, pr, _IsVisibleWebPage(m.Item1, m.Item2));
		});
		p = pr;
		if (propOK != _bTest.IsEnabled) _EnableDisableTopControls(propOK);
		if (!propOK) {
			_pageBorder.Child = null;
			_page = null;
			_info.InfoError("Failed to get UI element properties", lastError.message);
			return false;
		}
		_page ??= _commonPage ??= new(this);
		_pageBorder.Child = _page.panel;

		using var nevc = new _NoeventValueChanged(this);

		bool isWeb = browser != 0;
		_isWebIE = isWeb && browser == _BrowserEnum.IE;

		var role = p.Role; if (isWeb) role = "web:" + role;
		_SetHideIfEmpty(_page.roleA, role, check: true, escape: false);
		//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
		bool noName = !_SetHideIfEmpty(_page.nameA, p.Name, check: true, escape: true, dontHide: true);

		if (p.UiaId.ToInt(out int i1) && i1 == _con.ControlId) { //don't use UIA id if == control id
			if (_con.GetWindowAndClientRectInScreen(out var rw1, out var rc1))
				if (rc1 == p.Rect || rw1 == p.Rect) p.UiaId = null;
		}
		if (_SetHideIfEmpty(_page.uiaidA, p.UiaId, check: noName, escape: true)) noName = false;

		//control
		bool isClassId = !isWeb && !_con.Is0 && !_useCon;
		_page.idA.Visible = isClassId;
		_page.classA.Visible = isClassId;
		if (isClassId) {
			string sId = TUtil.GetUsefulControlId(_con, _wnd, out int id) ? id.ToString() : _con.NameWinforms;
			bool hasId = _SetHideIfEmpty(_page.idA, sId, check: true, escape: false);
			_Set(_page.classA, TUtil.StripWndClassName(_con.ClassName, true), check: !hasId);
		}

		_SetHideIfEmpty(_page.valueA, p.Value, check: false, escape: true);
		if (_SetHideIfEmpty(_page.descriptionA, p.Description, check: noName, escape: true)) noName = false;
		_SetHideIfEmpty(_page.actionA, p.DefaultAction, check: false, escape: true);
		if (_SetHideIfEmpty(_page.keyA, p.KeyboardShortcut, check: noName, escape: true)) noName = false;
		if (_SetHideIfEmpty(_page.helpA, p.Help, check: noName, escape: true)) noName = false;

		if (p.HtmlAttributes.Count > 0) {
			var b = new wpfBuilder(_page.htmlAttr).Columns((0, ..100), -1).Options(modifyPadding: false, margin: new());
			foreach (var attr in p.HtmlAttributes) {
				string na = attr.Key, va = attr.Value;
				bool check = noName && (na == "id" || na == "name") && va.Length > 0;
				var k = b.xAddCheckText("@" + na, TUtil.EscapeWildex(va));
				if (check) { k.c.IsChecked = true; noName = false; }
				var info = TUtil.CommonInfos.AppendWildexInfo(TUtil.CommonInfos.PrependName(na, "HTML attribute."));
				_info.ZAddElem(k.c, info);
				_info.ZAddElem(k.t, info);
			}
			b.End();
		} else _page.htmlAttr.Child = null;

		int item = _elm.Item;
		_page.elemA.Visible = item != 0;
		if (item != 0) {
			_Set(_page.elemA, item.ToS());
			if (noName && role is "BUTTON" or "CHECKBOX") { noName = false; _page.elemA.c.IsChecked = true; } //buttons in some toolbars don't have Name
		}

		_Set(_page.stateA, p.State.ToString());
		_Set(_page.rectA, $"{{W={p.Rect.Width} H={p.Rect.Height}}}");

		_page.alsoA.c.IsChecked = false;
		_page.skipA.c.IsChecked = false;
		_page.navigA.c.IsChecked = false;
		if (isWeb && !_waitAutoCheckedOnce) _wait.c.IsChecked = _waitAutoCheckedOnce = true;
		_page.uiaA.IsChecked = _elm.MiscFlags.Has(EMiscFlags.UIA);

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
			_capt.Capturing = _cCapture.IsChecked;
		} else if (_noeventValueChanged < 1 && _page != null) {
			using var nevc = new _NoeventValueChanged(this);
			if (source is KCheckBox c) {
				if (c == _cControl) {
					_useCon = c.IsChecked;
					_FillPropertiesTreeAndCode();
					return;
				} else if (c == _page.inPath) {
					_PathAddRemove();
				} else if (c == _page.uiaA) {
					_uiaUserChecked = c.IsChecked;
					_ClearTree();
					_cCapture.IsChecked = true;
					TUtil.InfoTooltip(ref _ttRecapture, c, "Please capture the UI element again.");
				}
			} else if (source is TextBox t && t.Tag is KCheckTextBox k) {
				k.CheckIfTextNotEmpty();
			}

			_FormatCode();
		}
	}

	int _noeventValueChanged;
	ref struct _NoeventValueChanged
	{
		Delm _d;
		public _NoeventValueChanged(Delm d) { _d = d; _d._noeventValueChanged++; }
		public void Dispose() { _d._noeventValueChanged--; }
	}

	KPopup _ttRecapture;

	(string code, string wndVar) _FormatCode(bool forTest = false) {
		if (_page == null) return default; //failed to get UI element props

		bool isFinder = !forTest && _ActionIsFinder;
		bool orThrow = !(forTest | isFinder) && _cException.IsChecked;
		bool isAction = !forTest && _ActionIsAction;
		bool isVar = !(forTest | isFinder /*|| (isAction && orThrow && _Opt.Has(_EOptions.Compact))*/);

		var b = new StringBuilder();
		string wndCode = null, wndVar = null;
		if (isFinder) {
			b.Append("var f = elm.path");
		} else {
			(wndCode, wndVar) = _code.ZGetWndFindCode(forTest, _wnd, _useCon ? _con : default);
			b.AppendLine(wndCode);
			if (isVar) b.Append("var e = ");
			b.Append(wndVar).Append(".Elm");
		}

		bool isInPath = _page != _commonPage;
		int nPathPages = isInPath ? _path.Count : 1;
		for (int iPathPage = 0; iPathPage < nPathPages; iPathPage++) {
			var page = isInPath ? _path[iPathPage].page : _page;
			b.Append('[');
			page.roleA.GetText(out var role, emptyToo: true);
			if (iPathPage > 0 && !role.NE()) role = role[(role.IndexOf(':') + 1)..];
			b.AppendStringArg(role);

			bool isName = page.nameA.GetText(out var name, emptyToo: true);
			if (isName) b.AppendStringArg(name);

			int nProp = 0, propStart = 0;
			void _AppendProp(KCheckTextBox k, bool emptyToo = false) {
				if (!k.GetText(out var va, emptyToo)) return;
				if (k == page.levelA && va == "0 1000") return;
				if (k == page.maxccA && va == "10000") return;
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

			_AppendProp(page.uiaidA, true);
			if (iPathPage == 0) {
				_AppendProp(page.idA, true);
				_AppendProp(page.classA);
			}
			_AppendProp(page.valueA, true);
			_AppendProp(page.descriptionA, true);
			_AppendProp(page.actionA, true);
			_AppendProp(page.keyA, true);
			_AppendProp(page.helpA, true);
			if (page.htmlAttr.Child is Grid g) foreach (var c in g.Children.OfType<KCheckBox>()) _AppendProp(c.Tag as KCheckTextBox, true);
			_AppendProp(page.elemA);
			_AppendProp(page.stateA);
			_AppendProp(page.rectA);
			_AppendProp(page.notinA);
			_AppendProp(page.maxccA);
			_AppendProp(page.levelA);
			if (nProp > 0) {
				if (nProp == 1) b.Remove(propStart, 4); //new(
				else b.Append(')');
			}

			if (TUtil.FormatFlags(out var s1,
				(page.hiddenTooA, EFFlags.HiddenToo),
				(page.reverseA, EFFlags.Reverse),
				(iPathPage == 0 ? page.uiaA : null, EFFlags.UIA),
				(iPathPage == 0 ? page.notInprocA : null, EFFlags.NotInProc),
				(iPathPage == 0 ? page.clientAreaA : null, EFFlags.ClientArea),
				(page.menuTooA, EFFlags.MenuToo)
				)) b.AppendOtherArg(s1, (isName && nProp > 0) ? null : "flags");

			if (page.alsoA.GetText(out var also)) b.AppendOtherArg(also, "also");
			if (page.skipA.GetText(out var skip)) b.AppendOtherArg(skip, "skip");
			if (page.navigA.GetText(out var navig)) b.AppendStringArg(navig, "navig");
			b.Append(']');
		}

		if (isFinder) {
			b.Append(';');
			b.Append(_screenshot);
		} else {
			b.Append(".Find(");
			if (!forTest && _wait.GetText(out var waitTime, emptyToo: true)) b.AppendWaitTime(waitTime, orThrow); else if (orThrow) b.Append('0');
#if true
			b.Append(");");
			if (!forTest) b.Append(_screenshot);
			if (isVar && !orThrow) b.Append("\r\nif(e == null) { print.it(\"not found\"); }");
			if (isAction) {
				b.Append("\r\ne");
				if (!orThrow) b.Append('?');
				b.Append('.').Append(_ActionGetCode(test: false)).Append(';');
			}
#else
//rejected: _EOptions.Compact - instead of 2 lines "elm e=find" and "e.Action()" format 1 line "find.Action()".
//	Often I forget to uncheck it and have to edit the code if need with variable. Also then .Action() often is far.

			b.Append(')');
			if (isAction && !isVar) _AppendAction(); else b.Append(';');
			if (!forTest) b.Append(_screenshot);
			if (isVar && !orThrow) b.Append("\r\nif(e == null) { print.it(\"not found\"); }");
			if (isAction && isVar) { b.Append("\r\ne"); _AppendAction(); }

			void _AppendAction() {
				if (!orThrow) b.Append('?');
				b.Append('.').Append(_ActionGetCode(test: false)).Append(';');
			}
#endif
		}

		var R = b.ToString();

		if (!forTest) _code.ZSetText(R, wndCode.Lenn());

		return (R, wndVar);
	}

	#region capture

	TUtil.CapturingWithHotkey _capt;

	void _InitCapturingWithHotkey() {
		_capt = new TUtil.CapturingWithHotkey(_cCapture,
			p => {
				var flags = _GetXYFlags(p);
				//bool xy = _ActionIsMouse(_iAction);
				return _RunElmTask(500, (p, flags), static m => {
					//don't show rects when a mouse button is pressed.
					//	With some apps then hangs. Eg Word -> Insert -> Symbol, click a symbol; notinproc too, but not UIA.
					//print.it(Au.mouse.isPressed());
					if (mouse.isPressed()) return default;

					//using var pe1 = perf.local();
					using var e = _ElmFromPointRaw(m.p, m.flags);
					//pe1.Next('e');
					if (e == null) return default;
					var r = e.Rect;
					//pe1.Next('r');
					var s = e.Role;
					//if (m.xy) s = $"{s}    {m.p.x - r.left}, {m.p.y - r.top}";

					//rejected: if big etc, inform about CapsLock
					//if (!m.flags.HasAny(EXYFlags.TrySmaller | EXYFlags.NotInProc) && r.Width * r.Height > 5000) {
					//	var ri = e.RoleInt;
					//	if (!(_RoleIsLinkOrButton(ri) || ri is ERole.Custom or ERole.TEXT or ERole.STATICTEXT or ERole.IMAGE or ERole.DIAGRAM)) {
					//		int wid = (int)Dpi.Unscale(r.Width, r), hei = Math2.MulDiv(r.Height, wid, r.Width);
					//		//print.it(wid, hei, wid * hei);
					//		bool big = wid * hei > 10000;
					//		if (!big && !m.flags.Has(EXYFlags.UIA)) {
					//			var ee = _ElmFromPointRaw(m.p, m.flags | EXYFlags.UIA);
					//			if (ee != null && ee.RoleInt != ri) {
					//				var rr = ee.Rect;
					//				if (rr.Width * rr.Height < r.Width * r.Height / 2) big = true;
					//			}
					//		}
					//		if (big) s += "\nCapsLock to try smaller";
					//	}
					//}

					return (r, s);
				});
			},
			(App.Settings.delm.hk_capture, _Capture),
			(App.Settings.delm.hk_insert, () => _Insert(hotkey: true))
			);

		_cCapture.IsChecked = true;
	}

	void _Capture() {
		_ttRecapture?.Close();
		_info.zText = _IsAutoTest ? "" : _dialogInfo; //clear error/info from previous test etc. If with auto-test options, make empty to reduce flickering.

		if (_ElmFromPoint(mouse.xy) is not elm e) return;

		_elm = e;
		_testedAction = 0;
		_SetElm(true);
		var w = this.Hwnd();
		if (w.IsMinimized) {
			w.ShowNotMinMax();
			w.ActivateL();
		}
	}

	const EXYFlags c_EXYFlags_CanAddUIA = (EXYFlags)0x1000000;

	EXYFlags _GetXYFlags(POINT p) {
		var flags = EXYFlags.PreferLink;
		if (_page != null) {
			if (_uiaUserChecked == true && _page.uiaA.IsChecked) flags |= EXYFlags.UIA;
			if (_page.notInprocA.IsChecked) flags |= EXYFlags.NotInProc;
		}
		if (!flags.Has(EXYFlags.UIA) && _uiaUserChecked == null) flags |= c_EXYFlags_CanAddUIA;
		if (keys.isCapsLock) flags |= EXYFlags.TrySmaller; //CONSIDER: need something better. Eg CapsLock may be remapped to a useful key without a possibility to toggle it. Also, users will forget to toggle off when finished using. But it should not be just a checkbox, because often need to quickly toggle without moving the mouse over other windows.
		return flags;
	}

	static elm _ElmFromPointRaw(POINT p, EXYFlags flags) {
		var hr = Cpp.Cpp_AccFromPoint(p, flags, static (flags, wFP, wTL) => {
			if (!flags.Has(EXYFlags.UIA) && flags.Has(c_EXYFlags_CanAddUIA)) {
				//UIA was faster by ~20% in all tested Store apps.
				//	Also, MSAA Invoke does not work with many controls, eg in Settings and XAML Gallery. The UIA wrapper tries various patterns (Toggle, Expand, Select) and usually it works.
				//	Also, when eg window minimized, the Store process is suspended, and MSAA hangs when trying to get properties etc; UIA fails immediately (good).
				if (0 != wTL.IsUwpApp || wTL.IsWindows8MetroStyle) flags |= EXYFlags.UIA | EXYFlags.NotInProc;
				else if (0 != wTL.ClassNameIs(
					"WinUIDesktopWin32WindowClass", //WinUI (UIA faster/better)
					"GlassWndClass*" //JavaFX (no MSAA)
					)) flags |= EXYFlags.UIA;
			}

			if (osVersion.minWin8_1 ? !flags.Has(EXYFlags.NotInProc) : flags.Has(EXYFlags.UIA)) {
				bool dpiV = Dpi.IsWindowVirtualized(wTL);
				if (dpiV) flags |= Enum_.EXYFlags_DpiScaled;
			}

			return flags & ~c_EXYFlags_CanAddUIA;
		}, out var a);
		//Debug_.PrintIf(hr != 0, "failed");
		if (hr != 0) return null;
		var e = new elm(a);
		return e;
	}

	//Called only when capturing, not to display rectangles of elements from mouse.
	elm _ElmFromPoint(POINT p, bool ctor = false) {
		var flags = _GetXYFlags(p);

		var (e, e2, role1, role2, tt1, tt2, eRect, eRect2)
			= _RunElmTask(2000, (p, flags, ctor), static m => _GetElm(m.p, m.flags, m.ctor));
		if (e == null) return null;

		_screenshot = TUtil.MakeScreenshot(p, _capt);

		if (e2 != null && !role1.NE() && !role2.NE()) {
			var m = new popupMenu();
			m.Add(1, "&" + role1).Tooltip = tt1;
			for (int i = 0; i < role2.Length; i++) if (role2[i] != role1[0]) { role2 = role2.Insert(i, "&"); break; }
			m.Add(2, role2).Tooltip = tt2;
			if (2 == _ShowMenu(m, eRect)) { e = e2; eRect = eRect2; }
		}

		//set x y field always, not only when a mouse action selected, because may select a mouse action afterwards
		_noeventValueChanged++;
		_xy.t.Text = $"{p.x - eRect.left}, {p.y - eRect.top}";
		_noeventValueChanged--;

		return e;

		static (elm e, elm e2, string role1, string role2, string tt1, string tt2, RECT eRect, RECT eRect2)
			_GetElm(POINT p, EXYFlags flags, bool ctor) {

			if (ctor /*&& wnd.fromXY(p).ClassNameIs("Chrome_*")*/) {
				//workaround for: Chrome may give wrong element at first. Eg youtube right list -> "x months ago".
				//	Possibly this can be useful with some other apps too.
				//	If not ctor, capturing works well because _ElmFromPointRaw is called every 250 ms to display element rectangles.
				//	Even with this delay, may be no HTML attributes at first. Never mind.
				_ElmFromPointRaw(p, flags & ~EXYFlags.TrySmaller);
				100.ms();
			}

			var e = _ElmFromPointRaw(p, flags);
			if (!uacInfo.isAdmin && wnd.fromMouse().UacAccessDenied) print.warning("The target process is admin and this process isn't. Can't use its UI elements.", -1); //not _info.InfoError, it's unreliable here, even with timer
			if (e == null) return default;

			//If e probably does not support Invoke or Focus, show menu with e and the first ancestor that supports it.
			//	In some cases use the ancestor without a menu (if LINK or BUTTON etc).
			//	This code is similar to the C++ code in _FromPoint_GetLink, but covers more cases.
			//	Cannot show menu in this thread. Pass e2 and strings to the UI thread, let it show menu if e2 not null.
			elm e2 = null; string tt1 = null, tt2 = null;
			if (!e.MiscFlags.HasAny(EMiscFlags.UIA | EMiscFlags.Java)) { //SHOULDDO: UIA too
				if (!_IsInteractive(e, true, out bool stop1) && !stop1) {
					bool found = false;
					tt1 = "This element probably does not support Invoke or Focus";
					tt2 = "This parent element probably supports Invoke";
					elm ep = e.Parent, ep0 = ep;
					for (int i = 20; i-- > 0 && ep != null; ep = ep.Parent) {
						if ((found = _IsInteractive(ep, false, out bool stop2)) || stop2) break;
						//print.it(ep.ChildCount);
						i -= ep.ChildCount; //sometimes the subtree is <div><div><div>..., 1-2 children at every level
					}
					if (!found && ep != null && e.MiscFlags.Has(EMiscFlags.InProc)) { //detect when e is not a child of its parent
						var ee = ep0.Elm[e.Role, prop: $"level=0\0rect={e.RectRawDpi_}", flags: EFFlags.MenuToo].Find();
						//print.it(ee);
						if (found = ee == null) {
							Debug_.Print("e is not a child of its parent: " + e);
							ep = ep0;
							tt1 = "Possibly disconnected from the tree";
							tt2 = "Parent element";
						}
					}
					if (found && ep.Name.NE() && !e.Name.NE()) found = false;
					if (found && ep.WndContainer.Is0) found = false; //see bug comment in C++
					if (found) {
						bool use = _RoleIsLinkOrButton(ep.RoleInt)
							//|| role ERole.MENUITEM //will need EFFlags.MenuToo. But then cannot capture child elements in some menu items, and cannot select in tree because it closes the menu.
							;
						if (use) e = ep; else e2 = ep;
					}
				} else if (!stop1) {
					//is in COMBOBOX?
					if (e.RoleInt is ERole.TEXT or ERole.BUTTON && e.Parent is elm ep) {
						var r1 = ep.RoleInt;
						if (r1 == ERole.COMBOBOX || (r1 == ERole.WINDOW && ((ep = ep.Parent)?.RoleInt ?? 0) == ERole.COMBOBOX)) e2 = ep;
					}
				}

				static bool _IsInteractive(elm e, bool orig, out bool stop) {
					stop = false;
					if (e.Item != 0) return true;
					var role = e.RoleInt;
					if (_RoleIsLinkOrButton(role)) return true;
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
			}

			if (e2 == null) return (e, null, null, null, null, null, e.Rect, default);
			return (e, e2, e.Role, e2.Role, tt1, tt2, e.Rect, e2.Rect);
		}
	}
	bool? _uiaUserChecked; //not null if the UIA checkbox was user-changed and therefore the capturing code must obey
	bool _waitAutoCheckedOnce; //if user unchecks, don't check next time

	#endregion

	#region Insert, Test

	///// <summary>
	///// When OK clicked, contains C# code. Else null.
	///// </summary>
	//public string ZResultCode { get; private set; }

	void _Insert(bool hotkey) {
		if (_close && !hotkey) {
			base.Close();
		} else if (_code.zText.NullIfEmpty_() is string s) {
			if (_Opt.Has(_EOptions.Activate) && !_wndNoActivate && !_ActionIsFinder) {
				s = s.RxReplace(@"^.+?\bwnd\.find\(.+[^(]\)\K;\r", ".Activate();\r", 1);
			}
			if (!_Opt.Has(_EOptions.NoScope)) {
				string newline = _ActionIsAction ? null : "\r\n"; //if no action, add empty line
				s = "{\r\n" + s + newline + "\r\n}";
			}
			InsertCode.Statements(s);
			if (!hotkey) {
				_close = true;
				_bInsert.Content = "Close";
				_bInsert.MouseLeave += (_, _) => {
					_close = !true;
					_bInsert.Content = "Insert";
				};
			}
		}
	}
	bool _close;

	void _Test(bool captured = false, bool testAction = false, bool actWin = false) {
		if (_page == null) return;

		var (code, wndVar) = _FormatCode(true); if (code.NE()) return;
		var elmSelected = _page == _commonPage ? _elm : _path[^1].ti.e;

		if (testAction) testAction = _ActionCanTest;
		bool autoInsert = captured && _cAutoInsert.IsChecked;

		var restoreOwner = new int[1];
		try {
			var (rr, bad) = _RunElmTask(10000, (this.Hwnd(), _WndSearchIn), m => {
				elmFinder.t_navigResult = (true, null, null);
				var rr = TUtil.RunTestFindObject(m.Item1, code, wndVar, m.Item2, o => (o as elm).Rect, actWin, restoreOwner, this.Dispatcher);
				elm elmFound = rr.obj as elm, elmFoundBN = null;
				//need elm found before navig
				if (elmFound != null) elmFoundBN = elmFinder.t_navigResult.after == elmFound ? elmFinder.t_navigResult.before : elmFound;
				elmFinder.t_navigResult = default;
				bool bad = false;
				if (elmFound != null) {
					RECT r1 = elmFoundBN.Rect, r2 = elmSelected.Rect;
					//print.it(r1, r2); //in DPI-scaled windows can be slightly different if different inproc of elmSelected and elmFoundBN. Would be completely different if using raw rect.
					int diff = elmFoundBN.MiscFlags.Has(EMiscFlags.InProc) == elmSelected.MiscFlags.Has(EMiscFlags.InProc) ? 0 : 2;
					bad = (!RECT.EqualFuzzy_(r1, r2, diff) || elmFoundBN.Role != elmSelected.Role);
				}
				return (rr, bad);
			});

			if (rr.obj is not elm elmFound) {
				string osd;
				if (rr.info == null) _info.InfoError(osd = "Timeout", "Not found in 10 s.");
				else if (rr.speed < 0) { //error
					_info.InfoErrorOrInfo(rr.info);
					osd = rr.info.header;
				} else { //not found
					osd = "Not found";
					string s2 = "Try: check <b>Find hidden too<>; check/uncheck/edit other controls.";
					int n = _page.maxccA.GetText(out _) ? 0 : _RunElmTask(200, 0, m => _elm.Parent?.ChildCount ?? 0);
					if (n > 10000) { //never mind: may be an indirect ancestor. Rare.
						s2 = $"The parent element has {n} children. Need to specify maxcc.";
					} else {
						if (_PathIsIntermediate()) s2 += "\r\nTry <b>skip<> -1 to search for next path element in all matching intermediate elements.";
						if (_page.navigA.GetText(out _)) s2 += "\r\nTry <b>skip<> -1 to retry failed <b>navig<>ation with all matching intermediate elements.";
						if (!_wnd.IsActive) {
							if (_page.actionA.GetText(out _)) s2 += "\r\nNote: <b>action<> often is unavailable in inactive window.";
							if (_page.keyA.GetText(out _)) s2 += "\r\nNote: <b>key<> sometimes is unavailable in inactive window.";
							if (_page.stateA.GetText(out _)) s2 += "\r\nNote: <b>state<> often is different in inactive window.";
							s2 += "\r\n<+actTest>Activate window and test find<>";
						}
					}
					_info.InfoError("Not found", s2, rr.info.headerSmall);
				}
				_Osd(osd, true);
				return;
			}
			if (bad) {
				var s2 = "Try: <b>Add to path<> or/and <b>skip<>; check/uncheck/edit other controls.\r\nIf this element cannot be uniquely identified (no name etc), try another element and use <b>navig<>.";
				if (_PathIsIntermediate()) s2 += "\r\nTry <b>skip<> -1 to search for next path element in all matching intermediate elements.";
				string s1 = "Found wrong element";
				_info.InfoError(s1, s2, rr.info.headerSmall);
				_Osd(s1, true);
				return;
			}
			_info.InfoErrorOrInfo(rr.info);

			//if (quickOK && r.speed < 1_000_000) {
			//	//timer.after(1000, _ => _bOK.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)));
			//	return;
			//}

			if (testAction) {
				string aCode = _ActionGetCode(test: true);
				if (aCode != null) {
					Api.AllowSetForegroundWindow();
#if true
					var re = _RunElmTask(2000, (elmFound, aCode), static m => TUtil.RunTestAction(m.elmFound, m.aCode));
					if (re != null) {
						_info.InfoErrorOrInfo(re);
						_Osd(re.header, true);
						return;
					}
					restoreOwner[0] = 1000;
#else
					var re = await _StartElmTask((elmFound, aCode), static m => TUtil.RunTestAction(m.elmFound, m.aCode));
					if (re != null) {
						_info.InfoErrorOrInfo(re);
						return;
					}
					restoreOwner[0] = 1000;
#endif
				}
			}

			if (autoInsert) {
				_Insert(hotkey: true);
				_Osd("Inserted", false);
			}
		}
		finally { if (restoreOwner[0] == 0) restoreOwner[0] = 1; }

		void _Osd(string s, bool error) {
			if (!captured) return;
			osdText.showTransparentText(s, 2, PopupXY.Mouse, error ? 0xFF6040 : 0xC000, showMode: OsdMode.ThisThread);
		}
	}

	bool _IsAutoTest => _Opt.HasAny(_EOptions.AutoTest) || _cAutoTestAction.IsChecked || _cAutoInsert.IsChecked;

	void _bTest_ContextMenuOpening(object sender, ContextMenuEventArgs e) {
		var m = new popupMenu();
		//bool isAction = _page != null && _ActionCanTest();
		//m["Test action", disable: !isAction] = _ => _Test(testAction: true);
		//m["Activate window and test action", disable: !isAction] = _ => _Test(testAction: true, actWin: true);
		//m["Activate window and test find", disable: _page == null] = _ => _Test(actWin: true);
		m["Activate window and test", disable: _page == null] = _ => _Test(actWin: true);
		m.Show(owner: this);
	}

	#endregion

	#region tree

	_TreeItem _treeRoot;
	//_TreeItem _nodeCaptured;
	bool _isWebIE; //_FillProperties sets it; then _FillTree uses it.

	void _InitTree() {
		_tree.SingleClickActivate = true;
		_tree.ItemActivated += (o, e) => {
			var ti = e.Item as _TreeItem;
			_elm = ti.e;
			//_screenshot = null;
			_SetWndCon(_wnd, _GetWndContainer(), _useCon);
			if (!_PathSetPageWhenTreeItemSelected(ti)) {
				if (!_FillProperties(out _)) return;
			}
			_FormatCode();
			TUtil.ShowOsdRect(_RunElmTask(1000, _elm, e => e.Rect));
		};

		_tree.ItemClick += (_, e) => {
			if (e.MouseButton == MouseButton.Right) {
				var ti = e.Item as _TreeItem;
				var m = new popupMenu();
				m["Navigate to this from the selected"] = _ => _NavigateTo(ti);
				m.Show(owner: this);
			}
		};
	}

	void _ClearTree() {
		_tree.SetItems(null);
		_treeRoot = null;
		_PathClear();
	}

	(_TreeItem xRoot, _TreeItem xSelect) _CreateTreeModel(wnd w, EProperties p, bool skipWINDOW) {
		EFFlags flags = Enum_.EFFlags_Mark | EFFlags.HiddenToo | EFFlags.MenuToo;
		if (_page.uiaA.IsChecked) flags |= EFFlags.UIA;
		if (!_elm.MiscFlags.Has(EMiscFlags.InProc)) flags |= EFFlags.NotInProc; //if captured notinproc in DPI-scaled, would fail to select in tree if tree elems retrieved inproc, because would compare with non-scaled rects

		var (xRoot, xSelect, exc) = _RunElmTask(10000, this, dlg => {
			//SHOULDDO: cancellation
			var us = (uint)p.State;
			var prop = $"rect={p.Rect}\0state=0x{us:X},!0x{~us:X}";
			if (skipWINDOW) prop += $"\0 notin=WINDOW";
			var role = p.Role.NullIfEmpty_();

			_TreeItem xRoot = new(dlg), xSelect = null;
			var stack = new Stack<_TreeItem>(); stack.Push(xRoot);
			int level = 0;

			try {
				w.Elm[role, "**tc " + p.Name, prop, flags, also: o => {
					//var x = new _ElmNode(o.Role);
					_TreeItem x = new(dlg);
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
						if (xSelect == null) xSelect = x;
					}
					stack.Peek().AddChild(x);
					return false;
				}
				].Exists();
			}
			catch (Exception ex) { return (null, null, ex.Message); }
			return (xRoot, xSelect, (string)null);
		});
		if (xRoot == null) _info.InfoError("Failed to get UI element tree.", exc ??= "Timeout.");
		return (xRoot, xSelect);
	}

	void _FillTree(EProperties p) {
		Debug.Assert(_treeRoot == null); Debug.Assert(_path == null); //_ClearTree must be called before

		var w = _WndSearchIn;
		if (_isWebIE && !_useCon && !_con.Is0) w = _con; //if IE, don't display whole tree. Could be very slow, because cannot use in-proc for web pages (and there may be many tabs with large pages), because its control is in other thread.
		var (xRoot, xSelect) = _CreateTreeModel(w, p, false);
		if (xRoot == null) return;

		if (xSelect == null && w.IsAlive) {
			//IAccessible of some controls are not connected to the parent.
			//	Noticed this in Office 2003 Word Options dialog and in Dreamweaver.
			//	Also, WndContainer then may get the top-level window. Eg in Word.
			//	Workaround: enum child controls and look for _elm in one them. Then add "class" row if need.
			Debug_.Print("broken IAccessible branch");
			foreach (var c in w.Get.Children(onlyVisible: true)) {
				var m = _CreateTreeModel(c, p, true);
				if (m.xSelect != null) {
					//m.xRoot.a = elm.fromWindow(c, flags: EWFlags.NoThrow);
					//if(m.xRoot.a != null) model.xRoot.Add(m.xRoot);
					//else model.xRoot = m.xRoot;
					(xRoot, xSelect) = m;
					if (!_page.classA.Visible) {
						_page.classA.t.Text = TUtil.StripWndClassName(c.ClassName, true);
						_page.classA.c.IsChecked = true;
						_page.classA.Visible = true;
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
	//Usually faster than recreating tree, but in some cases can be slower. Slower when fails to find.
	bool _TrySelectInSameTree(out _TreeItem ti) {
		ti = null;
		if (_treeRoot == null) return false;
		var a = _treeRoot.Descendants().ToArray();
		if (a.Length == 0) return false;

		if (_elm.MiscFlags.Has(EMiscFlags.UIA) != _page.uiaA.IsChecked //different tree
			|| _elm.MiscFlags.Has(EMiscFlags.InProc) != a[0].e.MiscFlags.Has(EMiscFlags.InProc) //different rects if DPI-scaled window
			) {
			_ClearTree();
			return false;
		}
		//if(keys.isScrollLock) return false;

		ti = _RunElmTask(5000, (_elm, a), static m => {
			//SHOULDDO: cancellation
			var e = m.Item1;
			if (!e.GetProperties("rn", out var p)) return null;
			int item = e.Item;
			var ri = e.RoleInt;
			string rs = e.RoleInt == ERole.Custom ? e.Role : null;
			//CONSIDER: to make faster, run all this code inproc. Or at least switch context once for each element.
			foreach (var v in m.a) {
				e = v.e;
				if (e.Item != item) continue;
				if (e.RoleInt != ri) continue;
				if (rs != null && e.Role != rs) continue;
				if (!e.GetRect(out var rr, raw: true) || rr != p.Rect) continue;
				if (e.Name != p.Name) continue;
				return v;
			}
			return null;
		});
		if (ti != null) _SelectTreeItem(ti);
		else Debug_.Print("recreating tree of same window");
		return ti != null;

		//Other ways to compare elm:
		//IAccIdentity. Unavailable in web pages.
		//IUIAutomationElement. Very slow ElementFromIAccessible. In Firefox can be 30 ms.
	}

	class _TreeItem : TreeBase<_TreeItem>, ITreeViewItem
	{
		readonly Delm _dlg;
		public elm e;
		string _displayText;
		bool _isFailed;
		bool _isInvisible;
		bool _isExpanded;

		public _TreeItem(Delm dlg) {
			_dlg = dlg;
		}

		#region ITreeViewItem

		string ITreeViewItem.DisplayText {
			get {
				if (_displayText == null) {
					(_displayText, _isInvisible, _isFailed) = _dlg._RunElmTask(500, e, static e => {
						bool isWINDOW = e.RoleInt == ERole.WINDOW;
						string props = isWINDOW ? "Rnsw" : "Rns";
						if (!e.GetProperties(props, out var p)) return ("Failed: " + lastError.message, false, true);

						string s;
						if (isWINDOW) {
							using (new StringBuilder_(out var b)) {
								b.Append(p.Role).Append("  (").Append(p.WndContainer.ClassName).Append(')');
								if (p.Name.Length > 0) b.Append("  \"").Append(p.Name).Append('\"');
								s = b.ToString();
							}
						} else if (p.Name.Length == 0) s = p.Role;
						else s = p.Role + " \"" + p.Name.Escape(limit: 250) + "\"";

						return (s, e.IsInvisible_(p.State), false);
					});
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
				if (_dlg._PathFind(this) >= 0) return 0x00C000;
				return -1;
			}
		}

		#endregion
	}

	#endregion

	#region path

	List<(_TreeItem ti, _PropPage page)> _path;

	void _PathAddRemove() {
		if (_tree.SelectedItem is not _TreeItem ti) return;
		bool add = _page.inPath.IsChecked;
		if (add) {
			if (_path == null) {
				_path = new() { (ti, _commonPage) };
			} else {
				//find index where to insert
				int i = -1;
				foreach (var v in ti.Ancestors()) if (v == _path[^1].ti) { i = _path.Count; break; } //is ti after current path?
				if (i < 0) { //is ti before current path?
					for (int j = 0; j < _path.Count; j++) {
						if (_path[j].ti.Ancestors().Contains(ti)) { i = j; break; }
					}
				}
				if (i < 0) { //isn't straight path. Eg the user wants to use navig.
					if (!dialog.showInputNumber(out i, "Index in path", "This element isn't an ancestor or descendant of a path element,\r\ntherefore its index in path cannot be determined automatically.\r\n\r\n0-based index:", 0, owner: this)) {
						_page.inPath.IsChecked = false;
						return;
					}
					i = Math.Clamp(i, 0, _path.Count);
				}
				_path.Insert(i, (ti, _commonPage));
			}
			_page = _commonPage;
			_commonPage = null;
			_page.ti = ti;
		} else {
			int i = _PathFind(ti); if (i < 0) return;
			if (_path.Count > 1) _path.RemoveAt(i); else _path = null;
			_page.ti = null;
			_commonPage = _page;
		}

		_tree.Redraw();
		//then _FormatCode will be called
	}

	bool _PathSetPageWhenTreeItemSelected(_TreeItem ti) {
		//print.it(ti.e);
		if (_path == null || _page == null) return false;
		if (_page.ti == ti) return false;
		int i = _PathFind(ti);
		_page = i < 0 ? _commonPage : _path[i].page;
		if (i >= 0) _pageBorder.Child = _page.panel;
		return i >= 0;
	}

	int _PathFind(_TreeItem ti) {
		if (_path != null) {
			for (int i = 0; i < _path.Count; i++) if (_path[i].ti == ti) return i;
		}
		return -1;
	}

	bool _PathIsIntermediate() {
		if (_path == null || _page == null) return false;
		return (uint)_PathFind(_page.ti) < _path.Count - 1;
	}

	//int _PathFind() {
	//	if (_path != null && _tree.SelectedItem is _TreeItem ti) return _PathFind(ti);
	//	return -1;
	//}

	void _PathClear() {
		if (_path == null) return;
		if (_page?.ti != null) {
			_page.ti = null;
			using var nevc = new _NoeventValueChanged(this);
			_page.inPath.IsChecked = false;
			_commonPage = _page;
		}
		_path = null;
	}

	#endregion

	#region action

	int _iAction;

	void _ActionInit() {
		_cbAction.Items.Add("");
		_cbAction.DropDownOpened += (o, e) => {
			_cbAction.IsDropDownOpen = false;
			_ActionMenu(false);
		};

		var s = App.Settings.delm.actionn;
		_ActionChange(s.NE() ? 0 : _ActionFind(s));
	}

	void _ActionChange(int i) {
		_iAction = i;
		_cbAction.Items[0] = s_actions[i].name;
		_cbAction.SelectedIndex = 0;
		_ActionSetControlsVisibility();
		if (_page != null) _FormatCode();
	}

	static readonly (string name, string code)[] s_actions = {
		("No action", null),
		("Invoke", "Invoke()"),
		("WebInvoke", "WebInvoke()"),
		//("JavaInvoke", "JavaInvoke()"), //rare. Usually Invoke is used. Would need UI for action name.
		("Mouse", ""), //submenus start with ("Name", "") and end with (null, null)
		("Mouse click", "MouseClick(%)"),
		("Mouse 2*click", "MouseClickD(%)"),
		("Mouse right click", "MouseClickR(%)"),
		("Mouse move", "MouseMove(%)"),
		("", null),
		("Post click", "PostClick(%)"),
		("Post 2*click", "PostClickD(%)"),
		("Post right click", "PostClickR(%)"),
		(null, null),
		("Keys, text", ""),
		("Send keys", "SendKeys(\"\")"),
		("Replace text", "SendKeys(\"Ctrl+A\", \"!text\")"),
		("Append text", "SendKeys(\"Ctrl+End\", \"!text\")"),
		("", null),
		("Click, send keys", "MouseClick(); keys.send(\"\")"),
		("Click, replace text", "MouseClick(); keys.send(\"Ctrl+A\", \"!text\")"),
		("Click, append text", "MouseClick(); keys.send(\"Ctrl+End\", \"!text\")"),
		(null, null),
		("Focus, select", ""),
		("Focus", "Focus()"),
		("Select", "Select()"),
		("Select and focus", "Focus(true)"),
		(null, null),
		("Check", ""),
		("Check", "Check(true)"),
		("Check (keys)", "Check(true, \"\")"),
		("Check (click)", "Check(true, e => e.MouseClick(%))"),
		("Check (post)", "Check(true, e => e.PostClick(%))"),
		("", null),
		("Uncheck", "Check(false)"),
		("Uncheck (keys)", "Check(false, \"\")"),
		("Uncheck (click)", "Check(false, e => e.MouseClick(%))"),
		("Uncheck (post)", "Check(false, e => e.PostClick(%))"),
		(null, null),
		("ComboSelect", ""),
		("ComboSelect", "ComboSelect(^)"),
		("ComboSelect (invoke)", "ComboSelect(^, \"i\")"),
		("ComboSelect (keys)", "ComboSelect(^, \"k\")"),
		("ComboSelect (mouse)", "ComboSelect(^, \"m\")"),
		("Item...", "#cs"),
		(null, null),
		("Expand", ""),
		("Expand", "Expand(true)"),
		("Expand (keys)", "Expand(true, \"\")"),
		("Expand (click)", "Expand(true, e => e.MouseClick(%))"),
		("Expand (2*click)", "Expand(true, e => e.MouseClickD(%))"),
		("Expand (post)", "Expand(true, e => e.PostClick(%))"),
		("Expand (2*post)", "Expand(true, e => e.PostClickD(%))"),
		("Path...", "#ep"),
		("", null),
		("Collapse", "Expand(false)"),
		("Collapse (keys)", "Expand(false, \"\")"),
		("Collapse (click)", "Expand(false, e => e.MouseClick(%))"),
		("Collapse (2*click)", "Expand(false, e => e.MouseClickD(%))"),
		("Collapse (post)", "Expand(false, e => e.PostClick(%))"),
		("Collapse (2*post)", "Expand(false, e => e.PostClickD(%))"),
		(null, null),
		("ScrollTo", "ScrollTo()"),

		//these must be at the end
		("WaitFor", "WaitFor(0, o => !o.IsDisabled)"), //must be before the last
		("new elmFinder", null), //must be the last
	};

	int _ActionMenu(bool test) {
		var m = new popupMenu();
		for (int i = test ? 1 : 0; i < s_actions.Length - (test ? 2 : 0); i++) {
			var (name, code) = s_actions[i];
			if (code == "") { //submenu
				int from = ++i, to = i = Array.FindIndex(s_actions, i, o => o.name == null);
				m.Submenu(name, m => { for (int i = from; i < to; i++) _Add(m, i); });
			} else _Add(m, i);

			void _Add(popupMenu m, int i) {
				if (test) if (s_actions[i].code?.Starts('#') ?? false) return;
				var s = s_actions[i].name;
				if (s == "") m.Separator(); else m.Add(i + 1, s);
			}
		}

		int ia;
		if (test) {
			ia = m.Show(owner: this);
		} else { //called when testing if no action selected
			var r = _cbAction.RectInScreen();
			ia = m.Show(MSFlags.AlignRectBottomTop, new POINT(r.left, r.bottom), r, owner: this);
		}
		if (--ia < 0) return 0;

		if (!test) _testedAction = 0;

		if (s_actions[ia].code is string s1 && s1.Starts('#')) {
			if (!_ActionInputStringArg(s1, out string selectAction)) return 0;
			//select the first action in this submenu, but don't change if was selected an action in this submenu
			ia = s_actions[_ActionIndex].name.Starts(selectAction) ? _ActionIndex : _ActionFind(selectAction);
			if (test && ia == _testedAction) _FormatCode();
		}

		if (!test) {
			_ActionChange(ia);
		} else if (ia != _testedAction) {
			_testedAction = ia;
			_ActionSetControlsVisibility();
			_FormatCode();
		}
		return ia;
	}

	int _ActionFind(string name) {
		if (!name.NE()) {
			var a = s_actions;
			for (int i = 0; i < a.Length; i++) if (a[i].name == name && a[i].code != "") return i;
		}
		Debug_.Print(name);
		return 0; //No action (not -1)
	}

	int _ActionIndex => _iAction > 0 ? _iAction : _testedAction;

	static bool _ActionIsMouse(int i) => s_actions[i].code?.Contains('%') ?? false;
	//bool _ActionIsMouse => _ActionIsMouse(_ActionIndex());

	bool _ActionIsAction => _ActionIndex is int i && i > 0 && i < s_actions.Length - 1;

	bool _ActionIsFinder => _iAction == s_actions.Length - 1;

	bool _ActionCanTest => _ActionIndex is int i && i < s_actions.Length - 2; //if 0, will show menu

	string _ActionGetCode(bool test) {
		int ia = test ? (_iAction > 0 ? _iAction : _ActionMenu(true)) : _ActionIndex;

		if (test && ia == 2) { //WebInvoke -> Invoke, to avoid waiting
			Debug.Assert(s_actions[2].name == "WebInvoke" && s_actions[1].name == "Invoke");
			ia = 1;
		}

		var s = s_actions[ia].code;
		if (s != null) {
			int j = s.IndexOf('%'); //mouse x y placeholder
			if (j > 0) {
				bool scroll = _cScroll.IsChecked;
				if (_xy.GetText(out var xy)) {
					if (s[j - 1] != '(') xy = ", " + xy;
					//if (s[j + 1] != ')') xy += ", ";
					if (scroll) xy += ", scroll: 50";
					s = s.ReplaceAt(j, 1, xy);
				} else {
					s = s.Remove(j, 1);
					if (scroll) s = s.Insert(j, "scroll: 50");
				}
			}

			if (s.Starts("Expand(true")) {
				if (_actionExpandPath != null) s = s.ReplaceAt(7, 4, _actionExpandPath.Escape(quote: true));
			} else if (s.Starts("ComboSelect(^")) {
				if (test) {
					if (_actionComboSelectItem == null || popupMenu.showSimple(new(_actionComboSelectItem, "8492 Other...")) == 8492) {
						if (!_ActionInputStringArg("#cs", out _) || _actionComboSelectItem == null) return null;
						_FormatCode();
					}
				}
				s = s.ReplaceAt(12, 1, (_actionComboSelectItem ?? "Item").Escape(quote: true));
			}
		}
		return s;
	}
	int _testedAction;
	string _actionExpandPath, _actionComboSelectItem;

	bool _ActionInputStringArg(string what, out string selectAction) {
		selectAction = null;
		switch (what) {
		case "#cs":
			selectAction = "ComboSelect";
			return _Dialog(ref _actionComboSelectItem, "ComboSelect item", "Combo box item name. Wildcard expression.");
		case "#ep":
			selectAction = "Expand";
			return _Dialog(ref _actionExpandPath, "Expand path", "Tree node path for Expand actions.\nExample: Folder1|Folder2|Folder3\nPath parts are wildcard expressions.");
		}
		return false; //impossible

		bool _Dialog(ref string r, string text1, string text2) {
			if (!dialog.showInput(out string s, text1, text2, editText: r, owner: this)) return false;
			r = s.NullIfEmpty_();
			return true;
		}
	}

	void _ActionSetControlsVisibility() {
		int i = _ActionIndex;
		bool isMouse = _ActionIsMouse(i);
		_xy.Visible = isMouse;
		_cScroll.Visibility = isMouse ? Visibility.Visible : Visibility.Hidden;
		bool vis2 = i < s_actions.Length - 1;
		_wait.Visible = vis2;
		_cException.Visibility = vis2 ? Visibility.Visible : Visibility.Hidden;
	}

	#endregion

	#region misc

	void _ToolSettings() {
		var m = new popupMenu();
		var cAT = m.AddCheck("Auto test find", _Opt.Has(_EOptions.AutoTest)); cAT.Tooltip = "Test find when captured";
		var cWA = m.AddCheck(".Activate()", _Opt.Has(_EOptions.Activate)); cWA.Tooltip = "Append .Activate() to wnd.find(...), unless the window looks like does not like to be activated";
		//var cCC = m.AddCheck("Compact code", _Opt.Has(_EOptions.Compact)); cNS.Tooltip = "Insert code without { } and don't use elm e with action";
		var cNS = m.AddCheck("No { }", _Opt.Has(_EOptions.NoScope)); cNS.Tooltip = "Insert the code without { }";
		m.Separator();
		m["Save action"] = _ => {
			App.Settings.delm.actionn = s_actions[_iAction].name;
			_SetOpt(_EOptions.MouseXY, _xy.c.IsChecked);
			_SetOpt(_EOptions.MouseScroll, _cScroll.IsChecked);
		}; m.Last.Tooltip = "Let the tool start with current action and its settings";
		m["Save wait"] = _ => {
			App.Settings.delm.wait = _wait.t.Text;
			_SetOpt(_EOptions.NoWait, !_wait.c.IsChecked);
		}; m.Last.Tooltip = "Let the tool start with current wait settings";
		m.Show(owner: this);
		_SetOpt(_EOptions.AutoTest, cAT.IsChecked);
		//bool format = _SetOpt(_EOptions.Compact, cCC.IsChecked);
		//if (format) _FormatCode();
		_SetOpt(_EOptions.NoScope, cNS.IsChecked);
		_SetOpt(_EOptions.Activate, cWA.IsChecked);
	}

	[Flags]
	enum _EOptions
	{
		AutoTest = 1,
		NoScope = 1 << 1,
		MouseXY = 1 << 2,
		NoWait = 1 << 3,
		MouseScroll = 1 << 4,
		Activate = 1 << 5,
		//and don't save autotestaction, autoinsert
	}

	static _EOptions _Opt {
		get => (_EOptions)App.Settings.delm.flags;
		set => App.Settings.delm.flags = (int)value;
	}

	static bool _SetOpt(_EOptions opt, bool on) {
		_EOptions f = _Opt;
		if (f.Has(opt) == on) return false;
		f.SetFlag(opt, on);
		_Opt = f;
		return true;
	}

	//Builds navig path from the selected tree node to *to*. Sets control text and checkbox.
	void _NavigateTo(_TreeItem to) {
		if (_tree.SelectedItem is not _TreeItem from) return;
		//print.it(from.e); print.it(to.e);
		var a = new List<(string s, int n)>();
		if (from.Parent == to.Parent) {
			_AppendNePr(from, to);
		} else {
			var aFrom = from.AncestorsFromRoot(andSelf: true);
			int i = Array.IndexOf(aFrom, to);
			if (i >= 0) { //'to' is ancestor of 'from'
				_AppendPa();
			} else {
				//find common ancestor
				var aTo = to.AncestorsFromRoot(andSelf: true);
				for (i = Math.Min(aFrom.Length, aTo.Length); --i >= 0;) if (aFrom[i] == aTo[i]) break;

				if (++i < aFrom.Length) {
					_AppendPa();
					_AppendNePr(aFrom[i], aTo[i]);
				} else i--;

				while (++i < aTo.Length) {
					var v = aTo[i]; var p = aTo[i].Parent;
					var s = v == p.FirstChild ? "fi" : v == p.LastChild ? "la" : null;
					if (s == null) a.Add(("ch", v.Index + 1));
					else if (a.Count > 0 && a[^1].s == s) a[^1] = (s, a[^1].n + 1);
					else a.Add((s, 1));
				}
			}

			void _AppendPa() {
				int n = aFrom.Length - i - 1;
				if (n > 0) a.Add(("pa", n)); //else common parent with a 'to' ancestor
			}
		}

		void _AppendNePr(_TreeItem from, _TreeItem to) {
			if (to == from) return;
			int n = to.Index - from.Index;
			a.Add((n > 0 ? "ne" : "pr", Math.Abs(n)));
			//never mind: n may be incorrect because some UI elements skip invisible siblings. Eg standard WINDOW elements.
			//	We could detect it and either display a warning or use pa ch instead (can be unreliable).
		}

		var b = new StringBuilder();
		foreach (var (s, n) in a) {
			if (b.Length > 0) b.Append(' ');
			b.Append(s);
			if (n > 1) b.Append(n);
		}
		var navig = b.ToString();

		_page.navigA.Set(navig.Length > 0, navig);
	}

	public static class Java
	{
		/// <summary>
		/// Calls <see cref="EnableDisableJab"/>. Before it shows dialog "enable/disable". After it shows dialog with results.
		/// </summary>
		/// <param name="owner"></param>
		public static void EnableDisableJabUI(AnyWnd owner) {
			bool enable;
			switch (dialog.showList("1 Enable|2 Disable|Cancel", "Java Access Bridge", owner: owner, flags: DFlags.CenterOwner)) {
			case 1: enable = true; break;
			case 2: enable = false; break;
			default: return;
			}
			var (ok, results) = EnableDisableJab(enable);
			if (results != null) dialog.show("Results", results, icon: ok ? DIcon.Info : DIcon.Error, owner: owner, flags: DFlags.CenterOwner);
		}

		/// <summary>
		/// Enables or disables Java Access Bridge for current user.
		/// Returns: ok = false if failed or cancelled. results = null if cancelled.
		/// </summary>
		public static (bool ok, string results) EnableDisableJab(bool enable/*, bool allUsers*/) {
			if (!GetJavaPath(out var dir)) return (false, "Cannot find Java " + (osVersion.is32BitProcess ? "32" : "64") + "-bit. Make sure it is installed.");

			//if(!allUsers) {
			string jabswitch = dir + @"\bin\jabswitch.exe", sout = null;
			if (!filesystem.exists(jabswitch).File) return (false, "Cannot find jabswitch.exe.");
			try {
				run.console(out sout, jabswitch, enable ? "-enable" : "-disable");
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
			if (!filesystem.exists(dll64).File) sout += "\r\n\r\nWarning: dll not found: " + dll64 + ".  64-bit apps will not be able to use UI elements of Java apps. Install 64-bit Java too.";
			if (!filesystem.exists(dll32).File) sout += "\r\n\r\nNote: dll not found: " + dll32 + ".  32-bit apps will not be able to use UI elements of Java apps. Install 32-bit Java too.";

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

	#region util

	wnd _WndSearchIn => _useCon ? _con : _wnd;

	//Returns true if e is in visible web page in one of 3 browsers, and not UIA.
	//Returns nonzero if container's class is like in one of browsers: 1 IES, 2 FF, 3 Chrome. Even if returns false.
	static _BrowserEnum _IsVisibleWebPage(elm e, wnd wContainer) {
		if (e.MiscFlags.HasAny(EMiscFlags.UIA | EMiscFlags.Java)) return 0;
		var browser = (_BrowserEnum)wContainer.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
		if (browser > _BrowserEnum.IE) {
			elm eDoc = null;
			do {
				if (e.RoleInt == ERole.DOCUMENT) eDoc = e;
				e = e.Parent;
			} while (e != null);
			if (eDoc == null || eDoc.IsInvisible) return 0;
		}
		return browser;
	}
	enum _BrowserEnum { IE = 1, FF, Chrome }

	void _EnableDisableTopControls(bool enable) {
		_bTest.IsEnabled = enable; _bInsert.IsEnabled = enable;
		_ActionSetControlsVisibility();
		var vis = enable ? Visibility.Visible : Visibility.Hidden;
		//_cbAction.Visibility = vis; _topRow2.Visibility = vis; //no, may want to change before capturing if 'Auto test action'
		_bWindow.Visibility = vis;
		_cControl.Visibility = vis;
	}

	//Shows popup menu m by RECT r or by the mouse cursor, depending on r size.
	//If r small, shows by r so that the menu does not cover the rectangle. Else shows by the mouse (else the user would not notice the menu easily).
	int _ShowMenu(popupMenu m, RECT r) {
		bool byMouse = r.Height > 200 || r.Is0;
		var mf = byMouse ? MSFlags.Underline : MSFlags.Underline | MSFlags.AlignRectBottomTop | MSFlags.AlignCenterH;
		return m.Show(mf, excludeRect: byMouse ? null : r, owner: this.IsLoaded ? this : default);
	}

	//In some cases used to deadlock, therefore tried to move all elm functions to other thread.
	//But it did not solve the problem, and added other problems, eg can reenter. Need full async/await, but it's too difficult.
	//After improvements in other places now does no deadlock.
#if true
	//This code runs in same thread.
	TRet _RunElmTask<TRet, TParam>(int timeoutMS, TParam param, Func<TParam, TRet> f, [CallerMemberName] string m_ = null) {
		return f(param);
	}
#elif true
	//This code should work well, but has 2 problems.
	//	1. Deadlocks anyway, although maybe temporarily.
	//	2. Can reenter easily, eg on WM_PAINT, because .NET 'wait' functions get/dispatch too many messages.
	//This code is unfinished. Used only for testing.
	TRet _RunElmTask<TRet, TParam>(int timeoutMS, TParam param, Func<TParam, TRet> f, [CallerMemberName] string m_ = null) {
		//using var p1 = perf.local();
		//using var whook = WindowsHook.ThreadGetMessage(k => { print.it(*k.msg); });
		//var hs = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
		//System.Windows.Interop.HwndSourceHook hook = (nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) => { WndUtil.PrintMsg((wnd)hwnd, msg, 0, 0); return default; };
		//hs?.AddHook(hook); using var removeHook = new UsingEndAction(() => hs?.RemoveHook(hook));
		////The windowshook shows that by default .NET gets many posted messages (WM_PAINT, WM_TIMER, WM_USER+, registered, but didn't notice input messages eg WM_MOUSEMOVE), and don't know what it does with them.
		////The hwndsource hook receives some posted messages (WM_PAINT but not others listed above) and (some or all?) sent messages.
		////Sometimes this func reenters on WM_PAINT when treeview wants to get elm properties to display.
		////No messages with NoPumpSynchronizationContext_. But then deadlocks.

		//using var noPump = new NoPumpSynchronizationContext_.Scope(); //fast

		//p1.Next();
		//Debug_.PrintIf(_threadWorking, $"working. Recursive: {_inRET}. Stack: {new StackTrace(true)}", m_: m_);
		Debug_.PrintIf(_threadWorking, $"working. Recursive: {_inRET}.", m_: m_);
		//if (_inRET > 0) return default;
		//if (_threadWorking) return default; //todo: not for all?
		_inRET++;
		try {
			var task = Task.Factory.StartNew(() => {
				//todo: return now if already timed out
				_threadWorking = true;
				try { return f(param); }
				catch (Exception e1) { Debug_.Print(e1); return default; }
				finally { _threadWorking = false; }
			}, default, 0, _threadS);
			bool ok = task.Wait(timeoutMS);
			//p1.Next(); //the Task/Wait code without calling invoke is 40/100 mcs hot/cold. Not too slow.
			if (ok) return task.Result;
		}
		finally { _inRET--; }
		Debug_.Print("timeout", m_: m_);
		return default;
	}
	StaTaskScheduler_ _threadS = new(1); //actually don't need STA, but need a scheduler that can have only 1 thread

	Task _StartElmTask<TParam>(TParam param, Action<TParam> action) {
		return Task.Factory.StartNew(() => {
			try { action(param); }
			catch (Exception e1) { Debug_.Print(e1); return; }
		}, default, 0, _threadS);
	}

	Task<TRet> _StartElmTask<TRet, TParam>(TParam param, Func<TParam, TRet> f) {
		return Task.Factory.StartNew(() => {
			try { return f(param); }
			catch (Exception e1) { Debug_.Print(e1); return default; }
		}, default, 0, _threadS);
	}

	protected override void OnClosed(EventArgs e) {
		_threadS.Dispose();
		base.OnClosed(e);
	}

	bool _threadWorking; int _inRET;
#else
	//This code uses SendMessageTimeout. It pumps less messages.
	//	Deadlocks too, eg when using UIA. Didn't notice reentering.
	//This code is unfinished. Used only for testing.
	TRet _RunElmTask<TRet, TParam>(int timeoutMS, TParam param, Func<TParam, TRet> f, [CallerMemberName] string m_ = null) {
		if (_et.thread == null) {
			var thread = run.thread(_ETThread, sta: false); //must not be STA, else fails non-UIA
			while (_et.w.Is0) { Thread.Sleep(10); if (!thread.IsAlive) return default; } //not ManualResetEvent because it pumps messages and can reenter
			_et.thread = thread;
		}

		//Debug_.PrintIf(_threadWorking, $"working. Recursive: {_inRET}. Stack: {new StackTrace(true)}", m_: m_);
		Debug_.PrintIf(_threadWorking, $"working. Recursive: {_inRET}.", m_: m_);
		//if (_inRET > 0) return default;
		//if (_threadWorking) return default; //todo: not for all?

		try {
			_inRET++;
			TRet r = default;
			_et.action = () => { r = f(param); };
			long t0 = Environment.TickCount64;//todo
			bool ok = _et.w.SendTimeout(timeoutMS, out nint rr, Api.WM_USER);
			_et.action = null;
			if (ok && rr == 1) return r;
			Debug_.Print($"timeout, {timeoutMS}, {Environment.TickCount64-t0}", m_: m_);
			return default;
		}
		finally { _inRET--; }
	}

	(WNDPROC wndProc, wnd w, Thread thread, Action action) _et;

	void _ETThread() {
		_et.w = WndUtil.CreateWindowDWP_(messageOnly: true, _et.wndProc = (w, m, wp, lp) => {
			//WndUtil.PrintMsg(w, m, wp, lp);

			if (m == Api.WM_USER) {
				var a = _et.action; if (a == null) return 0;
				_threadWorking = true;
				//if (keys.isScrollLock) 700.ms();//todo
				try { a(); }
				catch (Exception e1) { Debug_.Print(e1); return 0; }
				finally { _threadWorking = false; }
				return 1;
			}

			return Api.DefWindowProc(w, m, wp, lp);
		});
		while (Api.GetMessage(out var k, default, 0, 0) > 0) Api.DispatchMessage(k);
		Api.DestroyWindow(_et.w);
		_et = default;
		//print.it("thread ended");
	}

	protected override void OnClosed(EventArgs e) {
		if (!_et.w.Is0) _et.w.Post(Api.WM_QUIT);
		base.OnClosed(e);
	}

	bool _threadWorking; int _inRET;
#endif

	wnd _GetWndContainer() => _RunElmTask(1000, _elm, static e => e.WndContainer);

	static bool _RoleIsLinkOrButton(ERole role) => role is ERole.LINK
			or ERole.BUTTON or ERole.BUTTONMENU or ERole.BUTTONDROPDOWN or ERole.BUTTONDROPDOWNGRID
			or ERole.CHECKBOX or ERole.RADIOBUTTON;

	#endregion

	#region info

	TUtil.CommonInfos _commonInfos;
	void _InitInfo() {
		_commonInfos = new TUtil.CommonInfos(_info);

		_info.zText = _dialogInfo;
		_info.ZAddElem(this, _dialogInfo);
		_info.ZTags.AddLinkTag("+jab", _ => Java.EnableDisableJabUI(this));
		_info.ZTags.AddLinkTag("+actTest", _ => { if (_wnd.ActivateL()) _Test(); });
		_info.ZTags.AddLinkTag("+uiaReset", _ => _uiaUserChecked = null);
		TUtil.RegisterLink_DialogHotkey(_info, insertToo: true);

		//note: for Test button etc it's better to use tooltip, not _info.

		_info.InfoCT(_xy,
@"Mouse x y in the UI element. Empty = center.
See <help Au.Types.Coord>Coord<>. Examples:
<mono>10, 10
^10, .9f<>");
		_info.InfoC(_cScroll, @"At first call ScrollTo");
		_info.InfoC(_cControl,
@"Find first matching control and search in it, not in all matching controls.
To change window or/and control name etc, click <b>Window...<> or edit it in the code field.");
		_info.InfoCT(_wait,
@"The wait timeout, seconds.
The function waits max this time interval. On timeout throws exception if <b>Exception<> checked, else returns null. If empty, uses 8e88 (infinite).");
		_info.InfoC(_cException,
@"Throw exception if not found.
If unchecked, returns null.");

		_info.Info(_tree, "Tree view",
@"All UI elements in the window.");

		//SHOULDDO: now no info for HwndHost
		//			_Info(_code, "Code",
		//@"Created code to find the UI element.
		//Some parts can be edited directly.");
	}

	string _dialogInfo = $@"This tool creates code to find <help elm>UI element<> in <help wnd.find>window<>.
1. Move the mouse to a UI element. Press <+hotkey>hotkey<> <b>{App.Settings.delm.hk_capture}<>.
2. Click the Test button to see how the 'find' code works.
3. If need, change some fields or select another element.
4. Click Insert. Click Close, or capture/insert again.
5. If need, edit the code in editor. For example rename variables, delete duplicate wnd.find lines, replace part of window name with *, add code to click the UI element. <help elm>Examples<>.

How to find UI elements that don't have a name or other property with unique constant value? Capture another UI element near it, and use <b>navig<> to get it. Or try <b>skip<>. Or path.

With CapsLock the tool tries to capture a smaller element. Note: in some cases it can be slow, and some apps may crash (rare).

If the hotkey doesn't work when the target window is active, probably its process is admin and this process isn't. Or the process steals the hotkey; try another hotkey (Options -> Hotkeys).";

	//const string c_infoFirefox = @"To make much faster in Firefox, disable its multiprocess feature. More info in <help>elm<> help. Or use Chrome instead.";
	const string c_infoJava = "If there are no UI elements in this window, need to <+jab>enable<> Java Access Bridge etc. More info in <help>elm<> help.";

	partial class _PropPage
	{
		void _InitInfo() {
			var _info = _dlg._info;
			_info.InfoCT(roleA,
@"Role. Prefix <b>web:<> means ""in web page"".
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
Note: states can change. Use only states you need. Remove others from the list.");
			_info.InfoCT(rectA,
@"Raw rectangle. Can be specified width (W) and/or height (H).
Example: {W=100 H=20}");

			_info.InfoCT(alsoA,
@"<help>elmFinder[]<> <i>also<> " + TUtil.CommonInfos.c_alsoParameter);
			_info.InfoCT(skipA,
@"0-based index of matching UI element.
For example, if 1, gets the second matching element.
-1 means any matching intermediate element when used path or <b>navig<>.");
			_info.InfoCT(navigA,
@"Get another UI element using this tree path from the found element.
See <help>elm.Navigate<>. Tool: in the tree view right click that element...
One or several words: <u><i>parent<> <i>child<> <i>first<> <i>last<> <i>next<> <i>previous<><>. Or 2 letters, like <i>ne<>.
Example: pa ne2 ch3. The 2 means 2 times (ne ne). The 3 means 3-rd child; -3 would be 3-rd from end.
Note: ne/pr may skip invisible siblings.
Some elements also support <u><i>up<> <i>down<> <i>left<> <i>right<><>.");

			_info.InfoC(hiddenTooA, "Flag <help>Au.Types.EFFlags<>.HiddenToo.");
			_info.InfoC(reverseA, "Flag <help>Au.Types.EFFlags<>.Reverse (search bottom to top).");
			_info.InfoC(uiaA,
@"Flag <help>Au.Types.EFFlags<>.UIA.
This checkbox may change when capturing, depending on what is usually better in that case, unless you check or uncheck it (<+uiaReset>reset<>).");
			_info.InfoC(notInprocA, "Flag <help>Au.Types.EFFlags<>.NotInProc.");
			_info.InfoC(clientAreaA, "Flag <help>Au.Types.EFFlags<>.ClientArea.");
			_info.InfoC(menuTooA,
@"Flag <help>Au.Types.EFFlags<>.MenuToo.
Check this if the UI element is in a menu and its role is not MENUITEM or MENUPOPUP.");
			_info.InfoCT(notinA,
@"Don't search in UI elements that have these roles. Can make faster.
Example: LIST,TREE,TITLEBAR,SCROLLBAR");
			_info.InfoCT(maxccA, "Don't search in UI elements that have more direct children. Default 10000, min 1, max 1000000.");
			_info.InfoCT(levelA,
@"0-based level of the UI element in the tree of UI elements. Or min and max levels. Default 0 1000.
Relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc).");
			_info.InfoC(inPath,
@"Adds this element to a path like [element1][element2][wantedElement].
Use path when Test finds a similar element but in another ancestor element. Then need to find the correct ancestor element at first.
1. Check this checkbox for the wanted element. 2. In the tree select an ancestor element and check this too. 3. Click Test. It should find the wanted element. If it doesn't, try <b>skip<> -1 for the ancestor element. If the ancestor cannot be uniquely identified (no name etc), you can try to find an element near it in the tree and use <b>navig<> to navigate to it.

This tool remembers edited properties of the element if this checkbox is checked. Also draws a green border in the tree.");
		}
	}
	#endregion
}
