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
//using System.Linq;
using System.Buffers;
using System.IO.Compression;

using Au.Types;

//TODO: maybe make internal. Test System.Text.Unicode.Utf8, System.Buffers.Text classes.

namespace Au.Util
{
	/// <summary>
	/// Data conversion functions - compress, hex-encode, Base64, UTF8.
	/// </summary>
	/// <seealso cref="AHash"/>
	/// <seealso cref="AHash.MD5"/>
	public unsafe class AConvert
	{
		#region hex encode

		/// <summary>
		/// Converts binary data stored in any memory to hex-encoded string.
		/// </summary>
		/// <param name="data">The data. Can be any valid memory of specified size, for example a struct address.</param>
		/// <param name="size">data memory size (bytes).</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		public static string HexEncode(void* data, int size, bool upperCase = false) {
			int u = (upperCase ? 'A' : 'a') - 10;
			var bytes = (byte*)data;
			string R = new string('\0', size * 2);
			fixed (char* cp = R) {
				int* p = (int*)cp;
				for (int i = 0; i < size; i++) {
					int b = bytes[i];
					int t = b & 0xf; t += t < 10 ? '0' : u;
					b >>= 4; b += b < 10 ? '0' : u;
					p[i] = t << 16 | b;
				}
			}
			return R;

			//tested: ~50% of time takes string allocation.
			//tested: with string.Create same speed.
		}

		/// <summary>
		/// Converts byte[] to hex-encoded string.
		/// </summary>
		/// <param name="a">The data.</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		/// <remarks>
		/// The result string length is 2 * array length.
		/// In most cases it's better to use <see cref="Convert.ToBase64String(byte[])"/>, then result is 4/3 of array length. Both functions are fast.
		/// </remarks>
		public static string HexEncode(byte[] a, bool upperCase = false) {
			fixed (byte* p = a) {
				return HexEncode(p, a.Length, upperCase);
			}
		}

		/// <summary>
		/// Converts a struct variable to hex-encoded string.
		/// </summary>
		/// <param name="x">Variable.</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		public static string HexEncode<T>(T x, bool upperCase = false) where T : unmanaged
			=> HexEncode(&x, sizeof(T), upperCase);

		/// <summary>
		/// Converts hex-encoded string to binary data as byte[].
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Hex-encoded data.</param>
		/// <remarks>
		/// Skips spaces and other non-hex-digit characters. Example: "01 23 46 67" is the same as "01234667".
		/// The number of hex-digit characters should be divisible by 2, else the last character is ignored.
		/// </remarks>
		[SkipLocalsInit]
		public static byte[] HexDecode(ReadOnlySpan<char> encoded) {
			using ABuffer<byte> b = new(encoded.Length / 2);
			int n = HexDecode(encoded, b.p, b.n);
			var r = new byte[n];
			Marshal.Copy((IntPtr)b.p, r, 0, n);
			return r;
		}

		/// <summary>
		/// Converts hex-encoded string to binary data. Stores it in caller's memory buffer.
		/// Returns the number of bytes stored in <i>decoded</i> memory. It is equal or less than <c>Math.Min(bufferSize, encoded.Length/2)</c>.
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Hex-encoded data.</param>
		/// <param name="decoded">Memory buffer for the result.</param>
		/// <param name="bufferSize">The max number of bytes that can be written to the <i>decoded</i> memory buffer.</param>
		/// <remarks>
		/// Skips spaces and other non-hex-digit characters. Example: "01 23 46 67" is the same as "01234667".
		/// The number of hex-digit characters should be divisible by 2, else the last character is ignored.
		/// </remarks>
		public static int HexDecode(ReadOnlySpan<char> encoded, void* decoded, int bufferSize) {
			if (encoded.Length == 0) return 0;
			var t = Tables_.Hex;
			byte* r = (byte*)decoded, rTo = r + bufferSize;
			uint k = 1;
			for (int i = 0; i < encoded.Length; i++) {
				uint c = (uint)(encoded[i] - '0'); if (c >= 55) continue;
				c = t[c]; if (c == 0xFF) continue;
				k <<= 4;
				k |= c;
				if (0 != (k & 0x100)) {
					if (r >= rTo) break;
					*r++ = (byte)k;
					k = 1;
				}
			}

			return (int)(r - (byte*)decoded);

			//speed: slightly slower than Base64Decode for the same binary data size.
		}

		/// <summary>
		/// Converts hex-encoded string to a struct variable.
		/// Returns false if decoded size != <c>sizeof(T)</c>.
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Base64-encoded data.</param>
		/// <param name="decoded">The result variable.</param>
		public static bool HexDecode<T>(ReadOnlySpan<char> encoded, out T decoded) where T : unmanaged {
			T t;
			if (HexDecode(encoded, &t, sizeof(T)) != sizeof(T)) { decoded = default; return false; }
			decoded = t;
			return true;
		}

		#endregion

		#region base64

		/// <summary>
		/// Converts byte[] to Base64-encoded string that can be used in URLs and file names.
		/// </summary>
		/// <param name="bytes">Data to encode.</param>
		/// <remarks>Like <see cref="Convert.ToBase64String(byte[])"/>, but instead of '/' and '+' uses '_' and '-'.</remarks>
		public static string Base64UrlEncode(byte[] bytes) {
			fixed (byte* p = bytes) return Base64UrlEncode(p, bytes.Length);

			//speed: same as Convert.ToBase64String
		}

		/// <summary>
		/// Converts binary data stored in any memory to Base64-encoded string that can be used in URLs and file names.
		/// </summary>
		/// <param name="bytes">Data to encode.</param>
		/// <param name="length">Number of bytes to encode.</param>
		/// <remarks>Instead of '/' and '+' uses '_' and '-'.</remarks>
		public static string Base64UrlEncode(void* bytes, int length) {
			var ip = (IntPtr)bytes;
			return string.Create(Base64UrlEncodeLength(length), (ip, length), (span, tu) => {
				bool ok = Convert.TryToBase64Chars(new ReadOnlySpan<byte>((byte*)tu.ip, tu.length), span, out _);
				for (int i = 0; i < span.Length; i++) {
					switch (span[i]) { case '/': span[i] = '_'; break; case '+': span[i] = '-'; break; }
				}
			});
		}

		/// <summary>
		/// Converts a struct variable to Base64-encoded string that can be used in URLs and file names.
		/// </summary>
		/// <param name="x">Variable.</param>
		/// <remarks>Instead of '/' and '+' uses '_' and '-'.</remarks>
		public static string Base64UrlEncode<T>(T x) where T : unmanaged {
			return Base64UrlEncode(&x, sizeof(T));
		}

		/// <summary>
		/// Gets Base64-encoded string length for non-encoded length.
		/// It is <c>(length + 2) / 3 * 4</c>.
		/// </summary>
		public static int Base64UrlEncodeLength(int length) => checked((length + 2) / 3 * 4);

		/// <summary>
		/// Gets decoded data length from Base64-encoded string length.
		/// It is <c>(int)(len * 3L / 4)</c> minus the number of padding '=' characters (max 2).
		/// </summary>
		public static int Base64UrlDecodeLength(ReadOnlySpan<char> encoded) {
			int len = encoded.Length;
			if (len == 0) return 0;
			int r = (int)(len * 3L / 4);
			if (0 == (len & 3)) {
				if (encoded[len - 1] == '=') {
					r--;
					if (encoded[len - 2] == '=') r--;
				}
			}
			return r;
		}

		static void _Base64_Replace(ReadOnlySpan<char> encoded, char[] a) {
			for (int i = 0; i < encoded.Length; i++) {
				char c = encoded[i];
				a[i] = c switch { '_' => '/', '-' => '+', _ => c, };
			}
		}

		/// <summary>
		/// Converts string containing Base64-encoded data to byte[]. Supports standard encoding and URL-safe encoding.
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Base64-encoded data.</param>
		/// <remarks>Like <see cref="Convert.FromBase64String(string)"/>, but the string can contain '_' and '-' instead of '/' and '+'.</remarks>
		/// <exception cref="Exception">Exceptions of <see cref="Convert.FromBase64CharArray"/>.</exception>
		public static byte[] Base64UrlDecode(ReadOnlySpan<char> encoded) {
			char[] a = ArrayPool<char>.Shared.Rent(encoded.Length);
			try {
				_Base64_Replace(encoded, a);
				return Convert.FromBase64CharArray(a, 0, encoded.Length);
			}
			finally { ArrayPool<char>.Shared.Return(a); }
			//never mind: almost 2 times slower than Convert.FromBase64CharArray.
			//	Normally this func is used with short strings and not with many strings in loop.
			//	ArrayPool isn't as fast as should be. And copying to new array takes time.
		}

		/// <summary>
		/// Converts string containing Base64-encoded data to bytes and stores in memory of a Span variable. Supports standard encoding and URL-safe encoding.
		/// Returns false if the encoded string is invalid or the buffer is too small.
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Base64-encoded data.</param>
		/// <param name="decoded">Memory buffer for the result.</param>
		/// <param name="decodedLength"></param>
		/// <remarks>The string can contain '_' and '-' instead of '/' and '+'.</remarks>
		public static bool Base64UrlDecode(ReadOnlySpan<char> encoded, Span<byte> decoded, out int decodedLength) {
			char[] a = ArrayPool<char>.Shared.Rent(encoded.Length);
			try {
				_Base64_Replace(encoded, a);
				return Convert.TryFromBase64Chars(new ReadOnlySpan<char>(a, 0, encoded.Length), decoded, out decodedLength);
			}
			finally { ArrayPool<char>.Shared.Return(a); }
		}

		/// <summary>
		/// Converts string containing Base64-encoded data to bytes and stores in any memory. Supports standard encoding and URL-safe encoding.
		/// Returns false if the encoded string is invalid or the buffer is too small.
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Base64-encoded data.</param>
		/// <param name="decoded">Memory buffer for the result.</param>
		/// <param name="bufferSize">The max number of bytes that can be written to the <i>decoded</i> memory buffer.</param>
		/// <param name="decodedLength">Receives the number of bytes written to the <i>decoded</i> memory buffer.</param>
		/// <remarks>The string can contain '_' and '-' instead of '/' and '+'.</remarks>
		public static bool Base64UrlDecode(ReadOnlySpan<char> encoded, void* decoded, int bufferSize, out int decodedLength) {
			return Base64UrlDecode(encoded, new Span<byte>(decoded, bufferSize), out decodedLength);
		}

		/// <summary>
		/// Converts string containing Base64-encoded data to a struct variable. Supports standard encoding and URL-safe encoding.
		/// Returns false if the encoded string is invalid or decoded size != <c>sizeof(T)</c>.
		/// </summary>
		/// <param name="encoded">String or char[] or span of string/array/memory containing Base64-encoded data.</param>
		/// <param name="decoded">The result variable.</param>
		/// <remarks>The string can contain '_' and '-' instead of '/' and '+'.</remarks>
		public static bool Base64UrlDecode<T>(ReadOnlySpan<char> encoded, out T decoded) where T : unmanaged {
			T t;
			if (!Base64UrlDecode(encoded, &t, sizeof(T), out int n) || n != sizeof(T)) { decoded = default; return false; }
			decoded = t;
			return true;
		}

		#endregion

		#region compress

		/// <summary>
		/// Compresses data using <see cref="DeflateStream"/>.
		/// </summary>
		/// <param name="data"></param>
		/// <exception cref="Exception">Exceptions of DeflateStream.</exception>
		public static byte[] Compress(byte[] data) {
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
		public static byte[] Decompress(byte[] compressedData, int index = 0, int count = -1) {
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
		public static void Decompress(Stream streamForDecompressedData, byte[] compressedData, int index = 0, int count = -1) {
			if (count < 0) count = compressedData.Length - index;
			using var compressStream = new MemoryStream(compressedData, index, count, false);
			using var deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress);
			deflateStream.CopyTo(streamForDecompressedData);

			//note: cannot deflateStream.Read directly to array because its Length etc are not supported.
			//note: also cannot use decompressedStream.GetBuffer because it can be bigger.
		}

		#endregion

		#region utf8

		/// <summary>
		/// Converts string to UTF-8 byte[]. Appends "\0" or some other string.
		/// </summary>
		/// <param name="chars">String or char[] or span of string/array/memory.</param>
		/// <param name="append">An optional ASCII string to append. For example "\0" (default) or "\r\n" or null.</param>
		/// <exception cref="ArgumentException"><i>append</i> contains non-ASCII characters.</exception>
		public static byte[] ToUtf8(ReadOnlySpan<char> chars, string append = "\0") {
			int n = Encoding.UTF8.GetByteCount(chars);
			int nAppend = append.Lenn();
			var r = new byte[n + nAppend];
			int nn = Encoding.UTF8.GetBytes(chars, r);
			Debug.Assert(nn == n);
			if (nAppend > 0 && !(nAppend == 1 && append[0] == '\0')) {
				foreach (char c in append) {
					if (c > 127) throw new ArgumentException("append must be ASCII");
					r[nn++] = (byte)c;
				}
			}
			return r;

			//speed: faster than WideCharToMultiByte. Faster than with GetMaxByteCount + ApiBuffer_.Byte_.
			//About tiered JIT:
			//	Until fully optimized, many times slower with short strings, but not much slower with big strings.
			//	When fully optimized, faster than when tiered JIT turned off.
		}

		/// <summary>
		/// Converts UTF8 string to C# string (which is UTF16).
		/// The terminating '\0' character is not included in the returned string.
		/// </summary>
		/// <param name="utf8">UTF8 string. If null, returns null.</param>
		/// <param name="length">Length of <i>utf8</i>. If negative, the function finds length; then <i>utf8</i> must be '\0'-terminated.</param>
		public static string FromUtf8(byte* utf8, int length = -1) {
			if (utf8 == null) return null;
			int n = length;
			if (n < 0) n = BytePtr_.Length(utf8); else if (n > 0 && utf8[n - 1] == 0) n--;
			return Encoding.UTF8.GetString(utf8, n);
		}

		#endregion
	}
}
