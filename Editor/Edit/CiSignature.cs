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

class CiSignature
{
#if true
	public void ShowSignature(char ch = default)
	{
	}
#else

	public void ShowSignature(char ch = default)
	{
		APerf.First();
		var document = CodeInfo.CreateTestDocumentForEditorCode(out string code, out int position);
		APerf.Next();
		var providers = SignatureHelpProviders;
		APerf.Next();
		foreach(var p in providers) {
			var r = p.GetItemsAsync(document, position, new SignatureHelpTriggerInfo(), default).Result;
			if(r == null) continue;
			Print($"<><c orange>{p}    nItems={r.Items.Count}  argCount={r.ArgumentCount} argIndex={r.ArgumentIndex} argName={r.ArgumentName} sel={r.SelectedItemIndex}<>");
			//Print(r.Items);

			string s;
			if(r.Items.Count == 1) s = r.Items[0].ToString();
			else s = string.Join("\n", r.Items);

			var doc = Panels.Editor.ActiveDoc;
			doc.ST.SetString(Sci.SCI_CALLTIPSHOW, doc.ST.CountBytesFromChars(position), s);

			//foreach(var v in r.Items) {
			var v = r.Items[0];
			//Print(v.Parameters);
			foreach(var k in v.Parameters) {
				Print("--------");
				Print(k.Name);
				Print("DisplayParts:", string.Join("", k.DisplayParts));
				Print("Documentation:", string.Join("", k.DocumentationFactory(default)));
				//Print("PrefixDisplayParts:");
				//Print(k.PrefixDisplayParts);
				//Print("SelectedDisplayParts:");
				//Print(k.SelectedDisplayParts);
				//Print("SuffixDisplayParts:");
				//Print(k.SuffixDisplayParts);
			}
			//}

			break;
		}
		APerf.NW();
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
#endif

	public void Cancel(SciCode doc)
	{

	}
}
