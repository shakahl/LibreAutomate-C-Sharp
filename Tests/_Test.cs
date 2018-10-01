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
//using Microsoft.Win32;
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

using Au;
using static Au.NoClass;

//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;

using Microsoft.VisualBasic.FileIO;
using System.Globalization;

//for LikeEx_
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using VB = Microsoft.VisualBasic.FileIO;

//using ImapX;

//using System.Data.SQLite; //very slow (loads many assemblies, even Forms) and much work to use
using SQLite; //SQLite-net

//using CsvHelper;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

using Microsoft.Win32.SafeHandles;

using System.IO.Compression;
using System.Reflection.Emit;
using System.Net;
using System.Net.NetworkInformation;

using System.Configuration;

using Au.Types;
using Au.Util;
using Au.Controls;

using System.Dynamic;
using LiteDB;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

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
	//	Also can depend on other things, eg handling some exception types, using Output.Clear etc. Only if [STAThread].
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



	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestAuLoadingFormsAssembly()
	{
		_ = typeof(Stopwatch).Assembly; //System, +17 ms
		_ = typeof(System.Linq.Enumerable).Assembly; //System.Core, +18 ms
		Print("NEW");

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Perf.First();
		//	//Thread_.LibIsLoadedFormsWpf();
		//	"fffff".StartsWith_("ff", true);
		//	//var s = Convert_.HexEncode(new byte[] { 1, 2 });
		//	//Perf.First();
		//	//var b = Convert_.HexDecode(s);
		//	//Print(b);

		//	Perf.NW();
		//}


		//return;

		AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		//if(Keyb.IsCtrl) Print("ctrl");
		//AuDialog.ShowEx(secondsTimeout: 1);

		//Print(Thread_.LibIsLoadedFormsWpf());

		//var t = typeof(Application);
		//bool u = Thread_.IsUI;
		//Print(u);

		//var f = new Form();
		//f.Click += (unu, sed) => Print(Thread_.IsUI);
		////Application.Run(f);
		//f.ShowDialog(); f.Dispose();

		//Print(Thread_.LibIsLoadedFormsWpf());

		//var m = new Au.Util.MessageLoop();
		//Timer_.After(2000, () => m.Stop());
		////Timer_.After(2000, () => Api.PostQuitMessage(0));
		////Timer_.After(2000, () => Wnd.Misc.PostThreadMessage(Api.WM_QUIT));
		//m.Loop();

		//var m = new AuMenu();
		//m["one"] = o => Print(o);
		//m.Show();

		//Osd.ShowText("TEST", showMode: OsdShowMode.Wait);
		//var m = new Osd();

		//Perf.First();
		//var k = new Keyb(null);
		//Perf.Next();
		//for(int i = 0; i < 5; i++) {
		//	k.AddKeys("Left");
		//	//k.AddKeys("VolumeUp");
		//	Perf.Next();
		//}
		//Perf.NW();

		Print("FINALLY");
		foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v);
	}

	private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
	{
		var a = args.LoadedAssembly;
		Print(a);
		if(a.FullName.StartsWith_("System.Windows.Forms")) {
			//Print(1);
		}
	}

	static unsafe void _TestExceptionInInteropCallback()
	{
		using(Au.Util.WinHook.ThreadGetMessage(x =>
		{
			Print(x.msg->ToString(), x.PM_NOREMOVE);
			//throw new AuException("TEST");
		})) {
			Timer_.Every(1000, () =>
		{
			Print(1);
			//throw new AuException("TEST");
			//Thread.CurrentThread.Abort();
		});
			MessageBox.Show("");
			//AuDialog.Show();
			//AuDialog.ShowEx(secondsTimeout: 10);
			Print("thread OK");
		}

		//EnumWindows((w, param) =>
		//{
		//	//Thread.Sleep(100);
		//	//Thread.CurrentThread.Abort();
		//	throw new AuException("TEST");
		//	Print(w);
		//	return true;
		//}, 0);
	}
	[DllImport("user32.dll")]
	internal static extern bool EnumWindows(Api.WNDENUMPROC lpEnumFunc, LPARAM lParam);

	static unsafe void TestExceptionInInteropCallback()
	{
		AppDomain.CurrentDomain.UnhandledException += (_, __) => { Print("UE", __.ExceptionObject); };

		var t = new Thread(_TestExceptionInInteropCallback);
		t.SetApartmentState(ApartmentState.STA);
		t.Start();
		1500.ms();
		t.Abort();
		t.Join();
		Print("main OK");
	}

	static void TestFileOpenWaitLocked()
	{
		var file = Folders.Temp + "test.txt";

		Task.Run(() =>
		{
			try {
				var t = Time.Milliseconds;
				while(Time.Milliseconds - t < 1200) {
					//using(var f = File.Create(file)) {
					using(var f = File_.WaitIfLocked(() => File.Create(file))) {
						f.WriteByte(1);
						50.ms();
					}
					//File.WriteAllText(file, "TEXT"); //unsafe. Exception if the file is locked.
					//File_.WaitIfLocked(() => File.WriteAllText(file, "TEXT")); //safe. Waits while the file is locked.
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); Print((uint)e.HResult); }
		});

		Task.Run(() =>
		{
			10.ms();
			try {
				var t = Time.Milliseconds;
				while(Time.Milliseconds - t < 1200) {
					//using(var f = File.OpenRead(file)) {
					using(var f = File_.WaitIfLocked(() => File.OpenRead(file))) {
						f.ReadByte();
					}
					//var s1 = File.ReadAllText(file); //unsafe. Exception if the file is locked.
					//var s2 = File_.WaitIfLocked(() => File.ReadAllText(file)); //safe. Waits while the file is locked.
					//using(var f = File_.WaitIfLocked(() => File.OpenText(file))) { //safe. Waits while the file is locked.
					//	var s3 = f.ReadToEnd();
					//}
					//using(var f = File_.WaitIfLocked(() => File.Create(file))) { //safe. Waits while the file is locked.
					//	f.WriteByte(1);
					//}
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); Print((uint)e.HResult); }
		}).Wait();

		Print("OK");
	}

	static void TestCsvDictionary()
	{
		//		var csv =
		//@"A1, "" 3 5  12 100 ""
		//		A2,4 100 -8 0x10";
		//		var x = new CsvTable(csv);
		//		//var d = x.Data.ToDictionary(row => row[0], row => row[1], StringComparer.OrdinalIgnoreCase);
		//		//var d = x.Data.ToDictionary(row => row[0], row => Au.Util.StringMisc.StringToIntArray(row[1]), StringComparer.OrdinalIgnoreCase);
		//		var d = x.ToDictionary(true, s => Au.Util.StringMisc.StringToIntArray(s));
		//		//Print(d);
		//		foreach(var v in d) Print(v.Key, string.Join(" ", v.Value));
		//		x.FromDictionary(d, v => string.Join(" ", v));
		//		Print(x);

		var csv =
@"A1, 5
a1,-8";
		var x = CsvTable.Parse(csv);
		//var d = x.ToDictionary(true, true);
		//Print(d);
		//x = CsvTable.FromDictionary(d);
		//Print(x);
		//var d = x.ToDictionary(true, false, s => s.ToInt_()); //rejected
		var d = x.ToDictionary(true, true, row => row[1].ToInt_());
		Print(d);
		//x = CsvTable.FromDictionary(d, v => v.ToString());
		x = CsvTable.FromDictionary(d, 2, (v, r) => r[1] = v.ToString());
		//x = CsvTable.FromDictionary(d, 3, (v, r) => { r[1] = v.ToString(); r[2] = "TEST"; });
		Print(x);

		//var f = Folders.Temp + "test2.csv";
		//var x = CsvTable.Parse(csv);
		//x.Save(f);
		//x = CsvTable.Load(f);
		//Print(x);

		//var f = Folders.Temp + "test2.csv";
		//var x = CsvTable.Parse(csv);
		//x.Save(f);
		//x = CsvTable.Load(f);
		//Print(x);

	}

	//static void TestAssocQS()
	//{
	//	Print(Api.AssocQueryString(".cs"));
	//}

	static void TestFileReplace()
	{
		//Print(Path.GetRandomFileName());
		//Print(Path.GetTempFileName());

		//File_.Save(@"Q:\Test\test.txt", "d", true);
		//File_.Save(@"Q:\Test\test.txt", "d", true, encoding: Encoding.Unicode);
		//File_.Save(@"Q:\Test\test.txt", new byte[] { 65, 66, 70 }, true);
		//var x = XElement.Parse("<a><b>mmm</b></a>");
		//File_.Save(@"Q:\Test\test.txt", temp => x.Save(temp));
		//x.Save_(@"Q:\Test\test.txt", true);
		//while(true) {
		//	int i = AuDialog.Show(buttons: "1Backup|2No backup|0Cancel");
		//	if(i == 0) break;
		//	Perf.First();
		//	File_.Save(@"Q:\Test\test.txt", "data", i==1);
		//	Perf.NW();
		//}
		Print("OK");

		//XElement e;e.Save();
		//XDocument d;d.Save()
	}

	static unsafe void TestDB()
	{
		string fileS = @"Q:\test\guid string.db";
		string fileB = @"Q:\test\guid binary.db";
		string fileL = @"Q:\test\guid long.db";
		string fileS2 = @"Q:\test\guid string2.db";
		var fileQ = @"Q:\test\sqlite.db";
		int n = 1000;

		File_.Delete(fileS);
		File_.Delete(fileB);
		File_.Delete(fileL);
		File_.Delete(fileS2);
		File_.Delete(fileQ);

		var a = new List<_GuidS>();
		for(int i = 0; i < n; i++) {
			var x = new _GuidS() { id = Convert_.GuidToHex(Guid.NewGuid()) };
			a.Add(x);
		}

		Perf.First();
#if false
		using(var db = new LiteDatabase(fileL)) {
			var col = db.GetCollection<_GuidL>();
			for(int i = 0; i < n; i++) {
				var g =Convert_.GuidFromHex(a[i].id);
				var x = new _GuidL() { id = *(long*)&g };
				col.Upsert(x);
			}
		}
		Perf.Next();

		using(var db = new LiteDatabase(fileB)) {
			var col = db.GetCollection<_GuidB>();
			for(int i = 0; i < n; i++) {
				var g = Convert_.GuidFromHex(a[i].id);
				var x = new _GuidB() { id = g };
				col.Upsert(x);
			}
		}
		Perf.Next();

		using(var db = new LiteDatabase(fileS)) {
			var col = db.GetCollection<_GuidS>();
			for(int i = 0; i < n; i++) {
				var x = new _GuidS() { id = a[i].id };
				col.Upsert(x);
			}
		}
		Perf.Next();

		using(var db = new LiteDatabase(fileS2)) {
			//Perf.Next();
			var col = db.GetCollection<_GuidS>();
			col.Upsert(a);
		}
		Perf.Next();
#endif
		//System.Data.SQLite. Very slow to load.
		//using(var db = new SQLiteConnection($"Data Source={fileQ};Version=3;")) {
		//	db.Open();
		//	using(var trans = db.BeginTransaction()) {
		//		//string sql = "create table test (guid TEXT PRIMARY KEY, name TEXT, etc TEXT, flags INT, a1, a2, i2 INT, s2 TEXT, s3 TEXT)";
		//		string sql = "create table test (guid BLOB PRIMARY KEY, name TEXT, etc TEXT, flags INT, a1, a2, i2 INT, s2 TEXT, s3 TEXT)";
		//		var command = new SQLiteCommand(sql, db, trans);
		//		command.ExecuteNonQuery();
		//		Perf.Next();

		//		sql = "insert into test values (?,?,?,?,?,?,?,?,?)";
		//		command = new SQLiteCommand(sql, db, trans);
		//		var p = command.Parameters;
		//		for(int i = 0; i < n; i++) {
		//			var v = a[i];
		//			//p.AddWithValue("guid", v.id);
		//			p.AddWithValue("guid", Convert_.GuidFromHex(v.id));

		//			p.AddWithValue("name", v.name);
		//			p.AddWithValue("etc", v.etc);
		//			p.AddWithValue("flags", v.flags);
		//			p.AddWithValue("a1", "moo");
		//			p.AddWithValue("a2", "");
		//			p.AddWithValue("i2", v.i2);
		//			p.AddWithValue("s2", v.s2);
		//			p.AddWithValue("s3", v.s3);

		//			command.ExecuteNonQuery();
		//			p.Clear();
		//		}

		//		trans.Commit();
		//	}
		//}

		Perf.NW();
	}

	class _GuidBase
	{
		public string name { get; set; }
		public string etc { get; set; }
		public int flags { get; set; }
		public int[] a1 { get; set; }
		public string[] a2 { get; set; }
		public int i2 { get; set; }
		public string s2 { get; set; }
		public string s3 { get; set; }

		public _GuidBase()
		{
			name = "test name";
			etc = "test etc";
			flags = 1000;
			a2 = new string[] { "mmmmmmmmmmmm" };
			i2 = 8;
			s2 = "ddddddd";
		}
	}

	class _GuidS :_GuidBase
	{
		public string id { get; set; }
	}

	class _GuidB :_GuidBase
	{
		public Guid id { get; set; }
	}

	class _GuidL :_GuidBase
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
	}



#region test sqlite

//	static void TestSqlite()
//	{
//		var sb = new StringBuilder();

//		Perf.Next();
//		var file = @"Q:\test\sqlite.db";
//		bool isNew = !File_.ExistsAsFile(file);
//		Perf.Next();

//		using(var db = new SQLiteConnection(file)) {
//			Perf.Next();

//			if(isNew) {
//				db.CreateTable<Stock>();

//				var s = db.Insert(new Stock() {
//					Symbol = "one"
//				});
//				Perf.Next();
//			}

//			for(int i = 0; i < 5; i++) {
//#if true
//				//var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

//				var query = db.Table<Stock>();
//				foreach(var stock in query)
//					sb.AppendLine("Stock: " + stock.Symbol);
//#else
//				foreach(var stock in db.Query<Stock>("select * from Stock"))
//					sb.AppendLine("Stock: " + stock.Symbol);
//#endif
//				Perf.Next();
//			}
//		}
//		Perf.NW();

//		Print(sb);
//	}
//#endif

	static void TestSqlite()
	{
		Perf.Next();
#if false
		string fileS = @"Q:\test\guid string.db";

		var stream = File_.WaitIfLocked(() => new FileStream(fileS, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, System.IO.FileOptions.RandomAccess));
		using(var db = new LiteDatabase(stream, disposeStream: true)) {
			Perf.Next();
			var col = db.GetCollection<_GuidS>();
			Perf.Next();
			int n = col.Count();
			Perf.Next();
		}
#else
		var file = @"Q:\test\sqlite.db";
		//File_.Delete(file);
		bool isNew = !File_.ExistsAsFile(file);

		using(var db = new SQLiteConnection(file)) {
			Perf.Next();

			for(int j = 0; j < 1; j++) {
				if(isNew) {
					db.CreateTable<Stock>();
					isNew = false;
					//db.CreateTable<Stock>(CreateFlags.ImplicitPK);
				} else {
					db.DeleteAll<Stock>();
				}

				var x = new Stock { Symbol = "one" };
				x.Symbol = new string('A', 500);
				for(int i = 0; i < 5; i++) {
					//Print(SQLite3.LastInsertRowid(db.Handle));
					//x.id = i + 10;
					db.Insert(x);
					Print(x.id);
				}
			}
			Perf.Next();

			for(int i = 0; i < 1; i++) {
#if true
				//var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

				var query = db.Table<Stock>();
				foreach(var stock in query) { }
				//Print(stock.Symbol);
#else
						foreach(var stock in db.Query<Stock>("select * from Stock"))
							sb.AppendLine("Stock: " + stock.Symbol);
#endif
			}
			Perf.Next();
		}
#endif
		Perf.NW();
	}

	public class Stock
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string Symbol { get; set; }
	}

	#endregion

	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		//Output.IgnoreConsole = true;
		//Output.LogFile=@"Q:\Test\Au"+IntPtr.Size*8+".log";
		Output.LibUseQM2 = true;
		Output.RedirectConsoleOutput = true;
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			100.ms();
		}

		try {
#if true

			//Perf.Cpu();
			//AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
			Perf.First();
			TestSqlite();
			//AuDialog.Show();

			//TestDB();
			//TestFileReplace();
			//TestAssocQS();
			//TestCsvDictionary();
			//TestFileOpenWaitLocked();
			//TestAuLoadingFormsAssembly();
			//TestExceptionInInteropCallback();
#else
			try {

				//var w8 = Wnd.Find("*Firefox*", "MozillaWindowClass").OrThrow();
				////Print(w8);
				//var a = Acc.FindAll(w8, "web:TEXT", "??*");
				////var a = Acc.FindAll(w8, "web:");
				////var a = Acc.FindAll(w8, "web:TEXT", "??*", flags: AFFlags.NotInProc);
				////var a = Acc.Wait(3, w8, "web:TEXT", "Search the Web", flags: AFFlags.NotInProc);
				////var a = Acc.Wait(3, w8, "TEXT", "Search the Web", flags: AFFlags.UIA);
				//Print(a);
				//Print("---");
				//Print(a[0].MiscFlags);
				//return;

				//var w = Wnd.Find("Java Control Panel", "SunAwtFrame").OrThrow();
				////var a = Acc.Find(w, "push button", "Settings...").OrThrow();
				//var a = Acc.Find(w, "push button", "Settings...", flags: AFFlags.ClientArea).OrThrow();
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
				Cpp.Cpp_Unload();
			}
#endif
		}
		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }

	}
}
