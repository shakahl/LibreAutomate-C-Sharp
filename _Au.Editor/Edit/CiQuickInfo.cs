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
//using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Compiler;

using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.CSharp.QuickInfo;
using Au.Controls;

class CiQuickInfo
{
	public async void SciMouseDwellStarted(SciCode doc, int pos8, bool isDiag)
	{
		var pi = Panels.Info;
		if(!pi.Visible) pi = null;
		if(isDiag && pi == null) return;

		if(pos8 <= 0) { pi?.ZSetAboutInfo(); return; }

		//APerf.First();
		int pos16 = doc.Pos16(pos8);
		if(!CodeInfo.GetContextAndDocument(out var cd, pos16)) { pi?.ZSetAboutInfo(cd.metaEnd > 0); return; }

		//APerf.Next();
		var context = new QuickInfoContext(cd.document, pos16, default);

		//APerf.Next();
		var provider = new CSharpSemanticQuickInfoProvider();
		//var r = await provider.GetQuickInfoAsync(context); //not async
		var r = await Task.Run(async () => await provider.GetQuickInfoAsync(context));
		//APerf.Next();
		if(r == null) { pi?.ZSetAboutInfo(); return; }

		//Print(r.Span, r.RelatedSpans);
		//Print(r.Tags);

		var b = new StringBuilder("<body><div>");

		//image
		CiUtil.TagsToKindAndAccess(r.Tags, out var kind, out var access);
		if(kind != CiItemKind.None) {
			if(access != default) b.AppendFormat("<img src='@a{0}' style='padding-top: 6px' />", (int)access);
			b.AppendFormat("<img src='@k{0}' style='padding-top: 2px' />", (int)kind);
		}

		//bool hasDocComm = false;
		//QuickInfoSection descr = null;
		var a = r.Sections;
		for(int i = 0; i < a.Length; i++) {
			var se = a[i];
			//Print(se.Kind, se.Text);
			int excFrom = 0;
			switch(se.Kind) {
			//case QuickInfoSectionKinds.Description:
			//	descr = se;
			//	break;
			//case QuickInfoSectionKinds.DocumentationComments:
			//	hasDocComm = true;
			//	break;
			case QuickInfoSectionKinds.Exception:
				excFrom = b.Length + 12;
				break;
			}
			if(i > 0) b.Append("<p>");
			CiHtml.TaggedPartsToHtml(b, se.TaggedParts);
			b.Append(i > 0 ? "</p>" : "</div>");

			if(excFrom > 0) b.Replace(":<br>", ": ", excFrom, b.Length - excFrom).Replace("><br>", ">, ", excFrom, b.Length - excFrom); //exceptions make single line
		}

		b.Append("</body>");
		var html = b.ToString();
		//Print(html);
		html = html.Replace("<p><br>", "<p>");
		html = html.Replace("><br>", ">&nbsp;<br>"); //workaround for HtmlRenderer bug: adds 2 lines.
													 //APerf.Next();
		if(pi != null) {
			pi.ZSetText(html);
		} else {
			CodeInfo.ShowHtmlPopup(doc, pos16, html);
		}
		//APerf.NW();
	}
}
