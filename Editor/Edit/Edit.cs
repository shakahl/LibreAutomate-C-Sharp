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

using ScintillaNET;

using Catkeys;
using static Catkeys.NoClass;
using static Program;

partial class Edit
{
	Scintilla _c;
	FileNode _fn;
	SciImages _img;

	public Edit()
	{
		_c = new Scintilla();
		_c.Name = _c.AccessibleName = "Code";
		_c.BorderStyle = BorderStyle.None;
		_SetIsOpenState(false);

		_c.Styles[0].Font = "Courier New"; _c.Styles[0].Size = 8;
		//InitSyntaxColoring();
		//_InitImages();

		//_c.Styles[1].Font = "Courier New"; _c.Styles[1].Size = 8;
		//_c.Styles[1].Visible = false; //ignored

		//_c.Styles[1].Font = "MS Sans Serif"; _c.Styles[1].Size = 4;
#if TEST_SIMPLE
#else
#endif

#if TEST

		Time.SetTimer(10, true, t =>
		{
			MainForm.Model.SetCurrentFile(MainForm.Model.FindFile("form.cs"));
			Test();
		});
#endif
	}

	public Scintilla Sci { get => _c; }

	public void Open(FileNode f)
	{
		if(_img != null) {
			_img.Dispose();
			_img = null;
		}

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
		_SetIsOpenState(s != null);

		if(s != null) {
			_img = new SciImages(_c);

		}
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

	private void InitSyntaxColoring()
	{

		// Configure the default style
		_c.StyleResetDefault();
		//_c.Styles[Style.Default].Font = "Consolas";
		//_c.Styles[Style.Default].Size = 10;
		_c.Styles[Style.Default].Font = "Courier New";
		//_c.Styles[Style.Default].BackColor = IntToColor(0x212121);
		//_c.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
		_c.StyleClearAll();

		// Configure the CPP (C#) lexer styles
		_c.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0x404040);
		_c.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0x008000);
		_c.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x008000);
		//_c.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x808080); //?
		_c.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xA06000);
		_c.Styles[Style.Cpp.String].ForeColor = IntToColor(0xA06000);
		_c.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xC04000);
		_c.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0xFF0000);
		_c.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0x000080);
		//_c.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
		_c.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x808080); //works with: ///<summary>text</summary>
		_c.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x0000FF); //keywords
		_c.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0x008080); //types, namespaces

		//_c.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991); //?
		//_c.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000); //?
		//_c.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0xFF0000); //?

		_c.Lexer = Lexer.Cpp;

		_c.SetKeywords(0, "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
		_c.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");

		// Configure the default style
		//_c.StyleResetDefault();
		//_c.Styles[Style.Default].Font = "Consolas";
		//_c.Styles[Style.Default].Size = 10;
		//_c.Styles[Style.Default].BackColor = IntToColor(0x212121);
		//_c.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
		//_c.StyleClearAll();

		// Configure the CPP (C#) lexer styles
		//_c.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
		//_c.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
		//_c.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
		//_c.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
		//_c.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
		//_c.Styles[Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
		//_c.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
		//_c.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
		//_c.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
		//_c.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
		//_c.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
		//_c.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
		//_c.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
		//_c.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
		//_c.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
		//_c.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

		//_c.Lexer = Lexer.Cpp;

		//_c.SetKeywords(0, "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
		//_c.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");

	}

	public static Color IntToColor(int rgb)
	{
		return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
	}

	//unsafe void _InitImages()
	//{
	//	_sci_AnnotationDrawCallback = _AnnotationDrawCallback_;
	//	_c.DirectMessage(SciMod.SCI_SETANNOTATIONDRAWCALLBACK, Zero, Marshal.GetFunctionPointerForDelegate(_sci_AnnotationDrawCallback));

	//	_testIcon = Icons.GetFileIconHandle(@"Q:\", 16);
	//	//Print(_testIcon);
	//}

	//~Edit() { Icons.DestroyIconHandle(_testIcon); }


	//SciMod.Sci_AnnotationDrawCallback _sci_AnnotationDrawCallback;
	//unsafe int _AnnotationDrawCallback_(void* cbParam, ref SciMod.SCAnnotationDrawCallback c)
	//{
	//	var s = new string(c.text, 0, c.textLen, Encoding.UTF8);
	//	if(c.step == 0) return 100;
	//	PrintList(c.line, c.annotLine, s);
	//	//PrintList(c.hdc, _testIcon);
	//	DrawIconEx(c.hdc, c.rect.left, c.rect.top, _testIcon, 16, 16, 0, Zero, 3);
	//	return 100;
	//}

	//IntPtr _testIcon;
	//[DllImport("user32.dll")]
	//internal static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyWidth, uint istepIfAniCur, IntPtr hbrFlickerFreeDraw, uint diFlags);

	public void Test()
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

		_img.ClearCache();

		_c.Lines[0].AnnotationStyle = 1;
		if(Empty(_c.Lines[0].AnnotationText))
		_c.Lines[0].AnnotationText = "|Test\nAnnotations";

		//Perf.First();
		var text = _c.Text;
		//Perf.Next();
		//_img.ParseTextAndSetAnnotationsForImages(i, text);
		//Perf.NW();
		for(int i = 0, n = _c.Lines.Count; i < n; i++) {
			//for(int i=0, n=1; i<n; i++) {
			//Perf.First();
			var k = _c.Lines[i].Position;
			var len = _c.Lines[i].Length;
			//Print(text);
			//Perf.Next();
			_img.ParseTextAndSetAnnotationsForImages(i, text, k, len);
			//Perf.NW();
		}
		//for(int i=0, n=_c.Lines.Count; i<n; i++) {
		//	//for(int i=0, n=1; i<n; i++) {
		//	Perf.First();
		//	var text = _c.Lines[i].Text;
		//	//Print(text);
		//	Perf.Next();
		//	_img.ParseTextAndSetAnnotationsForImages(i, text);
		//	Perf.NW();
		//}

#endif
	}
}

static class SciCommon
{
	//public const int SCI_UPDATESCROLLBARS= 9501; //not impl
	public const int SCI_MARGINSTYLENEXT = 9502;
	//public unsafe delegate void Sci_NotifyCallback(void* cbParam, ref SCNotification n);
	//public const int SCI_SETNOTIFYCALLBACK = 9503;
	public unsafe delegate int Sci_AnnotationDrawCallback(void* cbParam, ref SCAnnotationDrawCallback c);
	public const int SCI_SETANNOTATIONDRAWCALLBACK = 9504;
	public const int SCI_ISXINMARGIN = 9506;
	//these not impl
	//public const int SC_DOCUMENT_USERDATA_OFFSET= 12;
	//public const int SC_DOCUMENT_USERDATA_SIZE= 4;

#pragma warning disable 649
	public unsafe struct SCAnnotationDrawCallback
	{
		public int step;
		public IntPtr hdc;
		public RECT rect;
		public sbyte* text;
		public int textLen, line, annotLine;
	};
#pragma warning restore 649

	public const int IndicImages = 8;
}
