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
using System.Numerics;
using System.Globalization;
//using AutoItX3Lib;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

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

	//[ComImport, Guid("3AB5235E-2768-47A2-909A-B5852A9D1868"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	//interface IInterface
	//{
	//	[PreserveSig] int Add(int a, int b);
	//}

	//[DllImport("AuCpp.dll")]
	//static extern IInterface Cpp_GetInterface();


	//static byte[] ReadFile(string file)
	//{
	//	var h = Api.CreateFile(file, Api.GENERIC_READ, Api.FILE_SHARE_ALL, default, Api.OPEN_EXISTING);
	//	if(h.Is0) throw new AuException(0);
	//	using var fs = new FileStream(h, FileAccess.Read, true);
	//	var r = new byte[fs.Length];
	//	fs.Read(r, 0, r.Length);
	//	return r;
	//}


	//static unsafe byte[] ReadFile2(string file)
	//{
	//	using var h = Api.CreateFile(file, Api.GENERIC_READ, Api.FILE_SHARE_ALL, default, Api.OPEN_EXISTING);
	//	if(h.Is0) throw new AuException(0);
	//	Api.GetFileInformationByHandle(h, out var k);
	//	var r = new byte[k.FileSize];
	//	Api.ReadFileArr(h, r, out var nr);
	//	return r;
	//}

	//static unsafe string ReadCodeFile(string file)
	//{
	//	using var h = Api.CreateFile(file, Api.GENERIC_READ, Api.FILE_SHARE_ALL, default, Api.OPEN_EXISTING);
	//	if(h.Is0) throw new AuException(0);
	//	Api.GetFileInformationByHandle(h, out var k);
	//	int len8 = (int)k.FileSize;
	//	var b = (byte*)Au.Util.AMemory.Alloc(len8);
	//	try {
	//		Api.ReadFile(h, b, len8, out int nr);
	//		var len16 = Encoding.UTF8.GetCharCount(b, len8);
	//		LPARAM t = b;
	//		return string.Create(len16, (t, len8), (span, a) => Encoding.UTF8.GetChars(new ReadOnlySpan<byte>(a.t, a.len8), span));
	//	}
	//	finally { Au.Util.AMemory.Free(b); }
	//}

	//static void _ArrMove(int)

	unsafe class MapArray
	{
		public int[] _a;
		public Vector128<int>[] _v;

		public MapArray(int n)
		{
			_a = new int[n];
			for(int i = 0; i < _a.Length; i++) _a[i] = i;

			_v = new Vector128<int>[n];
		}

		public void Move(int i)
		{
			int n = _a.Length - i - 1;
			Array.Copy(_a, i, _a, i+1, n);

			//fixed(int* p = _a) Api.memmove(p + i + 1, p + 1, n * 4); //same speed
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
		public void Inc(int i, int add)
		{
			//for(; i < _a.Length; i++) _a[i]+=add;

			long add2 = add; add2= add2 << 32 | add2;
			fixed(int* ip = _a) {
				var p = (long*)ip;
				for(int n=_a.Length / 2; i < n; i++) {
					//var v = p[i];
					p[i] += add2;
				}

			}

			//var va = Vector128.Create(add);
			//for(;  i < _v.Length; i++) {
			//	_v[i]=Sse2.Add(_v[i], va);
			//}
		}

		//public void Insert(int i, int add)
		//{
		//	for(; i < _a.Length; i++) _a[i]+=add;
		//}

		public void PrintVector()
		{
			Print(_a);

			//for(int i=0; i < _v.Length; i++) {
			//	Print(_v[i].GetElement(0), _v[i].GetElement(1), _v[i].GetElement(2), _v[i].GetElement(3));
			//}
		}
	}

	static unsafe int _SkipAscii(byte* bp, int i, int len)
	{
#if true
		for(; i < len && (i & 7) != 0; i++) if(bp[i] >= 0x80) return i;
		var up = (ulong*)(bp + i);
		int j = 0;
		for(int n = (len - i) / 8; j < n; j++) if((up[j] & 0x8080808080808080) != 0) break;
		for(i += j * 8; i < len; i++) if(bp[i] >= 0x80) break;
#else
			for(; i < len; i++) if(bp[i] >= 0x80) break;
#endif
		return i;
	}

	unsafe void _Main()
	{
		var s = "123456789 123456789ččč123456789 123456789ččč";
		var b = Encoding.UTF8.GetBytes(s);
		fixed(byte* p = b) {
			Print(_SkipAscii(p, 25, b.Length));
		}




		//var a = new int[1_000_000];

		//var k = new MapArray(6);
		//k.PrintVector();
		//Print("---");
		//k.Move(1);
		//k.PrintVector();
		//Print("---");
		//k.Inc(1, 10);
		//k.PrintVector();


		////return;

		//k = new MapArray(1_000_000);

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { k.Move(1); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { k.Inc(1, 10); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//var file = @"Q:\Test\ok\files\util\AAll.cs";

		//var b = File.ReadAllBytes(file);
		//var s = File.ReadAllText(file);
		//var r = ReadFile(file);
		//var r2 = ReadFile2(file);
		//var s2 = ReadCodeFile(file);
		////Print(s2); return;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 20; i1++) {
		//	int n2 = 1;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { File.ReadAllBytes(file); }
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { File.ReadAllText(file); }
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { ReadFile(file); }
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { ReadFile2(file); }
		//	//APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { ReadCodeFile(file); }
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { File.ReadAllBytes(file); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
	}

	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();

		try {
			_Main();
		}catch(Exception ex) { Print(ex); }

		return;

//#if true
//		System.Console.Write("d", 200, FAFlags.UseRawPath);

		//		Paste("str\r\ning");
		//#endif

		//Print(Encoding.UTF8.EncoderFallback);

		//var chars = "as".AsSpan();
		//var enc = Encoding.UTF8;
		//int n = enc.GetByteCount(chars);
		//var r = new byte[100];
		//int nn = Encoding.UTF8.GetBytes(chars, r);


		//var s = "aą";
		////s = null;
		//Print(Au.Util.AConvert.ToUtf8(s, "\r\n\0"));
		//Print(Au.Util.AConvert.ToUtf8_2(s));

		//var s = "twgbjsfjhfkljdklfjkl djf ljdlsjf;  jgkjgif fijgdifg (fdkjfkdj)f jkjk jdkjkjkj hhgfdhgfdgfgduhfudhuhfud sidiosutoiusioutiosuiru utrisutirtiusiuuisuriut ouosuituis ";

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { Au.Util.AConvert.ToUtf8(s); }
		//	//for(int i2 = 0; i2 < n2; i2++) { Au.Util.AConvert.Utf8FromString(s); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//Encoding.UTF8.GetByteCount()
		//Encoding.UTF8.GetBytes()

		//byte[] a = { 1, 10, 255, 220, 4, 0, 255, 170, 120, 7 };
		//var s = Au.Util.AConvert.HexEncode(a, true);
		//Print(s, Au.Util.AConvert.HexDecode(s));

		//POINT p = (10, 200);
		//var s = Au.Util.AConvert.HexEncode(p);
		//Print(s, Au.Util.AConvert.HexDecode(s, out POINT pp), pp);

		//using(var ms=new MemoryStream()) {
		//	ms.WriteByte(5);
		//	var a = ms.GetBuffer();
		//	Print(a);
		//}

		//var a = new byte[100];
		//Print(Au.Util.AConvert.Base64UrlDecode((string)null, new Span<byte>(a), out int len), len);

		//byte[] a = { 1, 10, 255, 220, 4, 0, 255, 170, 120, 7 };
		//var s = Au.Util.AConvert.Base64UrlEncode(a);
		//var b = Au.Util.AConvert.Base64UrlDecode(s);
		//Print(s, b);

		//POINT p = (100, 200);
		////RECT p = (100, 200, 300, 400);
		//var s = Au.Util.AConvert.Base64UrlEncode(p);
		////s = s.RemoveSuffix(4);
		//Print(s);
		//if(!Au.Util.AConvert.Base64UrlDecode(s, out POINT pp)) { Print("failed"); return; }
		//Print(pp);

		//unsafe {
		//	POINT p = (100, 200);
		//	var s = Au.Util.AConvert.Base64UrlEncode(&p, sizeof(POINT));
		//	Print(s);
		//	POINT pp;
		//	//if(!Au.Util.AConvert.Base64UrlDecode(s, &pp, sizeof(POINT), out int n)) { Print("failed"); return; }
		//	if(!Au.Util.AConvert.Base64UrlDecode(s, new Span<byte>(&pp, sizeof(POINT)), out int n)) { Print("failed"); return; }
		//	Print(pp, n);
		//}

		//unsafe {
		//	for(int i = 1; i < 10; i++) {
		//		var a = new byte[i];
		//		for(int j = 0; j < i; j++) a[j] = (byte)(j+220);
		//		var s = Convert.ToBase64String(a);
		//		//Print(s.Length, (i+2)/3*4, s);
		//		Print(i);
		//		Print(s);
		//		fixed(byte* p = a) {
		//			s= Au.Util.AConvert.Base64UrlEncode(p, a.Length);
		//		}
		//		Print(s);
		//	}
		//}

		//byte[] a = { 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, };

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { Convert.ToBase64String(a); }
		//	for(int i2 = 0; i2 < n2; i2++) { Convert.ToBase64String(a); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}



		//ReadOnlySpan<char> k = default;
		//Print(k.IsEmpty, k.Length);
		//k = "".AsSpan();
		//Print(k.IsEmpty, k.Length);

		//for(int i = 0; i < 12; i++) {
		//	var a = new byte[i];
		//	for(int j = 0; j < i; j++) a[j] = (byte)(j + 220);
		//	var s = Convert.ToBase64String(a);
		//	Print(a.Length, s.Length, Au.Util.AConvert.Base64UrlEncodeLength(a.Length), Au.Util.AConvert.Base64UrlDecodeLength(s));
		//}

		//byte[] a = { 1 };
		//var s = Convert.ToBase64String(a);
		////s = s.Replace('/', '_');
		////s = s.TrimEnd("=");
		//Print(s);
		//a = Convert.FromBase64String(s);

		//var b = new byte[s.Length * 3 / 4];
		//Print(Convert.TryFromBase64String(s, b, out int n), n, b);


		////byte[] a = { 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, 1, 10, 255, };
		//byte[] a = { 1, 10, 255 };
		//string s = Au.Util.AConvert.HexEncode(a);
		//Print(s);
		////return;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { Au.Util.AConvert.HexEncode(a); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

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

		void Goo() { }

		int Goo2() => 4;

		public void Dispose()
		{
			AOutput.QM2.Write("Dispose");

		}
	}


}
