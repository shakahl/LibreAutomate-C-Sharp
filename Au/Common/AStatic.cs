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
	/// This class contains aliases of some frequently used functions. You can call them without type name.
	/// </summary>
	/// <remarks>
	/// In C# source files add <c>using static Au.AStatic;</c>, and you can call these functions without type name.
	/// Examples:
	///	<c>Print</c> is the same as <c>AOutput.Write</c>;
	///	<c>Empty</c> is the same as <c>string.IsNullOrEmpty</c>;
	/// </remarks>
	public static partial class AStatic
	{
		/// <summary>
		/// Calls <see cref="AOutput.Write"/>. It displays text + <c>"\r\n"</c> in the output pane or console.
		/// </summary>
		/// <param name="value">
		/// Text.
		/// If "" or null, writes empty line. To write "null" if null, use code <c>Print((object)s);</c>.
		/// </param>
		/// <remarks>
		/// Can display links, colors, images, etc. More info: [](xref:print_tags).
		/// </remarks>
		public static void Print(string value)
		{
			AOutput.Write(value);
			//note: need this overload. Cannot use Write(object) for strings because string is IEnumerable<char> and we have overload with IEnumerable<T>.
		}

		/// <summary>
		/// Writes value of any type to the output.
		/// </summary>
		/// <param name="value">Value of any type. Can be null.</param>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="AOutput.Write"/>.
		/// If the type is unsigned integer (uint, ulong, ushort, byte), writes in hexadecimal format with prefix "0x".
		/// If null, prints "null".
		/// 
		/// This overload is used for all types except: strings, arrays, generic collections. They have own overloads; to use this function need to cast to object.
		/// </remarks>
		public static void Print(object value)
		{
			AOutput.Write(LibPrintObjectToString(value));
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
			//if(s.IndexOf('\0') >= 0) s = s.Escape(quote: true); //no
			return s;
		}

		/// <summary>
		/// Writes array, List, Dictionary or other generic collection to the output, as a list of items separated by "\r\n".
		/// </summary>
		/// <param name="value">Array or generic collection of any type. Can be null.</param>
		/// <remarks>
		/// Calls <see cref="PrintListEx"/>, which calls <see cref="AOutput.Write"/>.
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
		/// Calls <see cref="object.ToString"/> and <see cref="AOutput.Write"/>.
		/// </remarks>
		public static void PrintListEx<T>(IEnumerable<T> value, string format = "{s}\r\n", int trimEnd = 2)
		{
			string s = null;
			if(value != null) {
				using(new Util.LibStringBuilder(out var b)) {
					string[] a = null;
					if(format != "{s}\r\n") format.RegexFindAll(@"(?s)(\{[s01]\})|.+?(?=(?1)|$)", 0, out a);
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
					if(trimEnd > 0 && i > 0) b.Length -= trimEnd;
					s = b.ToString();
				}
			}
			AOutput.Write(s);
		}

		/// <summary>
		/// Writes multiple arguments of any type to the output, using separator ", ".
		/// </summary>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="AOutput.Write"/>.
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
				AOutput.Write(b.ToString());

				//rejected: escape strings (eg if contains characters "\r\n,\0"):
				//	it can damage formatting tags etc;
				//	the string may be already escaped, eg AWnd.ToString or AAcc.ToString;
				//	we don't know whether the caller wants it;
				//	let the caller escape it if wants, it's easy.
			}
		}
		static readonly object[] s_oaNull = { null };

		/// <summary>
		/// Writes warning text to the output.
		/// By default appends the stack trace.
		/// </summary>
		/// <param name="text">Warning text.</param>
		/// <param name="showStackFromThisFrame">If &gt;= 0, appends the stack trace, skipping this number of frames. Default 0.</param>
		/// <param name="prefix">Text before <i>text</i>. Default <c>"&lt;&gt;Warning: "</c>.</param>
		/// <remarks>
		/// Calls <see cref="AOutput.Write"/>.
		/// Does not show more that 1 warning/second, unless <b>AOpt.Debug.</b><see cref="OptDebug.Verbose"/> == true.
		/// To disable some warnings, use <b>AOpt.Debug.</b><see cref="OptDebug.DisableWarnings"/>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void PrintWarning(string text, int showStackFromThisFrame = 0, string prefix = "<>Warning: ")
		{
			if(AOpt.Debug.IsWarningDisabled(text)) return;

			if(!AOpt.Debug.Verbose) {
				var t = Api.GetTickCount64();
				if(t - s_warningTime < 1000) return;
				s_warningTime = t;
			}

			string s = text ?? "";
			if(showStackFromThisFrame >= 0) {
				var x = new StackTrace(showStackFromThisFrame + 1, true);
				var st = x.ToString(); var rn = st.Ends('\n') ? "" : "\r\n";
				s = $"{prefix}{s} <fold><\a>\r\n{st}{rn}</\a></fold>";
			} else s = prefix + s;

			AOutput.Write(s);
		}
		static long s_warningTime;

		/// <summary>
		/// Returns true if the string is null or "".
		/// The same as string.IsNullOrEmpty.
		/// </summary>
		public static bool Empty(string s) => (s?.Length ?? 0) == 0;

		/// <summary>
		/// Returns true if the collection (array, List, etc) is null or empty.
		/// </summary>
		public static bool Empty(System.Collections.ICollection a) => (a?.Count ?? 0) == 0;

		//rejected. Cannot use 'break' etc. Better add UI to create 'for' loop to "repeat n times".
		//public static void Repeat(int count, Action action)
		//{
		//	for(int i = 0; i < count; i++) {
		//		action();
		//	}
		//}
		//public static void Repeat(int count, Action<int> action)
		//{
		//	for(int i = 0; i < count; i++) {
		//		action(i);
		//	}
		//}
		//rejected. The code is longer and not easier than 'for'.
		//public static IEnumerable<int> Repeat(int n)
		//{
		//	for(int i = 0; i < n; i++) {
		//		yield return i;
		//	}
		//}
	}
}
