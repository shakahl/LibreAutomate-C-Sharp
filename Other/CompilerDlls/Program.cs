using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

//This small program modifies the Roslyn solution.
//Setup:
//Download Roslyn solution to Q:\Downloads\roslyn-master.
//Run this project. It modifies Roslyn solution files.
//	Currently does not add the InternalsVisible.cs files; will need to add them manually or edit this project.
//In Roslyn solution compile Microsoft.CodeAnalysis.CSharp.Features. It also compiles other 5. Copies the 6 dlls to Q:\app\Au\Other\CompilerDlls.
//In _Au.Editor: Add references to the 6 dlls that are in folder Q:\app\Au\Other\CompilerDlls. On build will copy to _.
//	VS will detect when the dlls modified when building Roslyn.
//To get other dlls:
//	Install or update Microsoft.CodeAnalysis.Features from NuGet in this project.
//	Optionally remove the main 6 references, to reduce noise in Object Browser etc.
//	Copy these dlls from C:\Users\G\.nuget\packages to _ (don't remember, maybe to Q:\app\Au\Other\CompilerDlls):
//		Microsoft.CodeAnalysis.FlowAnalysis.Utilities.dll
//		Microsoft.DiaSymReader.dll
//		System.Composition.AttributedModel.dll
//		System.Composition.Hosting.dll
//		System.Composition.Runtime.dll
//		System.Composition.TypedParts.dll
//		maybe in the future will need System.Composition.Convention.dll
//If at run time says "dll not found", try to add the dll to references.

namespace CompilerDlls
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				ModRoslyn();
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
    <Exec Command=""copy &quot;$(TargetPath)&quot; &quot;Q:\app\Au\Other\CompilerDlls\$(TargetFileName)&quot; /y"" />
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
	It will compile 6 projects.");
		}
	}
}
