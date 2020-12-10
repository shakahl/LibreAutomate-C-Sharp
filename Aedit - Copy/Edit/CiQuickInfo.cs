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

using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.CSharp.QuickInfo;

class CiQuickInfo
{
	public async void SciMouseDwellStarted(SciCode doc, int pos8, bool isDiag) {
		var pi = Panels.Info;
		if (!pi.IsVisible) pi = null;
		if (isDiag && pi == null) return;

		if (pos8 <= 0) { pi?.ZSetAboutInfo(); return; }

		//APerf.First();
		int pos16 = doc.Pos16(pos8);
		if (!CodeInfo.GetContextAndDocument(out var cd, pos16)) { pi?.ZSetAboutInfo(cd.metaEnd > 0); return; }

		//APerf.Next();
		var context = new QuickInfoContext(cd.document, pos16, default);

		//APerf.Next();
		//var provider = new CSharpSemanticQuickInfoProvider(); //error in new roslyn: obsolete ctor, use MEF
		var provider = typeof(CSharpSemanticQuickInfoProvider).GetConstructor(Type.EmptyTypes).Invoke(null) as CSharpSemanticQuickInfoProvider;
		//var r = await provider.GetQuickInfoAsync(context); //not async
		var r = await Task.Run(async () => await provider.GetQuickInfoAsync(context));
		//APerf.Next();
		if (r == null) { pi?.ZSetAboutInfo(); return; }

		//AOutput.Write(r.Span, r.RelatedSpans);
		//AOutput.Write(r.Tags);

		var x = new CiXaml();
		x.StartParagraph();

		//image
		CiUtil.TagsToKindAndAccess(r.Tags, out var kind, out var access);
		if (kind != CiItemKind.None) {
			//if(access != default) b.AppendFormat("<img src='@a{0}' style='padding-top: 6px' />", (int)access);
			//b.AppendFormat("<img src='@k{0}' style='padding-top: 2px' />", (int)kind);
			//TODO
			if (access != default) x.Image("@a" + (int)access);
			x.Image("@k" + (int)kind);
			x.Append(" ");
		}

		//bool hasDocComm = false;
		//QuickInfoSection descr = null;
		POINT excRange = default, retRange = default;
		var a = r.Sections;
		for (int i = 0; i < a.Length; i++) {
			var se = a[i];
			//AOutput.Write(se.Kind, se.Text);
			if (i > 0) x.StartParagraph();
			int from = x.SB.Length;
			x.AppendTaggedParts(se.TaggedParts);
			int to = x.SB.Length;
			switch (se.Kind) {
			case QuickInfoSectionKinds.ReturnsDocumentationComments: retRange = (from, to); break;
			case QuickInfoSectionKinds.Exception: excRange = (from, to); break;
			}
			x.EndParagraph();
		}

		x.End();
		var xaml = x.ToString();

		//join lines
		if (retRange.x > 0) xaml = xaml.RegexReplace(":<LineBreak/>", ": ", 1, 0, retRange.x..retRange.y);
		if (excRange.x > 0) xaml = xaml.RegexReplace(":<LineBreak/>", ": ", 1, 0, excRange.x..excRange.y).RegexReplace("><LineBreak/>", ">, ", range: excRange.x..excRange.y);

		xaml = xaml.Replace("<Paragraph><LineBreak/>", "<Paragraph>");
		//xaml = xaml.Replace("><LineBreak/>", ">&#160;<LineBreak/>"); //workaround for HtmlRenderer bug: adds 2 lines.
		//APerf.Next();
		if (pi != null) {
			pi.ZSetXaml(xaml);
		} else {
			CodeInfo.ShowXamlPopup(doc, pos16, xaml);
		}
		//APerf.NW();
	}
}
