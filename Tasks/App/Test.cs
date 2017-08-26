using System;
using System.Collections.Generic;
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
//using System.Linq;
//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

using K = System.Windows.Forms.Keys;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Triggers;


//using Cat = Catkeys.Input;
//using Meow = Catkeys.Show;

//using static Catkeys.Show;

using System.Xml.Serialization;

using System.Xml;
using System.Xml.Schema;

//using Microsoft.VisualBasic.FileIO;
using System.Globalization;


//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field


public partial class Test
{
	//this would be executed if using code 'domain.CreateInstance("CatkeysTasks", "Test");'
	public unsafe Test()
	{
	}

	public static unsafe void TestInNewAppDomain()
	{
		Output.Clear();

		//AppDomain.CurrentDomain.SetData("test7", "Catkeys-");
		//Perf.First();
		//InterDomainVariables.SetVariable("-", "");
		//InterDomainVariables.Init();
		//Perf.NW();

		for(int i = 0; i < 1; i++) {
			var t = new Thread(() =>
			{
				string domainName = "Test" + i;
				AppDomain domain = null;
				try {
					domain = AppDomain.CreateDomain(domainName);

					domain.SetData("Catkeys_DefaultDomain", AppDomain.CurrentDomain);

					//AppDomainSetup domainSetup = new AppDomainSetup() {
					//	ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
					//	ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
					//	ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
					//	LoaderOptimization = LoaderOptimization.MultiDomain
					//};
					domain.ExecuteAssembly(Folders.ThisApp + @"Tests.exe", new string[] { "/domain" });

					//domain.DoCallBack(TestX);

					//domain.CreateInstance("CatkeysTasks", "Test");

				}
				catch(Exception e) {
					var s = e.ToString();
					int iSys = s.IndexOf("\r\n   at System.AppDomain._"); if(iSys > 0) s = s.Remove(iSys);
					Print($"Unhandled exception in {domainName}: {s}");
				}
				finally {
					if(domain != null) {
						try {
							//.NET bug: AppDomain.Unload randomly does not end background threads that have Application.Run etc.
							//Then would wait eg 5 s and throw CannotUnloadAppDomainException here.
							//Workaround: call Application.Exit in that domain.
							//Tested: no problems when the assembly runs as separate exe.
							//Tested: no exception if the domain also called Application.Exit; then our Application.Exit does nothing and does not raise second ApplicationExit event.
							//However when the background thread uses Util.MessageLoop, we have 2 ApplicationExit events; possible workaround: in the event handler unsubscribe the event.
							domain.DoCallBack(() => { Application.Exit(); });

							//TODO: sometimes IndexOutOfRangeException or NullReferenceException when calling Application.Exit here.
							/*
System.IndexOutOfRangeException: Index was outside the bounds of the array.
   at System.Array.InternalGetReference(Void* elemRef, Int32 rank, Int32* pIndices)
   at System.Array.SetValue(Object value, Int32 index)
   at System.Collections.Hashtable.CopyValues(Array array, Int32 arrayIndex)
   at System.Windows.Forms.Application.ThreadContext.ExitCommon(Boolean disposing)
   at System.Windows.Forms.Application.ExitInternal()
   at System.Windows.Forms.Application.Exit(CancelEventArgs e)
   at Test.<>c.<TestInNewAppDomain>b__1_1() in Q:\app\Catkeys\Tasks\App\Test.cs:line 106
   at System.AppDomain.DoCallBack(CrossAppDomainDelegate callBackDelegate)
   at Test.<>c__DisplayClass1_0.<TestInNewAppDomain>b__0() in Q:\app\Catkeys\Tasks\App\Test.cs:line 106

System.NullReferenceException: Object reference not set to an instance of an object.
   at System.Windows.Forms.Application.ThreadContext.ExitCommon(Boolean disposing)
   at System.Windows.Forms.Application.ExitInternal()
   at System.Windows.Forms.Application.Exit(CancelEventArgs e)
   at Test.<>c.<TestInNewAppDomain>b__1_1() in Q:\app\Catkeys\Tasks\App\Test.cs:line 106
   at System.AppDomain.DoCallBack(CrossAppDomainDelegate callBackDelegate)
   at Test.<>c__DisplayClass1_0.<TestInNewAppDomain>b__0() in Q:\app\Catkeys\Tasks\App\Test.cs:line 106

							*/

							AppDomain.Unload(domain);
						}
						catch(Exception e) { Print(e); }
					}
				}
			}
			);
			t.SetApartmentState(ApartmentState.STA); //must be STA, or something will not work, eg some COM components, MSAA in TaskDialog.
			t.Start();
			t.Join();
		}

		//Print(InterDomainVariables.Get2("test"));
		//TaskDialog.Show("after all");
		//Print("exit default domain");
	}

	private static void Get45or451FromRegistry()
	{
		using(RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
			int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
			if(true) {
				Print("Version: " + CheckFor45DotVersion(releaseKey));
			}
		}
	}


	// Checking the version using >= will enable forward compatibility,  
	// however you should always compile your code on newer versions of 
	// the framework to ensure your app works the same. 
	private static string CheckFor45DotVersion(int releaseKey)
	{
		if(releaseKey >= 393273) {
			return "4.6 or later";
		}
		if((releaseKey >= 379893)) {
			return "4.5.2 or later";
		}
		if((releaseKey >= 378675)) {
			return "4.5.1 or later";
		}
		if((releaseKey >= 378389)) {
			return "4.5 or later";
		}
		// This line should never execute. A non-null release key should mean 
		// that 4.5 or later is installed. 
		return "No 4.5 or later version detected";
	}

	public static unsafe void TestInThisAppDomain()
	{
		Output.Clear();

		Get45or451FromRegistry();

		//#if false
		//		Perf.First();
		//		Perf.Next();
		//		Perf.NW();
		//#else
		//		var perf = new Perf.Inst(true);
		//		perf.Next();
		//		perf.NW();
		//#endif



		//new Form1().ShowDialog();
		return;

		//To hide the 'wait' cursor when the primary thread of a new process is going to wait:
		//Wnd0.Post(0); Native.MSG k; Api.PeekMessage(out k, Wnd0, 0, 0, 1);

		//Wait(1); return;

		//TestCatBar();

		//LibWorkarounds.WaitCursorWhenShowingMenuEtc();
		//Perf.First();
		//var m = new CatMenu(); m["test"] = null; m.Show();
		//var m = new CatMenu(); m["test"] = null; m.Show(Mouse.X+10, Mouse.Y+10);
		//var m = new CatMenu(); m["test"] = null; m.Show(Mouse.X-10, Mouse.Y-10);
		//var m = new ContextMenu(); m.MenuItems.Add("test"); m.Show();
		//MessageBox(Wnd0, "txt", "cap", 0);
		//Wnd.Misc.AllowActivate();
		//Thread.Sleep(100);
		//var f = new Form();
		//f.ShowDialog();
		//Thread.Sleep(3000);
		//f.ShowDialog();

		//var w = Api.CreateWindowEx(0, "#32770", null, Api.WS_OVERLAPPEDWINDOW | Api.WS_VISIBLE, 300, 300, 300, 300, Wnd0, 0, Zero, 0);

		//TODO: test wait cursor with TaskDialog.Show
	}

	static Catkeys.Util.MessageLoop _mlTb = new Catkeys.Util.MessageLoop();

	static void TestCatBar()
	{
		//var il = _TestCreateImageList();

		Perf.First();
		var m = new CatBar();
		//m.CMS.ImageList = il;
		Perf.Next();

		m["Close"] = o => { Print(o); _mlTb.Stop(); };
		Perf.Next();
#if true
		for(int i = 0; i < 30; i++) {
			m.Add("Text", null);
		}
#else
		m.Separator();
		m["Icon", @"q:\app\Cut.ico"] = o => Print(o);
		m["Icon", @"q:\app\Copy.ico"] = o => Print(o);
		m["Icon", @"q:\app\Paste.ico"] = o => Print(o);
		m["Icon", @"q:\app\Run.ico"] = o => Print(o);
		m["Icon", @"q:\app\Tip.ico"] = o => Print(o);
		m.LastItem.ForeColor = Color.OrangeRed;
		//m["Icon resource", 1] = o => Print(o);
		m["Imagelist icon name", "k0"] = o => Print(o);
		m["Imagelist icon index", 1, tooltip:"tooltip"] = o => Print(o);
		m.Separator();
		m.Add(new ToolStripTextBox());
#endif
		Perf.Next();

		//Print(w);
		m.Visible = true;
		Perf.Next();
		_mlTb.Loop();
		m.Close();
	}
}
