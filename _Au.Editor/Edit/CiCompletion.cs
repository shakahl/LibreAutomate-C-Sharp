#define HAVE_SYMBOLS //if the Roslyn project has been modified (added Symbols property to the CompletionItem class) and compiled

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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Completion;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Shared.Extensions;

class CiCompletion
{
	CiPopupList _popupList;
	_Data _data; //not null while the popup list window is visible
	CancellationTokenSource _cancelTS;

	class _Data
	{
		public CompletionService completionService;
		public Document document;
		public SemanticModel model;
		public List<CiComplItem> items;
		public int codeLength;
		public string filterText;
		public SciCode.ITempRange tempRange;
		public bool noAutoSelect;
		Dictionary<CompletionItem, CiComplItem> _map;

		public Dictionary<CompletionItem, CiComplItem> Map {
			get {
				if(_map == null) {
					_map = new Dictionary<CompletionItem, CiComplItem>(items.Count);
					foreach(var v in items) _map.Add(v.ci, v);
				}
				return _map;
			}
		}
	}

	public bool IsVisibleUI => _data != null;

	public void Cancel()
	{
		_cancelTS?.Cancel(); _cancelTS = null;
		_CancelList();
	}

	void _CancelList(bool popupListHidden = false, bool tempRangeRemoved = false)
	{
		//AOutput.Write("_CancelList", _data != null);
		if(_data == null) return;
		if(!tempRangeRemoved) _data.tempRange.Remove();
		_data = null;
		if(!popupListHidden) _popupList.Hide();
	}

	public void SciUpdateUI(SciCode doc) //modified, position changed, clicked
	{
		//int pos = doc.Z.CurrentPosChars;
		//var node = CiTools.NodeAt(pos);
		//AOutput.Write(CiTools.IsInString(ref node, pos));

	}

	/// <summary>
	/// Called before <see cref="CiAutocorrect.SciCharAdded"/> and before passing the character to Scintilla.
	/// If showing popup list, synchronously commits the selected item if need (inserts text).
	/// Else inserts text at caret position and now caret is after the text.
	/// </summary>
	public CiComplResult SciCharAdding_Commit(SciCode doc, char ch)
	{
		CiComplResult R = CiComplResult.None;
		if(_data != null) {
			if(!SyntaxFacts.IsIdentifierPartCharacter(ch)) {
				var ci = _popupList.SelectedItem;
				if(ci != null && _data.filterText.Length == 0 && ch != '.') ci = null;
				if(ci != null) R = _Commit(doc, ci, ch, default);
				_CancelList();
				//return;
			}
		}
		return R;
	}

	/// <summary>
	/// Asynchronously shows popup list if need. If already showing, synchronously filters/selects items.
	/// Called after <see cref="SciCharAdding_Commit"/> and <see cref="CiAutocorrect.SciCharAdded"/>.
	/// </summary>
	public void SciCharAdded_ShowList(CodeInfo.CharContext c)
	{
		if(_data == null) {
			_ShowList(c.ch);
		}
	}

	public void SciModified(SciCode doc, in Sci.SCNotification n)
	{
		if(_data != null) {
			bool trValid = _data.tempRange.GetCurrentFromTo(out int from, out int to, utf8: true);
			Debug.Assert(trValid); if(!trValid) { Cancel(); return; }
			string s = doc.Z.RangeText(false, from, to);
			foreach(var v in s) if(!SyntaxFacts.IsIdentifierPartCharacter(v)) return; //mostly because now is before SciCharAddedCommit, which commits (or cancels) if added invalid char
			_data.filterText = s;
			_FilterItems(_data);
			_popupList.UpdateVisibleItems(); //and calls SelectBestMatch
		}
	}

	public void ShowList()
	{
		_ShowList(default);
	}

	static bool s_workaround1;

	//SHOULDDO: delay
	async void _ShowList(char ch)
	{
		//using
		var p1 = APerf.Create(); //FUTURE: remove all p1 lines

		//AOutput.Write(_cancelTS);
		bool isCommand = ch == default, wasBusy = _cancelTS != null;
		Cancel();

		if(!CodeInfo.GetContextWithoutDocument(out var cd)) return; //returns false if position is in meta comments
		SciCode doc = cd.sciDoc;
		int position = cd.pos16;
		string code = cd.code;

		if(ch != default && position > 1 && SyntaxFacts.IsIdentifierPartCharacter(ch) && SyntaxFacts.IsIdentifierPartCharacter(code[position - 2])) { //in word
			if(!wasBusy) return;
			ch = default;
		}

		//using var nogcr = AKeys.IsScrollLock ? new NoGcRegion(50_000_000) : default;

		if(!cd.GetDocument()) return; //returns false if fails (unlikely)
		Document document = cd.document;
		Debug.Assert(code == document.GetTextAsync().Result.ToString());
		p1.Next('d');

		bool isDot = false, canGroup = false;
		PSFormat stringFormat = PSFormat.None; TextSpan stringSpan = default;
		CompletionService completionService = null;
		SemanticModel model = null;
		SyntaxNode node = null, root = null;

		_cancelTS = new CancellationTokenSource();
		var cancelTS = _cancelTS;
		var cancelToken = cancelTS.Token;
#if DEBUG
		if(Debugger.IsAttached) { cancelToken = default; _cancelTS = null; }
#endif

		try {
			var r = await Task.Run(async () => { //info: actually GetCompletionsAsync etc are not async
				completionService = CompletionService.GetService(document);
				if(cancelToken.IsCancellationRequested) return null;

				model = await document.GetSemanticModelAsync(cancelToken).ConfigureAwait(false); //speed: does not make slower, because GetCompletionsAsync calls it too. Currently we need only GetSyntaxTreeAsync here, but the speed is same, even when we don't call GetCompletionsAsync.
				root = model.Root;
				node = root.FindToken(position).Parent;
				p1.Next('s');

				//never mind: To make faster in some cases, could now return if in comments or non-regex etc string.
				//	It is not so easy to check correctly. GetCompletionsAsync then not very fast and not too slow.

				//is it member access syntax?
				ExpressionSyntax nodeL = null; //node at left of . etc
				int i = position - 1; while(i > 0 && SyntaxFacts.IsIdentifierPartCharacter(code[i])) i--;
				if(i > 0) {
					var token = root.FindToken(i); //fast
					if(position >= token.Span.End) {
						var tk = token.Kind();
						isDot = tk == SyntaxKind.DotToken || tk == SyntaxKind.MinusGreaterThanToken || tk == SyntaxKind.ColonColonToken;
						if(isDot) {
							node = token.Parent;
							switch(node) {
							case MemberAccessExpressionSyntax s1: nodeL = s1.Expression; break; // . or ->
							case MemberBindingExpressionSyntax s1 when s1.Parent is ConditionalAccessExpressionSyntax cae: nodeL = cae.Expression; break; // ?.
							case QualifiedNameSyntax s1: nodeL = s1.Left; break; // eg . outside functions
							case AliasQualifiedNameSyntax s1: nodeL = s1.Alias; break; // ::
							default: isDot = false; ADebug.Print(node.GetType()); break;
							}
						}
					}
				}

				if(!isDot && ch == '[' && node is AttributeListSyntax) ch = default; //else GetCompletionsAsync does not work

				//This is a temporary workaround for exception in new Roslyn version, in AbstractEmbeddedLanguageCompletionProvider.GetLanguageProviders().
				//	FUTURE: After some time try a newer version.
				if(!s_workaround1 && ch != default) {
					_ = completionService.GetCompletionsAsync(document, position).Result;
					s_workaround1 = true;
				}

				var trigger = ch == default ? default : CompletionTrigger.CreateInsertionTrigger(ch);
				var r1 = await completionService.GetCompletionsAsync(document, position, trigger, cancellationToken: cancelToken).ConfigureAwait(false);
				p1.Next('C');
				if(r1 != null) {
					if(isDot) {
						switch(node) {
						case MemberBindingExpressionSyntax _: canGroup = true; break; // ?.
						case AliasQualifiedNameSyntax _: break; // ::
						default:
							//AOutput.Write(model.GetTypeInfo(nodeL).Type); //null if namespace
							ISymbol s1 = null;
							if(!(nodeL is LiteralExpressionSyntax || nodeL is InterpolatedStringExpressionSyntax)) {
								s1 = model.GetSymbolInfo(nodeL).Symbol;
								ADebug.PrintIf(s1 == null, node);
							}
							canGroup = s1 == null || s1.Kind != SymbolKind.Namespace;
							break;
						}
					} else {
						canGroup = true;
					}
					//p1.Next('M');
				} else if(isCommand && !isDot) {
					if(CiUtil.IsInString(ref node, position)) {
						stringSpan = node.Span;
						stringFormat = CiUtil.GetParameterStringFormat(node, model, true);
						if(stringFormat == PSFormat.AWildex) { //is regex in wildex?
							if(code.RegexMatch(@"[\$@]*""(?:\*\*\*\w+ )?\*\*c?rc? ", 0, out RXGroup rg, RXFlags.ANCHORED, stringSpan.Start..stringSpan.End)
								&& position >= stringSpan.Start + rg.Length) stringFormat = PSFormat.ARegex;
						} else if(stringFormat == PSFormat.None) stringFormat = (PSFormat)100;
					}
				}
				return r1;
			});

			if(cancelToken.IsCancellationRequested) return;

			if(r == null) {
				if(stringFormat == (PSFormat)100) {
					var m = new AMenu { Modal = true };
					m["Regex"] = o => stringFormat = PSFormat.ARegex;
					m["Keys"] = o => stringFormat = PSFormat.AKeys;
					m.Show(doc, byCaret: true);
				}
				switch(stringFormat) {
				case PSFormat.ARegex:
				case PSFormat.ARegexReplacement:
					CodeInfo._tools.RegexWindowShow(doc, code, position, stringSpan, replace: stringFormat == PSFormat.ARegexReplacement);
					break;
				case PSFormat.AKeys:
					CodeInfo._tools.KeysWindowShow(doc, code, position, stringSpan);
					break;
				}
				return;
			}

			Debug.Assert(doc == Panels.Editor.ZActiveDoc); //when active doc changed, cancellation must be requested
			if(doc != Panels.Editor.ZActiveDoc || position != doc.Z.CurrentPos16 || (object)code != doc.Text) return; //we are async, so these changes are possible
			p1.Next('T');

			var provider = CiComplItem.Provider(r.Items[0]);

			var span = r.Span;
			if(span.Length > 0 && provider == CiComplProvider.Regex) span = new TextSpan(position, 0);

			var d = new _Data {
				completionService = completionService,
				document = document,
				model = model,
				codeLength = code.Length,
				filterText = code.Substring(span.Start, span.Length),
				items = new List<CiComplItem>(r.Items.Length),
				noAutoSelect = r.SuggestionModeItem != null,
			};

			//ADebug.PrintIf(r.SuggestionModeItem != null && r.SuggestionModeItem.ToString() != "<lambda expression>" && !r.SuggestionModeItem.ToString().NE(), r.SuggestionModeItem); //in '#if X' non-nul but empty text

			var groups = canGroup ? new Dictionary<INamespaceOrTypeSymbol, List<int>>() : null;
			List<int> keywordsGroup = null, etcGroup = null, snippetsGroup = null;
			foreach(var ci in r.Items) {
				var v = new CiComplItem(ci);
				var sym = v.FirstSymbol;
				//AOutput.Write(v.DisplayText, sym, canGroup);

				//AOutput.Write(ci.Flags); //a new internal property. Always None.

				//why cref provider adds internals from other assemblies?
				if(provider == CiComplProvider.Cref && sym != null) {
					switch(sym.Kind) {
					case SymbolKind.NamedType when v.access == CiItemAccess.Internal && !sym.IsInSource() && !model.IsAccessible(0, sym):
						//AOutput.Write(sym);
						continue;
					case SymbolKind.Namespace:
						//AOutput.Write(sym, sym.ContainingAssembly.Name);
						switch(sym.Name) {
						case "Internal" when sym.ContainingAssembly.Name == "System.Core":
						case "Windows" when sym.ContainingAssembly.Name == "mscorlib":
							continue;
						}
						break;
					}
				}

				switch(v.kind) {
				case CiItemKind.Method:
					if(sym != null) {
						if(sym.IsStatic) {
							switch(ci.DisplayText) {
							case "Equals":
							case "ReferenceEquals":
								if(sym.ContainingType.BaseType == null) { //Object
									if(isDot) continue;
									v.moveDown = CiItemMoveDownBy.Name;
								}
								break;
							case "Main" when _IsOurScriptClass(sym.ContainingType):
								v.moveDown = CiItemMoveDownBy.Name;
								break;
							}
						} else {
							switch(ci.DisplayText) {
							case "Equals":
							case "GetHashCode":
							case "GetType":
							case "ToString":
							case "MemberwiseClone":
							case "GetEnumerator": //IEnumerable
							case "CompareTo": //IComparable
							case "GetTypeCode": //IConvertible
							case "CreateObjRef": //MarshalByRefObject
							case "GetLifetimeService": //MarshalByRefObject
							case "InitializeLifetimeService": //MarshalByRefObject
							case "Clone" when sym.ContainingType.Name == "String": //this useless method would be the first in the list
							case "OnUnhandledException" when sym.ContainingType.Name == "AScript":
								v.moveDown = CiItemMoveDownBy.Name;
								break;
							}
							//var ct = sym.ContainingType;
							//AOutput.Write(ct.ToString(), ct.Name, ct.ContainingNamespace.ToString(), ct.BaseType);
						}
					}
					break;
				case CiItemKind.Namespace:
					switch(ci.DisplayText) {
					case "Accessibility":
					case "UIAutomationClientsideProviders":
						v.moveDown = CiItemMoveDownBy.Name;
						break;
					}
					break;
				case CiItemKind.TypeParameter:
					if(sym == null && ci.DisplayText == "T") continue;
					break;
				case CiItemKind.Class:
					if(!isDot && sym is INamedTypeSymbol ins && _IsOurScriptClass(ins)) v.moveDown = CiItemMoveDownBy.Name;
					break;
				case CiItemKind.EnumMember:
				case CiItemKind.Label:
					canGroup = false;
					break;
				case CiItemKind.LocalVariable:
					if(isDot) continue; //see the bug comments below
					break;
				}

				static bool _IsOurScriptClass(INamedTypeSymbol t) => t.Name == "Script" && t.BaseType?.Name == "AScript";

				if(sym != null && v.kind != CiItemKind.LocalVariable && v.kind != CiItemKind.Namespace && v.kind != CiItemKind.TypeParameter) {
					bool isObsolete = false;
					foreach(var k in sym.GetAttributes()) { //fast
						switch(k.AttributeClass.Name) {
						case "ObsoleteAttribute": isObsolete = true; break;
						}
						//AOutput.Write(ci.DisplayText, v.AttributeClass.Name);
					}
					if(isObsolete) v.moveDown = CiItemMoveDownBy.Obsolete;
				}

				d.items.Add(v);
			}

			//add snippets
			if(!isDot && canGroup && (provider == CiComplProvider.Symbol || provider == CiComplProvider.Keyword) && !d.noAutoSelect) {
				CiSnippets.AddSnippets(d.items, span, root, code);
			}

			if(d.items.Count == 0) return;

			if(canGroup) {
				for(int i = 0; i < d.items.Count; i++) {
					var v = d.items[i];
					var sym = v.FirstSymbol;
					if(sym == null) {
						if(v.kind == CiItemKind.Keyword) (keywordsGroup ??= new List<int>()).Add(i);
						else if(v.kind == CiItemKind.Snippet) (snippetsGroup ??= new List<int>()).Add(i);
						else (etcGroup ??= new List<int>()).Add(i);
					} else {
						INamespaceOrTypeSymbol nts;
						if(!isDot) nts = sym.ContainingNamespace;
						//else if(sym is ReducedExtensionMethodSymbol em) nts = em.ReceiverType; //rejected. Didn't work well, eg with linq.
						else nts = sym.ContainingType;

						//Roslyn bug: sometimes adds some garbage items.
						//To reproduce: invoke global list. Then invoke list for a string variable. Adds String, Object, all local string variables, etc. Next time works well. After Enum dot adds the enum type, even in VS; in VS sometimes adds enum methods and extmethods.
						//ADebug.PrintIf(nts == null, sym.Name);
						if(nts == null) continue;

						if(groups.TryGetValue(nts, out var list)) list.Add(i); else groups.Add(nts, new List<int> { i });
					}
				}
			}

			List<string> groupsList = null;
			if(canGroup && groups.Count + (keywordsGroup == null ? 0 : 1) + (etcGroup == null ? 0 : 1) + (snippetsGroup == null ? 0 : 1) > 1) {
				//foreach(var v in groups) AOutput.Write(v.Key, v.Value.Count, v.Key.ContainingAssembly?.Name);
				List<(string, List<int>)> g = null;
				if(isDot) {
					var gs = groups.ToList();
					gs.Sort((k1, k2) => {
						//let extension methods be at bottom, sorted by type name
						int em1 = d.items[k1.Value[0]].kind == CiItemKind.ExtensionMethod ? 1 : 0;
						int em2 = d.items[k2.Value[0]].kind == CiItemKind.ExtensionMethod ? 1 : 0;
						int diff = em1 - em2;
						if(diff != 0) return diff;
						if(em1 == 1) return string.Compare(k1.Key.Name, k2.Key.Name, StringComparison.OrdinalIgnoreCase);
						//sort non-extension members by inheritance
						var t1 = k1.Key as INamedTypeSymbol; var t2 = k2.Key as INamedTypeSymbol;
						if(_IsBase(t1, t2)) return -1;
						if(_IsBase(t2, t1)) return 1;
						ADebug.Print($"{t1}, {t2}, {k1.Value.Count}, {k2.Value.Count}"); //usually because of the above bug
						return 0;
					});

#if true
					if(gs[0].Key.Name == "String") { //don't group Au extension methods
						for(int i = 1; i < gs.Count; i++) {
							if(d.items[gs[i].Value[0]].kind != CiItemKind.ExtensionMethod) continue;
							var ns = gs[i].Key.ContainingNamespace;
							if(ns.Name == "Au") {
								gs[0].Value.AddRange(gs[i].Value);
								gs.RemoveAt(i);
								break;
							}
						}
					}
#else
					if(!Program.Settings.ci_complGroupEM) { //don't group non-Linq extension methods
						for(int i = 1; i < gs.Count; i++) {
							if(d.items[gs[i].Value[0]].kind != CiItemKind.ExtensionMethod) continue;
							var ns = gs[i].Key.ContainingNamespace;
							if(ns.Name != "Linq") {
								gs[0].Value.AddRange(gs[i].Value);
								gs.RemoveAt(i--);
							}
						}
					}
#endif

					g = gs.Select(o => (o.Key.Name, o.Value)).ToList(); //list<(itype, list)> -> list<typeName, list>

					static bool _IsBase(INamedTypeSymbol t, INamedTypeSymbol tBase)
					{
						for(t = t.BaseType; t != null; t = t.BaseType) if(t == tBase) return true;
						return false;
					}
				} else {
					g = groups.Select(o => (o.Key.QualifiedName(), o.Value)).ToList(); //dictionary<inamespace, list> -> list<namespaceName, list>
					for(int i = g.Count - 1; i > 0; i--) { //join duplicate namespace names. If n assemblies have the same namespace, there are n namespace objects.
						var nsName = g[i].Item1;
						for(int j = 0; j < i; j++) if(g[j].Item1 == nsName) { g[j].Item2.AddRange(g[i].Item2); g.RemoveAt(i); break; }
					}
					//AOutput.Write("----");
					//foreach(var v in g) AOutput.Write(v.Item1, v.Item2.Count);
					g.Sort((e1, e2) => string.Compare(e1.Item1, e2.Item1, StringComparison.OrdinalIgnoreCase));
				}
				if(keywordsGroup != null) g.Add(("keywords", keywordsGroup));
				if(snippetsGroup != null) g.Add(("snippets", snippetsGroup));
				if(etcGroup != null) g.Add(("etc", etcGroup));
				for(int i = 0; i < g.Count; i++) {
					foreach(var v in g[i].Item2) d.items[v].group = i;
				}
				groupsList = g.Select(o => o.Item1).ToList();
			}

			if(!span.IsEmpty) _FilterItems(d);
			p1.Next('F');

			d.tempRange = doc.ZTempRanges_Add(this, span.Start, span.End, () => {
				//AOutput.Write("leave", _data==d);
				if(_data == d) _CancelList(tempRangeRemoved: true);
			}, position == span.End ? SciCode.ZTempRangeFlags.LeaveIfPosNotAtEndOfRange : 0);

			_data = d;
			if(_popupList == null) {
				_popupList = new CiPopupList(this);
				_popupList.PopupWindow.ZHiddenOrDestroyed += destroyed => _CancelList(popupListHidden: true);
			}
			_popupList.SetListItems(_data.items, groupsList); //and calls SelectBestMatch
			_popupList.Show(doc, span.Start);
		}
		catch(OperationCanceledException) { /*ADebug.Print("canceled");*/ return; }
		finally {
			if(_data == null) {
				p1.Next('z');
				//AOutput.Write($"{p1.ToString()}  |  ch='{(ch == default ? "" : ch.ToString())}', canceled={cancelTS.IsCancellationRequested}");
			}
			cancelTS.Dispose();
			if(cancelTS == _cancelTS) _cancelTS = null;
		}
	}

	static void _FilterItems(_Data d)
	{
		var filterText = d.filterText;
		foreach(var v in d.items) {
			v.hidden &= ~CiItemHiddenBy.FilterText;
			v.hilite = 0;
			v.moveDown &= ~CiItemMoveDownBy.FilterText;
		}
		if(!filterText.NE()) {
			string textLower = filterText.ToLower(), textUpper = filterText.Upper();
			char c0Lower = textLower[0], c0Upper = textUpper[0];
			foreach(var v in d.items) {
				if(v.kind == CiItemKind.None) continue; //eg regex completion
				var s = v.ci.FilterText;
				//ADebug.PrintIf(v.ci.FilterText != v.ci.DisplayText, $"{v.ci.FilterText}, {v.ci.DisplayText}");
				//AOutput.Write(v.ci.DisplayText, v.ci.FilterText, v.ci.SortText, v.ci.ToString());
				bool found = false;
				int iFirst = _FilterFindChar(s, 0, c0Lower, c0Upper), iFirstFirst = iFirst;
				if(iFirst >= 0) {
					if(filterText.Length == 1) {
						_HiliteChar(iFirst);
					} else {
						while(!s.Eq(iFirst, filterText, true)) {
							iFirst = _FilterFindChar(s, iFirst + 1, c0Lower, c0Upper);
							if(iFirst < 0) break;
						}
						if(iFirst >= 0) {
							_HiliteSubstring(iFirst);
						} else { //has all uppercase chars? Eg add OneTwoThree if text is "ott" or "ot" or "tt".
							_HiliteChar(iFirstFirst);
							for(int i = 1, j = iFirstFirst + 1; i < filterText.Length; i++, j++) {
								j = _FilterFindChar(s, j, textLower[i], textUpper[i], camel: true);
								if(j < 0) { found = false; break; }
								_HiliteChar(j);
							}
						}
					}
				}

				void _HiliteChar(int i)
				{
					found = true;
					if(i < 64) v.hilite |= 1UL << i;
				}

				void _HiliteSubstring(int i)
				{
					found = true;
					int to = Math.Min(i + filterText.Length, 64);
					while(i < to) v.hilite |= 1UL << i++;
				}

				if(!found) {
					if(filterText.Length > 1 && (iFirst = s.Find(filterText, true)) >= 0) {
						v.moveDown |= CiItemMoveDownBy.FilterText; //sort bottom
						_HiliteSubstring(iFirst);
					} else v.hidden |= CiItemHiddenBy.FilterText;
				}

				//if DisplayText != FilterText, correct or clear hilites. Eg cref.
				if(found && s != v.ci.DisplayText) {
					//AOutput.Write(v.ci.DisplayText, v.ci.FilterText);
					iFirst = v.ci.DisplayText.Find(s);
					if(iFirst < 0) v.hilite = 0; else v.hilite <<= iFirst;
				}
			}
		}
	}

	/// <summary>
	/// Finds character in s where it is one of: uppercase; lowercase after '_'/'@'; not uppercase/lowercase; any at i=0.
	/// </summary>
	/// <param name="s"></param>
	/// <param name="i">Start index.</param>
	/// <param name="cLower">Lowercase version of character to find.</param>
	/// <param name="cUpper">Uppercase version of character to find.</param>
	/// <param name="camel">Uppercase must not be preceded and followed by uppercase.</param>
	static int _FilterFindChar(string s, int i, char cLower, char cUpper, bool camel = false)
	{
		for(; i < s.Length; i++) {
			char c = s[i];
			if(c == cUpper) { //any not lowercase
				if(!camel) return i;
				if(i == 0 || !char.IsUpper(c)) return i;
				if(!char.IsUpper(s[i - 1])) return i;
				if(i + 1 < s.Length && char.IsLower(s[i + 1])) return i;
			}
			if(c == cLower) { //lowercase
				if(i == 0) return i;
				switch(s[i - 1]) { case '_': case '@': return i; }
			}
		}
		return -1;
	}

	public void SelectBestMatch(IEnumerable<CompletionItem> listItems)
	{
		CiComplItem ci = null;
		var filterText = _data.filterText;
		if(!(_data.noAutoSelect || filterText == "_")) { //noAutoSelect when lambda argument

			//rejected. Need FilterItems anyway, eg to select enum type or 'new' type.
			//if(filterText.NE()) {
			//	_popupList.SelectFirstVisible();
			//	return;
			//}

			//APerf.First();
			var visible = listItems.ToImmutableArray();
			if(!visible.IsEmpty) {
				//APerf.Next();
				var fi = _data.completionService.FilterItems(_data.document, visible, filterText);
				//APerf.Next();
				//AOutput.Write(fi);
				//AOutput.Write(visible.Length, fi.Length);
				if(!fi.IsDefaultOrEmpty) if(fi.Length < visible.Length || filterText.Length > 0 || visible.Length == 1) ci = _data.Map[fi[0]];
			}
			//APerf.NW('b');
		}
		_popupList.SelectedItem = ci;
	}

	public string GetDescriptionHtml(CiComplItem ci, int iSelect)
	{
		if(_data == null) return null;
		switch(ci.kind) {
		case CiItemKind.Keyword: return CiHtml.KeywordToHtml(ci.DisplayText);
		case CiItemKind.Label: return CiHtml.LabelToHtml(ci.DisplayText);
		case CiItemKind.Snippet: return CiSnippets.GetDescriptionHtml(ci);
		}
		var symbols = ci.ci.Symbols;
		if(symbols != null) return CiHtml.SymbolsToHtml(symbols, iSelect, _data.model, _data.tempRange.CurrentFrom);
		ADebug.PrintIf(ci.kind != CiItemKind.None, ci.kind); //None if Regex
		var r = _data.completionService.GetDescriptionAsync(_data.document, ci.ci).Result; //fast if Regex, else not tested
		return r == null ? null : CiHtml.TaggedPartsToHtml(r.TaggedParts);
	}

	/// <summary>
	/// Inserts the replacement text of the completion item.
	/// ch == default if clicked or pressed Enter or Tab or a hotkey eg Ctrl+Enter.
	/// key == default if clicked or typed a character (except Tab and Enter). Does not include hotkey modifiers.
	/// </summary>
	CiComplResult _Commit(SciCode doc, CiComplItem item, char ch, Keys key)
	{
		if(item.IsRegex) { //can complete only on click or Tab
			if(ch != default || !(key == default || key == Keys.Tab)) return CiComplResult.None;
		}
		bool isSpace; if(isSpace = ch == ' ') ch = default;
		int codeLenDiff = doc.Len16 - _data.codeLength;

		if(item.kind == CiItemKind.Snippet) {
			if(ch != default && ch != '(') return CiComplResult.None;
			CiSnippets.Commit(doc, item, codeLenDiff);
			return CiComplResult.Complex;
		}

		var z = doc.Z;
		var ci = item.ci;
		var change = _data.completionService.GetChangeAsync(_data.document, ci).Result;
		//note: don't use the commitCharacter parameter. Some providers, eg XML doc, always set IncludesCommitCharacter=true, even when commitCharacter==null, but may include or not, and may include inside text or at the end.

		var s = change.TextChange.NewText;
		var span = change.TextChange.Span;
		int i = span.Start, len = span.Length + codeLenDiff;
		//AOutput.Write($"{change.NewPosition.HasValue}, cp={z.CurrentPosChars}, i={i}, len={len}, span={span}, repl='{s}'    filter='{_data.filterText}'");
		//AOutput.Write($"'{s}'");
		bool isComplex = change.NewPosition.HasValue;
		if(isComplex) { //xml doc, override, regex
			if(ch != default) return CiComplResult.None;
			//ci.DebugPrint();
			int newPos = change.NewPosition ?? (i + len);
			switch(APath.GetExtension(ci.ProviderName)) {
			case ".OverrideCompletionProvider":
				newPos = -1; //difficult to calculate and not useful

				//Replace 4 spaces with tab. Make { in same line.
				s = s.Replace("    ", "\t").RegexReplace(@"\R\t*\{", " {", 1);
				//Correct indentation. 
				int indent = 0, indent2 = z.LineIndentationFromPos(true, _data.tempRange.CurrentFrom);
				for(int j = s.IndexOf('\t'); (uint)j < s.Length && s[j] == '\t'; j++) indent++;
				if(indent > indent2) s = s.RegexReplace("(?m)^" + new string('\t', indent - indent2), "");
				break;
			case ".XmlDocCommentCompletionProvider" when !s.Ends('>') && s.RegexMatch(@"^<?(\w+)", 1, out string tag):
				if(s == tag || (ci.Properties.TryGetValue("AfterCaretText", out var s1) && s1.NE())) newPos++;
				s += "></" + tag + ">";
				break;
			}
			z.ReplaceRange(true, i, i + len, s);
			if(newPos >= 0) z.Select(true, newPos, newPos, makeVisible: true);
			return CiComplResult.Complex;
		}
		ADebug.PrintIf(i != _data.tempRange.CurrentFrom && !item.IsRegex, $"{_data.tempRange.CurrentFrom}, {i}");
		//ci.DebugPrint();

		//if typed space after method or keyword 'if' etc, replace the space with '(' etc. Also add if pressed Tab or Enter.
		CiAutocorrect.EBraces bracesOperation = default;
		int positionBack = 0, bracesFrom = 0, bracesLen = 0;
		bool isEnter = key == Keys.Enter;
		//ci.DebugPrint();
		if(s.FindAny("({[<") < 0) {
			if(ch == default) { //completed with Enter, Tab, Space or click
				string s2 = null;
				switch(item.kind) {
				case CiItemKind.Method:
				case CiItemKind.ExtensionMethod:
					ch = '(';
					break;
				case CiItemKind.Keyword:
					string name = item.DisplayText;
					switch(name) {
					case "nameof":
					case "sizeof":
					case "typeof":
						ch = '(';
						break;
					case "for":
					case "foreach":
					case "while":
					case "lock":
					case "catch":
					case "if" when !_IsDirective():
					case "fixed" when _IsInFunction(): //else probably a fixed array field
					case "using" when isEnter && _IsInFunction(): //else directive or without ()
					case "when" when _IsInAncestorNode(i, n => (n is CatchClauseSyntax, n is SwitchSectionSyntax)): //catch(...) when
						ch = '(';
						s2 = " ()"; //users may prefer space, like 'if (i<1)'. If not, let they type '(' instead.
						break;
					case "try":
					case "finally":
					case "get" when isEnter:
					case "set" when isEnter:
					case "add" when isEnter:
					case "remove" when isEnter:
					case "do" when isEnter:
					case "unsafe" when isEnter:
					case "else" when isEnter && !_IsDirective():
						ch = '{';
						break;
					case "checked":
					case "unchecked":
						ch = isEnter ? '{' : '(';
						break;
					case "switch":
						//is it switch statement or switch expression? Difficult to detect. Detect some common cases.
						if(i > 0 && CodeInfo.GetContextWithoutDocument(out var cd, i)) {
							if(cd.code[i - 1] == ' ' && cd.GetDocument()) {
								var node = cd.document.GetSyntaxRootAsync().Result.FindToken(i - 1).Parent;
								//AOutput.Write(node.Kind(), i, node.Span, node);
								if(node.SpanStart < i) switch(node) { case ExpressionSyntax _: case BaseArgumentListSyntax _: ch = '{'; break; } //expression
							}
							if(ch == default) goto case "for";
						}
						break;
					default:
						if(0 != name.Eq(false, s_kwType)) _NewExpression();
						break;
					}
					break;
				case CiItemKind.Class:
				case CiItemKind.Structure:
					if(ci.DisplayTextSuffix == "<>") ch = '<';
					else _NewExpression();
					break;
				}

				bool _IsInFunction() => _IsInAncestorNodeOfType<BaseMethodDeclarationSyntax>(i);

				bool _IsDirective() => doc.Text.Eq(i - 1, "#"); //info: CompletionItem of 'if' and '#if' are identical. Nevermind: this code does not detect '# if'.

				//If 'new Type', adds '()'.
				//If then the user types '[' for 'new Type[]' or '{' for 'new Type { initializers }', autocorrection will replace the '()' with '[]' or '{  }'.
				void _NewExpression()
				{
					if(!CodeInfo.GetDocumentAndFindNode(out _, out var node, i)) return;
					var p1 = node.Parent; if(p1 is QualifiedNameSyntax) p1 = p1.Parent;
					if(p1 is ObjectCreationExpressionSyntax) {
						ch = '(';
						bracesOperation = CiAutocorrect.EBraces.NewExpression;
					}
				}

				if(isComplex = ch != default) {
					if(ch == '{') {
						if(isEnter) {
							int indent = z.LineIndentationFromPos(true, i);
							var b = new StringBuilder(" {\r\n");
							b.Append('\t', indent + 1);
							b.Append("\r\n").Append('\t', indent).Append('}');
							s2 = b.ToString();
							positionBack = indent + 3;
						} else {
							s2 = " {  }";
							positionBack = 2;
						}
						bracesFrom = i + s.Length + 2;
						bracesLen = s2.Length - 3;
					} else if((isSpace || !Program.Settings.ci_complParenSpace) && !_data.noAutoSelect && !doc.Text.Eq(i + len, ch)) { //info: noAutoSelect when lambda argument
						s2 ??= ch == '(' ? "()" : "<>";
						positionBack = 1;
						bracesFrom = i + s.Length + s2.Length - 1;
					} else {
						ch = default;
						s2 = null;
					}
					s += s2;
				}
			} else if(!(ch == '(' || ch == '<' || _data.noAutoSelect)) { //completed with ';' or ',' or '.' or '?' or '-' or any other non-identifier char space, Tab, Enter
				switch(item.kind) {
				case CiItemKind.Method:
				case CiItemKind.ExtensionMethod:
					s += "()";
					break;
				}
			}
		}

		if(!isComplex && s == _data.filterText) return CiComplResult.None;

		z.SetAndReplaceSel(true, i, i + len, s);
		if(isComplex) {
			if(positionBack > 0) z.CurrentPos16 = i + s.Length - positionBack;
			if(bracesFrom > 0) CodeInfo._correct.BracesAdded(doc, bracesFrom, bracesFrom + bracesLen, bracesOperation);
			if(ch == '(' || ch == '<') CodeInfo._signature.SciCharAdded(doc, ch);
			return CiComplResult.Complex;
		}

		return CiComplResult.Simple;
	}

	bool _IsInAncestorNodeOfType<T>(int pos) where T : SyntaxNode
		=> CodeInfo.GetDocumentAndFindNode(out _, out var node, pos) && null != node.GetAncestor<T>();

	bool _IsInAncestorNode(int pos, Func<SyntaxNode, (bool yes, bool no)> f)
	{
		if(!CodeInfo.GetDocumentAndFindNode(out _, out var node, pos)) return false;
		while((node = node.Parent) != null) {
			//CiUtil.PrintNode(node);
			var (yes, no) = f(node);
			if(yes) return true;
			if(no) return false;
		}
		return false;
	}

	static string[] s_kwType = { "string", "object", "int", "uint", "long", "ulong", "byte", "sbyte", "short", "ushort", "char", "bool", "double", "float", "decimal" };

	/// <summary>
	/// Double-clicked item in list.
	/// </summary>
	public void Commit(SciCode doc, CiComplItem item) => _Commit(doc, item, default, default);

	/// <summary>
	/// Tab, Enter, Shift+Enter, Ctrl+Enter, Ctrl+;.
	/// </summary>
	public CiComplResult OnCmdKey_Commit(SciCode doc, Keys keyData)
	{
		var R = CiComplResult.None;
		if(_data != null) {
			var ci = _popupList.SelectedItem;
			if(ci != null) {
				R = _Commit(doc, ci, default, keyData & Keys.KeyCode);
				if(R == CiComplResult.None && keyData == Keys.Tab) R = CiComplResult.Simple; //always suppress Tab
			}
			_CancelList();
		}
		return R;
	}

	/// <summary>
	/// Esc, Arrow, Page.
	/// </summary>
	public bool OnCmdKey_SelectOrHide(Keys keyData) => _data == null ? false : _popupList.OnCmdKey(keyData);
}

[Flags]
enum CiItemHiddenBy : byte { FilterText = 1, Kind = 2 }

[Flags]
enum CiItemMoveDownBy : sbyte { Name = 1, Obsolete = 2, FilterText = 4 }

class CiComplItem
{
	public readonly CompletionItem ci;
	public readonly CiItemKind kind;
	public readonly CiItemAccess access;
	public CiItemHiddenBy hidden;
	public CiItemMoveDownBy moveDown;
	public int group;
	public ulong hilite; //bits for max 64 characters
	string _text;

	public string DisplayText => _text ??= ci.DisplayText + ci.DisplayTextSuffix;

	public Bitmap KindImage => CiUtil.GetKindImage(kind);

	public Bitmap AccessImage => CiUtil.GetAccessImage(access);

	public ISymbol FirstSymbol => ci.Symbols?[0];

	public bool IsRegex => kind == CiItemKind.None && Provider(ci) == CiComplProvider.Regex;

	public static CiComplProvider Provider(CompletionItem ci)
	{
		var s = ci.ProviderName;
		int i = s.LastIndexOf('.') + 1;
		Debug.Assert(i > 0);
		s = s.Substring(i);
		//AOutput.Write(s);
		return s switch
		{
			"SymbolCompletionProvider" => CiComplProvider.Symbol,
			"KeywordCompletionProvider" => CiComplProvider.Keyword,
			"CrefCompletionProvider" => CiComplProvider.Cref,
			"EmbeddedLanguageCompletionProvider" => CiComplProvider.Regex,
			//"OverrideCompletionProvider" => CiComplProvider.Override,
			_ => CiComplProvider.Other
		};
	}

	public CiComplItem(CompletionItem ci)
	{
		this.ci = ci;
		CiUtil.TagsToKindAndAccess(ci.Tags, out kind, out access);
		//ci.DebugPrint();
	}
}

enum CiComplProvider
{
	Other,
	Symbol,
	Keyword,
	Cref,
	Regex,
	//Override,
}

enum CiComplResult
{
	/// <summary>
	/// No completion.
	/// </summary>
	None,

	/// <summary>
	/// Inserted text displayed in the popup list. Now caret is after it.
	/// </summary>
	Simple,

	/// <summary>
	/// Inserted more text than displayed in the popup list, eg "(" or "{  }" or override. Now caret probably is somewhere in middle of it. Also if regex.
	/// Only if ch == ' ', '\n' (Enter) or default (Tab).
	/// </summary>
	Complex,
}

#if PREMATURE_OPTIMIZATION
//Can be used to prevent GC while getting completions. Reduces the time eg from 70 to 60 ms.
//It seems Roslyn uses weak references for expensive objects, eg syntax trees and semantic model.
//Usually GC starts in the middle, and Roslyn has to rebuild them anyway.
//But Roslyn is slow in other places too, eg syntax highlighting. Need to test everything. Maybe disable GC on every added char for eg 500 ms.
struct NoGcRegion : IDisposable
{
	bool _restore;

	public NoGcRegion(long memSize)
	{
		_restore = false;
		if(AVersion.Is32BitProcess) return;
		ADebug.MemorySetAnchor_();
		//AOutput.Write(System.Runtime.GCSettings.LatencyMode);
		try { _restore = GC.TryStartNoGCRegion(memSize); }
		catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
	}

	public void Dispose()
	{
		if(_restore) {
			_restore = false;
			//AOutput.Write(System.Runtime.GCSettings.LatencyMode == System.Runtime.GCLatencyMode.NoGCRegion);
			//if(System.Runtime.GCSettings.LatencyMode == System.Runtime.GCLatencyMode.NoGCRegion) GC.EndNoGCRegion();
			try { GC.EndNoGCRegion(); } //note: need to call even if not in nogc region (then exception); else TryStartNoGCRegion will throw exception.
			catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
			ADebug.MemoryPrint_();
			ThreadPool.QueueUserWorkItem(_ => GC.Collect());
		}
	}
}
#endif
