//#define PERF_LOAD

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
using System.Resources;

public static class Compiler
{
	internal static void CompileAndRun(FileNode f, bool run)
	//public static async void Test(object sender, EventArgs args)
	{
#if DEBUG
		if(s_thread == 0) s_thread = Thread.CurrentThread.ManagedThreadId; else Debug.Assert(Thread.CurrentThread.ManagedThreadId == s_thread);
#endif
		if(f == null) return;

		//ProfileOptimization.StartProfile("Compiler.speed"); //does not make better

		Model.Save.TextNowIfNeed();

		_Results r = default;
		try {
			if(!_Compile(out r, f)) return;
		}
		catch(Exception e) {
			Print("Failed", e.GetType().Name, e.Message);
			return;
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
		if(!run) return;
		if(r.kind == OutputKind.DynamicallyLinkedLibrary) {
			Print("Cannot run dll.");
			//TODO: postScript can run an app that uses the dll. If returns false, don't try to run now.
			//TODO: meta runDll //run app or script that uses the dll
			return;
		}
		if(r.kind == OutputKind.ConsoleApplication && r.isolation != Meta.EIsolation.process) {
			Print("<>Cannot run console in this process. In meta add line <c green>isolation process<>.");
			return; //works, but closing console window kills process
		}

#if PERF_LOAD
			//200.ms();
			Perf.WakeCPU();
			Perf.First();
#endif

		var thread = new Thread(() => Au.Util.AppDomain_.TestRunDomain(r.name, r.data, r.config));
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();

#if PERF_LOAD
		Au.Util.AppDomain_.s_event.WaitOne(2000);
		Perf.NW();
#endif
	}
#if DEBUG
	static int s_thread;
#endif
	static Timer_ s_timerGC = new Timer_(() => GC.Collect());

	/// <summary>_Compile() output assembly</summary>
	struct _Results
	{
		/// <summary>script file name without ext</summary>
		public string name;
		/// <summary>string filePath or byte[] fileData</summary>
		public object data;
		/// <summary>config file. Full path.</summary>
		public string config;
		public OutputKind kind;
		public Meta.EIsolation isolation;
		public Meta.EUac uac;
		public Meta.ERunAlone runAlone;
	}

	static bool _Compile(out _Results r, FileNode f)
	{
		bool inMemoryAsm = Keyb.IsScrollLock; //TODO

		r = default;
		Perf.First();

		var m = new Meta();
		if(!m.Parse(f)) {
			if(!m.Errors.IsEmpty) Print(m.Errors);
			return false;
		}
		var err = m.Errors;
		Perf.Next();

		var po = new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, m.IsScript ? SourceCodeKind.Script : SourceCodeKind.Regular, m.Defines);
		var tree = CSharpSyntaxTree.ParseText(m.Code, po, null, Encoding.UTF8);

		var options = new CSharpCompilationOptions(
			m.OutputType,
			usings: m.IsScript ? s_usingsForScript : null,
			optimizationLevel: m.IsDebug ? OptimizationLevel.Debug : OptimizationLevel.Release, //speed: compile the same, load Release slightly slower. Default Debug.
			allowUnsafe: true,
			platform: m.Prefer32Bit ? Platform.AnyCpu32BitPreferred : Platform.AnyCpu,
			warningLevel: m.WarningLevel,
			specificDiagnosticOptions: m.DisableWarnings?.Select(wa => new KeyValuePair<string, ReportDiagnostic>(wa, ReportDiagnostic.Suppress))
			);

		var compilation = CSharpCompilation.Create(m.Name, new[] { tree }, m.References.Refs, options);
		Perf.Next();

		string outPath, outFile = null; var memStream = new MemoryStream(4096);
		if(!inMemoryAsm) {
			if(m.OutputPath != null) {
				outPath = m.OutputPath; //info: the directory is already created
				outFile = Path_.Combine(outPath, m.Name) + (m.OutputType == OutputKind.DynamicallyLinkedLibrary ? ".dll" : ".exe");
			} else {
				outPath = Model.CacheDirectory;
				Files.CreateDirectory(outPath);
				outFile = outPath + @"\" + f.HexGUID; //convert Base64 to Hex, because Base64 is case-sensitive but filename isn't
			}
		}

		var resMan = _CreateManagedResources(m);
		if(!err.IsEmpty) { Print(err.ToString()); return false; }

		var resNat = _CreateNativeResources(m, compilation);
		if(!err.IsEmpty) { Print(err.ToString()); return false; }

		var emitResult = compilation.Emit(memStream, win32Resources: resNat, manifestResources: resMan);
		Perf.Next();

		var diag = emitResult.Diagnostics;
		if(!diag.IsEmpty) {
			foreach(var d in diag) {
				//if(!showWarnings && d.Severity != DiagnosticSeverity.Error) continue;
				if(d.Severity == DiagnosticSeverity.Hidden) continue;
				err.Add(d);
				if(d.Severity == DiagnosticSeverity.Error && d.Id == "CS0009") m.References.RemoveBadReference(d.GetMessage());
			}
			if(!err.IsEmpty) Print(err.ToString());
		}
		if(!emitResult.Success) {
			if(!inMemoryAsm) Files.Delete(outFile);
			return false;
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
			if(m.OutputPath != null) {
				Perf.Next();
				_CopyReferenceFiles(m);
			}
		}

		r.name = m.Name;
		r.config = m.ConfigFile;
		r.kind = m.OutputType;
		r.isolation = m.Isolation;
		r.uac = m.Uac;
		r.runAlone = m.RunAlone;

		Perf.NW();
		return true;
	}

	static ResourceDescription[] _CreateManagedResources(Meta m)
	{
		var a = m.ResourceFiles;
		if(a == null || a.Count == 0) return null;
		var stream = new MemoryStream();
		var rw = new ResourceWriter(stream);
		string curFile = null;
		try {
			foreach(var v in a) {
				curFile = v;
				var rn = Path_.GetFileNameWithoutExtension(v);
				object o;
				switch(Path_.GetExtension(v).ToLower_()) {
				case ".png":
				case ".bmp":
				case ".jpg":
				case ".gif":
					o = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(v);
					break;
				case ".ico":
					//somehow writes 4-bit icon, distorted
					//o = Icons.GetFileIcon(v, 16);
					////rw.AddResourceData(rn, "System.Drawing.Icon", File.ReadAllBytes(v)); //does not work
					throw new ArgumentException("Managed icon resources not supported");
				//case ".cur":
				//	o = Au.Util.Cursors_.LoadCursorFromFile(v); //error: Stock cursors cannot be serialized
				//	break;
				default:
					o = File.ReadAllBytes(v);
					break;
				}
				rw.AddResource(rn, o);
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
		//Print(stream.CanRead, stream.CanWrite, stream.Length, stream.Position);
		stream.Position = 0;
		return new ResourceDescription[] { new ResourceDescription("Project.Resources.resources", () => stream, false) };
	}

	static Stream _CreateNativeResources(Meta m, CSharpCompilation compilation)
	{
		if(m.NativeIconFile == null && m.ManifestFile == null && m.ResFile == null) return null;
		FileStream manStream = null, icoStream = null; string curFile = null;
		try {
			if(m.ResFile != null) return File.OpenRead(m.ResFile);
			if(m.ManifestFile != null) manStream = File.OpenRead(curFile = m.ManifestFile);
			if(m.NativeIconFile != null) icoStream = File.OpenRead(curFile = m.NativeIconFile);
			curFile = null;
			return compilation.CreateDefaultWin32Resources(false, m.ManifestFile == null, manStream, icoStream);
		}
		catch(Exception e) {
			manStream?.Dispose();
			icoStream?.Dispose();
			_ResourceException(e, m, curFile);
			return null;
		}
	}

	static void _ResourceException(Exception e, Meta m, string curFile)
	{
		var em = e.GetType().Name + ", " + e.Message;
		var err = m.Errors;
		if(curFile == null) err.Add("Failed to add resources. " + em);
		else err.Add($"Failed to add resource '{curFile}'. " + em);
	}

	static void _CopyReferenceFiles(Meta m)
	{
		//info: tried to get all used references, unsuccessfully.
		//	Would need to create appdomain, load the assembly and get its references through reflection.
		//	And don't need it. We'll copy Au.dll and all non-default references that are not in the .NET folder.

		_CopyFileIfNeed(typeof(Wnd).Assembly.Location, m.OutputPath + @"\Au.dll");

		var refs = m.References.Refs;
		int i = DefaultReferences.Count;
		if(refs.Count > i) {
			//var netDir = Folders.NetFrameworkRuntime; //no GAC
			var netDir = Folders.Windows + @"Microsoft.NET\";

			for(; i < refs.Count; i++) {
				var ri = refs[i];
				var s1 = ri.FilePath;
				if(s1.StartsWith_(netDir, true)) continue;
				var s2 = m.OutputPath + "\\" + Path_.GetFileName(s1);
				//PrintList(s1, s2);
				_CopyFileIfNeed(s1, s2);
			}
		}

		void _CopyFileIfNeed(string sFrom, string sTo)
		{
			if(Files.GetProperties(sTo, out var p2, FAFlags.UseRawPath) //if exists
				&& Files.GetProperties(sFrom, out var p1, FAFlags.UseRawPath)
				&& p2.LastWriteTimeUtc == p1.LastWriteTimeUtc
				&& p2.Size == p1.Size) return;
			//Print("copy");
			Files.Copy(sFrom, sTo, IfExists.Delete);
		}
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

	//note: DefaultReferences and s_usings must be in sync. If there is no reference for an using, will be compiler error.

	//Implicit usings for scripts. Used when compiling.
	static readonly string[] s_usingsForScript = {
		"System",
		"System.Collections.Generic",
		"System.Collections.Concurrent",
		"System.Text",
		"System.Diagnostics",
		"System.Runtime.InteropServices",
		"System.Runtime.CompilerServices",
		"System.IO",
		"System.Threading",
		"System.Threading.Tasks",
		"System.ComponentModel",
		"Microsoft.Win32",
		"System.Runtime.ExceptionServices",
		"System.Windows.Forms",
		"System.Drawing",
		"System.Text.RegularExpressions",
		"System.Linq",
		//"System.Xml.Linq",
		//"System.Net.Http",
		//"System.Reflection",
		"Au",
		"Au.Types",
		"Au.NoClass",

		//speed: many usings makes compiling slower, but not as much as many references.
	};

	//note: s_usings and DefaultUsingsForTemplate should be similar.

	//Used by FilesModel when creating new .cs (non-script) files.
	public const string DefaultUsingsForTemplate = @"using System;
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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
//using System.Linq;
//using System.Xml.Linq;
//using System.Net.Http;
//using System.Reflection;

using Au;
using Au.Types;
using static Au.NoClass;

";

	#endregion
}
