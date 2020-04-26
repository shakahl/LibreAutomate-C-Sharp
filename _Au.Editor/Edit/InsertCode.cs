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
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using acc = Microsoft.CodeAnalysis.Accessibility;

/// <summary>
/// Inserts various code in code editor. With correct indentation etc.
/// Some functions can insert in other controls too.
/// </summary>
static class InsertCode
{
	/// <summary>
	/// Inserts one or more statements at current line.
	/// If editor is null or readonly, prints in output.
	/// </summary>
	/// <param name="s">Text without "\r\n" at the end.</param>
	/// <param name="goToPercent">If contains '%', removes it and moves caret there.</param>
	public static void Statements(string s, bool goToPercent = false)
	{
		var d = Panels.Editor.ZActiveDoc;
		if(d == null || d.Z.IsReadonly) {
			AOutput.Write(s);
		} else {
			var z = d.Z;
			d.Focus();
			int start = z.LineStartFromPos(false, z.CurrentPos8);
			int indent = z.LineIndentationFromPos(false, start);
			if(indent == 0) {
				s += "\r\n";
			} else {
				var b = new StringBuilder();
				foreach(var v in s.SegLines()) b.Append('\t', indent).AppendLine(v);
				s = b.ToString();
			}

			int i = -1;
			if(goToPercent) {
				i = s.IndexOf('%');
				if(i >= 0) s = s.Remove(i, 1);
			}

			z.ReplaceSel(false, start, s);
			if(i >= 0) z.GoToPos(true, start + i);
		}
	}

	/// <summary>
	/// Inserts text at current position, not as new line, replaces selection.
	/// If editor is null or readonly, does nothing.
	/// </summary>
	/// <param name="s">If contains '%', removes it and moves caret there.</param>
	public static void TextSimply(string s)
	{
		var d = Panels.Editor.ZActiveDoc;
		if(d == null || d.Z.IsReadonly) return;
		TextSimplyInControl(d, s);
	}

	/// <summary>
	/// Inserts text in specified or focused control.
	/// At current position, not as new line, replaces selection.
	/// </summary>
	/// <param name="c">If null, uses the focused control, else sets focus.</param>
	/// <param name="s">If contains '%', removes it and moves caret there.</param>
	public static void TextSimplyInControl(System.Windows.Forms.Control c, string s)
	{
		if(c == null) {
			c = AWnd.ThisThread.FocusedControl;
			if(c == null) return;
		} else c.Focus();

		int i = s.IndexOf('%');
		if(i >= 0) {
			Debug.Assert(!s.Contains('\r'));
			s = s.Remove(i, 1);
			i = s.Length - i;
		}

		if(c is AuScintilla sci) {
			if(sci.Z.IsReadonly) return;
			sci.Z.ReplaceSel(s);
			while(i-- > 0) sci.Call(Sci.SCI_CHARLEFT);
		} else {
			Task.Run(() => {
				var k = new AKeys(null);
				k.AddText(s);
				if(i > 0) k.AddKey(KKey.Left).AddRepeat(i);
				k.Send();
			});
		}
	}

	/// <summary>
	/// Inserts code 'using ns;\r\n' in correct place in editor text, unless it is already exists.
	/// Returns true if inserted.
	/// </summary>
	/// <param name="ns">Namespace, eg "System.Diagnostics".</param>
	public static bool UsingDirective(string ns)
	{
		if(!CodeInfo.GetContextAndDocument(out var k, 0, metaToo: true)) return false;
		if(_FindUsings(k, out int end, ns) < 0) return false;
		var doc = k.sciDoc;
		//doc.Z.Select(true, start, end);

		var b = new StringBuilder();
		if(end > 0 && k.code[end - 1] != '\n') b.AppendLine();
		b.Append("using ").Append(ns).AppendLine(";");

		int line = doc.Z.LineFromPos(true, end), foldLine = (0 == doc.Call(Sci.SCI_GETLINEVISIBLE, line)) ? doc.Call(Sci.SCI_GETFOLDPARENT, line) : -1;
		doc.Z.InsertText(true, end, b.ToString(), addUndoPoint: true);
		if(foldLine >= 0) doc.Call(Sci.SCI_FOLDLINE, foldLine); //InsertText expands folding

		return true;
	}

	/// <summary>
	/// Finds start and end of using directives. Returns start.
	/// If no usings, sets start = end = where new using directives can be inserted: 0 or end of meta or extern aliases or preprocessor directives.
	/// Sets end = the start of next line if possible.
	/// If ifNoNamespace!=null, returns -1 if 'using ifNoNamespace;' exists.
	/// </summary>
	static int _FindUsings(in CodeInfo.Context k, out int end, string ifNoNamespace = null)
	{
		end = -1;
		int start = -1, end2 = -1;
		var root = k.document.GetSyntaxRootAsync().Result;
		foreach(var v in root.ChildNodes()) {
			switch(v) {
			case UsingDirectiveSyntax u:
				if(ifNoNamespace != null && ifNoNamespace == u.Name.ToString()) return -1;
				if(start < 0) start = v.SpanStart;
				end = v.FullSpan.End;
				break;
			case ExternAliasDirectiveSyntax _:
				end2 = v.FullSpan.End;
				break;
			default: goto gr;
			}
			//CiUtil.PrintNode(v);
		}
		gr:
		if(start < 0) {
			if(end2 < 0) foreach(var v in root.GetLeadingTrivia()) if(v.IsDirective) end2 = v.FullSpan.End;
			if(end2 < 0) {
				end2 = k.metaEnd;
				if(k.code.RegexMatch(@"\s*//.+\R", 0, out RXGroup g, RXFlags.ANCHORED, end2..)) end2 = g.End;
			}
			start = end = end2;
		}
		return start;
	}

	public static void ImplementInterfaceOrAbstractClass(bool explicitly, int position = -1)
	{
		if(!CodeInfo.GetContextAndDocument(out var cd, position)) return;
		var semo = cd.document.GetSemanticModelAsync().Result;
		var node = semo.Root.FindToken(cd.pos16).Parent;
		//CiUtil.PrintNode(node);

		bool haveBaseType = false;
		for(var n = node; n != null; n = n.Parent) {
			//AOutput.Write(n.Kind());
			if(n is BaseTypeSyntax bts) {
				node = bts.Type;
				haveBaseType = true;
			} else if(n is ClassDeclarationSyntax cds) {
				if(!haveBaseType) try { node = cds.BaseList.Types[0].Type; } catch { return; }
				position = cds.CloseBraceToken.Span.Start;
				goto g1;
			}
		}
		return;
		g1:

		var baseType = semo.GetTypeInfo(node).Type as INamedTypeSymbol;
		if(baseType == null) return;
		bool isInterface = false;
		switch(baseType.TypeKind) {
		case TypeKind.Interface: isInterface = true; break;
		case TypeKind.Class when baseType.IsAbstract: break;
		default: return;
		}

		var b = new StringBuilder();
		var format = CiHtml.s_symbolFullFormat;
		var formatExp = format.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeRef);

		b.Append("\r\n#region ").Append(baseType.ToMinimalDisplayString(semo, position, CiHtml.s_symbolFullFormat));

		if(isInterface) {
			_Base(baseType, explicitly);
			foreach(var bi in baseType.AllInterfaces) {
				_Base(bi,
					explicitly || (baseType.IsGenericType && !bi.IsGenericType) //eg public IEnumerable<T> and explicit IEnumerable
					);
			}
		} else {
			for(; baseType != null && baseType.IsAbstract; baseType = baseType.BaseType) {
				_Base(baseType, false);
			}
		}

		b.AppendLine("\r\n\r\n#endregion");

		void _Base(INamedTypeSymbol type, bool explicitly)
		{
			foreach(var v in type.GetMembers()) {
				//AOutput.Write(v, v.IsStatic, v.GetType().GetInterfaces());
				bool isAbstract = v.IsAbstract;
				if(!isAbstract && !isInterface) continue;
				if(v.IsStatic) continue;
				bool expl = explicitly || (isInterface && v.DeclaredAccessibility != acc.Public);

				string append = null;
				switch(v) {
				case IMethodSymbol ims when ims.MethodKind == MethodKind.Ordinary:
					append = ims.ReturnsVoid ? @" {
	
}" : @" {
	
	return default;
}";
					break;
				case IPropertySymbol ips:
					if(!expl && isInterface) {
						if(ips.GetMethod != null && ips.GetMethod.DeclaredAccessibility != acc.Public) expl = true;
						if(ips.SetMethod != null && ips.SetMethod.DeclaredAccessibility != acc.Public) expl = true;
					}
					break;
				case IEventSymbol _:
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
				if(isInterface) {
					if(!isAbstract) b.AppendLine("//has default implementation");
					if(!expl) b.Append("public ");
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

		//AOutput.Write(text);
		//AClipboard.Text = text;

		cd.sciDoc.Z.InsertText(true, position, text, addUndoPoint: true);
		cd.sciDoc.Z.GoToPos(true, position);

		//tested: Microsoft.CodeAnalysis.CSharp.ImplementInterface.CSharpImplementInterfaceService works but the result is badly formatted (without spaces, etc). Internal, undocumented.
	}
}
