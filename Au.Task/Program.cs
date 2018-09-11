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

using Au;
using Au.Types;
using static Au.NoClass;
using Au.LibRun;

namespace Au.Task
{
	static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			if(args.Length < 4) return;
			string name = args[0], asm = args[1];
			int pdbOffset = args[2].ToInt_();
			int flags = args[3].ToInt_();
			if(args.Length == 4) args = null; else args = args.RemoveAt_(0, 4);

			Script.Name = name;
			//AppDomain.CurrentDomain.SetData("APP_NAME", name); //does not work. How to change appdomain name?

			if(0 != (flags & 1)) { //hasConfig
				var config = asm + ".config";
				if(File_.ExistsAsFile(config, true)) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);
			}

			var threadName = "[script] " + name;
			if(0 != (flags & 2)) { //mtaThread
				var t = new Thread(() => _Thread(threadName, asm, pdbOffset, args));
				t.Start();
				t.Join();
			} else {
				_Thread(threadName, asm, pdbOffset, args);
			}
		}

		static void _Thread(string threadName, string asm, int pdbOffset, string[] args)
		{
			Thread.CurrentThread.Name = threadName;
			try { RunAsm.RunHere(RIsolation.process, asm, pdbOffset, args); }
			catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }
		}
	}
}
