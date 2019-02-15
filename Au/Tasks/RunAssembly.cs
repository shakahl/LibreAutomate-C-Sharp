//#define STANDARD_SCRIPT
//#define TEST_STARTUP_SPEED
//#define PRELOAD_PROCESS //not impl

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
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Used by other Au projects to execute a script assembly.
	/// </summary>
	internal unsafe class RunAssembly
	{
		/// <summary>
		/// Executes assembly in this appdomain in this thread. Must be main appdomain.
		/// Handles exceptions.
		/// </summary>
		/// <param name="asmFile">Full path of assembly file.</param>
		/// <param name="args">To pass to Main.</param>
		/// <param name="pdbOffset">0 or offset of portable PDB in assembly file.</param>
		/// <param name="flags"></param>
		[HandleProcessCorruptedStateExceptions]
		public static void Run(string asmFile, string[] args, int pdbOffset, RAFlags flags = 0)
		{
			bool findLoaded = 0 != (flags & RAFlags.InEditorThread);
			_LoadedScriptAssembly lsa = default;
			Assembly asm = findLoaded ? lsa.Find(asmFile) : null;
			if(asm == null) {
				byte[] bAsm, bPdb = null;
				using(var stream = File_.WaitIfLocked(() => File.OpenRead(asmFile))) {
					bAsm = new byte[pdbOffset > 0 ? pdbOffset : stream.Length];
					stream.Read(bAsm, 0, bAsm.Length);
					try {
						if(pdbOffset > 0) {
							bPdb = new byte[stream.Length - pdbOffset];
							stream.Read(bPdb, 0, bPdb.Length);
						} else {
							var s1 = Path.ChangeExtension(asmFile, "pdb");
							if(File_.ExistsAsFile(s1)) bPdb = File.ReadAllBytes(s1);
						}
					}
					catch(Exception ex) { bPdb = null; Debug_.Print(ex); } //not very important
				}
				asm = Assembly.Load(bAsm, bPdb);
				if(findLoaded) lsa.Add(asmFile, asm);

				//never mind: it's possible that we load newer compiled assembly version of script than intended.
			}

			AuTask.Role = 0 != (flags & RAFlags.InEditorThread) ? ATRole.EditorExtension : ATRole.MiniProgram; //default ExeProgram

			try {
				var entryPoint = asm.EntryPoint ?? throw new InvalidOperationException("assembly without entry point (function Main)");

				bool useArgs = entryPoint.GetParameters().Length != 0;
				if(useArgs) {
					if(args == null) args = Array.Empty<string>();
#if STANDARD_SCRIPT
				} else if(args != null) {
					//if standard script, set __script__.args. Our compiler adds class __script__ with static string[] args, and adds __script__ to usings.
					a.GetType("__script__")?.GetField("args", BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, args);

					//if standard script, will need to run triggers in this func. Depends on how we'll implement them.
#endif
				}

				if(useArgs) {
					entryPoint.Invoke(null, new object[] { args });
				} else {
					entryPoint.Invoke(null, null);
				}
			}
			catch(TargetInvocationException te) {
				var e = te.InnerException;
				if(0 != (flags & RAFlags.DontHandleExceptions)) throw e;
				//Print(e);
				AuAppBase.OnHostHandledException(new UnhandledExceptionEventArgs(e, false));
			}

			//see also: TaskScheduler.UnobservedTaskException event.
			//	tested: the event works.
			//	tested: somehow does not terminate process even with <ThrowUnobservedTaskExceptions enabled="true"/>.
			//		Only when AuDialog.Show called, the GC.Collect makes it to disappear but the process does not exit.
			//	note: the terminating behavior also can be set in registry or env var. It overrides <ThrowUnobservedTaskExceptions enabled="false"/>.
		}

		/// <summary>
		/// Remembers and finds script assemblies loaded in this appdomain, to avoid loading the same unchanged assembly multiple times.
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
			public Assembly Find(string asmFile)
			{
				if(_d == null) _d = new Dictionary<string, _Asm>(StringComparer.OrdinalIgnoreCase);
				if(!File_.GetProperties(asmFile, out var p, FAFlags.UseRawPath)) return null;
				_fileTime = p.LastWriteTimeUtc;
				if(_d.TryGetValue(asmFile, out var x)) {
					if(x.time == _fileTime) return x.asm;
					_d.Remove(asmFile);
				}
				return null;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public void Add(string asmFile, Assembly asm)
			{
				if(_fileTime == default) return; //File_.GetProperties failed
				_d.Add(asmFile, new _Asm { time = _fileTime, asm = asm });

				//foreach(var v in _d.Values) Print(v.asm.FullName);
				//foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v.FullName);
			}
		}
	}

	/// <summary>
	/// Flags for <see cref="RunAssembly.Run"/>.
	/// </summary>
	[Flags]
	internal enum RAFlags
	{
		InEditorThread = 1,
		DontHandleExceptions = 2,
	}
}

namespace Au.Types
{
	/// <summary>
	/// This class is used in automation script/app files as base of their main class. Adds some features.
	/// </summary>
	/// <remarks>
	/// This class adds these features:
	/// 1. Static constructor subscribes to <see cref="AppDomain.UnhandledException"/> event. On unhandled exception prints exception info. Without this class not all unhandled exceptions would be printed.
	/// 2. Provides function OnUnhandledException. The script/app can override it.
	/// 
	/// More features may be added in the future.
	/// </remarks>
	public abstract class AuAppBase
	{
		static AuAppBase()
		{
			AppDomain.CurrentDomain.UnhandledException += (ad, e) => {
				if((ad as AppDomain).Id != AppDomain.CurrentDomain.Id) return; //avoid printing twice if subscribed in main and other appdomain
				OnHostHandledException(e);

				//Does not see exceptions:
				//1. thrown in the primary thread of a non-primary appdomain. Workaround: our host process uses try/catch.
				//2. thrown in Task.Run etc threads. It's OK. .NET handles exceptions silently, unless something waits for the task.
				//3. corrupted state exceptions. Tried [HandleProcessCorruptedStateExceptions] etc, unsuccessfully. Never mind.

				//This is used for:
				//1. Exceptions thrown in non-primary threads.
				//2. Exceptions thrown in non-hosted exe process.
				//Other exceptions are handled by the host program with try/catch.
			};
		}

		static AuAppBase s_instance;

		///
		protected AuAppBase()
		{
			s_instance = this;
		}

		/// <summary>
		/// Prints exception info.
		/// Override this function to intercept unhandled exceptions. Call the base function if want to print exception info as usually.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnUnhandledException(UnhandledExceptionEventArgs e) => Print(e.ExceptionObject);

		internal static void OnHostHandledException(UnhandledExceptionEventArgs e)
		{
			if(e.ExceptionObject is ThreadAbortException) return;
			var k = s_instance;
			if(k != null) k.OnUnhandledException(e);
			else Print(e.ExceptionObject);
		}
	}
}
