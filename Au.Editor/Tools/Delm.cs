using System.Windows;
using System.Windows.Controls;
using Au.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Linq;

//SHOULDDO: like in QM2, option to capture smallest object at that point.
//SHOULDDO: if checked 'state', activate window before test. Else different FOCUSED etc.
//SHOULDDO: capture image to display in editor.

//TODO: test many elements in many windows with various options.

//note: don't use access keys (_ in control names). It presses the button without Alt, eg when the user forgets to activate editor and starts typing.

//FUTURE: in this dialog and Dwnd: UI for "contains image".
//	Also then need to optimize "elm containing image". Now with image finder in 'also' gets pixels multiple times.

namespace Au.Tools;

class Delm : KDialogWindow
{
	public static void Dialog(POINT? p = null) {
		if (Environment.CurrentManagedThreadId != 1) _Show(false); //cannot simply pass an iaccessible to other thread
		else run.thread(() => _Show(true)); //don't allow the main thread to hang when something is slow when working with UI elements or executing 'also' code

		void _Show(bool dialog) {
			try { //unhandled exception kills process if in nonmain thread
				var d = new Delm(p);
				if (dialog) d.ShowDialog(); else d.Show();
			}
			catch (Exception e1) { print.it(e1); }
		}
	}

	elm _elm;
	wnd _wnd, _con;
	bool _useCon;
	string _wndName;

	KSciInfoBox _info;
	Button _bTest, _bOK, _bInsert, _bDot3;
	ComboBox _cbAction;
	KCheckBox _cCapture, _cControl, _cException;
	TextBox _xy;
	Label _xyLabel;
	KCheckTextBox _wait;
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
			elemA = b.xAddCheckText("elem");
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
		if (p != null) _ElmFromPoint(p.Value, out _elm, capturing: true, ctor: true); //will be processed in OnLoad

		Title = "Find UI element";

		var b = new wpfBuilder(this).WinSize((600, 440..), (600, 480..)).Columns(-1, 0);
		b.R.Add(out _info).Height(60);
		b.R.StartGrid().Columns(0, 0, 0, 0, -1);
		//row 1
		b.R.AddButton(out _bTest, "Test", _ => _Test()).Width(70).Tooltip("Executes the code now (except wait/exception/action) and shows the found UI element");
		b.AddOkCancel(out _bOK, out _, out _, noStackPanel: true);
		b.AddButton(out _bInsert, "Insert", _ => _Insert(false)).Width(70).Tooltip("Insert code and don't close");
		b.Add(out _cCapture, "Capture").Align(y: "C").Tooltip("Enables hotkeys F3 and Ctrl+F3. Shows UI element rectangles when moving the mouse.");
		//row 2
		b.AddButton(out _bDot3, "More ▾", _ => _Dot3()).Align("L"); //align to make smaller than other buttons
		b.Add(out _cbAction).Span(2);
		b.StartDock().Add(out _xyLabel, "x, y", out _xy).End();
		//row 3
		b.R.StartStack(); _topRow3 = b.Panel;
		b.AddButton("Window...", _bWnd_Click).Width(70);
		b.xAddCheck(out _cControl, "Control").Margin("R30");
		_wait = b.xAddCheckText("Wait", "1", check: true); b.Width(48);
		b.xAddCheck(out _cException, "Fail if not found").Checked();
		b.End();

		b.End();
		b.OkApply += _bOK_Click;
		_ActionInit();
		_EnableDisableTopControls(false);

		b.R.AddSeparator(false);
		_commonPage = new(this);
		b.R.Add(out _pageBorder).Margin("LR").Height(_commonPage.panel.Height);
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
		TUtil.OnAnyCheckTextBoxValueChanged<Delm>((d, o) => d._AnyCheckTextBoxValueChanged(o), comboToo: true);
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
		//if control is in other thread, search in control by default, elso slow because cannot use inproc. Except for known windows.
		if (!useCon) useCon = c != w && c.ThreadId != w.ThreadId && 0 == c.ClassNameIs(Api.string_IES, "Windows.UI.Core.CoreWindow");

		_SetWndCon(w, c, useCon);

		if (!_FillPropertiesTreeAndCode(true, sameWnd)) return;

		//bool quickOK = !captured && _Opt.Has(EOptions.QuickOK);
		//if (_Opt.HasAny(EOptions.AutoTest | EOptions.QuickOK)) timerm.after(1, _ => _bTest_Click(quickOK));
		if (_Opt.HasAny(_EOptions.AutoTest | _EOptions.AutoTestAction | _EOptions.QuickInsert))
			timerm.after(1, _ => _Test(captured: true, testAction: _Opt.Has(_EOptions.AutoTestAction)));
	}

	void _SetWndCon(wnd w, wnd con, bool useCon = false) {
		_wnd = w;
		_con = con == w ? default : con;
		_useCon = useCon && !_con.Is0;
		using var nevc = new _NoeventValueChanged(this);
		_cControl.IsChecked = _useCon; _cControl.IsEnabled = !_con.Is0;
	}

	//Called when: 1. _SetElm. 2. The Window button changed window. 3. The Control checkbox checked/unchecked.
	bool _FillPropertiesTreeAndCode(bool setElm = false, bool sameWnd = false) {
		//_nodeCaptured = null;

		//perf.first();
		_TreeItem ti = null;
		bool sameTree = sameWnd && _TrySelectInSameTree(out ti);
		////perf.next();

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
			timerm.after(50, _ => {
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
		bool propOK = _elm.GetProperties("Rnuvdakh@srw", out p) /*&& !keys.isScrollLock*/;
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

		bool isWeb = _IsVisibleWebPage(_elm, out var browser, _con.Is0 ? _wnd : _con);
		_isWebIE = isWeb && browser == _BrowserEnum.IE;

		var role = p.Role; if (isWeb) role = "web:" + role;
		_SetHideIfEmpty(_page.roleA, role, check: true, escape: false);
		//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
		bool noName = !_SetHideIfEmpty(_page.nameA, p.Name, check: true, escape: true, dontHide: true);
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

		int elem = _elm.SimpleElementId; if (elem != 0) _Set(_page.elemA, elem.ToS()); else _page.elemA.Visible = false;
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
			_cCapture_CheckedChanged();
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

		bool isFinder = !forTest && _ActionIsFinder();
		bool orThrow = !(forTest | isFinder) && _cException.IsChecked;
		bool isAction = !forTest && _ActionIsAction();
		bool varAndAction = isAction && (!orThrow || _Opt.Has(_EOptions.ActionAndVar));
		bool isVar = !(forTest | isFinder || (isAction && !varAndAction));

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
		} else {
			b.Append(".Find(");
			if (!forTest && _wait.GetText(out var waitTime, emptyToo: true)) b.AppendWaitTime(waitTime, orThrow); else if (orThrow) b.Append('0');
			b.Append(");");
			if (isVar && !orThrow) b.Append("\r\nif(e == null) { print.it(\"not found\"); }");

			if (isAction) {
				if (!isVar) b.Remove(b.Length - 1, 1);
				b.Append(isVar ? "\r\ne" : "\r\n\t");
				if (!orThrow) b.Append('?');
				b.Append('.').Append(_ActionGetCode()).Append(';');
			}
		}

		var R = b.ToString();

		if (!forTest) _code.ZSetText(R, wndCode.Lenn());

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
		_info.zText = _Opt.HasAny(_EOptions.AutoTest | _EOptions.AutoTestAction | _EOptions.QuickInsert) ? "" : c_dialogInfo; //clear error/info from previous test etc. If with auto-test options, make empty to reduce flickering.

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
		if (!ctor && _page != null) {
			if (_uiaUserChecked && _page.uiaA.IsChecked) flags |= EXYFlags.UIA;
			if (_page.notInprocA.IsChecked) flags |= EXYFlags.NotInProc;
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
				if (!found && ep != null) { //detect when e is not a child of its parent, eg some elements in Chrome
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
		if (_page.uiaA.True()) { browser = 0; return false; }
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

	void _EnableDisableTopControls(bool enable) {
		_bTest.IsEnabled = enable; _bOK.IsEnabled = enable; _bInsert.IsEnabled = enable;
		var vis = enable ? Visibility.Visible : Visibility.Hidden;
		_cbAction.Visibility = vis;
		_ActionSetControlsVisibility();
		_topRow3.Visibility = vis;
	}

	#endregion

	#region OK, Test

	/// <summary>
	/// When OK clicked, contains C# code. Else null.
	/// </summary>
	public string ZResultCode { get; private set; }

	private void _bOK_Click(WBButtonClickArgs e) {
		if (!_Insert(true)) { e.Cancel = true; return; }
	}

	bool _Insert(bool isOK) {
		var code = _code.zText;
		if (code == "") code = null;
		if (isOK) ZResultCode = code;
		if (code == null) return false;
		InsertCode.Statements(code, activate: isOK);
		return true;
	}

	private void _Test(bool captured = false, bool testAction = false, bool actWin = false) {
		if (_page == null) return;

		if (testAction) testAction = _ActionCanTest();

		var (code, wndVar) = _FormatCode(true); if (code == null) return;
		elmFinder.t_navigResult = (true, null, null);
		var r = TUtil.RunTestFindObject(code, wndVar, _WndSearchIn, _info, o => (o as elm).Rect, actWin);
		if (r.obj is not elm elmFound) {
			if (r.speed >= 0) { //else was error
				string s2 = "Try: check <b>Find hidden too<>; check/uncheck/edit other controls.";
				if (_PathIsIntermediate()) s2 += "\r\nTry <b>skip<> -1 to search for next path element in all matching intermediate elements.";
				if (_page.navigA.GetText(out _)) s2 += "\r\nTry <b>skip<> -1 to retry failed <b>navig<>ation with all matching intermediate elements.";
				if (!_wnd.IsActive) {
					if (_page.actionA.GetText(out _)) s2 += "\r\nNote: <b>action<> often is unavailable in inactive window.";
					if (_page.keyA.GetText(out _)) s2 += "\r\nNote: <b>key<> sometimes is unavailable in inactive window.";
					if (_page.stateA.GetText(out _)) s2 += "\r\nNote: <b>state<> often is different in inactive window.";
					s2 += "\r\n<+actTest>Activate window and test (find)<>";
				}
				_info.InfoError("Not found", s2, ",  speed " + r.sSpeed);
			}
			elmFinder.t_navigResult = default;
			return;
		}
		var elmSelected = _page == _commonPage ? _elm : _path[^1].ti.e;
		var elmFoundBN = elmFinder.t_navigResult.after == elmFound ? elmFinder.t_navigResult.before : elmFound; //need elm found before navig
		elmFinder.t_navigResult = default;
		bool bad = elmFoundBN.Rect != elmSelected.Rect || elmFoundBN.Role != elmSelected.Role;
		if (bad) {
			var s2 = "Try: <b>Add to path<> or/and <b>skip<>; check/uncheck/edit other controls.\r\nIf this element cannot be uniquely identified (no name etc), try another element and use <b>navig<>.";
			if (_PathIsIntermediate()) s2 += "\r\nTry <b>skip<> -1 to search for next path element in all matching intermediate elements.";
			_info.InfoError("Found wrong element", s2, ",  speed " + r.sSpeed);
		}
		bool quickInsert = captured && _Opt.Has(_EOptions.QuickInsert);
		if (bad && (testAction || quickInsert)) return;
		//if (!bad && quickOK && r.speed < 1_000_000) {
		//	//timerm.after(1000, _ => _bOK.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)));
		//	return;
		//}

		//this info possibly is obsolete. And nobody would care and turn off Firefox multiprocess.
		//if (r.speed >= 20_000 && !elmFound.MiscFlags.Has(EMiscFlags.InProc) && !_page.notInprocA.IsChecked && !_page.uiaA.IsChecked) {
		//	if (_wnd.ClassNameIs("Mozilla*")) {
		//		//need full path. Run("firefox.exe") fails if firefox is not properly installed.
		//		string ffInfo = c_infoFirefox, ffPath = _wnd.ProgramPath;
		//		if (ffPath != null) ffInfo = ffInfo.Replace("firefox.exe", ffPath);
		//		_info.zText = ffInfo;
		//	}
		//}

		if (testAction) {
			TUtil.RunTestAction(elmFound, _ActionGetCode(), _info);
		}

		if (quickInsert) {
			_quickMenu?.Close();
			var rect = elmFound.Rect;
			timerm.after(100, _ => {
				_quickMenu = new();
				_quickMenu["&Insert"] = _ => _Insert(false);
				_quickMenu["&OK"] = _ => _bOK.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
				_quickMenu.Separator();
				_quickMenu["Test &action"] = _ => _Test(captured: true, testAction: true); //captured=true, to show this menu again, because then the user probably wants to insert
				_quickMenu.Show(MSFlags.Underline | MSFlags.AlignRectBottomTop | MSFlags.AlignCenterH, excludeRect: rect, owner: this);
				_quickMenu = null;
				//_quickMenu.Add("Cancel"); //don't need. The menu will be closed on click anywhere or on next capturing hotkey.
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
			var ti = e.Item as _TreeItem;
			_elm = ti.e;
			_SetWndCon(_wnd, _elm.WndContainer, _useCon);
			if (_PathSetPageWhenTreeItemSelected(ti)) {
				_FormatCode();
				TUtil.ShowOsdRect(ti.e.Rect);
			} else if (_FillProperties(out var p)) {
				_FormatCode();
				TUtil.ShowOsdRect(p.Rect);
			}
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

	(_TreeItem xRoot, _TreeItem xSelect) _CreateTreeModel(wnd w, in EProperties p, bool skipWINDOW) {
		_TreeItem xRoot = new(this), xSelect = null;
		var stack = new Stack<_TreeItem>(); stack.Push(xRoot);
		int level = 0;

		EFFlags flags = Enum_.EFFlags_Mark | EFFlags.HiddenToo | EFFlags.MenuToo;
		if (_page.uiaA.IsChecked) flags |= EFFlags.UIA;
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
		Debug.Assert(_treeRoot == null); Debug.Assert(_path == null); //_ClearTree must be called before

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
	//	Eg maybe the user captured because wants to use navigate; then wants to see both elements.
	//	Usually faster than recreating tree, but in some cases can be slower. Slower when fails to find.
	bool _TrySelectInSameTree(out _TreeItem ti) {
		ti = null;
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
			_SelectTreeItem(ti = v);
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

	void _ActionInit() {
		_cbAction.ItemsSource = s_actions.Select(o => o.name);
		int i = _Opt.Has(_EOptions.ActionRemember)
			? Math.Clamp((int)(_Opt & _EOptions.ActionMask), 0, s_actions.Length - 1)
			: 0;
		_cbAction.SelectedIndex = i;
		_ActionSetControlsVisibility();
		_cbAction.SelectionChanged += (o, _) => {
			int i = _cbAction.SelectedIndex;
			_Opt = (_Opt & ~_EOptions.ActionMask) | ((_EOptions)i & _EOptions.ActionMask);
			_ActionSetControlsVisibility();
		};
	}

	static readonly (string name, string code)[] s_actions = {
		("", null),
		("Invoke", "Invoke()"),
		("WebInvoke", "WebInvoke()"),
		//("JavaInvoke", "JavaInvoke()"), //rare. Usually Invoke is used. Would need UI to enter action name.
		("MouseClick", "MouseClick()"),
		("MouseClick(D)", "MouseClick(button: MButton.DoubleClick)"),
		("MouseClick(R)", "MouseClick(button: MButton.Right)"),
		("MouseMove", "MouseMove()"),
		("VirtualClick", "VirtualClick()"),
		("VirtualClick(D)", "VirtualClick(button: MButton.DoubleClick)"),
		("VirtualClick(R)", "VirtualClick(button: MButton.Right)"),
		("Focus", "Focus()"),
		("Select", "Select()"),
		("ScrollTo", "ScrollTo()"),
		("WaitFor", "WaitFor(0, o => !o.IsDisabled)"),
		("new elmFinder", null),
	};

	static bool _ActionIsMouse(int i) => 0 != s_actions[i].name.Starts(false, "Mouse", "Virtual");
	bool _ActionIsMouse() => _ActionIsMouse(_cbAction.SelectedIndex);

	bool _ActionIsAction() => _cbAction.SelectedIndex is int i && i > 0 && i < s_actions.Length - 1;

	bool _ActionCanTest() => _cbAction.SelectedIndex is int i && i > 0 && i < s_actions.Length - 2;

	bool _ActionIsFinder() => _cbAction.SelectedIndex == s_actions.Length - 1;

	string _ActionGetCode() {
		int ia = _cbAction.SelectedIndex;
		var s = s_actions[ia].code;
		if (_ActionIsMouse(ia)) {
			var xy = _xy.Text;
			if (!xy.NE()) {
				int j = s.IndexOf('(') + 1;
				if (s[j] != ')') xy += ", ";
				s = s.Insert(j, xy);
			}
		}
		return s;
	}

	void _ActionSetControlsVisibility() {
		int i = _cbAction.SelectedIndex;
		var vis = _ActionIsMouse(i) && _cbAction.IsVisible ? Visibility.Visible : Visibility.Hidden;
		_xy.Visibility = vis;
		_xyLabel.Visibility = vis;
		bool vis2 = i < s_actions.Length - 1;
		_wait.Visible = vis2;
		_cException.Visibility = vis2 ? Visibility.Visible : Visibility.Hidden;
	}

	#endregion

	#region misc

	void _Dot3() {
		var m = new popupMenu();
		bool isAction = _page != null && _ActionCanTest();
		m["Test action", disable: !isAction] = _ => _Test(testAction: true);
		m["Activate window and test action", disable: !isAction] = _ => _Test(testAction: true, actWin: true);
		m["Activate window and test find"] = _ => _Test(actWin: true);
		m.Separator();
		m.Add("TOOL SETTINGS", disable: true).FontBold = true;
		var cRA = m.AddCheck("Remember action", _Opt.Has(_EOptions.ActionRemember));
		var cVA = m.AddCheck("Variable && action", _Opt.Has(_EOptions.ActionAndVar));
		var cAT = m.AddCheck("Auto test (find)", _Opt.Has(_EOptions.AutoTest));
		var cATA = m.AddCheck("Auto test action", _Opt.Has(_EOptions.AutoTestAction));
		var cQI = m.AddCheck("Quick insert", _Opt.Has(_EOptions.QuickInsert));
		//var cQO = m.AddCheck("Quick OK", _Opt.Has(EOptions.QuickOK));
		m.Show(owner: this);
		_EOptions f = _Opt;
		f.SetFlag(_EOptions.ActionRemember, cRA.IsChecked);
		f.SetFlag(_EOptions.ActionAndVar, cVA.IsChecked);
		f.SetFlag(_EOptions.AutoTest, cAT.IsChecked);
		f.SetFlag(_EOptions.AutoTestAction, cATA.IsChecked);
		f.SetFlag(_EOptions.QuickInsert, cQI.IsChecked);
		//f.SetFlag(EOptions.QuickOK, cQO.IsChecked);
		var changed = _Opt ^ f;
		_Opt = f;
		if (changed.Has(_EOptions.ActionAndVar)) _FormatCode();
	}

	[Flags]
	enum _EOptions
	{
		ActionMask = 15,
		ActionRemember = 1 << 4,
		ActionAndVar = 1 << 5,
		AutoTest = 1 << 6,
		AutoTestAction = 1 << 7,
		QuickInsert = 1 << 8,
		//QuickOK = 1 << 9,
	}

	static _EOptions _Opt {
		get => (_EOptions)App.Settings.tools_Delm_flags;
		set => App.Settings.tools_Delm_flags = (int)value;
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
			if (!filesystem.exists(jabswitch).isFile) return (false, "Cannot find jabswitch.exe.");
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
		_info.ZAddElem(this, c_dialogInfo);
		_info.ZTags.AddLinkTag("+jab", _ => Java.EnableDisableJabUI(this));
		_info.ZTags.AddLinkTag("+actTest", _ => { if (_wnd.ActivateL()) _Test(); });

		_info.Info(_cbAction, "Action",
@"Call this function when found.
If empty, you'll call functions like <code>e.Invoke(); var s = e.Name</code>
With an elmFinder you can find a UI element multile times.
See also: More -> Variable & action.");
		_info.Info(_xy, "x, y",
@"Mouse x y in the UI element. Empty = center.
See <help Au.Types.Coord>Coord<>. Examples:
<mono>10, 10
^10, 0.9f<>");
		_info.Info(_bDot3, "More",
@"
<i>Test action<> - find the element and call the action function now.
<i>Remember action<> - select the last used action at startup.
<i>Variable & action<> - create code with both variable and action.
<i>Auto test (find)<> - automatically click <b>Test<> when captured.
<i>Auto test action<> - automatically click <b>Test action<> when captured.
<i>Quick insert<> - when captured, test and show a popup menu.");
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

	const string c_dialogInfo =
@"This dialog creates code to find <help elm>UI element<> in <help wnd.find>window<>.
1. Move the mouse to a UI element. Press key <b>F3<> or <b>Ctrl+F3<>.
2. Click the Test button. It finds and shows the UI element.
3. If need, change some fields or select another element.
4. Click OK, it inserts C# code in editor. Or copy/paste.
5. In editor add code to use the UI element. <help elm>Examples<>. If need, rename variables, delete duplicate wnd.find lines, replace part of window name with *, etc.

How to find UI elements that don't have a name or other property with unique constant value? Capture another UI element near it, and use <b>navig<> to get it. Or try <b>skip<>. Or path.

If F3 does not work when the target window is active, probably its process is admin and this process isn't. Ctrl+F3 works, but cannot get UI element properties.";
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
@"Rectangle. Can be specified width (W) and/or height (H).
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
Note: ne/pr may skip invisible siblings.");

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
			_info.InfoCT(notinA,
@"Don't search in UI elements that have these roles. Can make faster.
Example: LIST,TREE,TITLEBAR,SCROLLBAR");
			_info.InfoCT(maxccA, "Don't search in UI elements that have more direct children. Default 10000.");
			_info.InfoCT(levelA,
@"0-based level of the UI element in the tree of UI elements. Or min and max levels. Default 0 1000.
Relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc).");
			_info.InfoC(inPath,
@"Adds this element to a path like [element1][element2][wantedElement].
Use path when Test finds a similar element but in another ancestor element. Then need to find the correct ancestor element at first.
1. Check this checkbox for the wanted element. 2. In the tree select an ancestor element and check this too. 3. Click Test. It should find the wanted element. If it doesn't, try <b>skip<> -1 for the ancestor element. If the ancestor cannot be uniquely identified (no name etc), you can try to find an element near it in the tree and use <b>navig<> to navigate to it.

This dialog remembers edited properties of the element if this checkbox is checked. Also draws a green border in the tree.");
		}
	}
	#endregion
}
