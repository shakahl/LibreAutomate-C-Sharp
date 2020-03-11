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
//using System.Linq;


namespace Au.Types
{
	/// <summary>
	/// In DocFX-generated help files removes documentation and auto-generated links in TOC and class pages.
	/// </summary>
	public class NoDoc : Attribute
	{
	}

	/// <summary>
	/// Invokes specified action (calls callback function) at the end of <c>using(...) { ... }</c>.
	/// Usually returned by functions. Examples: <see cref="AOpt.Temp.Mouse"/>, <see cref="AMenu.Submenu"/>.
	/// </summary>
	public struct UsingAction : IDisposable
	{
		readonly Action _a;

		/// <summary>Sets action to be invoked when disposing this variable.</summary>
		public UsingAction(Action a) => _a = a;

		/// <summary>Invokes the action.</summary>
		public void Dispose() => _a();
	}

	/// <summary>
	/// Used with <see cref="ParamStringAttribute"/> to specify string parameter format.
	/// </summary>
	public enum PSFormat
	{
		///
		None,

		/// <summary>
		/// Keys. See <see cref="AKeys.Key(KKeysEtc[])"/>.
		/// </summary>
		AKeys,

		/// <summary>
		/// [Wildcard expression](xref:wildcard_expression).
		/// </summary>
		AWildex,

		/// <summary>
		/// PCRE regular expression.
		/// </summary>
		ARegex,

		/// <summary>
		/// PCRE regular expression replacement string.
		/// </summary>
		ARegexReplacement,

		/// <summary>
		/// .NET regular expression.
		/// </summary>
		Regex,
	}

	/// <summary>
	/// A function parameter with this attribute is a string of the specified format, for example regular expression.
	/// Code editors should help to create correct string arguments: provide tools or reference, show errors.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter /*| AttributeTargets.Field | AttributeTargets.Property*/, AllowMultiple = false)]
	public class ParamStringAttribute : Attribute
	{
		///
		public ParamStringAttribute(PSFormat format) => Format = format;

		///
		public PSFormat Format { get; set; }
	}

	///// <summary>
	///// Specifies whether to set, add or remove flags.
	///// </summary>
	//public enum SetAddRemove
	//{
	//	/// <summary>Set flags = the specified value.</summary>
	//	Set = 0,
	//	/// <summary>Add the specified flags, don't change others.</summary>
	//	Add = 1,
	//	/// <summary>Remove the specified flags, don't change others.</summary>
	//	Remove = 2,
	//	/// <summary>Toggle the specified flags, don't change others.</summary>
	//	Xor = 3,
	//}
}
