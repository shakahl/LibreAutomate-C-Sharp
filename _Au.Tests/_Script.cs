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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

[module: DefaultCharSet(CharSet.Unicode)]

class Script : AScript
{

	class TestGC
	{
		~TestGC()
		{
			if(Environment.HasShutdownStarted) return;
			if(AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			Print("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//ATimer.After(1, _ => new TestGC());
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

			//ATimer.Every(50, _ => {
			//	if(!s_debug) {
			//		s_debug = true;
			//		ATimer.After(100, _ => new TestGC());
			//	}
			//});
		}
	}

	//unsafe class MapArray
	//{
	//	public int[] _a;
	//	public Vector128<int>[] _v;

	//	public MapArray(int n)
	//	{
	//		_a = new int[n];
	//		for(int i = 0; i < _a.Length; i++) _a[i] = i;

	//		_v = new Vector128<int>[n];
	//	}

	//	public void Move(int i)
	//	{
	//		int n = _a.Length - i - 1;
	//		Array.Copy(_a, i, _a, i + 1, n);

	//		//fixed(int* p = _a) Api.memmove(p + i + 1, p + 1, n * 4); //same speed
	//	}

	//	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
	//	public void Inc(int i, int add)
	//	{
	//		//for(; i < _a.Length; i++) _a[i]+=add;

	//		long add2 = add; add2 = add2 << 32 | add2;
	//		fixed(int* ip = _a) {
	//			var p = (long*)ip;
	//			for(int n = _a.Length / 2; i < n; i++) {
	//				//var v = p[i];
	//				p[i] += add2;
	//			}

	//		}

	//		//var va = Vector128.Create(add);
	//		//for(;  i < _v.Length; i++) {
	//		//	_v[i]=Sse2.Add(_v[i], va);
	//		//}
	//	}

	//	//public void Insert(int i, int add)
	//	//{
	//	//	for(; i < _a.Length; i++) _a[i]+=add;
	//	//}

	//	public void PrintVector()
	//	{
	//		Print(_a);

	//		//for(int i=0; i < _v.Length; i++) {
	//		//	Print(_v[i].GetElement(0), _v[i].GetElement(1), _v[i].GetElement(2), _v[i].GetElement(3));
	//		//}
	//	}
	//}

	class JSettings
	{
		public string OneTwo { get; set; }
		public int ThreeFour { get; set; }
		public int Five { get; set; }
		public bool Six { get; set; }
		public string Seven { get; set; }
		public string Eight { get; set; } = "def";
	}

	void TestJson()
	{
		var file = @"Q:\test\sett.json";
		var file2 = @"Q:\test\sett.xml";

		var v = new JSettings { OneTwo = "text ąčę", ThreeFour = 100 };

		for(int i = 0; i < 5; i++) {
			//100.ms();
			//APerf.First();
			//var k1 = new JsonSerializerOptions { IgnoreNullValues = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };
			//var b1 = JsonSerializer.SerializeToUtf8Bytes(v, k1);
			//APerf.Next();
			//File.WriteAllBytes(file, b1);
			//APerf.NW();

			100.ms();
			APerf.First();
			var b2 = File.ReadAllBytes(file);
			APerf.Next();
			var k2 = new JsonSerializerOptions { IgnoreNullValues = true };
			APerf.Next();
			v = JsonSerializer.Deserialize<JSettings>(b2, k2);
			APerf.NW('J');
		}

		for(int i = 0; i < 5; i++) {
			//100.ms();
			//APerf.First();
			//var r1 = new XElement("r");
			//r1.Add(new XElement("OneTwo", v.OneTwo));
			//r1.Add(new XElement("ThreeFour", v.ThreeFour.ToString()));
			//APerf.Next();
			//r1.Save(file2);
			//APerf.NW();

			100.ms();
			APerf.First();
			var r2 = XElement.Load(file2);
			APerf.Next();
			v = new JSettings();
			v.OneTwo = r2.Element("OneTwo").Value;
			var s2 = r2.Element("ThreeFour").Value;
			APerf.NW('X');
			v.ThreeFour = s2.ToInt();
		}

		Print(v.OneTwo, v.ThreeFour, v.Five, v.Six, v.Seven, v.Eight);

		//JsonDocument d; d.RootElement.
	}

	[DllImport("CppE")]
	static extern int Cpp_Install(int step, string dir);

	[DllImport("CppE")]
	static extern int Cpp_Uninstall();

	unsafe void _Main()
	{
		//Print((uint)Cpp_Install(2, AFolders.ThisAppBS));
		//Print((uint)Cpp_Uninstall());

		//ADialog.Show("script");
		
	}

	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();
		Au.Util.LibAssertListener.Setup();

		try {
			_Main();
		}
		catch(Exception ex) { Print(ex); }
	}
}
