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
using System.Linq;

using Au.Types;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Resources;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Au.Compiler
{
	/// <summary>
	/// Compiles C# files.
	/// </summary>
	static partial class Compiler
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
		/// Adds <see cref="MetaReferences.DefaultReferences"/>.
		/// 
		/// If f role is classFile:
		///		If CompReason.Run, does not compile (just parses meta), sets r.role=classFile and returns false.
		///		Else compiles but does not create output files.
		/// </remarks>
		public static bool Compile(ECompReason reason, out CompResults r, FileNode f, FileNode projFolder = null) {
			Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1);
			r = null;
			var cache = XCompiled.OfCollection(f.Model);
			bool isCompiled = reason != ECompReason.CompileAlways && cache.IsCompiled(f, out r, projFolder);

			//AOutput.Write("isCompiled=" + isCompiled);

			if (!isCompiled) {
				bool ok = false;
				try {
					ok = _Compile(reason == ECompReason.Run, f, out r, projFolder);
				}
				catch (Exception ex) {
					//AOutput.Write($"Failed to compile '{f.Name}'. {ex.ToStringWithoutStack()}");
					AOutput.Write($"Failed to compile '{f.Name}'. {ex}");
				}

				if (!ok) {
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
			public EIfRunning2 ifRunning2;
			public EUac uac;
			public bool prefer32bit;
			public bool console;

			///// <summary>Has config file this.file + ".config".</summary>
			//public bool hasConfig;

			/// <summary>Main() does not have [STAThread].</summary>
			public bool mtaThread;

			/// <summary>The assembly is normal .exe or .dll file, not in cache. If exe, its dependencies were copied to its directory.</summary>
			public bool notInCache;

			/// <summary>
			/// |-separated list of full paths of references that are not directly in <see cref="AFolders.ThisApp"/> or its subfolder "Libraries".
			/// </summary>
			public string fullPathRefs;

			/// <summary>
			/// Adds path to <see cref="fullPathRefs"/> if it is not directly in <see cref="AFolders.ThisApp"/> or its subfolder "Libraries".
			/// Does not add if role is not miniProgram (it must be set before) or if path does not end with ".dll".
			/// </summary>
			/// <param name="path">Full path.</param>
			public void AddToFullPathRefsIfNeed(string path) {
				//AOutput.Write(path);
				if (role != ERole.miniProgram) return;
				var ta = AFolders.ThisAppBS;
				if (path.Starts(ta, true)) {
					int i = ta.Length, j = path.IndexOf('\\', i);
					if (j < 0) return;
					if (j - i == 9 && path.Eq(i, "Libraries", true) && path.IndexOf('\\', j + 1) < 0) return;
				}
				if (!path.Ends(".dll", true)) return;
				//AOutput.Write("AddToFullPathRefsIfNeed", path);
				fullPathRefs = fullPathRefs == null ? path : fullPathRefs + "|" + path;
			}
		}

		static bool _Compile(bool forRun, FileNode f, out CompResults r, FileNode projFolder) {
			var p1 = APerf.Create();
			r = new CompResults();

			var m = new MetaComments();
			if (!m.Parse(f, projFolder, EMPFlags.PrintErrors)) return false;
			var err = m.Errors;
			p1.Next('m');

			bool needOutputFiles = m.Role != ERole.classFile;

			//if for run, don't compile if f role is classFile
			if (forRun && !needOutputFiles) {
				r.role = ERole.classFile;
				return false;
			}

			XCompiled cache = XCompiled.OfCollection(f.Model);
			string outPath = null, outFile = null, fileName = null;
			bool notInCache = false;
			if (needOutputFiles) {
				if (notInCache = m.OutputPath != null) {
					outPath = m.OutputPath;
					fileName = m.Name + ".dll";
				} else {
					outPath = cache.CacheDirectory;
					fileName = f.IdString;
				}
				outFile = outPath + "\\" + fileName;
				AFile.CreateDirectory(outPath);
			}

			if (m.PreBuild.f != null && !_RunPrePostBuildScript(false, m, outFile)) return false;

			var po = m.CreateParseOptions();
			var trees = new CSharpSyntaxTree[m.CodeFiles.Count];
			for (int i = 0; i < trees.Length; i++) {
				var f1 = m.CodeFiles[i];
				trees[i] = CSharpSyntaxTree.ParseText(f1.code, po, f1.f.FilePath, Encoding.UTF8) as CSharpSyntaxTree;
				//info: file path is used later in several places: in compilation error messages, run time stack traces (from PDB), Visual Studio debugger, etc.
				//	Our AOutputServer.SetNotifications callback will convert file/line info to links. It supports compilation errors and run time stack traces.
			}
			//p1.Next('t');

			string asmName = m.Name;
			if (m.Role == ERole.editorExtension) asmName = asmName + "|" + (++c_versionCounter).ToString(); //AssemblyLoadContext.Default cannot load multiple assemblies with same name
			var compilation = CSharpCompilation.Create(asmName, trees, m.References.Refs, m.CreateCompilationOptions());
			//p1.Next('c');

			string xdFile = null;
			Stream xdStream = null;
			Stream resNat = null;
			List<ResourceDescription> resMan = null;
			EmitOptions eOpt = null;

			if (needOutputFiles) {
				_AddAttributes(ref compilation, m);

				//Create debug info always. It is used for run-time error links.
				//Embed it in assembly. It adds < 1 KB. Almost the same compiling speed. Same loading speed.
				//Don't use classic pdb file. It is 14 KB, 2 times slower compiling, slower loading; error after switching to .NET Core: Unexpected error writing debug information -- 'The version of Windows PDB writer is older than required: 'diasymreader.dll''.
				eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.Embedded);

				if (m.XmlDocFile != null) xdStream = AFile.WaitIfLocked(() => File.Create(xdFile = APath.Normalize(m.XmlDocFile, outPath)));

				resMan = _CreateManagedResources(m);
				if (err.ErrorCount != 0) { err.PrintAll(); return false; }

				if (m.Role == ERole.exeProgram || m.Role == ERole.classLibrary) resNat = _CreateNativeResources(m, compilation);
				if (err.ErrorCount != 0) { err.PrintAll(); return false; }

				//EmbeddedText.FromX //it seems we can embed source code in PDB. Not tested.
			}

			//p1.Next();
			var asmStream = new MemoryStream(8000);
			var emitResult = compilation.Emit(asmStream, null, xdStream, resNat, resMan, eOpt);

			if (needOutputFiles) {
				xdStream?.Dispose();
				resNat?.Dispose(); //info: compiler disposes resMan
			}
			//p1.Next('e');

			var diag = emitResult.Diagnostics;
			if (!diag.IsEmpty) {
				foreach (var d in diag) {
					if (d.Severity == DiagnosticSeverity.Hidden) continue;
					err.AddErrorOrWarning(d, f);
					if (d.Severity == DiagnosticSeverity.Error && d.Id == "CS0009") MetaReferences.RemoveBadReference(d.GetMessage());
				}
				err.PrintAll();
			}
			if (!emitResult.Success) {
				if (needOutputFiles) {
					Api.DeleteFile(outFile);
					if (xdFile != null) Api.DeleteFile(xdFile);
				}
				return false;
			}

			if (needOutputFiles) {
				//If there is no [STAThread], will need MTA thread.
				if (m.Role == ERole.miniProgram || m.Role == ERole.exeProgram) {
					bool hasSTAThread = compilation.GetEntryPoint(default)?.GetAttributes().Any(o => o.ToString() == "System.STAThreadAttribute") ?? false;
					if (!hasSTAThread) r.mtaThread = true;
				}

				//create assembly file
				p1.Next();
				gSave:
#if true
				var hf = Api.CreateFile(outFile, Api.GENERIC_WRITE, 0, default, Api.CREATE_ALWAYS);
				if (hf.Is0) {
					var ec = ALastError.Code;
					if (ec == Api.ERROR_SHARING_VIOLATION && _RenameLockedFile(outFile, notInCache: notInCache)) goto gSave;
					throw new AuException(ec);
				}
				var b = asmStream.GetBuffer();
				using (hf) if (!Api.WriteFile2(hf, b.AsSpan(0, (int)asmStream.Length), out _)) throw new AuException(0);
#else //same speed, but I like code without exceptions
				try {
						using var fileStream = File.Create(outFile, (int)asmStream.Length);
						asmStream.Position = 0;
						asmStream.CopyTo(fileStream);
					}
					catch(IOException e1) when((e1.HResult & 0xffff) == Api.ERROR_SHARING_VIOLATION) {
						if(!_RenameLockedFile(outFile)) throw;
						goto gSave;
					}
				}
#endif
				p1.Next('s'); //saving would be fast, but with AV can take half of time. Tested only with WD.
				r.file = outFile;

				if (m.Role == ERole.exeProgram) {
					bool need32 = m.Prefer32Bit || AVersion.Is32BitOS;
					bool need64 = !need32 || m.Optimize;
					need32 |= m.Optimize;

					//copy Core app host template exe, add native resources, set assembly name, set console flag if need
					if (need64) _AppHost(outFile, fileName, m, bit32: false);
					if (need32) _AppHost(outFile, fileName, m, bit32: true);
					p1.Next('h');

					//copy dlls to the output directory
					_CopyDlls(m, asmStream, need64: need64, need32: need32);
					p1.Next('d');

					//copy config file to the output directory
					//var configFile = exeFile + ".config";
					//if(m.ConfigFile != null) {
					//	r.hasConfig = true;
					//	_CopyFileIfNeed(m.ConfigFile.FilePath, configFile);
					//} else if(AFile.ExistsAsFile(configFile, true)) {
					//	AFile.Delete(configFile);
					//}
				}
			}

			if (m.PostBuild.f != null && !_RunPrePostBuildScript(true, m, outFile)) return false;

			if (needOutputFiles) {
				cache.AddCompiled(f, outFile, m, r.mtaThread);

				if (notInCache) AOutput.Write($"<>Output folder: <link>{m.OutputPath}<>");
			}

			r.name = m.Name;
			r.role = m.Role;
			r.runMode = m.RunMode;
			r.ifRunning = m.IfRunning;
			r.ifRunning2 = m.IfRunning2;
			r.uac = m.Uac;
			r.prefer32bit = m.Prefer32Bit;
			r.console = m.Console;
			r.notInCache = notInCache;
			if (m.Role == ERole.miniProgram) {
				var refs = m.References.Refs;
				for (int i = MetaReferences.DefaultReferences.Count; i < refs.Count; i++) r.AddToFullPathRefsIfNeed(refs[i].FilePath);
			}

#if TRACE
			//p1.NW('C');
#endif
			return true;

			//SHOULDDO: rebuild if missing apphost. Now rebuilds only if missing dll.
		}

		static int c_versionCounter;

		//public static void Warmup(Document document)
		//{
		//	var compilation = document.Project.GetCompilationAsync().Result;
		//	//var eOpt = new EmitOptions(debugInformationFormat: DebugInformationFormat.Embedded);
		//	var asmStream = new MemoryStream(4096);
		//	compilation.Emit(asmStream);
		//	//compilation.Emit(asmStream, null, options: eOpt); //somehow makes slower later
		//	//compilation.Emit(asmStream, null, xdStream, resNat, resMan, eOpt);
		//}

		/// <summary>
		/// Adds some attributes if not specified in code.
		/// Adds: [module: DefaultCharSet(CharSet.Unicode)], [assembly: TargetFramework("...")]
		/// </summary>
		static void _AddAttributes(ref CSharpCompilation compilation, MetaComments m) {
			int needAttr = 0x100;

			foreach (var v in compilation.SourceModule.GetAttributes()) {
				//AOutput.Write(v.AttributeClass.Name);
				switch (v.AttributeClass.Name) {
				case "DefaultCharSetAttribute": needAttr &= ~0x100; break;
				}
			}
			if (m.Role == ERole.exeProgram || m.Role == ERole.classLibrary) {
				needAttr |= 0x200; //need [TargetFramework] for exeProgram, else AppContext.TargetFrameworkName will return null: => Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
				foreach (var v in compilation.Assembly.GetAttributes()) {
					//AOutput.Write(v.AttributeClass.Name);
					switch (v.AttributeClass.Name) {
					case "TargetFrameworkAttribute": needAttr &= ~0x200; break;
					}
				}
			}

			//rejected. Now we don't load from byte[].
			///// If miniProgram or exeProgram, also adds: AssemblyCompany, AssemblyProduct, AssemblyInformationalVersion. It is to avoid exception in Application.ProductName etc when the entry assembly is loaded from byte[].
			//if(m.Role == ERole.miniProgram || m.Role == ERole.exeProgram) {
			//	needAttr |= 7;
			//	foreach(var v in compilation.Assembly.GetAttributes()) {
			//		//AOutput.Write(v.AttributeClass.Name);
			//		switch(v.AttributeClass.Name) {
			//		case "AssemblyCompanyAttribute": needAttr &= ~1; break;
			//		case "AssemblyProductAttribute": needAttr &= ~2; break;
			//		case "AssemblyInformationalVersionAttribute": needAttr &= ~4; break;
			//		}
			//	}
			//}

			if (needAttr != 0) {
				using (new Util.StringBuilder_(out var sb)) {
					sb.AppendLine("using System.Reflection;using System.Runtime.InteropServices;");
					if (0 != (needAttr & 0x100)) sb.AppendLine("[module: DefaultCharSet(CharSet.Unicode)]");
					if (0 != (needAttr & 0x200)) sb.AppendLine($"[assembly: System.Runtime.Versioning.TargetFramework(\"{AppContext.TargetFrameworkName}\")]");
					//if(0 != (needAttr & 1)) sb.AppendLine("[assembly: AssemblyCompany(\" \")]");
					//if(0 != (needAttr & 2)) sb.AppendLine("[assembly: AssemblyProduct(\"Script\")]");
					//if(0 != (needAttr & 4)) sb.AppendLine("[assembly: AssemblyInformationalVersion(\"0\")]");

					var tree = CSharpSyntaxTree.ParseText(sb.ToString(), new CSharpParseOptions(LanguageVersion.Preview)) as CSharpSyntaxTree;
					compilation = compilation.AddSyntaxTrees(tree);
				}
			}
		}

		static List<ResourceDescription> _CreateManagedResources(MetaComments m) {
			var a = m.Resources;
			if (a.NE_()) return null;
			var R = new List<ResourceDescription>();
			ResourceWriter rw = null;
			MemoryStream stream = null;
			string resourcesName = m.Name + ".g.resources";
			FileNode curFile = null;

			void _End() {
				curFile = null;
				if (rw == null) return;
				rw.Generate();
				var st = stream; stream = null; //to create new lambda delegate
				st.Position = 0;
				R.Add(new ResourceDescription(resourcesName, () => st, true));
				rw = null;
			}

			void _Add(FileNode f, string resType, FileNode folder = null) {
				curFile = f;
				string name = f.Name, path = f.FilePath;
				if (folder != null) for (var pa = f.Parent; pa != folder; pa = pa.Parent) name = pa.Name + "/" + name;
				//AOutput.Write(f, resType, folder, name, path);
				if (resType == "embedded") {
					R.Add(new ResourceDescription(name, () => AFile.LoadStream(path), true));
				} else {
					name = name.Lower(); //else pack URI does not work
					rw ??= new ResourceWriter(stream = new MemoryStream());
					switch (resType) {
					case null:
						//rw.AddResource(name, File.OpenRead(path), closeAfterWrite: true); //no, would not close on error
						rw.AddResource(name, new MemoryStream(AFile.LoadBytes(path)));
						break;
					case "byte[]":
						rw.AddResource(name, AFile.LoadBytes(path));
						break;
					case "string":
						rw.AddResource(name, AFile.LoadText(path));
						break;
					case "strings":
						var csv = ACsv.Load(path);
						if (csv.ColumnCount != 2) throw new ArgumentException("CSV must contain 2 columns separated with ,");
						foreach (var row in csv.Data) rw.AddResource(row[0], row[1]);
						break;
					default: throw new ArgumentException("error in meta: Incorrect /suffix");
					}
				}
			}

			try {
				foreach (var v in a) {
					//if (v.f == null) { // /resources //rejected
					//	_End();
					//	resourcesName = v.s + ".resources";
					//} else
					if (v.f.IsFolder) {
						foreach (var des in v.f.Descendants()) if (!des.IsFolder) _Add(des, v.s, v.f);
					} else {
						_Add(v.f, v.s);
					}
				}
				_End();
			}
			catch (Exception e) {
				rw?.Dispose();
				_ResourceException(e, m, curFile);
				return null;
			}
			//note: don't Close/Dispose rw. It closes stream. Compiler will close it. There is no other disposable data in rw.
			return R;
		}

		static void _ResourceException(Exception e, MetaComments m, FileNode curFile) {
			var em = e.ToStringWithoutStack();
			var err = m.Errors;
			var f = m.CodeFiles[0].f;
			if (curFile == null) err.AddError(f, "Failed to add resources. " + em);
			else err.AddError(f, $"Failed to add resource '{curFile.Name}'. " + em);
		}

		static Stream _CreateNativeResources(MetaComments m, CSharpCompilation compilation) {
#if true
			return compilation.CreateDefaultWin32Resources(versionResource: true, noManifest: true, null, null);
#else
			var manifest = m.ManifestFile;

			string manifestPath = null;
			if(manifest != null) manifestPath = manifest.FilePath;
			else if(m.Role == ERole.exeProgram /*&& m.ResFile == null*/) manifestPath = AFolders.ThisAppBS + "default.exe.manifest"; //don't: uac

			Stream manStream = null, icoStream = null;
			FileNode curFile = null;
			try {
				//if(m.ResFile != null) return File.OpenRead((curFile = m.ResFile).FilePath);
				if(manifestPath != null) { curFile = manifest; manStream = File.OpenRead(manifestPath); }
				if(m.IconFile != null) icoStream = File.OpenRead((curFile = m.IconFile).FilePath);
				curFile = null;
				return compilation.CreateDefaultWin32Resources(versionResource: true, noManifest: manifestPath == null, manStream, icoStream);
			}
			catch(Exception e) {
				manStream?.Dispose();
				icoStream?.Dispose();
				_ResourceException(e, m, curFile);
				return null;
			}
#endif
		}

		static unsafe string _AppHost(string outFile, string fileName, MetaComments m, bool bit32) {
			//A .NET Core exe actually is a managed dll hosted by a native exe file known as apphost.
			//When creating an exe, VS copies template apphost from eg "C:\Program Files\dotnet\sdk\3.1.100\AppHostTemplate\apphost.exe" and modifies it, eg copies native resources from the dll.
			//We have own apphost exe created by the Au.AppHost project. This function copies it and modifies in a similar way like VS does.

			//var p1 = APerf.Create();
			string exeFile = DllNameToAppHostExeName(outFile, bit32);

			if (AFile.ExistsAsAny(exeFile) && !Api.DeleteFile(exeFile)) {
				var ec = ALastError.Code;
				if (!(ec == Api.ERROR_ACCESS_DENIED && _RenameLockedFile(exeFile, notInCache: true))) throw new AuException(ec);
			}

			var appHost = AFolders.ThisAppBS + (bit32 ? "32" : "64") + @"\Au.AppHost.exe";
			bool done = false;
			try {
				var b = File.ReadAllBytes(appHost);
				//p1.Next();
				//write assembly name in placeholder memory. In AppHost.cpp: char s_asmName[800] = "\0hi7yl8kJNk+gqwTDFi7ekQ";
				fixed (byte* p = b) {
					int i = Util.BytePtr_.AsciiFindString(p, b.Length, "hi7yl8kJNk+gqwTDFi7ekQ") - 1;
					i += Encoding.UTF8.GetBytes(fileName, 0, fileName.Length, b, i);
					b[i] = 0;
				}

#if true
				var res = new _Resources();
				if (m.IconFile != null) res.AddIcon(m.IconFile.FilePath);
				res.AddVersion(outFile);

				string manifest = null;
				if (m.ManifestFile != null) manifest = m.ManifestFile.FilePath;
				else if (m.Role == ERole.exeProgram) manifest = AFolders.ThisAppBS + "default.exe.manifest"; //don't: uac
				if (manifest != null) res.AddManifest(manifest);

				res.WriteAll(exeFile, b, bit32, m.Console);

				//speed: AV makes this slooow.
#else
				//if console, write IMAGE_SUBSYSTEM_WINDOWS_CUI
				if(m.Console) b[0x14c] = 3; //todo: different if bit32

				for(int i = 5; ; i += Math.Min(i, 100)) { //retry, because sometimes access violation, maybe is open by AV
					try {
						File.WriteAllBytes(exeFile, b);
						//p1.Next('c');
						//copy native resources from assembly dll. Don't need native resources in dll (only version), but this is how VS does.
						using var u = new Microsoft.NET.HostModel.ResourceUpdater(exeFile);
						u.AddResourcesFromPEImage(outFile).Update();
					}
					catch when(i < 700) { //WriteAllBytes usually throws IOException; ResourceUpdater undocumented.
						Thread.Sleep(i);
						continue;
					}
					break;
				}

				//speed: Windows Defender makes this the slowest part of the compilation.
				//	Whole compilation 4 times slower if creating only 64bit or 32bit exe, and 6 times slower if both.
				//	With the above code whole compilation in slowest cases is 2 times faster that with this, eg 180 -> 90 ms.
				//	Workaround: add the output folder to AV exclusions.

				//or
				//Microsoft.NET.HostModel.AppHost.HostWriter.CreateAppHost(appHost, exeFile, fileName, !m.Console, outFile); //nuget Microsoft.NET.HostModel
				//works, but: 1. Need nuget Microsoft.NET.HostModel, or copy more code from its source. 2. Apphost exe must be console; can't because used not only here.
				//	Most of the above code is like in HostWriter.CreateAppHost.
#endif
				//p1.NW();
				done = true;
			}
			finally { if (!done) Api.DeleteFile(exeFile); }

			return exeFile;
		}

		static void _CopyDlls(MetaComments m, Stream asmStream, bool need64, bool need32) {
			asmStream.Position = 0;
			using var pr = new PEReader(asmStream, PEStreamOptions.LeaveOpen);
			var mr = pr.GetMetadataReader();
			var usedRefs = mr.AssemblyReferences.Select(handle => mr.GetString(mr.GetAssemblyReference(handle).Name)).ToList();
			//AOutput.Write(usedRefs); AOutput.Write("---");

			bool _CopyRefIfNeed(string sFrom, string sTo) {
				if (!usedRefs.Contains(APath.GetNameNoExt(sFrom), StringComparer.OrdinalIgnoreCase)) return false;
				_CopyFileIfNeed(sFrom, sTo);
				return true;
			}

			if (_CopyRefIfNeed(typeof(AWnd).Assembly.Location, m.OutputPath + @"\Au.dll")) {
				if (need64) _CopyFileIfNeed(AFolders.ThisAppBS + @"64\AuCpp.dll", m.OutputPath + @"\64\AuCpp.dll");
				if (need32) _CopyFileIfNeed(AFolders.ThisAppBS + @"32\AuCpp.dll", m.OutputPath + @"\32\AuCpp.dll");
			}

			var refs = m.References.Refs;
			for (int i = MetaReferences.DefaultReferences.Count; i < refs.Count; i++) {
				var s1 = refs[i].FilePath;
				var s2 = m.OutputPath + "\\" + APath.GetName(s1);
				//AOutput.Write(s1, s2);
				_CopyRefIfNeed(s1, s2);
			}

			//copy unmanaged dlls
			bool usesSqlite = false;
			foreach (var handle in mr.TypeReferences) {
				var tr = mr.GetTypeReference(handle);
				//AOutput.Write(mr.GetString(tr.Name), mr.GetString(tr.Namespace));
				string type = mr.GetString(tr.Name);
				if ((type.Starts("ASqlite") && mr.GetString(tr.Namespace) == "Au") || (type.Starts("SL") && mr.GetString(tr.Namespace) == "Au.Types")) {
					usesSqlite = true;
					break;
				}
			}
			//AOutput.Write(usesSqlite);
			if (usesSqlite) {
				if (need64) _CopyFileIfNeed(AFolders.ThisAppBS + @"64\sqlite3.dll", m.OutputPath + @"\64\sqlite3.dll");
				if (need32) _CopyFileIfNeed(AFolders.ThisAppBS + @"32\sqlite3.dll", m.OutputPath + @"\32\sqlite3.dll");
			}
		}

		static void _CopyFileIfNeed(string sFrom, string sTo) {
			//AOutput.Write(sFrom);
			if (AFile.GetProperties(sTo, out var p2, FAFlags.UseRawPath) //if exists
				&& AFile.GetProperties(sFrom, out var p1, FAFlags.UseRawPath)
				&& p2.LastWriteTimeUtc == p1.LastWriteTimeUtc
				&& p2.Size == p1.Size) return;
			AFile.Copy(sFrom, sTo, FIfExists.Delete);
		}

		static bool _RunPrePostBuildScript(bool post, MetaComments m, string outFile) {
			var x = post ? m.PostBuild : m.PreBuild;
			string[] args;
			if (x.s == null) {
				args = new string[] { _OutputFile() };
			} else {
				args = Util.AStringUtil.CommandLineToArray(x.s);

				//replace variables like $(variable)
				var f = m.CodeFiles[0].f;
				if (s_rx1 == null) s_rx1 = new ARegex(@"\$\((\w+)\)");
				string _ReplFunc(RXMatch k) {
					switch (k[1].Value) {
					case "outputFile": return _OutputFile();
					case "outputPath": return m.OutputPath;
					case "source": return f.ItemPath;
					case "role": return m.Role.ToString();
					case "optimize": return m.Optimize ? "true" : "false";
					case "prefer32bit": return m.Prefer32Bit ? "true" : "false";
					default: throw new ArgumentException("error in meta: unknown variable " + k.Value);
					}
				}
				for (int i = 0; i < args.Length; i++) args[i] = s_rx1.Replace(args[i], _ReplFunc);
			}

			string _OutputFile() => m.Role == ERole.exeProgram ? DllNameToAppHostExeName(outFile, m.Prefer32Bit || AVersion.Is32BitOS) : outFile;

			bool ok = Compile(ECompReason.Run, out var r, x.f);
			if (r.role != ERole.editorExtension) throw new ArgumentException($"meta of '{x.f.Name}' must contain role editorExtension");
			if (!ok) return false;

			RunAssembly.Run(r.file, args, RAFlags.InEditorThread | RAFlags.DontHandleExceptions);
			return true;
		}
		static ARegex s_rx1;

		/// <summary>
		/// Replaces ".dll" with "-32.exe" if bit32, else with ".exe".
		/// </summary>
		public static string DllNameToAppHostExeName(string dll, bool bit32)
			=> dll.ReplaceAt(dll.Length - 4, 4, bit32 ? "-32.exe" : ".exe");

		static bool _RenameLockedFile(string file, bool notInCache) {
			//If the assembly file is currently loaded, we get ERROR_SHARING_VIOLATION. But we can rename the file.
			//tested: can't rename if ERROR_USER_MAPPED_FILE or ERROR_LOCK_VIOLATION.
			string renamed = null;
			for (int i = 1; ; i++) {
				renamed = file + "'" + i.ToString();
				if (Api.MoveFileEx(file, renamed, 0)) goto g1;
				if (ALastError.Code != Api.ERROR_ALREADY_EXISTS) break;
				if (Api.MoveFileEx(file, renamed, Api.MOVEFILE_REPLACE_EXISTING)) goto g1;
			}
			return false;
			g1:
			if (notInCache) {
				if (s_renamedFiles == null) {
					s_renamedFiles = new List<string>();
					AProcess.Exit += (_, _) => _DeleteRenamedLockedFiles(null);
					s_rfTimer = new ATimer(_DeleteRenamedLockedFiles);
				}
				if (!s_rfTimer.IsRunning) s_rfTimer.Every(60_000);
				s_renamedFiles.Add(renamed);
			}
			return true;
		}
		static List<string> s_renamedFiles;
		static ATimer s_rfTimer;

		static void _DeleteRenamedLockedFiles(ATimer timer) {
			var a = s_renamedFiles;
			for (int i = a.Count; --i >= 0;) {
				if (Api.DeleteFile(a[i]) || ALastError.Code == Api.ERROR_FILE_NOT_FOUND) a.RemoveAt(i);
			}
			if (a.Count == 0) timer?.Stop();
		}
	}

	public enum ECompReason { Run, CompileAlways, CompileIfNeed }
}
