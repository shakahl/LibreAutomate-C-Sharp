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
	/// Simple calculation functions.
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
		/// Returns true if character is '0' to '9'.
		/// </summary>
		public static bool IsDigit(byte c) { return c <= '9' && c >= '0'; }

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z'.
		/// </summary>
		public static bool IsAlpha(char c) { return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'); }

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z'.
		/// </summary>
		public static bool IsAlpha(byte c) { return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'); }

		/// <summary>
		/// Calculates angle degrees from coordinates x and y.
		/// </summary>
		public static double AngleFromXY(int x, int y)
		{
			return Math.Atan2(y, x) * (180 / Math.PI);
		}

	}
}
