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
	/// <seealso cref="Wnd.Misc.InterProcessSendData"/>
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
		/// <exception cref="CatException">The API failed.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive at least until this process ends. Other processes can keep the memory alive even after that.
		/// There is no Close function to close the native shared memory object handle. The OS closes it when this process ends.
		/// </remarks>
		public static void* CreateOrGet(string name, uint size, out bool created)
		{
			created = false;
			lock("AF2liKVWtEej+lRYCx0scQ") {
				string interDomainVarName = "AF2liKVWtEej+lRYCx0scQ" + name.ToLower_();
				if(!InterDomain.GetVariable(name, out IntPtr t)) {
					var hm = Api.CreateFileMapping((IntPtr)(~0), Api.SECURITY_ATTRIBUTES.Common, Api.PAGE_READWRITE, 0, size, name);
					if(hm == Zero) goto ge;
					created = Native.GetError() != Api.ERROR_ALREADY_EXISTS;
					t = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
					if(t == Zero) { Api.CloseHandle(hm); goto ge; }
					InterDomain.SetVariable(name, t);
				}
				return (void*)t;
			}
			ge:
			throw new CatException(0, "*open shared memory");
		}
	}

	/// <summary>
	/// Memory shared by all appdomains and by other related processes.
	/// </summary>
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Explicit, Size = 0x10000)]
	unsafe struct LibSharedMemory
	{
		#region variables used by our library classes
		//Declare variables used by our library classes.
		//Be careful:
		//1. Some type sizes are different in 32 and 64 bit process, eg IntPtr.
		//	Solution: Use long and cast to IntPtr etc.
		//2. The memory may be used by processes that use different library versions.
		//	Solution: In new library versions don't change struct offsets and old members. Maybe reserve some space for future members. If need more, add new struct. Use Assert(sizeof) in our static ctor.

		//reserve 16 for some header, eg shared memory version.
		[FieldOffset(16)]
		internal Output.Server.LibSharedMemoryData outp; //now sizeof 2, reserve 16
		[FieldOffset(32)]
		internal Perf.Inst perf; //now sizeof 184, reserve 256-32
		[FieldOffset(256)]
		byte _futureStructPlaceholder;

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
			Debug.Assert(sizeof(Output.Server.LibSharedMemoryData) <= 16);
			Debug.Assert(sizeof(Perf.Inst) <= 256 - 32);

			_sm = (LibSharedMemory*)SharedMemory.CreateOrGet("Catkeys_SM_0x10000", Size, out var created);
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
		public static LibSharedMemory* Ptr { get => _sm; }
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
		//Api.RTL_CRITICAL_SECTION _cs; //slower than SRW but not much. Initialization speed not tested.
		//Api.RTL_SRWLOCK _lock; //2 times slower than C# lock, but we need this because C# lock is appdomain-local

		#region variables used by our library classes
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		//public int test;
		internal LibTables.ProcessVariables tables;
		internal LibWorkarounds.ProcessVariables workarounds;
		internal ThreadPoolSTA.ProcessVariables threadPool;

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
	/// Normally is used as a [ThreadStatic] variable. It is faster than StringBuilder, local array or native memory, and does not create .NET garbage. When big buffer, this is also faster than stackalloc which then is slow because sets all bytes to 0.
	/// When allocating large buffer using a [ThreadStatic] variable, finally call <see cref="Compact"/>, else the memory remains allocated until the thread ends or some other function compacts it.
	/// Because a [ThreadStatic] variable can be used by any function, make sure that, while a function uses it, cannot be called other functions that use the same variable (unless they are known to use the same buffer).
	/// </summary>
	public unsafe class CharBuffer
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
		/// Gets memory buffer of at least Math.Max(n+1, 300) characters.
		/// Allocates new buffer only if n is greater than Capacity. Else returns previous buffer.
		/// By default always returns true. If noThrow, returns false if fails to allocate memory (unlikely).
		/// The buffer content is uninitialized (random bytes).
		/// </summary>
		/// <param name="n">In - min needed buffer length. Out - available buffer length; not less than n and not less than 300.</param>
		/// <param name="ptr">Receives buffer pointer.</param>
		/// <param name="noThrow">If fails to allocate, return false and don't throw exception.</param>
		/// <exception cref="OutOfMemoryException"></exception>
		/// <exception cref="ArgumentException">n is less than 0.</exception>
		public bool Alloc(ref int n, out char* ptr, bool noThrow = false)
		{
			if(n < 0) throw new ArgumentException();
			if(_p == null || n > _n) {
				if(n < 300) n = 300;
				var p = (char*)NativeHeap.Alloc((n + 2) * 2);
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
		/// Gets memory buffer of at least Math.Max(n+1, 300) characters.
		/// Allocates new buffer only if n is greater than Capacity. Else returns previous buffer.
		/// The buffer content is uninitialized (random bytes).
		/// </summary>
		/// <exception cref="OutOfMemoryException"></exception>
		/// <exception cref="ArgumentException">n is less than 0.</exception>
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
			int n = CharPtr.Length(_p, _n);
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
			int n = CharPtr.Length((byte*)_p, _n * 2);
			return new String((sbyte*)_p, 0, n, enc);
		}

		/// <summary>
		/// Converts the buffer, which contains native ANSI string of n length, to String.
		/// </summary>
		/// <param name="enc">If null, uses system's default ANSI encoding.</param>
		public string ToStringFromAnsi(int n, Encoding enc = null)
		{
			if(_p == null) return null;
			Debug.Assert(n <= _n * 2);
			n = Math.Min(n, _n * 2);
			return new String((sbyte*)_p, 0, n, enc);
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
				NativeHeap.Free(p);
			}
		}

		///
		~CharBuffer()
		{
			_Free();
		}

		/// <summary>
		/// A [ThreadStatic] CharBuffer instance used in this library.
		/// Don't use in other assemblies even if can access it becuse of [InternalsVisibleTo].
		/// </summary>
		internal static CharBuffer LibCommon { get => _common ?? (_common = new CharBuffer()); }
		[ThreadStatic] static CharBuffer _common;
	}

	/// <summary>
	/// Allocates a native memory buffer as byte*.
	/// Normally is used as a [ThreadStatic] variable. It is faster than local array or native memory, and does not create .NET garbage. When big buffer, this is also faster than stackalloc which then is slow because sets all bytes to 0.
	/// When allocating large buffer using a [ThreadStatic] variable, finally call <see cref="Compact"/>, else the memory remains allocated until the thread ends or some other function compacts it.
	/// </summary>
	public unsafe class ByteBuffer
	{
		byte* _p;
		int _n;

		/// <summary>
		/// Gets buffer pointer.
		/// </summary>
		public byte* Ptr { get => _p; }

		/// <summary>
		/// Gets current buffer length (n bytes).
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
		/// Gets memory buffer of at least Math.Max(n+1, 300) bytes.
		/// Allocates new buffer only if n is greater than Capacity. Else returns previous buffer.
		/// By default always returns true. If noThrow, returns false if fails to allocate memory (unlikely).
		/// The buffer content is uninitialized (random bytes).
		/// </summary>
		/// <param name="n">In - min needed buffer length. Out - available buffer length; not less than n and not less than 300.</param>
		/// <param name="ptr">Receives buffer pointer.</param>
		/// <param name="noThrow">If fails to allocate, return false and don't throw exception.</param>
		/// <exception cref="OutOfMemoryException"></exception>
		/// <exception cref="ArgumentException">n is less than 0.</exception>
		public bool Alloc(ref int n, out byte* ptr, bool noThrow = false)
		{
			if(n < 0) throw new ArgumentException();
			if(_p == null || n > _n) {
				if(n < 300) n = 300;
				var p = (byte*)NativeHeap.Alloc(n + 1);
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
		/// Gets memory buffer of at least Math.Max(n+1, 300) bytes.
		/// Allocates new buffer only if n is greater than Capacity. Else returns previous buffer.
		/// The buffer content is uninitialized (random bytes).
		/// </summary>
		/// <exception cref="OutOfMemoryException"></exception>
		/// <exception cref="ArgumentException">n is less than 0.</exception>
		public byte* Alloc(int n)
		{
			Alloc(ref n, out var p);
			return _p;
		}

		/// <summary>
		/// Converts the buffer, which contains '\0'-terminated native ANSI string, to String.
		/// </summary>
		/// <param name="enc">If null, uses system's default ANSI encoding.</param>
		public string ToStringFromAnsi(Encoding enc = null)
		{
			if(_p == null) return null;
			int n = CharPtr.Length(_p, _n * 2);
			return new String((sbyte*)_p, 0, n, enc);
		}

		/// <summary>
		/// Frees the buffer if more than 10000 bytes.
		/// </summary>
		public void Compact()
		{
			if(_n > 10000) _Free();
		}

		void _Free()
		{
			if(_p != null) {
				var p = _p;
				_p = null;
				_n = 0;
				NativeHeap.Free(p);
			}
		}

		///
		~ByteBuffer()
		{
			_Free();
		}

		/// <summary>
		/// A [ThreadStatic] ByteBuffer instance used in this library.
		/// Don't use in other assemblies even if can access it becuse of [InternalsVisibleTo].
		/// </summary>
		internal static ByteBuffer LibCommon { get => _common ?? (_common = new ByteBuffer()); }
		[ThreadStatic] static ByteBuffer _common;
	}

	//Tried to create a fast memory buffer class, eg for calling API.
	//Failed, because compiler memsets the buffer. If buffer size is > 1000, it is slower than HeapAlloc/HeapFree.
	//stackalloc also memsets, in most cases.
	///// <summary>
	///// Allocates a memory buffer on the stack (fast but fixed-size) or heap (slower but can be any size), depending on the size required.
	///// The variable must be a local variable (not a class member etc). Else it is not on stack, which is dangerous because can be moved by GC.
	///// </summary>
	//public unsafe struct MemoryBufferOnStackOrHeap :IDisposable
	//{
	//	byte* _pOnHeap;
	//	int _sizeOnHeap;
	//	fixed byte _bOnStack[StackSize];

	//	/// <summary>
	//	/// Size of memory buffer in this variable (on stack).
	//	/// It is almost 1% of normal stack size (1 MB).
	//	/// </summary>
	//	public const int StackSize = 10000;

	//	/// <summary>
	//	/// Returns pointer to a memory buffer.
	//	/// If nBytes is less or equal to StackSize (10000), the memory is on the stack (in this variable), else on the native heap.
	//	/// </summary>
	//	/// <param name="nBytes">Requested memory size (number of bytes).</param>
	//	/// <exception cref="OutOfMemoryException"></exception>
	//	/// <remarks>
	//	/// Can be called multiple times, for example when need a bigger buffer than already allocated. In such case frees the previously allocated heap memory if need.
	//	/// </remarks>
	//	[MethodImpl(MethodImplOptions.NoInlining)]
	//	public byte* Allocate(int nBytes)
	//	{
	//		//if(nBytes <= c_stackSize) return _bOnStack; //error
	//		if(nBytes <= StackSize) {
	//			fixed (byte* p = _bOnStack) return p; //disassembly: no function calls etc, just returns _bOnStack address
	//		}
	//		if(nBytes > _sizeOnHeap) {
	//			Dispose();
	//			_pOnHeap = (byte*)NativeHeap.Alloc(nBytes);
	//			_sizeOnHeap = nBytes;
	//		}
	//		return _pOnHeap;
	//	}

	//	/// <summary>
	//	/// Frees the memory buffer if need (if uses heap).
	//	/// Always call this finally, either explicitly or through <c>using(...){...}</c>. Else the heap memory will never be freed, because this is a struct and therefore cannot have a finalizer.
	//	/// </summary>
	//	[MethodImpl(MethodImplOptions.NoInlining)]
	//	public void Dispose()
	//	{
	//		if(_sizeOnHeap > 0) {
	//			_sizeOnHeap = 0;
	//			NativeHeap.Free(_pOnHeap);
	//			_pOnHeap = null;
	//		}
	//	}

	//	//public MemoryBufferOnStackOrHeap(bool unused)
	//	//{
	//	//	_pOnHeap = null;
	//	//	_sizeOnHeap = 0;
	//	//	//_bOnStack is zeroed implicitly
	//	//}
	//}

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
