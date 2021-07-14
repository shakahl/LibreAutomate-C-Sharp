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
using System.Windows.Documents;
using Microsoft.CodeAnalysis;

//SHOULDDO: displayed parameter types are with undeclared namespace. Remove namespace if it is in favorites. Maybe always.

class CiQuickInfo
{
	public async Task<Section> GetTextAt(int pos16) {
		//perf.first();
		if (!CodeInfo.GetContextAndDocument(out var cd, pos16)) return null;

		//perf.next();
		var context = new QuickInfoContext(cd.document, pos16, default);

		//perf.next();
		//var provider = new CSharpSemanticQuickInfoProvider(); //error in new roslyn: obsolete ctor, use MEF
		var provider = typeof(CSharpSemanticQuickInfoProvider).GetConstructor(Type.EmptyTypes).Invoke(null) as CSharpSemanticQuickInfoProvider;
		//var r = await provider.GetQuickInfoAsync(context); //not async
		var r = await Task.Run(async () => await provider.GetQuickInfoAsync(context));
		//perf.next();
		if (r == null) return null;

		//print.it(r.Span, r.RelatedSpans);
		//print.it(r.Tags);

		var x = new CiText();

		//bool hasDocComm = false;
		//QuickInfoSection descr = null;
		var a = r.Sections;
		for (int i = 0; i < a.Length; i++) {
			var se = a[i];
			//print.it(se.Kind, se.Text);
			x.StartParagraph();

			if (i == 0) { //image
				CiUtil.TagsToKindAndAccess(r.Tags, out var kind, out var access);
				if (kind != CiItemKind.None) {
					if (access != default) x.Image(access);
					x.Image(kind);
					x.Append(" ");
				}
			}

			var tp = se.TaggedParts;
			if (tp[0].Tag == TextTags.LineBreak) { //remove/replace some line breaks in returns and exceptions
				int lessNewlines = se.Kind switch { QuickInfoSectionKinds.ReturnsDocumentationComments => 1, QuickInfoSectionKinds.Exception => 2, _ => 0 };
				var k = new List<TaggedText>(tp.Length - 1);
				for (int j = 1; j < tp.Length; j++) {
					var v = tp[j];
					if (lessNewlines > 0 && j > 1) {
						if (v.Tag == TextTags.LineBreak) {
							if (j == 2) continue; //remove line break after "Returns:" etc
							if (lessNewlines == 2) { //in list of exceptions replace "\n  " with ", "
								if (++j == tp.Length || tp[j].Tag != TextTags.Space) { j--; continue; }
								v = new(TextTags.Text, ", ");
							}
						}
					}
					k.Add(v);
				}
				x.AppendTaggedParts(k);
			} else {
				x.AppendTaggedParts(tp);
			}

			x.EndParagraph();
		}

		return x.Result;
	}
}
