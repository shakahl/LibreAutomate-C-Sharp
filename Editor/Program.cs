using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;


//using l = Catkeys;
//using static Catkeys.NoClass;
////using Catkeys.Winapi;

//using System.Collections.Generic;
//using SysText = System.Text;
//using SysRX = System.Text.RegularExpressions;
//using SysDiag = System.Diagnostics;
//using SysInterop = System.Runtime.InteropServices;
//using SysCompil = System.Runtime.CompilerServices;
//using SysIO = System.IO;
//using SysThread = System.Threading;
//using SysTask = System.Threading.Tasks;
//using SysReg = Microsoft.Win32;
//using SysForm = System.Windows.Forms;
//using SysDraw = System.Drawing;
////using System.Linq;



//using SysColl = System.Collections.Generic; //add directly, because often used, and almost everything is useful
//using SysCompon = System.ComponentModel;
//using SysExcept = System.Runtime.ExceptionServices; //only HandleProcessCorruptedStateExceptionsAttribute




namespace Editor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		//[SysCompil.MethodImpl(SysCompil.MethodImplOptions.NoOptimization)]
		static void Main()
		{
			//SysColl.List<int> a;
			//SysText.StringBuilder sb;
			//SysRX.Regex rx;
			//SysDiag.Debug.Assert(true);
			//SysInterop.SafeHandle se;
			//SysIO.File.Create("");
			//SysThread.Thread th;
			//SysTask.Task task;
			//SysReg.RegistryKey k;
			//SysForm.Form form;
			//SysDraw.Rectangle rect;
			//System.Runtime.CompilerServices



			//l.Perf.First();
			//l.Show.TaskDialog("f");
			//l.Util.Debug_.OutLoadedAssemblies();
			//Out(l.TDIcon.Info);

			//zPerf.First();
			//zShow.TaskDialog("f");
			//zOut(zTDIcon.Info);

			//qPerf.First();
			//qShow.TaskDialog("f");
			//qOut(zTDIcon.Info);

			//QPerf.First();
			//QShow.TaskDialog("f");
			//QOut(zTDIcon.Info);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
