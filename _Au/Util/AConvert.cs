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
using System.IO.Compression;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	/// <summary>
	/// Data conversion functions - compress, hex-encode, Base64, UTF8.
	/// </summary>
	/// <seealso cref="Util.AHash"/>
	/// <seealso cref="Util.AHash.MD5"/>
	public unsafe class AConvert
	{
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
			var b = Util.AMemoryArray.LibChar(n);
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
		public static unsafe int Base64Decode(char* s, int len, void* resultBuffer, int bufferSize)
		{
			if(bufferSize < len * 3L / 4) throw new ArgumentException("bufferSize too small");
			byte* r = (byte*)resultBuffer;
			var t = Util.LibTables.Base64;

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
		/// How this function differs from <see cref="Convert.FromBase64String"/>:
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
				fixed (byte* b = Util.AMemoryArray.LibByte(n)) {
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
		///// Such string can be parsed with <b>AConvert</b> class functions, not with <b>Convert</b> class functions.
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
			using var memoryStream = new MemoryStream();
			using var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress);
			deflateStream.Write(data, 0, data.Length);
			return memoryStream.ToArray();
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
			using var stream = new MemoryStream();
			Decompress(stream, compressedData, index, count);
			return stream.ToArray();
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
		/// using var stream = new MemoryStream();
		/// Decompress(stream, compressedData, index, count);
		/// return stream.ToArray();
		/// ]]></code>
		/// </example>
		public static void Decompress(Stream streamForDecompressedData, byte[] compressedData, int index = 0, int count = -1)
		{
			if(count < 0) count = compressedData.Length - index;
			using var compressStream = new MemoryStream(compressedData, index, count, false);
			using var deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress);
			deflateStream.CopyTo(streamForDecompressedData);

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
		/// Converts C# string to '\0'-terminated UTF8 string.
		/// </summary>
		/// <param name="s">C# string. If null, returns null.</param>
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
		/// Converts C# string to '\0'-terminated UTF8 string. Stores the UTF8 string in caller's buffer.
		/// Returns UTF8 string length in bytes, not including the terminating '\0' character.
		/// If fails (unlikely if passed correct arguments), returns 0 and sets buffer="".
		/// </summary>
		/// <param name="s">C# string. Can be null.</param>
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
			if(n < 0) n = LibCharPtr.Length(utf8); else if(n > 0 && utf8[n - 1] == 0) n--;
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
		/// Can be used instead of the .NET <b>Encoding</b> class. The speed is similar; this is slower with short strings but faster with long strings.
		/// </remarks>
		public static string Utf8ToString(byte* utf8, int lengthBytes = -1)
		{
			if(utf8 == null) return null;
			int n1 = lengthBytes;
			if(n1 < 0) n1 = LibCharPtr.Length(utf8); else if(n1 > 0 && utf8[n1 - 1] == 0) n1--;
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

		/// <summary>
		/// Converts UTF8 string to C# string (which is UTF16).
		/// The terminating '\0' character is not included in the returned string.
		/// </summary>
		/// <param name="utf8">UTF8 string. If null, returns null.</param>
		/// <remarks>
		/// Uses API <msdn>MultiByteToWideChar</msdn>.
		/// Can be used instead of the .NET <b>Encoding</b> class. The speed is similar; this is slower with short strings but faster with long strings.
		/// </remarks>
		public static string Utf8ToString(byte[] utf8)
		{
			fixed (byte* p = utf8) return Utf8ToString(p);
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
