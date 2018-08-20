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
//using System.Windows.Forms;
//using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Resources;

//TODO: delete all Print in this and related files.

static partial class Compiler
{
	internal static void CompileAndRun(FileNode f, bool run, string[] args = null)
	//public static async void Test(object sender, EventArgs args)
	{
#if DEBUG
		if(s_thread == 0) s_thread = Thread.CurrentThread.ManagedThreadId; else Debug.Assert(Thread.CurrentThread.ManagedThreadId == s_thread);
#endif
		Model.Save.TextNowIfNeed();

		if(f == null) return;

		var nodeType = f.NodeType;
		if(!(nodeType == ENodeType.Script || nodeType == ENodeType.CS)) return;

		//Output.Clear(); //TODO
		//Perf.Cpu();
		//200.ms();
		Perf.First();

		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		bool inMemoryAsm = Keyb.IsScrollLock; //TODO

		CompResults r = default;
		//Perf.First();
		bool isCompiled = run && !inMemoryAsm && Model.Compiled.IsCompiled(f, out r, projFolder);
		//Perf.NW(); Print(isCompiled);
		isCompiled = false;//TODO
		if(!isCompiled) {
			bool ok = false;
			try {
				ok = _Compile(f, out r, projFolder, inMemoryAsm);
			}
			catch(Exception e) {
				Print("Compilation failed", e.GetType().Name, e.Message);
			}
			finally {
				//GC.Collect();
				s_timerGC.Start(30_000, true);

				//problem 1: GC does not work automatically with PortableExecutableReference data.
				//	After first compilation Task Manager shows 40 MB. After several times, if we don't cache, can be 300 MB.
				//	Explicit GC collect makes 21 MB.
				//problem 2: Compiling is much slower if we always create all PortableExecutableReference.
				//	We can cache them, but then always 40 MB.
				//Solution: cache them in a WeakReference, and set timer to do GC collect after eg 30 s.
				//	Then, if compiling frequently, compiling is fast, also don't need frequent explicit GC collect. For 30 s we have constant 40 MB, then 21 MB.
			}

			if(!ok) {
				Model.Compiled.Remove(f);
				return;
			}

			if(!run) return;
		}

		if(r.run != null) {
			if(r.data is string asmFile) {
				if(args == null) args = new string[] { asmFile };
				else {
					var t = new string[args.Length + 1];
					t[0] = asmFile;
					Array.Copy(args, 0, t, 1, args.Length);
					args = t;
				}
			}
			CompileAndRun(r.run, true, args);
			return;
		}

		if(r.outputType == EOutputType.dll) {
			Print("Cannot run when meta option outputType is dll (default for .cs files). If this is an app with function Main, add meta option outputType app. Else, if want to run another script/app instead, add meta option run ScriptName; or put this file in an app project folder.");
			//TODO: postScript can run an app that uses the dll. If returns false, don't try to run now.
			//TODO: meta runDll //run app or script that uses the dll
			return;
		}
		if(r.maxInstances == 0) return;

		Perf.Next();
		bool isScript = nodeType == ENodeType.Script;
		if(r.isolation == EIsolation.hostThread) {
			Au.Util.LibRunAsm.RunHere(r.data, isScript, args);
		} else if(r.isolation == EIsolation.process) {
			//TODO: use Au.Tasks if no meta outputPath

			args = new string[] { Perf.StaticInst.Serialize() }; //TODO

			var si = new ProcessStartInfo(r.data as string) { UseShellExecute = false };
			if(args != null && args.Length == 1) si.Arguments = args[0]; //TODO
			Process.Start(si);
		} else {
			var thread = new Thread(() =>
			{
				if(r.isolation == EIsolation.appDomain) {
					Au.Util.LibRunAsm.RunInNewAppDomain(r.name, r.data, isScript, args, r.config);
				} else {
					Au.Util.LibRunAsm.RunHere(r.data, isScript, args);
				}
			});
			if(!r.mtaThread) thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
	}
#if DEBUG
	static int s_thread;
#endif
	static Timer_ s_timerGC = new Timer_(() => GC.Collect());

	/// <summary>_Compile() output assembly info.</summary>
	internal struct CompResults
	{
		/// <summary>C# file name without ext.</summary>
		public string name;
		/// <summary>string assemblyPath or byte[] assemblyData.</summary>
		public object data;
		/// <summary>config file. Full path.</summary>
		public string config;
		public EOutputType outputType;
		public EIsolation isolation;
		public EUac uac;
		public ERunAlone runAlone;
		public int maxInstances;
		/// <summary>Run this script instead. For example, allows to run dll.</summary>
		public FileNode run;
		public bool mtaThread;
	}

	static bool _Compile(FileNode f, out CompResults r, FileNode projFolder, bool inMemoryAsm)
	{
		r = default;
		Perf.First();

		var m = new MetaComments();
		if(!m.Parse(f, projFolder)) {
			m.Errors.PrintAll();
			return false;
		}
		if(m.Files == null) return false; //empty text
		var err = m.Errors;
		bool isScript = m.IsScript;
		Perf.Next();

		var po = new CSharpParseOptions(LanguageVersion.Latest,
			m.XmlDocFile != null ? DocumentationMode.Parse : DocumentationMode.None,
			isScript ? SourceCodeKind.Script : SourceCodeKind.Regular,
			m.Defines);

#if STANDARD_SCRIPT
		var trees = new List<CSharpSyntaxTree>(m.Files.Count + 1);
		foreach(var f1 in m.Files) {
			var tree = CSharpSyntaxTree.ParseText(f1.code, po, f1.f.Guid + " " + f1.f.Name, Encoding.UTF8) as CSharpSyntaxTree;
			trees.Add(tree);
		}
		if(isScript) {
			if(s_treeScriptAdd == null) s_treeScriptAdd = CSharpSyntaxTree.ParseText(
@"using System.Runtime.InteropServices;
[module: DefaultCharSet(CharSet.Unicode)]
static class __script__ {
internal static string[] args = System.Array.Empty<string>();
}", new CSharpParseOptions(LanguageVersion.Latest)) as CSharpSyntaxTree;
			trees.Add(s_treeScriptAdd);
		}
#else
		var trees = new List<CSharpSyntaxTree>(m.Files.Count);
		bool transform = isScript;
		foreach(var f1 in m.Files) {
			var tree = CSharpSyntaxTree.ParseText(f1.code, po, f1.f.Guid + " " + f1.f.Name, Encoding.UTF8) as CSharpSyntaxTree;
			if(transform) {
				transform = false;
				if(_TransformScript(ref tree, f1.code)) isScript = false;
				if(m.Files.Count > 1) po = po.WithKind(SourceCodeKind.Regular);
			}
			trees.Add(tree);
		}
#endif
		Perf.Next('t');

		//TODO: autodetect: if script or contains Main, it is app, else dll.
		OutputKind ot;
		switch(m.OutputType) {
		case EOutputType.dll: ot = OutputKind.DynamicallyLinkedLibrary; break;
		case EOutputType.console: ot = OutputKind.ConsoleApplication; break;
		default: ot = OutputKind.WindowsApplication; break;
		}

		var options = new CSharpCompilationOptions(
			ot,
			usings: isScript ? s_usingsForScript : null,
			optimizationLevel: m.IsDebug ? OptimizationLevel.Debug : OptimizationLevel.Release, //speed: compile the same, load Release slightly slower. Default Debug.
			allowUnsafe: true,
			platform: m.Prefer32Bit ? Platform.AnyCpu32BitPreferred : Platform.AnyCpu,
			warningLevel: m.WarningLevel,
			specificDiagnosticOptions: m.DisableWarnings?.Select(wa => new KeyValuePair<string, ReportDiagnostic>(wa, ReportDiagnostic.Suppress))
			);

		var compilation = CSharpCompilation.Create(m.Name, trees, m.References.Refs, options);
		Perf.Next();

		string outPath = null, outFile = null, pdbFile = null, xdFile = null, configFile = null;
		var memStream = new MemoryStream(4096);
		Stream pdbStream = null, xdStream = null;
		Microsoft.CodeAnalysis.Emit.EmitOptions eOpt = null;
		if(!inMemoryAsm) {
			string fileName, fileExt = null;
			if(m.OutputPath != null) {
				outPath = m.OutputPath; //info: the directory is already created
				fileName = m.Name;
				fileExt = m.OutputType == EOutputType.dll ? ".dll" : ".exe";
			} else {
				outPath = Model.Compiled.CacheDirectory;
				File_.CreateDirectory(outPath);
				fileName = f.GuidHex; //convert Base64 to Hex, because Base64 is case-sensitive but filename isn't
			}
			outFile = Path_.Combine(outPath, fileName);

			//create debug info always. It is used for run-time error links.
			if(m.OutputPath != null) {
				pdbStream = File.Create(pdbFile = outFile + ".pdb");
				eOpt = new Microsoft.CodeAnalysis.Emit.EmitOptions(pdbFilePath: pdbFile);
			} else {
				eOpt = new Microsoft.CodeAnalysis.Emit.EmitOptions(debugInformationFormat: Microsoft.CodeAnalysis.Emit.DebugInformationFormat.Embedded);
				//TODO: does it work? Adds < 1 KB; almost the same compiling speed. Separate pdb file is 14 KB; 2 times slower.
			}

			if(m.XmlDocFile != null) xdStream = File.Create(xdFile = Path_.Normalize(m.XmlDocFile, outPath));

			if(fileExt != null) outFile += fileExt;
		}

		var resMan = _CreateManagedResources(m);
		if(!err.IsEmpty) { err.PrintAll(); return false; }

		var resNat = _CreateNativeResources(m, compilation);
		if(!err.IsEmpty) { err.PrintAll(); return false; }

		var emitResult = compilation.Emit(memStream, pdbStream, xdStream, resNat, resMan, eOpt);
		pdbStream?.Dispose();
		xdStream?.Dispose();
		Perf.Next();
		//TODO: are the resX streams disposed now?

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
			if(!inMemoryAsm) {
				File_.Delete(outFile);
				if(pdbFile != null) File_.Delete(pdbFile);
				if(xdFile != null) File_.Delete(xdFile);
			}
			return false;
		}

		if(!m.IsScript && ot != OutputKind.DynamicallyLinkedLibrary) {
			//If there is no [STAThread], will need to run in MTA thread.
			bool hasSTAThread = compilation.GetEntryPoint(default)?.GetAttributes().Any(o => o.ToString() == "System.STAThreadAttribute") ?? false;
			if(!hasSTAThread) r.mtaThread = true;
		}

		memStream.Position = 0;
		if(inMemoryAsm) {
			var b = new byte[memStream.Length];
			memStream.Read(b, 0, b.Length);
			r.data = b;
		} else {
			using(var fileStream = File.Create(outFile, (int)memStream.Length)) {
				memStream.CopyTo(fileStream);
			}
			r.data = outFile;

			//copy non-.NET references to the output directory
			if(m.OutputPath != null && m.OutputType != EOutputType.dll) {
				Perf.Next();
				_CopyReferenceFiles(m);
			}

			if(m.ConfigFile != null && m.Isolation == EIsolation.process) { //TODO: finish
				configFile = m.ConfigFile.FilePath;
				_CopyFileIfNeed(configFile, outPath + "\\" + Path_.GetFileName(configFile));
			}

			//Perf.First();
			Model.Compiled.AddCompiled(f, outFile, m, r.mtaThread);
			//Perf.NW();
		}

		r.name = m.Name;
		r.config = configFile;
		r.outputType = m.OutputType;
		r.isolation = m.Isolation;
		r.uac = m.Uac;
		r.runAlone = m.RunAlone;
		r.maxInstances = m.MaxInstances;
		r.run = m.Run;

		Perf.NW();
		return true;
	}

#if STANDARD_SCRIPT
	static CSharpSyntaxTree s_treeScriptAdd; //TODO: remove
#endif

	static ResourceDescription[] _CreateManagedResources(MetaComments m)
	{
		var a = m.ResourceFiles;
		if(a == null || a.Count == 0) return null;
		var stream = new MemoryStream();
		var rw = new ResourceWriter(stream);
		FileNode curFile = null;
		try {
			foreach(var v in a) {
				curFile = v;
				string name = v.Name, path = v.FilePath;
				object o;
				switch(Path_.GetExtension(name).ToLower_()) {
				case ".png":
				case ".bmp":
				case ".jpg":
				case ".gif":
					o = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path);
					break;
				case ".ico":
					//somehow writes 4-bit icon, distorted
					//o = Icons.GetFileIcon(path, 16);
					////rw.AddResourceData(rn, "System.Drawing.Icon", File.ReadAllBytes(v)); //does not work
					throw new ArgumentException("Managed icon resources not supported");
				//case ".cur":
				//	o = Au.Util.Cursors_.LoadCursorFromFile(path); //error: Stock cursors cannot be serialized
				//	break;
				default:
					o = File.ReadAllBytes(path);
					break;
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
		//TODO: if(manifest == null && m.OutputPath != null && m.OutputType != EOutputType.dll && m.ResFile == null) manifest = Folders.ThisApp + "default.app.manifest";

		if(m.IconFile == null && manifest == null && m.ResFile == null) return null;
		Stream manStream = null, icoStream = null;
		FileNode curFile = null;
		try {
			if(m.ResFile != null) return File.OpenRead((curFile = m.ResFile).FilePath);
			if(manifest != null) manStream = File.OpenRead((curFile = manifest).FilePath);
			if(m.IconFile != null) icoStream = File.OpenRead((curFile = m.IconFile).FilePath);
			curFile = null;
			return compilation.CreateDefaultWin32Resources(true, manifest == null, manStream, icoStream);
		}
		catch(Exception e) {
			manStream?.Dispose();
			icoStream?.Dispose();
			_ResourceException(e, m, curFile);
			return null;
		}
	}

	static void _ResourceException(Exception e, MetaComments m, FileNode curFile)
	{
		var em = e.GetType().Name + ", " + e.Message;
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
				if(s1.StartsWith_(netDir, true)) continue;
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

	//Implicitly added references. Used by MetaReferences.
	public static readonly Dictionary<string, string> DefaultReferences = new Dictionary<string, string> {
		{"mscorlib", typeof(object).Assembly.Location},
		{"System", typeof(Component).Assembly.Location},
		{"System.Core", typeof(HashSet<>).Assembly.Location},
		{"System.Windows.Forms", typeof(System.Windows.Forms.Form).Assembly.Location},
		{"System.Drawing", typeof(System.Drawing.Point).Assembly.Location},
		//{"System.Xml", typeof(System.Xml.XmlNode).Assembly.Location},
		//{"System.Xml.Linq", typeof(System.Xml.Linq.XElement).Assembly.Location},
		//{"System.Net.Http", typeof(System.Net.Http.HttpClient).Assembly.Location},
		{"Au.dll", typeof(Wnd).Assembly.Location},

		//info: don't need to add System.ValueTuple.
		//speed: many references makes compiling much slower. We use temporary caching. Permanent caching would add many MB of process memory.
	};

	//note: DefaultReferences and s_usingsForScript must be in sync. If there is no reference for an using, will be compiler error.
	//	Also in usings.txt, which is used in new .cs file templates.

	//Implicit usings for scripts. Used when compiling.
	static readonly string[] s_usingsForScript = { //TODO: remove
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
		//"System.Xml.Linq",
		//"System.Net.Http",
		"Au",
		"Au.Types",
		"Au.NoClass",
#if STANDARD_SCRIPT
		"__script__", //static class in file added when compiling, to make args available
#endif
		//speed: many usings makes compiling slower, but not as much as many references.
	};

	//Used by FilesModel when creating new .cs (non-script) files.
	public const string DefaultUsingsForTemplate = @"";

	#endregion

	/// <summary>
	/// Converts script code to regular C# class with [STAThread]Main(string[] args).
	/// Need it because of the limitations of standard C# script:
	///		There is no normal Main, therefore cannot have string[] args (command line), [STAThread], etc.
	///			It can be simulated, but not when runs as exe. Probably would need to not allow script exe.
	///		Stack trace (eg on exception) contains lots of meaningless lines instead of void Main().
	///		Cannot have ref locals in global code. Error "async methods cannot have by reference locals".
	///			Now ref locals can be used in { } blocks. In any case, syntax error if not in { }.
	///		And maybe more, not discovered.
	/// Also makes these changes:
	///		Variables declared without a modifier (private, static, const, etc) are locals, not fields.
	///		Makes the class partial.
	/// </summary>
	static bool _TransformScript(ref CSharpSyntaxTree tree, string code)
	{
		foreach(var v in tree.GetDiagnostics()) if(v.Severity == DiagnosticSeverity.Error) return false;

		code = _TransformScriptCode(tree, code, true); if(code == null) return false;
		//Print(code);

		Perf.Next('5');

		//root = root.InsertNodesBefore(statements[0], new SyntaxNode[] { statements[0] });
		//tree = tree.WithRootAndOptions(root, tree.Options) as CSharpSyntaxTree;

		var po = new CSharpParseOptions(LanguageVersion.Latest, tree.Options.DocumentationMode, SourceCodeKind.Regular, tree.Options.PreprocessorSymbolNames);
		tree = CSharpSyntaxTree.ParseText(code, po, tree.FilePath, Encoding.UTF8) as CSharpSyntaxTree;

		//Also tried to parse wrapper template and then ReplaceNode etc. To avoid parsing script code 2 times.
		//	Same speed but more difficult. Need #line too. Emit is 10 times slower.

		return true;
	}

	static string _TransformScriptCode(CSharpSyntaxTree tree, string code, bool compiling)
	{
		Perf.Next('1');

		var aliases = new List<SyntaxNode>();
		var usings = new List<SyntaxNode>();
		var attributes = new List<SyntaxNode>();
		var members = new List<SyntaxNode>();
		var statements = new List<SyntaxNode>();

		Perf.Next('2');
		var root = tree.GetCompilationUnitRoot();
		Perf.Next('3');
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
				//Print(v.ChildNodesAndTokens().First().Kind());
				if(v.ChildNodesAndTokens().First().IsKind(SyntaxKind.VariableDeclaration)) statements.Add(v); //int x; //let it be local variable
				else members.Add(v); //private int x; //let it be field
				break;
			default:
				members.Add(v);
				break;
			}
		}
		Perf.Next('4');

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
		b.AppendLine(@"using System; using System.Collections.Generic; using System.Text; using System.Text.RegularExpressions; using System.Diagnostics; using System.Runtime.InteropServices; using System.IO; using System.Threading; using System.Threading.Tasks; using System.Windows.Forms; using System.Drawing; using System.Linq; using Au; using Au.Types; using static Au.NoClass;");
		if(usings.Count != 0) {
			if(compiling) b.AppendLine("#line default\r\n");
			prevLine = -2;
			foreach(var v in usings) _Append(v);
			if(compiling) b.AppendLine("\r\n#line 2 \"<wrapper>\"");
		}
		if(!compiling) b.AppendLine();
		b.AppendLine(@"[module: DefaultCharSet(CharSet.Unicode)]");
		if(!compiling) b.AppendLine();
		if(attributes.Count != 0) {
			if(compiling) b.AppendLine("#line default\r\n");
			prevLine = -2;
			foreach(var v in attributes) _Append(v);
			if(compiling) b.AppendLine("\r\n#line 3 \"<wrapper>\"");
		}
		b.AppendLine(@"partial class App {
[STAThread]
static void Main(string[] args) { new App()._Main(args); }
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

		return b.ToString();

		void _Append(SyntaxNode sn)
		{
			var span = compiling ? sn.Span : sn.FullSpan;
			if(compiling) {
				var linePos = tree.GetLineSpan(span).StartLinePosition;
				int line = linePos.Line, col = linePos.Character;
				if(line != prevLine + 1) {
					if(line == prevLine) {
						b.Remove(b.Length - 2, 2);
						for(int i = b.Length - 1; i >= 0 && b[i] != '\n'; i--) col--;
					} else if(prevLine >= 0 && (uint)(line - prevLine) < 10) {
						for(int i = line - prevLine - 1; i > 0; i--) b.AppendLine();
					} else b.Append("#line ").Append(line + 1).AppendLine();
				}
				prevLine = line;
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
	internal static string ConvertCodeScriptToApp(FileNode f)
	{
		var m = new MetaComments();
		if(!m.Parse(f, null)) {
			m.Errors.PrintAll();
			return null;
		}
		if(m.Files == null) return ""; //empty text
		var err = m.Errors;

		var po = new CSharpParseOptions(LanguageVersion.Latest,
			DocumentationMode.None,
			SourceCodeKind.Script,
			m.Defines);

		var f1 = m.Files[0];
		var tree = CSharpSyntaxTree.ParseText(f1.code, po, f1.f.Guid + " " + f1.f.Name, Encoding.UTF8) as CSharpSyntaxTree;

		foreach(var v in tree.GetDiagnostics())
			if(v.Severity == DiagnosticSeverity.Error) {
				err.AddErrorOrWarning(v, f);
			}
		if(!err.IsEmpty) {
			err.PrintAll();
			return null;
		}

		var s = _TransformScriptCode(tree, f1.code, false);

		if(m.EndOfMeta > 0 && !s.StartsWith_("/*")) { //inserted default usings before meta
			int metaStart = s.IndexOf_("/*"), metaLength = m.EndOfMeta;
			while(metaLength < f1.code.Length && f1.code[metaLength] <= ' ') metaLength++;
			int metaEnd = metaStart + metaLength;
			var b = new StringBuilder();
			b.Append(s, metaStart, metaLength).Append(s, 0, metaStart).Append(s, metaEnd, s.Length - metaEnd);
			s = b.ToString();
		}
		return s;
	}
}
