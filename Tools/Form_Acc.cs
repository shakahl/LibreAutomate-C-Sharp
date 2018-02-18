#define USE_CODEANALYSIS_REF

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

#if USE_CODEANALYSIS_REF
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
#endif

//FUTURE: when the AO is in a control of other thread, use Wnd.Find(...).Child(...), and show tree only of that control.
//FUTURE: ngen C# compiler assemblies. Now each Windows update unngens most of them.
//FUTURE: support Edge better. Eg can find without UIA. See the workarounds.

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

			//_TestFonts();

			_acc = acc; //will be processed in OnLoad
		}

		//todo: remove this test code.

		//protected override void OnPaint(PaintEventArgs e)
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
			//if(captured && _wnd.Is0) _wnd = Wnd.FromMouse(WXYFlags.NeedWindow);
			if(captured && !_wnd.IsVisibleEx) _wnd = Wnd.FromMouse(WXYFlags.NeedWindow); //Edge workaround. Without it, _wnd would be some cloaked window of other process, and there are many such cloaked windows, and Wnd.Find often finds wrong window.

			if(_grid2.RowsCount == 0) {
				_FillGrid2();
				_grid2.ZAutoSizeRows();
				_grid2.ZAutoSizeColumns();
			}

			_ClearTree();
			if(!_SetAccGrids(out var p)) return;

			if(!_wnd.Is0) {
				_SetWindow(); //set the Wnd.Find textbox
				_FillTree(p);
			}

			if(captured && p.Role == "CLIENT" && _wnd.ClassNameIs("SunAwt*") && !_acc.MiscFlags.Has_(AccMiscFlags.Java) && Ver.Is64BitOS)
				_SetFormInfo(c_infoJava);
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
				_tWnd.ReadOnly = true;
				return;
			}
			_AppendString(b, _StripClassName(s, true));

			if(!_wnd.IsVisibleEx) _AppendOther(b, "WFFlags.HiddenToo", "flags");

			b.Append(");");
			_tWnd.ReadOnly = false;
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
				_tAcc.Text = "Failed to get AO properties: " + Native.GetErrorMessage();
				return false;
			}

			_noeventGridValueChanged = true;

			Wnd w = p.WndContainer;
			if(!_wnd.Is0 && !w.IsChildOf(_wnd)) w = _wnd; //Edge workaround (see other workaround)

			bool isWeb = _IsVisibleWebPage(_acc, out int browser, w);

			var role = p.Role; if(isWeb) role = "web:" + role;
			_AddIfNotEmpty("role", role, true, false, info: c_infoRole);
			//CONSIDER: path too. But maybe don't encourage, because then the code depends on window/page structure.
			bool noName = !_AddIfNotEmpty("name", p.Name, true, true, info: _MakeInfoWildex("Name"));
			if(_AddIfNotEmpty("uiaid", p.UiaId, noName, true, info: _MakeInfoWildex("UIA AutomationId"))) noName = false;

			//control
			if(!isWeb && w != _wnd) {
				int id = w.ControlId;
				bool isId = (id > 0 && id < 0x10000 && id != (int)w.Handle && _wnd.ChildAll("**id:" + id).Length == 1);
				if(isId) _Add("id", id.ToString(), true, info: c_infoId);
				_Add("class", _StripClassName(w.ClassName), !isId, info: _MakeInfoWildex(c_infoClass));
			}

			_AddIfNotEmpty("value", p.Value, false, true, info: _MakeInfoWildex("Value"));
			if(_AddIfNotEmpty("description", p.Description, noName, true, info: _MakeInfoWildex("Description"))) noName = false;
			_AddIfNotEmpty("action", p.DefaultAction, false, true, info: _MakeInfoWildex("Default action"));
			if(_AddIfNotEmpty("key", p.KeyboardShortcut, noName, true, info: _MakeInfoWildex("Keyboard shortcut"))) noName = false;
			if(_AddIfNotEmpty("help", p.Help, noName, true, info: _MakeInfoWildex("Help"))) noName = false;
			foreach(var attr in p.HtmlAttributes as Dictionary<string, string>) {
				string na = attr.Key, va = attr.Value;
				bool check = noName && (na == "id" || na == "name") && va.Length > 0;
				if(check) noName = false;
				_Add("@" + na, _EscapeWildex(va), check, info: _MakeInfoWildex("HTML attribute"));
			}
			int elem = _acc.SimpleElementId; if(elem != 0) _Add("elem", elem.ToString(), info: c_infoElem);
			_Add("state", p.State.ToString(), info: c_infoState);
			_Add("rect", $"{{W={p.Rect.Width} H={p.Rect.Height}}}", tt: "Rectangle in screen: " + p.Rect.ToString(), info: c_infoRect);

			//CONSIDER: if no name etc, try to get uiaid. Eg winforms control name. Or use prop "wfName=...:".

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

			bool _AddIfNotEmpty(string name, string s, bool check, bool escape, string tt = null, string info = null)
			{
				if(Empty(s)) return false;
				if(escape) s = _EscapeWildex(s);
				_Add(name, s, check, tt, info);
				return true;
			}

			void _Add(string name, string value, bool check = false, string tt = null, string info = null)
			{
				//tests
				//name = value = "Tjgtli";
				//value = value + "\r\nline2";
				//if(name == "role") { _grid.ZAddRequired(null, name, value); return; }

				_grid.ZAddOptional(null, name, value, check, tt, info);
			}

			string _MakeInfoWildex(string prefix) => prefix + c_infoWildex;
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

		void _FillGrid2()
		{
			_AddProp(null, "also", "o => false", tt: "Lambda that returns true if Acc o is the wanted AO.", info: c_infoAlso);
			_AddProp(null, "skip", "1", tt: "0-based index of matching AO.\nFor example, if 1, gets the second matching AO.");
			_AddProp(null, "navig", null, tt: "When found, call Acc.Navigate to get another AO.", info: c_infoNavig);
			_AddProp(null, "wait", "3", tt: "Wait for the AO max this time, seconds.", info: c_infoWait);
			_AddFlag("orThrow", "Throw if not found", true, tt: "If not found, throw exception.\nIf this is unchecked, then the function returns null.");
			_grid2.ZAddHeaderRow("Search settings");
			_AddFlag(nameof(AFFlags.Reverse), "Reverse order", tt: "Flag AFFlags.Reverse.\nWalk the object tree from bottom to top.");
			_AddFlag(nameof(AFFlags.HiddenToo), "Can be invisible", tt: "Flag AFFlags.HiddenToo.");
			_AddFlag(nameof(AFFlags.UIA), "UI Automation", tt: "Flag AFFlags.UIA.\nUse UI Automation API instead of IAccessible.\nThe capturing tool checks/unchecks this automatically when need.");
			_AddFlag(nameof(AFFlags.NotInProc), "Not in-process", tt: "Flag AFFlags.NotInProc.\nMore info in AFFlags help.");
			_AddFlag(nameof(AFFlags.MenuToo), "Can be in MENUITEM", tt: "Flag AFFlags.MenuToo.\nCheck this if the AO is in a menu and its role is not MENUITEM or MENUPOPUP.");
			_AddProp("notin", "Not in", null, tt: "Don't search in AOs that have these roles. Can make faster.", info: c_infoNotin);
			_AddProp(null, "maxcc", null, tt: "Don't search in AOs that have more direct children. Default 10000.");
			_AddProp(null, "level", null, tt: "Level of the AO in the object tree. Or min and max levels. Default 0 1000.", info: c_infoLevel);

			void _AddProp(string key, string name, string value, string tt = null, string info = null)
			{
				_grid2.ZAddOptional(key, name, value, false, tt, info);
			}

			void _AddFlag(string flag, string name, bool check = false, string tt = null)
			{
				_grid2.ZAddFlag(flag, name, check, tt: tt);
			}
		}

		#region capture

		private void _cCapture_CheckedChanged(object sender, EventArgs e)
		{
			//Print(_cCapture.Checked);

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

				g1:
				_capturing = Api.RegisterHotKey((Wnd)this, 1, 0, (uint)(_CapturingKey + (int)Keys.F1 - 1));
				if(!_capturing) {
					if(_CapturingKeyDialog()) goto g1;
					_cCapture.Checked = false;
					return;
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
			}
		}
		bool _capturing;

		int _CapturingKey
		{
			get
			{
				if(_capturingKey == 0) {
					Registry_.GetInt(out var k, "key", c_registryKey);
					_capturingKey = Math_.MinMax(k, 3, 12);
				}
				return _capturingKey;
			}
			set
			{
				_capturingKey = value;
				Registry_.SetInt(value, "key", c_registryKey);
				if(_capturing) {
					_cCapture.Checked = false;
					_cCapture.Checked = true;
				}
				_SetFormInfo(null);
			}
		}
		int _capturingKey;

		bool _CapturingKeyDialog()
		{
			var key = _CapturingKey;
			var d = new TaskDialog("Accessible object capturing key", buttons: "OK|Cancel", owner: this, flags: TDFlags.OwnerCenter);
			d.SetRadioButtons("3F3|4F4|5F5|6F6|7F7|8F8|9F9|10F10|11F11|12F12", key);
			var r = d.ShowDialog(); if(r != 1) return false;
			if(r.RadioButton != key) _CapturingKey = r.RadioButton;
			return true;
		}

		void _Capture()
		{
			if(!_AccFromMouse(out var acc)) return;
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

		#endregion

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

		const string c_registryKey = @"\Tools\Acc";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Wnd w = (Wnd)this;
			if(Registry_.GetString(out var wndPos, "wndPos", c_registryKey))
				try { w.RestorePositionSizeState(wndPos, true); } catch { }

			_sWndVar = "w";
			_tWnd.TextChanged += _tWnd_TextChanged;

			if(_acc != null) _SetAcc(false);

			_InitInfo();

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
			_grid.ZValueChanged += f;
			_grid2.ZValueChanged += f;
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
					_ShowTreeInfo("Please capture the AO again.");
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

			if(_grid.ZGetValueIfExists("role", out var role, true)) _AppendString(b, role);
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

		static string _EscapeWildex(string s)
		{
			if(Wildex.HasWildcards(s)) s = "**t|" + s;
			return s;
		}

		static string _StripClassName(string s, bool escapeWildex = false)
		{
			if(!Empty(s)) {
				int n = s.RegexReplace_(out s, @"^WindowsForms\d+(\..+?\.).+", "*$1*");
				if(n == 0) n = s.RegexReplace_(out s, @"^(HwndWrapper\[.+?;).+", "$1*");
				if(escapeWildex && n == 0) s = _EscapeWildex(s);
			}
			return s;
		}

		OnScreenRect _CreateOnScreenRect() => new OnScreenRect() { Color = Color.DarkOrange, Thickness = 2 };

		void _ShowOnScreenRect(in RECT r, bool blink)
		{
			if(r.IsEmpty) return;
			var osr = new OnScreenRect() { Color = Color.DarkOrange, Thickness = 4 };

			osr.Rect = r;
			osr.Show(true);

			//FUTURE: show something more visible, eg line from Mouse.XY to r. For it create class OnScreenLine or extend OnScreenRect.
			//	Or could animate the rect, but then not good when small.

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
		}

		#endregion

		#region tree

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
			try {
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
			}
			catch(Exception ex) {
				_ShowTreeInfo("Failed to get object tree.\r\n" + ex.Message);
				return (null, null);
			}
			return (xRoot, xSelect);
		}

		//p - _acc properties. This func uses them to find and select the AO in the tree.
		void _FillTree(in AccProperties p)
		{
			//SHOULDDO: if web in IE, show tree only for the control, because now can be slow because not in-proc.

			//Perf.First();
			var (xRoot, xSelect) = _CreateModel(_wnd, in p, false);
			if(xRoot == null) return;

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
							_grid.ZAddOptional(null, "class", _StripClassName(c.ClassName), true);
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

					//tree control bug: if visible and need to scroll, does not scroll, and when scrolled paints items over items etc.
					//	Workaround 1: temporarily hide.
					//	Workaround 2: BeginUpdate, EnsureVisible, EndUpdate, EnsureVisible. Slightly slower.
				}
			}
			//Perf.NW();
		}

		void _ShowTreeInfo(string text)
		{
			if(_lTreeInfo == null) _tree.Controls.Add(_lTreeInfo = new Label());
			_lTreeInfo.Size = _tree.ClientSize;
			_lTreeInfo.Text = text;
			_lTreeInfo.Visible = true;
		}

		void _ClearTree()
		{
			_tree.Model = null;
			_lTreeInfo?.Hide();
		}

		Label _lTreeInfo;
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

		#endregion

		#region test

		//Namespaces and references for 'also' lambda, to use when testing.
		//FUTURE: user-defined imports and references. Probably as script header, where can bu used #r, #load, using, etc.
		static string[] s_testImports = { "Au", "Au.Types", "Au.NoClass", "System", "System.Collections.Generic", "System.Text.RegularExpressions", "System.Windows.Forms", "System.Drawing", "System.Linq" };
		static Assembly[] s_testReferences = { Assembly.GetAssembly(typeof(Wnd)) }; //info: somehow don't need to add System.Windows.Forms, System.Drawing.

#if USE_CODEANALYSIS_REF
		//C# script compiler setup:
		//Install nuget package Microsoft.CodeAnalysis.CSharp.Scripting.
		//The easy way:
		//	Install it in Au.Tools.
		//	Problem - cluttering: it installs about 25 packages, adds them to References, to the main output folder, etc.
		//Workaround:
		//	Install it in another solution (Compiler) that contains single project (Compiler).
		//	In Compiler project set output = subfolder "Compiler" of the main output folder "_".
		//	Compile the Compiler project. It adds all the dlls to the "Compiler" subfolder.
		//	In exe app.config add: configuration/runtime/assemblyBinding: <probing privatePath="Compiler"/>
		//	In Au.Tools add references from the "Compiler" subfolder:
		//		Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp.Scripting, Microsoft.CodeAnalysis.Scripting, System.Collections.Immutable.
		//		In reference assembly options, for each assembly make "copy local" = false.
		//		In the future may need more if we'll use more code.
		//	Issues:
		//		Adds System.Collections.Immutable.dll to the main output folder.
		//			Tried to edit app.config etc, unsuccessfully.
		//		Also, in app.config must be:
		//			<assemblyBinding xmlns = "urn:schemas-microsoft-com:asm.v1" >
		//				<probing privatePath="Compiler"/>
		//				<dependentAssembly>
		//					<assemblyIdentity name = "System.Collections.Immutable" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
		//					<bindingRedirect oldVersion = "0.0.0.0-1.2.1.0" newVersion="1.2.1.0" />
		//				</dependentAssembly>
		//			</assemblyBinding>

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
			b.AppendLine($"_p_.Next(); return (_p_.TimeTotal, _a_, {_sWndVar});");

			var code = b.ToString(); //Print(code);
			(long speed, Acc a, Wnd w) r = default;
			try {
				_bTest.Enabled = false;
				var so = ScriptOptions.Default.WithReferences(s_testReferences).WithImports(s_testImports);
				r = await CSharpScript.EvaluateAsync<(long, Acc, Wnd)>(code, so);
			}
			catch(CompilationErrorException e) {
				var es = String.Join("\r\n", e.Diagnostics);
				TaskDialog.ShowError(e.GetType().Name, es, owner: this, flags: TDFlags.OwnerCenter | TDFlags.Wider/*, expandedText: code*/);
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
			//Print(r);

			var t = Math.Round((double)r.speed / 1000, r.speed < 1000 ? 2 : (r.speed < 10000 ? 1 : 0));
			var sTime = t.ToString_() + " ms";
			if(r.a != null) {
				_lSpeed.ForeColor = Form.DefaultForeColor;
				_lSpeed.Text = sTime;
				_ShowOnScreenRect(r.a.Rect, true);
			} else {
				//TaskDialog.ShowEx("Not found", owner: this, flags: TDFlags.OwnerCenter, icon: TDIcon.Info, secondsTimeout: 2);
				_lSpeed.ForeColor = Color.Red;
				_lSpeed.Text = "Not found,";
				Timer_.After(500, tt => _lSpeed.Text = sTime);
			}

			if(r.w != _wnd) {
				TaskDialog.ShowWarning("Wnd.Find finds another window",
				$"Captured: {_wnd.ToString()}\r\n\r\nFound: {r.w.ToString()}",
				owner: this, flags: TDFlags.OwnerCenter | TDFlags.Wider);
			} else if(r.a != null && r.speed >= 20_000 && !_IsChecked2(nameof(AFFlags.NotInProc)) && !_IsChecked2(nameof(AFFlags.UIA))) {
				if(!r.a.MiscFlags.Has_(AccMiscFlags.InProc) && _wnd.ClassNameIs("Mozilla*")) {
					//need full path. Run("firefox.exe") fails if firefox is not properly installed.
					string ffInfo = c_infoFirefox, ffPath = _wnd.ProcessPath;
					if(ffPath != null) ffInfo = ffInfo.Replace("firefox.exe", ffPath);
					_SetFormInfo(ffInfo);
				}
			}
		}
#else

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
				result = ((long, Acc))(await Au.Compiler.Scripting.EvaluateAsync(s, s_testReferences, s_testImports));
			}
			catch(Au.Compiler.CompilationException e) {
				TaskDialog.ShowError(e.GetType().Name, e.Message, owner: this, flags: TDFlags.OwnerCenter | TDFlags.Wider/*, expandedText: s*/);
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
			var sTime=t.ToString_() + " ms";
			if(result.a != null) {
				_lSpeed.ForeColor = Form.DefaultForeColor;
				_lSpeed.Text = sTime;
				_ShowOnScreenRect(result.a.Rect, true);
			} else {
				//TaskDialog.ShowEx("Not found", owner: this, flags: TDFlags.OwnerCenter, icon: TDIcon.Info, secondsTimeout: 2);
				_lSpeed.ForeColor = Color.Red;
				_lSpeed.Text = "Not found,";
				Timer_.After(1000, tt => _lSpeed.Text = sTime);
			}
		}
#endif

		#endregion

		#region OK, Copy

		private void _bCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(_FormatFullCode() ?? "");
		}

		private void _bOK_Click(object sender, EventArgs e)
		{
			ResultCode = _FormatFullCode();
			if(ResultCode == null) this.DialogResult = DialogResult.Cancel;
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

		#endregion

		#region info

		//Called by OnLoad.
		void _InitInfo()
		{
			_SetFormInfo(null);
			Action<SG.CellContext, string> infoDelegate = (sender, info) => _SetFormInfo(info);
			_grid.ZShowEditInfo += infoDelegate;
			_grid2.ZShowEditInfo += infoDelegate;

			_tree.Paint += (object sender, PaintEventArgs e) =>
			 {
				 const string s = @"Object tree of the window.";
				 if(_tree.Model == null && !(_lTreeInfo?.Visible ?? false)) {
					 e.Graphics.Clear(this.BackColor); //like grids
					 _OnPaintDrawBackText(sender, e, s);
				 }
			 };

			_grid.Paint += (sender, e) => { if(_acc == null) _OnPaintDrawBackText(sender, e, @"AO properties."); };
			_grid2.Paint += (sender, e) => { if(_acc == null) _OnPaintDrawBackText(sender, e, @"Other parameters and search settings."); };

			void _OnPaintDrawBackText(object sender, PaintEventArgs e, string text)
			{
				var c = sender as Control;
				TextRenderer.DrawText(e.Graphics, text, Font, c.ClientRectangle, Color.FromKnownColor(KnownColor.GrayText), TextFormatFlags.WordBreak);
			}

			_info.Tags.AddLinkTag("_key", _ => _CapturingKeyDialog());
			_info.Tags.AddLinkTag("_resetInfo", _ => _SetFormInfo(null));
			_info.Tags.AddLinkTag("_jab", _ => Java.EnableDisableJabUI(this));
		}

		//If info == null, sets form info text.
		void _SetFormInfo(string info)
		{
			if(info == null) {
				info = c_infoForm;
				var k = _CapturingKey;
				if(k != 3) info = info.Replace("F3", "F" + k.ToString());
			}
			_info.Text = info;
		}

		const string c_infoForm =
@"Creates code to find an <i>accessible object (AO)<> - button, link, etc. Then your script can click it, etc. See <help M_Au_Acc_Find>Acc.Find<>, <help T_Au_Acc>Acc<>, <help M_Au_Wnd_Find>Wnd.Find<>. How to use:
1. Move the mouse to the AO you want. Press key <_key>F3<>.
2. Click the Test button. It finds and shows the AO and the search time.
3. If need, check/uncheck/edit some fields or select another AO, Test.
4. Click OK, it inserts C# code in the editor. Or Copy to the clipboard.
5. In the editor, after this code add code to use the AO. <help T_Au_Acc>Examples<>. If need, rename variables, delete duplicate Wnd.Find lines, replace part of window name with *, etc.

How to find AOs that don't have a name or other property with unique constant value? Capture another AO near it, and use <b>navig<> to get it. Or try <b>skip<>.";
		const string c_infoRole = @"Role. Or path, like ROLE1/ROLE2/ROLE3. Prefix <b>web:<>, <b>firefox:<> or <b>chrome:<> means 'in web page'. Path is relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc). Read more in <help M_Au_Acc_Find>Acc.Find<> help.";
		const string c_infoWildex = @". The text is <help 0248143b-a0dd-4fa1-84f9-76831db6714a>wildcard expression<>. Examples:  whole,  *end,  start*,  *middle*,  **t|non-wildcard,  **c|case-sensitive,  **tc|case-sens-non-wild,  **p|regex,  **pc|case-sens-regex,  **n|not this,
**m|this[]or this[]**p|or this regex[]**n|and not this";
		const string c_infoState = @"State. List of states that the AO must have and/or not have.
Example: CHECKED, !DISABLED
Note: AO state can change. Use only states you need. Remove others from the list.";
		const string c_infoRect = @"Rectangle. Can be specified width (W) and/or height (H).
Example: {W=100 H=20}";
		const string c_infoClass = @"Control class name. Will search only in controls that have it";
		const string c_infoId = @"Control id. Will search only in controls that have it.";
		const string c_infoElem = @"Simple element id.
Note: It usually changes when elements before the AO are added or removed. Use it only if really need.";
		const string c_infoAlso = @"<b>also<> examples:
<code>o => { Print(o); return false; }</code>
<code>o => o.GetRect(out var r, o.WndTopLevel) && r.Contains(266, 33)</code>";
		const string c_infoNavig = @"<b>navig<> is a path to another AO from the found AO in the object tree. One or more of these words: <u><i>parent<> <i>child<> <i>first<> <i>last<> <i>next<> <i>previous<><>. Or 2 letters, like <i>ne<>.
Example: pa ne2 ch3. The 2 means 2 times (ne ne). The 3 means 3-rd child (-3 would be 3-rd from end). More info: <help M_Au_Acc_Navigate>Acc.Navigate<>.";
		const string c_infoWait = @"The <b>wait<> value is the max number of seconds to wait for the AO. If 0 or empty, waits without a timeout. Else on timeout the function returns null or throws exception, depending on <b>Throw if not found<>.";
		const string c_infoLevel = @"<b>level<> - 0-based level of the AO in the object tree. Or min and max levels. Default 0 1000. Relative to the window, control (if used <b>class<> or <b>id<>) or web page (role prefix <b>web:<> etc).";
		const string c_infoNotin = @"<b>notin<> - don't search in AOs that have these roles. Can make faster.
Example: LIST,OUTLINE,TASKBAR,SCROLLBAR";
		const string c_infoFirefox = @"To make much faster in Firefox, disable its multiprocess feature: open URL <link firefox.exe|about:config>about:config<>, set <b>browser.tabs.remote.autostart<> = <b>false<>, restart Firefox. More info in <help T_Au_Acc>Acc<> help.
<_resetInfo>X<>";
		const string c_infoJava = @"If there are no AOs in this window, need to <_jab>enable<> Java Access Bridge etc. More info in <help T_Au_Acc>Acc<> help.
<_resetInfo>X<>";

		#endregion

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

		#region misc

		public static class Java
		{
			/// <summary>
			/// Calls <see cref="EnableDisableJab"/>(null) and shows results in task dialog.
			/// </summary>
			/// <param name="owner"></param>
			public static void EnableDisableJabUI(WndOrControl owner)
			{
				var (ok, results) = EnableDisableJab(null);
				if(results != null) TaskDialog.Show("Results", results, icon: ok ? TDIcon.Info : TDIcon.Error, owner: owner, flags: TDFlags.OwnerCenter | TDFlags.Wider);
			}

			/// <summary>
			/// Enables or disables Java Access Bridge for current user.
			/// Returns: ok = false if failed or cancelled. results = null if cancelled.
			/// </summary>
			/// <param name="enable">If null, shows enable/disable dialog.</param>
			public static (bool ok, string results) EnableDisableJab(bool? enable/*, bool allUsers*/)
			{
				if(enable == null) {
					switch(TaskDialog.ShowList("1 Enable|2 Disable|Cancel", "Java Access Bridge")) {
					case 1: enable = true; break;
					case 2: enable = false; break;
					default: return (false, null);
					}
				}
				bool en = enable.GetValueOrDefault();

				if(!GetJavaPath(out var dir)) return (false, "Cannot find Java" + (Ver.Is64BitProcess ? "64" : "") + "-bit. Make sure it is installed.");

				//if(!allUsers) {
				string jabswitch = dir + @"\bin\jabswitch.exe", sout = null;
				if(!Files.ExistsAsFile(jabswitch)) return (false, "Cannot find jabswitch.exe.");
				try {
					Shell.RunConsole(out sout, jabswitch, en ? "-enable" : "-disable");
					sout = sout?.Trim();
				}
				catch(Exception ex) {
					return (false, ex.GetType().Name + ", " + ex.Message);
				}
				//} else {
				//never mind
				//}

				sout += "\r\nRestart Java apps to apply the new settings.";

				string dll64 = Folders.SystemX64 + "WindowsAccessBridge-64.dll", dll32 = Folders.SystemX86 + "WindowsAccessBridge-32.dll";
				if(!Files.ExistsAsFile(dll64)) sout += "\r\n\r\nWarning: dll not found: " + dll64 + ".  64-bit apps will not be able to use AOs of Java apps. Install 64-bit Java too.";
				if(!Files.ExistsAsFile(dll32)) sout += "\r\n\r\nNote: dll not found: " + dll32 + ".  32-bit apps will not be able to use AOs of Java apps. Install 32-bit Java too.";

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
				if(!Registry_.GetString(out var ver, "CurrentVersion", rk)) return false;
				if(!Registry_.GetString(out path, "JavaHome", rk + @"\" + ver)) return false;
				return true;
			}
		}

		#endregion
	}
}
