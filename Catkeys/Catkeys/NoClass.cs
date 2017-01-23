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
		/// Default (0) value for IntPtr, LPARAM and some other types. The same as IntPtr.Zero or default(IntPtr).
		/// These types are struct, therefore you cannot assign 0 or null.
		/// However this is error: <c>void Func(IntPtr x=Zero){}</c>; use <c>void Func(IntPtr x=default(IntPtr)){}</c>.
		/// </summary>
		public static readonly IntPtr Zero;

		/// <summary>
		/// Default (0) value for Wnd.
		/// The Wnd type is struct, therefore you cannot assign 0 or null.
		/// However this is error: <c>void Func(Wnd x=Wnd0){}</c>; use <c>void Func(Wnd x=default(Wnd)){}</c>.
		/// </summary>
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
		/// Print() that works only in Debug configuration, ie if DEBUG is defined.
		/// In Release configuration the function call statement is removed, and arguments not evalueted.
		/// The 3 optional parameters are not used.
		/// </summary>
		[Conditional("DEBUG")]
		public static void DebugPrint(object value, [CallerFilePath]string cp = null, [CallerLineNumber]int cln = 0, [CallerMemberName]string cmn = null)
		{
			Output.Write($"Debug: {cmn} ({Path.GetFileName(cp)}:{cln}):  {value}");
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
			TaskDialog.ShowEx("Debug", text?.ToString(), flags:TDFlags.ExpandDown, expandedText: $"{cmn} ({Path.GetFileName(cp)}:{cln})");
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

		static readonly uint _winver = _GetWinVersion();
		static uint _GetWinVersion()
		{
			var x = new RTL_OSVERSIONINFOW(); x.dwOSVersionInfoSize = Api.SizeOf(x);
			try {
				if(0 == RtlGetVersion(ref x)) return Calc.MakeUshort(x.dwMinorVersion, x.dwMajorVersion);
				//use this because Environment.OSVersion.Version (GetVersionEx) lies, even if we have correct manifest when is debugger present
			}
			catch { }
			Debug.Fail("RtlGetVersion");

			var v = Environment.OSVersion.Version;
			return Calc.MakeUshort(v.Minor, v.Major);
		}

		struct RTL_OSVERSIONINFOW
		{
			public uint dwOSVersionInfoSize;
			public uint dwMajorVersion;
			public uint dwMinorVersion;
			public uint dwBuildNumber;
			public uint dwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;
		}

		[DllImport("ntdll.dll", ExactSpelling = true)]
		static extern int RtlGetVersion(ref RTL_OSVERSIONINFOW lpVersionInformation);

		/// <summary>
		/// Gets classic Windows major+minor version value:
		/// Win7 (0x601), Win8 (0x602), Win8_1 (0x603), Win10 (0xA00).
		/// Example: <c>if(WinVer>=Win8) ...</c>
		/// </summary>
		public static uint WinVer
		{
			get { return _winver; }
		}

		/// <summary>
		/// Classic Windows version major+minor values.
		/// Example: <c>if(WinVer>=Win8) ...</c>
		/// </summary>
		public const uint Win7 = 0x601, Win8 = 0x602, Win8_1 = 0x603, Win10 = 0xA00;

		/// <summary>
		/// Alias for <see cref="Time.WaitMS"/>.
		/// </summary>
		/// <param name="timeMS"></param>
		public static void WaitMS(int timeMS) { Time.WaitMS(timeMS); }
		//public static void Wait(double timeS) { Time.Wait(timeS); } //in Automation; here it could be easily confused with WaitMS

		//public static void Key(params string[] keys)
		//{
		//	//info:
		//	//Named not Keys because then hides enum Keys from System.Windows.Forms. Also various interfaces have members named Keys.
		//	//Named not SendKeys because too long and hides class SendKeys from System.Windows.Forms.
		//	//Never mind: Key hides enum Key from Microsoft.DirectX.DirectInput and System.Windows.Input. Also various classes have Key property.

		//	Input.Keys(keys);
		//}
	}
}
