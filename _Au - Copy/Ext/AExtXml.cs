using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Xml;

using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Adds extension methods and some static functions for <see cref="XElement"/> and <see cref="XDocument"/>.
	/// </summary>
	public static class AExtXml
	{
		/// <summary>
		/// Gets XML attribute value.
		/// If the attribute does not exist, returns null.
		/// If the attribute value is empty, returns "".
		/// </summary>
		public static string Attr(this XElement t, XName name)
		{
			return t.Attribute(name)?.Value;
		}

		/// <summary>
		/// Gets XML attribute value.
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty, returns "".
		/// </summary>
		public static string Attr(this XElement t, XName name, string defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value : defaultValue;
		}

		/// <summary>
		/// Gets XML attribute value.
		/// If the attribute does not exist, sets value=null and returns false.
		/// </summary>
		public static bool Attr(this XElement t, out string value, XName name)
		{
			value = t.Attribute(name)?.Value;
			return value != null;
		}

		/// <summary>
		/// Gets attribute value converted to int number.
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty or does not begin with a valid number, returns 0.
		/// </summary>
		public static int Attr(this XElement t, XName name, int defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value.ToInt() : defaultValue;
		}

		/// <summary>
		/// Gets attribute value converted to int number.
		/// If the attribute does not exist, sets value=0 and returns false.
		/// If the attribute value is empty or does not begin with a valid number, sets value=0 and returns true.
		/// </summary>
		public static bool Attr(this XElement t, out int value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0; return false; }
			value = x.Value.ToInt();
			return true;
		}

		/// <summary>
		/// Gets attribute value converted to long number.
		/// If the attribute does not exist, sets value=0 and returns false.
		/// If the attribute value is empty or does not begin with a valid number, sets value=0 and returns true.
		/// </summary>
		public static bool Attr(this XElement t, out long value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0; return false; }
			x.Value.ToInt(out value);
			return true;
		}

		/// <summary>
		/// Gets attribute value converted to float number.
		/// If the attribute does not exist, sets value=0 and returns false.
		/// If the attribute value is empty or is not a valid number, sets value=0 and returns true.
		/// </summary>
		public static bool Attr(this XElement t, out float value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0F; return false; }
			x.Value.ToNumber(out value);
			return true;
		}

		/// <summary>
		/// Returns true if this element has the specified attribute.
		/// </summary>
		public static bool HasAttr(this XElement t, XName name)
		{
			return t.Attribute(name) != null;
		}

		/// <summary>
		/// Gets the first found descendant element.
		/// Returns null if not found.
		/// </summary>
		public static XElement Desc(this XElement t, XName name)
		{
			return t.Descendants(name).FirstOrDefault();
		}

		/// <summary>
		/// Finds the first descendant element that has the specified attribute or value.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name. If null, uses the Value property of the element.</param>
		/// <param name="attributeValue">Attribute value (or Value). If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Desc(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Descendants(name) : t.Descendants()) {
				if(_CmpAttrOrValue(el, attributeName, attributeValue, ignoreCase)) return el;
			}
			return null;

			//speed: several times faster than XPathSelectElement
		}

		/// <summary>
		/// Finds all descendant elements that have the specified attribute or value.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name. If null, uses the Value property of the element.</param>
		/// <param name="attributeValue">Attribute value (or Value). If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static IEnumerable<XElement> Descs(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Descendants(name) : t.Descendants()) {
				if(_CmpAttrOrValue(el, attributeName, attributeValue, ignoreCase)) yield return el;
			}
		}

		/// <summary>
		/// Gets the first found direct child element that has the specified attribute or value.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name. If null, uses the Value property of the element.</param>
		/// <param name="attributeValue">Attribute value (or Value). If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Elem(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Elements(name) : t.Elements()) {
				if(_CmpAttrOrValue(el, attributeName, attributeValue, ignoreCase)) return el;
			}
			return null;
		}

		/// <summary>
		/// Gets all direct child elements that have the specified attribute or value.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name. If null, uses the Value property of the element.</param>
		/// <param name="attributeValue">Attribute value (or Value). If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static IEnumerable<XElement> Elems(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Elements(name) : t.Elements()) {
				if(_CmpAttrOrValue(el, attributeName, attributeValue, ignoreCase)) yield return el;
			}
		}

		static bool _CmpAttrOrValue(XElement el, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			if(attributeName != null) {
				var a = el.Attribute(attributeName); if(a == null) return false;
				if(attributeValue != null && !a.Value.Eq(attributeValue, ignoreCase)) return false;
			} else {
				if(attributeValue != null && !el.Value.Eq(attributeValue, ignoreCase)) return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the first found direct child element. If not found, adds new empty child element.
		/// Returns the found or added element.
		/// </summary>
		public static XElement ElemOrAdd(this XElement t, XName name)
		{
			var e = t.Element(name);
			if(e == null) t.Add(e = new XElement(name));
			return e;
		}

		/// <summary>
		/// Gets the first found direct child element that has the specified attribute. If not found, adds new child element with the attribute.
		/// Returns the found or added element.
		/// More info: <see cref="Elem"/>
		/// </summary>
		public static XElement ElemOrAdd(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			var e = t.Elem(name, attributeName, attributeValue, ignoreCase);
			if(e == null) t.Add(e = new XElement(name, new XAttribute(attributeName, attributeValue)));
			return e;
		}

		/// <summary>
		/// Gets previous sibling element.
		/// Returns null if no element.
		/// </summary>
		public static XElement PrevElem(this XElement t)
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
		public static XElement NextElem(this XElement t)
		{
			for(XNode n = t.NextNode; n != null; n = n.NextNode) {
				if(n is XElement e) return e;
			}
			return null;
		}

		/// <summary>
		/// Loads XML file in a safer way.
		/// Uses <see cref="XElement.Load(XmlReader, LoadOptions)"/> and <see cref="AFile.WaitIfLocked"/>.
		/// </summary>
		/// <param name="file">File. Must be full path. Can contain environment variables etc, see <see cref="APath.ExpandEnvVar"/>.</param>
		/// <param name="options"></param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="XElement.Load"/>.</exception>
		/// <remarks>
		/// Unlike <see cref="XElement.Load(string, LoadOptions)"/>, does not replace <c>\r\n</c> with <c>\n</c>.
		/// </remarks>
		public static XElement LoadElem(string file, LoadOptions options = default)
		{
			file = APath.LibNormalizeForNET(file);
			return AFile.WaitIfLocked(() => {
				using var r = new XmlTextReader(file); //to preserve \r\n
				if(0 == (options & LoadOptions.PreserveWhitespace)) r.WhitespaceHandling = WhitespaceHandling.Significant; //to save correctly formatted. Default of XElement.Load(string).
				return XElement.Load(r, options);
			});
			//tested: XElement.Load(string) uses XmlReader.Create. It replaces \r\n with \n and does not have an option to preserve \r\n.
		}

		/// <summary>
		/// Saves XML to a file in a safer way.
		/// Uses <see cref="XElement.Save(string, SaveOptions)"/> and <see cref="AFile.Save"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="XElement.Save"/> and <see cref="AFile.Save"/>.</exception>
		public static void SaveElem(this XElement t, string file, bool backup = false, SaveOptions? options = default)
		{
			AFile.Save(file, temp => {
				if(options.HasValue) t.Save(temp, options.GetValueOrDefault()); else t.Save(temp);
			}, backup);
		}

		/// <summary>
		/// Loads XML file in a safer way.
		/// Uses <see cref="XDocument.Load(XmlReader, LoadOptions)"/> and <see cref="AFile.WaitIfLocked"/>.
		/// </summary>
		/// <param name="file">File. Must be full path. Can contain environment variables etc, see <see cref="APath.ExpandEnvVar"/>.</param>
		/// <param name="options"></param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="XDocument.Load"/>.</exception>
		/// <remarks>
		/// Unlike <see cref="XDocument.Load(string, LoadOptions)"/>, does not replace <c>\r\n</c> with <c>\n</c>.
		/// </remarks>
		public static XDocument LoadDoc(string file, LoadOptions options = default)
		{
			file = APath.LibNormalizeForNET(file);
			return AFile.WaitIfLocked(() => {
				using var r = new XmlTextReader(file);
				if(0 == (options & LoadOptions.PreserveWhitespace)) r.WhitespaceHandling = WhitespaceHandling.Significant;
				return XDocument.Load(r, options);
			});
		}

		/// <summary>
		/// Saves XML to a file in a safer way.
		/// Uses <see cref="XDocument.Save(string)"/> and <see cref="AFile.Save"/>
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="XDocument.Save"/> and <see cref="AFile.Save"/>.</exception>
		public static void SaveDoc(this XDocument t, string file, bool backup = false, SaveOptions? options = default)
		{
			AFile.Save(file, temp => {
				if(options.HasValue) t.Save(temp, options.GetValueOrDefault()); else t.Save(temp);
			}, backup);
		}
	}
}
