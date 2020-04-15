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
using System.Security.Cryptography;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Data hash functions.
	/// </summary>
	public unsafe class AHash
	{
		#region FNV1

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int Fnv1(string data)
		{
			if(data == null) return 0;
			fixed (char* p = data) return Fnv1(p, data.Length);
		}

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int Fnv1(char* data, int lengthChars)
		{
			uint hash = 2166136261;

			for(int i = 0; i < lengthChars; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;

			//note: using i is slightly faster than pointers. Compiler knows how to optimize.
		}

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int Fnv1(byte[] data)
		{
			if(data == null) return 0;
			fixed (byte* p = data) return Fnv1(p, data.Length);
		}

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int Fnv1(byte* data, int lengthBytes)
		{
			uint hash = 2166136261;

			for(int i = 0; i < lengthBytes; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;

			//note: could be void* data, then callers don't have to cast other types to byte*, but then can accidentally pick wrong overload when char*. Also now it's completely clear that it hashes bytes, not the passed type directly (like the char* overload does).
		}

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int Fnv1<T>(T data) where T : unmanaged
				=> Fnv1((byte*)&data, sizeof(T));

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long Fnv1Long(string data)
		{
			if(data == null) return 0;
			fixed (char* p = data) return Fnv1Long(p, data.Length);
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long Fnv1Long(char* data, int lengthChars)
		{
			ulong hash = 14695981039346656037;

			for(int i = 0; i < lengthChars; i++)
				hash = (hash * 1099511628211) ^ data[i];

			return (long)hash;

			//speed: ~4 times slower than 32-bit
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long Fnv1Long(byte[] data)
		{
			if(data == null) return 0;
			fixed (byte* p = data) return Fnv1Long(p, data.Length);
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long Fnv1Long(byte* data, int lengthBytes)
		{
			ulong hash = 14695981039346656037;

			for(int i = 0; i < lengthBytes; i++)
				hash = (hash * 1099511628211) ^ data[i];

			return (long)hash;
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long Fnv1Long<T>(T data) where T : unmanaged
				=> Fnv1Long((byte*)&data, sizeof(T));

		/// <summary>
		/// FNV-1 hash, modified to make faster with long strings (then takes every n-th character).
		/// </summary>
		public static int Fast(char* data, int lengthChars)
		{
			//Also we take the last 1-2 characters (in the second loop), because often there are several strings like Chrome_WidgetWin_0, Chrome_WidgetWin_1...
			//Also we hash uints, not chars, unless the string is very short.
			//Tested speed with 400 unique strings (window/control names/classnames/programnames). The time was 7 mcs. For single call 17 ns.

			uint hash = 2166136261;
			int i = 0;

			if(lengthChars > 8) {
				int lc = lengthChars--;
				lengthChars /= 2; //we'll has uints, not chars
				int every = lengthChars / 8 + 1;

				for(; i < lengthChars; i += every)
					hash = (hash * 16777619) ^ ((uint*)data)[i];

				i = lengthChars * 2;
				lengthChars = lc;
			}

			for(; i < lengthChars; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;
		}

		/// <summary>
		/// FNV-1 hash, modified to make faster with long strings (then takes every n-th character).
		/// </summary>
		/// <param name="s">The string to hash. Can be null.</param>
		public static int Fast(string s)
		{
			if(s == null) return 0;
			fixed (char* p = s) return Fast(p, s.Length);
		}

		/// <summary>
		/// FNV-1 hash, modified to make faster with long strings (then takes every n-th character).
		/// This overload hashes a substring.
		/// </summary>
		/// <param name="s">The string containing the substring. Can be null/"" if other parameters are 0.</param>
		/// <param name="startIndex">Start of substring in s.</param>
		/// <param name="count">Length of substring.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static int Fast(string s, int startIndex, int count)
		{
			int len = s?.Length ?? 0;
			if((uint)startIndex > len || (uint)count > len - startIndex) throw new ArgumentOutOfRangeException();
			if(s == null) return 0;
			fixed (char* p = s) return Fast(p + startIndex, count);
		}

		#endregion

		#region MD5 fast

		/// <summary>
		/// Computes MD5 hash of some data.
		/// Multiple datas can be hashed, producing single result.
		/// Call <b>Add</b> one or more times. Finally use <see cref="Hash"/> to get result.
		/// </summary>
		/// <remarks>
		/// Faster than the .NET MD5 hash functions.
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		public struct MD5 //MD5_CTX + _state
		{
			[FieldOffset(88)] MD5Result _result;
			[FieldOffset(104)] long _state; //1 inited/added, 2 finalled

			/// <summary>
			/// true if no data was added.
			/// </summary>
			public bool IsEmpty => _state == 0;

			/// <summary>Adds data.</summary>
			/// <exception cref="ArgumentNullException">data is null.</exception>
			/// <exception cref="ArgumentOutOfRangeException">size &lt; 0.</exception>
			public void Add(void* data, int size)
			{
				if(data == null) throw new ArgumentNullException();
				if(size < 0) throw new ArgumentOutOfRangeException();
				if(_state != 1) { Api.MD5Init(out this); _state = 1; }
				Api.MD5Update(ref this, data, size);
			}

			/// <summary>Adds data.</summary>
			public void Add<T>(T data) where T : unmanaged
				=> Add(&data, sizeof(T));

			/// <summary>Adds data.</summary>
			/// <exception cref="ArgumentNullException">data is null.</exception>
			public void Add(byte[] data)
			{
				fixed (byte* p = data) Add(p, data?.Length ?? 0);
			}

			/// <summary>Adds data.</summary>
			/// <exception cref="ArgumentNullException">data is null.</exception>
			/// <remarks>Adds UTF8 bytes, not UTF16 characters.</remarks>
			public void Add(string data) => Add(Encoding.UTF8.GetBytes(data));

			//rejected. Better use unsafe address, then will not need to copy data.
			///// <summary>Adds data.</summary>
			//public void Add<T>(T data) where T: unmanaged
			//{
			//	Add(&data, sizeof(T));
			//}

			/// <summary>
			/// Computes final hash of datas added with <b>Add</b>.
			/// </summary>
			/// <exception cref="InvalidOperationException"><b>Add</b> was not called.</exception>
			/// <remarks>
			/// Resets state, so that if <b>Add</b> called again, it will start adding new datas.
			/// </remarks>
			public MD5Result Hash {
				get {
					if(_state != 2) {
						if(_state != 1) throw new InvalidOperationException();
						Api.MD5Final(ref this);
						_state = 2;
					}
					return _result;
				}
			}
		}

		/// <summary>
		/// Result of <see cref="MD5.Hash"/>.
		/// It is 16 bytes stored in 2 long fields r1 and r2.
		/// If need, can be converted to byte[] with <see cref="ToArray"/> or to hex string with <see cref="ToString"/>.
		/// </summary>
		[Serializable]
		public struct MD5Result : IEquatable<MD5Result>
		{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
			public long r1, r2;

			public bool Equals(MD5Result other) => this == other;

			public static bool operator ==(MD5Result h1, MD5Result h2) => h1.r1 == h2.r1 && h1.r2 == h2.r2;
			public static bool operator !=(MD5Result h1, MD5Result h2) => !(h1 == h2);

			//rejected. Not much shorter than hex.
			//public string ToBase64() => Convert.ToBase64String(ToArray());

			public override int GetHashCode() => r1.GetHashCode() ^ r2.GetHashCode();

			public override bool Equals(object obj) => obj is MD5Result && (MD5Result)obj == this;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

			/// <summary>
			/// Converts this to hex string of Length = 32.
			/// </summary>
			public override string ToString() => AConvert.HexEncode(this);

			/// <summary>
			/// Converts this to byte[16].
			/// </summary>
			public byte[] ToArray()
			{
				var r = new byte[16];
				fixed (byte* p = r) {
					*(long*)p = r1;
					*(long*)(p + 8) = r2;
				}
				return r;
			}

			/// <summary>
			/// Creates <b>MD5Result</b> from hex string returned by <see cref="ToString"/>.
			/// Returns false if <i>encoded</i> is invalid.
			/// </summary>
			public static bool FromString(ReadOnlySpan<char> encoded, out MD5Result r) => AConvert.HexDecode(encoded, out r);
		}

		#endregion

		#region other

		/// <summary>
		/// Computes binary data hash using the specified cryptographic algorithm.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		public static byte[] Crypto(byte[] a, string algorithm)
		{
			using var x = (HashAlgorithm)CryptoConfig.CreateFromName(algorithm);
			return x.ComputeHash(a);
		}

		/// <summary>
		/// Computes string hash using the specified cryptographic algorithm.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		/// <remarks>Uses UTF8 bytes, not UTF16 bytes.</remarks>
		public static byte[] Crypto(string s, string algorithm)
		{
			return Crypto(Encoding.UTF8.GetBytes(s), algorithm);
		}

		/// <summary>
		/// Computes binary data hash using the specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		public static string CryptoHex(byte[] a, string algorithm)
		{
			return AConvert.HexEncode(Crypto(a, algorithm));
		}

		/// <summary>
		/// Computes string hash using the specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		/// <remarks>Uses UTF8 bytes, not UTF16 bytes.</remarks>
		public static string CryptoHex(string s, string algorithm)
		{
			return AConvert.HexEncode(Crypto(s, algorithm));
		}

		#endregion

	}
}
