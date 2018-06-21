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
//using System.Linq;
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
	internal static class ToolsUtil
	{
		#region text

		/// <summary>
		/// Appends ', ' and string argument to StringBuilder.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="s">Argument value. If null, appends 'null'. If verbatim (like '@"text"'), appends 's'. Else appends '"escaped s"'.</param>
		/// <param name="param">If not null, appends 'param: s'. By default appends only 's'.</param>
		/// <param name="noComma">Don't append ', '. Use for the first parameter. If false, does not append only if b.Length is less than 2.</param>
		internal static void AppendStringArg(StringBuilder b, string s, string param = null, bool noComma = false)
		{
			if(!noComma && b.Length > 1) b.Append(", ");
			if(param != null) b.Append(param).Append(": ");
			if(s == null) b.Append("null");
			else if(IsVerbatim(s)) b.Append(s);
			else b.Append('\"').Append(s.Escape_()).Append('\"');
		}

		/// <summary>
		/// Appends ', ' and non-string argument to StringBuilder.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="s">Argument value. Must not be empty.</param>
		/// <param name="param">If not null, appends 'param: s'. By default appends only 's'.</param>
		/// <param name="noComma">Don't append ', '. Use for the first parameter. If false, does not append only if b.Length is less than 2.</param>
		internal static void AppendOtherArg(StringBuilder b, string s, string param = null, bool noComma = false)
		{
			Debug.Assert(!Empty(s));
			if(!noComma && b.Length > 1) b.Append(", ");
			if(param != null) b.Append(param).Append(": ");
			b.Append(s);
		}

		/// <summary>
		/// Returns true if s is like '@"*"' or '$"*"' or '$@"*"'.
		/// </summary>
		internal static bool IsVerbatim(string s)
		{
			if(s.Length < 3 || s[s.Length - 1] != '\"') return false;
			if(s[0] == '@') return s[1] == '\"';
			if(s[0] == '$') return s[1] == '\"' || (s[1] == '@' && s[2] == '\"' && s.Length > 3);
			return false;
		}

		/// <summary>
		/// If s has *? characters, prepends "**t ".
		/// </summary>
		internal static string EscapeWildex(string s)
		{
			if(Wildex.HasWildcards(s)) s = "**t " + s;
			return s;
		}

		/// <summary>
		/// Replaces known variable class names with wildcard. Eg "WindowsForms10.EDIT..." with "*.EDIT.*".
		/// </summary>
		/// <param name="s">Can be null/"".</param>
		/// <param name="escapeWildex">Finally call <see cref="EscapeWildex"/>.</param>
		internal static string StripWndClassName(string s, bool escapeWildex = false)
		{
			if(!Empty(s)) {
				int n = s.RegexReplace_(@"^WindowsForms\d+(\..+?\.).+", "*$1*", out s);
				if(n == 0) n = s.RegexReplace_(@"^(HwndWrapper\[.+?;).+", "$1*", out s);
				if(escapeWildex && n == 0) s = EscapeWildex(s);
			}
			return s;
		}

		#endregion

		#region OnScreenRect

		/// <summary>
		/// Creates standard <see cref="OsdRect"/>.
		/// </summary>
		internal static OsdRect CreateOsdRect(int thickness = 2) => new OsdRect() { Color = Color.DarkOrange, Thickness = thickness };

		/// <summary>
		/// Briefly shows standard static or blinking on-screen rectangle.
		/// </summary>
		internal static void ShowOsdRect(in RECT r, bool blink)
		{
			if(r.IsEmpty) return;
			var osr = CreateOsdRect(4);
			osr.Rect = r;
			osr.Show();

			//FUTURE: show something more visible, eg line from Mouse.XY to r. For it create class OnScreenLine or extend OnScreenRect.
			//	Or could animate the rect, but then not good when small.

			if(blink) {
				int i = 0;
				Timer_.Every(250, t =>
				{
					if(i++ < 5) osr.Visible = !osr.Visible;
					else {
						t.Stop();
						osr.Dispose();
					}
				});
			} else {
				Timer_.After(1000, () => osr.Dispose());
			}
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
			Func<RECT?> _cbGetRect;
			bool _capturing;

			public bool Capturing => _capturing;

			/// <param name="form">Tool form.</param>
			/// <param name="cbGetRect">Called to get rectangle of object from mouse. Can return default to hide rectangle.</param>
			public CaptureWindowEtcWithHotkey(Form form, Func<RECT?> cbGetRect)
			{
				_form = form;
				_cbGetRect = cbGetRect;
			}

			/// <summary>
			/// Starts or stops capturing.
			/// Does nothing if already in that state.
			/// </summary>
			public void StartStop(bool start)
			{
				if(start == _capturing) return;
				if(start) {
					//let other forms stop capturing
					Wnd.Find(_form.Text, "WindowsForms*", also: o => { if(o != (Wnd)_form) o.Send(s_msgStopCapturing); return false; });

					if(!Api.RegisterHotKey((Wnd)_form, 1, 0, KKey.F3)) {
						AuDialog.ShowError("Failed to register hotkey F3", owner: _form);
						return;
					}
					_capturing = true;

					//set timer that shows AO rect
					if(_timer == null) {
						_osr = ToolsUtil.CreateOsdRect();
						_timer = new Timer_(t =>
						{
							//show rect of UI object from mouse
							Wnd w = Wnd.FromMouse(WXYFlags.NeedWindow), wForm = (Wnd)_form;
							RECT? r = default;
							if(!(w.Is0 || w == wForm || w.WndOwner == wForm)) r = _cbGetRect();
							if(r.HasValue) {
								_osr.Rect = r.GetValueOrDefault();
								_osr.Show();
							} else {
								_osr.Visible = false;
							}
						});
					}
					_timer.Start(250, false);
				} else {
					_capturing = false;
					Api.UnregisterHotKey((Wnd)_form, 1);
					_timer.Stop();
					_osr.Hide();
				}
			}

			/// <summary>
			/// Must be called from WndProc of the tool form.
			/// If returns true, don't call base.WndProc.
			/// </summary>
			/// <param name="m"></param>
			/// <param name="capture">true if pressed the hotkey. false if must stop capturing.</param>
			public bool WndProc(ref Message m, out bool capture)
			{
				capture = false;
				var msg = (uint)m.Msg;
				if(msg == s_msgStopCapturing) {
					return true;
				} else if(msg == Api.WM_HOTKEY && (int)m.WParam == 1) {
					capture = true;
					return true;
				}
				return false;
			}
			static readonly uint s_msgStopCapturing = Wnd.Misc.RegisterMessage("Au.StopCapturing", true);

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
		/// <param name="sFind">Code that finds and returns an object. Example: "+Acc.Find(...);".</param>
		/// <param name="tWnd">Control that contains window handle and code to find the window.</param>
		/// <param name="bTest">The 'Test' button. This function will disable/enable it.</param>
		/// <param name="lSpeed">Control used to display speed.</param>
		/// <param name="getRect">Caller when found. Must return object's rectangle.</param>
		internal static async Task<TestFindObjectResults> RunTestFindObject(
			string sFind, TextBoxWnd tWnd, Button bTest, Label lSpeed, Func<object, RECT> getRect)
		{
			string sWnd = tWnd.Text; if(Empty(sWnd)) return default;
			Form form = lSpeed.FindForm();
			lSpeed.Text = "";

			//Perf.First();

			var b = new StringBuilder();
			b.Append(sWnd).Append("var _p_ = Perf.StartNew();").AppendLine("var _a_ = ");
			b.AppendLine(sFind);
			b.AppendLine($"_p_.Next(); return (_p_.TimeTotal, _a_, {tWnd.WndVar});");

			var code = b.ToString(); //Print(code);
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
			if(r.obj != null) {
				lSpeed.ForeColor = Form.DefaultForeColor;
				lSpeed.Text = sTime;
				ToolsUtil.ShowOsdRect(getRect(r.obj), true);
			} else {
				//AuDialog.ShowEx("Not found", owner: this, flags: DFlags.OwnerCenter, icon: DIcon.Info, secondsTimeout: 2);
				lSpeed.ForeColor = Color.Red;
				lSpeed.Text = "Not found,";
				Timer_.After(500, () => lSpeed.Text = sTime);
			}

			var w = tWnd.Hwnd;
			if(r.wnd != w) {
				AuDialog.ShowWarning("Wnd.Find finds another window",
				$"Need: {w.ToString()}\r\n\r\nFound: {r.wnd.ToString()}",
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
}
