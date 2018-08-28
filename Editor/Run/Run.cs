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
using static Program;
using Au.Compiler;

static class Run
{
	public static void CompileAndRun(FileNode f, bool run, string[] args = null)
	{
		Model.Save.TextNowIfNeed();

		if(f == null) return;

		var nodeType = f.NodeType;
		if(!(nodeType == ENodeType.Script || nodeType == ENodeType.CS)) return;

		Compiler.CompileAndRun(f, run, args);

		//int n = s_test1 ? 100 : 1; s_test1 = true;

		//Print("start");
		//for(int i = 0; i < n; i++) {
		//	if(!Compiler.CompileAndRun(f, run, args, CRFlags.Wait)) return;
		//	Time.SleepDoEvents(1);
		//}
		//Print("end");

		//GC.Collect();
	}
	//static bool s_test1;
}
