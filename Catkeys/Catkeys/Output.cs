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
	/// <summary>
	/// Writes text to the output pane or console.
	/// </summary>
	[DebuggerStepThrough]
	public static class Output
	{
		static readonly bool _isConsole = Console.OpenStandardInput(1) != Stream.Null;
		static IntPtr _hwndEditor;

		static bool _InitHwndEditor()
		{
			if(_hwndEditor==NULL || !Api.IsWindow(_hwndEditor)) _hwndEditor=Api.FindWindow("QM_Editor", null);
			return _hwndEditor != IntPtr.Zero;
		}

		/// <summary>
		/// Clears output pane or console text.
		/// </summary>
		public static void Clear()
		{
			if(_isConsole) Console.Clear();
			else if(_InitHwndEditor()) Api.SendMessageS(_hwndEditor, WM_.SETTEXT, (IntPtr)(-1), null);
		}

		/// <summary>
		/// Writes value to the output pane or console.
		/// </summary>
		public static void Write(string value)
		{
			if(_isConsole) Console.WriteLine(value);
			else if(_InitHwndEditor()) Api.SendMessageS(_hwndEditor, WM_.SETTEXT, (IntPtr)(-1), value==null ? "" : value);
		}
		public static void Write(int value) { Write(value.ToString()); }
		public static void Write(uint value) { Write(value.ToString()); }
		public static void Write(long value) { Write(value.ToString()); }
		public static void Write(ulong value) { Write(value.ToString()); }
		public static void Write(bool value) { Write(value.ToString()); }
		public static void Write(char value) { Write(value.ToString()); }
		public static void Write(char[] value) { Write(new string(value)); }
		public static void Write(double value) { Write(value.ToString()); }
		public static void Write(float value) { Write(value.ToString()); } //don't delete this, because converting float to double is not 100% precise, eg Out(1.2F) shows 1.20000004768372 if using the double overload
		public static void Write(decimal value) { Write(value.ToString()); }
		public static void Write(object value) { Write(value.ToString()); }
		public static void Write(IntPtr value) { Write(value.ToString()); }

		/// <summary>
		/// Writes array or other collection as a list of element values.
		/// Separator depends on element type: string "\r\n", other ", ".
		/// </summary>
		public static void Write<T>(IEnumerable<T> values) {  Write(string.Join((values is IEnumerable<string>) ? "\r\n" : ", ", values)); }

		/// <summary>
		/// Writes multiple values using the specified separator, default " ".
		/// <example>
		/// <code>
		/// Output.Write(", ", a, b, c);
		/// Output.Write($"{a}, {b}, {c}");
		/// </code>
		/// </example>
		/// </summary>
		public static void Write(string separator, params object[] values) { Write(string.Join(separator, values)); }

		/// <summary>
		/// Writes current function name.
		/// </summary>
		public static void WriteFunc([System.Runtime.CompilerServices.CallerMemberName] string name = "") { Write(name); }

		//Don't redirect console and don't use Console.WriteLine.
		//Because there is no way to auto-run a class library initialization code that would redirect console.
		//Static ctors run before the class is used first time, not when assembly loaded.
		//Instead use Out().
		//public static bool isConsole { get; private set; }
		//static Output()
		//{
		//	isConsole=Console.OpenStandardInput(1) != Stream.Null;
		//	if(!isConsole) Console.SetOut(new _QmWriter());
		//}
		//public static void RedirectConsoleWriteToQm()
		//{
		//	Console.SetOut(new _QmWriter());
		//}
		//
		//class _QmWriter : TextWriter
		//{
		//    public override void Write(string value) { Out(value); }
		//    public override void WriteLine(string value) { Out(value); }
		//    public override Encoding Encoding { get { return Encoding.Unicode; } }
		//}
	}
}
