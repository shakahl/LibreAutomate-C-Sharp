using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
//using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using Au;
using Au.Types;
using static Au.NoClass;

using System.Linq;
using System.Xml.XPath;

using SandcastleBuilder.Utils;
using SandcastleBuilder.Utils.BuildComponent;
using SandcastleBuilder.Utils.BuildEngine;

// Search for "TODO" to find changes that you need to make to this plug-in template.

namespace LineBreaks
{
	/// <summary>
	/// TODO: Set your plug-in's unique ID and description in the export attribute below.
	/// </summary>
	/// <remarks>The <c>HelpFileBuilderPlugInExportAttribute</c> is used to export your plug-in so that the help
	/// file builder finds it and can make use of it.  The example below shows the basic usage for a common
	/// plug-in.  Set the additional attribute values as needed:
	///
	/// <list type="bullet">
	///     <item>
	///         <term>IsConfigurable</term>
	///         <description>Set this to true if your plug-in contains configurable settings.  The
	/// <c>ConfigurePlugIn</c> method will be called to let the user change the settings.</description>
	///     </item>
	///     <item>
	///         <term>RunsInPartialBuild</term>
	///         <description>Set this to true if your plug-in should run in partial builds used to generate
	/// reflection data for the API Filter editor dialog or namespace comments used for the Namespace Comments
	/// editor dialog.  Typically, this is left set to false.</description>
	///     </item>
	/// </list>
	/// 
	/// Plug-ins are singletons in nature.  The composition container will create instances as needed and will
	/// dispose of them when the container is disposed of.</remarks>
	[HelpFileBuilderPlugInExport("LineBreaks", Version = AssemblyInfo.ProductVersion,
	  Copyright = AssemblyInfo.Copyright, Description = "LineBreaks plug-in")]
	public sealed class LineBreaksPlugIn :IPlugIn
	{
		#region Private data members
		//=====================================================================

		private List<ExecutionPoint> executionPoints;

		private BuildProcess builder;
		#endregion

		#region IPlugIn implementation
		//=====================================================================

		/// <summary>
		/// This read-only property returns a collection of execution points that define when the plug-in should
		/// be invoked during the build process.
		/// </summary>
		public IEnumerable<ExecutionPoint> ExecutionPoints
		{
			get
			{
				if(executionPoints == null)
					//PrintFunc();
					executionPoints = new List<ExecutionPoint>
					{
                        // TODO: Modify this to set your execution points
						new ExecutionPoint(BuildStep.GenerateReflectionInfo, ExecutionBehaviors.Before)
					};

				return executionPoints;
			}
		}

		/// <summary>
		/// This method is used by the Sandcastle Help File Builder to let the plug-in perform its own
		/// configuration.
		/// </summary>
		/// <param name="project">A reference to the active project</param>
		/// <param name="currentConfig">The current configuration XML fragment</param>
		/// <returns>A string containing the new configuration XML fragment</returns>
		/// <remarks>The configuration data will be stored in the help file builder project</remarks>
		public string ConfigurePlugIn(SandcastleProject project, string currentConfig)
		{
			//PrintFunc();
			// TODO: Add and invoke a configuration dialog if you need one.  You will also need to set the
			// IsConfigurable property to true on the class's export attribute.
			MessageBox.Show("This plug-in has no configurable settings", "Build Process Plug-In",
				MessageBoxButtons.OK, MessageBoxIcon.Information);

			return currentConfig;
		}

		/// <summary>
		/// This method is used to initialize the plug-in at the start of the build process
		/// </summary>
		/// <param name="buildProcess">A reference to the current build process</param>
		/// <param name="configuration">The configuration data that the plug-in should use to initialize itself</param>
		public void Initialize(BuildProcess buildProcess, XPathNavigator configuration)
		{
			Output.LibUseQM2 = true;
			Output.Clear();

			builder = buildProcess;

			var metadata = (HelpFileBuilderPlugInExportAttribute)this.GetType().GetCustomAttributes(
				typeof(HelpFileBuilderPlugInExportAttribute), false).First();

			builder.ReportProgress("{0} Version {1}\r\n{2}", metadata.Id, metadata.Version, metadata.Copyright);

			// TODO: Add your initialization code here such as reading the configuration data
		}

		/// <summary>
		/// This method is used to execute the plug-in during the build process
		/// </summary>
		/// <param name="context">The current execution context</param>
		public void Execute(ExecutionContext context)
		{
			// TODO: Add your execution code here
			builder.ReportProgress("In LineBreaksPlugIn Execute() method");

			switch(context.BuildStep) {
			case BuildStep.GenerateReflectionInfo:
				var f = builder.CommentsFiles[0];
				_Process(f.Members);
				break;
			}
		}
		#endregion

		#region IDisposable implementation
		//=====================================================================

		// TODO: If the plug-in hasn't got any disposable resources, this finalizer can be removed
		/// <summary>
		/// This handles garbage collection to ensure proper disposal of the plug-in if not done explicitly
		/// with <see cref="Dispose()"/>.
		/// </summary>
		~LineBreaksPlugIn()
		{
			this.Dispose();
		}

		/// <summary>
		/// This implements the Dispose() interface to properly dispose of the plug-in object
		/// </summary>
		public void Dispose()
		{
			// TODO: Dispose of any resources here if necessary
			GC.SuppressFinalize(this);
		}
		#endregion

		void _Process(XmlNode members)
		{
			//Debugger.Launch(); if(Debugger.IsAttached) Debugger.Break();

			ProcessLineBreaks(members);
			ProcessSee(members);
			ProcessMsdn(members);

			//doc.Save(@"q:\test\xmldoc.xml");
		}

		void ProcessLineBreaks(XmlNode members)
		{
			var doc = members.OwnerDocument;

			//line breaks
			foreach(XmlNode n in members.SelectNodes("member//text()")) { //info: could be "member/*//text()", but this allows to catch text outside <summary> etc, which is incorrect.
				var p = n.ParentNode;
				if(p.NodeType != XmlNodeType.Element) continue; //none

				var tag = p.Name;
				//don't process text in these tags
				switch(tag) { case "code": case "c": case "see": case "seealso": case "a": case "conceptualLink": case "para": case "b": case "i": case "term": case "msdn": continue; }
				//show new tags, maybe need to exclude too (add to the above switch)
				switch(tag) {
				case "summary": case "remarks": case "param": case "typeparam": case "value": case "exception": case "example": case "note": case "item": case "description": break;
				//default: Print(tag); break;
				default: Print(p.OuterXml); break;
				}

				var s = n.Value;
				//if(s.IndexOf('\n') < 0 && p.OuterXml.IndexOf('\n')>=0) PrintNode(p, s);
				if(s.IndexOf('\n') < 0) continue;

				int iStart = 0, iEnd = s.Length, i;
				if(iEnd < 3) continue;
				//keep first newline (don't add <br/>)?
				if(s[0] == '\r' && s[1] == '\n') {
					var xprev = n.PreviousSibling;
					if(xprev == null || _IsBlockElement(xprev, s, p)) iStart = 2;
				}
				//keep last newline (don't add <br/>)?
				for(i = iEnd - 1; i >= iStart; i--) if(s[i] != ' ') break;
				if(i > iStart && s[i] == '\n') {
					var xnext = n.NextSibling;
					if(xnext == null || _IsBlockElement(xnext, s, p)) {
						if(s[i - 1] == '\r') i--;
						iEnd = i;
					}
				}

				i = s.IndexOf('\n', iStart);
				if(i < 0 || i >= iEnd) continue;
				s = s.Substring(iStart, iEnd - iStart);
				//PrintNode(s, p);

				var a = s.SplitLines_();
				for(i = a.Length - 1; i >= 0; i--) {
					//p.InsertAfter(doc.CreateTextNode("\r\n" + a[i]), n); //no: if with \r\n, IE adds spaces where don't need
					p.InsertAfter(doc.CreateTextNode(a[i]), n);
					if(i > 0) p.InsertAfter(doc.CreateElement("br"), n);
				}
				p.RemoveChild(n);

				//TODO: preprocess tab-indented (after ///) lines.
			}
		}

		static bool _IsBlockElement(XmlNode n, string s, XmlNode parent)
		{
			var t = n.Name;
			switch(t) {
			case "code": case "list": case "note": case "para": case "h2": case "h3": case "h4": case "hr": case "ol": case "ul": case "table": case "p": return true;
			case "c": case "see": case "paramref": case "typeparamref": case "conceptualLink": case "a": case "b": case "i": case "div": case "span": case "img": break;
			default: PrintNode(s, parent); break; //detect misc small bugs in XML doc
			}
			return false;

			//info: I never use <returns> because: 1. Intellisense does not show it. 2. Often summary says "Gets ..." or "Returns ...".
			//info: not supported in HTML5: font.
		}

		static void PrintNode(string s, XmlNode parent)
		{
			Print($"<><Z 0xffffff>{s}</Z>\r\n<c 0x8000>{parent.OuterXml}</c>");
		}

		//<see cref="Class.Member"/> -> <see cref="Class.Member">{Class.Member}</see>, because SHFB removes "Class."
		void ProcessSee(XmlNode members)
		{
			//info: the original cref text is lost.
			//	C# compiler replaces it to fully-qualified, with type prefix, like "M:Namespace.Type.Method(String)".

			foreach(XmlElement n in members.SelectNodes("member/*//see|member/seealso")) {
				var an = n.GetAttributeNode("cref");
				if(an == null) continue; //href?

				if(!Empty(n.InnerText)) {
					//Print(n.InnerText);
					continue; //link text specified explicitly, like <see cref="X">Y</see>
				}

				if(n.GetAttributeNode("r") != null) continue;

#if false //bug in SHFB or IES: adds spaces in text of some links, like "Type. Member"
				n.SetAttribute("qualifyHint", "true");
#else //does not add some useful info for generic etc. Never mind.
				var s = an.Value;
				//Print($"<><c 0x8000>{s}</c>");

				//if(s.IndexOf('`') >= 0) continue; //generic
				//if(s.IndexOf('#') >= 0) continue; //eg M:Au.Type.#ctor
				//if(s.IndexOf_(".op_") >= 0) continue; //operator
				//if(s.IndexOf_(".Item(") >= 0) continue; //indexer

				//remove from s:
				//	the type prefix ("M:" etc)
				//	our namespaces that are used in every file
				//	method parameters
				//	generic suffix, like ``1
				int iStart = 2;
				if(s.EqualsAt_(2, "Au.Types.")) iStart += 9; else if(s.EqualsAt_(2, "Au.")) iStart += 3;
				int iEnd = s.IndexOf('`');
				if(iEnd < 0) iEnd = s.IndexOf('(');
				if(iEnd < 0) iEnd = s.Length;
				s = s.Substring(iStart, iEnd - iStart);
				//Print(s);

				n.InnerText = s;
#endif

				//Print(n.OuterXml);
			}
		}

		//<msdn>API</MSDN> -> <see href=''https://www.google.com/search?q=site:msdn.microsoft.com+{API}''>{API}</see>
		void ProcessMsdn(XmlNode members)
		{
			var doc = members.OwnerDocument;
			foreach(XmlElement n in members.SelectNodes("member/*//msdn")) {
				//Print(n.InnerText);
				var k = doc.CreateElement("see");
				k.InnerText = n.InnerText;
				k.SetAttribute("href", "https://www.google.com/search?q=site:msdn.microsoft.com+" + n.InnerText);
				var p = n.ParentNode;
				p.InsertAfter(k, n);
				p.RemoveChild(n);
			}
		}
	}
}
