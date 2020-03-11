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
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// .NET Core does not show the classic Debug.Assert message box. This class shows it. Call <see cref="Setup"/> once.
	/// The Write[Line] and Fail methods write line to QM2.
	/// If debugger attached, breaks and does not show message box.
	/// </summary>
	class AssertListener_ : TraceListener
	{
		[Conditional("DEBUG")]
		public static void Setup()
		{
			Trace.Listeners.Clear(); //the default listener calls Environment.FailFast with no info
			Trace.Listeners.Add(new AssertListener_());
		}

		public override void Write(string message)
		{
			WriteLine(message);
		}

		public override void WriteLine(string message)
		{
			AOutput.QM2.Write(message);
		}

		public override void Fail(string message, string detailMessage)
		{
			if(!detailMessage.IsNE()) message = message + " " + detailMessage;
			var s = message + "\r\n" + new StackTrace(4, true);
			WriteLine(s);
			if(!Debugger.IsAttached) {
				switch(Api.MessageBox(default, s, "Assertion failed. Continue?", 0x40012)) { //MB_ICONERROR|MB_TOPMOST|MB_ABORTRETRYIGNORE
				case 5: return; //ignore
				case 4: break; //retry
				default: Environment.FailFast(""); break;
				}
				Debugger.Launch();
			}
			Debugger.Break();
		}

		public override void Fail(string message)
		{
			Fail(message, null);
		}
	}
}
