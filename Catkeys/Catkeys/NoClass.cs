using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
//using System.Windows.Forms;

using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	[DebuggerStepThrough]
	public static class NoClass
	{
		/// <summary>
		/// Default (0) value for IntPtr, LPARAM and some other types. The same as IntPtr.Zero or default(IntPtr).
		/// These types are struct types, therefore you cannot assign 0 or null.
		/// However this is error: <c>void Func(IntPtr x=Zero){}</c>; use <c>void Func(IntPtr x=default(IntPtr)){}</c>.
		/// </summary>
		public static readonly IntPtr Zero;

		/// <summary>
		/// Default (0) value for Wnd.
		/// These types are struct types, therefore you cannot assign 0 or null.
		/// However this is error: <c>void Func(Wnd x=Wnd0){}</c>; use <c>void Func(Wnd x=default(Wnd)){}</c>.
		/// </summary>
		public static readonly Wnd Wnd0;

		/// <summary>
		/// Windows newline string "\r\n".
		/// Allows to replace "one\r\ntwo\r\nthree" with "one"+_+"two"+_+"three" or $"one{_}two{_}three" when you don't want @"multiline string".
		/// </summary>
		public const string _ = "\r\n";
		//Compiler optimizes "one"+_+"two"+_+"three", but not $"one{_}two{_}three"
		//public static readonly string _ = Environment.NewLine; //compiler does not optimize "one"+_+"two"+_+"three"

		//public const StringComparison CaseSens = StringComparison.Ordinal;
		//public const StringComparison CaseInsens = StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Alias of Output.Write.
		/// </summary>
		//public static void Out(string value) { Output.Write(value); }
		public static void Out(object value) { Output.Write(value); }
		//public static void Out<K, V>(IDictionary<K, V> value) { Output.Write(value); }
		//public static void Out<T>(IEnumerable<T> value) { Output.Write(value); }
		public static void OutList(params object[] values) { Output.WriteList(values); }
		//public static void OutListSep(string separator, params object[] values) { Output.WriteListSep(separator, values); }
		//public static void OutHex<T>(T value) { Output.WriteHex(value); }

		/// <summary>
		/// Alias of Output.Write.
		/// </summary>
		//public static void Print(string value) { Output.Write(value); }
		public static void Print(object value) { Output.Write(value); }
		//public static void Print<K, V>(IDictionary<K, V> value) { Output.Write(value); }
		//public static void Print<T>(IEnumerable<T> value) { Output.Write(value); }
		public static void PrintList(params object[] values) { Output.WriteList(values); }
		//public static void PrintListSep(string separator, params object[] values) { Output.WriteListSep(separator, values); }
		//public static void PrintHex<T>(T value) { Output.WriteHex(value); }

		///// <summary>
		///// Alias of Output.Write.
		///// </summary>
		//public static void Write(string value) { Output.Write(value); }
		//public static void Write(object value) { Output.Write(value); }
		//public static void Write<K, V>(IDictionary<K, V> value) { Output.Write(value); }
		//public static void Write<T>(IEnumerable<T> value) { Output.Write(value); }
		//public static void WriteList(params object[] values) { Output.WriteList(values); }
		////public static void WriteListSep(string separator, params object[] values) { Output.WriteListSep(separator, values); }
		////public static void WriteHex<T>(T value) { Output.WriteHex(value); }

		/// <summary>
		/// Gets function name.
		/// Does not get the type name. For example, not useful if called from a constructor (returns ".ctor").
		/// </summary>
		public static string FunctionName([CallerMemberName] string name = null) { return name; }

		/// <summary>
		/// Returns true if the string is null or "".
		/// The same as string.IsNullOrEmpty.
		/// </summary>
		public static bool Empty(string s) { return string.IsNullOrEmpty(s); }

		public static void Wait(double timeS) { Time.Wait(timeS); }
		public static void WaitMS(int timeMS) { Time.WaitMS(timeMS); }
    }
}
