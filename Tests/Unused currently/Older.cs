﻿using System;
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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;


//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

//using Catkeys.Triggers;


//using Cat = Catkeys.Input;
//using Meow = Catkeys.Show;

//using static Catkeys.Show;

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
//using System.Data.SQLite;
using SQLite;

//using CsvHelper;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

using Microsoft.Win32.SafeHandles;

using System.IO.Compression;
using System.Reflection.Emit;

//using static Test.CatAlias;

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field


static partial class Test
{

	static void TestAppDomainDoCallback(AppDomain d)
	{
		try {
			d.DoCallBack(TestMain);
		}
		catch(ThreadAbortException e) { Thread.ResetAbort(); }
		catch(Exception e) { PrintList("exception executing AppDomain", e); }
		Print("DoCallBack ended");
	}

	static void TestAppDomainUnload(AppDomain d)
	{
		try { AppDomain.Unload(d); } catch(Exception e) { PrintList("exception unloading AppDomain", e); }
		Print("Unload ended");
	}


	#region test sqlite

#if false
	static void TestSqlite()
	{
		var sb = new StringBuilder();

		Perf.First();
		var file = Folders.Temp + "test.db3";
		bool isNew = !Files.FileExists(file);
		if(isNew) SQLiteConnection.CreateFile(file);
		Perf.Next();

		SQLiteConnection m_dbConnection;
		m_dbConnection = new SQLiteConnection($"Data Source={file};Version=3;");
		m_dbConnection.Open();
		Perf.Next();
		try {
			string sql;

			if(isNew) {
				sql = "create table highscores (name varchar(20), score int)";
				using(var c = new SQLiteCommand(sql, m_dbConnection)) c.ExecuteNonQuery();

				using(var tran = m_dbConnection.BeginTransaction()) {
					sql = "insert into highscores (name, score) values ('Me', 9001)";
					using(var c = new SQLiteCommand(sql, m_dbConnection)) c.ExecuteNonQuery();

					tran.Commit();
				}
				Perf.Next();
			}

			for(int i = 0; i < 5; i++) {
				sql = "select * from highscores order by score desc";
				using(var c = new SQLiteCommand(sql, m_dbConnection)) {
					using(var reader = c.ExecuteReader())
						while(reader.Read())
							sb.AppendLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
				}
				Perf.Next();
			}
		}
		finally {
			m_dbConnection.Close();
		}
		Perf.NW();
		Print(sb);
	}
#elif false
	static void TestSqlite()
	{
		var sb = new StringBuilder();

		Perf.First();
		var file = Folders.Temp + "test3.db3";
		bool isNew = !Files.FileExists(file);
		//if(isNew) SQLiteConnection.CreateFile(file);
		Perf.Next();

		Perf.NW();
		Print(sb);
	}
#else

	public class Stock
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[MaxLength(8)]
		public string Symbol { get; set; }
	}

	const string LibraryPath = "Sqlite.Interop.dll";
	static void TestSqlite()
	{
		var sb = new StringBuilder();

		Perf.First();
		var file = Folders.Temp + "test2.db3";
		bool isNew = !Files.ExistsAsFile(file);
		Perf.Next();

		using(var db = new SQLiteConnection(file)) {
			Perf.Next();

			if(isNew) {
				db.CreateTable<Stock>();

				var s = db.Insert(new Stock() {
					Symbol = "one"
				});
				Perf.Next();
			}

			for(int i = 0; i < 5; i++) {
#if true
				//var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

				var query = db.Table<Stock>();
				foreach(var stock in query)
					sb.AppendLine("Stock: " + stock.Symbol);
#else
				foreach(var stock in db.Query<Stock>("select * from Stock"))
					sb.AppendLine("Stock: " + stock.Symbol);
#endif
				Perf.Next();
			}
		}
		Perf.NW();

		Print(sb);
	}
#endif

	static void TestSqlite2()
	{
	}

	#endregion


	//using l = Catkeys;
	//using static Catkeys.NoClass;
	////
	//using System.Collections.Generic;
	//using SysText = System.Text;
	//using SysRX = System.Text.RegularExpressions;
	//using SysDiag = System.Diagnostics;
	//using SysInterop = System.Runtime.InteropServices;
	//using SysCompil = System.Runtime.CompilerServices;
	//using SysIO = System.IO;
	//using SysThread = System.Threading;
	//using SysTask = System.Threading.Tasks;
	//using SysReg = Microsoft.Win32;
	//using SysForm = System.Windows.Forms;
	//using SysDraw = System.Drawing;
	////using System.Linq;



	//using SysColl = System.Collections.Generic; //add directly, because often used, and almost everything is useful
	//using SysCompon = System.ComponentModel;
	//using SysExcept = System.Runtime.ExceptionServices; //only HandleProcessCorruptedStateExceptionsAttribute

	public static class CatAlias
	{
		public static int speed { get => Options.MouseClickSleep; set => Options.MouseClickSleep = value; }
	}

	static unsafe void TestSharedMemory()
	{
		try {
			var m1 = (int*)Catkeys.Util.SharedMemory.CreateOrGet("test", 1000, out var _);
			Print((long)m1);
			*m1 = 7;
			//var m2 = (int*)Catkeys.Util.SharedMemory.CreateOrGet("test", 1000000);
			//Print((long)m2);
			//Print(*m2);



			TaskDialog.Show("");
		}
		catch(Exception e) { Print(e); }
	}


	//[DllImport("kernel32.dll")]
	//public static unsafe extern int CompareStringOrdinal(char* lpString1, int cchCount1, char* lpString2, int cchCount2, bool bIgnoreCase);

	//public const uint FIND_FROMSTART = 0x400000;
	//public const uint FIND_FROMEND = 0x800000;
	//public const uint FIND_STARTSWITH = 0x100000;
	//public const uint FIND_ENDSWITH = 0x200000;

	//[DllImport("kernel32.dll")]
	//public static unsafe extern int FindStringOrdinal(uint dwFindStringOrdinalFlags, char* lpStringSource, int cchSource, char* lpStringValue, int cchValue, bool bIgnoreCase);

	//public static unsafe int IndexOf2_(this string t, string s, bool ignoreCase = false)
	//{
	//	//speed: similar to string.IndexOf. In some cases faster, in others slower.
	//	fixed (char* s1 = t, s2 = s) {
	//		return FindStringOrdinal(FIND_FROMSTART, s1, t.Length, s2, s.Length, ignoreCase);
	//	}
	//}

	static void _LikeAssert(string p, string s, bool r)
	{
		Debug.Assert(s.Like_(p) == r);
		//Debug.Assert(s.LikeEx_(p) == r);
	}

	static bool TestLikeEx_(this string t, string pattern, bool ignoreCase = false)
	{
		return Operators.LikeString(t, pattern, ignoreCase ? CompareMethod.Text : CompareMethod.Binary);
	}

	static void TestWildcard()
	{
		string s = null, p = null;
		//s = "Microsoft Help Ąč Viewer 2.2 - Visual Studio Documentation Ąč";
		//s = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD Documentation Ąč";
		s = "Microsoft Help Ąč Viewer 2.2 - Visual Studio Documentation Ąč";
		//s = "short";
		//s += s;s += s;
		p = "* Documentation Ąč";
		p = "*ocumentation Ąč";
		p = "Microsoft Help Ąč *";
		p = "*?ocumentation*";
		p = "* Help Ąč Viewer ?.? -*";
		p = "* Help Ąč Viewer 2.2 - Visual Studio Documentation *";
		p = "* Help Ąč Viewer ?.? -* Documentation Ąč";
		p = "*ocumentation*";

		//s = "{267F16E4-020F-445C-9380-EA4D94291F77}";
		//p = "{*-*-*-*-*}";
		//p = s.Insert(0, " ").Remove(0, 1); //use this instead of p = s;, which makes Equals_ etc much faster, cannot compare speed
		//p = "*Microsoft Help Ąč Viewer 2.2+ - Visual Studio Documentation Ąč";
		//Debug.Assert((object)p != (object)s);
		//p = p.ToLower_();
		//p = p + "*";

		//s = "A 	B 	C 	Ç 	D 	E 	F 	G 	Ğ 	H 	I 	İ 	J 	K 	L 	M 	N 	O 	Ö 	P 	R 	S 	Ş 	T 	U 	Ü 	V 	Y 	Z";
		//p = "a 	b 	c 	ç 	d 	e 	f 	g 	ğ 	h 	ı 	i 	j 	k 	l 	m 	n 	o 	ö 	p 	r 	s 	ş 	t 	u 	ü 	v 	y 	z";

		bool ignoreCase = false;

		//Print(s.Like_(p, ignoreCase)); return;

#if !DEBUG
		bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false, b6 = false;
		//var a1 = new Action(() => { b1 = s.LikeEx_(p, ignoreCase); });
		var a2 = new Action(() => { b2 = s.Like_(p, ignoreCase); });
		//var a6 = new Action(() => { b6 = s.Like2_(p, ignoreCase); });
		var p2 = p.Replace("*", "");
		var a3 = new Action(() => { b3 = s.Equals_(p2, ignoreCase); });
		var a4 = new Action(() => { b4 = s.StartsWith_(p2, ignoreCase); });
		var a5 = new Action(() => { b5 = s.IndexOf_(p2, ignoreCase) >= 0; });
		Perf.ExecuteMulti(5, 1000, a2, a3, a4, a5);
		PrintList(b2, b3, b4, b5);
#else
		//_LikeAssert("A*?", "AB", false);

		//_LikeAssert("A**", "A*", true);
		//_LikeAssert("**A", "*A", true);
		//_LikeAssert("A**B", "A*B", true);
		//_LikeAssert("A**B*C*?D", "A*BZZZC?D", true);
		//_LikeAssert("A**", "AB", false);
		//_LikeAssert("**A", "BA", false);
		//_LikeAssert("A**B", "ABB", false);

		//_LikeAssert("A*?", "A?", true);
		//_LikeAssert("*?A", "?A", true);
		//_LikeAssert("A*?B", "A?B", true);
		//_LikeAssert("A*?", "AB", false);
		//_LikeAssert("*?A", "BA", false);
		//_LikeAssert("A*?B", "ABB", false);

		_LikeAssert("A*", "A", true);

		_LikeAssert("A?*", "A", false);
		_LikeAssert("A?*", "ABBCC", true);
		_LikeAssert("A?*", "BAA", false);
		_LikeAssert("A?*B", "AZZB", true);
		_LikeAssert("A?*B", "AAABBB", true);

		_LikeAssert("A*ZB", "AZBnnnAB", false);

		_LikeAssert("A*B", "ABAB", true);
		//_LikeAssert("A*?", "ABAB", true);
		_LikeAssert("A?", "AAB", false);
		_LikeAssert("A*B", "AABA", false);

		_LikeAssert("", "", true);
		_LikeAssert("*", "", true);
		_LikeAssert("*", "A", true);
		_LikeAssert("", "A", false);
		_LikeAssert("A*", "", false);
		_LikeAssert("A*", "AAB", true);
		_LikeAssert("A*", "BAA", false);
		_LikeAssert("A*B", "", false);
		_LikeAssert("A*B", "AAB", true);
		_LikeAssert("A*B", "AB", true);
		_LikeAssert("A*B", "ABBBB", true);
		_LikeAssert("A*B*C", "", false);
		_LikeAssert("A*B*C", "ABC", true);
		_LikeAssert("A*B*C", "ABCC", true);
		_LikeAssert("A*B*C", "ABBBC", true);
		_LikeAssert("A*B*C", "ABBBBCCCC", true);
		_LikeAssert("A*B*C", "ABCBBBCBCCCBCBCCCC", true);
		_LikeAssert("A*B*", "AB", true);
		_LikeAssert("A*B*", "AABA", true);
		_LikeAssert("A*B*", "ABAB", true);
		_LikeAssert("A*B*", "ABBBB", true);
		_LikeAssert("A*B*C*", "", false);
		_LikeAssert("A*B*C*", "ABC", true);
		_LikeAssert("A*B*C*", "ABCC", true);
		_LikeAssert("A*B*C*", "ABBBC", true);
		_LikeAssert("A*B*C*", "ABBBBCCCC", true);
		_LikeAssert("A*B*C*", "ABCBBBCBCCCBCBCCCC", true);
		_LikeAssert("A?B", "AAB", true);

		_LikeAssert("*ZZ*", "AZZB", true);
		_LikeAssert("*AZZ*", "AZZB", true);
		_LikeAssert("*ZZB*", "AZZB", true);

		_LikeAssert("A**B", "AZB", true);
		_LikeAssert("**AB", "AB", true);
		_LikeAssert("AB**", "AB", true);
		_LikeAssert("A*B**", "AZB", true);
		_LikeAssert("**A*B", "AZB", true);

		_LikeAssert("A*ABAZB", "AZB", false);

		Print("ok");
#endif
	}

	static void TestRegexEtcSpeed()
	{
		bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false, b6 = false, b7 = false, b8 = false;

		var rx = @"One Two Three Four Five One Two Three Four Five One Two Three Four Five One Two Three Four Five One Two Three Four Five ";
		var r1 = new Regex(rx);
		var r2 = new Regex(rx, RegexOptions.CultureInvariant);
		var r3 = new Regex(rx, RegexOptions.IgnoreCase);
		var r4 = new Regex(rx, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		string s = (" " + rx).Remove(0, 1);

		var a1 = new Action(() => { b1 = r1.IsMatch(s); });
		var a2 = new Action(() => { b2 = r2.IsMatch(s); });
		var a3 = new Action(() => { b3 = r3.IsMatch(s); });
		var a4 = new Action(() => { b4 = r4.IsMatch(s); });
		var a5 = new Action(() => { b5 = s.Like_(rx); });
		var a6 = new Action(() => { b6 = s.Equals_(rx); });
		var a7 = new Action(() => { b6 = s.Equals_(rx, true); });
		var a8 = new Action(() => { b7 = s.TestLikeEx_(rx); });
		var a9 = new Action(() => { b8 = s.TestLikeEx_(rx, true); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7, a8, a9);

		PrintList(b1, b2, b3, b4, b5, b6, b7, b8);
	}

	static void TestWildex()
	{
		////Find item whose name contains "example" and date starts with "2017-". Case-insensitive.
		//var x = Find3("[P]example", "2017-*");

		//Print(Find3("two", ""));
		//Print("end");

		//TODO: test Wnd.GetAll with LINQ from etc.

		string s = null, p = null;

#if false
		s = @"C";
		//p = s.Insert(0, "m").Remove(0, 1);
		p = s;

		bool b1 = false, b2 = false, b3 = false, b4 = false;
		string s1 = null, s2 = null;

		var w1 = new Wildex(p);
		var w2 = new String_.Wildex3(p);

		int n = 1000;
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b1 = w1.Match(s); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b2 = w2.Match(s); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b1 = Cmp1(s, p); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b2 = Cmp2(s, p); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { s1 = w1.Text; } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { s2 = w2.Text; } Perf.Next(); } Perf.Write();

		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { var t=new TOM1() { w = new Wildex(p) }; b3 = t.w.Match(s); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { var t = new TOM2() { w = new String_.Wildex3(p) }; b4 = t.w.Match(s); } Perf.Next(); } Perf.Write();

		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b1 = Cmp3(s, p); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b2 = Cmp4(s, p); } Perf.Next(); } Perf.Write();


		//var a1 = new Action(() => { b1 = w1.Match(s); });
		//var a2 = new Action(() => { b2 = w2.Match(s); });
		//var a3 = new Action(() => { var w3 = new Wildex(p); b3 = w3.Match(s); });
		//var a4 = new Action(() => { var w4 = new String_.Wildex3(p); b4 = w4.Match(s); });
		////var a4 = new Action(() => { String_.Wildex3 w4 = p; b4 = w4.Match(s); });
		//var a5 = new Action(() => { s1 = w1.Text; });
		//var a6 = new Action(() => { s2 = w2.Text; });
		////Perf.ExecuteMulti(5, n, a1, a2, a3, a4, a5, a6);
		//Perf.ExecuteMulti(5, n, a1, a3);
		//Print("");
		//Perf.ExecuteMulti(5, n, a2, a4);

		PrintList(b1, b2, b3, b4, s1, s2);
#else
		s = @"C:\*a\b.exe";
		p = @"*.exE";
		p = @"[c]*.exE";
		p = @"C:\*\b.exe";
		p = @"[L]C:\*\b.exe";
		p = @"[R]^c.+\.exe$";
		p = @"[n]*.exE";
		p = @"[m]one[]*.exE[][n]d*";
		//p = ""; s = "";

		var x = new Wildex(p);
		PrintList($"i={x.IgnoreCase}, n={x.Not}, type={x.TextType}, {x.ToString()}");
		Print(x.Match(s));
#endif
	}

	static void TestWndWithWildex()
	{

		//Wnd w = Wnd.Find("[E]Notepad", "NotepaD", "notepaD");
		//Wnd w = Wnd.Find("[E]Notepad", "NotepaD", Folders.SystemX86+"notepaD.exe", WFFlags.ProgramPath);
		//Print(w);
		//Wnd w = Wnd.Find("qm message", prop: new Wnd.WinProp() { childName = "one" });
		//Wnd w = Wnd.Find("qm message", also: e => e.Child("two") != default(Wnd));
		//Wnd w = Wnd.Find("qm message", also: e => e.HasChild("two"));
		//Wnd w = Wnd.Misc.AllWindows

		var s = "Help - Sandcastle Help File Builder";
		//Wildex x = "**c|*sandcastle*";
		//Print(x.Match(s));
		//Print(s.Like_("*sandcastle*"));
		//return;

		//var w = Wnd.Find("**c|*Sandcastle*", null, "SAND*");
		//var w = Wnd.Find("**t|" + s, null, "SAND*");
		var w = Wnd.Find(@"**r|\bsandcastle\b.+$", null, "SAND*");
		Print(w);
		if(w.Is0) return;
		Wnd c;
		//c = w.ChildByClass("*COMBO*");
		//Print(c);
		c = w.ChildById(67022);
		Print(c);
		c = w.Child("*chm*", "*static*");
		//c = w.Child(null, "*combo*");
		Print(c);

		//w = Wnd.Find(prop: new Wnd.WinProp() { childName = "Open *" });
		//w = Wnd.Find(prop: new Wnd.WinProp() { childClass = "QM_Code" });
		Print(w);
		Print("----");
		//Print(Wnd.ThreadWindows(w.ThreadId, "**m|**n|tooltips_class32[]**nc|*IME*"));
		//Print(Wnd.ThreadWindows(w.ThreadId, "**mn|tooltips_class32[]**c|*IME*"));

		//Print(Process_.GetProcessesByName("explorer"));
		//Print(Process_.GetProcessesByName(new Wildex("[m]notepad[]explorer[]qm")));
		//Print(Process_.GetProcessesByName(Wildex.OptimizedCreate("[m]notepad[]explorer[]qm")));

		//Print(TaskDialog.ShowInputEx("aa"));

		//Print(w);
		//w.Activate();

		//Print(Wnd.AllWindows("[m]*o*[][nP][", true));
		//Print(Wnd.
		//String_.Join(",", "", "");
		Print("fin");
	}

	static void Nooook()
	{
		if(true) {
			Print(1);
		}
	}

#if tuples
	//static (int, int) TestReturnTuple()
	static (int a, int b) TestReturnTuple()
	{
		//var r = (3, 4);
		//var r = (c: 3, d: 4);
		(int a, int b) r = (3, 4);
		r = (5, 6);
		return r;
		return (1, 2);
	}
#endif

	static void TestCSharpSeven()
	{


#if pattern_matching
		object o = "text";
		if(o is string s) Print(s);

		switch(o) {
		case string k:
			Print(k);
			break;
		}
#endif

#if nested_functions
		int i = 5;
		Print(NestedFunc(5));

		int NestedFunc(int c) { return c * c + i; }
#endif

#if tuples
		//var t = TestReturnTuple();
		////PrintList(t.Item1, t.Item2);
		//PrintList(t.a, t.b);

		var (a, b) = TestReturnTuple();
		PrintList(a, b);

		//var v = Catkeys.Util.AppDomain_.GetDefaultDomain();
#endif

#if binary_literals_and_digit_separators
		int i = 0b10_1;
		Print(i);
#endif

	}

	static void TestTaskDialog2()
	{
		//if(TaskDialog.Show("Text", null, TDButtons.OKCancel, TDIcon.Info) != TDResult.OK) return;
		Print("ok");

		//Wnd w = Wnd.Find("");
		//int i = 123;
		//Print(i);
		//Regex rx;

		//switch(TaskDialog.Show("Save changes?", "More info.", customButtons: "1 Save|2 Don't Save|Cancel")) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}


		//switch(BaskDialog.Show("Save changes?|More info.", "1 Save|2 Don't Save|3 Cancel")) {
		//switch(BaskDialog.Show("Save changes?|More info.", "Yes|No|Cancel")) {
		////switch(BaskDialog.Show("Save changes?|More info.", BaskDialog.YesNoCancel)) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}
		//switch(BaskDialog.Show("Save changes?", "info", 0, "1 Save|2 Don't Save|Cancel")) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}

		//TaskDialog.Show("text", icon: TDIcon.App);

		//switch(TaskDialog.Show("Save changes?",	"info", customButtons:"1 Save|2 Don't Save|Cancel")) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}

		//TaskDialog.Show("test", null, TDButtons.OKCancel | TDButtons.YesNo | TDButtons.Retry | TDButtons.Close);
		////TaskDialog.Show("test", customButtons:"1 Gerai|2 Kartoti");

		//switch(TaskDialog.Show("test", null, TDButtons.OKCancel)) {
		//case TDResult.OK: break;
		//}

		//switch(TaskDialog.Show("test", null, "1 OK|2 Cancel")) {
		//case 1: break;
		//}

		//DebugDialog("aaa");

		//switch(TaskDialog.Show("Header|Comment", "1 OK|2 Cancel")) {
		//switch(TaskDialog.Show("Header|Comment", "1 O|2 C|3 X")) {
		////switch(TaskDialog.Show("Header|Comment", "-1 OK|-2 Cancel")) {
		//case 1: Print(1); break;
		//case 2: Print(2); break;
		//}
		//TDButtons.

		//if(!TaskDialog.Debug("text", TDButtons.OKCancel)) return;
		//Print("OK");

		//TaskDialog.ShowInputEx("Big text.", "Small text bbbbhhhh gggg ccc.");
		//TaskDialog.ShowInputEx("Big text.");
		//TaskDialog.ShowInputEx("Big text.", editType:TDEdit.Multiline);
		//TaskDialog.ShowInputEx("Big text.", "Small jjj.", editType:TDEdit.Multiline);

		//string s;
		//if(!TaskDialog.ShowInput(out s, "Example")) return;
		//Print(s);

		//int i;
		//if(!TaskDialog.ShowInput(out i, "Example.")) return;
		//Print(i);

		//var r = TaskDialog.ShowInputEx("Header.", "Comments.", checkBox: "Check");
		//if(r.Button != TDResult.OK) return;
		//PrintList(r.EditText, r.IsChecked);


		//var d = new OpenFileDialog();
		//if(d.ShowDialog() != DialogResult.OK) return;
		//Print(d.FileName);

		//var d = new FolderBrowserDialog();
		//if(d.ShowDialog() != DialogResult.OK) return;
		//Print(d.SelectedPath);

		//var d = new TaskDialog();
		//d.ShowDialog();

		//var ad = AppDomain.CreateDomain("test");
		//ad.ExecuteAssembly(@"Q:\app\Catkeys\Test Projects\Test\Test.exe");

		//TaskDialog.Options.DefaultTitle = "DEFAULT";
		//TaskDialog.Options.RtlLayout = true;
		//TaskDialog.Options.ScreenIfNoOwner = 2;
		//TaskDialog.Options.TopmostIfNoOwnerWindow = true;
		//TaskDialog.Options.UseAppIcon = true;
		//TaskDialog.Show("text");

		//TaskDialog.Show("", "TODO: start to drag panel only when mouse moved outside its caption. Move tab button without undocking.", 0, TDIcon.Info, TDFlags.Wider);
		//TaskDialog.Show("", "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW123456789 EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");

		//TaskDialog.ShowNoWaitEx(null, "Text.", backgroundThread:true);
		//TaskDialog.ShowNoWaitEx(null, "Text.");

		//TaskDialog.Show("one");
		//Print(TaskDialog.Show("Text1", "Text2", TDButtons.OKCancel, TDIcon.Info, TDFlags.CommandLinks|TDFlags.OwnerCenter, TDResult.Cancel, "1 one|2 two", Wnd.FindFast("Notepad")));
		//Print(TaskDialog.ShowEx("Text1", "Text2", TDButtons.OKCancel, TDIcon.Info, TDFlags.CommandLinks, 0,
		//	"1 one|2 two", "11 rone|12 rtwo", "check|checked", "expanded", "x|footer", "TITLE", null, 100, -10, 10));
		//TaskDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: ed => { Print(ed.LinkHref); });

		//string s; int i;

		//var d = new TaskDialog("", "Aby", TDButtons.OKCancel, TDIcon.Info, flags: TDFlags.CommandLinks, radioButtons: "One", customButtons: "One", expandedText: "Exp");
		//d.SetExpandedText()
		//d.FlagShowProgressBar = true;
		//d.ShowDialog();

		//if(Util.LibDebug_.IsScrollLock) Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Combo, expandedText: "one\ntwo"));
		//else Print(TaskDialog.ShowInputEx("test", "Aby"));

		//TaskDialog.Options.RtlLayout = true;
		//Print(TaskDialog.ShowInputEx("test", "Aby"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, TDFlags.Wider));
		//Print(TaskDialog.ShowInputEx("test", "Aby", style:TDFlags.CommandLinks, customButtons:"One\r\ntwo"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Combo, expandedText:"one\ntwo", radioButtons:"10One|11Two"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, expandedText: "one\ntwo"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, radioButtons:"10One|11Two"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, customButtons:"One\r\ntwo", style:TDFlags.CommandLinks, expandedText:"one\ntwo"));
		//TaskDialog.ShowInputEx("test", "Aby", TDEdit.Combo, expandedText:"exp", footerText:"foo", onButtonClick: e =>
		// {
		//	 if(e.Button == TDResult.OK) {
		//		 e.DoNotCloseDialog = true;
		//		 //e.dialog.Send.ChangeText1("Header", true);
		//		  e.dialog.Send.ChangeText2("new\ntext\netc\netc", true);
		//		 //e.dialog.Send.ChangeExpandedText("AAA\nBBB", true);
		//		 //e.dialog.Send.ChangeFooterText("new\ntext", true);

		//		 //e.dialog.SetText("Header", "text");
		//		 //e.dialog.SetEditControl(TDEdit.None);
		//		 //e.dialog.SetEditControl(TDEdit.Text);
		//		 //e.dialog.Send.Reconstruct();
		//	 }
		// });
		//Print(s);
		//Print(TaskDialog.ShowInputEx("test", "one\nzero\none\ntwo", TDEdit.Combo, checkBox:"check", expandedText:"expanded", footerText:"footer",
		//	x:-1,y:-100, timeoutS: 15, customButtons:"1one|2two"));
		//Print(TaskDialog.ShowInputEx("test", customButtons:"1one|2two", onButtonClick: e => { if(e.Button == 1) { e.DoNotCloseDialog = true; Print(e.EditText); e.EditText="nnnnnnnnnnnnnnn"; } }));

		//Print(TaskDialog.ShowList("1one|2two|3three|Cancel", "Infooooo"));
		//Print(TaskDialog.ShowListEx("1one|2two|3three|Cancel", "Infooooo", checkBox:"Check", x:-10, timeoutS:10));


		//Print(Api.GetCurrentThreadId());
		////TaskDialog.ShowNoWait(e => { Print(Api.GetCurrentThreadId()); }, "Tesxt");

		//var f = new Form();
		//var c = new Button(); c.Text = "test";
		//f.Controls.Add(c);
		//f.Click += (unu, sed) =>
		//  {
		//	  //TaskDialog.Show("one", ownerWindow:f);
		//	  TaskDialog.ShowNoWait(e => { Print(Api.GetCurrentThreadId()); }, true, "Tesxt", ownerWindow: c);
		//	  //TaskDialog.ShowNoWait(e => { Print(Api.GetCurrentThreadId()); }, false, "Tesxt", ownerWindow: f);
		//	  //TaskDialog.ShowNoWait(null, true, "Tesxt", ownerWindow: f);
		//	  //TaskDialog.ShowNoWait(null, true, "Tesxt");

		//	  //var t = new Thread(() =>
		//	  //{
		//	  //  var ff = new Form();
		//	  //	ff.ShowDialog(f);
		//	  //});
		//	  //t.SetApartmentState(ApartmentState.STA);
		//	  //t.Start();
		//  };
		//f.ShowDialog();



		//try {
		////Print(TaskDialog.Show("Text1", "Text2", TDButtons.OKCancel, TDIcon.Info, TDFlags.CommandLinks));

		//}catch(Exception e) { Print(e); }

		//TaskDialog.Show("one");

		//int i;
		//if(!TaskDialog.ShowInput(out i, "Text1", 5, TDEdit.Text)) return;
		//Print(i);

		//string s;
		//if(!TaskDialog.ShowInput(out s, "Text2")) return;
		//PrintList(s, s!=null);

		//string s;
		//if(!TaskDialog.ShowInput(out s, "Text2", editType: TDEdit.Multiline)) return;
		////TODO: need resizable
		//PrintList(s, s != null);

		//Print(Application.StartupPath);
		//Print(Application.UserAppDataPath);

	}


#if true

#endif

	[DllImport("user32.dll")]
	public static extern int MessageBoxW(Wnd hWnd, string lpText, string lpCaption, uint uType);

	static void TestTaskDialog3()
	{


		//var d = new TaskDialog("dddd");
		////d.SetIcon(TDIcon.Info);
		////var ic =Icons.GetFileIconHandle(@"q:\app\macro.ico", 16);
		//var ic = Icon.ExtractAssociatedIcon(@"q:\app\macro.ico");
		//Print(ic);
		//d.SetIcon(ic);
		//d.ShowDialog();
		////Icons.DestroyIconHandle(ic);


		//TaskDialog.Show("Owned", owner:Wnd.Find(null, "Notepad"));

		//var f = new Form();
		//f.Click += (unu, sed) => TaskDialog.Show("Owned", owner: f);
		//f.ShowDialog();

		//Timer_.After(2000, t => MessageBox.Show("kkkk"));
		//TaskDialog.Options.AutoOwnerWindow = true;
		//Timer_.After(2000, t => TaskDialog.Show("kkkk"));
		//var f = new Form();
		//f.ShowDialog();

		//TaskDialog d = new TaskDialog("aa", "bb", "1 One|4 Four");

		//d.Created += e => { e.dialog.Send.EnableButton(4, false); };

		//d.ShowDialog();

		//Print("fin");
		//TaskDialog.Show("Simple");

		//Timer_.Every(50, t => GC.Collect());

		//Print(TaskDialog.Show("Simple"));
		//Print(TaskDialog.Show("Simple", "", "Yes|No"));
		//Print(TaskDialog.Show("Simple", "", "-1 One|-2 Two"));
		//TaskDialog.Options.UseAppIcon = true;
		//Print(TaskDialog.Show("Simple", "", "A123456789123456789|BBBBBBBBBBB|CCCCCCCCCCCCC|DDDDDDDDDDDD|EEEEEEEEEEEEE|FFFFFFFFFFFFFFF"));

		//Print(TaskDialog.ShowList("One|Two|Three|100 Four\r\naa\rbb\ncc"));
		//Print(TaskDialog.ShowList(5));
		//Print(TaskDialog.ShowList("One|Two|Three|100 Four", "Simple"));
		//var a = new string[100]; for(int i = 0; i < a.Length; i++) a[i] = (i+1).ToString();
		//Print(TaskDialog.ShowList(a, "Simple"));

		//Print(TaskDialog.ShowOKCancel("Text1"));
		//Print(TaskDialog.ShowOKCancel("Text1", "Text2.", TDIcon.Info, TDFlags.Wider));
		//Print(TaskDialog.ShowYesNo("Text1", "Text2.", TDIcon.Info, TDFlags.Wider));
		//TaskDialog.ShowInfo("Text1", "Text2.", TDFlags.Wider);
		//TaskDialog.ShowWarning("Text1", "Text2.", TDFlags.Wider);
		//TaskDialog.ShowError("Text1", "Text2.", TDFlags.Wider);

		//Print(TaskDialog.ShowInputEx("Text1", "Text2."));
		//Print(TaskDialog.ShowInput(out string s, "Simple", "aaaa", TDEdit.Combo, new string[] { "text", "A", "B", "C" }));
		//Print(s);


#if false
mes "downloaded"
mes "Display settings will be changed after you restart computer." "Display Settings" "i"
mes F"Func1 result={r}" "QM function" "i"
mes var
mes F"Failed to install {localfile}." title "x"
mes F"{localfile} successfully downloaded{ai}." title "i"
mes F"Failed to download {localfile}." title "x"
mes- "failed to create file"
err+ mes _error.description "Error" "x"
mes _error.description "Error" "x"
mes "Too many controls for this page" "Error" "x"
mes "QM cannot create controls of this class." "Error" "x"
mes "Failed to set styles." "Dialog Editor" "!"
mes "This control class is not supported." "" "!"
mes "Macro changed." "" "x"
mes "Need a window." "" "i"
mes "Failed to register hotkey Shift+F11."
mes "failed to update the grid, please retry"
mes _s "" "i" (_s is looong info text")
mes F"{sWarn}[][]You see this warning because 'Use acc' is checked in Options of this dialog." "" "!"
mes "Need a window." "" "i"
mes "Window not selected."
mes "Text empty."
mes _error.description
mes "Not found."
mes F"This 'background' option will not be applied for this window on this computer.{k}" "" "i"
mes "Failed"
mes "Failed"
mes "This dialog creates basic code to call function ListDialog. The function also allows you to set dialog position, size, owner window and item images. Read ListBox help and edit the inserted code." "" "i"
mes F"Error: {_error.description}"
mes "To add or remove a dialog (or an action from a dialog) to the Favorites toolbar, at first open the dialog and select the action." "" "i"
mes "Already exists. Use different name."
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes "The icon already exists in the imagelist." "" "!"
mes "Failed.[][]The bitmap must contain 1 or more images horizontally, each of equal width and height. Image size must be 8-256, divisible by 8. Colors must be 4, 8, 24 or 32 bit." "" "!"
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes "Please select 1 image." "" "!"
mes _error.description
mes _error.description "Error" "x"
mes _error.description "Error" "x"
mes "QM must run as administrator." "" "x"
mes "Failed" "" "x"
mes "QM must run as administrator." "" "!"
mes "Failed" "" "!"
mes sErr
mes "The event procedure must have sel wParam."
mes s "" "i"
mes "Portable QM warning: This will make changes in registry or file system." "" "!"
mes "Invalid class name."
mes F"A QM item '{_s}' already exists."
mes "Invalid base class name."
mes F"Invalid function name {vn}"
sub_sys.MsgBox t_hDlg _error.description "Regular expression error" "i"
sub_sys.MsgBox hEdit _error.description "" "i"
sub_sys.MsgBox hDlg F"Action '{s}' will be selected whenever you open this dialog." "" "i"
sub_sys.MsgBox hDlg F"{statement}[][]{st}" "Object not found" "!"
sub_sys.MsgBox hDlg statement "Element not found" "!"
sub_sys.MsgBox _hwndqm "Failed to save."
sub_sys.MsgBox hDlg iif(s.len s "Not found.") "" "i"
sub_sys.MsgBox hDlg statement "Not found" "!"
sub_sys.MsgBox hDlg "This macro is read-only." "" "!"
MsgBoxAsync "Used variables or incorrect expressions." "Cannot test" "x"


OKCancel
if(mes(F"Component not found. Download now?[][]Description: {description}[]Required file: {localfile}[]Download from: {url}[]File size: {filesizeKB} KB." title "OC?")!='O') ret
if(mes("Save the dialog in current macro?" "Dialog Editor" "OC?")!='O')
sel mes(F"{_error.description}[][]Failed to update dialog in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
if(mes("This will search for controls reference.[]In the list of results:[]click 'Control Library (Windows)',[]click a control link,[]scroll to Styles." "" "OCi")!='O')
if(mes("This will search for controls reference.[]In the list of results:[]click 'Control Library (Windows)',[]click a control link,[]scroll to Notifications." "" "OCi")!='O')
if('O'=mes("Select an icon in the Icons dialog. OK will open the dialog.[][]Other ways to add icons:[]Drag and drop files.[]Copy/paste files.[]Copy/paste list of file paths.[][]Icons are added to the end, except when pasting. Use Cut/Paste to reorder." "" "OCi"))
if('O'!=mes("Are you sure?" "" "!OC"))
if('O'!=mes("Are you sure?" "" "!OC"))
if(mes("Open the topic, then click OK." "" "OCn")!='O')
if(mes("Save the menu in current macro?" "Menu Editor" "OC?")!='O')
sel mes(F"{_error.description}[][]Failed to update menu in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
if(mes("Save the menu in current macro?" "Menu Editor" "OC?")!='O')
mes(F"{_error.description}[][]Failed to update menu in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
if('O'!mes(F"Restores default key actions for {s}." "" "OCi"))

YesNo
if(mes("You can find more functions in the forum. You can request to create a function for you.[][]Go to the forum?" "" "YNi")='Y')
if(sub_sys.MsgBox(hDlg "Are you sure?[][]Note: These email account settings are shared with Outlook Express (Windows XP)." "" "YN!")!='Y')

YesNoCancel
sel mes("Do you want to save changes?" "Dialog Editor" "YNC!")
sel mes("Save changes?" "" "YNC?")
sel(sub_sys.MsgBox(_hwndqm F"{s}[][]This file already exists. Delete?[][]Click No to make unique filename." "" "YNC!"))

Hyperlink
i=mes(F"<>Image not found.{_s}" "Test - not found" "!")
i=mes(F"<>{_error.description}{_s}" "Test - error" "!")






#endif
	}

	//TODO: maybe .NET has such types. Look in System namespace, eg where is Tuple<T>, Func<T> etc.


	//static void Example1(Types<string, IEnumerable<string>> param)
	//{
	//	switch(param.obj) {
	//	case string s: PrintList("string", s); break;
	//	case IEnumerable<string> a: PrintList("IEnumerable<string>", a); break;
	//	case null: Print("null"); break;
	//		//default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	//static void Example2(string requiredParam, Types<int, double>? optionalParam = null)
	//{
	//	switch(optionalParam?.obj) {
	//	case null: Print("null"); break;
	//	case int i: PrintList("int", i); break;
	//	case double d: PrintList("double", d); break;
	//		//default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	//static void TestTypes(Types<string, IEnumerable<string>> x)
	//{
	//	switch(x.obj) {
	//	case string s: PrintList("string", s); break;
	//	case IEnumerable<string> a: PrintList("IEnumerable<string>", a); break;
	//	case null: Print("null"); break;
	//	default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	//static void TestTypes2(Types<int, double>? x = null)
	//{
	//	switch(x?.obj) {
	//	case null: Print("null"); break;
	//	case int i: PrintList("int", i); break;
	//	case double d: PrintList("double", d); break;
	//	default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	static void TestValueTypes(Types<int, double> x)
	{
		switch(x.type) {
		case 0: Print("none"); break;
		case 1: PrintList("int", x.v1); break;
		case 2: PrintList("double", x.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}

	static void TestValueTypesRef(Types<string, IEnumerable<string>> x)
	{
		switch(x.type) {
		case 0: Print("none"); break;
		case 1: PrintList("string", x.v1); break;
		case 2: PrintList("IEnumerable<string>", x.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}

	static void TestValueTypesNullable(Types<int, double>? x)
	{
		switch(x?.type) {
		case null: Print("null"); break;
		case 0: Print("none"); break;
		case 1: PrintList("int", x?.v1); break;
		case 2: PrintList("double", x?.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}

	static void TestValueTypesRefNullable(Types<string, IEnumerable<string>>? x)
	{
		switch(x?.type) {
		case null: Print("null"); break;
		case 0: Print("none"); break;
		case 1: PrintList("string", x?.v1); break;
		case 2: PrintList("IEnumerable<string>", x?.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}




	#region speed

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static bool Speed1(Types<string, IEnumerable<string>> x)
	//{
	//	switch(x.obj) {
	//	case string s: return s is null;
	//	case IEnumerable<string> a: return a is null;
	//	default: return false;
	//	}
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static bool Speed2(Types<int, double> x)
	//{
	//	switch(x.obj) {
	//	case int s: return s == 0;
	//	case double a: return a == 0.0;
	//	default: return false;
	//	}
	//}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static bool Speed3(Types<int, double> x)
	{
		switch(x.type) {
		case 1: return x.v1 == 0;
		case 2: return x.v2 == 0.0;
		default: return false;
		}
	}

	static void TestMultiTypeParamSpeed()
	{
		int r = 0;
		string s = "kkkk";
		var a = new string[] { "A", "B" };
		int m = 7;
		double d = 3.4;

		for(int i = 0; i < 5; i++) {
			Perf.First();
			//for(int j = 0; j < 1000; j++) {
			//	if(Speed1(s)) r++;
			//	if(Speed1(a)) r++;
			//}
			//Perf.Next();
			//for(int j = 0; j < 1000; j++) {
			//	if(Speed2(m)) r++;
			//	if(Speed2(d)) r++;
			//}
			//Perf.Next();
			for(int j = 0; j < 1000; j++) {
				if(Speed3(m)) r++;
				if(Speed3(d)) r++;
			}
			Perf.NW();
		}

		Print(r);
	}

	#endregion

	static void TestMultiTypeParam()
	{
		//TestExamples();
		return;

		//TestTypes("S");
		//TestTypes(new string[] { "S0", "S1" });
		//TestTypes(new List<string>() { "L0", "L1" });
		////TestTypes(null);
		////TestMultiTypeParam(5);
		//TestTypes((string)null);
		//TestTypes(default(Types<string, IEnumerable<string>>));
		//Print("----");

		//TestTypes2(3);
		//TestTypes2(3.5);
		//TestTypes2(null);
		//TestTypes2();
		////int? n = 7;
		////TestTypes2(n);
		//Print("----");

		TestValueTypes(7);
		TestValueTypes(7.5);
		TestValueTypes(default(Types<int, double>));
		Print("----");

		TestValueTypesRef("S");
		TestValueTypesRef(new string[] { "S0", "S1" });
		TestValueTypesRef(new List<string>() { "L0", "L1" });
		//TestValueTypesRef(null);
		//TestValueTypesRef(5);
		TestValueTypesRef((string)null);
		TestValueTypesRef(default(Types<string, IEnumerable<string>>));
		Print("----");

		TestValueTypesNullable(7);
		TestValueTypesNullable(7.5);
		TestValueTypesNullable(default(Types<int, double>));
		TestValueTypesNullable(null);
		Print("----");

		TestValueTypesRefNullable("S");
		TestValueTypesRefNullable(new string[] { "S0", "S1" });
		TestValueTypesRefNullable(new List<string>() { "L0", "L1" });
		TestValueTypesRefNullable(null);
		//TestValueTypesRefNullable(5);
		TestValueTypesRefNullable((string)null);
		TestValueTypesRefNullable(default(Types<string, IEnumerable<string>>));
	}

	static void TypesFunc(Types<int, double> a1, Types<int, double>? a2, Types<string, int> a3, Types<string, int>? a4)
	{
		PrintList(a1.type, a1.v1, a1.v2);
		var t2 = a2.GetValueOrDefault();
		PrintList(t2.type, t2.v1, t2.v2);
		PrintList(a3.type, a3.v1, a3.v2);
		var t4 = a4.GetValueOrDefault();
		PrintList(t4.type, t4.v1, t4.v2);
	}

	static void TestTypesAgain()
	{
		//TypesFunc(1, null, "S", null);
		//TypesFunc(1, 2.5, "S", 4);
		//TypesFunc(1, 2.5, "S", "K");
		//TypesFunc(null, 2.5, "S", "K"); //error
		//TypesFunc(1, 2.5, null, "K"); //error
	}

	static void TestPrintIEnumerable()
	{
		var a = new string[] { "a", "b" };
		IEnumerable<string> e = a;
		Print(a);
		Print(e);
	}

	static void TestSystemTimePeriod()
	{
		_Test("before");
		using(new Time.LibSleepPrecision(2)) {
			_Test("in");
		}
		_Test("after");

		void _Test(string name)
		{
			Print(name);
			Perf.First();
			for(int i = 0; i < 10; i++) { Thread.Sleep(1); Perf.Next(); }
			Perf.Write();
			//DebugDialog(Perf.Times);
		}

		//using(new Time.LibSleepPrecision(5)) {
		//	int i = 0;
		//	Timer_.Every(10, t => { Perf.Next(); if(++i > 10) { Perf.Write(); t.Stop(); } });
		//	TaskDialog.Show("");
		//}
	}

	static bool TestExc2()
	{
		//return true || throw new Win32Exception(5);

		return false ? true : throw new Win32Exception(5);

		//if(!true) throw new Win32Exception(5); return true;
	}

	static void TestNewExceptions()
	{
		//Print(Native.GetErrorMessage(2));

		//if(!TestExc()) Print(ThreadError.ErrorText);

		//TestExc2();

	}

	static void TestNewRegistry()
	{
		using(var k = Registry_.Open("invalid/key")) {
			Print(k);

		}
	}

	static void TestWndException()
	{
		//throw new NullReferenceException();
		//throw new WndException();
		//throw new WndException(3);
		//throw new WndException("Custom string");
		//throw new WndException(3, "Custom string");
		//throw new WndException(0, "Custom string");
	}

	public static void Move(Types<int, float>? x, Types<int, float>? y, Types<int, float>? width, Types<int, float>? height, bool workArea = false)
	{
		Print("Move Types<int, float>?");
	}

	public static void Move(int? x, int? y, int? width, int? height, uint swpFlagsToAdd = 0)
	{
		Print("Move int?");
	}

	public static void Move(int x, int y, int width, int height, uint swpFlagsToAdd = 0)
	{
		Print("Move int");
	}

	static void TestMoveOverloads()
	{
		Move(1, 2, 3, 4);
		Move(1, null, 3, 4);
		Move(1, 2.5f, 3, 4);

		Move(null, null, 3, null);
		Move(null, 2.5f, null, null);
	}

	static void TestWndPropErrors()
	{
		//var w = Wnd.Find("Quick*");
		var w = Wnd.Find("* Notepad");
		Native.ClearError();
		//var a = w.PropList();
		//var p = w.GetProp("asd");
		//var p = w.GetProp("qmshex");
		//w.SetProp("sett", 5);
		w.PropRemove("sett");
		PrintList(Native.GetError(), Native.GetErrorMessage());
		//Print(p);
		Print(w.PropGet("sett"));
		//Print(a.Count);
		//Print(a);
	}

	static void TestWinErrorMessageAndExceptions()
	{
		//for(int i = 1; i < 10000; i++) {
		//	var s = Native.GetErrorMessage(i);
		//	if(s.StartsWith_("Unknown error (0x") || s.Length >500) continue;
		//	Print(i);
		//	Print(s);
		//}

		Wnd w = default(Wnd);
		Native.ClearError();
		//if(!w.IsChildOf(default(Wnd))) Print(Native.GetError());
		//var a = w.PropList();
		//if(a.Count==0) Print(Native.GetError());

		try {
			bool b = w.IsEnabled;
			if(Native.GetError() != 0) {
				//throw new CatException();
				//throw new CatException("Messssage");
				//throw new CatException(5);
				//throw new CatException(5, "Messssage");
				throw new CatException("Messsssage", new FileNotFoundException());
			}
		}
		catch(CatException e) {
			PrintList(e.NativeErrorCode, e.Message);
		}
	}

	static void TestWndAccessDenied()
	{
		Wnd w = Wnd.Find("Registry *");
		//Wnd c = w.Child("", "Edit");
		//Print(c);
		//c.FocusControl();
		w.SetTransparency(true, 50);

		//Wnd w = Wnd.Find("Quick*");
		//Print(w);
		//Wnd c = w.ChildById(2216);
		//Print(c);
		//Native.ClearError();
		//string s = c.ControlText;
		//if(s == null) Print(Native.GetErrorMessage());
		//else Print(s);

		//Wnd w = Wnd.Find("*YouTube Downl*");
		//Print(w);
		//Wnd c = w.ChildByClass("*SysListview32*");
		//Print(c);
		//Native.ClearError();
		//string s = c.ControlNameWinForms;
		//if(s == null) Print(Native.GetErrorMessage());
		//else Print(s);

		//Wnd w = Wnd.Find("*YouTube Downl*");
		//Print(w);
		//Wnd c = w.Child("**wfName:olvDownloads");
		//Print(c);

	}


	//int thisLen = t.Length, endof = index + length;
	//if(index < 0 || endof < index || endof > thisLen) throw new ArgumentException();
	//if(Empty(value)) return t;
	//var s = new StringBuilder(t);
	//s.

	static void TestStringReplaceAt()
	{
		var s = "/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>";
		//var s = "/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>";
		string s2 = null;

		Print(s.ReplaceAt_(27, 4, "HREF_ETC"));

		//int i = s.Length / 2;
		//var a1 = new Action(() => { s2 = s.ReplaceAt_(i, 4, "HREF_ETC"); });
		//var a2 = new Action(() => { s2 = s.ReplaceAt_2(i, 4, "HREF_ETC"); });
		//var a3 = new Action(() => { s2 = s.ReplaceAt_3(i, 4, "HREF_ETC"); });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);

	}

	static LPARAM WndProc33(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		Wnd.Misc.PrintMsg(w, msg, wParam, lParam);
		return Api.DefWindowProc(w, msg, wParam, lParam);
	}

	static void TestCreateWindow()
	{
		//var wc = Wnd.Misc.WindowClass.Register("TTTTest", WndProc33);
		//Wnd w = Wnd.Misc.CreateWindow(0, wc.Name, "Name", Native.WS_POPUPWINDOW | Native.WS_VISIBLE|Native.WS_CAPTION, 500, 100, 300, 200);

		Wnd.Misc.WindowClass.InterDomainRegister("TTTTest", WndProc33);
		Wnd w = Wnd.Misc.WindowClass.InterDomainCreateWindow(0, "TTTTest", "Name", Native.WS_POPUPWINDOW | Native.WS_VISIBLE | Native.WS_CAPTION, 500, 100, 300, 200);

		var ic = Icons.GetFileIconHandle(@"q:\app\paste.ico", 16);
		Wnd.Misc.SetIconHandle(w, ic);
		Wnd c = Wnd.Misc.CreateWindowAndSetFont(0, "Edit", "Text", Native.WS_CHILD | Native.WS_VISIBLE, 10, 10, 100, 20, w, 3);
		TaskDialog.ShowInput(out string s, "td", editText: "TaskDialog");
		Wnd.Misc.DestroyWindow(w);
		Icons.DestroyIconHandle(ic);
	}

	static void MinimizeAll()
	{
		var a = Wnd.Misc.MainWindows();
		for(int i = 0; i < a.Count; i++) {
			//for(int i=a.Count-1; i>=0; i--) {
			a[i].ShowMinimized(true);
		}
	}

	static void SendAltTab()
	{
		var k = new Api.INPUTKEY(Api.VK_MENU, 0);
		Api.SendInput_Key(ref k);
		k.wVk = Api.VK_TAB;
		Api.SendInput_Key(ref k);
		k.dwFlags = Api.IKFlag.Up;
		Api.SendInput_Key(ref k);
		k.wVk = Api.VK_MENU;
		Api.SendInput_Key(ref k);
	}

	static void TestSwitchActiveWindow()
	{
		Print(Wnd.Misc.SwitchActiveWindow());
		return;

		//Wnd w = Wnd.Find("Microsoft Excel - Book1");
		//Wnd w = Wnd.Find("id.xls");
		//Wnd w = Wnd.Find("Quick M*");
		//Wnd w = Wnd.Find("Registry*");
		//Wnd w = Wnd.Find("* Notepad");
		//Wnd w = Wnd.Find("*Dream*");
		//Wnd w = Wnd.Find("*Help Viewer*");
		//Wnd w = Wnd.Find("*Visual Studio");
		//Wnd w = Wnd.Find("*Firefox*", "MozillaWindowClass");
		//Wnd w = Wnd.Find("* Microsoft Document Explorer");
		//Print(w);
		//if(w.Is0) { Print("not found"); return; }
		////w.ShowNotMinimized(true); return;
		//Print("-----");
		//w.Activate(); Thread.Sleep(500);
		////return;

		////Wnd.Misc.AllowActivate();

		////Print(Wnd.MainWindows());
		////return;

		//Wnd.ShowDesktop();
		////Wnd.ShowDesktop(true);
		//////Thread.Sleep(2000);
		//////Wnd.ShowDesktop(false);
		////MinimizeAll();
		//w.ShowMinimized();

		Thread.Sleep(500);
		Print(Wnd.Misc.MainWindows());
		//return;

		//Print(Wnd.ActiveWindow);
		//Thread.Sleep(500);
		//Print(Wnd.ActiveWindow);
		////return;
		//Thread.Sleep(1000);

		//Print("-----");
		//Print(Wnd.MainWindows());
		//Print("-----");
		////return;

		Print(Wnd.Misc.SwitchActiveWindow());
		//SendAltTab();
		Thread.Sleep(500);
		Print(Wnd.WndActive);
	}

	static void TestStringEqualsPart()
	{
		string s = "**id:text";
		Print(s.EqualsAt_(2, "id:"));
		Print(s.EqualsAt_(2, "ID:", true));
		Print(s.EqualsAt_(2, false, "id:", "text:", "wfName:"));
	}

	static void TestChildAcc()
	{
		Wnd w = Wnd.Find("FileZilla");
		Wnd c = w.Child("Quickconnect");
		Print(c);
		Print(c.NameAcc);
	}

	static void TestWndNextMainWindow()
	{
		int f = TaskDialog.ShowList("1 default|2 allDesktops|3 likeAltTab|4 retryFromTop|5 skipMinimized");
		if(f == 0) return;
		Wnd w = Wnd.Misc.WndTop;
		int n = 0;
		while(!w.Is0) {
			Print(w);
			switch(f) {
			case 1:
				w = w.WndNextMain();
				break;
			case 2:
				w = w.WndNextMain(allDesktops: true);
				break;
			case 3:
				w = w.WndNextMain(likeAltTab: true);
				break;
			case 4:
				w = w.WndNextMain(retryFromTop: true); if(++n > 20) return;
				break;
			case 5:
				w = w.WndNextMain(skipMinimized: true);
				break;
			}
		}
	}

	static unsafe void TestBytesToHexString()
	{
		byte[] a = new byte[4] { 65, 66, 67, 68 };
		var s = Convert_.HexEncode(a, true);
		Print(s);
		Print(Convert_.HexDecode(s));
		fixed (byte* p = a) {
			var s2 = Convert_.HexEncode(p, 4, true);
			Print(s2);
			int a2;
			Print(Convert_.HexDecode(s2, &a2, 4));
			PrintHex(a2);
		}
	}

	class FormB :Form
	{
		protected override void WndProc(ref Message m)
		{
			if((uint)m.Msg == Api.WM_APP) {
				PrintList("WM_APP", m.WParam);
			}
			base.WndProc(ref m);
		}
	}

	[DllImport("winmm.dll")]
	internal static extern uint timeBeginPeriod(uint uPeriod);
	[DllImport("winmm.dll")]
	internal static extern uint timeEndPeriod(uint uPeriod);

	static void WaitEx(int timeMS)
	{
		int sysTimer = 0;
		if(timeMS > 0) {
			if(timeMS < 15) sysTimer = timeMS;
			else if(timeMS > 16 && timeMS < 30) sysTimer = (timeMS + 1) / 2;
			else if(timeMS == 16) timeMS = 15;
		}

		PrintList(timeMS, sysTimer);
	}

	static void TestWndAgain()
	{


		//var w = Wnd.Find("FileZilla");
		//var w = Wnd.Find("Find");
		//w.AllChildren(false, true).ForEach(c => { Print(c); /*c.Show(false);*/ c.Focus(); Wait(0.5); });
		//w.Child(null, "SysTreeView32", skip: 1).Focus();
		//w.Child("Find Next").Focus();

		//Wnd.Misc.WaitForAnActiveWindow();
		//Wnd w = Wnd.Find("* Notepad");
		//w.Activate(); Wait(0.5);
		////w.Close(true);
		//w.Send(Api.WM_CLOSE);
		////Wnd.Misc.WndRoot.Activate();
		//Perf.First();
		//var yes=Wnd.Misc.WaitForAnActiveWindow();
		//Perf.NW();
		//Print(yes);



		//Wnd.FindAll("* Notepad", "Notepad").ForEach(t => { t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Visual Studio*", "wndclass_desked_gsk").ForEach(t => { t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Visual Studio*", "wndclass_desked_gsk").ForEach(t => { t.Activate(); Print(t.Close(useXButton: true)); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Firefox").ForEach(t => { Print(t); t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Thunderbird").ForEach(t => { t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll(className:"QM_toolbar").ForEach(t => { Print(t.Close()); });
		//Wnd.Misc.MainWindows(likeAltTab:true).ForEach(t => {
		//	if(0!=t.Name.Like_(false, "Catkeys -*", "Quick Macros -*", "Process Expl*")) return;
		//	Print(t);
		//	t.Activate();
		//	long t1 = Time.Milliseconds;
		//	Print(t.Close());
		//	//Print(t.Close(useXButton: true));
		//	long t2 = Time.Milliseconds-t1;
		//	if(t2 > 300) Print(t2);
		//	Print(Wnd.WndActive);
		//	Print("---");
		//});
		//Wnd.Misc.MainWindows().ForEach(t => { Print(t); t.Activate(); Wait(0.5); });

		//Wnd.Find("*something");
		//Wnd.Find("*something", flags: WFFlags.HiddenToo);
		//Wnd.Find("*something");

		//Perf.First();
		//Wnd w;
		//////w=(Wnd)328956;
		//////w=(Wnd)263214;
		//////w=Wnd.Find("id.xls");
		////w=Wnd.Find("*Excel*");
		//////w = Wnd.Find("* Notepad");
		//////w = Wnd.Find("Quick Ma*");
		//////w = Wnd.Find("*Inno *");
		//w = Wnd.Find("PicPick");
		////Perf.Next();
		////w.Activate();
		//bool ok=w.Close();
		//Perf.NW();
		//Print(ok);



		//Wnd w = Wnd.Find("SQLite E*");
		//Wnd w = Wnd.Find("* Inno Setup*");
		//Print(w);
		//w.Activate();

		//var c = Wnd.Misc.WindowClass.InterDomainRegister("ffoo", null);
		////Wnd w = Wnd.Misc.CreateMessageWindow("#32770");
		//Wnd w = Wnd.Misc.CreateMessageWindow("ffoo");
		//Print(w);
		//w.Close();
		////Wnd.Misc.DestroyWindow(w);
		////w.Close(true);MessageBox.Show("fff");
		//Print(w);

		//for(int i = 1; i < 50; i++) WaitEx(i);
		//return;

		//using(new Time.LibSleepPrecision(2)) {
		//	Perf.First();
		//	for(int i = 1; i < 17; i++) {
		//		//timeBeginPeriod(5);
		//		////Thread.Sleep(1);
		//		//timeEndPeriod(5);
		//		//Perf.Next();
		//		//timeBeginPeriod(2);
		//		////Thread.Sleep(1);
		//		//timeEndPeriod(2);
		//		//Perf.Next();

		//		//Thread.Sleep(i);
		//		Thread.Sleep(2);
		//		Perf.Next((char)('0' + i % 10));
		//	}
		//	Perf.Write();

		//}

		//var f = new FormB();
		//f.Text = "FindMe";
		//f.Deactivate += (unu, sed) => Print("deact");
		////f.ShowDialog();return;
		//f.Show();

		////Thread.Sleep(5000);
		//Thread.CurrentThread.Join(5000);


		//TaskDialog.Show("");
		//return;

		//Wnd.Misc.WndRoot.ActivateLL();
		//Print(Wnd.WndActive);

		//var w1 = Wnd.Find("*- Notepad");
		//var w2 = Wnd.Find("*Excel*");

		//Perf.First();
		//for(int i = 0; i < 3; i++) {
		//	//w1.Activate();
		//	Perf.Next();
		//	Thread.Sleep(10);
		//	//w2.Activate();
		//	Perf.Next();
		//}
		////Perf.Write();
		//Trace.WriteLine(Perf.Times);

		//var w = Wnd.Find("* Notepad");
		//var s = w.SavePlacement();
		//Print(s);
		//MessageBox.Show("");
		//w.RestorePlacement(s);


		//Wnd w = Wnd.Find("Dialog");
		//w.Send(Api.WM_APP);

		//Timer_.After(500, t => { Thread.CurrentThread.Abort(); });
		//Timer_.After(500, t => { throw new IndexOutOfRangeException(); });
		//Timer_.After(500, t => { Api.PostQuitMessage(1); });
		//Timer_.Every(500, t => {  });
		//Thread.Sleep(3000);
		//Application.Run();
		//while(Time.DoEvents()) {  }
		//Api.WaitMessage();
		//MessageBox.Show("text", "Tests.exe");
		//new FolderBrowserDialog().ShowDialog();
		//new OpenFileDialog().ShowDialog();
		//new Form().ShowDialog();
		//TaskDialog.Show("1");
		//TaskDialog.Show("2");
		//TaskDialog.ShowEx("2", timeoutS:30);
		//TaskDialog.ShowEx("2", buttons:"OK|Cancel", timeoutS:30);
		//TaskDialogAPI(default(Wnd), Zero, "t", "m", null, 0, null, out int b);

		//var d = new TaskDialog("aa", null, "1 Test", timeoutS: 15);
		//d.ButtonClicked += e =>
		//{
		//	e.DoNotCloseDialog = true;
		//	//throw new IndexOutOfRangeException();
		//	Thread.CurrentThread.Abort();
		//};
		//d.ShowDialog();

		//var f = new Form();
		//f.Click += (unu, sed) =>
		//  {
		//	  //throw new IndexOutOfRangeException();
		//	  Thread.CurrentThread.Abort();
		//  };
		//f.ShowDialog();

		//Print("fin");

		//Wnd w = Wnd.Find("*Notepad", flags:WFFlags.HiddenToo);
		////Print(w.IsEnabled);
		////w.Close();
		////Wait(5);
		//w.Show(true);

		//Wnd w = Wnd.Find("*Notepad");
		//w.ZorderTopmost();
		//w.MoveToScreenCenter();
		//w.MoveToScreenCenter(2);
		//w.MoveToScreenCenter(Screen_.Primary);
		//w.MoveToScreenCenter(Screen_.OfMouse);
		//w.MoveToScreenCenter(Screen.PrimaryScreen);
		//w.MoveToScreenCenter(new Point(1, 10000));
		//w.MoveToScreenCenter(new Rectangle(300, 1000, 100, 10000));
		//w.MoveToScreenCenter(Wnd.WndActive);
		//w.MoveToScreenCenter(Wnd.FindFast(null, "QM_Editor"));

		//var r = new RECT(1000, 2000, 500, 500, true);
		//Print(r);
		//r.EnsureInScreen();
		//Print(r);
		//r.EnsureInScreen(1);
		//Print(r);


		//if(TaskDialog.ShowInput(out string s)) Print(s);
		//var d = new TaskDialog(text2: "small");
		//Print(d.ShowDialog());

		//Wnd w = Wnd.FindFast("Options", "#32770");
		//Print(w);
		//Wnd w1 = default(Wnd), w2 = default(Wnd), w3 = default(Wnd), w4 = default(Wnd);

		//var a1 = new Action(() => { w1 = Wnd.Find(null, "WordPadClass"); });
		//var a2 = new Action(() => { w2 = Wnd.FindFast(null, "WordPadClass"); });
		//var a3 = new Action(() => { w3 = w.Child("Apply", "Button"); });
		//var a4 = new Action(() => { w4 = w.ChildFast("&Apply", "Button"); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//Print(w1);
		//Print(w2);
		//Print(w3);
		//Print(w4);

		//Wnd.CloseAll("* Notepad", "Notepad");
		//foreach(var v in Wnd.FindAll("* Notepad", "Notepad")) v.Close();
		//close all Notepad windows
		//Wnd.FindAll("* Notepad", "Notepad").ForEach(t => t.Close());


		//var f = new Form();
		//f.Click += (unu, sed) =>
		//{
		//	//((Wnd)f).ShowMinimized();
		//	f.Wnd_().ShowMinimized();
		//};
		//f.ShowDialog();

		//return;

		////Wnd w = Wnd.Find("Dialog");
		////w = w.ChildById(1);
		////Print(c.ClassName);
		////Print(c.Name);
		////Print(c.ControlText);
		//////return;
		////Wait(0.2);

		//Wnd w = Wnd.Find("Options");
		//w = w.Child("Unicode");

		//PrintList("self", w);
		//PrintList("parent", w.WndDirectParent);
		//PrintList("par or own", w.WndDirectParentOrOwner);
		//PrintList("owner", w.WndOwner);
		//PrintList("first child", w.WndFirstChild);
		//PrintList("last child", w.WndLastChild);
		//PrintList("child 2", w.WndChild(2));
		//PrintList("first sib", w.WndFirstSibling);
		//PrintList("last sib", w.WndLastSibling);
		//PrintList("next", w.WndNext);
		//PrintList("prev", w.WndPrev);
		//PrintList("window", w.WndWindow);

		//Print("----");
		////Print(Wnd.Find("Options").Child("Layout*").WndNext);
		//Print(Wnd.Find("Options").WndFirstChild.WndChild(3));

		//return;

		//var a = Wnd.Misc.AllWindows(true);
		//int i, n = a.Count;
		//for(i = 0; i < n; i++) {
		//	var b = a[i].AllChildren();
		//	a.AddRange(b);
		//}
		//Print(a.Count); //1924

		//string s = null;
		//Perf.First();
		//for(i = 0; i < 5; i++) {
		//	foreach(var v in a) {
		//		//s = v.ClassName;
		//		//s = v.Name;
		//		s = v.ControlText;
		//	}
		//	Perf.Next();
		//}
		//Perf.Write();

		//Print(s);

		//foreach(var v in a) {
		//	var name = v.Name;
		//	var text = v.ControlText;
		//	Wnd.Misc.StringRemoveUnderlineAmpersand(ref text);

		//	//if(name == null) Print($"name null: {v}"); //0
		//	//if(text == null) Print($"text null: {v}"); //1: Windows.UI.Core.CoreWindow "Settings"

		//	//if(Empty(name) && Empty(text)) continue;

		//	//if(Empty(name)) Print($"----\n{v}\n{text}"); //almost all: Edit, Combo, StatusBar

		//	//if(Empty(name) && 0==v.ClassNameIs("*Edit*", "*ComboBox*", "*StatusBar*", "*Memo*")) Print($"----\n{v}\n{text}"); //1: VMwareStaticLink

		//	//if(Empty(text)) Print($"----\n{v}\n{name}"); //6 (3 classes)

		//	//if(text == name) PrintList(v, name);

		//	//if(text != name && !(Empty(name) || Empty(text))) Print($"----\n{v}\n{name}\n{text}"); //5 (3 classes)

		//	//if(Empty(name)) {
		//	//	var v2 = v.WndPrev;
		//	//	if(v2.Is0) {
		//	//		v2 = v.WndDirectParent;
		//	//		if(v2)
		//	//	}
		//	//}
		//}



		//Wnd w = Wnd.Find("FileZilla");
		//Wnd c = w.ChildById(-31804);
		//Wnd w = Wnd.Find("Dialog");
		//Wnd c = w.ChildById(3);
		//Print(c.ControlText);
		//Wait(0.2);
		//string s = "";

		//var a1 = new Action(() => { s = c.ControlText; });
		//var a2 = new Action(() => { });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4);

		//Print(s);

		//Wnd w=Wnd.Find(a, b, c, d, e); if(!w.Is0) Print(w);

		//var p = new Wnd.Finder(a, b, c, d, e); if(p.Find()) Print(p.Result);

		//WFFlags

		//Wnd w = Wnd.Find("FileZilla");
		//Wnd c;
		//c = w.Child("Port:");
		//Print(c);
		//c = w.Child(null, "Button");
		//Print(c);
		//c = w.Child("**id:-31782");
		//Print(c);
		//c = w.Child("777");
		//Print(c);
		//c = w.Child("**text:777");
		//Print(c);
		//c = w.Child("**text:Port:");
		//Print(c);
		//c = w.Child("**accName:Port:");
		//Print(c);

		//w = Wnd.Find("Free YouTube*");
		//c = w.Child("**wfName:olvDownloads");
		//Print(c);

		//w = Wnd.Find("Options");
		//c = w.Child("On error");
		//Print(c);
		//c = w.Child("**text:On error");
		//Print(c);
		//c = w.Child("Administrator");
		//Print(c);
		//c = w.Child("**text:Administrator");
		//Print(c);

		//c = w.Child("**id:15");
		//c = w.Child("**text:control text");
		//c = w.Child("**wfName:myControl");
		//c = w.Child("**accName:acc name");

		//c = w.ChildByClass("Button");
		//Print(c);
		//c = w.ChildById(-31930);
		//Print(c);
		//c = w.Child("-31930", flags: WCFlags.NameIsId);
		//Print(c);
		//Thread.Sleep(200);

		//var a1 = new Action(() => { c = w.Child(null, "Button"); });
		//var a2 = new Action(() => { c = w.ChildByClass("Button"); });
		//var a3 = new Action(() => { c = w.ChildById(-31930); });
		//var a4 = new Action(() => { c = w.Child("-31930", flags: WCFlags.NameIsId); });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4);


		//Wnd w = Wnd.Find("* Notepad");
		//Print(w);
		//w.SetProp("test", 1);
		//w = Wnd.Find(also: t => t.GetProp("test") != 0);
		//Print(w);


		//Wnd w = Wnd.Find(also: t => t.ContainsScreenXY(0.3f, 1.2f));
		//Print(w);

		//Wnd w = Wnd.Find("Options");
		//Print(w);
		//Wnd c = w.Child(null, "Button", also: t => t.ContainsWindowXY(257, 216));
		////Wnd c = w.Child(null, "Button", also: t => t.ContainsParentXY(0.7f, 0.3f));
		//Print(c);

		//Wait(0.2);

		//var a1 = new Action(() => { c = w.Child(null, "Button", also: t => t.ContainsWindowXY(w, 257, 216)); });
		//var a2 = new Action(() => { c = w.Child(null, "Button", also: t => t.ContainsWindowXY(257, 216)); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		//PrintList(Process_.CurrentProcessHandle, Process_.CurrentThreadHandle, Process_.CurrentProcessId, Process_.CurrentThreadId);

		//Wnd w = default(Wnd);
		//Perf.First();
		//for(int i = 0; i < 5; i++) {
		//	//w = Wnd.Find("* notepad", "notepad");
		//	w = Wnd.Find(programEtc: "notepad");
		//	//w = Wnd.Find(programEtc: "*\\notepad.exe", flags: WFFlags.ProgramPath);
		//	//w = Wnd.Find(programEtc: "notepag");
		//	//w = Wnd.Find(programEtc: "*\\notepag.exe", flags: WFFlags.ProgramPath);
		//	Perf.Next();
		//}
		//Perf.Write();
		//Print(w);

		//return;

		//Wnd w = Wnd.Find(prop: new Wnd.WinProp() { childName = "Open items" });

		//var c = new Wnd.ChildFinder("Open items");
		//Wnd w = Wnd.Find(also: t => c.FindIn(t));


		////Wnd w = Wnd.Find(prop: new Wnd.WinProp() { styleHas = Native.WS_CAPTION });

		////Wnd w = Wnd.Find(also: t => t.HasStyle(Native.WS_CAPTION));


		//Print(w);
		//Print(c.Result);


	}

	static void TestLnkShortcutExceptions()
	{
		//var x = Files.Shortcut.Open(@"q:\test\test.lnk");
		var x = Shell.Shortcut.Create(@"q:\test\test.lnk");
		//x.TargetPath = @"c:\windows";
		x.TargetPath = "|?:*gg";
		//x.TargetURL = "|?:*gg";
		//x.TargetURL = "http://www.quickmacros.com";
		//x.TargetPidl = Marshal.AllocCoTaskMem(100);
		//x.TargetAnyType = "|?:*gg";
		x.Save();
	}

	[DllImport("kernel32.dll")]
	internal static extern void Sleep(int dwMilliseconds);
	[DllImport("kernel32.dll")]
	internal static extern uint SleepEx(int dwMilliseconds, bool bAlertable);
	[DllImport("kernel32.dll")]
	internal static extern uint WaitForMultipleObjectsEx(uint nCount, [In] IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds, bool bAlertable);
	[DllImport("kernel32.dll")]
	internal static extern uint WaitForMultipleObjectsEx(uint nCount, ref IntPtr lpHandle, bool bWaitAll, uint dwMilliseconds, bool bAlertable);

	static void SpinMCS(int timeMCS)
	{
		var t = Time.Microseconds;
		while(Time.Microseconds - t < timeMCS) { }
	}

	static void TestAbortThreadAndWaitFunctions()
	{
		//var h = Api.CreateEvent(Zero, false, false, null);
		//using(new Time.LibSleepPrecision(1)) {
		//	Perf.First();
		//	for(int i = 0; i < 5; i++) {
		//		//SpinMCS(15000);
		//		//Perf.Next();
		//		Thread.Sleep(1);
		//		//Thread.CurrentThread.Join(1);
		//		//Time.SleepDoEvents(1);
		//		//Api.MsgWaitForMultipleObjectsEx(0, null, 15, 0);
		//		//Api.WaitForSingleObject(h, 15);
		//		//WaitForMultipleObjectsEx(1, ref h, false, 15, false);
		//		//Sleep(1);
		//		Perf.Next();
		//	}
		//	Perf.Write();
		//}
		//Api.CloseHandle(h);
		//return;

		Catkeys.Util.AppDomain_.Exit += (unu, ded) => { Print("exit"); };

		new Thread(o =>
		{
			Thread.Sleep(1002);
			Print("abort");
			var t = o as Thread;
			Perf.First();
			//t.Abort();
			//t.Interrupt();
			//t.Suspend();
			//Wait(10);
			//t.Resume();
			//TODO: test whether .NET wait functions support APC
			//Environment.Exit(1);
			//var a=Wnd.Misc.ThreadWindows(t.)
			//var w = Wnd.Find("Tests.exe");
			//Print(w);
			//w.CloseLL();
			//w.Close(false);
		}).Start(Thread.CurrentThread);

		//Timer_.After(1000, t => { Print("timer"); });

		try {
			//Print(Api.MsgWaitForMultipleObjectsEx())
			Wait(5);
			//Print(Time.WaitNoBlockMS(5000));
			//Sleep(5000);
			//SleepEx(5000, true);
			//for(int i = 0; i < 50; i++) Sleep(100);
			//Thread.CurrentThread.Join(5000);

			//using(var e = new EventWaitHandle(false, EventResetMode.AutoReset)) {
			//	e.WaitOne(5000);
			//}

			//Task.Delay(5000);

			//var m=new Catkeys.Util.MessageLoop();
			//Timer_.After(1000, t => { Print("timer"); m.Stop(); });
			//m.Loop();

			Print("after sleep");
		}
		catch(ThreadInterruptedException e) { Print("interrupted"); }
		finally {
			Perf.NW();
			Print("finally");
		}

		Print("return");
	}

	static LPARAM TestHookProc(int code, LPARAM wParam, LPARAM lParam)
	{
		Print(wParam);
		return Api.CallNextHookEx(Zero, code, wParam, lParam);
	}
	static Api.HOOKPROC s_testHookProc = TestHookProc;

	static void TestHooks()
	{
		var hh = Api.SetWindowsHookEx(Api.WH_KEYBOARD_LL, s_testHookProc, Zero, 0);
		//TaskDialog.Show();
		//Thread.Sleep(10000);
		Thread.CurrentThread.Join(10000); //OK
		Api.UnhookWindowsHookEx(hh);
	}

	[DllImport("kernel32.dll")]
	internal static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
	internal const uint THREAD_ALL_ACCESS = 0x1FFFFF;
	[DllImport("kernel32.dll")]
	internal static extern uint QueueUserAPC(PAPCFUNC pfnAPC, IntPtr hThread, LPARAM dwData);
	internal delegate void PAPCFUNC(LPARAM Parameter);
	internal const uint WAIT_IO_COMPLETION = 0xC0;

	static void TestApcProc(LPARAM Parameter)
	{
		Print(Parameter);
	}
	static PAPCFUNC _testApcProc = TestApcProc;

	static void TestWait()
	{
		//var f = new Form();
		//f.Click += async (unu, sed) =>
		//  {
		//	  //Print(WaitFor.Window(0, "* Notepad"));
		//	  Print(await WaitFor.WindowAsync(0, "* Notepad"));
		//  };
		////f.Click += F_Click;
		//f.ShowDialog();

		//async void F_Click(object sender, EventArgs e)
		//{
		//	Print(await WaitFor.WindowAsync(0, "* Notepad"));
		//}

		//Timer_.After(1000, t =>
		//{
		//	Print("begin");
		//	for(int i = 0; i < 1000; i++) {
		//		//Application.DoEvents();
		//		Time.DoEvents();
		//		Thread.Sleep(10);
		//	}
		//	Print("end");
		//}
		//);
		//var f = new Form();
		////var b = new Button();
		////b.KeyUp
		////b.MouseUp
		//f.Click
		//	+= (unu, sed) =>
		//  {
		//	  Print("begin");
		//	  //for(int i = 0; i < 1000; i++) {
		//	  // //Application.DoEvents();
		//	  // if(!Time.DoEvents()) break;
		//	  // Thread.Sleep(10);
		//	  //}
		//	  //Time.SleepDoEvents(10000);
		//	  Application.Exit();

		//	  //await Task.Delay(3000);
		//	  //await Task.WhenAll

		//	  Print("end");
		//  };
		//f.KeyUp += (unu, sed) => Print("key");
		////f.Controls.Add(b);
		//f.ShowDialog();
		////f.Show();
		////var m = new Native.MSG();
		////while(Api.GetMessage(out m, default(Wnd), 0, 0) > 0) {
		////	Wnd.Misc.PrintMsg(ref m);
		////	Api.TranslateMessage(ref m);
		////	Api.DispatchMessage(ref m);
		////}
		////Wnd.Misc.PrintMsg(ref m);

		//Print("exit");
		//return;

		//var mt = Thread.CurrentThread;
		//var th = OpenThread(THREAD_ALL_ACCESS, false, Api.GetCurrentThreadId());
		////Print(th);
		//new Thread(o =>
		//{
		//	for(int i = 0; i < 3; i++) {
		//		Wait(1);
		//		QueueUserAPC(_testApcProc, th, i);
		//	}

		//	//Wait(1);
		//	//mt.Suspend();
		//	//Wait(7);
		//	//mt.Resume();

		//}).Start();
		//Perf.First();
		////while(SleepEx(7000, true) == WAIT_IO_COMPLETION) { Print("WAIT_IO_COMPLETION"); }
		////Thread.Sleep(5000);
		//Time.SleepDoEvents(5000);

		////Thread.CurrentThread.Join(7000);
		//Api.CloseHandle(th);
		//Perf.NW();

		//Print("end");

		//var t1 = Time.Milliseconds;
		//Thread.Sleep(1000 * 30);
		//Print(Time.Milliseconds - t1);

		//var a = new List<int>(100);
		//using(new Time.LibSleepPrecision(1)) {
		//	for(int i = 1; i < 50; i++) {
		//		var t1 = Time.Milliseconds;
		//		Thread.Sleep(i);
		//		var t2 = Time.Milliseconds;
		//		a.Add((int)(t2 - t1));
		//	}
		//}
		//Print(a);
	}

	static void TestWaitFor()
	{
		//Print(WaitFor.WindowClosed(-5, Wnd.Find("* Notepad")));
		//Print(WaitFor.WindowNotExists(0, "* Notepad"));
		//Print("ok");

		//Wnd w = WaitFor.WindowExists(10, "* Notepad");
		//Print(w);

		//var f = new Form();
		//f.Click += async (unu, sed) =>
		//  {
		//	  Print("waiting for Notepad...");
		//	  Wnd w = await Task.Run(() => WaitFor.WindowExists(-10, "* Notepad"));
		//	  if(w.Is0) Print("timeout"); else Print(w);
		//  };
		//f.ShowDialog();
	}

	//private static async void F_Click(object sender, EventArgs e)
	//{
	//	Print(await WaitFor.WindowAsync(0, "* Notepad"));
	//}

	[DllImport("comctl32.dll", EntryPoint = "#344", PreserveSig = true)]
	internal static extern int TaskDialogAPI(Wnd hwndOwner, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, int dwCommonButtons, string pszIcon, out int pnButton);

	public static async Task<TDResult> ShowAsync(string text1)
	{
		var td = new TaskDialog(text1);
		return await Task.Run(() => td.ShowDialog());
	}

	static void TestTaskDialogAsyncAwait()
	{
		//var pd = TaskDialog.ShowProgress(false, "Working");
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		//	pd.Send.Progress(i); //don't need this if marquee
		//	Thread.Sleep(50); //do something in the loop
		//}
		//pd.Send.Close();

		var td = TaskDialog.ShowNoWaitEx("Another example", "text", "1 OK|2 Cancel", y: -1, timeoutS: 30);
		Print(td.DialogWindow);
		Wait(2); //do something while the dialog is open
		td.Send.ChangeText2("new text", false);
		td.Send.Close();
		Wait(2); //do something while the dialog is open
		td.ThreadWaitClosed(); Print(td.Result); //wait until the dialog is closed and get result. Optional, just an example.

		//Print(Api.GetCurrentThreadId());
		//var r=await ShowAsync("fff");
		//Print(Api.GetCurrentThreadId());

		//var f = new Form();
		//f.Click


		//Timer_.Every(1000, tt => { });
		//Thread.CurrentThread.Abort();
		//TaskDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e =>
		//{
		//	Print(e.LinkHref);
		//	Thread.CurrentThread.Abort();
		//}
		////,timeoutS:30
		//);
		//Print("after dialog");

		//var pd = TaskDialog.ShowProgressEx(false, "Working", buttons: "1 Stop", y: -1);
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		//	pd.Send.Progress(i); //don't need this if marquee
		//	Thread.Sleep(50); //do something in the loop
		//}
		////pd.Send.Close();
		//return;

		//TaskDialog.Show("Simple example");
		//var td=TaskDialog.ShowNoWait(null, "Simple example");
		//var td = TaskDialog.ShowNoWaitEx(null, "Simple example", timeoutS: 3);
		//var td=TaskDialog.ShowNoWait(null, "Simple example");
		//Print(1);
		//td.ThreadWaitClosed();
		//Print(2);
		//Task.Run(() => { Thread.Sleep(10000); });
		//Thread th = null;
		//Task.Run(() =>
		//{
		//	//try {
		//	//th = Thread.CurrentThread;
		//	//Timer_.Every(300, tt => { Print("timer"); });
		//	//MessageBox.Show("dd");
		//	//TaskDialog.Show("dd");
		//	TaskDialog.ShowEx("dd", timeoutS: 15);
		//	//Thread.CurrentThread.Abort();
		//	//Wait(0.1);
		//	//throw new Exception("dd");
		//	//}
		//	//catch(Exception e) { Print(e); }
		//});

		//throw new Exception("dd");
		//new Thread(() =>
		//{
		//	Wait(0.1);
		//	throw new Exception("dd");
		//}
		//).Start();

		//var th = new Thread(() =>
		//  {
		//	  //MessageBox.Show("ff");
		//	  //TaskDialog.Show("ff");
		//	  TaskDialog.ShowEx("dd", timeoutS: 15);
		//  });
		//th.IsBackground = true;
		//th.Start();

		//Print(1);
		//Wait(1);
		//Print(2);
		//th.Abort();
		//Wait(1);
		//Print(3);

		//var td = TaskDialog.ShowNoWaitEx(e => { Print(e); }, "Another example", buttons: "1 OK|2 Cancel", y: -1, timeoutS: 30);
		//Wait(2); //do something while the dialog is open in other thread
		//td.ThreadWaitClosed(); //wait until dialog closed (optional, just an example)

		//return;

		//Task.Run(() => TaskDialog.Show("example"));
		//Task.Run(() => TaskDialog.ShowEx("example", timeoutS: 15));
		//Print(1);
		//Wait(3); //do something in this thread
		//Print(2);

		//var t = Task.Run(() => TaskDialog.ShowEx("example", "<a href=\"example\">link</a>", buttons: "Yes|No|CL\nnn", icon: TDIcon.App, flags: TDFlags.CommandLinks, timeoutS: 15, onLinkClick: e => Print(e.LinkHref)));
		//var t = Task.Run(() => TaskDialogAPI(default(Wnd), Zero, "ti", "s1", "s2", 0, null, out int b));
		//Print(1);
		//Wait(3);
		//Print(2);
		//t.Wait();
		//Print(t.Result);


		//var f = new Form();
		////f.Click += (unu, sed) =>
		////{
		////	var r = TaskDialog.Show("sync");
		////	var r = TaskDialog.Show("sync", owner:f);
		////	Print(r);
		////};
		//f.Click += async (unu, sed) =>
		//{
		//	Print(Api.GetCurrentThreadId());
		//	//var r = await Task.Run(() => TaskDialog.Show("async"));
		//	var r = await Task.Run(() => TaskDialog.Show("async", owner: f));
		//	Print(r);
		//	Print(Api.GetCurrentThreadId());
		//};
		//f.ShowDialog();

	}

	public static int LoadFromImage_(this ImageList t, Image pngImage)
	{
		t.ColorDepth = ColorDepth.Depth32Bit;
		int k = pngImage.Height; t.ImageSize = new Size(k, k); //AddStrip throws exception if does not match
		int R = t.Images.AddStrip(pngImage);
		var h = t.Handle; //workaround for the lazy ImageList behavior that causes exception later because the Image is disposed when actually used
		return R;
	}

	//class ImageFromStrip
	//{
	//	Graphics _g, _g2;

	//	public ImageFromStrip(Bitmap b)
	//	{
	//		_g = Graphics.FromImage(b);
	//		int h = b.Height;
	//		var b2 = new Bitmap(h, h);
	//		_g2 = Graphics.FromImage(b2);
	//		b2.LockBits
	//	}

	//	Image GetImage(int i)
	//	{
	//		_g2.DrawImageUnscaled()
	//	}
	//}

	static void TestImagelistSpeed()
	{
		var f = new Form();
		f.Width = 1000;
		f.Click += (unu, sed) =>
		{
			var file = @"Q:\app\Catkeys\Editor\Resources\il_tb_24.png";
			Perf.First();
			var img = (Bitmap)Image.FromFile(file);
			int size = img.Height;
			Perf.Next();
			var a = new Image[40];
#if true
			ImageList il = new ImageList();
			il.LoadFromImage_(img);
			Perf.Next();
#endif
			for(int i = 0; i < 40; i++) {
				int x = i * size;
				//a[i] = il.Images[i];
				//a[i] = Icons.HandleToImage(Api.ImageList_GetIcon(il.Handle, i, 0));
				a[i] = img.Clone(new Rectangle(x, 0, size, size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			}
			Perf.Next();
			using(var g = f.CreateGraphics()) {
				g.Clear(Color.White);
				Perf.Next();
				for(int i = 0; i < 40; i++) {
					int x = i * size;
					//il.Draw(g, x, 0, i);
					g.DrawImage(a[i], x, 0);
				}
				Perf.NW();
			}
		};
		f.ShowDialog();
	}

	static void TestImagelistSpeed2()
	{
		var f = new Form();
		f.Width = 1000;
		f.Click += (unu, sed) =>
		{
			var files = new string[]
			{
@"Q:\app\Catkeys\Editor\Resources\TB\WriteBackPartition.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLDocumentTypeDefinitionFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLSchema.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLTransformation.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XnaLogo.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XPath.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XSLTTemplate.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XSLTTransformFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\ZoomIn.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSDatasetInternalInfoFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSShell.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSTAAbout.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSThemeEditor.png",
@"Q:\app\Catkeys\Editor\Resources\TB\Watch.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WCF.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WCFDataService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WeakHierarchy.png",
@"Q:\app\Catkeys\Editor\Resources\TB\Web.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebAdmin.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebConfiguration.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebConsole.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebCustomControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebCustomControlASCX.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebMethodAction.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebPart.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebPhone.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebSetupProject.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebTest.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebUserControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WeightMember.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WeightMemberFormula.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WF.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WFC.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WFService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsForm.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsLogo_Cyan.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsServiceStop.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsServiceWarning.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WinformToolboxControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WMIConnection.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkAsSomeoneElse.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkflowAssociationForm.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkflowInitiationForm.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkItemQuery.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFApplication.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFCustomControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFDesigner.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFFlowDocument.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFLibrary.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFPage.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFPage_gray.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFPageFunction.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFResourceDictionary.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFToolboxControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFUserControl.png"
			};

			Perf.First();
			int size = 24;
			var a = new Image[40];
			for(int i = 0; i < 40; i++) {
				a[i] = Image.FromFile(files[i]);
			}
			Perf.Next();
			using(var g = f.CreateGraphics()) {
				g.Clear(Color.White);
				Perf.Next();
				for(int i = 0; i < 40; i++) {
					int x = i * size;
					g.DrawImage(a[i], x, 0);
				}
				Perf.NW();
			}
		};
		f.ShowDialog();
	}

	static void TestImagelistSpeed3()
	{
		var f = new Form();
		f.Width = 1000;
		f.Click += (unu, sed) =>
		{
			var files = new string[]
			{
@"WriteBackPartition",
@"XMLDocumentTypeDefinitionFile",
@"XMLFile",
@"XMLSchema",
@"XMLTransformation",
@"XnaLogo",
@"XPath",
@"XSLTTemplate",
@"XSLTTransformFile",
@"ZoomIn",
@"VSDatasetInternalInfoFile",
@"VSShell",
@"VSTAAbout",
@"VSThemeEditor",
@"Watch",
@"WCF",
@"WCFDataService",
@"WeakHierarchy",
@"Web",
@"WebAdmin",
@"WebConfiguration",
@"WebConsole",
@"WebCustomControl",
@"WebCustomControlASCX",
@"WebMethodAction",
@"WebPart",
@"WebPhone",
@"WebService",
@"WebSetupProject",
@"WebTest",
@"WebUserControl",
@"WeightMember",
@"WeightMemberFormula",
@"WF",
@"WFC",
@"WFService",
@"WindowsForm",
@"WindowsLogo_Cyan",
@"WindowsService",
@"WindowsServiceStop",
@"WindowsServiceWarning",
@"WinformToolboxControl",
@"WMIConnection",
@"WorkAsSomeoneElse",
@"WorkflowAssociationForm",
@"WorkflowInitiationForm",
@"WorkItemQuery",
@"WPFApplication",
@"WPFCustomControl",
@"WPFDesigner",
@"WPFFlowDocument",
@"WPFLibrary",
@"WPFPage",
@"WPFPage_gray",
@"WPFPageFunction",
@"WPFResourceDictionary",
@"WPFToolboxControl",
@"WPFUserControl"
			};

			Perf.First();
			//var resMan = new System.Resources.ResourceManager("Resources.Images", Assembly.GetExecutingAssembly());
			var resMan = Tests.Properties.Resources.ResourceManager;
			Perf.Next();
			int size = 24;
			var a = new Image[40];
			for(int i = 0; i < 40; i++) {
				a[i] = (Image)resMan.GetObject(files[i]);
				if(i < 2) Perf.Next();
			}
			Perf.Next();
			using(var g = f.CreateGraphics()) {
				g.Clear(Color.White);
				Perf.Next();
				for(int i = 0; i < 40; i++) {
					int x = i * size;
					g.DrawImage(a[i], x, 0);
				}
				Perf.NW();
			}
		};
		f.ShowDialog();
	}

	static void TestCreateManyPngFromIcons()
	{
		var a = new string[]
			{
@"Q:\app\new.ico",
@"Q:\app\properties.ico",
@"Q:\app\save.ico",
@"Q:\app\icons\run.ico",
@"Q:\app\icons\compile.ico",
@"Q:\app\deb next.ico",
@"Q:\app\icons\deb into.ico",
@"Q:\app\icons\deb out.ico",
@"Q:\app\deb cursor.ico",
@"Q:\app\deb run.ico",
@"Q:\app\deb end.ico",
@"Q:\app\undo.ico",
@"Q:\app\redo.ico",
@"Q:\app\cut.ico",
@"Q:\app\copy.ico",
@"Q:\app\paste.ico",
@"Q:\app\icons\back.ico",
@"Q:\app\icons\active_items.ico",
@"Q:\app\icons\images.ico",
@"Q:\app\icons\annotations.ico",
@"Q:\app\help.ico",
@"Q:\app\droparrow.ico",
@"Q:\app\icons\record.ico",
@"Q:\app\find.ico",
@"Q:\app\icons\mm.ico",
@"Q:\app\icons\tags.ico",
@"Q:\app\icons\resources.ico",
@"Q:\app\icons\icons.ico",
@"Q:\app\options.ico",
@"Q:\app\icons\output.ico",
@"Q:\app\tip.ico",
@"Q:\app\icons\tip_book.ico",
@"Q:\app\delete.ico",
@"Q:\app\icons\back2.ico",
@"Q:\app\open.ico",
@"Q:\app\icons\floating.ico",
@"Q:\app\icons\clone dialog.ico",
@"Q:\app\dialog.ico",
		   };

		var destDir = @"Q:\app\Catkeys\Editor\Resources\png icons";
		Files.CreateDirectory(destDir);
		foreach(var s in a) {
			using(var im = Icons.GetFileIconImage(s, 16)) {
				var dest = destDir + "\\" + Path.GetFileNameWithoutExtension(s) + ".png";
				//Print(dest);
				File.Delete(dest);
				im.Save(dest);
			}
		}
	}

	static void TestResizePngIconBigger()
	{
		var f = new Form();
		f.BackColor = Color.White;

		var img = Image.FromFile(@"Q:\Downloads\famfamfam_silk_icons_v013\icons\application_edit.png");
		var t = new ToolStrip();
		t.Items.Add(img);
		t.ImageScalingSize = new Size(32, 32);

		f.Controls.Add(t);

		f.ShowDialog();
	}

	static string g_name = "Output";
	static void TestXDocument()
	{
		Perf.First();
		var x = XElement.Load(@"Q:\app\catkeys\editor\default\Panels.xml");
		Perf.Next();
		//var n =x.Elements().Count();
		var v1 = x.Descendants("panel").FirstOrDefault(el => el.Attribute("name")?.Value == "Output");
		var v2 = x.XPathSelectElement("//panel[@name=\"" + g_name + "\"]");
		Perf.NW();
		//Print(v2);
		Print(x.Descendant_("panel", "name"));
		Print(x.Descendant_("panel", "name", "Find"));

		var a1 = new Action(() => { v1 = x.Descendants("panel").FirstOrDefault(el => el.Attribute("name")?.Value == "Output"); });
		var a2 = new Action(() => { v2 = x.XPathSelectElement("//panel[@name=\"" + g_name + "\"]"); });
		var a3 = new Action(() => { });
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestMenuStripShortcuts()
	{
		var f = new FormMenu();
		//var ms = new MenuStrip();
		//var dd = new ToolStripDropDownMenu();
		//var k=dd.Items.Add("Test", null, (unu, sed) => Print("test")) as ToolStripMenuItem;
		//var ddi = new ToolStripMenuItem("File");
		//ddi.DropDown = dd;
		//ms.Items.Add(ddi);

		//k.ShortcutKeys = Keys.Control | Keys.Z;

		//f.Controls.Add(ms);
		//f.MainMenuStrip = ms;




		Application.Run(f);
	}

	static void TestKeysFromString()
	{
		Keys k;
		string s;
		s = " Ctrl + Shift + Left ";
		//s = "ctrl+shift+left";
		s = "Ctrl+Shift+Left";
		s = "A";
		s = "F12";
		s = ",";
		s = "5";
		s = "z";
		s = "Sleep";
		s = "Ctrl+Alt+Shift+Space";
		s = "  Ctrl  +  D  ";
		s = "Ctrl+Alt+Left";
		s = "End";
		s = "Win+R";
		s = "Ctrl+D";
		s = "5";

		//s = "K+D";
		//s = " C trl + F 2 ";
		//s = "Ctrl+Alt";
		//s = "Ctrl";
		//s = "Ctrl+A+K";
		//s = "A+Ctrl";
		//s = "+A";
		//s = "A+";
		//s = "Ctrl++D";

		Perf.First();
		if(!Input.Misc.ReadHotkeyString(s, out k)) { Print("failed"); return; }
		Perf.NW();
		PrintHex(k);
		Print(k);
		//return;
		Sleep(100);

		var a1 = new Action(() => { Input.Misc.ReadHotkeyString(s, out k); });
		var a2 = new Action(() => { Input.Misc.ReadHotkeyString(" Ctrl+F ", out k); });
		var a3 = new Action(() => { Input.Misc.ReadHotkeyString(" Ctrl + F ", out k); });
		var a4 = new Action(() => { Input.Misc.ReadHotkeyString("", out k); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		//var w = Wnd.Find("* Notepad");
		//w.Activate();
		//w = w.ChildById(15);

		//w.Post(Api.WM_KEYDOWN, (int)Keys.OemCloseBrackets);

		//PrintHex(Keys.Alt);

		//Perf.First();
		//var a =Input.Misc._CreateKeyMap();
		////Perf.NW();

		//////Print(a);
		////Print(a.Count);

		//Perf.First();
		//var k = a["Tab"];
		//Perf.NW();

		//var a1 = new Action(() => { k = a["Ctrl"]; });
		//var a2 = new Action(() => { k = a["Tab"]; });
		//var a3 = new Action(() => { k = a["VolumeUp"]; });
		//var a4 = new Action(() => { k = a[";"]; });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		//Print(k);
	}

	static void TestImageSerialize()
	{
		var x = new XElement("test");
		for(int i = 0; i < 5; i++) {
			Perf.First();
			Bitmap b = Icons.GetFileIconImage(@"q:\app\qm.exe", 16);
			Perf.Next();
			//Print(b != null);
			using(var ms = new MemoryStream()) {
				b.Save(ms, ImageFormat.Png);
				Perf.Next();
				var s = Convert.ToBase64String(ms.ToArray());
				Perf.Next();
				//Print(s);
				var d = new XElement("i", s);
				x.Add(d);
			}
			Perf.NW();
		}
		Print(x);

		var f = @"q:\test\image2.png";
		Bitmap bb = null;
		Perf.First();
		foreach(var k in x.Elements("i")) {
			//Print(k.Value);
			using(var ms = new MemoryStream(Convert.FromBase64String(k.Value))) {
				bb = new Bitmap(ms);
			}
			Perf.Next();
		}
		Perf.Write();

		bb.Save(f);
		Process.Start(f);

		//Bitmap bb = new Bitmap(ms);
		//Print(bb != null);
		//var f = @"q:\test\image.png";
		//bb.Save(f);
		//Process.Start(f);
	}

	static void TestCatExceptioNewOverload()
	{
		Wnd w = Wnd.Find("ILSpy");
		Api.SetLastError(10);
		try {
			try {
				throw new CatException(1, "Inner");
			}
			catch(Exception e) {
				//throw new CatException("Outer", e);
				//throw new CatException(5, "Outer", e);
				//throw new WndException(w, "Outer", e);
				throw new WndException(w, 5, "Outer", e);
			}
		}
		catch(CatException e) {
			PrintList(e.NativeErrorCode, e.Message);
		}
	}

	static void TestApiStringBuffer()
	{
		//string s = @"D:\simple\file.txt";
		//s = @"D:\simple\.\file.txt";
		////s = @"C:\Progra~1";
		//s = @"C:\Progra~2\Quickm~1\./qm.exe";

		//Print(Path_.Normalize(s));


		//var w = Wnd.Find("Quick*");
		////var w = Wnd.Find("MozBackup*", null, "MozBackup");
		//Print(w.ProcessName);
		//Print(w.ProcessPath);

		//Print(Files.SearchPath("notepad.exe"));
		//Print(Files.SearchPath("qm.exe"));

		////string lnk = @"D:\Test\notepad.lnk";
		//string lnk = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Microsoft Office\Microsoft Office Excel 2003.lnk";
		////var x = Files.Shortcut.Create(lnk);
		////x.TargetPath = Folders.System + "notepad.exe";
		////x.Arguments = @"D:\Test\test2.cs";
		////x.SetIconLocation(Folders.System + "shell32.dll", 10);
		////x.WorkingDirectory = @"D:\Test\x";
		////x.Description = "Descr";
		////x.Save();
		//var x = Files.Shortcut.Open(lnk);
		//Print(x.TargetPath);
		//Print(x.Arguments);
		//Print(x.GetIconLocation(out var ii)); Print(ii);
		//Print(x.WorkingDirectory);
		//Print(x.Description);

		//Print(Path_.LibGetEnvVar2("TEMP"));
		//Print(Path_.LibGetEnvVar("TEMP"));
		//Print(Path_.ExpandEnvVar2("%TEMP%\\etc"));
		//Print(Path_.ExpandEnvVar("%TEMP%\\etc"));

		//var a1 = new Action(() => { Path_.LibGetEnvVar2("TEMP"); });
		//var a2 = new Action(() => { Path_.LibGetEnvVar("TEMP"); });
		//var a3 = new Action(() => { Path_.ExpandEnvVar2("%TEMP%\\etc"); });
		//var a4 = new Action(() => { Path_.ExpandEnvVar("%TEMP%\\etc"); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//var w = Wnd.Find("Quick*");
		//var x = new Process_.Memory(w, 1000);
		//string s;
		//x.WriteUnicodeString("UNICODE");
		//s = x.ReadUnicodeString(7);
		//PrintList(s.Length, s);
		//x.WriteAnsiString("ANSI");
		//s = x.ReadAnsiString(7);
		//PrintList(s.Length, s);
		//x.WriteAnsiString("Ačę", 0, Encoding.UTF8);
		//s = x.ReadAnsiString(5, 0, Encoding.UTF8);
		//PrintList(s.Length, s);

		//var w = Wnd.Find(null, "Windows.UI.Core.CoreWindow");
		//Print(w);
		//var h = Wnd.Misc.GetIconHandle(w);
		//Print(h);
		//var m = new CatMenu();
		//m["test", Icons.HandleToImage(h)] = null;
		//m.Show();

		//var w = Wnd.Find("Quick*", flags: WFFlags.HiddenToo);
		//Print(w);
	}

	static void TestPathNormalize()
	{
		string s;
		s = @"c:\a\..\..\b.txt";
		s = @"c:\a\b";
		s = @"c:\";
		s = @"c:\a\b..";
		s = @"c:\progrm files\some folder\..\some file.txt";
		s = @"c:\progrm files\some folder\some file.txt";
		s = @"\\?\c:";
		s = @"c:\a\.\b.txt";
		s = @"c:\a\b\.";
		s = @"file.name";
		s = @"c:\a\..";
		s = @"\\?\\\\c:////kk";
		s = @"c:";
		s = @"c:\progra~1";
		s = @"c:\a\b. ";
		s = @"%programfiles%\etc\";
		s = @"c:/progrm files/some folder/some file.txt";
		s = @"\\server\share\a\b";
		s = @"c:\oo~nooo1";
		s = @"c:\a\..\b.txt";
		s = @"\\?\c:\progra~1";
		s = @"\\.\etc";
		s = @"\\?\etc";
		s = @"c:etc";
		s = @"\\?\c:etc";

		//Print(Path.GetFullPath(s));
		var u = Path_.Normalize(s);
		//var u=Path_.Normalize(s, @"c:\def dir/");
		Print(u);
		//Print(Path_.PrefixLongPath(u));
		return;

		Wait(0.2);
		Perf.SpinCPU(100);
		var a1 = new Action(() => { Path_.Normalize(s); });
		var a2 = new Action(() => { Path.GetFullPath(s); });
		var a3 = new Action(() => { });
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
	}

	[DllImport("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
	internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

	static void TestFileOp()
	{
		//Print(Path_.ExpandEnvVar("%appdata%\\more"));
		//Print(Path_.IsFullPath("%appdata%\\more"));
		//Print(Files.SearchPath("%appdata%\\Microsoft\\"));
		//Print(Path_.LibGetEnvVar("appdata"));
		//Print(Folders.EnvVar("appdata")+"sub");

		//Environment.SetEnvironmentVariable("test", "%appdata%");
		//Print(Environment.GetEnvironmentVariable("test"));
		//Print(Environment.ExpandEnvironmentVariables("%test%"));
		//Print(Path_.ExpandEnvVar("%test%\\more"));

		//var a1 = new Action(() => { Path_.ExpandEnvVar("%appdata%\\more"); });
		//var a2 = new Action(() => { Path_.ExpandEnvVar("%no%\\more"); });
		//var a3 = new Action(() => { Path_.IsFullPath("%appdata%\\more"); });
		//var a4 = new Action(() => { Path_.IsFullPath("c:\\more"); });
		//var a5 = new Action(() => { Environment.ExpandEnvironmentVariables("%appdata%\\more"); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5);

		//Directory.SetCurrentDirectory(@"q:\test");
		//Print(Files.FileOrDirectory(@"q:\test\test.cs"));
		//Print(Files.FileOrDirectory(@"test.cs"));
		//Print(Files.FileOrDirectory(@"test.cs", true));
		//Print(Files.SearchPath(@"test.cs"));

		//return;

		//if(!Files.GetFileId(@"q:\test", out var k)) throw new CatException(0, "failed");
		////if(!Files.GetFileId(@"q:\test\test.cs", out var k)) throw new CatException(0, "failed");
		////if(!Files.GetFileId(@"q:\test\no.cs", out var k)) throw new CatException(0, "failed");
		//PrintList(k.VolumeSerialNumber, k.FileIndex);

		//if(!Files.GetFileId(@"//Q7c/q/test", out var k2)) throw new CatException(0, "failed");
		//PrintList(k2.VolumeSerialNumber, k2.FileIndex);
		//Print(k2 == k);
		//Print(k.Equals(k2));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Files.GetFileId(@"q:\test", out var kk); });
		//var a2 = new Action(() => { });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
		//33 mcs

		//return;

		//Print(Files._IsDestInSrc(@"c:\A\b", @"c:\a"));

		//s = "name.txt.";
		//s = null;
		//s = "name.txt";
		//s = "na\\me.txt";
		//s = " ";
		//s = "CON.txt";
		//Print(Path_.IsInvalidFileName(s));

		//Files.Move(@"Q:\Test", @"Q:\Test\Find", true);
		//Files.Move(@"Q:\Test\test.cs", @"Q:\Test\test2.cs", true);

		Perf.First();
		//Directory.Move(@"q:\test\a", @"q:\test\c");
		//Directory.Move(@"q:\test\a", @"d:\test\a");
		//VB.FileSystem.MoveDirectory(@"q:\test\a", @"d:\test\a", true);
		//VB.FileSystem.MoveDirectory(@"q:\test\a", @"q:\test\a2", true);
		//VB.FileSystem.MoveDirectory(@"q:\test\a", @"q:\test\c", true);
		//VB.FileSystem.CopyDirectory(@"q:\test\a", @"q:\test\c", true);
		//VB.FileSystem.CopyDirectory(@"q:\test\a", @"d:\test\a", true);
		//VB.FileSystem.CopyDirectory(@"d:\test\x", @"d:\test\z", true);

		//Files.Copy2(@"d:\test\x", @"d:\test\z");
		//Files.Copy2(@"\\?\d:\x\", @"c:/test/~");

		//Files.Move(@"d:\test\x", @"d:\test\z", true);
		//Files.Move(@"d:\test\z\sub\z", @"d:\test\z", true);
		//Files.Move(@"q:\test\test2.cs", @"d:\test\test2.cs", true);
		//Files.Move(@"q:\test\z", @"d:\test\zz", true);

		//if(MoveFileEx(@"d:\test\z", @"d:\test\Z", 3)) return; //OK
		//if(MoveFileEx(@"d:\test\x", @"d:\test\z", 3)) return; //Access is denied (if exists)
		//if(MoveFileEx(@"d:\test", @"d:\test\z", 3)) return; //The process cannot access the file because it is being used by another process.
		//if(MoveFileEx(@"d:\test\z", @"d:\test", 3)) return; //Access is denied
		//if(MoveFileEx(@"d:\test\z\sub", @"d:\test", 3)) return; //Access is denied
		//if(MoveFileEx(@"q:\test\z", @"d:\test\z", 2)) return; //Access is denied
		//var ec = Native.GetError();
		//throw new CatException(ec, "*move");

		//Files.Test();
		//Files.Delete(@"d:\no file", true);

		var ifExist = IfExists.Fail;
		ifExist = IfExists.Delete;
		//ifExist = IfExists.RenameExisting;
		//ifExist = IfExists.MergeDirectory;

		//Files.Copy(@"q:\test\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Copy(@"d:\test\z\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Copy(@"q:\test\copy dir", @"d:\test\copy dir", ifExist);
		//Files.Copy(@"d:\test\copy.txt", @"d:\test\COPY.txt", ifExist);
		//Files.Copy(@"d:\test\copy dir", @"d:\test\COPY dir", ifExist);
		//Files.Move(@"d:\test\copy.txt", @"d:\test\COPY.txt", ifExist);
		//Files.Move(@"d:\test\copy dir", @"d:\test\COPY dir", ifExist);
		//Files.Rename(@"d:\test\copy.txt", @"COPY.txt", ifExist);
		//Files.Rename(@"d:\test\copy dir", @"COPY dir", ifExist);
		//Files.Move(@"q:\test\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Move(@"q:\test\copy dir", @"d:\test\copy dir", ifExist);
		//Files.Move(@"d:\test\z\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Move(@"d:\test\z\copy dir", @"d:\test\copy dir", ifExist);
		//Files.Move(@"d:\test\z\copy dir", @"d:\test\copy dir", ifExist);

		//copying drives
		//Files.CopyTo(@"E:\", @"d:\test\E", ifExist); //fails, it's OK
		//Files.Copy(@"E:\", @"d:\test\E", ifExist);
		//Files.Copy(@"E:\", @"G:\", ifExist);
		//Files.Move(@"E:\", @"G:\", ifExist);

		//if(Files.GetAttributes(@"E:\", out var att)) Print(att);

		//string sto = @"D:\Test\drive";
		//Files.Delete(sto, false);
		//Files.Copy(@"C:\", sto, ifExist);

		string sto = @"D:\Test\to";
		Files.Delete(sto);
		//Files.Copy(@"D:\Test\x", sto, ifExist, FCFlags.IgnoreAccessDeniedErrors);
		//Files.Copy(@"D:\Test\x", sto, ifExist, FCFlags.IgnoreAccessDeniedErrors);
		//Files.Copy(@"Q:\programs", sto, ifExist, filter: t => t.IsDirectory || t.Size < 100 * 1024);

		Perf.NW();
		//return;


		//string s1, s2;
		//s1 = @"Q:\Test\test.cs";
		//s2 = @"Q:\Test\test2.cs";
		//s1 = @"Q:\Test\Find";
		//s2 = @"Q:\Test\Find2";
		////s1 = @"Q:\Test\test.cs";
		//////s2 = @"Q:\Test\Find\test.cs";
		////s2 = @"Q:\Test\new dir\subdir\test.cs";

		////File.SetAttributes(s1, FileAttributes.ReadOnly);
		//Perf.First();
		//for(int i = 0; i < 2; i++) {
		//	var t1 = ((i & 1) == 0) ? s1 : s2;
		//	var t2 = ((i & 1) == 0) ? s2 : s1;
		//	//File.Move(t1, t2);

		//	//Files.Rename(t1, Path.GetFileName(t2), true);
		//	//Files.Move(t1, t2);
		//	//Files.MoveTo(t1, Path.GetDirectoryName(t2));
		//	Files.Copy(t1, t2, true);


		//	Perf.Next();

		//	break;
		//	if(i == 0) DebugDialog(i);
		//}
		//Perf.Write();
	}

	static void TestFilesDelete()
	{
		//string s1, s2;
		//s1 = @"Q:\Test\test.cs";
		//s2 = @"Q:\Test\test2.cs";
		//for(int i = 0; i < 4; i++) {
		//	File.Copy(s1, s2);
		//	Perf.First();
		//	Files.Delete(s2, false);
		//	Perf.NW();
		//}

		//for(int i = 0; i < 3; i++) {
		//	string s = @"Q:\Test\New Folder";
		//	string s2 = s + "\\sub";
		//	string s3 = s2 + "\\file.txt";
		//	if(true) {
		//		Directory.CreateDirectory(s2);
		//		if(!Files.FileExists(s3)) File.WriteAllText(s3, "aaa");
		//		//File.SetAttributes(s3, FileAttributes.ReadOnly);
		//		//File.SetAttributes(s3, FileAttributes.Hidden);
		//		File.SetAttributes(s2, FileAttributes.Directory | FileAttributes.ReadOnly);
		//		Process.Start(s2);
		//	} else {
		//		Directory.CreateDirectory(s);
		//	}
		//	Wait(2);
		//	//TaskDialog.Show();

		//	Perf.First(100);
		//	Files.Delete(s, false);
		//	Perf.NW();
		//}

		Files.Delete(@"D:\test\mount");

		Print("ok");
	}

	static void TestEnumDirectory()
	{
		int n = 0;
		string dir = @"D:\Test";
		//dir = @"C:\Program Files";
		//dir = @"C:\Program Files (x86)";
		dir = @"C:\Windows\System32";
		//dir = @"C:\Windows";
		//dir = @"C:\Windows.old";
		//dir = @"C:\";
		//dir = @"F:\";
		//dir = @"Q:\";
		//dir = @"Q:\app";
		//dir = @"\\?\D:\Test";
		//dir= @"D:\Test\..";
		//dir = @"D:\";
		//dir= @"D:\Test\mount";
		//dir= @"D:\Test\test2.cs";
		//dir = @"\\.\";
		//dir = @"U:\";
		//dir = @"\\WIN-BJHQMCDNJJR\Users\Public\Recorded TV";
		//dir = @"\\WIN-BJHQMCDNJJR\Users";
		//dir = @"\\WIN-BJHQMCDNJJR";
		//dir = @"\\hhhhhh";
		//dir = @"\\WIN-BJHQMCDNJJR\Moooo";
		//dir = @"Q:\<>";
		//dir = @"\\.\Device";
		//dir = @"//Q7c/q/test";
		//dir = @"//Q7c/d$/test";
		//dir = Folders.SystemX86;

		FEFlags fl = 0;
		fl |= FEFlags.IgnoreAccessDeniedErrors;
		fl |= FEFlags.AndSubdirectories;
		//fl |= FEFlags.AndSymbolicLinkSubdirectories;
		//fl |= FEFlags.SkipHidden;
		//fl |= FEFlags.SkipHiddenSystem;
		//fl |= FEFlags.RawPath;
		//fl |= FEFlags.DisableRedirection;

		Perf.SpinCPU(200);
		Perf.First();
		//Files.EnumDirectory(@"D:\Test", fl);
#if false
		//DirectoryInfo.EnumerateFileSystemInfos and other .NET directory enumeration methods are useless.
		//They cannot be used to enumerate subdirectories because:
		//	Throw UnauthorizedAccessException etc if subfolder access denied.
		//	Don't have option 'don't get symlink directory content'.
		//	Not tested, but probably fail if path length > 257.
		//	When testing, failed for most directories in C:\ (Windows, Program Files etc).
		//Also >2 times slower. Most of its code consists of security checks.
		var d = new DirectoryInfo(dir);
		foreach(var v in d.EnumerateFileSystemInfos("*", System.IO.SearchOption.AllDirectories)) {
			Print(v.FullName);
			n++;
		}
#elif true
		int n1 = 0, n2 = 0;
		//Files.EnumDirectory2(dir, fl);
		foreach(var f in Files.EnumDirectory(dir, fl)) {
			//n1 += f.FullPath.Length + 8; n2 += f.Name.Length + 8;
			//if(f.Name.StartsWith_('$')) Print(f.FullPath);
			PrintList(f.FullPath, f.Attributes.ToString("X"), f.Size, f.LastWriteTimeUtc.ToLocalTime(), f.Level);
			//if(f.FullPath.StartsWith_(@"C:\Windows")) f.SkipThisDirectory();
			////else if(f.FullPath.StartsWith_(@"C:\Program Files")) f.SkipThisDirectory();
			n++;
		}
#else
		foreach(var f in Files.EnumDirectory_old(dir, fl)) {
			//Print(f.FullPath);
			//if(f.FullPath.StartsWith_(@"C:\Windows") || f.FullPath.StartsWith_(@"C:\Program Files")) f.SkipThisDirectory();
			//if(!f.FullPath.StartsWith_(@"C:\$Recycle.Bin", true)) f.SkipThisDirectory();
			n++;
		}
#endif
		Perf.NW();
		Print(n);
		//PrintList(n1 / 1024, n2 / 1024);
	}

	[DllImport("shlwapi.dll", EntryPoint = "PathIsDirectoryEmptyW")]
	internal static extern bool PathIsDirectoryEmpty(string pszPath);

	public static bool IsEmptyDirectory(string path)
	{
		return !Files.EnumDirectory(path).Any();

		//PathIsDirectoryEmpty faster by several %. But then we don't know whether is not empty or it does not exist or is file.
	}

	static void TestDirectoryIsEmpty()
	{
		var s = @"D:\Test\empty";
		s = @"D:\Test\mount"; //False, True when target missing; False, False when target exists
		s = @"D:\Test\x\sym-link-to-y"; //False, False when target not empty; True, True when target empty
		var s2 = @"D:\Test\x";
		s2 = @"\\?\D:\Test\x";
		s2 = @"D:\Test\protected";
		PrintList(PathIsDirectoryEmpty(s), PathIsDirectoryEmpty(s2));
		PrintList(IsEmptyDirectory(s), IsEmptyDirectory(s2));
		Perf.SpinCPU(100);
		var a1 = new Action(() => { PathIsDirectoryEmpty(s); });
		var a2 = new Action(() => { IsEmptyDirectory(s); });
		var a3 = new Action(() => { PathIsDirectoryEmpty(s2); });
		var a4 = new Action(() => { IsEmptyDirectory(s2); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestFileProperties()
	{
		//Files.ExistsAsResult u = 0;
		//if(u > 0) Print(1);
		//if(u < 0) Print(-1);

		//var s = @"E:\setup.exe";
		////var s =@"D:\Test\protected";
		////var s =@"D:\Test\protected\no.txt";
		////var s =@"D:\Test\protected\f.txt";

		////Native.SetError(5);
		////if(Files.FileExists(s)) { Print("exists"); return; }
		////Print(Native.GetErrorMessage());
		//////if(Files.GetAttributes(s, out var attr)) PrintHex(attr); else Print("failed") ;
		////if(Files.GetAttributes(s, out var attr, FAFlags.DoNotThrow)) PrintHex(attr); else Print("failed") ;

		////Print(File.Exists(s));
		////Print(File.GetAttributes(s));

		////Print(Path_.Normalize(@"C:/aaaaaaaaa/bbbbb/../aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
		//return;

		//string s;
		//s = @"D:\Test\protected";
		//s = @"D:\Test\protected\no.txt"; //false
		//s = @"D:\Test\protected\f.txt"; //exception 'access denied'
		//s = @"D:\";
		//s = @"K:\"; //false
		//s = @"D:\<>\mm";
		//Print(Files.GetProperties(s, out var x));
		//PrintList(x.Attributes.ToString("X"), x.Size, x.LastWriteTimeUtc, x.CreationTimeUtc, x.LastAccessTimeUtc);

		foreach(var f in Files.EnumDirectory(@"C:", FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
			string k = f.FullPath;

			var ea = Files.ExistsAs2(k);
			switch(ea) {
			case FileDir2.File: case FileDir2.Directory: break;
			default:
				PrintList(ea, k);
				break;
			}

			//Print(k); continue;
			//if(Files.GetProperties(k, out var x, FAFlags.DoNotThrow)) {
			//	PrintList(x.Attributes.ToString("X"), x.Size, x.LastWriteTimeUtc, x.CreationTimeUtc, x.LastAccessTimeUtc, k);
			//} else {
			//	var es = Native.GetErrorMessage();
			//	PrintList("FAILED", k, Files.ExistsAsAny(k), f.FullPath.Length, es);
			//}
		}

		Print("end");
	}

	static void TestCalculateDriveSize()
	{
		//foreach(var f in Files.EnumDirectory(@"C:\")) {
		//	if(!f.IsDirectory) continue;
		//	PrintList(Files.CalculateDirectorySize(f.FullPath) / 1024 / 1024, f);
		//}

		foreach(var f in Files.EnumDirectory(@"C:\", filter: t => t.IsDirectory && 0 == (t.Attributes & FileAttributes.Hidden))) {
			PrintList(Files.CalculateDirectorySize(f.FullPath) / 1024 / 1024, f);
		}

		//foreach(var f in Files.EnumDirectory(@"D:\")) {
		//foreach(var f in Files.EnumDirectory(@"\\.\D\")) { //no
		//foreach(var f in Files.EnumDirectory(@"\\localhost\D$")) { //yes
		//	if(!f.IsDirectory) continue;
		//	Print(f);
		//}

		//Files.Copy("D:", @"Q:\Test\D", IfExists.Delete);

		Print("END");
	}

	[DllImport("kernel32.dll", EntryPoint = "RemoveDirectoryW")]
	internal static extern bool RemoveDirectory(string lpPathName);

	static void TestCreateDirectory()
	{
		//var s = @"D:\Test\x\new\new2\new3";
		//var s = @"D:\Test\x\new";
		//Files.Delete(@"D:\Test\x\new", false);
		//RemoveDirectory(s);
		//Wait(2);

		//Print(Files.CreateDirectory(s));

		//string path = @"D:\Test\new\test.txt";
		//Files.CreateDirectoryFor(path);
		//File.WriteAllText(path, "text");

		//Files.CopyTo(@"D:\Test\copy.txt", @"D:\Test\new\mew");

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Api.GetFileAttributes(s); });
		//var a2 = new Action(() => { Files.ExistsAsDirectory(s); });
		//var a3 = new Action(() => { Api.GetFileAttributes(Path_.Normalize(s)); });
		//var a4 = new Action(() => { Files.ExistsAsDirectory(Path_.Normalize(s)); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
	}

	//static unsafe string GetFullPath1(string s)
	//{
	//	int na = s.Length + 10;
	//	var b = new StringBuilder(na);
	//	int nr = (int)GetFullPathName(s, (uint)na, b, null);
	//	if(nr > 0 && nr < na) s = b.ToString();
	//	return s;
	//}
	//[DllImport("kernel32.dll", EntryPoint = "GetFullPathNameW", SetLastError = true)]
	//internal static extern unsafe uint GetFullPathName(string lpFileName, uint nBufferLength, [Out] StringBuilder lpBuffer, char** lpFilePart);

	//static unsafe string GetFullPath2(string s)
	//{
	//	int na = s.Length + 10;
	//	_sb.EnsureCapacity(na);
	//	int nr = (int)GetFullPathName(s, (uint)na, _sb, null);
	//	if(nr > 0 && nr < na) s = _sb.ToString();
	//	return s;
	//}
	//[ThreadStatic] static StringBuilder _sb = new StringBuilder();

	//static unsafe string GetFullPath3(string s)
	//{
	//	int na = s.Length + 10;
	//	var p = stackalloc char[na];
	//	int nr = GetFullPathName(s, na, p, null);
	//	if(nr > 0 && nr < na) s = new string(p, 0, nr);
	//	return s;
	//}
	//[DllImport("kernel32.dll", EntryPoint = "GetFullPathNameW", SetLastError = true)]
	//internal static extern unsafe int GetFullPathName(string lpFileName, int nBufferLength, char* lpBuffer, char** lpFilePart);

	//static unsafe string GetFullPath4(string s)
	//{
	//	int na = s.Length + 10;
	//	var p = new char[na];
	//	int nr = GetFullPathName(s, na, p, null);
	//	if(nr > 0 && nr < na) s = new string(p, 0, nr);
	//	return s;
	//}
	//[DllImport("kernel32.dll", EntryPoint = "GetFullPathNameW", SetLastError = true)]
	//internal static extern unsafe int GetFullPathName(string lpFileName, int nBufferLength, char[] lpBuffer, char** lpFilePart);

	//static unsafe string GetFullPath5(string s)
	//{
	//	int na = s.Length + 10;
	//	var ca = new char[na];
	//	fixed (char* p = ca) {
	//		int nr = GetFullPathName(s, na, p, null);
	//		if(nr > 0 && nr < na) s = new string(p, 0, nr);
	//	}
	//	return s;
	//}

	//static unsafe string GetFullPath6(string s)
	//{
	//	int na = s.Length + 10;
	//	if(_charBuffer == null || _charBuffer.Length < na) _charBuffer = new char[na];
	//	int nr = GetFullPathName(s, na, _charBuffer, null);
	//	if(nr > 0 && nr < na) s = new string(_charBuffer, 0, nr);
	//	if(_charBuffer.Length > 5000) _charBuffer = null; //let GC collect the memory
	//	return s;
	//}
	//[ThreadStatic] static char[] _charBuffer;

	//static unsafe string GetFullPath7(string s)
	//{
	//	var b = Catkeys.Util.LibCharBuffer.LibCommon;
	//	int na = s.Length + 10;
	//	int nr = GetFullPathName(s, na, b.Alloc(na), null);
	//	if(nr > 0 && nr < na) s = b.ToString(nr);
	//	b.Compact();
	//	return s;
	//}

	//static void TestApiStringBuilder()
	//{
	//	string s = @"D:\simple\file.txt";
	//	s = @"D:\simple\.\file.txt";

	//	Print(GetFullPath1(s));
	//	Print(GetFullPath2(s));
	//	Print(GetFullPath3(s));
	//	Print(GetFullPath4(s));
	//	Print(GetFullPath5(s));
	//	Print(GetFullPath6(s));
	//	Print(GetFullPath7(s));

	//	Perf.SpinCPU(100);
	//	var a1 = new Action(() => { GetFullPath1(s); });
	//	var a2 = new Action(() => { GetFullPath2(s); });
	//	var a3 = new Action(() => { GetFullPath3(s); });
	//	var a4 = new Action(() => { GetFullPath4(s); });
	//	var a5 = new Action(() => { GetFullPath5(s); });
	//	var a6 = new Action(() => { GetFullPath6(s); });
	//	var a7 = new Action(() => { GetFullPath7(s); });
	//	Perf.ExecuteMulti(7, 1000, a1, a2, a3, a4, a5, a6, a7);

	//}

	static void TestDrivePath()
	{
		//string s;
		//s = @"C:\";
		//s = @"C:";
		////s = @"C:\etc\";
		////s = @"\";

		////Print(Path.GetFullPath(s));
		//Print(Path_.Normalize(s));
		//Print(Path_.Normalize(s, PNFlags.DoNotRemoveEndSeparator));

		////FAFlags flags = 0;
		//////flags|=FAFlags.RawPath;
		////if(Files.GetAttributes(s, out var a, flags)) PrintHex(a); else Print("not found");

		//Print(Files.SearchPath(s));

		//return;

		//var m = new CatMenu();
		//m[@"C:\", @"C:\"] = null;
		//m[@"C:", @"C:"] = null;
		//m[@"\\?\C:\", @"\\?\C:\"] = null;
		//m[@"\\?\C:", @"\\?\C:"] = null;
		//m[@"\\?\UNC\localhost\C$\", @"\\?\UNC\localhost\C$\"] = null;
		//m.Show();

		//Perf.SpinCPU(100);
		//var s = @"D:\Test\x";
		//var a1 = new Action(() => { Files.ExistsAsDirectory(s); });
		//var a2 = new Action(() => { Files.CreateDirectory(s); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	//tested some Path_ functions. This func was in Path_ class.
	//public static void Test()
	//{
	//	//Print(_GetPrefixLength(@""));
	//	//Print(_GetPrefixLength(@"\\?\"));
	//	//Print(_GetPrefixLength(@"\\?\more"));
	//	//Print(_GetPrefixLength(@"\\?more"));
	//	//Print(_GetPrefixLength(@"//?/ddd"));
	//	//Print(_GetPrefixLength(@"\\?\UNC\"));
	//	//Print(_GetPrefixLength(@"\\?\unc\"));
	//	//Print(_GetPrefixLength(@"\\?\UNC\more"));
	//	//Print(_GetPrefixLength(@"//?/unc/more"));
	//	//Print(_GetPrefixLength(@"\\?\UKC\more"));
	//	//Print("---");
	//	//Print(GetRootLength(@""));
	//	//Print(GetRootLength(@"\\?\"));
	//	//Print(GetRootLength(@"\\?\more"));
	//	//Print(GetRootLength(@"\\?more"));
	//	//Print(GetRootLength(@"//?/ddd"));
	//	//Print(GetRootLength(@"\\?\UNC\"));
	//	//Print(GetRootLength(@"\\?\unc\"));
	//	//Print(GetRootLength(@"\\?\UNC\more"));
	//	//Print(GetRootLength(@"//?/unc/more"));
	//	//Print(GetRootLength(@"\\?\UKC\more"));
	//	//Print("---");
	//	//Print(GetRootLength(@"C"));
	//	//Print(GetRootLength(@"C:"));
	//	//Print(GetRootLength(@"C:\"));
	//	//Print(GetRootLength(@"C:etc"));
	//	//Print("---");
	//	//Print(GetRootLength(@"\\A"));
	//	//Print(GetRootLength(@"\\A\"));
	//	//Print(GetRootLength(@"\\A\B"));
	//	//Print(GetRootLength(@"\\A\B\"));
	//	//Print(GetRootLength(@"\\A\B\CDE"));
	//	//Print("---");
	//	//Print(GetRootLength(@"\\?\UNC\A"));
	//	//Print(GetRootLength(@"\\?\UNC\A\"));
	//	//Print(GetRootLength(@"\\?\UNC\A\B"));
	//	//Print(GetRootLength(@"\\?\UNC\A\B\"));
	//	//Print(GetRootLength(@"\\?\UNC\A\B\CDE"));
	//	//Print("---");
	//	//Print(GetRootLength(@"\\?\C"));
	//	//Print(GetRootLength(@"\\?\C:"));
	//	//Print(GetRootLength(@"\\?\C:\"));
	//	//Print(GetRootLength(@"\\?\C:\more"));

	//	string s;
	//	s = @"C:\etc";
	//	//s = @"\\server\share\etc";
	//	//s = @"//server/share/etc";
	//	s = @"\\?\C:\etc";
	//	s = @"//?/C:\etc";
	//	//s = @"\\?\server/share/etc";
	//	s = @"\\?\UNC\server/share/etc";
	//	s = @"//?/UNC/server/share/etc";

	//	//Print(s = PrefixLongPath(s));
	//	Print(UnprefixLongPath(s));
	//}

	static void TestPathGetFilenameEtc()
	{
		var a = new string[]
			{
				@"C:\aa\bb\file.txt",
				@"\\PC\SF\aa\bb\etc.moo",
				@"\\?\C:\aa\bb\file.txt",
				@"\\?\UNC\PC\SF\aa\bb\etc.moo",
				@"C",
				@"C:",
				@"C:\",
				@"\\?\UNC\PC\SF\",
				@"C:\aa\bb\file",
				@"C:\aa\bb\.txt",
				@"file.txt",
				@"file",
				@".txt",
				@"file.",
				@"\\",
				@"\",
				@"C:\aa\bb\..",
				@"C:\aa\bb\/",
				@"C:\A\/B\/",
				@"C:\aa\bb\/\/file.txt",
				@"C:\aa\file.txt:alt.stream",
				@"file:///aaa",
				@"\file.txt",
				@"\\file.txt",
				@"\\?\",
				@"::{A}\::{B}",
				@"::{A}",
				@"::",
				null,
				@"http://www.quickmacros.com",
				@"http:",
				@"http://",
				@"http://www.quickmacros.com:55",
				@"http://www.quickmacros.com:",
			};

		foreach(string v in a) {
			var dir = Path_.GetDirectoryPath(v); if(dir == null) dir = "<null>";
			var dir2 = Path_.GetDirectoryPath(v, true); if(dir2 == null) dir2 = "<null>";
			var name = Path_.GetFileName(v); if(name == null) name = "<null>";
			var name2 = Path_.GetFileNameWithoutExtension(v); if(name2 == null) name2 = "<null>";
			var ext = Path_.GetExtension(v); if(ext == null) ext = "<null>";
			var root = v?.Substring(0, Path_.GetRootLength(v)); if(root == null) root = "<null>";
			Print("<><Z 0x80E080>" + v + "</Z>");
			Print($"{dir,-25} {dir2,-25} {name,-20} {name2,-20} {ext,-20} {root,-20}");
		}
	}

	static void TestExistsAsProtected()
	{
		string s = @"C:\pagefile.sys";
		//s = @"D:\Test\protected\f.txt";
		Print(File.Exists(s));
		Print(Files.ExistsAs(s));
		Print(Files.ExistsAs2(s));

		//if(Files.GetAttributes(s, out var v)) Print(v);
		if(Files.GetAttributes(s, out var v, FAFlags.DoNotThrow)) Print(v);
		if(Files.GetProperties(s, out var p)) {
			PrintList(p.Size, p.Attributes);
			PrintList(p.CreationTimeUtc, p.LastWriteTimeUtc, p.LastAccessTimeUtc);
		}

		//foreach(var f in Files.EnumDirectory(@"C:\", FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
		//	//if(!Files.ExistsAsAny(f.FullPath)) Print(f);
		//	if(Files.ExistsAs2(f.FullPath) == FileDir2.AccessDenied) Print(f);
		//}
		//Print("end");
	}

	static void TestFilesSearchPath()
	{
		Print(Files.SearchPath(@"notepad.exe"));
		Print(Files.SearchPath(@"tests.exe"));
		Print(Files.SearchPath(@"copy.txt", @"D:\Test"));
		Directory.SetCurrentDirectory(@"D:\Test");
		Print(Files.SearchPath(@"copy.txt"));

		//var rk = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
		//foreach(var k in Registry.LocalMachine.OpenSubKey(rk).GetSubKeyNames()) {
		//	var s = Files.SearchPath(k);
		//	//Print(k);
		//	PrintList(k, s);
		//}
	}

	static void TestKnownFoldersAsEnvVar()
	{
		//Task.Run(() =>
		//{
		//	Print(Folders.GetFolder("Local Documents"));
		//	Print(Folders.GetFolder("Documents"));
		//	Print(Folders.GetFolder("ThisApp"));
		//});
		//Wait(1);
		//return;

		Print(Folders.GetFolder("ThisApp"));
		Print(Folders.GetFolder("Documents"));
		Print(Folders.GetFolder("LocalAppData"));
		Print(Folders.GetFolder("Local Documents"));
		Print(Folders.GetFolder("Local AppData"));

		return;

		var s = @"%temp%\etc";
		var s2 = @"%Folders.Documents%";
		Print(Environment.ExpandEnvironmentVariables(s));
		Print(Path_.ExpandEnvVar(s));
		Print(Path_.ExpandEnvVar(s2));

		Perf.SpinCPU(100);
		var a1 = new Action(() => { Environment.ExpandEnvironmentVariables(s); });
		var a2 = new Action(() => { Path_.ExpandEnvVar(s); });
		var a3 = new Action(() => { Path_.ExpandEnvVar(s2); });
		var a4 = new Action(() => { var u = Folders.Documents; });
		var a5 = new Action(() => { var u = Folders.GetFolder("Local Documents"); });
		var a6 = new Action(() => { var u = Folders.GetFolder("Documents"); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6);

	}

	static unsafe void TestPidl()
	{
		//Process.Start(@"::{26EE0668-A00A-44D7-9371-BEB064C98683}\0\::{A3DD4F92-658A-410F-84FD-6FBBBEF2FFFE}");

		string s = ":: 14001F706806EE260AA0D7449371BEB064C98683";
		//		string s2 = null;
		//#if false
		//		s = "14001F706806EE260AA0D7449371BEB064C98683";
		//		var a = Convert_.BytesFromHexString(s);
		//		s2 = Convert_.BytesToHexString(a);
		//#else
		//		var a = new byte[(s.Length - 3)/2];
		//		fixed(byte* p = a){
		//			Convert_.BytesFromHexString(s, p, a.Length, 3);
		//			s2 = Convert_.BytesToHexString(p, a.Length, false);
		//		}
		//#endif
		//		Print(a);
		//		Print(s2);

		//using(var pidl = Shell.Pidl.FromString(s)) {

		//}

		//s =Folders.System+"notepad.exe";
		//s ="notepad.exe";
		s = @"%Folders.Virtual.ControlPanel%";
		s = "%Folders.Virtual.ControlPanelFolder%";
		//s = ":: ";
		//s = ":: 14001F706806EE260AA0D7449371BEB064C98683";
		Shell.Run(s);
		//Shell.Run(@"Q:\app\qm.exe", flags:SRFlags.SupportExtVerbs, verb:"properties"); Wait(0.1);
		//TaskDialog.Show();

		s = @"%windir%\etc";
		s = @"%Folders.System%\etc";
		//s = "%Folders.Quick Launch%";
		//s = @"%Folders.Virtual.ControlPanel%\etc";
		//s = "%Folders.Virtual.ControlPanelFolder%";
		//Print(Path_.ExpandEnvVar(s));

		//s =":: ";
		//s = ":: 14001F706806EE260AA0D7449371BEB064C98683";
		//var pidl = Shell.Pidl.FromString(s);
		//Print(pidl.ToHexString());
		//Print(pidl.ToShellString());
		//Print(Shell.Pidl.FromString(pidl.ToHexString()).ToHexString());

		//s =@"C:\Windows\/System32\notepad.exe";
		//s =@"C:\Windows\System32\..\notepad.exe";
		//s =@"%windir%\System32\notepad.exe";
		//if(!Files.Misc.GetFileId(s, out var k)) Print("failed");
		//else Print(k.FileIndex);
	}

	static void TestLnkShortcut3()
	{
		var lnk = @"Q:\Test\lnk\favorites.lnk";
		lnk = @"Q:\Test\lnk\virtual.lnk";
#if true
		var x = Shell.Shortcut.Create(lnk);
		//x.TargetPidl = Folders.VirtualPidl.Connections;
		//x.TargetPidl = Folders.VirtualPidl.Internet; //invalid
		//x.TargetPidl = Folders.VirtualPidl.AddNewPrograms;
		//x.TargetPidl = Folders.VirtualPidl.AppUpdates;
		//x.TargetPidl = Folders.VirtualPidl.Apps_Win8;
		//x.TargetPidl = Folders.VirtualPidl.ChangeRemovePrograms;
		//x.TargetPidl = Folders.VirtualPidl.Computer;
		//x.TargetPidl = Folders.VirtualPidl.Conflict;
		//x.TargetPidl = Folders.VirtualPidl.ControlPanel;
		//x.TargetPidl = Folders.VirtualPidl.RecycleBin;
		x.TargetAnyType = ":: 14001F706806EE260AA0D7449371BEB064C98683";
		x.Save();
		x.Dispose();
		Wait(1);
#endif
		//#else
		var xx = Shell.Shortcut.Open(lnk);
		var s = xx.TargetAnyType;
		Print(s);
		Shell.Run(s);
		//#endif
	}

	static void TestPidlToString2()
	{
		string s1, s2, s3, s4, s5;
		s1 = @"Q:\app\qm.exe";
		s2 = @"http://www.quickmacros.com";
		s3 = @"::{26EE0668-A00A-44D7-9371-BEB064C98683}\0\::{A3DD4F92-658A-410F-84FD-6FBBBEF2FFFE}";
		s4 = @"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";
		s5 = @"mailto:support@quickmacros.com";
		//s4= @"Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";
		var p1 = Shell.Pidl.FromString(s1);
		var p2 = Shell.Pidl.FromString(s2);
		var p3 = Shell.Pidl.FromString(s3);
		var p4 = Shell.Pidl.FromString(s4);
		var p5 = Shell.Pidl.FromString(s5);

		Print(p1.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING));
		Print(p2.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING));
		Print(p3.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING));
		Print(p4.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING));
		Print(p5.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING));
		Print(p1.ToString());
		Print(p2.ToString());
		Print(p3.ToString());
		Print(p4.ToString());
		Print(p5.ToString());

		Perf.SpinCPU(100);
		var a1 = new Action(() => { p1.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING); });
		var a2 = new Action(() => { p2.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING); });
		var a3 = new Action(() => { p3.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING); });
		var a4 = new Action(() => { p4.ToShellString(Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING); });
		var a5 = new Action(() => { p1.ToString(); });
		var a6 = new Action(() => { p2.ToString(); });
		var a7 = new Action(() => { p3.ToString(); });
		var a8 = new Action(() => { p4.ToString(); });
		var a9 = new Action(() => { p3.ToHexString(); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7, a8, a9);
	}

	static void TestIconWithPidl()
	{
		string s1, s2, s3, s4, s5, s6;
		s1 = @"Q:\app\qm.exe";
		s2 = @"https://www.google.com";
		s3 = @"::{26EE0668-A00A-44D7-9371-BEB064C98683}\0\::{A3DD4F92-658A-410F-84FD-6FBBBEF2FFFE}";
		s4 = @"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";
		s5 = @"mailto:support@quickmacros.com";
		s6 = ":: 14001F706806EE260AA0D7449371BEB064C98683";

		var m = new CatMenu();
		m[s1, s1] = o => Shell.Run(o.ToString());
		m[s2, s2] = o => Shell.Run(o.ToString());
		m[s3, s3] = o => Shell.Run(o.ToString());
		m[s4, s4] = o => Shell.Run(o.ToString());
		m[s5, s5] = o => Shell.Run(o.ToString());
		m[s6, s6] = o => Shell.Run(o.ToString());
		m.Show();
	}

	static void TestVersion()
	{
		//Print($"Is64BitOS={Ver.Is64BitOS}, Is64BitProcess={Ver.Is64BitProcess}, Is32BitProcessOn64BitOS={Ver.Is32BitProcessOn64BitOS}");
		//PrintHex(Ver.WinVer);
		//PrintList(Ver.MinWin8, Ver.MinWin8_1, Ver.MinWin10);

		//Print(Folders.System);
		//Print(Folders.SystemX86);
		//Print(Folders.SystemX64);
		//Print(Files.ExistsAsAny(Folders.SystemX64));
		////Print(Files.Misc.DisableRedirection.GetNonRedirectedSystemPath(@"c:\windows\system32\mm"));
		//Print(Folders.ProgramFiles);
		//Print(Folders.ProgramFilesX86);
		//Print(Folders.ProgramFilesX64);
		//Print(Folders.ProgramFilesCommon);
		//Print(Folders.ProgramFilesCommonX86);
		//Print(Folders.ProgramFilesCommonX64);
		//Print(Folders.UserProgramFiles);

		//Print("---");
		//Print(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

		//int n = 0;
		//foreach(var v in Files.EnumDirectory(Folders.System, FEFlags.DisableRedirection)) {
		//	n++;
		//}
		//Print(n);

		//Environment.SetEnvironmentVariable("TMP", @"Q:/Test/Temp/x/../");
		//Print(Folders.Temp);

		//var s = Folders.System + "gfxuiex.exe";
		////Process.Start(s);
		//Shell.Run(s);
	}

	//static unsafe void TestBytesFromHexStringNew()
	//{
	//	//string s = ":: 1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A";
	//	//int offs = 3;
	//	//string s = "1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A";
	//	//string s = "1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A";
	//	string s = "1E 00 715800000000000000000000661AA9A87D3A24448D2404E180695C7A";
	//	int offs = 0;
	//	int n = (s.Length - offs) / 2;

	//	var b1 = stackalloc byte[n];
	//	int n1 = Convert_.BytesFromHexString_old(s, b1, n, offs);
	//	Print(Convert_.BytesToHexString(b1, n1, true));

	//	var b2 = stackalloc byte[n];
	//	int n2 = Convert_.BytesFromHexString(s, b2, n, offs);
	//	Print(Convert_.BytesToHexString(b2, n2, true));

	//	var b3=Convert_.BytesFromHexString(s);
	//	Print(Convert_.BytesToHexString(b3, true));

	//	PrintList(n, n1, n2, b3.Length);

	//	Perf.SpinCPU(100);
	//	var a1 = new Action(() => { Convert_.BytesFromHexString_old(s, b1, n, 3); });
	//	var a2 = new Action(() => { Convert_.BytesFromHexString(s, b2, n, 3); });
	//	var a3 = new Action(() => { Convert_.BytesFromHexString(s); });
	//	var a4 = new Action(() => { });
	//	Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	//}

	static void TestTemp()
	{
		//var m = new CatMenu();
		//m.IconFlags = Icons.IconFlags.SearchPath;
		//m["one", @"Q:\app\copy.ico"] = null;
		//m["one", @"CatkeysTasks.csproj"] = null;
		//Folders.ThisAppImages = @"q:\app";
		//m["one", @"paste.ico"] = null;
		//m["one", @"notepad.exe"] = null;
		//m.Show();

		//Print(Files.SearchPath(@"Q:\app\.\catkeys"));
		//Print(Files.SearchPath(@"notepad.exe"));
		//Print(Files.SearchPath(@"qm.exe"));
		//Print(Files.SearchPath(@"catkeys\.\", @"q:\app\.\"));
		//Print(Files.SearchPath(@"catkeys.dll"));

		//Environment.SetEnvironmentVariable("temp", "not full path");
		//string s = @"%temp%\mm";
		//Print(Path_.IsFullPathEEV(ref s));
		//Print(s);
	}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void TestNativeMemoryList()
	//{
	//	//using(var a = new Catkeys.Util.LibArrayBuilder(8, 0)) { }
	//	//using(var a = new Catkeys.Util.LibArrayBuilder.Specialized._Wnd(1024)) { }
	//	//var a = new Catkeys.Util.LibArrayBuilder.Specialized._Wnd(1024);

	//	//using(var a = new Catkeys.Util.LibArrayBuilder.Specialized._Wnd(1024)) {
	//	//	a.Add(Wnd.Find("Quick*"));
	//	//	a.Add(Wnd.Find("app*"));

	//	//	Print(a.ToArray());
	//	//}

	//	using(Catkeys.Util.LibArrayBuilder.Specialized._Wnd a = new Catkeys.Util.LibArrayBuilder.Specialized._Wnd(1024), aa = new Catkeys.Util.LibArrayBuilder.Specialized._Wnd(1024)) {
	//		a.Add(Wnd.Find("Quick*"));
	//		a.Add(Wnd.Find("app*"));

	//		aa.Add(Wnd.Find("Moz*"));
	//		aa.Add(Wnd.Find("Q*"));

	//		Print(a.ToArray(aa));
	//	}

	//	//using(var a = new Catkeys.Util.LibArrayBuilder<int>()) {
	//	//using(var a = new Catkeys.Util.LibArrayBuilder(4, 0)) {

	//	//	var p0 = a.Alloc(4);
	//	//	//Print((LPARAM)p0);
	//	//	for(int i = 0; i < a.Count; i++) {
	//	//		//Print((LPARAM)a[i]);
	//	//		*(int*)a[i] = i;
	//	//	}

	//	//	//a.ReAlloc(10);

	//	//	for(int i = a.Count; i < 25; i++) {
	//	//		//*(int*)a.Add() = i;
	//	//		*(int*)a.AddFast() = i;
	//	//	}

	//	//	for(int i = 0; i < a.Count; i++) {
	//	//		//Print((LPARAM)a[i]);
	//	//		Print(*(int*)a[i]);
	//	//	}

	//	//	Print(a.Capacity);

	//	//	Output.Clear();
	//	//	var b = new int[a.Count];
	//	//	//var b = new List<int>(); //cannot get address of its array
	//	//	//for(int i = 0; i < b.Length; i++) b[i] = *(int*)a[i];
	//	//	fixed (int* p = b) a.CopyTo(p);
	//	//	//Print(b);
	//	//	Print(b.ToList());
	//	//}

	//	//using(var a = new Catkeys.Util.LibArrayBuilder(8, 0)) {

	//	//	var p0 = a.Alloc(4);
	//	//	//Print((LPARAM)p0);
	//	//	for(int i = 0; i < a.Count; i++) {
	//	//		//Print((LPARAM)a[i]);
	//	//		*(Point*)a[i] = new Point(i, i * 10);
	//	//	}

	//	//	//a.ReAlloc(10);

	//	//	for(int i = a.Count; i < 25; i++) {
	//	//		//*(int*)a.Add() = i;
	//	//		*(Point*)a.AddFast() = new Point(i, i * 10);
	//	//	}

	//	//	for(int i = 0; i < a.Count; i++) {
	//	//		//Print((LPARAM)a[i]);
	//	//		Print(*(Point*)a[i]);
	//	//	}

	//	//	Print(a.Capacity);
	//	//}


	//	//Perf.Next();
	//	//var a1 = new Catkeys.Util.LibArrayBuilder<int>();
	//	//Perf.Next();
	//	//var a2 = new Catkeys.Util.LibArrayBuilder<Point>();
	//	//Perf.Next();
	//	//var a3 = new Catkeys.Util.LibArrayBuilder<RECT>();
	//	//Perf.Next();
	//	//var a4 = new Catkeys.Util.LibArrayBuilder<Wnd>();
	//	//Perf.Next();
	//	//var a5 = new Catkeys.Util.LibArrayBuilder<byte>();
	//	//Perf.Next();
	//	//var a6 = new Catkeys.Util.LibArrayBuilder<double>();
	//	//Perf.Next();
	//	//var a7 = new Catkeys.Util.LibArrayBuilder<Rectangle>();
	//	//Perf.Next();
	//	//var a8 = new Catkeys.Util.LibArrayBuilder<Point>();
	//	//Perf.Next();
	//	//var a9 = new Catkeys.Util.LibArrayBuilder<char>();
	//	//Perf.Next();
	//	//Perf.NW();

	//	//Perf.First();
	//	//a1.Add();
	//	//Perf.Next();
	//	//a2.Add();
	//	//Perf.Next();
	//	//a3.Add();
	//	//Perf.Next();
	//	//a4.Add();
	//	//Perf.Next();
	//	//a5.Add();
	//	//Perf.Next();
	//	//a6.Add();
	//	//Perf.Next();
	//	//a7.Add();
	//	//Perf.Next();
	//	//a8.Add();
	//	//Perf.Next();
	//	//a9.Add();
	//	//Perf.NW();

	//	//var b = new LibNativeMemoryList<RECT>();
	//	//var a2 = new LibNativeMemoryList2<Wnd>();
	//	//var b2 = new LibNativeMemoryList2<RECT>();


	//	//int n1 = 16384, n2 = 20480;
	//	//Perf.SpinCPU(100);
	//	//var a1 = new Action(() => { var a = Marshal.AllocHGlobal(n1); Marshal.FreeHGlobal(a); });
	//	//var a2 = new Action(() => { var a = Marshal.AllocCoTaskMem(n1); Marshal.FreeCoTaskMem(a); });
	//	//var a3 = new Action(() => { var a = Catkeys.Util.NativeHeap.Alloc(n1); Catkeys.Util.NativeHeap.Free(a); });
	//	////var a1 = new Action(() => { var a = Marshal.AllocHGlobal(n1); a = Marshal.ReAllocHGlobal(a, (IntPtr)n2); Marshal.FreeHGlobal(a); });
	//	////var a2 = new Action(() => { var a = Marshal.AllocCoTaskMem(n1); a = Marshal.ReAllocCoTaskMem(a, n2); Marshal.FreeCoTaskMem(a); });
	//	////var a3 = new Action(() => { var a = Catkeys.Util.NativeHeap.Alloc(n1); a = Catkeys.Util.NativeHeap.ReAlloc(a, n2); Catkeys.Util.NativeHeap.Free(a); });
	//	//var a4 = new Action(() => { var a = Api.VirtualAlloc(Zero, n1, 0x3000, 4); Api.VirtualFree(a); });
	//	//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	//	//Perf.Next();
	//	//int v1 = TypeSize<int>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<byte>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<IntPtr>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<RECT>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<int>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<Wnd>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<double>.Size;
	//	//Perf.Next();
	//	//v1 = TypeSize<long>.Size;
	//	//Perf.NW();

	//	//Print(TypeSize<IntPtr>.Size);
	//}

	static void TestShellRun()
	{
		//Perf.First();
		Thread.CurrentThread.Join(0); //fixes it (removes the wait cursor)

		//Time.DoEvents(); //does not fix
		//Time.SleepDoEvents(1); //fixes. Need min 1. But Thread.Join much faster.
		//Perf.NW();

		//Wait(10);
		//Thread.CurrentThread.Join(10000);
		//return;
		//TODO: Thread.Sleep causes 'wait' cursor when mouse is on 'Close' button of any window. But WaitOne and Thread.Join don't.

		//Shell.Run("notepad.exe", @"""%Folders.Documents%\Test.py""");
		//Shell.Run("notepad.exe", "Test.py", directory: "%Folders.Documents%");
		//Shell.Run(@"Q:\Test\x\..\am.txt");
		//Print(Path_.Normalize("C:"));
		//Shell.Run(@"C:");
		//Shell.Run(@"\\?\C:\Windows\notepad.exe");
		//Shell.Run(@"\\?\Q:\Test\x\..\am.txt");

#if false
		//Shell.Run("notepad.exe", flags: SRFlags.ReturnProcessHandle, more: new SRParams() { Verb = "properties" });
		var f = new Form();
		f.Click += (unu, sed) =>
		  {
			  Shell.Run("notepad.exe", flags: SRFlags.ReturnProcessHandle, more: new SRParams() { Verb = "properties", OwnerWindow=f });
			  //try { Shell.Run("no.exe", flags: SRFlags.ShowErrorUI, more: new SRParams() { OwnerWindow = f }); } catch(CatException) { }

		  };
		f.ShowDialog();
#elif true
		//Shell.Run(@"Q:\my qm\test_run_dir.exe");
		//Shell.Run(@"Q:\my qm\test_run_dir.exe", more: new SRParams() { CurrentDirectory=@"C:\Windows" });
		//Shell.Run(@"Q:\");
		//Shell.Run(@"notepad.exe", more: new SRParams() { WindowState = ProcessWindowStyle.Minimized });

		//var p = new SRParams() { NeedProcessHandle = true };
		//Shell.Run(@"notepad.exe", more: p);
		//using(var h = p.ProcessHandle) h?.WaitOne();

		//Print(Shell.Run(@"notepad.exe", flags: SRFlags.WaitForExit));
		//Shell.Run(@"mailto:qmgindi@gmail.com");

		//Wnd w = Wnd.Find("Settings");
		////Wnd w = Wnd.Find("Quick*");
		//Print(Wnd.Misc.GetWindowsStoreAppId(w, true, true));
		//Wnd w = Wnd.Find("- Notepad", "Notepad", Shell.Run("notepad.exe"));

		//Wnd w = Wnd.Find("*- Notepad", "Notepad");
		//if(!w.Is0) w.Activate();
		//else w = WaitFor.WindowActive(10, "*- Notepad", "Notepad", Shell.Run("notepad.exe"));
		//Print(w);

		//var f = new Wnd.Finder("*- Notepad", "Notepad");
		//if(f.Find()) f.Result.Activate();
		//else { f.ProcessId = Shell.Run("notepad.exe"); WaitFor.WindowActive(f, 5); }
		//else WaitFor.WindowActive(f, 5, Shell.Run("notepad.exe"));
		//else f.RunWaitActive(5, "notepad.exe");

		//for(int i = 0; i < 3; i++) {
		//	Wnd w = WaitFor.WindowActive(10, "*- Notepad", "Notepad", Shell.Run("notepad.exe"));
		//	Print(w);
		//	Wait(1);
		//}

		//Wnd w = Wnd.Find("*- Notepad", "Notepad");
		//if(w.Is0) { Shell.Run("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind, 10); }
		//w.Activate();
		//this code does the same without LastFind
		//var f = new Wnd.Finder("*- Notepad", "Notepad");
		//if(!f.Find()) { Shell.Run("notepad.exe"); WaitFor.WindowActive(f, 5); }
		//f.Result.Activate();

		//Wnd w = Wnd.Find("*- Notepad", "Notepad");
		//if(w.Is0) { Process.Start("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind, 5); }
		//w.Activate();

		//if(w.Is0) { var f = Wnd.LastFind; f.ProcessId = Shell.Run("notepad.exe"); w = WaitFor.WindowActive(f); }
		//if(w.Is0) { Wnd.LastFind.ProcessId = Shell.Run("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind); }
		//if(w.Is0) w = WaitFor.WindowActive(5, Shell.Run("notepad.exe"));

		//Print(w);
		//Print(f.Result);
#else
		//var thread = new Thread(() =>
		//  {
		//Print(Thread.CurrentThread.GetApartmentState());
		string s;
		s = @"notepad.exe";
		//s =@"Q:\Test\am.txt";
		//s = @"Q:\Test\CatkeysHelp.chm";
		//s = @"Q:\Test\CatkeysHelp.chm - Shortcut";
		//s = Folders.Virtual.ControlPanel + "1E00715800000000000000000000661AA9A87D3A24448D2404E180695C7A";
		Print(s);
		SRParams more = null;
		//more = new SRParams() { Verb = "properties" };
		more = new SRParams() { NeedProcessHandle = true };
		int pid = Shell.Run(s, more: more);
		Print(pid);
		using(var h = more?.ProcessHandle) {
			if(h == null) return;
			//Print(h.SafeWaitHandle.DangerousGetHandle());
			var nh = h.SafeWaitHandle.DangerousGetHandle();

			//var w = WaitFor.WindowExists(5, "* Notepad", programEtc: pid);
			//Print(w);

			h.WaitOne();
			//WaitFor.WindowNotExists(0, "* Notepad", programEtc: pid);
			//WaitFor.WindowClosed(0, w);
		}
#endif
		Print("end");
		//  });
		////thread.SetApartmentState(ApartmentState.STA);
		//thread.Start();
		//thread.Join();

		//var p = new Process();
		//Print(p.Responding);
	}

	static void TestWaitWindow()
	{
		Thread.CurrentThread.Join(0); //removes the wait cursor

		//Wnd w = Wnd.Find("* Notepad");

		////wait max 30 s until window w is active. Exception on timeout or if closed.
		//WaitFor.WindowCondition(w, t => t.IsActive); //the same as WaitFor.WindowActive(w, 30);
		//Print("active");

		////wait indefinitely until window w is minimized or closed
		//WaitFor.WindowCondition(w, t => t.IsMinimized || !t.IsAlive, 0, true);
		//if(!w.IsAlive) { Print("closed"); return; }
		//Print("minimized");

		//Print(Wnd.FindAll("Q*"));
		//var f=new Wnd.Finder("Q*");
		////Print(f.FindAll());
		//Print(f.FindAllInList(Wnd.Misc.AllWindows()));

		//Wnd w = Wnd.Find("Quick*");
		////Print(w.ChildAll("*i*"));
		//Print(new Wnd.ChildFinder("*i*").FindAll(w));



		//Wnd w = Wnd.Find("Quick*");
		//int tid = w.ThreadId;

		//Print(Wnd.Find("*int*", null, tid, WFFlags.ThreadId));

		////Print(Wnd.Misc.FindThreadWindow(tid));
		//Print(Wnd.Misc.FindThreadWindow(tid, "*int*"));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Find("*int*", null, tid, WFFlags.ThreadId); });
		//var a2 = new Action(() => { Wnd.Misc.FindThreadWindow(tid, "*int*"); });
		//var a3 = new Action(() => { Wnd.Find(null, null, tid, WFFlags.ThreadId); });
		//var a4 = new Action(() => { Wnd.Misc.FindThreadWindow(tid); });
		//var a5 = new Action(() => { Wnd.Find("*int*", null, tid, WFFlags.ThreadId | WFFlags.HiddenToo); });
		//var a6 = new Action(() => { Wnd.Misc.FindThreadWindow(tid, "*int*", flags: WFFlags.HiddenToo); });
		//Perf.ExecuteMulti(5, 10, a1, a2, a3, a4, a5, a6);



		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Misc.AllWindows(true); });
		//var a2 = new Action(() => { Wnd.Misc.ThreadWindows(tid); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 10, a1, a2, a3, a4);


		//Print(Wnd.Misc.AllWindows(false, true));
		//Print(Wnd.Misc.ThreadWindows(Wnd.Find("Quick*").ThreadId, false, true));
		//Print(Wnd.Misc.WndDesktop);

		//Print(Wnd.Find("Quick*"));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Find("Quick*"); });
		//var a2 = new Action(() => { });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4);


		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Misc.AllWindows(false, true); });
		//var a2 = new Action(() => { Wnd.Misc.AllWindows2(false, true); });
		//var a3 = new Action(() => { Wnd.Misc.AllWindows3(false, true); });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 10, a1, a2, a3, a4);

		//Wnd w = Wnd.Find("Options");
		////Print(w.ChildById(1103));
		//Print(w.Child("Unicode"));

		//Print(w.AllChildren());
		//Print(w.AllChildren(false, true));
		//Print(w.AllChildren(false, false, true));
		//Print(w.AllChildren(false, false, true, also: c => c.ClassNameIs("Combo*")));

		//Wait(5);
		//try {
		//	WaitFor.WindowExists(30, "no", "mo", null, WFFlags.HiddenToo);
		//}
		//catch(TimeoutException) {
		//	Print("timeout");
		//}
		//GC.Collect();
		//Wait(5);

		//Wnd w = WaitFor.WindowActive(60, "*- Notepad", also: t => !t.IsMinimized);
		//Print(w);

		//Wnd w = WaitFor.WindowActive(3, programEtc:"notepad");
		//Print(w);

		//Print(Wnd.Find("*Notepad"));

		//WaitFor.WindowExists(30, "*Notepad", not: true);
		//WaitFor.WindowActive(30, "*Notepad");
		//Wnd.Find("*Notepad").Activate();
		//WaitFor.WindowActive(30, "*Notepad", not: true);

		//var w = Wnd.Find("Quick*");
		//var w = Wnd.Find("*Notepad");
		//WaitFor.WindowClosed(w);
		//WaitFor.WindowActive(w);
		////WaitFor.WindowVisible(w, not: true);
		////WaitFor.WindowEnabled(w, not: true);
		//Print("yes");
		//WaitFor.WindowActive(w, not: true);
		////WaitFor.WindowVisible(w);
		////WaitFor.WindowEnabled(w);

		////find window containing specified control, and get the control too
		//var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		//Wnd w = Wnd.Find(className: "#32770", also: t => t.HasChild(f));
		//Print(w);
		//Print(f.Result);

		Wnd w = Wnd.Find("Options");
		var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		Wnd c = WaitFor.WindowChildExists(w, f);
		Print(c);
		//Print(WaitFor.WindowChildExists(w, f, -5, true));

		//Wnd w = Wnd.Find(className: "#32770", also: t => f.FindIn(t));


		Print("end");

		//Print(Wnd.Find("Quick*"));
	}

	static void TestMyIEnumerable()
	{
		var a = Wnd.Misc.AllWindows(true);
		//Print(a);
		IEnumerable<Wnd> ew = a;

		Wnd r = default(Wnd);
		Perf.SpinCPU(100);
		var a1 = new Action(() => { foreach(var w in a) { r = w; } });
		var a2 = new Action(() => { foreach(var w in ew) { r = w; } });
		var a3 = new Action(() =>
		{
			using(var en = ew.GetEnumerator()) {
				while(en.MoveNext()) {
					r = en.Current;
					//Print(en.Current);
				}
			}
		});
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		Print(r);

		//var en = ew.GetEnumerator();
		//while(en.MoveNext()) {
		//	var w = en.Current;
		//	//Print(en.Current);
		//}
		//en.Dispose();
		//Print("end");
	}

	static void TestSwitchTypeSpeed()
	{
		//object o = new Point(1, 2);
		object o = new int[2];
		int n = 0;

		Perf.SpinCPU(100);
		for(int j = 0; j < 5; j++) {
			Perf.First();
			for(int i = 0; i < 1000; i++) {
				if(o is string s) n = s.Length;
				//else if(o is int ii) n = 4;
				else if(o is List<Wnd> a2) n = a2.Count;
				else if(o is Wnd[] a1) n = a1.Length;
				//else if(o is uint u) Print(u);
				//else if(o is Wnd w) Print(w);
			}
			Perf.Next();
			for(int i = 0; i < 1000; i++) {
				switch(o) {
				case string s: n = s.Length; break;
				//case int ii: n=4; break;
				case List<Wnd> a2: n = a2.Count; break;
				case Wnd[] a1: n = a1.Length; break;
					//case uint u: Print(u); break;
					//case Wnd w: Print(w); break;
				}
			}
			Perf.NW();
		}
		Print(n);
	}

	static void TestColorLuma()
	{
		//Print(Color_.Brightness0to255(0x000000));
		//Print(Color_.Brightness0to255(0x808080));
		//Print(Color_.Brightness0to255(0xffffff));
		//Print(Color_.Brightness0to255(0xFF0000));
		//Print(Color_.Brightness0to255(0x00FF00));
		//Print(Color_.Brightness0to255(0x0000FF));

		uint c = 0x808080;
		c = 0xff0000;
		c = 0xff00;
		c = 0xff;
		c = 0x8000;
		int n = -250;
		PrintHex(Catkeys.Util.Color_.AdjustLuminance(c, n, false));
		//PrintHex(Color_.AdjustLuminance2(c, n, false));
		//PrintHex(Color_.AdjustLuminance(0xE0E0E0, -800, false));

		//int color =0x808080;
		//color = 0x0000FF;
		//color = 0x00FF00;
		//color = 0xFF0000;
		//color = 0;
		//color = 0xFFFFFF;

		//var c = Color.FromArgb(color);

		//Print(Color_.Brightness0to255((uint)color));
		//Print(Color_.Brightness0to255(c));

		//Print(c.GetBrightness());
	}

	//static void TestDeleteRB()
	//{
	//	Files.Delete(@"G:\New Bitmap Image.bmp", Files.Recycle.YesOrAskUser);
	//}

	static void TestRegexIndexOf()
	{
		string s = "name_38";
		//s = "name_a";
		{
			Print(s.RegexIndexOf_(@"_\d+"));
			Print(s.RegexIndexOf_(@"_\d+", out int len));
			Print(len);
			Print(s.RegexIndexOf_(@"_\d+", out string match));
			Print(match);
		}
		{
			Print(s.RegexIndexOf_(@"_(\d+)", 1));
			Print(s.RegexIndexOf_(@"_(\d+)", out int len, 1));
			Print(len);
			Print(s.RegexIndexOf_(@"_(\d+)", out string match, 1));
			Print(match);
		}
	}

	static void TestMultiDelete()
	{
		//Files.CreateDirectory(@"Q:\Test\delete");
		//var a = new string[30];
		//for(int i = 0; i < a.Length; i++) {
		//	a[i] = @"Q:\Test/delete\a" + i;
		//	File.WriteAllText(a[i], "a");
		//}
		//Wait(2);
		//Perf.First();
		////for(int i = 0; i < a.Length; i++) {
		////	Files.Delete(a[i], true);
		////}
		//Files.Delete(a, true);
		//Perf.NW();


		///var s = @"Q:\app\catkeys\Editor\Test\ok\files\iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii";
		//var s = @"Q:\app\catkeys\Editor\Test\ok\files\short";
		//Files.CreateDirectory(s);

		//Files.CopyTo(s, s);
		//Files.Delete(s, true);
		//Files.Delete(s, false);

		//Print(Path_.Combine(Folders.AdminTools, "dsjdjhsjdhjshdjhsj.ggg"));
		//Print(Path_.Combine(Folders.AdminTools, null));
		//Print(Path_.Combine(null, Folders.AdminTools));
		//Print(Path_.Combine(null, null));
		//Print(Path_.Combine("ignore", "%windir%", true));
		//Print(Path_.Combine(s, "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm"));
		//Print(Path_.Combine(s, "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm", prefixLongPath: false));

		var s = @"q:\test\delete\file.txt";
		var s2 = @"q:\test\delete\file2.txt";
		//File.WriteAllText(s, "a");
		File.WriteAllText(s2, "a");
		Wait(2);
		//Files.Delete(s, true);
		Files.Delete(new string[] { s, s2 }, true);

		Print("end");
	}

	static void TestFileUniqueName()
	{
		Print(Path_.MakeUnique(Folders.ThisApp + "file.txt", false));
		Print(Path_.MakeUnique(Folders.ThisApp + "tests.exe", false));
		Print(Path_.MakeUnique(Folders.ThisApp + "tests.exe", true));
	}

	static void TestDictionary()
	{
		var d = new Dictionary<string, int>();
		d.Add("zero", 0);
		d.Add("one", 1);
		d.Add("two", 2);
		d.Add("three", 3);
		d.Add("four", 4);

		Print(d.ElementAt(2));
	}

	static void TestPngSize()
	{
		//var path = @"Q:\My QM\C8375087.bmp";
		//var b = Image.FromFile(path);
		//b.Save(@"Q:\My QM\C8375087-NET.png", ImageFormat.Png);
		//b.Save(@"Q:\My QM\C8375087-NET.jpg", ImageFormat.Jpeg);

		//var f = new Form();
		//f.Paint += F_Paint;
		//f.ShowDialog();

		var path = @"Q:\My QM\C8375087.bmp";
		byte[] data = File.ReadAllBytes(path);
		Print(data.Length); //1638
		using(var m = new MemoryStream(data)) {
			byte[] compressed = Compress(m);
			Print(compressed.Length); //DeflateStream 422, GZipStream 440
			Print(Convert.ToBase64String(compressed));
		}
	}

	private static byte[] Compress(Stream input)
	{
		using(var compressStream = new MemoryStream())
		using(var compressor = new DeflateStream(compressStream, CompressionLevel.Optimal)) {
			input.CopyTo(compressor);
			compressor.Close();
			return compressStream.ToArray();
		}
	}

	private static void F_Paint(object sender, PaintEventArgs e)
	{
		string png = "iVBORw0KGgoAAAANSUhEUgAAAEwAAAAmBAMAAACc1mAkAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAwUExURRAXDvrTazB+svNbIf3wlHFcL5miwJDI5tnw+5CLfy6V+96bELPW5/X4/GN7ZOuFUiaTKXYAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFVSURBVDjLY3hHFBhByu4SBShWpoQAuujKqlbBQK0uQhRD2VoEC5+yFWlAADSro2OtRkdHR0QHGGAqyykvAypcBFGWHxoaj10ZxDCIss4ABobQH6jK5kKU5YKpPrCy/18DjI2NUZVNgCqbljnz7g2wsv7/X78Clf1AUTaHF6JsiovjzJlzIMr+/wcqs4AruzkTKHMAomyii6CgIA5lZ0CAF2Ia0DAgACmz/2yMYunNk0DTTkKVSYq4uLj8BHkBpMry3Qm4sgsMDAycE6Bu+wm0VDoCpKwfZNjHFyhe4AR7YU3uxIlAZVvnnFmrc+bMeWPjP2fenDmDpAzCWJM7/wvQsK1nwMqA4N27MyjKeCHK8mb+3r17dyROZRCw5sy8p6EMDDwwZefOPcCmbHk5DKxVh9AMQExYWUE5UcrKsSpDSuT4lCEBfFkGF2BIIwrQK9fTTNnduwAf6ikb0c9NvgAAAABJRU5ErkJggg==";
		Perf.First();
		var bytes = Convert.FromBase64String(png);
		Perf.Next();
		using(var m = new MemoryStream(bytes, false)) {
			Perf.Next();
			using(var b = Image.FromStream(m)) {
				Perf.Next();
				e.Graphics.DrawImageUnscaled(b, 0, 0);
				Perf.NW();
			}
		}
	}

	static unsafe void TestConvertUtf8()
	{
		string s = @"	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		Output.Clear();
		Thread.Sleep(100);

		//TODO: InitLibrary. Optional but recommended. Would set error mode, default domain data, etc.

		try {
			//TestPngSize();
			//Print(_big.Length);
			//Print(Convert.FromBase64String(_big).Length);
			//TestDictionary();
		}
		catch(CatException ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(ArgumentException ex) { PrintList(ex); }
		catch(Win32Exception ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(Exception ex) when(!(ex is ThreadAbortException)) { PrintList(ex.GetType(), ex); }
		//catch(Exception ex) { }
		//Why try/catch creates 3 more threads? Does not do it if there is only catch(Exception ex). Only if STA thread.
		catch(CatException ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(ArgumentException ex) { PrintList(ex); }
		catch(Win32Exception ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(Exception ex) when(!(ex is Thre
";
		//Print(s.Length); //1000
		//string s2 = s.Remove(50);

		//var b = new sbyte[4000];
		//fixed (sbyte* p = b) {
		//	int n = ConvertUtf8.ToUtf8(s, p, 4000);
		//	Print(n);
		//	//Print(new string(p, 0, n, Encoding.UTF8));

		//	byte[] r = null;

		//	Perf.SpinCPU(100);
		//	var a1 = new Action(() => { r = Encoding.UTF8.GetBytes(s); });
		//	var a2 = new Action(() => {
		//		fixed(sbyte* pp = b) {
		//			n = ConvertUtf8.ToUtf8(s, pp, 4000);
		//		}
		//	});
		//	var a3 = new Action(() => { });
		//	var a4 = new Action(() => { });
		//	Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//	PrintList(r.Length, n);
		//}

		//s = "abc";
		s = s.Remove(50);
		//Print(Encoding.UTF8.GetBytes(s).Length);

		//Print(Convert_.Utf8Length(s));
		//Print(Convert_.Utf8FromString(s));

		//using(var b = new Catkeys.Util.MemoryBufferOnStackOrHeap()) {
		//	var p = b.Allocate(200);
		//	Print((LPARAM)p);
		//	//for(int i = 0; i < Catkeys.Util.MemoryBufferOnStackOrHeap.StackSize; i++) Print(p[i]);
		//}

		//for(int j=0; j<2; j++) {
		//	var m = stackalloc byte[100];
		//	Print((LPARAM)m);
		//	for(int i = 0; i < 100; i++) {
		//		Print(m[i]);
		//		m[i] = 1;
		//	}
		//}

		//var p = stackalloc byte[10000];

		//Perf.SpinCPU(100);
		//var a1 = new Action(() =>
		//{
		//	Convert_.Utf8FromString(s, p, 10000);
		//});
		//var a2 = new Action(() =>
		//{
		//	int n = Convert_.Utf8LengthFromString(s) + 1;
		//	Convert_.Utf8FromString(s, p, n);
		//});
		////var a3 = new Action(() => { Convert_.Utf8FromString_stackalloc(s); });
		////var a4 = new Action(() => { Convert_.Utf8FromString_thread(s); });
		////var a5 = new Action(() => { Convert_.Utf8FromString_thread2(s); });
		////var a6 = new Action(() => { Convert_.Utf8FromString_thread4(s); });
		////var a7 = new Action(() => { Convert_.Utf8FromString_simple(s); });
		////Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7);
		//var a3 = new Action(() => { Convert_.Utf8FromString(s); });
		//var a4 = new Action(() => { Encoding.UTF8.GetBytes(s); });
		////var a5 = new Action(() => { Convert_.Utf8FromString2(s); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//var b = Convert_.Utf8FromString(s);
		//Print(b.Length);

		//Print(Convert_.Utf8ToStringLength(b));
		//var r = Convert_.Utf8ToString(b);
		//Print(r);

		//fixed (byte* p = b) {
		//	r = Convert_.Utf8ToString(p);
		//	PrintList(r, r.Length);
		//	r = Convert_.Utf8ToString(p, b.Length);
		//	PrintList(r, r.Length);
		//	r = Convert_.Utf8ToString(p, 10);
		//	PrintList(r, r.Length);
		//}

		//Print(Encoding.UTF8.GetString(b).Length);

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { var r = Convert_.Utf8ToString(b); });
		//var a2 = new Action(() => { var r = Encoding.UTF8.GetString(b); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static unsafe void SA2()
	{
		byte* p = stackalloc byte[1000];
		Print((LPARAM)p);
		for(int i = 0; i < 1000; i++) p[i] = 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static unsafe void SA1()
	{
		//SA2();
		//byte* p = stackalloc byte[1000];
		////Print((LPARAM)p);
		////Print2(p);
		//Print3("kk", p);
		////for(int i=0; i<1000; i++) Print(p[i]);

		//Convert_.Utf8FromString("string", p, 1000);

		//Convert_.Utf8FromString("stringgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg");
	}

	//public static unsafe void Print2(void* value) { Output.Write((LPARAM)value); }
	public static unsafe void Print2(byte* value) { Output.WriteList((LPARAM)value, value[500]); }
	public static unsafe void Print3(string s, byte* value) { Output.WriteList(s, (LPARAM)value, value[500]); }

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void TestStackallocSpeed1()
	//{
	//	//var m = new Catkeys.Util.MemoryBufferOnStackOrHeap();
	//	//var p =m.Allocate(10);
	//	//Convert_.Utf8FromString("", p, 10);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void TestStackallocSpeed2()
	//{
	//	var p = stackalloc byte[1000];
	//	Convert_.Utf8FromString("", p, 10);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void TestStackallocSpeed3()
	//{
	//	var p = (byte*)_mmm;
	//	if(p == null) { p = (byte*)Catkeys.Util.NativeHeap.Alloc(1000); _mmm = p; }
	//	Convert_.Utf8FromString("", p, 10);
	//}

	//[ThreadStatic] static LPARAM _mmm;

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void TestStackallocSpeed4()
	//{
	//	var p = (byte*)Catkeys.Util.NativeHeap.Alloc(1000);
	//	Convert_.Utf8FromString("", p, 10);
	//	Catkeys.Util.NativeHeap.Free(p);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void TestStackallocSpeed()
	//{
	//	//var m = new Catkeys.Util.MemoryBufferOnStackOrHeap();
	//	//var p =m.Allocate(1000);
	//	//Convert_.Utf8FromString("string", p, 1000);
	//	//SA1();

	//	Perf.SpinCPU(100);
	//	var a1 = new Action(() => { TestStackallocSpeed1(); });
	//	var a2 = new Action(() => { TestStackallocSpeed2(); });
	//	var a3 = new Action(() => { TestStackallocSpeed3(); });
	//	var a4 = new Action(() => { TestStackallocSpeed4(); });
	//	Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	//}

	static void TestReadTextFile()
	{
		var path = Folders.Temp + "test.txt";
		//File.WriteAllText(path, "12345", Encoding.UTF8);
		//Print(new FileInfo(path).Length);
		//var s = File.ReadAllBytes(path);
		//Print(s);

		//var f=File.OpenRead(path);
		//f.
	}

	static void TestSciControl()
	{
		var f = new Form();

		G.Controls.SciControl.SciLexerDllPath = Ver.Is64BitProcess ? @"Q:\app\catkeys\Editor\SciLexer64.dll" : @"Q:\app\catkeys\Editor\SciLexer32.dll";
		var x = new G.Controls.SciControl();
		x.Width = 200;
		x.Height = 100;
		f.Controls.Add(x);

		var c1 = new Button() { Text = "OK", Top = 100, DialogResult = DialogResult.OK };
		var c2 = new Button() { Text = "Cancel", Top = 100, Left = 100 };
		f.AcceptButton = c1;
		f.CancelButton = c2;
		f.Controls.Add(c1);
		f.Controls.Add(c2);

		f.ShowDialog();
	}

	public static unsafe int CharPtrFindChar(byte* s, int len, byte ch)
	{
		if(s != null)
			for(int i = 0; i < len; i++) if(s[i] == ch) return i;
		return -1;
	}

	public static unsafe int CharPtrFindString(byte* s, int len, byte* s2, int len2)
	{
		if(s != null && len2 <= len && len2 > 0) {
			var ch = s2[0];
			for(int i = 0, n = len - len2; i <= n; i++) if(s[i] == ch) {
					for(int j = 1; j < len2; j++) if(s[i + j] != s2[j]) goto g1;
					return i;
					g1:;
				}
		}
		return -1;
	}

	public static unsafe int CharPtrFindString(byte* s, int len, string s2)
	{
		int len2 = s2.Length;
		if(s != null && len2 <= len && len2 > 0) {
			var ch = s2[0];
			for(int i = 0, n = len - len2; i <= n; i++) if(s[i] == ch) {
					for(int j = 1; j < len2; j++) if(s[i + j] != s2[j]) goto g1;
					return i;
					g1:;
				}
		}
		return -1;
	}

	[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static unsafe extern byte* strstr(byte* s1, byte* s2);

	static unsafe void TestFindUtf8()
	{
		string s = "abcd", s2 = "cd";
		char ch = 'd';
		s = "jshdhskafhkldshfkshflsjdgl;sdjgkljsdghjshglshgjkshfgydsgfhjgfsgfdgfkshgahg;dlgh;slgjhsgjslghdghids <ijshdhskafhkldshfkshflsjdgl;sdjgkljsdghjshglshgjkshfgydsgfhjgfsgfdgfkshgahg;dlgh;slgjhsgjslghdghids <ijshdhskafhkldshfkshflsjdgl;sdjgkljsdghjshglshgjkshfgydsgfhjgfsgfdgfkshgahg;dlgh;slgjhsgjslghdghids <ijshdhskafhkldshfkshflsjdgl;sdjgkljsdghjshglshgjkshfgydsgfhjgfsgfdgfkshgahg;dlgh;slgjhsgjslghdghids <ijshdhskafhkldshfkshflsjdgl;sdjgkljsdghjshglshgjkshfgydsgfhjgfsgfdgfkshgahg;dlgh;slgjhsgjslghdghids <image \"mmm";
		s2 = "<image \"";
		ch = '<';

		var t = Convert_.Utf8FromString(s);
		var t2 = Convert_.Utf8FromString(s2);
		fixed (byte* p = t)
		fixed (byte* p2 = t2) {
			Print(CharPtrFindChar(p, s.Length, (byte)ch));
			Print(CharPtrFindString(p, s.Length, p2, s2.Length));
			Print(CharPtrFindString(p, s.Length, s2));
			Print(strstr(p, p2) - p);

			Perf.SpinCPU(100);
			for(int i = 0; i < 5; i++) {
				Perf.First();
				for(int j = 0; j < 1000; j++) CharPtrFindChar(p, s.Length, (byte)ch);
				Perf.Next();
				for(int j = 0; j < 1000; j++) CharPtrFindString(p, s.Length, p2, s2.Length);
				Perf.Next();
				for(int j = 0; j < 1000; j++) CharPtrFindString(p, s.Length, s2);
				Perf.Next();
				for(int j = 0; j < 1000; j++) strstr(p, p2);
				Perf.NW();
			}

		}
	}

	static unsafe void TestAsciiStartsWithI()
	{
		fixed (byte* p = Convert_.Utf8FromString("Test")) {
			Print(Catkeys.Util.LibCharPtr.AsciiStartsWith(p, "Tes"));
			Print(Catkeys.Util.LibCharPtr.AsciiStartsWith(p, "tes"));
			Print(Catkeys.Util.LibCharPtr.AsciiStartsWith(p, "Mes"));
			Print(Catkeys.Util.LibCharPtr.AsciiStartsWithI(p, "Tes"));
			Print(Catkeys.Util.LibCharPtr.AsciiStartsWithI(p, "tes"));
			Print(Catkeys.Util.LibCharPtr.AsciiStartsWithI(p, "Mes"));
		}
	}



	static unsafe void TestBase64()
	{
		//var b = new byte[] { 0, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, };
		var b = new byte[] { 0, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, };

		for(int i = 0; i < 0; i++) {
			var bb = b;
			b = new byte[bb.Length * 2];
			bb.CopyTo(b, bb.Length);
		}

		var s = Convert.ToBase64String(b);
		if(s.Length < 2000) Print(s);

		Print(Convert.FromBase64String(s).Length);
		Print("------");

		var b2 = new byte[b.Length + 4];
		fixed (char* p = s) {
			fixed (byte* pp = b2) {
				int n = b2.Length, n2 = n;
				//Convert_.Base64Decode(p, s.Length, pp, n); Print(b2);
				//return;
				//Print(Convert_.Base64Decode(s));
				//Print(Convert_.Base64Decode2(s));
				//Print(Convert_.Base64Decode3(s));

				Perf.SpinCPU(100);
				for(int i = 0; i < 5; i++) {
					int N = 100;
					Perf.First();
					for(int j = 0; j < N; j++) Convert.FromBase64String(s);
					Perf.Next();
					for(int j = 0; j < N; j++) Convert_.Base64Decode(p, s.Length, pp, n);
					Perf.Next();
					for(int j = 0; j < N; j++) Convert_.Base64Decode(s);
					Perf.NW();
				}

			}
		}
	}

	static unsafe void TestHexEncode()
	{
		var b = new byte[] { 0, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, };
		//var b = new byte[] { 0, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 251, 252, 253, 254, 255, };

		for(int i = 0; i < 0; i++) {
			var bb = b;
			b = new byte[bb.Length * 2];
			bb.CopyTo(b, bb.Length);
		}

		var s = Convert_.HexEncode(b);
		if(s.Length < 2000) Print(s);
		//s = "123";

		Print(Convert_.HexDecode(s).Length);
		Print("------");

		var b2 = new byte[b.Length + 4];
		fixed (char* p = s) {
			fixed (byte* pp = b2) {
				int n = b2.Length, n2 = n;
				Convert_.HexDecode(s, pp, n); Print(b2); return;
				//Convert_.HexDecode2(s, pp, n); Print(b2); return;

				Perf.SpinCPU(100);
				for(int i = 0; i < 5; i++) {
					int N = 100;
					Perf.First();
					for(int j = 0; j < N; j++) Convert_.HexDecode(s, pp, n);
					Perf.Next();
					for(int j = 0; j < N; j++) Convert_.HexEncode(b);
					Perf.NW();
				}

			}
		}
	}

	static unsafe void TestFnv64()
	{
		var s = "qjsjhjkhsfjkhajfkhsjkfklsjfsalkfjdslgkasglkdhsglshgldkjhgklsghjdlkhgkj";
		Print(Convert_.HashFnv1(s));
		Print(Convert_.HashFnv1_64(s));

		fixed (char* p = s) {

			Perf.SpinCPU(100);
			for(int i1 = 0; i1 < 5; i1++) {
				int n2 = 1000;
				Perf.First();
				for(int i2 = 0; i2 < n2; i2++) { Convert_.HashFnv1(s); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { Convert_.HashFnv1(p, s.Length); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { Convert_.HashFnv1_64(s); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { Convert_.HashFnv1_64(p, s.Length); }
				Perf.NW();
			}

			byte* b = (byte*)p;
			Perf.SpinCPU(100);
			for(int i1 = 0; i1 < 5; i1++) {
				int n2 = 1000;
				Perf.First();
				for(int i2 = 0; i2 < n2; i2++) { Convert_.HashFnv1(b, s.Length * 2); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { Convert_.HashFnv1_64(b, s.Length * 2); }
				Perf.NW();
			}
		}
	}

	static void TestConsoleSpeed()
	{
		//Perf.SpinCPU(100);
		Perf.First();
		for(int i = 0; i < 9; i++) {
			Console.Write("1\n");
			Perf.Next(); //~110, regardless of SpinCPU. Sometimes can be ~60, rarely 15-20.
		}
		Perf.Write();
	}

	static unsafe class _Api
	{
		internal struct SYSTEMTIME
		{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMilliseconds;
		}

		[DllImport("kernel32.dll")]
		internal static extern void GetLocalTime(out SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		internal static extern int GetDateFormatEx(string lpLocaleName, uint dwFlags, ref SYSTEMTIME lpDate, string lpFormat, char* lpDateStr, int cchDate, string lpCalendar);

		[DllImport("kernel32.dll")]
		internal static extern int GetTimeFormatEx(string lpLocaleName, uint dwFlags, ref SYSTEMTIME lpTime, string lpFormat, char* lpTimeStr, int cchTime);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int wsprintfW(char* lpOut1024, string lpFmt, int a, int b, int c, int d, int e, int f);
	}

	static unsafe int _FormatDateTime(ref _Api.SYSTEMTIME t, char* b)
	{
		int n1 = _Api.GetDateFormatEx("", 0, ref t, "yyyy-MM-dd", b, 1024, null); //LOCALE_NAME_INVARIANT
		if(n1 == 0) return 0;
		b[n1 - 1] = ' ';
		int n2 = _Api.GetTimeFormatEx("", 0, ref t, "HH:mm:ss", b + n1, 1024 - n1); //LOCALE_NAME_INVARIANT
		if(n2 == 0) return 0;
		return n1 + n2 - 1;
	}

	static unsafe void TestDateFormat()
	{
		var d = DateTime.Now;
		Print(string.Format("{0:u}", d));

		_Api.SYSTEMTIME t;
		_Api.GetLocalTime(out t);
		var b = stackalloc char[1024];
		int n = _FormatDateTime(ref t, b);
		PrintList(n, new string(b, 0, n));

		b[0] = 'N';
		Api.wsprintfW(b, "%04i-%02i-%02i %02i:%02i:%02i", __arglist(t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond));
		Print(new string(b));

		b[0] = 'N';
		_Api.wsprintfW(b, "%04i-%02i-%02i %02i:%02i:%02i", t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond); //same speed
		Print(new string(b));

		var s = $"{t.wYear:D4}-{t.wMonth:D2}-{t.wDay:D2} {t.wHour:D2}:{t.wMinute:D2}:{t.wSecond:D2}";
		Print(s);

		//Perf.SpinCPU(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { d = DateTime.Now; }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { string.Format("{0:u}", d); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { _Api.GetLocalTime(out t); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { _FormatDateTime(ref t, b); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { Api.wsprintfW(b, "%04i-%02i-%02i %02i:%02i:%02i", __arglist(t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond)); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { _Api.wsprintfW(b, "%04i-%02i-%02i %02i:%02i:%02i", t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s = $"{t.wYear:D4}-{t.wMonth:D2}-{t.wDay:D2} {t.wHour:D2}:{t.wMinute:D2}:{t.wSecond:D2}"; }
			Perf.NW();
		}
	}

	static void TestTuple()
	{
		Print(TupleFunc());

		//Print(TestTupleMemberNames());
		//(int one, string two) = TestTupleMemberNames();
		//PrintList(one, two);
		//(var one, var two) = TestTupleMemberNames2();
		//PrintList(one, two);
		//(var one, _) = TestTupleMemberNames2();
		//PrintList(one);
	}

	static (int, int) TupleFunc()
	{
		return (1, 2);
	}

	static (int i, string s) TestTupleMemberNames()
	{
		return (5, "hh");
	}

	static Pint TestTupleMemberNames2()
	{
		return new Pint(5, 8);
	}

	class Pint
	{
		public int X { get; }
		public int Y { get; }

		public Pint(int x, int y) { X = x; Y = y; }
		public void Deconstruct(out int x, out int y) { x = X; y = Y; }
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static unsafe void TestVariadicDllImport()
	{
		//int k = 5;
		//var r = __makeref(k);
		//var v = __refvalue(r, int);
		//Print(v);

		var p = stackalloc char[1024];

#if true
#elif true
		//var t = (1, 2); //cannot get address, managed type
		var t = new Point(1, 2);
		int n = Api.wvsprintfW(p, "%i %i", &t);
		Print(n); //OK
		var s = new string(p, 0, n);
		Print(s);
#else
		int n = Api.wsprintfW(p, "%i %s", __arglist(3, "fff"));
		PrintList(n, Native.GetErrorMessage()); //0, no error

		//fixed(char* k = "fff") {
		//	n = Api.wsprintfW(p, "%i %s", __arglist(3, k));
		//	Print(n); //the same
		//}

		//int n=Api.wsprintfW(p, "%i %s", 3, "fff"); //everything OK

		var s = new string(p);
		Print(s); //OK
		Print(s.Length); //OK
#endif
	}

	static void TestOutputDestinations()
	{
		var s = @"Q:\Test\CatLog.txt";
		//Files.Delete(s);
		Output.LogFile = s;
		Output.LogFileTimestamp = true;
		//Output.LogFile = @"\\.\mailslot\111\QM_test_mailslot";
		Perf.First();
		for(int i = 0; i < 2; i++) {
			//Print("test");
			Print("KKKKKKKKK");
			//TaskDialog.Show();
			Perf.Next();
		}
		Output.LogFile = null;

		//Output.LogFile = @"\\.\mailslot\111\QM_test_mailslot";
		//Print("NNNNNNNNNNNNNNNNN");
		//Output.LogFile = null;


		Perf.Next();
		//Perf.Write();

		Shell.Run(s);

		//Console.ReadKey();
	}

	#region Output.Server example

	//Complete simple program with output window.
	class OutputFormExample :Form
	{
		TextBox _tb;

		public OutputFormExample()
		{
			_tb = new TextBox();
			_tb.ReadOnly = true;
			_tb.Multiline = true;
			_tb.ScrollBars = ScrollBars.Both;
			_tb.WordWrap = false;
			_tb.Dock = DockStyle.Fill;
			_tb.TabStop = false;
			this.Controls.Add(_tb);

			_os.SetNotifications(_ProcessMessages, this);
		}

		void _ProcessMessages()
		{
			while(_os.Messages.TryDequeue(out var m)) {
				switch(m.Type) {
				case Output.Server.MessageType.Clear:
					_tb.Clear();
					break;
				case Output.Server.MessageType.Write:
					//_tb.AppendText(m.Text);
					_tb.AppendText($"{DateTime.FromFileTimeUtc(m.TimeUtc).ToLocalTime()}  {m.Domain}  {m.Text}");
					break;
				}
			}
		}

		static Output.Server _os = new Output.Server(false);

		[STAThread]
		public static void Main()
		{
			_os.Start();

			//test Print and Clear, before and after creating window
			Output.IgnoreConsole = true;
			Print("test before setting notifications");
			Task.Run(() => { Wait(1); Print("test after"); Wait(1); Output.Clear(); Wait(1); Print("test after Clear"); });

			var f = new OutputFormExample();
			f.ShowDialog();
			_os.Stop();
		}
	}

	#endregion
	//#region Output.Server example

	//class OutputFormExample :Form
	//{
	//	TextBox _tb;

	//	public OutputFormExample()
	//	{
	//		_tb = new TextBox();
	//		_tb.Multiline = true;
	//		_tb.ScrollBars = ScrollBars.Vertical;
	//		_tb.Dock = DockStyle.Fill;
	//		this.Controls.Add(_tb);

	//		_ProcessMessages();
	//		_os.SetNotifications(_ProcessMessages, this);
	//	}

	//	void _ProcessMessages()
	//	{
	//		while(_os.Messages.TryDequeue(out var b)) {
	//			if(b.Length == 0) {
	//				_tb.Clear();
	//			} else {
	//				var s = Encoding.UTF8.GetString(b);
	//				_tb.AppendText(s);
	//			}
	//		}
	//	}
	//}

	//static Output.Server _os = new Output.Server(true);

	//static void OutputServerExample()
	//{
	//	_os.Start();
	//	Output.IgnoreConsole = true;

	//	Print("test before setting notifications");
	//	Task.Run(() => { Wait(1); Print("test after"); Wait(1); Output.Clear(); Wait(1); Print("test after Clear"); });

	//	var f = new OutputFormExample();
	//	f.ShowDialog();
	//	_os.Stop();
	//}

	//#endregion

	static Output.Server _os = new Output.Server(true);

#if false
	class TestOutputForm :Form
	{
		RichTextBox _tb; //speed: similar as TextBox. Slower when need to scroll, else faster.

		public TestOutputForm()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Width = 1800;
			this.Height = 120;
			this.Top = 700;
			_tb = new RichTextBox();
			_tb.ReadOnly = true;
			//_tb.Multiline = true;
			//_tb.ScrollBars = ScrollBars.Vertical;
			_tb.WordWrap = false;
			_tb.DetectUrls = false;
			_tb.Dock = DockStyle.Fill;
			_tb.TabStop = false;
			this.Controls.Add(_tb);

			//PrintList(_tb.DetectUrls, _tb.EnableAutoDragDrop, _tb.MaxLength, _tb.ScrollBars, _tb.WordWrap);

			_os.SetNotifications(_ProcessMessages, this);
		}

		void _ProcessMessages()
		{
			while(_os.Messages.TryDequeue(out var m)) {
				switch(m.Type) {
				case Output.Server.MessageType.Clear:
					_tb.Clear();
					break;
				case Output.Server.MessageType.Write:
					//_tb.AppendText(m.Text);
					//_tb.AppendText($"{DateTime.FromFileTimeUtc(m.Time).ToLocalTime()}  {m.Caller}  {m.Text}");

					Perf.First();
					_tb.AppendText(m.Text);
					Perf.Next();
					//_tb.ScrollToCaret(); //slow
					((Wnd)_tb).Send(Api.WM_VSCROLL, 7); //SB_BOTTOM
					Perf.Next();
					_tb.Update();
					//UpdateWindow((Wnd)_tb);
					Perf.Next();
					Console.WriteLine(Perf.Times);
					break;
				}
			}
		}
	}
#else
	class TestOutputForm :Form
	{
		TextBox _tb;

		public TestOutputForm()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Width = 1800;
			//this.Height = 140;
			this.Top = 700;
			_tb = new TextBox();
			_tb.ReadOnly = true;
			_tb.Multiline = true;
			_tb.ScrollBars = ScrollBars.Both;
			_tb.WordWrap = false;
			_tb.Dock = DockStyle.Fill;
			_tb.TabStop = false;
			this.Controls.Add(_tb);

			_os.SetNotifications(_ProcessMessages, this);
		}

		void _ProcessMessages()
		{
			while(_os.Messages.TryDequeue(out var m)) {
				switch(m.Type) {
				case Output.Server.MessageType.Clear:
					_tb.Clear();
					break;
				case Output.Server.MessageType.Write:
					//_tb.AppendText(m.Text);
					_tb.AppendText($"{DateTime.FromFileTimeUtc(m.TimeUtc).ToLocalTime()}  {m.Domain}  {(object)m.Domain == _prevCaller}  {m.Text}");
					_prevCaller = m.Domain;

					//Perf.First();
					//_tb.AppendText(m.Text);
					//Perf.Next();
					//_tb.Update();
					////UpdateWindow((Wnd)_tb);
					//Perf.Next();
					//Console.WriteLine(Perf.Times);
					break;
				}
			}
		}

		string _prevCaller;
	}
#endif

	//[DllImport("user32.dll")]
	//internal static extern bool UpdateWindow(Wnd hWnd);

	static void TestOutputServer()
	{
		_os.Start();
		//_os.NeedCallerMethod = true;
		Output.IgnoreConsole = true;
		//Output.TimeStamp = true;

		//_TesOutputServerSpeed();
		Task.Run(() => { Wait(0.5); _TesOutputServerSpeed(); });
		//Print("<>test before setting notifications");
		//Task.Run(() => { Wait(1); Print("test after"); Wait(1); Output.Clear(); Wait(1); Print("test after Clear"); });

		//Task.Run(() =>
		//{
		//	Wait(0.2);
		//	var t0 = Time.Milliseconds;
		//	while(Time.Milliseconds-t0<1000) {
		//		Sleep(1);
		//		Print(Time.Microseconds);
		//	}
		//});

		//Wait(0.5);
		var f = new TestOutputForm();
		f.ShowDialog();
		//f.Dispose();
		//Wait(0.5);
		_os.Stop();
	}

	static void _TesOutputServerSpeed()
	{
		//Perf.SpinCPU(100);

		//Perf.First();
		//for(int j = 0; j < 8; j++) { Console.WriteLine("test speed"); Perf.Next(); }
		//Console.WriteLine(Perf.Times);

		Perf.First();
		for(int j = 0; j < 8; j++) { Print("test speed"); Perf.Next(); }
		Console.WriteLine(Perf.Times);

		//_Stack2(0);

		//Output.WriteDirectly("directly, long long long long long long long long long long long long");

	}

	//static void _Stack2(int i)
	//{
	//	if(i < 200) { _Stack2(i + 1); return; }

	//	Perf.First();
	//	for(int j = 0; j < 8; j++) { Print("test speed"); Perf.Next(); }
	//	Console.WriteLine(Perf.Times);
	//}

	static void TesOutputServerExternal()
	{
		Output.IgnoreConsole = true;

		//Perf.SpinCPU(100);
		//Perf.First();
		//for(int j = 0; j < 8; j++) { Print("test speed"); Perf.Next(); }
		//Console.WriteLine(Perf.Times);

		for(int i = 0; i < 4; i++) {
			Print(i);
			TaskDialog.Show();
		}

		//_Stack(0);
	}

	//static void _Stack(int i)
	//{
	//	if(i < 20) { _Stack(i + 1); return; }

	//	for(int j = 0; j < 1000; j++) { Print("test speed"); Sleep(100); }
	//}

	static void TestWTimer()
	{
		using(var t = Catkeys.Util.WaitableTimer.Create()) {
			//int v = 0;

			Task.Run(() =>
			{
				try { bool k = t.WaitOne(3000); Perf.Next(); Console.WriteLine(Perf.Times); Console.WriteLine(k); }
				catch(Exception ex) { Console.WriteLine(ex.Message); }
			});
			//Task.Run(() => { Console.WriteLine(Api.WaitForSingleObject(t.SafeWaitHandle.DangerousGetHandle(), 3000)); });

			Wait(0.2);
			Perf.First();
			//t.SetRelative(-30000);
			t.SetAbsolute(DateTime.UtcNow.AddSeconds(3));

			//v = 1;

			MessageBox.Show("");
		}
	}

	static void _TestGetStringFromMethod2(Action ac)
	{
		//Print(ac.Method.Name);
		var b = ac.Method.GetMethodBody();
		//Print(b.LocalVariables.Count);
		//foreach(var v in b.LocalVariables) {
		//	Print(v.ToString());
		//}
		var a = b.GetILAsByteArray();
		//Print(a);
		for(int i = 0; i < a.Length; i++) PrintHex(a[i]);

		//OpCodes.Add
		ac();
	}

	static IEnumerable<string> _FindLiterals(MethodInfo method)
	{
		ILReader reader = new ILReader(method);
		foreach(ILInstruction instruction in reader.Instructions) {
			if(instruction.Op == OpCodes.Ldstr) {
				yield return instruction.Data as string;
			}
		}
	}

	static void _TestGetStringFromMethod(Action ac)
	{
		foreach(var s in _FindLiterals(ac.Method)) {
			//Print(s);
		}
	}

	static void TestGetStringFromMethod()
	{
		//Output.IgnoreConsole = true;
		//_TestGetStringFromMethod(() => { var s = "gggg"; Shell.Run("notepad.exe"); });
		//_TestGetStringFromMethod(() =>
		//{
		//	Console.WriteLine("notepad.exe");
		//});

		Perf.First();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad2.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad3.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad4.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad5.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad6.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad7.exe"));
		Perf.Next();
		_TestGetStringFromMethod(() => Console.WriteLine("notepad8.exe"));
		Perf.Next();
		Perf.Write(); //first time ~10 ms, then ~5 mcs
	}

	static void TestCatMenuRun()
	{
		var m = new CatMenu();
		m["nnn", Folders.System + "notepad.exe"] = o => Shell.Run(Folders.System + "notepad.exe");
		m.Run(Folders.System + "notepad.exe", "nnn");
		m.Run2(Folders.System + "notepad.exe", "nnn");
		//m["nnn", Folders.System + "notepad.exe"] = o => Print(o.MenuItem);

		m.Show();
	}

	public static void Run(this CatMenu m, string path, string label = null)
	{
		m[label ?? path, path] = o => Shell.TryRun(path);
	}

	public static void Run2(this CatMenu m, string path, string label = null)
	{
		//in this version, the delegate does not capture variables, therefore will not be created new delegate for each menu item

		var t = m.Add(label ?? path, o => Shell.TryRun(o.Item.Tag as string), path);
		t.Tag = path;
	}

	static void TestFloatConvert()
	{
		string s;
		//decimal k = 0.55M;
		//s = 1234567.55.ToString();
		s = 1234567.55.ToString_();

		//Output.DebugWriteToQM2(s);

		Perf.SpinCPU(100);
		var a1 = new Action(() => { s = 0.55F.ToString(); });
		var a2 = new Action(() => { s = 0.55F.ToString_(); });
		var a3 = new Action(() => { });
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestOutputAllCases()
	{
		//string s = Folders.Temp + "test.log";
		//Output.LogFile = s;

		//Output.IgnoreConsole = true;

		Output.Writer = new TestOutputWriter();

		Print(1);
		Wait(1);
		Output.Clear();
		Print(2);

		//Shell.Run(s);

		//Output.RedirectConsoleOutput = true;
		//Console.WriteLine("yyyy");

		//Output.RedirectDebugOutput = true;
		////Output.IgnoreConsole = true;
		//Debug.Print("print");
		//Trace.WriteLine("trace");
	}

	class TestOutputWriter :TextWriter
	{
		public override void WriteLine(string value) { Output.WriteDirectly("redir: " + value); }
		public override void Write(string value) { Output.WriteDirectly("redir: " + value); }
		public override Encoding Encoding { get => Encoding.Unicode; }
	}

	static void TestLibProcessHandle()
	{
		var w = Wnd.Find("*Firefox*");
		//var w = Wnd.Find("Calculator"); //w=w.Child(className:"*Core*");
		Print(w);
		Print(w.ProcessName);
		Print(w.ProcessPath);

		//var m = new Process_.Memory(w, 1000);
		////var m = new Process_.Memory(w.ProcessId, 1000);
		////m.WriteUnicodeString("test");
		////Print(m.ReadUnicodeString(4));
		//m.WriteAnsiString("test");
		//Print(m.ReadAnsiString(4));

		var u = Process_.UacInfo.GetOfProcess(w.ProcessId);
		PrintList(u.IntegrityLevel);
	}

	static void TestRect()
	{
		RECT r1, r2;
		r1 = new RECT(1, 2, 3, 4, true);
		r2 = new RECT(2, 3, 4, 5, true);
		//Print(RECT.Union(r1, r2));
		//Print(r2.Union(r1)); Print(r2);
		Print(RECT.Intersect(r1, r2));
		Print(r2.Intersect(r1)); Print(r2);
	}

	//static Time.LibSleepPrecision s_tt = new Time.LibSleepPrecision(1);

	public static void _Sleep2(int ms)
	{
		using(new Time.LibSleepPrecision(1)) {
			//Perf.Next();
			Time.Sleep(ms);
		}
	}

	[DllImport("kernel32.dll", EntryPoint = "Sleep")]
	internal static extern void SleepApi(uint dwMilliseconds);

	static void TestSystemWaitPeriod()
	{
		if(Time.LibSleepPrecision.Current < 15) throw new CatException("small system time resolution: " + Time.LibSleepPrecision.Current);

		//Time.LibSleepPrecision.TempSet1(60000);
		for(int j = 0; j < 3; j++) {
#if true
			Print(Time.LibSleepPrecision.Current);
			Perf.First();
			for(int i = 0; i < 8; i++) {
				Time.Sleep(1);
				//Time.SleepDoEvents(1);
				//SleepApi(1);
				Perf.Next();
			}
			Perf.Write();
			Print(Time.LibSleepPrecision.Current);
			Thread.Sleep(1000);
			Print(Time.LibSleepPrecision.Current);
			Thread.Sleep(200);
			Print(Time.LibSleepPrecision.Current);
#else

					//using(new Time.LibSleepPrecision(1)) {
					for(var t0 = Time.Milliseconds; Time.Milliseconds < t0 + 5000;) {
						//using(new Time.LibSleepPrecision(1)) {
						//Perf.Next();
						Time.Sleep(1);
						//}
					}
					//}
					Print("end");
					Wait(2);
#endif
		}
		//Time.Sleep(1);
		//Thread.Sleep(500);
		Print("---");
	}

	#region thread count

	internal struct THREADENTRY32
	{
		public int dwSize;
		public uint cntUsage;
		public uint th32ThreadID;
		public uint th32OwnerProcessID;
		public int tpBasePri;
		public int tpDeltaPri;
		public uint dwFlags;
	}
	[DllImport("kernel32.dll")]
	internal static extern bool Thread32First(IntPtr hSnapshot, ref THREADENTRY32 lpte);
	[DllImport("kernel32.dll")]
	internal static extern bool Thread32Next(IntPtr hSnapshot, ref THREADENTRY32 lpte);
	[DllImport("kernel32.dll")]
	internal static extern uint GetCurrentProcessId();
	[DllImport("kernel32.dll")]
	internal static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);
	internal const uint TH32CS_SNAPTHREAD = 0x4;

	static int GetThreadCount()
	{
		var pid = GetCurrentProcessId();
		int n = 0;
		var h = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
		var t = new THREADENTRY32(); t.dwSize = Marshal.SizeOf(typeof(THREADENTRY32));
		for(bool ok = Thread32First(h, ref t); ok; ok = Thread32Next(h, ref t)) {
			if(t.th32OwnerProcessID == pid) n++;
		}
		return n;
	}

	#endregion

	static void TestThreadPoolPerf()
	{
		//Print("ff");
		//Thread.Sleep(30000);

		Print(GetThreadCount()); //9
								 //for(int i = 0; i < 10; i++) ThreadPool.QueueUserWorkItem(sta => { Wait(1); }); //12 if 1, but 14 if more
		for(int i = 0; i < 2; i++) Task.Run(() => { Wait(10); }); //12 if 1, but 14 if more

		//13 if 1 QueueUserWorkItem + 1 Task.Run.
		//14 if (QueueUserWorkItem+Task.Run)>2.

		Thread.Sleep(500);
		Print(GetThreadCount());


		//		for(int j = 0; j < 3; j++) {
		//			Perf.First();
		//#if false
		//		Task.Run(() => { Wait(1); Perf.Write(); }); //12 threads, speed 6000/10 (4000 non-ngened)
		//#elif false
		//		Task.Delay(1000).ContinueWith(_ => { Perf.Write(); }); //14 threads, speed 8000/25
		//#elif false
		//			//ThreadPool.SetMinThreads(1, 1); //does nothing
		//			//ThreadPool.GetMinThreads(out var v1, out var v2);
		//			//ThreadPool.GetMaxThreads(out var v3, out var v4);
		//			//PrintList(v1, v2, v3, v4);
		//			ThreadPool.QueueUserWorkItem(sta => { Wait(1); Perf.Write(); }); //12 threads, speed 400/8

		//			//ThreadPool.UnsafeQueueUserWorkItem(sta => { Wait(1); Perf.Write(); }, null); //12 threads, speed 400/8
		//#elif false
		//			var tim=new System.Threading.Timer() //difficult
		//#else
		//			var t = new Thread(() => { Wait(1); Perf.Write(); }); //9 threads, speed 400
		//			t.IsBackground = true;
		//			t.Start();
		//#endif
		//			Perf.Next();
		//			Time.Sleep(1500);
		//			Print(GetThreadCount());
		//		}
	}

	static void TestTimer_()
	{
		//this example sets 3 timers
		Timer_.After(500, t => Print("simple one-time timer"));
		Timer_.Every(1000, t => Print("simple periodic timer"));
		var t3 = new Timer_(t => Print("with Timer_ object")); t3.Start(3000, true); //the same as Timer_.After
		MessageBox.Show("");

		//if(Time.LibSleepPrecision.Current < 15) throw new CatException("small system time resolution: " + Time.LibSleepPrecision.Current);
		//Time.LibSleepPrecision.TempSet1();
		//int i = 0;
		//Perf.First();
		//Timer_.Every(10, t => { Perf.Next(); if(i++ == 8) { Perf.Write(); t.Stop(); } });

		//MessageBox.Show("");
	}

	static void TestWndFromXY()
	{
		//Print(Wnd.FromXY(1070, 1385, WXYFlags.NeedWindow));
		//Print(Wnd.FromXY(1070, 1600, WXYFlags.NeedControl));
		//Print(Wnd.FromXY(1070, 1385, WXYFlags.Raw|WXYFlags.NeedControl));
		//Print(Wnd.FromXY(1070, 1600, WXYFlags.Raw|WXYFlags.NeedControl));
		//Print(Wnd.FromXY(1070, 1385, WXYFlags.Raw | WXYFlags.WorkArea));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.FromXY(1070, 1385, WXYFlags.NeedWindow); });
		//var a2 = new Action(() => { Wnd.FromXY(1070, 1385, WXYFlags.NeedWindow|WXYFlags.Raw); });
		//var a3 = new Action(() => { Wnd.FromXY(1070, 1385, 0);});
		//var a4 = new Action(() => { Wnd.FromXY(1070, 1385, 0|WXYFlags.Raw);});
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestOptionsExample()
	{
		if(Time.LibSleepPrecision.Current < 15) throw new CatException("small system time resolution: " + Time.LibSleepPrecision.Current);

		//using(new Time.LibSleepPrecision(6)) {
		Wait(0.5);
		//Options.MouseClickSleep = 1;
		//Options.MouseMoveSpeed = 20;
		Perf.First();
		Mouse.Click(668, 1246);
		//Mouse.DoubleClick(668, 1246);
		Perf.NW();
		Perf.First();
		Mouse.Click(668, 1346);
		//Mouse.DoubleClick(668, 1346);
		Perf.NW();
		Perf.First();
		Mouse.Click(668, 1246);
		//Mouse.DoubleClick(668, 1246);
		Perf.NW();
		//}
	}

	static void TestMouseMoveClick()
	{
		//using(BlockUserInput.All()) {
		//	Mouse.Click();
		//}

		//Mouse.Move(default(Wnd), 1, 2, Mouse.MoveFlags.NonClient);

		var w = Wnd.Find("*- Notepad");
		//var w = Wnd.Find("*- Mozilla Firefox");
		//var w = Wnd.Find("Hung");
		//w = w.ChildById(15);

		//Options.MouseClickSleep = 0;

		//Mouse.Move(w, 50, 20, Mouse.MoveFlags.ForClick);
		//Mouse.Click(w, 50, 10);

		//try {
		//	GetClipCursor(out var r); r.right = 1000; ClipCursor(ref r);
		//	Wait(5);
		//	Mouse.Move(w, 50, 20);
		//}
		//finally { ClipCursor(Zero); }

		//for(int i = 0; i < 10; i++) {
		//	Mouse.Move(w, 50, 20);
		//	Mouse.Move(w, 70, 20);
		//}

		//var f = new Form();
		//w = (Wnd)f;
		//var b = new Button();
		//f.Controls.Add(b);
		//f.StartPosition = FormStartPosition.CenterScreen;
		//bool printMouse = false;
		////f.MouseClick += (unu, sed) => { if(sed.Button == MouseButtons.Left) { printMouse = true; _TestMouseMove(f); } }; //bad: somehow the click generates another MouseClick event
		//b.Click += (unu, sed) => { printMouse = true; _TestMouseMove(f); }; //bad: no events, does not close the form
		////Timer_.After(1000, t => { printMouse = true; _TestMouseMove(f); }); //ok
		////b.Click += async (unu, sed) => { printMouse = true; await Task.Run(() => { Mouse.Click(w, 0.9f, -10); }); }; //ok
		//f.MouseDown += (unu, sed) => { if(printMouse) PrintList("down", Time.Milliseconds, Mouse.XY); };
		//f.MouseUp += (unu, sed) => { if(printMouse) PrintList("up", Time.Milliseconds, Mouse.XY); };
		//f.ShowDialog();
		//Wait(0.2);

		var f = new FormTestMouse();
		f.ShowDialog();
		Wait(0.2);
	}

	class FormTestMouse :Form
	{
		public FormTestMouse()
		{
			var b = new Button();
			Controls.Add(b);
			StartPosition = FormStartPosition.CenterScreen;

			//Timer_.After(1000, t => { _TestMouseMove(this); });
			b.Click += (unu, sed) => { _TestMouseMove(this); };
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			PrintList("down", Time.Milliseconds, Mouse.XY);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			PrintList("up", Time.Milliseconds, Mouse.XY);
			base.OnMouseDown(e);
		}

		protected override void WndProc(ref Message m)
		{
			var M = (uint)m.Msg; LPARAM W = m.WParam, L = m.LParam;
			//Print(m);

			base.WndProc(ref m);
		}
	}

	static void _TestMouseMove(Form f)
	{
		var w = (Wnd)f;
		PrintList("start", Time.Milliseconds, Mouse.XY);
		Perf.First();
		//Mouse.RightClick(w, 50, 20);
		Mouse.Click(w, Coord.Fraction(0.9), -10);
		//Mouse.Click(w, Coord.Fraction(0.99), 0.99f);
		//Mouse.RightClick(w, Coord.Fraction(0.4), -10);
		//Task.Run(() => Mouse.Click(w, Coord.Fraction(0.9), -10));

		//Task.Run(() => Mouse.Click(w, Coord.Fraction(0.9), -10));
		//Time.SleepDoEvents(200);

		//Options.MouseClickSleep = 20;

		//using(var e = new AutoResetEvent(false)) {
		//	var o1 = Options;
		//	Task.Run(() =>
		//	{
		//		using(ScriptOptions.Temp()) {
		//			ScriptOptions.Options = o1;
		//			Mouse.RightClick(w, 50, 50);
		//			//Mouse.Click(w, Coord.Fraction(0.9), -10);
		//			//Mouse.RightClick(w, Coord.Fraction(0.4), -10);
		//			//Mouse.LeftDown(w, Coord.Fraction(0.99), Coord.Fraction(0.99));
		//			//Mouse.LeftUp(w, Coord.Fraction(0.99), Coord.Fraction(1.2));
		//			e.Set();
		//		}
		//	});
		//	Perf.First();
		//	//e.WaitOne();
		//	while(!e.WaitOne(10)) {
		//		Time.DoEvents();
		//	}
		//	//Time.SleepDoEvents(300);
		//	Perf.NW();
		//}

		PrintList("end", Time.Milliseconds, Mouse.XY);
		//Perf.NW();
	}

	[DllImport("user32.dll")]
	internal static extern bool GetClipCursor(out RECT lpRect);
	[DllImport("user32.dll")]
	internal static extern bool ClipCursor([In] ref RECT lpRect);
	[DllImport("user32.dll")]
	internal static extern bool ClipCursor(IntPtr zero);

	static void TestDisableWarnings()
	{
		ScriptOptions.Default.DisableWarnings("tes*");
		using(Options.DisableWarnings("*.NET*")) {
			Output.Warning("Test.");
			Output.Warning("nondisabled.");
			Output.Warning("Failed to get .NET control names.");
		}
		Print("end");
		Output.Warning("nondisabled: Failed to get .NET control names.");
	}

	public struct ObjMember
	{
		object _o;

		public object Obj
		{
			//[MethodImpl(MethodImplOptions.NoInlining)]
			get => _o;
		}

		public static implicit operator ObjMember(int v) => new ObjMember() { _o = v };
		public static implicit operator ObjMember(float v) => new ObjMember() { _o = v };

		//this should be slightly faster and smaller caller's code, but actually compiler generates exactly the same code
		//ObjMember(int v)=>_o=v;
		//public static implicit operator ObjMember(int v) => new ObjMember(v);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord2(Types<int, float> x, Types<int, float> y)
	{
		//PrintList(x.type, y.type);
		if(x.type == 5) Print(2);
		if(y.type == 5) Print(2);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord3(Coord x, Coord y)
	{
		//PrintList(x.type, y.type);
		if(x.Type == Coord.CoordType.Fraction) Print(3);
		if(y.Type == Coord.CoordType.Fraction) Print(3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord4(object x, object y)
	{
		//if(x is int i) { //much slower, same code size
		//	if(i > 1000) Print(1000);
		//}
		//if(x is string i) { //but this is very fast (reference type, don't need unboxing)
		//	if(i.Length > 5) Print("1000");
		//}
		if(x is int) {
			int i = (int)x;
			if(i > 1000) Print(1000);
		}
		if(y is int) {
			int i = (int)y;
			if(i > 1000) Print(1000);
		}

		//switch(x) { //slow
		//case null: Print("null"); break;
		//case float f: Print("float"); break;
		//case long l: Print("long"); break;
		//}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord5(ObjMember x, ObjMember y)
	{
		object o = x.Obj;
		if(o is int) {
			int i = (int)o;
			if(i > 1000) Print(1000);
		}
		o = y.Obj;
		if(o is int) {
			int i = (int)o;
			if(i > 1000) Print(1000);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord6(Coord x, Coord y)
	{
		//PrintList(x.Type, y.Type, x.Value, y.Value, x.Value / 10000.0, y.Value / 10000.0);
		PrintList(x.Type, y.Type, x.Value, y.Value, x.FractionValue, y.FractionValue);
		if(x.Type == Coord.CoordType.Fraction) Print(3);
		if(y.Type == Coord.CoordType.Fraction) Print(3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord6_1(Coord x)
	{
		//PrintList(x.Type, y.Type);
		if(x.Type == Coord.CoordType.Fraction) Print(3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord6_4(Coord x, Coord y, Coord z, Coord q)
	{
		PrintList(x.Type, y.Type, z.Type, q.Type);
		if(x.Type == Coord.CoordType.Fraction) Print(3);
		if(y.Type == Coord.CoordType.Fraction) Print(3);
		if(z.Type != Coord.CoordType.Fraction) Print(3);
		if(q.Type == Coord.CoordType.Fraction) Print(3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestBoxing(ObjMember x)
	{
		Print(x);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestIntNullable(int? x, int? y)
	{
		if(x.HasValue) Print(x);
		if(y.HasValue) Print(y);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestCoord(int u1 = 0, int u2 = 0, int x = 5, int y = 6)
	{
		//Print(Wnd.FromXY(1138, 1219));
		//Print(Wnd.FromXY(Coord.Reverse(10), 100));
		//Print(Wnd.FromXY(Coord.Fraction(0.99), 100));
		//Print(Wnd.FromXY(null, 100));
		//Print(Wnd.FromXY(500, Coord.Fraction(1.1)));
		//Print(Wnd.FromXY(500, Coord.Reverse(10)));
		//Print(Wnd.FromXY(500, Coord.Reverse(-10)));
		//Wnd w = Wnd.Find("Quick *");
		//Print(w.ChildFromXY(100, 100));
		//Print(w.ChildFromXY(Coord.Reverse(10), 100));
		//Print(w.ChildFromXY(Coord.Fraction(0.99), 100));
		//Print(w.ChildFromXY(100, Coord.Reverse(10)));
		//Print(w.ContainsScreenXY(10, 10));
		//Print(w.ContainsScreenXY(100, 700));
		//Print(w.ContainsScreenXY(100, Coord.Reverse(30)));
		//Print(w.ContainsScreenXY(100, Coord.Reverse(30), true));
		//Wnd c = Wnd.Find("*- Notepad");
		//Print(c.ContainsWindowXY(w, 100, 100));
		//Wnd w = Wnd.Find("*- Notepad");
		//w.Move(500, 1200);
		//w.Move(null, Coord.Reverse(-100));
		//w.Move(Coord.Fraction(0.5), Coord.Reverse(-100));
		//w.Move(Coord.Fraction(0.5), Coord.Reverse(-100), true);
		//w.Resize(500, 200);
		//w.Resize(Coord.Fraction(0.5), Coord.Fraction(0.5));
		//w.Resize(Coord.Fraction(0.5), Coord.Fraction(0.5), true);
		//w.Resize(Coord.Reverse(0), Coord.Reverse(0));
		//w.Resize(Coord.Reverse(0), Coord.Reverse(0), true);
		//w.Resize(Coord.Reverse(-10), Coord.Reverse(-10));
		//w.Move(300, null, 600, null);
		//w.Move(Coord.Fraction(0.5), Coord.Reverse(0), Coord.Fraction(0.2), Coord.Reverse(500));
		//w.Activate();
		//Wait(1);
		//w.MoveLL(100, 300, 250, 150);
		//w.MoveLL(100, 300);
		//w.ResizeLL(250, 150);
		//Print(w.Rect);

		//Mouse.Move(300, 100);
		//Mouse.Move(w, 300, 100);
		//Mouse.Move(w, 300, -10);
		//Mouse.Move(w, 300, 0, Mouse.MoveFlags.NonClient);
		//Mouse.Move(Coord.Fraction(0.9), Coord.Reverse(10));
		//Mouse.Move(Coord.Fraction(0.9), Coord.Reverse(10), true);
		//Mouse.Move(w, Coord.Fraction(0.9), Coord.Reverse(10));
		//Mouse.Move(w, Coord.Fraction(0.9), Coord.Reverse(10), Mouse.MoveFlags.NonClient);
		//Mouse.Move(w, 300, 100, Mouse.MoveFlags.ForClick);
		//Mouse.Move(w, 700, 300);
		//Mouse.Move(w, 700, 300, Mouse.MoveFlags.ForClick);
		//Mouse.Move(null, 100);
		//Mouse.Move(w, null, 100);



		//Coord k = 5;
		//Print(k);
		//k = -5;
		//Print(k);
		//k = Coord.Reverse(5);
		//Print(k);
		//k = Coord.Reverse(-5);
		//Print(k);
		//k = Coord.Fraction(0.5);
		//Print(k);
		//k = Coord.Fraction(-5);
		//Print(k);
		//k = null;
		//Print(k);


		//TestBoxing(3);

		//TestCoord3(3, 0.5f);
		//TestCoord3(Coord.Reverse(5), Coord.Fraction(0.4));

		//ObjInher k = 5;

		//TestIntNullable(5, 6);
		//TestCoord6(5, 6);
		//TestCoord6(Coord.Fraction(0.5), Coord.Fraction(-0.1));
		//TestCoord6(x, y);
		//TestCoord6_1(Coord.Fraction(0.5));
		//TestCoord6_4(5, Coord.Reverse(1), Coord.Fraction(0.5), null);

		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 8; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { TestCoord2(5, 6); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestCoord3(5, 6); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestCoord4(5, 6); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestCoord5(5, 6); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestCoord6(5, 6); }
		//	//Perf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { TestCoord6_4(5, 6, 7, null); }
		//	Perf.NW();
		//}
	}

	static void TestScriptOptionsDebug()
	{
		//Options.Debug = false;
		Perf.First();
		var y = Options.Debug;
		Perf.NW();
		Print(y);
	}

	static void TestDebug_()
	{
		//Debug.Print("Debug.Print");
		//Debug.WriteLine("Debug.WriteLine");
		//Debug_.Print("Debug_.Print");
		//Debug_.PrintIf(true, "Debug_.PrintIf");
		//Debug_.PrintFunc();
		//Options.Debug = false;
		//Debug_.Dialog("Dialog");
		//Debug_.DialogOpt("DialogOpt");
		//Debug_.PrintOpt("PrintOpt");
		//Debug_.WarningOpt("WarningOpt");
	}

	static void TestScreen_()
	{
		//Print(Screen.AllScreens.Length);
		//foreach(var k in Screen.AllScreens) Print(k.Bounds);

		//Screen s = null;
		//s = Screen_.FromObject(1);
		////s = Screen_.FromObject((byte)1);
		//s = Screen_.FromObject(Screen.FromPoint(new Point(0, 2000)));
		////s = Screen_.FromObject(Wnd.Find("Quick *"));
		////var f = new Form() { StartPosition = FormStartPosition.Manual, Left = 300, Top = 1200 }; f.Show();
		////s = Screen_.FromObject(f);
		//Print(s.Bounds);
		////Application.Run();

		//Print(Coord.Normalize(5, 6, screen:1));
		//TaskDialog.ShowEx()
		//Mouse.Move(10, 10, screen: 1);

		//Mouse.Move(Coord.Normalize(10, 10, screen: 1));

		//Print(Screen_.FromIndex(2));
		//Print(Screen_.FromIndex(2, true));

		TaskDialog.Options.DefaultScreen = 2; //invalid
		TaskDialog.Show();

		//var w = Wnd.Find("* Notepad");
		//w.Activate();
		////w.MoveInScreen(20, 20, 2);
		//w.EnsureInScreen(2); //invalid
	}

	//static void TestScreenGetIndex()
	//{
	//	var s = Screen_.FromIndex(1);
	//	TaskDialog.Show();
	//	Print(s.DeviceName);
	//	Print(s.GetHashCode());
	//	Print(s.Bounds); //{X=224,Y=1080,Width=1366,Height=768}
	//	Print(s.GetIndex_());
	//}

	static void TestMoveInScreenWithCoord()
	{
		//Wnd w = Wnd.Find("* Notepad");
		////w.Activate();

		//Coord x, y;
		//x = 200; y = 100;
		//x = y = -100;
		//x = y = Coord.Reverse(30);
		//x = y = Coord.Reverse(-100);
		//x = y = null;
		//x = y = Coord.Fraction(0.05);
		//x = y = Coord.Fraction(0.98);
		//x = y = Coord.Fraction(1.1);
		//x = y = Coord.Fraction(-0.1);

		//w.MoveInScreen(x, y, 1);
		//Wait(1);
		//w.MoveInScreen(x, y, 0);
		//Wait(1);
		//w.MoveInScreen(x, y, 0, workArea:false);
		//Wait(1);
		//w.MoveInScreen(x, y, 0, ensureInScreen:false);

		//w.EnsureInScreen();
		//w.EnsureInScreen(1);
		//w.EnsureInScreen(workArea:false);

		////var r = new RECT(100, 101, 20, 21, true);
		//var r = new RECT(-10, 101, 20, 21, true);
		//Print(r);
		////r.MoveInScreen(100, 100, 1);
		//r.MoveInScreen(-10, 100, 1, ensureInScreen:false);
		////r.EnsureInScreen(1);
		////r.EnsureInScreen();
		//Print(r);
		////Mouse.Move(new Point(r.left, r.top));

		//TaskDialog.Options.DefaultScreen = 1;
		//TaskDialog.Show();
		//TaskDialog.ShowEx(x:200, y:100);
		//TaskDialog.ShowEx(x:200, y:1400);
		//TaskDialog.ShowEx(x:200, y:1700, flags:TDFlags.RawXY);
		//TaskDialog.ShowEx(y:1700, flags:TDFlags.RawXY);
		//TaskDialog.ShowEx(x: Coord.Reverse(-10), y: Coord.Reverse(-10));
		//TaskDialog.ShowEx(x: Coord.Fraction(0.1), y: Coord.Fraction(0.95));
		//TaskDialog.ShowEx(x: Coord.Fraction(0.1), y: Coord.Fraction(1.5));
		//TaskDialog.ShowEx(x: Coord.Fraction(0.1), y: Coord.Fraction(1.5), flags:TDFlags.RawXY);

		//TaskDialog.ShowEx(x: Coord.Reverse(0), y: Coord.Reverse(0));
		//TaskDialog.ShowEx(x: Coord.Right, y: Coord.Bottom);
		TaskDialog.ShowEx(x: Coord.Max, y: Coord.Max);

		//Shell.Run("notepad.exe");
		//TaskDialog.Show();
		//TaskDialog.ShowEx(x: 200, y: 100);
		//TaskDialog.Show(owner:null);

		//var f = new Form();
		////f.Click+=(unu, sed) => TaskDialog.Show("test");
		////f.Click+=(unu, sed) => TaskDialog.Show("test", owner:f);
		////f.Click+=(unu, sed) => TaskDialog.Show("test", owner:f.Wnd_());
		//TaskDialog.Options.AutoOwnerWindow = true;
		//f.Click+=(unu, sed) => TaskDialog.Show("test");
		//f.ShowDialog();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestRejectTypes()
	{
		//var d = _TestRejectTypes_1();
		//for(int i=0; i<1; i++) {GC.Collect();Sleep(100); }
		//d.ShowDialog();

		//var ic=Icon.ExtractAssociatedIcon(Folders.System + "notepad.exe");
		//var hi = Icons.GetFileIconHandle(".cpp", 32);

		//var d = new TaskDialog("test");
		//d.SetIcon(ic);
		//d.SetFooterText("footer", Icon.FromHandle(hi));
		//d.ShowDialog();
		////d.ShowDialog();
		////ic.Dispose(); ic = null;

		////GC.Collect(); Sleep(100);

		//Icons.DestroyIconHandle(hi);


		//Print(TaskDialog.Show(buttons: "One|Two"));
		//Print(TaskDialog.ShowList("One|Two"));
		//Print(TaskDialog.ShowList(new string[] { "One", "Two" }));
		//Print(TaskDialog.ShowList(new List<string> { "One", "Two" }));
		//Print(TaskDialog.ShowList(5));

		//TestRejectTypes_Members("st");
		//TestRejectTypes_Object("st");
		//TestRejectTypes_Object(5);

		//var w = Wnd.Find("Quick*");
		//TestRejectTypes_Object(w);
		//TestRejectTypes_Members(w);
		//TestRejectTypes_Object(5);

		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 8; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { TestRejectTypes_Members("st"); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestRejectTypes_Object("st"); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestRejectTypes_Members(5); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { TestRejectTypes_Object(5); }
		//	Perf.NW();
		//}



		//var ic=Icon.ExtractAssociatedIcon(Folders.System + "notepad.exe");
		//var d = new TaskDialog("test");
		//d.SetIcon(ic);
		//ic.Dispose();
		//d.ShowDialog();

		//for(int i = 0; i < 10; i++) {
		//	Task.Run(() =>
		//	{
		//		//bool _locked=false;
		//		//Monitor.Enter("/0p4oSiwoE+7Saqf30udQQ", ref _locked);
		//		TaskDialog.ShowEx("test", timeoutS: 1);
		//		//if(_locked) Monitor.Exit("/0p4oSiwoE+7Saqf30udQQ");
		//	});
		//}
		//Wait(5);

		//lock("/0p4oSiwoE+7Saqf30udQQ") {
		//	Print("gg");
		//}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestRejectTypes_Members(Types<string, int> x)
	//static void TestRejectTypes_Members(Types<string, Wnd> x)
	{
		if(x.type == 3) Print("Control");
	}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static void TestRejectTypes_Object(Types2<string, int> x)
	////static void TestRejectTypes_Object(Types2<string, Wnd> x)
	//{
	//	if(x.Value is Control) Print("Control");
	//}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static TaskDialog _TestRejectTypes_1()
	{
		//var h = Icons.GetFileIconHandle(Folders.System + "notepad.exe,0", 32);
		//var ic = Icon.FromHandle(h);

		var ic = Icon.ExtractAssociatedIcon(Folders.System + "notepad.exe");

		var d = new TaskDialog("test");
		d.SetIcon(ic);
		//ic.Dispose();

		return d;
	}

	internal static void LibCheckEnumFlags<T>(int flags, int goodFlags) where T : struct, IConvertible
	{
		if(flags != (flags & goodFlags)) _WarningBadEnumFlags(typeof(T), goodFlags);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _WarningBadEnumFlags(Type t, int goodFlags)
	{
		if(!Options.Debug) return;
		var s = new StringBuilder("Bad flags. Only these flags are used: "); bool added = false;
		for(int i = 1; i != 0; i <<= 1) {
			if(0 == (i & goodFlags)) continue;
			if(added) s.Append(", "); else added = true;
			s.Append(t.GetEnumName(i));
		}
		s.Append('.');
		Print(s);
	}

	//Unsuccessfully tried to create a generic method with enum type parameters and to avoid boxing.
	//Probably it could be possible only with emiting code at run time.
	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestEnumToInt<T>(T e) where T : struct, IConvertible
	{
		//int h =e.GetHashCode();
		//Print(h);

		//these two are from https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int
		//both work but don't create fast code

		//1:
		//var d = _CreateDelegate<T>();
		//Print(d);
		//Print(d(e));

		//2:
		Func<T, int> converter = EqualityComparer<T>.Default.GetHashCode;
		Print(converter(e));
	}

	public static int Identity(int x) { return x; }
	static Func<int, int> _identity = Identity;
	static Func<T, int> _CreateDelegate<T>()
	{
		return Delegate.CreateDelegate(typeof(Func<T, int>), _identity.Method) as Func<T, int>;
	}

	//static unsafe void TestRefVariable()
	//{
	//	var p = new Point(1, 2);
	//	_TestRefVariable(&p);
	//	Print(p.Y);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void _TestRefVariable(void* p)
	//{
	//	ref var r = ref *(Point*)p;
	//	r.Y = 7;
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void _TestRefVariable2(LPARAM p)
	//{
	//	var r = (Point*)p;
	//	r->Y = 7;
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static unsafe void _TestRefVariable3(void* p)
	//{
	//	var r = (Point*)p;
	//	r->Y = 7;
	//}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestIntNullable2()
	{
		//TestIntNullable(5, 6);
		TestCoord6(5, 6);
	}

	static void TestBitArray()
	{
		var a = new BitArray(64);
		a[0] = true;
		a[31] = true;

		bool t = a.Get(31);
		if(t) Print("1");

		//var aa = new int[2];
		//a.CopyTo(aa, 0);
		//PrintHex(aa[0]);
	}

	static void TestApiTimeBeginPeriodBug()
	{
		//using(new Time.LibSleepPrecision(5)) {
		//for(int i = 0; i < 30; i++) {
		//	var v = Time.LibSleepPrecision.Current;
		//	using(new Time.LibSleepPrecision(1)) {
		//		PrintList(v, Time.LibSleepPrecision.Current);
		//	}
		//	Wait(0.25);
		//}
		//}
	}

	#region instant disposing of thread variables on thread exit

	static void TestThreadStaticDtor()
	{
		//_TestThreadStaticDtor.Create();

		//PrintList("main", Api.GetCurrentThreadId());

		for(int i = 0; i < 3; i++) {
			var t = new Thread(() =>
			{
				//PrintList(i+1, Api.GetCurrentThreadId());
				_TestThreadStaticDtor.Create();
				//t_loc.Value = null;

				//Thread_.Exit += (unu, sed) => { Print("event"); };
			});
			t.Start();
			Wait(0.5);
		}
		//GC.AddMemoryPressure(150000);

		//t_loc.Dispose();
		//t_loc = null;

		//Print("GC 1");
		//GC.Collect();
		//Wait(0.1);
		//Print("GC 2");
		Wait(1);
		Print("bye");
	}
	class _TestThreadStaticDtor
	{
		public int V;

		public _TestThreadStaticDtor() { V = 3; }
		~_TestThreadStaticDtor() { Print("dtor"); }

		public static void Create()
		{
			t_x = new _TestThreadStaticDtor();

			//if(!t_var.IsValueCreated) t_var.Value = new _TestThreadStaticDtor();
			//Print(t_var.Value.V);

			////Print(t_loc.IsValueCreated);
			////Print(t_loc.Value.V);
			////Print(t_loc.IsValueCreated);

			//return;

			//Application.ThreadException += (unu, sed) => { Print("Application.ThreadException"); }; //no
			//Application.ThreadExit += (unu, sed) => { Print("Application.ThreadExit"); }; //no
			SetThreadEndCallback(null);
			Wait(0.1);
			//throw new CatException();
			//FlsFree(s_flsIndex);
		}

		[ThreadStatic] static _TestThreadStaticDtor t_x;
	}
	//static ThreadLocal<_TestThreadStaticDtor> t_loc = new ThreadLocal<_TestThreadStaticDtor>(() => new _TestThreadStaticDtor());
	////static ThreadVar<_TestThreadStaticDtor> t_var=new ThreadVar<_TestThreadStaticDtor>(() => new _TestThreadStaticDtor());
	//static ThreadVar<_TestThreadStaticDtor> t_var=new ThreadVar<_TestThreadStaticDtor>();

	static unsafe void SetThreadEndCallback(Action<IntPtr> callback)
	{
		if(s_flsIndex == null) {
			lock(_flsCallback) {
				if(s_flsIndex == null) s_flsIndex = FlsAlloc(_flsCallback);
			}
		}
		if(s_flsIndex == -1) throw new CatException();
		//Print(s_flsIndex);
		FlsSetValue(s_flsIndex.GetValueOrDefault(), (IntPtr)5);
		FlsSetValue(s_flsIndex.GetValueOrDefault(), (IntPtr)6);
	}
	//static int s_flsIndex = FlsAlloc(_flsCallback);
	static int? s_flsIndex;
	static PFLS_CALLBACK_FUNCTION _flsCallback = _FlsCallback;
	static void _FlsCallback(IntPtr param)
	{
		//Print(GC.MaxGeneration); //2
		Perf.First();
		//Print(GC.GetTotalMemory(false));
		GC.Collect();
		//GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized); //no
		//GC.Collect(0); //ok, but not faster
		//GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, false); //ok, same speed, maybe slightly slower when true
		Perf.NW();
	}

	internal delegate void PFLS_CALLBACK_FUNCTION(IntPtr lpFlsData);

	[DllImport("kernel32.dll")]
	internal static extern int FlsAlloc(PFLS_CALLBACK_FUNCTION lpCallback);

	[DllImport("kernel32.dll")]
	internal static extern bool FlsSetValue(int dwFlsIndex, IntPtr lpFlsData);

	[DllImport("kernel32.dll")]
	internal static extern bool FlsFree(int dwFlsIndex);

	#endregion

	//static int s_four = 4;

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestUnsafeLibrary(MButton b, bool add)
	{
		//int i =Unsafe.SizeOf<RECT>();
		//if(i== s_four) Print("4");

		//bool y = b.Has_(MButton.Right|MButton.Left);
		//bool y = b.Has_(MButton.Right);
		//Print(y ? "yes" : "no");

		//var p = new Point();
		//p.

		//Math_.SetFlag(ref b, MButton.Left, add);
		////Math_.SetFlag(ref b, MButton.Down, false);
		////Print(b);
		//Print(b==(MButton.Left|MButton.Right) ? "yes" : "no");

		//Debug_.CheckFlagsOpt(b, MButton.Down|MButton.Up);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestArrayBuilder()
	{
		//var a = new Catkeys.Util.LibArrayBuilder<long>();
		//a.Add() = 8;
		//var v=a[i];
		//Print(v==8 ? "8" : "no");

		var a = Wnd.Misc.AllWindows(sortFirstVisible: true);
		Print(a);
		Print(a.Length);
		//Print(Wnd.Misc.AllWindows(onlyVisible:true));
	}
	//static void TestArrayBuilder()
	//{
	//	var a = new Catkeys.Util.LibArrayBuilder<long>();
	//	a.Add() = 8;
	//	PrintList(a.Count, a.Capacity);
	//	var v=a[0];
	//	Print(v);
	//	a[0] = 9;
	//	Print(a[0]);
	//	long i = 10;
	//	//a.Add(ref i);
	//	a.AddV(10); Print(a[1]);
	//	//Print(a[2]);

	//	var r = new RECT(1, 2, 3, 4, false);
	//	var aa = new Catkeys.Util.LibArrayBuilder<RECT>();
	//	//aa.Add(ref r);
	//	aa.AddV(r);
	//	Print(aa[0]);
	//	aa.Add()=r;
	//}

	static void TestWeakReference()
	{
		//var k = Catkeys.Util.LibCharBuffer.LibCommon;

		//for(int i = 0; i < 3; i++) {
		//	var w = Wnd.Find("* Notepad");
		//	Wait(0.5);
		//	GC.Collect();
		//	Wait(0.5);
		//}

		//var a = Catkeys.Util.LibBuffer.Get(100, ref t_ca);
		//Print(a.Length);

		//Perf.First();
		//t_ca = new WeakReference<char[]>(null);
		//t_test = null; //speed 1500-3000 when assigning a [ThreadStatic] first time in appdomain (maybe in process, not tested)
		//Perf.NW();
		//Task.Run(() =>
		//{
		//	Perf.First();
		////t_ca = new WeakReference<char[]>(null);
		//t_test = null;
		//	Perf.NW();
		//});
		Wait(0.2);

		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 8; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { var a = new char[300]; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var a = Catkeys.Util.LibBuffer.Get(300, ref t_ca); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}

		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	Perf.Incremental = true;
		//	for(int i2 = 0; i2 < n2; i2++) {
		//		//Thread.Sleep(1);
		//		Perf.First(1);
		//		Perf.Next();
		//		var a = new char[3000];
		//		Perf.Next();
		//		var a2 = Catkeys.Util.LibBuffer.Get(3000, ref t_ca);
		//		Perf.Next();
		//	}
		//	Perf.Write(); Perf.Incremental = false;
		//}

	}

	static unsafe void TestLibBuffers()
	{
		Wnd w = Wnd.FindFast(null, "Notepad");
		//string s;
		//s = w.ClassName;
		//Print(s);
		//s = w.Name;
		//Print(s);
		//Wait(0.1);

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { s = w.ClassName; });
		//var a2 = new Action(() => { s = w.Name; });
		//var a3 = new Action(() => { var b = Catkeys.Util.LibCharBuffer.LibCommon.Alloc(300); });
		//var a4 = new Action(() => { var b = Catkeys.Util.LibBuffers.LibChar(300); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//Wnd w = Wnd.FindFast("Calculator", null);
		//w = w.ChildFast(null, "Windows.UI.Core.CoreWindow");
		//Print(Wnd.Misc.GetWindowsStoreAppId(w));

		//var x = Shell.Shortcut.Create(Folders.Desktop + "test.lnk");
		//x.TargetPath = Folders.System + "notepad.exe";
		//x.Arguments = "/args";
		//x.Description = "descr";
		//x.WorkingDirectory="Q:\\";
		//x.SetIconLocation(@"Q:\app\paste.ico");
		//x.Save();

		//var x=Shell.Shortcut.Open(Folders.Desktop + "test.lnk");
		//PrintList(x.TargetPath, x.Arguments, x.Description, x.WorkingDirectory);
		//Print(x.GetIconLocation(out var i));

		//Print(Shell.Shortcut.GetTarget(Folders.CommonPrograms+@"Microsoft Office\Microsoft Office Word 2003.lnk"));

		//Print(Files.SearchPath("notepad.exe"));

		//Print(w.ProcessName);

		//var m = new Process_.Memory(w, 1000);
		//m.WriteUnicodeString("unicode", 100);
		//m.WriteAnsiString("ansi", 200);
		//Print(m.ReadUnicodeString(7, 100));
		//Print(m.ReadAnsiString(4, 200));

		//Print(Path_.ExpandEnvVar("%temp%"));
		//Print(Path_.LibGetEnvVar("temp"));
		//Print(Path_.LibEnvVarExists("temp"));
		//Print(Path_.Normalize(@"Q:\app\catkeys\..\icons"));
		//Print(Path_.Normalize(@"c:\progra~1"));

		//Print(Convert_.HexDecode("4041"));

		//var s = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 });
		//Print(Convert_.Base64Decode(s));

		//var file = Folders.Temp + "test.txt";
		//Output.LogFile = file;
		//Print("aaa bbb");
		//Output.LogFileTimestamp = true;
		//Print("ccc ddd");
		//Shell.Run(file);

	}

	class Example
	{
		public static void Test()
		{
			Wnd w = Wnd.FindFast(null, "Notepad");
			string s = GetWndText(w);
			Print(s);
		}

		public static string GetWndText(Wnd w)
		{
			for(int na = 300; ; na *= 2) {
				var b = _GetCharBuffer(ref na);
				int nr = GetWindowText(w, b, na);
				if(nr < na - 1) return (nr > 0) ? b.ToString(nr) : "";
			}
		}

		//this variable manages the buffer
		[ThreadStatic] static WeakReference<char[]> t_char;

		//a helper method
		static Catkeys.Util.Buffers.CharBuffer _GetCharBuffer(ref int n) { var r = Catkeys.Util.Buffers.Get(n, ref t_char); n = r.Length - 1; return r; }

		//we'll use this API in this example
		[DllImport("user32.dll", EntryPoint = "GetWindowTextW")]
		static extern int GetWindowText(Wnd hWnd, [Out] char[] lpString, int nMaxCount);
	}

	//[ThreadStatic] static WeakReference<char[]> t_ca;
	//[ThreadStatic] static char[] t_test;

	//static unsafe void TestSetStringLength()
	//{
	//	string s = new string('A', 10);

	//	fixed (char* p = s) {

	//		Unsafe.Write(p - 2, 4);
	//	}
	//	Print(s.Length);
	//	Print(s);
	//}

	public static unsafe class TestFormat
	{

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int _snwprintf(char* lpOut1024, int count, string lpFmt, __arglist);

		static void TestFormatSpeed()
		{
			var sb = new StringBuilder(1000);
			var ca = new char[1000];
			int x = 500, y = 600; string s = "sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss";
			string r1 = null, r2 = null, r3 = null, r4 = null;

			//sb.Clear(); sb.AppendFormat("{0}    {1}    {2}", x, y, s); r1 = sb.ToString();
			//fixed (char* p = ca) { Api.wsprintfW(p, "%i    %i    %s", __arglist(x, y, s)); r2 = new string(ca, 0, LibCharPtr.Length(p)); }
			//fixed (char* p = ca) { _snwprintf(p, 1000, "%i    %i    %s", __arglist(x, y, s)); r3 = new string(ca, 0, LibCharPtr.Length(p)); }
			//sb.Clear(); sb.Append(x); sb.Append("    "); sb.Append(y); sb.Append("    "); sb.Append(s); r4 = sb.ToString();

			Perf.SpinCPU(100);
			for(int i1 = 0; i1 < 9; i1++) {
				int n2 = 1000;
				Perf.First();
				for(int i2 = 0; i2 < n2; i2++) { sb.Clear(); sb.AppendFormat("{0}    {1}    {2}", x.ToString(), y.ToString(), s); r1 = sb.ToString(); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { sb.Clear(); sb.AppendFormat("{0}    {1}    {2}", x, y, s); r1 = sb.ToString(); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { fixed (char* p = ca) { Api.wsprintfW(p, "%i    %i    %s", __arglist(x, y, s)); r2 = new string(ca, 0, Catkeys.Util.LibCharPtr.Length(p)); } }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { fixed (char* p = ca) { _snwprintf(p, 1000, "%i    %i    %s", __arglist(x, y, s)); r3 = new string(ca, 0, Catkeys.Util.LibCharPtr.Length(p)); } }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { sb.Clear(); sb.Append(x); sb.Append("    "); sb.Append(y); sb.Append("    "); sb.Append(s); r4 = sb.ToString(); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { sb.Clear(); sb.Append(x.ToString(CultureInfo.InvariantCulture)); sb.Append("    "); sb.Append(y.ToString(CultureInfo.InvariantCulture)); sb.Append("    "); sb.Append(s); r4 = sb.ToString(); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { r1 = $"{x}    {y}    {s}"; }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { var ss = x.ToString(); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { var ss = x.ToString(CultureInfo.InvariantCulture); }
				Perf.NW();
			}

			Print(r1);
			Print(r2);
			Print(r3);
			Print(r4);
		}
	}


	//static _DefDomainVar s_ggg;
	//static bool boo = InterDomainVariables.DefaultDomainVariable2("gggg", out  s_ggg);

	static DefDomainVar s_idvTest = InterDomainVariables.DefaultDomainVariable<DefDomainVar>(nameof(s_idvTest));

	static void TestInterDomainVariables()
	{
		s_idvTest.Method(7);
	}

	class DefDomainVar :MarshalByRefObject
	{
		public void Method(int param)
		{
			PrintList(param, AppDomain.CurrentDomain.FriendlyName);
		}
	}

	#region mouse
	static void TestWndClick()
	{
		//Options.Relaxed = true;

		//Wnd.Find("Quick*").ChildById(2207).Click(10, 10);
		//Wnd.Find("Quick*").ChildById(2207).MouseMove(10, 10);
		//WaitFor.WindowActive(0, "Quick*").MouseMove(100, 100);

		//Print(WaitFor.ModifierKeysReleased(-5));
		//Print(WaitFor.MouseButtonsReleased(-5));
		//WaitFor.MouseButtonsReleased(out var wasPressed); Print(wasPressed);

		//Wnd.Find("Quick*").Click(10, 10);
		//Wnd.Find("Quick*").Click(20, 10, mmFlags:MFlags.NonClientXY|MFlags.WaitForButtonsReleased);

		//var w = Wnd.Find("Quick*");
		var w = Wnd.Find("* Notepad");
		//w.Click(100, 100);
		//w.Click(100, 100, flags:MFlags.Relaxed);
		//w.Click(10, 10, flags:MFlags.NonClientXY);
		//w.Click(10, 10, flags:MFlags.WorkAreaXY);
		//w.Click(400, 90, MButton.Down);
		//Wait(0.1);
		//Wait(3);
		//w.MouseMove(350, 70);
		//w.MouseMove(350, 70, mmFlags: MFlags.Relaxed);
		//Wait(0.1);
		//Mouse.LeftUp();

		//Mouse.Move(-100, 400);

		//Mouse.LeftDown(w, 1, 100, true);
		//Mouse.LeftDown(w, -1, 100);
		//Mouse.LeftUp(w, 200, w.MouseClientXY.Y);

	}

	static void TestOptionsMouseMoveSpeed()
	{
		//Wnd.Find("app -*").Activate(); Wait(0.5);
		Options.MouseMoveSpeed = 50;
		//Mouse.LeftDown(176, 910);
		//Mouse.MoveRelative(50, 0);
		//Mouse.LeftUp();

		Mouse.MoveRelative(50, 0);
		Mouse.MoveRelative(50, 200);
		Mouse.MoveRelative(-150, -100);
		Mouse.MoveRelative(500, 500);

		//test crossing an offscreen area
		//Mouse.Move(100, 200, screen: 1);
		//Options.MouseMoveSpeed = 500;
		//Mouse.Move(11, 962);

	}

	static void TestRecordMouseMove()
	{
		var xy0 = Mouse.XY;
		s_withSleepTimes = false;
		s_recptime = Time.Milliseconds;
		s_recMoves = new List<uint>();

		var hh = Api.SetWindowsHookEx(Api.WH_MOUSE_LL, _testRecordMouseMove_Proc, Catkeys.Util.ModuleHandle.OfProcessExe(), 0);
		Thread.CurrentThread.Join(7000);
		Api.UnhookWindowsHookEx(hh);

		var s = Catkeys.Util.Recording.MouseToString(s_recMoves, s_withSleepTimes);
		Print(s);

		Wait(1);
		Mouse.Move(xy0);
		Mouse.MoveRecorded(s);
	}

	static Api.HOOKPROC _testRecordMouseMove_Proc = _TestRecordMouseMove_Proc;
	static unsafe LPARAM _TestRecordMouseMove_Proc(int code, LPARAM msg, LPARAM lParam)
	{
		//var m = (Api.MSLLHOOKSTRUCT*)lParam;
		ref Api.MSLLHOOKSTRUCT m = ref *(Api.MSLLHOOKSTRUCT*)lParam;

		//PrintList(m.pt.X, m.pt.Y);
		//PrintList(m.pt.X - s_pp.X, m.pt.Y - s_pp.Y);

		var d = new Point(m.pt.X - s_pp.X, m.pt.Y - s_pp.Y);
		int ddx = d.X - s_ppd.X, ddy = d.Y - s_ppd.Y;
		//string s = null;
		//if(ddx < -16 || ddx > 15 || ddy < -16 || ddy > 15) s = ">15";
		//else if(ddx < -8 || ddx > 7 || ddy < -8 || ddy > 7) s = ">7";
		//else if(ddx < -4 || ddx > 3 || ddy < -4 || ddy > 3) s = ">3";
		//Print($"{d.X}, {d.Y}      {ddx}, {ddy}     {s}");
		s_pp = m.pt;
		s_ppd = d;

		if(s_withSleepTimes) {
			var t = Time.Milliseconds;
			s_recMoves.Add((uint)(t - s_recptime));
			s_recptime = t;
		}
		s_recMoves.Add(Math_.MakeUint(d.X, d.Y));

		return Api.CallNextHookEx(Zero, code, msg, lParam);
	}
	static Point s_pp = Mouse.XY;
	static Point s_ppd;
	static bool s_withSleepTimes;
	static long s_recptime;
	static List<uint> s_recMoves;

	static void TestHowLooksRecordedMouseScript()
	{
		Wnd w1 = Wnd.Find("Untitled - Notepad", "Notepad");

		//Mouse.Click(w1, 100, 100);
		//w1.MouseClick(100, 100);

		//Mouse.RightClick(w1, 100, 100);
		//w1.MouseClick(100, 100, MButton.Right);

		//Mouse.MoveRecorded("sjdksjdkjskdjksjdksjkdsjdks");

		//var speed = 0.5;
		//Mouse.MoveRecorded("sjdksjdkjskdjksjdksjkdsjdks", speed);
		//Mouse.Click(w1, 100, 100); Wait(0.5 * speed);
		////Mouse.Click(w1, 100, 100); WaitSF(0.5); //=Wait(0.5 * Options.SpeedFactor);
		////Mouse.ClickWait(w1, 100, 100, 0.5);

		//Mouse.RightClick()
		//Mouse.Move(
		Mouse.Click(w1, 1, 2);
		Mouse.ClickEx(MButton.Middle, w1, 1, 2);
		w1.MouseClick(1, 2);
		w1.MouseClick(1, 2, MButton.Right);
		Mouse.LeftDown(w1, 100, 200);
		Mouse.ClickEx(MButton.Left | MButton.Down, w1, 100, 200);

		//var f=new FileStream()
	}

	static void TestFindOrRun()
	{
		Wnd w = Wnd.FindOrRun("* Notepad", run: () => Shell.Run("notepad.exe"));
		Print(w);
	}

	static void TestMouseDragDrop()
	{
		var w = Wnd.Find("* Notepad");
		//w.Activate();
		//Wait(0.5);

		//Mouse.LeftDown(w, 8, 8);
		//Mouse.LeftUp(w, 28, 8);

		//Mouse.LeftDown(w, 8, 8);
		//Mouse.MoveRelative(20, 0, drop: true);

		//Mouse.LeftDown(w, 8, 8);
		//Mouse.MoveRelative(20, 0);
		////Mouse.Drop();
		//Wait(3);
		//Mouse.LeftUp(true);

		//using(Mouse.LeftDown(w, 8, 8)) Mouse.MoveRelative(20, 0);
		//using(Mouse.ClickEx(MButton.Left|MButton.Down, w, 8, 8)) Mouse.MoveRelative(20, 0);

		//Mouse.Move(w, 100, 100);
		//Mouse.Wheel(-5);
		//Wait(0.5);
		//Mouse.Wheel(1);

		//Wnd w2 = Wnd.Find("* WordPad");
		//w2.ZorderTopmost();
		//w2.ZorderNoTopmost();
		//Wait(0.5);

		//w = Wnd.Find("", "WorkerW");

		Mouse.Click(w, 20, 20);
	}
	#endregion
}