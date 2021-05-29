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
using System.Windows;
using System.Windows.Controls;
using System.Linq;

using Au.Types;
using Au.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Au.Util;

namespace Au.Tools
{
	static class TUtil
	{
		#region text

		/// <summary>
		/// Appends ', ' and string argument to this StringBuilder.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="s">Argument value. If null, appends 'null'. If verbatim (like '@"text"'), appends 's'. Else appends '"escaped s"'; can make verbatim.</param>
		/// <param name="param">If not null, appends 'param: s'. By default appends only 's'. If "null", appends 'null, s'.</param>
		/// <param name="noComma">Don't append ', '. Use for the first parameter. If false, does not append only if b.Length is less than 2.</param>
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
			if (!noComma && t.Length > 1) t.Append(", ");
			if (param != null) t.Append(param).Append(param == "null" ? ", " : ": ");
		}

		/// <summary>
		/// If some 'flags' checkboxes checked, appends ', ' and the checked flags.
		/// Returns true if checked.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="param">If not null, appends 'param: flags'. By default appends only 'flags'. If "null", appends 'null, s'.</param>
		/// <param name="flagsEnum">Name of the flags enum.</param>
		/// <param name="a"></param>
		public static bool AppendFlags(this StringBuilder t, string param, string flagsEnum, params (KCheckBox c, string flag)[] a) {
			var g = new (bool use, string flag)[a.Length];
			int i = 0; foreach (var (c, flag) in a) g[i++] = (c.IsChecked, flag);
			return AppendFlags(t, param, flagsEnum, g);
		}

		/// <summary>
		/// If some 'use' true, appends ', ' and the used flags.
		/// Returns true if appended.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="param">If not null, appends 'param: flags'. By default appends only 'flags'. If "null", appends 'null, s'.</param>
		/// <param name="flagsEnum">Name of the flags enum.</param>
		/// <param name="a"></param>
		public static bool AppendFlags(this StringBuilder t, string param, string flagsEnum, params (bool use, string flag)[] a) {
			bool isFlags = false;
			foreach (var v in a) {
				if (!v.use) continue;
				var flag = flagsEnum + "." + v.flag;
				if (!isFlags) {
					isFlags = true;
					AppendOtherArg(t, flag, param);
				} else {
					t.Append('|').Append(flag);
				}
			}
			return isFlags;
		}

		/// <summary>
		/// Appends waitTime. If !orThrow, appends "-" if need.
		/// </summary>
		public static StringBuilder AppendWaitTime(this StringBuilder t, string waitTime, bool orThrow) {
			if (waitTime.NE()) waitTime = "0"; else if (!orThrow && waitTime != "0" && !waitTime.Starts('-')) t.Append('-');
			t.Append(waitTime);
			return t;
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
			if (!s.Contains('\\') || s.RegexIsMatch(@"[\x00-\x1F\x85\x{2028}\x{2029}]")) return false;
			s = "@\"" + s.Replace("\"", "\"\"") + "\"";
			return true;
		}

		/// <summary>
		/// If s has *? characters, prepends "**t ".
		/// s can be null.
		/// </summary>
		public static string EscapeWildex(string s) {
			if (AWildex.HasWildcardChars(s)) s = "**t " + s;
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
			if (AWildex.HasWildcardChars(s)) {
				int i = s.IndexOf('*');
				if (i >= 0 && s.IndexOf('*', i + 1) < 0) {
					s = "**r " + ARegex.EscapeQE(s[..i]) + @"\*?" + ARegex.EscapeQE(s[++i..]);
				} else s = "**t " + s;
			}
			if (canMakeVerbatim) MakeVerbatim(ref s);
			return s;
		}

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
			AWildex x = tbValue;
			return !x.Match(newRawValue);
		}

		/// <summary>
		/// Replaces known non-constant window class names with wildcard. Eg "WindowsForms10.EDIT..." with "*.EDIT.*".
		/// </summary>
		/// <param name="s">Can be null.</param>
		/// <param name="escapeWildex">If didn't replace, call <see cref="EscapeWildex"/>.</param>
		public static string StripWndClassName(string s, bool escapeWildex) {
			if (!s.NE()) {
				int n = s.RegexReplace(@"^WindowsForms\d+(\..+?\.).+", "*$1*", out s);
				if (n == 0) n = s.RegexReplace(@"^(HwndWrapper\[.+?;|Afx:).+", "$1*", out s);
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
			public bool NeedWindow = true, NeedControl, Throw, Test;
			public string CodeBefore, VarWindow = "w", VarControl = "c";

			public string Format() {
				if (!(NeedWindow || NeedControl)) return CodeBefore;
				var b = new StringBuilder(CodeBefore);
				if (CodeBefore != null && !CodeBefore.Ends('\n')) b.AppendLine();

				bool orThrow = Throw && !Test;

				if (NeedWindow) {
					bool orThrowW = orThrow || NeedControl;
					bool isWait = waitW != null && !Test;

					b.Append(Test ? "AWnd " : "var ").Append(VarWindow);
					if (Test) b.AppendLine(";").Append(VarWindow);
					b.Append(" = ");

					if (isWait) {
						b.Append("AWnd.Wait(").AppendWaitTime(waitW, orThrowW);
						b.Append(", active: true");
					} else {
						if (orThrowW) b.Append('+');
						b.Append("AWnd.Find(");
					}

					b.AppendStringArg(nameW, noComma: !isWait);
					int m = 0;
					if (classW != null) m |= 1;
					if (programW != null) m |= 2;
					if (m != 0) b.AppendStringArg(classW);
					if (programW != null) {
						if (!programW.Starts("WOwner.")) b.AppendStringArg(programW);
						else if (!Test) b.AppendOtherArg(programW);
						else m &= ~2;
					}
					b.AppendFlags(m < 2 ? "flags" : null, nameof(WFlags), (hiddenTooW, nameof(WFlags.HiddenToo)), (cloakedTooW, nameof(WFlags.CloakedToo)));
					if (alsoW != null) b.AppendOtherArg(alsoW, "also");
					if (containsW != null) b.AppendStringArg(containsW, "contains");

					b.Append(");");
				}

				if (NeedControl) {
					if (NeedWindow) b.AppendLine();
					int m = 0;
					if (idC != null) m |= 1;
					else if (nameC != null) m |= 2;
					if (classC != null) m |= 4;
					if (alsoC != null) m |= 8;
					if (skipC != null) m |= 16;
					if (!Test) b.Append("var ").Append(VarControl).Append(" = ");
					if (orThrow) b.Append('+');
					b.Append(VarWindow).Append(".Child");
					if (m == 1) {
						b.Append("ById(").Append(idC);
						b.AppendFlags(null, nameof(WCFlags), (hiddenTooC, nameof(WCFlags.HiddenToo)));
					} else {
						b.Append('(');
						if (0 != (m & 1)) b.Append("\"***id ").Append(idC).Append('\"'); else b.AppendStringArg(nameC, noComma: true);
						if (0 != (m & 4)) b.AppendStringArg(classC);
						b.AppendFlags((0 == (m & 4)) ? "null" : null, nameof(WCFlags), (hiddenTooC, nameof(WCFlags.HiddenToo)));
						if (0 != (m & 8)) b.AppendOtherArg(alsoC, "also");
						if (0 != (m & 16)) b.AppendOtherArg(skipC, "skip");
					}

					b.Append(");");

					if (!Test && 0 == (m & 2)) { //if no control name, append // classC_comments nameC_comments
						string sn = 0 == (m & 2) ? nameC_comments : null, sc = 0 == (m & 4) ? classC_comments : null;
						m = 0; if (!sn.NE()) m |= 1; if (!sc.NE()) m |= 2;
						if (m != 0) {
							b.Append(" // ");
							if (0 != (m & 2)) b.Append(sc.Limit(70));
							if (0 != (m & 1)) {
								if (0 != (m & 2)) b.Append(' ');
								b.AppendStringArg(sn.Limit(100).RegexReplace(@"^\*\*\*\w+ (.+)", "$1"), noComma: true);
							}
						}
					}
				}

				if (!orThrow && !Test) b.AppendLine().Append("if(").Append(NeedControl ? VarControl : VarWindow).Append(".Is0) { AOutput.Write(\"not found\"); }");

				return b.ToString();
			}
		}

		#endregion

		#region misc

		/// <summary>
		/// Gets control id. Returns true if it can be used to identify the control in window wWindow.
		/// </summary>
		public static bool GetUsefulControlId(AWnd wControl, AWnd wWindow, out int id) {
			id = wControl.ControlId;
			if (id == 0 || id == -1 || id > 0xffff || id < -0xffff) return false;
			//info: some apps use negative ids, eg FileZilla. Usually >= -0xffff. Id -1 is often used for group buttons and separators.
			//if(id == (int)wControl) return false; //.NET forms, delphi. //possible coincidence //rejected, because window handles are > 0xffff
			Debug.Assert((int)wControl > 0xffff);

			//if(wWindow.Child("***id " + id) != wControl) return false; //problem with combobox child Edit that all have id 1001
			if (wWindow.ChildAll("***id " + id).Length != 1) return false; //note: searching only visible controls; else could find controls with same id in hidden pages of tabbed dialog.
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
				//AOutput.Write(sender, e.Source, e.OriginalSource);
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
				_name = APath.GetNameNoExt(path);
				if (path.Ends(".lnk", true)) {
					try {
						var g = AShortcutFile.Open(path);
						string target = g.TargetAnyType;
						if (target.Starts("::")) {
							using var pidl = Pidl.FromString(target);
							_name2 = pidl.ToShellString(SIGDN.NORMALDISPLAY);
						} else {
							_args = g.Arguments;
							if (!target.Ends(".exe", true) || _name.Contains("Shortcut"))
								_name2 = APath.GetNameNoExt(target);
						}
						lnkPath = path;
						filePath = target;
					}
					catch { }
				}

				_Format(ref filePath, out fileUnexpanded);
				if (lnkPath != null) _Format(ref lnkPath, out lnkUnexpanded);

				static void _Format(ref string s, out string unexpanded) {
					if (AFolders.UnexpandPath(s, out unexpanded, out var sn) && !sn.NE()) unexpanded = unexpanded + " + " + _Str(sn);
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
					_Append("|2 Path with AFolders", fileUnexpanded);
					_Append("|3 Shortcut path", lnkPath);
					_Append("|4 Shortcut path with AFolders", lnkUnexpanded);
					return ADialog.Show("Path format", buttons: b.ToString(), flags: DFlags.CommandLinks | DFlags.XCancel | DFlags.CenterMouse, owner: owner);

					void _Append(string label, string path) {
						if (path != null) b.Append(label).Append('\n').Append(path.Limit(50));
					}
				}
				return 1;
			}

			//static bool s_defUnexpanded, s_defLnk; //could be used to set default button depending on previous choice

			/// <summary>
			/// Gets path/name/args that match or are nearest to the return value of <see cref="SelectFormatUI"/>.
			/// Paths are unexpanded/escaped/enclosed, like <c>@"x:\a\b.c"</c> or <c>AFolders.Example + @"a\b.c"</c>.
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
		/// Creates standard <see cref="AOsdRect"/>.
		/// </summary>
		public static AOsdRect CreateOsdRect(int thickness = 4) => new() { Color = 0xFF8A2BE2, Thickness = thickness }; //Color.BlueViolet

		/// <summary>
		/// Briefly shows standard blinking on-screen rectangle.
		/// </summary>
		public static void ShowOsdRect(RECT r, bool limitToScreen = false) {
			var osr = CreateOsdRect();
			r.Inflate(2, 2); //2 pixels inside, 2 outside
			if (limitToScreen) {
				var k = AScreen.Of(r).Rect;
				r.Intersect(k);
			}
			osr.Rect = r;
			osr.Show();

			int i = 0;
			ATimer.Every(250, t => {
				if (i++ < 5) {
					osr.Color = (i & 1) != 0 ? 0xFFFFFF00 : 0xFF8A2BE2;
				} else {
					t.Stop();
					osr.Dispose();
				}
			});
		}

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
			ATimer _timer;
			long _prevTime;
			//AWnd _prevWnd;
			AOsdRect _osr;
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
						AWnd.Find(null, "HwndWrapper[*", flags: WFlags.HiddenToo | WFlags.CloakedToo, also: o => {
							if (o != wDialog && o.Prop[c_propName] == 1) o.Send(s_stopMessage);
							return false;
						});

						if (!(Api.RegisterHotKey(wDialog, c_hotkeyId, 0, KKey.F3) | Api.RegisterHotKey(wDialog, c_hotkeyId + 1, 2, KKey.F3))) {
							ADialog.ShowError("Failed to register hotkey F3 and Ctrl+F3", owner: wDialog);
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

						//set timer that shows AO rect
						if (_timer == null) {
							_osr = TUtil.CreateOsdRect();
							_timer = new ATimer(t => {
								//Don't capture too frequently.
								//	Eg if the callback is very slow. Or if multiple timer messages are received without time interval (possible in some conditions).
								long t1 = ATime.PerfMilliseconds, t2 = t1 - _prevTime; _prevTime = t1; if (t2 < 100) return;

								//show rect of UI object from mouse
								AWnd w = AWnd.FromMouse(WXYFlags.NeedWindow);
								RECT? r = default;
								if (!(w.Is0 || w == wDialog || w.OwnerWindow == wDialog)) {
									r = _cbGetRect();

									//F3 does not work if this process has lower UAC IL than the foreground process. Normally editor is admin, but if portable etc...
									//Shift+F3 too. But Ctrl+F3 works.
									//if (w!=_prevWnd && w.IsActive) {
									//	w = _prevWnd;
									//	if(w.UacAccessDenied)AOutput.Write("F3 ");
									//}
								}
								if (r.HasValue) {
									var rr = r.GetValueOrDefault();
									rr.Inflate(2, 2); //2 pixels inside, 2 outside
									_osr.Rect = rr;
									_osr.Show();
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
		/// Returns the found object and speed.
		/// </summary>
		/// <param name="code">
		/// Must start with one or more lines that find window or control and set AWnd variable named wndVar. Can be any code.
		/// The last line must be a 'find object' function call. Example: <c>AAcc.Find(...);</c>. Without 'var obj = ', without +, without Wait.
		/// </param>
		/// <param name="wndVar">Name of AWnd variable of the window or control in which to search.</param>
		/// <param name="wnd">Window or control in which to search.</param>
		/// <param name="bTest">The 'Test' button. This function disables it while executing code.</param>
		/// <param name="lSpeed">Label control that displays speed.</param>
		/// <param name="getRect">Callback function that returns object's rectangle in screen. Called when object has been found.</param>
		/// <remarks>
		/// The test code is executed in this thread. Else would get invalid AO etc. If need, caller can use Task.Run.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var r = await TUtil.RunTestFindObject(code, _wndVar, _wnd, _bTest, _lSpeed, o => (o as AAcc).Rect);
		/// ]]></code>
		/// </example>
		public static (object obj, long speed) RunTestFindObject(
			string code, string wndVar, AWnd wnd, Button bTest, Label lSpeed, Func<object, RECT> getRect, bool activateWindow = false) {
			if (code.NE()) return default;
			AWnd dialog = lSpeed.Hwnd();
			lSpeed.Content = "";

			//AOutput.Write(code);
			//APerf.First();

			//FUTURE: #line
			var b = new StringBuilder();
			b.AppendLine(@"static object[] __TestFunc__() {");
			if (activateWindow) b.Append("((AWnd)").Append(wnd.Window.Handle).Append(").ActivateL(); 200.ms(); ");
			b.AppendLine("var _p_ = APerf.Create();");
			var lines = code.Lines(true);
			int lastLine = lines.Length - 1;
			for (int i = 0; i < lastLine; i++) b.AppendLine(lines[i]);
			b.AppendLine("_p_.Next(); var _a_ =");
			b.AppendLine(lines[lastLine]);
			b.AppendLine($"_p_.Next(); return new object[] {{ _p_.ToArray(), _a_, {wndVar} }};");
			b.AppendLine("\r\n}");
			code = b.ToString(); //AOutput.Write(code);

			(long[] speed, object obj, AWnd wnd) r = default;
			bool ok = false;
			try {
				bTest.IsEnabled = false;
				if (!Au.Compiler.Scripting.Compile(code, out var c, wrap: true, load: true)) {
					ADebug_.Print("---- CODE ----\r\n" + code + "--------------");
					ADialog.ShowError("Errors in code", c.errors, owner: dialog, flags: DFlags.CenterOwner | DFlags.Wider/*, expandedText: code*/);
				} else {
					var rr = (object[])c.method.Invoke(null, null); //use array because fails to cast tuple, probably because in that assembly it is new type
					r = ((long[])rr[0], rr[1], (AWnd)rr[2]);
					ok = true;
				}

				//note: the script runs in this thread.
				//	Bad: blocks current UI thread. But maybe not so bad.
				//	Good: we get valid AAcc result. Else it would be marshalled for a script thread.
			}
			catch (Exception e) {
				if (e is TargetInvocationException tie) e = tie.InnerException;
				string s1, s2;
				if (e is NotFoundException) { s1 = "Window not found"; s2 = "Tip: If part of window name changes, replace it with *"; } //info: throws only when window not found. This is to show time anyway when acc etc not found.
				else { s1 = e.GetType().Name; s2 = e.Message; }
				ADialog.ShowError(s1, s2, owner: dialog, flags: DFlags.CenterOwner);
			}
			finally {
				bTest.IsEnabled = true;
			}
			if (!ok) return default;

			//APerf.NW();
			//AOutput.Write(r);

			static double _SpeedMcsToMs(long tn) => Math.Round(tn / 1000d, tn < 1000 ? 2 : (tn < 10000 ? 1 : 0));
			double t0 = _SpeedMcsToMs(r.speed[0]), t1 = _SpeedMcsToMs(r.speed[1]); //times of AWnd.Find and Object.Find
			string sTime;
			if (lastLine == 1 && lines[0].Length == 7) sTime = t1.ToStringInvariant() + " ms"; //only AWnd.Find: "AWnd w;\r\nw = AWnd.Find(...);"
			else sTime = t0.ToStringInvariant() + " + " + t1.ToStringInvariant() + " ms";

			if (r.obj is AWnd w1 && w1.Is0) r.obj = null;
			if (r.obj != null) {
				lSpeed.Foreground = SystemColors.ControlTextBrush;
				lSpeed.Content = sTime;
				var re = getRect(r.obj);
				TUtil.ShowOsdRect(re);

				//if dialog or its visible owners cover the found object, temporarily activate object's window
				foreach (var ow in dialog.Get.Owners(andThisWindow: true, onlyVisible: true)) {
					if (re.IntersectsWith(ow.Rect)) {
						r.wnd.Window.ActivateL();
						ATime.SleepDoEvents(1500);
						break;
					}
				}
			} else {
				//ADialog.Show("Not found", owner: dialog, flags: DFlags.OwnerCenter, icon: DIcon.Info, secondsTimeout: 2);
				lSpeed.Foreground = Brushes.Red;
				lSpeed.Content = "Not found,";
				ATimer.After(700, _ => lSpeed.Content = sTime);
			}

			dialog.ActivateL();

			if (r.wnd != wnd && !r.wnd.Is0) {
				ADialog.ShowWarning("The code finds another " + (r.wnd.IsChild ? "control" : "window"),
				$"Need:  {wnd}\r\n\r\nFound:  {r.wnd}",
				owner: dialog, flags: DFlags.CenterOwner | DFlags.Wider);
				TUtil.ShowOsdRect(r.wnd.Rect, true);
				return default;
			}
			return (r.obj, r.speed[1]);
		}

		#endregion

		#region info

		public static void Info(this KSciInfoBox t, FrameworkElement e, string name, string text) {
			text = CommonInfos.PrependName(name, text);
			t.AddElem(e, text);
		}

		public static void InfoC(this KSciInfoBox t, ContentControl k, string text) => Info(t, k, _ControlName(k), text);

		public static void InfoCT(this KSciInfoBox t, KCheckTextBox k, string text, bool wildex = false, string wildexPart = null) {
			text = CommonInfos.PrependName(_ControlName(k.c), text);
			if (wildex) text = CommonInfos.AppendWildexInfo(text, wildexPart);
			t.AddElem(k.c, text);
			t.AddElem(k.t, text);
		}

		public static void InfoCO(this KSciInfoBox t, KCheckComboBox k, string text) {
			text = CommonInfos.PrependName(_ControlName(k.c), text);
			t.AddElem(k.c, text);
			t.AddElem(k.t, text);
		}

		/// <summary>
		/// Returns k text without '_' character used for Alt+underline.
		/// </summary>
		static string _ControlName(ContentControl k) => AStringUtil.RemoveUnderlineChar(k.Content as string, '_');

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
	@" is <help articles/Wildcard expression>wildcard expression<>. Can contain <+regex>regular expression<> like <c brown>@""**rc regex""<>.
This and other text fields can contain text like <c brown>abcd<> or C# string like <c brown>""ab\tcd""<> or <c brown>@""abcd""<>.
Examples:
whole text
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
";
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
	//	protected AWpfBuilder _builder;
	//	public virtual string ZResultCode { get; protected set; }

	//	public CodeToolDialog() {
	//		_builder.OkApply += o => { InsertCode.Statements(ZResultCode); };
	//	}
	//}
}
