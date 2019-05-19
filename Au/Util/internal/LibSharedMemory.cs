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
using static Au.AStatic;

namespace Au.Util
{
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

		internal AOutputServer.LibSharedMemoryData outp;
		internal Triggers.ActionTriggers.LibSharedMemoryData triggers;
		internal AHookWin.LibSharedMemoryData winHook;

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
			_sm = (LibSharedMemory*)ASharedMemory.CreateOrGet("Au_SM_0x10000", Size, out var created);
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
}
