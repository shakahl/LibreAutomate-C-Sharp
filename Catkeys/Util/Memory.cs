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
using Util = Catkeys.Util;

namespace Catkeys.Util
{
	/// <summary>
	/// Memory that can be used by multiple processes and app domains.
	/// Faster and more raw/unsafe than System.IO.MemoryMappedFiles.MemoryMappedFile.
	/// </summary>
	//[DebuggerStepThrough]
	public unsafe static class SharedMemory
	{
		/// <summary>
		/// Creates named shared memory of specified size. If already exists, just gets its address.
		/// Returns shared memory address in this process.
		/// Calls API <msdn>CreateFileMapping</msdn> and API <msdn>MapViewOfFile</msdn>.
		/// </summary>
		/// <param name="name">Shared memory name. Case-insensitive.</param>
		/// <param name="size">Shared memory size. The function uses it only when creating the memory; it does not check whether the size argument matches the size of existing memory.</param>
		/// <param name="allowLIL">Allow UAC low-integrity-level processes to access the shared memory.</param>
		/// <exception cref="Win32Exception">The API failed.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive at least until this process ends. Other processes can keep the memory alive even after that.
		/// There is no Close function to close the native shared memory object handle. The OS closes it when this process ends.
		/// </remarks>
		public static void* CreateOrGet(string name, uint size, bool allowLIL = false)
		{
			lock("AF2liKVWtEej+lRYCx0scQ") {
				string interDomainVarName = "AF2liKVWtEej+lRYCx0scQ" + name.ToLower_();
				if(!InterDomain.GetVariable(name, out IntPtr t)) {
					var sa = new _SecurityAttributes();
					var hm = Api.CreateFileMapping((IntPtr)(~0), (allowLIL && sa.Create()) ? &sa.sa : null, Api.PAGE_READWRITE, 0, size, name);
					if(hm == Zero) throw new Win32Exception();
					//bool opened = Native.GetError() == Api.ERROR_ALREADY_EXISTS;
					sa.Dispose();
					t = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
					if(t == Zero) { var e = new Win32Exception(); Api.CloseHandle(hm); throw e; }
					InterDomain.SetVariable(name, t);
				}
				return (void*)t;
			}
		}

		struct _SecurityAttributes
		{
			internal Api.SECURITY_ATTRIBUTES sa;

			internal bool Create()
			{
				sa.nLength = (uint)sizeof(Api.SECURITY_ATTRIBUTES);
				return Api.ConvertStringSecurityDescriptorToSecurityDescriptor("D:NO_ACCESS_CONTROLS:(ML;;NW;;;LW)", 1, out sa.lpSecurityDescriptor);
			}

			internal void Dispose()
			{
				if(sa.lpSecurityDescriptor != null) Api.LocalFree(sa.lpSecurityDescriptor);
			}
		}
	}

	/// <summary>
	/// Memory shared by all appdomains and by other related processes.
	/// </summary>
	[DebuggerStepThrough]
	unsafe struct LibSharedMemory
	{
		#region variables used by our library classes
		//Declare variables used by our library classes.
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		internal Perf.Inst perf;

		#endregion

		/// <summary>
		/// Shared memory size.
		/// </summary>
		internal const int Size = 0x10000;

		/// <summary>
		/// Creates or opens shared memory on demand in a thread-safe and process-safe way.
		/// </summary>
		static LibSharedMemory* _sm = (LibSharedMemory*)SharedMemory.CreateOrGet("Catkeys_SM_0x10000", Size);

		/// <summary>
		/// Gets pointer to the shared memory.
		/// </summary>
		public static LibSharedMemory* Ptr { get => _sm; }
	}

	/// <summary>
	/// Memory shared by all appdomains of current process.
	/// Size 0x10000 (64 KB). Initially zero.
	/// </summary>
	/// <remarks>
	/// When need to prevent simultaneous access of the memory by multiple threads, use <c>lock("uniqueString"){...}</c> .
	/// It locks in all appdomains, because literal strings are interned, ie shared by all appdomains.
	/// Using some other object with 'lock' would lock only in that appdomain.
	/// However use this only in single module, because ngened modules have own interned strings.
	/// </remarks>
	[DebuggerStepThrough]
	unsafe struct LibProcessMemory
	{
		//Api.RTL_CRITICAL_SECTION _cs; //slower than SRW but not much. Initialization speed not tested.
		//Api.RTL_SRWLOCK _lock; //2 times slower than C# lock, but we need this because C# lock is appdomain-local

		#region variables used by our library classes
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		//public int test;
		internal LibWorkarounds.ProcessVariables workarounds;
		internal ThreadPoolSTA.ProcessVariables threadPool;
		internal String_.ProcessVariables str;

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
#if true
		static LibProcessMemory()
		{
			Ptr = (LibProcessMemory*)InterDomain.GetVariable("Catkeys_LibProcessMemory", () => Api.VirtualAlloc(Zero, Size));
		}
		//This is slower (especially if using InterDomain first time in domain) but not so bizarre as with window class. And less code.
#else
		static LibProcessMemory()
		{
			string name = "Catkeys_LibMem";

			var x = new Api.WNDCLASSEX(); x.cbSize = Api.SizeOf(x);
			if(0 == Api.GetClassInfoEx(Zero, name, ref x)) {
				x.lpfnWndProc = Api.VirtualAlloc(Zero, Size); //much faster when need to zero memory
				if(x.lpfnWndProc == Zero) throw new OutOfMemoryException(name);

				x.style = Api.CS_GLOBALCLASS;
				x.lpszClassName = Marshal.StringToHGlobalUni(name);
				bool ok = 0 != Api.RegisterClassEx(ref x);

				if(ok) {
					//Api.InitializeSRWLock(&((LibProcessMemory*)x.lpfnWndProc)->_lock);
					//Api.InitializeCriticalSection(&((LibProcessMemory*)x.lpfnWndProc)->_cs);
				} else {
					if(0 == Api.GetClassInfoEx(Zero, name, ref x)) throw new OutOfMemoryException(name);
				}

				Marshal.FreeHGlobal(x.lpszClassName);
			}
			Ptr = (LibProcessMemory*)x.lpfnWndProc;
		}
#endif
	}
}
