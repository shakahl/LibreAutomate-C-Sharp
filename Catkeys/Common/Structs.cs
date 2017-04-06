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

using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Catkeys
{
	/// <summary>
	/// Used for method parameters that accept one of two types.
	/// </summary>
	/// <remarks>
	/// Some methods have one or several parameters that must be one of several types. There are several ways of implementing such functions.
	/// 1. Overloaded methods. However some programmers often don't like it, because in some cases need to create many overloads, then maintain their documentation. Also then method users have to spend much time to find the correct overload.
	/// 2. Generic methods. However in most cases the compiler cannot protect from using an unsupported type. Also then method users don't see the allowed types instantly in intellisense.
	/// 3. Using the Object type for the parameter. However then the compiler does not protect from using an unsupported type. Also then method users don't see the allowed types instantly in intellisense. Also slower, especially with value types (need boxing).
	/// 4. Using this type for the parameter. It does not have the above problems.
	/// 
	/// When a parameter is of type Types&lt;T1, T2&gt;, method users can pass only values of type T1 or T2, else compiler error. They see the allowed types instantly in intellisense.
	/// Also there are similar types that can be used to support more parameter types:
	/// <see cref="Types{T1, T2, T3}"/>
	/// <see cref="Types{T1, T2, T3, T4}"/>
	/// 
	/// To support null (for example for optional parameters), use nullable. See examples.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// static void Example1(Types<string, IEnumerable<string>> x)
	/// {
	/// 	switch(x.type) {
	/// 	case 1: PrintList("string", x.v1); break;
	/// 	case 2: PrintList("IEnumerable<string>", x.v2); break;
	/// 	default: throw new ArgumentException(); //0 if has default value, eg assigned default(Types<string, IEnumerable<string>>), unlikely
	/// 	}
	/// }
	/// 
	/// static void Example2(int param1, Types<int, double>? optionalParam = null)
	/// {
	/// 	var p = optionalParam.GetValueOrDefault();
	/// 	switch(p.type) {
	/// 	case 0: Print("null"); break;
	/// 	case 1: PrintList("int", p.v1); break;
	/// 	case 2: PrintList("double", p.v2); break;
	/// 	}
	/// }
	/// 
	/// static void TestExamples()
	/// {
	/// 	Example1("S");
	/// 	Example1(new string[] { "a", "b" });
	/// 	//Example1(5); //compiler error
	/// 	//Example1(null); //compiler error
	/// 	Example1((string)null);
	/// 	//Example1(default(Types<string, IEnumerable<string>>)); //the function throws exception
	/// 
	/// 	Example2(0, 5);
	/// 	Example2(0, 5.5);
	/// 	//Example2(0, "S"); //compiler error
	/// 	Example2(0, null);
	/// 	Example2(0);
	/// 	//Example2(0, default(Types<int, double>)); //the function throws exception
	/// }
	/// ]]></code>
	/// </example>
	public struct Types<T1, T2>
	{
		/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), 0 if unassigned (unlikely). </summary>
		public byte type;
		/// <summary> Value of type T1. Valid when type is 1. </summary>
		public T1 v1;
		/// <summary> Value of type T2. Valid when type is 2. </summary>
		public T2 v2;

		Types(T1 v) { type = 1; v1 = v; v2 = default(T2); }
		Types(T2 v) { type = 2; v2 = v; v1 = default(T1); }

		/// <summary> Assignment of a value of type T1. </summary>
		public static implicit operator Types<T1, T2>(T1 x) { return new Types<T1, T2>(x); }
		/// <summary> Assignment of a value of type T2. </summary>
		public static implicit operator Types<T1, T2>(T2 x) { return new Types<T1, T2>(x); }

		//Prevent assigning null to a non-nullable Types variable when one of its types is a reference type, eg Types<string, int>.
		//Assigning null would set type 1 - string, which is confusing. Now compiler error.
		//Need two operators because with single operator would allow to assign null when all value types, eg Types<double, int>.
		//Because compiler allows to assign null only if exactly one operator is of a reference type.
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2>(_Private.TypesNull1 n) { return default(Types<T1, T2>); }
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2>(_Private.TypesNull2 n) { return default(Types<T1, T2>); }
	}

#pragma warning disable 3008 //not CLS-compliant
	namespace _Private
	{
		/// <tocexclude />
		internal class NamespaceDoc
		{
			//SHFB uses this for namespace documentation.
		}

		/// <summary>Infrastructure.</summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public class TypesNull1 { }
		/// <summary>Infrastructure.</summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public class TypesNull2 { }
	}
#pragma warning restore 3008

	/// <summary>
	/// Used for method parameters that accept one of three types.
	/// More info: <see cref="Types{T1, T2}"/>.
	/// </summary>
	[StructLayout(LayoutKind.Auto)]
	public struct Types<T1, T2, T3>
	{
		/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), 3 if T3 (v3 is valid), 0 if unassigned (unlikely). </summary>
		public byte type;
		/// <summary> Value of type T1. Valid when type is 1. </summary>
		public T1 v1;
		/// <summary> Value of type T2. Valid when type is 2. </summary>
		public T2 v2;
		/// <summary> Value of type T3. Valid when type is 3. </summary>
		public T3 v3;

		Types(T1 v) : this() { type = 1; v1 = v; }
		Types(T2 v) : this() { type = 2; v2 = v; }
		Types(T3 v) : this() { type = 3; v3 = v; }

		/// <summary> Assignment of a value of type T1. </summary>
		public static implicit operator Types<T1, T2, T3>(T1 x) { return new Types<T1, T2, T3>(x); }
		/// <summary> Assignment of a value of type T2. </summary>
		public static implicit operator Types<T1, T2, T3>(T2 x) { return new Types<T1, T2, T3>(x); }
		/// <summary> Assignment of a value of type T3. </summary>
		public static implicit operator Types<T1, T2, T3>(T3 x) { return new Types<T1, T2, T3>(x); }

		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3>(_Private.TypesNull1 n) { return default(Types<T1, T2, T3>); }
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3>(_Private.TypesNull2 n) { return default(Types<T1, T2, T3>); }
	}

	/// <summary>
	/// Used for method parameters that accept one of four types.
	/// More info: <see cref="Types{T1, T2}"/>.
	/// </summary>
	public struct Types<T1, T2, T3, T4>
	{
		/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), and so on. 0 if unassigned (unlikely). </summary>
		public byte type;
		/// <summary> Value of type T1. Valid when type is 1. </summary>
		public T1 v1;
		/// <summary> Value of type T2. Valid when type is 2. </summary>
		public T2 v2;
		/// <summary> Value of type T3. Valid when type is 3. </summary>
		public T3 v3;
		/// <summary> Value of type T4. Valid when type is 4. </summary>
		public T4 v4;

		Types(T1 v) : this() { type = 1; v1 = v; }
		Types(T2 v) : this() { type = 2; v2 = v; }
		Types(T3 v) : this() { type = 3; v3 = v; }
		Types(T4 v) : this() { type = 4; v4 = v; }

		/// <summary> Assignment of a value of type T1. </summary>
		public static implicit operator Types<T1, T2, T3, T4>(T1 x) { return new Types<T1, T2, T3, T4>(x); }
		/// <summary> Assignment of a value of type T2. </summary>
		public static implicit operator Types<T1, T2, T3, T4>(T2 x) { return new Types<T1, T2, T3, T4>(x); }
		/// <summary> Assignment of a value of type T3. </summary>
		public static implicit operator Types<T1, T2, T3, T4>(T3 x) { return new Types<T1, T2, T3, T4>(x); }
		/// <summary> Assignment of a value of type T4. </summary>
		public static implicit operator Types<T1, T2, T3, T4>(T4 x) { return new Types<T1, T2, T3, T4>(x); }

		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3, T4>(_Private.TypesNull1 n) { return default(Types<T1, T2, T3, T4>); }
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3, T4>(_Private.TypesNull2 n) { return default(Types<T1, T2, T3, T4>); }
	}


#pragma warning disable 660, 661 //no Equals()

	/// <summary>
	/// Similar to IntPtr (can be 32-bit or 64-bit), but more useful for usually-non-pointer values, eg wParam/lParam of SendMessage.
	/// Unlike IntPtr:
	///		Has implicit casts from/to most integral types.
	///		Does not check overflow when casting from uint etc. IntPtr throws exception on overflow, which can create strange bugs.
	/// </summary>
	/// <remarks>
	///	There is no struct WPARAM. Use LPARAM instead, because it is the same in all cases except when casting to long or ulong (ambigous signed/unsigned).
	///	There is no cast operators for enum. When need, cast through int or uint. For Wnd cast through IntPtr.
	/// </remarks>
	[DebuggerStepThrough]
	[Serializable]
	public unsafe struct LPARAM :IXmlSerializable
	{
#pragma warning disable 1591 //XML doc
		void* _v; //Not IntPtr, because it throws exception on overflow when casting from uint etc.

		LPARAM(void* v) { _v = v; }

		//LPARAM = int etc
		public static implicit operator LPARAM(void* x) { return new LPARAM(x); }
		public static implicit operator LPARAM(IntPtr x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(UIntPtr x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(int x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(uint x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(sbyte x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(byte x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(short x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(ushort x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(char x) { return new LPARAM((void*)(ushort)x); }
		public static implicit operator LPARAM(long x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(ulong x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(bool x) { return new LPARAM((void*)(x ? 1 : 0)); }
		//public static implicit operator LPARAM(Enum x) { return new LPARAM((void*)(int)x); } //error
		//public static implicit operator LPARAM(WPARAM x) { return new LPARAM(x); }
		//int etc = LPARAM
		public static implicit operator void* (LPARAM x) { return x._v; }
		public static implicit operator IntPtr(LPARAM x) { return (IntPtr)x._v; }
		public static implicit operator UIntPtr(LPARAM x) { return (UIntPtr)x._v; }
		public static implicit operator int(LPARAM x) { return (int)x._v; }
		public static implicit operator uint(LPARAM x) { return (uint)x._v; }
		public static implicit operator sbyte(LPARAM x) { return (sbyte)x._v; }
		public static implicit operator byte(LPARAM x) { return (byte)x._v; }
		public static implicit operator short(LPARAM x) { return (short)x._v; }
		public static implicit operator ushort(LPARAM x) { return (ushort)x._v; }
		public static implicit operator char(LPARAM x) { return (char)(ushort)x._v; }
		public static implicit operator long(LPARAM x) { return (long)x._v; }
		public static implicit operator ulong(LPARAM x) { return (ulong)x._v; }
		public static implicit operator bool(LPARAM x) { return x._v != null; }

		public static bool operator ==(LPARAM a, LPARAM b) { return a._v == b._v; }
		public static bool operator !=(LPARAM a, LPARAM b) { return a._v != b._v; }

		public override string ToString() { return ((IntPtr)_v).ToString(); }

		//IXmlSerializable implementation.
		//Need it because default serialization: 1. Gets only public members. 2. Exception if void*. 3. If would work, would format like <...><_v>value</_v></...>, but we need <...>value</...>.
		public XmlSchema GetSchema() { return null; }
		public void ReadXml(XmlReader reader) { _v = (void*)reader.ReadElementContentAsLong(); }
		public void WriteXml(XmlWriter writer) { writer.WriteValue((long)_v); }
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Contains point coordinates.
	/// The same as Point.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct POINT
	{
#pragma warning disable 1591 //XML doc
		public int x, y;

		public POINT(int x, int y) { this.x = x; this.y = y; }

		public static implicit operator POINT(Point p) { return new POINT(p.X, p.Y); }
		public static implicit operator Point(POINT p) { return new Point(p.x, p.y); }

		public static bool operator ==(POINT p1, POINT p2) { return p1.x == p2.x && p1.y == p2.y; }
		public static bool operator !=(POINT p1, POINT p2) { return !(p1 == p2); }

		public override string ToString() { return $"{{x={x} y={y}}}"; }

		/// <summary>
		/// POINT with all fields 0.
		/// </summary>
		public static readonly POINT Empty;
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Contains width and height.
	/// The same as Size.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct SIZE
	{
#pragma warning disable 1591 //XML doc
		public int cx, cy;

		public SIZE(int cx, int cy) { this.cx = cx; this.cy = cy; }

		public static implicit operator SIZE(Size z) { return new SIZE(z.Width, z.Height); }
		public static implicit operator Size(SIZE z) { return new Size(z.cx, z.cy); }

		public static bool operator ==(SIZE s1, SIZE s2) { return s1.cx == s2.cx && s1.cy == s2.cy; }
		public static bool operator !=(SIZE s1, SIZE s2) { return !(s1 == s2); }

		public override string ToString() { return $"{{cx={cx} cy={cy}}}"; }

		/// <summary>
		/// SIZE with all fields 0.
		/// </summary>
		public static readonly SIZE Empty;
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Contains rectangle coordinates.
	/// Unlike Rectangle, which contains fields for width and height and therefore cannot be used with Windows API functions, RECT contains fields for right and bottom and can be used with Windows API.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct RECT
	{
#pragma warning disable 1591 //XML doc
		public int left, top, right, bottom;

		public RECT(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight)
		{
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if(useWidthHeight) { right += left; bottom += top; }
		}

		public static implicit operator RECT(Rectangle r) { return new RECT(r.Left, r.Top, r.Width, r.Height, true); }
		public static implicit operator Rectangle(RECT r) { return new Rectangle(r.left, r.top, r.Width, r.Height); }

		public static bool operator ==(RECT r1, RECT r2) { return r1.left == r2.left && r1.right == r2.right && r1.top == r2.top && r1.bottom == r2.bottom; }
		public static bool operator !=(RECT r1, RECT r2) { return !(r1 == r2); }

		public void Set(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight)
		{
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if(useWidthHeight) { right += left; bottom += top; }
		}

		/// <summary>
		/// Sets all fields to 0.
		/// </summary>
		public void SetEmpty() { left = right = top = bottom = 0; }

		/// <summary>
		/// Returns true if this rectangle is empty or invalid:
		/// return right&lt;=left || bottom&lt;=top;
		/// </summary>
		public bool IsEmpty { get => right <= left || bottom <= top; }

		/// <summary>
		/// Gets or sets width.
		/// </summary>
		public int Width { get => right - left; set { right = left + value; } }

		/// <summary>
		/// Gets or sets height.
		/// </summary>
		public int Height { get => bottom - top; set { bottom = top + value; } }

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(int x, int y) { return x >= left && x < right && y >= top && y < bottom; }

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(POINT p) { return Contains(p.x, p.y); }

		/// <summary>
		/// Returns true if this rectangle contains entire specified rectangle.
		/// </summary>
		public bool Contains(RECT r2) { return r2.left >= left && r2.top >= top && r2.right <= right && r2.bottom <= bottom; }

		/// <summary>
		/// Returns true if this rectangle and another rectangle intersect.
		/// </summary>
		public bool Intersects(RECT r2) { RECT r; return Api.IntersectRect(out r, ref this, ref r2); }

		/// <summary>
		/// Virtually moves the rectangle:
		/// left+=dx; right+=dx; top+=dy; bottom+=dy;
		/// </summary>
		public void Offset(int dx, int dy) { left += dx; right += dx; top += dy; bottom += dy; }

		/// <summary>
		/// Makes the rectangle bigger or smaller:
		/// left-=dx; right+=dx; top-=dy; bottom+=dy;
		/// Note: negative coordinates can make the rectangle invalid (right&lt;left or bottom&lt;top).
		/// </summary>
		public void Inflate(int dx, int dy) { left -= dx; right += dx; top -= dy; bottom += dy; }

		/// <summary>
		/// Returns the intersection rectangle of two rectangles.
		/// If they don't intersect, returns empty rectangle (IsEmpty would return true).
		/// </summary>
		public static RECT Intersect(RECT r1, RECT r2) { RECT r; Api.IntersectRect(out r, ref r1, ref r2); return r; }

		/// <summary>
		/// Returns the rectangle that would contain two entire rectangles.
		/// </summary>
		public static RECT Union(RECT r1, RECT r2) { RECT r; Api.UnionRect(out r, ref r1, ref r2); return r; }

		/// <summary>
		/// Adjusts this rectangle so that it can be used to create a new window that will be entirely in specified screen.
		/// Initial coordinates must be relative to the specified screen. The function replaces them with coordinates relative to the primary screen.
		/// By default, 0 and negative left/top coordinates are interpreted as: 0 - screen center, &lt;0 - relative to the right or bottom edge of the screen.
		/// </summary>
		/// <param name="screen">Use this screen. If null, use the primary screen. See <see cref="Screen_.FromObject"/>.</param>
		/// <param name="workArea">Use the work area, not whole screen.</param>
		/// <param name="limitSize">If rectangle is bigger than screen, resize it.</param>
		/// <param name="rawXY">Don't interpret 0 and negative left/top coordinates in a special way. They are relative to the top-left corner of the screen.</param>
		/// <seealso cref="Wnd.MoveInScreen"/>
		public void MoveInScreen(object screen = null, bool workArea = true, bool limitSize = false, bool rawXY = false)
		{
			Wnd.LibMoveInScreen(false, Wnd0, ref this, screen, workArea, false, limitSize, rawXY);
		}

		/// <summary>
		/// Adjusts a rectangle so that it can be used to create a new window that will be entirely in screen.
		/// Similar to <see cref="MoveInScreen">MoveInScreen</see>, but initial rectangle coordinates are relative to the primary screen and are not interpreted in a special way.
		/// The function adjusts rectangle coordinates to ensure that whole rectangle is in screen.
		/// </summary>
		/// <param name="screen">Use this screen. If null, use screen of the rectangle. See <see cref="Screen_.FromObject"/>.</param>
		/// <param name="workArea">Use the work area, not whole screen.</param>
		/// <param name="limitSize">If rectangle is bigger than screen, resize it.</param>
		/// <seealso cref="Wnd.EnsureInScreen"/>
		public void EnsureInScreen(object screen = null, bool workArea = true, bool limitSize = false)
		{
			Wnd.LibMoveInScreen(false, Wnd0, ref this, screen, workArea, true, limitSize, false);
		}

		/// <summary>
		/// RECT with all fields 0.
		/// </summary>
		public static readonly RECT Empty;

		public override string ToString() { return $"{{L={left} T={top} R={right} B={bottom}  Width={Width} Height={Height}}}"; }
#pragma warning restore 1591 //XML doc
	}
#pragma warning restore 660, 661

}
