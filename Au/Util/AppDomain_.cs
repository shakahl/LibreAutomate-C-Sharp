//#define DOCALLBACK
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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Extends <see cref="AppDomain"/>.
	/// </summary>
	public static class AppDomain_
	{
		//TODO

		class _ADRun :MarshalByRefObject
		{
			public void Run(object data)
			{
				_ADCallback(data);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		internal static void TestRunDomain(string name, object data, string config)
		{
			bool doCallback = data is byte[] || Keyb.IsCtrl;

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
			Perf.Next();
			//ad.FirstChanceException += Ad_FirstChanceException;
			ad.UnhandledException += _ad_UnhandledException; //works only for threads of that appdomain started by Thread.Start. Does not work for its primary thread, Task.Run threads and corrupted state exceptions.
			Perf.Next();

			if(doCallback) {
#if DOCALLBACK
				ad.SetData("Au.asm", data);
				//ad.SetData("Au.args", ...); //_TODO
				ad.DoCallBack(_ADCallback);
#else
				var ty = typeof(_ADRun);
				//ad.CreateInstance(ty.Assembly.FullName, ty.FullName);
				var v = ad.CreateInstanceAndUnwrap(ty.Assembly.FullName, ty.FullName) as _ADRun;
				v.Run(data);
#endif
				AppDomain.Unload(ad);
			} else {
				string[] args = null; //_TODO
				Perf.Next();
				try {
					ad.ExecuteAssembly(data as string, args);
				}
				catch(Exception e) when(!(e is ThreadAbortException)) {
					//TODO: remove part of stack trace.
					//TODO: unmangle stack trace of script. Or don't use script.
					//TODO: for AuException we have: System.Runtime.Serialization.SerializationException: Type 'Au.Types.AuException' in assembly 'Au, Version=1.0.0.0, Culture=neutral, PublicKeyToken=112db45ebd62e36d' is not marked as serializable.
					Print(e);
				}
				finally {
					//Print("unloading");
					AppDomain.Unload(ad);
					Print("unloaded");
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

		internal static EventWaitHandle s_event = new EventWaitHandle(false, EventResetMode.AutoReset, "Au.TestEv"); //TODO: remove, it is only for speed testing

		[HandleProcessCorruptedStateExceptions]
		static void _ADCallback(
#if !DOCALLBACK
			object data
#endif
			) //TODO: remove this and other inMemoryAssembly-related stuff if unused
		{
			//s_event.Set();

			//TODO: why loads CodeAnalysis and Immutable assemblies? And why ExecuteAssembly loads Editor?
			//	Try to move everything to Au. Maybe will be faster too.
			//System.Windows.Forms.MessageBox.Show("");

			var ad = AppDomain.CurrentDomain;
			Assembly a = null;
#if DOCALLBACK
			switch(ad.GetData("Au.asm")) {
			case byte[] b:
				a = Assembly.Load(b);
				ad.SetData("Au.asm", null);
				break;
			case string s:
				//a = Assembly.LoadFile(s);
				a = Assembly.LoadFrom(s); //ExecuteAssembly calls this. Same speed.
				break;
			}
#else
			if(data is string s) data = File.ReadAllBytes(s); //avoid locking. Also 15% faster. AppDomainSetup.ShadowCopyFiles cannot be used here.
			a = Assembly.Load(data as byte[]);
			data = null;
#endif

			if(ad.DomainManager is _DomainManager dm) dm.LibEntryAssembly = a;
			//Print(Assembly.GetEntryAssembly());

			try {
				var entryPoint = a.EntryPoint;
				if(entryPoint.GetParameters().Length == 0) {
					entryPoint.Invoke(null, null);
				} else {
					var args = ad.GetData("Au.args") as string[];
					entryPoint.Invoke(null, new object[] { args ?? Array.Empty<string>() });

					//var del = entryPoint.CreateDelegate<Action<string[]>>();
					//del(args ?? Array.Empty<string>());
					////With delegate, exceptions are thrown differently: without TargetInvocationException, but stack includes this (caller) method.
				}
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
		/// Gets default appdomain.
		/// </summary>
		/// <param name="isCurrentDomain">Receives true if called from default appdomain.</param>
		public static AppDomain GetDefaultDomain(out bool isCurrentDomain)
		{
			if(_defaultAppDomain == null) {
				var d = AppDomain.CurrentDomain;
				if(d.IsDefaultAppDomain()) {
					_defaultAppDomain = d;
					_defaultAppDomainIsCurrent = true;
				} else {
					d = AppDomain.CurrentDomain.GetData("Au_DefaultDomain") as AppDomain;
					if(d != null) {
						_defaultAppDomain = d;
					} else { //current domain created not by Au
						ICorRuntimeHost host = new CorRuntimeHost() as ICorRuntimeHost;
						//Perf.Next(); //speed:  289  3251  (3542) ngened, else 4ms. Why GetDefaultDomain so slow?
						object o = null; host.GetDefaultDomain(out o);
						_defaultAppDomain = o as AppDomain;

						//this is slower
						//IntPtr hEnum;
						//if(0 != host.EnumDomains(out hEnum)) return null;
						//if(0 != host.NextDomain(hEnum, out defaultAppDomain)) return null;
						//host.CloseEnum(hEnum);
						//_defaultAppDomain = defaultAppDomain as AppDomain;
					}
				}
			}
			isCurrentDomain = _defaultAppDomainIsCurrent;
			return _defaultAppDomain;
		}
		static AppDomain _defaultAppDomain;
		static bool _defaultAppDomainIsCurrent;

		/// <summary>
		/// Gets default appdomain.
		/// </summary>
		public static AppDomain GetDefaultDomain()
		{
			return GetDefaultDomain(out bool isThisDef);
		}

		[ComImport, Guid("CB2F6722-AB3A-11d2-9C40-00C04FA30A3E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		unsafe interface ICorRuntimeHost
		{
			[PreserveSig] int CreateLogicalThreadState();
			[PreserveSig] int DeleteLogicalThreadState();
			[PreserveSig] int SwitchInLogicalThreadState(ref uint pFiberCookie);
			[PreserveSig] int SwitchOutLogicalThreadState(out uint* pFiberCookie);
			[PreserveSig] int LocksHeldByLogicalThread(out uint pCount);
			[PreserveSig] int MapFile(IntPtr hFile, out IntPtr hMapAddress);
			[PreserveSig] //int GetConfiguration(out ICorConfiguration pConfiguration);
			int GetConfiguration(IntPtr pConfiguration);
			[PreserveSig] int Start();
			[PreserveSig] int Stop();
			[PreserveSig] int CreateDomain([MarshalAs(UnmanagedType.LPWStr)] string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] Object pIdentityArray, [MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig] int GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig] int EnumDomains(out IntPtr hEnum);
			[PreserveSig] int NextDomain(IntPtr hEnum, [MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig] int CloseEnum(IntPtr hEnum);
			[PreserveSig] int CreateDomainEx([MarshalAs(UnmanagedType.LPWStr)] string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] Object pSetup, [MarshalAs(UnmanagedType.IUnknown)] Object pEvidence, [MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
			[PreserveSig] int CreateDomainSetup([MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomainSetup);
			[PreserveSig] int CreateEvidence([MarshalAs(UnmanagedType.IUnknown)] out Object pEvidence);
			[PreserveSig] int UnloadDomain([MarshalAs(UnmanagedType.IUnknown)] Object pAppDomain);
			[PreserveSig] int CurrentDomain([MarshalAs(UnmanagedType.IUnknown)] out Object pAppDomain);
		}

		[ComImport, Guid("CB2F6723-AB3A-11d2-9C40-00C04FA30A3E"), ClassInterface(ClassInterfaceType.None)]
		class CorRuntimeHost { }

		/// <summary>
		/// Occurs when current appdomain exits.
		/// </summary>
		/// <remarks>
		/// The event handler is called when one of these AppDomain events occur, with their parameters:
		/// <see cref="AppDomain.ProcessExit"/> (in default domain);
		/// <see cref="AppDomain.DomainUnload"/> (in non-default domain);
		/// <see cref="AppDomain.UnhandledException"/>.
		/// The event handler is called before static object finalizers.
		/// </remarks>
		public static event EventHandler Exit
		{
			add
			{
				if(!_subscribedADE) {
					lock("AVCyoRcQCkSl+3W8ZTi5oA") {
						if(!_subscribedADE) {
							var d = AppDomain.CurrentDomain;
							if(d.IsDefaultAppDomain()) d.ProcessExit += _CurrentDomain_DomainExit;
							else d.DomainUnload += _CurrentDomain_DomainExit;
							d.UnhandledException += _CurrentDomain_DomainExit;
							_subscribedADE = true;
							//We subscribe to UnhandledException because ProcessExit is missing on exception.
							//If non-default domain, on exception normally we receive DomainExit and not UnhandledException, because default domain handles domain exceptions. But somebody may create domains without domain exception handling.
						}
					}
				}
				_eventADE += value;
			}
			remove
			{
				_eventADE -= value;
			}
		}
		static EventHandler _eventADE;
		static bool _subscribedADE;

		//[HandleProcessCorruptedStateExceptions, System.Security.SecurityCritical] ;;tried to enable this event for corrupted state exceptions, but does not work
		static void _CurrentDomain_DomainExit(object sender, EventArgs e)
		{
			var k = _eventADE;
			if(k != null) try { k(sender, e); } catch { }
		}

#if false //rejected. Use InterDomainVariables.DefaultDomainVariable; call it once and assign the reference to a static field.
		/// <summary>
		/// Through this property you call methods added to the LibObjectInDefaultDomain class.
		/// Can be called from any appdomain, but the method is actually executed in the default appdomain.
		/// </summary>
		internal static LibObjectInDefaultDomain LibCallInDefaultDomain
		{
			get
			{
				_Init();
				return _inDD;
			}
		}
		static LibObjectInDefaultDomain _inDD;

		//public //could be called from default appdomain. But it saves < 100 mcs. Now we always auto-init on demand.
		static void _Init()
		{
			if(_inDD == null) {
				var d = GetDefaultDomain(out bool isThisDomainDefault);
				_inDD = d.GetData("Au_CIDD\x5") as LibObjectInDefaultDomain;
				if(_inDD == null) {
					if(isThisDomainDefault) {
						_inDD = new LibObjectInDefaultDomain();
					} else {
						var t = typeof(LibObjectInDefaultDomain);
						_inDD = (LibObjectInDefaultDomain)d.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName);
					}
					d.SetData("Au_CIDD\x5", _inDD);
				}
			}
		}

		//This can be used to execute methods in default appdomain. Unlike with AppDomain.DoCallback, we can have parameters. Also slightly faster. Unlike with AppDomain.CreateInstanceAndUnwrap or Activator.CreateInstance, don't need to create a MarshalByRefObject-derived class for each such method.
		//When will need it, add methods in this class and call through CallInDefaultDomain.
		//note: be careful with the number and type of parameters, as it can make much slower. Eg out parameters make several times slower.
		internal sealed class LibObjectInDefaultDomain :MarshalByRefObject
		{
			//This is the fastest, almost like AppDomain.CreateInstanceAndUnwrap.
			//public object SetDefaultDomainVariable(string assemblyName, string typeName)
			//{
			//	return Activator.CreateInstance(assemblyName, typeName).Unwrap();
			//}
			//public object SetDefaultDomainVariable(Type t)
			//{
			//	return Activator.CreateInstance(t);
			//}
			//public ObjectHandle SetDefaultDomainVariable(string assemblyName, string typeName)
			//{
			//	return Activator.CreateInstance(assemblyName, typeName);
			//         }
			//public void SetDefaultDomainVariable<T>(string name) where T : MarshalByRefObject, new()
			//{
			//	var x = new T();
			//	SetVariable(name, x);
			//}
			//public T SetDefaultDomainVariable<T>(string name) where T : MarshalByRefObject, new()
			//{
			//	var x = new T();
			//	SetVariable(name, x);
			//	return x;
			//}
			//public object SetDefaultDomainVariable<T>(string name) where T : MarshalByRefObject, new()
			//{
			//	var x = new T();
			//	SetVariable(name, x);
			//	return x;
			//}
			//public void SetDefaultDomainVariable<T>(string name, out T value) where T : MarshalByRefObject, new()
			//{
			//	value = new T();
			//}
		}
#endif
	}
}
