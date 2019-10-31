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
using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
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
	/// 		AWnd w = AWnd.FindFast(null, "Notepad");
	/// 		string s = GetWndText(w);
	/// 		Print(s);
	/// 	}
	/// 
	/// 	public static string GetWndText(AWnd w)
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
	/// 	static Au.Util.AMemoryArray.CharBuffer _GetCharBuffer(ref int n) { var r = Au.Util.AMemoryArray.Get(n, ref t_char); n = r.Length - 1; return r; }
	/// 
	/// 	//we use this API in this example
	/// 	[DllImport("user32.dll", EntryPoint = "GetWindowTextW")]
	/// 	static extern int GetWindowText(AWnd hWnd, [Out] char[] lpString, int nMaxCount);
	/// }
	/// ]]></code>
	/// </example>
	public static class AMemoryArray
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
		/// Assign char[] and call the ToString functions. Example: <see cref="AMemoryArray"/>.
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
	//			_pOnHeap = (byte*)AMemory.Alloc(nBytes);
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
	//			AMemory.Free(_pOnHeap);
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
}
