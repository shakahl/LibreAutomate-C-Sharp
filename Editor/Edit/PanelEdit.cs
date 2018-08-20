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
	List<SciCode> _docs = new List<SciCode>(); //documents that are actually open currently. Note that FilesModel.OpenFiles contains these and possibly more.
	SciCode _activeDoc;

	public AuScintilla ActiveDoc => _activeDoc;

	public PanelEdit()
	{
		this.Name = "Code";
		this.BackColor = SystemColors.AppWorkspace;

		_UpdateUI_IsOpen(); //never mind: makes startup slower by ~4ms (later, when enabling toolbars etc)

	}

	protected override void OnGotFocus(EventArgs e) { _activeDoc?.Focus(); }

	//public SciControl SC { get => _activeDoc; }

	/// <summary>
	/// If f is null, closes current file and destroys its control.
	/// Else hides current file's control, and:
	///		If f is already open, unhides its control.
	///		Else loads f text and creates control. If fails, does not change anything.
	/// </summary>
	/// <param name="f"></param>
	public bool Open(FileNode f)
	{
		Debug.Assert(MainForm.IsHandleCreated);

		if(f == null) {
			if(_activeDoc == null) return true;
			_activeDoc.Dispose();
			_docs.Remove(_activeDoc);
			_activeDoc = null;
		} else {
			bool focus = _activeDoc != null ? _activeDoc.Focused : false;
			var doc = _docs.Find(v => v.FN == f);
			if(doc != null) {
				if(_activeDoc != null) _activeDoc.Visible = false;
				_activeDoc = doc;
				_activeDoc.Visible = true;
			} else {
				string s = null;
				try {
					s = File.ReadAllText(f.FilePath);
				}
				catch(Exception ex) { Print(ex.Message); return false; }

				if(_activeDoc != null) _activeDoc.Visible = false;
				doc = new SciCode(f);
				_docs.Add(doc);
				_activeDoc = doc;
				this.Controls.Add(doc);
				//doc.CreateHandle_(); //info: not auto-created because not Visible
				doc.ST.SetText(s, noUndo: true, noNotif: true);
			}
			if(focus) _activeDoc.Focus();
		}

		bool wasOpen = IsOpen;
		IsOpen = _activeDoc != null;
		if(IsOpen != wasOpen) _UpdateUI_IsOpen();
		return true;
	}

	public bool Save()
	{
		if(IsOpen) return _activeDoc.Save();
		return true;
	}

	public bool IsOpen { get; private set; }

	//public bool IsModified { get => _activeDoc.IsModified; }

	/// <summary>
	/// Updates all UI (toolbars etc) depending on IsOpen.
	/// </summary>
	void _UpdateUI_IsOpen()
	{
		bool isOpen = IsOpen;

		//toolbars
		Strips.tbEdit.Enabled = isOpen;
		Strips.tbRun.Enabled = isOpen;
		//toolbar buttons
		Strips.tbFile.Items["File_Properties"].Enabled = isOpen;
		//top-level menu items
		Strips.Menubar.Items["Menu_Edit"].Enabled = isOpen;
		Strips.Menubar.Items["Menu_Run"].Enabled = isOpen;
		//drop-down menu items and submenus
		//don't disable these because can right-click...
		//Strips.ddFile.Items["File_Disable"].Enabled = isOpen;
		//Strips.ddFile.Items["File_Rename"].Enabled = isOpen;
		//Strips.ddFile.Items["File_Delete"].Enabled = isOpen;
		//Strips.ddFile.Items["File_Properties"].Enabled = isOpen;
		//Strips.ddFile.Items["File_More"].Enabled = isOpen;
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
			_InitFolding();
		}

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
			case NOTIF.SCN_MARGINCLICK:
				if(n.margin == c_marginFold) {
					_FoldLines(null, n.position);
					//TODO: save
				}
				break;
			}

			base.OnSciNotify(ref n);
		}

		public bool IsUnsaved => 0 != Call(SCI_GETMODIFY);

		public bool Save()
		{
			if(IsUnsaved) {
				try {
					File.WriteAllText(FN.FilePath, this.Text);
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

		void _InitFolding()
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

		bool _FoldLines(bool? fold, int startPos)
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
				Call(SCI_TOGGLEFOLD, line);
			}
			return true;
		}
	}
}
