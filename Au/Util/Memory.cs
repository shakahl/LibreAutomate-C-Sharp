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
		/// <exception cref="AuException">The API failed.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive at least until this process ends. Other processes can keep the memory alive even after that.
		/// There is no Close function to close the native shared memory object handle. The OS closes it when this process ends.
		/// </remarks>
		public static void* CreateOrGet(string name, uint size, out bool created)
		{
			created = false;
			lock("AF2liKVWtEej+lRYCx0scQ") {
				string interDomainVarName = "AF2liKVWtEej+lRYCx0scQ" + name.ToLower_();
				if(!InterDomainVariables.GetVariable(name, out IntPtr t)) {
					var hm = Api.CreateFileMapping((IntPtr)(~0), Api.SECURITY_ATTRIBUTES.Common, Api.PAGE_READWRITE, 0, size, name);
					if(hm == default) goto ge;
					created = Native.GetError() != Api.ERROR_ALREADY_EXISTS;
					t = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
					if(t == default) { Api.CloseHandle(hm); goto ge; }
					InterDomainVariables.SetVariable(name, t);
				}
				return (void*)t;
			}
			ge:
			throw new AuException(0, "*open shared memory");
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
		internal OutputServer.LibSharedMemoryData outp; //now sizeof 2, reserve 16
		[FieldOffset(32)]
#if PERF_SM
		internal Perf.Inst perf; //now sizeof 184, reserve 256-32
		[FieldOffset(256)]
#endif
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
			Debug.Assert(sizeof(OutputServer.LibSharedMemoryData) <= 16);
#if PERF_SM
			Debug.Assert(sizeof(Perf.Inst) <= 256 - 32);
#endif

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
		//internal Thread_.ProcessVariables thread_;
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
#if true
		static LibProcessMemory()
		{
			Ptr = (LibProcessMemory*)InterDomainVariables.GetVariable("Au_LibProcessMemory", () => Api.VirtualAlloc(default, Size));
		}
		//This is slower (especially if using InterDomainVariables first time in domain) but not so bizarre as with window class. And less code.
#else
		static LibProcessMemory()
		{
			string name = "Au_LibMem";

			var x = new Api.WNDCLASSEX(); x.cbSize = Api.SizeOf(x);
			if(0 == Api.GetClassInfoEx(default, name, ref x)) {
				x.lpfnWndProc = Api.VirtualAlloc(default, Size); //much faster when need to zero memory
				if(x.lpfnWndProc == default) throw new OutOfMemoryException(name);

				x.style = Api.CS_GLOBALCLASS;
				x.lpszClassName = Marshal.StringToHGlobalUni(name);
				bool ok = 0 != Api.RegisterClassEx(x);

				if(ok) {
					//Api.InitializeSRWLock(&((LibProcessMemory*)x.lpfnWndProc)->_lock);
					//Api.InitializeCriticalSection(&((LibProcessMemory*)x.lpfnWndProc)->_cs);
				} else {
					if(0 == Api.GetClassInfoEx(default, name, ref x)) throw new OutOfMemoryException(name);
				}

				Marshal.FreeHGlobal(x.lpszClassName);
			}
			Ptr = (LibProcessMemory*)x.lpfnWndProc;
		}
#endif
	}

	/// <summary>
	/// Allocates memory from native heap of this process using heap API.
	/// Uses the common heap of this process, API <msdn>GetProcessHeap</msdn>.
	/// Usually slightly faster than Marshal class functions.
	/// </summary>
	public static unsafe class NativeHeap
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

	/// <summary>
	/// Allocates memory buffers that can be used with API functions and not only.
	/// Can allocate arrays of any value type - char[], byte[] etc.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// class Example
	/// {
	/// 	public static void Test()
	/// 	{
	/// 		Wnd w = Wnd.FindFast(null, "Notepad");
	/// 		string s = GetWndText(w);
	/// 		Print(s);
	/// 	}
	/// 
	/// 	public static string GetWndText(Wnd w)
	/// 	{
	/// 		for(int na = 300; ; na *= 2) {
	/// 			var b = _GetCharBuffer(ref na);
	/// 			int nr = GetWindowText(w, b, na);
	/// 			if(nr < na - 1) return (nr > 0) ? b.ToString(nr) : "";
	/// 		}
	/// 	}
	/// 
	/// 	//this variable manages the buffer
	/// 	[ThreadStatic] static WeakReference<char[]> t_char;
	/// 
	/// 	//a helper method
	/// 	static Au.Util.Buffers.CharBuffer _GetCharBuffer(ref int n) { var r = Au.Util.Buffers.Get(n, ref t_char); n = r.Length - 1; return r; }
	/// 
	/// 	//we'll use this API in this example
	/// 	[DllImport("user32.dll", EntryPoint = "GetWindowTextW")]
	/// 	static extern int GetWindowText(Wnd hWnd, [Out] char[] lpString, int nMaxCount);
	/// }
	/// ]]></code>
	/// </example>
	public static class Buffers
	{
		/// <summary>
		/// Allocates new or gets "cached" array of type T of length n or more.
		/// The returned array is managed by a WeakReference&lt;T[]&gt; variable provided by the caller. Its contents is undefined.
		/// </summary>
		/// <typeparam name="T">Any simple value type, for example char, byte, RECT.</typeparam>
		/// <param name="n">
		/// How many elements you need.
		/// If array in the WeakReference variable is null or too small, creates new array and stores it there.
		/// For byte[] and char[] types actually allocates Math.Max(n, 300)+1 elements. The 300 is to avoid future reallocations. The +1 can be used for example for '\0' character at the end of string.</param>
		/// <param name="weakReference">
		/// A reference to a WeakReference variable that manages the returned array. If null, this function will create it.
		/// The variable should be [ThreadStatic] static. Or can be a non-static field of a long-living object. Must not be simply static, it's not thread-safe (unless locked).
		/// </param>
		/// <remarks>
		/// Used to avoid creating much garbage when array allocations are needed frequently. Also is faster than code like <c>var b=new byte[1000]</c> or StringBuilder.
		/// The WeakReference variable allows the array to be garbage-collected if it is not used when GC runs. It is automatic and safe. Next time this function will create new array.
		/// Actually this function is a wrapper for WeakReference&lt;T[]&gt; functions TryGetTarget/SetTarget. Makes it easier to use.
		/// </remarks>
		public static unsafe T[] Get<T>(int n, ref WeakReference<T[]> weakReference) where T : unmanaged
		{
			//if(threadStaticWeakReference != null && !threadStaticWeakReference.TryGetTarget(out var test)) Print("collected"); test = null;

			if(sizeof(T) <= 2) { //info: don't concern about speed. In Release this is removed completely by the compiler.
				if(n < 300) n = 300;
				n++; //for safety add 1 for terminating '\0'. See also code 'r.Length - 1' in LibChar etc.
			}

			if(weakReference == null) weakReference = new WeakReference<T[]>(null);
			if(!weakReference.TryGetTarget(out var a)
				|| a.Length < n
				) weakReference.SetTarget(a = new T[n]);
			return a;

			//speed: 7 ns (tested loop 1000)
			//	When already allocated, in most cases several times faster than allocating array every time.
			//	If need big array, can be >10 times faster, because does not set all bytes = 0.
		}

		[ThreadStatic] static WeakReference<char[]> t_char;

		internal static CharBuffer LibChar(int n) { return Get(n, ref t_char); }
		internal static CharBuffer LibChar(ref int n) { var r = Get(n, ref t_char); n = r.Length - 1; return r; }
		internal static CharBuffer LibChar(int n, out int nHave) { var r = Get(n, ref t_char); nHave = r.Length - 1; return r; }

		/// <summary>
		/// Provides functions to convert char[] to string easily.
		/// Assign char[] and call the ToString functions. Example: <see cref="Buffers"/>.
		/// </summary>
		public unsafe struct CharBuffer
		{
			/// <summary>
			/// The array that is stored in this variable.
			/// </summary>
			public char[] A;
			//public int N => A.Length;

			///
			public static implicit operator char[] (CharBuffer v) => v.A;
			///
			public static implicit operator CharBuffer(char[] v) => new CharBuffer() { A = v };

			/// <summary>
			/// Converts the array, which contains native '\0'-terminated UTF-16 string, to String.
			/// Unlike code <c>new string(charArray)</c>, gets array part until '\0' character, not whole array.
			/// </summary>
			public override string ToString()
			{
				if(A == null) return null;
				fixed (char* p = A) {
					int n = LibCharPtr.Length(p, A.Length);
					return new string(p, 0, n);
				}
			}

			/// <summary>
			/// Converts the array, which contains native UTF-16 string of n length, to String.
			/// Uses <c>new string(A, 0, n)</c>, which throws exception if the array is null or n is invalid.
			/// </summary>
			/// <param name="n">String length.</param>
			public string ToString(int n)
			{
				return new string(A, 0, n);
			}

			/// <summary>
			/// The same as <see cref="ToString()"/>, but uses our StringCache.
			/// </summary>
			internal string LibToStringCached()
			{
				return StringCache.LibAdd(A);
			}

			/// <summary>
			/// The same as <see cref="ToString(int)"/>, but uses our StringCache.
			/// </summary>
			/// <param name="n">String length.</param>
			internal string LibToStringCached(int n)
			{
				return StringCache.LibAdd(A, n);
			}

			//currently not used
			///// <summary>
			///// Converts the buffer, which contains '\0'-terminated native ANSI string, to String.
			///// </summary>
			///// <param name="enc">If null, uses system's default ANSI encoding.</param>
			//internal string LibToStringFromAnsi(Encoding enc = null)
			//{
			//	if(A == null) return null;
			//	fixed (char* p = A) {
			//		int n = LibCharPtr.Length((byte*)p, A.Length * 2);
			//		return new string((sbyte*)p, 0, n, enc);
			//	}
			//}

			/// <summary>
			/// Converts the buffer, which contains native ANSI string of n length, to String.
			/// </summary>
			/// <param name="n">String length.</param>
			/// <param name="enc">If null, uses system's default ANSI encoding.</param>
			internal string LibToStringFromAnsi(int n, Encoding enc = null)
			{
				if(A == null) return null;
				fixed (char* p = A) {
					Debug.Assert(n <= A.Length * 2);
					n = Math.Min(n, A.Length * 2);
					return new string((sbyte*)p, 0, n, enc);
				}
			}
		}

		[ThreadStatic] static WeakReference<byte[]> t_byte;

		internal static byte[] LibByte(int n) { return Get(n, ref t_byte); }
		//these currently not used
		//internal static byte[] LibByte(ref int n) { var r = Get(n, ref t_byte); n = r.Length - 1; return r; }
		//internal static byte[] LibByte(int n, out int nHave) { var r = Get(n, ref t_byte); nHave = r.Length - 1; return r; }

		//currently not used
		//internal static ByteBuffer LibByte(int n) { return Get(n, ref t_byte); }
		//internal static ByteBuffer LibByte(ref int n) { var r = Get(n, ref t_byte); n = r.Length - 1; return r; }
		//internal static ByteBuffer LibByte(int n, out int nHave) { var r = Get(n, ref t_byte); nHave = r.Length - 1; return r; }

		//public unsafe struct ByteBuffer
		//{
		//	public byte[] A;

		//	public static implicit operator byte[] (ByteBuffer v) => v.A;
		//	public static implicit operator ByteBuffer(byte[] v) => new ByteBuffer() { A = v };

		//	/// <summary>
		//	/// Converts the buffer, which contains '\0'-terminated native ANSI string, to String.
		//	/// </summary>
		//	/// <param name="enc">If null, uses system's default ANSI encoding.</param>
		//	public string ToStringFromAnsi(Encoding enc = null)
		//	{
		//		if(A == null) return null;
		//		fixed (byte* p = A) {
		//			int n = LibCharPtr.Length(p, A.Length * 2);
		//			return new string((sbyte*)p, 0, n, enc);
		//		}
		//	}
		//}
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
	/// Provides a cached reusable instance of StringBuilder per thread. It's an optimisation that reduces the number of instances constructed and collected.
	/// Used like <c>using(new Au.Util.LibStringBuilder(out var b)) { b.Append("example"); var s = b.ToString(); }</c>.
	/// </summary>
	/// <remarks>
	/// This is a modified copy of the .NET internal StringBuilderCache class.
	/// The cache uses 2 [ThreadLocal] StringBuilder instances, which allows 1 nesting level. Not error to use deeper nesting level, but then gets new StringBuilder, not from the cache.
	/// </remarks>
	internal struct LibStringBuilder :IDisposable
	{
		StringBuilder _sb;

		/// <summary>
		/// 2000. The cache is not used if capacity is bigger.
		/// </summary>
		public const int MAX_BUILDER_SIZE = 2000;

		[ThreadStatic] private static StringBuilder t_cachedInstance, t_cachedInstance2;

		/// <summary>
		/// Gets a new or cached/cleared StringBuilder of the specified capacity, min 200.
		/// </summary>
		public LibStringBuilder(out StringBuilder sb, int capacity = 200)
		{
			if(capacity <= MAX_BUILDER_SIZE) {
				if(capacity < 200) capacity = 200;
				StringBuilder b = t_cachedInstance;
				bool alt = b == null; if(alt) b = t_cachedInstance2;
				if(b != null) {
					if(capacity <= b.Capacity) {
						if(alt) t_cachedInstance2 = null; else t_cachedInstance = null;
						b.Clear();
						//Debug_.Print("StringBuilder cached, alt=" + alt);
						sb = _sb = b;
						return;
					}
				}
			}
			//Debug_.Print("StringBuilder new");
			sb = _sb = new StringBuilder(capacity);
		}

		/// <summary>
		/// Releases the StringBuilder to the cache.
		/// </summary>
		public void Dispose()
		{
			if(_sb.Capacity <= MAX_BUILDER_SIZE) {
				//Debug_.Print("StringBuilder released, alt=" + (t_cachedInstance != null));
				if(t_cachedInstance == null) t_cachedInstance = _sb; else t_cachedInstance2 = _sb;
			}
			_sb = null;
		}
	}

	/// <summary>
	/// <see cref="GC"/> extensions.
	/// </summary>
	public static class GC_
	{
		static ConditionalWeakTable<object, _Remover> s_table = new ConditionalWeakTable<object, _Remover>();

		/// <summary>
		/// Calls <see cref="GC.AddMemoryPressure"/>. Later, when object <paramref name="obj"/> is garbage-collected, will call <see cref="GC.RemoveMemoryPressure"/>.
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
