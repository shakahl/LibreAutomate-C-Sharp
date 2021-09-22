using System.Windows;
using System.Windows.Controls;
using Au.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace Au.Tools;

static class TUtil
{
	#region text

	/// <summary>
	/// Appends ', ' and string argument to this StringBuilder.
	/// </summary>
	/// <param name="t"></param>
	/// <param name="s">Argument value. If null, appends 'null'. If verbatim (like '@"text"'), appends 's'. Else appends '"escaped s"'; can make verbatim.</param>
	/// <param name="param">If not null, appends 'param: s'. By default appends only 's'. If "null", appends 'null, s'.</param>
	/// <param name="noComma">Don't append ', '. If false, does not append if b.Length is less than 2 or if b ends with one of: <c>,([{&lt;</c>.</param>
	public static StringBuilder AppendStringArg(this StringBuilder t, string s, string param = null, bool noComma = false) {
		_AppendArgPrefix(t, param, noComma);
		if (s == null) t.Append("null");
		else if (IsVerbatim(s, out _) || MakeVerbatim(ref s)) t.Append(s);
		else t.Append(s.Escape(quote: true));
		return t;
	}

	/// <summary>
	/// Appends ', ' and non-string argument to this StringBuilder.
	/// </summary>
	/// <param name="t"></param>
	/// <param name="s">Argument value. Must not be empty.</param>
	/// <param name="param">If not null, appends 'param: s'. By default appends only 's'. If "null", appends 'null, s'.</param>
	/// <param name="noComma">Don't append ', '. Use for the first parameter. If false, does not append only if b.Length is less than 2.</param>
	public static StringBuilder AppendOtherArg(this StringBuilder t, string s, string param = null, bool noComma = false) {
		Debug.Assert(!s.NE());
		_AppendArgPrefix(t, param, noComma);
		t.Append(s);
		return t;
	}

	static void _AppendArgPrefix(StringBuilder t, string param, bool noComma) {
		if (!noComma && t.Length > 1 && t[^1] is not ('(' or '[' or '{' or '<' or ',')) t.Append(", ");
		if (param != null) t.Append(param).Append(param == "null" ? ", " : ": ");
	}

	/// <summary>
	/// Appends waitTime. If !orThrow, appends "-" if need.
	/// </summary>
	public static StringBuilder AppendWaitTime(this StringBuilder t, string waitTime, bool orThrow) {
		if (waitTime.NE()) waitTime = "8e88";
		if (!orThrow && waitTime != "0" && !waitTime.Starts('-')) t.Append('-');
		t.Append(waitTime);
		return t;
	}

	/// <summary>
	/// If some 'use' are true, formats a flags argument like "Enum.Flag1|Enum.Flag2" and returns true.
	/// </summary>
	public static bool FormatFlags<T>(out string s, params (bool use, T flag)[] af) where T : unmanaged, Enum {
		if (!af.Any(o => o.use)) { s = null; return false; }
		s = string.Join('|', af.Where(o => o.use).Select(o => typeof(T).Name + "." + o.flag));
		return true;
	}

	/// <summary>
	/// If some 'flags' checkbox controls are checked, formats a flags argument like "Enum.Flag1|Enum.Flag2" and returns true. Ignores null controls.
	/// </summary>
	public static bool FormatFlags<T>(out string s, params (CheckBox use, T flag)[] af) where T : unmanaged, Enum {
		if (!af.Any(o => o.use?.IsChecked == true)) { s = null; return false; }
		s = string.Join('|', af.Where(o => o.use?.IsChecked == true).Select(o => typeof(T).Name + "." + o.flag));
		return true;
	}

	/// <summary>
	/// Returns true if s is like '@"*"' or '$"*"' or '$@"*"' or '@$"*"'.
	/// s can be null.
	/// </summary>
	public static bool IsVerbatim(string s, out int prefixLength) {
		prefixLength = (s.Like(false, "@\"*\"", "$\"*\"", "$@\"*\"", "@$\"*\"") + 1) / 2;
		return prefixLength > 0;
	}

	/// <summary>
	/// If s contains \ and no newlines/controlchars: replaces all " with "", prepends @", appends " and returns true.
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static bool MakeVerbatim(ref string s) {
		if (!s.Contains('\\') || s.RxIsMatch(@"[\x00-\x1F\x85\x{2028}\x{2029}]")) return false;
		s = "@\"" + s.Replace("\"", "\"\"") + "\"";
		return true;
	}

	/// <summary>
	/// If s has *? characters, prepends "**t ".
	/// s can be null.
	/// </summary>
	public static string EscapeWildex(string s) {
		if (wildex.hasWildcardChars(s)) s = "**t " + s;
		return s;
	}

	/// <summary>
	/// If s has *? characters, prepends "**t ".
	/// But if s has single * character, converts to "**r regex" that ignores it. Because single * often is used to indicate unsaved state.
	/// If canMakeVerbatim, finally calls <see cref="MakeVerbatim"/>.
	/// s can be null.
	/// </summary>
	public static string EscapeWindowName(string s, bool canMakeVerbatim) {
		if (s == null) return s;
		if (wildex.hasWildcardChars(s)) {
			int i = s.IndexOf('*');
			if (i >= 0 && s.IndexOf('*', i + 1) < 0) {
				s = "**r " + regexp.escapeQE(s[..i]) + @"\*?" + regexp.escapeQE(s[++i..]);
			} else s = "**t " + s;
		}
		if (canMakeVerbatim) MakeVerbatim(ref s);
		return s;
	}
	//CONSIDER: just remove *, and at run time find window with or without *.
	//	Now 2 problems: 1. The code looks ugly. 2. If recorded without *, at run time will not find with *.
	//	But then problem: what if user wants to find exactly what is specified?
	//	But maybe currently is the best. Users will learn and later will insert this regex when recorded without *.

	/// <summary>
	/// Returns true if newRawValue does not match wildex tbValue, unless contains is like $"..." or $@"...".
	/// </summary>
	/// <param name="tbValue">A wildex string, usually from a TextBox control. Can be raw or verbatim. Can be null.</param>
	/// <param name="newRawValue">New raw string, not wildex. Can be null.</param>
	public static bool ShouldChangeTextBoxWildex(string tbValue, string newRawValue) {
		tbValue ??= "";
		if (newRawValue == null) newRawValue = "";
		if (IsVerbatim(tbValue, out _)) {
			if (tbValue[0] == '$') return false;
			tbValue = tbValue[2..^1].Replace("\"\"", "\"");
		}
		wildex x = tbValue;
		return !x.Match(newRawValue);
	}

	/// <summary>
	/// Replaces known non-constant window class names with wildcard. Eg "WindowsForms10.EDIT..." with "*.EDIT.*".
	/// </summary>
	/// <param name="s">Can be null.</param>
	/// <param name="escapeWildex">If didn't replace, call <see cref="EscapeWildex"/>.</param>
	public static string StripWndClassName(string s, bool escapeWildex) {
		if (!s.NE()) {
			int n = s.RxReplace(@"^WindowsForms\d+(\..+?\.).+", "*$1*", out s);
			if (n == 0) n = s.RxReplace(@"^(HwndWrapper\[.+?;|Afx:).+", "$1*", out s);
			if (escapeWildex && n == 0) s = EscapeWildex(s);
		}
		return s;
	}

	#endregion

	#region formatters

	public class WindowFindCodeFormatter
	{
		public string nameW, classW, programW, containsW, alsoW, waitW;
		public bool hiddenTooW, cloakedTooW;
		public string idC, nameC, classC, alsoC, skipC, nameC_comments, classC_comments;
		public bool hiddenTooC;
		public bool NeedWindow = true, NeedControl, Throw, Activate, Test;
		public string CodeBefore, VarWindow = "w", VarControl = "c";

		public string Format() {
			if (!(NeedWindow || NeedControl)) return CodeBefore;
			var b = new StringBuilder(CodeBefore);
			if (CodeBefore != null && !CodeBefore.Ends('\n')) b.AppendLine();

			bool orThrow = Throw && !Test, activate = Activate && !Test;

			if (NeedWindow) {
				b.Append(Test ? "wnd " : "var ").Append(VarWindow);
				if (Test) b.AppendLine(";").Append(VarWindow);
				b.Append(" = wnd.find(");

				bool orThrowW = orThrow || NeedControl || activate;
				bool isWait = waitW != null && !Test;

				if (isWait) b.AppendWaitTime(waitW, orThrowW); else if (orThrowW) b.Append('0');
				b.AppendStringArg(nameW);
				int m = 0;
				if (classW != null) m |= 1;
				if (programW != null) m |= 2;
				if (m != 0) b.AppendStringArg(classW);
				if (programW != null) {
					if (!programW.Starts("WOwner.")) b.AppendStringArg(programW);
					else if (!Test) b.AppendOtherArg(programW);
					else m &= ~2;
				}
				if (FormatFlags(out var s1, (hiddenTooW, WFlags.HiddenToo), (cloakedTooW, WFlags.CloakedToo))) b.AppendOtherArg(s1, m < 2 ? "flags" : null);
				if (alsoW != null) b.AppendOtherArg(alsoW, "also");
				if (containsW != null) b.AppendStringArg(containsW, "contains");

				b.Append(')');
				if (activate) b.Append(isWait ? ".Activate(1)" : ".Activate()");
				b.Append(';');
			}

			if (NeedControl) {
				if (NeedWindow) b.AppendLine();
				if (!Test) b.Append("var ").Append(VarControl).Append(" = ");
				b.Append(VarWindow).Append(".Child(");
				if (!Test) {
					if (waitW is not (null or "0")) b.Append(orThrow ? "1" : "-1"); else if (orThrow) b.Append('0');
				}
				if (nameC != null) b.AppendStringArg(nameC);
				if (classC != null) b.AppendStringArg(classC, nameC == null ? "cn" : null);
				if (FormatFlags(out var s1, (hiddenTooC, WCFlags.HiddenToo))) b.AppendOtherArg(s1, nameC == null || classC == null ? "flags" : null);
				if (idC != null) b.AppendOtherArg(idC, "id");
				if (alsoC != null) b.AppendOtherArg(alsoC, "also");
				if (skipC != null) b.AppendOtherArg(skipC, "skip");
				b.Append(");");

				if (!Test && nameC == null) { //if no control name, append // classC_comments nameC_comments
					string sn = nameC == null ? nameC_comments : null, sc = classC == null ? classC_comments : null;
					int m = 0; if (!sn.NE()) m |= 1; if (!sc.NE()) m |= 2;
					if (m != 0) {
						b.Append(" // ");
						if (0 != (m & 2)) b.Append(sc.Limit(70));
						if (0 != (m & 1)) {
							if (0 != (m & 2)) b.Append(' ');
							b.AppendStringArg(sn.Limit(100).RxReplace(@"^\*\*\*\w+ (.+)", "$1"), noComma: true);
						}
					}
				}
			}

			if (!orThrow && !Test && !(Activate && !NeedControl))
				b.AppendLine().Append("if(").Append(NeedControl ? VarControl : VarWindow).Append(".Is0) { print.it(\"not found\"); }");

			return b.ToString();
		}
	}

	#endregion

	#region misc

	/// <summary>
	/// Gets control id. Returns true if it can be used to identify the control in window wWindow.
	/// </summary>
	public static bool GetUsefulControlId(wnd wControl, wnd wWindow, out int id) {
		id = wControl.ControlId;
		if (id == 0 || id == -1 || id > 0xffff || id < -0xffff) return false;
		//info: some apps use negative ids, eg FileZilla. Usually >= -0xffff. Id -1 is often used for group buttons and separators.
		//if(id == (int)wControl) return false; //.NET forms, delphi. //possible coincidence //rejected, because window handles are > 0xffff
		Debug.Assert((int)wControl > 0xffff);

		//if(wWindow.Child(id: id) != wControl) return false; //problem with combobox child Edit that all have id 1001
		if (wWindow.ChildAll(id: id).Length != 1) return false; //note: searching only visible controls; else could find controls with same id in hidden pages of tabbed dialog.
		return true;
	}

	/// <summary>
	/// Calls EventManager.RegisterClassHandler for CheckBox.CheckedEvent, CheckBox.UncheckedEvent, TextBox.TextChangedEvent and optionally ComboBox.SelectionChangedEvent.
	/// Call from static ctor of KDialogWindow-based classes.
	/// The specified event handler will be called on events of any of these controls in all dialogs of T type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="changed">Called on event.</param>
	/// <param name="comboToo">Register for ComboBox too.</param>
	public static void OnAnyCheckTextBoxValueChanged<T>(Action<T, object> changed, bool comboToo = false) where T : KDialogWindow {
		var h = new RoutedEventHandler((sender, e) => {
			//print.it(sender, e.Source, e.OriginalSource);
			var source = e.Source;
			if (source == e.OriginalSource) {
				switch (source) {
				case CheckBox:
				case TextBox:
				case ComboBox:
					changed(sender as T, source);
					break;
				}
			}
		});
		EventManager.RegisterClassHandler(typeof(T), ToggleButton.CheckedEvent, h);
		EventManager.RegisterClassHandler(typeof(T), ToggleButton.UncheckedEvent, h);
		EventManager.RegisterClassHandler(typeof(T), TextBoxBase.TextChangedEvent, h);
		if (comboToo) EventManager.RegisterClassHandler(typeof(T), Selector.SelectionChangedEvent, h);
	}

	/// <summary>
	/// From path gets name and various path formats (raw, unexpanded, shortcut) for inserting in code. If shortcut, also gets arguments. Supports ":: ITEMIDLIST".
	/// </summary>
	public class PathInfo
	{
		public string filePath, lnkPath, fileUnexpanded, lnkUnexpanded;
		string _name, _name2, _args;

		public PathInfo(string path) {
			filePath = path;
			_name = pathname.getNameNoExt(path);
			if (path.Ends(".lnk", true)) {
				try {
					var g = shortcutFile.open(path);
					string target = g.TargetAnyType;
					if (target.Starts("::")) {
						using var pidl = Pidl.FromString(target);
						_name2 = pidl.ToShellString(SIGDN.NORMALDISPLAY);
					} else {
						_args = g.Arguments;
						if (!target.Ends(".exe", true) || _name.Contains("Shortcut"))
							_name2 = pathname.getNameNoExt(target);
					}
					lnkPath = path;
					filePath = target;
				}
				catch { }
			}

			_Format(ref filePath, out fileUnexpanded);
			if (lnkPath != null) _Format(ref lnkPath, out lnkUnexpanded);

			static void _Format(ref string s, out string unexpanded) {
				if (folders.unexpandPath(s, out unexpanded, out var sn) && !sn.NE()) unexpanded = unexpanded + " + " + _Str(sn);
				s = _Str(s);
			}

			static string _Str(string s) {
				if (!MakeVerbatim(ref s)) s = s.Escape(quote: true);
				return s;
			}
		}

		/// <summary>
		/// If is shortcut or can unexpand path, shows dialog and returns: 0 cancel, 1 use filePath, 2 use fileUnexpanded, 2 use lnkPath, 4 use lnkUnexpanded.
		/// Else returns 1 (use filePath).
		/// </summary>
		/// <param name="owner"></param>
		public int SelectFormatUI(AnyWnd owner = default) {
			if (lnkPath != null || fileUnexpanded != null) {
				var b = new StringBuilder();
				_Append("1 Path", filePath);
				_Append("|2 Unexpanded path", fileUnexpanded);
				_Append("|3 Shortcut path", lnkPath);
				_Append("|4 Unexpanded shortcut path", lnkUnexpanded);
				return dialog.show("Path format", buttons: b.ToString(), flags: DFlags.CommandLinks | DFlags.XCancel | DFlags.CenterMouse, owner: owner);

				void _Append(string label, string path) {
					if (path != null) b.Append(label).Append('\n').Append(path.Limit(50));
				}
			}
			return 1;
		}

		//static bool s_defUnexpanded, s_defLnk; //could be used to set default button depending on previous choice

		/// <summary>
		/// Gets path/name/args that match or are nearest to the return value of <see cref="SelectFormatUI"/>.
		/// Paths are unexpanded/escaped/enclosed, like <c>@"x:\a\b.c"</c> or <c>folders.Example + @"a\b.c"</c>.
		/// </summary>
		public (string path, string name, string args) GetResult(int i) => (
			i switch { 1 => filePath, 2 => fileUnexpanded ?? filePath, 3 => lnkPath ?? filePath, 4 => lnkUnexpanded ?? lnkPath ?? filePath, _ => null },
			i <= 2 ? _name2 ?? _name : _name,
			i <= 2 ? _args : null
			);
	}

	#endregion

	#region OnScreenRect

	/// <summary>
	/// Creates standard <see cref="osdRect"/>.
	/// </summary>
	public static osdRect CreateOsdRect(int thickness = 4) => new() { Color = 0xFFFF0000, Thickness = thickness }; //red

	/// <summary>
	/// Briefly shows standard blinking on-screen rectangle.
	/// </summary>
	public static void ShowOsdRect(RECT r, bool error = false, bool limitToScreen = false) {
		int thick = error ? 6 : 2;
		var osr = new osdRect { Color = 0xFFFFFF00, Thickness = thick * 2 }; //yellow
		r.Inflate(thick, thick); //2 pixels inside, 2 outside
		if (limitToScreen) {
			var k = screen.of(r).Rect;
			r.Intersect(k);
		}
		osr.Rect = r;
		t_hideCapturingRect = true;
		osr.Show();

		int i = 0;
		timerm.every(250, t => {
			if (i++ < 5) {
				osr.Hwnd.ZorderTop();
				osr.Color = (i & 1) != 0 ? (error ? 0xFFFF0000 : 0xFF0000FF) : 0xFFFFFF00; //(red : blue) : yellow
			} else {
				t.Stop();
				osr.Dispose();
				t_hideCapturingRect = false;
			}
		});
	}

	[ThreadStatic] static bool t_hideCapturingRect;

	#endregion

	#region capture

	/// <summary>
	/// Common code for tools that capture UI objects with F3.
	/// </summary>
	public class CaptureWindowEtcWithHotkey
	{
		readonly KCheckBox _captureCheckbox;
		readonly Action _cbCapture;
		readonly Func<RECT?> _cbGetRect;
		HwndSource _hs;
		timerm _timer;
		long _prevTime;
		//wnd _prevWnd;
		osdRect _osr;
		bool _capturing;
		const string c_propName = "Au.Capture";
		readonly static int s_stopMessage = Api.RegisterWindowMessage(c_propName);
		const int c_hotkeyId = 1623031890;

		/// <param name="captureCheckbox">Checkbox that turns on/off capturing.</param>
		/// <param name="getRect">Called to get rectangle of object from mouse. Can return default to hide rectangle.</param>
		public CaptureWindowEtcWithHotkey(KCheckBox captureCheckbox, Action capture, Func<RECT?> getRect) {
			_captureCheckbox = captureCheckbox;
			_cbCapture = capture;
			_cbGetRect = getRect;
		}

		/// <summary>
		/// Starts or stops capturing.
		/// Does nothing if already in that state.
		/// </summary>
		public bool Capturing {
			get => _capturing;
			set {
				if (value == _capturing) return;
				var wDialog = _captureCheckbox.Hwnd();
				if (value) {
					//let other dialogs stop capturing
					//could instead use a static collection, but this code allows to have such tools in multiple processes, although currently it not used
					wDialog.Prop.Set(c_propName, 1);
					wnd.find(null, "HwndWrapper[*", flags: WFlags.HiddenToo | WFlags.CloakedToo, also: o => {
						if (o != wDialog && o.Prop[c_propName] == 1) o.Send(s_stopMessage);
						return false;
					});

					if (!(Api.RegisterHotKey(wDialog, c_hotkeyId, 0, KKey.F3) | Api.RegisterHotKey(wDialog, c_hotkeyId + 1, 2, KKey.F3))) {
						dialog.showError("Failed to register hotkey F3 and Ctrl+F3", owner: wDialog);
						return;
					}
					_capturing = true;

					if (_hs == null) {
						_hs = PresentationSource.FromDependencyObject(_captureCheckbox) as HwndSource;
						_hs.Disposed += (_, _) => {
							Capturing = false;
							_osr?.Dispose();
						};
					}
					_hs.AddHook(_WndProc);

					//set timer that shows UI element rect
					if (_timer == null) {
						_osr = TUtil.CreateOsdRect(2);
						_timer = new timerm(t => {
							//Don't capture too frequently.
							//	Eg if the callback is very slow. Or if multiple timer messages are received without time interval (possible in some conditions).
							long t1 = perf.ms, t2 = t1 - _prevTime; _prevTime = t1; if (t2 < 100) return;

							//show rect of UI object from mouse
							wnd w = wnd.fromMouse(WXYFlags.NeedWindow);
							RECT? r = default;
							if (!(w.Is0 || w == wDialog || w.OwnerWindow == wDialog)) {
								r = _cbGetRect();

								//F3 does not work if this process has lower UAC IL than the foreground process. Normally editor is admin, but if portable etc...
								//Shift+F3 too. But Ctrl+F3 works.
								//if (w!=_prevWnd && w.IsActive) {
								//	w = _prevWnd;
								//	if(w.UacAccessDenied)print.it("F3 ");
								//}
							}
							if (r.HasValue && !t_hideCapturingRect) {
								var rr = r.GetValueOrDefault();
								rr.Inflate(1, 1); //1 pixel inside, 1 outside
								_osr.Rect = rr;
								_osr.Show();
								//FUTURE: also display UI object role.
								//	Eg often user wants to capture a LINK, but there is a child TEXT.
								//	Also mouse x y in it. For MouseClick and VirtualClick.
							} else {
								_osr.Visible = false;
							}
						});
					}
					_timer.Every(250);
				} else {
					_capturing = false;
					_hs.RemoveHook(_WndProc);
					Api.UnregisterHotKey(wDialog, c_hotkeyId);
					Api.UnregisterHotKey(wDialog, c_hotkeyId + 1);
					wDialog.Prop.Remove(c_propName);
					_timer.Stop();
					_osr.Hide();
				}
			}
		}

		nint _WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
			if (msg == s_stopMessage) {
				handled = true;
				_captureCheckbox.IsChecked = false;
			} else if (msg == Api.WM_HOTKEY && (wParam == c_hotkeyId || wParam == c_hotkeyId + 1)) {
				handled = true;
				_cbCapture();
			}
			return default;
		}
	}

	#endregion

	#region test

	/// <summary>
	/// Executes test code that finds an object in window.
	/// Returns the found object and speed. On error speed negative.
	/// </summary>
	/// <param name="code">
	/// Must start with one or more lines that find window or control and set wnd variable named <i>wndVar</i>. Can be any code.
	/// The last line must be a 'find object' function call, like <c>uiimage.find(...);</c>. No 'var x = ', no 'not found' exception, no wait, no action.
	/// </param>
	/// <param name="wndVar">Name of wnd variable of the window or control in which to search.</param>
	/// <param name="w">Window or control in which to search.</param>
	/// <param name="tInfo">For error info.</param>
	/// <param name="getRect">Callback function that returns object's rectangle in screen. Called when object has been found.</param>
	/// <param name="activateWindow">Between finding window and object in it, activate the found window and wait 200 ms.</param>
	/// <remarks>
	/// The test code is executed in this thread. Else would get invalid UI element etc. If need, caller can use Task.Run.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// var r = await TUtil.RunTestFindObject(code, _wndVar, _wnd, _info, o => (o as elm).Rect);
	/// ]]></code>
	/// </example>
	public static (object obj, long speed, string sSpeed) RunTestFindObject(
		string code, string wndVar, wnd w, KSciInfoBox tInfo, Func<object, RECT> getRect, bool activateWindow = false) {
		if (code.NE()) return default;
		wnd dlg = tInfo.Hwnd();
		bool dlgWasActive = dlg.IsActive, dlgMinimized = false;
		//tInfo.zText = "Searching..."; Api.UpdateWindow(tInfo.Hwnd); //no, in most cases it just flickers for 5-50 ms. And cannot use a timer because this thread is blocked while searching.

		//print.it(code);
		//perf.first();

		var code0 = code;
		var b = new StringBuilder();
		b.AppendLine(@"static object[] __TestFunc__() {");
		if (activateWindow) b.Append("((wnd)").Append(w.Window.Handle).Append(").ActivateL(); 200.ms(); ");
		b.AppendLine("var _p_ = perf.local();");
		b.AppendLine("#line 1");
		var lines = code.Lines(true);
		int lastLine = lines.Length - 1;
		for (int i = 0; i < lastLine; i++) b.AppendLine(lines[i]);
		b.AppendLine("_p_.Next(); var _a_ =");
		b.AppendLine("#line " + (lastLine + 1));
		b.AppendLine(lines[lastLine]);
		b.AppendLine("_p_.Next();");
		b.AppendLine($"return new object[] {{ _p_.ToArray(), _a_, {wndVar} }};");
		b.AppendLine("\r\n}");
		code = b.ToString(); //print.it(code);

		(long[] speed, object obj, wnd w) r = default;
		try {
			if (!Au.Compiler.Scripting.Compile(code, out var c, addUsings: true, addGlobalCs: true, wrapInClass: true, dll: true)) {
				Debug_.Print("---- CODE ----\r\n" + code + "--------------");
				tInfo.InfoError("Errors:", $"{c.errors}\r\n\r\n<Z #C0C0C0><b>Code:<><>\r\n<code>{code0}</code>");
				//shows code too, because it may be different than in the code box
				return (null, -1, null);
			} else {
				var rr = (object[])c.method.Invoke(null, null); //use array because fails to cast tuple, probably because in that assembly it is new type
				r = ((long[])rr[0], rr[1], (wnd)rr[2]);
			}

			//note: the script runs in this thread.
			//	Bad: blocks current UI thread. But maybe not so bad. Good: don't need to disable the Test button or dlg.
			//	Good: we get valid elm result. Else it would be marshalled for a script thread.
		}
		catch (Exception e) {
			if (e is TargetInvocationException tie) e = tie.InnerException;
			string s1, s2;
			if (e is NotFoundException) { //info: throws only when window not found. This is to show time anyway when elm etc not found.
				s1 = "Window not found";
				s2 = "Tip: If part of window name changes, replace it with *";
			} else {
				s1 = e.GetType().Name;
				s2 = e.Message.RxReplace(@"^Exception of type '.+?' was thrown. ", "");
				if (e.StackTrace.RxMatch(@"(?m)^\s*( at .+?)\(.+\R\s+\Qat __script__.__TestFunc__()\E", 1, out string s3)) s1 += s3;
			}
			tInfo.InfoError(s1, s2);
			return (null, -2, null);
		}

		//perf.nw();
		//print.it(r);

		static double _SpeedMcsToMs(long tn) => Math.Round(tn / 1000d, tn < 1000 ? 2 : (tn < 10000 ? 1 : 0));
		double t0 = _SpeedMcsToMs(r.speed[0]), t1 = _SpeedMcsToMs(r.speed[1]); //times of wnd.find and Object.Find
		string sSpeed;
		if (lastLine == 1 && lines[0] == "wnd w;") sSpeed = t1.ToS() + " ms"; //only wnd.find: "wnd w;\r\nw = wnd.find(...);"
		else sSpeed = t0.ToS() + " + " + t1.ToS() + " ms";

		if (r.obj is wnd w1 && w1.Is0) r.obj = null;
		if (r.obj != null) {
			var re = getRect(r.obj);
			ShowOsdRect(re);

			//if dlg covers the found object, temporarily minimize it (may be always-on-top) and activate object's window. Never mind owners.
			var wTL = r.w.Window;
			if (dlgMinimized = dlg.Rect.IntersectsWith(re) && !r.w.IsOfThisThread && !dlg.IsMinimized) {
				dlg.ShowMinimized(noAnimation: true);
				wTL.ActivateL();
				wait.doEvents(1500);
			}
		}

		if (dlgWasActive || dlgMinimized) {
			int after = activateWindow && !dlgMinimized && r.w == w ? 1500 : 300;
			timerm.after(after, _ => {
				if (dlgMinimized) dlg.ShowNotMinimized(noAnimation: true);
				if (dlgWasActive) dlg.ActivateL();
			});
		}

		if (r.w != w && !r.w.Is0) {
			tInfo.InfoError("Finds another " + (r.w.IsChild ? "control" : "window"), $"<i>Need:<>  {w}\r\n<i>Found:<>  {r.w}");
			ShowOsdRect(r.w.Rect, error: true, limitToScreen: true);
			//FUTURE: show list of objects inside the wanted window, same as in the Dwnd 'contains' combo. Let user choose. Then update window code quickly.
			return (null, -3, null);
		}

		if (r.obj != null) {
			tInfo.InfoInfo("Found", null, ",  speed " + sSpeed);
		} else {
			tInfo.InfoError("Not found", null, ",  speed " + sSpeed);
		}

		return (r.obj, r.speed[1], sSpeed);
	}

	/// <summary>
	/// Executes action code for a found UI element etc.
	/// </summary>
	/// <param name="obj">The element. The function passes it to the test script.</param>
	/// <param name="code">Code like "Method(arguments)". The function prepends "obj." and appends ";".</param>
	/// <param name="tInfo">For errors.</param>
	public static void RunTestAction(object obj, string code, KSciInfoBox tInfo) {
		var code0 = code;
		code = $@"static void __TestFunc__({obj.GetType()} obj) {{
#line 1
obj.{code};
}}";
		//print.it(code);

		try {
			if (!Au.Compiler.Scripting.Compile(code, out var c, addUsings: true, addGlobalCs: true, wrapInClass: true, dll: true)) {
				tInfo.InfoError("Errors:", $"{c.errors}\r\n\r\n<Z #C0C0C0><b>Code:<><>\r\n<code>obj.{code0};</code>");
			} else {
				c.method.Invoke(null, new[] { obj });
			}
		}
		catch (Exception e) {
			if (e is TargetInvocationException tie) e = tie.InnerException;
			tInfo.InfoError("Action failed", e.GetType().Name + ". " + e.Message.RxReplace(@"^Exception of type '.+?' was thrown. ", ""));
		}
	}

	#endregion

	#region info

	public static void InfoError(this KSciInfoBox t, string header, string text, string headerSmall = null) {
		t.zText = $"<Z #F0E080><b>{header}<>{headerSmall}<>\r\n{text}";
		t.ZSuspendElems();
	}

	public static void InfoInfo(this KSciInfoBox t, string header, string text, string headerSmall = null) {
		t.zText = $"<Z #C0E0C0><b>{header}<>{headerSmall}<>\r\n{text}";
		t.ZSuspendElems();
	}

	public static void Info(this KSciInfoBox t, FrameworkElement e, string name, string text) {
		text = CommonInfos.PrependName(name, text);
		t.ZAddElem(e, text);
	}

	public static void InfoC(this KSciInfoBox t, ContentControl k, string text) => Info(t, k, _ControlName(k), text);

	public static void InfoCT(this KSciInfoBox t, KCheckTextBox k, string text, bool isWildex = false, string wildexPart = null) {
		text = CommonInfos.PrependName(_ControlName(k.c), text);
		if (isWildex) text = CommonInfos.AppendWildexInfo(text, wildexPart);
		t.ZAddElem(k.c, text);
		t.ZAddElem(k.t, text);
	}

	public static void InfoCO(this KSciInfoBox t, KCheckComboBox k, string text) {
		text = CommonInfos.PrependName(_ControlName(k.c), text);
		t.ZAddElem(k.c, text);
		t.ZAddElem(k.t, text);
	}

	/// <summary>
	/// Returns k text without '_' character used for Alt+underline.
	/// </summary>
	static string _ControlName(ContentControl k) => StringUtil.RemoveUnderlineChar(k.Content as string, '_');

	/// <summary>
	/// Can be used by tool dialogs to display common info in <see cref="KSciInfoBox"/> control.
	/// </summary>
	public class CommonInfos
	{
		KSciInfoBox _control;
		RegexWindow _regexWindow;

		public CommonInfos(KSciInfoBox control) {
			_control = control;
			_control.ZTags.AddLinkTag("+regex", o => _Regex(o));
		}

		void _Regex(string _) {
			_regexWindow ??= new RegexWindow();
			if (_regexWindow.Hwnd.Is0) {
				_regexWindow.ShowByRect(_control.Hwnd().Window, Dock.Bottom);
			} else _regexWindow.Hwnd.ShowL(true);
		}

		/// <summary>
		/// Formats "name - text" string, where name is bold: <c>"<b>" + name + "<> - " + text</c>.
		/// </summary>
		public static string PrependName(string name, string text) => "<b>" + name + "<> - " + text;

		public static string AppendWildexInfo(string s, string part = null) => s + "\r\n" + (part ?? "The text") +
@" is <help articles/Wildcard expression>wildcard expression<>. Can be <+regex>regex<>, like <mono>**rc regex<>.
Examples:
<mono>whole text
*end
start*
*middle*
time ??:??
**t literal text
**c case-sensitive text
**tc case-sensitive literal
**r regular expression
**rc case-sensitive regex
**n not this
**m this||or this||**r or this regex||**n and not this
**m(^^^) this^^^or this^^^or this
@""C# verbatim string""
<>";

		public const string c_alsoParameter = @"<i>also<> lambda.
Can be multiline.
Can use global usings and classes/functions from file ""global.cs"".";
	}

	/// <summary>
	/// Auto-creates and shows click-closed system-colored tooltip below element e.
	/// </summary>
	public static void InfoTooltip(ref KPopup p, UIElement e, string text, Dock side = Dock.Bottom) {
		if (p == null) {
			p = new(WS.POPUP | WS.BORDER, shadow: true) { Content = new Label(), ClickClose = KPopup.CC.Anywhere };
			p.Border.Background = SystemColors.InfoBrush;
		}
		(p.Content as Label).Content = text;
		p.ShowByRect(e, side);
	}

	///// <summary>
	///// Auto-creates and shows tooltip below element e. The tooltip has system colors, not WPF colors.
	///// </summary>
	//public static void InfoTooltip(ref ToolTip tt, UIElement e, string text) {
	//	tt ??= new ToolTip { StaysOpen = false, Placement = PlacementMode.Bottom, Background = SystemColors.InfoBrush, Foreground = SystemColors.InfoTextBrush };
	//	tt.PlacementTarget = e;
	//	tt.Content = text;
	//	tt.IsOpen = false;
	//	tt.IsOpen = true;
	//}

	#endregion
}

///// <summary>
///// All tool dialogs that insert code in editor should inherit from this class.
///// Adds ZResultCode property; child class sets, app can get, this class inserts text in editor on OK.
///// </summary>
//class CodeToolDialog : KDialogWindow
//{
//	protected wpfBuilder _builder;
//	public virtual string ZResultCode { get; protected set; }

//	public CodeToolDialog() {
//		_builder.OkApply += o => { InsertCode.Statements(ZResultCode); };
//	}
//}
