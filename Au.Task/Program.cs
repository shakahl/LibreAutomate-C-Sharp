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

namespace Au.Task
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//Output.LibUseQM2 = true;
			//Print(Environment.CommandLine);
			//Print(args);
			//return;

			if(args.Length == 0) return;
			string asm = args[0];
			if(args.Length == 1) args = null; else args = args.RemoveAt_(0);

			int pdbOffset = 0;
			int i = asm.IndexOf('|') + 1;
			if(i > 0) { pdbOffset = asm.ToInt_(i); asm = asm.Remove(i - 1); }

			var config = asm + ".config";
			if(File_.ExistsAsFile(config, true)) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);

			Au.Util.LibRunAsm.RunHere(asm, pdbOffset, args);
		}
	}
}
