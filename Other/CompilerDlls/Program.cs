using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

//This small program modifies the Roslyn solution.
//Setup:
//Download Roslyn solution to Q:\Downloads\roslyn-master.
//Open Roslyn.sln.
//To make VS not so slow, select all folders and unload projects. Then load only the 6 projects we need:
//	In folder Compilers: Core\Microsoft.CodeAnalysis, CSharp\Microsoft.CodeAnalysis.CSharp.
//	In folder Features: Microsoft.CodeAnalysis.CSharp.Features, Microsoft.CodeAnalysis.Features.
//	In folder Workspaces: Microsoft.CodeAnalysis.CSharp.Workspaces, Microsoft.CodeAnalysis.Workspaces.
//Edit as described in the '#if false' block at the bottom of this file.
//Run this project. It modifies Roslyn solution project files.
//In Roslyn solution compile Microsoft.CodeAnalysis.CSharp.Features. It also compiles other 5. Copies the 6 dlls to Q:\app\Au\Other\CompilerDlls.
//In editor project: Add references to the 6 dlls that are in folder Q:\app\Au\Other\CompilerDlls. On build will copy to _.
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

#if false
//Edit these manually, because either difficult to automate or Roslyn source in new version is very likely changed in that place.
//Add only internal members. If public, need to declare it in PublicApi.Shipped.txt. Roslyn's internals are visible to the editor project.

// - Add Symbols property to the CompletionItem class:
//1. Open Features\Core\Portable\Completion\CompletionItem.cs in project Microsoft.CodeAnalysis.Features.
//2. Find method private CompletionItem With(...). In it find: return new CompletionItem...{
//3. In the { } add line: Symbols = Symbols, //au
//4. Below the method add property: internal System.Collections.Generic.IReadOnlyList<ISymbol> Symbols { get; set; } //au
//5. Open Features\Core\Portable\Completion\Providers\SymbolCompletionItem.cs.
//6. In method CreateWorker find: tags: tags);
//7. Below add: item.Symbols = symbols; //au

// - Add Symbol property to the SymbolKeySignatureHelpItem class:
//1. Open Features\Core\Portable\SignatureHelp\AbstractSignatureHelpProvider.SymbolKeySignatureHelpItem.cs.
//2. Add property: internal ISymbol Symbol { get; } //au
//3. In ctor add:  Symbol = symbol; //au

// - Let it don't try to load VB assemblies, because then exception when debugging:
//In MefHostServices.cs, in s_defaultAssemblyNames init list, remove the 2 VB assemblies.

// - In all 6 projects add link to Au.InternalsVisible.cs. It is in this project.

// - In all 6 projects .csproj replace <TargetFrameworks>netcoreapp3.1;netstandard2.0</TargetFrameworks> with <TargetFramework>netcoreapp3.1</TargetFramework>

// - In project Microsoft.CodeAnalysis, in MetadataReader\PEAssembly.cs, replace GetInternalsVisibleToPublicKeys with:

        internal IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
        {
            if (_lazyInternalsVisibleToMap == null)
                Interlocked.CompareExchange(ref _lazyInternalsVisibleToMap, BuildInternalsVisibleToMap(), null);

            List<ImmutableArray<byte>> result;

            _lazyInternalsVisibleToMap.TryGetValue(simpleName, out result);

            //au
            return result
                ?? GetAuInternalsVisible(this.Identity.Name, simpleName, false)
                ?? SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
        }

        //au
        internal static Func<string, string, bool, bool> AuInternalsVisible;
        static ImmutableArray<byte>[] s_aivIA;
        internal static ImmutableArray<byte>[] GetAuInternalsVisible(string thisName, string toName, bool source)
        {
            if (AuInternalsVisible?.Invoke(thisName, toName, source) ?? false)
                return s_aivIA ??= new ImmutableArray<byte>[1] { default };
            return null;
        }

// - In project Microsoft.CodeAnalysis.CSharp, in Symbols\Source\SourceAssemblySymbol.cs, replace GetInternalsVisibleToPublicKeys with:

        //au
        internal override IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
        {
            EnsureAttributesAreBound();

            if (_lazyInternalsVisibleToMap != null && _lazyInternalsVisibleToMap.TryGetValue(simpleName, out var result))
                return result.Keys;

            return PEAssembly.GetAuInternalsVisible(this.Name, simpleName, true)
                ?? SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
        }
        //internal override IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
        //{
        //    //EDMAURER assume that if EnsureAttributesAreBound() returns, then the internals visible to map has been populated.
        //    //Do not optimize by checking if m_lazyInternalsVisibleToMap is Nothing. It may be non-null yet still
        //    //incomplete because another thread is in the process of building it.

        //    EnsureAttributesAreBound();

        //    if (_lazyInternalsVisibleToMap == null)
        //        return SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();

        //    ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>> result = null;

        //    _lazyInternalsVisibleToMap.TryGetValue(simpleName, out result);

        //    return (result != null) ? result.Keys : SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
        //}

#endif
