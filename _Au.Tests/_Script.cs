﻿using System;
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
using System.Numerics;
using System.Globalization;
//using AutoItX3Lib;

public static class AStrEx
{
	//public static (int start, int end) Trim(this string t, int start, int end)
	//{
	//	while(start < end && char.IsWhiteSpace(t[start])) start++;
	//	while(start < end && char.IsWhiteSpace(t[end - 1])) end--;
	//	return (start, end);
	//}
	//public static (int start, int end) Trim(this string t, (int start, int end) se) => Trim(t, se.start, se.end);
}

class Script : AScript
{
	void TestDtor2()
	{
		var v = new TestDtor();
		Print(v);
	}

	static TestDtor s_dt;

	IEnumerable<(int start, int end)> TestForeachTuple()
	{
		for(int i = 0; i < 10; i += 2) yield return (i, i + 2);
	}

	//[Flags]
	//enum TestEnum { One = 1, Two = 2 }
	//static TestEnum s_te = TestEnum.One;

	//[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
	//bool _TestEnum0(TestEnum a)
	//{
	//	return (a&TestEnum.One)==TestEnum.One;
	//}

	//[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
	//bool _TestEnum1(TestEnum a)
	//{
	//	return a.HasFlag(TestEnum.One);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
	//bool _TestEnum2(TestEnum a)
	//{
	//	return a.Has(TestEnum.One);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
	//bool _TestEnum3(TestEnum a)
	//{
	//	return a.HasAny(TestEnum.One);
	//}

	class TestGC
	{
		~TestGC()
		{
			if(Environment.HasShutdownStarted) return;
			if(AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			Print("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//ATimer.After(1, () => new TestGC());
			//var f = Program.MainForm; if(!f.IsHandleCreated) return;
			//f.BeginInvoke(new Action(() => new TestGC()));
			new TestGC();
		}
	}
	static bool s_debug2;

	void _MonitorGC()
	{
		//return;
		if(!s_debug2) {
			s_debug2 = true;
			new TestGC();

			//ATimer.Every(50, () => {
			//	if(!s_debug) {
			//		s_debug = true;
			//		ATimer.After(100, () => new TestGC());
			//	}
			//});
		}
	}

	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();

		//_MonitorGC();

		//AWnd.Wait(0, false, null, "notepad", flags: WFFlags.HiddenToo);

		//List<AWnd> a = null;
		//for(; !AKeys.IsCtrl;) {
		//	10.ms();
		//	AWnd.GetWnd.AllWindows(ref a);
		//	foreach(var v in a) {
		//		_ = v.ClassName;
		//		_ = v.Name;
		//	}
		//}

		//var s = "        dddddddddddd          kkkkkkkkkkkkkkkk            ffffffffff    FIND    mmmm";
		//Print(s.IndexOf("FIND", StringComparison.Ordinal));
		//Print(s.Find("FIND"));
		//Print(s.Find("FIND", 0..));
		//Print(s.AsSpan().IndexOf("FIND", StringComparison.Ordinal));

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { s.IndexOf("FIND", StringComparison.OrdinalIgnoreCase); }
		//	//for(int i2 = 0; i2 < n2; i2++) { s.Find("FIND"); }
		//	//for(int i2 = 0; i2 < n2; i2++) { s.Find("FIND", 0..); }
		//	//for(int i2 = 0; i2 < n2; i2++) { s.AsSpan().IndexOf("FIND", StringComparison.Ordinal); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
		//"".

		//string s = "123.34";
		//Print(double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var r1), r1);
		//Print(s.ToNumber(out double r2, 0, out int end), r2, end);

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out r1); }
		//	for(int i2 = 0; i2 < n2; i2++) { s.ToNumber(out r2); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//"".ToInt();
		//"".ToNumber();
		//Print("5.4E-1".ToNumber());
		//string s = null;
		//s = " -5,6.4 ";
		//Print(s.ToNumber());
		//Print(s.ToNumber(out float f), f);
		//Print("123".ToNumber(out int i), i);
		//Print("11".ToNumber(out uint h, style: NumberStyles.HexNumber), h);
		//Print("11".ToNumber(out uint h, style: 0), h);

		//Print("123".ToNumber(out int r1), r1);
		//Print("123".ToInt(out int r2), r2);

		////var w = (AWildex)"*mid*";

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { "123".ToNumber(out r1); }
		//	//for(int i2 = 0; i2 < n2; i2++) { "123".ToInt(out r2); }
		//	//for(int i2 = 0; i2 < n2; i2++) { "0x123".ToInt(out r2); }
		//	//for(int i2 = 0; i2 < n2; i2++) { "0x12               dkjfkdjkfj 3".Like("0x* *3"); }
		//	//for(int i2 = 0; i2 < n2; i2++) { w.Match("          mid,,,,,,,,,,"); }
		//	//for(int i2 = 0; i2 < n2; i2++) { "123 dkjskdjk dhushduhs sdshhgdhs sgdgsdgs".Split(' '); }
		//	//for(int i2 = 0; i2 < n2; i2++) { "123 dkjskdjk dhushduhs sdshhgdhs sgdgsdgs".SegSplit(" "); }
		//	//for(int i2 = 0; i2 < n2; i2++) { ACsv.Parse("one, two\r\nthree,four"); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//s = (ulong.MaxValue).ToString();
		//Print(s);
		//if(s.ToNumber(out ulong n, 0, out int end)) Print(n, end);
		//Print(s.ToInt64());

		//var s = "a 123 b";
		//var k = s.AsSpan(2, 4);
		//Print(int.Parse(k));

		//var s = "one tWo THREE four";
		//s = "𐐫ne tWo THREE four";
		////var w = "three";
		////Print(s.FindWord(w, 7));

		//Print(s.Upper());
		//Print(s.Upper(SUpper.AllChars));
		//Print(s.Upper(SUpper.FirstChar));
		//Print(s.Upper(SUpper.TitleCase));

		//uint u = 100;
		//Print(u);
		//Print("list", u);
		//Print(new uint[] { u });

		//Au.Util.AStringUtil.
		//var b = new StringBuilder();
		////b.AppendJoin(", ", new uint[] { 1, 10, 100 });
		////b.AppendJoin(", ", new string[] { "zero", null, "two" });
		//b.AppendJoin(", ", new string[] { null, "one", "two" });
		////b.AppendJoin(", ", new object[] { null, "one", "two" });
		////Print(b);

		//var a = new string[] { null, "one", "two" };
		////var a = new int?[] { null, 10, 20 };
		////Print(Print1(a));
		////Print("--");
		////Print(Print2(a));
		////Print("--");

		////Print(a);
		//Print("array:", a, "tail");
		////Print("--");
		////ADebug.Print(a);

		////APerf.SpeedUpCpu();
		////for(int i1 = 0; i1 < 10; i1++) {
		////	int n2 = 1000;
		////	APerf.First();
		////	for(int i2 = 0; i2 < n2; i2++) { _=Print2(a); }
		////	APerf.NW();
		////	Thread.Sleep(200);
		////}


		//TestEnum a = s_te;

		//Print(_TestEnum0(a));
		//Print(_TestEnum1(a));
		//Print(_TestEnum2(a));
		//Print(_TestEnum3(a));
		////return;

		////bool r = a.Has3(b);

		//int n = 0;
		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 10000;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { if(_TestEnum0(a)) n++; }
		//	//for(int i2 = 0; i2 < n2; i2++) { if(_TestEnum1(a)) n++; }
		//	for(int i2 = 0; i2 < n2; i2++) { if(_TestEnum2(a)) n++; }
		//	//for(int i2 = 0; i2 < n2; i2++) { if(_TestEnum3(a)) n++; }
		//	//for(int i2 = 0; i2 < n2; i2++) {
		//	//	if(a.Has3(TestEnum.One)) n++;
		//	//}
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
		//Print(n);


		//string s = ",asd;";
		////s = "asd";
		//Print(s.Trim(new char[] { ',', ';' }));
		//Print(s.Trim(",;"));
		//Print(s.TrimStart(",;"));
		//Print(s.TrimEnd(",;"));

		//int n = 0;
		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 5000;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { n += s.Trim(new char[] { ',', ';' }).Length; }
		//	for(int i2 = 0; i2 < n2; i2++) { n += s.Trim(",;").Length; }
		//	//for(int i2 = 0; i2 < n2; i2++) { n += s.TrimStart(",;").Length; }
		//	//for(int i2 = 0; i2 < n2; i2++) { n += s.TrimEnd(",;").Length; }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
		//Print(n);


		//		int start = 1;
		//#if true
		//		string s1 = "asdfghjklzxcvbnm\\rtyuiopasdfghjkl";
		//		string s2 = "\\/";
		//		//string s2 = "\\/,;.:";
		//		bool not = false;
		//#else
		//		string s1 = " \t \t \t \t \t \t \t \tqwertyuiopasdfghjkl";
		//		//string s1 = "                qwertyuiopasdfghjkl";
		//		//string s1 = "qwertyuiopasdfghjkl \t \t \t \t \t \t \t \t";
		//		string s2 = " \t";
		//		bool not = true;
		//#endif
		//		Print(not ? s1.FindNot(s2) : s1.FindAny(s2));
		//		Print(not ? s1.FindLastNot(s2) : s1.FindLastAny(s2));

		//		int n = 0;
		//		APerf.SpeedUpCpu();
		//		for(int i1 = 0; i1 < 10; i1++) {
		//			int n2 = 5000;
		//			APerf.First();
		//			//for(int i2 = 0; i2 < n2; i2++) { n+=s1.FindCharsOld(s2, not: not); }
		//			for(int i2 = 0; i2 < n2; i2++) { n += not ? s1.FindNot(s2) : s1.FindAny(s2); }
		//			//for(int i2 = 0; i2 < n2; i2++) { n += not ? s1.FindLastNot(s2) : s1.FindLastAny(s2); }
		//			APerf.NW();
		//			Thread.Sleep(200);
		//		}
		//		Print(n);


		//string s1 = "asdfghjklzxcvbnm";
		//string s2 = "asdf";
		//bool ignoreCase = false;
		//Print(s1.Starts(s2, ignoreCase));
		//Print(s1.Starts2(s2, ignoreCase));

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 10000;
		//	for(int i2 = 0; i2 < n2; i2++) { s1.Starts(s2, ignoreCase); }
		//	for(int i2 = 0; i2 < n2; i2++) { s1.Starts2(s2, ignoreCase); }
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { s1.Starts(s2, ignoreCase); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { s1.Starts2(s2, ignoreCase); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//var s = "one two three";
		//Print(s.ReplaceAt(4, 3, "KNURE"));
		//Print(s.ReplaceAt2(4, 3, "KNURE"));

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = s.ReplaceAt(4, 3, "KNURE"); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = s.ReplaceAt2(4, 3, "KNURE"); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//for(uint i = 0x10000; i < 0x20000; i++) {
		//	Rune r = (Rune)i;
		//	var u = Rune.ToUpperInvariant(r);
		//	if(u != r) Print(i, r, u);
		//}

		//var s=string.Concat()

		//string s = "asdfghjklqwertyuiop";
		////string s = "𐐫sdfghjklqwertyuiop";
		//Print(s.Upper(true));
		////Print(s.Upper2(true));
		////Print(s.Upper3(true));
		////Print(s.Upper5(true));

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = s.Upper(true); }
		//	//APerf.Next();
		//	////for(int i2 = 0; i2 < n2; i2++) { _ = s.Upper2(true); }
		//	////APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { _ = s.Upper3(true); }
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { _ = s.Upper5(true); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//var s = "one, two ,three , four";
		////foreach(var v in s.Segments(",")) {
		////	//var k = v;
		////	//var k = s.Trim(v.start, v.end);
		////	var k = s.Trim(v);
		////	Print($"'{s[k.start..k.end]}'");
		////}
		//foreach(var v in s.Segments(",")) {
		//	var g = s.AsSpan(v).Trim();
		//	Print($"'{g.ToString()}'");
		//}

		//List<int>.Enumerator e; e.
		//SegParser k; k.


		//TODO: find all Substring and Remove. Maybe can do with Span.
		//TODO: use string.Create, eg in Upper.
		//TEST: System.Buffers.Text.Base64, System.Buffers.Text.Utf8Parser and System.Buffers.Text.Utf8Formatter.
		//USE: Span<byte> bytes = length <= 128 ? stackalloc byte[length] : new byte[length];

		//var b = new StringBuilder();
		//b.Append()

		//string s = "qwertyuiop";
		////s.Split()

		//foreach(var v in TestForeachTuple()) {
		//	//v.start++;
		//	Print(v, s[v.start..v.end]);
		//	//var vv = (v.start + 1, v.end);
		//	var (start, end) = v;
		//	var k = v; k.start++;
		//	s.TrimRange(ref k.start, ref k.end);
		//}
		//s.Segments("");
		//ReadOnlyMemory<char> m; m.TrimEnd()

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

		//var s = "one, two, three, four.";
		//Print(s.RegexFindAll(@"\w+", 0, 3..11));

		//foreach(var v in s.Segments(" ,", SegFlags.NoEmpty, 5..^5)) {
		//	Print(v);
		//}


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
