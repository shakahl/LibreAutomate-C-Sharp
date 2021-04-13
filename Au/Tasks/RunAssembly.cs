//#define TEST_STARTUP_SPEED

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
//using System.Linq;

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
			//p1.First();
			//p1.Next('i');

			bool inEditorThread = 0 != (flags & RAFlags.InEditorThread);
			bool findLoaded = inEditorThread;
			_LoadedScriptAssembly lsa = default;
			Assembly asm = findLoaded ? lsa.Find(asmFile) : null;
			if (asm == null) {
				var alc = System.Runtime.Loader.AssemblyLoadContext.Default;
				//SHOULDDO: try to unload editorExtension assemblies. It seems AssemblyLoadContext supports it. Not tested. I guess it would create more problems than is useful.
				//p1.Next();
				asm = alc.LoadFromAssemblyPath(asmFile);
				//p1.Next('L'); //0.5-3 ms, depending on AV. LoadFromStream 30-100 ms, depending on AV.

				if (fullPathRefs != null) {
					var fpr = fullPathRefs.Split('|');
					alc.Resolving += (System.Runtime.Loader.AssemblyLoadContext alc, AssemblyName an) => {
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
					if (args == null) args = Array.Empty<string>();
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
				AScript.OnHostHandledException(new UnhandledExceptionEventArgs(e, false));
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
			public Assembly Find(string asmFile) {
				_d ??= new Dictionary<string, _Asm>(StringComparer.OrdinalIgnoreCase);
				if (!AFile.GetProperties(asmFile, out var p, FAFlags.UseRawPath)) return null;
				_fileTime = p.LastWriteTimeUtc;
				if (_d.TryGetValue(asmFile, out var x)) {
					if (x.time == _fileTime) return x.asm;
					_d.Remove(asmFile);
				}
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

	/// <summary>
	/// This class is used in automation script files as base of their main class. Adds some features.
	/// </summary>
	/// <remarks>
	/// This class adds these features:
	/// 1. The static constructor subscribes to the <see cref="AppDomain.UnhandledException"/> event. On unhandled exception shows exception info in output. Without this class not all unhandled exceptions would be shown.
	/// 2. Provides virtual function <see cref="OnUnhandledException"/>. The script can override it.
	/// 3. Provides property <see cref="Triggers"/>.
	/// 4. The static constructor calls <see cref="ADefaultTraceListener.Setup"/>(useAOutput: true). Then <see cref="Debug.Assert"/> etc will show a dialog with buttons Exit|Debug|Ignore instead of "Unknown hard error" message box.
	/// 5. The static constructor sets default culture of all threads = invariant. See <see cref="AProcess.CultureIsInvariant"/>.
	/// 
	/// More features may be added in the future.
	/// </remarks>
	public abstract class AScript
	{
		static AScript() {
			//AOutput.Write("static AScript"); //note: static ctor of inherited class is called BEFORE this. Never mind.
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

			ADefaultTraceListener.Setup(useAOutput: true);

			//#if !DEBUG
			AProcess.CultureIsInvariant = true;
			//#endif
		}

		static AScript s_instance;

		///
		protected AScript() {
			s_instance = this;
		}

		/// <summary>
		/// Writes exception info to the output.
		/// Override this function to intercept unhandled exceptions. Call the base function if want to see exception info as usually.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnUnhandledException(UnhandledExceptionEventArgs e) => AOutput.Write(e.ExceptionObject);

		internal static void OnHostHandledException(UnhandledExceptionEventArgs e) {
			var k = s_instance;
			if (k != null) k.OnUnhandledException(e);
			else AOutput.Write(e.ExceptionObject);
		}

		/// <summary>
		/// Gets an auto-created <see cref="Au.Triggers.ActionTriggers"/> object that can be used in this script.
		/// </summary>
		/// <remarks>
		/// This property can be used in scripts to avoid creating an <b>ActionTriggers</b> variable explicitly. The returned <b>ActionTriggers</b> object belongs to this class; it is auto-created.
		/// In scripts this property is available because this class is the base of the <b>Script</b> class. In other classes need to create an <b>ActionTriggers</b> variable explicitly. In scripts you also can create explicitly if you like, for example to have more than one instance.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Hotkey["Ctrl+K"] = o => AOutput.Write(o.Trigger);
		/// Triggers.Run();
		/// ]]></code>
		/// </example>
		protected Au.Triggers.ActionTriggers Triggers {
			get => _triggers ??= new Au.Triggers.ActionTriggers();
			//set => _triggers = value;
		}
		Au.Triggers.ActionTriggers _triggers;
	}
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
