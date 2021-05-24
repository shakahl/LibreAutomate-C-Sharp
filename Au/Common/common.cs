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

namespace Au.Types
{
	/// <summary>
	/// In DocFX-generated help files removes documentation and auto-generated links in TOC and class pages.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.All)]
	public sealed class NoDoc : Attribute { }

	/// <summary>
	/// Adds undeclared Windows API to the completion list of the inherited class.
	/// </summary>
	public abstract class WinAPI {
		//For it could use an attribute. But this easily solves 2 problems:
		//	1. In 'new' expression does not show completion list (with types from winapi DB) if the winapi class still does not have types inside. Because the completion service then returns null.
		//	2. If class with attributes is after top-level statements, code info often does not work when typing directly above it. Works better if without attributes.

		///
		[NoDoc]
		public class l { } //solves the first problem, because it's always in the list of completions

		//CONSIDER: add util functions.
	}

	/// <summary>
	/// Invokes specified action (calls callback function) at the end of <c>using(...) { ... }</c>.
	/// Usually returned by functions. Example: <see cref="AOpt.Scope.Mouse"/>.
	/// </summary>
	public struct UsingEndAction : IDisposable
	{
		readonly Action _a;

		/// <summary>Sets action to be invoked when disposing this variable.</summary>
		public UsingEndAction(Action a) => _a = a;

		/// <summary>Invokes the action if not null.</summary>
		public void Dispose() => _a?.Invoke();
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
	public sealed class ParamStringAttribute : Attribute
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

namespace System.Runtime.CompilerServices //the attribute must be in this namespace
{
	///
	[Au.Types.NoDoc]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class IgnoresAccessChecksToAttribute : Attribute
	{
		///
		public IgnoresAccessChecksToAttribute(string assemblyName) { AssemblyName = assemblyName; }
		///
		public string AssemblyName { get; }
	}
}
