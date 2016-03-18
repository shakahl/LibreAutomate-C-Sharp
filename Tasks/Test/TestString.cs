using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Threading;
//using System.Threading.Tasks;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

//using static System.StringComparison; //ok
//using static Catkeys.Winapi.api; //ok
//using static W = Catkeys.Winapi.api;

//using Catkeys.Automation;
//using l = Catkeys.Automation;
//using static Catkeys.Automation.Input;

using System.Windows.Input;

//public static partial class aaString
//{
//	public static string aTrim(this string s) { return "ext"; }
//	public static string aaTrimCI(this string s) { return "ext"; }
//	public static string xTrimCI(this string s) { return "ext"; }
//	public static string _aTrimCI(this string s) { return "ext"; }
//	public static string _kTrimCI(this string s) { return "ext"; }
//	public static string _xTrimCI(this string s) { return "ext"; }
//}

partial class Test
{
	static readonly string[] _Newlines = { "\r\n", "\n" }; //error if const; now elements can be changed, therefore don't make it public

	[MethodImpl(MethodImplOptions.NoInlining)]
	static int Split1(string s)
	{
		int n = 0;
		foreach(string s2 in s.SplitLines_()) {
			//Out(s2);
			n+=s2.Length;
		}
		return n;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static int Split2(string s)
	{
		int n = 0;
		return n;
	}

	static void Test_str()
	{
		string s = "one\ntwo\r\nthree\r\n\r\nfour";
		int i, n=0;

		for(i=0; i<10000; i++) n+=Split1(s);
		for(i=0; i<10000; i++) n+=Split2(s);
		Time.First(true);
		for(i=0; i<10000; i++) n+=Split2(s);
		Time.Next();
		for(i=0; i<10000; i++) n+=Split1(s);
		Time.Next();
		Time.Write();


		//str s = "abcd";
		//Out(s.IndexOf('c'));
		//Out(string.Join(",", s, s)); //ok
		//str[] a = { "one", "two" };
		//Out(string.Join(",", a)); //ok
		//var a=new List<str>{ "one", "two" };
		//Out(string.Join(",", a)); //ok

		//string[] a = { "hh", "kk" };
		//str[] b = { "tr", "rr" };
		//a=b;
		//b=a;

		//Out($"{DateTime.Now}");
		//Out($"{DateTime.Now:yyyy.MM.dd}");

		//DateTime t = DateTime.Now;
		//string s = "ddd";
		

		//int i = 5;
		//string s = "    oooo  ";
		//s=Path.Expand("ff");
		//Out("one"+_+"two"+_+"three");
		//Out($"one{_}two{_}three");
		//Out("exe".Equals("EXE", CaseInsens));
		//Console.Ted();

		//Out(Ordinal);

		//string s1=null, s2 = null, s3 = null;
		//int i;
		//Time.WakeCPU();
		//Time.First();
		//for(i = 0; i<1000; i++) s1="one\r\ntwo\r\nthree";
		//Time.Next();
		//for(i = 0; i<1000; i++) s2="one"+_+"two"+_+"three";
		//Time.Next();
		//for(i = 0; i<1000; i++) s3=$"one{_}two{_}three";
		////for(i = 0; i<1000; i++) s3=$"one{_}two{_}three{_}one{_}two{_}three{_}one{_}two{_}three{_}one{_}two{_}three{_}one{_}two{_}three{_}one{_}two{_}three{_}one{_}two{_}three{_}";
		//Time.Next();
		//Time.Out();
		//Out(s1+s2+s3);
	}
}



/*
		//Catkeys.Automation.Input.Keys("Ctrl+K");
		////Input.Keys("Ctrl+K");
		//l.Input.Keys("Ctrl+K");
		////l.Input.
		//Keys("Ctrl+K");

	//public static class str2
	//{
	//	public static void ExtMeth(this str s)
	//	{
	//		Out(s);
	//	}
	//}

	//struct Dekk :IComparable<Dekk>, IEquatable<Dekk>
	//{
	//	public int x;
	//	public Dekk(int i) { x=i; }
	//	public int CompareTo(Dekk d) { return x<d.x ? -1 : (x>d.x ? 1 : 0); }
	//	public bool Equals(Dekk d) { return x==d.x; }
	//}

	public partial class TestUtil
	{
		//static void Func(str s1, string s2, StringBuilder s3, str? s4=null)
		//{
		//	Out(s1);
		//	Out(s2);
		//	Out(s3);
		//	if(s4!=null) Out(s4);
		//}

		//[MethodImpl(MethodImplOptions.NoInlining)]
		//static char Foo1(string s)
		//{
		//	return s[0];
		//}
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//static char Foo2(str s)
		//{
		//	return s[0];
		//}

		static void TestSB(str s)
		{
			Out(s);
		}

		//unsafe
		public static void Test_str()
		{
			//str s1 = "abc-me";
			//Out(s1.EndsWith("cme"));

			//sbyte[] b = { 65, 66, 67 };
			//char[] b = { 'a', 'b' };
			//str s = new string(b);

			//var s = new StringBuilder("ddd");
			////Out(s);
			////str k = s;
			////Out(k);
			//TestSB(s);

			str s = "fff";
			StringBuilder k = s;
			Out(k);

			//	string s = "asd";
			//	str g = "bsf";
			//	int i, n1 = 0, n2=0;

			//	Time.WakeCPU();
			//	for(int j = 0; j<5; j++) {

			//		Time.First();
			//		//for(i=0; i<100000; i++) n1+=Foo2(s);
			//		//for(i=0; i<100000; i++) if(s.StartsWith("a", StringComparison.Ordinal)) n1++;
			//		for(i=0; i<100000; i++) if(g.StartsWith("b")) n1++;
			//		Time.Next();
			//		//for(i=0; i<100000; i++) n2+=Foo2(s);
			//		//Time.Next();
			//		if(j<3) continue;
			//		Time.Out();

			//	}
			//		Out($"{n1} {n2}");

			//	return;
			////	Out(sizeof(Dekk));

			////	Dekk d1=default(Dekk), d2=d1; d1.x=4; d2.x=5;
			////	Dekk[] b = new Dekk[3] { new Dekk(5), new Dekk(4), new Dekk(1) };
			////	fixed(int* p= &b[0].x) { Out((int)&p[1]-(int)&p[0]); }
			////	Array.Sort(b);
			////	foreach(Dekk h in b) Out(h.x);

			////	return;

			//	str s1="8";
			//	//str s1 = null; //error
			//	//str s1 = str.Null;
			//	//str s = (string)null;
			//	//str s1 = default(str);
			//	//str s1 = new str();
			//	string s2="8";
			//	StringBuilder s3=new StringBuilder("ccc");
			//	//str? s4=null;
			//	//str? s4="bla";
			//	//s1=new char[]{ 'g', 'k'}; //now error, no operator
			//	//s1.x=new char[] { 'g', 'k' }; //error too
			//	//s1=new string(new char[] { 'g', 'k' }); //ok

			//	//s2=5.ToString();
			//	//int i = Convert.ToInt32(s2);

			//	//str s5 = "55";
			//	//int i = Convert.ToInt32(s5); Out(i);
			//	//if(int.TryParse(s5, out i)) Out(i);

			//	//foreach(char c in s1) Out(c);
			//	//Out(s1==s2);

			//	//TODO: test speed of casting str to/from string.

			//	//object o = "8";
			//	//////Out(s1.Equals(o));
			//	////Out(s1.CompareTo(o));
			//	//Out(s1.Equals(o));
			//	//Out(s1.CompareTo(o));

			//	str[] a = { "Z", "A", "K" };
			//	//Array.Sort(a);
			//	Array.Sort(a, (X,Y)=>str.Compare(X, Y));
			//	//Array.Sort(a, (X,Y)=>X.CompareTo(Y));
			//	foreach(string j in a) Out(j);

			//var a=new List<str>{ "Z", "A", "K" };
			//a.Sort();
			//foreach(string j in a) Out(j);


			//s1=str.Null;
			//Out(s1.TestExc(0));
			//Out(s1[0]);
			//Out(s2[0]);
			//Out(s3[0]);
			////Out(s4[0]); //error
			////s1[0]='s'; //error
			//Out(s1+", "+s1);
			//Out(s1.Length);
			////str s5 = "aaa"; Out(s5==s1); Out(s5=="aaa");

			//s1=s2;
			//s1=s3;
			//s2=s1;
			//s3=s1;

			//object o = s2;
			//s1=o; //error
			//s2=o; //error

			//Func(s1, s2, s3, s4);
			//Func(s1, s2, s3, s1);
			//Func(s1, s1, s1);

			//s1.ExtMeth();
		}
	}
*/
