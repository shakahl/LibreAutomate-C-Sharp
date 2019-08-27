using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Mono.Cecil;
using System.Runtime.CompilerServices;

//This small program copies Roslyn dlls to _\Compiler folder.
//Injects InternalsVisibleTo attribute to some. For it uses Mono.Cecil from NuGet.
//The dlls are from NuGet package Microsoft.CodeAnalysis.Features, which is installed only for this project.

//C# compiler setup:
//Install or update Microsoft.CodeAnalysis.Features from NuGet in this project.
//Run this project. It copies Roslyn dlls to _\Compiler folder and injects [InternalsVisibleTo] to some.
//In other projects (Au.Compiler, Au.Editor) add references from _\Compiler folder and make "copy local" = false.
//	For Au.Compiler need only Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, System.Collections.Immutable. For Au.Editor need more.
//Let Au.Compiler output be _\Compiler folder. Also let its Au reference be "copy local" = false.
//In projects that use Au.Compiler as a reference:
//	Make its "copy local" = false.
//	In app.config add: configuration/runtime/assemblyBinding: <probing privatePath = "Compiler" />
//	Don't need "Auto-generate binding redirects" in project properties. With framework 4.7.2, no problems with compiler assembly binding. Previously, with 4.6.2, 'auto generate...' did not work well; would need to add several binding redirections manually to app.config.


namespace CompilerDlls
{
	class Program
	{
		static void Main(string[] args)
		{
			var rx = new Regex(@"(?i)^(System|Microsoft)\..+\.(dll|pdb|xml)$", RegexOptions.CultureInvariant);
			string d1 = AppDomain.CurrentDomain.BaseDirectory, d2 = Path.GetFullPath(d1 + @"..\..\..\_\Compiler");

			if(Directory.Exists(d2)) {
				foreach(var f in new DirectoryInfo(d2).EnumerateFiles()) {
					if(rx.IsMatch(f.Name)) File.Delete(f.FullName);
				}
			} else Directory.CreateDirectory(d2);

			foreach(var f in new DirectoryInfo(d1).EnumerateFiles()) {
				var s = f.Name;
				if(!rx.IsMatch(s)) continue;
				var s2 = d2 + @"\" + s;
				switch(s) {
				//case "Microsoft.CodeAnalysis.dll": //then somehow SemanticModel.GetSymbolInfo (extension method) throws MethodAccessException
				case "Microsoft.CodeAnalysis.CSharp.dll":
				case "Microsoft.CodeAnalysis.Features.dll":
				case "Microsoft.CodeAnalysis.CSharp.Features.dll":
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(s);
					_InjectAttribute(f.FullName, s2);
					break;
				default:
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine(s);
					File.Copy(f.FullName, s2, true);
					break;
				}
			}
		}

		/// <summary>
		/// Adds [assembly: InternalsVisibleTo("Au.Editor...")]
		/// </summary>
		/// <param name="path"></param>
		/// <param name="savePath"></param>
		static void _InjectAttribute(string path, string savePath)
		{
			var md = ModuleDefinition.ReadModule(path, new ReaderParameters());
			foreach(var s in s_inject) {
				var ca = new CustomAttribute(md.ImportReference(typeof(InternalsVisibleToAttribute).GetConstructor(new[] { typeof(string) })));
				ca.ConstructorArguments.Add(new CustomAttributeArgument(md.TypeSystem.String, s));
				md.Assembly.CustomAttributes.Add(ca);
			}

			//rejected: make method public: private Microsoft.CodeAnalysis.CSharp.CSharpSemanticModel.LookupSymbolsInternal. Exception when calling. Same as when calling through reflection.
			//if(path.EndsWith("Microsoft.CodeAnalysis.CSharp.dll")) {
			//	var tdef = md.GetType("Microsoft.CodeAnalysis.CSharp.CSharpSemanticModel");
			//	var method = tdef.Methods.Where(o => o.Name == "LookupSymbolsInternal").First();
			//	//Console.WriteLine(method);
			//	//Console.WriteLine($"{method.IsPrivate}, {method.IsPublic}");
			//	//method.IsPrivate = false;
			//	method.IsPublic = true;
			//	//Console.WriteLine($"{method.IsPrivate}, {method.IsPublic}");

			//	tdef = md.GetType("Microsoft.CodeAnalysis.CSharp.Symbol");
			//	tdef.IsPublic = true;
			//}

			md.Write(savePath);
			//note: if the assembly is signed, need to resign:
			//md.Write(savePath, new WriterParameters { StrongNameKeyPair = new StrongNameKeyPair(File.ReadAllBytes(snkFile)) });
		}

		static string[] s_inject = {
			"Au.Editor, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e"
			};
	}
}
