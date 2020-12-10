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
//using System.Linq;

using Au;
using Au.Types;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Shared.Extensions;

class CiErrors
{
	SemanticModel _semo;
	List<(Diagnostic d, int start, int end)> _codeDiag;
	readonly List<(int from, int to, string s)> _metaErrors = new List<(int, int, string)>();
	readonly List<(int from, int to, string s)> _stringErrors = new List<(int, int, string)>();

	public void Indicators(int start16, int end16) {
		_semo = null;

		if (!CodeInfo.GetContextAndDocument(out var cd, 0, metaToo: true)) return;
		var doc = cd.sciDoc;
		var code = cd.code;
		bool has = false;
		var semo = cd.document.GetSemanticModelAsync().Result;
		var a = semo.GetDiagnostics(TextSpan.FromBounds(start16, end16));
		if (!a.IsDefaultOrEmpty) {
			_codeDiag = new List<(Diagnostic d, int start, int end)>(a.Length);
			foreach (var d in a) {
				if (d.IsSuppressed) continue;
				var loc = d.Location; if (!loc.IsInSource) continue;
				var span = loc.SourceSpan;
				//AOutput.Write(d.Severity, span, d.Id);
				int start = span.Start, end = span.End;
				if (end == start) {
					if (end < code.Length && !(code[end] == '\r' || code[end] == '\n')) end++;
					else if (start > 0) start--;
				}
				if (d.Severity == DiagnosticSeverity.Hidden && d.Code == 8019) { //unnecessary using directive
					if (0 != code.Eq(start + 6, false, s_defaultUsings)) continue;
				}
				if (!has) doc.InicatorsDiag_(has = true);
				var indic = d.Severity switch { DiagnosticSeverity.Error => SciCode.c_indicError, DiagnosticSeverity.Warning => SciCode.c_indicWarning, DiagnosticSeverity.Info => SciCode.c_indicInfo, _ => SciCode.c_indicDiagHidden };
				doc.Z.IndicatorAdd(true, indic, start..end);
				_codeDiag.Add((d, start, end));

				if (d.Severity == DiagnosticSeverity.Error) {
					switch ((ErrorCode)d.Code) {
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
				doc.Z.IndicatorAdd(true, SciCode.c_indicError, v.from..v.to);
			}
		}
		_Strings(semo, cd, start16, end16);
		if (_stringErrors.Count > 0) {
			if (!has) doc.InicatorsDiag_(has = true);
			foreach (var v in _stringErrors) {
				doc.Z.IndicatorAdd(true, SciCode.c_indicWarning, v.from..v.to);
			}
		}
		if (!has) {
			doc.InicatorsDiag_(false);
			_codeDiag = null;
		}
	}

	static readonly string[] s_defaultUsings = new string[] { "Au;", "Au.Types;", "System;", "System.Collections.Generic;" };

	void _Strings(SemanticModel semo, in CodeInfo.Context cd, int start16, int end16) {
		//using var p1 = APerf.Create();
		_stringErrors.Clear();
		var code = cd.code;
		foreach (var node in semo.Root.DescendantNodes(TextSpan.FromBounds(start16, end16))) {
			var format = CiUtil.GetParameterStringFormat(node, semo, false);
			if (format == PSFormat.None || format == PSFormat.ARegexReplacement) continue;
			var s = node.GetFirstToken().ValueText; //replaced escape sequences
													//AOutput.Write(format, s);
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
		var (_, start, end) = doc.Z.LineStartEndFromPos(false, pos8, withRN: true);
		doc.Z.IndicatorClear(false, SciCode.c_indicDiagHidden, start..end);
		doc.Z.IndicatorClear(false, SciCode.c_indicInfo, start..end);
		doc.Z.IndicatorClear(false, SciCode.c_indicWarning, start..end);
		doc.Z.IndicatorClear(false, SciCode.c_indicError, start..end);
	}

	public void SciModified() {
		//clear arrays to prevent showing tooltip because positions changed. But don't clear indicators because we'll update them soon.
		_codeDiag = null;
		_stringErrors.Clear();
	}

	public bool SciMouseDwellStarted(SciCode doc, int pos8) {
		if (_codeDiag == null && _metaErrors.Count == 0 && _stringErrors.Count == 0) return false;
		if (pos8 < 0) return false;
		int all = doc.Call(Au.Controls.Sci.SCI_INDICATORALLONFOR, pos8);
		//AOutput.Write(all);
		if (0 == (all & ((1 << SciCode.c_indicError) | (1 << SciCode.c_indicWarning) | (1 << SciCode.c_indicInfo) | (1 << SciCode.c_indicDiagHidden)))) return false;
		int pos16 = doc.Pos16(pos8);

		var x = new CiXaml();
		x.StartParagraph();
		x.LineBreakIfLonger = x.SB.Length;

		ErrorCode ecPrev = 0;
		int implPos = -1; bool implInterface = false;
		for (int i = 0, n = _codeDiag?.Count ?? 0; i < n; i++) {
			var v = _codeDiag[i];
			if (pos16 < v.start || pos16 > v.end) continue;
			var d = v.d;
			var s1 = d.Severity switch { DiagnosticSeverity.Error => "Error", DiagnosticSeverity.Warning => "Warning", _ => "Info" };
			var s2 = System.Web.HttpUtility.HtmlEncode(d.GetMessage());
			x.LineBreak();
			x.AppendFormat("{0}: {1}", s1, s2);

			if (d.Severity == DiagnosticSeverity.Error) {
				if (_semo == null) continue;
				//AOutput.Write(d.Code, d.Id);
				bool extMethod = false;
				var ec = (ErrorCode)d.Code;
				switch (ec) {
				case ErrorCode.ERR_NoSuchMemberOrExtension:
				case ErrorCode.ERR_NoSuchMemberOrExtensionNeedUsing: //all these end with (are you missing a using directive...
					extMethod = true;
					goto case ErrorCode.ERR_NameNotInContext;
				case ErrorCode.ERR_NameNotInContext:
				case ErrorCode.ERR_SingleTypeNameNotFound:
					if (ec == ecPrev) continue; //probably "not found 'AbcAttribute'" followed by "not found 'Abc'"
					ecPrev = ec;
					_UsingsEtc(x, v, doc, extMethod);
					break;
				case ErrorCode.ERR_UnimplementedInterfaceMember:
				case ErrorCode.ERR_UnimplementedAbstractMethod:
					Debug.Assert(implPos == -1 || implPos == v.start);
					implPos = v.start;
					implInterface = ec == ErrorCode.ERR_UnimplementedInterfaceMember;
					break;
				}
			} else if (d.Severity == DiagnosticSeverity.Warning) {
				switch ((ErrorCode)d.Code) {
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
				x.LineBreak(prefix); x.Append(v.s);
			}
		}

		x.EndParagraph();
		var xaml = x.End();

		CodeInfo.ShowXamlPopup(doc, pos16, xaml, onLinkClick: (ph, e) => _LinkClicked(e), above: true);
		return true;
	}

	void _UsingsEtc(CiXaml x, in (Diagnostic d, int start, int end) v, SciCode doc, bool extMethod) {
		string code = doc.Text;
		bool isGeneric = false;
		int end2 = code.IndexOf('<', v.start, v.end - v.start);
		if (end2 < 0) end2 = v.end; else isGeneric = true;
		var errName = code[v.start..end2];
		bool isAttribute = !extMethod && _semo.Root.FindToken(v.start).Parent?.Parent is AttributeSyntax && !errName.Ends("Attribute");
		var errName2 = isAttribute ? errName + "Attribute" : null;

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
							foreach (var v in nt.GetMembers()) { //fast; slightly slower than nt.MemberNames.Contains(errName) which gets member types etc too
								if (v is IMethodSymbol && (found = v.Name == errName)) { sym = v; break; }
							}
						}
						//p1.Next();
					} else {
						if (sn != errName) {
							if (errName2 == null || sn != errName2) continue;
						}
						var its = nt as INamedTypeSymbol;
						//found = its.IsGenericType == isGeneric;
						found = !isGeneric || its.IsGenericType; //TODO: test more. Was above line instead, but then did not find when generic in editor is without <>.
					}
					if (found) found = sym.IsAccessibleWithin(comp.Assembly);
					if (found) usings.Add(string.Join('.', stack));
				}
			}
		}

		if (usings.Count > 0) {
			var sstart = doc.Pos8(v.start).ToString();
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
			x.Hyperlink("^r", "\nAdd assembly reference...");
			if (!(extMethod | isGeneric | isAttribute)) x.Hyperlink("^w" + errName, "\nFind Windows API...");
		}
	}

	void _LinkClicked(string s) {
		CodeInfo.HideXamlPopup();
		char action = s[1];
		if (action == 'u' || action == 'p') { //add 'using', prefix namespace
			int pos8 = s.ToInt(2, out int i);
			s = s[i..];
			var doc = Panels.Editor.ZActiveDoc;
			EraseIndicatorsInLine(doc, pos8);
			if (action == 'p') {
				doc.Z.InsertText(false, pos8, s + ".", addUndoPoint: true);
			} else {
				InsertCode.UsingDirective(s);
			}
		} else if (action == 'w') { //Windows API
			s = s[2..];
			FormWinapi.ZShowDialog(s);
		} else if (action == 'r') { //Add reference
			Menus.File.Properties();
		} else if (action == 'i') { //implement interface or abstract class
			InsertCode.ImplementInterfaceOrAbstractClass(s[2] == 'e', s.ToInt(3));
		}
	}

	static void _Implement(CiXaml x, int pos, bool isInterface) {
		x.Hyperlink("^ii" + pos, "\nImplement " + (isInterface ? "interface" : "abstract class"));
		if (isInterface) x.Hyperlink("^ie" + pos, "\nImplement explicitly");
	}

	static void _XmlComment(CiXaml x/*, in (Diagnostic d, int start, int end) v*/) {
		//x.Hyperlink("^xa+v.start, "\nAdd XML comment");
		//x.Hyperlink("^xd+v.start, "\nDisable warning");

		x.Append("\nTo add XML comment, use docSnippet. Type <Bold>doc</Bold> above, and select docSnippet from the completion list.");
		x.Append("\nTo disable warning, add <Bold>///</Bold> above or disable warning 1591 (warningDisableSnippet) or make non-public.");
	}
}
