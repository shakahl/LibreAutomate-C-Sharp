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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;


static partial class Test
{

	class AssemblyOptions
	{
		public int k;
	}
	static ConditionalWeakTable<Assembly, AssemblyOptions> s_ao = new ConditionalWeakTable<Assembly, AssemblyOptions>();

	static void TestAssemblySettings()
	{
		var a = Assembly.GetEntryAssembly();
		var ao = new AssemblyOptions() { k = 5 };
		s_ao.Add(a, ao);
		if(s_ao.TryGetValue(a, out var o)) Print(o.k);
	}

	public class Autotext :IDisposable
	{

		public void Dispose()
		{

		}

		//public void Add(string text, ATValue replacement)
		//{

		//}

		public void Add(string text, string replacement)
		{
			Print(1);
		}

		public void Add(string text, Func<string, string> replacement)
		{
			Print(2);
		}

		public void Add(string text, Action<string> replacement)
		{
			Print(3);
		}

		public object this[string text]
		{
			set
			{
				Print(4);
			}
		}

		//public ATValue this[string text]
		//{
		//	set
		//	{

		//	}
		//}

		//public int this[string text]
		//{
		//	set
		//	{

		//	}
		//}

		public void Run()
		{

			//Time.SleepDoEvents(Timeout.Infinite);
		}
	}

	//public struct ATValue
	//{
	//	object _o;
	//	ATValue(object o) => _o = o;

	//	public static implicit operator ATValue(string s) => new ATValue(s);
	//	public static implicit operator ATValue(Func<string, string> s) => new ATValue(s);
	//	public static implicit operator ATValue(Action<string> s) => new ATValue(s);
	//}

	static void TestAutotext()
	{
		using(var a = new Autotext()) {
			a.Add("one", "ONE");
			a.Add("two", o => "TWO");
			a.Add("three", o => Print("THREE"));
			//a.Add("two", new Func<string, string>(o => "TWO"));
			a["one"] = "ONE";
			//a["two"] = o => "TWO";
			//a["two"] = new Func<string, string>(o => "TWO");
			//a["two"] = new FuncSS(o => "TWO");
			a.Run();
		}
	}

	//public delegate string FuncSS(string s);



	/*
	print(...); //Output.Write(...);
	key(...); //Keyb.Key(...);
	tkey(...); //Keyb.Text(...); //or txt
	paste(...); //Keyb.Paste(...);
	msgbox(...); //ADialog.Show(...);
	click(...); //Mouse.Click(...);
	mmove(...); //Mouse.Move(...);
	run(...); //Shell.Run(...);
	act(...); //Wnd.Activate(...);
	win(...); //Wnd.Find(...);
	*/
}
