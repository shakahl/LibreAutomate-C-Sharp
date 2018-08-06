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


		//foreach(var f in Files.EnumDirectory(Folders.ProgramFiles, FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
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

			ST.MarginWidth(1, 30);
			ST.StyleFont(STYLE_DEFAULT, "Courier New", 8);
			ST.StyleClearAll();

			//_SetLexer(LexLanguage.SCLEX_CPP);
			ST.SetLexerCpp();
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
			}

			base.OnSciNotify(ref n);
		}

		//void _SetLexer(LexLanguage lang)
		//{
		//	if(lang == _currentLexer) return;
		//	_currentLexer = lang;

		//	if(lang == LexLanguage.SCLEX_CPP) {
		//		ST.SetLexerCpp();
		//		return;
		//	}

		//	ST.StyleClearRange(0, STYLE_HIDDEN); //STYLE_DEFAULT - 1
		//	Call(SCI_SETLEXER, (int)lang);

		//	const int colorComment = 0x8000;
		//	const int colorString = 0xA07040;
		//	const int colorNumber = 0xA04000;
		//	const int colorDoc = 0x606060;
		//	switch(lang) {
		//	case LexLanguage.SCLEX_CPP:
		//		ST.StyleForeColor((int)LexCppStyles.SCE_C_COMMENT, colorComment); //  /*...*/

		//		//... (see SciText.SetLexerCpp)
		//		break;
		//	}
		//}
		//LexLanguage _currentLexer;

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
	}
}
