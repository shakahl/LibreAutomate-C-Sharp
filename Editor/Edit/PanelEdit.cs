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
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;
using static Au.Controls.Sci;

partial class PanelEdit :Control
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

	//public SciControl SC { get => _activeDoc; }

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
		} else {
			string s = null;
			try {
				s = File_.LoadText(f.FilePath);
			}
			catch(Exception ex) { Print(ex.Message); return false; }

			if(_activeDoc != null) _activeDoc.Visible = false;
			doc = new SciCode(f);
			_docs.Add(doc);
			_activeDoc = doc;
			this.Controls.Add(doc);
			//doc.CreateHandle_(); //info: not auto-created because not Visible
			doc.ST.SetText(s, noUndo: true, noNotif: true);
			doc.LoadEditorData();
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

	//public bool IsModified { get => _activeDoc?.IsModified ?? false; }

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
		if(disable.Has_(EUpdateUI.Copy) || 0 != d.Call(SCI_GETREADONLY)) disable |= EUpdateUI.Cut;
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



	class SciCode :AuScintilla
	{
		public readonly FileNode FN;

		const int c_marginFold = 0;
		const int c_marginLineNumbers = 1;
		const int c_marginMarkers = 2; //breakpoints etc

		public SciCode(FileNode file)
		{
			//_edit = edit;
			FN = file;

			this.Dock = DockStyle.Fill;
			this.AccessibleName = "Code";

			InitImagesStyle = ImagesStyle.AnyString;
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

			//_SetLexer(LexLanguage.SCLEX_CPP);
			ST.SetLexerCpp();
			_FoldingInit();
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
			//case NOTIF.SCN_PAINTED:
			//case NOTIF.SCN_UPDATEUI:
			//case NOTIF.SCN_FOCUSIN:
			//case NOTIF.SCN_FOCUSOUT:
			//	break;
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
			switch(m.Msg) {
			case Api.WM_SETFOCUS:
				Model?.OnEditorFocused();
				break;
			}
			base.WndProc(ref m);
		}

		public bool IsUnsaved => 0 != Call(SCI_GETMODIFY);

		public bool SaveText()
		{
			if(IsUnsaved) {
				try {
					File_.Save(FN.FilePath, this.Text);
				}
				catch(Exception ex) {
					Print(ex.Message);
					return false;
				}
				Call(SCI_SETSAVEPOINT);
				//Print("saved");
			}
			return true;
		}

		#region editor data

		DBEdit _savedED;

		internal void LoadEditorData()
		{
			_savedED = Model.TableEdit?.FindById((int)FN.Id);
			_savedED?.folding?.ForEach(line => Call(SCI_FOLDLINE, line));
			_savedED?.bookmarks?.ForEach(line => Call(SCI_MARKERADDSET, line, 1));
			_savedED?.breakpoints?.ForEach(line => Call(SCI_MARKERADDSET, line, 2));

			//speed with LiteDB: load or save: first time ngened min 31 ms, non-ngened min 105 ms; then 1 ms.
			//speed with PersistentDictionary (ESENT): load: first time 250 ms, then 95 ms; save 120 ms. Don't remember whether ngened.
		}

		internal void SaveEditorData()
		{
			List<int> folding = _savedED?.folding, bookmarks = _savedED?.bookmarks, breakpoints = _savedED?.breakpoints;
			bool changed = false;
			if(_GetLineDataToSave(0, ref folding)) changed = true;
			if(_GetLineDataToSave(1, ref bookmarks)) changed = true;
			if(_GetLineDataToSave(2, ref breakpoints)) changed = true;

			if(changed) {
				if(_savedED == null) _savedED = new DBEdit { id = (int)FN.Id };
				_savedED.folding = folding;
				_savedED.bookmarks = bookmarks;
				_savedED.breakpoints = breakpoints;
				Model.TableEdit?.Upsert(_savedED);
			}
		}

		/// <summary>
		/// Gets indices of lines containing markers or contracted folding points.
		/// Returns true if changed and need to save.
		/// </summary>
		/// <param name="marker">If 0, uses SCI_CONTRACTEDFOLDNEXT. Else uses SCI_MARKERNEXT; it is markerMask.</param>
		/// <param name="saved">On input - previously saved line indices; can be null or empty if none. On output - current line indices; null if was null and returned false.</param>
		bool _GetLineDataToSave(int marker, ref List<int> saved)
		{
			bool changed = false; int nContracted = 0;
			var a = saved;
			for(int i = 0; ; i++, nContracted++) {
				if(marker == 0) i = Call(SCI_CONTRACTEDFOLDNEXT, i);
				else i = Call(SCI_MARKERNEXT, i, marker);
				if(i < 0) break;

				if(a == null) {
					changed = true;
					a = new List<int>();
				} else if(!changed) {
					if(nContracted < a.Count && a[nContracted] == i) continue;
					changed = true;
					a.RemoveRange(nContracted, a.Count - nContracted);
				}
				a.Add(i);
			}
			if(!changed && a != null && nContracted < a.Count) {
				changed = true;
				a.RemoveRange(nContracted, a.Count - nContracted);
			}
			saved = a;
			return changed;
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
				string s = ST.LineText(line), s2 = "";
				if(s.Contains("//{{")) s2 = "... }}"; else if(s.Contains("/*")) s2 = "... */";
				ST.SetString(SCI_TOGGLEFOLDSHOWTEXT, line, s2);
				//move caret out of contracted region
				int pos = ST.PositionBytes;
				if(pos > startPos) {
					int i = ST.LineEnd(Call(SCI_GETLASTCHILD, line, -1));
					if(pos <= i) ST.PositionBytes = startPos;
				}
			} else {
				Call(SCI_FOLDLINE, line, 1);
			}
			return true;
		}

		#endregion
	}
}
