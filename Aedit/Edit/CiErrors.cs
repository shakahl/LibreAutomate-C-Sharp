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

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;

class CiErrors
{
	SemanticModel _semo;
	List<(Diagnostic d, int start, int end)> _codeDiag;
	readonly List<(int from, int to, string s)> _metaErrors = new();
	readonly List<(int from, int to, string s)> _stringErrors = new();

	public void Indicators(int start16, int end16, bool pasting = false) {
		_semo = null;

		if (!CodeInfo.GetContextAndDocument(out var cd, 0, metaToo: true)) return;
		var doc = cd.sciDoc;
		var code = cd.code;
		List<string> addUsings = null;
		bool has = false;
		var semo = cd.document.GetSemanticModelAsync().Result;
		var a = semo.GetDiagnostics(TextSpan.FromBounds(start16, end16));
		if (!a.IsDefaultOrEmpty) {
			_codeDiag = new(a.Length);
			foreach (var d_ in a) {
				var d = d_;
				if (d.IsSuppressed) continue;
				var loc = d.Location; if (!loc.IsInSource) continue;
				var span = loc.SourceSpan;
				//AOutput.Write(d.Severity, span, d.Id);
				int start = span.Start, end = span.End;
				if (end == start) {
					if (end < code.Length && !(code[end] == '\r' || code[end] == '\n')) end++;
					else if (start > 0) start--;
				}
				var ec = (ErrorCode)d.Code;
				//if (d.Severity == DiagnosticSeverity.Hidden && ec == ErrorCode.HDN_UnusedUsingDirective) {
				//	if (0 != code.Eq(start + 6, false, s_defaultUsings)) continue;
				//}
				if (start == 0 && ec == ErrorCode.WRN_UnprocessedXMLComment) continue; //XML comment at start

				//workaround for: when starting to type the last top-level statement, if it's a word (a C# type or non-keyword), error
				//	"A namespace cannot directly contain members such as fields or methods". Users would think it's a bug of this program.
				//	Another workaround would be to add ';' after. But it does not work if it's a C# type eg 'int'.
				//	Let's ignore it in editor. It's a lesser evil. And nobody will compile such unfinished code.
				//if (ec == ErrorCode.ERR_NamespaceUnexpected) continue;
				if (ec == ErrorCode.ERR_NamespaceUnexpected) {
					//If unknown name, convert to ERR_NameNotInContext. Then on mouse hover will display tooltip with links to add using directive etc.
					var d2 = cd.document.WithText(SourceText.From(code.Insert(end, ";"), Encoding.UTF8));
					var m2 = d2.GetSemanticModelAsync().Result;
					//AOutput.Write(m2.GetDiagnostics(span));
					d = m2.GetDiagnostics(span).FirstOrDefault(o => (ErrorCode)o.Code == ErrorCode.ERR_NameNotInContext);
					if (d == null) continue;
					ec = ErrorCode.ERR_NameNotInContext;
				}

				//if pasted or called InsertCode.Statements() etc, try to insert favorite using directive
				if (pasting) {
					if (ec is ErrorCode.ERR_NameNotInContext or ErrorCode.ERR_SingleTypeNameNotFound) {
						var s1 = code[start..end];
						if (!CodeInfo._favorite.GetNamespaceFor(s1, ref addUsings))
							if (_IsAttributeNameWithoutSuffix(s1, start, semo)) CodeInfo._favorite.GetNamespaceFor(s1 + "Attribute", ref addUsings);
					} else if (ec == ErrorCode.ERR_NoSuchMemberOrExtension) {
						var emReceiverType = _GetExtensionMethodReceiverType(semo, start);
						if (emReceiverType != null) CodeInfo._favorite.GetNamespaceFor(code[start..end], ref addUsings, emReceiverType);
					}
				}

				if (!has) doc.InicatorsDiag_(has = true);
				var indic = d.Severity switch { DiagnosticSeverity.Error => SciCode.c_indicError, DiagnosticSeverity.Warning => SciCode.c_indicWarning, DiagnosticSeverity.Info => SciCode.c_indicInfo, _ => SciCode.c_indicDiagHidden };
				doc.zIndicatorAdd(true, indic, start..end);
				_codeDiag.Add((d, start, end));

				if (d.Severity == DiagnosticSeverity.Error) {
					switch (ec) {
					case ErrorCode.ERR_NameNotInContext:
					case ErrorCode.ERR_SingleTypeNameNotFound:
					case ErrorCode.ERR_NoSuchMemberOrExtension:
					case ErrorCode.ERR_NoSuchMemberOrExtensionNeedUsing: //all these end with (are you missing a using directive...
					case ErrorCode.ERR_UnimplementedInterfaceMember:
					case ErrorCode.ERR_UnimplementedAbstractMethod:
						_semo = semo;
						break;
					}
				}
			}
		}
		if (_metaErrors.Count > 0) {
			foreach (var v in _metaErrors) {
				if (v.to <= start16 || v.from >= end16) continue;
				if (!has) doc.InicatorsDiag_(has = true);
				doc.zIndicatorAdd(true, SciCode.c_indicError, v.from..v.to);
			}
		}
		_Strings(semo, cd, start16, end16);
		if (_stringErrors.Count > 0) {
			if (!has) doc.InicatorsDiag_(has = true);
			foreach (var v in _stringErrors) {
				doc.zIndicatorAdd(true, SciCode.c_indicWarning, v.from..v.to);
			}
		}
		if (!has) {
			doc.InicatorsDiag_(false);
			_codeDiag = null;
		} else {
			//if (addUsings != null) InsertCode.UsingDirective(string.Join(';', addUsings)); //does not work because called from scintilla notification
			if (addUsings != null) doc.Dispatcher.InvokeAsync(() => InsertCode.UsingDirective(string.Join(';', addUsings)));
		}
	}

	//static readonly string[] s_defaultUsings = new string[] { "Au;", "Au.Types;", "System;", "System.Collections.Generic;", "System.Linq", "System.IO" };

	void _Strings(SemanticModel semo, CodeInfo.Context cd, int start16, int end16) {
		//using var p1 = APerf.Create();
		_stringErrors.Clear();
		var code = cd.code;
		foreach (var node in semo.Root.DescendantNodes(TextSpan.FromBounds(start16, end16))) {
			var format = CiUtil.GetParameterStringFormat(node, semo, false);
			if (format == PSFormat.None || format == PSFormat.ARegexReplacement) continue;
			var s = node.GetFirstToken().ValueText; //replaced escape sequences
			string es = null;
			try {
				switch (format) {
				case PSFormat.Regex:
					new System.Text.RegularExpressions.Regex(s); //never mind: may have 'options' argument, eg ECMAScript or Compiled
					break;
				case PSFormat.ARegex:
					new ARegex(s);
					break;
				case PSFormat.AWildex:
					if (s.Starts("***")) s = s[(s.IndexOf(' ') + 1)..]; //eg AWnd.Child("***accName ...")
					new AWildex(s);
					break;
				case PSFormat.AKeys:
					new AKeys(null).AddKeys(s);
					break;
				}
			}
			catch (ArgumentException ex) { es = ex.Message; }
			if (es != null) {
				var span = node.Span;
				_stringErrors.Add((span.Start, span.End, es));
			}
		}
	}

	public void ClearMetaErrors() => _metaErrors.Clear();

	public void AddMetaError(int from, int to, string s) => _metaErrors.Add((from, to > from ? to : from + 1, s));

	public void EraseIndicatorsInLine(SciCode doc, int pos8) {
		var (_, start, end) = doc.zLineStartEndFromPos(false, pos8, withRN: true);
		doc.zIndicatorClear(false, SciCode.c_indicDiagHidden, start..end);
		doc.zIndicatorClear(false, SciCode.c_indicInfo, start..end);
		doc.zIndicatorClear(false, SciCode.c_indicWarning, start..end);
		doc.zIndicatorClear(false, SciCode.c_indicError, start..end);
	}

	public void SciModified(SciCode doc, in Sci.SCNotification n) {
		//clear arrays to prevent showing tooltip because positions changed. But don't clear indicators because we'll update them soon.
		_codeDiag = null;
		_stringErrors.Clear();

		if (_pasting != null && n.modificationType.Has(Sci.MOD.SC_MOD_INSERTTEXT | Sci.MOD.SC_PERFORMED_USER)) {
			var p = _pasting; _pasting = null;
			if (doc == p && n.length > 3) {
				int start = doc.zPos16(n.position), end = doc.zPos16(n.position + n.length);
				Indicators(start, end, pasting: true);
			}
		}
	}
	SciCode _pasting;

	public void Pasting(SciCode doc) { _pasting = doc; }

	public System.Windows.Documents.Section GetPopupTextAt(SciCode doc, int pos8, int pos16, out Action<CiPopupText, string> onLinkClick) {
		onLinkClick = null;
		if (_codeDiag == null && _metaErrors.Count == 0 && _stringErrors.Count == 0) return null;
		if (pos8 < 0) return null;
		int all = doc.Call(Sci.SCI_INDICATORALLONFOR, pos8);
		//AOutput.Write(all);
		if (0 == (all & ((1 << SciCode.c_indicError) | (1 << SciCode.c_indicWarning) | (1 << SciCode.c_indicInfo) | (1 << SciCode.c_indicDiagHidden)))) return null;

		var x = new CiText();
		x.StartParagraph();

		ErrorCode ecPrev = 0;
		int implPos = -1; bool implInterface = false;
		for (int i = 0, n = _codeDiag?.Count ?? 0; i < n; i++) {
			var v = _codeDiag[i];
			if (pos16 < v.start || pos16 > v.end) continue;
			var d = v.d;
			var s1 = d.Severity switch { DiagnosticSeverity.Error => "Error", DiagnosticSeverity.Warning => "Warning", _ => "Info" };
			x.LineBreak(s1, notIfFirstInParagraph: true);
			x.Append(": " + d.GetMessage());

			var ec = (ErrorCode)d.Code;
			if (d.Severity == DiagnosticSeverity.Error) {
				if (_semo == null) continue;
				//AOutput.Write(ec, d.Id);
				bool extMethod = false;
				switch (ec) {
				case ErrorCode.ERR_NoSuchMemberOrExtension:
				case ErrorCode.ERR_NoSuchMemberOrExtensionNeedUsing: //all these end with (are you missing a using directive...
					extMethod = true;
					goto case ErrorCode.ERR_NameNotInContext;
				case ErrorCode.ERR_NameNotInContext:
				case ErrorCode.ERR_SingleTypeNameNotFound:
					//not tested: ERR_GlobalSingleTypeNameNotFound, ERR_DottedTypeNameNotFoundInAgg, ERR_AliasNotFound, ERR_TypeNotFound
					if (ec == ecPrev) continue; //probably "not found 'AbcAttribute'" followed by "not found 'Abc'"
					ecPrev = ec;
					_UsingsEtc(x, v, doc, extMethod);
					break;
				case ErrorCode.ERR_DottedTypeNameNotFoundInNS: //using Namespace.NotFound;
					ecPrev = ec;
					x.Hyperlink("^r", "\nAdd assembly reference or class file...");
					break;
				case ErrorCode.ERR_UnimplementedInterfaceMember:
				case ErrorCode.ERR_UnimplementedAbstractMethod:
					Debug.Assert(implPos == -1 || implPos == v.start);
					implPos = v.start;
					implInterface = ec == ErrorCode.ERR_UnimplementedInterfaceMember;
					break;
				}
			} else if (d.Severity == DiagnosticSeverity.Warning) {
				switch (ec) {
				case ErrorCode.WRN_MissingXMLComment:
					_XmlComment(x/*, v*/);
					break;
				}
			}
		}
		if (implPos >= 0) _Implement(x, implPos, implInterface);

		_Also(_metaErrors, "Error: ");
		_Also(_stringErrors, null);
		void _Also(List<(int from, int to, string s)> a, string prefix) {
			foreach (var v in a) {
				if (pos16 < v.from || pos16 > v.to) continue;
				x.LineBreak(prefix, notIfFirstInParagraph: true);
				x.Append(v.s);
			}
		}

		x.EndParagraph();
		onLinkClick = (ph, e) => _LinkClicked(e);
		return x.Result;
	}

	void _UsingsEtc(CiText x, in (Diagnostic d, int start, int end) v, SciCode doc, bool extMethod) {
		string code = doc.zText;
		bool isGeneric = false;
		int start = v.start;
		int end2 = code.IndexOf('<', start, v.end - start);
		if (end2 < 0) end2 = v.end; else isGeneric = true;
		var errName = code[start..end2];
		bool isAttribute = !extMethod && _IsAttributeNameWithoutSuffix(errName, start, _semo);
		var errName2 = isAttribute ? errName + "Attribute" : null;
		ITypeSymbol emReceiverType = null;

		var comp = _semo.Compilation;
		var usings = new List<string>();
		var stack = new List<string>();
		//var p1 = APerf.Create();
		//var p1 = new APerf.Local { Incremental = true };
		_EnumNamespace(comp.GlobalNamespace);
		//p1.Write();
		//p1.NW();

		//shoulddo: async, because quite slow. Or use AssemblyMetadata.CachedSymbols (internal), it's faster except first time.
		void _EnumNamespace(INamespaceSymbol ns) {
			bool found = false;
			foreach (var nt in ns.GetMembers()) {
				string sn = nt.Name;
				//AOutput.Write(sn);
				if (sn.NE() || sn[0] == '<') continue;
				if (nt is INamespaceSymbol ins) {
					stack.Add(sn);
					_EnumNamespace(ins);
					stack.RemoveAt(stack.Count - 1);
				} else if (!found) { //if found, continue to search in nested namespaces
					ISymbol sym = nt;
					if (extMethod) {
						var its = nt as INamedTypeSymbol;
						//p1.First();
						if (its.IsStatic && its.MightContainExtensionMethods) { //fast, but without IsStatic slow first time
							foreach (var m in nt.GetMembers().OfType<IMethodSymbol>()) { //fast; slightly slower than nt.MemberNames.Contains(errName) which gets member types etc too
								if (m.Name == errName && m.IsExtensionMethod) {
									emReceiverType ??= _GetExtensionMethodReceiverType(_semo, start);
									ADebug.PrintIf(emReceiverType == null, "failed to get extension method receiver type");
									if (emReceiverType == null) continue;
									if (null == m.ReduceExtensionMethod(emReceiverType)) { /*ADebug.Print(emReceiverType);*/ continue; }
									found = true;
									sym = m;
									break;
								}
							}
						}
						//p1.Next();
					} else {
						if (sn != errName) {
							if (errName2 == null || sn != errName2) continue;
						}
						var its = nt as INamedTypeSymbol;
						//found = its.IsGenericType == isGeneric;
						found = !isGeneric || its.IsGenericType; //SHOULDDO: test more. Was above line instead, but then did not find when generic in editor is without <>.
					}
					if (found) found = sym.IsAccessibleWithin(comp.Assembly);
					if (found) usings.Add(string.Join('.', stack));
				}
			}
		}

		if (usings.Count > 0) {
			var sstart = doc.zPos8(v.start).ToString();
			x.Append("\nAdd using ");
			for (int i = 0; i < usings.Count; i++) {
				var u = usings[i];
				if (i > 0) x.Append(" or ");
				x.Hyperlink("^u" + sstart + u, u);
			}
			if (!extMethod) {
				x.Append("\nOr prefix ");
				for (int i = 0; i < usings.Count; i++) {
					var u = usings[i];
					if (i > 0) x.Append(" or ");
					x.Hyperlink("^p" + sstart + u, u);
				}
			}
		} else {
			x.Hyperlink("^r", "\nAdd assembly reference or class file...");
			if (!(extMethod | isGeneric | isAttribute)) x.Hyperlink("^w" + errName, "\nFind Windows API...");
		}
	}

	void _LinkClicked(string s) {
		CodeInfo.HideTextPopup();
		char action = s[1];
		if (action == 'u' || action == 'p') { //add 'using', prefix namespace
			int pos8 = s.ToInt(2, out int i);
			s = s[i..];
			var doc = Panels.Editor.ZActiveDoc;
			EraseIndicatorsInLine(doc, pos8);
			if (action == 'p') {
				doc.zInsertText(false, pos8, s + ".", addUndoPointAfter: true);
			} else {
				InsertCode.UsingDirective(s);
			}
		} else if (action == 'w') { //Windows API
			new Au.Tools.DWinapi(s[2..]).Show();
		} else if (action == 'r') { //Add reference
			Menus.File.Properties();
		} else if (action == 'i') { //implement interface or abstract class
			InsertCode.ImplementInterfaceOrAbstractClass(s[2] == 'e', s.ToInt(3));
		}
	}

	static void _Implement(CiText x, int pos, bool isInterface) {
		x.Hyperlink("^ii" + pos, "\nImplement " + (isInterface ? "interface" : "abstract class"));
		if (isInterface) x.Hyperlink("^ie" + pos, "\nImplement explicitly");
	}

	static void _XmlComment(CiText x/*, in (Diagnostic d, int start, int end) v*/) {
		//x.Hyperlink("^xa+v.start, "\nAdd XML comment");
		//x.Hyperlink("^xd+v.start, "\nDisable warning");

		x.Append("\nTo add XML comment, type /// above.");
		x.Append("\nTo disable warning, add just /// or disable warning 1591 (use warningDisableSnippet).");
	}

	static bool _IsAttributeNameWithoutSuffix(string name, int pos, SemanticModel semo) {
		if (name.Ends("Attribute")) return false;
		return semo.SyntaxTree.IsAttributeNameContext(pos, default);
	}

	static ITypeSymbol _GetExtensionMethodReceiverType(SemanticModel semo, int startOfMethodName) {
		ITypeSymbol t = null;
		if (semo.SyntaxTree.GetRoot().FindToken(startOfMethodName).Parent.Parent is MemberAccessExpressionSyntax ma)
			t = semo.GetTypeInfo(ma.Expression).Type;
		ADebug.PrintIf(t == null, "failed to get extension method receiver type");
		return t;
	}
}
