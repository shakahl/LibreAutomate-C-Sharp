using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys.Util
{
	/// <summary>
	/// Extends the .NET AppDomain class.
	/// </summary>
	public static class AppDomain_
	{
		//Don't use tuples, at least with public functions. Problems:
		//	This and caller project need to install System.ValueTuple from NuGet.
		//	This dll and caller must be accompanied with System.ValueTuple.dll.
		//	SHFB cannot resolve it.
#if USE_TUPLE
		/// <summary>
		/// Gets default app domain.
		/// Returns tuple containing AppDomain domain and bool isCurrentDomain which is true if the domain is current (caller's) domain.
		/// </summary>
		public static (AppDomain domain, bool isCurrentDomain) GetDefaultDomain()
		{
			if(_defaultAppDomain == null) {
				var d = AppDomain.CurrentDomain;
				if(d.IsDefaultAppDomain()) {
					_defaultAppDomain = d;
					_defaultAppDomainIsCurrent = true;
				} else {
					d = AppDomain.CurrentDomain.GetData("Catkeys_DefaultDomain") as AppDomain;
					if(d != null) {
						_defaultAppDomain = d;
					} else { //current domain created not by Catkeys
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
			return (_defaultAppDomain, _defaultAppDomainIsCurrent);
		}
		static AppDomain _defaultAppDomain; static bool _defaultAppDomainIsCurrent;
#else
		/// <summary>
		/// Gets default app domain.
		/// </summary>
		/// <param name="isCurrentDomain">Receives true if called from default app domain.</param>
		public static AppDomain GetDefaultDomain(out bool isCurrentDomain)
		{
			if(_defaultAppDomain == null) {
				var d = AppDomain.CurrentDomain;
				if(d.IsDefaultAppDomain()) {
					_defaultAppDomain = d;
					_defaultAppDomainIsCurrent = true;
				} else {
					d = AppDomain.CurrentDomain.GetData("Catkeys_DefaultDomain") as AppDomain;
					if(d != null) {
						_defaultAppDomain = d;
					} else { //current domain created not by Catkeys
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
		static AppDomain _defaultAppDomain; static bool _defaultAppDomainIsCurrent;

		/// <summary>
		/// Gets default app domain.
		/// </summary>
		public static AppDomain GetDefaultDomain()
		{
			return GetDefaultDomain(out bool isThisDef);
		}
#endif
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
		/// Occurs when current app domain exits.
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

		/// <summary>
		/// Gets the entry assembly of current appdomain.
		/// Normally instead can be used <see cref="Assembly.GetEntryAssembly"/>, but it fails if appdomain launched through <see cref="AppDomain.DoCallBack">AppDomain.DoCallBack</see>.
		/// </summary>
		public static Assembly EntryAssembly
		{
			get
			{
				if(_appdomainAssembly == null) {
					var asm = Assembly.GetEntryAssembly(); //fails if this domain launched through DoCallBack
					if(asm == null) asm = AppDomain.CurrentDomain.GetAssemblies()[1]; //[0] is mscorlib, 1 should be our assembly
					_appdomainAssembly = asm;
				}
				return _appdomainAssembly;
			}
		}
		static Assembly _appdomainAssembly;

	}
}
