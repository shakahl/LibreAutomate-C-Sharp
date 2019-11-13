//Code colors and folding.

#define CLASSIFIER

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using static Au.Controls.Sci;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class CiStyling
{
	enum EStyle
	{
		None,
		Comment,
		String,
		StringEscape,
		Number,
		Punct,
		Operator,
		Keyword,
		Namespace,
		Type,
		Function,
		Variable,
		Constant,
		Label,
		Preprocessor,
		Excluded,
		XmlDoc, //tags, CDATA, ///, etc. For simple text we use Comment.

		EndOfFunctionOrType = 30,

		//STYLE_HIDDEN=31,
		//STYLE_DEFAULT=32,
	}

	public static void InitSciDoc(SciCode doc)
	{
		var z = doc.Z;

		z.StyleForeColor((int)EStyle.Comment, 0x60a000);
		z.StyleForeColor((int)EStyle.String, 0xA07040);
		z.StyleForeColor((int)EStyle.StringEscape, 0xc0c0c0);
		//z.StyleForeColor((int)EStyle.StringEscape, 0xB776FB); //pink-purple like in VS
		//z.StyleForeColor((int)EStyle.StringEscape, 0xc0e000); //light yellow-green. Too vivid.
		z.StyleForeColor((int)EStyle.Number, 0x804000);
		//z.StyleForeColor((int)EStyle.Punct, 0x0);
		z.StyleForeColor((int)EStyle.Operator, 0x0000ff);
		z.StyleForeColor((int)EStyle.Keyword, 0x0000ff);
		z.StyleForeColor((int)EStyle.Namespace, 0x808000);
		z.StyleForeColor((int)EStyle.Type, 0x0080c0);
		z.StyleBold((int)EStyle.Function, true); //z.StyleForeColor((int)EStyle.Function, 0x0);
		z.StyleForeColor((int)EStyle.Variable, 0x204020);
		z.StyleForeColor((int)EStyle.Constant, 0x204020);
		z.StyleForeColor((int)EStyle.Label, 0xff00ff);
		z.StyleForeColor((int)EStyle.Preprocessor, 0xff8000);
		z.StyleForeColor((int)EStyle.Excluded, 0x808080);
		z.StyleForeColor((int)EStyle.XmlDoc, 0x808080);

		z.StyleHotspot((int)EStyle.EndOfFunctionOrType, true);

		doc.Call(SCI_MARKERDEFINE, SciCode.c_markerUnderline, SC_MARK_UNDERLINE); //SC_MARK_BACKGROUND
		doc.Call(SCI_MARKERSETBACK, SciCode.c_markerUnderline, 0xe0e0e0);

		z.StyleForeColor(STYLE_LINENUMBER, 0x808080);

		doc.Call(SCI_SETIDLESTYLING, SC_IDLESTYLING_TOVISIBLE); //TODO: test more
		//doc.Call(SCI_SETLEXER, (int)LexLanguage.SCLEX_CONTAINER); //default
		//doc.Call(SCI_SETLEXER, (int)LexLanguage.SCLEX_NULL);

		_InitFolding(doc);
	}

	public static void Warmup(Document document, int codeLength)
	{
#if CLASSIFIER
		Classifier.GetClassifiedSpansAsync(document, TextSpan.FromBounds(0, codeLength));
#endif
	}

	public void Update(SciCode doc) => doc.Call(SCI_COLOURISE);
	//we'll receive 2 notifications. First 0,0, then visible lines.
	//Not (SCI_COLOURISE,0,-1), because then Scintilla wants styling of offscreen lines too.

	public void SciStyleNeeded(SciCode doc, int end8)
	{
		if(end8 == 0) return;
		var z = doc.Z;
		int start8 = z.LineStartFromPos(false, doc.Call(SCI_GETENDSTYLED));

		if(!CodeInfo.IsReadyForStyling) {
			CodeInfo.ReadyForStyling += () => { if(doc.IsHandleCreated) Update(doc); };
			_StyleBlack();
			return;
		}
		Print($"style needed: {start8}-{end8}, lines {z.LineFromPos(false, start8)+1}-{z.LineFromPos(false, end8)}");

		var p1 = APerf.Create();
		var cd = new CodeInfo.Context(doc);
		if(!cd.GetDocument()) {
			_StyleBlack();
			return;
		}
		p1.Next('d');

		var semo = cd.document.GetSemanticModelAsync().Result;
		p1.Next();

		//never mind: if in arglist, we should start at the start of the invocation expression, which may be in some previous line.

		int start16 = doc.Pos16(start8), end16 = doc.Pos16(end8);

#if CLASSIFIER
		var a = Classifier.GetClassifiedSpans(semo, TextSpan.FromBounds(start16, end16), cd.document.Project.Solution.Workspace);
		p1.Next('c');
		//info: GetClassifiedSpansAsync calls GetSemanticModelAsync and GetClassifiedSpans, like here.
		//GetSemanticModelAsync+GetClassifiedSpans are slow, about 30+60 % of total time. Cannot use async, because Scintilla then sends multiple notifications.
		//Tried to implement own "GetClassifiedSpans", but slow too, often slower, because GetSymbolInfo is slow.

		//some spans are outside start16..end16, eg when in /**/ or in unclosed @" or #if
		int startMin = start16, endMax = end16;
		foreach(var v in a) {
			var ss = v.TextSpan.Start;
			if((object)v.ClassificationType == ClassificationTypeNames.ExcludedCode) ss -= 2; //move to the #if etc line
			startMin = Math.Min(startMin, ss);
			endMax = Math.Max(endMax, v.TextSpan.End);
		}
		if(startMin < start16) start8 = doc.Pos8(start16 = startMin);
		if(endMax > end16) end8 = doc.Pos8(end16 = endMax);
		//Print(start8, end8, start16, end16);

		var b = new byte[end8 - start8];
		int underlinedLine = z.LineFromPos(false, start8);
		List<_FoldPoint> f = null;

		foreach(var v in a) {
			//Print(v.ClassificationType, v.TextSpan);
			EStyle style = v.ClassificationType switch
			{
				#region
				ClassificationTypeNames.ClassName => EStyle.Type,
				ClassificationTypeNames.Comment => EStyle.Comment,
				ClassificationTypeNames.ConstantName => EStyle.Constant,
				ClassificationTypeNames.ControlKeyword => EStyle.Keyword,
				ClassificationTypeNames.DelegateName => EStyle.Type,
				ClassificationTypeNames.EnumMemberName => EStyle.Constant,
				ClassificationTypeNames.EnumName => EStyle.Type,
				ClassificationTypeNames.EventName => EStyle.Function,
				ClassificationTypeNames.ExcludedCode => EStyle.Excluded,
				ClassificationTypeNames.ExtensionMethodName => EStyle.Function,
				ClassificationTypeNames.FieldName => EStyle.Variable,
				ClassificationTypeNames.Identifier => _TryResolveMethod(),
				ClassificationTypeNames.InterfaceName => EStyle.Type,
				ClassificationTypeNames.Keyword => EStyle.Keyword,
				ClassificationTypeNames.LabelName => EStyle.Label,
				ClassificationTypeNames.LocalName => EStyle.Variable,
				ClassificationTypeNames.MethodName => EStyle.Function,
				ClassificationTypeNames.NamespaceName => EStyle.Namespace,
				ClassificationTypeNames.NumericLiteral => EStyle.Number,
				ClassificationTypeNames.Operator => EStyle.Operator,
				ClassificationTypeNames.OperatorOverloaded => EStyle.Function,
				ClassificationTypeNames.ParameterName => EStyle.Variable,
				ClassificationTypeNames.PreprocessorKeyword => EStyle.Preprocessor,
				//ClassificationTypeNames.PreprocessorText => EStyle.None,
				ClassificationTypeNames.PropertyName => EStyle.Function,
				ClassificationTypeNames.Punctuation => EStyle.Punct,
				ClassificationTypeNames.StringEscapeCharacter => EStyle.StringEscape,
				ClassificationTypeNames.StringLiteral => EStyle.String,
				ClassificationTypeNames.StructName => EStyle.Type,
				//ClassificationTypeNames.Text => EStyle.None,
				ClassificationTypeNames.VerbatimStringLiteral => EStyle.String,
				ClassificationTypeNames.TypeParameterName => EStyle.Type,
				//ClassificationTypeNames.WhiteSpace => EStyle.None,

				ClassificationTypeNames.XmlDocCommentAttributeName => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentAttributeQuotes => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentAttributeValue => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentCDataSection => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentComment => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentDelimiter => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentEntityReference => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentName => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentProcessingInstruction => EStyle.XmlDoc,
				ClassificationTypeNames.XmlDocCommentText => EStyle.Comment,

				//FUTURE: Regex. But how to apply it to ARegex?
				//ClassificationTypeNames. => EStyle.,
				_ => EStyle.None
				#endregion
			};

			EStyle _TryResolveMethod()
			{ //ClassificationTypeNames.Identifier. Possibly method name when there are errors in arguments.
				var node = semo.Root.FindNode(v.TextSpan);
				if(node?.Parent is InvocationExpressionSyntax && !semo.GetMemberGroup(node).IsDefaultOrEmpty) return EStyle.Function; //not too slow
				return EStyle.None;
			}

			if(style == EStyle.None) {
#if DEBUG
				switch(v.ClassificationType) {
				case ClassificationTypeNames.Identifier: break;
				case ClassificationTypeNames.LabelName: break;
				case ClassificationTypeNames.PreprocessorText: break;
				case ClassificationTypeNames.StaticSymbol: break;
				default: ADebug.PrintIf(!v.ClassificationType.Starts("regex"), $"<><c gray>{v.ClassificationType}, {v.TextSpan}<>"); break;
				}
#endif
				continue;
			}

			int spanStart16 = v.TextSpan.Start, spanEnd16 = v.TextSpan.End;
			int spanStart8 = doc.Pos8(spanStart16), spanEnd8 = doc.Pos8(spanEnd16);

			var code = cd.code;
			int foldLevel = 0; //relative
			if(style == EStyle.Punct && v.TextSpan.Length == 1) {
				char ch = code[spanStart16];
				if(ch == '{' || ch == '}') {
					var node = semo.Root.FindToken(spanStart16).Parent; //fast
					switch(node) {
					case BaseTypeDeclarationSyntax sy1: //class, struct, interface, enum
														//if(ch == '}') { style = EStyle.EndOfClass; foldLevel--; } else foldLevel++;
														//break;
					case AccessorListSyntax sy2: //property, event
					case BlockSyntax sy3 when node.Parent is BaseMethodDeclarationSyntax: //method
						if(ch == '}') {
							foldLevel--;
							style = EStyle.EndOfFunctionOrType;

							int line = z.LineFromPos(false, spanStart8);
							_DeleteUnderlinedLineMarkers(line);
							if(underlinedLine != line) doc.Call(SCI_MARKERADD, line, SciCode.c_markerUnderline);
							else underlinedLine++;
						} else foldLevel++;
						break;
					}
				}
			} else if(style == EStyle.Preprocessor) {
				//Print(code[spanStart16..spanEnd16]);
				if(code.Eq(spanStart16, "region")) foldLevel++;
				else if(code.Eq(spanStart16, "endregion")) foldLevel--;
			} else if(style == EStyle.Excluded) {
				//Print($"'{code[spanStart16..spanEnd16]}'");
				_AddFoldPoint(spanStart8 - 2, 1); //-2 moves to the #if etc line
				_AddFoldPoint(spanEnd8 - 2, -1); //-2 moves out of the #endif etc line
			} else if(style == EStyle.Comment) {
				//Print($"'{code[spanStart16..spanEnd16]}'");
				if(code.Eq(spanStart16, "//-{")) {
					foldLevel++;
				} else if(code.Eq(spanStart16, "//-}")) {
					for(int j = spanStart16 + 3; j < code.Length && code[j] == '}'; j++) foldLevel--;
				} else if(code.Eq(spanStart16, "/*") && code.Eq(spanEnd16 - 2, "*/") && code.AsSpan((spanStart16, spanEnd16)).Contains('\n')) {
					_AddFoldPoint(spanStart8, 1);
					_AddFoldPoint(spanEnd8, -1);
				}
			}

			if(foldLevel != 0) _AddFoldPoint(spanStart8, foldLevel);

			void _AddFoldPoint(int pos, int level) => (f ??= new List<_FoldPoint>()).Add(new _FoldPoint { pos = pos, level = level });

			for(int i = spanStart8; i < spanEnd8; i++) b[i - start8] = (byte)style;
		}

		_DeleteUnderlinedLineMarkers(z.LineFromPos(false, end8));

		void _DeleteUnderlinedLineMarkers(int beforeLine)
		{
			if((uint)underlinedLine > beforeLine) return;
			for(; ; ) {
				underlinedLine = doc.Call(SCI_MARKERNEXT, underlinedLine, 1 << SciCode.c_markerUnderline);
				if((uint)underlinedLine >= beforeLine) break;
				doc.Call(SCI_MARKERDELETE, underlinedLine++, SciCode.c_markerUnderline);
			}
		}

#else

		doc.Call(SCI_STARTSTYLING, start8);
		var b = new byte[end8 - start8];

		//foreach(var d in semo.GetDiagnostics()) { //very slow too
		//	if(d.Severity != DiagnosticSeverity.Error) continue;
		//	Print(d);
		//}
		//p1.Next('D');

		//var root = semo.SyntaxTree.GetRoot();
		var root = semo.SyntaxTree.GetCompilationUnitRoot();
		string code = cd.code;

		foreach(var t in root.DescendantTokens()) {
			var span = t.Span;
			if(span.Start < start16) continue;
			if(span.End > end16) break;
			if(span.IsEmpty) continue;

			//string color = "green";

			EStyle style = (EStyle)~0;
			var tkind = t.Kind();
			char ch = code[span.Start];
			bool isIdent = SyntaxFacts.IsIdentifierStartCharacter(ch);
			if(isIdent) {
				if(tkind == SyntaxKind.IdentifierToken) {
					var node = t.Parent;
					//Print(node.Kind());
					switch(node.Kind()) {
					case SyntaxKind.IdentifierName:
						var p2 = APerf.Create();
						var sym = semo.GetSymbolInfo(node).Symbol; //very slow
						p2.Next();
						if(sym == null && node.Parent is InvocationExpressionSyntax ies) {
							sym = semo.GetMemberGroup(node).FirstOrDefault(); //fast
							p2.Next('M');
						}
						Print(p2.ToString(), sym?.Kind, sym);
						if(sym != null) {
							//Print(sym, sym.Kind);
							switch(sym.Kind) {
							case SymbolKind.Namespace: style = EStyle.Namespace; break;
							case SymbolKind.NamedType: case SymbolKind.TypeParameter: style = EStyle.Type; break;
							case SymbolKind.Method: case SymbolKind.Property: case SymbolKind.Event: style = EStyle.Function; break;
							case SymbolKind.Local: case SymbolKind.Parameter: case SymbolKind.Field: case SymbolKind.RangeVariable: style = EStyle.Variable; break;
							}
						} else if(ch >= 'a' && ch <= 'z' && span.Length >= 2) {
							//SyntaxFacts.GetContextualKeywordKind(code[span.Start..span.End]) //no: need Substring, not full list, etc

						}
						break;
					case SyntaxKind.ClassDeclaration:
					case SyntaxKind.DelegateDeclaration:
					case SyntaxKind.EnumDeclaration:
					case SyntaxKind.InterfaceDeclaration:
					case SyntaxKind.StructDeclaration:
						style = EStyle.Type;
						break;
					case SyntaxKind.MethodDeclaration:
					case SyntaxKind.PropertyDeclaration:
					case SyntaxKind.ConstructorDeclaration:
						style = EStyle.Function;
						break;
					case SyntaxKind.VariableDeclarator:
					case SyntaxKind.Parameter:
						style = EStyle.Variable;
						break;
					}
				} else if(SyntaxFacts.IsReservedKeyword(tkind)) {
					style = EStyle.Keyword; //not IsKeywordKind because for contextual keywords we get SyntaxKind.IdentifierToken
				} else {
					switch(tkind) {
					//case :
					//	break;
					}
				}

				//if(style == (EStyle)~0) {
				//	style = EStyle.None;
				//	color = "#c08000";
				//}
				//Print($"<><c {color}>{t.Kind()}<>, {t.ValueText}");
			} else {
				//Print(SyntaxFacts.IsPunctuation(tkind)); //includes operators

				switch(tkind) {
				case SyntaxKind.StringLiteralToken:
				case SyntaxKind.CharacterLiteralToken:
					style = EStyle.String;
					break;
				case SyntaxKind.NumericLiteralToken:
					style = EStyle.Number;
					break;
				case SyntaxKind.OpenParenToken:
				case SyntaxKind.OpenBraceToken:
				case SyntaxKind.OpenBracketToken:
				case SyntaxKind.CloseParenToken:
				case SyntaxKind.CloseBraceToken:
				case SyntaxKind.CloseBracketToken:
				case SyntaxKind.CommaToken:
				case SyntaxKind.ColonToken:
				case SyntaxKind.SemicolonToken:
					style = EStyle.Punct;
					break;
				}

				if(style == (EStyle)~0) {
					style = EStyle.None;
					//color = "#c08000";
				}
				//Print($"<><c {color}>{t.Kind()}<>, {t.ValueText}");
			}

			if(style == (EStyle)~0) {
				style = EStyle.None;
				//color = "#c08000";
			}
			//Print($"<><c {color}>{t.Kind()}<>, {SyntaxFacts.IsPunctuation(tkind)}, {t.ValueText}");

			for(int i = span.Start; i < span.End; i++) b[i - start8] = (byte)style;
		}

#endif

		p1.Next();
		doc.Call(SCI_STARTSTYLING, start8);
		unsafe { fixed(byte* bp = b) doc.Call(SCI_SETSTYLINGEX, b.Length, bp); }
		p1.Next();

		_Fold(doc, start8, end8, f);

		//p1.NW('s');

		void _StyleBlack()
		{
			doc.Call(SCI_STARTSTYLING, start8);
			doc.Call(SCI_SETSTYLING, end8 - start8, (int)EStyle.None);
		}
	}

	void _Fold(SciCode doc, int start8, int end8, List<_FoldPoint> a)
	{
		var z = doc.Z;
		int line = z.LineFromPos(false, start8);
		int lineTo = z.LineFromPos(false, end8); if(end8 > z.LineStart(false, lineTo)) lineTo++;
		//Print(line, lineTo, z.LineFromPos(false, end8));
		int levelCurrent = line == 0 ? SC_FOLDLEVELBASE : doc.Call(SCI_GETFOLDLEVEL, line - 1) >> 16, levelNext = levelCurrent; //like in LexCPP.cxx
		for(int i = 0; line < lineTo; line++) {
			int eol = z.LineEnd(false, line, withRN: true);
			if(a != null) for(; i < a.Count && a[i].pos < eol; i++) levelNext += a[i].level;
			levelNext = Math.Max(levelNext, SC_FOLDLEVELBASE);
			int lev = levelCurrent | levelNext << 16;
			if(levelNext > levelCurrent) lev |= SC_FOLDLEVELHEADERFLAG;
			//Print(line+1, (uint)lev, (uint)doc.Call(SCI_GETFOLDLEVEL, line));
			if(lev != doc.Call(SCI_GETFOLDLEVEL, line)) doc.Call(SCI_SETFOLDLEVEL, line, lev);
			levelCurrent = levelNext;
		}
		doc.ZFoldingDone();
	}

	struct _FoldPoint { public int pos, level; }

	static void _InitFolding(SciCode doc)
	{
		const int foldMrgin = SciCode.c_marginFold;
		doc.Call(SCI_SETMARGINTYPEN, foldMrgin, SC_MARGIN_SYMBOL);
		doc.Call(SCI_SETMARGINMASKN, foldMrgin, SC_MASK_FOLDERS);
		doc.Call(SCI_SETMARGINSENSITIVEN, foldMrgin, 1);

		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDEROPEN, SC_MARK_BOXMINUS);
		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDER, SC_MARK_BOXPLUS);
		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDERSUB, SC_MARK_VLINE);
		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDERTAIL, SC_MARK_LCORNER);
		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDEREND, SC_MARK_BOXPLUSCONNECTED);
		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDEROPENMID, SC_MARK_BOXMINUSCONNECTED);
		doc.Call(SCI_MARKERDEFINE, SC_MARKNUM_FOLDERMIDTAIL, SC_MARK_TCORNER);
		for(int i = 25; i < 32; i++) {
			doc.Call(SCI_MARKERSETFORE, i, 0xffffff);
			doc.Call(SCI_MARKERSETBACK, i, 0x808080);
			doc.Call(SCI_MARKERSETBACKSELECTED, i, i == SC_MARKNUM_FOLDER ? 0xFF : 0x808080);
		}
		doc.Call(SCI_MARKERENABLEHIGHLIGHT, 1);

		doc.Call(SCI_SETAUTOMATICFOLD, SC_AUTOMATICFOLD_SHOW //show hidden lines when header line deleted
			| SC_AUTOMATICFOLD_CHANGE); //show hidden lines when header line modified like '#region' -> '//#region'
		doc.Call(SCI_SETFOLDFLAGS, SC_FOLDFLAG_LINEAFTER_CONTRACTED);
		doc.Call(SCI_FOLDDISPLAYTEXTSETSTYLE, SC_FOLDDISPLAYTEXT_STANDARD);
		doc.Z.StyleForeColor(STYLE_FOLDDISPLAYTEXT, 0x808080);

		doc.Call(SCI_SETMARGINCURSORN, foldMrgin, SC_CURSORARROW);

		int wid = doc.Call(SCI_TEXTHEIGHT) - 4;
		doc.Z.MarginWidth(foldMrgin, Math.Max(wid, 12));
	}
}

partial class SciCode
{
	bool _FoldingOnMarginClick(bool? fold, int startPos)
	{
		int line = Call(SCI_LINEFROMPOSITION, startPos);
		if(0 == (Call(SCI_GETFOLDLEVEL, line) & SC_FOLDLEVELHEADERFLAG)) return false;
		bool isExpanded = 0 != Call(SCI_GETFOLDEXPANDED, line);
		if(fold.HasValue && fold.GetValueOrDefault() != isExpanded) return false;
		if(isExpanded) {
			_FoldingFoldLine(line);
			//move caret out of contracted region
			int pos = Z.CurrentPos8;
			if(pos > startPos) {
				int i = Z.LineEnd(false, Call(SCI_GETLASTCHILD, line, -1));
				if(pos <= i) Z.CurrentPos8 = startPos;
			}
		} else {
			Call(SCI_FOLDLINE, line, 1);
		}
		return true;
	}

	void _FoldingFoldLine(int line)
	{
#if false
		Call(SCI_FOLDLINE, line);
#else
		string s = Z.LineText(line), s2 = "";
		for(int i = 0; i < s.Length; i++) {
			char c = s[i];
			if(c == '{') { s2 = "... }"; break; }
			if(c == '/' && i < s.Length - 1) {
				c = s[i + 1];
				if(c == '*') break;
				if(i < s.Length - 3 && c == '/' && s[i + 2] == '-' && s[i + 3] == '{') break;
			}
		}
		//quite slow. At startup ~250 mcs. The above code is fast.
		if(s2.Length == 0) Call(SCI_FOLDLINE, line); //slightly faster
		else Z.SetString(SCI_TOGGLEFOLDSHOWTEXT, line, s2);
#endif
	}

	public void ZFoldingDone()
	{
		if(_openState == 2) return;
		bool newFile = _openState == 1;
		_openState = 2;
		if(newFile) {
			//fold boilerplate code
			var code = Text;
			int i = code.Find("//-{\r\nusing Au;");
			if(i >= 0) {
				i = Z.LineFromPos(true, i);
				if(0 != (SC_FOLDLEVELHEADERFLAG & Call(SCI_GETFOLDLEVEL, i))) Call(SCI_FOLDCHILDREN, i);
			}
			//set caret below boilerplate
			var s1 = "//-}}}\r\n\r\n";
			i = code.Find(s1);
			if(i >= 0) Z.CurrentPos16 = i + s1.Length;
		} else {
			//restore saved folding and markers
			var db = Program.Model.DB; if(db == null) return;
			try {
				using var p = db.Statement("SELECT lines FROM _editor WHERE id=?", ZFile.Id);
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
			catch(SLException ex) { ADebug.Print(ex); }
		}
	}
	byte _openState; //0 old file, 1 new file, 2 folding done

	/// <summary>
	/// Saves folding, markers etc in database.
	/// </summary>
	internal void ZSaveEditorData()
	{
		var db = Program.Model.DB; if(db == null) return;
		var a = new List<int>();
		_GetLineDataToSave(c_markerBookmark, a);
		_GetLineDataToSave(c_markerBreakpoint, a);
		_GetLineDataToSave(31, a);
		var hash = _Hash(a);
		if(hash != _savedMD5) {
			//Print("changed");
			try {
				if(a.Count == 0) {
					db.Execute("DELETE FROM _editor WHERE id=?", ZFile.Id);
				} else {
					using var p = db.Statement("REPLACE INTO _editor (id,lines) VALUES (?,?)");
					p.Bind(1, ZFile.Id).Bind(2, a).Step();
				}
				_savedMD5 = hash;
			}
			catch(SLException ex) { ADebug.Print(ex); }
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
				else i = Call(SCI_MARKERNEXT, i, 1 << marker);
				if((uint)i > 0x7FFFFFF) break; //-1 if no more; ensure we have 5 high-order bits for marker; max 134 M lines.
				a.Add(i | (marker << 27));
			}
		}
	}

	Au.Util.AHash.MD5Result _savedMD5;

	static Au.Util.AHash.MD5Result _Hash(List<int> a)
	{
		if(a.Count == 0) return default;
		Au.Util.AHash.MD5 md5 = default;
		foreach(var v in a) md5.Add(v);
		return md5.Hash;
	}
}
