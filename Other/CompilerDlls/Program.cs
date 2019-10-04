using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Mono.Cecil;
using System.Runtime.CompilerServices;

//This small program does one of the following, depending on the '#if ...' below:

//#if false: Copies Roslyn dlls to _\Compiler folder. Setup:
//	Install or update Microsoft.CodeAnalysis.Features from NuGet in this project.
//	Optionally remove the main 6 references, to reduce noise in Object Browser etc.
//	Make '#if false' below and run this project. It copies Roslyn dlls to _\Compiler folder.
//	Except main 6 dlls, which we build separately in Roslyn solution.

//#if true: Adds modifications to the Roslyn solution. Setup:
//	Download Roslyn solution to Q:\Downloads\roslyn-master.
//	Make '#if true' below and run this project. It modifies Roslyn solution files.
//	Currently does not add the InternalsVisible.cs files; will need to add them manually or edit this project.

//Roslyn setup in other projects (Au.Compiler, Au.Editor):
//Add references from _\Compiler folder and make "copy local" = false.
//	For Au.Compiler need only Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, System.Collections.Immutable. For Au.Editor need more.
//Let Au.Compiler output be _\Compiler folder. Also let its Au reference be "copy local" = false.
//In projects that use Au.Compiler as a reference:
//	Make its "copy local" = false.
//	In app.config add: configuration/runtime/assemblyBinding: <probing privatePath = "Compiler" />
//	May need "Auto-generate binding redirects" in project properties.


namespace CompilerDlls
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				ModRoslyn();
				//CopyDlls();
			}
			catch(Exception ex) { Console.WriteLine(ex); }
		}

		static void ModRoslyn()
		{
			bool writeFile = true;

			string roslynDir = @"Q:\Downloads\roslyn-master\src\";

#if true
			//add Symbols property to the CompletionItem class
			_Mod(@"Features\Core\Portable\Completion\CompletionItem.cs",
(@"internal bool IsCached { get; set; }",
@"
        public System.Collections.Generic.IReadOnlyList<ISymbol> Symbols { get; internal set; } //au
", 1),
				(@"ProviderName = ProviderName",
				@"                , Symbols = Symbols //au
", 1)
				);

			_Mod(@"Features\Core\Portable\Completion\Providers\SymbolCompletionItem.cs",
(@"tags: tags);",
@"            item.Symbols = symbols; //au
", 1)
				);

			_Mod(@"Features\Core\Portable\PublicAPI.Unshipped.txt",
(@"",
@"Microsoft.CodeAnalysis.Completion.CompletionItem.Symbols.get -> System.Collections.Generic.IReadOnlyList<Microsoft.CodeAnalysis.ISymbol>
", -1)
				);

			//add Symbol property to the SymbolKeySignatureHelpItem class (this code still not tested)
			_Mod(@"Features\Core\Portable\SignatureHelp\AbstractSignatureHelpProvider.SymbolKeySignatureHelpItem.cs",
(@"public SymbolKey? SymbolKey { get; }",
@"
            public ISymbol Symbol { get; } //au
", 1),
(@"SymbolKey = symbol?.GetSymbolKey();",
@"
                Symbol = symbol; //au
", 1)
				);
#endif

			var project = @"</Project>";
			var copy = @"  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""copy &quot;$(TargetPath)&quot; &quot;q:\app\au\_\Compiler\$(TargetFileName)&quot; /y"" />
  </Target>
";
			_Mod(@"Features\CSharp\Portable\Microsoft.CodeAnalysis.CSharp.Features.csproj", (project, copy, -1));
			_Mod(@"Features\Core\Portable\Microsoft.CodeAnalysis.Features.csproj", (project, copy, -1));
			_Mod(@"Compilers\CSharp\Portable\Microsoft.CodeAnalysis.CSharp.csproj", (project, copy, -1));
			_Mod(@"Compilers\Core\Portable\Microsoft.CodeAnalysis.csproj", (project, copy, -1));
			_Mod(@"Workspaces\CSharp\Portable\Microsoft.CodeAnalysis.CSharp.Workspaces.csproj", (project, copy, -1));
			_Mod(@"Workspaces\Core\Portable\Microsoft.CodeAnalysis.Workspaces.csproj", (project, copy, -1));

			//how: 0 replace, 1 insert after, -1 insert before
			void _Mod(string file, params (string find, string add, int how)[] p)
			{
				file = roslynDir + file;
				var s = File.ReadAllText(file);
				int moded = 0;
				foreach(var v in p) {
					if(_Mod1(ref s, v.find, v.add, v.how)) moded++;
				}
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"Made {moded} mods in {file}");
				Console.ForegroundColor = ConsoleColor.White;
				if(moded == 0) return;
				if(writeFile) File.WriteAllText(file, s);
				else Console.WriteLine(s);
			}

			bool _Mod1(ref string s, string find, string add, int how)
			{
				//if(s.Contains(add)) return false;
				var s2 = s.Replace("\r", "");
				var add2 = add.Replace("\r", "");
				if(s2.Contains(add2)) return false;

				int i = 0, len = 0;
				if(find.Length > 0) {
					var m = Regex.Match(s, "(?m)^[ \t]*" + Regex.Escape(find) + (how < 0 ? "$" : "\r?\n"), RegexOptions.CultureInvariant);
					if(!m.Success) throw new Exception($"Cannot find '{find}'.");
					i = m.Index;
					len = m.Length;
				}
				switch(how) {
				case 0:
					s = s.Remove(i, len);
					break;
				case 1:
					i += len;
					break;
				}
				s = s.Insert(i, add);

				return true;
			}

			Console.WriteLine(@"Roslyn source has heen modified successfully.
	Please compile project Microsoft.CodeAnalysis.CSharp.Features in Roslyn solution.
	It will compile 6 projects. Each will copy its output dll to the _\Compilers folder.");
		}

		static void CopyDlls()
		{
			var rx = new Regex(@"(?i)^(System|Microsoft)\..+\.(dll|xml)$", RegexOptions.CultureInvariant); //info: don't copy pdb, because debug info is embedded in assemblies
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
				//case "Microsoft.CodeAnalysis.CSharp.dll":
				//case "Microsoft.CodeAnalysis.Features.dll":
				//case "Microsoft.CodeAnalysis.CSharp.Features.dll":
				//	Console.ForegroundColor = ConsoleColor.Green;
				//	Console.WriteLine(s);
				//	_InjectAttribute(f.FullName, s2);
				//	break;
				case "Microsoft.CodeAnalysis.dll":
				case "Microsoft.CodeAnalysis.CSharp.dll":
				case "Microsoft.CodeAnalysis.Features.dll":
				case "Microsoft.CodeAnalysis.CSharp.Features.dll":
				case "Microsoft.CodeAnalysis.Workspaces.dll":
				case "Microsoft.CodeAnalysis.CSharp.Workspaces.dll":
					//don't copy because now we build these in the Roslyn solution and directly copy where need.
					//Console.ForegroundColor = ConsoleColor.Green;
					//Console.WriteLine(s);
					//File.Copy(@"Q:\Downloads\roslyn-master\artifacts\bin\" + Path.GetFileNameWithoutExtension(s) + @"\Debug\netstandard2.0\" + s, s2, true);
					break;
				default:
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine(s);
					File.Copy(f.FullName, s2, true);
					break;
				}
			}
		}

		//rejected. Now we instead modify and compile Roslyn dlls.
		///// <summary>
		///// Adds [assembly: InternalsVisibleTo("Au.Editor...")]
		///// </summary>
		///// <param name="path"></param>
		///// <param name="savePath"></param>
		//static void _InjectAttribute(string path, string savePath)
		//{
		//	var md = ModuleDefinition.ReadModule(path, new ReaderParameters());
		//	foreach(var s in s_inject) {
		//		var ca = new CustomAttribute(md.ImportReference(typeof(InternalsVisibleToAttribute).GetConstructor(new[] { typeof(string) })));
		//		ca.ConstructorArguments.Add(new CustomAttributeArgument(md.TypeSystem.String, s));
		//		md.Assembly.CustomAttributes.Add(ca);
		//	}

		//	//rejected: make method public: private Microsoft.CodeAnalysis.CSharp.CSharpSemanticModel.LookupSymbolsInternal. Exception when calling. Same as when calling through reflection.
		//	//if(path.EndsWith("Microsoft.CodeAnalysis.CSharp.dll")) {
		//	//	var tdef = md.GetType("Microsoft.CodeAnalysis.CSharp.CSharpSemanticModel");
		//	//	var method = tdef.Methods.Where(o => o.Name == "LookupSymbolsInternal").First();
		//	//	//Console.WriteLine(method);
		//	//	//Console.WriteLine($"{method.IsPrivate}, {method.IsPublic}");
		//	//	//method.IsPrivate = false;
		//	//	method.IsPublic = true;
		//	//	//Console.WriteLine($"{method.IsPrivate}, {method.IsPublic}");

		//	//	tdef = md.GetType("Microsoft.CodeAnalysis.CSharp.Symbol");
		//	//	tdef.IsPublic = true;
		//	//}

		//	md.Write(savePath);
		//	//note: if the assembly is signed, need to resign:
		//	//md.Write(savePath, new WriterParameters { StrongNameKeyPair = new StrongNameKeyPair(File.ReadAllBytes(snkFile)) });
		//}

		//static string[] s_inject = {
		//	"Au.Editor, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e"
		//	};
	}
}
