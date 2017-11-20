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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys.Util
{
	/// <summary>
	/// Caches managed strings created from unmanaged char* buffers.
	/// </summary>
	/// <remarks>
	/// Can be used in possibly very frequently called functions to avoid much string garbage and frequent garbage collections.
	/// For example, Wnd.ClassName and Wnd.Name use this. They are called by Wnd.Find for each window (can be hundreds of them). And WaitFor.WindowExists calls Wnd.Find every 30 or so milliseconds. It could create megabytes of garbage (window classname and name strings) in a few seconds. Every Wnd.Find call creates almost the same set of strings, therefore it makes sense to cache them.
	/// To allow GC free the cached strings, use a WeakReferenc&lt;StringCache&gt; object. See example.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// internal static unsafe string GetCachedString(char* p, int len)
	/// {
	/// 	lock(_cacheWR) {
	/// 		if(!_cacheWR.TryGetTarget(out var cache)) _cacheWR.SetTarget(cache = new StringCache());
	/// 		return cache.Add(p, len);
	/// 	}
	/// }
	/// static WeakReference<StringCache> _cacheWR = new WeakReference<StringCache>(null);
	/// 
	/// //When char[] is used as text buffer.
	/// internal static unsafe string GetCachedString(char[] a, int len)
	/// {
	/// 	Debug.Assert(len < a.Length);
	/// 	fixed (char* p = a) return GetCachedString(p, len);
	/// }
	/// ]]></code>
	/// </example>
	public unsafe class StringCache
	{
		//Most of this code is from HashSet<T> source code.

		int[] _buckets;
		_Slot[] _slots;
		int _count;
		int _lastIndex;
		int _freeList;

		/// <summary>
		/// Number of elements in this cache.
		/// </summary>
		public int Count
		{
			get { return _count; }
		}

		//rejected: use this at a higher level, before locking etc.
		///// <summary>
		///// Searches in the cache for a managed string that matches the first len characters of char[] a.
		///// If finds, returns it. Else adds <c>new string(a, 0, len)</c> to the cache and returns it.
		///// </summary>
		///// <param name="a">char[] used as text buffer.</param>
		///// <param name="len">Length of valid text in a.</param>
		///// <remarks>
		///// speed when found in cache: similar to <c>new string(p, 0, len)</c>.
		///// </remarks>
		//public string Add(char[] a, int len)
		//{
		//	fixed(char* p=a)
		//		return Add(p, len);
		//}

		/// <summary>
		/// Searches in the cache for a managed string that matches unmanaged string p of length len.
		/// If finds, returns it. Else adds <c>new string(p, 0, len)</c> to the cache and returns it.
		/// </summary>
		/// <param name="p">Unmanaged string.</param>
		/// <param name="len">p length.</param>
		/// <remarks>
		/// speed when found in cache: similar to <c>new string(p, 0, len)</c>.
		/// </remarks>
		public string Add(char* p, int len)
		{
			if(p == null) return null;
			if(len == 0) return "";

			if(_buckets == null) {
				int size = HashHelpers.GetPrime(500);

				_buckets = new int[size];
				_slots = new _Slot[size];
				_freeList = -1;
			}

			int hashCode = Convert_.HashFast(p, len) & 0x7fffffff;
			int bucket = hashCode % _buckets.Length;
			for(int i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].next) {
				if(_slots[i].hashCode == hashCode && LibCharPtr.Equals(p, len, _slots[i].value)) {
					return _slots[i].value;
				}
			}

			int index;
			if(_freeList >= 0) {
				index = _freeList;
				_freeList = _slots[index].next;
			} else {
				if(_lastIndex == _slots.Length) {
					_IncreaseCapacity();
					// this will change during resize
					bucket = hashCode % _buckets.Length;
				}
				index = _lastIndex;
				_lastIndex++;
			}
			string R = new string(p, 0, len);
			_slots[index].value = R;
			_slots[index].hashCode = hashCode;
			_slots[index].next = _buckets[bucket] - 1;
			_buckets[bucket] = index + 1;
			_count++;

			return R;
		}

		void _IncreaseCapacity()
		{
			int newSize = HashHelpers.ExpandPrime(_count);
			if(newSize <= _count) throw new Exception(); //overflow

			_Slot[] newSlots = new _Slot[newSize];
			if(_slots != null) {
				Array.Copy(_slots, 0, newSlots, 0, _lastIndex);
			}

			int[] newBuckets = new int[newSize];
			for(int i = 0; i < _lastIndex; i++) {
				int bucket = newSlots[i].hashCode % newSize;
				newSlots[i].next = newBuckets[bucket] - 1;
				newBuckets[bucket] = i + 1;
			}
			_slots = newSlots;
			_buckets = newBuckets;
		}

		struct _Slot
		{
			internal int hashCode;      // Lower 31 bits of hash code, -1 if unused
			internal int next;          // Index of next entry, -1 if last
			internal string value;
		}
		//The int[] _buckets and _Slot[] _slots arrays take 20 bytes for each string (64-bit).
		//	A .NET string object takes 24+length*2+2 bytes (64-bit).
		//	Assuming that average string length is 12 characters, average string object memory size is 50. So we add 40%.


		internal static class HashHelpers
		{
			public static readonly int[] primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

			public static bool IsPrime(int candidate)
			{
				if((candidate & 1) != 0) {
					int limit = (int)Math.Sqrt(candidate);
					for(int divisor = 3; divisor <= limit; divisor += 2) {
						if((candidate % divisor) == 0)
							return false;
					}
					return true;
				}
				return (candidate == 2);
			}

			public static int GetPrime(int min)
			{
				for(int i = 0; i < primes.Length; i++) {
					int prime = primes[i];
					if(prime >= min) return prime;
				}

				//outside of our predefined table. 
				//compute the hard way. 
				for(int i = (min | 1); i < Int32.MaxValue; i += 2) {
					if(IsPrime(i) && ((i - 1) % 101 != 0))
						return i;
				}
				return min;
			}

			public static int GetMinPrime()
			{
				return primes[0];
			}

			// Returns size of hashtable to grow to.
			public static int ExpandPrime(int oldSize)
			{
				int newSize = 2 * oldSize;

				// Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
				// Note that this check works even when _items.Length overflowed thanks to the (uint) cast
				if((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize) {
					return MaxPrimeArrayLength;
				}

				return GetPrime(newSize);
			}

			// This is the maximum prime smaller than Array.MaxArrayLength
			public const int MaxPrimeArrayLength = 0x7FEFFFFD;
		}

		/// <summary>
		/// Gets string cached in a weak-referenced internal static LibStringCache object.
		/// Locks and calls <see cref="Add"/>.
		/// </summary>
		/// <param name="p">Unmanaged string.</param>
		/// <param name="len">p length.</param>
		/// <param name="lenIsMaxLen">Call <c>len = LibCharPtr.Length(p, len)</c>.</param>
		internal static string LibAdd(char* p, int len, bool lenIsMaxLen = false)
		{
			if(lenIsMaxLen) len = LibCharPtr.Length(p, len);
			lock(_cacheWR) {
				if(!_cacheWR.TryGetTarget(out var cache)) _cacheWR.SetTarget(cache = new StringCache());
				return cache.Add(p, len);
			}
		}
		static WeakReference<StringCache> _cacheWR = new WeakReference<StringCache>(null);

		//internal static int LibDebugStringCount => _cacheWR.TryGetTarget(out var v) ? v.Count : 0;

		/// <summary>
		/// Gets string cached in a weak-referenced internal static LibStringCache object.
		/// Locks and calls <see cref="Add"/>.
		/// </summary>
		/// <param name="a">Text buffer.</param>
		/// <param name="len">Length of valid text in a.</param>
		internal static string LibAdd(char[] a, int len)
		{
			Debug.Assert(len < a.Length);
			fixed (char* p = a) return LibAdd(p, len);
		}

		/// <summary>
		/// This overload calls LibCharPtr.Length.
		/// </summary>
		internal static string LibAdd(char* p)
		{
			return LibAdd(p, LibCharPtr.Length(p));
		}

		/// <summary>
		/// This overload calls LibCharPtr.Length.
		/// </summary>
		internal static string LibAdd(char[] a)
		{
			fixed (char* p = a) {
				int len = LibCharPtr.Length(p, a.Length);
				Debug.Assert(len < a.Length); //one char for '\0'
				return LibAdd(p, len);
			}
		}

		/// <summary>
		/// Gets substring of string s as a string cached in a weak-referenced internal static LibStringCache object.
		/// Locks and calls <see cref="Add"/>.
		/// </summary>
		/// <param name="s">The string containing the substring. Can be null/"" if other parameters are 0.</param>
		/// <param name="startIndex">Start of substring in s.</param>
		/// <param name="count">Length of substring.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		internal static string LibAdd(string s, int startIndex, int count)
		{
			int len = s?.Length ?? 0;
			if((uint)startIndex > len || count < 0 || startIndex + count > len) throw new ArgumentOutOfRangeException();
			if(len == 0) return s;
			fixed (char* p = s) return LibAdd(p + startIndex, count);
		}
	}
}
