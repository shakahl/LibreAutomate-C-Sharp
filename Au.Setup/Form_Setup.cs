//#define WEB_SETUP

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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
//using System.Configuration;
using System.Xml.Linq;
using System.Runtime;

using Au;
using Au.Types;
using static Au.NoClass;

public partial class Form_Setup :Form
{
	string _dir;

	public Form_Setup()
	{
		InitializeComponent();

#if WEB_SETUP
		var f = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		var auDll = f + @"\Au.dll";
		if(!File.Exists(auDll)) {
			using(var web = new System.Net.WebClient() { BaseAddress = "http://www.quickmacros.com/com/" }) {
				web.DownloadFile("Au.dll", auDll);
				//and maybe more
			}
		}
		//_TODO: later download Au.Setup.zip (program files) with UI/progress. Unzip directly into the program folder.
#endif

		_TestInit();
	}

	void _TestInit()
	{
		//_destDir = @"Q:\app\au\_\";
		_dir = Folders.ThisAppBS;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		Au.Util.LibTaskScheduler.CreateTaskToRunProgramOnDemand("Au", "Au.Editor", UacIL.System, _dir + "Au.CL.exe", "/s $(Arg0)");

		Close();
	}

	//protected override void OnClick(EventArgs e)
	//{
	//	base.OnClick(e);

	//	ADialog.Show("Au");
	//}
}
