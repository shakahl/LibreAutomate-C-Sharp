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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Used by other Au projects to execute script assembly in new appdomain etc.
	/// The code must be in Au, to avoid loading the program assembly into the new appdomain. Now Au is loaded instead.
	/// </summary>
	internal static class LibRunAsm
	{
		/// <summary>
		/// Executes assembly in new appdomain in this thread.
		/// Handles exceptions.
		/// </summary>
		/// <param name="name">Appdomain name.</param>
		/// <param name="asmFile">Full path of assembly file.</param>
		/// <param name="pdbOffset">0 or offset of portable PDB in file.</param>
		/// <param name="args">To pass to Main.</param>
		/// <param name="config">Config file path.</param>
		[HandleProcessCorruptedStateExceptions]
		internal static void RunInNewAppDomain(string name, string asmFile, int pdbOffset, string[] args = null, string config = null)
		{
#if true
			var t = typeof(_DomainManager);
			AppDomainSetup se = new AppDomainSetup { AppDomainManagerAssembly = t.Assembly.FullName, AppDomainManagerType = t.FullName };
			if(config != null) se.ConfigurationFile = config; //default is AppDomain.CurrentDomain.SetupInformation.ConfigurationFile

			var ad = AppDomain.CreateDomain(name, null, se);
			ad.UnhandledException += _ad_UnhandledException; //works only for threads of that appdomain started by Thread.Start. Does not work for its primary thread, Task.Run threads and corrupted state exceptions.

			if(args == null) { //TODO
				Perf.Next('x');
				args = new string[] { Perf.StaticInst.Serialize() };
			}

			var ty = typeof(_ADRun);
			//ad.CreateInstance(ty.Assembly.FullName, ty.FullName);
			var v = ad.CreateInstanceAndUnwrap(ty.Assembly.FullName, ty.FullName) as _ADRun;
			v.Run(asmFile, pdbOffset, args);
			AppDomain.Unload(ad);
#else //similar speed. Not used, mostly because locks the assembly file.
			AppDomainSetup se = null;
			if(config != null) {
				se = new AppDomainSetup { ConfigurationFile = config }; //default is AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
			}
			var ad = AppDomain.CreateDomain(name, null, se);
			ad.UnhandledException += _ad_UnhandledException; //works only for threads of that appdomain started by Thread.Start. Does not work for its primary thread, Task.Run threads and corrupted state exceptions.

			if(args == null) { //TODO
				Perf.Next('x');
				args = new string[] { Perf.StaticInst.Serialize() };
			}

			try {
				ad.ExecuteAssembly(asmFile as string, args);
			}
			catch(Exception e) when(!(e is ThreadAbortException)) {
				Print(e.ToString());
				//problems with stack trace and nonserializable exceptions
			}
			finally {
				AppDomain.Unload(ad);
			}
#endif
		}

		private static void _ad_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			//Print("Ad_UnhandledException:");
			Print(e.ExceptionObject.ToString());
		}

		//private static void _ad_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		//{
		//	Print("Ad_FirstChanceException");
		//}

		class _ADRun :MarshalByRefObject
		{
			public void Run(string asmFile, int pdbOffset, string[] args)
			{
				_Run(asmFile, pdbOffset, args, true);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		static void _Run(string asmFile, int pdbOffset, string[] args, bool inAD, bool reThrow = false)
		{
			byte[] bAsm, bPdb = null;
			if(pdbOffset > 0) {
				using(var stream = File.OpenRead(asmFile)) {
					stream.Read(bAsm = new byte[pdbOffset], 0, pdbOffset);
					stream.Read(bPdb = new byte[stream.Length - pdbOffset], 0, bPdb.Length);
				}
			} else {
				bAsm = File.ReadAllBytes(asmFile);
				var pdb = Path.ChangeExtension(asmFile, "pdb");
				if(File_.ExistsAsFile(pdb)) bPdb = File.ReadAllBytes(pdb);
			}
			var a = Assembly.Load(bAsm, bPdb);
			bAsm = bPdb = null;

			if(inAD) {
				var ad = AppDomain.CurrentDomain;
				if(ad.DomainManager is _DomainManager dm) dm.LibEntryAssembly = a;
				//Print(Assembly.GetEntryAssembly());
			}

			try {
				var entryPoint = a.EntryPoint;

				bool useArgs = entryPoint.GetParameters().Length != 0;
				if(useArgs) {
					if(args == null) args = Array.Empty<string>();
#if STANDARD_SCRIPT
				} else if(args != null) {
					//if standard script, set __script__.args. Our compiler adds class __script__ with static string[] args, and adds __script__ to usings.
					a.GetType("__script__")?.GetField("args", BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, args);
#endif
				}

				if(!useArgs) {
					entryPoint.Invoke(null, null);
				} else {
					entryPoint.Invoke(null, new object[] { args });

					//var del = entryPoint.CreateDelegate<Action<string[]>>();
					//del(args);
					////With delegate, exceptions are thrown differently: without TargetInvocationException, but stack includes this (caller) method.
				}

				//TODO: if standard script, run triggers if need. Depends on how we'll implement them.
			}
			catch(TargetInvocationException te) {
				var e = te.InnerException;
				if(reThrow) throw e;
				Print(e.ToString());
			}
			catch(Exception e) when(!_IsSilentException(e)) {
				Debug_.Print(e);
			}

			bool _IsSilentException(Exception e) => e is ThreadAbortException /*|| e is AppDomainUnloadedException*/;

			//see also: TaskScheduler.UnobservedTaskException event.
			//	tested: the event works.
			//	tested: somehow does not terminate process even with <ThrowUnobservedTaskExceptions enabled="true"/>.
			//		Only when AuDialog.Show called, the GC.Collect makes it to disappear but the process remains running.
			//	note: the terminating behavior also can be set in registry or env var. It overrides <ThrowUnobservedTaskExceptions enabled="false"/>.
		}

		//static T CreateDelegate<T>(this MethodInfo methodInfo)
		//{
		//	return (T)(object)methodInfo.CreateDelegate(typeof(T));
		//}

		class _DomainManager :AppDomainManager
		{
			public override Assembly EntryAssembly => LibEntryAssembly;

			internal Assembly LibEntryAssembly;
		}

		/// <summary>
		/// Executes assembly in this appdomain in this thread.
		/// Handles exceptions.
		/// </summary>
		/// <param name="asmFile">Full path of assembly file.</param>
		/// <param name="pdbOffset">0 or offset of portable PDB in file.</param>
		/// <param name="args">To pass to Main.</param>
		/// <param name="reThrow">Don't handle exceptions.</param>
		internal static void RunHere(string asmFile, int pdbOffset, string[] args = null, bool reThrow = false)
		{
			_Run(asmFile, pdbOffset, args, false, reThrow);
		}
	}
}
