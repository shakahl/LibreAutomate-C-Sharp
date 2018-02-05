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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Controls;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections;

using SG = SourceGrid;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

//TODO: option to choose hotkey. Because eg F3 can be used for triggers, and we cannot detect it.
//TODO: Java
//TODO: Firefox multiprocess

namespace Au.Tools
{
	public partial class Form_Acc :Form_
	{
		Acc _acc;
		Wnd _wnd;
		string _sWndVar;
		Timer_ _timer;
		OnScreenRect _osr;

		public Form_Acc(Acc acc = null)
		{
			InitializeComponent();
			_SetGridEvents();
			_InitTree();

			_sWndVar = "w";
			_tWnd.TextChanged += _tWnd_TextChanged;

			//_TestFonts();

			_acc = acc; //will be processed in OnLoad
		}

		//protected override void OnPaint(PaintEventArgs e) //TODO
		//{
		//	base.OnPaint(e);
		//	Perf.NW();
		//}

		//#if DEBUG
		//	void _TestFonts()
		//	{
		//		//var f = new Font("Segoe UI", 9);
		//		//var f = new Font("MS Sans Serif", 9);
		//		var f = new Font("Courier New", 9);
		//		//var f = new Font("Verdana", 9);
		//		//var f = new Font("Tahoma", 9);
		//		//var f = new Font("Segoe UI", 14);
		//		//var f = new Font("MS Sans Serif", 14);
		//		_grid.Font = f;

		//		using(var grap = _grid.CreateGraphics())
		//			Print(TextRenderer.MeasureText(grap, "l", f));
		//	}
		//#endif

		void _SetAcc(bool captured)
		{
			//note: don't reorder all the calls.

			_wnd = _acc.WndTopLevel;
			if(_wnd.Is0 && captured) _wnd = Wnd.FromMouse(WXYFlags.NeedWindow);

			if(_grid2.RowsCount == 0) {
				_FillGrid2();
				_grid2.ZAutoSizeRows();
				_grid2.ZAutoSizeColumns();
			}

			_tree.Model = null; //clear grid. Normally not necessary, but eg may fill slowly.
			if(!_SetAccGrids(out var p)) return;

			if(!_wnd.Is0) {
				_SetWindow(); //set the Wnd.Find textbox
				_FillTree(p);
			}
		}

		bool _SetAccGrids(out AccProperties p)
		{
			//fill _grid and check/uncheck some rows in other grids
			_grid.Clear();
			if(!_FillGrid(out p)) return false;

			_grid.ZAutoSizeRows();
			_grid.ZAutoSizeColumns();
			//tested: suspeding layout does not make faster.

			_OnGridChanged(); //set the Acc.Find readonly textbox
			return true;
		}

		void _SetWindow()
		{
			var b = new StringBuilder("var ");
			b.Append(_sWndVar ?? "w");
			b.Append(" = +Wnd.Find(");

			var s = _wnd.Name;
			_AppendString(b, s, noComma: true);

			s = _wnd.ClassName;
			if(s == null) {
				_tWnd.Text = "invalid window handle";
				return;
			}
			_AppendString(b, _ClassNameEscapeWildcardEtc(s));

			if(!_wnd.IsVisibleEx) _AppendOther(b, "WFFlags.HiddenToo", "flags");

			b.Append(");");
			_noeventWndTextChanged = true;
			_tWnd.Text = b.ToString();
			_noeventWndTextChanged = false;
		}

		//When the user edits the Wnd.Find textbox, updates the window variable in the Acc.Find/Wait textbox.
		private void _tWnd_TextChanged(object sender, EventArgs e)
		{
			if(_noeventWndTextChanged) return;
			var s = (sender as TextBox).Text;
			if(Empty(s)) return;
			if(s.RegexIndexOf_(@"^(?:Wnd|var) +(\w+) *=", out string w, 1) < 0) return;
			if(w != _sWndVar) {
				_sWndVar = w;
				_OnGridChanged();
			}
		}
		bool _noeventWndTextChanged;

		bool _FillGrid(out AccProperties p)
		{
			if(!_acc.GetProperties("Rnuvdakh@srw", out p)) {
				_tAcc.Text = "Failed: " + Native.GetErrorMessage();
				return false;
			}

			_noeventGridValueChanged = true;

			Wnd w = p.WndContainer;
			bool isWeb = _IsVisibleWebPage(_acc, out int browser, w);

			var role = p.Role; if(isWeb) role = "web:" + role;
			_AddIfNotEmpty("role", role, true, false, tt: "Role.\nTo search in web page, use prefix web:, firefox: or chrome:.\nCan be path, like ROLE1/ROLE2/ROLE3.\nPath is relative to the window, control (if used class or id) or web page (prefix web: etc).\nRead more in Acc.Find help.");
			//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
			bool noName = !_AddIfNotEmpty("name", p.Name, true, true, tt: _MakeTooltipForWildex("Name."));
			if(_AddIfNotEmpty("uiaid", p.UiaId, noName, true, tt: _MakeTooltipForWildex("UiaId."))) noName = false;

			//control
			if(!isWeb && w != _wnd) {
				int id = w.ControlId;
				bool isId = (id > 0 && id < 0x10000 && id != (int)w.Handle && _wnd.ChildAll("**id:" + id).Length == 1);
				if(isId) _Add("id", id.ToString(), true, tt: "Control id.\nWill search only in controls that have this id.");
				_Add("class", _ClassNameEscapeWildcardEtc(w.ClassName), !isId, tt: "Control class name.\nWill search only in controls that have this class name.");
			}

			_AddIfNotEmpty("value", p.Value, false, true, tt: _MakeTooltipForWildex("Value."));
			if(_AddIfNotEmpty("description", p.Description, noName, true, tt: _MakeTooltipForWildex("Description."))) noName = false;
			_AddIfNotEmpty("action", p.DefaultAction, false, true, tt: _MakeTooltipForWildex("DefaultAction."));
			if(_AddIfNotEmpty("key", p.KeyboardShortcut, noName, true, tt: _MakeTooltipForWildex("KeyboardShortcut."))) noName = false;
			if(_AddIfNotEmpty("help", p.Help, noName, true, tt: _MakeTooltipForWildex("Help."))) noName = false;
			foreach(var attr in p.HtmlAttributes as Dictionary<string, string>) {
				string na = attr.Key, va = attr.Value;
				bool check = noName && (na == "id" || na == "name") && va.Length > 0;
				if(check) noName = false;
				_Add("@" + na, _EscapeWildex(va), check, tt: _MakeTooltipForWildex("HTML attribute."));
			}
			int elem = _acc.SimpleElementId; if(elem != 0) _Add("elem", elem.ToString(), tt: "SimpleElementId.");
			_Add("state", p.State.ToString(), tt: "State.\nList of states that the AO must have and/or not have.\nExample: CHECKED, FOCUSABLE, !DISABLED");
			_Add("rect", p.Rect.ToString(), tt: "Rect.\nCan be specified left, top, width and/or height.\nExample: {W=100 H=20}");

			//CONSIDER: if no name etc, try to get uiaid. Eg winforms control name. Or use role prefix "wfName=...:".

			_Check2("also", false); _Check2("skip", false); _Check2("navig", false);
			if(isWeb && !_waitAutoCheckedOnce) { _waitAutoCheckedOnce = true; _Check2("wait", true); }

			_Check2(nameof(AFFlags.UIA), _acc.MiscFlags.Has_(AccMiscFlags.UIA));

			_noeventGridValueChanged = false;


			//int r = _grid.ZAddRequired("test", "test");
			//var ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string), null, true, null, false, null, new RegexTypeEditor());
			//Print(ed != null);
			//_grid[r, 1] = new SourceGrid.Cells.Cell(@"^test$");
			//_grid[r, 1].Editor = ed;


			return true;

			bool _AddIfNotEmpty(string name, string s, bool check, bool escape, string tt = null)
			{
				if(Empty(s)) return false;
				if(escape) s = _EscapeWildex(s);
				_Add(name, s, check, tt);
				return true;
			}

			void _Add(string name, string value, bool check = false, string tt = null)
			{
				//tests
				//name = value = "Tjgtli";
				//value = value + "\r\nline2";
				//if(name == "role") { _grid.ZAddRequired(null, name, value); return; }

				_grid.ZAddOptional(null, name, value, check, tt: tt);
			}

			string _EscapeWildex(string s)
			{
				if(Wildex.HasWildcards(s)) s = "**t|" + s;
				return s;
			}

			string _MakeTooltipForWildex(string tt)
			{
				return tt + "\nWildcard expression. By default case-insensitive. Examples:\nwhole text\n*ends with\nstarts with*\n**t|non-wildcard text\n**c|match case\n**pc|regular expression (PCRE), match case\n**n|not this\n**m|this[]or this[]**r|or this regex[]**n|and not this";
			}
		}

		//Returns true if a is in visible web page in one of 3 browsers.
		//browser - receives nonzero if container's class is like in one of browsers: 1 IES, 2 FF, 3 Chrome. Even if returns false.
		static bool _IsVisibleWebPage(Acc a, out int browser, Wnd wContainer = default)
		{
			browser = 0;
			if(wContainer.Is0) wContainer = a.WndContainer;
			browser = wContainer.ClassNameIs(Api.string_IES, "Mozilla*", "Chrome*");
			if(browser == 0) return false;
			if(browser == 1) return true;
			Acc ad = null;
			do {
				if(a.RoleInt == AccROLE.DOCUMENT) ad = a;
				a = a.Navigate("pa");
			} while(a != null);
			if(ad == null || ad.IsInvisible) return false;
			return true;
		}

		static string _ClassNameEscapeWildcardEtc(string s)
		{
			if(!Empty(s)) {
				s = s.Replace('*', '?');
				int n = s.RegexReplace_(out s, @"^WindowsForms\d+(\..+?\.).+", "*$1*");
				if(n == 0) n = s.RegexReplace_(out s, @"^(HwndWrapper\[.+?;).+", "$1*");
			}
			return s;
		}

		void _FillGrid2()
		{
			_AddProp(null, "also", "o => false", tt: "Lambda that returns true if Acc o is the wanted object.");
			_AddProp(null, "skip", "0", tt: "0-based index of matching object.\nFor example, if 1, gets the second matching object.");
			_AddProp(null, "navig", null, tt: "When found, call Acc.Navigate to get another object.\nThis example contains most of what can be used:\nparent next previous first last parent child3 first2\nThe number for child is 1-based child index; if negative - from end.\nThe number for first and others is how many times.\nCan be 2 letters, like pa ne pr fi la ch3 fi2.");
			_AddProp(null, "wait", "3", tt: "Wait max this time, seconds.\nIf not 0/empty, on timeout returns null or throws exception, depending on the 'Throw ...' checkbox.");
			_AddFlag("orThrow", "Throw if not found", true, tt: "If not found, throw exception.\nIf this is unchecked, then returns null.");
			_grid2.ZAddHeaderRow("Search settings");
			_AddFlag(nameof(AFFlags.Reverse), "Reverse order", tt: "Flag AFFlags.Reverse.\nWalk the object tree from bottom to top.");
			_AddFlag(nameof(AFFlags.HiddenToo), "Can be invisible", tt: "Flag AFFlags.HiddenToo.");
			_AddFlag(nameof(AFFlags.UIA), "UI Automation", tt: "Flag AFFlags.UIA.\nUse UI Automation API instead of IAccessible.\nThe capturing tool checks/unchecks this automatically when need.");
			_AddFlag(nameof(AFFlags.NotInProc), "Not in-process", tt: "Flag AFFlags.NotInProc.\nMore info in AFFlags help.");
			_AddFlag(nameof(AFFlags.MenuToo), "Can be in MENUITEM", tt: "Flag AFFlags.MenuToo.\nCheck this if the object is in a menu and its role is not MENUITEM or MENUPOPUP.");
			_AddProp("notin", "Not in", "LIST,OUTLINE,TASKBAR,SCROLLBAR", tt: "Don't search in objects that have these roles.");
			_AddProp(null, "maxcc", "10000", tt: "Don't search in objects that have more direct children.\nDefault 10000.");
			_AddProp(null, "level", "0 1000", tt: "0-based level of the object in the object tree. Or min and max levels.\nRelative to the window, control or web page.\nDefault 0 1000.");

			void _AddProp(string key, string name, string value, string tt = null)
			{
				_grid2.ZAddOptional(key, name, value, tt: tt);
			}

			void _AddFlag(string flag, string name, bool check = false, string tt = null)
			{
				_grid2.ZAddFlag(flag, name, check, tt: tt);
			}
		}

		private void _cCapture_CheckedChanged(object sender, EventArgs e)
		{
			if(_cCapture.Checked) {
				//let other dialogs stop capturing
				foreach(var w in Wnd.FindAll("Find accessible object")) {
					if(w == (Wnd)this) continue;
					var c = w.Child("Capture with F*", "*.BUTTON.*");
					if(!c.Is0) {
						var b = (Wnd.WButton)c;
						if(b.IsChecked()) b.Check(false);
					}
				}

				Keys vk;
				for(vk = Keys.F3; vk <= Keys.F12; vk++) {
					_capturing = Api.RegisterHotKey((Wnd)this, 1, 0, (uint)vk);
					if(_capturing) break;
				}
				if(_capturing) {
					_cCapture.Text = "Capture, " + vk;
					if(Empty(_tAcc.Text)) _tAcc.Text = "Move the mouse to an object in any window. Press " + vk + ".";
				} else {
					_cCapture.Checked = false;
					TaskDialog.ShowError("Failed to set hotkey F3...F12");
				}
			} else if(_capturing) {
				_capturing = !Api.UnregisterHotKey((Wnd)this, 1);
			}

			if(_capturing) {
				if(_timer == null) {
					_osr = _CreateOnScreenRect();
					_timer = new Timer_(t =>
					{
						var w = Wnd.FromMouse(WXYFlags.NeedWindow);
						if(w.Is0 || w == (Wnd)this || w.WndOwner == (Wnd)this) _osr.Visible = false;
						else {
							if(!_AccFromMouse(out var a)) _osr.Visible = false;
							else {
								_osr.Rect = a.Rect;
								_osr.Show(true);
							}
						}
					});
				}
				_timer.Start(250, false);
			} else if(_timer != null) {
				_timer.Stop();
				_osr.Show(false);
				if(_tAcc.Text?.StartsWith_("Move") ?? false) _tAcc.Text = "";
			}
		}
		bool _capturing;

		protected override void WndProc(ref Message m)
		{
			Wnd w = (Wnd)this; uint msg = (uint)m.Msg; LPARAM wParam = m.WParam, lParam = m.LParam;
			//var s = m.ToString();

			switch(msg) {
			case Api.WM_HOTKEY:
				switch((int)wParam) {
				case 1:
					_Capture();
					return;
				}
				break;
			}

			base.WndProc(ref m);
		}

		void _Capture()
		{
			if(!_AccFromMouse(out var acc)) return;
			//_cCapture.Checked = false;
			_acc = acc;
			_SetAcc(true);
			var w = (Wnd)this;
			if(w.IsMinimized) {
				w.ShowNotMinMax();
				w.ActivateLL();
			}
		}

		bool _AccFromMouse(out Acc a)
		{
			var flags = AXYFlags.PreferLink | AXYFlags.NoThrow;
			if(_grid2.RowsCount > 0) {
				if(_uiaUserChecked && _IsChecked2(nameof(AFFlags.UIA))) flags |= AXYFlags.UIA;
				if(_IsChecked2(nameof(AFFlags.NotInProc))) flags |= AXYFlags.NotInProc;
			}
			a = Acc.FromMouse(flags);
			return a != null;
		}
		bool _uiaUserChecked; //to prevent capturing with AXYFlags.UIA when the checkbox was checked automatically (not by the user)
		bool _waitAutoCheckedOnce; //if user unchecks, don't check next time

		const string c_registryKey = @"\Tools\Acc";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Wnd w = (Wnd)this;
			if(Registry_.GetString(out var wndPos, "wndPos", c_registryKey))
				try { w.RestorePositionSizeState(wndPos, true); } catch { }

			if(_acc != null) _SetAcc(false);

			_cCapture.Checked = true;

			//_grid.ValueEditor.Control.Validating += (sender, cea) => _GridValidating(_grid, cea);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if(_acc != null) try { Mouse.Move((Wnd)_bTest); } catch { }
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Wnd w = (Wnd)this;
			if(_capturing) _cCapture.Checked = false;
			_osr?.Dispose();
			Registry_.SetString(w.SavePositionSizeState(), "wndPos", c_registryKey);
			base.OnFormClosing(e);
		}

		void _SetGridEvents()
		{
			Action<SG.CellContext> f = _OnValueChanged;
			_grid.ValueChanged += f;
			_grid2.ValueChanged += f;
		}

		void _OnValueChanged(SG.CellContext sender)
		{
			//Print(sender.DisplayText);
			//Print(_inSetGrid);

			if(_noeventGridValueChanged) return; _noeventGridValueChanged = true;
			var g = sender.Grid as ParamGrid;
			var pos = sender.Position;
			switch(pos.Column) {
			case 0:
				if(g == _grid2 && g.ZGetRowKey(pos.Row) == nameof(AFFlags.UIA)) {
					_uiaUserChecked = _IsChecked2(nameof(AFFlags.UIA));
					TaskDialog.Show(null, "Please re-capture the object to update this window.", flags: TDFlags.OwnerCenter, owner: this);
					//TODO: balloon tooltip
					_cCapture.Checked = true;
				}
				break;
			case 1:
				break;
			}
			_noeventGridValueChanged = false;

			_OnGridChanged();
		}
		bool _noeventGridValueChanged;

		void _OnGridChanged()
		{
			//Print("_OnGridChanged");
			if(_grid.RowsCount == 0) return; //cleared on exception
			_tAcc.Text = _FormatAcc(false);
		}

		string _FormatAcc(bool forTest)
		{
			var b = new StringBuilder();

			bool orThrow = !forTest && _IsChecked2("orThrow");
			bool isNavig = _grid2.ZGetValue("navig", out var navig, true);

			string wait = null;
			bool isWait = !forTest && _grid2.ZGetValue("wait", out wait, false);
			if(isWait) {
				if(orThrow && isNavig) b.Append('+');
				b.Append("Acc.Wait(");
				if(wait == null) wait = "0"; else if(!orThrow && !wait.StartsWith_('-')) b.Append('-');
				b.Append(wait);
			} else {
				if(orThrow) b.Append('+');
				b.Append("Acc.Find(");
			}

			_AppendOther(b, _sWndVar, noComma: !isWait);

			_grid.ZGetValue("role", out var role, true);
			_AppendString(b, role);
			bool isName = _grid.ZGetValueIfExists("name", out var name, false);
			if(isName) _AppendString(b, name ?? "");

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
					switch(na) { case "class": case "id": case "elem": case "state": case "rect": continue; }
				}
				if(isProp) b.Append(@" + '\0' + ");
				else {
					isProp = true;
					b.Append(", ");
					if(!isName) b.Append("prop: ");
				}
				b.Append('\"');
				b.Append(na);
				b.Append('=');
				if(!Empty(va)) {
					if(_IsVerbatim(va)) {
						b.Append("\" + ");
						b.Append(va);
						continue;
					} else {
						va = va.Escape_();
						b.Append(va);
					}
				}
				b.Append('\"');
			}

			bool isFlags = false;
			string[] flagNames = typeof(AFFlags).GetEnumNames();
			for(int r = 0, n = _grid2.RowsCount; r < n; r++) {
				if(!flagNames.Contains(_grid2.ZGetRowKey(r))) continue;
				if(!_grid2.ZIsChecked(r)) continue;
				var flag = "AFFlags." + _grid2.ZGetRowKey(r);
				if(!isFlags) {
					isFlags = true;
					_AppendOther(b, flag, (isName && isProp) ? null : "flags");
				} else {
					b.Append('|');
					b.Append(flag);
				}
			}

			if(_grid2.ZGetValue("also", out var also, true)) _AppendOther(b, also, "also");
			if(_grid2.ZGetValue("skip", out var skip, true)) _AppendOther(b, skip, "skip");

			b.Append(')');
			if(isNavig) {
				b.Append("?[");
				_AppendString(b, navig, noComma: true);
				if(isWait) _AppendOther(b, "1");
				b.Append(']');
			}
			b.Append(';');

			return b.ToString();
		}

		#region util

		bool _IsChecked2(string rowKey) => _grid2.ZIsChecked(rowKey);
		void _Check2(string rowKey, bool check) => _grid2.ZCheck(rowKey, check);

		static void _AppendString(StringBuilder b, string s, string param = null, bool noComma = false)
		{
			if(!noComma && b.Length > 1) b.Append(", ");
			if(param != null) { b.Append(param); b.Append(": "); }
			if(s == null) b.Append("null");
			else if(_IsVerbatim(s)) b.Append(s);
			else {
				b.Append('\"');
				b.Append(s.Escape_());
				b.Append('\"');
			}
		}

		//Returns true if s is like @"*" or $"*" or $@"*".
		static bool _IsVerbatim(string s)
		{
			if(s.Length < 3 || s[s.Length - 1] != '\"') return false;
			if(s[0] == '@') return s[1] == '\"';
			if(s[0] == '$') return s[1] == '\"' || (s[1] == '@' && s[2] == '\"' && s.Length > 3);
			return false;
		}

		static void _AppendOther(StringBuilder b, string s, string param = null, bool noComma = false)
		{
			Debug.Assert(!Empty(s));
			if(!noComma && b.Length > 1) b.Append(", ");
			if(param != null) { b.Append(param); b.Append(": "); }
			b.Append(s);
		}

		#endregion

		(_AccNode xRoot, _AccNode xSelect) _CreateModel(Wnd w, in AccProperties p, bool skipWINDOW)
		{
			_AccNode xRoot = new _AccNode("root"), xSelect = null;
			var stack = new Stack<_AccNode>(); stack.Push(xRoot);
			int level = 0;

			AFFlags flags = AFFlags.Mark | AFFlags.HiddenToo | AFFlags.MenuToo;
			if(_IsChecked2(nameof(AFFlags.UIA))) flags |= AFFlags.UIA;
			var us = (uint)p.State;
			var prop = $"rect={p.Rect.ToString()}\0state=0x{(us.ToString("X"))},!0x{((~us).ToString("X"))}";
			if(skipWINDOW) prop += $"\0 notin=WINDOW";
			//Print(prop.Replace('\0', ';'));
			var role = p.Role; if(role.Length == 0) role = null;
			Acc.Find(w, role, p.Name, prop, flags, also: o =>
			{
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
				if(o.MiscFlags.Has_(AccMiscFlags.Marked)) {
				//Print(o);
				if(xSelect == null) xSelect = x;
				}
				stack.Peek().Add(x);
				return false;
			});
			return (xRoot, xSelect);
		}

		//p - _acc properties. This func uses them to find and select the object in the tree.
		void _FillTree(in AccProperties p)
		{
			//SHOULDDO: if web in IE, show tree only for the control, because now can be slow because not in-proc.

			//Perf.First();
			var (xRoot, xSelect) = _CreateModel(_wnd, in p, false);

			if(xSelect == null) {
				//IAccessible of some controls are not connected to the parent.
				//	Noticed this in Office 2003 Word Options dialog and in Dreamweaver.
				//	Also, WndContainer then may get the top-level window. Eg in Word.
				//	Workaround: enum child controls and look for _acc in one them. Then add "class" row if need.
				Debug_.Print("broken IAccessible branch");
				foreach(var c in _wnd.AllChildren(onlyVisible: true)) {
					var m = _CreateModel(c, in p, true);
					if(m.xSelect != null) {
						//m.xRoot.a = Acc.FromWindow(c, flags: AWFlags.NoThrow);
						//if(m.xRoot.a != null) model.xRoot.Add(m.xRoot);
						//else model.xRoot = m.xRoot;
						xRoot = m.xRoot;
						xSelect = m.xSelect;
						if(_grid.ZFindRow("class") < 0) {
							_grid.ZAddOptional(null, "class", _ClassNameEscapeWildcardEtc(c.ClassName), true);
							_grid.ZAutoSizeColumns();
						}
					}
				}
			}

			//Print("------");
			//Print(xr);

			//Perf.Next();
			_tree.Model = new _AccTree(xRoot);
			//Perf.Next();

			if(xSelect != null) {
				var n = _tree.FindNodeByTag(xSelect);
				if(n != null) {
					_tree.Visible = false;
					_tree.EnsureVisible(n);
					n.IsSelected = true;
					_tree.Visible = true;

					//tree control bug: if visible and need to scroll, does not scroll, and when scrolled paits items over items etc.
					//	Workaround 1: temporarily hide.
					//	Workaround 2: BeginUpdate, EnsureVisible, EndUpdate, EnsureVisible. Slightly slower.
				}
			}
			//Perf.NW();
		}

		NodeTextBox _ccName;

		void _InitTree()
		{
			_tree.Indent = 10;
			//_tree.LoadOnDemand = true; //makes slightly faster, but then not so easy to expand to ensure visible (cannot use FindNodeByTag)

			_ccName = new NodeTextBox();
			_tree.NodeControls.Add(_ccName);
			_ccName.LeftMargin = 0;
			//_ccName.Trimming = StringTrimming.EllipsisCharacter;

			_ccName.ValueNeeded = node =>
			{
				var a = node.Tag as _AccNode;
			//Print(a.a);
			return a.DisplayText;
			};

			_tree.NodeMouseClick += _tree_NodeMouseClick;
			_tree.KeyDown += _tree_KeyDown;
			_tree.DrawControl += _tree_DrawControl;
		}

		private void _tree_DrawControl(object sender, DrawEventArgs e)
		{
			var a = e.Node.Tag as _AccNode;
			//Print(a.a);
			if(e.Node.IsSelected) {
				//PrintList(e.Text, e.Context.DrawSelection);
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
			if(!_SetAccGrids(out var p)) return;

			var r = p.Rect;
			if(!r.IsEmpty) {
				var osr = _CreateOnScreenRect();
				osr.Rect = r;
				osr.Show(true);
				Timer_.After(1000, t => osr.Dispose());
			}
		}

		OnScreenRect _CreateOnScreenRect() => new OnScreenRect() { Color = Color.DarkOrange, Thickness = 2 };

		void _ShowOnScreenRect(in RECT r, bool blink)
		{
			if(r.IsEmpty) return;
			var osr = _CreateOnScreenRect();
			osr.Rect = r;
			osr.Show(true);
			if(blink) {
				int i = 0;
				Timer_.Every(250, t =>
				{
					if(i < 4) osr.Show((i++ & 1) != 0);
					else {
						t.Stop();
						osr.Dispose();
					}
				});
			} else {
				Timer_.After(1000, t => osr.Dispose());
			}
			//TODO: try AnimateWindow, or implement it with timer and osr.Rect
		}

		class _AccTree :ITreeModel
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

		class _AccNode :XElement
		{
			public _AccNode(string name) : base(name) { }

			public Acc a;
			string _displayText;

			public string DisplayText
			{
				get
				{
					if(_displayText == null) {
						bool isWINDOW = a.RoleInt == AccROLE.WINDOW;
						string props = isWINDOW ? "Rnsw" : "Rns";
						if(!a.GetProperties(props, out var p)) {
							IsException = true;
							return _displayText = "Failed: " + Native.GetErrorMessage();
						}

						if(isWINDOW) {
							var b = Au.Util.LibStringBuilderCache.Acquire();
							b.Append(p.Role);
							b.Append("  (");
							b.Append(p.WndContainer.ClassName);
							b.Append(")");
							if(p.Name.Length > 0) {
								b.Append("  \"");
								b.Append(p.Name);
								b.Append("\"");
							}
							_displayText = b.ToStringCached_();
						} else if(p.Name.Length == 0) _displayText = p.Role;
						else _displayText = p.Role + " \"" + p.Name.Limit_(250).Escape_() + "\"";

						IsInvisible = a.LibIsInvisible(p.State);
					}
					return _displayText;
				}
			}

			public bool IsInvisible { get; private set; }
			public bool IsException { get; private set; }
		}

		private async void _bTest_Click(object sender, EventArgs ea)
		{
			if(_grid.RowsCount == 0) return;
			string sWnd = _tWnd.Text; if(Empty(sWnd)) return;
			_lSpeed.Text = "";
			string sAcc = _FormatAcc(true);

			//Perf.First();

			var b = new StringBuilder();
			b.Append(sWnd); b.Append("var _p_ = Perf.StartNew();"); b.AppendLine("var _a_ = ");
			b.AppendLine(sAcc);
			b.AppendLine("_p_.Next(); return (_p_.TimeTotal, _a_);");

			var s = b.ToString(); //Print(s);
			(long speed, Acc a) result = default;
			try {
				_bTest.Enabled = false;
				var so = ScriptOptions.Default.WithReferences(s_testReferences).WithImports(s_testImports);
				result = await CSharpScript.EvaluateAsync<(long, Acc)>(s, so);
			}
			catch(CompilationErrorException e) {
				var es = String.Join("\r\n", e.Diagnostics);
				TaskDialog.ShowError(e.GetType().Name, es, owner: this, flags: TDFlags.OwnerCenter | TDFlags.Wider/*, expandedText: s*/);
				return;
			}
			catch(NotFoundException) {
				TaskDialog.ShowInfo("Window not found", owner: this, flags: TDFlags.OwnerCenter);
				return;
				//info: throws only when window not found. This is to show time anyway when acc not found.
			}
			catch(Exception e) {
				TaskDialog.ShowError(e.GetType().Name, e.Message, owner: this, flags: TDFlags.OwnerCenter);
				return;
			}
			finally { _bTest.Enabled = true; }

			//Perf.NW();
			//Print(result);

			var t = Math.Round((double)result.speed / 1000, result.speed < 1000 ? 2 : (result.speed < 10000 ? 1 : 0));
			_lSpeed.Text = "time: " + t.ToString_() + " ms";
			//_lSpeed.ForeColor = t <= 100 ? Form.DefaultForeColor : Color.Red;
			if(result.a != null) {
				_ShowOnScreenRect(result.a.Rect, true);
			} else {
				TaskDialog.ShowInfo("Not found", owner: this, flags: TDFlags.OwnerCenter);
				//TODO: balloon tooltip
			}
		}

		//Namespaces and references for 'also' lambda, to use when testing.
		//FUTURE: user-defined imports and references. Probably as script header, where can bu used #r, #load, using, etc.
		static string[] s_testImports = { "Au", "Au.Types", "Au.NoClass", "System", "System.Collections.Generic", "System.Text.RegularExpressions", "System.Windows.Forms", "System.Drawing", "System.Linq" };
		static Assembly[] s_testReferences = { Assembly.GetAssembly(typeof(Wnd)) }; //info: somehow don't need to add System.Windows.Forms, System.Drawing.

		private void _bCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(_FormatFullCode());
		}

		private void _bOK_Click(object sender, EventArgs e)
		{
			ResultCode = _FormatFullCode();
		}

		/// <summary>
		/// When OK clicked, contains C# code to find the accessible object.
		/// </summary>
		public string ResultCode { get; private set; }

		string _FormatFullCode()
		{
			if(_grid.RowsCount == 0) return null;
			string sWnd = _tWnd.Text;
			string sAcc = _FormatAcc(false);

			var b = new StringBuilder();
			if(!Empty(sWnd)) b.AppendLine(sWnd);
			b.Append("var a = "); b.AppendLine(sAcc);

			return b.ToString();
		}

		//This func can be used to validate any text cell in any grid.
		//void _GridValidating(ParamGrid grid, CancelEventArgs ce)
		//{
		//	var editor = grid.ValueEditor;
		//	var pos = editor.EditPosition;
		//	string sOld = grid[pos].Value as string, sNew = editor.GetEditedValue() as string, err = null;
		//	if(sNew == sOld) return;
		//	if(sNew == null) return;
		//	//PrintList(sOld, sNew);
		//	switch(grid.ZGetRowKey(pos.Row)) {
		//	case "example":
		//		if(!sNew.RegexIs_(@"^example$")) err = "example.";
		//		break;
		//	}
		//	if(err != null) {
		//		ce.Cancel = true;
		//		_errorProvider.SetIconAlignment(editor.Control, ErrorIconAlignment.MiddleLeft);
		//	}
		//	_errorProvider.SetError(editor.Control, err);
		//}
	}
}
