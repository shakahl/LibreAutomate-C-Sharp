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
using static Au.NoClass;
using static Program;
using Au.Controls;
using static Au.Controls.Sci;

partial class PanelEdit : Control
{
	List<SciCode> _docs = new List<SciCode>(); //documents that are actually open currently. Note: FilesModel.OpenFiles contains not only these.
	SciCode _activeDoc;

	public AuScintilla ActiveDoc => _activeDoc;

	public bool IsOpen => _activeDoc != null;

	public PanelEdit()
	{
		this.Name = "Code";
		this.BackColor = SystemColors.AppWorkspace;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		_UpdateUI_IsOpen();
	}

	protected override void OnGotFocus(EventArgs e) { _activeDoc?.Focus(); }

	//public SciControl SC => _activeDoc;

	/// <summary>
	///	If f is already open, unhides its control.
	///	Else loads f text and creates control. If fails, does not change anything.
	/// Hides current file's control.
	/// Returns false if failed to read file.
	/// Does not save text of previously active document.
	/// </summary>
	/// <param name="f"></param>
	public bool Open(FileNode f)
	{
		Debug.Assert(MainForm.IsHandleCreated);
		Debug.Assert(!Model.IsAlien(f));

		if(f == _activeDoc?.FN) return true;
		bool focus = _activeDoc != null ? _activeDoc.Focused : false;
		var doc = _docs.Find(v => v.FN == f);
		if(doc != null) {
			if(_activeDoc != null) _activeDoc.Visible = false;
			_activeDoc = doc;
			_activeDoc.Visible = true;
			_UpdateUI_Cmd();
		} else {
			byte[] text = null;
			SciText.FileLoaderSaver fls = default;
			try { text = fls.Load(f.FilePath); }
			catch(Exception ex) { Print("Failed to open file. " + ex.Message); }
			if(text == null) return false;

			if(_activeDoc != null) _activeDoc.Visible = false;
			doc = new SciCode(f, fls);
			_docs.Add(doc);
			_activeDoc = doc;
			this.Controls.Add(doc);
			doc.Init(text);
		}
		if(focus) _activeDoc.Focus();

		_UpdateUI_IsOpen();
		return true;
	}

	/// <summary>
	/// If f is open, closes its document and destroys its control.
	/// f can be any, not necessary the active document.
	/// Saves text before closing the active document.
	/// Does not show another document when closed the active document.
	/// </summary>
	/// <param name="f"></param>
	public void Close(FileNode f)
	{
		Debug.Assert(f != null);
		SciCode doc;
		if(f == _activeDoc?.FN) {
			Model.Save.TextNowIfNeed();
			doc = _activeDoc;
			_activeDoc = null;
		} else {
			doc = _docs.Find(v => v.FN == f);
			if(doc == null) return;
		}
		doc.Dispose();
		_docs.Remove(doc);
		_UpdateUI_IsOpen();
	}

	/// <summary>
	/// Closes all documents and destroys controls.
	/// </summary>
	public void CloseAll(bool saveTextIfNeed)
	{
		if(saveTextIfNeed) Model.Save.TextNowIfNeed();
		_activeDoc = null;
		foreach(var doc in _docs) doc.Dispose();
		_docs.Clear();
		_UpdateUI_IsOpen();
	}

	public bool SaveText()
	{
		return _activeDoc?.SaveText() ?? true;
	}

	public void SaveEditorData()
	{
		_activeDoc?.SaveEditorData();
	}

	//public bool IsModified => _activeDoc?.IsModified ?? false;

	/// <summary>
	/// Enables/disables Edit and Run toolbars/menus and some other UI parts depending on whether a document is open in editor.
	/// </summary>
	void _UpdateUI_IsOpen(bool asynchronously = true)
	{
		bool enable = _activeDoc != null;
		if(enable != _uiDisabled_IsOpen) return;

		if(asynchronously) {
			BeginInvoke(new Action(() => _UpdateUI_IsOpen(false)));
			return;
		}
		_uiDisabled_IsOpen = !enable;

		//toolbars
		Strips.tbEdit.Enabled = enable;
		Strips.tbRun.Enabled = enable;
		//menus
		Strips.Menubar.Items["Menu_Edit"].Enabled = enable;
		Strips.Menubar.Items["Menu_Run"].Enabled = enable;
		//toolbar buttons
		Strips.tbFile.Items["File_Properties"].Enabled = enable;
		//drop-down menu items and submenus
		//don't disable these because can right-click...
		//Strips.ddFile.Items["File_Disable"].Enabled = enable;
		//Strips.ddFile.Items["File_Rename"].Enabled = enable;
		//Strips.ddFile.Items["File_Delete"].Enabled = enable;
		//Strips.ddFile.Items["File_Properties"].Enabled = enable;
		//Strips.ddFile.Items["File_More"].Enabled = enable;
	}
	bool _uiDisabled_IsOpen;

	/// <summary>
	/// Enables/disables commands (toolbar buttons, menu items) depending on document state such as "can undo".
	/// Called on SCN_UPDATEUI.
	/// </summary>
	void _UpdateUI_Cmd()
	{
		EUpdateUI disable = 0;
		var d = _activeDoc;
		if(d == null) return; //we disable the toolbar and menu
		if(0 == d.Call(SCI_CANUNDO)) disable |= EUpdateUI.Undo;
		if(0 == d.Call(SCI_CANREDO)) disable |= EUpdateUI.Redo;
		if(0 != d.Call(SCI_GETSELECTIONEMPTY)) disable |= EUpdateUI.Copy;
		if(disable.Has_(EUpdateUI.Copy) || d.ST.IsReadonly) disable |= EUpdateUI.Cut;
		//if(0 == d.Call(SCI_CANPASTE)) disable |= EUpdateUI.Paste; //rejected. Often slow. Also need to see on focused etc.

		var dif = disable ^ _cmdDisabled; if(dif == 0) return;

		//Print(dif);
		_cmdDisabled = disable;
		if(dif.Has_(EUpdateUI.Undo)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Undo), !disable.Has_(EUpdateUI.Undo));
		if(dif.Has_(EUpdateUI.Redo)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Redo), !disable.Has_(EUpdateUI.Redo));
		if(dif.Has_(EUpdateUI.Cut)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Cut), !disable.Has_(EUpdateUI.Cut));
		if(dif.Has_(EUpdateUI.Copy)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Copy), !disable.Has_(EUpdateUI.Copy));
		//if(dif.Has_(EUpdateUI.Paste)) Strips.EnableCmd(nameof(CmdHandlers.Edit_Paste), !disable.Has_(EUpdateUI.Paste));

	}

	EUpdateUI _cmdDisabled;

	[Flags]
	enum EUpdateUI
	{
		Undo = 1,
		Redo = 2,
		Cut = 4,
		Copy = 8,
		//Paste = 16,

	}

	public unsafe void Test()
	{
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\any.dll,-85"));
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\a.bmp"));
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\.bmp"));
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\any.ico"));
		//Print(EImageUtil.ImageTypeFromString(true, @"\\a\b\any.png"));
		//Print(EImageUtil.ImageTypeFromString(true, @"~:123456"));
		//Print(EImageUtil.ImageTypeFromString(true, @"resource:mmm"));

		//_img.ClearCache();

		//for(int i = 0; i < 10; i++) Print($"{i}: '{_t.AnnotationText(i)}'");
		//for(int i = 0; i < 10; i++) _t.AnnotationText(i, "||||new text");
		//for(int i = 0; i < 10; i++) _t.AnnotationText(i, null);

		//_t.AnnotationText(0, "Test\nAnnotations");
		//_t.AnnotationText(0, Empty(_t.AnnotationText(0)) ? "Test\nAnnotations" : "");
		//_t.AnnotationText(0, (_t.AnnotationText(0).Length<5) ? "Test\nAnnotations" : "abc");

		//Print(_c.Images.Visible);

		//switch(_c.Images.Visible) {
		//case Sci.AnnotationsVisible.ANNOTATION_HIDDEN:
		//	_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_STANDARD;
		//	//_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_BOXED;
		//	break;
		//default:
		//	_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_HIDDEN;
		//	//_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_BOXED;
		//	break;
		//}

		//switch((Sci.AnnotationsVisible)(int)_c.Call(Sci.SCI_ANNOTATIONGETVISIBLE)) {
		//case Sci.AnnotationsVisible.ANNOTATION_HIDDEN:
		//	_c.Call(Sci.SCI_ANNOTATIONSETVISIBLE, (int)Sci.AnnotationsVisible.ANNOTATION_STANDARD);
		//	break;
		//default:
		//	_c.Call(Sci.SCI_ANNOTATIONSETVISIBLE, (int)Sci.AnnotationsVisible.ANNOTATION_HIDDEN);
		//	break;
		//}

		var o = Panels.Output;
		//o.Write(@"Three green strips: <image ""C:\Users\G\Documents\Untitled.bmp"">");
		//Print(_c.Text);
		Output.Clear();
		Print(_activeDoc?.Text);
		//_c.Text = "";

		//Print("one\0two");
		//Print("<><c 0x8000>one\0two</c>");


		//foreach(var f in File_.EnumDirectory(Folders.ProgramFiles, FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
		//	if(f.IsDirectory) continue;
		//	if(0 == f.Name.EndsWith_(true, ".png", ".bmp", ".jpg", ".gif", ".ico")) continue;
		//	//Print(f.FullPath);
		//	MainForm.Panels.Output.Write($"<image \"{f.FullPath}\">");
		//	Time.DoEvents();
		//}

	}

	//static bool _debugOnce;



	class SciCode : AuScintilla
	{
		public readonly FileNode FN;
		SciText.FileLoaderSaver _fls;

		const int c_marginFold = 0;
		const int c_marginLineNumbers = 1;
		const int c_marginMarkers = 2; //breakpoints etc

		public SciCode(FileNode file, SciText.FileLoaderSaver fls)
		{
			//_edit = edit;
			FN = file;
			_fls = fls;

			this.Dock = DockStyle.Fill;
			this.AccessibleName = "Code";
			this.AllowDrop = true;

			InitImagesStyle = ImagesStyle.AnyString;
			if(fls.IsBinary) InitReadOnlyAlways = true;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			int dpi = Au.Util.Dpi.BaseDPI;

			Call(SCI_SETMARGINTYPEN, c_marginLineNumbers, SC_MARGIN_NUMBER);
			ST.MarginWidth(c_marginLineNumbers, 40 * dpi / 96);
			//Call(SCI_SETMARGINTYPEN, c_marginMarkers, SC_MARGIN_SYMBOL);
			//ST.MarginWidth(c_marginMarkers, 0);

			ST.StyleFont(STYLE_DEFAULT, "Courier New", 8);
			ST.StyleClearAll();

			if(FN.IsCodeFile) {
				//_SetLexer(LexLanguage.SCLEX_CPP);
				ST.SetLexerCpp();
				_FoldingInit();
			}
		}

		//protected override void Dispose(bool disposing)
		//{
		//	Output.LibWriteQM2($"Dispose disposing={disposing} IsHandleCreated={IsHandleCreated} Visible={Visible}");
		//	base.Dispose(disposing);
		//}

		//protected override void OnVisibleChanged(EventArgs e)
		//{
		//	if(!Visible) Output.LibWriteQM2("hide");
		//	base.OnVisibleChanged(e);
		//}

		//protected override void OnMouseDown(MouseEventArgs e)
		//{
		//	switch(e.Button) {
		//	case MouseButtons.Middle:
		//		ST.ClearText();
		//		break;
		//	}
		//	base.OnMouseDown(e);
		//}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			switch(e.Button) {
			case MouseButtons.Right:
				Strips.ddEdit.ShowAsContextMenu_();
				break;
			}
			base.OnMouseUp(e);
		}

		protected override void OnSciNotify(ref SCNotification n)
		{
			//switch(n.nmhdr.code) {
			////case NOTIF.SCN_PAINTED:
			////case NOTIF.SCN_UPDATEUI:
			////case NOTIF.SCN_FOCUSIN:
			////case NOTIF.SCN_FOCUSOUT:
			////	break;
			//case NOTIF.SCN_MODIFIED:
			//	Print(n.nmhdr.code, n.modificationType);
			//	break;
			//default:
			//	Print(n.nmhdr.code);
			//	break;
			//}

			switch(n.nmhdr.code) {
			case NOTIF.SCN_SAVEPOINTLEFT:
				Model.Save.TextLater();
				break;
			case NOTIF.SCN_SAVEPOINTREACHED:
				//never mind: we should cancel the 'save text later'
				break;
			case NOTIF.SCN_MODIFIED:

				break;
			case NOTIF.SCN_UPDATEUI:
				if(_initDeferred) { _initDeferred = false; _InitDeferred(); }
				//Print((uint)n.updated);
				if(0 != (n.updated & 3)) Panels.Editor._UpdateUI_Cmd();
				break;
			case NOTIF.SCN_MARGINCLICK:
				if(n.margin == c_marginFold) {
					_FoldingOnMarginClick(null, n.position);
				}
				break;
			}

			base.OnSciNotify(ref n);
		}

		protected override void WndProc(ref Message m)
		{
			//var w = (Wnd)m.HWnd;
			//Print(m);
			switch(m.Msg) {
			case Api.WM_SETFOCUS:
				if(!_noModelEnsureCurrentSelected) Model?.EnsureCurrentSelected();
				break;
			}
			base.WndProc(ref m);
		}
		bool _noModelEnsureCurrentSelected;

		public bool IsUnsaved => 0 != Call(SCI_GETMODIFY);

		public bool SaveText()
		{
			if(IsUnsaved) {
				try { _fls.Save(ST, FN.FilePath); }
				catch(Exception ex) { Print(ex.Message); return false; }
				Call(SCI_SETSAVEPOINT);
				//Print("saved");
			}
			return true;
		}

		internal void Init(byte[] text)
		{
			if(!IsHandleCreated) CreateHandle();
			_fls.SetText(ST, text);
			_initDeferred = true; //now folding does not work well. The first place where it works is SCN_UPDATEUI.
		}

		#region editor data

		Convert_.MD5HashResult _savedMD5;
		bool _initDeferred;

		unsafe void _InitDeferred()
		{
			var db = Model.DB; if(db == null) return;
			try {
				using(var p = db.Statement("SELECT lines FROM _editor WHERE id=?", FN.Id)) {
					if(p.Step()) {
						var a = p.GetList<int>(0);
						if(a != null) {
							_savedMD5 = _Hash(a);
							for(int i = a.Count - 1; i >= 0; i--) { //must be in reverse order, else does not work
								int v = a[i];
								int line = v & 0x7FFFFFF, marker = v >> 27 & 31;
								if(marker == 31) _FoldingFoldLine(line);
								else Call(SCI_MARKERADDSET, line, 1 << marker);
							}
						}
					}
				}
			}
			catch(SLException ex) { Debug_.Print(ex); }
		}

		static unsafe Convert_.MD5HashResult _Hash(List<int> a)
		{
			if(a.Count == 0) return default;
			Convert_.MD5Hash md5 = default;
			foreach(var v in a) md5.Add(v);
			return md5.Hash;
		}

		internal void SaveEditorData()
		{
			var db = Model.DB; if(db == null) return;
			var a = new List<int>();
			_GetLineDataToSave(0, a);
			_GetLineDataToSave(1, a);
			_GetLineDataToSave(31, a);
			var hash = _Hash(a);
			if(hash != _savedMD5) {
				//Print("changed");
				try {
					if(a.Count == 0) {
						db.Execute("DELETE FROM _editor WHERE id=?", FN.Id);
					} else {
						using(var p = db.Statement("REPLACE INTO _editor (id,lines) VALUES (?,?)")) {
							p.Bind(1, FN.Id).Bind(2, a).Step();
						}
					}
					_savedMD5 = hash;
				}
				catch(SLException ex) { Debug_.Print(ex); }
			}
		}

		/// <summary>
		/// Gets indices of lines containing markers or contracted folding points.
		/// </summary>
		/// <param name="marker">If 31, uses SCI_CONTRACTEDFOLDNEXT. Else uses SCI_MARKERNEXT; must be 0...24 (markers 25-31 are used for folding).</param>
		/// <param name="saved">Receives line indices | marker in high-order 5 bits.</param>
		void _GetLineDataToSave(int marker, List<int> a)
		{
			Debug.Assert((uint)marker < 32); //we have 5 bits for marker
			for(int i = 0; ; i++) {
				if(marker == 31) i = Call(SCI_CONTRACTEDFOLDNEXT, i);
				else i = Call(SCI_MARKERNEXT, 1 << i, marker);
				if((uint)i > 0x7FFFFFF) break; //-1 if no more; ensure we have 5 high-order bits for marker; max 134 M lines.
				a.Add(i | (marker << 27));
			}
		}

		#endregion

		#region folding

		void _FoldingInit()
		{
			ST.SetStringString(SCI_SETPROPERTY, "fold\0" + "1");
			ST.SetStringString(SCI_SETPROPERTY, "fold.comment\0" + "1");
			ST.SetStringString(SCI_SETPROPERTY, "fold.preprocessor\0" + "1");
			ST.SetStringString(SCI_SETPROPERTY, "fold.cpp.preprocessor.at.else\0" + "1");
#if false
			ST.SetStringString(SCI_SETPROPERTY, "fold.cpp.syntax.based\0" + "0");
#else
			//ST.SetStringString(SCI_SETPROPERTY, "fold.at.else\0" + "1");
#endif
			ST.SetStringString(SCI_SETPROPERTY, "fold.cpp.explicit.start\0" + "//{{"); //default is //{
			ST.SetStringString(SCI_SETPROPERTY, "fold.cpp.explicit.end\0" + "//}}");

			Call(SCI_SETMARGINTYPEN, c_marginFold, SC_MARGIN_SYMBOL);
			Call(SCI_SETMARGINMASKN, c_marginFold, SC_MASK_FOLDERS);
			Call(SCI_SETMARGINSENSITIVEN, c_marginFold, 1);

			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDEROPEN, SC_MARK_BOXMINUS);
			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDER, SC_MARK_BOXPLUS);
			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDERSUB, SC_MARK_VLINE);
			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDERTAIL, SC_MARK_LCORNER);
			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDEREND, SC_MARK_BOXPLUSCONNECTED);
			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDEROPENMID, SC_MARK_BOXMINUSCONNECTED);
			Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDERMIDTAIL, SC_MARK_TCORNER);
			for(int i = 25; i < 32; i++) {
				Call(SCI_MARKERSETFORE, i, 0xffffff);
				Call(SCI_MARKERSETBACK, i, 0x808080);
				//Call(SCI_MARKERSETBACKSELECTED, i, i == SC_MARKNUM_FOLDER ? 0xFF0000 : 0x808080); //why dos not work?
			}
			//Call(SCI_MARKERENABLEHIGHLIGHT, 1); //why dos not work?

			Call(SCI_SETFOLDFLAGS, SC_FOLDFLAG_LINEAFTER_CONTRACTED);
			Call(SCI_FOLDDISPLAYTEXTSETSTYLE, SC_FOLDDISPLAYTEXT_STANDARD);
			ST.StyleForeColor(STYLE_FOLDDISPLAYTEXT, 0x808080);

			Call(SCI_SETMARGINCURSORN, c_marginFold, SC_CURSORARROW);

			int wid = Call(SCI_TEXTHEIGHT) - 4;
			ST.MarginWidth(c_marginFold, Math.Max(wid, 12));
		}

		bool _FoldingOnMarginClick(bool? fold, int startPos)
		{
			int line = Call(SCI_LINEFROMPOSITION, startPos);
			if(0 == (Call(SCI_GETFOLDLEVEL, line) & SC_FOLDLEVELHEADERFLAG)) return false;
			bool isExpanded = 0 != Call(SCI_GETFOLDEXPANDED, line);
			if(fold.HasValue && fold.GetValueOrDefault() != isExpanded) return false;
			if(isExpanded) {
				_FoldingFoldLine(line);
				//move caret out of contracted region
				int pos = ST.CurrentPositionBytes;
				if(pos > startPos) {
					int i = ST.LineEnd(Call(SCI_GETLASTCHILD, line, -1));
					if(pos <= i) ST.CurrentPositionBytes = startPos;
				}
			} else {
				Call(SCI_FOLDLINE, line, 1);
			}
			return true;
		}

		void _FoldingFoldLine(int line)
		{
			string s = ST.LineText(line), s2 = "";
			for(int i = 0; i < s.Length; i++) {
				char c = s[i];
				if(c == '{') { s2 = "... }"; break; }
				if(c == '/' && i < s.Length - 1) {
					c = s[i + 1];
					if(c == '*') { s2 = "... */"; break; }
					if(i < s.Length - 3 && c == '/' && s[i + 2] == '{' && s[i + 3] == '{') { s2 = "... }}"; break; }
				}
			}
			//problem: quite slow. At startup ~250 mcs. The above code is fast.
			if(s2.Length == 0) Call(SCI_FOLDLINE, line); //slightly faster
			else ST.SetString(SCI_TOGGLEFOLDSHOWTEXT, line, s2);
		}

		#endregion

		#region drag drop

		enum _DD_DataType { None, Text, Files, Shell, Link, Script };
		_DD_DataType _drag;

		protected override void OnDragEnter(DragEventArgs e)
		{
			var d = e.Data;
			//foreach(var v in d.GetFormats()) Print(v, d.GetData(v, false)?.GetType()); Print("--");
			_drag = 0;
			if(d.GetDataPresent("Aga.Controls.Tree.TreeNodeAdv[]", false)) _drag = _DD_DataType.Script;
			else if(d.GetDataPresent("FileDrop", false)) _drag = _DD_DataType.Files;
			else if(d.GetDataPresent("Shell IDList Array", false)) _drag = _DD_DataType.Shell;
			else if(d.GetDataPresent("UnicodeText", false))
				_drag = d.GetDataPresent("FileGroupDescriptorW", false) ? _DD_DataType.Link : _DD_DataType.Text;
			e.Effect = _DD_GetEffect(e);
			base.OnDragEnter(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			if((e.Effect = _DD_GetEffect(e)) != 0) _DD_Over(e);
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
				pos = ST.LineStartFromPosition(pos);
				p.X = Call(SCI_POINTXFROMPOSITION, 0, pos);
				p.Y = Call(SCI_POINTYFROMPOSITION, 0, pos);
			} else pos = 0;
			return p;
		}

		unsafe void _DD_Over(DragEventArgs e)
		{
			var p = _DD_GetDropPos(e, out _);
			var z = new Sci_DragDropData { x = p.X, y = p.Y };
			Call(SCI_DRAGDROP, 1, &z);

			//FUTURE: auto-scroll
		}

		unsafe void _DD_Drop(DragEventArgs e)
		{
			var xy = _DD_GetDropPos(e, out int pos);
			string s = null; StringBuilder t = null;
			int endOfMeta = 0; bool inMeta = false; string menuVar = null;

			if(_drag != _DD_DataType.Text) {
				t = new StringBuilder();
				if(FN.IsCodeFile) {
					var text = ST.GetText();
					if(Au.Compiler.MetaComments.FindMetaComments(text, out endOfMeta) && pos > 0 && pos < endOfMeta) inMeta = true;
					else if(pos > endOfMeta) text.RegexMatch_(@"\b(\w+)\s*=\s*new\s+Au(?:Menu|Toolbar)", 1, out menuVar, 0, new RXMore(endOfMeta, pos));
				}
			}

			var d = e.Data;
			switch(_drag) {
			case _DD_DataType.Text:
				s = d.GetData("UnicodeText", false) as string;
				break;
			case _DD_DataType.Files:
				var paths = d.GetData("FileDrop", false) as string[];
				if(paths != null) {
					foreach(var path in paths) {
						bool isLnk = path.EndsWithI_(".lnk");
						if(isLnk) t.Append("//");
						var name = Path_.GetFileName(path, true);
						_AppendFile(path, name);
						if(isLnk) {
							try {
								var g = Shell.Shortcut.Open(path);
								string target = g.TargetAnyType, args = null;
								if(target.StartsWith_("::")) {
									using(var pidl = Shell.Pidl.FromString(target))
										name = pidl.ToShellString(Native.SIGDN.NORMALDISPLAY);
								} else {
									args = g.Arguments;
									if(!target.EndsWithI_(".exe") || name.IndexOf_("Shortcut") >= 0)
										name = Path_.GetFileName(target, true);
								}
								_AppendFile(target, name, args);
							}
							catch(AuException) { break; }
						}
					}
					s = t.ToString();
				}
				break;
			case _DD_DataType.Shell:
				_DD_GetShell(d, out var shells, out var names);
				if(shells != null) {
					for(int i = 0; i < shells.Length; i++) {
						_AppendFile(shells[i], names[i]);
					}
					s = t.ToString();
				}
				break;
			case _DD_DataType.Link:
				_DD_GetLink(d, out s, out var s2);
				if(s != null) {
					_AppendFile(s, s2);
					s = t.ToString();
				}
				break;
			case _DD_DataType.Script:
				var nodes = d.GetData("Aga.Controls.Tree.TreeNodeAdv[]", false) as Aga.Controls.Tree.TreeNodeAdv[];
				if(nodes != null) {
					foreach(var tn in nodes) {
						var fn = tn.Tag as FileNode;
						_AppendFile(fn.ItemPath, fn.Name, null, fn);
					}
					s = t.ToString();
				}
				break;
			}

			if(!Empty(s)) {
				var z = new Sci_DragDropData { x = xy.X, y = xy.Y };
				var b = Convert_.Utf8FromString(s);
				fixed (byte* bp = b) {
					z.text = bp;
					z.len = b.Length - 1;
					if(_drag != _DD_DataType.Text || 0 == (e.Effect & DragDropEffects.Move)) z.copy = 1;
					Call(SCI_DRAGDROP, 2, &z);
				}
				if(!Focused && ((Wnd)(FindForm())).IsActive) { //note: don't activate window; let the drag source do it, eg Explorer activates on drag-enter.
					_noModelEnsureCurrentSelected = true; //don't scroll treeview to currentfile
					Focus();
					_noModelEnsureCurrentSelected = false;
				}
			} else {
				Call(SCI_DRAGDROP, 3);
			}

			void _AppendFile(string path, string name, string args = null, FileNode fn = null)
			{
				if(!FN.IsCodeFile) {
					t.Append(path);
				} else if(inMeta) {
					string opt = "//";
					switch(_drag) {
					case _DD_DataType.Files:
						opt = File_.ExistsAsDirectory(path) ? "outputPath " : "r ";
						break;
					case _DD_DataType.Script:
						if(fn.IsFolder) {
							if(fn.IsProjectFolder(out fn)) { opt = "library "; path = fn.ItemPath; }
						} else if(!fn.IsScript) {
							if(!fn.IsCodeFile) opt = "resource ";
							else if(fn.FindProject(out _, out var fMain) && fn == fMain) opt = "library ";
							else opt = "c ";
						}
						break;
					}
					//make relative path
					var p2 = FN.ItemPath; int i = p2.LastIndexOf('\\') + 1;
					if(0 == string.CompareOrdinal(path, 0, p2, 0, i)) path = path.Substring(i);

					t.Append(opt).Append(path);
				} else {
					name = name.Escape_();
					if(menuVar != null) t.Append(menuVar).Append("[\"").Append(name).Append("\"] =o=> ");
					bool isFN = fn != null;
					if(isFN && !fn.IsCodeFile) {
						t.Append("//").Append(path);
					} else {
						t.Append(isFN ? "AuTask.Run(@\"" : "Shell.Run(@\"").Append(path);
						if(!Empty(args)) t.Append("\", \"").Append(args.Escape_());
						t.Append("\");");
						if(menuVar == null && !isFN && (path.StartsWith_("::") || path.IndexOf_(name, true) < 0)) t.Append(" //").Append(name);
						//FUTURE: add unexpanded path version
					}
				}
				t.AppendLine();
			}
		}

		DragDropEffects _DD_GetEffect(DragEventArgs e)
		{
			if(_drag == 0) return 0;
			if(ST.IsReadonly) return 0;
			var ae = e.AllowedEffect;
			DragDropEffects r = 0;
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
			fixed (byte* p = b) {
				int* pi = (int*)p;
				int n = *pi++; if(n < 1) return;
				shells = new string[n]; names = new string[n];
				IntPtr pidlFolder = (IntPtr)(p + *pi++);
				for(int i = 0; i < n; i++) {
					using(var pidl = new Shell.Pidl(pidlFolder, (IntPtr)(p + pi[i]))) {
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
			fixed (byte* p = b) { //FILEGROUPDESCRIPTORW
				if(*(int*)p != 1) return; //count of FILEDESCRIPTORW
				var s = new string((char*)(p + 76));
				if(!s.EndsWithI_(".url")) return;
				url = d.GetData("UnicodeText", false) as string;
				if(url != null) text = s.Remove(s.Length - 4);
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
	}
}
