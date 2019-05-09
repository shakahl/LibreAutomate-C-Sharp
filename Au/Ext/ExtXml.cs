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

using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Extends <see cref="XElement"/> and <see cref="XDocument"/>.
	/// </summary>
	public static class ExtXml
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
		/// Gets attribute value converted to int (<see cref="ExtString.ToInt"/>).
		/// If the attribute does not exist, returns defaultValue.
		/// If the attribute value is empty or does not begin with a valid number, returns 0.
		/// </summary>
		public static int Attr(this XElement t, XName name, int defaultValue)
		{
			var x = t.Attribute(name);
			return x != null ? x.Value.ToInt() : defaultValue;
		}

		/// <summary>
		/// Gets attribute value converted to int (<see cref="ExtString.ToInt"/>).
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
		/// Gets attribute value converted to long (<see cref="ExtString.ToInt64"/>).
		/// If the attribute does not exist, sets value=0 and returns false.
		/// If the attribute value is empty or does not begin with a valid number, sets value=0 and returns true.
		/// </summary>
		public static bool Attr(this XElement t, out long value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0; return false; }
			value = x.Value.ToInt64();
			return true;
		}

		/// <summary>
		/// Gets attribute value converted to float (<see cref="ExtString.ToFloat"/>).
		/// If the attribute does not exist, sets value=0F and returns false.
		/// If the attribute value is empty or is not a valid number, sets value=0F and returns true.
		/// </summary>
		public static bool Attr(this XElement t, out float value, XName name)
		{
			var x = t.Attribute(name);
			if(x == null) { value = 0F; return false; }
			value = x.Value.ToFloat();
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
		/// Finds the first descendant element that has the specified attribute.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value. If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static XElement Desc(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Descendants(name) : t.Descendants()) {
				var a = el.Attribute(attributeName); if(a == null) continue;
				if(attributeValue != null && !a.Value.Eq(attributeValue, ignoreCase)) continue;
				return el;
			}
			return null;

			//speed: several times faster than XPathSelectElement
		}

		/// <summary>
		/// Finds all descendant elements that have the specified attribute.
		/// Returns null if not found.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name">Element name. If null, can be any name.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value. If null, can be any value.</param>
		/// <param name="ignoreCase">Case-insensitive attributeValue.</param>
		public static IEnumerable<XElement> Descs(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Descendants(name) : t.Descendants()) {
				var a = el.Attribute(attributeName); if(a == null) continue;
				if(attributeValue != null && !a.Value.Eq(attributeValue, ignoreCase)) continue;
				yield return el;
			}
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
		public static XElement Elem(this XElement t, XName name, XName attributeName, string attributeValue = null, bool ignoreCase = false)
		{
			foreach(var el in (name != null) ? t.Elements(name) : t.Elements()) {
				var a = el.Attribute(attributeName); if(a == null) continue;
				if(attributeValue != null && !a.Value.Eq(attributeValue, ignoreCase)) continue;
				return el;
			}
			return null;
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
		/// Uses <see cref="XElement.Load"/> and <see cref="AFile.WaitIfLocked"/>.
		/// </summary>
		/// <param name="file">File. Must be full path. Can contain environment variables etc, see <see cref="APath.ExpandEnvVar"/>.</param>
		/// <param name="options"></param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="XElement.Load"/>.</exception>
		public static XElement LoadElem(string file, LoadOptions options = default)
		{
			file = APath.LibNormalizeForNET(file);
			return AFile.WaitIfLocked(() => XElement.Load(file, options));
		}

		/// <summary>
		/// Saves XML to a file in a safer way.
		/// Uses <see cref="XElement.Save(string, SaveOptions)"/> and <see cref="AFile.Save"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="XElement.Save"/> and <see cref="AFile.Save"/>.</exception>
		public static void SaveElem(this XElement t, string file, bool backup = false, SaveOptions? options = default)
		{
			AFile.Save(file, temp =>
			{
				if(options.HasValue) t.Save(temp, options.GetValueOrDefault()); else t.Save(temp);
			}, backup);
		}

		/// <summary>
		/// Loads XML file in a safer way.
		/// Uses <see cref="XDocument.Load"/> and <see cref="AFile.WaitIfLocked"/>.
		/// </summary>
		/// <param name="file">File. Must be full path. Can contain environment variables etc, see <see cref="APath.ExpandEnvVar"/>.</param>
		/// <param name="options"></param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="XDocument.Load"/>.</exception>
		public static XDocument LoadDoc(string file, LoadOptions options = default)
		{
			file = APath.LibNormalizeForNET(file);
			return AFile.WaitIfLocked(() => XDocument.Load(file, options));
		}

		/// <summary>
		/// Saves XML to a file in a safer way.
		/// Uses <see cref="XDocument.Save(string)"/> and <see cref="AFile.Save"/>
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="XDocument.Save"/> and <see cref="AFile.Save"/>.</exception>
		public static void SaveDoc(this XDocument t, string file, bool backup = false, SaveOptions? options = default)
		{
			AFile.Save(file, temp =>
			{
				if(options.HasValue) t.Save(temp, options.GetValueOrDefault()); else t.Save(temp);
			}, backup);
		}
	}
}
