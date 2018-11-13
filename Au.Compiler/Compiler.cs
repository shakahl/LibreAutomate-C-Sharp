//#define STANDARD_SCRIPT

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
using System.Linq;

using Au.Types;
using static Au.NoClass;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Resources;
using Au.LibRun;
using System.Collections.Immutable;

namespace Au.Compiler
{
	/// <summary>
	/// Compiles C# files.
	/// </summary>
	public static partial class Compiler
	{
		/// <summary>
		/// Compiles C# file or project if need.
		/// Returns false if fails (C# errors etc).
		/// </summary>
		/// <param name="forRun">Don't recompile if compiled.</param>
		/// <param name="r">Results.</param>
		/// <param name="f">C# file. If projFolder used, must be the main file of the project.</param>
		/// <param name="projFolder">null or project folder.</param>
		/// <remarks>
		/// Must be always called in the main UI thread (Thread.CurrentThread.ManagedThreadId == 1), because calls its file collection functions.
		/// 
		/// Adds <see cref="DefaultReferences"/>. For scripts adds <see cref="DefaultUsings"/>.
		/// 
		/// If f is not script/app/dll:
		///		If forRun, does not compile (just parses meta), sets r.outputType=dll and returns false.
		///		Else compiles but does not create output files.
		/// </remarks>
		public static bool Compile(bool forRun, out CompResults r, IWorkspaceFile f, IWorkspaceFile projFolder = null)
		{
			Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1);
			r = null;
			var cache = XCompiled.OfCollection(f.IcfWorkspace);
			bool isCompiled = forRun && cache.IsCompiled(f, out r, projFolder);

			//Print("isCompiled=" + isCompiled);

			if(!isCompiled) {
				bool ok = false;
				try {
					ok = _Compile(forRun, f, out r, projFolder);
				}
				catch(Exception ex) {
					Print($"Failed to compile '{f.Name}'. {ex.ToStringWithoutStack_()}");
				}
				finally {
					LibSetTimerGC();
				}

				if(!ok) {
					f.IcfTriggers(null);
					cache.Remove(f, false);
					return false;
				}
			}

			return true;
		}

		/// <summary>_Compile() output assembly info.</summary>
		public class CompResults
		{
			/// <summary>C# file name without ".cs".</summary>
			public string name;

			/// <summary>Full path of assembly file.</summary>
			public string file;

			public EOutputType outputType;
			public ERunMode runMode;
			public EIfRunning ifRunning;
			public EUac uac;
			public bool prefer32bit;

			/// <summary>Has config file this.file + ".config".</summary>
			public bool hasConfig;

			/// <summary>Main() does not have [STAThread].</summary>
			public bool mtaThread;

			/// <summary>The assembly is normal .exe or .dll file, not in cache. If exe, its dependencies were copied to its directory.</summary>
			public bool notInCache;

			/// <summary>In cache assembly files we append portable PDB to the assembly file at this offset.</summary>
			public int pdbOffset;
		}

		static bool _Compile(bool forRun, IWorkspaceFile f, out CompResults r, IWorkspaceFile projFolder)
		{
			r = new CompResults();
			//Perf.Next();

			var m = new MetaComments();
			if(!m.Parse(f, projFolder, EMPFlags.PrintErrors)) return false;
			var err = m.Errors;
			//Perf.Next('m');

			bool needOutputFiles = !(m.OutputType == EOutputType.dll && m.OutputPath == null);

			//if for run, don't compile if f is not executable, unless creating dll
			if(forRun && !needOutputFiles) {
				r.outputType = EOutputType.dll;
				return false;
			}

			XCompiled cache = XCompiled.OfCollection(f.IcfWorkspace);
			string outPath = null, outFile = null;
			if(needOutputFiles) {
				string fileName;
				if(m.OutputPath != null) {
					outPath = m.OutputPath; //info: the directory is already created
					fileName = m.Name + (m.OutputType == EOutputType.dll ? ".dll" : ".exe");
				} else {
					outPath = cache.CacheDirectory;
					File_.CreateDirectory(outPath);
					fileName = f.IdString;
				}
				outFile = outPath + "\\" + fileName;
			}

			if(m.PreBuild.f != null && !_RunPrePostBuildScript(false, m, outFile)) return false;

			string[] usings = null;
			var trees = new List<CSharpSyntaxTree>(m.Files.Count + 1);
			var po = new CSharpParseOptions(LanguageVersion.Latest,
				m.XmlDocFile != null ? DocumentationMode.Parse : DocumentationMode.None,
				m.IsScript ? SourceCodeKind.Script : SourceCodeKind.Regular,
				m.Defines);

#if STANDARD_SCRIPT
		foreach(var f1 in m.Files) {
			var tree = CSharpSyntaxTree.ParseText(f1.code, po, f1.f.IdString + " " + f1.f.Name, Encoding.UTF8) as CSharpSyntaxTree;
			trees.Add(tree);
		}
		if(m.IsScript) {
			usings = s_usingsForScript;
			var treeAdd = CSharpSyntaxTree.ParseText(
@"static class __script__ {
internal static string[] args = System.Array.Empty<string>();
}", new CSharpParseOptions(LanguageVersion.Latest)) as CSharpSyntaxTree;
			trees.Add(treeAdd);
		}
#else
			bool transform = m.IsScript;
			foreach(var f1 in m.Files) {
				var tree = CSharpSyntaxTree.ParseText(f1.code, po, f1.f.FilePath, Encoding.UTF8) as CSharpSyntaxTree;
				//info: file path is used later in several places: in compilation error messages, run time stack traces (from PDB), Visual Studio debugger, etc.
				//	Our OutputServer.SetNotifications callback will convert file/line info to links. It supports compilation errors and run time stack traces.

				if(err.AddAllAndPrint(tree, f1.f, printWarnings: transform)) return false;
				//info: if script, print warnings now, else #warning would not work

				if(transform) {
					transform = false;
					po = po.WithKind(SourceCodeKind.Regular);
					var code2 = _TransformScriptCode(tree, f1.code, true, err, f1.f);
					if(err.ErrorCount != 0) { err.PrintAll(); return false; }
					tree = CSharpSyntaxTree.ParseText(code2, po, tree.FilePath, Encoding.UTF8) as CSharpSyntaxTree;

					//speed: adds 5% of total compilation time. Emit is 90%.

					//Also tried to parse wrapper template and then ReplaceNode etc. To avoid parsing script code 2 times.
					//	Same speed or slower. Much more difficult. Need #line too.
				}
				trees.Add(tree);
			}
#endif
			//Perf.Next('t');

			OutputKind ot;
			switch(m.OutputType) {
			case EOutputType.dll: ot = OutputKind.DynamicallyLinkedLibrary; break;
			case EOutputType.console: ot = OutputKind.ConsoleApplication; break;
			default: ot = OutputKind.WindowsApplication; break;
			}

			var options = new CSharpCompilationOptions(
			   ot,
			   usings: usings,
			   optimizationLevel: m.IsDebug ? OptimizationLevel.Debug : OptimizationLevel.Release, //speed: compile the same, load Release slightly slower. Default Debug.
			   allowUnsafe: true,
			   platform: m.Prefer32Bit ? Platform.AnyCpu32BitPreferred : Platform.AnyCpu,
			   warningLevel: m.WarningLevel,
			   specificDiagnosticOptions: m.DisableWarnings?.Select(wa => new KeyValuePair<string, ReportDiagnostic>(Char_.IsAsciiDigit(wa[0]) ? ("CS" + wa.PadLeft(4, '0')) : wa, ReportDiagnostic.Suppress)),
			   cryptoKeyFile: m.SignFile?.FilePath, //also need strongNameProvider
			   strongNameProvider: m.SignFile == null ? null : new DesktopStrongNameProvider()
			   );

			var compilation = CSharpCompilation.Create(m.Name, trees, m.References.Refs, options);
			//Perf.Next('c');

			string pdbFile = null, xdFile = null;
			MemoryStream pdbStream = null;
			Stream xdStream = null, resNat = null;
			ResourceDescription[] resMan = null;
			EmitOptions eOpt = null;

			if(needOutputFiles) {
				_AddAttributes(ref compilation, ot != OutputKind.DynamicallyLinkedLibrary && m.RunMode != ERunMode.editorThread);

				//create debug info always. It is used for run-time error links.
				pdbStream = new MemoryStream();
				if(m.OutputPath != null) {
					pdbFile = Path.ChangeExtension(outFile, "pdb");
					eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.Pdb, pdbFilePath: pdbFile);
					//eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb, pdbFilePath: pdbFile); //smaller, but not all tools support it
				} else {
					//eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.Embedded); //no, it is difficult to extract, because we load assembly from byte[] to avoid locking. We instead append portable PDB stream to the assembly stream.
					eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);
					//adds < 1 KB; almost the same compiling speed. Separate pdb file is 14 KB; 2 times slower compiling, slower loading.
				}

				if(m.XmlDocFile != null) xdStream = File_.WaitIfLocked(() => File.Create(xdFile = Path_.Normalize(m.XmlDocFile, outPath)));

				resMan = _CreateManagedResources(m);
				if(err.ErrorCount != 0) { err.PrintAll(); return false; }

				resNat = _CreateNativeResources(m, compilation);
				if(err.ErrorCount != 0) { err.PrintAll(); return false; }

				//EmbeddedText.FromX //it seems we can embed source code in PDB. Not tested.
			}

			var asmStream = new MemoryStream(4096);
			var emitResult = compilation.Emit(asmStream, pdbStream, xdStream, resNat, resMan, eOpt);

			if(needOutputFiles) {
				xdStream?.Dispose();
				resNat?.Dispose(); //info: compiler disposes resMan
			}
			//Perf.Next('e');

			var diag = emitResult.Diagnostics;
			if(!diag.IsEmpty) {
				foreach(var d in diag) {
					if(d.Severity == DiagnosticSeverity.Hidden) continue;
					err.AddErrorOrWarning(d, f);
					if(d.Severity == DiagnosticSeverity.Error && d.Id == "CS0009") m.References.RemoveBadReference(d.GetMessage());
				}
				if(!err.IsEmpty) err.PrintAll();
			}
			if(!emitResult.Success) {
				if(needOutputFiles) {
					File_.Delete(outFile);
					if(pdbFile != null) File_.Delete(pdbFile);
					if(xdFile != null) File_.Delete(xdFile);
				}
				return false;
			}

			if(needOutputFiles) {
				//If there is no [STAThread], will need MTA thread.
				if(!m.IsScript && ot != OutputKind.DynamicallyLinkedLibrary && m.RunMode != ERunMode.editorThread) {
					bool hasSTAThread = compilation.GetEntryPoint(default)?.GetAttributes().Any(o => o.ToString() == "System.STAThreadAttribute") ?? false;
					if(!hasSTAThread) r.mtaThread = true;
				}

				//create assembly file
				asmStream.Position = 0;
				using(var fileStream = File_.WaitIfLocked(() => File.Create(outFile, (int)asmStream.Length))) {
					asmStream.CopyTo(fileStream);

					pdbStream.Position = 0;
					if(m.OutputPath == null) {
						pdbStream.CopyTo(fileStream);
						r.pdbOffset = (int)asmStream.Length;
					} else {
						using(var v = File_.WaitIfLocked(() => File.Create(pdbFile))) pdbStream.CopyTo(v);
					}
				}
				r.file = outFile;

				//copy non-.NET references to the output directory
				if(m.OutputPath != null && m.OutputType != EOutputType.dll) {
					//Perf.Next();
					_CopyReferenceFiles(m);
				}

				//copy config file to the output directory
				var configFile = outFile + ".config";
				if(m.ConfigFile != null) {
					r.hasConfig = true;
					_CopyFileIfNeed(m.ConfigFile.FilePath, configFile);
				} else if(File_.ExistsAsFile(configFile, true)) {
					File_.Delete(configFile);
				}
			}

			if(m.PostBuild.f != null && !_RunPrePostBuildScript(true, m, outFile)) return false;

			//Perf.First();
			if(needOutputFiles) {
				if(m.OutputType != EOutputType.dll) _Triggers(f, compilation);
				cache.AddCompiled(f, outFile, m, r.pdbOffset, r.mtaThread);
			}
			//Perf.NW();

			r.name = m.Name;
			r.outputType = m.OutputType;
			r.ifRunning = m.IfRunning;
			r.runMode = m.RunMode;
			r.uac = m.Uac;
			r.prefer32bit = m.Prefer32Bit;
			r.notInCache = m.OutputPath != null;

			//Perf.NW();
			return true;
		}

		/// <summary>
		/// Adds some attributes if not specified in code.
		/// Adds: [module: DefaultCharSet(CharSet.Unicode)];
		/// If needVersionEtc, also adds: AssemblyCompany, AssemblyProduct, AssemblyInformationalVersion. It is to avoid exception in Application.ProductName etc when the entry assembly is loaded from byte[].
		/// </summary>
		static void _AddAttributes(ref CSharpCompilation compilation, bool needVersionEtc)
		{
			int needAttr = 0x100;

			foreach(var v in compilation.SourceModule.GetAttributes()) {
				//Print(v.AttributeClass.Name);
				switch(v.AttributeClass.Name) {
				case "DefaultCharSetAttribute": needAttr &= ~0x100; break;
				}
			}

			if(needVersionEtc) {
				needAttr |= 7;
				foreach(var v in compilation.Assembly.GetAttributes()) {
					//Print(v.AttributeClass.Name);
					switch(v.AttributeClass.Name) {
					case "AssemblyCompanyAttribute": needAttr &= ~1; break;
					case "AssemblyProductAttribute": needAttr &= ~2; break;
					case "AssemblyInformationalVersionAttribute": needAttr &= ~4; break;
					}
				}
			}

			if(needAttr != 0) {
				using(new Util.LibStringBuilder(out var sb)) {
					sb.AppendLine("using System.Reflection;using System.Runtime.InteropServices;");
					if(0 != (needAttr & 0x100)) sb.AppendLine("[module: DefaultCharSet(CharSet.Unicode)]");
					if(0 != (needAttr & 1)) sb.AppendLine("[assembly: AssemblyCompany(\"Au\")]");
					if(0 != (needAttr & 2)) sb.AppendLine("[assembly: AssemblyProduct(\"Script\")]");
					if(0 != (needAttr & 4)) sb.AppendLine("[assembly: AssemblyInformationalVersion(\"0\")]");

					var tree = CSharpSyntaxTree.ParseText(sb.ToString(), new CSharpParseOptions(LanguageVersion.Latest)) as CSharpSyntaxTree;
					compilation = compilation.AddSyntaxTrees(tree);
				}
			}
		}

		static void _Triggers(IWorkspaceFile fMain, CSharpCompilation compilation)
		{
			List<CompTriggerData> a = null;
			_Triggers2(compilation.Assembly.GetAttributes(), null);
			var entry = compilation.GetEntryPoint(default);
			foreach(var m in entry.ContainingType.GetMembers()) {
				if(m.Kind != SymbolKind.Method || !m.CanBeReferencedByName || m.IsExtern || m == entry) continue;
				_Triggers2(m.GetAttributes(), m.Name);
			}
			fMain.IcfTriggers(a);

			void _Triggers2(ImmutableArray<AttributeData> attributes, string method)
			{
				foreach(var v in attributes) {
					var c = v.AttributeClass;
					var t1 = c.ContainingType; if(t1 == null || t1.Name != "Trigger") continue;
					var ca = v.ConstructorArguments;
					var na = v.NamedArguments;
					int n1 = ca.Length, n2 = na.Length;
					var k = new KeyValuePair<string, object>[n1 + n2];
					for(int i = 0; i < n1; i++) k[i] = new KeyValuePair<string, object>(null, ca[i].Value);
					for(int i = 0; i < n2; i++) k[i + n1] = new KeyValuePair<string, object>(na[i].Key, na[i].Value.Value);

					(a ?? (a = new List<CompTriggerData>())).Add(new CompTriggerData(method, c.Name, k));
				}
			}
		}

		static ResourceDescription[] _CreateManagedResources(MetaComments m)
		{
			var a = m.Resources;
			if(a == null || a.Count == 0) return null;
			var stream = new MemoryStream();
			var rw = new ResourceWriter(stream);
			IWorkspaceFile curFile = null;
			try {
				foreach(var v in a) {
					curFile = v.f;
					string name = curFile.Name, path = curFile.FilePath;
					object o;
					switch(v.s) {
					case null:
						switch(Path_.GetExtension(name).ToLower_()) {
						case ".png":
						case ".bmp":
						case ".jpg":
						case ".jpeg":
						case ".gif":
						case ".tif":
						case ".tiff":
							o = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path);
							break;
						case ".ico":
							//o = Icon_.GetFileIcon(path, size); //adds 4-bit icon, distorted
							o = new System.Drawing.Icon(path); //adds of all sizes. At run time: var icon=new Icon(GetResourceIcon(), 16, 16).
							break;
						//case ".cur":
						//	o = Au.Util.Cursor_.LoadCursorFromFile(path); //error: Stock cursors cannot be serialized
						//	break;
						default:
							o = File_.LoadBytes(path);
							break;
						}
						break;
					case "string":
						o = File_.LoadText(path);
						break;
					case "strings":
						var csv = CsvTable.Load(path);
						if(csv.ColumnCount != 2) throw new ArgumentException("CSV must contain 2 columns separated with ,");
						foreach(var row in csv.Data) {
							rw.AddResource(row[0], row[1]);
						}
						continue;
					default: throw new ArgumentException("error in meta: Incorrect /suffix");
					}
					rw.AddResource(name, o);
				}
				curFile = null;
				rw.Generate();
			}
			catch(Exception e) {
				rw.Dispose();
				_ResourceException(e, m, curFile);
				return null;
			}
			//note: don't Close/Dispose rw. It closes stream. Compiler will close it. There is no other disposable data in rw.
			stream.Position = 0;
			return new ResourceDescription[] { new ResourceDescription("Project.Resources.resources", () => stream, false) };
		}

		static Stream _CreateNativeResources(MetaComments m, CSharpCompilation compilation)
		{
			var manifest = m.ManifestFile;

			//add default manifest if need
			string manifestPath = null;
			if(manifest != null) manifestPath = manifest.FilePath;
			else if(m.OutputPath != null && m.OutputType != EOutputType.dll && m.ResFile == null) {
				manifestPath = Folders.ThisAppBS + "default.exe.manifest"; //don't: uac
				if(!File_.ExistsAsFile(manifestPath)) manifestPath = null;
			}

			if(m.IconFile == null && manifestPath == null && m.ResFile == null && m.OutputPath == null) return null;
			Stream manStream = null, icoStream = null;
			IWorkspaceFile curFile = null;
			try {
				if(m.ResFile != null) return File.OpenRead((curFile = m.ResFile).FilePath);
				if(manifestPath != null) { curFile = manifest; manStream = File.OpenRead(manifestPath); }
				if(m.IconFile != null) icoStream = File.OpenRead((curFile = m.IconFile).FilePath);
				curFile = null;
				return compilation.CreateDefaultWin32Resources(versionResource: m.OutputPath != null, noManifest: manifestPath == null, manStream, icoStream);
			}
			catch(Exception e) {
				manStream?.Dispose();
				icoStream?.Dispose();
				_ResourceException(e, m, curFile);
				return null;
			}
		}

		static void _ResourceException(Exception e, MetaComments m, IWorkspaceFile curFile)
		{
			var em = e.ToStringWithoutStack_();
			var err = m.Errors;
			var f = m.Files[0].f;
			if(curFile == null) err.AddError(f, "Failed to add resources. " + em);
			else err.AddError(f, $"Failed to add resource '{curFile.Name}'. " + em);
		}

		static void _CopyReferenceFiles(MetaComments m)
		{
			//info: tried to get all used references, unsuccessfully.
			//	Would need to create appdomain, load the assembly and get its references through reflection.
			//	And don't need it. We'll copy Au.dll and all non-default references that are not in the .NET folder.

			_CopyFileIfNeed(typeof(Wnd).Assembly.Location, m.OutputPath + @"\Au.dll");

			var refs = m.References.Refs;
			int i = DefaultReferences.Count;
			if(refs.Count > i) {
				//string netDir = Folders.NetFrameworkRuntime; //no GAC
				string netDir = Folders.Windows + @"Microsoft.NET\";
				for(; i < refs.Count; i++) {
					var s1 = refs[i].FilePath;
					if(s1.StartsWithI_(netDir)) continue;
					var s2 = m.OutputPath + "\\" + Path_.GetFileName(s1);
					//Print(s1, s2);
					_CopyFileIfNeed(s1, s2);
				}
			}
		}

		static void _CopyFileIfNeed(string sFrom, string sTo)
		{
			if(File_.GetProperties(sTo, out var p2, FAFlags.UseRawPath) //if exists
				&& File_.GetProperties(sFrom, out var p1, FAFlags.UseRawPath)
				&& p2.LastWriteTimeUtc == p1.LastWriteTimeUtc
				&& p2.Size == p1.Size) return;
			//Debug_.Print("copy");
			File_.Copy(sFrom, sTo, IfExists.Delete);
		}

		#region default references and usings

		/// <summary>
		/// These references are added when compiling any script/app/library.
		/// mscorlib, System, System.Core, System.Windows.Forms, System.Drawing, Au.dll.
		/// </summary>
		public static readonly Dictionary<string, string> DefaultReferences = new Dictionary<string, string>
		{
			{"mscorlib", typeof(object).Assembly.Location},
			{"System", typeof(Component).Assembly.Location},
			{"System.Core", typeof(HashSet<>).Assembly.Location},
			{"System.Windows.Forms", typeof(System.Windows.Forms.Form).Assembly.Location},
			{"System.Drawing", typeof(System.Drawing.Point).Assembly.Location},
			{"Au.dll", typeof(Wnd).Assembly.Location},

			//info: don't need to add System.ValueTuple.
			//speed: many references makes compiling much slower. We use temporary caching. Permanent caching would add many MB of process memory.
		};

		//note: DefaultReferences and DefaultUsings must be in sync. If there is no reference for an using, will be compiler error.
		//	Also in usings.txt, which is used in new .cs file templates.

		/// <summary>
		/// These usings are added to script code, before scripts's usings.
		/// </summary>
		public const string DefaultUsings = @"using System; using System.Collections.Generic; using System.Text; using System.Text.RegularExpressions; using System.Diagnostics; using System.Runtime.InteropServices; using System.IO; using System.Threading; using System.Threading.Tasks; using System.Windows.Forms; using System.Drawing; using System.Linq; using Au; using Au.Types; using static Au.NoClass; using Au.Triggers;";

#if STANDARD_SCRIPT
		//Implicit usings for scripts.
		static readonly string[] s_usingsForScript =
		{
			"System",
			"System.Collections.Generic",
			"System.Text",
			"System.Text.RegularExpressions",
			"System.Diagnostics",
			"System.Runtime.InteropServices",
			"System.IO",
			"System.Threading",
			"System.Threading.Tasks",
			"System.Windows.Forms",
			"System.Drawing",
			"System.Linq",
			"Au",
			"Au.Types",
			"Au.NoClass",
			"Au.Triggers",
			"__script__", //static class in file added when compiling, to make args available
			//speed: many usings makes compiling slower, but not as much as many references.
		};
#endif

		#endregion

		/// <summary>
		/// Converts script code to regular C# class with default usings, App class, [STAThread]Main(string[] args){} and }.
		/// Returns false if there are ##/#} errors; writes errors to err or prints in the output.
		/// </summary>
		/// <param name="f">Used only for error links.</param>
		/// <param name="code">f text. Receives transformed text.</param>
		/// <param name="compiling">Insert #line etc.</param>
		/// <param name="err">If used, this func writes errors to it. Else prints errors in the output.</param>
		/// <remarks>
		/// Need it because of these limitations of standard C# script:
		///		There is no normal Main, therefore cannot have string[] args (command line), [STAThread], etc.
		///			It can be simulated, but not when runs as exe. Probably would need to not allow script exe.
		///		Stack trace (eg on exception) contains lots of meaningless lines instead of void Main().
		///		Cannot have ref locals in global code.
		///			Now still problem, but smaller: ref locals can be used, but only if in { }.
		///		All variables in global code are fields, unless in { }. Their initialization behavior is different than in regular code. No warnings 'unused variable'.
		///			Now variables declared without a modifier (private, static, const, etc) are locals, not fields. Also, they must be declared before other code.
		///			Difficult to make it like in standard C# script. Need to transform eg 'var i=5;' to 'int i; i=5;' (note the 'var' -> 'int').
		///		Cannot be partial class. Now the wrapper class is partial.
		///		And maybe more, not discovered.
		///	So, now we still have these not very important limitations:
		///		ref locals must be in { }.
		///		Namespaces not allowed.
		///	Also, I don't like parsing the same code 2 times, although only 5% slower. Inserting parsed syntaxnodes into wrapper tree is difficult and not faster.
		///	Also I tried another way, but rejected:
		///		If the script contains global code (usings etc) or class-level code (functions etc), let the user mark it, eg using directives.
		///		What is good:
		///			Don't need to parse 2 times. Just find the directives and split code using string functions, eg regex.
		///			Supports ref locals and even namespaces.
		///			Don't need 'private' etc modifiers for shared variables.
		///		What is bad:
		///			Users have to learn and remember to add these directives. Makes scripting less pleasant. The script is nonstandard, less elegant.
		///			When users forget or misplace a directive, parser shows series of errors with weird descriptions. Eg if [DllImport...]... is in Main code.
		///			Probably more difficult to implement intellisense.
		///	Now the script is standard C# script, except variable scope.
		/// </remarks>
		static string _TransformScriptCode(CSharpSyntaxTree tree, string code, bool compiling, ErrBuilder err, IWorkspaceFile f)
		{
			//Perf.Next('1');

			var aliases = new List<SyntaxNode>();
			var usings = new List<SyntaxNode>();
			var attributes = new List<SyntaxNode>();
			var members = new List<SyntaxNode>();
			var statements = new List<SyntaxNode>();

			//Perf.Next('2');
			var root = tree.GetCompilationUnitRoot();
			//Perf.Next('3');
			foreach(var v in root.ChildNodes()) {
				switch(v.Kind()) {
				case SyntaxKind.ExternAliasDirective:
					aliases.Add(v);
					break;
				case SyntaxKind.UsingDirective:
					usings.Add(v);
					break;
				case SyntaxKind.AttributeList:
					attributes.Add(v);
					break;
				case SyntaxKind.GlobalStatement:
					statements.Add(v);
					break;
				case SyntaxKind.FieldDeclaration:
					var kind = v.ChildNodesAndTokens().First().Kind(); //Print(kind);
					if(kind == SyntaxKind.VariableDeclaration) statements.Add(v); //int x; //let it be local variable
					else if(statements.Count == 0) members.Add(v); //private|public|internal|protected|static|readonly|volatile|const int x; //let it be field or shared constant
					else if(kind == SyntaxKind.ConstKeyword) statements.Add(v); //local constant
					else err.AddError(f, tree.GetLineSpan(v.Span).StartLinePosition, "error: Field declarations in script must precede other statements. Fields in script are variables declared with a modifier: private, public, internal, protected, static, readonly, volatile. They can be used in all functions of the script."); //don't allow to mix field declarations with other code. Fields are inited first, and users can make errors if they don't know or forget it.
					break;
				default:
					members.Add(v);
					break;
				}
			}
			//Perf.Next('4');

			//Print("<><Z 0xc000>globals:<>");
			//Print(globals);
			//Print("<><Z 0xc000>members:<>");
			//Print(members);
			//Print("<><Z 0xc000>statements:<>");
			//Print(statements);

			var b = new StringBuilder();
			int prevLine = -1;

			if(aliases.Count != 0) {
				foreach(var v in aliases) _Append(v);
				b.AppendLine();
			}
			//info: append usings here, because CSharpCompilationOptions.usings is ignored if not script.
			if(compiling) b.AppendLine(@"#line 1 ""<wrapper>""");
			b.AppendLine(DefaultUsings);
			if(usings.Count != 0) {
				if(compiling) b.AppendLine("#line default\r\n");
				prevLine = -2;
				foreach(var v in usings) _Append(v);
				if(compiling) b.AppendLine("\r\n#line 2 \"<wrapper>\"");
			}
			if(attributes.Count != 0) {
				if(compiling) b.AppendLine("#line default\r\n");
				prevLine = -2;
				foreach(var v in attributes) _Append(v);
				if(compiling) b.AppendLine("\r\n#line 3 \"<wrapper>\"");
			}
			if(!compiling) b.AppendLine();
			b.AppendLine(
@"sealed unsafe partial class App :AuAppBase {
[STAThread] static void Main(string[] args) { new App()._Main(args); }
void _Main(string[] args) {");
			if(compiling) b.AppendLine("#line default\r\n");
			prevLine = -2;
			foreach(var v in statements) _Append(v);
			if(compiling) b.AppendLine().AppendLine(@"#line 10 ""<wrapper>""");
			b.AppendLine("}");
			if(members.Count != 0) {
				if(compiling) b.AppendLine("#line default\r\n");
				prevLine = -2;
				foreach(var v in members) _Append(v);
				if(compiling) b.AppendLine("\r\n#line 11 \"<wrapper>\"");
			}
			b.AppendLine("}");
			if(!compiling) b.Append(root.EndOfFileToken.ToFullString());

			code = b.ToString();
			//Print(code);
			//Perf.Next('5');
			return code;

			void _Append(SyntaxNode sn)
			{
				var span = compiling ? sn.Span : sn.FullSpan;
				if(compiling) {
					//preserve #pragma warning
					if(sn.HasStructuredTrivia) {
						foreach(var v in sn.GetLeadingTrivia()) {
							if(!v.IsKind(SyntaxKind.PragmaWarningDirectiveTrivia)) continue;
							var sn2 = v.GetStructure() as PragmaWarningDirectiveTriviaSyntax;
							if(sn2.IsActive) _Append(sn2);
						}
					}

					var lineSpan = tree.GetLineSpan(span);
					var start = lineSpan.StartLinePosition;
					int line = start.Line, col = start.Character;
					if(line != prevLine + 1) {
						if(line == prevLine) {
							b.Remove(b.Length - 2, 2);
							for(int i = b.Length - 1; i >= 0 && b[i] != '\n'; i--) col--;
						} else if(prevLine >= 0 && (uint)(line - prevLine) < 10) {
							for(int i = line - prevLine - 1; i > 0; i--) b.AppendLine();
						} else b.Append("#line ").Append(line + 1).AppendLine();
					}
					//prevLine = line; //does not support multiline statements
					prevLine = lineSpan.EndLinePosition.Line;
					if(col > 0) b.Append(' ', col);
				}
				b.Append(code, span.Start, span.Length);
				if(compiling) b.AppendLine();
			}
		}

		/// <summary>
		/// Converts script code to app code.
		/// If there are syntax errors, shows them (like when compiling) and returns null.
		/// The caller should save text before, because gets text from file, not from editor.
		/// </summary>
		/// <param name="f">Script.</param>
		public static string ConvertCodeScriptToApp(IWorkspaceFile f)
		{
			Debug.Assert(f.IcfIsScript);
			var m = new MetaComments();
			if(!m.Parse(f, null, EMPFlags.PrintErrors)) return null;
			var err = m.Errors;
			var code = m.Files[0].code;
			var po = new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, SourceCodeKind.Script, m.Defines);
			var tree = CSharpSyntaxTree.ParseText(code, po, f.FilePath, Encoding.UTF8) as CSharpSyntaxTree;
			if(err.AddAllAndPrint(tree, f)) return null;

			var s = _TransformScriptCode(tree, code, false, err, f);
			if(err.ErrorCount != 0) { err.PrintAll(); return null; }

			if(m.EndOfMeta > 0 && !s.StartsWith_("/*")) { //inserted default usings before meta
				int metaStart = s.IndexOf_("/*"), metaLength = m.EndOfMeta;
				while(metaLength < code.Length && code[metaLength] <= ' ') metaLength++;
				int metaEnd = metaStart + metaLength;
				var b = new StringBuilder();
				b.Append(s, metaStart, metaLength).Append(s, 0, metaStart).Append(s, metaEnd, s.Length - metaEnd);
				s = b.ToString();
			}
			return s;
		}

		static bool _RunPrePostBuildScript(bool post, MetaComments m, string outFile)
		{
			var x = post ? m.PostBuild : m.PreBuild;
			string[] args;
			if(x.s == null) {
				args = new string[] { outFile };
			} else {
				args = Au.Util.StringMisc.CommandLineToArray(x.s);

				//replace variables like $(variable)
				var f = m.Files[0].f;
				if(s_rx1 == null) s_rx1 = new Regex_(@"\$\((\w+)\)");
				string _ReplFunc(RXMatch k)
				{
					switch(k[1].Value) {
					case "outputFile": return outFile;
					case "sourceFile": return f.FilePath;
					case "source": return f.ItemPath;
					case "outputPath": return m.OutputPath;
					case "debug": return m.IsDebug ? "true" : "false";
					default: throw new ArgumentException("error in meta: unknown variable " + k.Value);
					}
				}
				for(int i = 0; i < args.Length; i++) args[i] = s_rx1.Replace(args[i], _ReplFunc);
			}

			bool ok = Compile(true, out var r, x.f);
			if(r.outputType == EOutputType.dll) throw new ArgumentException($"error in meta: '{x.f.Name}' is not script/app");
			if(!ok) return false;

			RunAsm.Run(r.file, args, r.pdbOffset, RAFlags.InEditorThread | RAFlags.DontHandleExceptions);
			return true;
		}
		static Regex_ s_rx1;

		/// <summary>
		/// If Thread.CurrentThread.ManagedThreadId == 1, sets timer to call GC.Collect after 10 s.
		/// </summary>
		internal static void LibSetTimerGC()
		{
			//problem 1: GC does not start automatically to release PortableExecutableReference data, many MB. Probably most of it is unmanaged memory.
			//	After first compilation Task Manager shows 40 MB. After several times can be 300 MB.
			//	Explicit GC collect makes 21 MB.
			//problem 2: Compiling is much slower if we always create all PortableExecutableReference.
			//	We can cache them, but then always 40 MB.
			//Solution: cache them in a WeakReference, and set timer to do GC collect after eg 10 s.
			//	Then, if compiling frequently, compiling is fast, also don't need frequent explicit GC collect. For 10 s we have constant 40 MB, then 21 MB.

			if(Thread.CurrentThread.ManagedThreadId != 1) return;
			if(t_timerGC == null) t_timerGC = new Timer_(() => {
				GC.Collect();
				//GC.WaitForPendingFinalizers(); SetProcessWorkingSetSize(Process_.CurrentProcessHandle, -1, -1);
			});
			t_timerGC.Start(10_000, true);
		}
		static Timer_ t_timerGC;
	}
}
