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
//using Au.Util;
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
		var file = @"Q:\test\test.txt";

		Task.Run(() =>
		{
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

		Task.Run(() =>
		{
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

	#region

	class TAttr
	{
		internal TAttr _next;
	}

	class TAttr<T> :TAttr
	{
		public T Value { get; set; }
		public static implicit operator T(TAttr<T> a) => a.Value;
		//public static implicit operator T(TAttr<T> a, T value) => a.Value = value;

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	class TAttrOne :TAttr<string> { }
	class TAttrTwo :TAttr<int> { }

	class TFile3
	{
		TFolder3 _parent;
		TFile3 _next;
		string _name;
		TAttr _lastAttr;

		//public void SetAttr<AttrType, ValueType>(ValueType value) where AttrType : XAttr
		//{
		//	var a = Attr<AttrType>();
		//	if(a == null) {

		//	}
		//	a.V
		//}

		//public T Attr<T>() where T : XAttr
		//{
		//	XAttr a = _lastAttr;
		//	if(a != null) {
		//		do {
		//			a = a._next;
		//			if(a is T r) return r;
		//		} while(a != _lastAttr);
		//	}
		//	return null;
		//}

		//public bool Attr<T>(out T value) where T : XAttr
		//{
		//	XAttr a = _lastAttr;
		//	if(a != null) {
		//		do {
		//			a = a._next;
		//			if(a is T r) { value = r; return true; }
		//		} while(a != _lastAttr);
		//	}
		//	value = default;
		//	return false;
		//}

		public T Attr<T>(bool add = false) where T : TAttr, new()
		{
			//find
			TAttr a = _lastAttr;
			if(a != null) {
				do {
					a = a._next;
					if(a is T r) return r;
				} while(a != _lastAttr);
			}
			if(!add) return null;
			//add
			var t = new T();
			if(_lastAttr != null) {
				t._next = _lastAttr._next; //= first
				_lastAttr._next = t;
			} else {
				t._next = t;
			}
			_lastAttr = t;
			return t;
		}

		public void SetAttr<T>(ETA attr, T value)
		{

		}

		//public bool GetAttr<T>(ETA attr, out T value)
		//{

		//}
	}

	enum ETA { One, Two }

	class TFolder3 :TFile3
	{
		TFile3 _firstChild;
	}

	static void TestXFile2()
	{
		//var f = new TFile();
		////f.SetAttr<XAttrOne>("test");
		////f.SetAttr<XAttrOne, string>("test");
		//f.Attr<TAttrOne>(true).Value = "test";
		//f.Attr<TAttrTwo>(true).Value = 5;
		//f.Attr<TAttrTwo>(true).Value = 7;
		//string s = f.Attr<TAttrOne>();
		//int i = f.Attr<TAttrTwo>();
		//Print(s, f.Attr<TAttrOne>(), i, f.Attr<TAttrTwo>());

		//Debug_.LibMemorySetAnchor();

		////for(int i = 0; i < 1024; i++) {
		////	var v = new TFile2();
		////	v._id = i;
		////	v._flags = i;
		////	v._name = new string('a', 10);
		////	//v.Value = null;

		////	//var a = new TAttrOne[4];
		////}

		//var d = new Dictionary<int, TFile2>(1024);

		//Debug_.LibMemoryPrint();
	}

	#endregion

#if false
	class TFile
	{
		_Folder _parent;
		TFile _next;
		int _id;
		_F _flags;
		string _name;
		object _misc; //null or rarely used data

		class TMisc
		{
			public string iconOrLinkTarget;
			public object run; //string id or TFile
		}

		[Flags]
		enum _F
		{
			Folder = 1,
			Link = 2,
		}

		TFile() { } //for _Folder

		//reads XML attributes and sets fields except _next
		TFile(XmlReader r, bool isFolder)
		{
			if(isFolder) _flags = _F.Folder;
			string linkTarget = null, iconPath = null, runId = null;
			do {
				//Print(r.Name, r.Value);
				string an = r.Name, av = r.Value;
				//Print(an, av);
				if(av.Length == 0) throw new XmlException("empty attribute " + an);
				switch(an) {
				case "n":
					_name = av;
					break;
				case "i":
					_id = av.ToInt_();
					break;
				case "f":
					_flags |= (_F)av.ToInt_() & ~(_F.Folder | _F.Link);
					break;
				case "path":
					if(!isFolder) { linkTarget = av; _flags |= _F.Link; }
					break;
				case "icon":
					iconPath = av;
					break;
				case "run":
					if(!isFolder) runId = av;
					break;
				default:
					PrintWarning("unknown XML attribute " + an, -1);
					continue;
				}
			} while(r.MoveToNextAttribute());
			if(Empty(_name)) throw new XmlException("no n attribute");
			//if(_id == 0) throw new XmlException("no i attribute"); //no, it may be exported, then will need new id

			var iconOrLink = linkTarget ?? iconPath;
			if(runId != null) {
				var m = new TMisc { run = runId, iconOrLinkTarget = iconOrLink };
				_misc = m;
			} else _misc = iconOrLink;

			//Print(this);
		}

		public static TFile Load(string file)
		{
			Perf.Next();
			var xs = new XmlReaderSettings() { IgnoreComments = true, IgnoreProcessingInstructions = true, IgnoreWhitespace = true };
			using(var r = File_.WaitIfLocked(() => XmlReader.Create(file, xs))) {
				Perf.Next();
				r.MoveToContent();
				var root = new _Folder();
				root.ReadContent(r);
				Perf.NW();
				return root;
			}
		}

		class _Folder :TFile
		{
			internal TFile _lastChild;

			//root node
			public _Folder()
			{
				_flags = _F.Folder;
				//_next = this;
			}

			public _Folder(XmlReader x) : base(x, true) { }

			public void ReadContent(XmlReader r)
			{
				var parent = this;
				//int n = 0;
				while(r.Read()) {
					//Print(r.NodeType);
					var nodeType = r.NodeType;
					if(nodeType == XmlNodeType.Element) {
						//Print($"{new string(' ', r.Depth)}{r.Name} {(r["n"])}");
						bool isFolder = false;
						string tag = r.Name;
						switch(tag) {
						case "f": break;
						case "d": isFolder = true; break;
						default: throw new XmlException("unknown tag " + tag);
						}
						if(!r.MoveToFirstAttribute()) throw new XmlException("no attributes");
						TFile f; _Folder d = null;
						if(isFolder) f = d = new _Folder(r);
						else f = new TFile(r, false);
						parent.Add(f);
						r.MoveToElement();
						if(!r.IsEmptyElement) {
							if(isFolder) parent = d;
							else if(!r.Read() || r.NodeType != XmlNodeType.EndElement) throw new XmlException("f element with content"); //can be <f ...></f>
						}
						//n++;
					} else if(nodeType == XmlNodeType.EndElement) {
						if(parent == this) break;
						parent = parent._parent;
					}
				}
				//Print(n);
			}

			public void Add(TFile f)
			{
				Debug.Assert(IsFolder);
				Debug.Assert(f != null);
				Debug.Assert(f._parent == null);
				Debug.Assert(f != RootAncestor);
				f._parent = this;
				var last = _lastChild;
				if(last == null) { //our first child!
					f._next = f; //f now is LastChild and FirstChild
				} else {
					f._next = last._next; //_next of LastChild is FirstChild
					last._next = f;
				}
				_lastChild = f;
			}

			public void Remove(TFile f)
			{
				Debug.Assert(f._parent == this);
				TFile p = _lastChild;
				while(p._next != f) p = p._next;
				if(p == f) {
					_lastChild = null;
				} else {
					if(_lastChild == f) _lastChild = p;
					p._next = f._next;
				}
				f._parent = null;
				f._next = null;
			}
			//info: code from XContainer.RemoveNode
		}

		public bool IsFolder => 0 != (_flags & _F.Folder);

		public bool IsLink => 0 != (_flags & _F.Link);

		/// <summary>
		/// Returns parent, or null if this is Root.
		/// </summary>
		public TFile Parent => _parent;

		/// <summary>
		/// Returns the root ancestor. Its Parent is null.
		/// If this is root, returns this.
		/// </summary>
		public TFile RootAncestor
		{
			get
			{
				var p = this;
				while(p._parent != null) p = p._parent;
				return p;
				//TODO: ModelRoot => _model.Root
			}
		}

		/// <summary>
		/// Gets the last child of this folder, or null if no children.
		/// </summary>
		public TFile LastChild => (this as _Folder)?._lastChild;

		/// <summary>
		/// Gets the first child of this folder, or null if no children.
		/// </summary>
		public TFile FirstChild => LastChild?._next;

		public TFile Next => _parent == null || this == _parent._lastChild ? null : _next;
		//info: code from XContainer.NextNode

		public TFile Previous
		{
			get
			{
				if(_parent == null) return null;
				TFile n = _parent._lastChild._next;
				TFile p = null;
				while(n != this) {
					p = n;
					n = n._next;
				}
				return p;
			}
		}
		//info: code from XContainer.PreviousNode

		public IEnumerable<TFile> Children(bool andSelf)
		{
			if(andSelf) yield return this;
			var last = LastChild;
			if(last != null) {
				var f = last;
				do {
					f = f._next;
					yield return f;
				} while(f != last);
			}
		}
		//info: code from XContainer.Nodes

		public IEnumerable<TFile> Descendants(bool andSelf)
		{
			if(andSelf) yield return this;
			var last = LastChild;
			var f = this;
			while(true) {
				TFile first;
				if(f.IsFolder && (first = f.FirstChild) != null) {
					f = first;
				} else {
					while(f != null && f != this && f == f._parent._lastChild) f = f._parent;
					if(f == null || f == this) break;
					f = f._next;
				}
				yield return f;
			}
		}
		//info: code from XContainer.GetDescendantNodes

		public void Remove() => _parent?.Remove(this);

		/// <summary>
		/// Returns link target path.
		/// Don't call for non-links.
		/// </summary>
		public string LinkTarget
		{
			get
			{
				Debug.Assert(IsLink);
				if(_misc is string s) return s;
				return (_misc as TMisc).iconOrLinkTarget;
			}
		}

		/// <summary>
		/// Returns custom icon path or link target path or null.
		/// For links always returns null; use LinkTarget.
		/// </summary>
		public string IconPath
		{
			get
			{
				if(!IsLink)
					switch(_misc) {
					case string s: return s;
					case TMisc m: return m.iconOrLinkTarget;
					}
				return null;
			}
		}

#if DEBUG
		public override string ToString()
		{
			string fd = IsFolder ? "d" : "f";
			return $"{fd} {_name} {_id}";
		}
#endif

		void _SaveNode(XmlWriter x)
		{
			bool isFolder = IsFolder;
			x.WriteStartElement(isFolder ? "d" : "f");
			//name, id
			x.WriteAttributeString("n", _name);
			if(_id != 0) x.WriteAttributeString("i", _id.ToString());
			//icon path or link target
			string icon;
			if(IsLink) x.WriteAttributeString("path", LinkTarget);
			else if((icon = IconPath) != null) x.WriteAttributeString("icon", icon);
			//run
			if(_misc is TMisc m) {
				string runId = null;
				switch(m.run) {
				case string s: runId = s; break;
				case TFile fr: runId = fr._id.ToString(); break;
				}
				if(runId != null) x.WriteAttributeString("run", runId);
			}
			//folder descendants
			if(isFolder) {
				foreach(var v in Children(false)) v._SaveNode(x);
			}
			x.WriteEndElement();
		}

		public void Save(XmlWriter x)
		{
			Debug.Assert(_parent == null && IsFolder); //must be root
			x.WriteStartDocument();
			x.WriteStartElement("files");
			foreach(var v in Children(false)) v._SaveNode(x);
			x.WriteEndDocument();
		}

		public void Save(string file)
		{
			var sett = new XmlWriterSettings() {
				OmitXmlDeclaration = true,
				Indent = true,
				IndentChars = "\t"
			};
			File_.Save(file, temp =>
			{
				using(var x = XmlWriter.Create(temp, sett)) {
					Save(x);
				}
			});
		}
	}
#endif

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

			//Perf.Cpu();
			//TNodeExample();
			TestTFile();

			//Perf.Cpu();
			//Perf.First();
			//TestSqlite();
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
