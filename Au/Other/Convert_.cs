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
//using System.Xml.Linq;
using System.IO.Compression;
using System.Security.Cryptography;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Data conversion functions - hash, compress, hex-encode, Base64, UTF8.
	/// </summary>
	public unsafe class Convert_
	{
		#region hash FNV1

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int HashFnv1(string data)
		{
			if(data == null) return 0;
			fixed (char* p = data) return HashFnv1(p, data.Length);
		}

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int HashFnv1(char* data, int lengthChars)
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
		public static int HashFnv1(byte[] data)
		{
			if(data == null) return 0;
			fixed (byte* p = data) return HashFnv1(p, data.Length);
		}

		/// <summary>
		/// 32-bit FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int HashFnv1(byte* data, int lengthBytes)
		{
			uint hash = 2166136261;

			for(int i = 0; i < lengthBytes; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;

			//note: could be void* data, then callers don't have to cast other types to byte*, but then can accidentally pick wrong overload when char*. Also now it's completely clear that it hashes bytes, not the passed type directly (like the char* overload does).
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long HashFnv1_64(string data)
		{
			if(data == null) return 0;
			fixed (char* p = data) return HashFnv1_64(p, data.Length);
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long HashFnv1_64(char* data, int lengthChars)
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
		public static long HashFnv1_64(byte[] data)
		{
			if(data == null) return 0;
			fixed (byte* p = data) return HashFnv1_64(p, data.Length);
		}

		/// <summary>
		/// 64-bit FNV-1 hash.
		/// </summary>
		public static long HashFnv1_64(byte* data, int lengthBytes)
		{
			ulong hash = 14695981039346656037;

			for(int i = 0; i < lengthBytes; i++)
				hash = (hash * 1099511628211) ^ data[i];

			return (long)hash;
		}

		/// <summary>
		/// FNV-1 hash, modified to make faster with long strings (then takes every n-th character).
		/// </summary>
		public static int HashFast(char* data, int lengthChars)
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
		public static int HashFast(string s)
		{
			if(s == null) return 0;
			fixed (char* p = s) return HashFast(p, s.Length);
		}

		/// <summary>
		/// FNV-1 hash, modified to make faster with long strings (then takes every n-th character).
		/// This overload hashes a substring.
		/// </summary>
		/// <param name="s">The string containing the substring. Can be null/"" if other parameters are 0.</param>
		/// <param name="startIndex">Start of substring in s.</param>
		/// <param name="count">Length of substring.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static int HashFast(string s, int startIndex, int count)
		{
			int len = s?.Length ?? 0;
			if((uint)startIndex > len || (uint)count > len - startIndex) throw new ArgumentOutOfRangeException();
			if(s == null) return 0;
			fixed (char* p = s) return HashFast(p + startIndex, count);
		}

		#endregion

		#region MD5 fast

		/// <summary>
		/// Computes MD5 hash of some data.
		/// Multiple datas can be hashed, producing single result.
		/// Call <b>Add</b> one or more times. Finally use <see cref="Hash"/> to get result.
		/// </summary>
		/// <remarks>
		/// Faster than the static <b>Hash</b> functions.
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		public struct MD5Hash //MD5_CTX + _state
		{
			[FieldOffset(88)] MD5HashResult _result;
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
			public MD5HashResult Hash {
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
		/// Result of <see cref="MD5Hash.Hash"/>.
		/// It is 16 bytes stored in 2 long fields r1 and r2.
		/// If need, can be converted to byte[] with <see cref="ToArray"/> or to hex string with <see cref="ToString"/>.
		/// </summary>
		[Serializable]
		public struct MD5HashResult :IEquatable<MD5HashResult>
		{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
			public long r1, r2;

			public bool Equals(MD5HashResult other) => this == other;

			public static bool operator ==(MD5HashResult h1, MD5HashResult h2) => h1.r1 == h2.r1 && h1.r2 == h2.r2;
			public static bool operator !=(MD5HashResult h1, MD5HashResult h2) => !(h1 == h2);

			//rejected: not much shorter than hex.
			//public string ToBase64() => Convert.ToBase64String(ToArray());

			public override int GetHashCode() => r1.GetHashCode() ^ r2.GetHashCode();

			public override bool Equals(object obj) => obj is MD5HashResult && (MD5HashResult)obj == this;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

			/// <summary>
			/// Converts this to hex string of Length = 32.
			/// </summary>
			public override unsafe string ToString()
			{
				var t = this;
				return HexEncode(&t, sizeof(MD5HashResult));
			}

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
			/// Creates MD5HashResult from hex string returned by <see cref="ToString"/>.
			/// Returns false if some parameter is invalid. Does not throw exception.
			/// </summary>
			public static bool FromString(string s, int startIndex, int length, out MD5HashResult r)
			{
				if(s == null || length != 32 || (uint)startIndex > s.Length - 32) { r = default; return false; }
				MD5HashResult rr;
				HexDecode(s, &rr, 16, startIndex);
				r = rr;
				return true;
			}
		}

		#endregion

		#region hash other

		/// <summary>
		/// Computes binary data hash using the specified cryptographic algorithm.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		public static byte[] Hash(byte[] a, string algorithm)
		{
			using(var x = (HashAlgorithm)CryptoConfig.CreateFromName(algorithm)) {
				return x.ComputeHash(a);
			}
		}

		/// <summary>
		/// Computes string hash using the specified cryptographic algorithm.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		/// <remarks>Uses UTF8 bytes, not UTF16 bytes.</remarks>
		public static byte[] Hash(string s, string algorithm)
		{
			return Hash(Encoding.UTF8.GetBytes(s), algorithm);
		}

		/// <summary>
		/// Computes binary data hash using the specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		public static string HashHex(byte[] a, string algorithm)
		{
			return HexEncode(Hash(a, algorithm));
		}

		/// <summary>
		/// Computes string hash using the specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". See <see cref="CryptoConfig"/>.</param>
		/// <remarks>Uses UTF8 bytes, not UTF16 bytes.</remarks>
		public static string HashHex(string s, string algorithm)
		{
			return HexEncode(Hash(s, algorithm));
		}

		#endregion

		#region hex encode

		/// <summary>
		/// Converts binary data stored in byte[] to hex-encoded string.
		/// </summary>
		/// <param name="a">The data. Can be null, then returns null.</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		/// <remarks>
		/// The result string length is 2 * array length.
		/// In most cases it's better to use <see cref="Convert.ToBase64String(byte[])"/>, then result is 4/3 of array length. Both functions are fast.
		/// </remarks>
		public static string HexEncode(byte[] a, bool upperCase = false)
		{
			if(a == null) return null;
			fixed (byte* p = a) {
				return HexEncode(p, a.Length, upperCase);
			}
		}

		/// <summary>
		/// Converts binary data stored in any memory to hex-encoded string.
		/// </summary>
		/// <param name="data">The data. Can be any valid memory of specified size, for example a struct address. Can be null, then returns null.</param>
		/// <param name="size">data memory size (bytes).</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		public static string HexEncode(void* data, int size, bool upperCase = false)
		{
			if(data == null) return null;
			byte* a = (byte*)data;
			int i;
			string R = new string('\0', size * 2);
			fixed (char* p = R) {
				if(upperCase) {
					for(i = 0; i < size; i++) {
						p[i * 2] = _HalfByteToHexCharU(a[i] >> 4);
						p[i * 2 + 1] = _HalfByteToHexCharU(a[i] & 0xf);
					}
				} else {
					for(i = 0; i < size; i++) {
						p[i * 2] = _HalfByteToHexCharL(a[i] >> 4);
						p[i * 2 + 1] = _HalfByteToHexCharL(a[i] & 0xf);
					}
				}
			}
			return R;

			char _HalfByteToHexCharU(int b)
			{
				return (char)((b < 10 ? '0' : 'A' - 10) + b);
			}

			char _HalfByteToHexCharL(int b)
			{
				return (char)((b < 10 ? '0' : 'a' - 10) + b);
			}

			//speed: fast enough without a lookup table.
			//CryptBinaryToString speed: 50 times slower.
		}

		/// <summary>
		/// Converts hex-encoded string to binary data. Stores it in byte[].
		/// </summary>
		/// <param name="s">Hex-encoded string. Can be null, then returns null.</param>
		/// <remarks>
		/// Skips spaces and other non-hex-digit characters. Example: "01 23 46 67" is the same as "01234667".
		/// The number of hex-digit characters should be divisible by 2, else the last character is ignored.
		/// </remarks>
		/// <seealso cref="Base64Decode(string)"/>
		public static byte[] HexDecode(string s)
		{
			if(s == null) return null;
			int n = s.Length / 2;
			var b = Util.Buffers.LibChar(n);
			fixed (char* p = b.A) {
				n = HexDecode(s, p, n, 0);
				var r = new byte[n];
				Marshal.Copy((IntPtr)p, r, 0, n);
				//var b1 = (byte*)p; for(int i = 0; i < n; i++) r[i] = b1[i]; //faster when array small, slower when big
				return r;
			}

			//These functions have similar speed as the Base64 functions. Slower for the same binary data size because then the hex string is significantly longer.
		}

		/// <summary>
		/// Converts hex-encoded string to binary data. Stores it in caller's memory buffer.
		/// Returns the number of bytes stored in resultBuffer memory. It is equal or less than Math.Min(bufferSize, (s.Length - stringStartIndex) / 2).
		/// </summary>
		/// <param name="s">Hex-encoded string. Can be null.</param>
		/// <param name="resultBuffer">Where to write the data. Can be any valid memory of specified size, for example a struct address.</param>
		/// <param name="bufferSize">resultBuffer memory size. Note: this function will damage process memory if using bad resultBuffer or bufferSize.</param>
		/// <param name="stringStartIndex">0 or index of first character of hex-encoded substring in s.</param>
		/// <remarks>
		/// Skips spaces and other non-hex-digit characters. Example: "01 23 46 67" is the same as "01234667".
		/// The number of hex-digit characters should be divisible by 2, else the last character is ignored.
		/// </remarks>
		public static int HexDecode(string s, void* resultBuffer, int bufferSize, int stringStartIndex = 0)
		{
			if(s == null) return 0;
			fixed (char* p = s) return HexDecode(p + stringStartIndex, s.Length - stringStartIndex, resultBuffer, bufferSize);
		}

		/// <summary>
		/// The same as <see cref="HexDecode(string, void*, int, int)"/> but the string can be an unmanaged UTF-16 string.
		/// </summary>
		/// <param name="s">UTF-16 string.</param>
		/// <param name="length">s length (characters).</param>
		/// <param name="resultBuffer"></param>
		/// <param name="bufferSize"></param>
		public static int HexDecode(char* s, int length, void* resultBuffer, int bufferSize)
		{
			var t = Util.LibTables.Hex;

			if(s == null || length < 1) return 0;
			byte* r = (byte*)resultBuffer, rTo = r + bufferSize;
			uint k = 1;
			for(char* sTo = s + length; s < sTo; s++) {
				uint c = (uint)(s[0] - '0'); if(c >= 55) continue;
				c = t[c]; if(c == 0xFF) continue;
				k <<= 4;
				k |= c;
				if(0 != (k & 0x100)) {
					if(r >= rTo) break;
					*r++ = (byte)k;
					k = 1;
				}
			}
			//if(k != 1 && r < rTo) *r++ = (byte)(k & 0xF); //not good. What to do with it? Programmers should learn to not use half-byte in hex string.

			return (int)(r - (byte*)resultBuffer);

			//info: this is very unsafe.
			//	Cannot use generic because C# does not allow to get generic parameter address.
			//	Cannot use GCHandle.AddrOfPinnedObject because it gets address of own copy of the variable.

			//speed: slightly slower than Base64Decode for the same binary data size.
		}

		////this version is 35% faster, but does not support split bytes like "A B", must be "AB". Eg user may want to wrap at any width.
		//public static int HexDecode2(char* s, int length, void* data, int size)
		//{
		//	var t = Util.LibTables.Hex;

		//	if(s == null || length < 2) return 0;
		//	char* sTo = s + length - 1;
		//	byte* r = (byte*)data, rTo = r + size;
		//	while(s < sTo) {
		//		uint k1 = (uint)(s[0] - '0'); if(k1 >= 55) continue;
		//		k1 = t[k1]; if(k1 == 0xFF) continue;
		//		uint k2 = (uint)(s[1] - '0'); if(k2 >= 55) continue;
		//		k2 = t[k2]; if(k2 == 0xFF) continue;
		//		s += 2;
		//		if(r >= rTo) break;
		//		*r++ = (byte)((k1 << 4) | k2);
		//	}
		//	return (int)(r - (byte*)data);

		//	//info: this is very unsafe.
		//	//	Cannot use generic because C# does not allow to get generic parameter address.
		//	//	Cannot use GCHandle.AddrOfPinnedObject because it gets address of own copy of the variable.
		//}

		/// <summary>
		/// Converts Guid struct to hex string without {}-.
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		public static unsafe string GuidToHex(Guid guid, bool upperCase = false)
		{
			return HexEncode(&guid, sizeof(Guid), upperCase);
		}

		/// <summary>
		/// Converts hex GUID string to Guid struct.
		/// Returns false if s is null or not a hex string of 32 length.
		/// </summary>
		public static unsafe bool GuidFromHex(string s, out Guid guid)
		{
			Guid g;
			if(s != null && s.Length == 32)
				fixed (char* p = s) {
					if(HexDecode(p, 32, &g, sizeof(Guid)) == sizeof(Guid)) { guid = g; return true; }
				}
			guid = default;
			return false;
		}

		/// <summary>
		/// Converts hex GUID string to Guid struct.
		/// </summary>
		/// <exception cref="ArgumentException">guid is null or not hex string of 32 length.</exception>
		public static unsafe Guid GuidFromHex(string s)
		{
			if(GuidFromHex(s, out var g)) return g;
			throw new ArgumentException("expected hex GUID string of 32 length");
		}

		#endregion

		#region base64

		//currently not used
		///// <summary>
		///// Converts Base64 ANSI string to binary data (bytes). Stores it in caller's memory buffer.
		///// Returns the number of bytes stored in resultBuffer.
		///// </summary>
		///// <param name="s">Base64 string. Can be null if len is 0.</param>
		///// <param name="len">s length (bytes).</param>
		///// <param name="resultBuffer">A memory buffer for the result bytes. Must be of at least bufferSize size, else this function will damage process memory.</param>
		///// <param name="bufferSize">resultBuffer buffer length (bytes). Must be at least (int)(len * 3L / 4), else exception.</param>
		///// <remarks>
		///// Allows (discards) non-base64 characters.
		///// Supports URL-safe characters: '-' for '+' and '_' for '/'.
		///// </remarks>
		///// <exception cref="ArgumentException">bufferSize too small.</exception>
		//public static unsafe int Base64Decode(byte* s, int len, void* resultBuffer, int bufferSize)
		//{
		//	if(bufferSize < len * 3L / 4) throw new ArgumentException("bufferSize too small");
		//	byte* r = (byte*)resultBuffer;
		//	byte* t = Util.LibTables.Base64;

		//	var sTo = s + len;
		//	uint x = 0x80;
		//	while(s < sTo) {
		//		byte ch = *s++;
		//		if(ch > 'z') continue;
		//		uint sixBits = t[ch];
		//		if(sixBits == 0xff) continue;
		//		x <<= 6;
		//		x |= sixBits;

		//		if(0 != (x & 0x80000000)) {
		//			r[0] = (byte)(x >> 16);
		//			r[1] = (byte)(x >> 8);
		//			r[2] = (byte)(x);
		//			r += 3;
		//			x = 0x80;
		//		}
		//	}

		//	if(x != 0x80) {
		//		if(0 != (x & 0x2000000)) { //the last 3 chars -> 2 bytes
		//			r[0] = (byte)(x >> 10); //x contains 18 bits
		//			r[1] = (byte)(x >> 2);
		//			r += 2;
		//		} else if(0 != (x & 0x80000)) { //the last 2 chars -> 1 byte
		//			r[0] = (byte)(x >> 4); //x contains 12 bits
		//			r += 1;
		//		}
		//	}
		//	return (int)(r - (byte*)resultBuffer);
		//}

		/// <summary>
		/// Converts Base64 UTF-16 string to binary data (bytes). Stores it in caller's memory buffer.
		/// Returns the number of bytes stored in resultBuffer.
		/// </summary>
		/// <param name="s">Base64 string. Can be null if len is 0.</param>
		/// <param name="len">s length (characters).</param>
		/// <param name="resultBuffer">A memory buffer for the result bytes. Must be of at least bufferSize size, else this function will damage process memory.</param>
		/// <param name="bufferSize">resultBuffer buffer size (bytes). Must be at least (int)(len * 3L / 4), else exception.</param>
		/// <exception cref="ArgumentException">bufferSize too small.</exception>
		/// <remarks><inheritdoc cref="Base64Decode(string)"/></remarks>
		public static unsafe int Base64Decode(char* s, int len, void* resultBuffer, int bufferSize)
		{
			if(bufferSize < len * 3L / 4) throw new ArgumentException("bufferSize too small");
			byte* r = (byte*)resultBuffer;
			byte* t = Util.LibTables.Base64;

			var sTo = s + len;
			uint x = 0x80;
			while(s < sTo) {
				int ch = *s++;
				if(ch > 'z') continue;
				uint sixBits = t[(byte)ch];
				if(sixBits == 0xff) continue;
				x <<= 6;
				x |= sixBits;

				if(0 != (x & 0x80000000)) {
					r[0] = (byte)(x >> 16);
					r[1] = (byte)(x >> 8);
					r[2] = (byte)(x);
					r += 3;
					x = 0x80;
				}
			}

			if(x != 0x80) {
				if(0 != (x & 0x2000000)) { //the last 3 chars -> 2 bytes
					r[0] = (byte)(x >> 10); //x contains 18 bits
					r[1] = (byte)(x >> 2);
					r += 2;
				} else if(0 != (x & 0x80000)) { //the last 2 chars -> 1 byte
					r[0] = (byte)(x >> 4); //x contains 12 bits
					r += 1;
				}
			}
			return (int)(r - (byte*)resultBuffer);

			//speed: slightly faster than HexDecode for the same binary data size.
			//	Near 2 times faster than the .NET function, when don't need to allocate buffer for result.

			//tested CryptBinaryToString speed: 5 times slower than Convert.ToBase64String.
		}

		/// <summary>
		/// Converts Base64 string to binary data (bytes).
		/// Returns byte[] containing the data.
		/// </summary>
		/// <param name="s">Base64 string. Can be null, then returns null.</param>
		/// <remarks>
		/// How this function is different than <see cref="Convert.FromBase64String"/>:
		/// 1. Allows (discards) non-base64 characters.
		/// 2. Supports URL-safe characters: '-' for '+' and '_' for '/'.
		/// 3. No exception when string length is not multiple of 4, eg if does not contain '=' characters for padding. 
		/// 
		/// This library does not have Base64-encode functions. Use <see cref="Convert.ToBase64String(byte[])"/>.
		/// </remarks>
		public static unsafe byte[] Base64Decode(string s)
		{
			if(s == null) return null;
			fixed (char* p = s) {
				int len = s.Length, n = (int)(len * 3L / 4);
				fixed (byte* b = Util.Buffers.LibByte(n)) {
					n = Base64Decode(p, len, b, n);
					var r = new byte[n];
					Marshal.Copy((IntPtr)b, r, 0, n);
					return r;
				}
			}
		}

		//rejected, because such GUID is case-sensitive, but filenames usually aren't.
		///// <summary>
		///// Converts GUID to string that can be used in file paths and URLs.
		///// </summary>
		///// <param name="guid"></param>
		///// <remarks>
		///// Standard Base64 strings cannot be used in file paths and URLs, because can contain characters '/' and '+'. This function replaces them with '_' and '-'. Also trims "==" at the end.
		///// Such string can be parsed with <b>Convert_</b> class functions, not with <b>Convert</b> class functions.
		///// </remarks>
		//public static string GuidToBase64Filename(in Guid guid)
		//{
		//	return Convert.ToBase64String(guid.ToByteArray()).TrimEnd('=').Replace('/', '_').Replace('+', '-');
		//}

		#endregion

		#region compress

		/// <summary>
		/// Compresses data using <see cref="DeflateStream"/>.
		/// </summary>
		/// <param name="data"></param>
		/// <exception cref="Exception">Exceptions of DeflateStream.</exception>
		public static byte[] Compress(byte[] data)
		{
			using(MemoryStream memoryStream = new MemoryStream()) {
				using(DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress)) {
					deflateStream.Write(data, 0, data.Length);
				}
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Decompresses data using <see cref="DeflateStream"/>.
		/// Returns byte[] containing decompressed data.
		/// </summary>
		/// <param name="compressedData">Compressed data.</param>
		/// <param name="index">Start index of compressed data in the compressedData array.</param>
		/// <param name="count">Length of compressed data in the compressedData array.</param>
		/// <exception cref="Exception">Exceptions of DeflateStream.</exception>
		public static byte[] Decompress(byte[] compressedData, int index = 0, int count = -1)
		{
			using(var stream = new MemoryStream()) {
				Decompress(stream, compressedData, index, count);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Decompresses data using <see cref="DeflateStream"/>.
		/// Writes the decompressed data to a caller-provided memory stream.
		/// </summary>
		/// <param name="streamForDecompressedData">A memory stream where this function will write decompressed data. See example.</param>
		/// <param name="compressedData">Compressed data.</param>
		/// <param name="index">Start index of compressed data in the compressedData array.</param>
		/// <param name="count">Length of compressed data in the compressedData array.</param>
		/// <exception cref="Exception">Exceptions of DeflateStream.</exception>
		/// <example>
		/// This code is used by the other Decompress overload.
		/// <code><![CDATA[
		/// using(var stream = new MemoryStream()) {
		/// 	Decompress(stream, compressedData, index, count);
		/// 	return stream.ToArray();
		/// }
		/// ]]></code>
		/// </example>
		public static void Decompress(Stream streamForDecompressedData, byte[] compressedData, int index = 0, int count = -1)
		{
			if(count < 0) count = compressedData.Length - index;
			using(MemoryStream compressStream = new MemoryStream(compressedData, index, count, false)) {
				using(DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress)) {
					deflateStream.CopyTo(streamForDecompressedData);
				}
			}

			//note: cannot deflateStream.Read directly to array because its Length etc are not supported.
			//note: also cannot use decompressedStream.GetBuffer because it can be bigger.
		}

		#endregion

		#region utf8

		/// <summary>
		/// Returns the number of bytes that would by produced by converting C# string to UTF8, not including the terminating '\0' character.
		/// </summary>
		/// <param name="s">C# string (UTF16). Can be null.</param>
		public static int Utf8LengthFromString(string s)
		{
			if(Empty(s)) return 0;
			return Api.WideCharToMultiByte(Api.CP_UTF8, 0, s, s.Length, null, 0, default, null);
		}

		/// <summary>
		/// Converts C# string to '\0'-terminated UTF8 string. Stores the UTF8 string in caller's buffer.
		/// Returns UTF8 string length in bytes, not including the terminating '\0' character.
		/// If fails (unlikely if passed correct arguments), returns 0 and sets buffer="".
		/// </summary>
		/// <param name="s">C# string (UTF16). Can be null.</param>
		/// <param name="buffer">Caller-allocated memory of bufLen length.</param>
		/// <param name="bufLen">buffer length in bytes. Should be at least <see cref="Utf8LengthFromString"/>+1, else converts only part of string. The maximal possible required buffer length for whole string can be s.Length*3+1.</param>
		/// <remarks>
		/// Calls API <msdn>WideCharToMultiByte</msdn>.
		/// This is the most low-level and therefore fastest overload. Does not allocate any memory. With big strings much faster than .NET Encoding class functions.
		/// Use buffer/bufLen carefully. If bufLen is greater that the memory buffer length, other memory will be overwritten/damaged.
		/// </remarks>
		public static int Utf8FromString(string s, byte* buffer, int bufLen)
		{
			if(buffer == null || bufLen <= 0) return 0; //use Utf8ToStringLength for it
			int n = Empty(s) ? 0 : Api.WideCharToMultiByte(Api.CP_UTF8, 0, s, s.Length, buffer, bufLen, default, null);
			buffer[Math.Min(n, bufLen - 1)] = 0;
			return n;
		}

		/// <summary>
		/// Converts C# string to '\0'-terminated UTF8 string.
		/// </summary>
		/// <param name="s">C# string (UTF16). If null, returns null.</param>
		/// <remarks>
		/// How this is different from .NET Encoding class functions: 1. Uses <msdn>WideCharToMultiByte</msdn>. 2. Faster with big strings. 3. The returned string is '\0'-terminated. 4. No exceptions (unless the string is so large that fails to allocate so much memory).
		/// </remarks>
		public static byte[] Utf8FromString(string s)
		{
			if(s == null) return null;
			var len = s.Length;
			if(len == 0) return new byte[] { 0 };
			int n = Utf8LengthFromString(s) + 1;
			var a = new byte[n];
			fixed (byte* p = a) {
				var r = Utf8FromString(s, p, n);
				Debug.Assert(r + 1 == n);
			}
			return a;

			//tried to optimize: if not too long, use an optimized intermediate buffer to avoid Utf8ToStringLength. Can make faster by max 20%. Not worth.
			//Now with short strings this is slower than .NET. Becomes faster when string length is > 50. More than 2 times faster when string length 1000.
		}

		/// <summary>
		/// Converts C# string to '\0'-terminated UTF8 string managed by a WeakReference variable.
		/// </summary>
		/// <param name="s">C# string (UTF16). If null, returns null, unless allocExtraBytes is not 0.</param>
		/// <param name="buffer">A WeakReference variable (probably [ThreadStatic]) that manages the returned array. If null, this function will create it.</param>
		/// <param name="utf8Length">If not null, receives UTF8 text length, not including '\0' and allocExtraBytes.</param>
		/// <param name="allocExtraBytes">Allocate this number of extra bytes after the string.</param>
		internal static byte[] LibUtf8FromString(string s, ref WeakReference<byte[]> buffer, int* utf8Length = null, int allocExtraBytes = 0)
		{
			if(utf8Length != null) *utf8Length = 0;
			if(s == null) {
				if(allocExtraBytes == 0) return null;
				s = "";
			}
			byte[] b; int len = s.Length;
			if(len == 0) {
				b = Util.Buffers.Get(allocExtraBytes, ref buffer);
				b[0] = 0;
			} else {
				int n = Utf8LengthFromString(s);
				if(utf8Length != null) *utf8Length = n;
				b = Util.Buffers.Get(n + allocExtraBytes, ref buffer);
				fixed (byte* p = b) {
					var r = Utf8FromString(s, p, n + 1);
					Debug.Assert(r == n);
				}
			}
			return b;
		}

		/// <summary>
		/// Returns the number of characters that would by produced by converting UTF8 to C# string.
		/// The terminating '\0' character is not included in the return value.
		/// </summary>
		/// <param name="utf8">UTF8 string. Can be null.</param>
		/// <param name="lengthBytes">Length of utf8 or part of it. If negative, the function finds utf8 length; then utf8 must be '\0'-terminated.</param>
		/// <remarks>
		/// Uses API <msdn>MultiByteToWideChar</msdn>.
		/// There is no overload that takes byte[], because for it can be used .NET Encoding class functions.
		/// </remarks>
		public static int Utf8ToStringLength(byte* utf8, int lengthBytes = -1)
		{
			if(utf8 == null) return 0;
			int n = lengthBytes;
			if(n < 0) n = Util.LibCharPtr.Length(utf8); else if(n > 0 && utf8[n - 1] == 0) n--;
			if(n == 0) return 0;
			return Api.MultiByteToWideChar(Api.CP_UTF8, 0, utf8, n, null, 0);
		}

		/// <summary>
		/// Converts UTF8 string to C# string (which is UTF16).
		/// The terminating '\0' character is not included in the returned string.
		/// </summary>
		/// <param name="utf8">UTF8 string. If null, returns null.</param>
		/// <param name="lengthBytes">Length of utf8 or part of it. If negative, the function finds utf8 length; then utf8 must be '\0'-terminated.</param>
		/// <remarks>
		/// Uses API <msdn>MultiByteToWideChar</msdn>.
		/// There is no overload that takes byte[], because for it can be used .NET Encoding class functions. The speed is similar; this is slower with short strings but faster with long strings.
		/// </remarks>
		public static string Utf8ToString(byte* utf8, int lengthBytes = -1)
		{
			if(utf8 == null) return null;
			int n1 = lengthBytes;
			if(n1 < 0) n1 = Util.LibCharPtr.Length(utf8); else if(n1 > 0 && utf8[n1 - 1] == 0) n1--;
			if(n1 == 0) return "";
			int n2 = Api.MultiByteToWideChar(Api.CP_UTF8, 0, utf8, n1, null, 0);
			var s = new string('\0', n2);
			fixed (char* p2 = s) {
				int r = Api.MultiByteToWideChar(Api.CP_UTF8, 0, utf8, n1, p2, n2);
				Debug.Assert(r == n2);
				return s;
			}

			//speed: with short strings slower than .NET Encoding. With long - not much faster.
		}

#if false
		/// <summary>
		/// Returns the number of characters that would by produced by converting UTF8 to C# string.
		/// </summary>
		/// <param name="utf8">UTF8 string. If ends with '\0' character, it is not included in the return value. Can be null.</param>
		public static int Utf8ToStringLength(byte[] utf8)
		{
			if(utf8 == null) return 0;
			int n = utf8.Length; if(n == 0) return 0;
			if(utf8[n - 1] == 0) { n--; if(n == 0) return 0; }
			fixed (byte* p = utf8) {
				return Api.MultiByteToWideChar(Api.CP_UTF8, 0, p, n, null, 0);
			}
		}

		/// <summary>
		/// Converts UTF8 string to C# string (which is UTF16).
		/// </summary>
		/// <param name="utf8">UTF8 string. If null, returns null.</param>
		public static string Utf8ToString(byte[] utf8)
		{
			if(utf8 == null) return null;
			int n1 = utf8.Length; if(n1 == 0) return "";
			if(utf8[n1 - 1] == 0) { n1--; if(n1 == 0) return ""; }
			fixed (byte* p1 = utf8) {
				int n2 = Api.MultiByteToWideChar(Api.CP_UTF8, 0, p1, n1, null, 0);
				var s = new string('\0', n2);
				fixed (char* p2 = s) {
					int r = Api.MultiByteToWideChar(Api.CP_UTF8, 0, p1, n1, p2, n2);
					Debug.Assert(r == n2);
					return s;
				}
			}
		}
#endif

		#endregion
	}
}
