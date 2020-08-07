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
	/// Allocates memory from native heap of this process using heap API.
	/// Uses the common heap of this process, API <msdn>GetProcessHeap</msdn>.
	/// Usually slightly faster than Marshal class functions.
	/// </summary>
	public static unsafe class AMemory
	{
		static IntPtr _processHeap = Api.GetProcessHeap();

		/// <summary>
		/// Allocates new memory block and returns its address.
		/// </summary>
		/// <param name="size">Byte count.</param>
		/// <param name="zeroInit">Set all bytes = 0.</param>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		/// <remarks>
		/// Calls API <msdn>HeapAlloc</msdn>.
		/// The memory is unmanaged and will not be freed automatically. Always call <see cref="Free"/> when done or <see cref="ReAlloc"/> if need to resize it without losing data.
		/// </remarks>
		public static void* Alloc(LPARAM size, bool zeroInit = false) => ReAlloc(null, size, zeroInit);

		/// <summary>
		/// Reallocates a memory block to make it bigger or smaller.
		/// Returns its address.
		/// </summary>
		/// <param name="mem">Old memory address. If null, allocates new memory like <see cref="Alloc"/>.</param>
		/// <param name="size">New byte count.</param>
		/// <param name="zeroInit">When size is growing, set all added bytes = 0.</param>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		/// <remarks>
		/// Calls API <msdn>HeapReAlloc</msdn> or <msdn>HeapAlloc</msdn>.
		/// Preserves data in <c>Math.Min(old size, new size)</c> bytes of old memory (copies from old memory if need).
		/// The memory is unmanaged and will not be freed automatically. Always call <see cref="Free"/> when done or <b>ReAlloc</b> if need to resize it without losing data.
		/// </remarks>
		public static void* ReAlloc(void* mem, LPARAM size, bool zeroInit = false)
		{
			uint flag = zeroInit ? 8u : 0u;
			for(int i = 0; i < 5; i++) {
				if(i > 0) {
					GC.Collect();
					GC.WaitForPendingFinalizers();
					Thread.Sleep(i*100);
				}
				void* r;
				if(mem == null) r = Api.HeapAlloc(_processHeap, flag, size);
				else r = Api.HeapReAlloc(_processHeap, flag, mem, size);
				if(r != null) return r;
			}
			throw new OutOfMemoryException();

			//note: don't need GC.AddMemoryPressure.
			//	Native memory usually is used for temporary buffers etc and is soon released eg with try/finally.
			//	Marshal.AllocHGlobal does not do it too.
		}

		/// <summary>
		/// Frees a memory block.
		/// Does nothing if <i>mem</i> is null.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>HeapFree</msdn>.
		/// </remarks>
		public static void Free(void* mem)
		{
			if(mem != null) Api.HeapFree(_processHeap, 0, mem);
		}
	}

	/// <summary>
	/// Allocates memory with API <msdn>VirtualAlloc</msdn>.
	/// Faster than managed and <see cref="AMemory"/> when memory size is large, eg more than 1 MB.
	/// The memory is initialized to zero (all bytes 0).
	/// </summary>
	internal static unsafe class AVirtualMemory_
	{
		/// <summary>
		/// Allocates new memory block and returns its address: <c>VirtualAlloc(default, size, MEM_COMMIT|MEM_RESERVE, PAGE_READWRITE)</c>.
		/// </summary>
		/// <param name="size">Byte count.</param>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		public static void* Alloc(LPARAM size)
		{
			for(int i = 0; i < 5; i++) {
				if(i > 0) {
					GC.Collect();
					GC.WaitForPendingFinalizers();
					Thread.Sleep(i*100);
				}
				void* r = Api.VirtualAlloc(default, size, Api.MEM_COMMIT | Api.MEM_RESERVE, Api.PAGE_READWRITE);
				if(r != null) return r;
			}
			throw new OutOfMemoryException();

			//note: don't need GC.AddMemoryPressure.
			//	Native memory usually is used for temporary buffers etc and is soon released eg with try/finally.
			//	Marshal.AllocHGlobal does not do it too.
		}

		/// <summary>
		/// Frees a memory block.
		/// Does nothing if <i>mem</i> is null.
		/// </summary>
		public static void Free(void* mem)
		{
			if(mem != null) Api.VirtualFree(mem);
		}
	}
}
