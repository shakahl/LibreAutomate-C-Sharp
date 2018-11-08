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
//using SQLite; //SQLite-net

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
//using Au.Util;
using Au.Controls;
using Au.Triggers;

using System.Dynamic;
using System.Security.Principal;
//using LiteDB;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 169, 219, 649 //unreachable code, unused var/field


//[assembly: Trigger.Hotkey("F4")]

//class App :AuAppBase
//{
//	static App()
//	{
//		Output.LibUseQM2 = true;
//		Output.RedirectConsoleOutput = true;
//		if(!Output.IsWritingToConsole) {
//			Output.Clear();
//			//100.ms();
//		}
//	}

//	//[Trigger.Hotkey("Ctrl+-")]
//	[STAThread] static void Main(string[] args) { new App()._Main(args); }
//	//[STAThread] static void Main(string[] args) { var a = new App(); a._Main(args); a.RunTriggers(); }

//	//[Trigger.Hotkey("Ctrl+-")]
//	void _Main(string[] args)
//	{
//		try {
//			Print("main");
//			RunTriggers();
//			//Trigger.Run(this);
//			//Trigger.Run(typeof(App));
//			//TestTriggers();
//		}
//		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }

//	}

//	static void TestTriggers()
//	{
//		//Trigger.Hotkey["Ctrl+U"] = m => { Print("target"); };


//		AuDialog.Show();
//	}

//	[Trigger.Hotkey("Ctrl+K")]
//	void TrigOne(HotkeyTriggers.Message m)
//	{
//		Print(1);
//	}

//	[Trigger.Hotkey("Ctrl+M")]
//	static void TrigTwo()
//	{
//		Print(1);
//	}
//}



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
		using(Au.Util.WinHook.ThreadGetMessage(x => {
			Print(x.msg->ToString(), x.PM_NOREMOVE);
			//throw new AuException("TEST");
		})) {
			Timer_.Every(1000, () => {
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
		var file = @"Q:\test\test.txt";

		Task.Run(() => {
			try {
				var t = Time.Milliseconds;
				while(Time.Milliseconds - t < 1200) {
					//using(var f = File.Create(file)) {
					using(var f = File_.WaitIfLocked(() => File.Create(file))) {
						f.WriteByte(1);
						7.ms();
					}
					//File.WriteAllText(file, "TEXT"); //unsafe. Exception if the file is locked.
					//File_.WaitIfLocked(() => File.WriteAllText(file, "TEXT")); //safe. Waits while the file is locked.
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); Print((uint)e.HResult); }
		});

		Task.Run(() => {
			10.ms();
			try {
				var t = Time.Milliseconds;
				while(Time.Milliseconds - t < 1200) {
					//using(var f = File.OpenRead(file)) {
					using(var f = File_.WaitIfLocked(() => File.OpenRead(file))) {
						f.ReadByte();
						5.ms();
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

	//	static unsafe void TestDB()
	//	{
	//		string fileS = @"Q:\test\guid string.db";
	//		string fileB = @"Q:\test\guid binary.db";
	//		string fileL = @"Q:\test\guid long.db";
	//		string fileS2 = @"Q:\test\guid string2.db";
	//		var fileQ = @"Q:\test\sqlite.db";
	//		int n = 1000;

	//		File_.Delete(fileS);
	//		File_.Delete(fileB);
	//		File_.Delete(fileL);
	//		File_.Delete(fileS2);
	//		File_.Delete(fileQ);

	//		var a = new List<_GuidS>();
	//		for(int i = 0; i < n; i++) {
	//			var x = new _GuidS() { id = Convert_.GuidToHex(Guid.NewGuid()) };
	//			a.Add(x);
	//		}

	//		Perf.First();
	//#if false
	//		using(var db = new LiteDatabase(fileL)) {
	//			var col = db.GetCollection<_GuidL>();
	//			for(int i = 0; i < n; i++) {
	//				var g =Convert_.GuidFromHex(a[i].id);
	//				var x = new _GuidL() { id = *(long*)&g };
	//				col.Upsert(x);
	//			}
	//		}
	//		Perf.Next();

	//		using(var db = new LiteDatabase(fileB)) {
	//			var col = db.GetCollection<_GuidB>();
	//			for(int i = 0; i < n; i++) {
	//				var g = Convert_.GuidFromHex(a[i].id);
	//				var x = new _GuidB() { id = g };
	//				col.Upsert(x);
	//			}
	//		}
	//		Perf.Next();

	//		using(var db = new LiteDatabase(fileS)) {
	//			var col = db.GetCollection<_GuidS>();
	//			for(int i = 0; i < n; i++) {
	//				var x = new _GuidS() { id = a[i].id };
	//				col.Upsert(x);
	//			}
	//		}
	//		Perf.Next();

	//		using(var db = new LiteDatabase(fileS2)) {
	//			//Perf.Next();
	//			var col = db.GetCollection<_GuidS>();
	//			col.Upsert(a);
	//		}
	//		Perf.Next();
	//#endif
	//		//System.Data.SQLite. Very slow to load.
	//		//using(var db = new SQLiteConnection($"Data Source={fileQ};Version=3;")) {
	//		//	db.Open();
	//		//	using(var trans = db.BeginTransaction()) {
	//		//		//string sql = "create table test (guid TEXT PRIMARY KEY, name TEXT, etc TEXT, flags INT, a1, a2, i2 INT, s2 TEXT, s3 TEXT)";
	//		//		string sql = "create table test (guid BLOB PRIMARY KEY, name TEXT, etc TEXT, flags INT, a1, a2, i2 INT, s2 TEXT, s3 TEXT)";
	//		//		var command = new SQLiteCommand(sql, db, trans);
	//		//		command.ExecuteNonQuery();
	//		//		Perf.Next();

	//		//		sql = "insert into test values (?,?,?,?,?,?,?,?,?)";
	//		//		command = new SQLiteCommand(sql, db, trans);
	//		//		var p = command.Parameters;
	//		//		for(int i = 0; i < n; i++) {
	//		//			var v = a[i];
	//		//			//p.AddWithValue("guid", v.id);
	//		//			p.AddWithValue("guid", Convert_.GuidFromHex(v.id));

	//		//			p.AddWithValue("name", v.name);
	//		//			p.AddWithValue("etc", v.etc);
	//		//			p.AddWithValue("flags", v.flags);
	//		//			p.AddWithValue("a1", "moo");
	//		//			p.AddWithValue("a2", "");
	//		//			p.AddWithValue("i2", v.i2);
	//		//			p.AddWithValue("s2", v.s2);
	//		//			p.AddWithValue("s3", v.s3);

	//		//			command.ExecuteNonQuery();
	//		//			p.Clear();
	//		//		}

	//		//		trans.Commit();
	//		//	}
	//		//}

	//		Perf.NW();
	//	}

	//class _GuidBase
	//{
	//	public string name { get; set; }
	//	public string etc { get; set; }
	//	public int flags { get; set; }
	//	public int[] a1 { get; set; }
	//	public string[] a2 { get; set; }
	//	public int i2 { get; set; }
	//	public string s2 { get; set; }
	//	public string s3 { get; set; }

	//	public _GuidBase()
	//	{
	//		name = "test name";
	//		etc = "test etc";
	//		flags = 1000;
	//		a2 = new string[] { "mmmmmmmmmmmm" };
	//		i2 = 8;
	//		s2 = "ddddddd";
	//	}
	//}

	//class _GuidS :_GuidBase
	//{
	//	public string id { get; set; }
	//}

	//class _GuidB :_GuidBase
	//{
	//	public Guid id { get; set; }
	//}

	//class _GuidL :_GuidBase
	//{
	//	[PrimaryKey, AutoIncrement]
	//	public long id { get; set; }
	//}



	#region test sqlite

	//	static void TestSqlite()
	//	{
	//		var sb = new StringBuilder();

	//		Perf.Next();
	//		var file = @"Q:\test\sqlite.db";
	//		bool isNew = !File_.ExistsAsFile(file);
	//	static void TestSqliteNet()
	//	{
	//		Perf.Next();
	//#if false
	//		string fileS = @"Q:\test\guid string.db";

	//		var stream = File_.WaitIfLocked(() => new FileStream(fileS, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, System.IO.FileOptions.RandomAccess));
	//		using(var db = new LiteDatabase(stream, disposeStream: true)) {
	//			Perf.Next();
	//			var col = db.GetCollection<_GuidS>();
	//			Perf.Next();
	//			int n = col.Count();
	//			Perf.Next();
	//		}
	//#else
	//		var file = @"Q:\test\sqlite.db";
	//		File_.Delete(file);
	//		bool isNew = !File_.ExistsAsFile(file);

	//		using(var db = new SQLiteConnection(file)) {
	//			Perf.Next();

	//			for(int j = 0; j < 1; j++) {
	//				if(isNew) {
	//					//var m = db.GetMapping<Stock>();
	//					//m.


	//					db.CreateTable<Stock>();
	//					isNew = false;
	//					//db.CreateTable<Stock>(CreateFlags.ImplicitPK);
	//				} else {
	//					db.DeleteAll<Stock>();
	//				}

	//				var x = new Stock { Symbol = "one" };
	//				//x.folding = new List<int> { 3, 5 };
	//				x.folding = new int[] { 3, 5 };
	//				//x.Symbol = new string('A', 500);
	//				var p = Perf.StartNew();
	//				db.BeginTransaction();
	//				for(int i = 0; i < 1; i++) {
	//#if false
	//					db.
	//#else
	//					//Print(SQLite3.LastInsertRowid(db.Handle));
	//					x.id = i + 1;
	//					db.Insert(x);
	//					//Print(x.id);
	//#endif
	//				}
	//				db.Commit();
	//				p.NW();
	//			}
	//			Perf.Next();

	//			//return;
	//			//AuDialog.Show();

	//			for(int i = 0; i < 1; i++) {
	//#if true
	//				//var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

	//				var query = db.Table<Stock>();
	//				foreach(var stock in query) {
	//					Print(stock.Symbol);
	//					//Print(stock.folding);
	//				}
	//#else
	//						foreach(var stock in db.Query<Stock>("select * from Stock"))
	//							sb.AppendLine("Stock: " + stock.Symbol);
	//#endif
	//			}
	//			Perf.Next();
	//		}
	//#endif
	//		Perf.NW();
	//	}	//		Perf.Next();

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



	//public class Stock
	//{
	//	[PrimaryKey]
	//	public long id { get; set; }
	//	public string Symbol { get; set; }
	//	//public List<int> folding { get; set; }
	//	public int[] folding { get; set; }
	//}

	#endregion

	#region test TreeBase

	class TFile :Au.Util.TreeBase<TFile>
	{
		public string Name { get; set; }
		public int Id { get; private set; }
		public bool IsFolder { get; private set; }

		public TFile(string name, int id, bool isFolder) { Name = name; Id = id; IsFolder = isFolder; }

		//XML element -> TFile object
		TFile(XmlReader x, TFile parent)
		{
			if(parent == null) { //the root XML element
				if(x.Name != "example") throw new ArgumentException("XML root element name must be example");
				IsFolder = true;
			} else {
				switch(x.Name) {
				case "e": break;
				case "f": IsFolder = true; break;
				default: throw new ArgumentException("XML element name must be e or f");
				}
				while(x.MoveToNextAttribute()) {
					var v = x.Value;
					switch(x.Name) {
					case "name": Name = v; break;
					case "id": Id = v.ToInt_(); break;
					}
				}
				if(Empty(Name)) throw new ArgumentException("no name attribute in XML");
				if(Id == 0) throw new ArgumentException("no id attribute in XML");
			}
		}

		public static TFile Load(string file) => XmlLoad(file, (x, p) => new TFile(x, p));

		public void Save(string file) => XmlSave(file, (x, n) => n._XmlWrite(x));

		//TFile object -> XML element
		void _XmlWrite(XmlWriter x)
		{
			if(Parent == null) {
				x.WriteStartElement("example");
			} else {
				x.WriteStartElement(IsFolder ? "f" : "e");
				x.WriteAttributeString("name", Name);
				x.WriteAttributeString("id", Id.ToString());
			}
		}

		public override string ToString() => $"{new string(' ', Level)}{(IsFolder ? 'f' : 'e')} {Name} ({Id})";
	}

	static void TestTFile()
	{
		//string file = @"Q:\Test\ok\files2.xml";

		//Perf.First();
		//var t = TFile.Load(file);

		TFile t = new TFile(null, 0, true); //root
		TFile n1, n2;
		//test AddChild
		t.AddChild(new TFile("one", 1, false));
		t.AddChild(n1 = new TFile("two", 2, true)); //folder
		n1.AddChild(new TFile("three", 3, false)); //in folder
		n1.AddChild(new TFile("four", 4, false)); //in folder
		t.AddChild(new TFile("five", 5, false), true); //first child
													   //test AddSibling
		t.AddChild(n2 = new TFile("six", 6, false));
		n2.AddSibling(new TFile("seven", 7, false), false); //before six
		n2.AddSibling(new TFile("eith", 8, false), true); //after six (now at the end)
		n1.FirstChild.AddSibling(new TFile("nine", 9, false), false); //before three in folder
		n1.AddSibling(n2 = new TFile("ten", 10, false), true); //after folder two

		//Print(t.Children(false));
		Print(t.Descendants(true));
		//Print("LINQ");
		//Print(t.Descendants().FirstOrDefault(k => k.Name == "seven"));
		//Print(t.Descendants().Where(k => k.Level > 1));
		//Print("ANCESTORS");
		var n3 = t.Descendants().FirstOrDefault(k => k.Name == "three");
		////Print(n3.Ancestors());
		//Print(n3.AncestorsReverse());
		//Print("NAVIGATE");
		////Print(n2.Parent, n3.Parent, t.Parent, t.RootAncestor, n2.RootAncestor);
		////Print(t.FirstChild, t.LastChild, n1.FirstChild, n1.LastChild, n3.FirstChild, n3.LastChild);
		//Print(t.Next, t.Previous, n1.Next, n1.Previous, n3.Next, n3.Previous, n3.Previous.Previous, n3.Next.Next);
		//Print("HAS");
		//Print(t.HasChildren, t.HasParent, n1.HasChildren, n1.HasParent, n2.HasChildren, n2.HasParent);
		//Print("REMOVE");
		////n2.Remove();
		//n1.Remove();
		//Print(t.Descendants(true));
		//Print(n1.HasChildren, n1.HasParent, n1.Next, n1.Previous);
		Print("IsDescendantOf");
		//Print(t.IsDescendantOf(n1), t.IsDescendantOf(t));
		Print(n3.IsDescendantOf(n1), n1.IsDescendantOf(n3));

		t.Save(@"Q:\Test\ok\files3.xml");







		//#region MyTree
		//var x = MyTree.Load(@"Q:\test\example.xml");

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { foreach(var n in x.Children()) { if(n == null) Print("null"); } }
		//	Perf.NW();
		//	Thread.Sleep(10);
		//}
		//#endregion
	}

	#endregion

	#region TreeBase examples

	/* meta r System.Xml */
	//using System.Xml;

	class MyTree :Au.Util.TreeBase<MyTree>
	{
		public string Name { get; set; }
		public int Id { get; private set; }
		public bool IsFolder { get; private set; }

		public MyTree(string name, int id, bool isFolder) { Name = name; Id = id; IsFolder = isFolder; }

		//XML element -> MyTree object
		MyTree(XmlReader x, MyTree parent)
		{
			if(parent == null) { //the root XML element
				if(x.Name != "example") throw new ArgumentException("XML root element name must be example");
				IsFolder = true;
			} else {
				switch(x.Name) {
				case "e": break;
				case "f": IsFolder = true; break;
				default: throw new ArgumentException("XML element name must be e or f");
				}
#if true //two ways of reading attributes
				Name = x["name"];
				Id = x["id"].ToInt_();
#else
				while(x.MoveToNextAttribute()) {
					var v = x.Value;
					switch(x.Name) {
					case "name": Name = v; break;
					case "id": Id = v.ToInt_(); break;
					}
				}
#endif
				if(Empty(Name)) throw new ArgumentException("no name attribute in XML");
				if(Id == 0) throw new ArgumentException("no id attribute in XML");
			}
		}

		public static MyTree Load(string file) => XmlLoad(file, (x, p) => new MyTree(x, p));

		public void Save(string file) => XmlSave(file, (x, n) => n._XmlWrite(x));

		//MyTree object -> XML element
		void _XmlWrite(XmlWriter x)
		{
			if(Parent == null) {
				x.WriteStartElement("example");
			} else {
				x.WriteStartElement(IsFolder ? "f" : "e");
				x.WriteAttributeString("name", Name);
				x.WriteAttributeString("id", Id.ToString());
			}
		}

		public override string ToString() => $"{new string(' ', Level)}{(IsFolder ? 'f' : 'e')} {Name} ({Id})";
	}

	static void TNodeExample()
	{
		/*
		<example>
		  <e name="one" id="1" />
		  <f name="two" id="112">
			<e name="three" id="113" />
			<e name="four" id="114" />
			<f name="five" id="120">
			  <e name="six" id="121" />
			  <e name="seven" id="122" />
			</f>
		  </f>
		  <f name="eight" id="217" />
		  <e name="ten" id="144" />
		</example>
		*/

		var x = MyTree.Load(@"Q:\test\example.xml");
		foreach(MyTree n in x.Descendants(true)) Print(n);
		//Print(x.Descendants().FirstOrDefault(k => k.Name == "seven")); //use LINQ to find a descendant
		//Print(x.Descendants().Where(k => k.Level > 1)); //use LINQ to find some descendants
		x.Save(@"Q:\test\example2.xml");
	}

	#endregion

	static void TestUnusedKey()
	{
		int n = 10000;
		var d = new Dictionary<int, string>(n);
		for(int i = 0; i < n; i++) {
			d.Add(i, "");
		}

		Perf.Cpu();
		for(int i1 = 0; i1 < 5; i1++) {
			int r1 = 0, r2 = 0;
			Perf.First();
			//var k = d.Keys;
			for(int i = 1; i < int.MaxValue; i++) {
				if(!d.ContainsKey(i)) { r1 = i; break; }
			}
			Perf.Next();
			//r2= Enumerable.Range(0, int.MaxValue).Except(d.Keys).FirstOrDefault(); //slow
			r2 = d.Keys.Max() + 1;
			Perf.NW();
			Print(r1, r2);
			Thread.Sleep(10);
		}
	}

	static unsafe void TestGetBoxedPointer()
	{
		POINT g = (10, 20);
		object v = g;
		//v = new int[] { 30, 40 };
		var h = GCHandle.Alloc(v, GCHandleType.Pinned);
		var p = (int*)h.AddrOfPinnedObject();
		Print(p[0], p[1], p[-1], p[-2]);
		Print((long)p);

		//var h2 = GCHandle.Alloc(v, GCHandleType.Pinned);
		//var p2 = (int*)h2.AddrOfPinnedObject();
		//Print((long)p2);

		//var t = v.GetType();
		//Print(t.);

		//int[] x = new int[10];
		//Type type = x.GetType();
		//if(type.IsArray) {
		//	Type typeOfElement = type.GetElementType();
		//	int length = 0; // how do I get array's length????
		//}
	}

	static unsafe void TestSqlite()
	{
		//Sqlite.Test(); return;

		var file = @"Q:\test\sqlite.db";
		File_.Delete(file);
		bool isNew = !File_.ExistsAsFile(file);

		Perf.Cpu();
		string dbsql = null;
		//dbsql = "PRAGMA encoding='UTF-16'";
		using(var db = new SqliteDB(file, sql: dbsql)) {
			db.Execute("CREATE TABLE test(id INTEGER PRIMARY KEY, name TEXT, re REAL, ąčę BLOB, i64 INTEGER)");

			db.Execute("BEGIN");
			using(var p = db.Statement("INSERT INTO test VALUES(?, $name, :do, ?, @ąčę);")) {
				//using(var p = new Sqlite.Statement(db, "INSERT INTO test VALUES(?, $name, :do, zeroblob(8), @ąčę);")) {
				p.Bind(1, 1);
				//p.Bind(1, 20).Bind(2, "hhhhhhhhhhh").Bind(3, 4.55);
				//p.BindZeroBlob(4, 10);
				var a = new byte[] { 1, 2, 3 };
				//p.Bind(4, a);
				//fixed(byte* bp=a) p.Bind(4, bp, 2);
				//p.BindStruct(4, 123456789012.5m); //decimal
				//p.BindStruct(4, Guid.NewGuid());
				//p.Bind(5, true);
				//p.Bind(5, 123456789012);
				//p.Bind(5, DateTime.Now, true);
				//p.Bind(2, "HHHH");
				//p.Bind("$name", "HHHH");
				//p.Bind(5, AccROLE.APPLICATION);
				//p.Bind(5, _ELong.Test);
				//p.Bind(5, _EByte.Test);
				//p.Bind(5, true);
				//p.Bind(3, 0.58);
				//p.BindStruct("@ąčę", new POINT(10, 80));
				//p.Step();

				//p.BindAll(1, "text", 5.2, a, _EByte.Test);
				//p.BindAll(1, "text", 5.2f, Guid.NewGuid(), true);
				//p.BindAll(1, "text", 5.2, 123456m, DateTime.UtcNow.ToBinary());
				p.Step();

				//p.Reset(clearBindings: false);
				//p.Bind(1, 100).Bind(":do", 9.4);
				//p.Step();

				//p.Reset();
				//p.Bind(1, 1000).BindStruct(4, Guid.NewGuid());
				////var m = new byte[2, 2] { { 1, 2 },{ 3, 4 } };
				////p.Bind(4, m);
				//p.Step();

				//p.Reset();
				//p.BindAll(123456789012, null, 1.2f, Guid.NewGuid(), _ELong.Test);
				//p.Step();

				//Perf.First();
				//for(int i = 0; i < 1000; i++) {
				//	p.Reset();
				//	p.Bind(1, i).Bind(2, new string('i', i));
				//	p.Step();
				//	if(i == 0) Perf.Next();
				//}
				//Perf.NW();
			}
			db.Execute("COMMIT");

			//var a1 = new byte[] { 1, 2, 3 };
			//db.Execute("INSERT INTO test VALUES(?, ?, ?, ?, ?);", 5, "exec", -0.1, a1, 123456789012345);

			//if(db.Get(out string vi, "SELECT name FROM test WHERE id=?", 1)) Print(vi); else Print("NOT FOUND");
			//if(db.Get(out List<byte> vi, "SELECT ąčę FROM test WHERE id=?", 1)) Print(vi); else Print("NOT FOUND");
			//if(db.Get(out long vi, "SELECT i64 FROM test WHERE id=?", 1)) Print(DateTime.FromBinary(vi).ToLocalTime()); else Print("NOT FOUND");
			//if(db.GetStruct(out decimal vi, "SELECT ąčę FROM test WHERE id=?", 1)) Print(vi); else Print("NOT FOUND");

			//using(var p = new Sqlite.Statement(db, "SELECT name FROM test WHERE id=1")) {
			////using(var p = new Sqlite.Statement(db, "SELECT i64 FROM test WHERE id=1")) {
			////using(var p = new Sqlite.Statement(db, "SELECT re FROM test WHERE id=1")) {
			////using(var p = new Sqlite.Statement(db, "SELECT ąčę FROM test WHERE id=1")) {
			//	Perf.First(); bool was = false;
			//	if(p.Step()) {
			//		var v = p.GetText("name");
			//		Print((object)v);
			//		//var v = p.GetInt(0);
			//		//var v = p.GetBool(0);
			//		//var v = p.GetDouble(0);
			//		//var v = p.GetDateTime(0, true);
			//		//Print(v);
			//		//var v = (int*)p.GetBlob(0, out int n);
			//		//Print(n, v[0], v[1]);
			//		//var v = p.GetArray<int>(0);
			//		//var v = p.GetList<int>(0);
			//		//var v = p.GetStruct<POINT>(0);
			//		//Print(v);

			//		//Print(p.ColumnCount, p.ColumnName(0), p.ColumnIndex("ąčę"));
			//	}
			//	Perf.NW();
			//}

			//using(var p = new Sqlite.Statement(db, "SELECT name FROM test")) {
			//	Perf.First(); bool was = false;
			//	while(p.Step()) {
			//		var s = p.GetText(0);
			//		if(!was) { was = true; Perf.Next(); }
			//		//Print(s);
			//	}
			//	Perf.NW();
			//}
		}
		Print("OK");
	}
	enum _ELong :long { Test = 123456789012 }
	enum _EByte :byte { Test = 5 }

	static void TestSqliteExamples()
	{
		//open database file
		using(var db = new SqliteDB(@"Q:\test\sqlite.db")) {
			//create table
			db.Execute("CREATE TABLE IF NOT EXISTS test(id INTEGER PRIMARY KEY, name TEXT, x INT, guid BLOB, array BLOB)");

			//add 2 rows of data
			using(var trans = db.Transaction()) { //optional, but makes much faster when making multiple changes, and ensures that all or none of these changes are written to the database
				using(var p = db.Statement("INSERT OR REPLACE INTO test VALUES(?, ?, :x, ?, ?)")) {
					//assume we want to add values of these variables to the database table
					int id = 1; string name = "TEXT"; long x = -10; Guid guid = Guid.NewGuid(); int[] arr = new int[] { 1, 2, 3 };
					//add first row
					p.Bind(1, id);
					p.Bind(2, name).BindStruct(4, guid).Bind(5, arr);
					p.Bind(":x", x);
					p.Step();
					//add second row
					p.Reset().Bind(1, 2).Bind(":x", 123456789012345).Step(); //unbound columns are null
				}
				//update single row
				db.Execute("UPDATE test SET name=?2 WHERE id=?1", 2, "two");
				//write all this to database
				trans.Commit();
			}

			//get data
			using(var p = db.Statement("SELECT * FROM test")) {
				while(p.Step()) { //for each row of results
					Print(p.GetInt(0), p.GetText(1), p.GetLong(2));
					Print(p.GetStruct<Guid>("guid"));
					Print(p.GetArray<int>("array"));
					Print("----");
				}
			}
			//get single value
			if(db.Get(out string s1, "SELECT name FROM test WHERE id=?", 1)) Print(s1); else Print("not found");
			if(db.Get(out int i1, "PRAGMA page_size")) Print(i1);
		}

		Print("OK");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	//static bool _TestCollectionEmpty(int[] a)
	static bool _TestCollectionEmpty(List<int> a)
	{
		return Empty(a);
	}

	static void TestCollectionEmpty()
	{
		//int[] a = new int[0];
		//var a = new List<int>();
		////a.Add(1);
		////a = null;
		//Print(_TestCollectionEmpty(a));

		//var d = new Dictionary<int, string> { { 1, "" } };
		//d = null;
		//Print(Empty(d));
	}

	public enum FileTextEncoding
	{
		/// <summary>Not a text file.</summary>
		Binary = 0,

		/// <summary>UTF-8 without BOM or ASCII.</summary>
		Utf8NoBOM = 1,

		/// <summary>UTF-8 with BOM (3 bytes).</summary>
		Utf8BOM = 1 | (3 << 4),

		/// <summary>ANSI, unknown code page.</summary>
		Ansi = 2,

		/// <summary>UTF-16 without BOM.</summary>
		Utf16NoBOM = 3,

		/// <summary>UTF-16 with BOM (2 bytes).</summary>
		Utf16BOM = 3 | (2 << 4),

		/// <summary>UTF-16 with big endian BOM (2 bytes).</summary>
		Utf16BigEndian = 4 | (2 << 4),

		/// <summary>UTF-32 with BOM (4 bytes).</summary>
		Utf32BOM = 5 | (4 << 4),

		/// <summary>UTF-32 with big endian BOM (4 bytes).</summary>
		Utf32BigEndian = 6 | (4 << 4),

		//rejected. .NET does not save/load with UTF-7 BOM, so we too. Can be different BOM length.
		///// <summary>UTF-7 with BOM.</summary>
		//Utf7BOM,
	}

	static unsafe Encoding _DetectFileTextEncoding(byte* s, int len, out FileTextEncoding fte, out int bomLength)
	{
		var t = _DetectFileTextEncoding(s, len);
		bomLength = (int)t >> 4;
		fte = t;
		switch(t) {
		case FileTextEncoding.Ansi: return Encoding.Default;
		case FileTextEncoding.Utf8BOM: case FileTextEncoding.Utf8NoBOM: return Encoding.UTF8;
		case FileTextEncoding.Utf16BOM: case FileTextEncoding.Utf16NoBOM: return Encoding.Unicode;
		case FileTextEncoding.Utf16BigEndian: return Encoding.BigEndianUnicode;
		case FileTextEncoding.Utf32BOM: case FileTextEncoding.Utf32BigEndian: return Encoding.UTF32;
		}
		return null; //Binary
	}

	static unsafe FileTextEncoding _DetectFileTextEncoding(byte* s, int len)
	{
		if(len == 0) return FileTextEncoding.Utf8NoBOM;
		if(len == 1) return s[0] == 0 ? FileTextEncoding.Binary : (s[0] < 128 ? FileTextEncoding.Utf8NoBOM : FileTextEncoding.Ansi);
		if(len >= 3 && s[0] == 0xEF && s[1] == 0xBB && s[2] == 0xBF) return FileTextEncoding.Utf8BOM;
		//bool canBe16 = 0 == (fileSize & 1), canBe32 = 0 == (fileSize & 3); //rejected. .NET ignores it too.
		if(s[0] == 0xFF && s[1] == 0xFE) {
			if(len >= 4 && s[2] == 0 && s[3] == 0) return FileTextEncoding.Utf32BOM;
			return FileTextEncoding.Utf16BOM;
		}
		if(s[0] == 0xFE && s[1] == 0xFF) return FileTextEncoding.Utf16BigEndian;
		if(len >= 4 && *(uint*)s == 0xFFFE0000) return FileTextEncoding.Utf32BigEndian;
		if(Au.Util.LibCharPtr.Length(s, len) == len) { //no '\0'
			byte* p = s, pe = s + len; for(; p < pe; p++) if(*p >= 128) break; //is ASCII?
			if(p < pe && 0 == Api.MultiByteToWideChar(Api.CP_UTF8, Api.MB_ERR_INVALID_CHARS, s, len, null, 0)) return FileTextEncoding.Ansi;
			return FileTextEncoding.Utf8NoBOM;
		}
		var u = (char*)s; len /= 2;
		if(Au.Util.LibCharPtr.Length(u, len) == len) //no '\0'
			if(0 != Api.WideCharToMultiByte(Api.CP_UTF8, Api.WC_ERR_INVALID_CHARS, u, len, null, 0, default, null)) return FileTextEncoding.Utf16NoBOM;
		return FileTextEncoding.Binary;
	}

	static unsafe void TestDetectFileTextEncoding()
	{
		//byte* s = stackalloc byte[10000];

		string file = @"Q:\test\ansi.txt";
		//string file = @"Q:\test\application_edit.ico";
		string text = "ABąč";
		//File.WriteAllText(file, text);
		//File.WriteAllText(file, text, Encoding.UTF8);
		File.WriteAllText(file, text, Encoding.UTF32);
		//File.WriteAllText(file, text, Encoding.GetEncoding(1257));
		return;

		var k = Encoding.Unicode.GetBytes(text);
		k = k.Insert_<byte>(0, 0xff, 0xfe);
		k = k.Insert_<byte>(k.Length, 65);
		File.WriteAllBytes(file, k);
		Print(File.ReadAllText(file));// return;

		Print("---- bytes ----");
		Print(File.ReadAllBytes(file));
		Print("------------");

		using(var r = File.OpenRead(file)) {
			var fileSize = r.Length;
			int len = (int)Math.Min(fileSize, 65_000);
			var b = new byte[len];
			len = r.Read(b, 0, len);
			//Print(len);
			fixed (byte* p = b) {
				var fte = _DetectFileTextEncoding(p, len);
				Print(fte);
			}
		}

		////var s = File.ReadAllText(file, Encoding.Default);
		//if(File_.GetProperties(file, out var p)) Print(p.Size);
		//Print("bytes:");
		//Print(File.ReadAllBytes(file));
		//Print("---");
		////var s = File.ReadAllText(file);
		////Print(s);
		//var x = File.OpenText(file);
		//Print(x.Peek());
		//Print(x.CurrentEncoding);
		//Print(x.ReadToEnd());
	}

	static void TestEditAndContinue()
	{
		Print(1);
	}

	static void TestThreadStart()
	{
		//var t = new Thread(() => {

		//});
		//var t = Thread_.Start(() => { });
		//Print(t.IsBackground, t.GetApartmentState());
		//t.Start();

		Osd.ShowText("test");
		AuDialog.Show();
	}


	static void TestUacTS()
	{
#if false
		//Au.Util.LibTaskScheduler.CreateTaskToRunProgramAsAdmin(@"Au", "test UAC 2", Folders.System + "cmd.exe");
		Au.Util.LibTaskScheduler.CreateTaskToRunProgramAsAdmin(@"Au", "test UAC", @"Q:\My QM\test_ts_UAC.exe", "test: $(Arg0)");
#else
		Print(Au.Util.LibTaskScheduler.TaskExists(@"\Au", "test UAC"));
		//Print(Au.Util.LibTaskScheduler.RunTask(@"\Au", "test UAC", "moo"));

		//Au.Util.LibTaskScheduler.RunTask(@"\Quick Macros", "test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(@"Quick Macros", "test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(@"\", @"\Quick Macros\test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(@"", @"\Quick Macros\test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(null, @"\Quick Macros\test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(null, @"Quick Macros\test UAC"); //works
#endif

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Perf.First();
		//	Au.Util.LibTaskScheduler.RunTask(@"\Quick Macros", "test UAC");
		//	Perf.NW();
		//	Thread.Sleep(1000);
		//}
	}

	static void TestLibSerialize()
	{
		var b = Au.Util.LibSerializer.Serialize(3, (string)null, "test", new string[] { "one", "two" }, 4);
		var a = Au.Util.LibSerializer.Deserialize(b);
		Print((int)a[0]);
		Print((string)a[1]);
		Print((string)a[2]);
		Print((string[])a[3]);
		Print((int)a[4]);
	}


	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		//Output.IgnoreConsole = true;
		//Output.LogFile=@"Q:\Test\Au"+IntPtr.Size*8+".log";
		Output.LibUseQM2 = true;
		Output.RedirectConsoleOutput = true;
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			//100.ms();
		}
		//AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

		try {
#if true

			TestLibSerialize();
			//TestUacTS();
			//TestThreadStart();
			//TestEditAndContinue();
			//TestDetectFileTextEncoding();
			//TestCollectionEmpty();
			//Perf.Cpu();
			//Perf.First();
			//TestSqlite();
			//TestSqliteExamples();
			//TestGetBoxedPointer();
			//AuDialog.Show();

			//TestDB();
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
