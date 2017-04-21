//Small extension classes for .NET classes. Except those that have own files, eg String_.
//Naming:
//	Class name: related .NET class name with _ suffix.
//	Extension method name: related .NET method name with _ suffix. Or new name with _ suffix.
//	Static method name: any name without _ suffix.

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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Security; //for XML comments

using Catkeys;
using static Catkeys.NoClass;


namespace Catkeys
{
	/// <summary>
	/// Extends the .NET class Screen.
	/// </summary>
	public static class Screen_
	{
		/// <summary>
		/// Special screen index to specify the primary screen.
		/// </summary>
		public const int Primary = 0;
		/// <summary>
		/// Special screen index to specify the screen of the mouse pointer.
		/// </summary>
		public const int OfMouse = -1;
		/// <summary>
		/// Special screen index to specify the screen of the active window.
		/// </summary>
		public const int OfActiveWindow = -2;

		/// <summary>
		/// Gets <see cref="Screen"/> object from 1-based screen index.
		/// index also can be one of constants defined in this class: Primary (0), OfMouse, OfActiveWindow.
		/// If index is invalid, gets the primary screen.
		/// </summary>
		/// <remarks>
		/// As screen index is used index in the array returned by <see cref="Screen.AllScreens"/> + 1. It is not the screen index that you can see in Control Panel.
		/// </remarks>
		public static Screen FromIndex(int index)
		{
			if(index > 0) {
				var a = Screen.AllScreens;
				if(--index < a.Length) return a[index];
				//SHOULDDO: ignore invisible pseudo-monitors associated with mirroring drivers.
				//	iScreen.AllScreens and EnumDisplayMonitors should include them,
				//	but in my recent tests with NetMeeting (noticed this long ago on an old OS version) and UltraVnc (wiki etc say) they didn't.
				//	Therefore I cannot test and add filtering. No problems if they are the last in the list. Never mind.
				//	Wiki about mirror drivers: https://en.wikipedia.org/wiki/Mirror_driver
			} else if(index == OfMouse) return Screen.FromPoint(Mouse.XY);
			else if(index == OfActiveWindow) return FromWindow(Wnd.WndActive);

			return Screen.PrimaryScreen;

			//speed compared with the API monitor functions:
			//	First time several times slower, but then many times faster. It means that .NET uses caching.
			//	Tested: correctly updates after changing multi-monitor config.
		}

		/// <summary>
		/// Gets <see cref="Screen"/> object of the screen that contains the specified window (the biggest part of it) or is nearest to it.
		/// If w handle is 0 or invalid, gets the primary screen (<see cref="Screen.FromHandle"/> would return an invalid object if the window handle is invalid).
		/// </summary>
		public static Screen FromWindow(Wnd w)
		{
			if(w.Is0) return Screen.PrimaryScreen;
			Screen R = Screen.FromHandle((IntPtr)w);
			if(R.Bounds.IsEmpty) return Screen.PrimaryScreen;
			return R;
		}

		/// <summary>
		/// Gets <see cref="Screen"/> object by index or of the screen that contains (or is nearest to) the specified window, rectangle or point.
		/// </summary>
		/// <param name="screen">
		/// Depends on type:
		/// <list type="bullet">
		/// <item>null: primary screen (calls <see cref="Screen.PrimaryScreen"/>).</item>
		/// <item>int: 1-based screen index (calls <see cref="FromIndex"/>), or Screen_.Primary, Screen_.OfMouse, Screen_.OfActiveWindow.</item>
		/// <item>Wnd: a window (calls <see cref="FromWindow"/>). If invalid, gets primary screen.</item>
		/// <item>POINT, Point: a point (calls <see cref="Screen.FromPoint"/>).</item>
		/// <item>RECT, Rectangle: a rectangle (calls <see cref="Screen.FromRectangle"/>).</item>
		/// <item>Screen: a <see cref="Screen"/> object. If invalid, gets primary screen.</item>
		/// </list>
		/// </param>
		/// <remarks>
		/// If something fails, gets primary screen.
		/// As screen index is used index in the array returned by <see cref="Screen.AllScreens"/> + 1. It is not the screen index that you can see in Control Panel.
		/// </remarks>
		public static Screen FromObject(object screen)
		{
			switch(screen) {
			case null: return Screen.PrimaryScreen;
			case int index: return FromIndex(index);
			case Wnd wnd: return FromWindow(wnd);
			case POINT p: return Screen.FromPoint(p);
			case RECT r: return Screen.FromRectangle(r);
			case Point pp: return Screen.FromPoint(pp);
			case Rectangle rr: return Screen.FromRectangle(rr);
			case Screen s: return s.Bounds.IsEmpty ? Screen.PrimaryScreen : s;
			}

			//throw new ArgumentException("Bad type.", nameof(screen)); //no, forgive it. This place is too deep etc to throw exceptions.
			return Screen.PrimaryScreen;
		}

		/// <summary>
		/// Gets primary screen width.
		/// </summary>
		public static int Width { get => Api.GetSystemMetrics(Api.SM_CXSCREEN); }
		/// <summary>
		/// Gets primary screen height.
		/// </summary>
		public static int Height { get => Api.GetSystemMetrics(Api.SM_CYSCREEN); }
		//public static int Width { get => Screen.PrimaryScreen.Bounds.Width; } //faster (gets cached value), but very slow first time, eg 15 ms

		/// <summary>
		/// Gets primary screen work area.
		/// </summary>
		public static unsafe RECT WorkArea
		{
			get
			{
				RECT r;
				Api.SystemParametersInfo(Api.SPI_GETWORKAREA, 0, (void*)&r, 0);
				return r;
			}
		}
	}

	/// <summary>
	/// Extends the .NET struct Color.
	/// </summary>
	public static class Color_
	{
		/// <summary>
		/// Converts color from ARGB (0xAARRGGBB) to ABGR (0xAABBGGRR) or vice versa (swaps the red and blue bytes).
		/// ARGB is used in .NET, GDI+ and HTML/CSS.
		/// ABGR is used by most Windows native API.
		/// </summary>
		public static uint SwapRedBlue(uint color)
		{
			return (color & 0xff00ff00) | ((color << 16) & 0xff0000) | ((color >> 16) & 0xff);
		}

		/// <summary>
		/// Converts from System.Drawing.Color to native Windows COLORREF with alpha (0xAABBGGRR).
		/// </summary>
		public static uint ColorToABGR(Color color)
		{
			return SwapRedBlue((uint)color.ToArgb());
		}

		/// <summary>
		/// Converts from native Windows COLORREF with alpha (0xAABBGGRR) to System.Drawing.Color.
		/// </summary>
		public static Color ColorFromABGR(uint color)
		{
			return Color.FromArgb((int)SwapRedBlue(color));
		}

		/// <summary>
		/// Converts from native Windows COLORREF without alpha (0x00BBGGRR) to opaque System.Drawing.Color.
		/// The alpha byte of the color argument can be 0 or any other value. Makes it 0xFF.
		/// </summary>
		public static Color ColorFromBGR(uint color)
		{
			return Color.FromArgb((int)(SwapRedBlue(color) | 0xFF000000));
		}

		/// <summary>
		/// Converts from ARGB uint (0xAARRGGBB) to System.Drawing.Color.
		/// Same as <see cref="Color.FromArgb(int)"/> but don't need <c>unchecked((int)0xFF...)</c>.
		/// </summary>
		public static Color ColorFromARGB(uint color)
		{
			return Color.FromArgb((int)color);
		}

		/// <summary>
		/// Converts from RGB uint (0x00RRGGBB) to opaque System.Drawing.Color.
		/// Same as <see cref="Color.FromArgb(int)"/> but don't need to specify alpha. You can replace <c>Color.FromArgb(unchecked((int)0xFF123456))</c> with <c>Color_.ColorFromRGB(0x123456)</c>.
		/// </summary>
		public static Color ColorFromRGB(uint color)
		{
			return Color.FromArgb((int)(color | 0xFF000000));
		}

		/// <summary>
		/// Converts string to opaque System.Drawing.Color.
		/// The string can be a color name (<see cref="Color.FromName(string)"/>) or 0xRRGGBB.
		/// Returns false if invalid name.
		/// </summary>
		public static bool ColorFromNameOrRGB(string s, out Color c)
		{
			if(Empty(s)) goto ge;
			if(s[0] == '0') {
				int i = s.ToInt32_(0, out int len);
				if(len == 0 || (len < s.Length && char.IsLetterOrDigit(s[len]))) goto ge;
				c = ColorFromRGB((uint)i);
			} else c = Color.FromName(s);
			return (c.A != 0);
			ge:
			c = default(Color);
			return false;
		}
	}

	/// <summary>
	/// Add extension methods to some .NET classes.
	/// </summary>
	public static class DotNetExtensions
	{
		#region Control

		/// <summary>
		/// Gets window handle as <see cref="Wnd"/>.
		/// The same as the explicit cast Control-to-Wnd.
		/// </summary>
		public static Wnd Wnd_(this Control t)
		{
			return (Wnd)t;
		}

		/// <summary>
		/// Gets mouse cursor position in client area coordinates.
		/// </summary>
		public static Point MouseClientXY_(this Control t)
		{
			return ((Wnd)t).MouseClientXY;
		}

		/// <summary>
		/// Gets mouse cursor position in window coordinates.
		/// </summary>
		public static Point MouseWindowXY_(this Control t)
		{
			var p = Mouse.XY;
			var k = t.Location;
			return new Point(p.x - k.X, p.y - k.Y);
		}

		/// <summary>
		/// Sets the textual cue, or tip, that is displayed by the edit control to prompt the user for information.
		/// Does not if Multiline.
		/// Sends API <msdn>EM_SETCUEBANNER</msdn>.
		/// </summary>
		public static void SetCueBanner_(this TextBox t, string text, bool showWhenFocused = false)
		{
			Debug.Assert(!t.Multiline);
			((Wnd)t).SendS(Api.EM_SETCUEBANNER, showWhenFocused, text);
		}

		/// <summary>
		/// Sets the textual cue, or tip, that is displayed by the ComboBox edit control to prompt the user for information.
		/// Sends API <msdn>CB_SETCUEBANNER</msdn>.
		/// </summary>
		public static void SetCueBanner_(this ComboBox t, string text)
		{
			((Wnd)t).SendS(Api.CB_SETCUEBANNER, 0, text);
		}

		#endregion

		#region Xml

		/// <summary>
		/// Gets XML attribute value.
		/// If the attribute does not exist, returns null.
		/// If the attribute value is empty, returns "".
		/// </summary>
		public static string Attribute_(this XElement t, XName name)
		{
			return t.Attribute(name)?.Value;
		}

		/// <summary>
		/// Gets XML attribute value.
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty, returns "".
		/// </summary>
		public static string Attribute_(this XElement t, XName name, string defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value : defaultValue;
		}

		/// <summary>
		/// Gets XML attribute value.
		/// If the attribute does not exist, sets value=null and returns false.
		/// </summary>
		public static bool Attribute_(this XElement t, out string value, XName name)
		{
			value = t.Attribute(name)?.Value;
			return value != null;
		}

		/// <summary>
		/// Gets attribute value converted to int (<see cref="String_.ToInt32_(string)"/>).
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty or does not begin with a valid number, returns 0.
		/// </summary>
		public static int Attribute_(this XElement t, XName name, int defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value.ToInt32_() : defaultValue;
		}

		/// <summary>
		/// Gets attribute value converted to int (<see cref="String_.ToInt32_(string)"/>).
		/// If the attribute does not exist, sets value=0 and returns false.
		/// If the attribute value is empty or does not begin with a valid number, sets value=0 and returns true.
		/// </summary>
		public static bool Attribute_(this XElement t, out int value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0; return false; }
			value = x.Value.ToInt32_();
			return true;
		}

		/// <summary>
		/// Gets attribute value converted to float (<see cref="String_.ToFloat_"/>).
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty or is not a valid float number, returns 0F.
		/// </summary>
		public static float Attribute_(this XElement t, XName name, float defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value.ToFloat_() : defaultValue;
		}

		/// <summary>
		/// Gets attribute value converted to float (<see cref="String_.ToFloat_"/>).
		/// If the attribute does not exist, sets value=0F and returns false.
		/// If the attribute value is empty or is not a valid number, sets value=0F and returns true.
		/// </summary>
		public static bool Attribute_(this XElement t, out float value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0F; return false; }
			value = x.Value.ToFloat_();
			return true;
		}

		/// <summary>
		/// Returns true if this element has the specified attribute.
		/// </summary>
		public static bool HasAttribute_(this XElement t, XName name)
		{
			return t.Attribute(name) != null;
		}

		/// <summary>
		/// Gets the first found descendant element.
		/// Returns null if not found.
		/// </summary>
		public static XElement Descendant_(this XElement t, XName name)
		{
			return t.Descendants(name).FirstOrDefault();
		}

		/// <summary>
		/// Gets the first found descendant element that has the specified attribute.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value. If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Descendant_(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in t.Descendants(name)) {
				var a = el.Attribute(attributeName); if(a == null) continue;
				if(attributeValue != null && !a.Value.Equals_(attributeValue, ignoreCase)) continue;
				return el;
			}
			return null;

			//speed: several times faster than XPathSelectElement
		}

		/// <summary>
		/// Gets the first found direct child element that has the specified attribute.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value. If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Element_(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in t.Elements(name)) {
				var a = el.Attribute(attributeName); if(a == null) continue;
				if(attributeValue != null && !a.Value.Equals_(attributeValue, ignoreCase)) continue;
				return el;
			}
			return null;
		}

		/// <summary>
		/// Gets previous sibling element.
		/// Returns null if no element.
		/// </summary>
		public static XElement PreviousElement_(this XElement t)
		{
			for(XNode n = t.PreviousNode; n != null; n = n.PreviousNode) {
				if(n is XElement e) return e;
			}
			return null;
		}

		/// <summary>
		/// Gets next sibling element.
		/// Returns null if no element.
		/// </summary>
		public static XElement NextElement_(this XElement t)
		{
			for(XNode n = t.NextNode; n != null; n = n.NextNode) {
				if(n is XElement e) return e;
			}
			return null;
		}
		#endregion

		#region value types

		/// <summary>
		/// Returns true if t.Width &lt;= 0 || t.Height &lt;= 0.
		/// This extension method has been added because Rectangle.IsEmpty returns true only when all fields are 0, which is not very useful.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty_(this Rectangle t)
		{
			return t.Width <= 0 || t.Height <= 0;
		}

#if dont_use
	////[DebuggerStepThrough]
	//public static class Int32_
	//{
	//	//This does not work because int is a value type and therefore the x parameter is a copy. Better don't need this func.
	//	//public static void LimitMinMax_(this int x, int min, int max)
	//	//{
	//	//	if(x>max) x=max;
	//	//	if(x<min) x=min;
	//	//}

	//	////Allows code like this: foreach(int u in 5.Times()) Print(u);
	//	////However the code is longer than for(int u=0; u<5; u++) Print(u);
	//	//public static IEnumerable<int> Times(this int nTimes)
	//	//{
	//	//	for(int i = 0; i<nTimes; i++) yield return i;
	//	//}
	//}

	/// <summary>
	/// Enum extension methods.
	/// </summary>
	[DebuggerStepThrough]
	public static class Enum_
	{
		/// <summary>
		/// Returns true if this has one or more flags specified in flagOrFlags.
		/// It is different from HasFlag, which returns true if this has ALL specified flags.
		/// Speed: 0.1 mcs. It is several times slower than HasFlag, and 100 times slower than operators.
		/// Like HasFlag, does not catch wrong enum type at compile time.
		/// </summary>
		public static bool HasAny(this Enum t, Enum flagOrFlags)
		{
			return (Convert.ToUInt64(t) & Convert.ToUInt64(flagOrFlags)) !=0;
			//info: cannot apply operator & to Enum, or cast to uint, or use unsafe pointer.
		}
		////same speed:
		//public static bool Has<T>(this T t, T flagOrFlags) where T: struct
		//{
		//	//return ((uint)t & (uint)flagOrFlags) !=0; //error
		//	return (Convert.ToUInt64(t) & Convert.ToUInt64(flagOrFlags)) !=0;
		//}

		public static void SetFlag(ref Enum t, Enum flag, bool set)
		{
			ulong _t = Convert.ToUInt64(t), _f = Convert.ToUInt64(flag);
			if(set) _t|=_f; else _t&=~_f;
			t=(Enum)(object)_t; //can do it better?
			//info: Cannot make this a true extension method.
			//	If we use 'this Enum t', t is a copy of the actual variable, because Enum is a value type.
			//	That is why we need ref.
			//Speed not tested.
		}
	}
#endif
		#endregion
	}
}
