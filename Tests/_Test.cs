using System;
using System.Collections.Generic;
using System.Collections;
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
//using Registry = Microsoft.Win32.Registry;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Security.AccessControl;

using Au;
using Au.Types;
using static Au.AStatic;
//using Au.Util;
using Au.Controls;
using Au.Triggers;

using Microsoft.Win32.SafeHandles;

using System.IO.Compression;
using System.Reflection.Emit;
using System.Net;
using System.Net.NetworkInformation;

using System.Configuration;


using System.Dynamic;
using System.Security.Principal;
using System.Runtime.InteropServices.ComTypes;
using System.Numerics;

#pragma warning disable 162, 168, 169, 219, 649 //unreachable code, unused var/field

[System.Security.SuppressUnmanagedCodeSecurity]
static partial class Test
{
	//Why .NET creates so many threads?
	//Even simplest app has 6 threads.
	//9 threads if there is 'static Au SOMESTRUCT _var;' and [STAThread]. Why???
	//	Not if it's a class (even if '= new Class()'). Not if it's a non-library struct. Not if it's a .NET struct.
	//	Not if app is ngened (but lib can be non-ngened).
	//		But why only if STAThread? And why only if it's a struct (and not class) of a User.dll (and not eg of this assembly)?
	//		Tested with a simplest dll, not only with Au.dll.
	//	Also can depend on other things, eg handling some exception types, using AOutput.Clear etc. Only if [STAThread].
	//	With or without [STAThread], 1 call to Task.Run makes 12 (from 6 or 9), >=2 Task.Run makes 14.
	//The above numbers (6 and 9) are on Win10. On Win7 (virtual PC) the numbers are 4 and 7. Older .NET framework version.

	//static Point s_p;
	//static SimpleLib.Struct1 s_p;

	[STAThread]
	static void Main(string[] args)
	{
		//Application.EnableVisualStyles();
		//Application.SetCompatibleTextRenderingDefault(false);

		TestMain();
	}





	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		//AOutput.IgnoreConsole = true;
		//AOutput.LogFile=@"Q:\Test\Au"+IntPtr.Size*8+".log";
		AOutput.QM2.UseQM2 = true;
		AOutput.RedirectConsoleOutput = true;
		if(!AOutput.IsWritingToConsole) {
			AOutput.Clear();
			//100.ms();
		}
		//AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

		try {
#if true
			//AThread.Start(() => { for(; ; ) { 1.s(); GC.Collect(); } });



			//TestWndGroup();
			//TestMulticastDelegate();
			//TestCastDelegate();
			//TestListForeach();
			//TestActionThread();
			//TestTaskSpeed();
			//TestQm2SendMessage();
			//TestTasks();
			//TestDelegateBeginInvoke();
			//TestDelegateInvoke();
			//TestAbortThreadAndContinue();
			//TestStackTrace();
			//TestHooks();
			//TestMillisecondsSpeed();
			//TestWndFinderCache();
			//TestFormLoadVisible();
			//TestDictionaryForTriggers();
			//TestReferenceCOM2();
			//TestOptimizeTreeViewAdv();
			//TestPopupList();
			//TestGetObjectSize();
			//Cpp.Cpp_Test();
			//TestSerializeBytes();
			//TestUacTS();
			//TestFolderSecurity();
			//TestLibSerialize();
			//TestThreadStart();
			//TestEditAndContinue();
			//TestDetectFileTextEncoding();
			//TestCollectionEmpty();
			//APerf.Cpu();
			//APerf.First();
			//TestSqlite();
			//TestSqliteExamples();
			//TestGetBoxedPointer();
			//ADialog.Show();

			//TestDB();
			//TestAssocQS();
			//TestCsvDictionary();
			//TestFileOpenWaitLocked();
			//TestAuLoadingFormsAssembly();
			//TestExceptionInInteropCallback();
#else
			try {

				//var w8 = AWnd.Find("*Firefox*", "MozillaWindowClass").OrThrow();
				////Print(w8);
				//var a = AAcc.FindAll(w8, "web:TEXT", "??*");
				////var a = AAcc.FindAll(w8, "web:");
				////var a = AAcc.FindAll(w8, "web:TEXT", "??*", flags: AFFlags.NotInProc);
				////var a = AAcc.Wait(3, w8, "web:TEXT", "Search the Web", flags: AFFlags.NotInProc);
				////var a = AAcc.Wait(3, w8, "TEXT", "Search the Web", flags: AFFlags.UIA);
				//Print(a);
				//Print("---");
				//Print(a[0].MiscFlags);
				//return;

				//var w = AWnd.Find("Java Control Panel", "SunAwtFrame").OrThrow();
				////var a = AAcc.Find(w, "push button", "Settings...").OrThrow();
				//var a = AAcc.Find(w, "push button", "Settings...", flags: AFFlags.ClientArea).OrThrow();
				//Print(a);

				TestAccForm();
				//TestAccLeaks();
				//TestWndFindContains();
				//TestAccFindWithChildFinder();
				//TestIpcWithWmCopydataAndAnonymousPipe();
				//TestAccProcessDoesNotExit3();
				//TestAccFirefoxNoSuchInterface();
				//TestAccThrowOperator();
			}
			finally {
				Cpp.Unload();
			}
#endif
		}
		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }

	}
}
