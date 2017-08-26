//#define ENUMWINDOWS_LESS_GARBAGE

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
//using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;
using System.Collections;

namespace Catkeys.Util
{
	//CONSIDER: if rarely used, remove and use List or create something better. Now used only in single function.

	/// <summary>
	/// Like List or StringBuilder, used as a temporary variable-size array to create final fixed-size array.
	/// To avoid much garbage (and many reallocations when growing), uses native memory heap. See <see cref="NativeHeap"/>.
	/// Should be explicitly disposed to free the native memory (else it will be freed later by the finalizer).
	/// Does not support reference types.
	/// </summary>
	//[DebuggerStepThrough]
	internal unsafe class LibArrayBuilder<T> :IDisposable
#if ENUMWINDOWS_LESS_GARBAGE
		, IEnumerable<T>
#endif
		where T : struct
	{
		void* _p;
		int _len, _cap, _minCap;

		//int _typeSize;
		int _typeSize { get => Unsafe.SizeOf<T>(); } //optimized to the contant value

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity">
		/// Minimal Capacity to set when adding elements, to avoid frequent reallocations.
		/// If 0, uses default capacity; it is 500 elements for types not bigger than 32 bytes; it is smaller for bigger types, trying to not exceed 16384 bytes.
		/// </param>
		public LibArrayBuilder(int capacity = 0)
		{
			//_typeSize = Unsafe.SizeOf<T>();

			//minimal capacity
			if(capacity > 0) _minCap = capacity;
			else {
				var r = 16384 / _typeSize; //above 16384 the memory allocation API become >=2 times slower
				if(r > 500) r = 500; else if(r < 8) r = 8;
				_minCap = r;
			}
		}

		/// <summary>
		/// Gets array memory address (address of element 0).
		/// </summary>
		public void* Ptr { get => _p; }

		/// <summary>
		/// Gets the size of the type of elements.
		/// </summary>
		public int ElementSize { get => _typeSize; }

		///// <summary>
		///// Gets the number of bytes in the array (Count*ElementSize).
		///// </summary>
		//public int ByteCount { get => _typeSize * _len; }

		/// <summary>
		/// Gets the number of elements.
		/// </summary>
		public int Count { get => _len; }

		/// <summary>
		/// Gets or sets the total number of elements (not bytes) the internal memory can hold without resizing.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">(the 'set' function) value less than Count. Instead use ReAlloc or Free.</exception>
		public int Capacity
		{
			get => _cap;
			set
			{
				if(value != _cap) {
					if(value < _len) throw new ArgumentOutOfRangeException();
					_p = NativeHeap.ReAlloc(_p, value * _typeSize);
					_cap = value;
				}
			}
		}

		/// <summary>
		/// Allocates count elements. Sets Count=count.
		/// Frees previously allocated memory.
		/// Returns memory address (address of element 0).
		/// </summary>
		/// <param name="count">Element count.</param>
		/// <param name="zeroInit">Set all bytes = 0. If false, the memory is uninitialized, ie random byte values. Default true. Slower when true.</param>
		public void* Alloc(int count, bool zeroInit = true)
		{
			if(_cap != 0) Free();
			int cap = Math.Max(count, 8);
			_p = NativeHeap.Alloc(cap * _typeSize, zeroInit);
			_cap = cap; _len = count;
			return _p;
		}

		/// <summary>
		/// Adds or removes elements at the end. Sets Count=count.
		/// Preserves Math.Min(Count, count) existing elements.
		/// Returns memory address (address of element 0).
		/// </summary>
		/// <param name="count">New element count.</param>
		/// <param name="zeroInit">Set all added bytes = 0. If false, the added memory is uninitialized, ie random byte values. Default true. Slower when true.</param>
		/// <remarks>
		/// The new memory usually is at a new location. The preserved elements are copied there.
		/// Sets Count=count. To allocate more memory without changing Count, change Capacity instead.
		/// </remarks>
		public void* ReAlloc(int count, bool zeroInit = true)
		{
			int cap = Math.Max(count, 8);
			_p = NativeHeap.ReAlloc(_p, cap * _typeSize, zeroInit);
			_cap = cap; _len = count;
			return _p;
		}

		/// <summary>
		/// Frees memory. Sets Count and Capacity = 0.
		/// </summary>
		public void Free()
		{
			if(_cap == 0) return;
			_len = _cap = 0;
			var p = _p; _p = null;
			NativeHeap.Free(p);
		}

		/// <summary>
		/// Calls Free and GC.SuppressFinalize.
		/// </summary>
		public void Dispose()
		{
			Free();
			GC.SuppressFinalize(this);
		}

		~LibArrayBuilder()
		{
			Free();
			Debug_.Print("not disposed " + nameof(LibArrayBuilder<T>));
		}

		/// <summary>
		/// Adds one element.
		/// Uses ref to avoid copying when T size is big. Does not modify the passed variable.
		/// </summary>
		public void AddR(ref T value)
		{
			if(_len == _cap) _EnsureCapacity();
			void* dest = (byte*)_p + _typeSize * _len;
			Unsafe.Copy(dest, ref value);
			//Unsafe.Write(dest, value);
			//Unsafe.AsRef<T>(dest) = value;
			_len++;

			//tested the Unsafe calls, 64-bit:
			//	T = long. All 3 compiled identically, 2 instructions.
			//	T = RECT. Copy and AsRef identic, 2 instructions. Write 4 instructions.
		}

		/// <summary>
		/// Adds one element.
		/// </summary>
		public void Add(T value)
		{
			if(_len == _cap) _EnsureCapacity();
			void* dest = (byte*)_p + _typeSize * _len;
			Unsafe.Copy(dest, ref value);
			//Unsafe.Write(dest, value);
			//Unsafe.AsRef<T>(dest) = value;
			_len++;

			//tested the Unsafe calls, 64-bit:
			//	T = long. All 3 compiled identically, 1 instruction.
			//	T = RECT. Everything same as with ref, because value actually is passed by ref.
			//32-bit:
			//	T = long. All 3 compiled identically, 4 instructions.
			//	T = RECT. All 3 compiled almost identically, Copy and Write 8 instructions, AsRef 9 instructions.
		}

		/// <summary>
		/// Adds one zero-inited element and returns its reference.
		/// </summary>
		public ref T Add()
		{
			if(_len == _cap) _EnsureCapacity();
			void* dest = (byte*)_p + _typeSize * _len;
			ref T r = ref Unsafe.AsRef<T>(dest);
			r = default(T);
			_len++;
			return ref r;
		}

		/// <summary>
		/// Capacity = Math.Max(_cap * 2, _minCap).
		/// </summary>
		void _EnsureCapacity()
		{
			Capacity = Math.Max(_cap * 2, _minCap);
		}

		/// <summary>
		/// Gets element reference.
		/// </summary>
		/// <param name="i">Element index.</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public ref T this[int i]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if((uint)i >= (uint)_len) _ThrowBadIndex();
				return ref Unsafe.AsRef<T>((byte*)_p + _typeSize * i);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void _ThrowBadIndex()
		{
			throw new IndexOutOfRangeException();
		}

		/// <summary>
		/// Copies elements to a new array.
		/// </summary>
		public T[] ToArray()
		{
			var r = new T[_len];
			for(int i = 0, n = _len; i < n; i++)
				r[i] = Unsafe.Read<T>((byte*)_p + i * _typeSize);
			return r;
		}

		/// <summary>
		/// Copies elements of this and another variable to a new array.
		/// </summary>
		public T[] ToArrayAndAppend(LibArrayBuilder<T> append)
		{
			var r = new T[_len + append._len];
			_CopyTo(r, 0);
			append._CopyTo(r, _len);
			return r;
		}

		void _CopyTo(T[] a, int aFirst)
		{
			for(int i = 0, n = _len; i < n; i++)
				a[aFirst++] = Unsafe.Read<T>((byte*)_p + i * _typeSize);
		}

#if ENUMWINDOWS_LESS_GARBAGE
		public IEnumerator<T> GetEnumerator() => new Enumerator(this);

		IEnumerator IEnumerable.GetEnumerator() => null; //not used

		public class Enumerator :IEnumerator<T>
		{
			LibArrayBuilder<T> _a;
			int _index;

			internal Enumerator(LibArrayBuilder<T> a) { _a = a; _index = -1; }

			public T Current => _a[_index];

			object IEnumerator.Current => null; //not used

			public void Dispose() { }

			public bool MoveNext() => ++_index < _a.Count;

			public void Reset() => _index = -1;
		}
#endif
	}
}
