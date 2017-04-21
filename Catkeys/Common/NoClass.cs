using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Often-used static functions and fields, to be used like Func instead of Class.Func.
	/// Mostly aliases of functions and fields of this library and .NET.
	/// In C# source files add <c>using static Catkeys.NoClass;</c>. Then you can use:
	///		<c>Print</c> instead of <c>Output.Write</c>;
	///		<c>Empty</c> instead of <c>string.IsNullOrEmpty</c>;
	///		<c>Zero</c> instead of <c>IntPtr.Zero</c>;
	///		and more.
	/// </summary>
	[DebuggerStepThrough]
	public static class NoClass
	{
		/// <summary>
		/// Default value for <see cref="IntPtr"/> (0).
		/// The same value as <see cref="IntPtr.Zero"/> or <c>default(IntPtr)</c>.
		/// </summary>
		/// <remarks>
		/// Can be used like <c>IntPtr p = Zero;</c> or <c>if(p == Zero)</c> or FunctionX(Zero). Cannot use 0 or null for it because the IntPtr type is struct.
		/// However this is error: <c>void FunctionX(IntPtr p = Zero){}</c>; use <c>void FunctionX(IntPtr p = default(IntPtr)){}</c> .
		/// </remarks>
		public static readonly IntPtr Zero;

		/// <summary>
		/// Default value for <see cref="Wnd"/> (0 handle).
		/// The same value as <c>default(Wnd)</c>.
		/// </summary>
		/// <remarks>
		/// Can be used like <c>Wnd w = Wnd0;</c> or <c>if(w == Wnd0)</c> (the same as <c>if(w.Is0)</c>) or FunctionX(Wnd0). Cannot use 0 or null for it because the Wnd type is struct.
		/// However this is error: <c>void FunctionX(Wnd w = Wnd0){}</c>; use <c>void FunctionX(Wnd w = default(Wnd)){}</c> .
		/// </remarks>
		public static readonly Wnd Wnd0;

		///// <summary>
		///// Windows newline string "\r\n".
		///// Allows to replace "one\r\ntwo\r\nthree" with "one"+_+"two"+_+"three" or $"one{_}two{_}three" when you don't want @"multiline string".
		///// </summary>
		//public const string _ = "\r\n";
		////Compiler optimizes "one"+_+"two"+_+"three", but not $"one{_}two{_}three"
		////public static readonly string _ = Environment.NewLine; //compiler does not optimize "one"+_+"two"+_+"three"
		////Not so useful, and interferes with intellisense.

		/// <summary>
		/// Alias of <see cref="Output.Write(string)"/>.
		/// </summary>
		public static void Print(string value) { Output.Write(value); }
		/// <summary>
		/// Alias of <see cref="Output.Write(object)"/>.
		/// </summary>
		public static void Print(object value) { Output.Write(value); }
		/// <summary>
		/// Alias of <see cref="Output.Write{T}(IEnumerable{T}, string)"/>.
		/// </summary>
		public static void Print<T>(IEnumerable<T> value, string separator = "\r\n") { Output.Write(value, separator); }
		/// <summary>
		/// Alias of <see cref="Output.Write(System.Collections.IEnumerable, string)"/>.
		/// </summary>
		public static void Print(System.Collections.IEnumerable value, string separator = "\r\n") { Output.Write(value, separator); }
		/// <summary>
		/// Alias of <see cref="Output.Write{K, V}(IDictionary{K, V}, string)"/>.
		/// </summary>
		public static void Print<K, V>(IDictionary<K, V> value, string separator = "\r\n") { Output.Write(value, separator); }
		/// <summary>
		/// Alias of <see cref="Output.WriteList"/>.
		/// </summary>
		public static void PrintList(params object[] values) { Output.WriteList(values); }
		/// <summary>
		/// Alias of <see cref="Output.WriteHex"/>.
		/// </summary>
		public static void PrintHex(object value) { Output.WriteHex(value); }

		/// <summary>
		/// <see cref="Print(object)">Print</see> that works only in Debug configuration, ie if DEBUG is defined.
		/// In Release configuration the function call statement is removed, and arguments not evalueted.
		/// The 3 optional parameters are not used.
		/// </summary>
		[Conditional("DEBUG")]
		public static void DebugPrint(object value, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			Output.Write($"Debug: {cmn} ({Path_.GetFileName(cp)}:{cln}):  {value}");
		}

		/// <summary>
		/// Calls <see cref="DebugPrint">Print</see> if condition is true.
		/// Works only in Debug configuration, ie if DEBUG is defined. Else the function call statement is removed, and arguments not evalueted.
		/// The 3 optional parameters are not used.
		/// </summary>
		[Conditional("DEBUG")]
		public static void DebugPrintIf(bool condition, object value, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			if(condition) DebugPrint(value, cp, cln, cmn);
		}

		/// <summary>
		/// Shows simplest task dialog that works only in Debug configuration, ie if DEBUG is defined.
		/// In Release configuration the function call statement is removed, and arguments not evalueted.
		/// The 3 optional parameters are not used.
		/// Calls <see cref="TaskDialog.Show"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void DebugDialog(object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			TaskDialog.ShowEx("Debug", text?.ToString(), flags: TDFlags.ExpandDown, expandedText: $"{cmn} ({Path_.GetFileName(cp)}:{cln})");
		}

		/// <summary>
		/// Gets caller function name.
		/// Does not get the type name. For example, not useful if called from a constructor (returns ".ctor").
		/// </summary>
		public static string FunctionName([CallerMemberName] string name = null) { return name; }
		/// <summary>
		/// Calls Output.Write(caller function name).
		/// Does not get the type name. For example, not useful if called from a constructor (returns ".ctor").
		/// </summary>
		public static void PrintFunc([CallerMemberName] string name = null) { Output.Write(name); }

		/// <summary>
		/// Returns true if the string is null or "".
		/// The same as string.IsNullOrEmpty.
		/// </summary>
		public static bool Empty(string s) { return s == null || s.Length == 0; }


		//public static void Key(params string[] keys)
		//{
		//	//info:
		//	//Named not Keys because then hides enum Keys from System.Windows.Forms. Also various interfaces have members named Keys.
		//	//Named not SendKeys because too long and hides class SendKeys from System.Windows.Forms.
		//	//Never mind: Key hides enum Key from Microsoft.DirectX.DirectInput and System.Windows.Input. Also various classes have Key property.

		//	Input.Keys(keys);
		//}

		///
		public static void Key(params string[] keys_text_keys_text_andSoOn) { Input.Key(keys_text_keys_text_andSoOn); }
		//note: don't use name SendKeys. It conflicts with System.Windows.Forms.SendKeys.

		/// <summary>
		/// Suspends this thread for the specified amount of time.
		/// Alias of <see cref="Time.Wait"/>.
		/// </summary>
		/// <param name="timeS">
		/// The number of seconds to wait.
		/// The smallest value is 0.001 (1 ms), but the system usually makes it longer. More info: <see cref="Time.Wait"/>.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">timeS is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		public static void Wait(double timeS) { Time.Wait(timeS); }
	}
}
