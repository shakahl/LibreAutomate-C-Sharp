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
	/// Allocates memory from native heap of this process using heap API.
	/// Uses the common heap of this process, API <msdn>GetProcessHeap</msdn>.
	/// Usually slightly faster than Marshal class functions.
	/// </summary>
	public static unsafe class AMemory
	{
		static IntPtr _processHeap = Api.GetProcessHeap();

		/// <summary>
		/// Allocates new memory block and returns its address.
		/// Calls API <msdn>HeapAlloc</msdn>.
		/// </summary>
		/// <param name="size">Byte count.</param>
		/// <param name="zeroInit">Set all bytes = 0. If false (default), the memory is uninitialized, ie random byte values. Slower when true.</param>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		/// <remarks>The memory is unmanaged and will not be freed automatically. Always call <see cref="Free"/> when done or <see cref="ReAlloc"/> if need to resize it without losing data.</remarks>
		public static void* Alloc(LPARAM size, bool zeroInit = false)
		{
			void* r = Api.HeapAlloc(_processHeap, zeroInit ? 8u : 0u, size);
			if(r == null) throw new OutOfMemoryException();
			return r;

			//note: don't need GC.AddMemoryPressure.
			//	Native memory usually is used for temporary buffers etc and is soon released eg with try/finally.
			//	Marshal.AllocHGlobal does not do it too.
		}

		/// <summary>
		/// Reallocates a memory block to make it bigger or smaller.
		/// Returns its address, which in most cases is different than the old memory block address.
		/// Preserves data in Math.Min(old size, new size) bytes of old memory (copies from old memory if need).
		/// Calls API <msdn>HeapReAlloc</msdn> or <msdn>HeapAlloc</msdn>.
		/// </summary>
		/// <param name="mem">Old memory address. If null, allocates new memory like Alloc.</param>
		/// <param name="size">New byte count.</param>
		/// <param name="zeroInit">When size is growing, set all added bytes = 0. If false (default), the added memory is uninitialized, ie random byte values. Slower when true.</param>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		/// <remarks>The memory is unmanaged and will not be freed automatically. Always call <see cref="Free"/> when done or ReAlloc if need to resize it without losing data.</remarks>
		public static void* ReAlloc(void* mem, LPARAM size, bool zeroInit = false)
		{
			uint flag = zeroInit ? 8u : 0u;
			void* r;
			if(mem == null) r = Api.HeapAlloc(_processHeap, flag, size);
			else r = Api.HeapReAlloc(_processHeap, flag, mem, size);
			if(r == null) throw new OutOfMemoryException();
			return r;
		}

		/// <summary>
		/// Frees a memory block.
		/// Does nothing if mem is null.
		/// Calls API <msdn>HeapFree</msdn>.
		/// </summary>
		public static void Free(void* mem)
		{
			if(mem == null) return;
			Api.HeapFree(_processHeap, 0, mem);
		}
	}
}
