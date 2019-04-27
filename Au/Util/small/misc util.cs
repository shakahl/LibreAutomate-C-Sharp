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
	/// Static functions to open a help topic etc.
	/// </summary>
	public static class Help
	{
		/// <summary>
		/// Opens file "Au Help.chm" and a help topic in it.
		/// The file must be in <see cref="Folders.ThisApp"/>.
		/// </summary>
		/// <param name="topic">Topic file name, like "M_Au_Acc_Find" or "0248143b-a0dd-4fa1-84f9-76831db6714a".</param>
		public static void AuHelp(string topic) //TODO: open URL in DocFX-created website
		{
			var s = Folders.ThisAppBS + "Help/Au Help.chm::/html/" + topic + ".htm";
			Api.HtmlHelp(Api.GetDesktopWindow(), s, 0, 0); //HH_DISPLAY_TOPIC
		}

	}







	//currently not used
	///// <summary>
	///// Extends <see cref="Marshal"/>.
	///// </summary>
	//public static class Marshal_
	//{
	//	//currently not used
	//	///// <summary>
	//	///// Increments the reference count of COM object's RCW (managed runtime callable wrapper).
	//	///// </summary>
	//	///// <param name="o">Managed COM object (RCW).</param>
	//	///// <remarks>
	//	///// This function is the opposite of <see cref="Marshal.ReleaseComObject"/>, which decrements the RCW reference count.
	//	///// Call this function when cloning a variable of a type that wraps a managed COM object and calls Marshal.ReleaseComObject when disposing. Without it, after disposing one of the variables, cannot call methods etc of other variable because the RCW then is invalid.
	//	///// This function does not increment the reference count of the native COM object.
	//	///// </remarks>
	//	//public static void AddRefComObject<T>(T o) where T: class
	//	//{
	//	//	//ugly, but .NET does not have a better method for it.

	//	//	var u = Marshal.GetIUnknownForObject(o); //gets native COM pointer and calls its AddRef
	//	//	var o2 = Marshal.GetObjectForIUnknown(u); //increments RCW ref count. Calls QueryInterface and Release of the native COM object.
	//	//											  //var o2 = Marshal.GetTypedObjectForIUnknown(u, typeof(T)); //works too, but MSDN info is unclear. In both cases ToString says it's System.__ComObject.
	//	//	Marshal.Release(u); //because GetIUnknownForObject called AddRef
	//	//	Debug.Assert(ReferenceEquals(o, o2));
	//	//}

	//	//returns new RCW
	//	//internal static object GetUniqueComObject<T>(T o) where T: class
	//	//{
	//	//	var u = Marshal.GetIUnknownForObject(o);
	//	//	var o2 = Marshal.GetUniqueObjectForIUnknown(u); //many QI etc
	//	//	Marshal.Release(u);
	//	//	Debug.Assert(!ReferenceEquals(o, o2));
	//	//	return u2;
	//	//}

	//	//currently not used
	//	///// <summary>
	//	///// Gets another COM interface through <msdn>IUnknown.QueryInterface</msdn>.
	//	///// Returns false if the COM object (iunkFrom) does not support the requested interface or if fails.
	//	///// </summary>
	//	///// <typeparam name="T">IntPtr or an IntPtr-based type. Must have size of IntPtr (exception if not).</typeparam>
	//	///// <param name="iunkFrom">COM object as IUnknown.</param>
	//	///// <param name="iTo">Receives the requested COM interface pointer.</param>
	//	///// <param name="riid">Interface GUID.</param>
	//	//internal static unsafe bool QueryInterface<T>(IntPtr iunkFrom, out T iTo, Guid riid) where T : unmanaged
	//	//{
	//	//	if(sizeof(T) != IntPtr.Size) throw new ArgumentException();
	//	//	iTo = default;
	//	//	if(0 != Marshal.QueryInterface(iunkFrom, ref riid, out IntPtr ip) || ip == default) return false;
	//	//	var v=*(T*)&ip; iTo=v;
	//	//	return true;
	//	//}

	//	//currently not used
	//	///// <summary>
	//	///// Gets another COM interface through <msdn>IServiceProvider.QueryService</msdn>.
	//	///// Returns false if the COM object (iunkFrom) does not support IServiceProvider or the requested interface or if fails.
	//	///// </summary>
	//	///// <typeparam name="T">IntPtr or an IntPtr-based type. Must have size of IntPtr (exception if not).</typeparam>
	//	///// <param name="iunkFrom">COM object as IUnknown.</param>
	//	///// <param name="iTo">Receives the requested COM interface pointer.</param>
	//	///// <param name="guidService">Service GUID. If it is the same as riid, you can use other overload.</param>
	//	///// <param name="riid">Interface GUID.</param>
	//	//internal static unsafe bool QueryService<T>(IntPtr iunkFrom, out T iTo, in Guid guidService, in Guid riid) where T: unmanaged
	//	//{
	//	//	if(sizeof(T) != IntPtr.Size) throw new ArgumentException();
	//	//	if(iunkFrom==default) throw new ArgumentNullException();
	//	//	iTo = default;
	//	//	if(0 != Api.IUnknown_QueryService(iunkFrom, guidService, riid, out IntPtr ip) || ip==default) return false;
	//	//	var v=*(T*)&ip; iTo=v;
	//	//	return true;
	//	//}

	//	//currently not used
	//	///// <summary>
	//	///// This overload calls <see cref="QueryService{T}(IntPtr, out T, in Guid, in Guid)"/> with guidService = riid.
	//	///// </summary>
	//	//internal static unsafe bool QueryService<T>(IntPtr iunkFrom, out T iTo, in Guid riid) where T : struct
	//	//{
	//	//	return QueryService(iunkFrom, out iTo, riid, riid);
	//	//}
	//}


	///// <summary>
	///// Gets managed run-time size of type T. Works with any type.
	///// Unlike sizeof, can be used in generic classes too.
	///// Unlike Marshal.SizeOf, gets managed type size (eg 1 for bool), not native type size (eg 4 for bool).
	///// Example: <c>Print(Au.Util.TypeSize&lt;T&gt;.Size);</c>.
	///// </summary>
	///// <typeparam name="T"></typeparam>
	//public static class TypeSize<T>
	//{
	//	/// <summary>
	//	/// Gets T type size.
	//	/// </summary>
	//	public readonly static int Size;
	//	static TypeSize() { Size = Misc.LibGetTypeSize(typeof(T)); }

	//	//speed: quite fast, especially when ngened. When using this generic class, LibGetTypeSize is called once for each type.
	//}



	//rejected: does not work with Key/Text/Paste. And too dirty. Somehow similar code worked in QM2.
	///// <summary>
	///// Switches to another thread and sleeps for the remainder of this time slice of the thread scheduler.
	///// Unlike Sleep(0) etc, works on multi-processor computers too.
	///// </summary>
	///// <remarks>
	///// Causes this thread to yield execution to another thread that is ready to run on ANY logical processor.
	///// If the remainder of this time slice is less than 400 mcs, retries to sleep during the next time slice.
	///// If there are no other ready threads, does not sleep. Then the speed is about 100 mcs.
	///// </remarks>
	//internal struct LibThreadSwitcher :IDisposable
	//{
	//	ulong _processAffinity, _threadAffinity;

	//	public void Dispose()
	//	{
	//		if(_threadAffinity != 0) {
	//			//Print(_threadAffinity);
	//			Api.SetThreadAffinityMask(Api.GetCurrentThread(), _threadAffinity);
	//		}
	//	}

	//	static uint s_nProc = Api.GetMaximumProcessorCount(Api.ALL_PROCESSOR_GROUPS);

	//	/// <summary>
	//	/// Switches to another thread and sleeps for the remainder of this time slice of the thread scheduler.
	//	/// Returns false if fails. Fails if there are more than 64 logical processors.
	//	/// </summary>
	//	public bool Switch()
	//	{
	//		const int c_nTry = 2;
	//		uint nProc = s_nProc;
	//		long t0 = Time.PerfMicroseconds;
	//		bool switched, retry = false;
	//		g2:
	//		switched = false;
	//		if(nProc == 1) {
	//			for(int i = 0; i < c_nTry; i++) {
	//				if(switched=Api.SwitchToThread()) break;
	//			}
	//		} else {
	//			if(nProc == 0 || nProc > 64) return false;
	//			if(_processAffinity == 0) {
	//				if(!Api.GetProcessAffinityMask(Api.GetCurrentProcess(), out var amProcess, out var amSystem)) return false;
	//				ulong processAffinity = amProcess, systemAffinity = amSystem;
	//				//Print((int)nProc, processAffinity);
	//				for(int i = 0; i < nProc; i++) {
	//					var bit = 1UL << i;
	//					if((bit & processAffinity) == 0 && (bit & systemAffinity) != 0) return false; //are all bits 1?
	//				}
	//				_processAffinity = processAffinity;
	//			}

	//			var ht = Api.GetCurrentThread();
	//			for(int i = 0; i < c_nTry; i++) {
	//				for(int j = 0; j < nProc; j++) {
	//					var bit = 1UL << j;
	//					if((bit & _processAffinity) == 0) continue; //is this bit in system affinity?
	//					var tam = Api.SetThreadAffinityMask(ht, bit);
	//					if(tam == 0) return false;
	//					if(_threadAffinity == 0) _threadAffinity = tam;
	//					if(switched=Api.SwitchToThread()) {
	//						//Print(i, j);
	//						goto g1;
	//					}
	//				}
	//			}
	//		}
	//		g1:
	//		if(switched) {
	//			long t1 = Time.PerfMicroseconds;
	//			if(t1 - t0 < 400) {
	//				//Print("-->", t1 - t0, retry);
	//				if(!retry) { retry = true; goto g2; }
	//				Time.Sleep(1);
	//			}
	//		}
	//		return true;
	//	}

	//[DllImport("kernel32.dll")]
	//internal static extern bool SwitchToThread();

	//internal const ushort ALL_PROCESSOR_GROUPS = 0xFFFF;

	//[DllImport("kernel32.dll", SetLastError = true)]
	//internal static extern uint GetMaximumProcessorCount(ushort GroupNumber);

	//[DllImport("kernel32.dll", SetLastError = true)]
	//internal static extern bool GetProcessAffinityMask(IntPtr hProcess, out LPARAM lpProcessAffinityMask, out LPARAM lpSystemAffinityMask);

	//[DllImport("kernel32.dll", SetLastError = true)]
	//internal static extern LPARAM SetThreadAffinityMask(IntPtr hThread, LPARAM dwThreadAffinityMask);
	//}

}
