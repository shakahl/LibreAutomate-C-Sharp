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
#if true
	internal unsafe class LibArrayBuilder<T> :IDisposable where T : struct
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
		public void Add(ref T value)
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
		/// Adds one element by value.
		/// </summary>
		public void AddV(T value)
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
	}
#elif true
	internal unsafe class LibArrayBuilder :IDisposable
	{
		void* _p;
		int _typeSize, _len, _cap, _minCap;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeSize">Element type size. If don't know, use <see cref="TypeSize{T}"/>. Note that size of some types is different in 32-bit and 64-bit process.</param>
		/// <param name="capacity">Initial Capacity. If 0 (or set to 0 later), on first allocation will be used default capacity; it is 512 elements for types not bigger than 32 bytes; it is smaller for bigger types, trying to not exceed 16384 bytes.</param>
		/// <exception cref="ArgumentException">typeSize less than 1 or capacity less than 0.</exception>
		public LibArrayBuilder(int typeSize, int capacity)
		{
			if(typeSize < 1 || capacity < 0) throw new ArgumentException();
			_typeSize = typeSize;

			//minimal capacity
			var r = 16384 / _typeSize; //above 16384 the memory allocation API become >=2 times slower
			if(r > 512) r = 512; else if(r < 4) r = 4; //tested with Wnd: normally there are ~400 windows on my PC, rarely exceeds 512
			_minCap = r;

			Capacity = capacity;
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
		/// <exception cref="ArgumentOutOfRangeException">value less than Count. Instead use ReAlloc or Free.</exception>
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
		/// Allocates count zero-inited elements. Sets Count=count.
		/// Frees previously allocated memory.
		/// Returns memory address (address of element 0).
		/// </summary>
		/// <param name="count"></param>
		public void* Alloc(int count)
		{
			if(_cap != 0) Free();
			int cap = Math.Max(count, 16);
			_p = NativeHeap.AllocZero(cap * _typeSize);
			_cap = cap; _len = count;
			return _p;
		}

		/// <summary>
		/// Allocates count elements. Sets Count=count.
		/// Preserves Math.Min(Count, count) existing elements. Added elements are zero-inited.
		/// Returns memory address (address of element 0).
		/// The new memory usually is at a new location. The preserved elements are copied there.
		/// If want to allocate more memory without changing Count, use Capacity instead.
		/// </summary>
		/// <param name="count"></param>
		public void* ReAlloc(int count)
		{
			int cap = Math.Max(count, 16);
			_p = NativeHeap.ReAllocZero(_p, cap * _typeSize);
			_cap = cap; _len = count;
			return _p;
		}

		/// <summary>
		/// Frees memory. Sets Count and Capacity to 0.
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

		~LibArrayBuilder() { Free(); }

		/// <summary>
		/// Adds one zero-inited element and returns its address.
		/// </summary>
		public void* AddZero()
		{
			if(_len == _cap) _EnsureCapacity();
			var b = (byte*)_p + _typeSize * _len;
			if(0 == (_typeSize & 3)) {
				for(int* p = (int*)b, pe = p + _typeSize / 4; p < pe; p++) *p = 0;
			} else {
				for(byte* be = b + _typeSize; b < be; b++) *b = 0;
			}
			_len++;
			return b;
		}

		/// <summary>
		/// Adds one non-zero-inited element and returns its address.
		/// </summary>
		public void* AddFast()
		{
			if(_len == _cap) _EnsureCapacity();
			return (byte*)_p + _typeSize * _len++;
		}

		/// <summary>
		/// Doubles Capacity. Sets 16 or more.
		/// </summary>
		void _EnsureCapacity()
		{
			Capacity = Math.Max(_cap * 2, 16);
		}

		/// <summary>
		/// Gets element address.
		/// </summary>
		/// <param name="i"></param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public void* this[int i]
		{
			get
			{
				if((uint)i >= (uint)_len) throw new IndexOutOfRangeException();
				return (byte*)_p + _typeSize * i;
			}
		}

		/// <summary>
		/// Copies whole array (Count*ElementSize bytes) to another memory location, for example to fixed(TYPE* p=managedArray).
		/// Tip: If you need List, copy to an intermediate array and call its ToList. Cannot get fixed pointer of List array.
		/// </summary>
		/// <param name="mem"></param>
		public void CopyTo(void* mem)
		{
			if(_len > 0) Api.memcpy(mem, _p, _typeSize * _len);
		}

		//public List<T> ToList<T>()
		//{
		//	var a = new List<T>(_len);
		//	fixed(T* p = a) { } //cannot get pointer...
		//	return a;
		//}

		//public List<T> ToList<T>()
		//{
		//	var a = new List<T>(_len);
		//	for(int i=0; i<_len; i++) a[i]= //cannot get pointer...
		//	return a;
		//}

		/// <summary>
		/// Classes that wrap LibArrayBuilder for types used in this library.
		/// Because LibArrayBuilder is quite difficult and unsafe to use, and we cannot create a generic class.
		/// </summary>
		public static class Specialized
		{
			public struct _Wnd :IDisposable
			{
				LibArrayBuilder _a;

				public _Wnd(int capacity = 0) { _a = new LibArrayBuilder(IntPtr.Size, capacity); }

				public void Dispose() { _a?.Dispose(); }

				public void Add(Wnd w) { *(Wnd*)_a.AddFast() = w; }

				//public int Count { get => _a.Count; }

				//public Wnd this[int i] { get => *(Wnd*)_a[i]; set { *(Wnd*)_a[i] = value; } }

				public Wnd[] ToArray()
				{
					var r = new Wnd[_a.Count];
					fixed (Wnd* p = r) _a.CopyTo(p);
					return r;
				}

				public Wnd[] ToArray(_Wnd append)
				{
					var r = new Wnd[_a.Count + append._a.Count];
					fixed (Wnd* p = r) { _a.CopyTo(p); append._a.CopyTo(p + _a.Count); }
					return r;
				}
			}
		}
	}
#else
	internal unsafe class LibNativeList<T> :IDisposable where T : struct
	{
		static readonly int s_typeSize;

		static LibNativeList()
		{
			s_typeSize = TypeSize<T>.Size;
		}

		void* _p;
		int _typeSize, _len, _cap;

		public LibNativeList(int capacity = 0)
		{
			_typeSize = s_typeSize;
			Capacity = capacity;
		}

		/// <summary>
		/// Gets array memory address (address of element 0).
		/// </summary>
		public void* Ptr { get => _p; }

		/// <summary>
		/// Gets the size of the type of elements.
		/// </summary>
		public int ElementSize { get => _typeSize; }

		/// <summary>
		/// Gets the number of elements.
		/// </summary>
		public int Count { get => _len; }

		/// <summary>
		/// Gets or sets the total number of elements the internal memory can hold without resizing.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">value less than Count. Instead use ReAlloc or Free.</exception>
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
		/// Allocates count zero-inited elements. Sets Count=count.
		/// Frees previously allocated memory.
		/// Returns memory address (address of element 0).
		/// </summary>
		/// <param name="count"></param>
		public void* Alloc(int count)
		{
			if(_cap != 0) Free();
			int cap = Math.Max(count, 16);
			_p = NativeHeap.AllocZero(cap * _typeSize);
			_cap = cap; _len = count;
			return _p;
		}

		/// <summary>
		/// Allocates count elements. Sets Count=count.
		/// Preserves Math.Min(Count, count) existing elements. Added elements are zero-inited.
		/// Returns memory address (address of element 0).
		/// The new memory usually is at a new location. The preserved elements are copied there.
		/// If want to allocate more memory without changing Count, use Capacity instead.
		/// </summary>
		/// <param name="count"></param>
		public void* ReAlloc(int count)
		{
			int cap = Math.Max(count, 16);
			_p = NativeHeap.ReAllocZero(_p, cap * _typeSize);
			_cap = cap; _len = count;
			return _p;
		}

		/// <summary>
		/// Frees memory. Sets Count and Capacity to 0.
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

		/// <summary>
		/// Adds one zero-inited element and returns its address.
		/// </summary>
		public void* Add()
		{
			if(_len == _cap) _EnsureCapacity();
			var b = (byte*)_p + _typeSize * _len;
			if(0 == (_typeSize & 3)) {
				for(int* p = (int*)b, pe = p + _typeSize / 4; p < pe; p++) *p = 0;
			} else {
				for(byte* be = b + _typeSize; b < be; b++) *b = 0;
			}
			_len++;
			return b;
		}

		/// <summary>
		/// Adds one non-zero-inited element and returns its address.
		/// </summary>
		public void* AddFast()
		{
			if(_len == _cap) _EnsureCapacity();
			return (byte*)_p + _typeSize * _len++;
		}

		/// <summary>
		/// Doubles Capacity. Sets 16 or more.
		/// </summary>
		void _EnsureCapacity()
		{
			Capacity = Math.Max(_cap * 2, 16);
		}

		/// <summary>
		/// Gets element address.
		/// </summary>
		/// <param name="i"></param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public void* this[int i]
		{
			get
			{
				if((uint)i >= (uint)_len) throw new IndexOutOfRangeException();
				return (byte*)_p + _typeSize * i;
			}
		}

		//public List<T> ToList()
		//{
		//	var a = new List<T>(_len);
		//	for(int i=0; i<_len; i++) a[i]= //cannot get pointer...
		//	return a;
		//}
	}
#endif
}
