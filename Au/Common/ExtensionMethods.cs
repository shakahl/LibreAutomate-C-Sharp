//Small extension classes for .NET classes. Except those that have own files.
//Naming:
//	Class name: related .NET class name with _ suffix.
//	Extension method name: related .NET method name with _ suffix. Or new name with _ suffix.
//	Static method name: any name without _ suffix.

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
using System.Linq;
using System.Xml.Linq;
using System.Security; //for XML comments
using System.Globalization;
using System.Windows.Forms;

using Au.Types;
using static Au.NoClass;


namespace Au.Types
{
	/// <summary>
	/// Adds extension methods to some .NET classes.
	/// </summary>
	public static partial class ExtensionMethods
	{
		#region Control

		/// <summary>
		/// If control handle still not created, creates handle.
		/// Like <see cref="Control.CreateHandle"/>, which is protected.
		/// Unlike <see cref="Control.CreateControl"/>, creates handle even if invisible, and does not create child control handles.
		/// </summary>
		public static void CreateHandle_(this Control t)
		{
			if(!t.IsHandleCreated) {
				var h = t.Handle;
			}
		}

		/// <summary>
		/// Gets mouse cursor position in client area coordinates.
		/// </summary>
		public static POINT MouseClientXY_(this Control t)
		{
			return ((Wnd)t).MouseClientXY;
		}

		/// <summary>
		/// Gets mouse cursor position in window coordinates.
		/// </summary>
		public static POINT MouseWindowXY_(this Control t)
		{
			POINT p = Mouse.XY;
			POINT k = t.Location;
			return new POINT(p.x - k.x, p.y - k.y);
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

		/// <summary>
		/// Creates a control, sets its commonly used properties (Bounds, Text, tooltip, Anchor) and adds it to the Controls collection of this.
		/// </summary>
		/// <typeparam name="T">Control class.</typeparam>
		/// <param name="t"></param>
		/// <param name="x">Left.</param>
		/// <param name="y">Top.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="text">The <see cref="Control.Text"/> property.</param>
		/// <param name="tooltip">Tooltip text.
		/// This function creates a ToolTip component and assigns it to the Tag property of this.</param>
		/// <param name="anchor">The <see cref="Control.Anchor"/> property.</param>
		public static T Add_<T>(this ContainerControl t, int x, int y, int width, int height, string text = null, string tooltip = null, AnchorStyles anchor = AnchorStyles.None/*, string name = null*/) where T : Control, new()
		{
			var c = new T();
			//if(!Empty(name)) c.Name = name;
			c.Bounds = new System.Drawing.Rectangle(x, y, width, height);
			if(anchor != AnchorStyles.None) c.Anchor = anchor;
			if(text != null) c.Text = text;
			if(!Empty(tooltip)) {
				var tt = t.Tag as ToolTip;
				if(tt == null) {
					t.Tag = tt = new ToolTip();
					//t.Disposed += (o, e) => Print((o as ContainerControl).Tag as ToolTip);
					//t.Disposed += (o, e) => ((o as ContainerControl).Tag as ToolTip)?.Dispose(); //it seems tooltip is auto-disposed when its controls are disposed. Anyway, this event is only if the form is disposed explicitly, but nobody does it.
				}
				tt.SetToolTip(c, tooltip);
			}
			t.Controls.Add(c);
			return c;
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
		/// Gets attribute value converted to int (<see cref="String_.ToInt_(string)"/>).
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty or does not begin with a valid number, returns 0.
		/// </summary>
		public static int Attribute_(this XElement t, XName name, int defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value.ToInt_() : defaultValue;
		}

		/// <summary>
		/// Gets attribute value converted to int (<see cref="String_.ToInt_(string)"/>).
		/// If the attribute does not exist, sets value=0 and returns false.
		/// If the attribute value is empty or does not begin with a valid number, sets value=0 and returns true.
		/// </summary>
		public static bool Attribute_(this XElement t, out int value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0; return false; }
			value = x.Value.ToInt_();
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
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value. If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Descendant_(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Descendants(name) : t.Descendants()) {
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
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value. If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Element_(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Elements(name) : t.Elements()) {
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
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="double.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToString_(this double t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="float.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToString_(this float t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="decimal.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToString_(this decimal t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Returns true if t.Width &lt;= 0 || t.Height &lt;= 0.
		/// This extension method has been added because Rectangle.IsEmpty returns true only when all fields are 0, which is not very useful.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty_(this System.Drawing.Rectangle t)
		{
			return t.Width <= 0 || t.Height <= 0;
		}

		//rejected: too simple. We have Print(uint), also can use $"0x{t:X}" or "0x" + t.ToString("X").
		///// <summary>
		///// Converts int to hexadecimal string like "0x3A".
		///// </summary>
		//public static string ToHex_(this int t)
		//{
		//	return "0x" + t.ToString("X");
		//}

		#endregion

		#region enum
#pragma warning disable CS3024 // Constraint type is not CLS-compliant (IConvertible uses uint)

		/// <summary>
		/// Returns true if this enum variable has flag(s) f (all bits).
		/// Compiled as inlined code <c>(t &amp; flag) == flags</c>. The same as Enum.HasFlag, but much much faster.
		/// The enum type must be of size 4 (default).
		/// </summary>
		public static bool Has_<T>(this T t, T flag) where T : struct, IComparable, IFormattable, IConvertible
		{
			int a = Unsafe.As<T, int>(ref t);
			int b = Unsafe.As<T, int>(ref flag);
			return (a & b) == b;
		}

		/// <summary>
		/// Returns true if this enum variable has one or more flag bits specified in f.
		/// Compiled as inlined code <c>(t &amp; flags) != 0</c>. This is different from Enum.HasFlag.
		/// The enum type must be of size 4 (default).
		/// </summary>
		public static bool HasAny_<T>(this T t, T flags) where T : struct, IComparable, IFormattable, IConvertible
		{
			int a = Unsafe.As<T, int>(ref t);
			int b = Unsafe.As<T, int>(ref flags);
			return (a & b) != 0;
		}

#pragma warning restore CS3024 // Constraint type is not CLS-compliant
		#endregion

		#region internal


		#endregion
	}
}
