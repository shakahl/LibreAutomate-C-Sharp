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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.Text;

//TODO: test C# 10 'global using'.
//TODO: if in completion list selected like Favorite.Namespace.Enum, remove Favorite.Namespace and insert using Favorite.Namespace.

//FUTURE: favorite meta r and c. Then can add favorite namespaces from these too.

//FUTURE: support using static.
//	Then users could create their common "functions.cs" files used with meta c and 'using static'.
//	When sharing in forums, recommend to put their functions.cs in patebin and in forum signature add link to it.

class CiFavorite
{
	record _NamespaceTypes(
		INamespaceSymbol ns,
		(INamedTypeSymbol t, bool hidden, bool isAttribute)[] types,
		List<IMethodSymbol[]> em //array for each type that contains extension methods
		);

	_NamespaceTypes[] _namespaces;
	string _usings; //= App.Settings.ci_usings, to detect when changed

	bool _Init() {
		var usings = App.Settings.ci_usings;
		if (usings.NE()) return false;

		if (usings != _usings) {
			_usings = usings;
			//compile code with these using directives, to get types from these namespaces
			var code = usings.RegexReplace(@"(?m)^\w.*$", "using $0;");
			var tree = CSharpSyntaxTree.ParseText(code, encoding: Encoding.UTF8);
			var refs = new Au.Compiler.MetaReferences().Refs; //FUTURE: support nondefault references.
			var comp = CSharpCompilation.Create("f", new SyntaxTree[] { tree }, refs);
			var semo = comp.GetSemanticModel(tree, false) as SyntaxTreeSemanticModel;

			var d = semo.GetDeclarationDiagnostics();
			if (!d.IsDefaultOrEmpty) {
				foreach (var v in d) if (v.Severity == DiagnosticSeverity.Error) print.it("Error in Options -> Code -> namespaces: " + v.GetMessage());
			}

			//get types and group by namespace
			_namespaces = semo.LookupNamespacesAndTypes(code.Length)
				.OfType<INamedTypeSymbol>()
				.GroupBy(o => o.ContainingNamespace, (ns, et) => {
					//types
					var types = et.Select(o => (o, _IsHidden(o), o.IsAttribute())).ToArray();

					//extension methods
					List<IMethodSymbol[]> em = null;
					foreach (var t in et) {
						if (!t.MightContainExtensionMethods) continue;
						var a = t.GetMembers().OfType<IMethodSymbol>().Where(m => m.IsExtensionMethod && !_IsHidden(m)).ToArray();
						if (a.Any()) (em ??= new()).Add(a);
					}

					return new _NamespaceTypes(ns, types, em);
				}, new CiNamespaceSymbolEqualityComparer())
				.ToArray();

			//foreach(var v in _namespaces) {
			//	print.it(v.ns);
			//}

			static bool _IsHidden(ISymbol sym) {
				//return !sym.IsEditorBrowsable(false, comp); //same result but much slower
				return sym.GetAttributes().Any(
					o => o.AttributeClass.Name == "EditorBrowsableAttribute" && o.ConstructorArguments.FirstOrDefault().Value is int i && i == (int)EditorBrowsableState.Never
					);
				//|| o.AttributeClass.Name is "ObsoleteAttribute"
			}
		}
		return true;
	}

	public void AddCompletions(List<CiComplItem> items, Dictionary<INamespaceOrTypeSymbol, List<int>> groups, TextSpan span, CSharpSyntaxContext syncon, ITypeSymbol typeForExtensionMethods = null) {
		if (!_Init()) return;

		//note: compare name, not ISymbol etc reference.
		//	For example when meta testInternal, all references of these assemblies are != ours, and we would add duplicates if comparing references.
		//info: groups.Comparer is CiNamespaceOrTypeSymbolEqualityComparer. It compares by name. As well as our comparer used with GroupBy(namespace).
		//	Therefore groups and _namespaces contain single item for each namespace full name. If comparing references, would contain multiple (eg when same namespace is in multiple assemblies).

		//perf.first();
		CiComplItem prevci = null; string prevname = null; //to join overloads
		if (typeForExtensionMethods != null) { //add extension methods

			//var hs = groups.Keys.Select(o => o.ContainingNamespace).ToHashSet();

			foreach (var nt in _namespaces) {
				if (nt.em == null) continue;
				foreach (var a in nt.em) {
					//don't add from namespaces specified in code (using directives)
					//if (hs.Contains(a[0].ContainingNamespace)) continue; //usually does not make much faster. Also need to test more, maybe in some cases prevents adding our methods.
					if (groups.ContainsKey(a[0].ContainingType)) continue;
					//print.it(nt.ns, a[0].ContainingType, a);

					prevname = null;
					foreach (var m in a) {
						var rm = m.ReduceExtensionMethod(typeForExtensionMethods);
						if (rm == null) continue;
						//print.it(m);
						var name = rm.Name;
						if (name != prevname) {
							prevname = name;
							items.Add(prevci = new CiComplItem(CiComplProvider.Favorite, span, rm, CiItemKind.ExtensionMethod));

							var ctype = m.ContainingType;
							int i = items.Count - 1;
							if (groups.TryGetValue(ctype, out var list)) list.Add(i); else groups.Add(ctype, new List<int> { i });
						} else {
							prevci.AddOverload(rm);
						}
					}
				}
			}
			//slow, usually 5-10 ms. But cannot optimize. Type of [this] parameter can be any base or implemented interface of typeForExtensionMethods.
		} else { //add types
			bool onlyAttributes = syncon.IsAttributeNameContext;
			foreach (var nt in _namespaces) {
				//don't add from namespaces specified in code (using directives)
				var ns = nt.ns;
				bool skip = groups.TryGetValue(ns, out var ati);
				//print.it(skip, ns);
				if (skip) {
					//don't skip if the group contains single item "Unimported.Namespace.Type" for parameter enum or new, like in this code: run.it("", "", RFlags, new ROptions);
					if (ati.Count == 1 && items[ati[0]].Text.Contains('.')) skip = false;
				}
				int nExist = ati.Lenn_();

				prevname = null;
				foreach (var (t, hidden, isAttribute) in nt.types) {
					if (hidden) continue;
					if (onlyAttributes && !isAttribute) continue;

					var name = t.Name;
					if (onlyAttributes && name.Ends("Attribute") && name.Length > 9) name = name[..^9];
					//print.it(name);
					//note: in some cases Roslyn may not remove suffix "Attribute" from its completion items. Rare, never mind.

					if (skip) {
						//although the namespace exists in groups, it may contain only some members, eg of Object or other base type
						bool found = false;
						for (int i = 0; i < nExist; i++)
							if (items[ati[i]].Text == name) {
								found = true;
								break;
							}
						if (found) continue;
						//print.it(name, items.Take(nExist).Select(o => o.Text));
					}

					if (name != prevname) {
						prevname = name;
						var kind = t.TypeKind switch {
							TypeKind.Class => CiItemKind.Class,
							TypeKind.Delegate => CiItemKind.Delegate,
							TypeKind.Enum => CiItemKind.Enum,
							TypeKind.Interface => CiItemKind.Interface,
							_ => CiItemKind.Structure
						};
						items.Add(prevci = new CiComplItem(CiComplProvider.Favorite, span, t, kind, name));

						int i = items.Count - 1;
						if (ati != null) ati.Add(i); else groups.Add(ns, ati = new List<int> { i });
					} else { //join generic overloads, like Action<T>, Action<T1,T2>, ...
						prevci.AddOverload(t);
					}
				}
			}
			//rejected: optimize to avoid calling this code each time. Fast, although creates garbage.
			//rejected: support 'using static Namespace.Type'. Rare. Initially implemented, but not fully; also would need to: 1. use context. 2. join overloads.
		}
		//perf.nw();

		//code scraps for 'using static'
		//} else if (sym is IFieldSymbol f) {
		//	kind = f.IsConst ? CiItemKind.Constant : CiItemKind.Field;
		//} else {
		//	kind = sym.Kind switch {
		//		SymbolKind.Event => CiItemKind.Event,
		//		SymbolKind.Method => CiItemKind.Method,
		//		SymbolKind.Property => CiItemKind.Property,
		//		_ => CiItemKind.None
		//	};
		//	if (kind == CiItemKind.None) continue;
	}

	public static void OnCommitInsertUsingDirective(CiComplItem item) {
		var sym = item.FirstSymbol;
		InsertCode.UsingDirective(sym.ContainingNamespace.ToString());

		//code scraps for 'using static'
		//var ct = sym.ContainingType;
		//var ns = ct == null ? sym.ContainingNamespace.ToString() : "static " + ct.ToString();
		//InsertCode.UsingDirective(ns);
	}

	/// <summary>
	/// If <i>name</i> is a type or extension method (if typeForExtensionMethods not null) in favorite namespaces, calls <c>namespaces ??= new()</c>, adds its namespace to <i>namespaces</i> (once) and returns true.
	/// </summary>
	public bool GetNamespaceFor(string name, ref List<string> namespaces, ITypeSymbol typeForExtensionMethods = null) {
		if (!_Init()) return false;
		INamespaceSymbol ns = null;
		foreach (var nt in _namespaces) {
			if (typeForExtensionMethods != null) {
				if (nt.em == null) continue;
				foreach (var a in nt.em) {
					foreach (var m in a) {
						if (m.Name == name) {
							var rm = m.ReduceExtensionMethod(typeForExtensionMethods);
							if (rm == null) continue;
							if (ns != null) return false; //multiple namespaces contain extension methods with this name
							ns = nt.ns;
							break;
						}
					}
				}
			} else {
				foreach (var (t, _, _) in nt.types) {
					if (t.Name == name) {
						if (ns != null) return false; //multiple namespaces contain type with this name
						ns = nt.ns;
						break;
					}
				}
			}
		}
		if (ns == null) return false;
		var s = ns.ToString();
		if (!(namespaces ??= new()).Contains(s)) namespaces.Add(s);
		return true;
	}
}
