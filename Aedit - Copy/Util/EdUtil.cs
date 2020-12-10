using System;
using System.Collections.Generic;
using System.Collections;
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

using Au;
using Au.Types;

/// <summary>
/// Misc util functions.
/// </summary>
static class EdUtil
{
	//public static void MinimizeProcessPhysicalMemory(int afterMS)
	//{
	//	Task.Delay(afterMS).ContinueWith(_ => {
	//		GC.Collect();
	//		GC.WaitForPendingFinalizers();
	//		Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), -1, -1);
	//	});
	//}
}

#if DEBUG

static class EdDebug
{
}

#endif
