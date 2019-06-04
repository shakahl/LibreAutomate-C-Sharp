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
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using static Program;

class Find : AUserControlBase
{
	private void InitializeComponent()
	{
			this.components = new System.ComponentModel.Container();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._tFind = new Au.Controls.AuComboBox();
			this._bFind = new Au.Controls.AuButton();
			this._bFindIF = new Au.Controls.AuButton();
			this._bName = new Au.Controls.AuButton();
			this._cCase = new Au.Controls.AuCheckBox();
			this._cWord = new Au.Controls.AuCheckBox();
			this._cRegex = new Au.Controls.AuCheckBox();
			this._tReplace = new Au.Controls.AuComboBox();
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
			this.tableLayoutPanel1.Controls.Add(this._bName, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this._cCase, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._cWord, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._cRegex, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this._tReplace, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._bReplace, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._bReplaceAll, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._bOptions, 2, 4);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(60, 142);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(205, 142);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// _tFind
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._tFind, 3);
			this._tFind.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tFind.Location = new System.Drawing.Point(3, 3);
			this._tFind.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._tFind.MinimumSize = new System.Drawing.Size(4, 21);
			this._tFind.Multiline = true;
			this._tFind.Name = "_tFind";
			this._tFind.Size = new System.Drawing.Size(199, 23);
			this._tFind.TabIndex = 0;
			this._toolTip.SetToolTip(this._tFind, "Text to find");
			this._tFind.ArrowButtonPressed += new System.EventHandler(this._tFind_ArrowButtonPressed);
			this._tFind.ImageButtonClicked += new System.EventHandler(this._tFind_ImageButtonClicked);
			this._tFind.TextChanged += new System.EventHandler(this._tFind_TextChanged);
			// 
			// _bFind
			// 
			this._bFind.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bFind.Location = new System.Drawing.Point(3, 29);
			this._bFind.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
			this._bFind.Name = "_bFind";
			this._bFind.Size = new System.Drawing.Size(64, 24);
			this._bFind.TabIndex = 2;
			this._bFind.Text = "Find";
			this._toolTip.SetToolTip(this._bFind, "Find next match in editor");
			this._bFind.Click += new System.EventHandler(this._bFind_Click);
			// 
			// _bFindIF
			// 
			this._bFindIF.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bFindIF.Location = new System.Drawing.Point(70, 29);
			this._bFindIF.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this._bFindIF.Name = "_bFindIF";
			this._bFindIF.Size = new System.Drawing.Size(64, 24);
			this._bFindIF.TabIndex = 3;
			this._bFindIF.Text = "in files";
			this._toolTip.SetToolTip(this._bFindIF, "Search in files.\r\nTo skip some files, click the ... button.");
			this._bFindIF.Click += new System.EventHandler(this._bFindIF_Click);
			// 
			// _bName
			// 
			this._bName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bName.Location = new System.Drawing.Point(137, 29);
			this._bName.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
			this._bName.Name = "_bName";
			this._bName.Size = new System.Drawing.Size(65, 24);
			this._bName.TabIndex = 3;
			this._bName.Text = "name";
			this._toolTip.SetToolTip(this._bName, "Search in filenames");
			this._bName.Click += new System.EventHandler(this._bName_Click);
			// 
			// _cCase
			// 
			this._cCase.AutoSize = true;
			this._cCase.Location = new System.Drawing.Point(4, 59);
			this._cCase.Margin = new System.Windows.Forms.Padding(4, 3, 3, 3);
			this._cCase.Name = "_cCase";
			this._cCase.Size = new System.Drawing.Size(57, 20);
			this._cCase.TabIndex = 0;
			this._cCase.Text = "Case";
			this._toolTip.SetToolTip(this._cCase, "Match case");
			this._cCase.CheckedChanged += new System.EventHandler(this._cCase_CheckedChanged);
			// 
			// _cWord
			// 
			this._cWord.AutoSize = true;
			this._cWord.Location = new System.Drawing.Point(71, 59);
			this._cWord.Name = "_cWord";
			this._cWord.Size = new System.Drawing.Size(61, 20);
			this._cWord.TabIndex = 0;
			this._cWord.Text = "Word";
			this._toolTip.SetToolTip(this._cWord, "Whole word");
			this._cWord.CheckedChanged += new System.EventHandler(this._cWord_CheckedChanged);
			// 
			// _cRegex
			// 
			this._cRegex.AutoSize = true;
			this._cRegex.Location = new System.Drawing.Point(138, 59);
			this._cRegex.Margin = new System.Windows.Forms.Padding(2, 3, 3, 3);
			this._cRegex.Name = "_cRegex";
			this._cRegex.Size = new System.Drawing.Size(64, 20);
			this._cRegex.TabIndex = 0;
			this._cRegex.Text = "Regex";
			this._toolTip.SetToolTip(this._cRegex, "Regular expression");
			this._cRegex.CheckedChanged += new System.EventHandler(this._cRegex_CheckedChanged);
			// 
			// _tReplace
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._tReplace, 3);
			this._tReplace.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tReplace.Location = new System.Drawing.Point(3, 89);
			this._tReplace.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._tReplace.MinimumSize = new System.Drawing.Size(4, 21);
			this._tReplace.Multiline = true;
			this._tReplace.Name = "_tReplace";
			this._tReplace.Size = new System.Drawing.Size(199, 23);
			this._tReplace.TabIndex = 1;
			this._toolTip.SetToolTip(this._tReplace, "Replacement text");
			this._tReplace.TextChanged += new System.EventHandler(this._tReplace_TextChanged);
			// 
			// _bReplace
			// 
			this._bReplace.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bReplace.Location = new System.Drawing.Point(3, 115);
			this._bReplace.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
			this._bReplace.Name = "_bReplace";
			this._bReplace.Size = new System.Drawing.Size(64, 24);
			this._bReplace.TabIndex = 2;
			this._bReplace.Text = "Replace";
			this._toolTip.SetToolTip(this._bReplace, "Replace single match in editor.\r\nRight click - find next match.");
			this._bReplace.Click += new System.EventHandler(this._bReplace_Click);
			this._bReplace.MouseUp += new System.Windows.Forms.MouseEventHandler(this._bReplace_MouseUp);
			// 
			// _bReplaceAll
			// 
			this._bReplaceAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bReplaceAll.Location = new System.Drawing.Point(70, 115);
			this._bReplaceAll.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this._bReplaceAll.Name = "_bReplaceAll";
			this._bReplaceAll.Size = new System.Drawing.Size(64, 24);
			this._bReplaceAll.TabIndex = 2;
			this._bReplaceAll.Text = "all";
			this._toolTip.SetToolTip(this._bReplaceAll, "Replaces all matches in editor");
			this._bReplaceAll.Click += new System.EventHandler(this._bReplaceAll_Click);
			// 
			// _bOptions
			// 
			this._bOptions.Dock = System.Windows.Forms.DockStyle.Right;
			this._bOptions.Location = new System.Drawing.Point(168, 115);
			this._bOptions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this._bOptions.Name = "_bOptions";
			this._bOptions.Size = new System.Drawing.Size(35, 24);
			this._bOptions.TabIndex = 2;
			this._bOptions.Text = "...";
			this._toolTip.SetToolTip(this._bOptions, "Options");
			this._bOptions.Click += new System.EventHandler(this._bOptions_Click);
			// 
			// _errorProvider
			// 
			this._errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this._errorProvider.ContainerControl = this;
			// 
			// Find
			// 
			this.AccessibleName = "Find";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "Find";
			this.Size = new System.Drawing.Size(205, 142);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

	}

	private TableLayoutPanel tableLayoutPanel1;
	private AuComboBox _tFind;
	private AuButton _bFind;
	private AuButton _bFindIF;
	private AuButton _bName;
	private AuCheckBox _cCase;
	private AuCheckBox _cWord;
	private AuCheckBox _cRegex;
	private AuComboBox _tReplace;
	private AuButton _bReplace;
	private AuButton _bReplaceAll;
	private AuButton _bOptions;
	private ErrorProvider _errorProvider;
	private ToolTip _toolTip;
	private IContainer components;

	public Find()
	{
		InitializeComponent();

		//the following code is to test AuComboBox

		//_tFind.NoArrow = true;
		//_tFind.ButtonIcon = AIcon.GetAppIcon(16);
		_tFind.ButtonImage = Project.Properties.Resources.folderOpen;
		//_tFind.IconButtonClicked += _tFind_IconButtonClicked;

		//_tFind.Multiline = false;
		//_tFind.ScrollBars = ScrollBars.Vertical;

		//_tFind.Text = "init";
		//_tFind.ReadOnly = true;

		//ATimer.After(2000, () => _tFind.ButtonIcon = AIcon.GetStockIcon(StockIcon.DESKTOPPC, 16));
		//ATimer.After(2000, () => _tFind.NoArrow = true);

		//EdDebug.PrintTabOrder(this);
		//Print("----");
		//ATimer.After(100, () => {
		//	//TabIndex = 0; TabStop = true;
		//	//Print(TabStop, TabIndex);
		//	EdDebug.PrintTabOrder(this);
		//});
	}

	//protected override void OnGotFocus(EventArgs e) { _tFind.Focus(); }

	#region control events

	private void _tFind_TextChanged(object sender, EventArgs e)
	{
		var c = sender as TextBox;
		_AutoScrollbar(c);
		UpdateEditor();
		//_replacePreview = false;
	}

	private void _tReplace_TextChanged(object sender, EventArgs e)
	{
		var c = sender as TextBox;
		_AutoScrollbar(c);
	}

	void _AutoScrollbar(TextBox c)
	{
		var s = c.Text;
		bool ml = s.FindChars("\r\n") >= 0;
		if(ml != (c.ScrollBars == ScrollBars.Vertical)) c.ScrollBars = ml ? ScrollBars.Vertical : 0;
	}

	private void _tFind_ArrowButtonPressed(object sender, EventArgs e)
	{
		var m = new PopupList { IsModal = true, ComboBoxAnimation = true };
		m.Items = new string[] { "one", "two" };
		m.ClosedAction = o => { Print(o.ResultIndex, o.ResultItem, o.ResultWasKey); };
		m.Show(_tFind);
	}

	private void _tFind_ImageButtonClicked(object sender, EventArgs e)
	{
		Print("_tFind_ImageButtonClicked");
	}

	private void _cCase_CheckedChanged(object sender, EventArgs e)
	{
		UpdateEditor();
		//_replacePreview = false;
	}

	private void _cWord_CheckedChanged(object sender, EventArgs e)
	{
		if(_cWord.Checked) _cRegex.Checked = false;
		UpdateEditor();
		//_replacePreview = false;
	}

	private void _cRegex_CheckedChanged(object sender, EventArgs e)
	{
		if(_cRegex.Checked) _cWord.Checked = false;
		UpdateEditor();
		//_replacePreview = false;
	}

	private void _bFind_Click(object sender, EventArgs e)
	{
		if(!_GetTextToFind(out var f, false)) return;
		_FindNextInEditor(f, false);
	}

	private void _bFindIF_Click(object sender, EventArgs e) => _FindNameOrInFiles(false);

	private void _bName_Click(object sender, EventArgs e) => _FindNameOrInFiles(true);

	private void _bReplace_Click(object sender, EventArgs e)
	{
		if(!_GetTextToFind(out var f, true)) return;
		_FindNextInEditor(f, true);
	}

	private void _bOptions_Click(object sender, EventArgs e)
	{
		using var f = new Form_FindOptions();
		f._tSkip.Text = string.Join("\r\n", _SkipWildcards);

		if(f.ShowDialog(this) != DialogResult.OK) return;

		Settings.Set("Find.Skip", f._tSkip.Text);
		_aSkipWildcards = null;
		//_replacePreview = false;
	}

	#endregion

	#region in editor

	void _FindNextInEditor(in _TextToFind f, bool replace)
	{
		var doc = Panels.Editor.ActiveDoc; if(doc == null) return;
		var t = doc.ST;
		var text = t.AllText(); if(text.Length == 0) return;
		int i = -1, len = 0, from8 = replace ? t.SelectionStart : t.SelectionEnd, from = t.CountBytesToChars(0, from8);
		bool retryFromStart = false, retryRx = false;
		g1:
		if(f.rx != null) {
			if(f.rx.MatchG(text, out var g, 0, new RXMore(from))) {
				i = g.Index;
				len = g.Length;
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
			i = f.wholeWord ? text.FindWord(f.findText, from, !f.matchCase, "_") : text.Find(f.findText, from, !f.matchCase);
			len = f.findText.Length;
		}
		//Print(from, i, len);
		if(i < 0) {
			if(retryFromStart || from8 == 0) return;
			from = 0; retryFromStart = true; replace = false; goto g1;
		}
		i = t.CountBytesFromChars(i);
		int to = t.CountBytesFromChars(i, len);
		if(replace && i == from8 && to == t.SelectionEnd) {
			//t.ReplaceRange(i, to, f.replaceText); //also would need to set caret pos = to
			t.ReplaceSel(f.replaceText);
			_FindNextInEditor(f, false);
		} else {
			t.Call(Sci.SCI_SETSEL, i, to);
		}
	}

	private void _bReplaceAll_Click(object sender, EventArgs e)
	{
		if(!_GetTextToFind(out var f, true)) return;
		var doc = Panels.Editor.ActiveDoc; if(doc == null) return;
		var text = doc.Text;
		string r;
		if(f.rx != null) {
			if(0 == f.rx.Replace(text, f.replaceText, out r)) return;
		} else {
			_aEditor.Clear();
			_FindAllInText(text, f, _aEditor);
			if(_aEditor.Count == 0) return;
			var b = new StringBuilder();
			int i = 0;
			foreach(var v in _aEditor) {
				b.Append(text, i, v.x - i).Append(f.replaceText);
				i = v.x + v.y;
			}
			b.Append(text, i, text.Length - i);
			r = b.ToString();
			if(r == text) return;
		}
		doc.ST.SetText(r);
	}

	/// <summary>
	/// Async-updates find-hiliting in editor.
	/// Called when changed find text or options. Also when activated another document.
	/// </summary>
	public void UpdateEditor()
	{
		if(!Visible) return;
		//Print("UpdateEditor", Visible);
		if(_timerUE == null) _timerUE = new ATimer(() => {
			_FindAllInEditor();
			Panels.Editor.ActiveDoc?.HiliteFind(_aEditor);
		});
		_timerUE.Start(100, true);
	}
	ATimer _timerUE;

	List<POINT> _aEditor = new List<POINT>(); //index/length of all found instances in editor text

	void _FindAllInEditor()
	{
		_aEditor.Clear();
		if(!_GetTextToFind(out var f, false)) return;
		var text = Panels.Editor.ActiveDoc?.Text; if(text.Lenn() == 0) return;
		_FindAllInText(text, f, _aEditor);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		Panels.Editor.ActiveDoc?.HiliteFind(Visible ? _aEditor : null);
	}

	public void CtrlF()
	{
		if(!Visible) Panels.PanelManager.GetPanel(this).Visible = true;
		string s = "";
		switch(Control.FromHandle(Api.GetFocus().Handle)) {
		case AuScintilla c:
			s = c.ST.SelectedText();
			break;
		case TextBox c:
			s = c.SelectedText;
			break;
		}
		_tFind.Focus();
		if(s.Length == 0) return;
		_tFind.Text = s;
		_tFind.SelectAll();
	}

	#endregion

	#region common

	void _FindAllInText(string text, in _TextToFind f, List<POINT> a, bool one = false)
	{
		if(f.rx != null) {
			foreach(var g in f.rx.FindAllG(text)) {
				a.Add((g.Index, g.Length));
				if(one) break;
			}
		} else {
			for(int i = 0; i < text.Length; i += f.findText.Length) {
				i = f.wholeWord ? text.FindWord(f.findText, i, !f.matchCase, "_") : text.Find(f.findText, i, !f.matchCase);
				if(i < 0) break;
				a.Add((i, f.findText.Length));
				if(one) break;
			}
		}
	}

	struct _TextToFind
	{
		public string findText;
		public string replaceText;
		public ARegex rx;
		public bool wholeWord;
		public bool matchCase;
	}

	bool _GetTextToFind(out _TextToFind f, bool forReplace)
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
				_errorProvider.SetError(_tFind, e.Message);
				return false;
			}
		} else f.wholeWord = _cWord.Checked;
		if(forReplace) f.replaceText = _tReplace.Text;
		return true;
	}

	#endregion

	#region in files

	void _FindNameOrInFiles(bool names)
	{
		var button = names ? _bName : _bFindIF;
		if(!_GetTextToFind(out var f, false)) return;
		using var _ = new _TempDisableControl(button);
		_FindAllInFiles(f, names);
	}

	List<FileNode> _aFiles;

	string[] _SkipWildcards => _aSkipWildcards ?? (_aSkipWildcards = Settings.GetString("Find.Skip", "").SegSplit("\r\n", SegFlags.NoEmpty));
	string[] _aSkipWildcards;
	string[] _aSkipImages = new string[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".ico", ".cur", ".ani" };

	void _FindAllInFiles(in _TextToFind f, bool names/*, bool forReplace*/)
	{
		if(!names && _aFiles == null) {
			_aFiles = new List<FileNode>();
			Panels.Found.Control.Tags.AddLinkTag("+f", s => {
				var a = s.Split(' ');
				if(!Model.SetCurrentFile(_aFiles[a[0].ToInt()])) return;
				var doc = Panels.Editor.ActiveDoc;
				doc.Focus();
				var t = doc.ST;
				int i = t.CountBytesFromChars(a[1].ToInt()), to = t.CountBytesFromChars(i, a[2].ToInt());
				t.Call(Sci.SCI_SETSEL, i, to);
			});
			Panels.Files.WorkspaceLoaded += () => {
				Panels.Found.Control.ST.ClearText();
				//_replacePreview = false;
				_aFiles?.Clear();
			};
		}
		_aFiles?.Clear();

		var b = new StringBuilder();
		var a = new List<POINT>();
		var bSlow = !names && f.rx != null ? new StringBuilder() : null;
		bool jited = false;

		foreach(var v in Model.Root.Descendants()) {
			string text = null;
			if(names) {
				text = v.Name;
			} else {
				//APerf.First();
				if(!v.IsCodeFile) {
					if(v.IsFolder) continue;
					if(0 != v.Name.Ends(true, _aSkipImages)) continue;
				}
				var sw = _SkipWildcards; if(sw.Length != 0 && 0 != v.ItemPath.Like(true, sw)) continue;
				text = v.GetText();
				if(text.Length == 0) continue;
				if(text.Has('\0')) continue;
				//APerf.NW();
			}

			long time = bSlow != null ? ATime.PerfMilliseconds : 0;

			a.Clear();
			_FindAllInText(text, f, a, names);

			if(a.Count != 0) {
				//if(forReplace && b.Length == 0) {
				//	_replacePreview = true;
				//	b.AppendLine("<Z 0xffc0c0>Preview. Click 'in files' again to replace.<>");
				//}

				if(!names) b.Append("<Z 0xC0E0C0>");
				b.Append("<open>").Append(v.ItemPath).Append("<>");
				if(!names) b.Append("<>");
				b.AppendLine();

				if(!names && b.Length < 10_000_000) {
					for(int i = 0; i < a.Count; i++) {
						var p = a[i];
						int lineStart = p.x, lineEnd = p.x + p.y;
						int lsMax = Math.Max(lineStart - 100, 0), leMax = Math.Min(lineEnd + 200, text.Length); //start/end limits like in VS
						for(; lineStart > lsMax; lineStart--) { char c = text[lineStart - 1]; if(c == '\n' || c == '\r') break; }
						bool limitStart = lineStart == lsMax && lineStart > 0;
						for(; lineEnd < leMax; lineEnd++) { char c = text[lineEnd]; if(c == '\r' || c == '\n') break; }
						bool limitEnd = lineEnd == leMax && lineEnd < text.Length;
						AExtString.More.LineAndColumn(text, p.x, out int lineIndex, out _);
						b.AppendFormat("<+f {0} {1} {2}>", _aFiles.Count.ToString(), p.x.ToString(), p.y.ToString()).Append((lineIndex + 1).ToString("D3")).Append(":<> ")
							.Append(limitStart ? "…<\a>" : "<\a>").Append(text, lineStart, p.x - lineStart).Append("</\a>")
							.Append("<z 0xffffa0><\a>").Append(text, p.x, p.y).Append("</\a><>")
							.Append("<\a>").Append(text, p.x + p.y, lineEnd - p.x - p.y).AppendLine(limitEnd ? "</\a>…" : "</\a>");
					}
					_aFiles.Add(v);
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

		Panels.Found.Control.ST.SetText(b.Append(bSlow).ToString());
	}

	//rejected: replace in files. Rarely used, dangerous, need much work to make more useful. It's easy to click each file in results and click 'Replace' or 'all'.
	//bool _replacePreview;
	//private void _bReplaceIF_Click(object sender, EventArgs e)
	//{
	//	if(!_GetTextToFind(out var f, true)) return;
	//	if(!_replacePreview) {
	//		using var _ = new _TempDisableControl(_bReplaceIF, 500);
	//		_FindAllInFiles(f, false, true);
	//		return;
	//	}
	//	_replacePreview = false;
	//	Panels.Found.Control.ST.ClearText();

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
			else { var w = _w; ATimer.After(_enableAfter, () => w.Enable(true)); }
		}
	}

	#endregion

	private void _bReplace_MouseUp(object sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Right) _bFind_Click(sender, e);
	}
}
