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
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Provides function to get ASCII character type (is digit etc) etc.
	/// </summary>
	/// <remarks>
	/// Unlike <see cref="Char.IsDigit"/> etc, these functions never return true for non-ASCII characters. Also they are faster.
	/// </remarks>
	[DebuggerStepThrough]
	public static class AChar
	{
		/// <summary>
		/// Returns true if character is '0' to '9'.
		/// </summary>
		public static bool IsAsciiDigit(char c) => c <= '9' && c >= '0';

		/// <summary>
		/// Returns true if character is '0' to '9'.
		/// </summary>
		public static bool IsAsciiDigit(byte c) => c <= '9' && c >= '0';

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z'.
		/// </summary>
		public static bool IsAsciiAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z'.
		/// </summary>
		public static bool IsAsciiAlpha(byte c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z' or '0' to '9'.
		/// </summary>
		public static bool IsAsciiAlphaDigit(char c) => IsAsciiAlpha(c) || IsAsciiDigit(c);

		/// <summary>
		/// Returns true if character is 'A' to 'Z' or 'a' to 'z' or '0' to '9'.
		/// </summary>
		public static bool IsAsciiAlphaDigit(byte c) => IsAsciiAlpha(c) || IsAsciiDigit(c);

	}
}
