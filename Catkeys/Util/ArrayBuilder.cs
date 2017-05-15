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
	/// <summary>
	/// Like List or StringBuilder, used as a temporary variable-size array to create final fixed-size array.
	/// To avoid much garbage (and many reallocations when growing), uses native memory heap. See <see cref="NativeHeap"/>.
	/// Is not generic, and functions return void*, because C# does not allow to get T* in generic class. The caller can cast void* to the array type pointer.
	/// The nested static class Specialized contains safe and easy-to-use wrapper classes for some types. Add more wrapper classes there when need.
	/// Does not support reference types.
	/// </summary>
	//[DebuggerStepThrough]
#if true
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
