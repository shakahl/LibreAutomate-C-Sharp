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

using Au.Types;

namespace Au
{
	/// <summary>
	/// Simple calculation functions.
	/// </summary>
	//[DebuggerStepThrough]
	public static class AMath
	{
		/// <summary>
		/// Creates uint by placing (ushort)loWord in bits 1-16 and (ushort)hiWord in bits 17-32.
		/// Like C macro MAKELONG, MAKEWPARAM, MAKELPARAM, MAKELRESULT.
		/// </summary>
		public static uint MakeUint(uint loWord, uint hiWord) => ((hiWord & 0xffff) << 16) | (loWord & 0xffff);
		
		/// <summary>
		/// Creates uint by placing (ushort)loWord in bits 1-16 and (ushort)hiWord in bits 17-32.
		/// Like C macro MAKELONG, MAKEWPARAM, MAKELPARAM, MAKELRESULT.
		/// </summary>
		public static uint MakeUint(int loWord, int hiWord) => MakeUint((uint)loWord, (uint)hiWord);

		/// <summary>
		/// Creates ushort by placing (byte)loByte in bits 1-8 and (byte)hiByte in bits 9-16.
		/// Like C macro MAKEWORD.
		/// </summary>
		public static ushort MakeUshort(uint loByte, uint hiByte) => (ushort)(((hiByte & 0xff) << 8) | (loByte & 0xff));
		
		/// <summary>
		/// Creates ushort by placing (byte)loByte in bits 1-8 and (byte)hiByte in bits 9-16.
		/// Like C macro MAKEWORD.
		/// </summary>
		public static ushort MakeUshort(int loByte, int hiByte) => MakeUshort((uint)loByte, (uint)hiByte);

		/// <summary>
		/// Gets bits 1-16 as ushort.
		/// Like C macro LOWORD.
		/// </summary>
		/// <remarks>
		/// The parameter is interpreted as uint. Its declared type is LPARAM because it allows to avoid explicit casting from other integer types and IntPtr (casting from IntPtr to uint could throw OverflowException).
		/// </remarks>
		public static ushort LoUshort(LPARAM x) => (ushort)((uint)x & 0xFFFF);

		/// <summary>
		/// Gets bits 17-32 as ushort.
		/// Like C macro HIWORD.
		/// </summary>
		/// <remarks>
		/// The parameter is interpreted as uint. Its declared type is LPARAM because it allows to avoid explicit casting from other integer types and IntPtr (casting from IntPtr to uint could throw OverflowException).
		/// </remarks>
		public static ushort HiUshort(LPARAM x) => (ushort)((uint)x >> 16);

		/// <summary>
		/// Gets bits 1-16 as short.
		/// Like C macro GET_X_LPARAM.
		/// </summary>
		/// <remarks>
		/// The parameter is interpreted as uint. Its declared type is LPARAM because it allows to avoid explicit casting from other integer types and IntPtr (casting from IntPtr to uint could throw OverflowException).
		/// </remarks>
		public static short LoShort(LPARAM x) => (short)((uint)x & 0xFFFF);

		/// <summary>
		/// Gets bits 17-32 as short.
		/// Like C macro GET_Y_LPARAM.
		/// </summary>
		/// <remarks>
		/// The parameter is interpreted as uint. Its declared type is LPARAM because it allows to avoid explicit casting from other integer types and IntPtr (casting from IntPtr to uint could throw OverflowException).
		/// </remarks>
		public static short HiShort(LPARAM x) => (short)((uint)x >> 16);

		/// <summary>
		/// Gets bits 1-8 as byte.
		/// Like C macro LOBYTE.
		/// </summary>
		public static byte LoByte(ushort x) => (byte)((uint)x & 0xFF);

		/// <summary>
		/// Gets bits 9-16 as byte.
		/// Like C macro HIBYTE.
		/// </summary>
		public static byte HiByte(ushort x) => (byte)((uint)x >> 8);

		/// <summary>
		/// Returns <c>number * multiply / divide</c>.
		/// Multiplies without overflow and rounds the result up or down to the nearest integer.
		/// </summary>
		/// <exception cref="OverflowException"></exception>
		/// <exception cref="DivideByZeroException"></exception>
		public static int MulDiv(int number, int multiply, int divide)
		{
			long r = (long)number * multiply;
			int d = divide / 2; if(r < 0 == divide < 0) r += d; else r -= d; //round
			return checked((int)(r / divide));

			//This code produces the same results as API MulDiv. Tested with millions of random and edge values. Faster.
			//The only difference, API does not support int.MinValue.
		}
		//public static int MulDiv(int number, int multiply, int divide) => Api.MulDiv(number, multiply, divide);

		/// <summary>
		/// Returns percent of part in whole.
		/// </summary>
		public static int Percent(int whole, int part) => (int)(100L * part / whole);

		/// <summary>
		/// Returns percent of part in whole.
		/// </summary>
		public static double Percent(double whole, double part) => 100.0 * part / whole;

		/// <summary>
		/// If value is divisible by alignment, returns value. Else returns nearest bigger number that is divisible by alignment.
		/// </summary>
		/// <param name="value">An integer value.</param>
		/// <param name="alignment">Alignment. Must be a power of two (2, 4, 8, 16...).</param>
		/// <remarks>
		/// For example if alignment is 4, returns 4 if value is 1-4, returns 8 if value is 5-8, returns 12 if value is 9-10, and so on.
		/// </remarks>
		public static uint AlignUp(uint value, uint alignment) => (value + (alignment - 1)) & ~(alignment - 1);

		/// <summary>
		/// If value is divisible by alignment, returns value. Else returns nearest bigger number that is divisible by alignment.
		/// </summary>
		/// <param name="value">An integer value.</param>
		/// <param name="alignment">Alignment. Must be a power of two (2, 4, 8, 16...).</param>
		/// <remarks>
		/// For example if alignment is 4, returns 4 if value is 1-4, returns 8 if value is 5-8, returns 12 if value is 9-10, and so on.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// for (int i = 0; i <= 20; i++) AOutput.Write(i, AMath.AlignUp(i, 4));
		/// ]]></code>
		/// </example>
		public static int AlignUp(int value, uint alignment) => (int)AlignUp((uint)value, alignment);

		/// <summary>
		/// Swaps values of variables a and b: <c>T t = a; a = b; b = t;</c>
		/// </summary>
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a; a = b; b = t;
		}

		/// <summary>
		/// Calculates angle degrees from coordinates x and y.
		/// </summary>
		public static double AngleFromXY(int x, int y) => Math.Atan2(y, x) * (180 / Math.PI);

		/// <summary>
		/// Calculates distance between two points.
		/// </summary>
		public static double Distance(POINT p1, POINT p2)
		{
			if(p1.y == p2.y) return Math.Abs(p2.x - p1.x); //horizontal line
			if(p1.x == p2.x) return Math.Abs(p2.y - p1.y); //vertical line

			long dx = p2.x - p1.x, dy = p2.y - p1.y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		/// <summary>
		/// Calculates distance between rectangle and point.
		/// Returns 0 if point is in rectangle.
		/// </summary>
		public static double Distance(RECT r, POINT p)
		{
			r.Normalize(swap: true);
			if(r.Contains(p)) return 0;
			int x = p.x < r.left ? r.left : (p.x > r.right ? r.right : p.x);
			int y = p.y < r.top ? r.top : (p.y > r.bottom ? r.bottom : p.y);
			return Distance((x, y), p);
		}
	}
}
