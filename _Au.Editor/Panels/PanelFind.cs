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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using Au.Controls;
using Au.Tools;

class PanelFind : AuUserControlBase
{
	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this._tFind = new PanelFind._SciTextBox();
		this._bFind = new Au.Controls.AuButton();
		this._bFindIF = new Au.Controls.AuButton();
		this._cName = new Au.Controls.AuCheckBox();
		this._cCase = new Au.Controls.AuCheckBox();
		this._cWord = new Au.Controls.AuCheckBox();
		this._cRegex = new Au.Controls.AuCheckBox();
		this._tReplace = new PanelFind._SciTextBox();
		this._bReplace = new Au.Controls.AuButton();
		this._bReplaceAll = new Au.Controls.AuButton();
		this._bOptions = new Au.Controls.AuButton();
		this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
		this._toolTip = new System.Windows.Forms.ToolTip(this.components);
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
		this.SuspendLayout();
		// 
		// tableLayoutPanel1
		// 
		this.tableLayoutPanel1.ColumnCount = 3;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
		this.tableLayoutPanel1.Controls.Add(this._tFind, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this._bFind, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this._bFindIF, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this._cName, 2, 1);
		this.tableLayoutPanel1.Controls.Add(this._cCase, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this._cWord, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this._cRegex, 2, 2);
		this.tableLayoutPanel1.Controls.Add(this._tReplace, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this._bReplace, 0, 4);
		this.tableLayoutPanel1.Controls.Add(this._bReplaceAll, 1, 4);
		this.tableLayoutPanel1.Controls.Add(this._bOptions, 2, 4);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(60, 130);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 5;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(205, 130);
		this.tableLayoutPanel1.TabIndex = 0;
		// 
		// _tFind
		// 
		this.tableLayoutPanel1.SetColumnSpan(this._tFind, 3);
		this._tFind.Dock = System.Windows.Forms.DockStyle.Fill;
		this._tFind.ZInitBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this._tFind.ZInitUseDefaultContextMenu = true;
		this._tFind.Location = new System.Drawing.Point(3, 3);
		this._tFind.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
		this._tFind.MinimumSize = new System.Drawing.Size(4, 17);
		this._tFind.Name = "_tFind";
		this._tFind.Size = new System.Drawing.Size(199, 20);
		this._tFind.TabIndex = 0;
		this._toolTip.SetToolTip(this._tFind, "Text to find");
		this._tFind.ZTextChanged += new System.EventHandler(this._tFind_TextChanged);
		this._tFind.MouseUp += new System.Windows.Forms.MouseEventHandler(this._tFind_MouseUp);
		// 
		// _bFind
		// 
		this._bFind.Dock = System.Windows.Forms.DockStyle.Fill;
		this._bFind.Location = new System.Drawing.Point(3, 26);
		this._bFind.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
		this._bFind.Name = "_bFind";
		this._bFind.Size = new System.Drawing.Size(64, 22);
		this._bFind.TabIndex = 1;
		this._bFind.Text = "Find";
		this._toolTip.SetToolTip(this._bFind, "Find next match in editor");
		this._bFind.Click += new System.EventHandler(this._bFind_Click);
		// 
		// _bFindIF
		// 
		this._bFindIF.Dock = System.Windows.Forms.DockStyle.Fill;
		this._bFindIF.Location = new System.Drawing.Point(70, 26);
		this._bFindIF.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this._bFindIF.Name = "_bFindIF";
		this._bFindIF.Size = new System.Drawing.Size(64, 22);
		this._bFindIF.TabIndex = 2;
		this._bFindIF.Text = "in files";
		this._toolTip.SetToolTip(this._bFindIF, "Search in files.\r\nTo skip some files, click the ... button.");
		this._bFindIF.Click += new System.EventHandler(this._bFindIF_Click);
		// 
		// _cName
		// 
		this._cName.AutoSize = true;
		this._cName.Location = new System.Drawing.Point(137, 26);
		this._cName.Margin = new System.Windows.Forms.Padding(2, 4, 3, 3);
		this._cName.Name = "_cName";
		this._cName.Size = new System.Drawing.Size(65, 22);
		this._cName.TabIndex = 3;
		this._cName.Text = "Name";
		this._toolTip.SetToolTip(this._cName, "Search in filenames");
		this._cName.CheckedChanged += new System.EventHandler(this._cName_CheckedChanged);
		// 
		// _cCase
		// 
		this._cCase.AutoSize = true;
		this._cCase.Location = new System.Drawing.Point(4, 54);
		this._cCase.Margin = new System.Windows.Forms.Padding(4, 3, 3, 3);
		this._cCase.Name = "_cCase";
		this._cCase.Size = new System.Drawing.Size(57, 20);
		this._cCase.TabIndex = 4;
		this._cCase.Text = "Case";
		this._toolTip.SetToolTip(this._cCase, "Match case");
		this._cCase.CheckedChanged += new System.EventHandler(this._cCase_CheckedChanged);
		// 
		// _cWord
		// 
		this._cWord.AutoSize = true;
		this._cWord.Location = new System.Drawing.Point(71, 54);
		this._cWord.Name = "_cWord";
		this._cWord.Size = new System.Drawing.Size(61, 20);
		this._cWord.TabIndex = 5;
		this._cWord.Text = "Word";
		this._toolTip.SetToolTip(this._cWord, "Whole word");
		this._cWord.CheckedChanged += new System.EventHandler(this._cWord_CheckedChanged);
		// 
		// _cRegex
		// 
		this._cRegex.AutoSize = true;
		this._cRegex.Location = new System.Drawing.Point(138, 54);
		this._cRegex.Margin = new System.Windows.Forms.Padding(2, 3, 3, 3);
		this._cRegex.Name = "_cRegex";
		this._cRegex.Size = new System.Drawing.Size(64, 20);
		this._cRegex.TabIndex = 6;
		this._cRegex.Text = "Regex";
		this._toolTip.SetToolTip(this._cRegex, "Regular expression");
		this._cRegex.CheckedChanged += new System.EventHandler(this._cRegex_CheckedChanged);
		// 
		// _tReplace
		// 
		this.tableLayoutPanel1.SetColumnSpan(this._tReplace, 3);
		this._tReplace.Dock = System.Windows.Forms.DockStyle.Fill;
		this._tReplace.ZInitBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this._tReplace.ZInitUseDefaultContextMenu = true;
		this._tReplace.Location = new System.Drawing.Point(3, 82);
		this._tReplace.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
		this._tReplace.MinimumSize = new System.Drawing.Size(4, 17);
		this._tReplace.Name = "_tReplace";
		this._tReplace.Size = new System.Drawing.Size(199, 20);
		this._tReplace.TabIndex = 7;
		this._toolTip.SetToolTip(this._tReplace, "Replacement text");
		this._tReplace.MouseUp += new System.Windows.Forms.MouseEventHandler(this._tFind_MouseUp);
		// 
		// _bReplace
		// 
		this._bReplace.Dock = System.Windows.Forms.DockStyle.Fill;
		this._bReplace.Location = new System.Drawing.Point(3, 105);
		this._bReplace.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
		this._bReplace.Name = "_bReplace";
		this._bReplace.Size = new System.Drawing.Size(64, 22);
		this._bReplace.TabIndex = 8;
		this._bReplace.Text = "Replace";
		this._toolTip.SetToolTip(this._bReplace, "Replace single match in editor.\r\nRight click - find next match.");
		this._bReplace.Click += new System.EventHandler(this._bReplace_Click);
		this._bReplace.MouseUp += new System.Windows.Forms.MouseEventHandler(this._bReplace_MouseUp);
		// 
		// _bReplaceAll
		// 
		this._bReplaceAll.Dock = System.Windows.Forms.DockStyle.Fill;
		this._bReplaceAll.Location = new System.Drawing.Point(70, 105);
		this._bReplaceAll.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this._bReplaceAll.Name = "_bReplaceAll";
		this._bReplaceAll.Size = new System.Drawing.Size(64, 22);
		this._bReplaceAll.TabIndex = 9;
		this._bReplaceAll.Text = "all";
		this._toolTip.SetToolTip(this._bReplaceAll, "Replaces all matches in editor");
		this._bReplaceAll.Click += new System.EventHandler(this._bReplaceAll_Click);
		// 
		// _bOptions
		// 
		this._bOptions.Dock = System.Windows.Forms.DockStyle.Right;
		this._bOptions.Location = new System.Drawing.Point(168, 105);
		this._bOptions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this._bOptions.Name = "_bOptions";
		this._bOptions.Size = new System.Drawing.Size(35, 22);
		this._bOptions.TabIndex = 10;
		this._bOptions.Text = "...";
		this._toolTip.SetToolTip(this._bOptions, "Options");
		this._bOptions.Click += new System.EventHandler(this._bOptions_Click);
		// 
		// _errorProvider
		// 
		this._errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
		this._errorProvider.ContainerControl = this;
		// 
		// PanelFind
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.Controls.Add(this.tableLayoutPanel1);
		this.Name = "PanelFind";
		this.Size = new System.Drawing.Size(205, 130);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
		this.ResumeLayout(false);

	}

	private TableLayoutPanel tableLayoutPanel1;
	private _SciTextBox _tFind;
	private AuButton _bFind;
	private AuButton _bFindIF;
	private AuCheckBox _cName;
	private AuCheckBox _cCase;
	private AuCheckBox _cWord;
	private AuCheckBox _cRegex;
	private _SciTextBox _tReplace;
	private AuButton _bReplace;
	private AuButton _bReplaceAll;
	private AuButton _bOptions;
	private ErrorProvider _errorProvider;
	private ToolTip _toolTip;
	private IContainer components;

	ComboWrapper _comboFind, _comboReplace;

	public PanelFind()
	{
		InitializeComponent();
		this.AccessibleName = this.Name = "Find"; //note: VS designer bug: changes Name to be = the class name.

		_tFind._f = this;
		_tReplace._f = this;
		_comboFind = new ComboWrapper(_tFind);
		_comboReplace = new ComboWrapper(_tReplace);
		_comboFind.ArrowButtonPressed += _comboFindReplace_ArrowButtonPressed;
		_comboReplace.ArrowButtonPressed += _comboFindReplace_ArrowButtonPressed;
	}

	#region control events

	private void _tFind_MouseUp(object sender, MouseEventArgs e)
	{
		(sender as _SciTextBox).Z.ClearText();
	}

	private void _tFind_TextChanged(object sender, EventArgs e)
	{
		ZUpdateQuickResults(false);
	}

	private void _cCase_CheckedChanged(object sender, EventArgs e)
	{
		ZUpdateQuickResults(false);
	}

	private void _cWord_CheckedChanged(object sender, EventArgs e)
	{
		if(_cWord.Checked) _cRegex.Checked = false;
		ZUpdateQuickResults(false);
	}

	private void _cRegex_CheckedChanged(object sender, EventArgs e)
	{
		if(_cRegex.Checked) {
			_cWord.Checked = false;
			if(_regexWindow == null) {
				_regexWindow = new RegexWindow();
			}
		} else {
			_regexWindow?.Dispose();
			_regexWindow = null;
		}
		ZUpdateQuickResults(false);
	}

	RegexWindow _regexWindow;
	string _regexTopic;

	void _ShowRegexInfo(_SciTextBox tb, bool F1)
	{
		if(_regexWindow == null || !_cRegex.Checked) return;
		if(_regexWindow.UserClosed) { if(!F1) return; _regexWindow.UserClosed = false; }

		if(!_regexWindow.Window.IsHandleCreated) {
			var r = ((AWnd)this).Rect;
			r.Offset(0, -20);
			_regexWindow.Show(Program.MainForm, r, true);
		} else _regexWindow.Window.Show();

		_regexWindow.InsertInControl = tb;

		bool replace = tb == _tReplace;
		var s = _regexWindow.CurrentTopic;
		if(s == "replace") {
			if(!replace) _regexWindow.CurrentTopic = _regexTopic;
		} else if(replace) {
			_regexTopic = s;
			_regexWindow.CurrentTopic = "replace";
		}
	}

	void _OnGotLostFocus(bool got, _SciTextBox tb)
	{
		if(!_cRegex.Checked) return;
		if(got) {
			//use timer to avoid temporary focus problems, for example when tabbing quickly or closing active Regex window
			ATimer.After(70, _ => { if(tb.Focused) _ShowRegexInfo(tb, false); });
		} else {
			if(_regexWindow.Window.Visible) {
				var c = AWnd.ThisThread.FocusedControl;
				if(c == null || (c != _tFind && c != _tReplace && c != _regexWindow.Window && c.TopLevelControl != _regexWindow.Window)) {
					_regexWindow.Hide();
				}
			}
		}
	}

	private void _bFind_Click(object sender, EventArgs e)
	{
		_cName.Checked = false;
		if(!_GetTextToFind(out var f, false)) return;
		_FindNextInEditor(f, false);
	}

	private void _bFindIF_Click(object sender, EventArgs e)
	{
		_cName.Checked = false;
		using var _ = new _TempDisableControl(_bFindIF);
		_FindAllInFiles(false);
	}

	private void _cName_CheckedChanged(object sender, EventArgs e)
	{
		Panels.Found.ZControl.Z.ClearText();
		if(_cName.Checked) {
			_aEditor.Clear();
			Panels.Editor.ZActiveDoc?._InicatorsFind(null);
		}
		ZUpdateQuickResults(false);
	}

	private void _bReplace_Click(object sender, EventArgs e)
	{
		_cName.Checked = false;
		if(!_GetTextToFind(out var f, true)) return;
		_FindNextInEditor(f, true);
	}

	private void _bOptions_Click(object sender, EventArgs e)
	{
		using var f = new FFindOptions();
		f._tSkip.Text = string.Join("\r\n", _SkipWildcards);
		f._cbSearchIn.SelectedIndex = _SearchIn;

		if(f.ShowDialog(this) != DialogResult.OK) return;

		Program.Settings.find_skip = f._tSkip.Text; _searchIn = -1;
		Program.Settings.find_searchIn = f._cbSearchIn.SelectedIndex; _aSkipWildcards = null;
	}

	#endregion

	#region common

	/// <summary>
	/// Called when changed find text or options. Also when activated another document.
	/// Async-updates find-hiliting in editor or 'find name' results.
	/// </summary>
	public void ZUpdateQuickResults(bool onlyEditor)
	{
		if(!Visible) return;
		if(onlyEditor && _cName.Checked) return;
		//AOutput.Write("UpdateQuickResults", Visible);

		_timerUE ??= new ATimer(_ => {
			if(_cName.Checked) {
				_FindAllInFiles(true);
			} else {
				_FindAllInEditor();
				Panels.Editor.ZActiveDoc?._InicatorsFind(_aEditor);
			}
		});

		_timerUE.After(150);
	}
	ATimer _timerUE;

	struct _TextToFind
	{
		public string findText;
		public string replaceText;
		public ARegex rx;
		public bool wholeWord;
		public bool matchCase;
	}

	bool _GetTextToFind(out _TextToFind f, bool forReplace, bool noRecent = false)
	{
		_errorProvider.Clear();
		f = default;
		f.findText = _tFind.Text;
		if(f.findText.Length == 0) return false;
		f.matchCase = _cCase.Checked;
		if(_cRegex.Checked) {
			try {
				var fl = RXFlags.MULTILINE;
				if(!f.matchCase) fl |= RXFlags.CASELESS;
				f.rx = new ARegex(f.findText, flags: fl);
			}
			catch(ArgumentException e) {
				_SetErrorProvider(_tFind, e.Message);
				return false;
			}
		} else f.wholeWord = _cWord.Checked;
		if(forReplace) f.replaceText = _tReplace.Text;

		_AddToRecent(f, noRecent);

		return true;
	}

	void _FindAllInString(string text, in _TextToFind f, List<Range> a, bool one = false)
	{
		a.Clear();
		if(f.rx != null) {
			foreach(var g in f.rx.FindAllG(text)) {
				a.Add(g.Start..g.End);
				if(one) break;
			}
		} else {
			for(int i = 0; i < text.Length; i += f.findText.Length) {
				i = f.wholeWord ? text.FindWord(f.findText, i.., !f.matchCase, "_") : text.Find(f.findText, i, !f.matchCase);
				if(i < 0) break;
				a.Add(i..(i + f.findText.Length));
				if(one) break;
			}
		}
	}

	void _SetErrorProvider(Control c, string text)
	{
		_errorProvider.SetIconAlignment(c, ErrorIconAlignment.BottomRight);
		_errorProvider.SetIconPadding(c, -16);
		_errorProvider.SetError(c, text);
	}

	#endregion

	#region in editor

	void _FindNextInEditor(in _TextToFind f, bool replace)
	{
		var doc = Panels.Editor.ZActiveDoc; if(doc == null) return;
		var z = doc.Z;
		var text = doc.Text; if(text.Length == 0) return;
		int i, len = 0, from8 = replace ? z.SelectionStart8 : z.SelectionEnd8, from = doc.Pos16(from8);
		RXMatch rm = null;
		bool retryFromStart = false, retryRx = false;
		g1:
		if(f.rx != null) {
			if(f.rx.Match(text, out rm, from..)) {
				i = rm.Start;
				len = rm.Length;
				if(i == from && len == 0 && !(replace | retryRx | retryFromStart)) {
					if(++i > text.Length) i = -1;
					else {
						if(i < text.Length) if(text.Eq(i - 1, "\r\n") || char.IsSurrogatePair(text, i - 1)) i++;
						from = i; retryRx = true; goto g1;
					}
				}
				if(len == 0) doc.Focus();
			} else i = -1;
		} else {
			i = f.wholeWord ? text.FindWord(f.findText, from.., !f.matchCase, "_") : text.Find(f.findText, from, !f.matchCase);
			len = f.findText.Length;
		}
		//AOutput.Write(from, i, len);
		if(i < 0) {
			if(retryFromStart || from8 == 0) return;
			from = 0; retryFromStart = true; replace = false; goto g1;
		}
		int to = doc.Pos8(i + len);
		i = doc.Pos8(i);
		if(replace && i == from8 && to == z.SelectionEnd8) {
			var repl = f.replaceText;
			if(rm != null) repl = rm.ExpandReplacement(repl);
			//z.ReplaceRange(i, to, repl); //also would need to set caret pos = to
			z.ReplaceSel(repl);
			_FindNextInEditor(f, false);
		} else {
			z.Select(false, i, to, true);
		}
	}

	private void _bReplace_MouseUp(object sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Right) _bFind_Click(sender, e);
	}

	private void _bReplaceAll_Click(object sender, EventArgs e)
	{
		_cName.Checked = false;
		if(!_GetTextToFind(out var f, true)) return;
		var doc = Panels.Editor.ZActiveDoc; if(doc == null) return;
		var text = doc.Text;
		var repl = f.replaceText;
		if(f.rx != null) {
			if(!f.rx.FindAll(text, out var ma)) return;
			doc.Call(Sci.SCI_BEGINUNDOACTION);
			for(int i = ma.Length - 1; i >= 0; i--) {
				var m = ma[i];
				doc.Z.ReplaceRange(true, m.Start, m.End, m.ExpandReplacement(repl));
			}
			doc.Call(Sci.SCI_ENDUNDOACTION);
		} else {
			var a = _aEditor;
			_FindAllInString(text, f, a);
			if(a.Count == 0) return;
			doc.Call(Sci.SCI_BEGINUNDOACTION);
			for(int i = a.Count - 1; i >= 0; i--) {
				var v = a[i];
				doc.Z.ReplaceRange(true, v.Start.Value, v.End.Value, repl);
			}
			doc.Call(Sci.SCI_ENDUNDOACTION);
		}
		//Easier/faster would be to create new text and call Z.SetText. But then all non-text data is lost: markers, folds, caret position...
	}

	List<Range> _aEditor = new List<Range>(); //all found in editor text

	void _FindAllInEditor()
	{
		_aEditor.Clear();
		if(!_GetTextToFind(out var f, false, noRecent: true)) return;
		var text = Panels.Editor.ZActiveDoc?.Text; if(text.IsNE()) return;
		_FindAllInString(text, f, _aEditor);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		if(!_cName.Checked) Panels.Editor.ZActiveDoc?._InicatorsFind(Visible ? _aEditor : null);
	}

	/// <summary>
	/// Makes visible and sets find text = selected text of c (can be null).
	/// </summary>
	public void ZCtrlF(Control c)
	{
		if(!Visible) Panels.PanelManager.ZGetPanel(this).Visible = true;
		string s = "";
		switch(c) {
		case AuScintilla k:
			s = k.Z.SelectedText();
			break;
		case TextBox k:
			s = k.SelectedText;
			break;
		}
		_tFind.Focus();
		if(s.Length == 0) return;
		_tFind.Text = s;
		_tFind.Call(Sci.SCI_SELECTALL);
	}

	/// <summary>
	/// Makes visible and sets find text = selected text of focused control.
	/// </summary>
	public void ZCtrlF() => ZCtrlF(AWnd.ThisThread.FocusedControl);

	#endregion

	#region in files

	int _SearchIn => _searchIn >= 0 ? _searchIn : (_searchIn = Program.Settings.find_searchIn);
	int _searchIn = -1;

	string[] _SkipWildcards => _aSkipWildcards ??= (Program.Settings.find_skip ?? "").SegSplit("\r\n", SegFlags.NoEmpty);
	string[] _aSkipWildcards;
	string[] _aSkipImages = new string[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".ico", ".cur", ".ani" };
	bool _init1;
	const int c_indic = 0;

	void _FindAllInFiles(bool names/*, bool forReplace*/)
	{
		if(!_GetTextToFind(out var f, false, noRecent: names)) {
			Panels.Found.ZControl.Z.ClearText();
			return;
		}

		if(!_init1) {
			_init1 = true;
			var c = Panels.Found.ZControl;
			c.Hwnd(create: true);
			Panels.Files.ZWorkspaceLoadedAndDocumentsOpened += () => Panels.Found.ZControl.Z.ClearText();

			c.ZTags.AddLinkTag("+open", s => {
				_OpenLinkClicked(s);
			});
			c.ZTags.AddLinkTag("+ra", s => {
				if(!_OpenLinkClicked(s)) return;
				ATimer.After(10, _ => _bReplaceAll_Click(null, null));
				//info: without timer sometimes does not set cursor pos correctly
			});
			c.ZTags.AddLinkTag("+f", s => {
				var a = s.Split(' ');
				if(!_OpenLinkClicked(a[0])) return;
				var doc = Panels.Editor.ZActiveDoc;
				//doc.Focus();
				int from = a[1].ToInt(), to = a[2].ToInt();
				ATimer.After(10, _ => doc.Z.Select(true, from, to, true));
				//info: scrolling works better with async when now opened the file
			});
			bool _OpenLinkClicked(string file)
			{
				var f = Program.Model.Find(file, null); //<id>
				if(f == null) return false;
				if(f.IsFolder) f.SelectSingle();
				else if(!Program.Model.SetCurrentFile(f)) return false;
				//add indicator to make it easier to find later
				var z = Panels.Found.ZControl.Z;
				z.IndicatorClear(c_indic);
				var v = z.LineStartEndFromPos(false, z.CurrentPos8);
				z.IndicatorAdd(false, c_indic, v.start..v.end);
				return true;
			}
			c.Call(Sci.SCI_INDICSETSTYLE, c_indic, Sci.INDIC_BOX);
			c.Call(Sci.SCI_INDICSETFORE, c_indic, 0x0080e0);
		}

		var b = new StringBuilder();
		var a = new List<Range>();
		var bSlow = !names && f.rx != null ? new StringBuilder() : null;
		bool jited = false;
		int searchIn = names ? 0 : _SearchIn;

		foreach(var v in Program.Model.Root.Descendants()) {
			string text = null, path = null;
			if(names) {
				text = v.Name;
			} else {
				//APerf.First();
				if(v.IsCodeFile) {
					switch(searchIn) { //0 all, 1 C#, 2 script, 3 class, 4 other
					case 4: continue;
					case 2 when !v.IsScript: continue;
					case 3 when !v.IsClass: continue;
					}
				} else {
					if(searchIn >= 1 && searchIn <= 3) continue;
					if(v.IsFolder) continue;
					if(0 != v.Name.Ends(true, _aSkipImages)) continue;
				}
				var sw = _SkipWildcards; if(sw.Length != 0 && 0 != (path = v.ItemPath).Like(true, sw)) continue;
				text = v.GetText();
				if(text.Length == 0) continue;
				if(text.Contains('\0')) continue;
				//APerf.NW();
			}

			long time = bSlow != null ? ATime.PerfMilliseconds : 0;

			_FindAllInString(text, f, a, one: names);

			if(a.Count != 0) {
				if(!names) b.Append("<Z 0xC0E0C0>");
				path ??= v.ItemPath;
				string link = v.IdStringWithWorkspace;
				if(v.IsFolder) {
					b.AppendFormat("<+open \"{0}\"><c 0x808080>{1}<><>    <c 0x008000>//folder<>", link, path);
				} else {
					int i1 = path.LastIndexOf('\\') + 1;
					string s1 = path.Remove(i1), s2 = path.Substring(i1);
					if(names) {
						b.AppendFormat("<+open \"{0}\"><c 0x808080>{1}<>{2}<>", link, s1, s2);
					} else {
						int ns = 120 - path.Length * 7 / 4;
#if true //open and select the first found text
						b.AppendFormat("<+f \"{0} {1} {2}\"><c 0x808080>{3}<><b>{4}{5}      <><>    <+ra \"{0}\"><c 0x80ff>Replace all<><>",
							link, a[0].Start.Value, a[0].End.Value, s1, s2, ns > 0 ? new string(' ', ns) : null);
#else //just open
						b.AppendFormat("<+open \"{0}\"><c 0x808080>{1}<><b>{2}{3}      <><>    <+ra \"{0}\"><c 0x80ff>Replace all<><>",
							link, s1, s2, ns > 0 ? new string(' ', ns) : null);
#endif
					}
				}
				if(!names) b.Append("<>");
				b.AppendLine();
				if(names) {
					//FUTURE: icon; maybe hilite.
				} else {
					if(b.Length < 10_000_000) {
						for(int i = 0; i < a.Count; i++) {
							var range = a[i];
							int start = range.Start.Value, end = range.End.Value, lineStart = start, lineEnd = end;
							int lsMax = Math.Max(lineStart - 100, 0), leMax = Math.Min(lineEnd + 200, text.Length); //start/end limits like in VS
							for(; lineStart > lsMax; lineStart--) { char c = text[lineStart - 1]; if(c == '\n' || c == '\r') break; }
							bool limitStart = lineStart == lsMax && lineStart > 0;
							for(; lineEnd < leMax; lineEnd++) { char c = text[lineEnd]; if(c == '\r' || c == '\n') break; }
							bool limitEnd = lineEnd == leMax && lineEnd < text.Length;
							b.AppendFormat("<+f \"{0} {1} {2}\">", link, start.ToString(), end.ToString())
								.Append(limitStart ? "…<\a>" : "<\a>").Append(text, lineStart, start - lineStart).Append("</\a>")
								.Append("<z 0xffff5f><\a>").Append(text, start, end - start).Append("</\a><>")
								.Append("<\a>").Append(text, end, lineEnd - end).Append(limitEnd ? "</\a>…" : "</\a>")
								.AppendLine("<>");
						}
					}
				}
			}

			if(bSlow != null) {
				time = ATime.PerfMilliseconds - time;
				if(!jited) jited = true;
				else if(time >= 100) {
					if(bSlow.Length == 0) bSlow.AppendLine("<Z orange>Slow in these files<>");
					bSlow.Append(time).Append(" ms in <open>").Append(v.ItemPath).Append("<> , length ").Append(text.Length).AppendLine();
				}
			}
		}

		if(searchIn > 0) b.Append("<Z orange>Note: searched only in ")
			.Append(searchIn switch { 1 => "C#", 2 => "C# script", 3 => "C# class", _ => "non-C#" })
			.AppendLine(" files. It is set in Find options (the ... button).<>");
		b.Append(bSlow);

		Panels.Found.ZControl.Z.SetText(b.ToString());
		var ip = Panels.PanelManager.ZGetPanel(Panels.Found);
		ip.Visible = true;
	}

	//rejected: replace in files. Rarely used, dangerous, need much work to make more useful. It's easy to click each file in results and click 'Replace' or 'all'.
	//private void _bReplaceIF_Click(object sender, EventArgs e)
	//{
	//}

	struct _TempDisableControl : IDisposable
	{
		AWnd _w;
		int _enableAfter;

		public _TempDisableControl(Control c, int enableAfter = 0)
		{
			_w = (AWnd)c;
			_enableAfter = enableAfter;
			_w.Enable(false);
			Api.UpdateWindow(_w);
			//note: this does not work correctly: c.Enabled=false; c.Update();
		}

		public void Dispose()
		{
			if(_enableAfter == 0) _w.Enable(true);
			else { var w = _w; ATimer.After(_enableAfter, _ => w.Enable(true)); }
		}
	}

	#endregion

	#region recent

	public class RecentItem //not struct because used with PopupList
	{
		public string t { get; set; } //not fields because used with JsonSerializer
		public int o { get; set; }

		public override string ToString() => t.Limit(100); //PopupList item display text
	}

	string _recentPrevFind, _recentPrevReplace;
	int _recentPrevOptions;

	//temp is false when clicked a button, true when changed the find text or a checkbox.
	void _AddToRecent(in _TextToFind f, bool temp)
	{
		if(temp) return; //not implemented. Was implemented, but was not perfect and probably not useful. Adds too many intermediate garbage, although filtered.

		int k = f.matchCase ? 1 : 0; if(f.wholeWord) k |= 2; else if(f.rx != null) k |= 4;

		if(f.findText != _recentPrevFind || k != _recentPrevOptions) _Add(false, _recentPrevFind = f.findText, _recentPrevOptions = k);
		if(!f.replaceText.IsNE() && f.replaceText != _recentPrevReplace) _Add(true, _recentPrevReplace = f.replaceText, 0);

		static void _Add(bool replace, string text, int options)
		{
			if(text.Length > 1000) {
				//if(0 != (options & 4)) AWarning.Write("The find text of length > 1000 will not be saved to 'recent'.", -1);
				return;
			}
			var a = (replace ? Program.Settings.find_recentReplace : Program.Settings.find_recent) ?? new RecentItem[0];
			for(int i = a.Length - 1; i >= 0; i--) if(a[i].t == text) a = a.RemoveAt(i); //avoid duplicates
			if(a.Length >= 20) a = a[0..19]; //limit count
			a = a.InsertAt(0, new RecentItem { t = text, o = options });
			if(replace) Program.Settings.find_recentReplace = a; else Program.Settings.find_recent = a;
		}
	}

	private void _comboFindReplace_ArrowButtonPressed(object sender, EventArgs e)
	{
		bool replace = sender == _comboReplace;
		var a = (replace ? Program.Settings.find_recentReplace : Program.Settings.find_recent) ?? new RecentItem[0];
		if(a == null) return;
		var c = replace ? _tReplace : _tFind;
		var m = new PopupList { IsModal = true, ComboBoxAnimation = true };
		m.Items = a;
		m.SelectedAction = o => {
			var r = o.ResultItem as RecentItem;
			c.Text = r.t;
			if(!replace) {
				int k = r.o;
				_cCase.Checked = 0 != (k & 1);
				_cWord.Checked = 0 != (k & 2);
				_cRegex.Checked = 0 != (k & 4);
			}
		};
		m.Show(c);
	}

	#endregion

	#region _SciTextBox

	class _SciTextBox : AuScintilla
	{
		internal PanelFind _f; //caller sets this, because the forms designer does not support ctor with parameters

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			Z.MarginWidth(1, 0);
			Call(Sci.SCI_SETHSCROLLBAR);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData & Keys.KeyCode) {
			case Keys.Tab: return false;
			}
			return base.IsInputKey(keyData);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			_f._OnGotLostFocus(true, this);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			_f._OnGotLostFocus(false, this);
		}

		protected override void OnHelpRequested(HelpEventArgs he)
		{
			he.Handled = true;
			_f._ShowRegexInfo(this, true);
			base.OnHelpRequested(he);
		}
	}

	#endregion
}
