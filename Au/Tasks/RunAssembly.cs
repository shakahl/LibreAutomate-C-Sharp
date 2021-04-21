//#define TEST_STARTUP_SPEED
//#define TEST_UNLOAD

using Au.Types;
using Au.Util;
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
using System.Runtime.Loader;

namespace Au
{
	/// <summary>
	/// Used by other Au projects to execute a script assembly.
	/// </summary>
	static unsafe class RunAssembly
	{
		/// <summary>
		/// Executes assembly in this thread.
		/// Handles exceptions.
		/// </summary>
		/// <param name="asmFile">Full path of assembly file.</param>
		/// <param name="args">To pass to Main.</param>
		/// <param name="flags"></param>
		/// <param name="fullPathRefs">Paths of assemblies specified using full path.</param>
		public static void Run(string asmFile, string[] args, RAFlags flags = 0, string fullPathRefs = null) {
			//ref var p1 = ref APerf.Shared;
			//p1.Next('i');
			//p1.First();
			//using var p1 = APerf.Create();

			bool inEditorThread = 0 != (flags & RAFlags.InEditorThread);
			bool findLoaded = inEditorThread/*, notLoaded = false*/;
			_LoadedScriptAssembly lsa = default;
			Assembly asm = findLoaded ? lsa.Find(asmFile/*, out notLoaded*/) : null;
			if (asm == null) {
				var alc = inEditorThread
					? new AssemblyLoadContext(null, isCollectible: true)
					: AssemblyLoadContext.Default;
				//p1.Next();

				if (inEditorThread/* && !notLoaded*/) {
					//Use LoadFromStream. It is slower with some AV (eg WD), but LoadFromAssemblyPath has this problem: does not reload modified assembly from same file.
					//	Or would need a dll file with unique name for each version of same script.
					//		Would need to manage all these files. Also, they aren't unloaded, although their Assembly objects are unloaded.
					//		And not always it makes faster even with WD. With Avast never faster.
					//Note: the notLoaded was intended to make faster in some cases. However cannot use it because of the xor.
					var b = File.ReadAllBytes(asmFile); //with WD ~7 ms, but ~25 ms without xor. With Avast ~7 ms regardless of xor. Both fast if already scanned.
					for (int i = 0; i < b.Length; i++) b[i] ^= 1; //prevented AV full dll scan twice. Now fully scans once (WD always scans when loading assembly from stream; Avast when loading from stream or earlier).
					using var stream = new MemoryStream(b, false);
					//p1.Next();
					asm = alc.LoadFromStream(stream); //with WD always 15-25 ms. With Avast it seems always fast.
#if TEST_UNLOAD
					new _DebugAsmDtor().AttachTo(asm);
#endif
					//tested: debugging works. Don't need the overload with pdb parameter.
				} else {
					asm = alc.LoadFromAssemblyPath(asmFile); //0.5-4 ms (depends on AV) if already AV-scanned. Else usually 15-30, but with Avast sometimes eg 70.
				}
				//p1.Next('L');

				if (fullPathRefs != null) {
					var fpr = fullPathRefs.Split('|');
					alc.Resolving += (AssemblyLoadContext alc, AssemblyName an) => {
						//AOutput.Write(an, an.Name, an.FullName);
						foreach (var v in fpr) {
							var s1 = an.Name;
							int iName = v.Length - s1.Length - 4;
							if (iName <= 0 || v[iName - 1] != '\\' || !v.Eq(iName, s1, true)) continue;
							if (!AFile.ExistsAsFile(v)) continue;
							//try {
							return alc.LoadFromAssemblyPath(v);
							//} catch(Exception ex) { ADebug.Print(ex.ToStringWithoutStack()); break; }
						}
						return null;
					};
				}
				if (findLoaded) lsa.Add(asmFile, asm);

				//never mind: it's possible that we load a newer compiled assembly version of script than intended.
			}

			try {
				var entryPoint = asm.EntryPoint ?? throw new InvalidOperationException("assembly without entry point (function Main)");

				bool useArgs = entryPoint.GetParameters().Length != 0;
				if (useArgs) {
					args ??= Array.Empty<string>();
				}

				//p1.Next('m');
				if (!inEditorThread) {
					ATask.s_mainAssembly = asm;
					Log_.Run.Write("Task started.");
				}
				//p1.Next('n');

				if (useArgs) {
					entryPoint.Invoke(null, new object[] { args });
				} else {
					entryPoint.Invoke(null, null);
				}

				//if(!inEditorThread) Log_.Run.Write("Task ended."); //no, wait for other foreground threads
				if (!inEditorThread) AProcess.Exit += (_, _) => Log_.Run.Write("Task ended.");
			}
			catch (TargetInvocationException te) {
				var e = te.InnerException;

				if (!inEditorThread) Log_.Run.Write($"Task failed. Exception: {e.ToStringWithoutStack()}");

				if (0 != (flags & RAFlags.DontHandleExceptions)) throw e;
				//AOutput.Write(e);
				ATask.OnHostHandledException_(new UnhandledExceptionEventArgs(e, false));
			}

			//never mind: although Script.Main starts fast, but the process ends slowly, because of .NET.
			//	Eg if starting an empty "runSingle" script every <70 ms, cannot start eg every 2-nd time.
			//	This func could notify when Script.Main ended, but it cannot know when other foreground threads end, plus process exit event handlers etc.

			//see also: TaskScheduler.UnobservedTaskException event.
			//	tested: the event works.
			//	tested: somehow does not terminate process even with <ThrowUnobservedTaskExceptions enabled="true"/>.
			//		Only when ADialog.Show called, the GC.Collect makes it to disappear but the process does not exit.
			//	note: the terminating behavior also can be set in registry or env var. It overrides <ThrowUnobservedTaskExceptions enabled="false"/>.
		}


		/// <summary>
		/// Remembers and finds script assemblies loaded in this process, to avoid loading the same unchanged assembly multiple times.
		/// </summary>
		struct _LoadedScriptAssembly
		{
			class _Asm
			{
				public DateTime time;
				public Assembly asm;
			}

			static Dictionary<string, _Asm> _d;
			DateTime _fileTime;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public Assembly Find(string asmFile/*, out bool notLoaded*/) {
				//notLoaded = false;
				_d ??= new Dictionary<string, _Asm>(StringComparer.OrdinalIgnoreCase);
				if (!AFile.GetProperties(asmFile, out var p, FAFlags.UseRawPath)) return null;
				_fileTime = p.LastWriteTimeUtc;
				if (_d.TryGetValue(asmFile, out var x)) {
					if (x.time == _fileTime) return x.asm;
					_d.Remove(asmFile);
				} //else notLoaded = true;
				return null;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public void Add(string asmFile, Assembly asm) {
				if (_fileTime == default) return; //AFile.GetProperties failed
				_d.Add(asmFile, new _Asm { time = _fileTime, asm = asm });

				//foreach(var v in _d.Values) AOutput.Write(v.asm.FullName);
				//foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) AOutput.Write(v.FullName);
			}
		}
	}

#if TEST_UNLOAD
	//This shows that AssemblyLoadContext are unloaded on GC.
	class _AssemblyLoadContext : AssemblyLoadContext
	{
		public _AssemblyLoadContext(string name, bool isCollectible) : base(name, isCollectible) { }

		~_AssemblyLoadContext() { AOutput.Write("AssemblyLoadContext unloaded", Name); }

		protected override Assembly Load(AssemblyName assemblyName) {
			//AOutput.Write("Load", assemblyName);
			return null;
		}
	}

	//This shows that Assembly are unloaded on GC, although later than AssemblyLoadContext.
	//However, if using LoadFromAssemblyPath, the dll file remains loaded/locked. That is why we don't use it. For same script would load hundreds of dlls with unique name while developing it.
	class _DebugAsmDtor
	{
		static ConditionalWeakTable<Assembly, _DebugAsmDtor> s_cwt;

		public void AttachTo(Assembly a) { (s_cwt ??= new()).Add(a, this); }

		~_DebugAsmDtor() { AOutput.Write("Assembly unloaded"); }
	}
#endif

}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="RunAssembly.Run"/>.
	/// </summary>
	[Flags]
	enum RAFlags
	{
		InEditorThread = 1,
		DontHandleExceptions = 2,
	}
}
