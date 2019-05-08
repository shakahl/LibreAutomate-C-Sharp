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
	public static class AAppDomain
	{
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
			if(e is UnhandledExceptionEventArgs u && !u.IsTerminating) return;
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
