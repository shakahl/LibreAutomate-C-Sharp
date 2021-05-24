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
//using System.Linq;

using Au.Types;

namespace Au
{
	/// <summary>
	/// Functions useful to debug code.
	/// </summary>
	/// <remarks>
	/// The ADebug.PrintX functions write to the same output as <see cref="AOutput.Write"/>, not to the trace listeners like <see cref="Debug.Print(string)"/> etc do. Also they add caller's name, file and line number.
	/// Functions Print, PrintIf, PrintFunc and Dialog work only if DEBUG is defined, which normally is when the caller project is in Debug configuration. Else they are not called, and arguments not evaluated at run time. This is because they have [<see cref="ConditionalAttribute"/>("DEBUG")].
	/// Note: when used in a library, the above functions depend on DEBUG of the library project and not on DEBUG of the consumer project of the library. For example, the library may be in Release configuration even if its consumer project is in Debug configuration. If your library wants to show some info only if its consumer project is in Debug config, instead you can use code like <c>if(AOpt.Warnings.Verbose) AWarning.Write("text");</c>; see <see cref="AWarning.Write"/>, AOpt.Warnings.<see cref="OWarnings.Verbose"/>.
	/// </remarks>
	internal static class ADebug //FUTURE: make public, when will be more tested and if really need.
	{
		static void _Print(object text, string f_, int l_, string m_) {
			string s = AOutput.ObjectToString_(text);
			string prefix = null; if (s.Starts("<>")) { prefix = "<>"; s = s[2..]; }
			s = $"{prefix}Debug: {m_} ({APath.GetName(f_)}:{l_}):  {s}";
			_Print2(s);
		}

		static void _Print2(object o) {
			string s = o?.ToString();
			if (UseQM2) AOutput.QM2.Write(s); else AOutput.Write(s);
		}

		internal static bool UseQM2;

		/// <summary>
		/// Calls <see cref="AOutput.Write"/> to show some debug info. Also shows current function name/file/line.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional arguments are not used explicitly.
		/// If text starts with "&lt;&gt;", it can contain output tags.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Print(object text, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0, [CallerMemberName] string m_ = null)
			=> _Print(text, f_, l_, m_);

		/// <summary>
		/// If condition is true, calls <see cref="AOutput.Write"/> to show some debug info. Also shows current function name/file/line.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional arguments are not used explicitly.
		/// If text starts with "&lt;&gt;", it can contain output tags.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintIf(bool condition, object text, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0, [CallerMemberName] string m_ = null) {
			if (condition) _Print(text, f_, l_, m_);
		}

		/// <summary>
		/// Calls <see cref="AOutput.Write"/> with current function name.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The optional argument is not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintFunc([CallerMemberName] string m_ = null)
			=> _Print2(m_);

		/// <summary>
		/// In DEBUG config prints ALastError.Message.
		/// </summary>
		[Conditional("DEBUG")]
		internal static void PrintNativeError_([CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0, [CallerMemberName] string m_ = null)
			=> _Print(ALastError.Message, f_, l_, m_);

		/// <summary>
		/// In DEBUG config prints ALastError.MessageFor(code).
		/// </summary>
		[Conditional("DEBUG")]
		internal static void PrintNativeError_(int code, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0, [CallerMemberName] string m_ = null)
			=> _Print(ALastError.MessageFor(code), f_, l_, m_);

		/// <summary>
		/// Calls <see cref="ADialog.Show"/> to show some debug info.
		/// Works only if DEBUG is defined. Read more in class help.
		/// The 3 optional arguments are not used explicitly.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Dialog(object text, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0, [CallerMemberName] string m_ = null) {
			string s = AOutput.ObjectToString_(text);
			ADialog.Show("Debug", s, flags: DFlags.ExpandDown, expandedText: $"{m_} ({APath.GetName(f_)}:{l_})");
		}

		//rejected: use if(AOpt.Warnings.Verbose) ADialog.ShowWarning(...). It adds stack trace.
		///// <summary>
		///// If AOpt.Warnings.<see cref="OWarnings.Verbose"/> == true, calls <see cref="ADialog.Show"/> with text and stack trace.
		///// Read more in class help.
		///// </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//public static void DialogOpt(string text)
		//{
		//	if(!AOpt.Warnings.Verbose) return;
		//	var x = new StackTrace(1, true);
		//	ADialog.Show("Debug", text, flags: DFlags.ExpandDown | DFlags.Wider, expandedText: x.ToString());
		//}

		//rejected: Not used in this library. Not useful for debug because don't show the stack trace. Instead use AWarning.Write; it supports prefix "Debug: ", "Note: ", "Info :"; it also supports disabling warnings etc.
		///// <summary>
		///// If AOpt.Warnings.<see cref="OWarnings.Verbose"/> == true, calls <see cref="AOutput.Write(string)"/>.
		///// Read more in class help.
		///// </summary>
		//public static void PrintOpt(string text)
		//{
		//	if(AOpt.Warnings.Verbose) _Print("Debug: " + text);
		//}

		//rejected: Don't need multiple warning functions. Now AWarning.Write does not show more than 1 warning/second if AOpt.Warnings.Verbose is false.
		///// <summary>
		///// If AOpt.Warnings.<see cref="OWarnings.Verbose"/> == true, calls <see cref="AWarning.Write"/>.
		///// Read more in class help.
		///// </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//public static void WarningOpt(string text)
		//{
		//	if(AOpt.Warnings.Verbose) AWarning.Write(text, 1);
		//}

		/// <summary>
		/// Checks flags and throws ArgumentException if some flags are invalid. The error message includes valid flag names.
		/// </summary>
		/// <param name="flags">Flags to check.</param>
		/// <param name="goodFlags">Valid flags.</param>
		/// <remarks>
		/// Can be used in functions that have an enum flags parameter but not all passed flags are valid for that function or object state.
		/// Does nothing if AOpt.Warnings.<see cref="OWarnings.Verbose"/> == false.
		/// When flags are valid, this function is fast.
		/// </remarks>
		internal static unsafe void CheckFlagsOpt_<T>(T flags, T goodFlags) where T : unmanaged, Enum {
			//FUTURE: if this is really often useful, make it public. If not used - remove.

			Debug.Assert(sizeof(T) == 4);
			int a = *(int*)&flags;
			int b = *(int*)&goodFlags;
			if (a != (a & b)) _CheckFlagsOpt(typeof(T), b);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void _CheckFlagsOpt(Type t, int goodFlags) {
			if (!AOpt.Warnings.Verbose) return;
			if (!t.IsEnum) throw new ArgumentException("Bad type.");
			var s = new StringBuilder("Invalid flags. Only these flags can be used: "); bool added = false;
			for (int i = 1; i != 0; i <<= 1) {
				if (0 == (i & goodFlags)) continue;
				if (added) s.Append(", "); else added = true;
				s.Append(t.GetEnumName(i));
			}
			s.Append('.');
			//AWarning.Write(s.ToString(), 1);
			throw new ArgumentException(s.ToString());
		}

#if DEBUG
		internal static int GetComObjRefCount_(IntPtr obj) {
			Marshal.AddRef(obj);
			return Marshal.Release(obj);
		}
#endif

		/// <summary>
		/// Returns true if using Debug configuration of Au.dll.
		/// </summary>
		public static bool IsAuDebugConfiguration {
			get {
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
		/// <param name="fromAnchor">Get the difference from previous call to <b>MemorySetAnchor_</b>.</param>
		internal static string MemoryGet_(bool fromAnchor = true) {
			var mem = GC.GetTotalMemory(false);
			//if(s_mem0 == 0) s_mem0 = mem;
			if (fromAnchor) mem -= s_mem0;
			return (mem / 1024d / 1024d).ToStringInvariant("F3");
		}
		static long s_mem0;

		/// <summary>
		/// Prints managed memory size. Uses GC.GetTotalMemory.
		/// Works in Release too.
		/// </summary>
		/// <param name="fromAnchor">Get the difference from previous call to <b>MemorySetAnchor_</b>.</param>
		internal static void MemoryPrint_(bool fromAnchor = true) => _Print2(MemoryGet_(fromAnchor));

		/// <summary>
		/// Memorizes current managed memory size, so that next call to another <b>MemoryX</b> function with fromAnchor=true (default) will get memory size difference from current memory size.
		/// </summary>
		internal static void MemorySetAnchor_() { s_mem0 = GC.GetTotalMemory(false); }

		/// <summary>
		/// Prints assemblies already loaded or/and loaded in the future.
		/// </summary>
		internal static void PrintLoadedAssemblies(bool now, bool future, bool stackTrace = false) {
			if (now) {
				var a = AppDomain.CurrentDomain.GetAssemblies();
				_Print2("-- now --");
				foreach (var v in a) _Print2("# " + v.FullName);
			}
			if (future) {
				if (stackTrace) new StackTrace(1, true); //load assemblies used by stack trace
				_Print2("-- future --");
				AppDomain.CurrentDomain.AssemblyLoad += (object sender, AssemblyLoadEventArgs e) => {
					_Print2("# " + e.LoadedAssembly.FullName);
					if (stackTrace) _Print2(new StackTrace(1, true));
				};
				//var stack = new Stack<string>();
			}
		}
	}
}
