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
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.CodeAnalysis.CSharp.SignatureHelp;

//FUTURE: show for lambda parameters. Currently VS does not show too.

class CiSignature
{
	CiPopupHtml _popupHtml;
	_Data _data; //not null while the popup window is visible

	class _Data
	{
		//public Compilation compilation;
		//public ISignatureHelpProvider provider;
		//public string code;
		public SignatureHelpItems r;
		public _Span span;
		public int iSelected, iUserSelected;
		public SciCode sciDoc;

		public bool IsSameSpan(_Span span2)
		{
			return span2.start == span.start && span2.fromEnd == span.fromEnd;
			//never mind: we don't check whether text before and after is still the same. Not that important.
		}

		public int GetUserSelectedItemIfSameSpan(_Span span2, SignatureHelpItems r2)
		{
			if(iUserSelected < 0 || !IsSameSpan(span2) || r2.Items.Count != r.Items.Count) return -1;
			for(int i = 0; i < r.Items.Count; i++) {
				var hi1 = r.Items[i] as AbstractSignatureHelpProvider.SymbolKeySignatureHelpItem;
				var hi2 = r2.Items[i] as AbstractSignatureHelpProvider.SymbolKeySignatureHelpItem;
				Debug.Assert(!(hi1 == null || hi2 == null));
				if(hi1 == null || hi2 == null || hi2.Symbol != hi1.Symbol) return -1;
			}
			return iUserSelected;
		}
	}

	struct _Span
	{
		public int start, fromEnd;
		public _Span(int start, int fromEnd) { this.start = start; this.fromEnd = fromEnd; }
		public _Span(TextSpan span, string code) { this.start = span.Start; this.fromEnd = code.Length - span.End; }
	}

	public bool IsVisibleUI => _data != null;

	public void Cancel()
	{
		if(_data == null) return;
		foreach(var r in _data.sciDoc.ZTempRanges_Enum(this)) r.Remove();
		_data = null;
		_popupHtml.Hide();
	}

	public void SciPositionChanged(SciCode doc)
	{
		if(_afterCharAdded) { _afterCharAdded = false; return; }
		if(_data == null) return;
		_ShowSignature(doc, default);
	}
	bool _afterCharAdded;

	public void SciCharAdded(SciCode doc, char ch)
	{
		switch(ch) { case '(': case '[': case '<': case ')': case ']': case '>': case ',': break; default: return; }
		_ShowSignature(doc, ch);
		_afterCharAdded = true;
	}

	public void ShowSignature(SciCode doc)
	{
		_ShowSignature(doc, default);
	}

	void _ShowSignature(SciCode doc, char ch)
	{
		//APerf.First();
		if(!CodeInfo.GetContextAndDocument(out var cd, -2)) return; //returns false if position is in meta comments

		//APerf.Next();
		var trigger = new SignatureHelpTriggerInfo(ch == default ? SignatureHelpTriggerReason.InvokeSignatureHelpCommand : SignatureHelpTriggerReason.TypeCharCommand, ch);
		var providers = SignatureHelpProviders;
		//Print(providers);
		//APerf.Next();
		SignatureHelpItems r = null;
		foreach(var p in providers) {
			//APerf.First();
			var r2 = p.GetItemsAsync(cd.document, cd.pos16, trigger, default).Result;
			//APerf.NW(); //quite fast, don't need async. But in the future can try to wrap this foreach+SignatureHelpProviders in async Task. Need to test with large files.
			if(r2 == null) continue;
			if(r == null || r2.ApplicableSpan.Start > r.ApplicableSpan.Start) r = r2;
			//Example: 'Print(new Something())'.
			//	The first provider probably is for Print (invocation).
			//	Then the second is for Something (object creation).
			//	We need the innermost, in this case Something.
		}

		if(r == null) {
			Cancel();
			return;
		}
		//APerf.NW('s');
		//Print($"<><c orange>pos={de.position}, span={r.ApplicableSpan},    nItems={r.Items.Count},  argCount={r.ArgumentCount}, argIndex={r.ArgumentIndex}, argName={r.ArgumentName}, sel={r.SelectedItemIndex},    provider={provider}<>");

		//var node = document.GetSyntaxRootAsync().Result;
		var span = new _Span(r.ApplicableSpan, cd.code);
		int iSel = _data?.GetUserSelectedItemIfSameSpan(span, r) ?? -1; //preserve user selection in same session

		_data = new _Data {
			r = r,
			span = span,
			iUserSelected = iSel,
			sciDoc = doc,
		};

		if(iSel < 0) {
			iSel = r.SelectedItemIndex ?? (r.ArgumentCount == 0 ? 0 : -1);
			if(iSel < 0) {
				for(int i = 0; i < r.Items.Count; i++) if(r.Items[i].Parameters.Length >= r.ArgumentCount) { iSel = i; break; }
				if(iSel < 0) {
					for(int i = 0; i < r.Items.Count; i++) if(r.Items[i].IsVariadic) { iSel = i; break; }
					if(iSel < 0) iSel = 0;
				}
			}
		}

		string html = _FormatHtml(iSel, userSelected: false);

		doc.ZTempRanges_Add(this, r.ApplicableSpan.Start, r.ApplicableSpan.End, onLeave: () => {
			if(doc.ZTempRanges_Enum(doc.Z.CurrentPos8, this, utf8: true).Any()) return;
			Cancel();
		}, SciCode.ZTempRangeFlags.NoDuplicate);

		var rect1 = CiUtil.GetCaretRectFromPos(doc, r.ApplicableSpan.Start);
		var rect2 = CiUtil.GetCaretRectFromPos(doc, cd.pos16);
		var rect = doc.RectangleToScreen(Rectangle.Union(rect1, rect2));
		rect.Width += Au.Util.ADpi.ScaleInt(200);
		rect.X -= 6;

		_popupHtml ??= new CiPopupHtml(CiPopupHtml.UsedBy.Signature, onHiddenOrDestroyed: _ => _data = null) {
			OnLinkClick = (ph, e) => ph.Html = _FormatHtml(e.Link.ToInt(1), userSelected: true)
		};
		_popupHtml.Html = html;
		_popupHtml.Show(Panels.Editor.ZActiveDoc, rect, PopupAlignment.TPM_VERTICAL);
		//APerf.NW();
	}

	string _FormatHtml(int iSel, bool userSelected)
	{
		_data.iSelected = iSel;
		if(userSelected) _data.iUserSelected = iSel;

		var r = _data.r;
		ISymbol currentItem = null;
		SignatureHelpParameter currentParameter = null;
		var b = new StringBuilder("<body>");

		for(int i = 0; i < r.Items.Count; i++) {
			var k = r.Items[i];
			if(k is AbstractSignatureHelpProvider.SymbolKeySignatureHelpItem kk) {
				var sym = kk.Symbol;
				using var li = new CiHtml.HtmlListItem(b, i == iSel);
				if(i != iSel) b.AppendFormat("<a href='^{0}'>", i); else currentItem = sym;
				if(!Empty(sym.Name)) CiHtml.SymbolWithoutParametersToHtml(b, sym); //empty name if tuple
				string b1 = "(", b2 = ")";
				switch(sym) {
				case IPropertySymbol _: b1 = "["; b2 = "]"; break;
				case INamedTypeSymbol ints when ints.IsGenericType: b1 = "&lt;"; b2 = "&gt;"; break;
				}
				b.Append(b1);
				int iArg = r.ArgumentIndex, lastParam = k.Parameters.Length - 1;
				int selParam = iArg <= lastParam ? iArg : (k.IsVariadic ? lastParam : -1);
				if(!Empty(r.ArgumentName) && sym is IMethodSymbol ims) {
					var pa = ims.Parameters; for(int pi = 0; pi < pa.Length; pi++) if(pa[pi].Name == r.ArgumentName) { selParam = pi; break; }
					//selParam = ims.Parameters.FirstOrDefault(o => o.Name == r.ArgumentName)?.Ordinal ?? selParam; //the same
				}
				CiHtml.ParametersToHtml(b, sym, selParam);
				b.Append(b2);
				if(i != iSel) b.Append("</a>"); else if(selParam >= 0) currentParameter = k.Parameters[selParam];
			} else {
				ADebug.Print(k);
			}
		}

		if(currentItem != null) {
			var tt = r.Items[iSel].DocumentationFactory?.Invoke(default);
			bool haveDoc = tt?.Any() ?? false;
			string helpUrl = CiUtil.GetSymbolHelpUrl(currentItem);
			string sourceUrl = CiGoTo.GetLinkData(currentItem);
			bool haveLinks = helpUrl != null || sourceUrl != null;
			if(haveDoc || haveLinks) {
				b.Append("<p>");
				if(haveDoc) CiHtml.TaggedPartsToHtml(b, tt);
				if(haveLinks) CiHtml.SymbolLinksToHtml(b, helpUrl, sourceUrl, haveDoc ? " " : "", ".");
				b.Append("</p>");
			}
		}

		if(currentParameter != null && !Empty(currentParameter.Name)) { //if tuple, Name is "" and then would be exception
			b.Append("<p class='parameter'><b>").Append(currentParameter.Name).Append(":</b> &nbsp;");
			CiHtml.TaggedPartsToHtml(b, currentParameter.DocumentationFactory?.Invoke(default));
			b.Append("</p>");
		}

		b.Append("</body>");
		return b.ToString();
	}

	static List<ISignatureHelpProvider> _GetSignatureHelpProviders()
	{
		var a = new List<ISignatureHelpProvider>();
		foreach(var t in Assembly.GetAssembly(typeof(InvocationExpressionSignatureHelpProvider)).DefinedTypes.Where(t => t.ImplementedInterfaces.Contains(typeof(ISignatureHelpProvider)) && !t.IsAbstract)) {
			//Print(t);
			var c = t.GetConstructor(Type.EmptyTypes); Debug.Assert(c != null); if(c == null) continue;
			var o = c.Invoke(null) as ISignatureHelpProvider; Debug.Assert(o != null); if(o == null) continue;
			a.Add(o);
		}
		return a;
	}

	List<ISignatureHelpProvider> SignatureHelpProviders => _shp ??= _GetSignatureHelpProviders();
	List<ISignatureHelpProvider> _shp;

	public bool OnCmdKey(Keys keyData)
	{
		if(_data != null) {
			switch(keyData) {
			case Keys.Escape:
				Cancel();
				return true;
			case Keys.Down:
			case Keys.Up:
				int i = _data.iSelected, n = _data.r.Items.Count;
				if(keyData == Keys.Down) {
					if(++i >= n) i = 0;
				} else {
					if(--i < 0) i = n - 1;
				}
				if(i != _data.iSelected) _popupHtml.Html = _FormatHtml(i, userSelected: true);
				return true;
			}
		}
		return false;
	}
}
