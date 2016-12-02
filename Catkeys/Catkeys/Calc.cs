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
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Catkeys
{
	[DebuggerStepThrough]
	public static class Calc
	{
		/// <summary>
		/// Creates uint by placing (ushort)loWord in low-order 16-bits and (ushort)hiWord in high-order 16 bits.
		/// </summary>
		public static uint MakeUint(uint loWord, uint hiWord)
		{
			return ((hiWord & 0xffff) << 16) | (loWord & 0xffff);
		}
		/// <summary>
		/// Creates uint by placing (ushort)loWord in low-order 16-bits and (ushort)hiWord in high-order 16 bits.
		/// </summary>
		public static uint MakeUint(int loWord, int hiWord) { return MakeUint((uint)loWord, (uint)hiWord); }

		/// <summary>
		/// Creates ushort by placing (byte)loByte in low-order 8-bits and (byte)hiByte in high-order 8 bits.
		/// </summary>
		public static ushort MakeUshort(uint loByte, uint hiByte)
		{
			return (ushort)(((hiByte & 0xff) << 8) | (loByte & 0xff));
		}
		/// <summary>
		/// Creates ushort by placing (byte)loByte in low-order 8-bits and (byte)hiByte in high-order 8 bits.
		/// </summary>
		public static ushort MakeUshort(int loByte, int hiByte) { return MakeUshort((uint)loByte, (uint)hiByte); }

		/// <summary>
		/// Multiplies number and numerator without overflow, and divides by denominator.
		/// The return value is rounded up or down to the nearest integer.
		/// If either an overflow occurred or denominator was 0, the return value is –1.
		/// See also: Percent().
		/// </summary>
		public static int MulDiv(int number, int numerator, int denominator)
		{
			return _MulDiv(number, numerator, denominator);

			//could use this, but the API rounds down or up to the nearst integer, but this always rounds down
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

		public static int AlignUp(int value, uint alignment) { return (int)AlignUp((uint)value, alignment); }

		/// <summary>
		/// Converts from System.Drawing.Color (ARGB) to native Windows COLORREF (ABGR).
		/// </summary>
		public static uint ColorToNative(System.Drawing.Color color)
		{
			uint t = (uint)color.ToArgb();
			return (t & 0xff00ff00) | ((t << 16) & 0xff0000) | ((t >> 16) & 0xff);
		}

		/// <summary>
		/// Converts from native Windows COLORREF (ABGR) to System.Drawing.Color (ARGB).
		/// </summary>
		public static System.Drawing.Color ColorFromNative(uint color)
		{
			return System.Drawing.Color.FromArgb((int)((color & 0xff00ff00) | ((color << 16) & 0xff0000) | ((color >> 16) & 0xff)));
		}

		/// <summary>
		/// Returns true if character is '0' to '9'.
		/// </summary>
		public static bool IsDigit(char c) { return c <= '9' && c >= '0'; }

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
				MD5_CTX x;
				MD5Init(out x);
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
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		/// <remarks>The hash is of UTF8 bytes, not of UTF16 bytes.</remarks>
		public static byte[] Hash(string s, string algorithm)
		{
			return Hash(Encoding.UTF8.GetBytes(s), algorithm);
		}

		/// <summary>
		/// Computes binary data hash using a specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		public static string HashHex(byte[] a, string algorithm, bool upperCase = false)
		{
			return BytesToHexString(Hash(a, algorithm), upperCase);
		}

		/// <summary>
		/// Computes string hash using a specified cryptographic algorithm and converts to hex string.
		/// </summary>
		/// <param name="algorithm">Algorithm name, eg "SHA256". <see cref="System.Security.Cryptography.CryptoConfig"/></param>
		/// <remarks>The hash is of UTF8 bytes, not of UTF16 bytes.</remarks>
		public static string HashHex(string s, string algorithm, bool upperCase = false)
		{
			return BytesToHexString(Hash(s, algorithm), upperCase);
		}

		/// <summary>
		/// Converts byte array (binary data) to hex-encoded string.
		/// </summary>
		public static unsafe string BytesToHexString(byte[] a, bool upperCase = false)
		{
			if(a == null) return null;
			int i, n = a.Length;
			string R = new string('\0', n * 2);
			fixed (char* p = R)
			{
				if(upperCase) {
					for(i = 0; i < n; i++) {
						p[i * 2] = _HalfByteToHexCharU(a[i] >> 4);
						p[i * 2 + 1] = _HalfByteToHexCharU(a[i] & 0xf);
					}
				} else {
					for(i = 0; i < n; i++) {
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int _HexCharToHalfByte(char c)
		{
			if(c >= '0' && c <= '9') return c - '0';
			if(c >= 'A' && c <= 'F') return c - ('A'-10);
			if(c >= 'a' && c <= 'f') return c - ('a'-10);
			//throw new ArgumentException(); //makes slower
			return 0;
		}

		/// <summary>
		/// Converts hex-encoded string to byte array (binary data).
		/// </summary>
		public static byte[] BytesFromHexString(string s)
		{
			if(s == null) return null;
			int i, n = s.Length / 2;
			var r = new byte[n];
			for(i = 0; i < n; i++) {
				r[i] = (byte)((_HexCharToHalfByte(s[i * 2]) << 4) | _HexCharToHalfByte(s[i * 2 + 1]));
            }
			return r;

			//speed: fast enough without a lookup table. Faster than BytesToHexString.
			//These functions have similar speed as Convert.ToBase64String and Convert.FromBase64String.
		}
	}
}
