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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Functions useful to debug code.
	/// </summary>
	/// <remarks>
	/// The PrintX functions write to the same output as <see cref="Output.Write(object)"/>, not to the trace listeners like <see cref="Debug.Print(string)"/> etc do. Also they add caller's name, file and line number.
	/// Functions Print, PrintIf, PrintFunc and Dialog work only if DEBUG is defined, which normally is when the caller project is in Debug configuration. Else they are not called, and arguments not evaluated at run time. This is because they have [<see cref="ConditionalAttribute"/>("DEBUG")].
	/// Note: when used in a library, the above functions depend on DEBUG of the library project and not on DEBUG of the consumer project of the library. For example, the library may be in Release configuration even if its consumer project is in Debug configuration. If your library wants to show some info only if its consumer project is in Debug config, instead you can use code like <c>if(Options.Debug) Output.Warning("text");</c> or Debug_ class functions with Opt suffix, eg WarningOpt.
	/// </remarks>
	//[DebuggerStepThrough]
	public static class Debug_
	{
		static void _Print(object text, string cp, int cln, string cmn)
		{
			Output.Write($"Debug: {cmn} ({Path_.GetFileName(cp)}:{cln}):  {text}");
		}

		/// <summary>
		/// Calls <see cref="Output.Write(object)"/> to show some debug info.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional parameters are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Print(object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			_Print(text, cp, cln, cmn);
		}

		/// <summary>
		/// Calls <see cref="Output.Write(object)"/> if condition is true.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional parameters are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintIf(bool condition, object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			if(condition) _Print(text, cp, cln, cmn);
		}

		/// <summary>
		/// Calls <see cref="Output.Write(object)"/> with current function name.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The optional parameter is not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintFunc([CallerMemberName] string name = null) { Output.Write(name); }

		//rejected: rarely used
		///// <summary>
		///// Gets current function name.
		///// The optional parameter is not used explicitly.
		///// </summary>
		//public static string FuncName([CallerMemberName] string name = null) { return name; }

		/// <summary>
		/// Calls <see cref="TaskDialog.Show"/> to show some debug info.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional parameters are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Dialog(object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			TaskDialog.ShowEx("Debug", text?.ToString(), flags: TDFlags.ExpandDown, expandedText: $"{cmn} ({Path_.GetFileName(cp)}:{cln})");
		}

		//rejected. Better use Output.Warning or Debug_.WarningOpt. They show the stack trace and have an option to disable some warnings. Also they support prefix "Note:", "Debug:" etc. Or need the same features for this func.
		///// <summary>
		///// If <see cref="ScriptOptions.Debug">Options.Debug</see> is true, calls <see cref="Output.Write(string)"/>.
		///// Read more in class help.
		///// </summary>
		//public static void PrintOpt(string text)
		//{
		//	if(Options.Debug) Output.Write("Debug: " + text);
		//}

		/// <summary>
		/// If <see cref="ScriptOptions.Debug">Options.Debug</see> is true, calls <see cref="Output.Warning"/>.
		/// Read more in class help.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void WarningOpt(string text)
		{
			if(Options.Debug) Output.Warning(text, 1);
		}

		/// <summary>
		/// If <see cref="ScriptOptions.Debug">Options.Debug</see> is true, calls <see cref="TaskDialog.Show"/> with text and stack trace.
		/// Read more in class help.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void DialogOpt(string text)
		{
			if(!Options.Debug) return;
			var x = new StackTrace(1, true);
			TaskDialog.ShowEx("Debug", text, flags: TDFlags.ExpandDown | TDFlags.Wider, expandedText: x.ToString());
		}

		/// <summary>
		/// Checks flags and throws ArgumentException if some flags are invalid. The message includes valid flag names.
		/// Can be used in functions that have an enum flags parameter but not all passed flags are valid for that function or object state.
		/// Does nothing if <see cref="ScriptOptions.Debug">Options.Debug</see> is false.
		/// </summary>
		/// <param name="flags">Flags to check.</param>
		/// <param name="goodFlags">Valid flags.</param>
		/// <typeparam name="T">The enum type used for flags.</typeparam>
		/// <remarks>
		/// When flags are valid, this function is very fast (inline, no calls).
		/// </remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant (IConvertible uses uint)
		internal static void LibCheckFlagsOpt<T>(T flags, T goodFlags) where T : struct, IComparable, IFormattable, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
		{
			//FUTURE: if this is really often useful, make it public.

			int a = Unsafe.As<T, int>(ref flags);
			int b = Unsafe.As<T, int>(ref goodFlags);
			if(a != (a & b)) _CheckFlagsOpt(typeof(T), b);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void _CheckFlagsOpt(Type t, int goodFlags)
		{
			if(!Options.Debug) return;
			if(!t.IsEnum) throw new ArgumentException("Bad type.");
			var s = new StringBuilder("Invalid flags. Only these flags can be used: "); bool added = false;
			for(int i = 1; i != 0; i <<= 1) {
				if(0 == (i & goodFlags)) continue;
				if(added) s.Append(", "); else added = true;
				s.Append(t.GetEnumName(i));
			}
			s.Append('.');
			//Output.Warning(s.ToString(), 1);
			throw new ArgumentException(s.ToString());
		}
	}
}
