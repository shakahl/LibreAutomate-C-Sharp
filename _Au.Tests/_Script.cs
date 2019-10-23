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
//using System.Linq;
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
//using AutoItX3Lib;

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
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();

		//var s = "jhsdalkhgkjdhgjkhdjgkhdfjkghldkjhgldhgjkhdgjkg";

		//s.

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = s.ToCharArray(); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = s.AsSpan(); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = s.AsMemory(); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//var a = "abcd".ToCharArray();
		//var v = a.AsSpan(2);
		////v.Clear();
		////v[0] = 'K';
		////Print(v.Contains('a'), v.Contains('d'));
		//v.
		//Print(a);

		unsafe {
			//var s = "ab  cd";
			//var g = s.AsSpan(2);
			//Print(g[0]);
			//g[0] = 'K';
			//Print(g.IndexOf('d'));
			//Print(g.SequenceEqual("cd"));
			//Print(g.TrimStart(" ").ToString());
			//var m = s.AsMemory(2);
			//using(var h = m.Pin()) {
			//	//Print(*(char*)h.Pointer);
			//	*(char*)h.Pointer = 'K';
			//}
			//Print(s);

			//var r = s.AsSpan(2);

			//s.FindChars
		}

		//Print(~1);

		//var r = 5..^3;
		//Print(r.GetOffsetAndLength(10));

		//ARange r = default;
		//Print(r.GetRealRange(10));
		//Print(r);

		////r = ^3;
		//r = 2..3;
		//Print(r.GetRealRange(10));
		//Print(r);
		//r = 2..^3;
		//Print(r.GetRealRange(10));
		//Print(r);
		//r = ^6..^3;
		//Print(r.GetRealRange(10));
		//Print(r);
		//r = ..;
		//Print(r.GetRealRange(10));
		//Print(r);
		//r = 4..;
		//Print(r.GetRealRange(10));
		//Print(r);
		//r = ..7;
		//Print(r.GetRealRange(10));
		//Print(r);
		//r = ..^7;
		//Print(r.GetRealRange(10));
		//Print(r);

		//Range? r = 1..;

		//var m = "abcd".AsMemory(2);

		var s = "one, two, three, four.";
		Print(s.RegexFindAll(@"\w+", 0, 3..11));

		foreach(var v in s.Segments(" ")) {
			//Print(v[v.EndOffset])
		}



		//Range rr = default;
		//Print(Range.All);

		//Nee(..);
		//void Nee(Range k) { }

		//var s = "ab cd";
		//foreach(var v in s.EnumerateRunes()) Print(v., Rune.);

		//unsafe {
		//	Print(sizeof(Span<int>));
		//}

		//foreach(var i in 0..3) {

		//}

		//int[] a = { 3, 4, 5 };
		//List<int> a = { 3, 4, 5 };
		//Print(a[1..]);

		//var m= a.AsMemory(1);
		//m.

		//a.CopyTo()
		//"".Substring(1);
		//var s = "*do it";
		//Print("Failed to " + s.Substring(1));
		//Print("Failed to " + s[1..]);


		//switch(1) {
		//case 0..3:

		//	break;
		//}

		//Range r = 1..;

		//unsafe {
		//	Print(sizeof(Range));
		//}

		//var s = "one.two";
		//Print(s.RemoveSuffix(3));
		//Print(s[..^3]);


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

	class TestDtor : IDisposable
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
