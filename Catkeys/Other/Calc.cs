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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Simple calculation functions, similar to Math.
	/// </summary>
	//[DebuggerStepThrough]
	public static class Calc
	{
		/// <summary>
		/// Creates uint by placing (ushort)loWord in bits 1-16 and (ushort)hiWord in bits 17-32.
		/// Like C macro MAKELONG, MAKEWPARAM, MAKELPARAM, MAKELRESULT.
		/// </summary>
		public static uint MakeUint(uint loWord, uint hiWord)
		{
			return ((hiWord & 0xffff) << 16) | (loWord & 0xffff);
		}
		/// <summary>
		/// Creates uint by placing (ushort)loWord in bits 1-16 and (ushort)hiWord in bits 17-32.
		/// Like C macro MAKELONG, MAKEWPARAM, MAKELPARAM, MAKELRESULT.
		/// </summary>
		public static uint MakeUint(int loWord, int hiWord) { return MakeUint((uint)loWord, (uint)hiWord); }

		/// <summary>
		/// Creates ushort by placing (byte)loByte in bits 1-8 and (byte)hiByte in bits 9-16.
		/// Like C macro MAKEWORD.
		/// </summary>
		public static ushort MakeUshort(uint loByte, uint hiByte)
		{
			return (ushort)(((hiByte & 0xff) << 8) | (loByte & 0xff));
		}
		/// <summary>
		/// Creates ushort by placing (byte)loByte in bits 1-8 and (byte)hiByte in bits 9-16.
		/// Like C macro MAKEWORD.
		/// </summary>
		public static ushort MakeUshort(int loByte, int hiByte) { return MakeUshort((uint)loByte, (uint)hiByte); }

		/// <summary>
		/// Gets bits 1-16 as ushort.
		/// Like C macro LOWORD.
		/// </summary>
		public static ushort LoUshort(LPARAM x) { return (ushort)((uint)x & 0xFFFF); }

		/// <summary>
		/// Gets bits 17-32 as ushort.
		/// Like C macro HIWORD.
		/// </summary>
		public static ushort HiUshort(LPARAM x) { return (ushort)((uint)x >> 16); }

		/// <summary>
		/// Gets bits 1-16 as short.
		/// Like C macro GET_X_LPARAM.
		/// </summary>
		public static short LoShort(LPARAM x) { return (short)((uint)x & 0xFFFF); }

		/// <summary>
		/// Gets bits 17-32 as short.
		/// Like C macro GET_Y_LPARAM.
		/// </summary>
		public static short HiShort(LPARAM x) { return (short)((uint)x >> 16); }

		/// <summary>
		/// Gets bits 1-8 as byte.
		/// Like C macro LOBYTE.
		/// </summary>
		public static byte LoByte(LPARAM x) { return (byte)((uint)x & 0xFF); }

		/// <summary>
		/// Gets bits 9-16 as byte.
		/// Like C macro HIBYTE.
		/// </summary>
		public static byte HiByte(LPARAM x) { return (byte)((uint)x >> 8); }

		/// <summary>
		/// Multiplies number and numerator without overflow, and divides by denominator.
		/// The return value is rounded up or down to the nearest integer.
		/// If either an overflow occurred or denominator was 0, the return value is –1.
		/// </summary>
		public static int MulDiv(int number, int numerator, int denominator)
		{
			return _MulDiv(number, numerator, denominator);

			//could use this, but the API rounds down or up to the nearest integer, but this always rounds down
			//return (int)(((long)number * numerator) / denominator);
		}

		[DllImport("kernel32.dll", EntryPoint = "MulDiv")]
		static extern int _MulDiv(int nNumber, int nNumerator, int nDenominator);

		/// <summary>
		/// Returns percent of part in whole.
		/// </summary>
		public static int Percent(int whole, int part)
		{
			return (int)(100L * part / whole);
		}

		/// <summary>
		/// Returns percent of part in whole.
		/// </summary>
		public static double Percent(double whole, double part)
		{
			return 100.0 * part / whole;
		}

		/// <summary>
		/// If value is divisible by alignment, returns value. Else returns nearest bigger number that is divisible by alignment.
		/// </summary>
		/// <param name="value">An integer value.</param>
		/// <param name="alignment">Alignment. Must be a power of two (2, 4, 8, 16...).</param>
		/// <remarks>For example if alignment is 4, returns 4 if value is 1-4, returns 8 if value is 5-8, returns 12 if value is 9-10, and so on.</remarks>
		public static uint AlignUp(uint value, uint alignment)
		{
			return (value + (alignment - 1)) & ~(alignment - 1);
		}

		/// <summary>
		/// If value is divisible by alignment, returns value. Else returns nearest bigger number that is divisible by alignment.
		/// </summary>
		/// <param name="value">An integer value.</param>
		/// <param name="alignment">Alignment. Must be a power of two (2, 4, 8, 16...).</param>
		/// <remarks>For example if alignment is 4, returns 4 if value is 1-4, returns 8 if value is 5-8, returns 12 if value is 9-10, and so on.</remarks>
		public static int AlignUp(int value, uint alignment) { return (int)AlignUp((uint)value, alignment); }

		/// <summary>
		/// Returns value but not less than min and not greater than max.
		/// If value is less than min, returns min.
		/// If value is greater than max, returns max.
		/// </summary>
		public static int MinMax(int value, int min, int max)
		{
			Debug.Assert(max >= min);
			if(value < min) return min;
			if(value > max) return max;
			return value;

			//why this is not an extension method:
			//1. Less chances to not use the return value (expecting that the function modifies variable's value, which it can't do). There is no attribute to warn about it, unless you have ReSharper (then [Pure] etc).
			//2. All similar functions are in Calc class. Like most similar .NET functions are in .NET class, and not in Int32 etc.
		}

		/// <summary>
		/// Returns true if character is '0' to '9'.
		/// </summary>
		public static bool IsDigit(char c) { return c <= '9' && c >= '0'; }

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z'.
		/// </summary>
		public static bool IsAlpha(char c) { return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'); }

		/// <summary>
		/// Calculates angle degrees from coordinates x and y.
		/// </summary>
		public static double AngleFromXY(int x, int y)
		{
			return Math.Atan2(y, x) * (180 / Math.PI);
		}

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
	}
}
