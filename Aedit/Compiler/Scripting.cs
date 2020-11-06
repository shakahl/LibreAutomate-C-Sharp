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

using Au.Types;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Au.Compiler
{
	public static class Scripting
	{
		/// <summary>
		/// Compiles C# code. Optionally loads in-memory assembly and gets MethodInfo of the first static method for executing.
		/// Returns false if there are errors in code.
		/// </summary>
		/// <param name="code">C# code. If wrap is false - full code with class and usings; if true - one or more functions and other class-level declarations.</param>
		/// <param name="r">Receives results when compiled successfully.</param>
		/// <param name="wrap">Add default usings and wrap code into "[module: DefaultCharSet(CharSet.Unicode)]\r\npublic class __script__ {\r\n#line 1" ... "}".</param>
		/// <param name="load">Load in-memory assembly and get MethodInfo of the first static method for executing.</param>
		/// <remarks>
		/// Adds <see cref="MetaReferences.DefaultReferences"/>. If wrap is true, adds <see cref="c_defaultUsings"/>.
		/// 
		/// Function's code does not throw exceptions, but the CodeAnalysis API may throw, although undocumented and never noticed.
		/// 
		/// Thread-safe.
		/// </remarks>
		public static bool Compile(string code, out Result r, bool wrap, bool load)
		{
			if(wrap) {
				var b = new StringBuilder();
				b.AppendLine(c_defaultUsings);
				b.AppendLine("[module: DefaultCharSet(CharSet.Unicode)]\r\npublic class __script__ {\r\n#line 1");
				b.AppendLine(code).Append("}");
				code = b.ToString();
			}

			var tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest), "", Encoding.UTF8);
			var refs = new MetaReferences().Refs;
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);
			var compilation = CSharpCompilation.Create("test", new SyntaxTree[] { tree }, refs, options);
			var memStream = new MemoryStream(4096);
			var emitResult = compilation.Emit(memStream);

			r = new Result();
			if(!emitResult.Success) {
				var sb = new StringBuilder();
				foreach(var d in emitResult.Diagnostics) if(d.Severity == DiagnosticSeverity.Error) sb.AppendLine(d.ToString());
				r.errors = sb.ToString();
				return false;
			}

			memStream.Position = 0;
			if(load) {
				r.assembly = Assembly.Load(memStream.ToArray());
				r.method = r.assembly.GetTypes()[0].GetMethods(BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)[0];
			} else {
				r.stream = memStream;
			}
			return true;
		}

		const string c_defaultUsings = @"using Au; using Au.Types; using System; using System.Collections.Generic; using System.Text; using System.Text.RegularExpressions; using System.Diagnostics; using System.Runtime.InteropServices; using System.IO; using System.Threading; using System.Threading.Tasks; using System.Windows.Forms; using System.Drawing; using System.Linq;";

		public class Result
		{
			/// <summary>
			/// Receives errors when fails to compile.
			/// </summary>
			public string errors;

			/// <summary>
			/// When load is false, receives assembly bytes in stream (position=0).
			/// </summary>
			public MemoryStream stream;

			/// <summary>
			/// When load is true, receives loaded assembly.
			/// </summary>
			public Assembly assembly;

			/// <summary>
			/// When load is true, receives MethodInfo of the first static method for executing.
			/// </summary>
			public MethodInfo method;
		}
	}
}
