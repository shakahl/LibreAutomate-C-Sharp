//#define TEST_SIMPLE

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
//using System.Xml.XPath;

using G.Controls;
using static G.Controls.Sci;

using Catkeys;
using static Catkeys.NoClass;
using static Program;

partial class Edit
{
	SciControl _c;
	SciText _t;
	FileNode _fn;

	public Edit()
	{
		_c = new SciControl();
		_t = _c.ST;
		_c.Name = _c.AccessibleName = "Code";
		_c.Visible = false;
		//_c.InitImagesStyle = SciControl.ImagesStyle.ImageTag;
		_c.InitImagesStyle = SciControl.ImagesStyle.AnyString;
		_SetIsOpenState(false);

		_t.StyleFont(STYLE_DEFAULT, "Courier New", 8);
		_t.StyleClearAll();

		//_c.Deb = true;

		_SetLexer(LexLanguage.SCLEX_CPP);

#if TEST_SIMPLE
#else
#endif

#if TEST

		Time.SetTimer(10, true, t =>
		{
			MainForm.Model.SetCurrentFile(MainForm.Model.FindFile("form.cs"));

			//for(int i = 0; i < 5; i++) _t.AnnotationText(i, "----Test\n----Annotations");

			//Test();
		});
#endif
	}

	public SciControl SC { get => _c; }

	public void Open(FileNode f)
	{
		_fn = f;
		string s = null;
		if(f != null) {
			try {
				s = File.ReadAllText(f.FilePath);
			}
			catch(Exception ex) { Print(ex.Message); }

		} else {

		}
		_c.Text = s;
		_c.Call(SCI_EMPTYUNDOBUFFER);
		//_c.Call(SCI_SETSAVEPOINT); //SCI_EMPTYUNDOBUFFER probably does it

		_SetIsOpenState(s != null);
	}

	public void Save()
	{
		if(_fn == null) return;
		try {
			File.WriteAllText(_fn.FilePath, _c.Text);
		}
		catch(Exception ex) { Print(ex.Message); }
	}

	void _SetIsOpenState(bool isOpen)
	{
		if(isOpen == _c.Visible) return;
		_c.Visible = isOpen;

		var t = MainForm.Strips;
		//toolbars
		t.tbEdit.Enabled = isOpen;
		t.tbRun.Enabled = isOpen;
		//toolbar buttons
		t.tbFile.Items["File_Properties"].Enabled = isOpen;
		//top-level menu items
		t.Menubar.Items["Menu_Edit"].Enabled = isOpen;
		t.Menubar.Items["Menu_Run"].Enabled = isOpen;
		//drop-down menu items and submenus
		//don't disable these because can right-click...
		//t.ddFile.Items["File_Disable"].Enabled = isOpen;
		//t.ddFile.Items["File_Rename"].Enabled = isOpen;
		//t.ddFile.Items["File_Delete"].Enabled = isOpen;
		//t.ddFile.Items["File_Properties"].Enabled = isOpen;
		//t.ddFile.Items["File_More"].Enabled = isOpen;
	}

	void _SetLexer(LexLanguage lang)
	{
		if(lang == _currentLexer) return;
		_currentLexer = lang;
		_t.StyleClearRange(0, STYLE_HIDDEN); //STYLE_DEFAULT - 1
		_c.Call(SCI_SETLEXER, (int)lang);

		const int colorComment = 0x8000;
		const int colorString = 0xA07040;
		const int colorNumber = 0xA04000;
		const int colorDoc = 0x606060;
		switch(lang) {
		case LexLanguage.SCLEX_CPP:
			_t.StyleForeColor((int)LexCppStyles.SCE_C_COMMENT, colorComment); //  /*...*/
			_t.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTLINE, colorComment); //  //...
			_t.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTLINEDOC, colorDoc); //  ///...
			_t.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTDOC, colorDoc); //  /**...*/
			_t.StyleForeColor((int)LexCppStyles.SCE_C_CHARACTER, colorNumber);
			_t.StyleForeColor((int)LexCppStyles.SCE_C_NUMBER, colorNumber);
			_t.StyleForeColor((int)LexCppStyles.SCE_C_STRING, colorString);
			_t.StyleForeColor((int)LexCppStyles.SCE_C_VERBATIM, colorString); //@"string"
			_t.StyleForeColor((int)LexCppStyles.SCE_C_ESCAPESEQUENCE, colorString);
			_t.StyleUnderline((int)LexCppStyles.SCE_C_ESCAPESEQUENCE, true);
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_OPERATOR, 0x80); //+,;( etc. Let it be black.
			_t.StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSOR, 0xFF8000);
			_t.StyleForeColor((int)LexCppStyles.SCE_C_WORD, 0xFF); //keywords
			_t.StyleForeColor((int)LexCppStyles.SCE_C_TASKMARKER, 0xFFFF00);
			_t.StyleBackColor((int)LexCppStyles.SCE_C_TASKMARKER, 0x0);
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_WORD2, 0x80F0); //functions. Not using here.
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_GLOBALCLASS, 0xC000C0); //types. Not using here.

			//_t.StyleForeColor((int)LexCppStyles.SCE_C_USERLITERAL, ); //C++, like 10_km
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_STRINGRAW, ); //R"string"
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_COMMENTDOCKEYWORD, ); //supports only JavaDoc and Doxygen
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSORCOMMENT, ); //?
			//_t.StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSORCOMMENTDOC, ); //?

			_t.SetStringString(SCI_SETPROPERTY, "styling.within.preprocessor\0" + "1");
			_t.SetStringString(SCI_SETPROPERTY, "lexer.cpp.allow.dollars\0" + "0");
			_t.SetStringString(SCI_SETPROPERTY, "lexer.cpp.track.preprocessor\0" + "0"); //default 1
			_t.SetStringString(SCI_SETPROPERTY, "lexer.cpp.escape.sequence\0" + "1");
			//_t.SetStringString(SCI_SETPROPERTY, "lexer.cpp.verbatim.strings.allow.escapes\0" + "1"); //expected to style "", but it does nothing

			//Print(_t.GetString(SCI_DESCRIBEKEYWORDSETS, 0, -1));
			//Primary keywords and identifiers
			//Secondary keywords and identifiers
			//Documentation comment keywords
			//Global classes and typedefs
			//Preprocessor definitions
			//Task marker and error marker keywords
			_t.SetString(SCI_SETKEYWORDS, 0, "abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in in int interface internal is lock long namespace new null object operator out out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using using static void volatile while add alias ascending async await descending dynamic from get global group into join let orderby partial partial remove select set value var when where yield");
			//_t.SetString(SCI_SETKEYWORDS, 1, "Print"); //functions. Not using here.
			//_t.SetString(SCI_SETKEYWORDS, 2, "summary <summary>"); //supports only JavaDoc and Doxygen
			//_t.SetString(SCI_SETKEYWORDS, 3, "Catkeys"); //types. Not using here.
			//_t.SetString(SCI_SETKEYWORDS, 4, "DEBUG TRACE"); //if used with #if, lexer knows which #if/#else branch to style. Not using here (see "lexer.cpp.track.preprocessor").
			_t.SetString(SCI_SETKEYWORDS, 5, "TO" + "DO SHOULD" + "DO CON" + "SIDER FU" + "TURE B" + "UG");
			break;
		}
	}
	LexLanguage _currentLexer;

	public unsafe void Test()
	{
#if TEST_SIMPLE
		//_c.Indicators[8].Style=IndicatorStyle.Hidden;
		//_c.IndicatorCurrent = 8;
		//_c.IndicatorValue = 100;
		//_c.IndicatorFillRange(2, 1);
		//PrintList(_c.Indicators[8].ValueAt(2), _c.Indicators[8].ValueAt(3));

		//_c.Styles[1].Font="Comic Sans MS";
		//_c.Styles[1].Size = 20;

		//Scintilla.SetModulePath
		//_c.Markers
		_c.Lines[0].AnnotationStyle = 1;
		_c.Lines[0].AnnotationText = "Test\nAnnotations";
		_c.AnnotationVisible = Annotation.Indented;

		//_c.ChangeLexerState(0, 100);
#else
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

		var o = MainForm.Panels.Output;
		//o.Write(@"Three green strips: <image ""C:\Users\G\Documents\Untitled.bmp"">");
		//Print(_c.Text);
		o.Output.Clear();
		o.Output.Write(_c.Text);
		//_c.Text = "";


		//foreach(var f in Files.EnumDirectory(Folders.ProgramFiles, Files.EDFlags.AndSubdirectories | Files.EDFlags.IgnoreAccessDeniedErrors)) {
		//	if(f.IsDirectory) continue;
		//	if(0 == f.Name.EndsWith_(true, ".png", ".bmp", ".jpg", ".gif", ".ico")) continue;
		//	//Print(f.FullPath);
		//	MainForm.Panels.Output.Write($"<image \"{f.FullPath}\">");
		//	Time.DoEvents();
		//}

#endif
	}

	//static bool _debugOnce;
}
