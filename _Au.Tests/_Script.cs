using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Au;
using Au.Types;
using static Au.AStatic;
using Au.Triggers;
using Au.Controls;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime;
using Microsoft.Win32;
using System.Runtime.InteropServices.ComTypes;

class Script : AScript
{
	void TestDtor2()
	{
		var v = new TestDtor();
		Print(v);
	}

	static TestDtor s_dt;

	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		//AOutput.QM2.UseQM2 = true;
		//AOutput.Clear();


		//TestDtor2();
		//GC.Collect();
		//1500.ms();

		//ITypeLib tl = null;

		//tl.GetLibAttr(out IntPtr ipta);
		//var ta = Marshal.PtrToStructure<System.Runtime.InteropServices.ComTypes.TYPELIBATTR>(ipta);
		//tl.ReleaseTLibAttr(ipta);
		//var hash = Au.Util.AHash.Fnv1(ta).ToString("x");

		//Thread.Sleep(30);

		//var x = new AutoItX3();
		//x.Init();
		//x.ClipPut("hhhh");
		//Print(x.IsAdmin());
		//Print(x.Run(@"c:\windows\system32\notepad.exe", "", 3));
		//x.Send("abc{HOME}");

		//var e = new Excel.Application();
		////Print(e);
		//e.Visible = true;
		//ADialog.Show();
		//e.Quit();

		//Excel.Application xlApp = new Excel.Application();
		//Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\G\Documents\Book1.xls");
		//Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
		//Excel.Range xlRange = xlWorksheet.UsedRange;

		//ADialog.Show("");

		//MessageBox(default, "", "", 0);

		//var s = "abc.def.";
		//Print(s.LastIndexOf('.', s.Length-2));



		//Print(SimpleLib.Class1.Test);

		//Print(Api.GetOEMCP());
		//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		//var e = Encoding.GetEncoding(Api.GetOEMCP());
		//Print(e.EncodingName);

		//AExec.RunConsole(@"C:\Program Files\dotnet\dotnet.exe", "--info");

		//Print("test");

		//test: native exceptions such as access violation.

		//200.ms();
		//int i = 0;
		//foreach(var f in AFile.EnumDirectory(@"Q:\Test\ok\.compiled")) {
		//	if(f.IsDirectory || f.Size>6_000) continue;
		//	Print(f.Name);
		//	APerf.First();
		//	//var a = File.ReadAllBytes(f.FullPath);
		//	//using var fs = File.OpenRead(f.FullPath);
		//	using var fs = OpenRead(f.FullPath);
		//	APerf.Next();
		//	var a = new byte[fs.Length];
		//	fs.Read(a, 0, a.Length);
		//	APerf.NW();
		//	if(++i > 7) break;
		//}

		//Print(args);

		//string s = "one two three";
		////var r = s[4..7];
		////Print(r);
		//int i = 4, j = 7;
		//var ii = ^2;
		//var r = s[i..ii];
		//Print(r);
		//int i = 5;
		//Print(TestRange(s));

		//Span<char> k= new Span<char>(

	}

	[DllImport("user32.dll", EntryPoint = "MessageBoxW")]
	internal static extern int MessageBox(AWnd hWnd, string lpText, string lpCaption, uint uType);

	//FileStream OpenRead(string file)
	//{
	//	var h = Api.CreateFile(file, Api.GENERIC_READ, Api.FILE_SHARE_ALL, default, Api.OPEN_EXISTING);
	//	return new FileStream(h, FileAccess.Read, ownsHandle: true);
	//}

	string TestRange(string s, Range? range = null)
	{
		var r = range ?? Range.All;
		int i = r.Start.Value;
		Print(r, r.Start, r.End, i);
		return s[r];
	}

	class TestDtor :IDisposable
	{
		public TestDtor()
		{
			AProcess.Exit += AProcess_Exit;
		}

		private void AProcess_Exit(object sender, EventArgs e)
		{
			Dispose();
		}

		~TestDtor()
		{
			AOutput.QM2.Write("dtor");

		}

		public void Dispose()
		{
			AOutput.QM2.Write("Dispose");
			
		}
	}
}
