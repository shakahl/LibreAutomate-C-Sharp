using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Compiler
{
	static unsafe class CUtil
	{
#if false
		internal struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		[DllImport("kernel32.dll", EntryPoint = "CreateProcessW", SetLastError = true)]
		internal static extern bool CreateProcess(string lpApplicationName, char[] lpCommandLine, Api.SECURITY_ATTRIBUTES lpProcessAttributes, Api.SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, string lpEnvironment, string lpCurrentDirectory, ref Api.STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

		public static int StartProcess(string exeFile, string args, bool wait = false)
		{
			if(exeFile != null) exeFile = Path_.Normalize(exeFile, Folders.ThisApp);

			string s;
			if(args == null) s = "\"" + exeFile + "\"" + "\0";
			else s = "\"" + exeFile + "\" " + args + "\0";

			Api.STARTUPINFO si = default; //si.cb = Api.SizeOf<Api.STARTUPINFO>();

			if(!CreateProcess(null, s.ToCharArray(), null, null, false, 0, null, null, ref si, out var pi)) return 0;

			Api.CloseHandle(pi.hThread);
			int r;
			if(wait) {
				try {
					WaitFor.Handle(-1, WHFlags.DoEvents, pi.hProcess);
					r = 0; //TODO

				}
				finally { Api.CloseHandle(pi.hProcess); }
			} else {
				Api.CloseHandle(pi.hProcess);
				r = pi.dwProcessId;
			}

			return r;
		}
#endif
	}
}
