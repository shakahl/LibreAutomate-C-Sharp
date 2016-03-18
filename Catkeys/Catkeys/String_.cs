using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
//using System.Windows.Forms;

//for Like_
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	/// <summary>
	/// Adds extension methods to System.String.
	/// Also adds StringComparison.Ordinal[IgnoreCase] versions of .NET String methods that use StringComparison.CurrentCulture by default. See https://msdn.microsoft.com/en-us/library/ms973919.aspx
	/// Extension method names have suffix _. Some have case-insensitive versions with suffix I_.
	/// </summary>
	[DebuggerStepThrough]
	public static class String_
	{
		/// <summary>
		/// Returns EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static bool EndsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns EndsWith(value, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static bool EndsWithI_(this string t, string value)
		{
			return t.EndsWith(value, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static bool StartsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.StartsWith(value, StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns StartsWith(value, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static bool StartsWithI_(this string t, string value)
		{
			return t.StartsWith(value, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns true if this string matches wildcard pattern.
		/// </summary>
		/// <param name="pattern">String with wildcard characters:
		///  * (zero or more characters),
		///  ? (single character),
		///  # (digit 0-9),
		///  [charlist] (a character in charlist),
		///  [!charlist] (a character not in charlist).
		/// </param>
		/// <remarks>
		/// Works like the VB operator 'Like': https://msdn.microsoft.com/en-us/library/swf8kaxw%28v=vs.100%29.aspx
		/// </remarks>
		public static bool Like_(this string t, string pattern, bool ignoreCase = false)
		{
			return Operators.LikeString(t, pattern, ignoreCase ? CompareMethod.Text : CompareMethod.Binary);
		}
		/// <summary>
		/// Returns true if this string matches wildcard pattern. Case-insensitive.
		/// </summary>
		/// <param name="pattern">String with wildcard characters:
		///  * (zero or more characters),
		///  ? (single character),
		///  # (digit 0-9),
		///  [charlist] (a character in charlist),
		///  [!charlist] (a character not in charlist).
		/// </param>
		/// <remarks>
		/// Works like the VB operator 'Like': https://msdn.microsoft.com/en-us/library/swf8kaxw%28v=vs.100%29.aspx
		/// </remarks>
		public static bool LikeI_(this string t, string pattern)
		{
			return Operators.LikeString(t, pattern, CompareMethod.Text);
		}
		//Speed for 1000 times (@"C:\Users\G\Documents\dictionary.xls", "*.xls"):
		//this 85, regex 830, compiled regex 440, regex with WildcardToRegex 1200, QM2 FindRX 400, QM2 MatchW ~30 (matchw 70 + rep 35).
		//Not tested System.Management.Automation.WildcardPattern, because assembly System.Management.Automation is not installed. It seems need to install it through NuGet.

		/// <summary>
		/// Returns IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int IndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns IndexOf(value, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static int IndexOfI_(this string t, string value)
		{
			return t.IndexOf(value, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Returns IndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int IndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns IndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static int IndexOfI_(this string t, string value, int startIndex)
		{
			return t.IndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Returns IndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int IndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns IndexOf(value, startIndex, count, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static int IndexOfI_(this string t, string value, int startIndex, int count)
		{
			return t.IndexOf(value, startIndex, count, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns IndexOfAny(anyOf.ToCharArray()).
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf)
		{
			return t.IndexOfAny(anyOf.ToCharArray());
		}
		/// <summary>
		/// Returns IndexOfAny(anyOf.ToCharArray(), startIndex).
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf, int startIndex)
		{
			return t.IndexOfAny(anyOf.ToCharArray(), startIndex);
		}
		/// <summary>
		/// Returns IndexOfAny(anyOf.ToCharArray(), startIndex, count).
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf, int startIndex, int count)
		{
			return t.IndexOfAny(anyOf.ToCharArray(), startIndex, count);
		}

		/// <summary>
		/// Returns LastIndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int LastIndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns LastIndexOf(value, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static int LastIndexOfI_(this string t, string value)
		{
			return t.LastIndexOf(value, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Returns LastIndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int LastIndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns LastIndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static int LastIndexOfI_(this string t, string value, int startIndex)
		{
			return t.LastIndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Returns LastIndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int LastIndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns LastIndexOf(value, startIndex, count, StringComparison.OrdinalIgnoreCase).
		/// </summary>
		public static int LastIndexOfI_(this string t, string value, int startIndex, int count)
		{
			return t.LastIndexOf(value, startIndex, count, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns LastIndexOfAny(anyOf.ToCharArray()).
		/// </summary>
		public static int LastIndexOfAny_(this string t, string anyOf)
		{
			return t.LastIndexOfAny(anyOf.ToCharArray());
		}
		/// <summary>
		/// Returns LastIndexOfAny(anyOf.ToCharArray(), startIndex).
		/// </summary>
		public static int LastIndexOfAny_(this string t, string anyOf, int startIndex)
		{
			return t.LastIndexOfAny(anyOf.ToCharArray(), startIndex);
		}
		/// <summary>
		/// Returns LastIndexOfAny(anyOf.ToCharArray(), startIndex, count).
		/// </summary>
		public static int LastIndexOfAny_(this string t, string anyOf, int startIndex, int count)
		{
			return t.LastIndexOfAny(anyOf.ToCharArray(), startIndex, count);
		}

		/// <summary>
		/// Returns Split(separators.ToCharArray()).
		/// </summary>
		public static string[] Split_(this string t, string separators)
		{
			return t.Split(separators.ToCharArray());
		}
		/// <summary>
		/// Returns Split(separators.ToCharArray(), count).
		/// </summary>
		public static string[] Split_(this string t, string separators, int count)
		{
			return t.Split(separators.ToCharArray(), count);
		}
		/// <summary>
		/// Returns Split(separators.ToCharArray(), options).
		/// </summary>
		public static string[] Split_(this string t, string separators, StringSplitOptions options)
		{
			return t.Split(separators.ToCharArray(), options);
		}
		/// <summary>
		/// Returns Split(separators.ToCharArray(), count, options).
		/// </summary>
		public static string[] Split_(this string t, string separators, int count, StringSplitOptions options)
		{
			return t.Split(separators.ToCharArray(), count, options);
		}

		/// <summary>
		/// Returns array of lines. Line separators can be \r\n or \n or \r.
		/// </summary>
		public static string[] SplitLines_(this string t, bool removeEmptyLines = false)
		{
			return t.Split(_newlines, removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
		}
		static readonly string[] _newlines = { "\r\n", "\n", "\r" }; //error if const; not public because elements can be changed

		/// <summary>
		/// Returns ToLowerInvariant().
		/// </summary>
		public static string ToLower_(this string t)
		{
			return t.ToLowerInvariant();
		}

		/// <summary>
		/// Returns ToUpperInvariant().
		/// </summary>
		public static string ToUpper_(this string t)
		{
			return t.ToUpperInvariant();
		}

		/// <summary>
		/// Returns Trim(trimChars.ToCharArray()).
		/// </summary>
		public static string Trim_(this string t, string trimChars)
		{
			return t.Trim(trimChars.ToCharArray());
		}
		//This is not in System.String.

		/// <summary>
		/// Returns TrimEnd(trimChars.ToCharArray()).
		/// </summary>
		public static string TrimEnd_(this string t, string trimChars)
		{
			return t.TrimEnd(trimChars.ToCharArray());
		}

		/// <summary>
		/// Returns TrimStart(trimChars.ToCharArray()).
		/// </summary>
		public static string TrimStart_(this string t, string trimChars)
		{
			return t.TrimStart(trimChars.ToCharArray());
		}
	}



}
