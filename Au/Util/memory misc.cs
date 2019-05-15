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
	/// Memory that can be used by multiple processes and app domains.
	/// Faster and more raw/unsafe than System.IO.MemoryMappedFiles.MemoryMappedFile.
	/// </summary>
	/// <seealso cref="Wnd.More.CopyDataStruct"/>
	//[DebuggerStepThrough]
	public unsafe static class SharedMemory
	{
		/// <summary>
		/// Creates named shared memory of specified size. Opens if already exists.
		/// Returns shared memory address in this process.
		/// Calls API <msdn>CreateFileMapping</msdn> and API <msdn>MapViewOfFile</msdn>.
		/// </summary>
		/// <param name="name">Shared memory name. Case-insensitive.</param>
		/// <param name="size">Shared memory size. Ignored if the shared memory already exists.</param>
		/// <param name="created">Receives true if created and not opened.</param>
		/// <exception cref="AException">The API failed.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive at least until this process ends. Other processes can keep the memory alive even after that.
		/// There is no Close function to close the native shared memory object handle. The OS closes it when this process ends.
		/// </remarks>
		public static void* CreateOrGet(string name, uint size, out bool created)
		{
			created = false;
			lock("AF2liKVWtEej+lRYCx0scQ") {
				string interDomainVarName = "AF2liKVWtEej+lRYCx0scQ" + name.Lower();
				if(!InterDomainVariables.GetVariable(name, out IntPtr t)) {
					var hm = Api.CreateFileMapping((IntPtr)~0, Api.SECURITY_ATTRIBUTES.ForLowIL, Api.PAGE_READWRITE, 0, size, name);
					if(hm.Is0) goto ge;
					created = WinError.Code != Api.ERROR_ALREADY_EXISTS;
					t = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
					if(t == default) { hm.Dispose(); goto ge; }
					InterDomainVariables.SetVariable(name, t);
				}
				return (void*)t;
			}
			ge:
			throw new AException(0, "*open shared memory");
		}
	}

	/// <summary>
	/// Memory shared by all processes using this library.
	/// </summary>
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Sequential, Size = 0x10000)]
	unsafe struct LibSharedMemory
	{
		#region variables used by our library classes
		//Declare variables used by our library classes.
		//Be careful:
		//1. Some type sizes are different in 32 and 64 bit process, eg IntPtr.
		//	Solution: Use long and cast to IntPtr etc.
		//2. The memory may be used by processes that use different library versions.
		//	Solution: In new library versions don't change struct sizes and old members.
		//		Maybe reserve some space for future members. If need more, add new struct.
		//		Use eg [StructLayout(LayoutKind.Sequential, Size = 16)].

		//reserve 16 for some header, eg shared memory version.
		[StructLayout(LayoutKind.Sequential, Size = 16)] struct _Header { }
		_Header _h;

		internal OutputServer.LibSharedMemoryData outp;
		internal Triggers.ActionTriggers.LibSharedMemoryData triggers;
		internal WinHook.LibSharedMemoryData winHook;

		#endregion

		/// <summary>
		/// Shared memory size.
		/// </summary>
		internal const int Size = 0x10000;

		/// <summary>
		/// Creates or opens shared memory on demand in a thread-safe and process-safe way.
		/// </summary>
		static LibSharedMemory* _sm;

		static LibSharedMemory()
		{
			_sm = (LibSharedMemory*)SharedMemory.CreateOrGet("Au_SM_0x10000", Size, out var created);
#if DEBUG
			if(created) { //must be zero-inited, it's documented
				int* p = (int*)_sm;
				int i, n = 1000;
				for(i = 0; i < n; i++) if(p[i] != 0) break;
				Debug.Assert(i == n);
			}
#endif
		}

		/// <summary>
		/// Gets pointer to the shared memory.
		/// </summary>
		public static LibSharedMemory* Ptr => _sm;
	}

	/// <summary>
	/// Memory shared by all appdomains of current process.
	/// Size 0x10000 (64 KB). Initially zero.
	/// </summary>
	/// <remarks>
	/// When need to prevent simultaneous access of the memory by multiple threads, use <c>lock("uniqueString"){...}</c> .
	/// It locks in all appdomains, because literal strings are interned, ie shared by all appdomains.
	/// Using some other object with 'lock' would lock only in that appdomain. Maybe except Type.
	/// Use this only in single module, because ngened modules have own interned strings.
	/// </remarks>
	[DebuggerStepThrough]
	unsafe struct LibProcessMemory
	{
		#region variables used by our library classes
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		//public int test;
		internal LibTables.ProcessVariables tables; //sizeof = 200
		internal LibWorkarounds.ProcessVariables workarounds;
		internal ThreadPoolSTA.ProcessVariables threadPool;
		internal ATRole taskRole;
		//internal AThread.ProcessVariables thread_;
		//internal Perf.Inst perf;

		#endregion

		/// <summary>
		/// Gets pointer to the memory.
		/// </summary>
		internal static LibProcessMemory* Ptr { get; }

		/// <summary>
		/// Memory size.
		/// </summary>
		internal const int Size = 0x10000;

		[DebuggerStepThrough]
		static LibProcessMemory()
		{
#if true
			Ptr = (LibProcessMemory*)InterDomainVariables.GetVariable("Au_LibProcessMemory", () => Api.VirtualAlloc(default, Size));
#else
			//SHOULDDO: try AuCpp dll. Then can store tables in dll shared memory; then we'll have max 2 copies of it - in 64-bit dll and in 32-bit dll. But slower startup.
			//	Also try shared memory for tables.
#endif
		}
	}

	/// <summary>
	/// <see cref="GC"/> extensions.
	/// </summary>
	public static class AGC
	{
		static ConditionalWeakTable<object, _Remover> s_table = new ConditionalWeakTable<object, _Remover>();

		/// <summary>
		/// Calls <see cref="GC.AddMemoryPressure"/>. Later, when object <i>obj</i> is garbage-collected, will call <see cref="GC.RemoveMemoryPressure"/>.
		/// </summary>
		/// <param name="obj">An object of any type.</param>
		/// <param name="size">Unmanaged memory size. It is passed to <b>GC.AddMemoryPressure</b> and <b>GC.RemoveMemoryPressure</b>.</param>
		public static void AddObjectMemoryPressure(object obj, int size)
		{
			GC.AddMemoryPressure(size);
			s_table.Add(obj, new _Remover(size));
		}

		class _Remover
		{
			int _size;

			public _Remover(int size)
			{
				_size = size;
			}

			~_Remover()
			{
				//Print("removed " + _size);
				GC.RemoveMemoryPressure(_size);
			}
		}
	}
}
