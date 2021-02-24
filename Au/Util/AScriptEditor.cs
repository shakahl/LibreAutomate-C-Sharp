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
	/// Can be used in scripts to interact with the script editor, if available.
	/// </summary>
	public static class AScriptEditor
	{
		/// <summary>
		/// Opens the specified source file and goes to the specified line.
		/// Does nothing if editor isn't running.
		/// </summary>
		/// <param name="sourceFile">The source file. Can be full path, or relative path in workspace, or file name with ".cs".</param>
		/// <param name="line">1-based line index. If 0, just opens file.</param>
		public static void GoToEdit(string sourceFile, int line) {
			var wmsg = ATask.WndMsg_; if (wmsg.Is0) return;
			Api.AllowSetForegroundWindow(wmsg.ProcessId);
			AWnd.More.CopyDataStruct.SendString(wmsg, 4, sourceFile, line);
		}

		/// <summary>
		/// Returns true if editor is running.
		/// </summary>
		public static bool Available => !ATask.WndMsg_.Is0;
	}
}
