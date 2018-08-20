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
		/// <param name="asmPathOrBytes">String (path) or byte[] (assembly in memory).</param>
		/// <param name="isScript"></param>
		/// <param name="args">To pass to Main.</param>
		/// <param name="config">Config file path.</param>
		[HandleProcessCorruptedStateExceptions]
		internal static void RunInNewAppDomain(string name, object asmPathOrBytes, bool isScript, string[] args = null, string config = null)
		{
			bool doCallback = asmPathOrBytes is byte[] || !Keyb.IsCtrl; //TODO

			AppDomainSetup se = null;
			if(doCallback || config != null) {
				se = new AppDomainSetup();
				if(doCallback) {
					var t = typeof(_DomainManager);
					se.AppDomainManagerAssembly = t.Assembly.FullName;
					se.AppDomainManagerType = t.FullName;
				}
				if(config != null) se.ConfigurationFile = config; //default is AppDomain.CurrentDomain.SetupInformation.ConfigurationFile

				//r.LoaderOptimization = LoaderOptimization.MultiDomainHost, //instead use [LoaderOptimization] on Program.Main().
				//r.ApplicationBase= AppDomain.CurrentDomain.SetupInformation.ApplicationBase, //default
				//r.AppDomainInitializer
			}
			//Perf.Next();
			var ad = AppDomain.CreateDomain(name, null, se);
			//Perf.Next();
			//ad.FirstChanceException += Ad_FirstChanceException;
			ad.UnhandledException += _ad_UnhandledException; //works only for threads of that appdomain started by Thread.Start. Does not work for its primary thread, Task.Run threads and corrupted state exceptions.

			if(args == null) { //TODO
				Perf.Next('x');
				args = new string[] { Perf.StaticInst.Serialize() };
			}

			if(doCallback) {
				var ty = typeof(_ADRun);
				//ad.CreateInstance(ty.Assembly.FullName, ty.FullName);
				var v = ad.CreateInstanceAndUnwrap(ty.Assembly.FullName, ty.FullName) as _ADRun;
				v.Run(asmPathOrBytes, isScript, args);
				AppDomain.Unload(ad);
			} else {
				try {
					ad.ExecuteAssembly(asmPathOrBytes as string, args);
				}
				catch(Exception e) when(!(e is ThreadAbortException)) {
					//TODO: remove part of stack trace.
					//TODO: unmangle stack trace of script. Or don't use script.
					//TODO: for AuException we have: System.Runtime.Serialization.SerializationException: Type 'Au.Types.AuException' in assembly 'Au, Version=1.0.0.0, Culture=neutral, PublicKeyToken=112db45ebd62e36d' is not marked as serializable.
					Print(e);
				}
				finally {
					//Debug_.Print("unloading");
					AppDomain.Unload(ad);
					//Debug_.Print("unloaded");
				}
			}
		}

		private static void _ad_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			//Print("Ad_UnhandledException:");
			Print(e.ExceptionObject);
		}

		//private static void _ad_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		//{
		//	Print("Ad_FirstChanceException");
		//}

		class _ADRun :MarshalByRefObject
		{
			public void Run(object asmPathOrBytes, bool isScript, string[] args)
			{
				_Run(asmPathOrBytes, isScript, args, true);
			}
		}

		//TODO: remove inMemoryAssembly-related stuff if unused.
		//TODO: remove parameter isScript from all functions in this file if not used.

		[HandleProcessCorruptedStateExceptions]
		static void _Run(object asmPathOrBytes, bool isScript, string[] args, bool inAD)
		{
			Assembly a = null;
			if(asmPathOrBytes is string s) asmPathOrBytes = File.ReadAllBytes(s); //avoid locking. Also 15% faster. AppDomainSetup.ShadowCopyFiles cannot be used here.
			a = Assembly.Load(asmPathOrBytes as byte[]);
			asmPathOrBytes = null;

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
				} else if(isScript && args != null) {
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
				Print(e);
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
		/// <param name="asmPathOrBytes">String (path) or byte[] (assembly in memory).</param>
		/// <param name="isScript"></param>
		/// <param name="args">To pass to Main.</param>
		internal static void RunHere(object asmPathOrBytes, bool isScript, string[] args = null)
		{
			_Run(asmPathOrBytes, isScript, args, false);
		}
	}
}
