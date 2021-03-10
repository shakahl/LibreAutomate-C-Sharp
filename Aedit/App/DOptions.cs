using Au;
using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Au.Controls;
using System.ComponentModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;

public class DOptions : KDialogWindow
{
	public static void ZShow() {
		if (s_dialog == null) {
			s_dialog = new DOptions();
			s_dialog.Show();
		} else {
			s_dialog.Activate();
		}
	}
	static DOptions s_dialog;

	protected override void OnClosed(EventArgs e) {
		s_dialog = null;
		base.OnClosed(e);
	}

	AWpfBuilder _b;
	TabControl _tc;

	public DOptions() {
		Title = "Options";
		Owner = App.Wmain;
		_b = new AWpfBuilder(this).WinSize(540);
		_b.WinProperties(WindowStartupLocation.CenterOwner, showInTaskbar: false);
		_b.Row(-1).Add(out _tc).Height(300..);
		_b.R.AddOkCancel(apply: "_Apply");

		_General();
		//_Files();
		_Templates();
		_Font();
		_Code();

		//_tc.SelectedIndex = 2;

		_b.End();
	}

	/// <summary>
	/// Adds new TabItem to _tc. Creates and returns new AWpfBuilder for building the tab page.
	/// </summary>
	AWpfBuilder _Page(string name, WBPanelType panelType = WBPanelType.Grid) {
		var tp = new TabItem { Header = name };
		_tc.Items.Add(tp);
		return new AWpfBuilder(tp, panelType).Margin("3");
	}

	#region General

	void _General() {
		var b = _Page("General").Columns(-1, -1);
		//left column
		b.StartStack(vertical: true);
		b.Add(out CheckBox startWithWin, "Start with Windows");
		b.Add(out CheckBox startHidden, "Start hidden; hide when closing");
		b.End();
		//right column
		b.StartStack(vertical: true);
		b.Add("Run scripts when this workspace loaded", out TextBox startupScripts).Multiline(60, TextWrapping.NoWrap).Validation(_startupScripts_Validation);
		b.End();
		b.End();

		//b.Loaded += () => {

		//};
		const string c_rkRun = @"Software\Microsoft\Windows\CurrentVersion\Run";
		bool init_startWithWin = Registry.GetValue(@"HKEY_CURRENT_USER\" + c_rkRun, "Aedit", null) is string;
		startWithWin.IsChecked = init_startWithWin;
		startHidden.IsChecked = App.Settings.runHidden;
		string init_startupScripts = App.Model.StartupScriptsCsv;
		startupScripts.Text = init_startupScripts;

		_b.OkApply += e => {
			if (startWithWin.True() != init_startWithWin) {
				try {
					using var rk = Registry.CurrentUser.OpenSubKey(c_rkRun, true);
					if (init_startWithWin) rk.DeleteValue("Aedit");
					else rk.SetValue("Aedit", "\"" + AFolders.ThisAppBS + "Au.CL.exe\" /e");
				}
				catch (Exception ex) { AOutput.Write("Failed to change 'Start with Windows'. " + ex.ToStringWithoutStack()); }
			}
			App.Settings.runHidden = startHidden.True();

			var s = startupScripts.Text;
			if (s != init_startupScripts) App.Model.StartupScriptsCsv = s;
		};

		static string _startupScripts_Validation(FrameworkElement fe) {
			//AOutput.Write("validating");
			string s = (fe as TextBox).Text; if (s.NE()) return null;
			try {
				var t = ACsv.Parse(s);
				if (t.ColumnCount > 2) return "Too many commas in a line. If script name contains comma, enclose in \"\".";
				ARegex rxDelay = null;
				foreach (var v in t.Data) {
					var script = v[0];
					if (script.Starts("//")) continue;
					if (App.Model.FindScript(script) == null) return "Script not found: " + script;
					var delay = v.Length == 1 ? null : v[1];
					if (!delay.NE()) {
						rxDelay ??= new ARegex(@"(?i)^\d+ *m?s$");
						if (!rxDelay.IsMatch(delay)) return "Delay must be like 2 s or 500 ms";
					}
				}
			}
			catch (FormatException ex) { return ex.Message; }
			return null;
		}
	}

	#endregion

	#region Files

	void _Files() {
		var b = _Page("Files");
		b.End();

		b.Loaded += () => {

		};

		_b.OkApply += e => {

		};
	}

	#endregion

	#region Templates

	void _Templates() {
		var b = _Page("Templates").Columns(0, 100, -1, 0, 100);
		b.R.Add("Template", out ComboBox template).Items("Script|Class")
			.Skip().Add("Use", out ComboBox use).Items("Default|Custom");
		b.Row(-1).Add(out KSciCodeBoxAWnd sci).Span(5); sci.ZInitBorder = true;
		b.End();

		string[] customText = new string[2];
		var useCustom = (FileNode.ETempl)App.Settings.templ_use;

		template.SelectionChanged += _Combo_Changed;
		use.SelectionChanged += _Combo_Changed;
		sci.ZTextChanged += (_, _) => customText[template.SelectedIndex] = sci.zText;
		b.Loaded += () => {
			_Combo_Changed(template, null);
		};

		_b.OkApply += e => {
			for (int i = 0; i < customText.Length; i++) {
				string text = customText[i]; if (text == null) continue;
				var tt = (FileNode.ETempl)(1 << i);
				var file = FileNode.Templates.FilePathRaw(tt, true);
				try {
					if (text == FileNode.Templates.Load(tt, false)) {
						AFile.Delete(file);
					} else {
						AFile.SaveText(file, text);
					}
				}
				catch (Exception ex) { AOutput.Write(ex.ToStringWithoutStack()); }
			}
			App.Settings.templ_use = (int)useCustom;
		};

		void _Combo_Changed(object sender, SelectionChangedEventArgs e) {
			int i = template.SelectedIndex;
			FileNode.ETempl tt = i switch { 1 => FileNode.ETempl.Class, _ => FileNode.ETempl.Script, };
			if (sender == template) use.SelectedIndex = useCustom.Has(tt) ? 1 : 0;
			bool custom = use.SelectedIndex > 0;
			string text = null;
			if (e != null) {
				useCustom.SetFlag(tt, custom);
				if (custom) text = customText[i];
			}
			text ??= FileNode.Templates.Load(tt, custom);
			sci.ZSetText(text, readonlyFrom: custom ? -1 : 0);
		}
	}

	#endregion

	#region Font

	void _Font() {
		var b = _Page("Font", WBPanelType.Dock);
		b.Add(out KScintilla sciStyles).Width(150); sciStyles.ZInitBorder = true;
		b.StartGrid().Columns(-1).Margin(20);
		b.R.StartGrid();
		var pFont = b.Panel as Grid;
		b.R.Add("Font", out ComboBox fontName).Editable();
		b.R.Add("Size", out TextBox fontSize).Width(40).Align("L");
		b.End();
		b.R.StartGrid();
		var pColor = b.Panel as Grid;
		b.R.Add(out KColorPicker color);
		b.R.Add(out KCheckBox bold, "Bold");
		b.End();
		b.Row(-1);
		b.R.Add(out Button bInfo, "?").Align("r").Width(20);
		b.End();
		b.End();

		pColor.Visibility = Visibility.Collapsed;

		b.Loaded += () => {
			var styles = CiStyling.TStyles.Settings;

			//font

			var fontsMono = new List<string>();
			var fontsVar = new List<string>();
			using (var dc = new ScreenDC_()) {
				unsafe {
					EnumFontFamiliesEx(dc, default, (lf, tm, fontType, lParam) => {
						if (lf->lfFaceName[0] != '@') {
							var fn = new string(lf->lfFaceName);
							if ((lf->lfPitchAndFamily & 0xf0) == 48) fontsMono.Add(fn); else fontsVar.Add(fn); //FF_MODERN=48
						}
						return 1;
					}, default, 0);
				}
			}
			fontsMono.Sort();
			fontsVar.Sort();
			var fonts = new List<string>();
			fonts.Add("[ Fixed-width fonts ]");
			fonts.AddRange(fontsMono);
			fonts.Add("");
			fonts.Add("[ Variable-width fonts ]");
			fonts.AddRange(fontsVar.ToArray());
			fontName.ItemsSource = fonts;
			var selFont = styles.FontName;
			fontName.SelectedItem = selFont; if (fontName.SelectedItem == null) fontName.Text = selFont;
			fontSize.Text = styles.FontSize.ToStringInvariant();

			//styles

			sciStyles.zMarginWidth(1, 0);
			styles.ToScintilla(sciStyles);
			bool ignoreColorEvents = false;
			int backColor = styles.BackgroundColor;
			var s = @"Font
Background
None
//Comment
""String"" 'c'
\r\n\t\0\\
1234567890
()[]{},;:
Operator
Keyword
Namespace
Type
Function
Variable
Constant
GotoLabel
#preprocessor
#if-excluded
XML doc text
/// <doc tag>
Line number";
			sciStyles.zText = s;
			int i = -3;
			foreach (var v in s.Segments(SegSep.Line)) {
				i++;
				if (i < 0) { //Font, Background

				} else {
					if (i == (int)CiStyling.EToken.countUserDefined) i = Sci.STYLE_LINENUMBER;
					//AOutput.Write(i, s[v.start..v.end]);
					sciStyles.Call(Sci.SCI_STARTSTYLING, v.start);
					sciStyles.Call(Sci.SCI_SETSTYLING, v.end - v.start, i);
				}
			}
			//when selected line changed
			int currentLine = -1;
			sciStyles.ZNotify += (KScintilla c, ref Sci.SCNotification n) => {
				switch (n.nmhdr.code) {
				case Sci.NOTIF.SCN_UPDATEUI:
					int line = c.zLineFromPos(false, c.zCurrentPos8);
					if (line != currentLine) {
						currentLine = line;
						int tok = _SciStylesLineToTok(line);
						if (tok == -2) { //Font
							pColor.Visibility = Visibility.Collapsed;
							pFont.Visibility = Visibility.Visible;
						} else {
							pFont.Visibility = Visibility.Collapsed;
							pColor.Visibility = Visibility.Visible;
							ignoreColorEvents = true;
							int col;
							if (tok == -1) {
								col = backColor;
								bold.Visibility = Visibility.Collapsed;
							} else {
								col = ColorInt.SwapRB(sciStyles.Call(Sci.SCI_STYLEGETFORE, tok));
								bold.IsChecked = 0 != sciStyles.Call(Sci.SCI_STYLEGETBOLD, tok);
								bold.Visibility = Visibility.Visible;
							}
							color.Color = col;
							ignoreColorEvents = false;
						}
					}
					break;
				}
			};
			//when values of style controls changed
			TextChangedEventHandler textChanged = (sender, _) => _ChangeFont(sender);
			fontName.AddHandler(TextBoxBase.TextChangedEvent, textChanged);
			fontSize.AddHandler(TextBoxBase.TextChangedEvent, textChanged);
			void _ChangeFont(object control = null) {
				var (fname, fsize) = _GetFont();
				for (int i = 0; i <= Sci.STYLE_LINENUMBER; i++) {
					if (control == fontName) sciStyles.zStyleFont(i, fname);
					else sciStyles.zStyleFontSize(i, fsize);
				}
			}
			(string name, int size) _GetFont() {
				var s = fontName.Text; if (s == "" || s.Starts("[ ")) s = "Consolas";
				return (s, fontSize.Text.ToInt());
			}
			bold.CheckChanged += (sender, _) => { if (!ignoreColorEvents) _UpdateSci(sender); };
			color.ColorChanged += col => { if (!ignoreColorEvents) _UpdateSci(); };
			void _UpdateSci(object control = null) {
				int tok = _SciStylesLineToTok(sciStyles.zLineFromPos(false, sciStyles.zCurrentPos8));
				int col = color.Color;
				if (tok >= 0) {
					if (control == bold) sciStyles.zStyleBold(tok, bold.True());
					else sciStyles.zStyleForeColor(tok, col);
				} else if (tok == -1) {
					backColor = col;
					for (int i = 0; i <= Sci.STYLE_DEFAULT; i++) sciStyles.zStyleBackColor(i, col);
				}
			}

			int _SciStylesLineToTok(int line) {
				line -= 2; if (line < 0) return line;
				int tok = line, nu = (int)CiStyling.EToken.countUserDefined;
				if (tok >= nu) tok = tok - nu + Sci.STYLE_LINENUMBER;
				return tok;
			}

			_b.OkApply += e => {
				var styles = new CiStyling.TStyles(sciStyles); //gets colors and bold
				var (fname, fsize) = _GetFont();
				styles.FontName = fname;
				styles.FontSize = fsize;

				if (!styles.Equals(CiStyling.TStyles.Settings)) {
					CiStyling.TStyles.Settings = styles;
					App.Settings.SaveLater();
					foreach (var v in Panels.Editor.ZOpenDocs) {
						styles.ToScintilla(v);
						v._SetLineNumberMarginWidth();
					}
				}
			};

			//[?] button
			bInfo.Click += (_, _) => {
				string link = CiStyling.TStyles.s_settingsFile;
				ADialog.Show(null, $@"Changed font/color settings are saved in file
<a href=""{link}"">{link}</a>

To reset: delete the file.
To reset some colors etc: delete some lines.
To change all: replace the file.
To backup: copy the file.

To apply changes after deleting etc, restart this application.
", icon: DIcon.Info, onLinkClick: e => { AFile.SelectInExplorer(e.LinkHref); });
			};
		};
	}

	[DllImport("gdi32.dll", EntryPoint = "EnumFontFamiliesExW")]
	internal static extern int EnumFontFamiliesEx(IntPtr hdc, in Api.LOGFONT lpLogfont, FONTENUMPROC lpProc, LPARAM lParam, uint dwFlags);
	internal unsafe delegate int FONTENUMPROC(Api.LOGFONT* lf, IntPtr tm, uint fontType, LPARAM lParam);

	#endregion

	#region Code

	void _Code() {
		var b = _Page("Code", WBPanelType.VerticalStack);
		b.StartGrid<GroupBox>("Completion lists");
		b.R.Add(out CheckBox complParenSpace, "Only spacebar adds () and <>");
		b.End();
		b.StartGrid<GroupBox>("Auto correction");
		b.R.Add(out CheckBox correctStringEnter, @"Enter in string adds \r\n");
		b.End();
		b.End();

		//b.Loaded += () => {

		//};
		complParenSpace.IsChecked = App.Settings.ci_complParenSpace;
		correctStringEnter.IsChecked = 0 == App.Settings.ci_correctStringEnter;

		_b.OkApply += e => {
			App.Settings.ci_complParenSpace = complParenSpace.True();
			App.Settings.ci_correctStringEnter = (byte)(correctStringEnter.True() ? 0 : 1);
		};
	}

	#endregion

}
