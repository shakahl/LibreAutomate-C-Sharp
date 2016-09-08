using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
//using System.Linq;

using System.IO;
//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;
using K = System.Windows.Forms.Keys;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Triggers;


//using Cat = Catkeys.Automation.Input;
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
		//InterDomain.SetVariable("-", "");
		//InterDomain.Init();
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
					domain.ExecuteAssembly(Folders.App + @"Tests.exe", new string[] { "/domain" });

					//domain.DoCallBack(TestX);

					//domain.CreateInstance("CatkeysTasks", "Test");

				}
				catch(Exception e) {
					var s = e.ToString();
					int iSys = s.IndexOf("\r\n   at System.AppDomain._"); if(iSys > 0) s = s.Remove(iSys);
					Out($"Unhandled exception in {domainName}: {s}");
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
						catch(Exception e) { Out(e); }
					}
				}
			}
			);
			t.SetApartmentState(ApartmentState.STA); //must be STA, or something will not work, eg some COM components, MSAA in TaskDialog.
			t.Start();
			t.Join();
		}

		//Out(InterDomain.Get2("test"));
		//Show.TaskDialog("after all");
		//Out("exit default domain");
	}

	//[DllImport("user32.dll")]
	//public static extern int ShowCursor(bool bShow);
	//[DllImport("user32.dll")]
	//public static extern IntPtr SetCursor(IntPtr hCursor);
	//[DllImport("kernel32.dll")]
	//public static extern void Sleep(uint dwMilliseconds);
	[DllImport("user32.dll", EntryPoint = "MessageBoxW")]
	public static extern int MessageBox(Wnd hWnd, string lpText, string lpCaption, uint uType);

	class Form1 :Form
	{
		public Form1()
		{
			var m = new ContextMenuStrip();
			m.Items.Add("Close");
			var dd1 = new ToolStripDropDownMenu(); (m.Items.Add("Sub 1") as ToolStripMenuItem).DropDown = dd1;
			{
				dd1.Items.Add("Aaaaaaaaaaaaaa");
				var dd2 = new ToolStripDropDownMenu();
				var k = new ToolStripMenuItem_(); k.Text = "Sub 2";
				k.DropDown = dd2;
				dd1.Items.Add(k);
				{
					dd2.Items.Add("Zzzzzzzzzzzz");
				}
			}
			m.Items.Add("Last");

			ContextMenuStrip = m;
		}

		class ToolStripMenuItem_ :ToolStripMenuItem
		{
			protected override void OnMouseHover(EventArgs e)
			{
				Out("hover 1");
				base.OnMouseHover(e);
				Out("hover 2, dd visible=" + this.DropDown.Visible);
				Perf.First();
				bool y = this.HasDropDownItems && !this.DropDown.Visible;
				Perf.NW();
				Out(y);
				if(this.HasDropDownItems && !this.DropDown.Visible) this.ShowDropDown();
				//Perf.First();
			}

			protected override void OnDropDownShow(EventArgs e)
			{
				//Perf.NW();
				Out("show");
				base.OnDropDownShow(e);
			}

			//protected override void OnDropDownOpened(EventArgs e)
			//{
			//	Out("opened 1");
			//	base.OnDropDownShow(e);
			//	Out("opened 2");
			//}
		}
	}

	public static unsafe void TestInThisAppDomain()
	{
		Output.Clear();

#if false
		Perf.First();
		Perf.Next();
		Perf.NW();
#else
		var perf = new Perf.Inst(true);
		perf.Next();
		perf.NW();
#endif



		//new Form1().ShowDialog();
		return;

		//To hide the 'wait' cursor when the primary thread of a new process is going to wait:
		//Wnd0.Post(0); Api.MSG k; Api.PeekMessage(out k, Wnd0, 0, 0, 1);

		//WaitMS(1000); return;

		//TestCatBar();

		//LibWorkarounds.WaitCursorWhenShowingMenuEtc();
		//Perf.First();
		//var m = new CatMenu(); m["test"] = null; m.Show();
		//var m = new CatMenu(); m["test"] = null; m.Show(Mouse.X+10, Mouse.Y+10);
		//var m = new CatMenu(); m["test"] = null; m.Show(Mouse.X-10, Mouse.Y-10);
		//var m = new ContextMenu(); m.MenuItems.Add("test"); m.Show();
		//MessageBox(Wnd0, "txt", "cap", 0);
		//Wnd.Misc.AllowActivate();
		//WaitMS(100);
		//var f = new Form();
		//f.ShowDialog();
		//WaitMS(3000);
		//f.ShowDialog();

		//var w = Api.CreateWindowEx(0, "#32770", null, Api.WS_OVERLAPPEDWINDOW | Api.WS_VISIBLE, 300, 300, 300, 300, Wnd0, 0, Zero, 0);

		//TODO: test wait cursor with Show.TaskDialog
	}

	static Util.MessageLoop _mlTb = new Util.MessageLoop();

	static void TestCatBar()
	{
		//var il = _TestCreateImageList();

		Perf.First();
		var m = new CatBar();
		//m.CMS.ImageList = il;
		Perf.Next();

		m["Close"] = o => { Out(o); _mlTb.Stop(); };
		Perf.Next();
#if true
		for(int i = 0; i < 30; i++) {
			m.Add("Text", null);
		}
#else
		m.Separator();
		m["Icon", @"q:\app\Cut.ico"] = o => Out(o);
		m["Icon", @"q:\app\Copy.ico"] = o => Out(o);
		m["Icon", @"q:\app\Paste.ico"] = o => Out(o);
		m["Icon", @"q:\app\Run.ico"] = o => Out(o);
		m["Icon", @"q:\app\Tip.ico"] = o => Out(o);
		m.LastItem.ForeColor = Color.OrangeRed;
		//m["Icon resource", 1] = o => Out(o);
		m["Imagelist icon name", "k0"] = o => Out(o);
		m["Imagelist icon index", 1, tooltip:"tooltip"] = o => Out(o);
		m.Separator();
		m.Add(new ToolStripTextBox());
#endif
		Perf.Next();

		//Out(w);
		m.Visible = true;
		Perf.Next();
		_mlTb.Loop();
		m.Close();
	}
}
