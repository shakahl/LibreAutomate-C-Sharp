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
	/// This class contains aliases of some frequently used functions.
	/// In C# source files add <c>using static Au.NoClass;</c>, and you can call these functions without specifying a type name.
	/// Examples:
	///	<c>Print</c> is the same as <c>Output.Write</c>;
	///	<c>Empty</c> is the same as <c>string.IsNullOrEmpty</c>;
	/// </summary>
	//[DebuggerStepThrough]
	public static partial class NoClass
	{
		/// <summary>
		/// Calls <see cref="Output.Write" qualifyHint="true"/>. It writes string + "\r\n" to the output.
		/// </summary>
		/// <remarks>
		/// If "" or null, writes empty line. To write "null" if null, use code <c>Print((object)s);</c>.
		/// </remarks>
		public static void Print(string value)
		{
			Output.Write(value);
			//note: need this overload. Cannot use Write(object) for strings because string is IEnumerable<char> and we have overload with IEnumerable<T>.
		}
		//CONSIDER: PrintColor.

		/// <summary>
		/// Writes value of any type to the output.
		/// </summary>
		/// <param name="value">Value of any type. Can be null.</param>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="Output.Write" qualifyHint="true"/>.
		/// If the type is unsigned integer (uint, ulong, ushort, byte), writes in hexadecimal format with prefix "0x".
		/// If null, prints "null".
		/// 
		/// This overload is used for value types (int, Point) etc and other types except strings, arrays and generic collections (they have own overloads; to use this function need to cast to object).
		/// </remarks>
		public static void Print(object value)
		{
			Output.Write(LibPrintObjectToString(value));
		}

		/// <summary>
		/// Converts object to string like <see cref="Print(object)"/> does.
		/// </summary>
		internal static string LibPrintObjectToString(object value)
		{
			string s;
			switch(value) {
			case null: s = "null"; break;
			case string t: s = t; break;
			case uint u: s = "0x" + u.ToString("X"); break;
			case ulong u: s = "0x" + u.ToString("X"); break;
			case ushort u: s = "0x" + u.ToString("X"); break;
			case byte u: s = "0x" + u.ToString("X"); break;
			case char[] t: s = new string(t); break;
			case System.Collections.IEnumerable e: s = string.Join("\r\n", System.Linq.Enumerable.Cast<object>(e)); break;
			default: s = value.ToString(); break;
			}
			//if(s.IndexOf('\0') >= 0) s = s.Escape_(quote: true); //no
			return s;
		}

		/// <summary>
		/// Writes array, List, Dictionary or other generic collection to the output, as a list of items separated by "\r\n".
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
		/// Writes array, List, Dictionary or other generic collection to the output, as a list of items.
		/// </summary>
		/// <param name="value">Array or generic collection of any type. Can be null.</param>
		/// <param name="format">
		/// Item format string.
		/// 
		/// These special substrings are replaced with:
		/// {s} - <c>item.ToString</c>, or "null" if null.
		/// {0} - item index, starting from 0.
		/// {1} - item index, starting from 1.
		/// 
		/// Default "{s}\r\n". It works like <see cref="Print{T}(IEnumerable{T})"/>.
		/// </param>
		/// <param name="trimEnd">
		/// How many characters to remove from the end of the result string.
		/// Default 2 (removes the default "\r\n" separator from the last item).
		/// </param>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="Output.Write" qualifyHint="true"/>.
		/// </remarks>
		public static void PrintListEx<T>(IEnumerable<T> value, string format = "{s}\r\n", int trimEnd = 2)
		{
			string s = null;
			if(value != null) {
				using(new Util.LibStringBuilder(out var b)) {
					string[] a = null;
					if(format != "{s}\r\n") format.RegexFindAll_(@"(?s)(\{[s01]\})|.+?(?=(?1)|$)", 0, out a);
					int i = 0;
					foreach(T v in value) {
						i++;
						string t = v == null ? "null" : v.ToString();
						if(a == null) {
							b.AppendLine(t);
						} else {
							foreach(string m in a) {
								switch(m) {
								case "{s}": b.Append(t); break;
								case "{0}": b.Append(i - 1); break;
								case "{1}": b.Append(i); break;
								default: b.Append(m); break;
								}
							}
						}
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
		/// If a value is unsigned integer (uint, ulong, ushort, byte), writes in hexadecimal format with prefix "0x".
		/// </remarks>
		public static void Print(object value1, object value2, params object[] more)
		{
			if(more == null) more = s_oaNull; //workaround for: if third argument is null, we receive null and not array containing null
			using(new Util.LibStringBuilder(out var b)) {
				for(int i = 0, n = 2 + more.Length; i < n; i++) {
					if(i > 0) b.Append(", ");
					object v = i == 0 ? value1 : (i == 1 ? value2 : more[i - 2]);
					switch(v) {
					case string s: b.Append(s); break;
					case null: b.Append("null"); break;
					case uint u: b.Append("0x").Append(u.ToString("X")); break;
					case ulong u: b.Append("0x").Append(u.ToString("X")); break;
					case ushort u: b.Append("0x").Append(u.ToString("X")); break;
					case byte u: b.Append("0x").Append(u.ToString("X")); break;
					default: b.Append(v); break;
					}
				}
				Output.Write(b.ToString());

				//rejected: escape strings (eg if contains characters "\r\n,\0"):
				//	it can damage formatting tags etc;
				//	the string may be already escaped, eg Wnd.ToString or Acc.ToString;
				//	we don't know whether the caller wants it;
				//	let the caller escape it if wants, it's easy.
			}
		}
		static readonly object[] s_oaNull = new object[] { null };

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
			string s = text ?? "";

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

		//rejected. Let use like 3.s() instead.
		///// <summary>
		///// Suspends this thread for the specified amount of time.
		///// Calls <see cref="Time.SleepS" qualifyHint="true"/>.
		///// </summary>
		///// <param name="seconds">
		///// The number of seconds to wait.
		///// The smallest value is 0.001 (1 ms), but the system usually makes it longer. More info: <see cref="Time.SleepS" qualifyHint="true"/>.
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
	}
}
