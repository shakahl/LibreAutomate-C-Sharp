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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Often-used static functions, to be used like Func instead of Class.Func.
	/// Mostly aliases of functions of this library.
	/// In C# source files add <c>using static Au.NoClass;</c>. Then you can use:
	///		<c>Print</c> instead of <c>Output.Write</c>;
	///		<c>Empty</c> instead of <c>string.IsNullOrEmpty</c>;
	///		and more.
	/// </summary>
	[DebuggerStepThrough]
	public static class NoClass
	{
		/// <summary>
		/// Calls <see cref="Output.Write" qualifyHint="true"/>. It writes string + "\r\n" to the output.
		/// </summary>
		public static void Print(string value)
		{
			Output.Write(value);
			//note: need this overload. Cannot use Write(object) for strings because string is IEnumerable<char> and we have overload with IEnumerable<T>.
		}

		/// <summary>
		/// Writes value of any type to the output.
		/// </summary>
		/// <param name="value">Value of any type. Can be null.</param>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="Output.Write" qualifyHint="true"/>.
		/// If the type implements IEnumerable (non-generic), writes list like <see cref="Print{T}(IEnumerable{T})"/>.
		/// This overload is used for value types (int, Point) etc and other types except strings, arrays and generic collections (they have own overloads).
		/// </remarks>
		public static void Print(object value)
		{
			if(value is System.Collections.IEnumerable e) {
				Print(System.Linq.Enumerable.Cast<object>(e));
			} else {
				Output.Write(value?.ToString());
			}
		}

		/// <summary>
		/// Writes array, List, Dictionary or other generic collection to the output, as a list of item values separated by "\r\n".
		/// </summary>
		/// <param name="value">Array or generic collection of any type. Can be null.</param>
		/// <remarks>
		/// Calls <see cref="PrintListEx"/>, which calls <see cref="Output.Write" qualifyHint="true"/>.
		/// </remarks>
		public static void Print<T>(IEnumerable<T> value)
		{
			PrintListEx(value);
		}

		/// <summary>
		/// Writes array, List, Dictionary or other generic collection to the output, as a list of item values.
		/// </summary>
		/// <param name="value">Array or generic collection of any type. Can be null.</param>
		/// <param name="format">
		/// Item format string.
		/// 
		/// These special substrings are replaced with:
		/// {0} - item index, starting from 0.
		/// {1} - item index, starting from 1.
		/// {2} - item string value, or empty string if null.
		/// {3} - item string value, or "null" if null. Strings are escaped (<see cref="String_.Escape_"/>) and enclosed in "".
		/// 
		/// If this parameter is null or omitted, uses "{3}\r\n". It works like <see cref="Print{T}(IEnumerable{T})"/>.
		/// </param>
		/// <param name="trimEnd">
		/// How many characters to remove from the end of the result string.
		/// Default 2 (removes the default "\r\n" separator from the last item).
		/// </param>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="Output.Write" qualifyHint="true"/>.
		/// </remarks>
		public static void PrintListEx<T>(IEnumerable<T> value, string format = "{3}\r\n", int trimEnd = 2)
		{
			string s = null;
			if(value != null) {
				using(new Util.LibStringBuilder(out var b)) {
					if(format == null) format = "{3}\r\n";
					bool simpleFormat = false, has0 = false, has1 = false, has3 = false;
					if(format == "{3}\r\n") has3 = true;
					else if(format == "{2}\r\n") simpleFormat = true;
					else {
						has0 = format.Contains("{0}");
						has1 = format.Contains("{1}");
						has3 = format.Contains("{3}");
					}
					var a = new string[5];
					int i = 0;
					foreach(T v in value) {
						var t = v?.ToString();
						if(simpleFormat) b.AppendLine(t);
						else {
							if(has0) a[0] = i.ToString();
							if(has1) a[1] = (i + 1).ToString();
							a[2] = t;
							if(has3) a[3] = (v == null) ? "null" : ((v is string) ? t?.Escape_(quote: true) : t);
							b.AppendFormat(format, a);
						}
						i++;
					}
					if(trimEnd > 0 && i > 0) b.Remove(b.Length - trimEnd, trimEnd);
					s = b.ToString();
				}
			}
			Output.Write(s);
		}

		/// <summary>
		/// Writes multiple arguments of any type to the output, using separator ", ".
		/// </summary>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="Output.Write" qualifyHint="true"/>.
		/// If a value is null, writes "null".
		/// If a value is string, escapes it (<see cref="String_.Escape_"/>), limits to 250 characters (<see cref="String_.Limit_"/>) and encloses in "".
		/// </remarks>
		public static void Print(object value1, object value2, params object[] more)
		{
			using(new Util.LibStringBuilder(out var b)) {
				for(int i = 0, n = 2 + more?.Length ?? 0; i < n; i++) {
					if(i > 0) b.Append(", ");
					object v = i == 0 ? value1 : (i == 1 ? value2 : more[i - 2]);
					switch(v) {
					case string s: b.Append(s.Escape_(limit: 250, quote: true)); break;
					case null: b.Append("null"); break;
					default: b.Append(v); break;
					}
				}
				Output.Write(b.ToString());
			}
		}

		/// <summary>
		/// Writes integer number to the output (<see cref="Output.Write" qualifyHint="true"/>), in hexadecimal format like "0x5A".
		/// </summary>
		/// <param name="value">Value of an integer type (int, uint, long, etc).</param>
		public static void PrintHex(object value) { Output.Write($"0x{value:X}"); }
		//never mind: this is slower, but with Write("0x" + value.ToString("X")); we'd need switch or overloads for all integer types.
		//FUTURE: add overload to write hex-encoded byte[], void*, etc. Like QM2 outb.

		/// <summary>
		/// Writes warning text to the output.
		/// By default appends the stack trace.
		/// </summary>
		/// <param name="text">Warning text.</param>
		/// <param name="showStackFromThisFrame">If &gt;= 0, appends the stack trace, skipping this number of frames. Default 0.</param>
		/// <param name="prefix">Text before <paramref name="text"/>. Default "Warning: ".</param>
		/// <remarks>
		/// Calls <see cref="Output.Write" qualifyHint="true"/>.
		/// If <see cref="AuScriptOptions.Debug">Options.Debug</see> is false, does not show more that 1 warning/second.
		/// To disable some warnings, use <see cref="AuScriptOptions.DisableWarnings">Options.DisableWarnings</see>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void PrintWarning(string text, int showStackFromThisFrame = 0, string prefix = "Warning: ")
		{
			string s = text??"";

			var a = Options.LibDisabledWarnings;
			if(a != null) foreach(var k in a) if(s.Like_(k, true)) return;

			if(!Options.Debug) {
				var t = Time.Milliseconds;
				if(t - s_warningTime < 1000) return;
				s_warningTime = t;
			}

			if(showStackFromThisFrame >= 0) {
				var x = new StackTrace(showStackFromThisFrame + 1, true);
				s = prefix + s + "\r\n" + x.ToString();
			} else s = prefix + s;

			Output.Write(s);
		}
		static long s_warningTime;

		//rejected. Let use like 3.s() instead of WaitS(3).
		///// <summary>
		///// Suspends this thread for the specified amount of time.
		///// Calls <see cref="Time.WaitS" qualifyHint="true"/>.
		///// </summary>
		///// <param name="seconds">
		///// The number of seconds to wait.
		///// The smallest value is 0.001 (1 ms), but the system usually makes it longer. More info: <see cref="Time.WaitS" qualifyHint="true"/>.
		///// </param>
		///// <exception cref="ArgumentOutOfRangeException">seconds is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		//public static void WaitS(double seconds) => Time.WaitS(seconds);

		/// <summary>
		/// Gets AuScriptOptions object of this thread.
		/// Alias of <see cref="AuScriptOptions.Options" qualifyHint="true"/>.
		/// </summary>
		public static AuScriptOptions Options => AuScriptOptions.Options;

		/// <summary>
		/// Returns true if the string is null or "".
		/// The same as string.IsNullOrEmpty.
		/// </summary>
		public static bool Empty(string s) => (s?.Length ?? 0) == 0;

		///
		public static void Key(params object[] keys) => Input.Common.Key(keys);

		///
		public static void Text(string text, string keys = null) => Input.Common.Text(text, keys);

		///
		public static void Paste(string text, string keys = null) => Input.Common.Paste(text, keys);
	}
}

namespace Au.Types
{
	///
	public enum KScan { } //TODO
}
