using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

partial class FOptions : DialogForm
{
	public FOptions()
	{
		InitializeComponent();
	}

	public static void ZShow()
	{
		if(s_form == null) {
			s_form = new FOptions();
			s_form.Show(Program.MainForm);
		} else {
			s_form.Activate();
		}
	}
	static FOptions s_form;

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		//workaround for:
		//	Cannot close the form with Cancel/X/Esc when failed validation of an edit control.
		//	_bCancel.CausesValidation is false, but somehow it does not prevent validation. I guess it works only with ShowDialog, but we use Show.
		e.Cancel = false;

		base.OnFormClosing(e);
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);
		s_form = null;
	}

	bool _loaded;

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		_InitGeneral();
		_InitFiles();
		_InitTemplates();
		_InitFont();
		_InitCodeInfo();
		_loaded = true;
	}

	private void _bApply_Click(object sender, EventArgs e)
	{
		_ApplyGeneral();
		_ApplyFiles();
		_ApplyTemplates();
		_ApplyFont();
		_ApplyCodeInfo();
	}

	private void _bOK_Click(object sender, EventArgs e) => _bApply_Click(sender, e);

	#region General

	const string c_rkRun = @"Software\Microsoft\Windows\CurrentVersion\Run";
	bool _initRunAtStartup;

	void _InitGeneral()
	{
		_runAtStartup.Checked = _initRunAtStartup = ARegistry.GetString(out _, "Au.Editor", c_rkRun, ARegistry.HKEY_CURRENT_USER);
		_runHidden.Checked = Program.Settings.runHidden;
		_startupScripts.Text = Program.Model.StartupScriptsCsv;

	}

	void _ApplyGeneral()
	{
		if(_runAtStartup.Checked != _initRunAtStartup) {
			try {
				using var rk = Registry.CurrentUser.OpenSubKey(c_rkRun, true);
				if(_initRunAtStartup) rk.DeleteValue("Au.Editor");
				else rk.SetValue("Au.Editor", "\"" + AFolders.ThisAppBS + "Au.CL.exe\" /e");
			}
			catch(Exception ex) { AOutput.Write("Failed to change 'Start with Windows'. " + ex.ToStringWithoutStack()); }
		}
		Program.Settings.runHidden = _runHidden.Checked;

		if(_startupScripts.Modified) Program.Model.StartupScriptsCsv = _startupScripts.Text;

	}

	private void _startupScripts_Validating(object sender, CancelEventArgs e)
	{
		//AOutput.Write("validating");
		_errorProvider.Clear();
		string s = _startupScripts.Text; if(s.NE()) return;
		string err = null;
		try {
			var t = ACsv.Parse(s);
			if(t.ColumnCount > 2) { err = "Too many commas in a line. If script name contains comma, enclose in \"\"."; goto ge; }
			foreach(var v in t.Data) {
				var script = v[0];
				if(script.Starts("//")) continue;
				if(Program.Model.FindScript(script) == null) { err = "Script not found: " + script; break; }
				var delay = v.Length == 1 ? null : v[1];
				if(!delay.NE()) {
					_rxDelay ??= new ARegex(@"(?i)^\d+ *m?s$");
					if(!_rxDelay.IsMatch(delay)) { err = "Delay must be like 2 s or 500 ms"; break; }
				}
			}
		}
		catch(FormatException ex) { err = ex.Message; }
		ge:
		if(err != null) {
			_errorProvider.SetIconAlignment(_startupScripts, ErrorIconAlignment.TopLeft);
			_errorProvider.SetError(_startupScripts, err);
			e.Cancel = true;
		}
	}
	ARegex _rxDelay;

	//If using DataGridView for startup scripts:
	// 
	// _gridScripts
	// 
	//this._gridScripts.AllowUserToResizeRows = false;
	//this._gridScripts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
	//this._gridScripts.BorderStyle = System.Windows.Forms.BorderStyle.None;
	//this._gridScripts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
	//this._gridScripts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {

	//this._colScript,
	//this._colDelay});
	//this._gridScripts.Location = new System.Drawing.Point(8, 180);
	//this._gridScripts.Name = "_gridScripts";
	//this._gridScripts.RowHeadersWidth = 12;
	//this._gridScripts.RowTemplate.Height = 19;
	//this._gridScripts.Size = new System.Drawing.Size(424, 76);
	//this._gridScripts.TabIndex = 4;

	//_gridScripts.Rows.Clear(); //sets correct height of the empty "add new" row
	//_gridScripts.Rows.Add("one", 1);
	//_gridScripts.Rows.Add("two", 2);

	//foreach(DataGridViewRow row in _gridScripts.Rows) {
	//	AOutput.Write(row.Cells[0].Value, row.Cells[1].Value);
	//}

	#endregion

	#region Files

	void _InitFiles()
	{
	}

	void _ApplyFiles()
	{
	}

	#endregion

	#region Templates

	void _InitTemplates()
	{
		_cCustTemplS.CheckedChanged += _CustomTempl_CheckedChanged;
		_cCustTemplC.CheckedChanged += _CustomTempl_CheckedChanged;
		if(Program.Settings.templ_script) _cCustTemplS.Checked = true; else { _cDefTemplS.Checked = true; _CustomTempl_CheckedChanged(_cCustTemplS, null); }
		if(Program.Settings.templ_class) _cCustTemplC.Checked = true; else { _cDefTemplC.Checked = true; _CustomTempl_CheckedChanged(_cCustTemplC, null); }
	}

	private void _CustomTempl_CheckedChanged(object sender, EventArgs e)
	{
		string text = null;
		var rb = sender as RadioButton;
		bool custom = rb.Checked;
		bool script = sender == _cCustTemplS;
		var codeBox = script ? _templScript : _templClass;
		if(_loaded) {
			_templCustom ??= new string[2];
			int i = script ? 0 : 1;
			if(custom) text = _templCustom[i]; else _templCustom[i] = codeBox.Text;
		}
		text ??= FileNode.Templates.Load(script, custom);
		codeBox.ZSetText(text, readonlyFrom: custom ? -1 : 0);
	}
	string[] _templCustom;

	void _ApplyTemplates()
	{
		Program.Settings.templ_script = _ApplyTempl(true);
		Program.Settings.templ_class = _ApplyTempl(false);

		bool _ApplyTempl(bool script)
		{
			var rb = script ? _cCustTemplS : _cCustTemplC;
			bool custom = rb.Checked;
			var userFile = FileNode.Templates.FilePathRaw(script, true);
			var codeBox = script ? _templScript : _templClass;
			string saveText = null;
			if(custom) {
				saveText = codeBox.Text;
			} else if(_templCustom != null) {
				saveText = _templCustom[script ? 0 : 1];
			}
			if(saveText != null) {
				if(saveText == FileNode.Templates.Load(script, false)) {
					custom = false;
					AFile.Delete(userFile);
				} else if(saveText != FileNode.Templates.Load(script, true)) {
					AFile.SaveText(userFile, saveText);
				}
			}
			return custom;
		}
	}

	#endregion

	#region Font

	unsafe void _InitFont()
	{
		var styles = Program.Settings.edit_styles;

		//font

		var fontsMono = new List<string>();
		var fontsVar = new List<string>();
		using(var dc = new Au.Util.ScreenDC_(0)) {
			EnumFontFamiliesEx(dc, default, (lf, tm, fontType, lParam) => {
				if(lf->lfFaceName[0] != '@') {
					var fn = new string(lf->lfFaceName);
					if((lf->lfPitchAndFamily & 0xf0) == 48) fontsMono.Add(fn); else fontsVar.Add(fn); //FF_MODERN=48
				}
				return 1;
			}, default, 0);

		}
		_cbFont.Items.Add("[ Fixed-width fonts ]");
		fontsMono.Sort();
		fontsVar.Sort();
		_cbFont.Items.AddRange(fontsMono.ToArray());
		_cbFont.Items.Add("");
		_cbFont.Items.Add("[ Variable-width fonts ]");
		_cbFont.Items.AddRange(fontsVar.ToArray());
		var selFont = styles.FontName;
		_cbFont.SelectedItem = selFont; if(_cbFont.SelectedItem == null) _cbFont.Text = selFont;
		_nFontSize.Value = styles.FontSize;
		_pFont.Location = _pColor.Location; //_pFont in designer is at wrong place, else would overlap with _pColor. _pColor is initially hidden.

		//styles

		_sciStyles.Z.MarginWidth(1, 0);
		styles.ToScintilla(_sciStyles);
		bool ignoreColorEvents = false;
		int backColor = styles.BackgroundColor ?? 0xffffff;
		PopupList listColors = null;
		var s = @"Font
Background
// Comment
""String"" 'c'
\r\n\t\\\0
1234567890
()[]{},;:
operator
keyword
Namespace
Type
Function
localVariable
parameter
_field
constant
EnumMember
label
#preprocessor
excluded
XML doc text
/// <doc tag>
line number";
		_sciStyles.Text = s;
		int i = -2;
		foreach(var v in s.Segments(SegSep.Line)) {
			i++;
			if(i <= 0) { //Font, Background

			} else {
				if(i == (int)CiStyling.EToken.countUserDefined) i = Sci.STYLE_LINENUMBER;
				//AOutput.Write(i, s[v.start..v.end]);
				_sciStyles.Call(Sci.SCI_STARTSTYLING, v.start);
				_sciStyles.Call(Sci.SCI_SETSTYLING, v.end - v.start, i);
			}
		}
		//when selected line changed
		int currentLine = -1;
		_sciStyles.ZNotify += (AuScintilla c, ref Sci.SCNotification n) => {
			switch(n.nmhdr.code) {
			case Sci.NOTIF.SCN_UPDATEUI:
				int line = c.Z.LineFromPos(false, c.Z.CurrentPos8);
				if(line != currentLine) {
					currentLine = line;
					int tok = _SciStylesLineToTok(line);
					if(tok == -1) { //Font
						_pColor.Visible = false;
						_pFont.Visible = true;
					} else {
						_pFont.Visible = false;
						_pColor.Visible = true;
						int color;
						if(tok == 0) {
							color = backColor;
							_cBold.Visible = false;
						} else {
							color = ColorInt.SwapRB(_sciStyles.Call(Sci.SCI_STYLEGETFORE, tok));
							_cBold.Checked = 0 != _sciStyles.Call(Sci.SCI_STYLEGETBOLD, tok);
							_cBold.Visible = true;
						}
						_SetColorControls(color, _EColorControls.All);
					}
				}
				break;
			}
		};
		//when values of style controls changed
		_cbFont.TextChanged += (sender, _) => _ChangeFont(sender);
		_nFontSize.TextChanged += (sender, _) => _ChangeFont(sender);
		void _ChangeFont(object control = null)
		{
			var z = _sciStyles.Z;
			var fname = _cbFont.Text; if(fname == "" || fname.Starts("[ ")) fname = "Consolas";
			int fsize = _UpDownValue(_nFontSize);
			for(int i = 0; i <= Sci.STYLE_LINENUMBER; i++) {
				if(control == _cbFont) z.StyleFont(i, fname);
				else z.StyleFontSize(i, fsize);
			}
		}
		_cBold.CheckedChanged += (sender, _) => { if(!ignoreColorEvents) _UpdateSci(sender); };
		_eColor.TextChanged += (_1, _2) => {
			if(ignoreColorEvents) return;
			_SetColorControls(_eColor.Text.ToInt(), _EColorControls.RGB | _EColorControls.HLS);
			_UpdateSci();
		};
		_nRed.TextChanged += (_1, _2) => _ChangedRGB();
		_nGreen.TextChanged += (_1, _2) => _ChangedRGB();
		_nBlue.TextChanged += (_1, _2) => _ChangedRGB();
		void _ChangedRGB()
		{
			if(ignoreColorEvents) return;
			var k = (_UpDownValue(_nRed) << 16) | (_UpDownValue(_nGreen) << 8) | _UpDownValue(_nBlue);
			_SetColorControls(k, _EColorControls.Hex | _EColorControls.HLS);
			_UpdateSci();
		}
		_nHue.TextChanged += (_1, _2) => _ChangedHLS();
		_nLum.TextChanged += (_1, _2) => _ChangedHLS();
		_nSat.TextChanged += (_1, _2) => _ChangedHLS();
		void _ChangedHLS()
		{
			if(ignoreColorEvents) return;
			int H = _UpDownValue(_nHue), L = _UpDownValue(_nLum), S = _UpDownValue(_nSat);
			int color = ColorInt.SwapRB(ColorHLSToRGB((ushort)H, (ushort)L, (ushort)S));
			_SetColorControls(color, _EColorControls.Hex | _EColorControls.RGB);
			_UpdateSci();
		}
		void _SetColorControls(int color, _EColorControls ctl)
		{
			ignoreColorEvents = true;
			if(ctl.Has(_EColorControls.Hex)) _eColor.Text = "0x" + color.ToString("X6");
			if(ctl.Has(_EColorControls.RGB)) {
				_nRed.Value = color >> 16 & 0xff;
				_nGreen.Value = color >> 8 & 0xff;
				_nBlue.Value = color & 0xff;
			}
			if(ctl.Has(_EColorControls.HLS)) {
				ColorRGBToHLS(ColorInt.SwapRB(color), out var H, out var L, out var S);
				_nHue.Value = H;
				_nLum.Value = L;
				_nSat.Value = S;
			}
			ignoreColorEvents = false;
		}
		void _UpdateSci(object control = null)
		{
			var z = _sciStyles.Z;
			int tok = _SciStylesLineToTok(z.LineFromPos(false, z.CurrentPos8));
			int color = _eColor.Text.ToInt();
			if(tok > 0) {
				if(control == _cBold) z.StyleBold(tok, _cBold.Checked);
				else z.StyleForeColor(tok, color);
			} else if(tok == 0) {
				backColor = color;
				for(int i = 0; i <= Sci.STYLE_DEFAULT; i++) _sciStyles.Z.StyleBackColor(i, color);
				listColors = null;
			}
		}

		int _SciStylesLineToTok(int line)
		{
			line--; if(line <= 0) return line;
			int tok = line, nu = (int)CiStyling.EToken.countUserDefined;
			if(tok >= nu) tok = tok - nu + Sci.STYLE_LINENUMBER;
			return tok;
		}
		//colors dropdown
		var comboColors = new ComboWrapper(_eColor);
		comboColors.ArrowButtonPressed += (_1, _2) => {
			if(listColors == null) {
				var a = new _Color[27];
				for(int i = 0, r = 0; r < 3; r++) {
					int R = r == 0 ? 0 : (r == 1 ? 0x800000 : 0xff0000);
					for(int g = 0; g < 3; g++) {
						int G = g == 0 ? 0 : (g == 1 ? 0x8000 : 0xff00);
						for(int b = 0; b < 3; b++) {
							int B = b == 0 ? 0 : (b == 1 ? 0x80 : 0xff);
							a[i++] = new _Color(R | G | B, backColor);
						}
					}
				}
				listColors = new PopupList { IsModal = true, ComboBoxAnimation = true, Items = a };
				listColors.SelectedAction = o => _eColor.Text = o.ResultItem.ToString();
			}
			listColors.Show(_eColor);
		};
	}

	class _Color : IPopupListItem
	{
		string _text;
		int _color, _backColor;

		public _Color(int color, int backColor)
		{
			_color = color;
			_backColor = backColor;
			_text = "0x" + color.ToString("X6");
		}

		public override string ToString() => _text;

		public string TooltipText { get; }
		public Image Icon { get; }
		public ColorInt BackColor => _backColor;
		public ColorInt TextColor => _color;
		public PLCheckType CheckType { get; }
		public bool Checked { get; set; }
		public bool Disabled { get; }
		public bool BoldFont { get; }
		public short Group { get; }
	}

	[Flags]
	enum _EColorControls { Hex = 1, RGB = 2, HLS = 4, All = 7 }

	[DllImport("gdi32.dll", EntryPoint = "EnumFontFamiliesExW")]
	internal static extern int EnumFontFamiliesEx(IntPtr hdc, in Api.LOGFONT lpLogfont, FONTENUMPROC lpProc, LPARAM lParam, uint dwFlags);
	internal unsafe delegate int FONTENUMPROC(Api.LOGFONT* lf, IntPtr tm, uint fontType, LPARAM lParam);

	[DllImport("shlwapi.dll")]
	internal static extern void ColorRGBToHLS(int clrRGB, out ushort pwHue, out ushort pwLuminance, out ushort pwSaturation);

	[DllImport("shlwapi.dll")]
	internal static extern int ColorHLSToRGB(ushort wHue, ushort wLuminance, ushort wSaturation);

	void _ApplyFont()
	{
		var styles = new CiStyling.TStyles(_sciStyles); //gets colors and bold
		var fname = _cbFont.Text; if(fname == "" || fname.Starts("[ ")) fname = "Consolas";
		styles.FontName = fname;
		int fsize = _UpDownValue(_nFontSize);
		styles.FontSize = fsize;

		if(!styles.Equals(Program.Settings.edit_styles)) {
			Program.Settings.edit_styles = styles;
			Program.Settings.SaveLater();
			foreach(var v in Panels.Editor.ZOpenDocs) styles.ToScintilla(v);
		}

	}

	#endregion

	#region Code info

	unsafe void _InitCodeInfo()
	{
		//_cComplGroupEM.Checked = Program.Settings.ci_complGroupEM; //checkbox "Group all extension methods" (if unchecked, would group only Linq)
		_cComplParenSpace.Checked = Program.Settings.ci_complParenSpace;
	}

	void _ApplyCodeInfo()
	{
		//Program.Settings.ci_complGroupEM = _cComplGroupEM.Checked;
		Program.Settings.ci_complParenSpace = _cComplParenSpace.Checked;
	}

	#endregion

	#region util

	static int _UpDownValue(NumericUpDown c) => Convert.ToInt32(c.Value);

	#endregion
}
