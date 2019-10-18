//#define NOGCREGION //Prevent GC while getting completions. In Debug build can reduce the time eg from 70 to 60 ms, but in Release almost same speed.
#define HAVE_SYMBOLS //if the Roslyn project has been modified (added Symbols property to the CompletionItem class) and compiled

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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
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
	CiTools _tools = new CiTools();

	class _Data
	{
		public CompletionService completionService;
		public Document document;
		public SemanticModel model;
		public List<CiComplItem> items;
		public int codeLength;
		public string filterText;
		public SciCode.ITempRange tempRange;
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

	public void HideTools()
	{
		_tools.RegexWindowHide();
	}

	void _CancelList(bool popupListHidden = false, bool tempRangeRemoved = false)
	{
		//Print("_CancelList", _data != null);
		if(_data == null) return;
		if(!tempRangeRemoved) _data.tempRange.Remove();
		_data = null;
		if(!popupListHidden) _popupList.Hide();
	}

	public void SciUpdateUI(SciCode doc, bool modified) //position changed, clicked, etc
	{
		//int pos = doc.Z.CurrentPosChars;
		//var node = CiTools.NodeAt(pos);
		//Print(CiTools.IsInString(ref node, pos));

		_tools.RegexWindowHideIfNotInString(doc);

		if(_data == null && !modified) { //else we use SciCharAdded and SciCharDeletedBack
			int position = doc.Z.CurrentPos8;
			if(doc.Call(Sci.SCI_GETANCHOR) == position) { //if no selection
				position = doc.Z.CountBytesToChars(position);
				if(_IsAfterDot(doc.Text, position)) {
					_ShowList(_ShowReason.clickedDot, position);
				}
			}
		}
	}

	/// <summary>
	/// If showing popup list, synchronously commits the selected item if need (inserts text).
	/// Called before <see cref="CiAutocorrect.SciCharAdded"/> and before passing the character to Scintilla.
	/// Else inserts text at caret position and now caret is after the text.
	/// </summary>
	public CiComplResult SciCharAdding_Commit(SciCode doc, char ch)
	{
		CiComplResult R = CiComplResult.None;
		if(_data != null) {
			if(!_IsValidChar(ch)) {
				var ci = _popupList.SelectedItem;
				if(ci != null) R = _Commit(doc, ci, ch, default);
				_CancelList();
				//return;
			}
		}
		return R;
	}

	static bool _IsValidChar(char ch) => char.IsLetterOrDigit(ch) || ch == '_' || ch == '@';

	/// <summary>
	/// Asynchronously shows popup list if need. If already showing, synchronously filters/selects items.
	/// Called after <see cref="SciCharAdding_Commit"/> and <see cref="CiAutocorrect.SciCharAdded"/>.
	/// </summary>
	public void SciCharAdded_ShowList(CodeInfo.CharContext c)
	{
		if(_data == null) {
			_ShowList(_cancelTS == null ? _ShowReason.charAdded : _ShowReason.charAdded2, -1, c.ch);
		}
	}

	public void SciModified(SciCode doc, in Sci.SCNotification n)
	{
		if(_data != null) {
			bool trValid = _data.tempRange.GetCurrentFromTo(out int from, out int to, utf8: true);
			Debug.Assert(trValid); if(!trValid) { Cancel(); return; }
			string s = doc.Z.RangeText(false, from, to);
			foreach(var v in s) if(!_IsValidChar(v)) return; //mostly because now is before SciCharAddedCommit, which commits (or cancels) if added invalid char
			_data.filterText = s;
			_FilterItems(_data);
			_popupList.UpdateVisibleItems(); //and calls SelectBestMatch
		}
	}

	bool _IsAfterDot(string code, int position)
	{
		if(position > 1) {
			char c = code[position - 1];
			if(c == '.') return true;
			if(position > 2) {
				char c2 = code[position - 2];
				switch(c) {
				case '>' when c2 == '-':
				case ':' when c2 == ':':
					return true;
				}
			}
		}
		return false;
	}

	public void ShowList()
	{
		_ShowList(_ShowReason.command);
	}

	enum _ShowReason { charAdded, charAdded2, clickedDot, command }

	//long _debugTime;

	async void _ShowList(_ShowReason showReason, int position = -1, char ch = default)
	{
		//long time = ATime.PerfMilliseconds;
		//Print(_debugTime == 0 ? 0 : time - _debugTime, time);
		//_debugTime = time;

		var p1 = APerf.Create();

		//Print(_cancelTS);
		Cancel();

		if(showReason != _ShowReason.charAdded) ch = default;

		if(!CodeInfo.GetContextWithoutDocument(out var cd, position)) return; //returns false if position is in meta comments
		SciCode doc = cd.sciDoc;
		position = cd.position; //changed if -1 or -2
		string code = cd.code;

		if(showReason == _ShowReason.charAdded) {
			if(position > 1 && char.IsLetterOrDigit(ch) && char.IsLetterOrDigit(code[position - 2])) return; //middle of word

			//FUTURE: try to improve performance when typing text (_ShowReason.charAdded).
			//	Return if the text would not trigger autocompletion.
			//	Eg if in comments, non-regex string. Then need to get syntax tree after GetDocument.
		}

#if NOGCREGION
		ADebug.LibMemorySetAnchor();
		//Print(GCSettings.LatencyMode);
		bool noGC = false;
		if(AVersion.Is64BitProcess) try { noGC = GC.TryStartNoGCRegion(50_000_000); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
#endif
		if(!cd.GetDocument()) return; //returns false if fails (unlikely)
		Document document = cd.document;
		Debug.Assert(code == document.GetTextAsync().Result.ToString());
		p1.Next('d');

		bool isDot = false, canGroup = false; int isRegex = 0;
		CompletionService completionService = null;
		SemanticModel model = null;
		//INamedTypeSymbol typeSymbol = null;

		_cancelTS = new CancellationTokenSource();
		var cancelTS = _cancelTS;
		var cancelToken = cancelTS.Token;
#if DEBUG
		if(Debugger.IsAttached) { cancelToken = default; _cancelTS = null; }
#endif

		try {
			var r = await Task.Run(async () => { //info: somehow GetCompletionsAsync etc are not async
				completionService = CompletionService.GetService(document);
				if(cancelToken.IsCancellationRequested) return null;

				//var q = completionService.GetRules();
				//Print(q.DefaultEnterKeyRule, q.DismissIfEmpty, q.DismissIfLastCharacterDeleted, q.SnippetsRule);
				//Print(q.DefaultCommitCharacters);
				//p1.Next('s');

				if(showReason == _ShowReason.charAdded && ch == '[') { //attribute?
					var node = cd.document.GetSyntaxRootAsync().Result.FindToken(position - 1).Parent;
					//Print(node.Kind());
					if(node.IsKind(SyntaxKind.AttributeList)) ch = default; //else GetCompletionsAsync does not work
				}

				var trigger = ch == default ? default : CompletionTrigger.CreateInsertionTrigger(ch);
				//Print($"'{ch}'");
				var r1 = await completionService.GetCompletionsAsync(document, position, trigger, cancellationToken: cancelToken).ConfigureAwait(false);
				p1.Next('C');
				if(r1 != null) {
					model = await document.GetSemanticModelAsync(cancelToken).ConfigureAwait(false);
					//INamespaceOrTypeSymbol type = null;
					int pos2 = position - 1;
					if(isDot = (pos2 > 0 && code[pos2] == '.')) {
						if(pos2 > 1 && code[pos2 - 1] == '?') pos2--;
					} else if(isDot = (pos2 > 1 && ((code[pos2] == '>' && code[pos2 - 1] == '-') || (code[pos2] == ':' && code[pos2 - 1] == ':')))) {
						pos2--;
					}
					if(isDot) {
						var tree = model.SyntaxTree;
						var node = tree.GetRoot().FindToken(pos2).Parent;
						//Print($"NODE: {node.Kind()}, {node.GetType().Name}, <<{node}>>");

						ExpressionSyntax es = null;
						switch(node) {
						case QualifiedNameSyntax s1: es = s1.Left; break; //can be namespace, type or value
						case MemberAccessExpressionSyntax s1: es = s1.Expression; break; //can be namespace, type or value
						case ConditionalAccessExpressionSyntax s1: canGroup = true; break; //can be only value
																						   //case ConditionalAccessExpressionSyntax s1: es = s1.Expression; break;
																						   //case AliasQualifiedNameSyntax s1: es = s1.Alias; break; //can be only root namespace
						}
						if(es != null) {
							canGroup = true;
							//Print("es kind:", es.Kind());
							var u = model.GetSymbolInfo(es).Symbol;
							if(u == null && model.GetTypeInfo(es).Type is IErrorTypeSymbol ee) {
								var can = ee.CandidateSymbols;
								if(!can.IsDefaultOrEmpty) u = can[0];
							}
							//Print(u, u?.Kind??default);
							if(u != null) {
								if(u.Kind == SymbolKind.Namespace) {
									canGroup = false;
									//	type = u as INamespaceSymbol;
									//} else {
									//	type = model.GetTypeInfo(es).Type;
								}
							}

							//if(type == null) {
							//	type = model.GetSpeculativeTypeInfo(pos2, es, SpeculativeBindingOption.BindAsExpression).Type;
							//}

							//if(type is IPointerTypeSymbol pts) type = pts.PointedAtType;
							//typeSymbol = type as INamedTypeSymbol;
						}
					} else {
						canGroup = true;
					}
					//p1.Next('M');
				} else if(showReason == _ShowReason.command) {
					var model = await document.GetSemanticModelAsync(cancelToken).ConfigureAwait(false);
					var root = model.SyntaxTree.GetRoot();
					var node = root.FindToken(position).Parent;
					if(CiTools.IsInString(ref node, position)) {
						isRegex = CiTools.IsInRegexString(model, node, code, position);

					}
				}
				return r1;
			});

			if(isRegex != 0) {
				_tools.RegexWindowShow(doc, position, replace: isRegex == 2);
				return;
			}

			if(r == null || cancelToken.IsCancellationRequested) return;
			Debug.Assert(doc == Panels.Editor.ZActiveDoc); //when active doc changed, cancellation must be requested
			if(doc != Panels.Editor.ZActiveDoc || position != doc.Z.CurrentPos16 || code != doc.Text) return; //we are async, so these changes are possible
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
			};

			var groups = canGroup ? new Dictionary<INamespaceOrTypeSymbol, List<int>>() : null;
			List<int> keywordsGroup = null, etcGroup = null;
			foreach(var ci in r.Items) {
				var v = new CiComplItem(ci);
				var sym = v.FirstSymbol;
				//Print(v.DisplayText, sym, canGroup);

				//why cref provider adds internals from other assemblies?
				if(provider == CiComplProvider.Cref && sym != null) {
					switch(sym.Kind) {
					case SymbolKind.NamedType when v.access == CiItemAccess.Internal && !sym.IsInSource() && !model.IsAccessible(0, sym):
						//Print(sym);
						continue;
					case SymbolKind.Namespace:
						//Print(sym, sym.ContainingAssembly.Name);
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
							case "OnUnhandledException" when sym.ContainingType.Name == "AScript":
								v.moveDown = CiItemMoveDownBy.Name;
								break;
							}
							//var ct = sym.ContainingType;
							//Print(ct.ToString(), ct.Name, ct.ContainingNamespace.ToString(), ct.BaseType);
						}
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

				if(canGroup) {
					int i = d.items.Count;
					if(sym == null) {
						if(v.kind == CiItemKind.Keyword) (keywordsGroup ??= new List<int>()).Add(i);
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

				if(sym != null && v.kind != CiItemKind.LocalVariable && v.kind != CiItemKind.Namespace && v.kind != CiItemKind.TypeParameter) {
					bool isObsolete = false;
					foreach(var k in sym.GetAttributes()) { //fast
						switch(k.AttributeClass.Name) {
						case "ObsoleteAttribute": isObsolete = true; break;
						}
						//Print(ci.DisplayText, v.AttributeClass.Name);
					}
					if(isObsolete) v.moveDown = CiItemMoveDownBy.Obsolete;
				}

				d.items.Add(v);
			}

			if(d.items.Count == 0) return;

			List<string> groupsList = null;
			if(canGroup && groups.Count + (keywordsGroup == null ? 0 : 1) + (etcGroup == null ? 0 : 1) > 1) {
				//foreach(var v in groups) Print(v.Key, v.Value.Count, v.Key.ContainingAssembly?.Name);
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
					//Print("----");
					//foreach(var v in g) Print(v.Item1, v.Item2.Count);
					g.Sort((e1, e2) => string.Compare(e1.Item1, e2.Item1, StringComparison.OrdinalIgnoreCase));
				}
				if(keywordsGroup != null) g.Add(("keywords", keywordsGroup));
				if(etcGroup != null) g.Add(("etc", etcGroup));
				for(int i = 0; i < g.Count; i++) {
					foreach(var v in g[i].Item2) d.items[v].group = i;
				}
				groupsList = g.Select(o => o.Item1).ToList();
			}

			if(!span.IsEmpty) _FilterItems(d);
			p1.Next('F');

			d.tempRange = doc.ZTempRanges_Add(this, span.Start, span.End, () => {
				//Print("leave", _data==d);
				if(_data == d) _CancelList(tempRangeRemoved: true);
			}, SciCode.ZTempRangeFlags.LeaveIfPosNotAtEndOfRange);

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
				//Print($"{p1.ToString()}  |  ch='{(ch == default ? "" : ch.ToString())}', canceled={cancelTS.IsCancellationRequested}");
			}
			cancelTS.Dispose();
			if(cancelTS == _cancelTS) _cancelTS = null;
		}
#if NOGCREGION
		finally {
			Print(noGC, GCSettings.LatencyMode == GCLatencyMode.NoGCRegion);
			//if(noGC && GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) GC.EndNoGCRegion();
			if(noGC) {
				try { GC.EndNoGCRegion(); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); } //note: need to call even if not in nogc region (then exception); else TryStartNoGCRegion will throw exception.
				ADebug.LibMemoryPrint();
				ThreadPool.QueueUserWorkItem(_ => GC.Collect());
			}
		}
#endif
		//p1.NW(); //FUTURE: remove all p1 lines
	}

	static void _FilterItems(_Data d)
	{
		var filterText = d.filterText;
		foreach(var v in d.items) {
			v.hidden &= ~CiItemHiddenBy.FilterText;
			v.hilite = 0;
			v.moveDown &= ~CiItemMoveDownBy.FilterText;
		}
		if(!Empty(filterText)) {
			string textLower = filterText.ToLower(), textUpper = filterText.Upper();
			char c0Lower = textLower[0], c0Upper = textUpper[0];
			foreach(var v in d.items) {
				if(v.kind == CiItemKind.None) continue; //eg regex completion
				var s = v.ci.FilterText;
				//ADebug.PrintIf(v.ci.FilterText != v.ci.DisplayText, $"{v.ci.FilterText}, {v.ci.DisplayText}");
				//Print(v.ci.DisplayText, v.ci.FilterText, v.ci.SortText, v.ci.ToString());
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
					//Print(v.ci.DisplayText, v.ci.FilterText);
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
		var filterText = _data.filterText;
		CiComplItem ci = null;

		//rejected. Need FilterItems anyway, eg to select enum type or 'new' type.
		//if(Empty(filterText)) {
		//	_popupList.SelectFirstVisible();
		//	return;
		//}

		//APerf.First();
		var visible = listItems.ToImmutableArray();
		if(!visible.IsEmpty) {
			//APerf.Next();
			var fi = _data.completionService.FilterItems(_data.document, visible, filterText);
			//APerf.Next();
			//Print(fi);
			//Print(visible.Length, fi.Length);
			if(!fi.IsDefaultOrEmpty) if(fi.Length < visible.Length || filterText.Length > 0 || visible.Length == 1) ci = _data.Map[fi[0]];
		}
		//APerf.NW('b');
		_popupList.SelectedItem = ci;
	}

	public string GetDescriptionHtml(CiComplItem ci, int iSelect)
	{
		if(_data == null) return null;
		var symbols = ci.ci.Symbols;
		if(symbols != null) return CiHtml.SymbolsToHtml(symbols, iSelect, _data.model, _data.tempRange.CurrentFrom);
		if(ci.kind == CiItemKind.Keyword) return CiHtml.KeywordToHtml(ci.DisplayText);
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
		if(ch == ' ') ch = default;

		var ci = item.ci;
		var change = _data.completionService.GetChangeAsync(_data.document, ci).Result;
		//note: don't use the commitCharacter parameter. Some providers, eg XML doc, always set IncludesCommitCharacter=true, even when commitCharacter==null, but may include or not, and may include inside text or at the end.

		var s = change.TextChange.NewText;
		var span = change.TextChange.Span;
		int i = span.Start, len = span.Length + (doc.Z.TextLength16 - _data.codeLength);
		//Print($"{change.NewPosition.HasValue}, cp={doc.Z.CurrentPosChars}, i={i}, len={len}, span={span}, repl='{s}'    filter='{_data.filterText}'");
		//Print($"'{s}'");
		ADebug.PrintIf(i != _data.tempRange.CurrentFrom && !item.IsRegex, $"{_data.tempRange.CurrentFrom}, {i}");
		bool isComplex = change.NewPosition.HasValue;
		if(isComplex) { //xml doc, override, regex
			if(ch != default) return CiComplResult.None;
			//ci.DebugPrint();
			int newPos = change.NewPosition ?? (i + len);
			if(ci.ProviderName.Ends(".XmlDocCommentCompletionProvider") && !s.Ends('>') && s.RegexMatch(@"^<?(\w+)", 1, out string tag)) {
				if(s == tag || (ci.Properties.TryGetValue("AfterCaretText", out var s1) && Empty(s1))) newPos++;
				s += "></" + tag + ">";
			}
			doc.Z.ReplaceRange(true, i, len, s, true);
			doc.Z.GoToPos(true, newPos);
			return CiComplResult.Complex;
		}
		//ci.DebugPrint();

		//if typed space after method or keyword 'if' etc, replace the space with '(' etc. Also add if pressed Tab or Enter.
		CiAutocorrect.EBraces bracesOperation = default;
		int positionBack = 0, bracesFrom = 0, bracesLen = 0;
		bool isEnter = key == Keys.Enter;
		//ci.DebugPrint();
		if(ch == default && s.FindChars("({[<") < 0) {
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
				case "when" when _IsInAncestorNodeOfType<CatchClauseSyntax>(i): //else switch case when
					ch = '(';
					s2 = " ()"; //users may prefer space, like 'if (i<1)'. If not, let they type '(' instead.
					break;
				case "do":
				case "try":
				case "finally":
				case "get":
				case "set":
				case "add":
				case "remove":
				case "unsafe" when isEnter:
				case "else" when !_IsDirective():
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
							//Print(node.Kind(), i, node.Span, node);
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
				if(_IsInAncestorNodeOfType<ObjectCreationExpressionSyntax>(i)) {
					ch = '(';
					bracesOperation = CiAutocorrect.EBraces.NewExpression;
				}
			}

			if(isComplex = ch != default) {
				if(ch == '{') {
					if(isEnter) {
						int indent = doc.Z.LineIndentationFromPos(true, i);
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
				} else {
					s2 ??= ch == '(' ? "()" : "<>";
					positionBack = 1;
					bracesFrom = i + s.Length + s2.Length - 1;
				}
				s += s2;
			}
		}

		if(!isComplex && s == _data.filterText) return CiComplResult.None;

		doc.Z.SetAndReplaceSel(true, i, len, s, true);
		if(isComplex) {
			if(positionBack > 0) doc.Z.CurrentPos16 = i + s.Length - positionBack;
			if(bracesFrom > 0) CodeInfo.BracesAdded(doc, bracesFrom, bracesFrom + bracesLen, bracesOperation);
			if(ch == '(' || ch == '<') CodeInfo.CompletionSignatureCharAdded(doc, ch);
			return CiComplResult.Complex;
		}

		return CiComplResult.Simple;
	}

	bool _IsInAncestorNodeOfType<T>(int pos) where T : SyntaxNode
		=> CodeInfo.GetDocumentAndFindNode(out _, out var node, pos) && null != node.GetAncestor<T>();

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
		//Print(s);
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

public enum CiComplProvider
{
	Other,
	Symbol,
	Keyword,
	Cref,
	Regex,
	//Override,
}

public enum CiComplResult
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
