using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Configuration;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;

namespace XmlDocPreprocessor
{
	class Program
	{
		static void Main(string[] args)
		{
			bool debug = args.Length == 0;
			var xmlFile = debug ? Folders.ThisApp + @"..\..\Catkeys\Catkeys.xml" : args[0];
			if(debug) Output.Clear();
			Print(xmlFile);

			var doc = new XmlDocument();
			doc.Load(xmlFile);
			foreach(XmlNode n in doc.SelectNodes("doc/members/member/*/text()")) {
				var p = n.ParentNode;
				if(p.NodeType != XmlNodeType.Element) continue; //none
				var tag = p.Name;
				//don't process text in these tags
				switch(tag) { case "code": case "c": case "see": case "para": continue; }
				if(debug) {
					//show new tags, maybe need to exclude too (add to the above switch)
					switch(tag) { case "summary": case "remarks": case "param": case "exception": case "example": break; default: Print(tag); break; }
				}
				var s = n.Value;
				if(s.StartsWith_("\r")) continue;
					//s=s.Trim();
				PrintList(tag, s);
				Print("----");
			}
		}
	}
}
