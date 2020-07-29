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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using Au.Types;
using Au.Controls;
using SG = SourceGrid;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Microsoft.Win32;

//SHOULDDO: if checked state, activate window before test. Else different FOCUSED etc.
//SHOULDDO: update window name in code when capturing new AO.

namespace Au.Tools
{
	partial class FormAAcc : ToolForm
	{
		AAcc _acc;
		AWnd _wnd, _con;
		bool _useCon;
		TUtil.CaptureWindowEtcWithHotkey _capt;
		CommonInfos _commonInfos;
		string _wndName;

		public FormAAcc(AAcc acc = null)
		{
			InitializeComponent();

			AWnd.More.SavedRect.Restore(this, Program.Settings.tools_AAcc_wndPos);

			Action<SG.CellContext> f = _grid_ZValueChanged;
			_grid.ZValueChanged += f;
			_grid2.ZValueChanged += f;

			_InitTree();

			_acc = acc; //will be processed in OnLoad
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if(_acc != null) _SetAcc(false);

			_InitInfo();

			_cCapture.Checked = true;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			_cCapture.Checked = false;
			_capt?.Dispose();

			Program.Settings.tools_AAcc_wndPos = new AWnd.More.SavedRect(this).ToString();

			base.OnFormClosing(e);
		}

		void _SetAcc(bool captured)
		{
			//note: don't reorder all the calls.

			AWnd c = _acc.WndContainer, w = c.Window;
			if(w.Is0) return;
			if(captured && w.IsCloaked) {
				//workaround for old pre-Chromium Edge. w is a cloaked windowsuicorecorewindow of other process. There are many such cloaked windows, and AWnd.Find often finds wrong window.
				c = AWnd.FromMouse();
				w = c.Window;
				if(w.Is0) return;
			}

			string wndName = w.NameTL_;
			bool sameWnd = captured && w == _wnd && wndName == _wndName;
			_wndName = wndName;
			//rejected: update window name in code box if changed name of same window. In AWinImage tool too. Currently only AWnd tool does it.

			//if control is in other thread, search in control by default, elso slow because cannot use inproc. Except for known windows.
			bool useCon = c != w && c.ThreadId != w.ThreadId && 0 == c.ClassNameIs(Api.string_IES, "Windows.UI.Core.CoreWindow");

			_SetWndCon(w, c, useCon);

			if(_grid2.RowsCount == 0) _FillGrid2();

			if(!_FillGridTreeCode(captured, sameWnd)) return;

			_bTest.Enabled = true; _bOK.Enabled = true; _bEtc.Enabled = true;
		}

		void _SetWndCon(AWnd wnd, AWnd con, bool useCon = false)
		{
			_wnd = wnd;
			_con = con == wnd ? default : con;
			_useCon = useCon && !_con.Is0;
		}

		bool _FillGridTreeCode(bool captured = false, bool sameWnd = false)
		{
			//APerf.First();
			bool sameTree = !_isTreeInfo && sameWnd && _TrySelectInSameTree();
			//APerf.Next();

			if(!sameTree) _ClearTree();
			if(!_FillGrid(out var p)) return false;
			if(!sameTree) _FillTree(p);
			_UpdateCodeBox();

			if(captured && p.Role == "CLIENT" && _wnd.ClassNameIs("SunAwt*") && !_acc.MiscFlags.Has(AccMiscFlags.Java) && !AVersion.Is32BitOS)
				_SetFormInfo(c_infoJava);

			//APerf.NW();
			return true;
		}

		bool _FillGrid(out AccProperties p)
		{
			var g = _grid;
			g.ZClear();

			if(!_acc.GetProperties("Rnuvdakh@srw", out p)) {
				_propError = "Failed to get AO properties: \r\n" + ALastError.Message;
				g.Invalidate();
				return false;
			}
			_propError = null;

			_noeventGridValueChanged = true;

			bool isWeb = _IsVisibleWebPage(_acc, out var browser, _con.Is0 ? _wnd : _con);
			_isWebIE = isWeb && browser == _BrowserEnum.IE;

			var role = p.Role; if(isWeb) role = "web:" + role;
			_AddIfNotEmpty("role", role, true, false, info: c_infoRole);
			//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
			bool noName = !_AddIfNotEmpty("name", p.Name, true, true, info: "Name.$");
			if(noName) _Add("name", "", info: "Name.$");
			if(_AddIfNotEmpty("uiaid", p.UiaId, noName, true, info: "UIA AutomationId.$")) noName = false;

			//control
			if(!isWeb && !_con.Is0 && !_useCon) {
				string sId = TUtil.GetUsefulControlId(_con, _wnd, out int id) ? id.ToString() : _con.NameWinForms;
				if(sId != null) _Add("id", sId, sId.Length > 0, info: c_infoId);
				_Add("class", TUtil.StripWndClassName(_con.ClassName, true), sId.NE(), info: c_infoClass);
			}

			_AddIfNotEmpty("value", p.Value, false, true, info: "Value.$");
			if(_AddIfNotEmpty("description", p.Description, noName, true, info: "Description.$")) noName = false;
			_AddIfNotEmpty("action", p.DefaultAction, false, true, info: "Default action.$");
			if(_AddIfNotEmpty("key", p.KeyboardShortcut, noName, true, info: "Keyboard shortcut.$")) noName = false;
			if(_AddIfNotEmpty("help", p.Help, noName, true, info: "Help.$")) noName = false;
			foreach(var attr in p.HtmlAttributes as Dictionary<string, string>) {
				string na = attr.Key, va = attr.Value;
				bool check = noName && (na == "id" || na == "name") && va.Length > 0;
				if(check) noName = false;
				_Add("@" + na, TUtil.EscapeWildex(va), check, info: "HTML attribute.$");
			}
			int elem = _acc.SimpleElementId; if(elem != 0) _Add("elem", elem.ToString(), info: c_infoElem);
			_Add("state", p.State.ToString(), info: c_infoState);
			_Add("rect", $"{{W={p.Rect.Width} H={p.Rect.Height}}}", tt: "Rectangle in screen: " + p.Rect.ToString(), info: c_infoRect);

			//CONSIDER: if no name etc, try to get uiaid. Eg winforms control name. Or use prop "wfName=...:".

			_Check2("also", false); _Check2("skip", false); _Check2("navig", false);
			if(isWeb && !_waitAutoCheckedOnce) { _waitAutoCheckedOnce = true; _Check2("wait", true); }

			_Check2(nameof(AFFlags.UIA), _acc.MiscFlags.Has(AccMiscFlags.UIA));

			_noeventGridValueChanged = false;
			g.ZAutoSize(); //tested: suspending layout does not make faster.
			return true;

			bool _AddIfNotEmpty(string name, string value, bool check, bool escape, string tt = null, string info = null)
			{
				if(value.NE()) return false;
				if(escape) value = TUtil.EscapeWildex(value);
				_Add(name, value, check, tt, info);
				return true;
			}

			void _Add(string name, string value, bool check = false, string tt = null, string info = null)
			{
				g.ZAdd(null, name, value, check, tt, info);
			}
		}

		void _FillGrid2()
		{
			var g = _grid2;

			g.ZAdd(null, "also", "o => true", tt: "Lambda that returns true if AAcc o is the wanted AO.", info: c_infoAlso);
			g.ZAdd(null, "skip", "1", tt: "0-based index of matching AO.\nFor example, if 1, gets the second matching AO.");
			g.ZAdd(null, "navig", null, tt: "When found, call AAcc.Navigate to get another AO.", info: c_infoNavig);
			g.ZAdd(null, "wait", "5", tt: c_infoWait);
			g.ZAddCheck("orThrow", "Exception if not found", true, tt: "Checked - throw exception.\nUnchecked - return null.");
			g.ZAddHeaderRow("Search settings");
			g.ZAddCheck(nameof(AFFlags.HiddenToo), "Can be invisible", tt: "Flag AFFlags.HiddenToo.");
			g.ZAddCheck(nameof(AFFlags.Reverse), "Reverse order", tt: "Flag AFFlags.Reverse.\nWalk the object tree from bottom to top.");
			g.ZAddCheck(nameof(AFFlags.UIA), "UI Automation", tt: "Flag AFFlags.UIA.\nUse UI Automation API instead of IAccessible.\nThe capturing tool checks/unchecks this automatically when need.");
			g.ZAddCheck(nameof(AFFlags.NotInProc), "Not in-process", tt: "Flag AFFlags.NotInProc.\nMore info in AFFlags help.");
			g.ZAddCheck(nameof(AFFlags.ClientArea), "Only client area", tt: "Flag AFFlags.ClientArea.\nMore info in AFFlags help.");
			g.ZAddCheck(nameof(AFFlags.MenuToo), "Can be in MENUITEM", tt: "Flag AFFlags.MenuToo.\nCheck this if the AO is in a menu and its role is not MENUITEM or MENUPOPUP.");
			g.ZAdd("notin", "Not in", null, tt: "Don't search in AOs that have these roles. Can make faster.\nExample: LIST,TREE,TASKBAR,SCROLLBAR");
			g.ZAdd(null, "maxcc", null, tt: "Don't search in AOs that have more direct children. Default 10000.");
			g.ZAdd(null, "level", null, tt: "Level of the AO in the object tree. Or min and max levels. Default 0 1000.", info: c_infoLevel);

			g.ZAutoSize();
		}

		void _grid_ZValueChanged(SG.CellContext sender)
		{
			//AOutput.Write(sender.DisplayText);
			//AOutput.Write(_inSetGrid);

			if(_noeventGridValueChanged) return; _noeventGridValueChanged = true;
			var g = sender.Grid as ParamGrid;
			var pos = sender.Position;
			switch(pos.Column) {
			case 0:
				if(g == _grid2 && g.ZGetRowKey(pos.Row) == nameof(AFFlags.UIA)) {
					_uiaUserChecked = _IsChecked2(pos.Row);
					_ShowTreeInfo("Please capture the AO again.\r\nNeed it when changed the 'UI Automation' option.");
					_cCapture.Checked = true;
				}
				break;
			case 1:
				break;
			}
			_noeventGridValueChanged = false;

			_UpdateCodeBox();
		}
		bool _noeventGridValueChanged;

		(string code, string wndVar) _FormatCode(bool forTest = false)
		{
			if(_grid.RowsCount == 0) return default; //cleared on exception

			var (wndCode, wndVar) = _code.ZGetWndFindCode(_wnd, _useCon ? _con : default);

			var b = new StringBuilder();
			b.AppendLine(wndCode);
			if(!forTest) b.Append("var a = ");

			bool orThrow = !forTest && _IsChecked2("orThrow");

			string waitTime = null;
			bool isWait = !forTest && _grid2.ZGetValue("wait", out waitTime, false);
			if(isWait) {
				b.Append("AAcc.Wait(").AppendWaitTime(waitTime, orThrow);
			} else {
				if(orThrow) b.Append('+');
				b.Append("AAcc.Find(");
			}

			b.AppendOtherArg(wndVar, noComma: !isWait);

			_grid.ZGetValueIfExists("role", out var role, false);
			b.AppendStringArg(role);

			bool isName = _grid.ZGetValueIfExists("name", out var name, false);
			if(isName) b.AppendStringArg(name ?? "");

			bool isProp = false;
			var query = (from r in _grid.Rows where _grid.ZIsChecked(r.Index) select r)
				.Concat(from r in _grid2.Rows where r.Index >= 4 && _grid2.ZIsChecked(r.Index) select r);
			foreach(var r in query) {
				var g = r.Grid as ParamGrid;
				int i = r.Index;
				string na = g.ZGetRowKey(i), va = g.ZGetCellText(i, 1);
				switch(na) {
				case "role": case "name": continue;
				case "level": if(va == "0 1000") continue; break;
				case "maxcc": if(va == "10000") continue; break;
				}
				if(va == null) {
					if(g == _grid2) continue;
					switch(na) { case "class": case "elem": case "state": case "rect": continue; }
				}
				if(isProp) b.Append(@" + '\0' + ");
				else {
					isProp = true;
					b.Append(", ");
					if(!isName) b.Append("prop: ");
				}
				int j = b.Length;
				b.Append('\"').Append(na).Append('=');
				if(!va.NE()) {
					if(TUtil.IsVerbatim(va, out int prefixLen)) {
						b.Insert(j, va.Remove(prefixLen++));
						b.Append(va, prefixLen, va.Length - prefixLen);
						continue;
					} else {
						va = va.Escape();
						b.Append(va);
					}
				}
				b.Append('\"');
			}

			b.AppendFlagsFromGrid(typeof(AFFlags), _grid2, (isName && isProp) ? null : "flags");

			if(_grid2.ZGetValue("also", out var also, true)) b.AppendOtherArg(also, "also");
			if(_grid2.ZGetValue("skip", out var skip, true)) b.AppendOtherArg(skip, "skip");
			if(_grid2.ZGetValue("navig", out var navig, true)) b.AppendStringArg(navig, "navig");

			b.Append(");");
			if(!orThrow && !forTest) b.AppendLine().Append("if(a == null) { AOutput.Write(\"not found\"); }");

			var R = b.ToString();

			if(!forTest) _code.ZSetText(R, wndCode.Length);

			return (R, wndVar);
		}

		#region capture

		private void _cCapture_CheckedChanged(object sender, EventArgs e)
		{
			if(_capt == null) _capt = new TUtil.CaptureWindowEtcWithHotkey(this, _cCapture, () => _AccFromMouse(out var a) ? a.Rect : default);
			_capt.StartStop(_cCapture.Checked);
		}

		void _Capture()
		{
			if(!_AccFromMouse(out var acc)) return;
			_acc = acc;
			_SetAcc(true);
			var w = (AWnd)this;
			if(w.IsMinimized) {
				w.ShowNotMinMax();
				w.ActivateLL();
			}
		}

		bool _AccFromMouse(out AAcc a)
		{
			var flags = AXYFlags.PreferLink | AXYFlags.NoThrow;
			if(_grid2.RowsCount > 0) {
				if(_uiaUserChecked && _IsChecked2(nameof(AFFlags.UIA))) flags |= AXYFlags.UIA;
				if(_IsChecked2(nameof(AFFlags.NotInProc))) flags |= AXYFlags.NotInProc;
			}
			a = AAcc.FromMouse(flags);
			return a != null;
		}
		bool _uiaUserChecked; //to prevent capturing with AXYFlags.UIA when the checkbox was checked automatically (not by the user)
		bool _waitAutoCheckedOnce; //if user unchecks, don't check next time

		protected override void WndProc(ref Message m)
		{
			//AWnd w = (AWnd)this; LPARAM wParam = m.WParam, lParam = m.LParam;

			if(_capt != null && _capt.WndProc(ref m, out bool capture)) {
				if(capture) _Capture();
				return;
			}

			base.WndProc(ref m);
		}

		#endregion

		#region wnd, etc

		private void _bEtc_Click(object sender, EventArgs e)
		{
			var m = new AMenu();
			m["Control"] = o => { _useCon = o.MenuItem.Checked && !_con.Is0; _FillGridTreeCode(); };
			m.LastMenuItem.Enabled = !_con.Is0;
			m.LastMenuItem.CheckOnClick = true;
			m.LastMenuItem.Checked = _useCon;
			m["Edit window/control..."] = o => {
				var wPrev = _WndSearchIn;
				bool captCheck = _cCapture.Checked;
				var r = _code.ZShowWndTool(_wnd, _con, !_useCon);
				if(captCheck) _cCapture.Checked = true;
				if(!r.ok) return;
				_SetWndCon(r.wnd, r.con, r.useCon);
				if(_WndSearchIn != wPrev) _FillGridTreeCode();
			};
			m.LastMenuItem.ToolTipText = "Search only in control (if captured), not in whole window.\r\nTo edit window or/and control name etc, click 'Edit window/control...' or edit it in the code field.";
			m.LastMenuItem.Enabled = !_wnd.Is0;
			m.Show(_bEtc);
		}

		#endregion

		#region util

		bool _IsChecked2(int row) => _grid2.ZIsChecked(row);
		bool _IsChecked2(string rowKey) => _grid2.ZIsChecked(rowKey);
		void _Check2(string rowKey, bool check) => _grid2.ZCheck(rowKey, check);

		AWnd _WndSearchIn => _useCon ? _con : _wnd;

		void _UpdateCodeBox() => _FormatCode();

		//Returns true if a is in visible web page in one of 3 browsers.
		//browser - receives nonzero if container's class is like in one of browsers: 1 IES, 2 FF, 3 Chrome. Even if returns false.
		static bool _IsVisibleWebPage(AAcc a, out _BrowserEnum browser, AWnd wContainer = default)
		{
			browser = 0;
			if(wContainer.Is0) wContainer = a.WndContainer;
			browser = (_BrowserEnum)wContainer.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
			if(browser == 0) return false;
			if(browser == _BrowserEnum.IE) return true;
			AAcc ad = null;
			do {
				if(a.RoleInt == AccROLE.DOCUMENT) ad = a;
				a = a.Navigate("pa");
			} while(a != null);
			if(ad == null || ad.IsInvisible) return false;
			return true;
		}
		enum _BrowserEnum { IE = 1, FF, Chrome }

		#endregion

		#region OK, Test

		/// <summary>
		/// When OK clicked, contains C# code.
		/// </summary>
		public override string ZResultCode { get; protected set; }

		private void _bOK_Click(object sender, EventArgs e)
		{
			ZResultCode = _code.Text;
			if(ZResultCode.NE()) this.DialogResult = DialogResult.Cancel;
		}

		private void _bTest_Click(object sender, EventArgs ea)
		{
			var (code, wndVar) = _FormatCode(true); if(code == null) return;
			var r = TUtil.RunTestFindObject(code, wndVar, _WndSearchIn, _bTest, _lSpeed, o => (o as AAcc).Rect);

			if(r.obj is AAcc a && r.speed >= 20_000 && !_IsChecked2(nameof(AFFlags.NotInProc)) && !_IsChecked2(nameof(AFFlags.UIA))) {
				if(!a.MiscFlags.Has(AccMiscFlags.InProc) && _wnd.ClassNameIs("Mozilla*")) {
					//need full path. Run("firefox.exe") fails if firefox is not properly installed.
					string ffInfo = c_infoFirefox, ffPath = _wnd.ProgramPath;
					if(ffPath != null) ffInfo = ffInfo.Replace("firefox.exe", ffPath);
					_SetFormInfo(ffInfo);
				}
			}
		}

		#endregion

		#region tree

		(_AccNode xRoot, _AccNode xSelect) _CreateModel(AWnd w, in AccProperties p, bool skipWINDOW)
		{
			_AccNode xRoot = new _AccNode("root"), xSelect = null;
			var stack = new Stack<_AccNode>(); stack.Push(xRoot);
			int level = 0;

			AFFlags flags = Enum_.AFFlags_Mark | AFFlags.HiddenToo | AFFlags.MenuToo;
			if(_IsChecked2(nameof(AFFlags.UIA))) flags |= AFFlags.UIA;
			var us = (uint)p.State;
			var prop = $"rect={p.Rect.ToString()}\0state=0x{us.ToString("X")},!0x{(~us).ToString("X")}";
			if(skipWINDOW) prop += $"\0 notin=WINDOW";
			//AOutput.Write(prop.Replace('\0', ';'));
			var role = p.Role; if(role.Length == 0) role = null;
			try {
				AAcc.Find(w, role, "**tc " + p.Name, prop, flags, also: o => {
					//var x = new _AccNode(o.Role);
					var x = new _AccNode("a");
					int lev = o.Level;
					if(lev != level) {
						if(lev > level) {
							Debug.Assert(lev - level == 1);
							stack.Push(stack.Peek().LastNode as _AccNode);
						} else {
							while(level-- > lev) stack.Pop();
						}
						level = lev;
					}
					x.a = o;
					if(o.MiscFlags.Has(Enum_.AccMiscFlags_Marked)) {
						//AOutput.Write(o);
						if(xSelect == null) xSelect = x;
					}
					stack.Peek().Add(x);
					return false;
				});
			}
			catch(Exception ex) {
				_ShowTreeInfo("Failed to get object tree.\r\n" + ex.Message);
				return (null, null);
			}
			return (xRoot, xSelect);
		}

		bool _isWebIE; //_FillGrid sets it; then _FillTree uses it.

		//p - _acc properties. This func uses them to find and select the AO in the tree.
		void _FillTree(in AccProperties p)
		{
			//APerf.First();
			var w = _WndSearchIn;
			if(_isWebIE && !_useCon && !_con.Is0) w = _con; //if IE, don't display whole tree. Could be very slow, because cannot use in-proc for web pages (and there may be many tabs with large pages), because its control is in other thread.
			var (xRoot, xSelect) = _CreateModel(w, in p, false);
			if(xRoot == null) return;

			if(xSelect == null && w.IsAlive) {
				//IAccessible of some controls are not connected to the parent.
				//	Noticed this in Office 2003 Word Options dialog and in Dreamweaver.
				//	Also, WndContainer then may get the top-level window. Eg in Word.
				//	Workaround: enum child controls and look for _acc in one them. Then add "class" row if need.
				ADebug.Print("broken IAccessible branch");
				foreach(var c in w.Get.Children(onlyVisible: true)) {
					var m = _CreateModel(c, in p, true);
					if(m.xSelect != null) {
						//m.xRoot.a = AAcc.FromWindow(c, flags: AWFlags.NoThrow);
						//if(m.xRoot.a != null) model.xRoot.Add(m.xRoot);
						//else model.xRoot = m.xRoot;
						xRoot = m.xRoot;
						xSelect = m.xSelect;
						if(_grid.ZFindRow("class") < 0) {
							_grid.ZAdd(null, "class", TUtil.StripWndClassName(c.ClassName, true), true);
							_grid.ZAutoSize(rows: false);
						}
					}
				}
			}

			//AOutput.Write("------");
			//AOutput.Write(xr);

			//APerf.Next();
			_tree.Model = new _AccTree(xRoot);
			//APerf.Next();

			if(xSelect != null) _SelectTreeNode(xSelect);
			//APerf.NW();
		}

		void _SelectTreeNode(_AccNode an)
		{
			var n = _tree.FindNodeByTag(an);
			_tree.Visible = false;
			if(n != null) _tree.EnsureVisible(n);
			_tree.SelectedNode = n;
			_tree.Visible = true;

			//tree control bug: if visible and need to scroll, does not scroll, and when scrolled paints items over items etc.
			//	Workaround 1: temporarily hide and call EnsureVisible.
			//	Workaround 2: BeginUpdate, EnsureVisible, EndUpdate, EnsureVisible. Slightly slower.
		}

		//Tries to find and select _acc in current tree when captured from same window.
		//	Eg maybe the user captured because wants to use navigate; then wants to see both AO.
		//	Usually faster than recreating tree, but in some cases can be slower. Slower when fails to find.
		bool _TrySelectInSameTree()
		{
			//if(AKeys.IsScrollLock) return false;
			int elem = _acc.SimpleElementId;
			var ri = _acc.RoleInt;
			if(!_acc.GetProperties(ri == 0 ? "Rrn" : "rn", out var p)) return false;
			string rs = ri == 0 ? p.Role : null;
			//AOutput.Write(elem, ri, rs, p.Rect, _acc);
			var xr = (_tree.Model as _AccTree).Root;
			foreach(var xe in xr.Descendants()) {
				var n = xe as _AccNode;
				var a = n.a;
				if(a.SimpleElementId != elem) continue;
				if(a.RoleInt != ri) continue;
				if(rs != null && a.Role != rs) continue;
				if(a.Rect != p.Rect) continue;
				if(a.Name != p.Name) continue;
				_SelectTreeNode(n);
				return true;
			}
			ADebug.Print("recreating tree of same window");
			return false;

			//Other ways to compare AAcc:
			//IAccIdentity. Unavailable in web pages.
			//IUIAutomationElement. Very slow ElementFromIAccessible. In Firefox can be 30 ms.
		}

		void _ShowTreeInfo(string text)
		{
			if(_lTreeInfo == null) _tree.Controls.Add(_lTreeInfo = new Label { Dock = DockStyle.Fill });
			_lTreeInfo.Text = text;
			_lTreeInfo.Visible = true;
			_isTreeInfo = true;
		}

		void _ClearTree()
		{
			_tree.Model = null;
			_lTreeInfo?.Hide();
			_isTreeInfo = false;
		}

		bool _isTreeInfo;
		Label _lTreeInfo;
		NodeTextBox _ccName;

		void _InitTree()
		{
			_tree.Indent = 10;
			//_tree.LoadOnDemand = true; //makes slightly faster, but then not so easy to expand to ensure visible (cannot use FindNodeByTag)

			_ccName = new NodeTextBox();
			_tree.NodeControls.Add(_ccName);
			//_ccName.Trimming = StringTrimming.EllipsisCharacter;

			_ccName.ValueNeeded = node => {
				var a = node.Tag as _AccNode;
				//AOutput.Write(a.a);
				return a.DisplayText;
			};
			_ccName.DrawText += _ccName_DrawText;

			_tree.NodeMouseClick += _tree_NodeMouseClick;
			_tree.KeyDown += _tree_KeyDown;
		}

		private void _ccName_DrawText(object sender, DrawEventArgs e)
		{
			var a = e.Node.Tag as _AccNode;
			//AOutput.Write(a.a);
			if(e.Node.IsSelected) {
				//AOutput.Write(e.Text, e.Context.DrawSelection);
				if(e.Context.DrawSelection == DrawSelectionMode.Inactive) e.TextColor = Color.Blue;
			} else {
				if(a.IsInvisible) e.TextColor = Color.Gray;
			}
			if(a.IsException) e.TextColor = Color.Red;
		}

		private void _tree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left && e.ModifierKeys == 0) {
				_SelectFromTree(e.Node);
			}
		}

		private void _tree_KeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Space || e.KeyCode == Keys.Return) && e.Modifiers == 0) {
				_SelectFromTree(_tree.SelectedNode);
			}
		}

		void _SelectFromTree(TreeNodeAdv node)
		{
			if(node == null) return;
			_acc = (node.Tag as _AccNode).a;
			_SetWndCon(_wnd, _acc.WndContainer, _useCon);
			if(!_FillGrid(out var p)) return;
			_UpdateCodeBox();
			TUtil.ShowOsdRect(p.Rect);
		}

		class _AccTree : ITreeModel
		{
			public _AccNode Root;

			public _AccTree(_AccNode root)
			{
				Root = root;
			}

#pragma warning disable 67
			public event EventHandler<TreeModelEventArgs> NodesChanged;
			public event EventHandler<TreeModelEventArgs> NodesInserted;
			public event EventHandler<TreeModelEventArgs> NodesRemoved;
			public event EventHandler<TreePathEventArgs> StructureChanged;
#pragma warning restore 67

			public IEnumerable GetChildren(object nodeTag)
			{
				var x = nodeTag as _AccNode ?? Root;
				return x.Elements();
			}

			public bool IsLeaf(object nodeTag)
			{
				var x = nodeTag as _AccNode;
				return !x.HasElements;
			}
		}

		class _AccNode : XElement
		{
			public _AccNode(string name) : base(name) { }

			public AAcc a;
			string _displayText;

			public string DisplayText {
				get {
					if(_displayText == null) {
						bool isWINDOW = a.RoleInt == AccROLE.WINDOW;
						string props = isWINDOW ? "Rnsw" : "Rns";
						if(!a.GetProperties(props, out var p)) {
							IsException = true;
							return _displayText = "Failed: " + ALastError.Message;
						}

						if(isWINDOW) {
							using(new Util.StringBuilder_(out var b)) {
								b.Append(p.Role).Append("  (").Append(p.WndContainer.ClassName).Append(")");
								if(p.Name.Length > 0) b.Append("  \"").Append(p.Name).Append("\"");
								_displayText = b.ToString();
							}
						} else if(p.Name.Length == 0) _displayText = p.Role;
						else _displayText = p.Role + " \"" + p.Name.Escape(limit: 250) + "\"";

						IsInvisible = a.IsInvisible_(p.State);
					}
					return _displayText;
				}
			}

			public bool IsInvisible { get; private set; }
			public bool IsException { get; private set; }
		}

		#endregion

		#region info

		//Called by OnLoad.
		void _InitInfo()
		{
			_SetFormInfo(null);
			Action<SG.CellContext, string> infoDelegate = (sender, info) => _SetFormInfo(info);
			_grid.ZShowEditInfo += infoDelegate;
			_grid2.ZShowEditInfo += infoDelegate;

			_tree.Paint += (object sender, PaintEventArgs e) => {
				if(_tree.Model == null && !_isTreeInfo) {
					e.Graphics.Clear(this.BackColor); //like grids
					_OnPaintDrawBackText(sender, e, "All AOs in window.");
				}
			};

			_grid.Paint += (sender, e) => { if(_acc == null || _propError != null) _OnPaintDrawBackText(sender, e, _propError ?? "AO properties."); };
			_grid2.Paint += (sender, e) => { if(_acc == null) _OnPaintDrawBackText(sender, e, "Other parameters and search settings."); };

			void _OnPaintDrawBackText(object sender, PaintEventArgs e, string text)
			{
				var c = sender as Control;
				TextRenderer.DrawText(e.Graphics, text, Font, c.ClientRectangle, Color.FromKnownColor(KnownColor.GrayText), TextFormatFlags.WordBreak);
			}

			_commonInfos = new CommonInfos(_info);

			_info.ZTags.AddLinkTag("+resetInfo", _ => _SetFormInfo(null));
			_info.ZTags.AddLinkTag("+jab", _ => Java.EnableDisableJabUI(this));
		}

		string _propError;

		void _SetFormInfo(string info)
		{
			if(info == null) {
				info = c_infoForm;
			} else if(info.Ends('$')) {
				_commonInfos.SetTextWithWildexInfo(info.RemoveSuffix(1));
				return;
			}
			_info.Z.SetText(info);
		}

		const string c_infoForm =
@"Creates code to find <help AAcc.Find>accessible object<> (AO) in <help AWnd.Find>window<>. Your script can click it, etc.
1. Move the mouse to an AO (button, link, etc). Press key <b>F3<>.
2. Click the Test button. It finds and shows the AO and the search time.
3. If need, check/uncheck/edit some fields or select another AO; click Test.
4. Click OK, it inserts C# code in editor. Or copy/paste.
5. In editor add code to use the AO. <help AAcc>Examples<>. If need, rename variables, delete duplicate AWnd.Find lines, replace part of window name with *, etc.

How to find AOs that don't have a name or other property with unique constant value? Capture another AO near it, and use <b>navig<> to get it. Or try <b>skip<>.";
		const string c_infoRole = @"Role. Prefix <b>web:<> means 'in web page'. Can be path, like ROLE1/ROLE2/ROLE3. Path is relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<>).
Read more in <help>AAcc.Find<> help.";
		const string c_infoState = @"State. List of states that the AO must have and/or not have.
Example: CHECKED, !DISABLED
Note: AO state can change. Use only states you need. Remove others from the list.";
		const string c_infoRect = @"Rectangle. Can be specified width (W) and/or height (H).
Example: {W=100 H=20}";
		const string c_infoClass = @"Control class name. Will search only in controls that have it.$";
		const string c_infoId = @"Control id. Will search only in controls that have it.";
		const string c_infoElem = @"Simple element id.
Note: It usually changes when elements before the AO are added or removed. Use it only if really need.";
		const string c_infoAlso = @"<help>AAcc.Find<> <i>also<> lambda.
Can be multiline. For newline use Ctrl+Enter.";
		const string c_infoNavig = @"<b>navig<> is a path to another AO from the found AO in the object tree. One or more of these words: <u><i>parent<> <i>child<> <i>first<> <i>last<> <i>next<> <i>previous<><>. Or 2 letters, like <i>ne<>.
Example: pa ne2 ch3. The 2 means 2 times (ne ne). The 3 means 3-rd child (-3 would be 3-rd from end). More info: <help>AAcc.Navigate<>.";
		const string c_infoWait = @"Wait timeout, seconds.
If unchecked, does not wait. Else if 0 or empty, waits infinitely. Else waits max this time interval; on timeout returns null or throws exception, depending on the 'Exception...' checkbox.";
		const string c_infoLevel = @"<b>level<> - 0-based level of the AO in the object tree. Or min and max levels. Default 0 1000. Relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc).";
		const string c_infoFirefox = @"To make much faster in Firefox, disable its multiprocess feature. More info in <help>AAcc<> help. Or use Chrome instead.
<+resetInfo>X<>";
		const string c_infoJava = @"If there are no AOs in this window, need to <+jab>enable<> Java Access Bridge etc. More info in <help>AAcc<> help.
<+resetInfo>X<>";

		#endregion

		#region misc

		public static class Java
		{
			/// <summary>
			/// Calls <see cref="EnableDisableJab"/>(null) and shows results in task dialog.
			/// </summary>
			/// <param name="owner"></param>
			public static void EnableDisableJabUI(AnyWnd owner)
			{
				var (ok, results) = EnableDisableJab(null);
				if(results != null) ADialog.Show("Results", results, icon: ok ? DIcon.Info : DIcon.Error, owner: owner, flags: DFlags.OwnerCenter);
			}

			/// <summary>
			/// Enables or disables Java Access Bridge for current user.
			/// Returns: ok = false if failed or cancelled. results = null if cancelled.
			/// </summary>
			/// <param name="enable">If null, shows enable/disable dialog.</param>
			public static (bool ok, string results) EnableDisableJab(bool? enable/*, bool allUsers*/)
			{
				if(enable == null) {
					switch(ADialog.ShowList("1 Enable|2 Disable|Cancel", "Java Access Bridge")) {
					case 1: enable = true; break;
					case 2: enable = false; break;
					default: return (false, null);
					}
				}
				bool en = enable.GetValueOrDefault();

				if(!GetJavaPath(out var dir)) return (false, "Cannot find Java " + (AVersion.Is32BitProcess ? "32" : "64") + "-bit. Make sure it is installed.");

				//if(!allUsers) {
				string jabswitch = dir + @"\bin\jabswitch.exe", sout = null;
				if(!AFile.ExistsAsFile(jabswitch)) return (false, "Cannot find jabswitch.exe.");
				try {
					AFile.RunConsole(out sout, jabswitch, en ? "-enable" : "-disable");
					sout = sout?.Trim();
				}
				catch(Exception ex) {
					return (false, ex.ToStringWithoutStack());
				}
				//} else {
				//never mind
				//}

				sout += "\r\nRestart Java apps to apply the new settings.";

				string dll64 = AFolders.SystemX64 + "WindowsAccessBridge-64.dll", dll32 = AFolders.SystemX86 + "WindowsAccessBridge-32.dll";
				if(!AFile.ExistsAsFile(dll64)) sout += "\r\n\r\nWarning: dll not found: " + dll64 + ".  64-bit apps will not be able to use AOs of Java apps. Install 64-bit Java too.";
				if(!AFile.ExistsAsFile(dll32)) sout += "\r\n\r\nNote: dll not found: " + dll32 + ".  32-bit apps will not be able to use AOs of Java apps. Install 32-bit Java too.";

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
			public static bool GetJavaPath(out string path)
			{
				path = null;
				string rk = @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Runtime Environment";
				if(!(Registry.GetValue(rk, "CurrentVersion", null) is string ver)) return false;
				path = Registry.GetValue(rk + @"\" + ver, "JavaHome", null) as string;
				return path != null;
			}
		}

		#endregion
	}
}
