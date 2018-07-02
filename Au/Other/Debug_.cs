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
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Functions useful to debug code.
	/// </summary>
	/// <remarks>
	/// The Debug_.PrintX functions write to the same output as <see cref="Output.Write"/>, not to the trace listeners like <see cref="Debug.Print(string)"/> etc do. Also they add caller's name, file and line number.
	/// Functions Print, PrintIf, PrintFunc and Dialog work only if DEBUG is defined, which normally is when the caller project is in Debug configuration. Else they are not called, and arguments not evaluated at run time. This is because they have [<see cref="ConditionalAttribute"/>("DEBUG")].
	/// Note: when used in a library, the above functions depend on DEBUG of the library project and not on DEBUG of the consumer project of the library. For example, the library may be in Release configuration even if its consumer project is in Debug configuration. If your library wants to show some info only if its consumer project is in Debug config, instead you can use code like <c>if(Opt.Debug.Verbose) PrintWarning("text");</c>; see <see cref="PrintWarning"/>, Opt.Debug.<see cref="OptDebug.Verbose" r=""/>.
	/// </remarks>
	//[DebuggerStepThrough]
	public static class Debug_
	{
		/// <summary>
		/// Prefix for Debug_.Print, Debug_.PrintIf.
		/// Default is "Debug: ".
		/// </summary>
		/// <example>
		/// Blue text.
		/// <code><![CDATA[
		/// Debug_.TextPrefix = "<><c 0xff0000>"; Debug_.TextSuffix = "</c>";
		/// ]]></code>
		/// </example>
		public static string TextPrefix { get; set; } = "Debug: ";
		//info: named not Prefix, because intellisense selects it when we want Print, it is annoying

		/// <summary>
		/// Suffix for Debug_.Print, Debug_.PrintIf.
		/// </summary>
		/// <seealso cref="TextPrefix"/>
		public static string TextSuffix { get; set; }

		static void _Print(object text, string cp, int cln, string cmn)
		{
			string s = LibPrintObjectToString(text);
			Output.Write($"{TextPrefix}{cmn} ({Path_.GetFileName(cp)}:{cln}):  {s}{TextSuffix}");
		}

		/// <summary>
		/// Calls <see cref="Output.Write"/> to show some debug info. Also shows current function name/file/line.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional arguments are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Print(object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			_Print(text, cp, cln, cmn);
		}

		/// <summary>
		/// If condition is true, calls <see cref="Output.Write"/> to show some debug info. Also shows current function name/file/line.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional arguments are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintIf(bool condition, object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			if(condition) _Print(text, cp, cln, cmn);
		}

		/// <summary>
		/// Calls <see cref="Output.Write"/> with current function name.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The optional argument is not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintFunc([CallerMemberName] string name = null) { Output.Write(name); }

		/// <summary>
		/// Calls <see cref="AuDialog.Show"/> to show some debug info.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional arguments are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Dialog(object text, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			string s = LibPrintObjectToString(text);
			AuDialog.ShowEx("Debug", s, flags: DFlags.ExpandDown, expandedText: $"{cmn} ({Path_.GetFileName(cp)}:{cln})");
		}

		//rejected: use if(Opt.Debug.Verbose) AuDialog.ShowWarning(...). It adds stack trace.
		///// <summary>
		///// If Opt.Debug.<see cref="OptDebug.Verbose" r=""/> == true, calls <see cref="AuDialog.Show"/> with text and stack trace.
		///// Read more in class help.
		///// </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//public static void DialogOpt(string text)
		//{
		//	if(!Opt.Debug.Verbose) return;
		//	var x = new StackTrace(1, true);
		//	AuDialog.ShowEx("Debug", text, flags: DFlags.ExpandDown | DFlags.Wider, expandedText: x.ToString());
		//}

		//rejected: Not used in this library. Not useful for debug because don't show the stack trace. Instead use PrintWarning; it supports prefix "Debug: ", "Note: ", "Info :"; it also supports disabling warnings etc.
		///// <summary>
		///// If Opt.Debug.<see cref="OptDebug.Verbose" r=""/> == true, calls <see cref="Output.Write(string)"/>.
		///// Read more in class help.
		///// </summary>
		//public static void PrintOpt(string text)
		//{
		//	if(Opt.Debug.Verbose) Output.Write("Debug: " + text);
		//}

		//rejected: Don't need multiple warning functions. Now PrintWarning does not show more than 1 warning/second if Opt.Debug.Verbose is false. Also users can add this in script themplate: #if !DEBUG Options.DisableWarnings(...);
		///// <summary>
		///// If Opt.Debug.<see cref="OptDebug.Verbose" r=""/> == true, calls <see cref="PrintWarning"/>.
		///// Read more in class help.
		///// </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//public static void WarningOpt(string text)
		//{
		//	if(Opt.Debug.Verbose) PrintWarning(text, 1);
		//}

		/// <summary>
		/// Checks flags and throws ArgumentException if some flags are invalid. The message includes valid flag names.
		/// </summary>
		/// <param name="flags">Flags to check.</param>
		/// <param name="goodFlags">Valid flags.</param>
		/// <typeparam name="T">The enum type used for flags.</typeparam>
		/// <remarks>
		/// Can be used in functions that have an enum flags parameter but not all passed flags are valid for that function or object state.
		/// Does nothing if Opt.Debug.<see cref="OptDebug.Verbose" r=""/> == false.
		/// When flags are valid, this function is very fast (inline, no calls).
		/// </remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant (IConvertible uses uint)
		internal static void LibCheckFlagsOpt<T>(T flags, T goodFlags) where T : struct, IComparable, IFormattable, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
		{
			//FUTURE: if this is really often useful, make it public. If not used - remove.

			int a = Unsafe.As<T, int>(ref flags);
			int b = Unsafe.As<T, int>(ref goodFlags);
			if(a != (a & b)) _CheckFlagsOpt(typeof(T), b);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void _CheckFlagsOpt(Type t, int goodFlags)
		{
			if(!Opt.Debug.Verbose) return;
			if(!t.IsEnum) throw new ArgumentException("Bad type.");
			var s = new StringBuilder("Invalid flags. Only these flags can be used: "); bool added = false;
			for(int i = 1; i != 0; i <<= 1) {
				if(0 == (i & goodFlags)) continue;
				if(added) s.Append(", "); else added = true;
				s.Append(t.GetEnumName(i));
			}
			s.Append('.');
			//PrintWarning(s.ToString(), 1);
			throw new ArgumentException(s.ToString());
		}

#if DEBUG
		internal static int LibGetComObjRefCount(IntPtr obj)
		{
			Marshal.AddRef(obj);
			return Marshal.Release(obj);
		}
#endif

		/// <summary>
		/// Returns true if using Debug configuration of Au.dll.
		/// </summary>
		public static bool IsAuDebugConfiguration
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// Returns managed memory size as formatted string. Uses GC.GetTotalMemory.
		/// Works in Release too.
		/// </summary>
		static long s_mem0;
		internal static string LibGetMemory()
		{
			var mem = GC.GetTotalMemory(false);
			if(s_mem0 == 0) s_mem0 = mem;
			return ((mem - s_mem0) / 1024d / 1024d).ToString_("F3");
		}
		/// <summary>
		/// Prints managed memory size. Uses GC.GetTotalMemory.
		/// Works in Release too.
		/// </summary>
		internal static void LibPrintMemory() => Print(LibGetMemory());
	}
}
