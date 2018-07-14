#define USE_CODEANALYSIS_REF

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
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
//using System.Xml.Linq;

//using SG = SourceGrid;

#if USE_CODEANALYSIS_REF
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
#endif

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Tools
{
	internal static class TUtil
	{
		#region text

		/// <summary>
		/// Appends ', ' and string argument to this StringBuilder.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="s">Argument value. If null, appends 'null'. If verbatim (like '@"text"'), appends 's'. Else appends '"escaped s"'.</param>
		/// <param name="param">If not null, appends 'param: s'. By default appends only 's'. If "null", appends 'null, s'.</param>
		/// <param name="noComma">Don't append ', '. Use for the first parameter. If false, does not append only if b.Length is less than 2.</param>
		internal static StringBuilder AppendStringArg(this StringBuilder t, string s, string param = null, bool noComma = false)
		{
			_AppendArgPrefix(t, param, noComma);
			if(s == null) t.Append("null");
			else if(IsVerbatim(s, out _)) t.Append(s);
			else t.Append('\"').Append(s.Escape_()).Append('\"'); //FUTURE: make verbatim if contains \ and no newlines/tabs/etc
			return t;
		}

		/// <summary>
		/// Appends ', ' and non-string argument to this StringBuilder.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="s">Argument value. Must not be empty.</param>
		/// <param name="param">If not null, appends 'param: s'. By default appends only 's'. If "null", appends 'null, s'.</param>
		/// <param name="noComma">Don't append ', '. Use for the first parameter. If false, does not append only if b.Length is less than 2.</param>
		internal static StringBuilder AppendOtherArg(this StringBuilder t, string s, string param = null, bool noComma = false)
		{
			Debug.Assert(!Empty(s));
			_AppendArgPrefix(t, param, noComma);
			t.Append(s);
			return t;
		}

		static void _AppendArgPrefix(StringBuilder t, string param, bool noComma)
		{
			if(!noComma && t.Length > 1) t.Append(", ");
			if(param != null) t.Append(param).Append(param == "null" ? ", " : ": ");
		}

		/// <summary>
		/// If in grid are checked some flags, appends ', ' and the checked flags.
		/// Returns true if checked.
		/// Grid row keys must be enum member names.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flagsEnum">The type of the flags enum.</param>
		/// <param name="grid"></param>
		/// <param name="param">If not null, appends 'param: flags'. By default appends only 'flags'. If "null", appends 'null, s'.</param>
		/// <param name="prefix">Row keys of these flags in grid have this prefix. Use when the grid contains flags of another enum with same member names.</param>
		internal static bool AppendFlagsFromGrid(this StringBuilder t, Type flagsEnum, Controls.ParamGrid grid, string param = null, string prefix = null)
		{
			bool isFlags = false;
			string[] flagNames = flagsEnum.GetEnumNames();
			for(int r = 0, n = grid.RowsCount; r < n; r++) {
				var key = grid.ZGetRowKey(r); if(Empty(key)) continue;
				if(prefix != null) { if(key.StartsWith_(prefix)) key = key.Substring(prefix.Length); else continue; }
				if(!flagNames.Contains(key)) continue;
				if(!grid.ZIsChecked(r)) continue;
				var flag = flagsEnum.Name + "." + key;
				if(!isFlags) {
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
		internal static StringBuilder AppendWaitTime(this StringBuilder t, string waitTime, bool orThrow)
		{
			if(Empty(waitTime)) waitTime = "0"; else if(!orThrow && waitTime != "0" && !waitTime.StartsWith_('-')) t.Append('-');
			t.Append(waitTime);
			return t;
		}

		/// <summary>
		/// Returns true if s is like '@"*"' or '$"*"' or '$@"*"'.
		/// s can be null.
		/// </summary>
		internal static bool IsVerbatim(string s, out int prefixLength)
		{
			prefixLength = 0;
			if(s != null && s.Length >= 3 && s[s.Length - 1] == '\"') {
				if(s[0] == '$') prefixLength = 1;
				if(s[prefixLength] == '@') prefixLength++;
				if(s[prefixLength] == '\"' && prefixLength != s.Length - 1) return true;
				prefixLength = 0;
			}
			return false;
		}

		/// <summary>
		/// If s has *? characters, prepends "**t ".
		/// s can be null.
		/// </summary>
		internal static string EscapeWildex(string s)
		{
			if(Wildex.HasWildcards(s)) s = "**t " + s;
			return s;
		}

		/// <summary>
		/// If s has *? characters, prepends "**t ".
		/// But if s has single * character, converts to "**r regexp" that ignores it. Because single * often is used to indicate unsaved state.
		/// If makeRegexVerbatim and makes regex, prepends @" and appends ".
		/// s can be null.
		/// </summary>
		internal static string EscapeWindowName(string s, bool makeRegexVerbatim)
		{
			if(Wildex.HasWildcards(s)) {
				int i = s.IndexOf('*');
				if(i >= 0 && s.IndexOf('*', i + 1) < 0) {
					var b = new StringBuilder();
					if(makeRegexVerbatim) b.Append("@\"");
					if(i > 0) b.Append(@"**r \Q").Append(s, 0, i).Append(@"\E");
					b.Append(@"\*?");
					int len = s.Length - ++i;
					if(len > 0) b.Append(@"\Q").Append(s, i, len).Append(@"\E");
					if(makeRegexVerbatim) b.Append("\"");
					return b.ToString();
				}
				s = "**t " + s;
			}
			return s;
		}

		/// <summary>
		/// Replaces known non-constant window class names with wildcard. Eg "WindowsForms10.EDIT..." with "*.EDIT.*".
		/// </summary>
		/// <param name="s">Can be null.</param>
		/// <param name="escapeWildex">If didn't replace, call <see cref="EscapeWildex"/>.</param>
		internal static string StripWndClassName(string s, bool escapeWildex)
		{
			if(!Empty(s)) {
				int n = s.RegexReplace_(@"^WindowsForms\d+(\..+?\.).+", "*$1*", out s);
				if(n == 0) n = s.RegexReplace_(@"^(HwndWrapper\[.+?;).+", "$1*", out s);
				if(escapeWildex && n == 0) s = EscapeWildex(s);
			}
			return s;
		}

		#endregion

		#region misc

		/// <summary>
		/// Gets control id. Returns true if it can be used to identify the control in window wWindow.
		/// </summary>
		internal static bool GetUsefulControlId(Wnd wControl, Wnd wWindow, out int id)
		{
			id = wControl.ControlId;
			if(id == 0 || id == -1 || id > 0xffff || id < -0xffff) return false;
			//info: some apps use negative ids, eg FileZilla. Usually >= -0xffff. Id -1 is often used for group buttons and separators.
			//if(id == (int)wControl.Handle) return false; //.NET forms, delphi. //possible coincidence //rejected, because window handles are > 0xffff
			Debug.Assert((int)wControl.Handle > 0xffff);

			//if(wWindow.Child("***id " + id) != wControl) return false; //problem with combobox child Edit that all have id 1001
			if(wWindow.ChildAll("***id " + id).Length != 1) return false; //note: searching only visible controls; else could find controls with same id in hidden pages of tabbed dialog.
			return true;
		}

		#endregion

		#region OnScreenRect

		/// <summary>
		/// Creates standard <see cref="OsdRect"/>.
		/// </summary>
		internal static OsdRect CreateOsdRect(int thickness = 4) => new OsdRect() { Color = 0xFF8A2BE2, Thickness = thickness }; //Color.BlueViolet

		/// <summary>
		/// Briefly shows standard blinking on-screen rectangle.
		/// </summary>
		internal static void ShowOsdRect(RECT r, bool limitToScreen = false)
		{
			var osr = CreateOsdRect();
			r.Inflate(2, 2); //2 pixels inside, 2 outside
			if(limitToScreen) {
				var k = Screen.FromRectangle(r).Bounds;
				r.Intersect(k);
			}
			osr.Rect = r;
			osr.Show();

			int i = 0;
			Timer_.Every(250, t =>
			{
				if(i++ < 5) {
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
		internal class CaptureWindowEtcWithHotkey
		{
			Timer_ _timer;
			OsdRect _osr;
			Form _form;
			CheckBox _captureCheckbox;
			Func<RECT?> _cbGetRect;
			const string c_propName = "Au.Capture";
			const uint c_stopMessage = Api.WM_USER + 242;

			public bool Capturing { get; private set; }

			/// <param name="form">Tool form.</param>
			/// <param name="captureCheckbox">Checkbox that turns on/off capturing.</param>
			/// <param name="cbGetRect">Called to get rectangle of object from mouse. Can return default to hide rectangle.</param>
			public CaptureWindowEtcWithHotkey(Form form, CheckBox captureCheckbox, Func<RECT?> cbGetRect)
			{
				_form = form;
				_captureCheckbox = captureCheckbox;
				_cbGetRect = cbGetRect;
			}

			/// <summary>
			/// Starts or stops capturing.
			/// Does nothing if already in that state.
			/// </summary>
			public void StartStop(bool start)
			{
				if(start == Capturing) return;
				var wForm = (Wnd)_form;
				if(start) {
					//let other forms stop capturing
					wForm.Prop.Set(c_propName, 1);
					Wnd.Find(null, "WindowsForms*", also: o =>
					{
						if(o != wForm && o.Prop[c_propName] == 1) o.Send(c_stopMessage);
						return false;
					});

					if(!Api.RegisterHotKey(wForm, 1, 0, KKey.F3)) {
						AuDialog.ShowError("Failed to register hotkey F3", owner: _form);
						return;
					}
					Capturing = true;

					//set timer that shows AO rect
					if(_timer == null) {
						_osr = TUtil.CreateOsdRect();
						_timer = new Timer_(t =>
						{
							//show rect of UI object from mouse
							Wnd w = Wnd.FromMouse(WXYFlags.NeedWindow);
							RECT? r = default;
							if(!(w.Is0 || w == wForm || w.WndOwner == wForm)) r = _cbGetRect();
							if(r.HasValue) {
								var rr = r.GetValueOrDefault();
								rr.Inflate(2, 2); //2 pixels inside, 2 outside
								_osr.Rect = rr;
								_osr.Show();
							} else {
								_osr.Visible = false;
							}
						});
					}
					_timer.Start(250, false);
				} else {
					Capturing = false;
					Api.UnregisterHotKey(wForm, 1);
					wForm.Prop.Remove(c_propName);
					_timer.Stop();
					_osr.Hide();
				}
			}

			/// <summary>
			/// Must be called from WndProc of the tool form.
			/// If returns true, don't call base.WndProc.
			/// </summary>
			/// <param name="m"></param>
			public bool WndProc(ref Message m)
			{
				switch((uint)m.Msg) {
				case Api.WM_HOTKEY:
					if((int)m.WParam == 1) return true;
					break;
				case c_stopMessage:
					_captureCheckbox.Checked = false;
					return true;
				}
				return false;
			}

			/// <summary>
			/// Must be called when closing the tool form.
			/// </summary>
			public void Dispose()
			{
				StartStop(false);
				_osr?.Dispose();
			}
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

		/// <summary>
		/// Executes test code that finds an object in window.
		/// Returns the found object and the speed.
		/// </summary>
		/// <param name="code">
		/// The first line should be 'find window' code like <c>var w = Wnd.Find(...);</c>. This function uses this regex to get Wnd variable name: "^(?:var|Wnd) (\w+)"; if does not match, uses name w.
		/// The last line must be a 'find object' function call. Example: <c>Acc.Find(...);</c>. Without variable and without OrThrow.
		/// Between them can be more lines of code. For example <c>w = w.Child(...);</c>.
		/// </param>
		/// <param name="wnd">Window handle.</param>
		/// <param name="bTest">The 'Test' button. This function will disable/enable it.</param>
		/// <param name="lSpeed">Label control that displays speed.</param>
		/// <param name="getRect">Callback function that returns object's rectangle in screen. Called when object has been found.</param>
		/// <example>
		/// <code><![CDATA[
		/// var r = await TUtil.RunTestFindObject(code, _wnd, _bTest, _lSpeed, o => (o as Acc).Rect);
		/// ]]></code>
		/// </example>
		internal static async Task<TestFindObjectResults> RunTestFindObject(
			string code, Wnd wnd, Button bTest, Label lSpeed, Func<object, RECT> getRect)
		{
			if(Empty(code)) return default;
			Form form = lSpeed.FindForm();
			lSpeed.Text = "";

			//Print(code);
			//Perf.First();

			if(!code.RegexMatch_(@"^(?:var|Wnd) (\w+)", 1, out var wndVar)) wndVar = "w";

			var b = new StringBuilder();
			var lines = code.SplitLines_(true);
			int lastLine = lines.Length - 1;
			for(int i = 0; i < lastLine; i++) b.AppendLine(lines[i]);
			b.AppendLine("var _p_ = Perf.StartNew(); var _a_ =");
			b.AppendLine(lines[lastLine]);
			b.AppendLine($"_p_.Next(); return (_p_.TimeTotal, _a_, {wndVar});");
			code = b.ToString(); //Print(code);

			(long speed, object obj, Wnd wnd) r = default;
			bool ok = false;
			try {
				bTest.Enabled = false;
				var so = ScriptOptions.Default.WithReferences(s_testReferences).WithImports(s_testImports);
				r = await CSharpScript.EvaluateAsync<(long, object, Wnd)>(code, so);
				ok = true;
			}
			catch(CompilationErrorException e) {
				var es = String.Join("\r\n", e.Diagnostics);
				//lastLine += 3; es = es.RegexReplace_($@"(?m)^\({lastLine},", $"({lastLine - 1},"); //correct the line number of the last line, because we inserted one line before it; cannot insert at the end of previous line because it can end with comments
				Print("---- CODE ----\r\n" + code + "--------------");
				AuDialog.ShowError(e.GetType().Name, es, owner: form, flags: DFlags.OwnerCenter | DFlags.Wider/*, expandedText: code*/);
			}
			catch(NotFoundException) {
				AuDialog.ShowInfo("Window not found", owner: form, flags: DFlags.OwnerCenter);
				//info: throws only when window not found. This is to show time anyway when acc not found.
			}
			catch(Exception e) {
				AuDialog.ShowError(e.GetType().Name, e.Message, owner: form, flags: DFlags.OwnerCenter);
			}
			finally {
				GC.Collect(); //GC does not work with compiler. Task Manager shows 53 MB. After several times can be 300 MB. This makes 22 MB.
				bTest.Enabled = true;
			}
			if(!ok) return default;

			//Perf.NW();
			//Print(r);

			var t = Math.Round((double)r.speed / 1000, r.speed < 1000 ? 2 : (r.speed < 10000 ? 1 : 0));
			var sTime = t.ToString_() + " ms";
			if(r.obj is Wnd w1 && w1.Is0) r.obj = null;
			if(r.obj != null) {
				lSpeed.ForeColor = Form.DefaultForeColor;
				lSpeed.Text = sTime;
				var re = getRect(r.obj);
				TUtil.ShowOsdRect(re);

				//if form or its visible owners cover the found object, temporarily activate object's window
				foreach(var ow in Wnd.Misc.OwnerWindowsAndThis((Wnd)form, true)) {
					if(re.IntersectsWith(ow.Rect)) {
						r.wnd.ActivateLL();
						Time.SleepDoEvents(1500);
						break;
					}
				}
			} else {
				//AuDialog.ShowEx("Not found", owner: this, flags: DFlags.OwnerCenter, icon: DIcon.Info, secondsTimeout: 2);
				lSpeed.ForeColor = Color.Red;
				lSpeed.Text = "Not found,";
				Timer_.After(700, () => lSpeed.Text = sTime);
			}

			((Wnd)form).ActivateLL();

			if(r.wnd != wnd && !r.wnd.Is0) {
				AuDialog.ShowWarning("The code finds another window",
				$"Need:  {wnd.ToString()}\r\n\r\nFound:  {r.wnd.ToString()}",
				owner: form, flags: DFlags.OwnerCenter | DFlags.Wider);
				return default;
			}
			return new TestFindObjectResults() { obj = r.obj, speed = r.speed };
		}
#endif
		//FUTURE: ngen C# compiler assemblies. Now each Windows update unngens most of them.

		internal struct TestFindObjectResults
		{
			public object obj;
			public long speed;
		}

		#endregion
	}

	//public static class Test
	//{
	//	public static void OsdRect()
	//	{
	//		RECT r = (500, 500, 30, 20);
	//		TUtil.ShowOsdRect(r);
	//		AuDialog.Show();
	//	}
	//}
}
