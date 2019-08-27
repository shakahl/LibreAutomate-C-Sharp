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
using System.Xml;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using Au.Triggers;
using System.Security.AccessControl;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;

static partial class Test
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestAuLoadingFormsAssembly()
	{
		_ = typeof(Stopwatch).Assembly; //System, +17 ms
		_ = typeof(System.Linq.Enumerable).Assembly; //System.Core, +18 ms
		Print("NEW");

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	APerf.First();
		//	//AThread.LibIsLoadedFormsWpf();
		//	"fffff".Starts("ff", true);
		//	//var s = AConvert.HexEncode(new byte[] { 1, 2 });
		//	//APerf.First();
		//	//var b = AConvert.HexDecode(s);
		//	//Print(b);

		//	APerf.NW();
		//}


		//return;

		AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		//if(AKeys.IsCtrl) Print("ctrl");
		//ADialog.ShowEx(secondsTimeout: 1);

		//Print(AThread.LibIsLoadedFormsWpf());

		//var t = typeof(Application);
		//bool u = AThread.IsUI;
		//Print(u);

		//var f = new Form();
		//f.Click += (unu, sed) => Print(AThread.IsUI);
		////Application.Run(f);
		//f.ShowDialog(); f.Dispose();

		//Print(AThread.LibIsLoadedFormsWpf());

		//var m = new Au.Util.MessageLoop();
		//ATimer.After(2000, () => m.Stop());
		////ATimer.After(2000, () => Api.PostQuitMessage(0));
		////ATimer.After(2000, () => AWnd.More.PostThreadMessage(Api.WM_QUIT));
		//m.Loop();

		//var m = new AMenu();
		//m["one"] = o => Print(o);
		//m.Show();

		//AOsd.ShowText("TEST", showMode: OsdShowMode.Wait);
		//var m = new AOsd();

		//APerf.First();
		//var k = new AKeys(null);
		//APerf.Next();
		//for(int i = 0; i < 5; i++) {
		//	k.AddKeys("Left");
		//	//k.AddKeys("VolumeUp");
		//	APerf.Next();
		//}
		//APerf.NW();

		Print("FINALLY");
		foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v);
	}

	private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
	{
		var a = args.LoadedAssembly;
		Print(a);
		if(a.FullName.Starts("System.Windows.Forms")) {
			//Print(1);
		}
	}

	static unsafe void _TestExceptionInInteropCallback()
	{
		using(AHookWin.ThreadGetMessage(x => {
			Print(x.msg->ToString(), x.PM_NOREMOVE);
			//throw new AException("TEST");
		})) {
			ATimer.Every(1000, () => {
				Print(1);
				//throw new AException("TEST");
				//Thread.CurrentThread.Abort();
			});
			MessageBox.Show("");
			//ADialog.Show();
			//ADialog.ShowEx(secondsTimeout: 10);
			Print("thread OK");
		}

		//EnumWindows((w, param) =>
		//{
		//	//Thread.Sleep(100);
		//	//Thread.CurrentThread.Abort();
		//	throw new AException("TEST");
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
				var t = ATime.PerfMilliseconds;
				while(ATime.PerfMilliseconds - t < 1200) {
					//using(var f = File.Create(file)) {
					using(var f = AFile.WaitIfLocked(() => File.Create(file))) {
						f.WriteByte(1);
						7.ms();
					}
					//File.WriteAllText(file, "TEXT"); //unsafe. Exception if the file is locked.
					//AFile.WaitIfLocked(() => File.WriteAllText(file, "TEXT")); //safe. Waits while the file is locked.
				}
			}
			catch(Exception e) { ADebug.Print(e.ToString()); Print((uint)e.HResult); }
		});

		Task.Run(() => {
			10.ms();
			try {
				var t = ATime.PerfMilliseconds;
				while(ATime.PerfMilliseconds - t < 1200) {
					//using(var f = File.OpenRead(file)) {
					using(var f = AFile.WaitIfLocked(() => File.OpenRead(file))) {
						f.ReadByte();
						5.ms();
					}
					//var s1 = File.ReadAllText(file); //unsafe. Exception if the file is locked.
					//var s2 = AFile.WaitIfLocked(() => File.ReadAllText(file)); //safe. Waits while the file is locked.
					//using(var f = AFile.WaitIfLocked(() => File.OpenText(file))) { //safe. Waits while the file is locked.
					//	var s3 = f.ReadToEnd();
					//}
					//using(var f = AFile.WaitIfLocked(() => File.Create(file))) { //safe. Waits while the file is locked.
					//	f.WriteByte(1);
					//}
				}
			}
			catch(Exception e) { ADebug.Print(e.ToString()); Print((uint)e.HResult); }
		}).Wait();

		Print("OK");
	}

	static void TestCsvDictionary()
	{
		//		var csv =
		//@"A1, "" 3 5  12 100 ""
		//		A2,4 100 -8 0x10";
		//		var x = new ACsv(csv);
		//		//var d = x.Data.ToDictionary(row => row[0], row => row[1], StringComparer.OrdinalIgnoreCase);
		//		//var d = x.Data.ToDictionary(row => row[0], row => Au.Util.AStringUtil.StringToIntArray(row[1]), StringComparer.OrdinalIgnoreCase);
		//		var d = x.ToDictionary(true, s => Au.Util.AStringUtil.StringToIntArray(s));
		//		//Print(d);
		//		foreach(var v in d) Print(v.Key, string.Join(" ", v.Value));
		//		x.FromDictionary(d, v => string.Join(" ", v));
		//		Print(x);

		var csv =
@"A1, 5
a1,-8";
		var x = ACsv.Parse(csv);
		//var d = x.ToDictionary(true, true);
		//Print(d);
		//x = ACsv.FromDictionary(d);
		//Print(x);
		//var d = x.ToDictionary(true, false, s => s.ToInt()); //rejected
		var d = x.ToDictionary(true, true, row => row[1].ToInt());
		Print(d);
		//x = ACsv.FromDictionary(d, v => v.ToString());
		x = ACsv.FromDictionary(d, 2, (v, r) => r[1] = v.ToString());
		//x = ACsv.FromDictionary(d, 3, (v, r) => { r[1] = v.ToString(); r[2] = "TEST"; });
		Print(x);

		//var f = AFolders.Temp + "test2.csv";
		//var x = ACsv.Parse(csv);
		//x.Save(f);
		//x = ACsv.Load(f);
		//Print(x);

		//var f = AFolders.Temp + "test2.csv";
		//var x = ACsv.Parse(csv);
		//x.Save(f);
		//x = ACsv.Load(f);
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

	//		AFile.Delete(fileS);
	//		AFile.Delete(fileB);
	//		AFile.Delete(fileL);
	//		AFile.Delete(fileS2);
	//		AFile.Delete(fileQ);

	//		var a = new List<_GuidS>();
	//		for(int i = 0; i < n; i++) {
	//			var x = new _GuidS() { id = AConvert.GuidToHex(Guid.NewGuid()) };
	//			a.Add(x);
	//		}

	//		APerf.First();
	//#if false
	//		using(var db = new LiteDatabase(fileL)) {
	//			var col = db.GetCollection<_GuidL>();
	//			for(int i = 0; i < n; i++) {
	//				var g =AConvert.GuidFromHex(a[i].id);
	//				var x = new _GuidL() { id = *(long*)&g };
	//				col.Upsert(x);
	//			}
	//		}
	//		APerf.Next();

	//		using(var db = new LiteDatabase(fileB)) {
	//			var col = db.GetCollection<_GuidB>();
	//			for(int i = 0; i < n; i++) {
	//				var g = AConvert.GuidFromHex(a[i].id);
	//				var x = new _GuidB() { id = g };
	//				col.Upsert(x);
	//			}
	//		}
	//		APerf.Next();

	//		using(var db = new LiteDatabase(fileS)) {
	//			var col = db.GetCollection<_GuidS>();
	//			for(int i = 0; i < n; i++) {
	//				var x = new _GuidS() { id = a[i].id };
	//				col.Upsert(x);
	//			}
	//		}
	//		APerf.Next();

	//		using(var db = new LiteDatabase(fileS2)) {
	//			//APerf.Next();
	//			var col = db.GetCollection<_GuidS>();
	//			col.Upsert(a);
	//		}
	//		APerf.Next();
	//#endif
	//		//System.Data.SQLite. Very slow to load.
	//		//using(var db = new SQLiteConnection($"Data Source={fileQ};Version=3;")) {
	//		//	db.Open();
	//		//	using(var trans = db.BeginTransaction()) {
	//		//		//string sql = "create table test (guid TEXT PRIMARY KEY, name TEXT, etc TEXT, flags INT, a1, a2, i2 INT, s2 TEXT, s3 TEXT)";
	//		//		string sql = "create table test (guid BLOB PRIMARY KEY, name TEXT, etc TEXT, flags INT, a1, a2, i2 INT, s2 TEXT, s3 TEXT)";
	//		//		var command = new SQLiteCommand(sql, db, trans);
	//		//		command.ExecuteNonQuery();
	//		//		APerf.Next();

	//		//		sql = "insert into test values (?,?,?,?,?,?,?,?,?)";
	//		//		command = new SQLiteCommand(sql, db, trans);
	//		//		var p = command.Parameters;
	//		//		for(int i = 0; i < n; i++) {
	//		//			var v = a[i];
	//		//			//p.AddWithValue("guid", v.id);
	//		//			p.AddWithValue("guid", AConvert.GuidFromHex(v.id));

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

	//		APerf.NW();
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

	//		APerf.Next();
	//		var file = @"Q:\test\sqlite.db";
	//		bool isNew = !AFile.ExistsAsFile(file);
	//	static void TestSqliteNet()
	//	{
	//		APerf.Next();
	//#if false
	//		string fileS = @"Q:\test\guid string.db";

	//		var stream = AFile.WaitIfLocked(() => new FileStream(fileS, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, System.IO.FileOptions.RandomAccess));
	//		using(var db = new LiteDatabase(stream, disposeStream: true)) {
	//			APerf.Next();
	//			var col = db.GetCollection<_GuidS>();
	//			APerf.Next();
	//			int n = col.Count();
	//			APerf.Next();
	//		}
	//#else
	//		var file = @"Q:\test\sqlite.db";
	//		AFile.Delete(file);
	//		bool isNew = !AFile.ExistsAsFile(file);

	//		using(var db = new SQLiteConnection(file)) {
	//			APerf.Next();

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
	//				var p = APerf.Create();
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
	//			APerf.Next();

	//			//return;
	//			//ADialog.Show();

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
	//			APerf.Next();
	//		}
	//#endif
	//		APerf.NW();
	//	}	//		APerf.Next();

	//		using(var db = new SQLiteConnection(file)) {
	//			APerf.Next();

	//			if(isNew) {
	//				db.CreateTable<Stock>();

	//				var s = db.Insert(new Stock() {
	//					Symbol = "one"
	//				});
	//				APerf.Next();
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
	//				APerf.Next();
	//			}
	//		}
	//		APerf.NW();

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

	#region test ATreeBase

	class TFile : Au.Util.ATreeBase<TFile>
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
					case "id": Id = v.ToInt(); break;
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

		//APerf.First();
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

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { foreach(var n in x.Children()) { if(n == null) Print("null"); } }
		//	APerf.NW();
		//	Thread.Sleep(10);
		//}
		//#endregion
	}

	#endregion

	#region ATreeBase examples

	/*/ r System.Xml */
	//using System.Xml;

	class MyTree : Au.Util.ATreeBase<MyTree>
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
				Id = x["id"].ToInt();
#else
				while(x.MoveToNextAttribute()) {
					var v = x.Value;
					switch(x.Name) {
					case "name": Name = v; break;
					case "id": Id = v.ToInt(); break;
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

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 5; i1++) {
			int r1 = 0, r2 = 0;
			APerf.First();
			//var k = d.Keys;
			for(int i = 1; i < int.MaxValue; i++) {
				if(!d.ContainsKey(i)) { r1 = i; break; }
			}
			APerf.Next();
			//r2= Enumerable.Range(0, int.MaxValue).Except(d.Keys).FirstOrDefault(); //slow
			r2 = d.Keys.Max() + 1;
			APerf.NW();
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
		AFile.Delete(file);
		bool isNew = !AFile.ExistsAsFile(file);

		APerf.SpeedUpCpu();
		string dbsql = null;
		//dbsql = "PRAGMA encoding='UTF-16'";
		using(var db = new ASqlite(file, sql: dbsql)) {
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

				//APerf.First();
				//for(int i = 0; i < 1000; i++) {
				//	p.Reset();
				//	p.Bind(1, i).Bind(2, new string('i', i));
				//	p.Step();
				//	if(i == 0) APerf.Next();
				//}
				//APerf.NW();
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
			//	APerf.First(); bool was = false;
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
			//	APerf.NW();
			//}

			//using(var p = new Sqlite.Statement(db, "SELECT name FROM test")) {
			//	APerf.First(); bool was = false;
			//	while(p.Step()) {
			//		var s = p.GetText(0);
			//		if(!was) { was = true; APerf.Next(); }
			//		//Print(s);
			//	}
			//	APerf.NW();
			//}
		}
		Print("OK");
	}
	enum _ELong : long { Test = 123456789012 }
	enum _EByte : byte { Test = 5 }

	static void TestSqliteExamples()
	{
		//open database file
		using(var db = new ASqlite(@"Q:\test\sqlite.db")) {
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
		k = k.InsertAt<byte>(0, 0xff, 0xfe);
		k = k.InsertAt<byte>(k.Length, 65);
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
		//if(AFile.GetProperties(file, out var p)) Print(p.Size);
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
		//var t = AThread.Start(() => { });
		//Print(t.IsBackground, t.GetApartmentState());
		//t.Start();

		AOsd.ShowText("test");
		ADialog.Show();
	}


	static void TestUacTS()
	{
#if false
		//Au.Util.LibTaskScheduler.CreateTaskToRunProgramAsAdmin(@"Au", "test UAC 2", AFolders.System + "cmd.exe");
		Au.Util.LibTaskScheduler.CreateTaskToRunProgramAsAdmin(@"Au", "test UAC", @"Q:\My QM\test_ts_UAC.exe", "test: $(Arg0)");
#else
		//Print(Au.Util.LibTaskScheduler.TaskExists(@"\Au", "test UAC"));
		//Print(Au.Util.LibTaskScheduler.RunTask(@"\Au", "test UAC", "moo"));

		//Au.Util.LibTaskScheduler.RunTask(@"\Quick Macros", "test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(@"Quick Macros", "test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(@"\", @"\Quick Macros\test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(@"", @"\Quick Macros\test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(null, @"\Quick Macros\test UAC"); //works
		//Au.Util.LibTaskScheduler.RunTask(null, @"Quick Macros\test UAC"); //works
#endif

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	APerf.First();
		//	Au.Util.LibTaskScheduler.RunTask(@"\Quick Macros", "test UAC");
		//	APerf.NW();
		//	Thread.Sleep(1000);
		//}

		var s = AFolders.System + "notepad.exe";
		//Au.Util.LibTaskScheduler.CreateTaskToRunProgramOnDemand("Au", "High", UacIL.High, s);
		Au.Util.LibTaskScheduler.CreateTaskToRunProgramOnDemand("Au", "Medium", UacIL.Medium, s);
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

	static void TestSerializeBytes()
	{
		var b = new byte[] { 1, 2, 3 };
		var sa = new string[] { "one", null, "three" };
		var x = Au.Util.LibSerializer.Serialize(5, b, sa, "text");
		var a = Au.Util.LibSerializer.Deserialize(x);
		int i = a[0]; Print(i);
		byte[] c = a[1]; Print(c);
		string[] sa2 = a[2]; Print(sa2);
		string s = a[3]; Print(s);
	}

	static void _SetDirectorySecurity(string path, bool secure)
	{
		var ds = Directory.GetAccessControl(path);

		//disable or enable security inheritance
		if(secure != ds.AreAccessRulesProtected) {
			ds.SetAccessRuleProtection(secure, true);
			Directory.SetAccessControl(path, ds);
			if(secure) ds = Directory.GetAccessControl(path);
		}
		if(!secure) return;

		var u = new NTAccount("G");
		ds.PurgeAccessRules(u);
		ds.AddAccessRule(new FileSystemAccessRule(u, FileSystemRights.ReadAndExecute, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
		Directory.SetAccessControl(path, ds);
	}

	static void TestFolderSecurity2()
	{
		//bool secure = false;

		var path = AFolders.Documents + @"Au\Secure";
		if(Directory.Exists(path)) Directory.Delete(path);
		Directory.CreateDirectory(path);

		_SetDirectorySecurity(path, true);
		return;

		//var x = new DirectorySecurity(AFolders.Documents + @"Au", AccessControlSections.All);
		//x.RemoveAccessRule(new FileSystemAccessRule("G", FileSystemRights.Modify, AccessControlType.Allow));
		//Directory.CreateDirectory(s, x);

		var ds = Directory.GetAccessControl(path);

		if(!ds.AreAccessRulesProtected) { //if security inherited from parent
			ds.SetAccessRuleProtection(true, true); //disable inheritance
			Directory.SetAccessControl(path, ds);
			ds = Directory.GetAccessControl(path);
		}

		//var rules = ds.GetAccessRules(true, true, typeof(NTAccount));
		//foreach(FileSystemAccessRule rule in rules) {
		//	var k = rule.IdentityReference as NTAccount;
		//	Print(k.Value);
		//	//Print(rule.IdentityReference.Value);
		//	//if(rule.IdentityReference.Value == @"Q7C\G")
		//	//	ds.RemoveAccessRuleSpecific(rule);
		//}

		var u = new NTAccount("G");
		ds.PurgeAccessRules(u);
		ds.AddAccessRule(new FileSystemAccessRule(u, FileSystemRights.ReadAndExecute, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

		Directory.SetAccessControl(path, ds);

	}

	static void TestFolderSecurity()
	{
		string path = AFolders.ProgramFiles;
		var ds = Directory.GetAccessControl(path);

		var rules = ds.GetAccessRules(true, true, typeof(NTAccount));
		foreach(FileSystemAccessRule rule in rules) {
			var k = rule.IdentityReference as NTAccount;
			Print(k.Value);
			//Print(rule.IdentityReference.Value);
			//if(rule.IdentityReference.Value == @"Q7C\G")
			//	ds.RemoveAccessRuleSpecific(rule);
		}

	}

	static void TestPopupList()
	{
		Application.SetCompatibleTextRenderingDefault(false);
		var f = new Form { Text = "Nikn", Font = Au.Util.AFonts.Regular, StartPosition = FormStartPosition.Manual, Location = new Point(300, 300) };
		var t = new TextBox { Width = 120, Left = 20 }; f.Controls.Add(t);
		var b = new Button { Text = "button", Left = 200 }; f.Controls.Add(b);
		var bOK = new ButtonOK { Left = 10, Top = 230 }; f.Controls.Add(bOK); f.AcceptButton = bOK;
		var bCancel = new ButtonCancel { Left = 90, Top = 230 }; f.Controls.Add(bCancel); f.CancelButton = bCancel;

#if true
		//var a = new string[] { "One 123456789 123456789", "Folder", "Three", "Four", "W" };
		//var a = new string[] { "One 123456789 123456789", "Folder", "Three", "Fo\r\nur", "Fo\r\nur", "Fo\r\nur", "Fo\r\nur" };
		var icon = AIcon.GetFileIconImage(@"q:\app\ontop.ico", 16);
		var a = new PLItem[] {
			new PLItem("Done 123456789 123456789"){},
			new PLItem("Two"){CheckType=PLCheckType.Box},
			new PLItem("Three"){CheckType=PLCheckType.Row,Checked=true},
			new PLItem("Three 123456789 123456789"){Icon=icon},
			new PLItem("Three 123456789 123456789"){Icon=icon, CheckType=PLCheckType.Row},
			new PLItem("Disabled"){Disabled=true},
			new PLItem("Disabled + checkbox"){Disabled=true,CheckType=PLCheckType.Box},
			new PLItem("W Text color"){TextColor=Color.BlueViolet},
			new PLItem("W Text color, disabled"){TextColor=Color.DarkGoldenrod, Disabled=true},
			new PLItem("Back color"){BackColor=Color.GreenYellow},
			new PLItem("Back color, disabled"){BackColor=Color.MediumOrchid, Disabled=true},
			new PLItem("Bold"){BoldFont=true},
			new PLItem("Tooltip"){TooltipText="Tooltip text"},
			new PLItem("Tooltip2 wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww"){TooltipText="Multi\r\nLine"},
			new PLItem("Group 1"){CheckType=PLCheckType.Row, Group=1,Checked=true},
			new PLItem("Group 2"){CheckType=PLCheckType.Row, Group=1},
			new PLItem("Group 3"){CheckType=PLCheckType.Row, Group=1},
			new PLItem("-Group 1"){CheckType=PLCheckType.Row, Group=-1,Checked=true},
			new PLItem("-Group 2"){CheckType=PLCheckType.Row, Group=-1},
		};
#else
		var a = new string[300];
		for(int i = 0; i < a.Length; i++) a[i] = "a";
		//var a = new string[1000];
		//for(int i = 0; i < a.Length; i++) a[i] = "QwertyuiopAsdfghjk ZXCVBNMQWERTYIOPASDFGHJKL" + i;
#endif

		var p = new PopupList();
		p.DontCloseWhenAppDeactivated = true;
		//p.MultiShow = true;
		//p.FixedWidth = true;
		//p.ComboBoxAnimation = true;
		//p.PopupWindow.Font = new Font("Courier New", 14);
		//p.PopupWindow.Font = new Font("Verdana", 8);
		//p.PopupWindow.Font = new Font("MS Sans Serif", 8.25f);
		p.Items = a;

		p.SelectedAction = k => Print("Selected", k.ResultItem, k.ResultIndex, k.ResultWasKey, k.PopupWindow.Visible, k.PopupWindow.IsHandleCreated);
		//p.PopupWindow.FormClosed += (unu2, sed2) => Print("CLOSED", p.ResultItem, p.PopupWindow.Visible, p.PopupWindow.IsHandleCreated);
		p.ClosedAction = k => {
			Print("Closed", k.ResultItem, k.PopupWindow.IsHandleCreated);
			//ATimer.After(1000, () => p.Show(t));
			//p.PopupWindow.Close();
		};

		f.Load += (unu, sed) => {
			//var tt = new TextBox { Width = 150, Height = 25 };
			//ATimer.After(50, () => {
			//	var m = new AMenu();
			//	m["ddd"] = o => Print(o);
			//	tt.MinimumSize = tt.Size;
			//	var h = new ToolStripControlHost(tt);
			//	m.Add(h);
			//	m.Show(f, 100, 100);
			//});

			ATimer.After(200, () => {
				//t.Visible = false;
				p.Show(t);
				//p.Show(new Rectangle(200, 500, 0, 0));
				//p.Show(f, new Rectangle(50, 100, 0, 0));

				//for(int i = 0; i < 10; i++) _ShowPL(f, a, i);
			});

			//ATimer.After(500, tim => {
			//	//p.Show(t);
			//	p.Show(new Rectangle(200, 500, 0, 0));
			//});
		};
		//f.TopMost = true;

		//ATimer.After(2000, () => {
		t.Click += (unu, sed) => {
			p.ComboBoxAnimation = true;
			p.Show(t);

			//var f2 = new Form { Text = "Two", Font = SystemFonts.MessageBoxFont };
			//var t2 = new TextBox { Width = 120, Left = 20 }; f2.Controls.Add(t2);
			//ATimer.After(500, () => p.Show(t2));
			//f2.ShowDialog(); f2.Dispose();
		};

#if true
		Application.Run(f);
		//ADialog.Show();
#else
		var f2 = new Form();
		f2.Click += (unu, sed) => f.ShowDialog();
		Application.Run(f2);
#endif
	}

	static void _ShowPL(Form f, string[] a, int i)
	{
		var p = new PopupList();
		p.DontCloseWhenAppDeactivated = true;
		p.MultiShow = true;
		//p.FixedWidth = true;
		//p.ComboBoxAnimation = true;
		p.Items = a;
		var r = new Rectangle(i * 100, 50, 90, 0);
		p.Show(f, r);
	}

	class PLItem : IPopupListItem
	{
		public string TooltipText { get; set; }
		public Image Icon { get; set; }
		public ColorInt BackColor { get; set; }
		public ColorInt TextColor { get; set; }
		public PLCheckType CheckType { get; set; }
		public bool Checked { get; set; }
		public bool Disabled { get; set; }
		public bool BoldFont { get; set; }
		public short Group { get; set; }

		public PLItem(string text)
		{
			Text = text;
		}
		public string Text { get; set; }
		public override string ToString() => Text;
	}

	class Hin
	{
		string a, b, c, d;
	}

	static unsafe void TestGetObjectSize()
	{
		//object obj = new Aga.Controls.Tree.TreeNodeAdv(""); //TreeNodeAdv 128
		//obj = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string>());
		var t = typeof(List<Aga.Controls.Tree.TreeNodeAdv>);
		RuntimeTypeHandle th = t.TypeHandle;
		int size = Marshal.ReadInt32(th.Value, 4);
		Print(size);
	}

	static void TestOptimizeTreeViewAdv()
	{
		var w = AWnd.FindFast(null, "QM_Editor").OrThrow();
		var f = new Au.Tools.FormAWnd(w);
		Application.Run(f);
	}

	//static void TestReferenceCOM()
	//{
	//	var w = AWnd.Find("Quick *").OrThrow();
	//	w = w.ChildById(2212).OrThrow();
	//	Accessibility.IAccessible a = null;
	//	if(0 != AccessibleObjectFromWindow(w, OBJID_CLIENT, IID_IAccessible, out a)) throw new Exception("failed");
	//	var s = a.accName[1];
	//	Print(s);
	//}
	//[DllImport("oleacc.dll", PreserveSig = true)]
	//internal static extern int AccessibleObjectFromWindow(AWnd hwnd, uint dwId, in Guid riid, out Accessibility.IAccessible ppvObject);
	//internal static Guid IID_IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
	//internal const uint OBJID_CLIENT = 0xFFFFFFFC;

	static void TestReferenceCOM2()
	{
		//var e = new Excel.Application();
		////Print(e);
		//e.Visible = true;
		//ADialog.Show();
		//e.Quit();

		//Print(typeof(ServicesManagerLib.enControlsAccepted).GetEnumNames());

		//int hr = QueryPathOfRegTypeLib(new Guid("{00000200-0000-0010-8000-00AA006D2EA4}"), 2, 0, 0, out var s);
		//int hr = QueryPathOfRegTypeLib(new Guid("{000204EF-0000-0000-C000-000000000046}"), 6, 0, 9, out var s);
		//if(hr == 0) Print(s); else Print(ALastError.MessageFor(hr));

		APerf.First();
		var a = new List<_RegTypelib>(1000);
		using(var tlKey = Registry.ClassesRoot.OpenSubKey("TypeLib")) { //guids
			foreach(var sGuid in tlKey.GetSubKeyNames()) {
				if(sGuid.Length != 38) continue;
				//Print(sGuid);
				using(var guidKey = tlKey.OpenSubKey(sGuid)) { //versions
					foreach(var sVer in guidKey.GetSubKeyNames()) {
						using(var verKey = guidKey.OpenSubKey(sVer)) {
							if(verKey.GetValue("") is string description) {
								var text = description + ", " + sVer;
								foreach(var sLocale in verKey.GetSubKeyNames()) {
									if(sLocale[0] < '0' || sLocale[0] > '9') continue;
									var r = new _RegTypelib { guid = sGuid, text = text, version = sVer, locale = sLocale };
									a.Add(r);
								}
							} //else Print(sGuid); //some Microsoft typelibs. VS does not show these too.
						}
					}
				}
			}
		}
		APerf.NW(); //87000, with locale 118000
		a.Sort((x, y) => string.Compare(x.text, y.text, true));

		//Print(a.Count); return;
		foreach(var r in a) {
			//Print(r.text);
#if true
			int hr = r.Load(out var tl);
#else
			if(!Guid.TryParse(r.guid, out var guid)) continue;
			var ver = _RegTypelibParseVersion(r.version);
			var lcid = (uint)r.locale.ToInt(0, STIFlags.IsHexWithout0x);
			int hr = LoadRegTypeLib(guid, ver.major, ver.minor, lcid, out var tl);
#endif
			if(hr != 0) { Print($"<><c 0xff>{r.text}, {r.version}, {r.locale}, {r.guid}, {ALastError.MessageFor(hr)}</c>"); continue; }
			Marshal.ReleaseComObject(tl);
		}
	}

	[DllImport("oleaut32.dll", EntryPoint = "#164", PreserveSig = true)]
	internal static extern int QueryPathOfRegTypeLib(in Guid guid, ushort wMaj, ushort wMin, uint lcid, [MarshalAs(UnmanagedType.BStr)] out string lpbstrPathName);

	private enum RegKind
	{
		RegKind_Default = 0,
		RegKind_Register = 1,
		RegKind_None = 2
	}

	[DllImport("oleaut32.dll", EntryPoint = "#183", PreserveSig = true)]
	static extern int LoadTypeLibEx(string szFile, RegKind regkind, out ITypeLib pptlib);

	[DllImport("oleaut32.dll", EntryPoint = "#162", PreserveSig = true)]
	internal static extern int LoadRegTypeLib(in Guid rguid, ushort wVerMajor, ushort wVerMinor, uint lcid, out ITypeLib pptlib);

	class _RegTypelib
	{
		public string text, guid, version, locale;

		public override string ToString() => text;

		public int Load(out ITypeLib tl)
		{
			tl = null; string path = null; int hr = 1;
			var k0 = $@"TypeLib\{guid}\{version}\{locale}\win";
			for(int i = 0; i < 2; i++) {
				var bits = AVersion.Is64BitProcess == (i == 0) ? "64" : "32";
				using(var hk = Registry.ClassesRoot.OpenSubKey(k0 + bits)) {
					path = hk?.GetValue("") as string;
					if(path == null) continue;
					path = path.Trim('\"');
				}
				hr = LoadTypeLibEx(path, RegKind.RegKind_None, out tl);
				if(hr == 0 && tl == null) hr = 1;
				if(hr == 0) break;
			}
			return hr;
		}
	}

	static (ushort major, ushort minor) _RegTypelibParseVersion(string version)
	{
		int verMinor = 0, verMajor = version.ToInt(0, out int iEnd, STIFlags.IsHexWithout0x);
		if(iEnd < version.Length && version[iEnd] == '.') verMinor = version.ToInt(iEnd + 1, STIFlags.IsHexWithout0x);
		return ((ushort)verMajor, (ushort)verMinor);
	}

	static void TestDictionaryForTriggers()
	{
		int n2 = 1000;
		var a0 = new int[n2];
		var rand = new Random();
		int kFind = 0;
		for(int i = 0; i < a0.Length; i++) {
			var k = rand.Next(0xffff);
			while(a0.Contains(k)) k = rand.Next(0xffff);
			a0[i] = k;
			if(i == n2 / 2) kFind = k;
		}

		var a1 = new List<RECT>();
		var a2 = new SortedList<int, RECT>();
		var a3 = new Dictionary<int, RECT>();

		Print("ADD");
		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 5; i1++) {
			a1.Clear(); a2.Clear(); a3.Clear();
			APerf.First();
			for(int i2 = 0; i2 < n2; i2++) { var key = a0[i2]; a1.Add(new RECT(key, key, i2, 0, false)); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var key = a0[i2]; a2.Add(key, new RECT(key, 0, i2, 0, false)); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var key = a0[i2]; a3.Add(key, new RECT(key, 0, i2, 0, false)); }
			APerf.NW();
			Thread.Sleep(200);
		}

		//Print("FIND");
		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int v1 = 0, v2 = 0, v3 = 0;

		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { for(int i = 0; i < a1.Count; i++) if(a1[i].left == kFind) { v1 = a1[i].top; break; } }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { if(a2.TryGetValue(kFind, out var r)) v2 = r.left; }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { if(a3.TryGetValue(kFind, out var r)) v3 = r.left; }
		//	APerf.NW();
		//	Print(v1, v2, v3);
		//	Thread.Sleep(200);
		//}

		Print("REMOVE");
		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 5; i1++) {
			int rFrom = n2 / 4, rTo = n2 - rFrom;

			APerf.First();
			//for(int i2 = 0; i2 < n2; i2++) { for(int i = a2.Count - 1; i >= 0; i--) { var g = a2[i].right; if(g >= rFrom && g < rTo) a2.RemoveAt(i); } }
			//APerf.Next();
			//foreach(var v in a3.Where(kv => { var g = kv.Value.right; return g >= rFrom && g < rTo; }).ToList()) { a3.Remove(v.Key); }
			//foreach(var v in a3.Where(kv => { var g = kv.Value.right; return g >= rFrom && g < rTo; }).Select(kv2=>kv2.Key).ToList()) { a3.Remove(v); }
			a3.RemoveWhere(kv => { var g = kv.Value.right; return g >= rFrom && g < rTo; });
			APerf.NW();
			Print(a2.Count, a3.Count);
			for(int i2 = rFrom; i2 < rTo; i2++) { var key = a0[i2]; a3.Add(key, new RECT(key, 0, i2, 0, false)); }
			Print(a3.Count);
			Thread.Sleep(200);
		}
	}

	private static void X_DoWork(object sender, DoWorkEventArgs e)
	{
		var t = Thread.CurrentThread;
		Print(AThread.NativeId, t.IsBackground, t.IsThreadPoolThread, t.GetApartmentState(), e.Argument);
	}

	static void TestFormLoadVisible()
	{
		var f = new Form45();
		//f.WindowState = FormWindowState.Maximized;

		var b = new Button { Text = "kkk" };
		f.Controls.Add(b);
		var e = new TextBox { Left = 100 };
		f.Controls.Add(e);

		//ATimer.After(2000, () => Application.Exit());
		Application.Run(f);
	}

	class Form45 : Form
	{
		protected override void OnLoad(EventArgs e)
		{
			Print("OnLoad", ((AWnd)this).IsVisible);
			base.OnLoad(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			//AOutput.QM2.Write($"_canShow={_canShow}, _visibleOnce={_visibleOnce}, Visible={Visible}");
			Print("OnVisibleChanged");
		}

		bool _first;

		//protected override void SetVisibleCore(bool value)
		//{
		//	Print("SetVisibleCore", value);
		//	if(!_first) {
		//		_first = true;
		//		value = false;
		//		//CreateHandle();
		//		this.CreateControlNow();

		//		//void _CreateControls(Control parent)
		//		//{
		//		//	foreach(Control v in parent.Controls) {
		//		//		v.
		//		//	}
		//		//}
		//	}
		//	base.SetVisibleCore(value);
		//}

		protected override void WndProc(ref Message m)
		{
			//Print(m);

			base.WndProc(ref m);
		}
	}

	static void TestWndFinderCache()
	{
		var a = new AWnd.Finder[10];
		for(int i = 0; i < a.Length; i++) {
			a[i] = new AWnd.Finder("Quick *", "QM_Editor", "qm.exe");
			//a[i] = new AWnd.Finder("Quick *", "QM_Editor");
		}
		AWnd w = AWnd.Find("Quick *").OrThrow();

		//for(int j = 0; j < 5; j++) {
		//	int n = 0;
		//	APerf.First();
		//	var cache = new WFCache(true);
		//	for(int i = 0; i < a.Length; i++) {
		//		//if(a[i].IsMatch(w)) n++;
		//		if(a[i].IsMatch(w, cache)) n++;
		//	}
		//	APerf.NW();
		//	Print(n);
		//	200.ms();
		//}

		//speed: 500, LibGetNameCached 250, WFCache 115
		//JIT: 5.8, LibGetNameCached 6.5, WFCache 6.0


		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 10;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { var s = w.ProgramName; } //75/370, JIT 4.4;  cached: 80/135 , JIT 5.3
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var s = w.ProgramPath; } //250/2000;  cached: 250/310
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { var s = w.ProgramDescription; }
		//	//APerf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { var t = ATime.PerfMilliseconds; }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
	}

	//[DllImport("kernel32.dll")]
	//internal static extern int GetTickCount();
	[DllImport("winmm.dll")]
	internal static extern int timeGetTime();

	internal static double s_freqMCS, s_freqMS; //s_freqMCS used by APerf too
	static long s_lastCorrectionTime;

	//public static long Milliseconds {
	//	get {
	//		Api.QueryPerformanceCounter(out var t);

	//		var r = (long)(t * s_freqMS);
	//		var k = Api.GetTickCount64();
	//		var d = r - k;
	//		Print(s_freqMS, (double)k / t, d);
	//		if(d>100 || d < -100) {
	//			s_freqMS = (double)k / t;
	//			r = (long)(t * s_freqMS);
	//		}

	//		return r;
	//	}
	//}
	//public static long Milliseconds {
	//	get {
	//		Api.QueryPerformanceCounter(out var t);

	//		var r = (long)(t * s_freqMS);
	//		var k = Api.GetTickCount64();
	//		var d = r - k;
	//		//Print(s_freqMS, (double)k / t, d);
	//		if(d>100 || d < -100) {
	//			s_freqMS = (double)k / t;
	//			r = (long)(t * s_freqMS);
	//		}

	//		return r;
	//	}
	//}
	public static long Milliseconds {
		get {
			Api.QueryPerformanceCounter(out var t);

			var r = (long)(t * s_freqMS);
			if(s_lastCorrectionTime == 0 || r - s_lastCorrectionTime > 1000) {
				var k = Api.GetTickCount64();
				s_freqMS = (double)k / t;
				r = (long)(t * s_freqMS);
				s_lastCorrectionTime = r;
			}

			return r;
		}
	}

	static void TestMillisecondsSpeed()
	{
		//var t1 = ATime.PerfMilliseconds;
		//var t2 = ATime.WinMillisecondsWithoutSleep;
		//var t3 = ATime.WinMilliseconds64;
		//ADialog.Show();

		Print(ATime.WinMillisecondsWithoutSleep, ATime.PerfMilliseconds, Api.GetTickCount64(), timeGetTime());
		Print(ATime.WinMillisecondsWithoutSleep, ATime.PerfMilliseconds, Api.GetTickCount64(), timeGetTime());
		Print(ATime.PerfMilliseconds - Api.GetTickCount64());
		Print(Milliseconds, Milliseconds - Api.GetTickCount64());

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 10; i1++) {
			int n2 = 1000;
			APerf.First();
			//for(int i2 = 0; i2 < n2; i2++) { var t1 = Stopwatch.GetTimestamp(); }
			//APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var t1 = ATime.PerfMilliseconds; }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var t2 = ATime.WinMillisecondsWithoutSleep; }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var t3 = ATime.WinMilliseconds64; }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var t3 = Api.GetTickCount64(); }
			//APerf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { var t1 = Milliseconds; }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var t1 = Environment.TickCount; }
			APerf.NW();
			Thread.Sleep(200);
		}
	}

	static void TestHooks()
	{
		//AOutput.QM2.UseQM2 = true; AOutput.Clear();

		//using(var h = AHookWin.Keyboard(k => {
		//	Print($"{k.Key}, {!k.IsUp}");
		//	if(k.Key == KKey.Up && !k.IsUp) {
		//		1100.ms();
		//		Print("<");
		//	}

		//	return false;
		//})) ADialog.Show("hook");


		using(var h = AHookWin.Mouse(k => {
			if(k.IsWheel) {
				Print(">");
				800.ms();
				Print("<");
			}

			return false;
		})) ADialog.Show("hook");
	}

	static void TestStackTrace()
	{
		_ = typeof(System.Linq.Enumerable).Assembly;
		//_ = typeof(Form).Assembly;
		//Print(AppDomain.CurrentDomain.GetAssemblies());
		//ADialog.Show();
		//10.s();
		for(int i = 0; i < 5; i++) {
			200.ms();
			string s;
			APerf.First();
			var x = new StackTrace(0, true);
			APerf.Next();
			s = x.ToString();
			APerf.NW();
			//AOutput.QM2.Write("'" + s + "'");
			Print(s);

			//APerf.First();
			//try {
			//	throw new Exception("test");
			//}
			//catch(Exception e) {
			//	APerf.Next();
			//	s = e.ToString();
			//	APerf.NW();
			//	//AOutput.QM2.Write("'" + s + "'");
			//	Print(s);
			//}
		}
		//ADialog.Show();
	}

	struct TContext
	{
		object _o;
		TContext(object o) => _o = o;

		public static implicit operator TContext(AWnd.Finder f) => new TContext(f);
		public static implicit operator TContext(AWnd f) => new TContext(f);
		public static implicit operator TContext(Func<bool> f) => new TContext(f);
	}

	static void TestAbortThreadAndContinue()
	{
		var t = Thread.CurrentThread;
		AThread.Start(() => {
			for(int i = 0; i < 3; i++) {
				1.s();
				t.Abort();
			}
		});

		for(int i = 0; i < 3; i++) {
			try {
				for(int j = 0; ; j++) { 300.ms(); Print(j); }
			}
			catch(Exception e) {
				if(e is ThreadAbortException m) {
					Print("reset");
					Thread.ResetAbort();
				}
			}
		}
		Print("END");
	}

	static void TestDelegateInvoke()
	{
		//ThreadStart a1 = () => Print(1);
		//Action a2 = () => Print(2);
		ThreadStart a1 = () => Api.GetTickCount();
		Action a2 = () => Api.GetTickCount();

		_Tdi1(a1);
		_Tdi2(a2);

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			APerf.First();
			for(int i2 = 0; i2 < n2; i2++) { _Tdi1(a1); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { _Tdi2(a2); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			APerf.NW();
			Thread.Sleep(200);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Tdi1(ThreadStart a)
	{
		_Tdi3(a);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Tdi2(Action a)
	{
		_Tdi3(a.Invoke);
		//_Tdi3(()=>a()); //slower
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Tdi3(ThreadStart a)
	{
		a();
	}

	static void TestDelegateBeginInvoke()
	{
		Action a = () => Print(AThread.NativeId, Thread.CurrentThread.IsThreadPoolThread);

		a();
		var r = a.BeginInvoke(null, null);
		100.ms();
		a.EndInvoke(r);
		Print("END");
	}

	static void TestTasks()
	{
		var f = Task.Factory;
		var a = new Task[20];
		for(int i = 0; i < a.Length; i++) {
			int j = i;
			a[i] = f.StartNew(() => {
				var t = Thread.CurrentThread;
				if(t.GetApartmentState() != ApartmentState.STA) {
					if(t.TrySetApartmentState(ApartmentState.Unknown)) t.TrySetApartmentState(ApartmentState.STA);
				}
				Print(j, AThread.NativeId, t.IsThreadPoolThread, t.GetApartmentState());
			}, TaskCreationOptions.LongRunning);
			10.ms();
		}
		//500.ms();
		ADialog.Show();
	}

	//using System.Runtime.InteropServices;

	[DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
	internal static extern IntPtr SendMessageS(IntPtr hWnd, int Msg, uint wParam, string lParam);

	[DllImport("user32.dll", EntryPoint = "FindWindowW", CharSet = CharSet.Unicode)]
	internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	static void TestQm2SendMessage()
	{
		var hwnd = FindWindow("QM_Editor", null);
		if(hwnd == default(IntPtr)) return;
		SendMessageS(hwnd, 12, 1, "Q ' M 'Macro295' C test C#");
	}

	static void TestTaskSpeed()
	{
		Print("----");

		_ = typeof(Form).Assembly;
		//_ = typeof(System.Linq.Enumerable);
		//Print(AppDomain.CurrentDomain.GetAssemblies());
		AppDomain.CurrentDomain.AssemblyLoad += (a, b) => {
			var s = b.LoadedAssembly.GetName().Name;
			if(s == "System.Windows.Forms") {
				int stop = 1;
			}
			Print(s);
		};

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 5; i1++) {
			APerf.First();
			//Task.Run(() => APerf.NW()); //100
			//ThreadPool.QueueUserWorkItem(o => APerf.NW()); //90
			Au.Util.ThreadPoolSTA.SubmitCallback(null, o => APerf.NW()); //90
			Thread.Sleep(200);
		}
		//Print(AppDomain.CurrentDomain.GetAssemblies());
	}

	class ActionThread : IDisposable
	{
		struct _Action { public Action action; public long time; }

		Queue<_Action> _q;
		Action _running;
		IntPtr _event;
		bool _disposed;

		public void Run(Action ac, int ifRunningWaitMS, bool canWaitIfRunningOther = false)
		{
			if(_disposed) return;
			if(_q == null) {
				_q = new Queue<_Action>();
				_event = Api.CreateEvent(false);
				AThread.Start(() => {
					while(!_disposed && 0 == Api.WaitForSingleObject(_event, -1)) {
						while(!_disposed) {
							_Action x;
							lock(_q) {
								g1:
								if(_q.Count == 0) { _running = null; break; }
								x = _q.Dequeue();
								if(x.time != 0 && ATime.PerfMilliseconds > x.time) goto g1;
								_running = x.action;
							}
							x.action();
						}
					}
					Api.CloseHandle(_event);
					Print("thread ended");
				});
			}

			lock(_q) {
				if(_running != null) {
					if(ifRunningWaitMS == 0) return;
					if(_running != ac && !canWaitIfRunningOther) return;
				} else {
					_running = ac;
					//if(ifRunningWaitMS > 0 && ifRunningWaitMS < 1000000000) ifRunningWaitMS += 1000;
				}
				_q.Enqueue(new _Action { action = ac, time = ifRunningWaitMS <= 0 ? 0 : ATime.PerfMilliseconds + ifRunningWaitMS });
			}
			Api.SetEvent(_event);
		}

		public void Dispose()
		{
			if(_disposed) return; _disposed = true;
			GC.SuppressFinalize(this);
			Api.SetEvent(_event);
		}

		~ActionThread() => Dispose();
	}

	static void TestActionThread()
	{
		var x = new ActionThread();

		var f = new Form();

		Action eh = () => {
			APerf.First();
			//x.Run(() => ADialog.Show(), 2000);
			x.Run(() => { APerf.NW(); 1000.ms(); }, -1);
		};
		f.MouseDown += (unu, sed) => eh();
		f.KeyDown += (unu, sed) => eh();

		f.ShowDialog();

		x.Dispose();
		1.s();
		//ADialog.Show("main end");
	}

	static void TestListForeach()
	{
		var a = new List<string>(10);
		for(int i = 0; i < 10; i++) a.Add("asasa");
		var b = new string[10];
		for(int i = 0; i < 10; i++) b[i] = "asasa";

		for(int j = 0; j < 5; j++) {
			//ADebug.LibMemorySetAnchor();
			APerf.First();
			for(int i = 0; i < 1000; i++) {
				foreach(var v in b) if(v == "fff") Print("mmm");
			}
			APerf.Next();
			//ADebug.LibMemoryPrint();

			//ADebug.LibMemorySetAnchor();
			for(int i = 0; i < 1000; i++) {
				for(int k = 0; k < a.Count; k++) if(a[k] == "fff") Print("mmm");
			}
			APerf.Next();
			//ADebug.LibMemoryPrint();

			//ADebug.LibMemorySetAnchor();
			for(int i = 0; i < 1000; i++) {
				foreach(var v in a) if(v == "fff") Print("mmm");
			}
			APerf.NW();
			//ADebug.LibMemoryPrint();
			200.ms();
		}
	}

	static void TestCastDelegate()
	{
		//Action<string> func_string = s => Print(s);
		//Action<object> func_object;
		//func_object = (Action<object>)func_string;
		//func_object("te"); //System.InvalidCastException

		Action<object> func_object = s => Print(s);
		Action<string> func_string;
		func_string = func_object;
		func_string("te"); //OK
	}

	static void TestMulticastDelegate()
	{
		Action a = null;
		a += () => Print(1);
		Action b = a;
		a += () => Print(2);
		b += () => Print(3);
		Print("call a");
		a();
		Print("call b");
		b();
		Print("call b+a");
		b += a;
		b();
		Print("call b=a");
		b = a;
		b();
		//a.GetInvocationList

	}

	static void TestWndGroup()
	{
		//AWnd.Find("Quick*-").Activate();

		//var f = new AWnd.Finder("Quick*");
		//f.

		//Print(AWnd.WaitAny(0, false, new AWnd.Finder("Quick*"))).wnd;
		Print(AWnd.WaitAny(10, true, "Quick*,,,'LISTITEM' one, two, three", "*Notepad")).wnd;
		//Print(AWnd.WaitAny(10, true, "*one, two, three*\0", "*Notepad")).wnd;
		//Print(AWnd.WaitNot(10, "*Notepad"));
		//Print(AWnd.WaitNot(10, out _, ",,Notepad.exe"));
	}

	static void TestOutTuple(int k, out (bool, int) t)
	{
		t = (true, 1);
	}

	static Bitmap GetBitmap()
	{
		return null;
	}
}
