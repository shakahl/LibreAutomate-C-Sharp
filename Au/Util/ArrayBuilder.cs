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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Like List or StringBuilder, used as a temporary variable-size array to create final fixed-size array.
	/// To avoid much garbage (and many reallocations when growing), uses native memory heap. See <see cref="NativeHeap"/>.
	/// Must be explicitly disposed to free the native memory. Does not have a finalizer because is struct (to avoid garbage).
	/// Does not support reference types. Does not call Dispose.
	/// </summary>
	//[DebuggerStepThrough]
	internal unsafe struct LibArrayBuilder<T> :IDisposable where T : struct
	{
		void* _p;
		int _len, _cap;

		static int s_minCap;

		static int _TypeSize => Unsafe.SizeOf<T>(); //optimized to the constant value, except in static ctor (why?)

		static LibArrayBuilder()
		{
			var r = 16384 / Unsafe.SizeOf<T>(); //above 16384 the memory allocation API become >=2 times slower
			if(r > 500) r = 500; else if(r < 8) r = 8;
			s_minCap = r;

			//info: 500 is optimal for getting all top-level windows (and invisible) as LibArrayBuilder<Wnd>.
			//	Normally there are 200-400 windows on my PC, rarely > 500.
		}

		public void Dispose()
		{
			Free();
		}

		/// <summary>
		/// Gets array memory address (address of element 0).
		/// </summary>
		public void* Ptr => _p;

		/// <summary>
		/// Gets the number of elements.
		/// </summary>
		public int Count => _len;

		/// <summary>
		/// Gets the number of bytes in the array (Count*sizeof(T)).
		/// </summary>
		public int ByteCount => _len * _TypeSize;

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
					_p = NativeHeap.ReAlloc(_p, value * _TypeSize);
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
			int cap = Math.Max(count, s_minCap);
			_p = NativeHeap.Alloc(cap * _TypeSize, zeroInit);
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
			int cap = Math.Max(count, s_minCap);
			_p = NativeHeap.ReAlloc(_p, cap * _TypeSize, zeroInit);
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
		/// Adds one element.
		/// Uses ref to avoid copying when T size is big. Does not modify the passed variable.
		/// </summary>
		public void AddR(ref T value)
		{
			if(_len == _cap) _EnsureCapacity();
			void* dest = (byte*)_p + _TypeSize * _len;
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
			void* dest = (byte*)_p + _TypeSize * _len;
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
			void* dest = (byte*)_p + _TypeSize * _len;
			ref T r = ref Unsafe.AsRef<T>(dest);
			r = default;
			_len++;
			return ref r;
		}

		/// <summary>
		/// Capacity = Math.Max(_cap * 2, s_minCap).
		/// </summary>
		void _EnsureCapacity()
		{
			Capacity = Math.Max(_cap * 2, s_minCap);
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
				return ref Unsafe.AsRef<T>((byte*)_p + _TypeSize * i);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void _ThrowBadIndex()
		{
			throw new IndexOutOfRangeException();
		}

		/// <summary>
		/// Copies elements to a new managed array.
		/// </summary>
		public T[] ToArray()
		{
			if(_len == 0) return Array.Empty<T>();
			var r = new T[_len];
			for(int i = 0, n = _len; i < n; i++)
				r[i] = Unsafe.Read<T>((byte*)_p + i * _TypeSize);
			return r;
		}
	}
}
