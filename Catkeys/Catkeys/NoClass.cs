using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
using System.IO;
//using System.Windows.Forms;

using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	public static class NoClass
	{
		/// <summary>
		/// Windows newline string "\r\n".
		/// </summary>
		public const string _ = "\r\n";
		//Compiler optimizes "one"+_+"two"+_+"three", but not $"one{_}two{_}three"
		//public static readonly string _ = Environment.NewLine; //compiler does not optimize "one"+_+"two"+_+"three"

		//public const StringComparison CaseSens = StringComparison.Ordinal;
		//public const StringComparison CaseInsens = StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Alias of Output.Write.
		/// </summary>
		public static void Out(string value) { Output.Write(value); }
		public static void Out(int value) { Output.Write(value); }
		public static void Out(uint value) { Output.Write(value); }
		public static void Out(long value) { Output.Write(value); }
		public static void Out(ulong value) { Output.Write(value); }
		public static void Out(bool value) { Output.Write(value); }
		public static void Out(char value) { Output.Write(value); }
		public static void Out(char[] value) { Output.Write(value); }
		public static void Out(double value) { Output.Write(value); }
		public static void Out(float value) { Output.Write(value); }
		public static void Out(decimal value) { Output.Write(value); }
		public static void Out(object value) { Output.Write(value); }
		public static void Out(IntPtr value) { Output.Write(value); }
		//public static void Out<T>(T value) { Output.Write(value); } //this could replace all the above overloads, but it disables Out<T>(IEnumerable<T> values). Also, for intellisense it's better to have all overloads here.
		public static void Out<T>(IEnumerable<T> values) { Output.Write(values); }
		public static void Out(string separator, params object[] values) { Output.Write(separator, values); }

	}
}
