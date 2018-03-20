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
//using System.Xml.XPath;

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
				doc.Text = s;
				//TODO: maybe temp disable undo collection etc
				doc.Call(SCI_EMPTYUNDOBUFFER);
				//doc.Call(SCI_SETSAVEPOINT); //SCI_EMPTYUNDOBUFFER probably does it
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

			_SetLexer(LexLanguage.SCLEX_CPP);
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
			//Print(n.nmhdr.code);

			base.OnSciNotify(ref n);
		}

		void _SetLexer(LexLanguage lang)
		{
			if(lang == _currentLexer) return;
			_currentLexer = lang;
			ST.StyleClearRange(0, STYLE_HIDDEN); //STYLE_DEFAULT - 1
			Call(SCI_SETLEXER, (int)lang);

			const int colorComment = 0x8000;
			const int colorString = 0xA07040;
			const int colorNumber = 0xA04000;
			const int colorDoc = 0x606060;
			switch(lang) {
			case LexLanguage.SCLEX_CPP:
				ST.StyleForeColor((int)LexCppStyles.SCE_C_COMMENT, colorComment); //  /*...*/
				ST.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTLINE, colorComment); //  //...
				ST.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTLINEDOC, colorDoc); //  ///...
				ST.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTDOC, colorDoc); //  /**...*/
				ST.StyleForeColor((int)LexCppStyles.SCE_C_CHARACTER, colorNumber);
				ST.StyleForeColor((int)LexCppStyles.SCE_C_NUMBER, colorNumber);
				ST.StyleForeColor((int)LexCppStyles.SCE_C_STRING, colorString);
				ST.StyleForeColor((int)LexCppStyles.SCE_C_VERBATIM, colorString); //@"string"
				ST.StyleForeColor((int)LexCppStyles.SCE_C_ESCAPESEQUENCE, colorString);
				ST.StyleUnderline((int)LexCppStyles.SCE_C_ESCAPESEQUENCE, true);
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_OPERATOR, 0x80); //+,;( etc. Let it be black.
				ST.StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSOR, 0xFF8000);
				ST.StyleForeColor((int)LexCppStyles.SCE_C_WORD, 0xFF); //keywords
				ST.StyleForeColor((int)LexCppStyles.SCE_C_TASKMARKER, 0xFFFF00);
				ST.StyleBackColor((int)LexCppStyles.SCE_C_TASKMARKER, 0x0);
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_WORD2, 0x80F0); //functions. Not using here.
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_GLOBALCLASS, 0xC000C0); //types. Not using here.

				//ST.StyleForeColor((int)LexCppStyles.SCE_C_USERLITERAL, ); //C++, like 10_km
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_STRINGRAW, ); //R"string"
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTDOCKEYWORD, ); //supports only JavaDoc and Doxygen
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSORCOMMENT, ); //?
				//ST.StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSORCOMMENTDOC, ); //?

				ST.SetStringString(SCI_SETPROPERTY, "styling.within.preprocessor\0" + "1");
				ST.SetStringString(SCI_SETPROPERTY, "lexer.cpp.allow.dollars\0" + "0");
				ST.SetStringString(SCI_SETPROPERTY, "lexer.cpp.track.preprocessor\0" + "0"); //default 1
				ST.SetStringString(SCI_SETPROPERTY, "lexer.cpp.escape.sequence\0" + "1");
				//ST.SetStringString(SCI_SETPROPERTY, "lexer.cpp.verbatim.strings.allow.escapes\0" + "1"); //expected to style "", but it does nothing

				//Print(ST.GetString(SCI_DESCRIBEKEYWORDSETS, 0, -1));
				//Primary keywords and identifiers
				//Secondary keywords and identifiers
				//Documentation comment keywords
				//Global classes and typedefs
				//Preprocessor definitions
				//Task marker and error marker keywords
				ST.SetString(SCI_SETKEYWORDS, 0, "abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using using static void volatile while add alias ascending async await descending dynamic from get global group into join let orderby partial partial remove select set value var when where yield");
				//ST.SetString(SCI_SETKEYWORDS, 1, "Print"); //functions. Not using here.
				//ST.SetString(SCI_SETKEYWORDS, 2, "summary <summary>"); //supports only JavaDoc and Doxygen
				//ST.SetString(SCI_SETKEYWORDS, 3, "Au"); //types. Not using here.
				//ST.SetString(SCI_SETKEYWORDS, 4, "DEBUG TRACE"); //if used with #if, lexer knows which #if/#else branch to style. Not using here (see "lexer.cpp.track.preprocessor").
				ST.SetString(SCI_SETKEYWORDS, 5, "TO" + "DO SHOULD" + "DO CON" + "SIDER FU" + "TURE B" + "UG");
				break;
			}
		}
		LexLanguage _currentLexer;

		public bool IsModified { get; private set; }

		public bool Save()
		{
			if(IsModified) {
				try {
					File.WriteAllText(FN.FilePath, this.Text);
				}
				catch(Exception ex) {
					Print(ex.Message);
					return false;
				}
			}
			return true;
		}
	}
}
