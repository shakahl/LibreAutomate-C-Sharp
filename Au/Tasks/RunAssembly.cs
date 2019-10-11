//#define TEST_STARTUP_SPEED

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

using Au.Types;
using static Au.AStatic;

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
		/// <param name="pdbOffset">0 or offset of portable PDB in assembly file.</param>
		/// <param name="flags"></param>
		[HandleProcessCorruptedStateExceptions]
		public static void Run(string asmFile, string[] args, int pdbOffset, RAFlags flags = 0)
		{
			bool inEditorThread = 0 != (flags & RAFlags.InEditorThread);
			bool findLoaded = inEditorThread;
			_LoadedScriptAssembly lsa = default;
			Assembly asm = findLoaded ? lsa.Find(asmFile) : null;
			if(asm == null) {
				byte[] bAsm, bPdb = null;
				using(var stream = AFile.WaitIfLocked(() => File.OpenRead(asmFile))) {
					bAsm = new byte[pdbOffset > 0 ? pdbOffset : stream.Length];
					stream.Read(bAsm, 0, bAsm.Length);
					try {
						if(pdbOffset > 0) {
							bPdb = new byte[stream.Length - pdbOffset];
							stream.Read(bPdb, 0, bPdb.Length);
						} else {
							var s1 = Path.ChangeExtension(asmFile, "pdb");
							if(AFile.ExistsAsFile(s1)) bPdb = File.ReadAllBytes(s1);
						}
					}
					catch(Exception ex) { bPdb = null; ADebug.Print(ex); } //not very important
				}
				//APerf.First();
				asm = Assembly.Load(bAsm, bPdb);
				//APerf.NW(); //without AV 7 ms. With Windows Defender 10 ms, but first time 20-900 ms.
				if(findLoaded) lsa.Add(asmFile, asm);

				//never mind: it's possible that we load a newer compiled assembly version of script than intended.
			}

			try {
				var entryPoint = asm.EntryPoint ?? throw new InvalidOperationException("assembly without entry point (function Main)");

				bool useArgs = entryPoint.GetParameters().Length != 0;
				if(useArgs) {
					if(args == null) args = Array.Empty<string>();
				}

				if(!inEditorThread) Util.LibLog.Run.Write("Task started.");

				if(useArgs) {
					entryPoint.Invoke(null, new object[] { args });
				} else {
					entryPoint.Invoke(null, null);
				}

				if(!inEditorThread) Util.LibLog.Run.Write("Task ended.");
			}
			catch(TargetInvocationException te) {
				var e = te.InnerException;

				if(!inEditorThread) Util.LibLog.Run.Write($"Unhandled exception: {e.ToStringWithoutStack()}");

				if(0 != (flags & RAFlags.DontHandleExceptions)) throw e;
				//Print(e);
				AScript.OnHostHandledException(new UnhandledExceptionEventArgs(e, false));
			}

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
			public Assembly Find(string asmFile)
			{
				if(_d == null) _d = new Dictionary<string, _Asm>(StringComparer.OrdinalIgnoreCase);
				if(!AFile.GetProperties(asmFile, out var p, FAFlags.UseRawPath)) return null;
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
				if(_fileTime == default) return; //AFile.GetProperties failed
				_d.Add(asmFile, new _Asm { time = _fileTime, asm = asm });

				//foreach(var v in _d.Values) Print(v.asm.FullName);
				//foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v.FullName);
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// This class is used in automation script files as base of their main class. Adds some features.
	/// </summary>
	/// <remarks>
	/// This class adds these features:
	/// 1. The static constructor subscribes to the <see cref="AppDomain.UnhandledException"/> event. On unhandled exception prints exception info. Without this class not all unhandled exceptions would be printed.
	/// 2. Provides virtual function <see cref="OnUnhandledException"/>. The script can override it.
	/// 3. Provides property <see cref="Triggers"/>.
	/// 
	/// More features may be added in the future.
	/// </remarks>
	public abstract class AScript
	{
		static AScript()
		{
			//Print("static AScript"); //note: static ctor of inherited class is called BEFORE this. Never mind.
			AppDomain.CurrentDomain.UnhandledException += (ad, e) => {
				OnHostHandledException(e);

				//Does not see exceptions:
				//1. thrown in Task.Run etc threads. It's OK. .NET handles exceptions silently, unless something waits for the task.
				//3. corrupted state exceptions. Tried [HandleProcessCorruptedStateExceptions] etc, unsuccessfully. Never mind.

				//This is used for:
				//1. Exceptions thrown in non-primary threads.
				//2. Exceptions thrown in non-hosted exe process.
				//Other exceptions are handled by the host program with try/catch.
			};
		}

		static AScript s_instance;

		///
		protected AScript()
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

		/// <summary>
		/// Gets or sets an <see cref="Au.Triggers.ActionTriggers"/> instance, as a field of this class.
		/// </summary>
		/// <remarks>
		/// This property can be used in automation scripts to avoid creating an <b>ActionTriggers</b> variable explicitly. The returned value is a field of this class. The <b>ActionTriggers</b> object is auto-created when callig this property first time or after setting it = null.
		/// In automation scripts this property is available because this class is the base of the Script class. In other classes need to create an <b>ActionTriggers</b> variable explicitly. In scripts you also can create explicitly if you like, for example to have more than one instance.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Hotkey["Ctrl+K"] = o => Print(o.Trigger);
		/// Triggers.Run();
		/// ]]></code>
		/// </example>
		protected Au.Triggers.ActionTriggers Triggers {
			get => _triggers ??= new Au.Triggers.ActionTriggers();
			set => _triggers = value;
		}
		Au.Triggers.ActionTriggers _triggers;
	}

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
