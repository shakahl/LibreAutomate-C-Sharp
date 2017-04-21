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

	/// <summary>
	/// Allocates a native memory buffer as char*.
	/// The property <see cref="Common"/> can be used when calling API functions that require a char buffer. It returns a ThreadStatic variable.
	/// Using this is much faster than StringBuilder, and does not collect .NET garbage. Almost as fast as stackalloc.
	/// </summary>
	internal unsafe class LibCharBuffer
	{
		char* _p;
		int _n;

		/// <summary>
		/// Gets buffer pointer.
		/// </summary>
		public char* Ptr { get => _p; }

		/// <summary>
		/// Gets current buffer length (n characters).
		/// </summary>
		public int Capacity { get => _n; }

		/// <summary>
		/// Returns Math.Max(n, Capacity).
		/// </summary>
		public int Max(int n)
		{
			return Math.Max(n, _n);
		}

		/// <summary>
		/// Gets memory buffer of at least Math.Max(n, 300) characters.
		/// Allocates new buffer only if n is greater than Capacity. Else returns previous buffer.
		/// By default always returns true. If noThrow, returns false if fails to allocate meory (unlikely).
		/// </summary>
		/// <param name="n">In - min needed buffer length. Out - available buffer length; not less than n and not less than 300.</param>
		/// <param name="ptr">Receives buffer pointer.</param>
		/// <param name="noThrow">If fails to allocate, return false and don't throw exception.</param>
		/// <exception cref="OutOfMemoryException"></exception>
		public bool Alloc(ref int n, out char* ptr, bool noThrow = false)
		{
			if(_p == null || n > _n) {
				if(n < 300) n = 300;
				var p = (char*)Api.LocalAlloc(0, (n + 2) * 2);
				if(p == null) {
					ptr = null;
					if(noThrow) return false;
					throw new OutOfMemoryException();
				}
				_Free();
				_p = p;
				_n = n;
			}
			n = _n;
			ptr = _p;
			return true;
		}

		/// <summary>
		/// Gets memory buffer of at least Math.Max(n, 300) characters.
		/// Allocates new buffer only if n is greater than Capacity. Else returns previous buffer.
		/// </summary>
		/// <exception cref="OutOfMemoryException"></exception>
		public char* Alloc(int n)
		{
			Alloc(ref n, out var p);
			return _p;
		}

		/// <summary>
		/// Converts the buffer, which contains native '\0'-terminated UTF-16 string, to String.
		/// </summary>
		public override string ToString()
		{
			if(_p == null) return null;
			int n = Misc.CharPtrLength(_p, _n);
			return new string(_p, 0, n);
		}

		/// <summary>
		/// Converts the buffer, which contains native UTF-16 string of n length, to String.
		/// </summary>
		public string ToString(int n)
		{
			Debug.Assert(n <= _n && _p != null);
			return new string(_p, 0, Math.Min(n, _n));
		}

		/// <summary>
		/// Converts the buffer, which contains '\0'-terminated native ANSI string, to String.
		/// </summary>
		/// <param name="enc">If null, uses system's default ANSI encoding.</param>
		public string ToStringFromAnsi(Encoding enc = null)
		{
			if(_p == null) return null;
			var s = (sbyte*)_p;
			int n = Misc.CharPtrLength(s, _n * 2);
			return new String(s, 0, n, enc);
		}

		/// <summary>
		/// Frees the buffer if more than 5000 characters.
		/// </summary>
		public void Compact()
		{
			if(_n > 5000) _Free();
		}

		void _Free()
		{
			if(_p != null) {
				var p = _p;
				_p = null;
				_n = 0;
				Api.LocalFree(p);
			}
		}

		~LibCharBuffer()
		{
			_Free();
		}

		/// <summary>
		/// A ThreadStatic LibCharBuffer instance.
		/// </summary>
		public static LibCharBuffer Common { get => _common ?? (_common = new LibCharBuffer()); }
		[ThreadStatic] static LibCharBuffer _common;
		//this works only in the first thread. In other threads it will be null. It's documented.
		//[ThreadStatic] public static readonly LibCharBuffer Common = new LibCharBuffer();
	}

	/// <summary>
	/// Allocates memory from native heap of this process using heap API.
	/// Uses the common heap of this process, API <msdn>GetProcessHeap</msdn>.
	/// About 20% faster than Marshal class functions, but it depends on allocation size etc.
	/// </summary>
	public static unsafe class NativeHeap
	{
		static IntPtr _processHeap = Api.GetProcessHeap();

		/// <summary>
		/// Allocates size bytes of memory.
		/// The memory is uninitialized, ie random byte values.
		/// Calls API <msdn>HeapAlloc</msdn>.
		/// </summary>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		public static void* Alloc(LPARAM size)
		{
			void* r = Api.HeapAlloc(_processHeap, 0, size);
			if(r == null) throw new OutOfMemoryException();
			return r;
		}

		/// <summary>
		/// Allocates size bytes of memory and sets all bytes to 0.
		/// Calls API <msdn>HeapAlloc</msdn> with flag HEAP_ZERO_MEMORY.
		/// </summary>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		public static void* AllocZero(LPARAM size)
		{
			void* r = Api.HeapAlloc(_processHeap, 8, size);
			if(r == null) throw new OutOfMemoryException();
			return r;
		}

		/// <summary>
		/// Reallocates size bytes of memory.
		/// When size is growing, the added memory is uninitialized, ie random byte values.
		/// If mem is null, allocates new memory like Alloc.
		/// Calls API <msdn>HeapReAlloc</msdn> or <msdn>HeapAlloc</msdn>.
		/// </summary>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		public static void* ReAlloc(void* mem, LPARAM size)
		{
			void* r;
			if(mem == null) r = Api.HeapAlloc(_processHeap, 0, size);
			else r = Api.HeapReAlloc(_processHeap, 0, mem, size);
			if(r == null) throw new OutOfMemoryException();
			return r;
		}

		/// <summary>
		/// Reallocates size bytes of memory and sets added bytes to 0.
		/// If mem is null, allocates new memory like AllocZero.
		/// Calls API <msdn>HeapReAlloc</msdn> or <msdn>HeapAlloc</msdn> with flag HEAP_ZERO_MEMORY.
		/// </summary>
		/// <exception cref="OutOfMemoryException">Failed. Probably size is too big.</exception>
		public static void* ReAllocZero(void* mem, LPARAM size)
		{
			void* r;
			if(mem == null) r = Api.HeapAlloc(_processHeap, 8, size);
			else r = Api.HeapReAlloc(_processHeap, 8, mem, size);
			if(r == null) throw new OutOfMemoryException();
			return r;
		}

		/// <summary>
		/// Frees memory.
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
