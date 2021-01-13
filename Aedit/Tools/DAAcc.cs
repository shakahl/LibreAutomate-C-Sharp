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
//using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Au.Types;
using Au.Controls;
using Au.Util;
using System.Windows.Input;

//SHOULDDO: when capturing, if fails to get element from point, try UIA. Eg now htmlhelp tree. Maybe also if gets CLIENT.
//	Or like in QM2, option to capture smallest object at that point.
//SHOULDDO: if checked state, activate window before test. Else different FOCUSED etc.

namespace Au.Tools
{
	class DAAcc : KDialogWindow
	{
		AAcc _acc;
		AWnd _wnd, _con;
		bool _useCon;
		string _wndName;

		KSciInfoBox _info;
		Button _bTest, _bOK;
		Label _speed;
		KCheckBox _cCapture;
		ScrollViewer _scroller;
		Border _attr;
		KSciCodeBoxAWnd _code;
		KTreeView _tree;

		KCheckTextBox roleA, nameA, uiaidA, idA, classA, valueA, descriptionA, actionA, keyA, helpA, elemA, stateA, rectA, alsoA, skipA, navigA, waitA, notinA, maxccA, levelA;
		KCheckBox controlC, exceptionA, hiddenTooA, reverseA, uiaA, notInprocA, clientAreaA, menuTooA;

		public DAAcc(AAcc acc = null) {
			Title = "Find accessible object";

			var b = new AWpfBuilder(this).WinSize((600, 440..), (600, 420..)).Columns(-1, 0);
			b.R.Add(out _info).Height(60);
			b.R.StartOkCancel()
				.AddButton(out _bTest, "_Test", _bTest_Click).Width(70..).Disabled().Tooltip("Executes the code now.\nShows rectangle of the found AO.\nIgnores options: wait, Exception.")
				.Add(out _speed).Width(110).Tooltip("Search time (window + AO). Red if not found.")
				.AddOkCancel(out _bOK, out _, out _)
				.End().Align("L")
				.Add(out _cCapture, "_Capture").Align(y: "C").Tooltip("Enables hotkeys F3 and Ctrl+F3. Shows AO rectangles when moving the mouse.");
			_bOK.IsEnabled = false;
			b.OkApply += _bOK_Click;

			//AO properties, other parameters, search settings
			b.Row(184);
			_scroller = b.StartPropertyGrid("L2 T3 R2 B1"); _scroller.Visibility = Visibility.Hidden;
			b.Columns(-2, 0, -1);
			//AO properties
			b.Row(0);
			b.StartGrid(); //left side
			roleA = b.AddCheckText("role");
			nameA = b.AddCheckText("name");
			uiaidA = b.AddCheckText("uiaid");
			idA = b.AddCheckText("id");
			classA = b.AddCheckText("class");
			valueA = b.AddCheckText("value");
			descriptionA = b.AddCheckText("description");
			actionA = b.AddCheckText("action");
			keyA = b.AddCheckText("key");
			helpA = b.AddCheckText("help");
			b.R.Add(out _attr); //HTML attributes will be added with another builder
			elemA = b.AddCheckText("elem");
			stateA = b.AddCheckText("state");
			rectA = b.AddCheckText("rect");
			b.End();
			b.AddSplitterV(span: 4, thickness: 12);
			//other parameters
			b.StartGrid(); //right side
			controlC = b.AddCheck("Control");
			b.PropAddButton("Window/control...", _bWnd_Click);
			alsoA = b.AddCheckText("also", "o => true");
			skipA = b.AddCheckText("skip", "1");
			navigA = b.AddCheckText("navig");
			waitA = b.AddCheckText("wait", "5");
			exceptionA = b.AddCheck("Exception if not found"); b.Checked();
			//search settings
			//b.R.Add<Label>("Search settings").SetHeaderProp();
			hiddenTooA = b.AddCheck("Find hidden too");
			reverseA = b.AddCheck("Reverse order");
			uiaA = b.AddCheck("UI Automation");
			notInprocA = b.AddCheck("Not in-process");
			clientAreaA = b.AddCheck("Only client area");
			menuTooA = b.AddCheck("Can be MENUITEM");
			notinA = b.AddCheckText("Not in");
			maxccA = b.AddCheckText("maxcc");
			levelA = b.AddCheckText("level");
			b.End();

			b.EndPropertyGrid();

			//code
			b.Row(64).AddInBorder(out _code, "B");

			//tree
			b.AddSplitterH(span: -1);
			b.Row(-1).StartGrid().Columns(-1, 0, -1);
			b.Row(-1).Add(out _tree).Border().Margin("T");
			b.End();

			b.End();

			_InitTree();

			_acc = acc; //will be processed in OnLoad

			AWnd.More.SavedRect.Restore(this, App.Settings.tools_AAcc_wndPos, o => App.Settings.tools_AAcc_wndPos = o);
		}

		static DAAcc() {
			TUtil.OnAnyCheckTextBoxValueChanged<DAAcc>((d, o) => d._AnyCheckTextBoxValueChanged(o));
		}

		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);

			if (_acc != null) _SetAcc(false);
			_InitInfo();
			_cCapture.IsChecked = true;
		}

		protected override void OnClosing(CancelEventArgs e) {
			_cCapture.IsChecked = false;

			base.OnClosing(e);
		}

		void _SetAcc(bool captured) {
			AWnd c = _acc.WndContainer, w = c.Window;
			if (w.Is0) return;
			if (captured && w.IsCloaked) {
				//workaround for old pre-Chromium Edge. w is a cloaked windowsuicorecorewindow of other process. There are many such cloaked windows, and AWnd.Find often finds wrong window.
				c = AWnd.FromMouse();
				w = c.Window;
				if (w.Is0) return;
			}

			string wndName = w.NameTL_;
			bool sameWnd = captured && w == _wnd && wndName == _wndName;
			_wndName = wndName;

			bool useCon = _useCon && captured && sameWnd && c == _con;
			//if control is in other thread, search in control by default, elso slow because cannot use inproc. Except for known windows.
			if(!useCon) useCon = c != w && c.ThreadId != w.ThreadId && 0 == c.ClassNameIs(Api.string_IES, "Windows.UI.Core.CoreWindow");

			_SetWndCon(w, c, useCon);

			if (!_FillPropertiesTreeAndCode(captured, sameWnd)) return;

			_bTest.IsEnabled = true; _bOK.IsEnabled = true;
		}

		void _SetWndCon(AWnd wnd, AWnd con, bool useCon = false) {
			_wnd = wnd;
			_con = con == wnd ? default : con;
			_useCon = useCon && !_con.Is0;
			_noeventValueChanged = true;
			controlC.IsChecked = _useCon; controlC.IsEnabled = !_con.Is0;
			_noeventValueChanged = false;
		}

		bool _FillPropertiesTreeAndCode(bool captured = false, bool sameWnd = false) {
			//APerf.First();
			bool sameTree = sameWnd && _TrySelectInSameTree();
			////APerf.Next();

			if (!sameTree) _ClearTree();
			if (!_FillProperties(out var p)) return false;
			if (!sameTree) {
				Mouse.SetCursor(Cursors.Wait);
				_FillTree(p);
				Mouse.SetCursor(Cursors.Arrow);
			}
			_FormatCode();

			if (captured && p.Role == "CLIENT" && _wnd.ClassNameIs("SunAwt*") && !_acc.MiscFlags.Has(AccMiscFlags.Java) && !AVersion.Is32BitOS)
				_info.Text = c_infoJava;

			//APerf.NW();
			return true;
		}

		bool _FillProperties(out AccProperties p) {
			_attr.Child = null;

			if (!_acc.GetProperties("Rnuvdakh@srw", out p)) {
				_info.Text = "Failed to get AO properties: \r\n" + ALastError.Message;
				_scroller.Visibility = Visibility.Hidden;
				return false;
			}
			_scroller.Visibility = Visibility.Visible;

			_noeventValueChanged = true;

			bool isWeb = _IsVisibleWebPage(_acc, out var browser, _con.Is0 ? _wnd : _con);
			_isWebIE = isWeb && browser == _BrowserEnum.IE;

			var role = p.Role; if (isWeb) role = "web:" + role;
			_SetHideIfEmpty(roleA, role, check: true, escape: false);
			//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
			bool noName = !_SetHideIfEmpty(nameA, p.Name, check: true, escape: true, dontHide: true);
			if (_SetHideIfEmpty(uiaidA, p.UiaId, check: noName, escape: true)) noName = false;

			//control
			if (!isWeb && !_con.Is0 && !_useCon) {
				string sId = TUtil.GetUsefulControlId(_con, _wnd, out int id) ? id.ToString() : _con.NameWinforms;
				bool hasId = _SetHideIfEmpty(idA, sId, check: true, escape: false);
				_Set(classA, TUtil.StripWndClassName(_con.ClassName, true), check: !hasId);
			} else {
				idA.Visible = false;
				classA.Visible = false;
			}

			_SetHideIfEmpty(valueA, p.Value, check: false, escape: true);
			if (_SetHideIfEmpty(descriptionA, p.Description, check: noName, escape: true)) noName = false;
			_SetHideIfEmpty(actionA, p.DefaultAction, check: false, escape: true);
			if (_SetHideIfEmpty(keyA, p.KeyboardShortcut, check: noName, escape: true)) noName = false;
			if (_SetHideIfEmpty(helpA, p.Help, check: noName, escape: true)) noName = false;
			if (p.HtmlAttributes.Count > 0) {
				var b = new AWpfBuilder(_attr).Columns((0, ..100), -1).Options(modifyPadding: false, margin: new());
				foreach (var attr in p.HtmlAttributes) {
					string na = attr.Key, va = attr.Value;
					bool check = noName && (na == "id" || na == "name") && va.Length > 0;
					var k = b.AddCheckText("@" + na, TUtil.EscapeWildex(va));
					if (check) { k.c.IsChecked = true; noName = false; }
					var info = TUtil.CommonInfos.AppendWildexInfo(TUtil.CommonInfos.PrependName(na, "HTML attribute."));
					_info.AddElem(k.c, info);
					_info.AddElem(k.t, info);
				}
				b.End();
			}
			int elem = _acc.SimpleElementId; if (elem != 0) _Set(elemA, elem.ToStringInvariant()); else elemA.Visible = false;
			_Set(stateA, p.State.ToString());
			_Set(rectA, $"{{W={p.Rect.Width} H={p.Rect.Height}}}");

			alsoA.c.IsChecked = false;
			skipA.c.IsChecked = false;
			navigA.c.IsChecked = false;
			if (isWeb && !_waitAutoCheckedOnce) waitA.c.IsChecked = _waitAutoCheckedOnce = true;
			uiaA.IsChecked = _acc.MiscFlags.Has(AccMiscFlags.UIA);

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
			_SetWndCon(r.wnd, r.con, r.useCon);
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
						_ttRecapture ??= new ToolTip { PlacementTarget = c, Content = "Please capture the AO again." };
						_ttRecapture.IsOpen = true;
					}
				} else if (source is TextBox t && t.Tag is KCheckTextBox k) {
					k.CheckIfTextNotEmpty();
				}
				_noeventValueChanged = false;

				_FormatCode();
			}
		}
		bool _noeventValueChanged = true;
		ToolTip _ttRecapture;

		(string code, string wndVar) _FormatCode(bool forTest = false) {
			if (!_scroller.IsVisible) return default; //failed to get AO props

			var (wndCode, wndVar) = _code.ZGetWndFindCode(_wnd, _useCon ? _con : default);

			var b = new StringBuilder();
			b.AppendLine(wndCode);
			if (!forTest) b.Append("var a = ");

			bool orThrow = !forTest && exceptionA.IsChecked;

			string waitTime = null;
			bool isWait = !forTest && waitA.GetText(out waitTime, emptyToo: true);
			if (isWait) {
				b.Append("AAcc.Wait(").AppendWaitTime(waitTime, orThrow);
			} else {
				if (orThrow) b.Append('+');
				b.Append("AAcc.Find(");
			}

			b.AppendOtherArg(wndVar, noComma: !isWait);

			roleA.GetText(out var role, emptyToo: true);
			b.AppendStringArg(role);

			bool isName = nameA.GetText(out var name, emptyToo: true);
			if (isName) b.AppendStringArg(name);

			bool isProp = false;
			void _AppendProp(KCheckTextBox k, bool emptyToo = false) {
				if (!k.GetText(out var va, emptyToo)) return;
				if (k == levelA && va == "0 1000") return;
				if (k == maxccA && va == "10000") return;
				if (isProp) b.Append(@" + '\0' + ");
				else {
					isProp = true;
					b.Append(", ");
					if (!isName) b.Append("prop: ");
				}
				int j = b.Length;
				b.Append('\"').Append(k.c.Content as string).Append('=');
				if (!va.NE()) {
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
			if (_attr.Child is Grid g) {
				foreach (FrameworkElement c in g.Children) {
					if (c.Tag is KCheckTextBox k) _AppendProp(k, true);
				}
			}
			_AppendProp(elemA);
			_AppendProp(stateA);
			_AppendProp(rectA);
			_AppendProp(notinA);
			_AppendProp(maxccA);
			_AppendProp(levelA);

			b.AppendFlags((isName && isProp) ? null : "flags", nameof(AFFlags),
				(hiddenTooA, nameof(AFFlags.HiddenToo)),
				(reverseA, nameof(AFFlags.Reverse)),
				(uiaA, nameof(AFFlags.UIA)),
				(notInprocA, nameof(AFFlags.NotInProc)),
				(clientAreaA, nameof(AFFlags.ClientArea)),
				(menuTooA, nameof(AFFlags.MenuToo))
				);

			if (alsoA.GetText(out var also)) b.AppendOtherArg(also, "also");
			if (skipA.GetText(out var skip)) b.AppendOtherArg(skip, "skip");
			if (navigA.GetText(out var navig)) b.AppendStringArg(navig, "navig");

			b.Append(");");
			if (!orThrow && !forTest) b.AppendLine().Append("if(a == null) { AOutput.Write(\"not found\"); }");

			var R = b.ToString();

			if (!forTest) _code.ZSetText(R, wndCode.Length);

			return (R, wndVar);
		}

		#region capture

		TUtil.CaptureWindowEtcWithHotkey _capt;

		void _cCapture_CheckedChanged() {
			_capt ??= new TUtil.CaptureWindowEtcWithHotkey(_cCapture, _Capture, () => _AccFromMouse(out var a) ? a.Rect : default);
			_capt.Capturing = _cCapture.IsChecked;
		}

		void _Capture() {
			if (_ttRecapture != null) {
				_ttRecapture.IsOpen = false;
				_ttRecapture = null;
			}
			_info.Text = c_dialogInfo; //clear error info

			if (!_AccFromMouse(out var acc)) {
				if (AWnd.FromMouse().IsUacAccessDenied) _info.Text = "<c red>Failed to get AO. The target process is admin and this process isn't.<>";
				return;
			}
			_acc = acc;
			_SetAcc(true);
			var w = this.Hwnd();
			if (w.IsMinimized) {
				w.ShowNotMinMax();
				w.ActivateLL();
			}
		}

		bool _AccFromMouse(out AAcc a) {
			var flags = AXYFlags.PreferLink | AXYFlags.NoThrow;
			if (_uiaUserChecked && uiaA.IsChecked) flags |= AXYFlags.UIA;
			if (notInprocA.IsChecked) flags |= AXYFlags.NotInProc;
			a = AAcc.FromMouse(flags);
			return a != null;
		}
		bool _uiaUserChecked; //to prevent capturing with AXYFlags.UIA when the checkbox was checked automatically (not by the user)
		bool _waitAutoCheckedOnce; //if user unchecks, don't check next time

		#endregion

		#region util

		AWnd _WndSearchIn => _useCon ? _con : _wnd;

		//Returns true if a is in visible web page in one of 3 browsers.
		//browser - receives nonzero if container's class is like in one of browsers: 1 IES, 2 FF, 3 Chrome. Even if returns false.
		static bool _IsVisibleWebPage(AAcc a, out _BrowserEnum browser, AWnd wContainer = default) {
			if (wContainer.Is0) wContainer = a.WndContainer;
			browser = (_BrowserEnum)wContainer.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
			if (browser == 0) return false;
			if (browser == _BrowserEnum.IE) return true;
			AAcc ad = null;
			do {
				if (a.RoleInt == AccROLE.DOCUMENT) ad = a;
				a = a.Navigate("pa");
			} while (a != null);
			if (ad == null || ad.IsInvisible) return false;
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
			ZResultCode = _code.Text;
			if (ZResultCode.NE()) { ZResultCode = null; e.Cancel = true; return; }

			InsertCode.Statements(ZResultCode);
		}

		private void _bTest_Click(WBButtonClickArgs ea) {
			var (code, wndVar) = _FormatCode(true); if (code == null) return;
			var r = TUtil.RunTestFindObject(code, wndVar, _WndSearchIn, _bTest, _speed, o => (o as AAcc).Rect);

			if (r.obj is AAcc a && r.speed >= 20_000 && !notInprocA.IsChecked && !uiaA.IsChecked) {
				if (!a.MiscFlags.Has(AccMiscFlags.InProc) && _wnd.ClassNameIs("Mozilla*")) {
					//need full path. Run("firefox.exe") fails if firefox is not properly installed.
					string ffInfo = c_infoFirefox, ffPath = _wnd.ProgramPath;
					if (ffPath != null) ffInfo = ffInfo.Replace("firefox.exe", ffPath);
					_info.Text = ffInfo;
				}
			}
		}

		#endregion

		#region tree

		_TreeItem _treeRoot;
		bool _isWebIE; //_FillProperties sets it; then _FillTree uses it.

		void _InitTree() {
			_tree.ImageCache = App.ImageCache;
			_tree.SingleClickActivate = true;
			_tree.ItemActivated += (_, e) => {
				_acc = (e.Item as _TreeItem).a;
				_SetWndCon(_wnd, _acc.WndContainer, _useCon);
				if (!_FillProperties(out var p)) return;
				_FormatCode();
				TUtil.ShowOsdRect(p.Rect);
			};
		}

		void _ClearTree() {
			_tree.SetItems(null, false);
			_treeRoot = null;
		}

		(_TreeItem xRoot, _TreeItem xSelect) _CreateTreeModel(AWnd w, in AccProperties p, bool skipWINDOW) {
			_TreeItem xRoot = new(), xSelect = null;
			var stack = new Stack<_TreeItem>(); stack.Push(xRoot);
			int level = 0;

			AFFlags flags = Enum_.AFFlags_Mark | AFFlags.HiddenToo | AFFlags.MenuToo;
			if (uiaA.IsChecked) flags |= AFFlags.UIA;
			var us = (uint)p.State;
			var prop = $"rect={p.Rect}\0state=0x{us:X},!0x{~us:X}";
			if (skipWINDOW) prop += $"\0 notin=WINDOW";
			//AOutput.Write(prop.Replace('\0', ';'));
			var role = p.Role; if (role.Length == 0) role = null;
			try {
				AAcc.Find(w, role, "**tc " + p.Name, prop, flags, also: o => {
					//var x = new _AccNode(o.Role);
					_TreeItem x = new();
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
					x.a = o;
					if (o.MiscFlags.Has(Enum_.AccMiscFlags_Marked)) {
						//AOutput.Write(o);
						if (xSelect == null) xSelect = x;
					}
					stack.Peek().AddChild(x);
					return false;
				});
			}
			catch (Exception ex) {
				_info.Text = "<c red>Failed to get AO tree.<>\r\n" + ex.Message;
				return (null, null);
			}
			return (xRoot, xSelect);
		}

		void _FillTree(in AccProperties p) {
			_treeRoot = null;

			var w = _WndSearchIn;
			if (_isWebIE && !_useCon && !_con.Is0) w = _con; //if IE, don't display whole tree. Could be very slow, because cannot use in-proc for web pages (and there may be many tabs with large pages), because its control is in other thread.
			var (xRoot, xSelect) = _CreateTreeModel(w, in p, false);
			if (xRoot == null) return;

			if (xSelect == null && w.IsAlive) {
				//IAccessible of some controls are not connected to the parent.
				//	Noticed this in Office 2003 Word Options dialog and in Dreamweaver.
				//	Also, WndContainer then may get the top-level window. Eg in Word.
				//	Workaround: enum child controls and look for _acc in one them. Then add "class" row if need.
				ADebug.Print("broken IAccessible branch");
				foreach (var c in w.Get.Children(onlyVisible: true)) {
					var m = _CreateTreeModel(c, in p, true);
					if (m.xSelect != null) {
						//m.xRoot.a = AAcc.FromWindow(c, flags: AWFlags.NoThrow);
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

			_tree.SetItems(xRoot.Children(), false);
			_treeRoot = xRoot;
			if (xSelect != null) _SelectTreeItem(xSelect);
		}

		void _SelectTreeItem(_TreeItem x) {
			_tree.EnsureVisible(x);
			_tree.SelectSingle(x, true);
		}

		//Tries to find and select _acc in current tree when captured from same window.
		//	Eg maybe the user captured because wants to use navigate; then wants to see both AO.
		//	Usually faster than recreating tree, but in some cases can be slower. Slower when fails to find.
		bool _TrySelectInSameTree() {
			if (_treeRoot == null) return false;
			//if(AKeys.IsScrollLock) return false;
			int elem = _acc.SimpleElementId;
			var ri = _acc.RoleInt;
			if (!_acc.GetProperties(ri == 0 ? "Rrn" : "rn", out var p)) return false;
			string rs = ri == 0 ? p.Role : null;
			//AOutput.Write(elem, ri, rs, p.Rect, _acc);
			foreach (var v in _treeRoot.Descendants()) {
				var a = v.a;
				if (a.SimpleElementId != elem) continue;
				if (a.RoleInt != ri) continue;
				if (rs != null && a.Role != rs) continue;
				if (a.Rect != p.Rect) continue;
				if (a.Name != p.Name) continue;
				_SelectTreeItem(v);
				return true;
			}
			ADebug.Print("recreating tree of same window");
			return false;

			//Other ways to compare AAcc:
			//IAccIdentity. Unavailable in web pages.
			//IUIAutomationElement. Very slow ElementFromIAccessible. In Firefox can be 30 ms.
		}

		class _TreeItem : ATreeBase<_TreeItem>, ITreeViewItem
		{
			public AAcc a;
			string _displayText;
			bool _isExpanded;
			bool _isFailed;
			bool _isInvisible;

			#region ITreeViewItem

			string ITreeViewItem.DisplayText {
				get {
					if (_displayText == null) {
						bool isWINDOW = a.RoleInt == AccROLE.WINDOW;
						string props = isWINDOW ? "Rnsw" : "Rns";
						if (!a.GetProperties(props, out var p)) {
							_isFailed = true;
							return _displayText = "Failed: " + ALastError.Message;
						}

						if (isWINDOW) {
							using (new StringBuilder_(out var b)) {
								b.Append(p.Role).Append("  (").Append(p.WndContainer.ClassName).Append(")");
								if (p.Name.Length > 0) b.Append("  \"").Append(p.Name).Append('\"');
								_displayText = b.ToString();
							}
						} else if (p.Name.Length == 0) _displayText = p.Role;
						else _displayText = p.Role + " \"" + p.Name.Escape(limit: 250) + "\"";

						_isInvisible = a.IsInvisible_(p.State);
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

			#endregion
		}

		#endregion

		#region misc

		public static class Java
		{
			/// <summary>
			/// Calls <see cref="EnableDisableJab"/>(null) and shows results in task dialog.
			/// </summary>
			/// <param name="owner"></param>
			public static void EnableDisableJabUI(AnyWnd owner) {
				var (ok, results) = EnableDisableJab(null);
				if (results != null) ADialog.Show("Results", results, icon: ok ? DIcon.Info : DIcon.Error, owner: owner, flags: DFlags.OwnerCenter);
			}

			/// <summary>
			/// Enables or disables Java Access Bridge for current user.
			/// Returns: ok = false if failed or cancelled. results = null if cancelled.
			/// </summary>
			/// <param name="enable">If null, shows enable/disable dialog.</param>
			public static (bool ok, string results) EnableDisableJab(bool? enable/*, bool allUsers*/) {
				if (enable == null) {
					switch (ADialog.ShowList("1 Enable|2 Disable|Cancel", "Java Access Bridge")) {
					case 1: enable = true; break;
					case 2: enable = false; break;
					default: return (false, null);
					}
				}
				bool en = enable.GetValueOrDefault();

				if (!GetJavaPath(out var dir)) return (false, "Cannot find Java " + (AVersion.Is32BitProcess ? "32" : "64") + "-bit. Make sure it is installed.");

				//if(!allUsers) {
				string jabswitch = dir + @"\bin\jabswitch.exe", sout = null;
				if (!AFile.ExistsAsFile(jabswitch)) return (false, "Cannot find jabswitch.exe.");
				try {
					AFile.RunConsole(out sout, jabswitch, en ? "-enable" : "-disable");
					sout = sout?.Trim();
				}
				catch (Exception ex) {
					return (false, ex.ToStringWithoutStack());
				}
				//} else {
				//never mind
				//}

				sout += "\r\nRestart Java apps to apply the new settings.";

				string dll64 = AFolders.SystemX64 + "WindowsAccessBridge-64.dll", dll32 = AFolders.SystemX86 + "WindowsAccessBridge-32.dll";
				if (!AFile.ExistsAsFile(dll64)) sout += "\r\n\r\nWarning: dll not found: " + dll64 + ".  64-bit apps will not be able to use AOs of Java apps. Install 64-bit Java too.";
				if (!AFile.ExistsAsFile(dll32)) sout += "\r\n\r\nNote: dll not found: " + dll32 + ".  32-bit apps will not be able to use AOs of Java apps. Install 32-bit Java too.";

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

			_info.Text = c_dialogInfo;
			_info.AddElem(this, c_dialogInfo);
			_info.ZTags.AddLinkTag("+jab", _ => Java.EnableDisableJabUI(this));

			_info.InfoCT(roleA,
@"Role. Prefix <b>web:<> means 'in web page'. Can be path, like ROLE1/ROLE2/ROLE3. Path is relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<>).
Read more in <help>AAcc.Find<> help.");
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
@"Simple element id.
Note: It usually changes when elements before the AO are added or removed. Use it only if really need.");
			_info.InfoCT(stateA,
@"State. List of <help Au.Types.AccSTATE>states<> that the AO must have and/or not have.
Example: CHECKED, !DISABLED
Note: AO state can change. Use only states you need. Remove others from the list.");
			_info.InfoCT(rectA,
@"Rectangle. Can be specified width (W) and/or height (H).
Example: {W=100 H=20}");

			_info.InfoC(controlC,
@"Find first matching control and search in it, not in all matching controls.
To change window or/and control name etc, click 'Window/control...' or edit it in the code field.");
			_info.InfoCT(alsoA,
@"<help>AAcc.Find<> <i>also<> lambda.
Can be multiline.");
			_info.InfoCT(skipA,
@"0-based index of matching AO.
For example, if 1, gets the second matching AO.");
			_info.InfoCT(navigA,
@"Get another AO using this path from the found AO. See <help>AAcc.Navigate<>.
One or several words: <u><i>parent<> <i>child<> <i>first<> <i>last<> <i>next<> <i>previous<><>. Or 2 letters, like <i>ne<>.
Example: pa ne2 ch3. The 2 means 2 times (ne ne). The 3 means 3-rd child; -3 would be 3-rd from end.");
			_info.InfoCT(waitA,
@"Wait timeout, seconds.
If unchecked, does not wait. Else if 0 or empty, waits infinitely. Else waits max this time interval; on timeout returns null or throws exception, depending on the 'Exception...' checkbox.");
			_info.InfoCT(notinA,
@"Don't search in AOs that have these roles. Can make faster.
Example: LIST,TREE,TITLEBAR,SCROLLBAR");
			_info.InfoCT(maxccA, "Don't search in AOs that have more direct children. Default 10000.");
			_info.InfoCT(levelA,
@"0-based level of the AO in the AO tree. Or min and max levels. Default 0 1000.
Relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc).");

			_info.InfoC(exceptionA,
@"Throw exception if not found.
If unchecked, returns null.");

			_info.InfoC(hiddenTooA, "Flag <help>Au.Types.AFFlags<>.HiddenToo.");
			_info.InfoC(reverseA, "Flag <help>Au.Types.AFFlags<>.Reverse (search bottom to top).");
			_info.InfoC(uiaA,
@"Flag <help>Au.Types.AFFlags<>.UIA (use UI Automation API instead of IAccessible).
The capturing tool checks/unchecks this automatically when need.");
			_info.InfoC(notInprocA, "Flag <help>Au.Types.AFFlags<>.NotInProc.");
			_info.InfoC(clientAreaA, "Flag <help>Au.Types.AFFlags<>.ClientArea.");
			_info.InfoC(menuTooA,
@"Flag <help>Au.Types.AFFlags<>.MenuToo.
Check this if the AO is in a menu and its role is not MENUITEM or MENUPOPUP.");

			_info.Info(_tree, "Tree view",
@"All AOs in the window.
Useful when creating <b>navig<> string.");

			//SHOULDDO: now no info for HwndHost
			//			_Info(_code, "Code",
			//@"Created code to find the AO.
			//Some parts can be edited directly.");
		}

		const string c_dialogInfo =
@"Creates code to find <help AAcc.Find>accessible object<> (AO) in <help AWnd.Find>window<>. Your script can click it, etc.
1. Move the mouse to an AO (button, link, etc). Press key <b>F3<> or <b>Ctrl+F3<>.
2. Click the Test button. It finds and shows the AO and the search time.
3. If need, check/uncheck/edit some fields or select another AO; click Test.
4. Click OK, it inserts C# code in editor. Or copy/paste.
5. In editor add code to use the AO. <help AAcc>Examples<>. If need, rename variables, delete duplicate AWnd.Find lines, replace part of window name with *, etc.

How to find AOs that don't have a name or other property with unique constant value? Capture another AO near it, and use <b>navig<> to get it. Or try <b>skip<>.

If F3 does not work when the target window is active, probably its process is admin and this process isn't. Ctrl+F3 works, but cannot get AO properties.";
		const string c_infoFirefox = @"To make much faster in Firefox, disable its multiprocess feature. More info in <help>AAcc<> help. Or use Chrome instead.";
		const string c_infoJava = @"If there are no AOs in this window, need to <+jab>enable<> Java Access Bridge etc. More info in <help>AAcc<> help.";

		#endregion
	}
}
