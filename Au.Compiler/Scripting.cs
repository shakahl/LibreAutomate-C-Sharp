//C# compiler setup:
//Install nuget package Microsoft.CodeAnalysis.CSharp or Microsoft.CodeAnalysis.CSharp.Scripting.
//Problem - cluttering: it installs about 50 packages and 150 files/folders, adds them to References, to the main output folder, etc.
//Workaround:
//	Install it in another solution (Compiler) that contains single project (Compiler).
//		In that project set output = subfolder "Compiler" of the main output folder "_".
//		Compile that project. It adds all the dlls to the "Compiler" subfolder.
//In this project (Au.Compiler.dll) add references from the "Compiler" subfolder:
//	Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, System.Collections.Immutable. Make "copy local" = false.
//Let this project's output be the "Compiler" subfolder. Also let its Au reference be "copy local" = false.
//	It is workaround for: msbuild copies most CodeAnalysis dependencies (~50) to the main output folder.
//		That is why all code that uses CodeAnalysis was moved to this new project. Would be less work to use C# compiler directly in projects that need it.
//In projects that use this project as a reference:
//	Make its "copy local" = false.
//	In app.config add: configuration/runtime/assemblyBinding: <probing privatePath = "Compiler" />
//	Don't need "Auto-generate binding redirects" in project properties. With framework 4.7.2, no problems with compiler assembly binding. Previously, with 4.6.2, 'auto generate...' did not work well; would need to add several binding redirections manually to app.config.

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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

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
		/// Adds <see cref="Compiler.DefaultReferences"/>. If wrap is true, adds <see cref="Compiler.DefaultUsings"/>.
		/// 
		/// Function's code does not throw exceptions, but the CodeAnalysis API may throw, although undocumented and never noticed.
		/// 
		/// Thread-safe.
		/// </remarks>
		public static bool Compile(string code, out Result r, bool wrap, bool load)
		{
			if(wrap) {
				var b = new StringBuilder();
				b.AppendLine(Compiler.DefaultUsings);
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

			Compiler.LibSetTimerGC(); //after some time free many MB of unmanaged memory of metadata references

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
