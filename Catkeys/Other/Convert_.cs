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
using System.IO.Compression;

using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Data conversion functions - hash, encode, compress.
	/// </summary>
	public class Convert_
	{
		/// <summary>
		/// FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int HashFnv1(string data)
		{
			uint hash = 2166136261;

			for(int i = 0; i < data.Length; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;
		}

		/// <summary>
		/// FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int HashFnv1(string data, int iStart, int count)
		{
			uint hash = 2166136261;

			for(int i = iStart, n = iStart + count; i < n; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;
		}

		/// <summary>
		/// FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static unsafe int HashFnv1(char* data, int lengthChars)
		{
			uint hash = 2166136261;

			for(int i = 0; i < lengthChars; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;
		}

		/// <summary>
		/// FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static int HashFnv1(byte[] data)
		{
			uint hash = 2166136261;

			for(int i = 0; i < data.Length; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;
		}

		/// <summary>
		/// FNV-1 hash.
		/// Useful for fast hash table and checksum use, not cryptography. Similar to CRC32; faster but creates more collisions.
		/// </summary>
		public static unsafe int HashFnv1(byte* data, int lengthBytes)
		{
			uint hash = 2166136261;

			for(int i = 0; i < lengthBytes; i++)
				hash = (hash * 16777619) ^ data[i];

			return (int)hash;

			//note: could be void* data, then callers don't have to cast other types to byte*, but then can accidentally pick wrong overload when char*. Also now it's completely clear that it hashes bytes, not the passed type directly (like the char* overload does).
		}

		unsafe struct MD5_CTX
		{
			long _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;
			public long r1, r2;
			//public fixed byte r[16]; //same speed, maybe slightly slower
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			//public byte[] r; //slow like .NET API
		}
		[DllImport("ntdll.dll")]
		static extern void MD5Init(out MD5_CTX context);
		[DllImport("ntdll.dll")]
		static extern void MD5Update(ref MD5_CTX context, byte[] data, int dataLen);
		[DllImport("ntdll.dll")]
		static extern void MD5Final(ref MD5_CTX context);

		/// <summary>
		/// Computes MD5 hash of binary data.
		/// The same as Hash(..., "MD5") but much faster.
		/// </summary>
		public static unsafe byte[] HashMD5(byte[] a)
		{
			try {
				MD5Init(out MD5_CTX x);
				MD5Update(ref x, a, a.Length);
				MD5Final(ref x);

				var r = new byte[16];
				Marshal.Copy((IntPtr)(&x.r1), r, 0, 16);
				return r;

				//speed: slightly slower than QM2 (it uses MD5x too).
			}
			catch { //the API are undocumented and may be removed in the future
				Debug.Assert(false);
				return Hash(a, "MD5");

				//speed: 9 times slower than the above code. Could use a static variable to make 2 times faster.
				//First time 3700, the above code 1700.
			}
		}

		/// <summary>
		/// Computes MD5 hash of string.
		/// The same as Hash(..., "MD5") but much faster.
		/// </summary>
		/// <remarks>The hash is of UTF8 bytes, not of UTF16 bytes.</remarks>
		public static byte[] HashMD5(string s)
		{
			return HashMD5(Encoding.UTF8.GetBytes(s));
		}

		/// <summary>
		/// Computes MD5 hash of binary data and converts to hex string.
		/// The same as Hash(..., "MD5") but much faster.
		/// </summary>
		public static string HashMD5Hex(byte[] a, bool upperCase = false)
		{
			return BytesToHexString(HashMD5(a), upperCase);
		}

		/// <summary>
		/// Computes MD5 hash of string and converts to hex string.
		/// The same as Hash(..., "MD5") but much faster.
		/// </summary>
		/// <remarks>The hash is of UTF8 bytes, not of UTF16 bytes.</remarks>
		public static string HashMD5Hex(string s, bool upperCase = false)
		{
			return BytesToHexString(HashMD5(s), upperCase);
		}

		/// <summary>
		/// Computes binary data hash using a specified cryptographic algorithm.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		public static byte[] Hash(byte[] a, string algorithm)
		{
			using(var x = (System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName(algorithm)) {
				return x.ComputeHash(a);
			}
		}

		/// <summary>
		/// Computes string hash using a specified cryptographic algorithm.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		/// <remarks>The hash is of UTF8 bytes, not of UTF16 bytes.</remarks>
		public static byte[] Hash(string s, string algorithm)
		{
			return Hash(Encoding.UTF8.GetBytes(s), algorithm);
		}

		/// <summary>
		/// Computes binary data hash using a specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		public static string HashHex(byte[] a, string algorithm, bool upperCase = false)
		{
			return BytesToHexString(Hash(a, algorithm), upperCase);
		}

		/// <summary>
		/// Computes string hash using a specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		/// <remarks>The hash is of UTF8 bytes, not of UTF16 bytes.</remarks>
		public static string HashHex(string s, string algorithm, bool upperCase = false)
		{
			return BytesToHexString(Hash(s, algorithm), upperCase);
		}

		/// <summary>
		/// Converts byte array (binary data) to hex-encoded string.
		/// </summary>
		/// <param name="a">The data. Can be null.</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		/// <seealso cref="Convert.ToBase64String(byte[])"/>
		public static unsafe string BytesToHexString(byte[] a, bool upperCase = false)
		{
			if(a == null) return null;
			fixed (byte* p = a) {
				return BytesToHexString(p, a.Length, upperCase);
			}
		}

		/// <summary>
		/// Converts binary data to hex-encoded string.
		/// </summary>
		/// <param name="data">The data. Can be any valid memory of specified size, for example a struct address. Can be null.</param>
		/// <param name="size">data memory size.</param>
		/// <param name="upperCase">Let the hex string contain A-F, not a-f.</param>
		public static unsafe string BytesToHexString(void* data, int size, bool upperCase = false)
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char _HalfByteToHexCharU(int b)
		{
			return (char)(b < 10 ? '0' + b : 'A' - 10 + b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char _HalfByteToHexCharL(int b)
		{
			return (char)(b < 10 ? '0' + b : 'a' - 10 + b);
		}

		/// <summary>
		/// Converts hex-encoded string to byte array (binary data).
		/// </summary>
		/// <param name="s">Hex-encoded string. Can be null.</param>
		/// <remarks>
		/// Skips spaces and other non-hex-digit characters. Example: "01 23 46 67" is the same as "01234667". Skips hex-digits that are not in pair, for example 2 in "01 2 34".
		/// </remarks>
		/// <seealso cref="Convert.FromBase64String(string)"/>
		public static unsafe byte[] BytesFromHexString(string s)
		{
			if(s == null) return null;
			int n = s.Length / 2;
			var p = Util.LibCharBuffer.Common.Alloc(n);
			n = BytesFromHexString(s, p, n, 0);
			var r = new byte[n];
			Marshal.Copy((IntPtr)p, r, 0, n);
			//var b1 = (byte*)p; for(int i = 0; i < n; i++) r[i] = b1[i]; //faster when array small, slower when big
			return r;

			//speed: fast enough without a lookup table. Faster than BytesToHexString.
			//These functions have similar speed as Convert.ToBase64String and Convert.FromBase64String.
		}

		/// <summary>
		/// Converts hex-encoded string to binary data.
		/// Returns the number of bytes stored in data memory. It is equal or less than Math.Min(size, (s.Length - startFrom) / 2).
		/// </summary>
		/// <param name="s">Hex-encoded string. Can be null.</param>
		/// <param name="data">Where to write the data. Can be any valid memory of specified size, for example a struct address.</param>
		/// <param name="size">data memory size.</param>
		/// <param name="startIndex">0 or index of first character of hex-encoded substring in s.</param>
		/// <remarks>
		/// Skips spaces and other non-hex-digit characters. Example: "01 23 46 67" is the same as "01234667". Skips hex-digits that are not in pair, for example 2 in "01 2 34".
		/// <note>This function is unsafe, it will damage process memory if using bad data or size.</note>
		/// </remarks>
		public static unsafe int BytesFromHexString(string s, void* data, int size, int startIndex = 0)
		{
			var t = _aHex;
			if(t == null) {
				t = new byte[55];
				for(int u = 0; u < 55; u++) {
					char c = (char)(u + '0');
					if(c >= '0' && c <= '9') t[u] = (byte)u;
					else if(c >= 'A' && c <= 'F') t[u] = (byte)(c - ('A' - 10));
					else if(c >= 'a' && c <= 'f') t[u] = (byte)(c - ('a' - 10));
					else t[u] = 0xFF;
				}
				_aHex = t;
			}

			if(s == null) return 0;
			var r = (byte*)data;
			int i = 0, j = startIndex, n = s.Length - 1;
			while(j < n && i < size) {
				int k1 = _HexCharToHalfByte(s[j++]); if(k1 == 0xFF) continue;
				int k2 = _HexCharToHalfByte(s[j++]); if(k2 == 0xFF) continue;
				r[i++] = (byte)((k1 << 4) | k2);
			}
			return i;

			//tested: inlined. Slightly faster than using this code not in func.
			int _HexCharToHalfByte(char c)
			{
				uint k = (uint)(c - '0');
				return (k < 55) ? t[k] : 0xFF;
			}

			//info: this is very unsafe.
			//	Cannot use generic because C# does not allow to get generic parameter address.
			//	Cannot use GCHandle.AddrOfPinnedObject because it gets address of own copy of the variable.
		}
		static byte[] _aHex;

		//Old version. Same speed. Does not support non-hex-digits.
		//public static unsafe int BytesFromHexString_old(string s, void* data, int size, int startIndex = 0)
		//{
		//	if(s == null) return 0;
		//	int i, j, n = (s.Length - startIndex) / 2;
		//	if(n > size) n = size;
		//	var r = (byte*)data;
		//	for(i = 0, j = startIndex; i < n; i++, j += 2) {
		//		r[i] = (byte)((_HexCharToHalfByte(s[j]) << 4) | _HexCharToHalfByte(s[j + 1]));
		//	}
		//	return n;

		//	//info: this is very unsafe.
		//	//	Cannot use generic because C# does not allow to get generic parameter address.
		//	//	Cannot use GCHandle.AddrOfPinnedObject because it gets address of own copy of the variable.
		//}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//static int _HexCharToHalfByte(char c)
		//{
		//	if(c >= '0' && c <= '9') return c - '0';
		//	if(c >= 'A' && c <= 'F') return c - ('A' - 10);
		//	if(c >= 'a' && c <= 'f') return c - ('a' - 10);
		//	//throw new ArgumentException(); //makes slower
		//	return int.MinValue;
		//}

		/// <summary>
		/// Compresses data using <see cref="DeflateStream"/>.
		/// </summary>
		/// <param name="data"></param>
		/// <exception cref="Exception">Exceptions thrown by DeflateStream.</exception>
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
		/// </summary>
		/// <param name="data"></param>
		/// <exception cref="Exception">Exceptions thrown by DeflateStream.</exception>
		public static byte[] Decompress(byte[] data)
		{
			using(MemoryStream decompressedStream = new MemoryStream()) {
				using(MemoryStream compressStream = new MemoryStream(data)) {
					using(DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress)) {
						deflateStream.CopyTo(decompressedStream);
					}
				}
				return decompressedStream.ToArray();
			}
		}
	}
}
