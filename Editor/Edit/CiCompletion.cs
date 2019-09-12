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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Completion;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Snippets;

class CiCompletion
{
	CiPopupList _popupList;
	_Data _data; //not null while the popup list window is visible
	CancellationTokenSource _cancelTS;
	CiTools _tools = new CiTools();

	//public CiCompletion()
	//{
	//}

	class _Data
	{
		public CompletionService completionService;
		public Document document;
		public List<CiComplItem> items;
		public string code;
		public TextSpan span;
		public string filterText;
		Dictionary<CompletionItem, CiComplItem> _map;
		public SemanticModel model;

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

	public void Cancel()
	{
		_cancelTS?.Cancel(); _cancelTS = null;
		_CancelList();
	}

	public void HideTools()
	{
		_tools.RegexWindowHide();
	}

	void _CancelList()
	{
		if(_data == null) return;
		_data = null;
		_popupList.Hide();
	}

	public void SciPositionChanged(SciCode doc, bool modified)
	{
		//int pos = doc.ST.CurrentPosChars;
		//var node = CiTools.NodeAt(pos);
		//Print(CiTools.IsInString(ref node, pos));

		_tools.RegexWindowHideIfNotInString(doc);

		if(!modified) { //else we use SciCharAdded and SciTextChanged(SC_MOD_DELETETEXT)
			int position = doc.ST.CurrentPos;
			if(doc.Call(Sci.SCI_GETANCHOR) == position) { //if no selection
				position = doc.ST.CountBytesToChars(position);
				if(_IsAfterDot(doc.Text, position)) {
					_ShowList(_ShowReason.clickedDot, position);
				} else {
					Cancel();
				}
			}
		}
	}

	public void SciCharAdded(SciCode doc)
	{
		_CharAddedRemoved(doc, true, doc.ST.CurrentPosChars);
	}

	public void SciTextChanged(SciCode doc, in Sci.SCNotification n)
	{
		if(0 == (n.modificationType & Sci.MOD.SC_MOD_DELETETEXT)) return; //info: instead of SC_MOD_INSERTTEXT we use SciCharAdded, because now cannot replace text range
		if(_data == null) return;
		_CharAddedRemoved(doc, false, doc.ST.CountBytesToChars(n.position));
	}

	void _CharAddedRemoved(SciCode doc, bool added, int position)
	{
		var code = doc.Text;
		if(_data != null) {
			if(code.Length >= _data.code.Length - _data.span.Length
				&& code.Length - position == _data.code.Length - _data.span.End
				&& 0 == string.CompareOrdinal(code, 0, _data.code, 0, _data.span.Start)
				&& 0 == string.CompareOrdinal(code, position, _data.code, _data.span.End, code.Length - position)
				&& !_data.items[0].IsRegex
				) {
				int len = position - _data.span.Start;
				if(added) {
					char c = code[position - 1];
					if(!(char.IsLetterOrDigit(c) || c == '_' || (c == '@' && len == 1))) {
						var ci = _popupList.SelectedItem;
						if(ci != null) Commit(ci, c);
						_CancelList();
						position = doc.ST.CurrentPosChars;
					}
				}
				if(_data != null) {
					_data.filterText = code.Substring(_data.span.Start, len);
					_FilterItems(true);
					return;
				}
			} else if(!added) { //else _ShowList will do
				_CancelList();
			}
		}

		if(added) _ShowList(_ShowReason.charAdded, position);
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

	enum _ShowReason { charAdded, clickedDot, command }

	//long _debugTime;

	async void _ShowList(_ShowReason showReason, int position = -1)
	{
		//long time = ATime.PerfMilliseconds;
		//Print(_debugTime == 0 ? 0 : time - _debugTime, time);
		//_debugTime = time;

		var p1 = APerf.Create();
		Cancel();
		_cancelTS = new CancellationTokenSource();
		var cancelTS = _cancelTS;

#if NOGCREGION
		ADebug.LibMemorySetAnchor();
		//Print(GCSettings.LatencyMode);
		bool noGC = false;
		if(AVersion.Is64BitProcess) try { noGC = GC.TryStartNoGCRegion(50_000_000); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
#endif
		char debugChar = default;
		try {
			var document = CodeInfo.GetDocument();
			p1.Next('d');

			//Print(document.GetTextAsync().Result);

			var doc = Panels.Editor.ActiveDoc;
			if(position < 0) position = doc.ST.CurrentPosChars;
			var code = doc.Text;
			char ch = showReason == _ShowReason.charAdded && position > 0 ? code[position - 1] : default;
			debugChar = ch;
			var cancelToken = cancelTS.Token;
			bool isDot = false, canGroup = false; int isRegex = 0;
#if DEBUG
			if(Debugger.IsAttached) cancelToken = default;
#endif
			CompletionService completionService = null;
			SemanticModel model = null;
			//INamedTypeSymbol typeSymbol = null;
			var r = await Task.Run(async () => { //info: somehow GetCompletionsAsync etc are not async
				completionService = CompletionService.GetService(document);

				//var q = completionService.GetRules();
				//Print(q.DefaultEnterKeyRule, q.DismissIfEmpty, q.DismissIfLastCharacterDeleted, q.SnippetsRule);
				//Print(q.DefaultCommitCharacters);

				//p1.Next('s');
				if(cancelToken.IsCancellationRequested) return null;
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
					} else if(isDot = (pos2 > 1 && (code[pos2] == '>' && code[pos2 - 1] == '-') || (code[pos2] == ':' && code[pos2 - 1] == ':'))) {
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
								if(!can.IsEmpty) u = can[0];
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
			Debug.Assert(doc == Panels.Editor.ActiveDoc); //when active doc changed, cancellation must be requested
			p1.Next('T');

			var span = r.Span;
			if(span.Length > 0 && CiComplItem.IsRegexProvider(r.Items[0])) span = new TextSpan(position, 0);

			_data = new _Data {
				document = document,
				completionService = completionService,
				code = code,
				span = span,
				filterText = code.Substring(span.Start, span.Length),
				items = new List<CiComplItem>(r.Items.Length),
				model = model
			};

			var groups = canGroup ? new Dictionary<INamespaceOrTypeSymbol, List<int>>() : null;
			List<int> keywordsGroup = null, etcGroup = null;
			foreach(var ci in r.Items) {
				var v = new CiComplItem(ci);
				var sym = v.FirstSymbol;
				//Print(v.DisplayText, sym, canGroup);
				switch(v.kind) {
				case CiItemKind.Method:
					if(sym != null) {
						if(sym.IsStatic) {
							switch(ci.DisplayText) {
							case "Equals":
							case "ReferenceEquals":
								if(sym.ContainingType.BaseType == null) if(isDot) continue; else v.moveDown = CiItemMoveDownBy.Name;
								break;
							case "Main" when sym.ContainingType.Name == "Script":
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
				case CiItemKind.EnumMember:
				case CiItemKind.Label:
					canGroup = false;
					break;
				case CiItemKind.LocalVariable:
					if(isDot) continue; //see the bug comments below
					break;
				}

				if(canGroup) {
					int i = _data.items.Count;
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

				_data.items.Add(v);
			}

			if(_data.items.Count == 0) {
				_data = null;
				return;
			}

			List<string> groupsList = null;
			if(canGroup && groups.Count + (keywordsGroup == null ? 0 : 1) + (etcGroup == null ? 0 : 1) > 1) {
				//foreach(var v in groups) Print(v.Key, v.Value.Count, v.Key.ContainingAssembly?.Name);
				List<(string, List<int>)> g = null;
				if(isDot) {
					var gs = groups.ToList();
					gs.Sort((k1, k2) => {
						//let extension methods be at bottom, sorted by type name
						int em1 = _data.items[k1.Value[0]].kind == CiItemKind.ExtensionMethod ? 1 : 0;
						int em2 = _data.items[k2.Value[0]].kind == CiItemKind.ExtensionMethod ? 1 : 0;
						int diff = em1 - em2;
						if(diff != 0) return diff;
						if(em1 == 1) return string.Compare(k1.Key.Name, k2.Key.Name, StringComparison.OrdinalIgnoreCase);
						//sort non-extension members by inheritance
						var t1 = k1.Key as INamedTypeSymbol; var t2 = k2.Key as INamedTypeSymbol;
						if(_IsBase(t1, t2)) return -1;
						if(_IsBase(t2, t1)) return 1;
						ADebug.Print($"{t1}, {t2}");
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
					foreach(var v in g[i].Item2) _data.items[v].group = i;
				}
				groupsList = g.Select(o => o.Item1).ToList();
			}

			if(!span.IsEmpty) _FilterItems(false);
			p1.Next('F');

			if(_popupList == null) {
				_popupList = new CiPopupList(this);
				_popupList.PopupWindow.VisibleChanged += _popupList_VisibleChanged;
			}
			_popupList.SetListItems(_data.items, groupsList, o => _GetDescription(o.ci)); //and calls SelectBestMatch
			_popupList.Show(doc, span.Start);
		}
		catch(OperationCanceledException) { /*ADebug.Print("canceled");*/ return; }
		finally {
			if(_data == null) { p1.Next('z'); Print($"{p1.ToString()}  |  ch='{(debugChar == default ? '-' : debugChar)}', canceled={cancelTS.IsCancellationRequested}"); }
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
		p1.NW();
	}

	private void _popupList_VisibleChanged(object sender, EventArgs e)
	{
		if((sender as Form).Visible) return;
		_data = null;
	}

	void _FilterItems(bool updatePopupList)
	{
		var filterText = _data.filterText;
		foreach(var v in _data.items) {
			v.hidden &= ~CiItemHiddenBy.FilterText;
			v.hilite = default;
			v.hilites = null;
			v.moveDown &= ~CiItemMoveDownBy.FilterText;
		}
		if(!Empty(filterText)) {
			string textLower = filterText.ToLower(), textUpper = filterText.Upper();
			char c0Lower = textLower[0], c0Upper = textUpper[0];
			var ah = new List<(int, int)>();
			foreach(var v in _data.items) {
				if(v.kind == CiItemKind.None) continue; //eg regex completion
														//ADebug.PrintIf(v.ci.FilterText != v.ci.DisplayText, $"{v.ci.FilterText}, {v.ci.DisplayText}");
														//Print(v.DisplayText, v.FilterText, v.SortText, v.ToString());
				var s = v.ci.FilterText;
				int iFirst = _FilterFindChar(s, 0, c0Lower, c0Upper), iFirstFirst = iFirst;
				if(iFirst >= 0) {
					if(filterText.Length == 1) {
						v.hilite = (iFirst, iFirst + 1);
					} else {
						while(!s.Eq(iFirst, filterText, true)) {
							iFirst = _FilterFindChar(s, iFirst + 1, c0Lower, c0Upper);
							if(iFirst < 0) break;
						}
						if(iFirst >= 0) {
							v.hilite = (iFirst, iFirst + filterText.Length);
						} else { //has all uppercase chars? Eg add OneTwoThree if text is "ott" or "ot" or "tt".
							ah.Clear();
							bool no = false;
							for(int i = 1, j = iFirstFirst + 1; i < filterText.Length; i++, j++) {
								j = _FilterFindChar(s, j, textLower[i], textUpper[i], camel: true);
								if(j < 0) { no = true; break; }
								if(ah.Count == 0) ah.Add((iFirstFirst, iFirstFirst + 1));
								ah.Add((j, j + 1));
							}
							if(!no) v.hilites = ah.ToArray();
						}
					}
				}
				if(v.hilite.end == 0 && v.hilites == null) {
					if(filterText.Length > 1 && (iFirst = s.Find(filterText, true)) >= 0) {
						v.moveDown |= CiItemMoveDownBy.FilterText; //sort bottom and hilite light yellow
						v.hilite = (iFirst, iFirst + filterText.Length);
					} else v.hidden |= CiItemHiddenBy.FilterText;
				}
			}
		}

		if(updatePopupList) _popupList.UpdateVisibleItems(); //and calls SelectBestMatch
	}

	/// <summary>
	/// Finds character in s where it is one of: uppercase; lowercase after '_'/'@'; not uppercase/lowercase; any at i=0.
	/// </summary>
	/// <param name="s"></param>
	/// <param name="i">Start index.</param>
	/// <param name="cLower">Lowercase version of character to find.</param>
	/// <param name="cUpper">Uppercase version of character to find.</param>
	/// <param name="camel">Uppercase must not be preceded and followed by uppercase.</param>
	int _FilterFindChar(string s, int i, char cLower, char cUpper, bool camel = false)
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

	public void SelectBestMatch()
	{
		var filterText = _data.filterText;

		//rejected. Need FilterItems anyway, eg to select enum type or 'new' type.
		//if(Empty(filterText)) {
		//	_popupList.SelectFirstVisible();
		//	return;
		//}

		//APerf.First();
		var visible = _data.items.Where(o => o.hidden == 0).Select(o => o.ci).ToImmutableArray();
		//APerf.Next();
		var fi = _data.completionService.FilterItems(_data.document, visible, filterText);
		//APerf.Next();
		//Print(fi);
		CiComplItem ci = null;
		int nv = visible.Length;
		if(fi.Length < nv || nv == 1) {
			foreach(var v in fi.Select(o => _data.Map[o])) {
				if(v.moveDown == 0) { ci = v; break; }
				if(ci == null || v.moveDown < ci.moveDown) ci = v;
			}
		}
		//APerf.NW('b');
		_popupList.SelectedItem = ci;
	}

	async Task<CompletionDescription> _GetDescription(CompletionItem v)
	{
		//bug workaround: GetDescriptionAsync sometimes gets wrong description. Then also wrong v.Properties["ContextPosition"].
		//	To reproduce: code: "var v=AWnd.Find(); w.". Invoke completions after the first dot, then after the second, then after the first. Description/Properties of method Equals will be wrong second and third time. The second time will use the ones from the first time, and third time from second.
		if(v.Properties.TryGetValue("ContextPosition", out var s)) {
			int i = s.ToInt();
			if(i < v.Span.Start || i > v.Span.End) {
				//ADebug.Print(string.Join("|", v.Properties));
				v = v.WithProperties(v.Properties.SetItem("ContextPosition", v.Span.Start.ToString()));
			}
		}
		//return await completionService.GetDescriptionAsync(document, v); //why it is not async?
		return await Task.Run(() => _data.completionService.GetDescriptionAsync(_data.document, v));
	}

	public IEnumerable<TaggedText> GetDescription(ISymbol sym)//TODO
	{
		//var p1 = APerf.Create();
		var model = _data.model;
		var formatter = _data.document.Project.Solution.Workspace.Services.GetLanguageServices(model.Language).GetService<Microsoft.CodeAnalysis.DocumentationComments.IDocumentationCommentFormattingService>();
		//p1.Next();
		var tt = sym.GetDocumentationParts(model, _data.span.Start, formatter, default);
		//p1.NW('G');
		return tt;
	}

	public void Commit(CiComplItem item, char? commitCharacter = null)
	{
		var ci = item.ci;
		var change = _data.completionService.GetChangeAsync(_data.document, ci).Result;
		//note: don't use the commitCharacter parameter. Some providers, eg XML doc, always set IncludesCommitCharacter=true, even when commitCharacter==null, but may include or not, and may include inside text or at the end.

		var doc = Panels.Editor.ActiveDoc;
		var s = change.TextChange.NewText;
		var span = change.TextChange.Span;
		int i = span.Start, len = span.Length + (doc.ST.TextLengthChars - _data.code.Length);
		//Print($"{change.NewPosition.HasValue}, cp={doc.ST.CurrentPosChars}, i={i}, len={len}, span={span}, repl='{s}'    filter='{_data.filterText}'");
		//Print($"'{s}'");
		if(change.NewPosition.HasValue) { //xml doc, override, regex
										  //ci.DebugPrint();
			int newPos = change.NewPosition.GetValueOrDefault();
			if(ci.ProviderName.Ends(".XmlDocCommentCompletionProvider") && !s.Ends('>') && s.RegexMatch(@"^<?(\w+)", 1, out string tag)) {
				if(s == tag || (ci.Properties.TryGetValue("AfterCaretText", out var s1) && Empty(s1))) newPos++;
				s += "></" + tag + ">";
			}
			doc.ST.ReplaceRange(i, len, s, SciFromTo.BothChars | SciFromTo.ToIsLength);
			doc.ST.GoToPos(newPos, true);
		} else if(s != _data.filterText) {
			if(commitCharacter.HasValue) s += commitCharacter.GetValueOrDefault(); //the undo will be better than with len--
			doc.ST.SetAndReplaceSel(i, len, s, SciFromTo.BothChars | SciFromTo.ToIsLength);
		}

		//good: when changed text/pos with the above code, we don't receive notifications, because text/pos are modified by code.
		//rejected: append () if method.
	}

	public void SciTextChangedInMeta(in Sci.SCNotification n)
	{

	}
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
	public (int start, int end) hilite;
	public (int start, int end)[] hilites;
	string _text;

	public string DisplayText => _text ??= ci.DisplayText + ci.DisplayTextSuffix;

	public Bitmap KindImage => CiUtil.GetKindImage(kind);

	public Bitmap AccessImage => CiUtil.GetAccessImage(access);

	public ISymbol FirstSymbol => ci.Symbols?[0];

	public bool IsRegex => kind == CiItemKind.None && IsRegexProvider(ci);

	public static bool IsRegexProvider(CompletionItem ci) => ci.Symbols == null && ci.Properties.Contains(new KeyValuePair<string, string>("EmbeddedProvider", "Microsoft.CodeAnalysis.Features.EmbeddedLanguages.RegularExpressions.RegexEmbeddedCompletionProvider"));

	public CiComplItem(CompletionItem ci)
	{
		this.ci = ci;
		var tags = ci.Tags;
		if(tags.Length > 0) {
			kind = tags[0] switch
			{
				WellKnownTags.Class => CiItemKind.Class,
				WellKnownTags.Structure => CiItemKind.Structure,
				WellKnownTags.Enum => CiItemKind.Enum,
				WellKnownTags.Delegate => CiItemKind.Delegate,
				WellKnownTags.Interface => CiItemKind.Interface,
				WellKnownTags.Method => CiItemKind.Method,
				WellKnownTags.ExtensionMethod => CiItemKind.ExtensionMethod,
				WellKnownTags.Property => CiItemKind.Property,
				WellKnownTags.Event => CiItemKind.Event,
				WellKnownTags.Field => CiItemKind.Field,
				WellKnownTags.Local => CiItemKind.LocalVariable,
				WellKnownTags.Parameter => CiItemKind.LocalVariable,
				WellKnownTags.RangeVariable => CiItemKind.LocalVariable,
				WellKnownTags.Constant => CiItemKind.Constant,
				WellKnownTags.EnumMember => CiItemKind.EnumMember,
				WellKnownTags.Keyword => CiItemKind.Keyword,
				WellKnownTags.Namespace => CiItemKind.Namespace,
				WellKnownTags.Label => CiItemKind.Label,
				WellKnownTags.Snippet => CiItemKind.Snippet,
				WellKnownTags.TypeParameter => CiItemKind.TypeParameter,
				_ => CiItemKind.None
			};
			ci.DebugPrintIf(kind == CiItemKind.None);
			if(ci.Tags.Length > 1) {
				access = ci.Tags[1] switch { WellKnownTags.Private => CiItemAccess.Private, WellKnownTags.Protected => CiItemAccess.Protected, WellKnownTags.Internal => CiItemAccess.Internal, _ => default };
				ci.DebugPrintIf(ci.Tags.Length > 2 || (access == default && ci.Tags[1] != WellKnownTags.Public), "green");
			}
		} else {
			kind = CiItemKind.None;
			ci.DebugPrintIf(!IsRegex);
		}
		//ci.DebugPrint();
	}
}

static class CiExt
{
	[Conditional("DEBUG")]
	public static void DebugPrint(this CompletionItem t, string color = "blue")
	{
		Print($"<><c {color}>{t.DisplayText},    {string.Join("|", t.Tags)},    prefix={t.DisplayTextPrefix},    suffix={t.DisplayTextSuffix},    filter={t.FilterText},    sort={t.SortText},    inline={t.InlineDescription},    automation={t.AutomationText},    provider={t.ProviderName}<>");
		Print(string.Join("\n", t.Properties));
	}

	[Conditional("DEBUG")]
	public static void DebugPrintIf(this CompletionItem t, bool condition, string color = "blue")
	{
		if(condition) DebugPrint(t, color);
	}

	public static string QualifiedName(this ISymbol t, bool onlyNamespace = false)
	{
		var g = s_qnStack ??= new Stack<string>();
		g.Clear();
		if(!onlyNamespace) for(var k = t; k != null; k = k.ContainingType) g.Push(k.Name);
		for(var n = t.ContainingNamespace; n != null && !n.IsGlobalNamespace; n = n.ContainingNamespace) g.Push(n.Name);
		return string.Join(".", g);
	}
	[ThreadStatic] static Stack<string> s_qnStack;
}