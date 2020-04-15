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

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Memory shared by all processes using this library.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 0x10000)]
	unsafe struct SharedMemory_
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

		internal AOutputServer.SharedMemoryData_ outp;
		internal Triggers.ActionTriggers.SharedMemoryData_ triggers;
		internal AHookWin.SharedMemoryData_ winHook;

		#endregion

		/// <summary>
		/// Shared memory size.
		/// </summary>
		internal const int Size = 0x10000;

		static SharedMemory_()
		{
			Ptr = (SharedMemory_*)CreateOrGet("Au_SM_0x10000", Size, out var created);
#if DEBUG
			if(created) { //must be zero-inited, it's documented
				int* p = (int*)Ptr;
				int i, n = 1000;
				for(i = 0; i < n; i++) if(p[i] != 0) break;
				Debug.Assert(i == n);
			}
#endif
		}

		/// <summary>
		/// Gets pointer to the shared memory.
		/// </summary>
		public static readonly SharedMemory_* Ptr;

		/// <summary>
		/// Creates named shared memory of specified size. Opens if already exists.
		/// Returns shared memory address in this process.
		/// Calls API <msdn>CreateFileMapping</msdn> and API <msdn>MapViewOfFile</msdn>.
		/// </summary>
		/// <param name="name">Shared memory name. Case-insensitive.</param>
		/// <param name="size">Shared memory size. Ignored if the shared memory already exists.</param>
		/// <param name="created">Receives true if created and not opened.</param>
		/// <exception cref="AuException">The API failed.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive at least until this process ends. Other processes can keep the memory alive even after that.
		/// There is no Close function to close the native shared memory object handle. The OS closes it when this process ends.
		/// </remarks>
		public static void* CreateOrGet(string name, uint size, out bool created)
		{
			Debug.Assert(AppDomain.CurrentDomain.IsDefaultAppDomain());

			created = false;
			var hm = Api.CreateFileMapping((IntPtr)~0, Api.SECURITY_ATTRIBUTES.ForLowIL, Api.PAGE_READWRITE, 0, size, name);
			if(!hm.Is0) {
				created = ALastError.Code != Api.ERROR_ALREADY_EXISTS;
				var r = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
				if(r != default) return (void*)r;
				hm.Dispose();
			}
			throw new AuException(0, "*open shared memory");
		}
	}
}
