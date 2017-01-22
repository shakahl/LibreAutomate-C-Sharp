using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Catkeys;
using static Catkeys.NoClass;


class ScriptClass :Script
{
	//static ScriptClass() { ScriptOptions.Default.speed=50; }

	//[STAThread]
	//static void Main()
	public static void TestMain()
	{
		//new ScriptClass().Script();
		//new ScriptClass().CallFirstMethod();
		new ScriptClass().CallTriggerMethod("Script", "bla");
	}

	//public void Meth1()
	//{
	//	Print("Meth1");
	//}

	void Script()
	//void Script(int unu)
	//void Script(string unu)
	//void Script(int unu, string unus)
	{
		//PrintFunc();
		//Print(unu);
		//ScriptOptions.ThreadDefault.speed=50;

		Print(Option.Speed);
		Option.Speed=10;
		//Print(Option.speed);
		TestStatic();
		Option.Speed=20;
		AnotherClass.TestAnother();

		//var t=new Thread(ThreadProc);
		//t.Start();
		new Thread(ThreadProc).Start();
		Thread.Sleep(300);

		//var m=new Macro();
		//m.speed=10; m.SlowKeys=true;
		//Print(m.WaitMsg);

		//m.Key("Ctrl+K");
		//m.Click();
		//m.UserMethod();
		//m.Mouse.Click();
	}

	public void Meth2()
	{
		Print("Meth2");
	}


	void ThreadProc()
	{
		Print(Option.Speed);
	}

	public static void TestStatic()
	{
		Print(Option.Speed);
	}
}

class AnotherClass
{
	public static void TestAnother()
	{
		Print(Script.Option.Speed);

		var o = new ScriptOptions();
		Print(o.Speed);
		Moo(new ScriptOptions() { Speed=6, SlowKeys=true });
	}

	public static void Moo(ScriptOptions o)
	{
		Print(o.Speed);
	}
}

partial class Test
{
	static void TestOptions()
	{
		ScriptClass.TestMain();
	}
}
