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
	/// Writes text to the output pane.
	/// In console process writes to the console window by default.
	/// </summary>
	[DebuggerStepThrough]
	public static class Output
	{
		static readonly bool _isConsole = Console.OpenStandardInput(1) != Stream.Null;

		/// <summary>
		/// Returns true if this is a console process.
		/// </summary>
		public static bool IsConsoleProcess { get { return _isConsole; } }

		/// <summary>
		/// If true, Output.WriteX and Output.Clear functions will use output even in console process.
		/// This is not applied to Console.Write and other Console class functions.
		/// </summary>
		public static bool AlwaysOutput { get; set; }

		static Wnd _hwndEditor;

		static bool _InitHwndEditor()
		{
			if(_hwndEditor==Zero || !Api.IsWindow(_hwndEditor)) _hwndEditor=Api.FindWindow("QM_Editor", null);
			return _hwndEditor != Zero;
		}

		/// <summary>
		/// Clears output pane or console text.
		/// </summary>
		public static void Clear()
		{
			if(_isConsole && !AlwaysOutput) Console.Clear();
			else if(_InitHwndEditor()) _hwndEditor.Send(Api.WM_SETTEXT, -1, null);
		}

		/// <summary>
		/// Writes value to the output pane or console.
		/// </summary>
		public static void Write(string value) { Writer.WriteLine(value); }
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
		public static void Write<T>(IEnumerable<T> values) { Write(string.Join((values is IEnumerable<string>) ? "\r\n" : ", ", values)); }
		//public static void Write<T>(IEnumerable<T> values) { Write(string.Join(((values is IEnumerable<string>) || (values is IEnumerable<KeyValuePair>) ? "\r\n" : ", ", values)); }

		/// <summary>
		/// Writes dictionary as a list of element pair values.
		/// Separator "\r\n".
		/// </summary>
		public static void Write<K, V>(IDictionary<K, V> values) { Write(string.Join("\r\n", values)); }

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

		//Note:
		//In library don't redirect console and don't use Console.WriteLine.
		//Because there is no way to auto-run a class library initialization code that would redirect console.
		//Static ctors run before the class is used first time, not when assembly loaded.
		//Instead use Out() (Output.Write()).
		//To redirect Console.WriteLine used in scripts, call Output.RedirectConsoleOutput in script code.

		//Used to redirect Console and Debug/Trace output.
		class _OutputWriter :TextWriter
		{
			public override void WriteLine(string value) { WriteDirectly(value); }
			public override void Write(string value) { WriteDirectly(value); }
			public override Encoding Encoding { get { return Encoding.Unicode; } }
		}

		static TextWriter _writer;

		/// <summary>
		/// Gets or sets object that actually writes text when your script or the automation library calls Output.Write or Out.
		/// If you want to redirect or modify output text (for example write to a file or add time), use code like this:
		/// <c>Output.Writer=myWriter;</c>, where myWriter is a variable of your class that is derived from TextWriter and overrides its functions WriteLine, Write and Encoding.
		/// It is like redirecting console output with code <c>Console.SetOut(myWriter);</c> (google for more info).
		/// Usually the best place to redirect is in static ScriptClass() {  }.
		/// Redirection is applied to whole script appdomain, and does not affect other scripts.
		/// Redirection affects Write, RedirectConsoleOutput, RedirectDebugOutput, AlwaysOutput and all OutX. It does not affect WriteDirectly and Clear.
		/// Don't call Output.Write and Out in your writer class; it would create stack overflow; if need, use Output.WriteDirectly.
		/// </summary>
		public static TextWriter Writer
		{
			get { return _writer ?? (_writer=new _OutputWriter()); }
			set { _writer=value; }
		}

		/// <summary>
		/// Writes string value to the output or console.
		/// Unlike other Write methods, this function ignores custom Writer used to redirect output, therefore can be used in your custom writer class.
		/// </summary>
		public static void WriteDirectly(string value)
		{
			if(_isConsole && !AlwaysOutput) Console.WriteLine(value);
			else if(_InitHwndEditor()) _hwndEditor.SendS(Api.WM_SETTEXT, -1, value==null ? "" : value);
		}

		static bool _consoleRedirected, _debugRedirected;

		/// <summary>
		/// Let Console.WriteX methods write to the output, unless this is a console process (even if AlwaysOutput is true).
		/// Console.Write will write line, like Console.WriteLine.
		/// Note: Console.Clear will not clear output; it will throw exception; use Output.Clear or try/catch.
		/// </summary>
		public static void RedirectConsoleOutput()
		{
			if(_consoleRedirected || _isConsole) return;
			_consoleRedirected=true;
            Console.SetOut(Writer);
			//speed: 870
		}

		/// <summary>
		/// Let Debug.WriteX and Trace.WriteX methods write to the output or console.
		/// To write to the output even in console process, set Output.AlwaysOutput=true; before calling this method first time.
		/// </summary>
		public static void RedirectDebugOutput()
		{
			if(_debugRedirected) return;
			_debugRedirected=true;
			Trace.Listeners.Add((_isConsole && !AlwaysOutput) ? (new ConsoleTraceListener()) : (new TextWriterTraceListener(Writer)));
			//speed: 6100
		}

		//TODO: WriteStatusBar()
	}
}
