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
using System.Linq;

using Au;
using Au.Types;
using Au.More;
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using acc = Microsoft.CodeAnalysis.Accessibility;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

/// <summary>
/// Inserts various code in code editor. With correct indentation etc.
/// Some functions can insert in other controls too.
/// </summary>
static class InsertCode
{
	/// <summary>
	/// Inserts one or more statements at current line.
	/// If editor is null or readonly, prints in output.
	/// Async if called from non-main thread.
	/// </summary>
	/// <param name="s">Text without "\r\n" at the end.</param>
	/// <param name="goToPercent">If contains '%', removes it and moves caret there.</param>
	public static void Statements(string s, bool goToPercent = false) {
		if (Environment.CurrentManagedThreadId != 1) App.Wmain.Dispatcher.InvokeAsync(() => _Action(s, goToPercent)); else _Action(s, goToPercent);
		static void _Action(string s, bool goToPercent) {
			var d = Panels.Editor.ZActiveDoc;
			if (d == null || d.zIsReadonly) {
				print.it(s);
			} else {
				d.Focus();
				int start = d.zLineStartFromPos(false, d.zCurrentPos8);
				int indent = d.zLineIndentationFromPos(false, start);
				if (indent == 0) {
					s += "\r\n";
				} else {
					var b = new StringBuilder();
					foreach (var v in s.Lines()) b.Append('\t', indent).AppendLine(v);
					s = b.ToString();
				}

				int i = -1;
				if (goToPercent) {
					i = s.IndexOf('%');
					if (i >= 0) s = s.Remove(i, 1);
				}

				CodeInfo.Pasting(d);
				d.zReplaceSel(false, start, s);
				if (i >= 0) d.zGoToPos(true, start + i);
			}
		}
	}

	/// <summary>
	/// Inserts text in code editor at current position, not as new line, replaces selection.
	/// If editor is null or readonly, does nothing.
	/// </summary>
	/// <param name="s">If contains '%', removes it and moves caret there.</param>
	public static void TextSimply(string s) {
		Debug.Assert(Environment.CurrentManagedThreadId == 1);
		var d = Panels.Editor.ZActiveDoc;
		if (d == null || d.zIsReadonly) return;
		TextSimplyInControl(d, s);
	}

	/// <summary>
	/// Inserts text in specified or focused control.
	/// At current position, not as new line, replaces selection.
	/// </summary>
	/// <param name="c">If null, uses the focused control, else sets focus.</param>
	/// <param name="s">If contains '%', removes it and moves caret there. Alternatively use '\b', then does not touch '%'.</param>
	public static void TextSimplyInControl(FrameworkElement c, string s) {
		Debug.Assert(Environment.CurrentManagedThreadId == 1);
		if (c == null) {
			c = App.FocusedElement;
			if (c == null) return;
		} else {
			if (c != App.FocusedElement) //be careful with HwndHost
				c.Focus();
		}

		int i = s.IndexOf('\b');
		if (i < 0) i = s.IndexOf('%');
		if (i >= 0) {
			Debug.Assert(!s.Contains('\r'));
			s = s.Remove(i, 1);
			i = s.Length - i;
		}

		if (c is KScintilla sci) {
			if (sci.zIsReadonly) return;
			sci.zReplaceSel(s);
			while (i-- > 0) sci.Call(Sci.SCI_CHARLEFT);
		} else if (c is TextBox tb) {
			if (tb.IsReadOnly) return;
			tb.SelectedText = s;
			tb.CaretIndex = tb.SelectionStart + tb.SelectionLength - Math.Max(i, 0);
		} else {
			Debug_.Print(c);
			Task.Run(() => {
				var k = new keys(null);
				k.AddText(s);
				if (i > 0) k.AddKey(KKey.Left).AddRepeat(i);
				k.SendIt();
			});
		}
	}

	/// <summary>
	/// Inserts code 'using ns;\r\n' in correct place in editor text, unless it is already exists.
	/// Returns true if inserted.
	/// </summary>
	/// <param name="ns">Namespace, eg "System.Diagnostics". Can be multiple, separated with semicolon (can be whitespce around).</param>
	public static bool UsingDirective(string ns) {
		Debug.Assert(Environment.CurrentManagedThreadId == 1);
		if (!CodeInfo.GetContextAndDocument(out var k, 0, metaToo: true)) return false;
		var namespaces = ns.Split(';', StringSplitOptions.TrimEntries);
		var (_, end) = _FindUsings(k, namespaces);
		if (!namespaces.Any(o => o != null)) return false;
		var doc = k.sciDoc;
		//doc.Select(true, start, end);

		var b = new StringBuilder();
		if (end > 0 && k.code[end - 1] != '\n') b.AppendLine();
		foreach (var v in namespaces) {
			if (v != null) b.Append("using ").Append(v).AppendLine(";");
		}

		doc.zInsertText(true, end, b.ToString(), addUndoPointAfter: true, restoreFolding: true);

		return true;

		//tested: CompilationUnitSyntax.AddUsings isn't better than this code. Does not skip existing. Does not add newlines. Does not skip comments etc. Did't test #directives.
	}

	/// <summary>
	/// Finds start and end of using directives.
	/// If no usings, sets start = end = where new using directives can be inserted: 0 or end of extern aliases or #directives or meta or doc comments or 1 comments line.
	/// Sets end = the start of next line if possible.
	/// If namespaces!=null, clears existing namespaces in it (sets =null).
	/// Ignores using directives within namespace.
	/// </summary>
	static (int start, int end) _FindUsings(CodeInfo.Context k, string[] namespaces = null) {
		int start = -1, end = -1, end2 = -1;
		var cu = k.document.GetSyntaxRootAsync().Result as CompilationUnitSyntax;
		//print.it(cu.Externs, cu.Usings, cu.AttributeLists, cu.Members);
		if (cu.Usings.Any()) {
			if (namespaces != null) {
				foreach (var u in cu.Usings) {
					int i = Array.IndexOf(namespaces, u.Name.ToString());
					if (i >= 0) namespaces[i] = null;
				}
			}
			start = cu.Usings[0].SpanStart;
			end = cu.Usings[^1].FullSpan.End;
		} else if (cu.Externs.Any()) {
			end2 = cu.Externs[^1].FullSpan.End;
		}
		if (start < 0) {
			if (end2 < 0) foreach (var v in cu.GetLeadingTrivia()) if (v.IsDirective) end2 = v.FullSpan.End; //skip directives
			if (end2 < 0) {
				end2 = k.meta.end; //skip meta
				if(end2 == 0) if (k.code.RegexMatch(@"(\s*///.*\R)+", 0, out RXGroup g1, RXFlags.ANCHORED, end2..)) end2 = g1.End; //skip ///comments
				if (k.code.RegexMatch(@"\s*//.+\R", 0, out RXGroup g2, RXFlags.ANCHORED, end2..)) end2 = g2.End; //skip 1 line of //comments, because usually it is //. for folding
			}
			start = end = end2;
		}
		return (start, end);

		//SHOULDDO: detect existing usings in namespace.
	}

	/// <summary>
	/// Called from CiCompletion._ShowList on char '/'. If need, inserts XML doc comment with empty summary, param and returns tags.
	/// </summary>
	public static void DocComment(CodeInfo.Context cd) {
		int pos = cd.pos16;
		string code = cd.code;
		SciCode doc = cd.sciDoc;

		if (0 == code.Eq(pos - 3, false, "///\r", "///\n") || !IsLineStart(code, pos - 3)) return;

		var root = cd.document.GetSyntaxRootAsync().Result;
		var node = root.FindToken(pos).Parent;
		var start = node.SpanStart;
		if (start < pos) return;

		if (node is not MemberDeclarationSyntax) { //can be eg func return type (if no public etc) or attribute
			node = node.Parent;
			if (node is not MemberDeclarationSyntax || node.SpanStart != start) return;
		}

		//already has doc comment?
		foreach (var v in node.GetLeadingTrivia()) {
			if (v.IsDocumentationCommentTrivia) { //singleline (preceded by ///) or multiline (preceded by /**)
				var span = v.Span;
				if (span.Start != pos || span.Length > 2) return; //when single ///, span includes only newline after ///
			}
		}
		//print.it(pos);
		//CiUtil.PrintNode(node);

		string s = @" <summary>
/// 
/// </summary>";

		if (node is BaseMethodDeclarationSyntax md) //method, ctor
			s += CiUtil.FormatSignatureXmlDoc(md, code);

		s = IndentStringForInsert(s, doc, pos);

		doc.zInsertText(true, pos, s, true, true);
		doc.zGoToPos(true, pos + s.Find("/ ") + 2);
	}

	/// <summary>
	/// Returns true if pos in string is at a line start + any number of spaces and tabs.
	/// </summary>
	public static bool IsLineStart(string s, int pos) {
		int i = pos; while (--i >= 0 && (s[i] == ' ' || s[i] == '\t')) { }
		return i < 0 || s[i] == '\n';
	}

	/// <summary>
	/// Returns string with same indentation as of the document line from pos.
	/// </summary>
	public static string IndentStringForInsert(string s, SciCode doc, int pos) {
		int indent = doc.zLineIndentationFromPos(true, pos);
		if (indent > 0) s = s.RegexReplace(@"(?<=\n)", new string('\t', indent));
		return s;
	}

	//TODO: now optional parameters enclosed in [], like public override void Return(T[] array, [bool clearArray = false])
	public static void ImplementInterfaceOrAbstractClass(bool explicitly, int position = -1) {
		if (!CodeInfo.GetContextAndDocument(out var cd, position)) return;
		var semo = cd.document.GetSemanticModelAsync().Result;
		var node = semo.Root.FindToken(cd.pos16).Parent;
		//CiUtil.PrintNode(node);

		bool haveBaseType = false;
		for (var n = node; n != null; n = n.Parent) {
			//print.it(n.Kind());
			if (n is BaseTypeSyntax bts) {
				node = bts.Type;
				haveBaseType = true;
			} else if (n is ClassDeclarationSyntax cds) {
				if (!haveBaseType) try { node = cds.BaseList.Types[0].Type; } catch { return; }
				position = cds.CloseBraceToken.Span.Start;
				goto g1;
			}
		}
		return;
		g1:

		var baseType = semo.GetTypeInfo(node).Type as INamedTypeSymbol;
		if (baseType == null) return;
		bool isInterface = false;
		switch (baseType.TypeKind) {
		case TypeKind.Interface: isInterface = true; break;
		case TypeKind.Class when baseType.IsAbstract: break;
		default: return;
		}

		var b = new StringBuilder();
		var format = CiText.s_symbolFullFormat;
		var formatExp = format.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeRef);

		b.Append("\r\n#region ").Append(baseType.ToMinimalDisplayString(semo, position, CiText.s_symbolFullFormat));

		if (isInterface) {
			_Base(baseType, explicitly);
			foreach (var bi in baseType.AllInterfaces) {
				_Base(bi,
					explicitly || (baseType.IsGenericType && !bi.IsGenericType) //eg public IEnumerable<T> and explicit IEnumerable
					);
			}
		} else {
			for (; baseType != null && baseType.IsAbstract; baseType = baseType.BaseType) {
				_Base(baseType, false);
			}
		}

		b.AppendLine("\r\n\r\n#endregion");

		void _Base(INamedTypeSymbol type, bool explicitly) {
			foreach (var v in type.GetMembers()) {
				//print.it(v, v.IsStatic, v.GetType().GetInterfaces());
				bool isAbstract = v.IsAbstract;
				if (!isAbstract && !isInterface) continue;
				if (v.IsStatic) continue;
				bool expl = explicitly || (isInterface && v.DeclaredAccessibility != acc.Public);

				string append = null;
				switch (v) {
				case IMethodSymbol ims when ims.MethodKind == MethodKind.Ordinary:
					append = ims.ReturnsVoid ? @" {
	
}" : @" {
	
	return default;
}";
					break;
				case IPropertySymbol ips:
					if (!expl && isInterface) {
						if (ips.GetMethod != null && ips.GetMethod.DeclaredAccessibility != acc.Public) expl = true;
						if (ips.SetMethod != null && ips.SetMethod.DeclaredAccessibility != acc.Public) expl = true;
					}
					break;
				case IEventSymbol:
					append = !expl ? ";" : @" {
	add {  }
	remove {  }
}";
					break;
				default:
					continue;
				}

				//if(null != classType.FindImplementationForInterfaceMember(v)) continue; //never mind

				b.AppendLine("\r\n");
				if (isInterface) {
					if (!isAbstract) b.AppendLine("//has default implementation");
					if (!expl) b.Append("public ");
				} else {
					b.Append(v.DeclaredAccessibility switch { acc.Public => "public", acc.Internal => "internal", acc.Protected => "protected", acc.ProtectedOrInternal => "protected internal", acc.ProtectedAndInternal => "private protected", _ => "" });
					b.Append(" override ");
				}
				b.Append(v.ToMinimalDisplayString(semo, position, expl ? formatExp : format)).Append(append);
			}
		}

		var text = b.ToString();
		text = text.Replace("] { get; set; }", @"] {
	get { return default; }
	set {  }
}"); //indexers
		text = text.RegexReplace(@"[^\]] \{\K set; \}", @"
	set {  }
}"); //write-only properties

		//print.it(text);
		//clipboard.text = text;

		cd.sciDoc.zInsertText(true, position, text, addUndoPointAfter: true);
		cd.sciDoc.zGoToPos(true, position);

		//tested: Microsoft.CodeAnalysis.CSharp.ImplementInterface.CSharpImplementInterfaceService works but the result is badly formatted (without spaces, etc). Internal, undocumented.
	}

	public static void ConvertTlsScriptToClass() {
		//SHOULDDO: use CompilationUnitSyntax.Usings etc, like in _FindUsings.
		if (!CodeInfo.GetContextAndDocument(out var cd) /*|| !cd.sciDoc.ZFile.IsScript*/) return;
		var root = cd.document.GetSyntaxRootAsync().Result;
		int start = -1, end = cd.code.Length;
		foreach (var v in root.ChildNodes()) {
			switch (v) {
			case UsingDirectiveSyntax or ExternAliasDirectiveSyntax:
			case AttributeListSyntax als when als.Target.Identifier.Text is "module" or "assembly":
				break;
			case GlobalStatementSyntax:
				if (start < 0) start = v.FullSpan.Start;
				break;
			default:
				//CiUtil.PrintNode(v);
				end = v.FullSpan.Start;
				goto g1;
			}
		}
		g1:
		if (start < 0) start = end;

		using var undo = new KScintilla.UndoAction(cd.sciDoc);
		cd.sciDoc.zInsertText(true, end, "\r\n}\r\n}\r\n");
		string s = "class Script { static void Main(string[] a) => new Script(a); Script(string[] args) { //;;\r\n";
		//string s = "new K(args); class K { public K(string[] args) { //;;\r\n";
		cd.sciDoc.zInsertText(true, start, s);
	}
}
