//#define NOGCREGION //Prevent GC while getting completions. In Debug build can reduce the time eg from 70 to 60 ms, but in Release almost same speed.

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
using Au.Compiler;
using Au.Controls;
using Au.Editor.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Completion;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.CodeAnalysis.CSharp.SignatureHelp;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.CSharp.QuickInfo;
using TheArtOfDev.HtmlRenderer.WinForms;
using System.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.CSharp.Symbols;

class CiCompletion
{
	ATimer _delayTimer;
	CancellationTokenSource _cancelTS;
	bool _needTriggerChar;
	CiPopupList _popupList;
	string _popupText;
	CompletionService _completionService;
	CompletionList _results;
	List<CiComplItem> _items;
	int _startPos;

	public void Cancel()
	{
		_CancelWork();
		_CancelList();
	}

	void _CancelWork()
	{
		_cancelTS?.Cancel(); _cancelTS = null;
		_delayTimer?.Stop();

	}

	void _CancelList()
	{
		_popupList?.Hide();
	}

	//class _ListState
	//{

	//}

	public void SciPositionChangedNotModified()
	{
		var doc = Panels.Editor.ActiveDoc;
		if(_IsAfterDot(doc.ST.SelectionStartChars)) {
			_ShowList(needTriggerChar: false, withDelay: false);
		} else {
			Cancel();
		}
	}

	public void SciTextChanged(in Sci.SCNotification n)
	{
		bool added = 0 != (n.modificationType & Sci.MOD.SC_MOD_INSERTTEXT);
		var doc = Panels.Editor.ActiveDoc;
		int position = n.position; if(added) position += n.length; //CurrentPosChars now still old

		if(_popupText != null) {

		} else if(!added) return;

		bool dot = _IsAfterDot(doc.ST.CountBytesToChars(position));
		_ShowList(needTriggerChar: true, withDelay: !dot);
	}

	bool _IsAfterDot(int position)
	{
		if(position > 0) {
			var code = Panels.Editor.ActiveDoc.Text;
			char c = code[position - 1];
			switch(c) {
			case '.':
			case '>' when position > 2 && code[position - 2] == '-': return true;
			}
		}
		return false;
	}

	public void ShowList()
	{
		_ShowList(needTriggerChar: false, withDelay: false);
	}

	void _ShowList(bool needTriggerChar, bool withDelay)
	{
		_cancelTS?.Cancel();
		_cancelTS = new CancellationTokenSource();
		_needTriggerChar = needTriggerChar;
		int delay = withDelay ? 100 : 1;
		if(_delayTimer == null) _delayTimer = new ATimer(() => _ShowListAsync());
		_delayTimer.Start(delay, true);
	}

	async void _ShowListAsync()
	{
		APerf.First();
#if NOGCREGION
		ADebug.LibMemorySetAnchor();
		//Print(GCSettings.LatencyMode);
		bool noGC = false;
		if(AVersion.Is64BitProcess) try { noGC = GC.TryStartNoGCRegion(50_000_000); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
#endif
		try {
			var document = CodeInfo.GetDocument();
			APerf.Next('d');

			//Print(document.GetTextAsync().Result);

			var doc = Panels.Editor.ActiveDoc;
			var position = doc.ST.CurrentPosChars;
			var code = doc.Text;
			char ch = _needTriggerChar && position > 0 ? code[position - 1] : default;
			var cancelToken = _cancelTS.Token;
			CompletionService completionService = null;
			Dictionary<string, object> d = null;
			INamedTypeSymbol typeSymbol = null;
			var r = await Task.Run(async () => { //info: somehow GetCompletionsAsync etc are not async
				completionService = CompletionService.GetService(document);
				//APerf.Next('s');
				if(cancelToken.IsCancellationRequested) return null;
				var trigger = ch == default ? default : CompletionTrigger.CreateInsertionTrigger(ch);
				var r1 = await completionService.GetCompletionsAsync(document, position, trigger, cancellationToken: cancelToken).ConfigureAwait(false);
				APerf.Next('C');
				if(r1 != null) {

					//foreach(var v in r1.Items) {
					//	var prov = v.ProviderName;
					//	if(!prov.Ends(".SymbolCompletionProvider"))
					//		Print(v.DisplayText, v.ProviderName);
					//}
					//Print("----------");

					//foreach(var v in r1.Items) {
					//	//Print(v.DisplayText, v.ProviderName, string.Join("|", v.Properties), Microsoft.CodeAnalysis.Completion.Providers.SymbolCompletionItem.HasSymbols(v));
					//	Print(v.DisplayText, v.Properties.Count, string.Join("|", v.Properties));
					//	//Print(v.DisplayText, Microsoft.CodeAnalysis.Completion.Providers.SymbolCompletionItem.GetSymbolsAsync(v, document, default).Result.Length);
					//}

					var model = await document.GetSemanticModelAsync(cancelToken).ConfigureAwait(false); //fast
					INamespaceOrTypeSymbol type = null;
					bool onlyStatic = false, onlyInstance=false, isNamespace = false;
					if(position > 1 && code[position - 1] == '.') { //TODO: or ->
						var tree = model.SyntaxTree;
#if true
						int pos2 = position - 2; if(code[pos2] == '?' && pos2 > 0) pos2--;
						pos2++;
						var node = tree.GetRoot().FindToken(pos2).Parent;
						Print(node, node.GetType());
						if(node is MemberAccessExpressionSyntax ma) {
							ExpressionSyntax es = ma.Expression;
							Print(es.Kind());
							var op = model.GetOperation(es);
							if(op != null) Print("OPERATION", op, op.Kind, op.Type); //ILocalReferenceOperation
							var u = model.GetSymbolInfo(es).Symbol;
							if(u != null) Print("SYMBOL", u.Name, u.MetadataName, u.Kind, u.IsStatic, u.ContainingAssembly?.Name, u.ContainingNamespace, u.ContainingSymbol, u.ContainingType, u.IsDefinition, u.IsOverride, u.Locations[0]);
							if(u == null) {
								var oper = model.GetOperation(es);
								if(oper != null && oper.Kind == OperationKind.Literal) { type = oper.Type; onlyInstance = true; } //eg "string".
							} else if(u.Kind == SymbolKind.Namespace) {
								isNamespace = true;
								//onlyStatic = true; //same results but slower
								type = u as INamespaceSymbol;
							} else {
								if(u.Kind == SymbolKind.NamedType) onlyStatic = true; else onlyInstance = true;
								type = model.GetTypeInfo(es).Type;
								typeSymbol = type as INamedTypeSymbol;
								//Print("typeSymbol", typeSymbol);
							}
						}
#else
						int pos2 = position - 2; if(code[pos2] == '?' && pos2 > 0) pos2--;
						var node = tree.GetRoot().FindToken(pos2).Parent;
						Print(node, node.GetType());
						if(node is ExpressionSyntax es) {
							var op = model.GetOperation(es);
							//OperationKind.
							if(op != null) Print("OPERATION", op, op.Kind, op.Type); //ILocalReferenceOperation
							var u = model.GetSymbolInfo(es).Symbol;
							if(u != null) Print("SYMBOL", u.Name, u.MetadataName, u.Kind, u.IsStatic, u.ContainingAssembly?.Name, u.ContainingNamespace, u.ContainingSymbol, u.ContainingType, u.IsDefinition, u.IsOverride, u.Locations[0]);
							if(u == null) {
							} else if(u.Kind == SymbolKind.Namespace) {
								isNamespace = true;
								//onlyStatic = true; //same results but slower
								type = u as INamespaceSymbol;
							} else {
								if(u.Kind == SymbolKind.NamedType) onlyStatic = true;
								type = model.GetTypeInfo(node).Type;
							}
							//Print(type);
						}
#endif
					}
					Print(type);
					var p1 = APerf.Create();
					var symbols = isNamespace ?
					model.LookupNamespacesAndTypes(position, type)
						: (onlyStatic
							? model.LookupStaticMembers(position, type)
							: model.LookupSymbols(position, type, includeReducedExtensionMethods: true));
					p1.NW('L');
					if(cancelToken.IsCancellationRequested) return null;
					//APerf.Next('M');

					Print(symbols.Length);
					if(AKeys.IsScrollLock) {
						Print(symbols);
						Print("----------");
					}
					//foreach(var v in r1.Items) {
					//	//CompletionItem
					//	Print(v.DisplayText, v.ProviderName, string.Join("|", v.Properties));
					//}

					d = new Dictionary<string, object>(symbols.Length);
					foreach(var sy in symbols) {
						if(onlyInstance && sy.IsStatic) continue;
						string name = sy.Name;
						if(d.TryGetValue(name, out var o)) {
							switch(o) {
							case ISymbol sy2: d[name] = new List<ISymbol> { sy2, sy }; break;
							case List<ISymbol> list: list.Add(sy); break;
							}
						} else d.Add(name, sy);
					}
					//APerf.Next('m');
				}
				return r1;
			});
			if(r == null || cancelToken.IsCancellationRequested) { _CancelWork(); APerf.NW('z'); return; }
			Debug.Assert(doc == Panels.Editor.ActiveDoc); //when active doc changed, cancellation must be requested
			APerf.Next('T');

			var span = r.Span;
			_completionService = completionService;
			_startPos = span.Start;
			_popupText = code.Substring(span.Start, span.Length);
			_results = r;
			_items = new List<CiComplItem>(r.Items.Length);
			foreach(var v in r.Items) {
				bool noFilter = false;
				switch(v.Tags[0]) {
				case WellKnownTags.Keyword:
				case WellKnownTags.TypeParameter:
				case WellKnownTags.EnumMember:
				case WellKnownTags.Local:
				case WellKnownTags.Parameter:
				case WellKnownTags.RangeVariable:
				case WellKnownTags.Label:
				case WellKnownTags.Snippet:
					noFilter = true;
					break;
				}
				object sym = null; bool isInherited = false;
				if(!noFilter) {
					var name = v.DisplayText;
					if(d.TryGetValue(name, out sym)) {
						ISymbol s1 = (sym is List<ISymbol> list) ? list[0] : (sym as ISymbol);
						if(typeSymbol != null) {
							var ct = s1.ContainingType;
							if(ct != null && ct != typeSymbol) isInherited = true;
							//Print(s1.Name, s1.ContainingType, s1.ContainingType == typeSymbol);
						}

						if(s1.Kind!= SymbolKind.NamedType && s1.Kind != SymbolKind.Namespace) {
							if(sym is List<ISymbol> li) Print($"<><c blue>{name}, {string.Join("|", li)}<>");
							else Print($"<><c green>{name}, {s1}<>");
						}
					} else {
						Print($"<><c red>{name}, {v.Tags[0]}<>");
					}
				}

				_items.Add(new CiComplItem(v, sym, isInherited));
			}
			if(!span.IsEmpty) _FilterItems(_popupText, false);
			APerf.Next('F');

			if(_popupList == null) {
				_popupList = new CiPopupList(this);
				_popupList.PopupWindow.VisibleChanged += _popupList_VisibleChanged;
			}
			_popupList.SetListItems(_items, o => _GetDescription(completionService, document, o.ci));
			_SelectBestMatch(document);
			_popupList.Show(doc, span.Start);
			//APerf.Next();
		}
		catch(OperationCanceledException) { /*ADebug.Print("canceled");*/ _CancelWork(); return; }
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
		APerf.NW();
	}

	private void _popupList_VisibleChanged(object sender, EventArgs e)
	{
		if((sender as Form).Visible) return;
		_completionService = null;
		_popupText = null;
		_results = null;
		_items = null;
	}

	void _FilterItems(string text, bool updatePopupList)
	{
		if(Empty(text)) {
			foreach(var v in _items) {
				v.hidden &= ~CiItemHiddenBy.Text;
				v.hilite = null;
			}
		} else {
			var textUpper = text.Upper();
			var ah = new List<int>();
			foreach(var v in _items) {
				ADebug.PrintIf(v.ci.FilterText != v.ci.DisplayText, $"{v.ci.FilterText}, {v.ci.DisplayText}");
				//Print(v.DisplayText, v.FilterText, v.SortText, v.ToString());
				int[] hilite = null;
				var s = v.DisplayText;
				int iSub = s.Find(text, true);
				if(iSub >= 0) {
					hilite = new int[] { iSub, iSub + text.Length };
					//TODO: VS adds only if the substring starts with uppercase
				} else if(text.Length > 1) { //has all uppercase chars? Eg add OneTwoThree if text is "ott" or "ot" or "tt".
					ah.Clear();
					bool no = false;
					for(int i = 0, j = 0; i < textUpper.Length; i++, j++) {
						j = s.IndexOf(textUpper[i], j);
						if(j < 0) { no = true; break; }
						ah.Add(j); ah.Add(j + 1);
					}
					if(!no) hilite = ah.ToArray();
				}
				//TODO: support _ and all ucase, like WM_PAINT

				v.hilite = hilite;
				if(hilite == null) v.hidden |= CiItemHiddenBy.Text; else v.hidden &= ~CiItemHiddenBy.Text;
			}
		}
		if(updatePopupList) {
			_popupList.UpdateVisibleItems();
			_SelectBestMatch();
			//TODO: Invalidate?
		}
	}

	void _SelectBestMatch(Document document = null)
	{
		document ??= CodeInfo.GetDocument();
		var fi = _completionService.FilterItems(document, _results.Items, _popupText);
		//Print(fi);
		if(!fi.IsEmpty) _popupList.ListControl.SelectedIndex = _items.FindIndex(o => o.ci == fi[0]);
	}

	static async Task<CompletionDescription> _GetDescription(CompletionService completionService, Document document, CompletionItem v)
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
		return await Task.Run(() => completionService.GetDescriptionAsync(document, v));
	}

	public void OnCompletionListItemDClicked(CiComplItem item)
	{
		var ci = item.ci;
		Print(item.DisplayText, ci.Span);
		var doc = Panels.Editor.ActiveDoc;
		//var s = item.DisplayText;
		var change = _completionService.GetChangeAsync(CodeInfo.GetDocument(), ci).Result; //TODO: commitCharacter
		var s = change.TextChange.NewText;
		//Print(change.TextChange.Span, change.NewPosition);
		var span = change.TextChange.Span;
		if(change.NewPosition.HasValue) {
			doc.ST.ReplaceRange(span.Start, span.Length, s, SciFromTo.BothChars | SciFromTo.ToIsLength);
			doc.ST.GoToPos(change.NewPosition.GetValueOrDefault(), true);
		} else {
			doc.ST.SetAndReplaceSel(span.Start, span.Length, s, SciFromTo.BothChars | SciFromTo.ToIsLength);
		}
		//good: when changed text/pos with the above code, we don't receive notifications, because text/pos are modified by code.
	}

	public void SciTextChangedInMeta(in Sci.SCNotification n)
	{

	}
}

class CiComplItem
{
	public readonly CompletionItem ci;
	public readonly object symbol; //null or ISymbol or List<ISymbol>
	public readonly CiItemKind kind;
	public readonly CiItemAccess access;
	public CiItemHiddenBy hidden;
	public readonly bool isInherited;
	public int[] hilite;

	public string DisplayText => _text ??= ci.DisplayText + ci.DisplayTextSuffix;
	string _text;

	//string _GetDisplayText()
	//{
	//	string prefix = null;
	//	ISymbol sym = null;
	//	switch(symbol) {
	//	case ISymbol k: sym = k;break;
	//	case List<ISymbol> k: sym = k[0];break;
	//	}
	//	if(sym!=null && sym.)
	//}

	public Bitmap Image => CodeInfo.GetImage(kind, access);

	public CiComplItem(CompletionItem ci, object symbol, bool isInherited)
	{
		this.ci = ci;
		this.symbol = symbol;
		this.isInherited = isInherited;
		kind = ci.Tags[0] switch
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
			WellKnownTags.Local => CiItemKind.Local,
			WellKnownTags.Parameter => CiItemKind.Local,
			WellKnownTags.RangeVariable => CiItemKind.Local,
			WellKnownTags.Constant => CiItemKind.Constant,
			WellKnownTags.EnumMember => CiItemKind.EnumMember,
			WellKnownTags.Keyword => CiItemKind.Keyword,
			WellKnownTags.Namespace => CiItemKind.Namespace,
			WellKnownTags.Label => CiItemKind.Label,
			WellKnownTags.Snippet => CiItemKind.Snippet,
			WellKnownTags.TypeParameter => CiItemKind.TypeParameter,
			_ => CiItemKind.None
		};
		_PrintCI(kind == CiItemKind.None, "blue");
		if(ci.Tags.Length > 1) {
			access = ci.Tags[1] switch { WellKnownTags.Private => CiItemAccess.Private, WellKnownTags.Protected => CiItemAccess.Protected, WellKnownTags.Internal => CiItemAccess.Internal, _ => default };
			_PrintCI(ci.Tags.Length > 2 || (access == default && ci.Tags[1] != WellKnownTags.Public), "green");
		}
	}

	[Conditional("DEBUG")]
	void _PrintCI(bool condition, string color)
	{
		if(!condition) return;
		Print($"<><c {color}>{ci.DisplayText}<>");
		foreach(var v in ci.Tags) Print(v);
		//foreach(var v in ci.Properties) Print(v);
	}
}
