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
using System.Xml.Linq;
//using System.Xml.XPath;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Compiler
{
	//[DebuggerStepThrough]
	public class Scripting
	{
		public static Task<object> EvaluateAsync(string code, Assembly[] references, string[] namespaces)
		{

			try {
				var so = ScriptOptions.Default.WithReferences(references).WithImports(namespaces);
				return CSharpScript.EvaluateAsync<object>(code, so);
			}
			catch(CompilationErrorException e) {
				throw new CompilationException(e);
			}
		}
	}

	public class CompilationException :Exception
	{
		public CompilationException(CompilationErrorException e) : base(String.Join("\r\n", e.Diagnostics)) { }
	}
}
