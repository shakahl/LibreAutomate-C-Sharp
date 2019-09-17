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
using static Au.AStatic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Resources;
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
		/// <param name="reason">Whether to recompile if compiled, etc. See also Remarks.</param>
		/// <param name="r">Results.</param>
		/// <param name="f">C# file. If projFolder used, must be the main file of the project.</param>
		/// <param name="projFolder">null or project folder.</param>
		/// <remarks>
		/// Must be always called in the main UI thread (Thread.CurrentThread.ManagedThreadId == 1).
		/// 
		/// Adds <see cref="DefaultReferences"/>.
		/// 
		/// If f role is classFile:
		///		If CompReason.Run, does not compile (just parses meta), sets r.role=classFile and returns false.
		///		Else compiles but does not create output files.
		/// </remarks>
		public static bool Compile(ECompReason reason, out CompResults r, IWorkspaceFile f, IWorkspaceFile projFolder = null)
		{
			Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1);
			r = null;
			var cache = XCompiled.OfCollection(f.IwfWorkspace);
			bool isCompiled = reason != ECompReason.CompileAlways && cache.IsCompiled(f, out r, projFolder);

			//Print("isCompiled=" + isCompiled);

			if(!isCompiled) {
				bool ok = false;
				try {
					ok = _Compile(reason == ECompReason.Run, f, out r, projFolder);
				}
				catch(Exception ex) {
					Print($"Failed to compile '{f.Name}'. {ex.ToStringWithoutStack()}");
				}

				if(!ok) {
					f.IwfTriggers(null);
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

			public ERole role;
			public ERunMode runMode;
			public EIfRunning ifRunning;
			public EUac uac;
			public bool prefer32bit;
			public bool console;

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
			//APerf.First();
			r = new CompResults();

			var m = new MetaComments();
			if(!m.Parse(f, projFolder, EMPFlags.PrintErrors)) return false;
			var err = m.Errors;
			//APerf.Next('m');

			bool needOutputFiles = m.Role != ERole.classFile;

			//if for run, don't compile if f role is classFile
			if(forRun && !needOutputFiles) {
				r.role = ERole.classFile;
				return false;
			}

			XCompiled cache = XCompiled.OfCollection(f.IwfWorkspace);
			string outPath = null, outFile = null;
			if(needOutputFiles) {
				string fileName;
				if(m.OutputPath != null) {
					outPath = m.OutputPath; //info: the directory is already created
					fileName = m.Name + (m.Role == ERole.classLibrary ? ".dll" : ".exe");
				} else {
					outPath = cache.CacheDirectory;
					fileName = f.IdString;
				}
				outFile = outPath + "\\" + fileName;
				AFile.CreateDirectory(outPath);
			}

			if(m.PreBuild.f != null && !_RunPrePostBuildScript(false, m, outFile)) return false;

			var trees = new List<CSharpSyntaxTree>(m.CodeFiles.Count + 1);
			var po = m.CreateParseOptions();

			foreach(var f1 in m.CodeFiles) {
				string code = f1.code;
				bool addedBraces = f1.f == f && f.IsScript; if(addedBraces) code += "\r\n}}"; //assume it is script without the closing }}

				var tree = CSharpSyntaxTree.ParseText(code, po, f1.f.FilePath, Encoding.UTF8) as CSharpSyntaxTree;
				//info: file path is used later in several places: in compilation error messages, run time stack traces (from PDB), Visual Studio debugger, etc.
				//	Our AOutputServer.SetNotifications callback will convert file/line info to links. It supports compilation errors and run time stack traces.

				if(addedBraces) { //if the script has the closing } or }}, remove the added } or }}
					int nErr = 0;
					foreach(var v in tree.GetDiagnostics()) {
						if(v.Severity == DiagnosticSeverity.Error && v.Id == "CS1022") nErr++; //"Type or namespace definition, or end-of-file expected"
					}
					if(nErr > 0) {
						code = f1.code;
						if(nErr == 1) code += "\r\n}";
						tree = tree.WithChangedText(Microsoft.CodeAnalysis.Text.SourceText.From(code, Encoding.UTF8)) as CSharpSyntaxTree;
						//fast, <50% of the first parsing time, <0.5% of total compilation time
					}
				}

				trees.Add(tree);
			}
			//APerf.Next('t');

			var compilation = CSharpCompilation.Create(m.Name, trees, m.References.Refs, m.CreateCompilationOptions());
			//APerf.Next('c');

			string pdbFile = null, xdFile = null;
			MemoryStream pdbStream = null;
			Stream xdStream = null, resNat = null;
			ResourceDescription[] resMan = null;
			EmitOptions eOpt = null;

			if(needOutputFiles) {
				_AddAttributes(ref compilation, needVersionEtc: m.Role == ERole.miniProgram || m.Role == ERole.exeProgram);

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

				if(m.XmlDocFile != null) xdStream = AFile.WaitIfLocked(() => File.Create(xdFile = APath.Normalize(m.XmlDocFile, outPath)));

				resMan = _CreateManagedResources(m);
				if(err.ErrorCount != 0) { err.PrintAll(); return false; }

				resNat = _CreateNativeResources(m, compilation);
				if(err.ErrorCount != 0) { err.PrintAll(); return false; }

				//EmbeddedText.FromX //it seems we can embed source code in PDB. Not tested.
			}

			//APerf.Next();
			var asmStream = new MemoryStream(4096);
			var emitResult = compilation.Emit(asmStream, pdbStream, xdStream, resNat, resMan, eOpt);

			if(needOutputFiles) {
				xdStream?.Dispose();
				resNat?.Dispose(); //info: compiler disposes resMan
			}
			//APerf.Next('e');

			var diag = emitResult.Diagnostics;
			if(!diag.IsEmpty) {
				foreach(var d in diag) {
					if(d.Severity == DiagnosticSeverity.Hidden) continue;
					err.AddErrorOrWarning(d, f);
					if(d.Severity == DiagnosticSeverity.Error && d.Id == "CS0009") MetaReferences.RemoveBadReference(d.GetMessage());
				}
				if(!err.IsEmpty) err.PrintAll();
			}
			if(!emitResult.Success) {
				if(needOutputFiles) {
					AFile.Delete(outFile);
					if(pdbFile != null) AFile.Delete(pdbFile);
					if(xdFile != null) AFile.Delete(xdFile);
				}
				return false;
			}

			if(needOutputFiles) {
				//If there is no [STAThread], will need MTA thread.
				if(m.Role == ERole.miniProgram || m.Role == ERole.exeProgram) {
					bool hasSTAThread = compilation.GetEntryPoint(default)?.GetAttributes().Any(o => o.ToString() == "System.STAThreadAttribute") ?? false;
					if(!hasSTAThread) r.mtaThread = true;
				}

				//create assembly file
				asmStream.Position = 0;
				using(var fileStream = AFile.WaitIfLocked(() => File.Create(outFile, (int)asmStream.Length))) {
					asmStream.CopyTo(fileStream);

					pdbStream.Position = 0;
					if(m.OutputPath == null) {
						pdbStream.CopyTo(fileStream);
						r.pdbOffset = (int)asmStream.Length;
					} else {
						using(var v = AFile.WaitIfLocked(() => File.Create(pdbFile))) pdbStream.CopyTo(v);
					}
				}
				r.file = outFile;

				//copy non-.NET references to the output directory
				if(m.Role == ERole.exeProgram) {
					//APerf.Next();
					_CopyReferenceFiles(m);
				}

				//copy config file to the output directory
				var configFile = outFile + ".config";
				if(m.ConfigFile != null) {
					r.hasConfig = true;
					_CopyFileIfNeed(m.ConfigFile.FilePath, configFile);
				} else if(AFile.ExistsAsFile(configFile, true)) {
					AFile.Delete(configFile);
				}
			}

			if(m.PostBuild.f != null && !_RunPrePostBuildScript(true, m, outFile)) return false;

			if(needOutputFiles) {
				if(m.Role != ERole.classLibrary) _Triggers(f, compilation);
				cache.AddCompiled(f, outFile, m, r.pdbOffset, r.mtaThread);
			}

			r.name = m.Name;
			r.role = m.Role;
			r.ifRunning = m.IfRunning;
			r.runMode = m.RunMode;
			r.uac = m.Uac;
			r.prefer32bit = m.Prefer32Bit;
			r.console = m.Console;
			r.notInCache = m.OutputPath != null;

			//APerf.NW('C');
			return true;
		}

		public static void Warmup(Document document)
		{
			var compilation = document.Project.GetCompilationAsync().Result;
			//var pdbStream = new MemoryStream();
			//var eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);
			var asmStream = new MemoryStream(4096);
			compilation.Emit(asmStream);
			//compilation.Emit(asmStream, pdbStream, options: eOpt); //somehow makes slower later
			//compilation.Emit(asmStream, pdbStream, xdStream, resNat, resMan, eOpt);
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

					var tree = CSharpSyntaxTree.ParseText(sb.ToString(), new CSharpParseOptions(LanguageVersion.Preview)) as CSharpSyntaxTree;
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
			fMain.IwfTriggers(a);

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

					(a ??= new List<CompTriggerData>()).Add(new CompTriggerData(method, c.Name, k));
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
						switch(APath.GetExtension(name).Lower()) {
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
							//o = AIcon.GetFileIcon(path, size); //adds 4-bit icon, distorted
							o = new System.Drawing.Icon(path); //adds of all sizes. At run time: var icon=new Icon(GetResourceIcon(), 16, 16).
							break;
						//case ".cur":
						//	o = Au.Util.ACursor.LoadCursorFromFile(path); //error: Stock cursors cannot be serialized
						//	break;
						default:
							o = AFile.LoadBytes(path);
							break;
						}
						break;
					case "string":
						o = AFile.LoadText(path);
						break;
					case "strings":
						var csv = ACsv.Load(path);
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

			string manifestPath = null;
			if(manifest != null) manifestPath = manifest.FilePath;
			else if(m.Role == ERole.exeProgram && m.ResFile == null) { //add default manifest if need
				manifestPath = AFolders.ThisAppBS + "default.exe.manifest"; //don't: uac
				if(!AFile.ExistsAsFile(manifestPath)) manifestPath = null;
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
			var em = e.ToStringWithoutStack();
			var err = m.Errors;
			var f = m.CodeFiles[0].f;
			if(curFile == null) err.AddError(f, "Failed to add resources. " + em);
			else err.AddError(f, $"Failed to add resource '{curFile.Name}'. " + em);
		}

		static void _CopyReferenceFiles(MetaComments m)
		{
			//info: tried to get all used references, unsuccessfully.
			//	Would need to create appdomain, load the assembly and get its references through reflection.
			//	And don't need it. We'll copy Au.dll and all non-default references that are not in the .NET folder.

			_CopyFileIfNeed(typeof(AWnd).Assembly.Location, m.OutputPath + @"\Au.dll");

			var refs = m.References.Refs;
			int i = DefaultReferences.Count;
			if(refs.Count > i) {
				//string netDir = AFolders.NetFrameworkRuntime; //no GAC
				string netDir = AFolders.Windows + @"Microsoft.NET\";
				for(; i < refs.Count; i++) {
					var s1 = refs[i].FilePath;
					if(s1.Starts(netDir, true)) continue;
					var s2 = m.OutputPath + "\\" + APath.GetFileName(s1);
					//Print(s1, s2);
					_CopyFileIfNeed(s1, s2);
				}
			}

			//also copy C++ dlls
			_CopyFileIfNeed(AFolders.ThisAppBS + @"dll\64bit\AuCpp.dll", m.OutputPath + @"\dll\64bit\AuCpp.dll");
			_CopyFileIfNeed(AFolders.ThisAppBS + @"dll\32bit\AuCpp.dll", m.OutputPath + @"\dll\32bit\AuCpp.dll");
			//SHOULDDO: copy sqlite3.dll, if used class ASqlite. Or add a 'copyFile' meta.
		}

		static void _CopyFileIfNeed(string sFrom, string sTo)
		{
			if(AFile.GetProperties(sTo, out var p2, FAFlags.UseRawPath) //if exists
				&& AFile.GetProperties(sFrom, out var p1, FAFlags.UseRawPath)
				&& p2.LastWriteTimeUtc == p1.LastWriteTimeUtc
				&& p2.Size == p1.Size) return;
			//ADebug.Print("copy");
			AFile.Copy(sFrom, sTo, IfExists.Delete);
		}

		static bool _RunPrePostBuildScript(bool post, MetaComments m, string outFile)
		{
			var x = post ? m.PostBuild : m.PreBuild;
			string[] args;
			if(x.s == null) {
				args = new string[] { outFile };
			} else {
				args = Util.AStringUtil.CommandLineToArray(x.s);

				//replace variables like $(variable)
				var f = m.CodeFiles[0].f;
				if(s_rx1 == null) s_rx1 = new ARegex(@"\$\((\w+)\)");
				string _ReplFunc(RXMatch k)
				{
					switch(k[1].Value) {
					case "source": return f.ItemPath;
					case "outputFile": return outFile;
					case "outputPath": return m.OutputPath;
					case "optimize": return m.Optimize ? "true" : "false";
					case "role": return m.Role.ToString();
					default: throw new ArgumentException("error in meta: unknown variable " + k.Value);
					}
				}
				for(int i = 0; i < args.Length; i++) args[i] = s_rx1.Replace(args[i], _ReplFunc);
			}

			bool ok = Compile(ECompReason.Run, out var r, x.f);
			if(r.role != ERole.editorExtension) throw new ArgumentException($"meta of '{x.f.Name}' must contain role editorExtension");
			if(!ok) return false;

			RunAssembly.Run(r.file, args, r.pdbOffset, RAFlags.InEditorThread | RAFlags.DontHandleExceptions);
			return true;
		}
		static ARegex s_rx1;

		#region default references and usings

		/// <summary>
		/// These references are added when compiling any script/library.
		/// mscorlib, Microsoft.CSharp, System, System.Core, System.Windows.Forms, System.Drawing, Au.dll.
		/// </summary>
		public static readonly Dictionary<string, string> DefaultReferences = new Dictionary<string, string>
		{
			{"mscorlib", typeof(object).Assembly.Location},
			{"Microsoft.CSharp", typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location}, //need eg for dynamic support
			{"System", typeof(Component).Assembly.Location},
			{"System.Core", typeof(HashSet<>).Assembly.Location},
			{"System.Windows.Forms", typeof(System.Windows.Forms.Form).Assembly.Location},
			{"System.Drawing", typeof(System.Drawing.Point).Assembly.Location},
			{"Au.dll", typeof(AWnd).Assembly.Location},

			//speed: many references makes compiling much slower. We use temporary caching. Permanent caching would add many MB of process memory.
		};

		//note: DefaultReferences and using.txt/DefaultUsings must be in sync. If there is no reference for an using, will be compiler error.

		/// <summary>
		/// Usings for <see cref="Scripting.Compile"/>.
		/// </summary>
		public const string DefaultUsings = @"using Au; using static Au.AStatic; using Au.Types; using System; using System.Collections.Generic; using System.Text; using System.Text.RegularExpressions; using System.Diagnostics; using System.Runtime.InteropServices; using System.IO; using System.Threading; using System.Threading.Tasks; using System.Windows.Forms; using System.Drawing; using System.Linq;";

		#endregion
	}

	public enum ECompReason { Run, CompileAlways, CompileIfNeed }
}
