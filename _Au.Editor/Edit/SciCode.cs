//#define TRACE_TEMP_RANGES

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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;
using static Au.Controls.Sci;

partial class SciCode : AuScintilla
{
	readonly SciText.FileLoaderSaver _fls;
	readonly FileNode _fn;

	public FileNode ZFile => _fn;

	//margins. Initially 0-4. We can add more with SCI_SETMARGINS.
	public const int c_marginFold = 0;
	public const int c_marginLineNumbers = 1;
	public const int c_marginMarkers = 2; //breakpoints etc

	//markers. We can use 0-24. Folding 25-31.
	public const int c_markerUnderline = 0, c_markerBookmark = 1, c_markerBreakpoint = 2;
	//public const int c_markerStepNext = 3;

	//indicators. We can use 8-31. Lexers use 0-7. Draws indicators from smaller to bigger, eg error on warning.
	public const int c_indicFind = 8, c_indicDiagHidden = 17, c_indicInfo = 18, c_indicWarning = 19, c_indicError = 20;

	internal SciCode(FileNode file, SciText.FileLoaderSaver fls)
	{
		//_edit = edit;
		_fn = file;
		_fls = fls;

		this.Dock = DockStyle.Fill;
		this.Name = "Code_text";
		this.AccessibleRole = AccessibleRole.Document;
		this.AllowDrop = true;

		ZInitImagesStyle = ZImagesStyle.AnyString;
		if(fls.IsBinary) ZInitReadOnlyAlways = true;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		Call(SCI_SETMODEVENTMASK, (int)(MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT /*| MOD.SC_MOD_INSERTCHECK*/));

		Call(SCI_SETLEXER, (int)LexLanguage.SCLEX_NULL); //default SCLEX_CONTAINER

		Call(SCI_SETMARGINTYPEN, c_marginLineNumbers, SC_MARGIN_NUMBER);
		Z.MarginWidth(c_marginLineNumbers, 40 * Au.Util.ADpi.OfThisProcess / 96);

		_InicatorsInit();

		if(_fn.IsCodeFile) {
			//C# interprets Unicode newline characters NEL, LS and PS as newlines. Visual Studio too.
			//	Scintilla and C++ lexer support it, but by default it is disabled.
			//	If disabled, line numbers in errors/warnings/stacktraces may be incorrect.
			//	Ascii VT and FF are not interpreted as newlines by C# and Scintilla.
			//	Not tested, maybe this must be set for each document in the control.
			//	Scintilla controls without C++ lexer don't support it.
			//		But if we temporarily set C++ lexer for <code>, newlines are displayed in whole text.
			//	Somehow this disables <fold> tag, therefore now not used for output etc.
			Call(SCI_SETLINEENDTYPESALLOWED, 1);

			Call(SCI_SETMOUSEDWELLTIME, 500);

			CiStyling.DocHandleCreated(this);

			//Call(SCI_ASSIGNCMDKEY, 3 << 16 | 'C', SCI_COPY); //Ctrl+Shift+C = raw copy

			//Z.StyleFont(STYLE_CALLTIP, "Calibri");
			//Z.StyleBackColor(STYLE_CALLTIP, 0xf8fff0);
			//Z.StyleForeColor(STYLE_CALLTIP, 0);
			//Call(SCI_CALLTIPUSESTYLE);
		} else {
			Z.StyleFont(STYLE_DEFAULT, "Consolas", 9);
			Z.StyleClearAll();
		}

		Z.StyleForeColor(STYLE_INDENTGUIDE, 0xcccccc);
		Call(SCI_SETINDENTATIONGUIDES, SC_IV_REAL);

		//Call(SCI_SETXCARETPOLICY, CARET_SLOP | CARET_EVEN, 20); //does not work

		//Call(SCI_SETVIEWWS, 1); Call(SCI_SETWHITESPACEFORE, 1, 0xcccccc);
	}

	//Called by PanelEdit.ZOpen.
	internal void _Init(byte[] text, bool newFile)
	{
		if(!IsHandleCreated) CreateHandle();
		bool editable = _fls.SetText(Z, text);
		if(newFile) _openState = _EOpenState.NewFile; else if(Program.Model.OpenFiles.Contains(_fn)) _openState = _EOpenState.Reopen;
		if(_fn.IsCodeFile) CiStyling.DocTextAdded(this, newFile);

		//detect \r without '\n', because it is not well supported
		if(editable) {
			bool badCR = false;
			for(int i = 0, n = text.Length - 1; i <= n; i++) {
				if(text[i] == '\r' && (i == n || text[i + 1] != '\n')) badCR = true;
			}
			if(badCR) {
				AOutput.Write($@"<>Note: text of {_fn.Name} contains single \r (CR) as line end characters. It can create problems. <+badCR s>Show<>, <+badCR h>hide<>, <+badCR f>fix<>.");
				if(!s_badCR) {
					s_badCR = true;
					Panels.Output.ZOutput.ZTags.AddLinkTag("+badCR", s1 => {
						bool fix = s1.Starts('f');
						Panels.Editor.ZActiveDoc?.Call(fix ? SCI_CONVERTEOLS : SCI_SETVIEWEOL, fix || s1.Starts('h') ? 0 : 1); //tested: SCI_CONVERTEOLS ignored if readonly
					});
				}
			}
		}
	}
	static bool s_badCR;

	//protected override void Dispose(bool disposing)
	//{
	//	AOutput.QM2.Write($"Dispose disposing={disposing} IsHandleCreated={IsHandleCreated} Visible={Visible}");
	//	base.Dispose(disposing);
	//}

	protected unsafe override void ZOnSciNotify(ref SCNotification n)
	{
		//switch(n.nmhdr.code) {
		//case NOTIF.SCN_PAINTED:
		////case NOTIF.SCN_UPDATEUI:
		//case NOTIF.SCN_FOCUSIN:
		//case NOTIF.SCN_FOCUSOUT:
		//case NOTIF.SCN_DWELLSTART:
		//case NOTIF.SCN_DWELLEND:
		//case NOTIF.SCN_NEEDSHOWN:
		//	break;
		//case NOTIF.SCN_MODIFIED:
		//	AOutput.Write(n.nmhdr.code, n.modificationType);
		//	break;
		//default:
		//	AOutput.Write(n.nmhdr.code);
		//	break;
		//}

		switch(n.nmhdr.code) {
		case NOTIF.SCN_SAVEPOINTLEFT:
			Program.Model.Save.TextLater();
			break;
		case NOTIF.SCN_SAVEPOINTREACHED:
			//never mind: we should cancel the 'save text later'
			break;
		case NOTIF.SCN_MODIFIED:
			//AOutput.Write("SCN_MODIFIED", n.modificationType, n.position, n.FinalPosition, Z.CurrentPos8, n.Text);
			//AOutput.Write(n.modificationType);
			//if(n.modificationType.Has(MOD.SC_PERFORMED_USER | MOD.SC_MOD_BEFOREINSERT)) {
			//	AOutput.Write($"'{n.Text}'");
			//	if(n.length == 2 && n.textUTF8!=null && n.textUTF8[0]=='\r' && n.textUTF8[1] == '\n') {
			//		Call(SCI_BEGINUNDOACTION); Call(SCI_ENDUNDOACTION);
			//	}
			//}
			if(n.modificationType.HasAny(MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT)) {
				_modified = true;
				_TempRangeOnModifiedOrPosChanged(n.modificationType, n.position, n.length);
				CodeInfo.SciModified(this, n);
				Panels.Find.ZUpdateQuickResults(true);
				//} else if(n.modificationType.Has(MOD.SC_MOD_INSERTCHECK)) {
				//	//AOutput.Write(n.Text);
				//	//if(n.length==1 && n.textUTF8[0] == ')') {
				//	//	Call(Sci.SCI_SETOVERTYPE, _testOvertype = true);

				//	//}
			}
			break;
		case NOTIF.SCN_CHARADDED:
			//AOutput.Write($"SCN_CHARADDED  {n.ch}  '{(char)n.ch}'");
			if(n.ch == '\n' /*|| n.ch == ';'*/) { //split scintilla Undo
				Z.AddUndoPoint();
			}
			if(n.ch != '\r' && n.ch <= 0xffff) { //on Enter we receive notifications for '\r' and '\n'
				CodeInfo.SciCharAdded(this, (char)n.ch);
			}
			break;
		case NOTIF.SCN_UPDATEUI:
			//AOutput.Write((uint)n.updated, _modified);
			if(0 != (n.updated & 1)) {
				if(_modified) _modified = false; else n.updated &= ~1; //ignore notifications when changed styling or markers
			}
			if(0 == (n.updated & 15)) break;
			if(0 != (n.updated & 3)) { //text (1), selection/click (2)
				_TempRangeOnModifiedOrPosChanged(0, 0, 0);
				Panels.Editor._UpdateUI_EditEnabled();
			}
			CodeInfo.SciUpdateUI(this, n.updated);
			break;
		case NOTIF.SCN_DWELLSTART:
			CodeInfo.SciMouseDwellStarted(this, n.position);
			break;
		case NOTIF.SCN_DWELLEND:
			CodeInfo.SciMouseDwellEnded(this);
			break;
		case NOTIF.SCN_MARGINCLICK:
			if(_fn.IsCodeFile) {
				CodeInfo.Cancel();
				if(n.margin == c_marginFold) {
					_FoldOnMarginClick(null, n.position);
				}

				//SHOULDDO: when clicked selbar to select a fold header line, should select all hidden lines. Like in VS.
			}
			break;
		}

		base.ZOnSciNotify(ref n);
	}
	bool _modified;

	protected override void WndProc(ref Message m)
	{
		//if(m.Msg== Api.WM_PAINT) {
		//	var p1 = APerf.Create();
		//	base.WndProc(ref m);
		//	p1.NW('P');
		//	return;
		//}

		//var w = (AWnd)m.HWnd;
		//AOutput.Write(m);
		switch(m.Msg) {
		case Api.WM_SETFOCUS:
			if(!_noModelEnsureCurrentSelected) Program.Model.EnsureCurrentSelected();
			break;
		case Api.WM_CHAR: {
			int c = (int)m.WParam;
			if(c < 32) {
				if(!(c == 9 || c == 10 || c == 13)) return;
			} else {
				if(CodeInfo.SciBeforeCharAdded(this, (char)c)) return;
			}
		}
		break;
		case Api.WM_KEYDOWN:
			if((KKey)m.WParam == KKey.Insert) return;
			break;
		case Api.WM_MBUTTONDOWN:
			this.Focus();
			return;
		case Api.WM_RBUTTONDOWN: {
			//workaround for Scintilla bug: when right-clicked a margin, if caret or selection start is at that line, goes to the start of line
			POINT p = (AMath.LoShort(m.LParam), AMath.HiShort(m.LParam));
			int margin = Z.MarginFromPoint(p, false);
			if(margin >= 0) {
				var selStart = Z.SelectionStart8;
				var (_, start, end) = Z.LineStartEndFromPos(false, Z.PosFromXY(false, p, false));
				if(selStart >= start && selStart <= end) return;
				//do vice versa if the end of non-empty selection is at the start of the right-clicked line, to avoid comment/uncomment wrong lines
				if(margin == c_marginLineNumbers || margin == c_marginMarkers) {
					if(Z.SelectionEnd8 == start) Z.GoToPos(false, start); //clear selection above start
				}
			}
		}
		break;
		case Api.WM_CONTEXTMENU: {
			bool kbd = (int)m.LParam == -1;
			int margin = kbd ? -1 : Z.MarginFromPoint((AMath.LoShort(m.LParam), AMath.HiShort(m.LParam)), true);
			switch(margin) {
			case -1:
				Strips.ddEdit.ZShowAsContextMenu(kbd);
				break;
			case c_marginLineNumbers:
			case c_marginMarkers:
				ZCommentLines(null);
				break;
				//case c_marginFold:
				//	break;
			}
			return;
		}
		}

		base.WndProc(ref m);

		switch(m.Msg) {
		//case Api.WM_MOUSEMOVE:
		//	CodeInfo.SciMouseMoved(this, AMath.LoShort(m.LParam), AMath.HiShort(m.LParam));
		//	break;
		case Api.WM_KILLFOCUS:
			CodeInfo.SciKillFocus(this);
			break;
		case Api.WM_LBUTTONUP:
			if(ModifierKeys == Keys.Control) CiGoTo.GoToSymbolFromPos(onCtrlClick: true);
			break;
		}
	}
	bool _noModelEnsureCurrentSelected;

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch(keyData) {
		case Keys.Control | Keys.C:
			ZForumCopy(onlyInfo: true);
			break;
		case Keys.Control | Keys.V:
			if(ZForumPaste()) return true;
			break;
		case Keys.Control | Keys.W:
			Strips.Cmd.Edit_WrapLines();
			return true;
		case Keys.F12:
			Strips.Cmd.Edit_GoToDefinition();
			return true;
		default:
			if(CodeInfo.SciCmdKey(this, keyData)) return true;
			switch(keyData) {
			case Keys.Enter:
				Z.AddUndoPoint();
				break;
			}
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	bool _IsUnsaved {
		get => _isUnsaved || 0 != Call(SCI_GETMODIFY);
		set {
			if(_isUnsaved = value) Program.Model.Save.TextLater(1);
		}
	}
	bool _isUnsaved;

	//Called by PanelEdit.ZSaveText.
	internal bool _SaveText()
	{
		if(_IsUnsaved) {
			//AOutput.QM2.Write("saving");
			_fn.UnCacheText();
			if(!Program.Model.TryFileOperation(() => _fls.Save(Z, _fn.FilePath, tempDirectory: _fn.IsLink ? null : _fn.Model.TempDirectory))) return false;
			//info: with tempDirectory less noise for FileSystemWatcher
			_isUnsaved = false;
			Call(SCI_SETSAVEPOINT);
		}
		return true;
	}

	//Called by FileNode.UnCacheText.
	internal void _FileModifiedExternally()
	{
		if(Z.IsReadonly) return;
		var text = _fn.GetText(saved: true); if(text == this.Text) return;
		ZReplaceTextGently(text);
		Call(SCI_SETSAVEPOINT);
		if(this == Panels.Editor.ZActiveDoc) AOutput.Write($"<>Info: file {_fn.Name} has been modified outside and therefore reloaded. You can Undo.");
	}

	#region drag drop

	enum _DD_DataType { None, Text, Files, Shell, Link, Script };
	_DD_DataType _drag;

	protected override void OnDragEnter(DragEventArgs e)
	{
		var d = e.Data;
		//foreach(var v in d.GetFormats()) AOutput.Write(v, d.GetData(v, false)?.GetType()); AOutput.Write("--");
		_drag = 0;
		if(d.GetDataPresent("Aga.Controls.Tree.TreeNodeAdv[]", false)) _drag = _DD_DataType.Script;
		else if(d.GetDataPresent("FileDrop", false)) _drag = _DD_DataType.Files;
		else if(d.GetDataPresent("Shell IDList Array", false)) _drag = _DD_DataType.Shell;
		else if(d.GetDataPresent("UnicodeText", false))
			_drag = d.GetDataPresent("FileGroupDescriptorW", false) ? _DD_DataType.Link : _DD_DataType.Text;
		e.Effect = _DD_GetEffect(e);
		CodeInfo.Cancel();
		base.OnDragEnter(e);
	}

	protected override unsafe void OnDragOver(DragEventArgs e)
	{
		if((e.Effect = _DD_GetEffect(e)) != 0) {
			var p = _DD_GetDropPos(e, out _);
			var z = new Sci_DragDropData { x = p.X, y = p.Y };
			Call(SCI_DRAGDROP, 1, &z);
			//FUTURE: upgrade Scintilla. Version 4.3.1 supports auto-scroll when dragging.
		}
		base.OnDragOver(e);
	}

	protected override void OnDragDrop(DragEventArgs e)
	{
		if((e.Effect = _DD_GetEffect(e)) != 0) _DD_Drop(e);
		_drag = 0;
		base.OnDragDrop(e);
	}

	protected override void OnDragLeave(EventArgs e)
	{
		if(_drag != 0) {
			_drag = 0;
			Call(SCI_DRAGDROP, 3);
		}
		base.OnDragLeave(e);
	}

	Point _DD_GetDropPos(DragEventArgs e, out int pos)
	{
		var p = this.PointToClient(new Point(e.X, e.Y));
		if(_drag != _DD_DataType.Text) { //if files etc, drop as lines, not anywhere
			pos = Call(SCI_POSITIONFROMPOINT, p.X, p.Y);
			pos = Z.LineStartFromPos(false, pos);
			p.X = Call(SCI_POINTXFROMPOSITION, 0, pos);
			p.Y = Call(SCI_POINTYFROMPOSITION, 0, pos);
		} else pos = 0;
		return p;
	}

	unsafe void _DD_Drop(DragEventArgs e)
	{
		var xy = _DD_GetDropPos(e, out int pos8);
		string s = null;
		var b = new StringBuilder();
		int what = 0;

		if(_drag != _DD_DataType.Text) {
			if(_fn.IsCodeFile) {
				var text = this.Text;
				int endOfMeta = Au.Compiler.MetaComments.FindMetaComments(text);
				if(endOfMeta > 0 && Pos16(pos8) < endOfMeta) return;

				var m = new AMenu { Modal = true };
				if(_drag == _DD_DataType.Script) {
					m["var s = name;"] = o => what = 1;
					m["var s = path;"] = o => what = 2;
					m["ATask.Run(path);"] = o => what = 3;
					m["t[name] = o => ATask.Run(path);"] = o => what = 4;
				} else {
					m["var s = path;"] = o => what = 11;
					m["AFile.Run(path);"] = o => what = 12;
					m["t[name] = o => AFile.Run(path);"] = o => what = 13;
					//FUTURE: also add same items with unexpanded path.
				}
				m.Show(this);
				if(what == 0) return;
			}
		}

		var d = e.Data;
		switch(_drag) {
		case _DD_DataType.Text:
			s = d.GetData("UnicodeText", false) as string;
			break;
		case _DD_DataType.Files:
			if(d.GetData("FileDrop", false) is string[] paths) {
				foreach(var path in paths) {
					bool isLnk = path.Ends(".lnk", true);
					if(isLnk) b.Append("//");
					var name = APath.GetNameNoExt(path);
					_AppendFile(path, name);
					if(isLnk) {
						try {
							var g = AShortcutFile.Open(path);
							string target = g.TargetAnyType, args = null;
							if(target.Starts("::")) {
								using var pidl = APidl.FromString(target);
								name = pidl.ToShellString(Native.SIGDN.NORMALDISPLAY);
							} else {
								args = g.Arguments;
								if(!target.Ends(".exe", true) || name.Find("Shortcut") >= 0)
									name = APath.GetNameNoExt(target);
							}
							_AppendFile(target, name, args);
						}
						catch(AuException) { break; }
					}
				}
				s = b.ToString();
			}
			break;
		case _DD_DataType.Shell:
			_DD_GetShell(d, out var shells, out var names);
			if(shells != null) {
				for(int i = 0; i < shells.Length; i++) {
					_AppendFile(shells[i], names[i]);
				}
				s = b.ToString();
			}
			break;
		case _DD_DataType.Link:
			_DD_GetLink(d, out s, out var s2);
			if(s != null) {
				_AppendFile(s, s2);
				s = b.ToString();
			}
			break;
		case _DD_DataType.Script:
			if(d.GetData("Aga.Controls.Tree.TreeNodeAdv[]", false) is Aga.Controls.Tree.TreeNodeAdv[] nodes) {
				foreach(var tn in nodes) {
					var fn = tn.Tag as FileNode;
					_AppendFile(fn.ItemPath, fn.Name, null, fn);
				}
				s = b.ToString();
			}
			break;
		}

		if(!s.NE()) {
			var z = new Sci_DragDropData { x = xy.X, y = xy.Y };
			var s8 = Au.Util.AConvert.ToUtf8(s);
			fixed(byte* p8 = s8) {
				z.text = p8;
				z.len = s8.Length - 1;
				if(_drag != _DD_DataType.Text || 0 == (e.Effect & DragDropEffects.Move)) z.copy = 1;
				Call(SCI_DRAGDROP, 2, &z);
			}
			if(!Focused && FindForm().Hwnd().IsActive) { //note: don't activate window; let the drag source do it, eg Explorer activates on drag-enter.
				_noModelEnsureCurrentSelected = true; //don't scroll treeview to currentfile
				Focus();
				_noModelEnsureCurrentSelected = false;
			}
		} else {
			Call(SCI_DRAGDROP, 3);
		}

		void _AppendFile(string path, string name, string args = null, FileNode fn = null)
		{
			b.Append('\t', Z.LineIndentationFromPos(false, pos8));
			if(what == 0) {
				b.Append(path);
			} else {
				name = name.Escape();
				switch(what) {
				case 1: case 2: case 11: b.Append("var s = "); break;
				case 4: case 13: b.AppendFormat("t[\"{0}\"] = o => ", what == 4 ? name.RemoveSuffix(".cs") : name); break;
				}
				if(what == 12 || what == 13) b.Append("AFile.Run(");
				if((what == 11 || what == 12) && path.Starts(":: ")) b.AppendFormat("/* {0} */ ", name);
				switch(what) {
				case 1: b.AppendFormat("\"{0}\";", name); break;
				case 2: case 11: b.AppendFormat("@\"{0}\";", path); break;
				case 3: case 4: b.AppendFormat("ATask.Run(@\"{0}\");", path); break;
				case 12:
				case 13:
					b.AppendFormat("@\"{0}", path);
					if(!args.NE()) b.Append("\", \"").Append(args.Escape());
					b.Append("\");");
					break;
				}
			}
			b.AppendLine();
		}
	}

	DragDropEffects _DD_GetEffect(DragEventArgs e)
	{
		if(_drag == 0) return 0;
		if(Z.IsReadonly) return 0;
		var ae = e.AllowedEffect;
		DragDropEffects r;
		switch(e.KeyState & (4 | 8 | 32)) { case 0: r = DragDropEffects.Move; break; case 8: r = DragDropEffects.Copy; break; default: return 0; }
		if(_drag == _DD_DataType.Text) return 0 != (ae & r) ? r : ae;
		if(0 != (ae & DragDropEffects.Link)) r = DragDropEffects.Link;
		else if(0 != (ae & DragDropEffects.Copy)) r = DragDropEffects.Copy;
		else r = ae;
		return r;
	}

	static unsafe void _DD_GetShell(IDataObject d, out string[] shells, out string[] names)
	{
		shells = names = null;
		var b = _DD_GetByteArray(d, "Shell IDList Array"); if(b == null) return;
		fixed(byte* p = b) {
			int* pi = (int*)p;
			int n = *pi++; if(n < 1) return;
			shells = new string[n]; names = new string[n];
			IntPtr pidlFolder = (IntPtr)(p + *pi++);
			for(int i = 0; i < n; i++) {
				using(var pidl = new APidl(pidlFolder, (IntPtr)(p + pi[i]))) {
					shells[i] = pidl.ToString();
					names[i] = pidl.ToShellString(Native.SIGDN.NORMALDISPLAY);
				}
			}
		}
	}

	static unsafe void _DD_GetLink(IDataObject d, out string url, out string text)
	{
		url = text = null;
		var b = _DD_GetByteArray(d, "FileGroupDescriptorW"); if(b == null) return;
		fixed(byte* p = b) { //FILEGROUPDESCRIPTORW
			if(*(int*)p != 1) return; //count of FILEDESCRIPTORW
			var s = new string((char*)(p + 76));
			if(!s.Ends(".url", true)) return;
			url = d.GetData("UnicodeText", false) as string;
			if(url != null) text = s.RemoveSuffix(4);
		}
	}

	static byte[] _DD_GetByteArray(IDataObject d, string format)
	{
		switch(d.GetData(format, false)) {
		case byte[] b: return b; //when d is created from data transferred from non-admin process to this admin process by UacDragDrop
		case MemoryStream m: return m.ToArray(); //original .NET DataObject. Probably this process is non-admin.
		}
		return null;
	}

	#endregion

	#region copy paste

	public void ZForumCopy(bool onlyInfo = false)
	{
		int i1 = Z.SelectionStart8, i2 = Z.SelectionEnd8, textLen = Len8;
		if(textLen == 0) return;
		bool isCS = _fn.IsCodeFile;
		bool isFragment = (i2 != i1 && !(i1 == 0 && i2 == textLen)) || !isCS;
		if(onlyInfo) {
			if(isFragment || s_infoCopy) return; s_infoCopy = true;
			AOutput.Write("Info: To copy C# code for pasting in the forum, use menu Edit -> Forum Copy. Then simply paste there; don't use the Code button.");
			return;
		}

		bool isScript = _fn.IsScript;
		var b = new StringBuilder(isCS ? "[cs]" : "[code]");
		string s;
		if(isFragment) {
			b.Append(Z.RangeText(false, i1, i2));
		} else {
			s = CiUtil.GetTextWithoutUnusedUsingDirectives();
			var name = _fn.Name; if(name.RegexIsMatch(@"(?i)^(Script|Class)\d*\.cs")) name = null;
			b.AppendFormat("// {0} \"{1}\"{2}{3}", isScript ? "script" : "class", name, s[0] == '/' ? " " : "\r\n", s);
		}
		b.AppendLine(isCS ? "[/cs]" : "[/code]");
		s = b.ToString();
		new AClipboardData().AddText(s).SetClipboard();
	}
	static bool s_infoCopy;

	public bool ZForumPaste()
	{
		var s = AClipboard.Text;
		if(s == null) return false;
		if(s.Like("[cs]*[/cs]\r\n")) s = s[4..^7];

		if(!s.RegexMatch(@"^// (script|class) ""(.*?)""( |\R)", out var m)) return false;
		bool isClass = s[3] == 'c';
		s = s[m.End..];
		var name = m[2].Length > 0 ? m[2].Value : (isClass ? "Class1.cs" : "Script1.cs");

		string buttons = _fn.FileType != (isClass ? EFileType.Class : EFileType.Script)
			? "1 Create new file|0 Cancel"
			: "1 Create new file|2 Replace all text|3 Paste|0 Cancel";
		switch(ADialog.Show("Import C# file text from clipboard", "Source file: " + name, buttons, DFlags.CommandLinks, owner: this)) {
		case 1: //Create new file
			Program.Model.NewItem(isClass ? "Class.cs" : "Script.cs", null, name, text: new EdNewFileText(replaceTemplate: true, s));
			break;
		case 2: //Replace all text
			Z.SetText(s);
			break;
		case 3: //Paste
			Z.ReplaceSel(s);
			break;
		} //rejected: option to rename this file

		return true;
	}

	#endregion

	#region script header

	//const string c_usings = "using Au; using Au.Types; using System; using System.Collections.Generic;";
	//const string c_scriptMain = "class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;";

	//static ARegex _RxScriptHeader => s_rxScript ??= new ARegex(@"(?sm)//\.(.*?)\R\Q" + c_usings + @"\E$(.*?)\R\Q[\w ]*" + c_scriptMain + @"\E$");
	//static ARegex s_rxScript;

	//currently not used and does not work
	///// <summary>
	///// Finds script header "//. ... //;;;\r\n" using regular expression.
	///// </summary>
	///// <param name="s">Script text.</param>
	///// <param name="m">
	///// Group 1 is "" or text between //. and c_usings. Includes the starting newline but not the ending newline.
	///// Group 2 is "" or text between c_usings and c_scriptMain. Includes the starting newline but not the ending newline.
	///// </param>
	//public static bool ZFindScriptHeader(string s, out RXMatch m) => _RxScriptHeader.Match(s, out m);

	/// <summary>
	/// Finds script header "//.\r\nusing Au; ... //;;;\r\n".
	/// The results are UTF-8.
	/// Does not get whole text; instead uses SCI_FINDTEXT.
	/// Returns false if not script or not found.
	/// </summary>
	public bool ZFindScriptHeader8(out (int start, int end, int startLine, int endLine) found)
	{
		//never mind: can be text between "//.\r\n" and "using Au;". Could use regex, but then need to get UTF-16 text; better avoid it.
		found = default;
		if(!_fn.IsScript) return false;
		const string s1 = "//.\r\nusing Au;", s2 = "//;;;\r\n";
		int start = Z.FindText(false, s1); if(start < 0) return false;
		int end = Z.FindText(false, s2, start); if(end < 0) return false;
		end += s2.Length;
		found = (start, end, Z.LineFromPos(false, start), Z.LineFromPos(false, end));
		return true;
	}

	/// <summary>
	/// Folds script header "//.\r\nusing Au; ... //;;;\r\n" if found.
	/// Does not get whole text; instead uses SCI_FINDTEXT.
	/// </summary>
	/// <param name="setCaret">Set caret position below header.</param>
	public unsafe void ZFoldScriptHeader(bool setCaret = false)
	{
		if(!ZFindScriptHeader8(out var k)) return;
		var a = stackalloc int[2] { k.start, (k.end - 2) | unchecked((int)0x80000000) };
		Sci_SetFoldLevels(SciPtr, 0, k.endLine, 2, a);
		Call(SCI_FOLDCHILDREN, k.startLine);

		if(setCaret) {
			int i = k.end;
			if((char)Call(SCI_GETCHARAT, i + 1) == '\n') i += 2;
			Z.CurrentPos16 = i;
		}
	}

	//bool _IsScriptHeaderFolded()
	//{
	//	if(!ZFile.IsScript) return false;
	//	string s = Text;
	//	if(!_RxScriptHeader.Match(s, out var m)) return false;
	//	int line = Z.LineIndexFromPos(m.Start, utf16: true);
	//	return 0 == Call(SCI_GETFOLDEXPANDED, line);
	//}

	#endregion

	#region indicators

	void _InicatorsInit()
	{
		Call(SCI_INDICSETSTYLE, c_indicFind, INDIC_FULLBOX);
		//Call(SCI_INDICSETFORE, c_indicFind, 0x00a0f0); Call(SCI_INDICSETALPHA, c_indicFind, 160); //orange-brown, almost like in VS
		Call(SCI_INDICSETFORE, c_indicFind, 0x00ffff); Call(SCI_INDICSETALPHA, c_indicFind, 160); //yellow
		Call(SCI_INDICSETUNDER, c_indicFind, 1); //draw before text

		Call(SCI_INDICSETSTYLE, c_indicError, INDIC_SQUIGGLE); //INDIC_SQUIGGLEPIXMAP thicker
		Call(SCI_INDICSETFORE, c_indicError, 0xff); //red
		Call(SCI_INDICSETSTYLE, c_indicWarning, INDIC_SQUIGGLE);
		Call(SCI_INDICSETFORE, c_indicWarning, 0x008000); //dark green
		Call(SCI_INDICSETSTYLE, c_indicInfo, INDIC_DIAGONAL);
		Call(SCI_INDICSETFORE, c_indicInfo, 0xc0c0c0);
		Call(SCI_INDICSETSTYLE, c_indicDiagHidden, INDIC_DOTS);
		Call(SCI_INDICSETFORE, c_indicDiagHidden, 0xc0c0c0);
	}

	bool _indicHaveFind, _indicHaveDiag;

	internal void InicatorsFind_(List<Range> a)
	{
		if(_indicHaveFind) {
			_indicHaveFind = false;
			Z.IndicatorClear(c_indicFind);
		}
		if(a == null || a.Count == 0) return;
		_indicHaveFind = true;

		foreach(var v in a) Z.IndicatorAdd(true, c_indicFind, v);
	}

	internal void InicatorsDiag_(bool has)
	{
		if(_indicHaveDiag) {
			_indicHaveDiag = false;
			Z.IndicatorClear(c_indicDiagHidden);
			Z.IndicatorClear(c_indicInfo);
			Z.IndicatorClear(c_indicWarning);
			Z.IndicatorClear(c_indicError);
		}
		if(!has) return;
		_indicHaveDiag = true;
	}

	#endregion

	#region view

	[Flags]
	public enum EView { Wrap = 1, Images = 2 }

	public void ZToggleView(EView what)
	{
		if(what.Has(EView.Wrap)) {
			bool on = !Program.Settings.edit_wrap;
			Program.Settings.edit_wrap = on;
			Call(SCI_SETWRAPMODE, on);
		}
		if(what.Has(EView.Images)) {
			bool on = Program.Settings.edit_noImages;
			Program.Settings.edit_noImages = !on;
			ZImages.Visible = on ? AnnotationsVisible.ANNOTATION_STANDARD : AnnotationsVisible.ANNOTATION_HIDDEN;
		}
		Panels.Editor._UpdateUI_EditView();
	}

	#endregion

	#region temp ranges

	[Flags]
	public enum ZTempRangeFlags
	{
		/// <summary>
		/// Call onLeave etc when current position != current end of range.
		/// </summary>
		LeaveIfPosNotAtEndOfRange = 1,

		/// <summary>
		/// Call onLeave etc when range text modified.
		/// </summary>
		LeaveIfRangeTextModified = 2,

		/// <summary>
		/// Don't add new range if already exists a range with same current from, to, owner and flags. Then returns that range.
		/// </summary>
		NoDuplicate = 4,
	}

	public interface ITempRange
	{
		/// <summary>
		/// Removes this range from the collection of ranges of the document.
		/// Optional. Temp ranges are automatically removed sooner or later.
		/// Does nothing if already removed.
		/// </summary>
		void Remove();

		/// <summary>
		/// Gets current start and end positions of this range added with <see cref="ZTempRanges_Add"/>.
		/// Returns false if the range is removed; then sets from = to = -1.
		/// </summary>
		bool GetCurrentFromTo(out int from, out int to, bool utf8 = false);

		/// <summary>
		/// Gets current start position of this range added with <see cref="ZTempRanges_Add"/>. UTF-16.
		/// Returns -1 if the range is removed.
		/// </summary>
		int CurrentFrom { get; }

		/// <summary>
		/// Gets current end position of this range added with <see cref="ZTempRanges_Add"/>. UTF-16.
		/// Returns -1 if the range is removed.
		/// </summary>
		int CurrentTo { get; }

		object Owner { get; }

		/// <summary>
		/// Any data. Not used by temp range functions.
		/// </summary>
		object OwnerData { get; set; }
	}

	class _TempRange : ITempRange
	{
		SciCode _doc;
		readonly object _owner;
		readonly int _fromUtf16;
		internal readonly int from;
		internal int to;
		internal readonly Action onLeave;
		readonly ZTempRangeFlags _flags;

		internal _TempRange(SciCode doc, object owner, int fromUtf16, int fromUtf8, int toUtf8, Action onLeave, ZTempRangeFlags flags)
		{
			_doc = doc;
			_owner = owner;
			_fromUtf16 = fromUtf16;
			from = fromUtf8;
			to = toUtf8;
			this.onLeave = onLeave;
			_flags = flags;
		}

		public void Remove()
		{
			_TraceTempRange("remove", _owner);
			if(_doc != null) {
				_doc._tempRanges.Remove(this);
				_doc = null;
			}
		}

		internal void Leaved() => _doc = null;

		public bool GetCurrentFromTo(out int from, out int to, bool utf8 = false)
		{
			if(_doc == null) { from = to = -1; return false; }
			if(utf8) {
				from = this.from;
				to = this.to;
			} else {
				from = _fromUtf16;
				to = CurrentTo;
			}
			return true;
		}

		public int CurrentFrom => _doc != null ? _fromUtf16 : -1;

		public int CurrentTo => _doc?.Pos16(to) ?? -1;

		public object Owner => _owner;

		public object OwnerData { get; set; }

		internal bool MustLeave(int pos, int pos2, int modLen)
		{
			return pos < from || pos2 > to
				|| (0 != (_flags & ZTempRangeFlags.LeaveIfPosNotAtEndOfRange) && pos2 != to)
				|| (0 != (_flags & ZTempRangeFlags.LeaveIfRangeTextModified) && modLen != 0);
		}

		internal bool Contains(int pos, object owner, bool endPosition)
			=> (endPosition ? (pos == to) : (pos >= from || pos <= to)) && (owner == null || ReferenceEquals(owner, _owner));

		internal bool Equals(int from2, int to2, object owner2, ZTempRangeFlags flags2)
		{
			if(from2 != from || to2 != to || flags2 != _flags
				//|| onLeave2 != onLeave //delegate always different if captured variables
				//|| !ReferenceEquals(onLeave2?.Method, onLeave2?.Method) //can be used but slow. Also tested Target, always different.
				) return false;
			return ReferenceEquals(owner2, _owner);
		}

		public override string ToString() => $"({CurrentFrom}, {CurrentTo}), owner={_owner}";
	}

	List<_TempRange> _tempRanges = new List<_TempRange>();

	/// <summary>
	/// Marks a temporary working range of text and later notifies when it is leaved.
	/// Will automatically update range bounds when editing text inside it.
	/// Supports many ranges, possibly overlapping.
	/// The returned object can be used to get range info or remove it.
	/// Used mostly for code info, eg to cancel the completion list or signature help.
	/// </summary>
	/// <param name="owner">Owner of the range. See also <see cref="ITempRange.OwnerData"/>.</param>
	/// <param name="from">Start of range, UTF-16.</param>
	/// <param name="to">End of range, UTF-16. Can be = from.</param>
	/// <param name="onLeave">
	/// Called when current position changed and is outside this range (before from or after to) or text modified outside it. Then also forgets the range.
	/// Called after removing the range.
	/// If leaved several ranges, called in LIFO order.
	/// Can be null.
	/// </param>
	/// <param name="flags"></param>
	public ITempRange ZTempRanges_Add(object owner, int from, int to, Action onLeave = null, ZTempRangeFlags flags = 0)
	{
		int fromUtf16 = from;
		Z.NormalizeRange(true, ref from, ref to);
		Debug.Assert(Z.CurrentPos8 >= from && (flags.Has(ZTempRangeFlags.LeaveIfPosNotAtEndOfRange) ? Z.CurrentPos8 == to : Z.CurrentPos8 <= to));

		if(flags.Has(ZTempRangeFlags.NoDuplicate)) {
			for(int i = _tempRanges.Count - 1; i >= 0; i--) {
				var t = _tempRanges[i];
				if(t.Equals(from, to, owner, flags)) return t;
			}
		}

		_TraceTempRange("ADD", owner);
		var r = new _TempRange(this, owner, fromUtf16, from, to, onLeave, flags);
		_tempRanges.Add(r);
		return r;
	}

	/// <summary>
	/// Gets ranges containing the specified position and optionally of the specified owner, in LIFO order.
	/// It's safe to remove the retrieved ranges while enumerating.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="owner">If not null, returns only ranges where ReferenceEquals(owner, range.owner).</param>
	/// <param name="endPosition">position must be at the end of the range.</param>
	/// <param name="utf8"></param>
	public IEnumerable<ITempRange> ZTempRanges_Enum(int position, object owner = null, bool endPosition = false, bool utf8 = false)
	{
		if(!utf8) position = Pos8(position);
		for(int i = _tempRanges.Count - 1; i >= 0; i--) {
			var r = _tempRanges[i];
			if(r.Contains(position, owner, endPosition)) yield return r;
		}
	}

	/// <summary>
	/// Gets ranges of the specified owner, in LIFO order.
	/// It's safe to remove the retrieved ranges while enumerating.
	/// </summary>
	/// <param name="owner">Returns only ranges where ReferenceEquals(owner, range.owner).</param>
	public IEnumerable<ITempRange> ZTempRanges_Enum(object owner)
	{
		for(int i = _tempRanges.Count - 1; i >= 0; i--) {
			var r = _tempRanges[i];
			if(ReferenceEquals(owner, r.Owner)) yield return r;
		}
	}

	void _TempRangeOnModifiedOrPosChanged(MOD mod, int pos, int len)
	{
		if(_tempRanges.Count == 0) return;
		if(mod == 0) pos = Z.CurrentPos8;
		int pos2 = pos;
		if(mod.Has(MOD.SC_MOD_DELETETEXT)) { pos2 += len; len = -len; }
		for(int i = _tempRanges.Count - 1; i >= 0; i--) {
			var r = _tempRanges[i];
			if(r.MustLeave(pos, pos2, len)) {
				_TraceTempRange("leave", r.Owner);
				_tempRanges.RemoveAt(i);
				r.Leaved();
				r.onLeave?.Invoke();
			} else {
				r.to += len;
				Debug.Assert(r.to >= r.from);
			}
		}
	}

	[Conditional("TRACE_TEMP_RANGES")]
	static void _TraceTempRange(string action, object owner) => AOutput.Write(action, owner);

	#endregion
}
