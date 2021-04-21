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

namespace Au.Util
{
	/// <summary>
	/// If defined DEBUG or TRACE, <see cref="Setup"/> replaces default trace listener with listener that overrides its <see cref="DefaultTraceListener.Fail(string?, string?)"/> method. On failed assertion (<see cref="Debug.Assert"/>, <see cref="Trace.Assert"/>, <see cref="Debug.Fail"/>, <see cref="Trace.Fail"/>) it shows message box with buttons Exit|Debug|Ignore, unless debugger is attached or !<b>AssertUiEnabled</b>.
	/// </summary>
	public class ADefaultTraceListener : DefaultTraceListener
	{
		/// <summary>
		/// Replaces default trace listener.
		/// </summary>
		/// <param name="useAOutput">Also set <see cref="AOutput.RedirectDebugOutput"/> = true.</param>
		//[Conditional("DEBUG"), Conditional("TRACE")] //no, in most cases this is called by this library, not directly by the app
		public static void Setup(bool useAOutput) {
			if (!s_setup) {
				s_setup = true;
				Trace.Listeners.Remove("Default"); //remove DefaultTraceListener. It calls Environment.FailFast which shows message box "Unknown hard error".
				Trace.Listeners.Add(new ADefaultTraceListener());
			}
			AOutput.RedirectDebugOutput = useAOutput;
		}
		static bool s_setup;

		///
		public override void Fail(string message, string detailMessage) {
			var s = message;
			if (s.NE()) s = detailMessage; else if (!detailMessage.NE()) s = message + "\r\n" + detailMessage;
			if (!s.NE()) s += "\r\n";

			string st = new StackTrace(2, true).ToString(), st1 = null;
			if (st.RegexMatch(@"(?m)^\s+at (?!System\.Diagnostics\.)", 0, out RXGroup g)) {
				st = st[g.Start..];
				st1 = st.Lines(true)[0];
			}

			var s2 = "---- Debug assertion failed ----\r\n" + s + st;
			Trace.WriteLine(s2);
			if (!(AOutput.RedirectDebugOutput && AOutput.QM2.UseQM2)) AOutput.QM2.Write(s2);

			if (Debugger.IsAttached) {
				Debugger.Break();
			} else {
				if (!AssertUiEnabled) return; //like default listener

				switch (ADialog.ShowWarning("Debug assertion failed", s + st1, "Exit|Debug|Ignore", expandedText: st)) {
				case 3: return;
				case 2: break;
				default: Api.ExitProcess(-1); break;
				}
				Debugger.Launch();
			}
		}
	}
}
