using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
using System.Runtime.InteropServices;
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

		///// <summary>
		///// Windows newline string "\r\n".
		///// Allows to replace "one\r\ntwo\r\nthree" with "one"+_+"two"+_+"three" or $"one{_}two{_}three" when you don't want @"multiline string".
		///// </summary>
		//public const string _ = "\r\n";
		////Compiler optimizes "one"+_+"two"+_+"three", but not $"one{_}two{_}three"
		////public static readonly string _ = Environment.NewLine; //compiler does not optimize "one"+_+"two"+_+"three"
		////Not so useful, and interferes with intellisense.

		//public const StringComparison CaseSens = StringComparison.Ordinal;
		//public const StringComparison CaseInsens = StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Alias of Output.Write and Print.
		/// Info: The Output class has more similar functions.
		/// </summary>
		public static void Out(string value) { Output.Write(value); }
		public static void Out(object value) { Output.Write(value); }
		public static void Out<T>(IEnumerable<T> value, string separator = "\r\n") { Output.Write(value, separator); }
		public static void Out(System.Collections.IEnumerable value, string separator = "\r\n") { Output.Write(value, separator); }
		public static void Out<K, V>(IDictionary<K, V> value, string separator = "\r\n") { Output.Write(value, separator); }
		public static void OutList(params object[] values) { Output.WriteList(values); }
		public static void OutListSep(string separator, params object[] values) { Output.WriteListSep(separator, values); }
		public static void OutHex<T>(T value) { Output.WriteHex(value); }
		/// <summary>
		/// Out() that is removed in Release config, ie if DEBUG is not defined.
		/// </summary>
		[Conditional("DEBUG")]
		public static void OutDebug(string value) { Output.Write(value); }

		/// <summary>
		/// Alias of Output.Write and Out.
		/// Info: The Output class has more similar functions.
		/// </summary>
		public static void Print(string value) { Output.Write(value); }
		public static void Print(object value) { Output.Write(value); }
		public static void Print<T>(IEnumerable<T> value, string separator = "\r\n") { Output.Write(value, separator); }
		public static void Print(System.Collections.IEnumerable value, string separator = "\r\n") { Output.Write(value, separator); }
		public static void Print<K, V>(IDictionary<K, V> value, string separator = "\r\n") { Output.Write(value, separator); }
		public static void PrintList(params object[] values) { Output.WriteList(values); }
		public static void PrintListSep(string separator, params object[] values) { Output.WriteListSep(separator, values); }
		public static void PrintHex<T>(T value) { Output.WriteHex(value); }
		/// <summary>
		/// Print() that is removed in Release config, ie if DEBUG is not defined.
		/// </summary>
		[Conditional("DEBUG")]
		public static void PrintDebug(string value) { Output.Write(value); }

		/// <summary>
		/// Gets function name.
		/// Does not get the type name. For example, not useful if called from a constructor (returns ".ctor").
		/// </summary>
		public static string FunctionName([CallerMemberName] string name = null) { return name; }
		public static void OutFunc([CallerMemberName] string name = null) { Output.Write(name); }
		public static void PrintFunc([CallerMemberName] string name = null) { Output.Write(name); }

		/// <summary>
		/// Returns true if the string is null or "".
		/// The same as string.IsNullOrEmpty.
		/// </summary>
		public static bool Empty(string s) { return string.IsNullOrEmpty(s); }

		static readonly uint _winver = _GetWinVersion();
		static uint _GetWinVersion()
		{
			var x = new RTL_OSVERSIONINFOW(); x.dwOSVersionInfoSize = Api.SizeOf(x);
			try {
				if(0 == RtlGetVersion(ref x)) return Calc.MakeUshort(x.dwMinorVersion, x.dwMajorVersion);
				//use this because Environment.OSVersion.Version (GetVersionEx) lies, even if we have correct manifest when is debugger present
			} catch { }
			Debug.Fail("RtlGetVersion");

			var v = Environment.OSVersion.Version;
			return Calc.MakeUshort(v.Minor, v.Major);
		}

		public struct RTL_OSVERSIONINFOW
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

		//public static void Wait(double timeS) { Time.Wait(timeS); } //in Automation; here it could be easily confused with WaitMS
		public static void WaitMS(int timeMS) { Time.WaitMS(timeMS); }
	}
}
