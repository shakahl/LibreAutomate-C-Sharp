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

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Often-used static functions, to be used like Func instead of Class.Func.
	/// Mostly aliases of functions of this library.
	/// In C# source files add <c>using static Catkeys.NoClass;</c>. Then you can use:
	///		<c>Print</c> instead of <c>Output.Write</c>;
	///		<c>Empty</c> instead of <c>string.IsNullOrEmpty</c>;
	///		and more.
	/// </summary>
	[DebuggerStepThrough]
	public static class NoClass
	{
		//rejected:
		//	Somehow compiler creates quite big code for 'public static readonly Wnd Wnd0'. Gets it from pointer to pointer instead of simply loading 0 into a register.
		//		The same for other structs defined in this library. Although good for Zero (IntPtr). Adding to GAC and ngening does not help.
		//		Could use property, it is optimized to default(Wnd), but not in Debug config. Also it is not like in .NET, eg IntPtr.Zero is a readonly variable.
		//	Instead use default(Wnd), or just default (C# 7.1).
		///// <summary>
		///// Default value for <see cref="Wnd"/> (0 handle).
		///// The same value as <c>default(Wnd)</c>.
		///// </summary>
		///// <remarks>
		///// Can be used like <c>Wnd w = Wnd0;</c> or FunctionX(Wnd0). Cannot use 0 or null for it because the Wnd type is struct.
		///// However this is error: <c>void FunctionX(Wnd w = Wnd0){}</c>; use <c>void FunctionX(Wnd w = default){}</c> .
		///// </remarks>
		//public static readonly Wnd Wnd0;

		/// <summary>
		/// Default value for <see cref="IntPtr"/> (0).
		/// The same value as <see cref="IntPtr.Zero"/> or <c>default(IntPtr)</c>.
		/// </summary>
		/// <remarks>
		/// Can be used like <c>IntPtr p = Zero;</c> or <c>if(p == Zero)</c> or FunctionX(Zero). Cannot use 0 or null for it because the IntPtr type is struct.
		/// However this is error: <c>void FunctionX(IntPtr p = Zero){}</c>; use <c>void FunctionX(IntPtr p = default){}</c> .
		/// </remarks>
		internal static readonly IntPtr Zero; //TODO: remove and use default

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
		/// Alias of <see cref="Output.WriteList"/>.
		/// </summary>
		public static void PrintList(params object[] values) { Output.WriteList(values); }

		/// <summary>
		/// Alias of <see cref="Output.WriteHex"/>.
		/// </summary>
		public static void PrintHex(object value) { Output.WriteHex(value); }

		/// <summary>
		/// Returns true if the string is null or "".
		/// The same as string.IsNullOrEmpty.
		/// </summary>
		public static bool Empty(string s) { return s == null || s.Length == 0; }

		/// <summary>
		/// Gets ScriptOptions object of this thread.
		/// Alias of <see cref="ScriptOptions.Options"/>.
		/// </summary>
		public static ScriptOptions Options => ScriptOptions.Options;

		///
		public static void Key(params string[] keys) { Input.Key(keys); }
		//	//info:
		//	//Named not Keys because then hides enum Keys from System.Windows.Forms. Also various interfaces have members named Keys.
		//	//Named not SendKeys because too long and hides class SendKeys from System.Windows.Forms.

		///
		public static void Text(params string[] text) { Input.Text(text); }

		///
		public static void Paste(string text) { Input.Paste(text); }

		/// <summary>
		/// Suspends this thread for the specified amount of time.
		/// Alias of <see cref="Time.Wait"/>.
		/// </summary>
		/// <param name="seconds">
		/// The number of seconds to wait.
		/// The smallest value is 0.001 (1 ms), but the system usually makes it longer. More info: <see cref="Time.Wait"/>.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">seconds is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		public static void Wait(double seconds) { Time.Wait(seconds); }
	}
}
