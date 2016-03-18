using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;


class ScriptClass :Script
{
	//static ScriptClass() { ScriptOptions.Default.speed=50; }

	//static void Main()
	public static void TestMain()
	{
		//new ScriptClass().Script();
		//new ScriptClass().CallFirstMethod();
		new ScriptClass().CallTriggerMethod("Script", "bla");
	}

	//public void Meth1()
	//{
	//	Out("Meth1");
	//}

	void Script()
	//void Script(int unu)
	//void Script(string unu)
	//void Script(int unu, string unus)
	{
		//OutFunc();
		//Out(unu);
		//ScriptOptions.ThreadDefault.speed=50;

		Out(Option.speed);
		Option.speed=10;
		//Out(Option.speed);
		TestStatic();
		Option.speed=20;
		AnotherClass.TestAnother();

		//var t=new Thread(ThreadProc);
		//t.Start();
		new Thread(ThreadProc).Start();
		Thread.Sleep(300);

		//var m=new Macro();
		//m.speed=10; m.slowKeys=true;
		//Out(m.waitMsg);

		//m.Key("Ctrl+K");
		//m.Click();
		//m.UserMethod();
		//m.Mouse.Click();
	}

	public void Meth2()
	{
		Out("Meth2");
	}


	void ThreadProc()
	{
		Out(Option.speed);
	}

	public static void TestStatic()
	{
		Out(Option.speed);
	}
}

class AnotherClass
{
	public static void TestAnother()
	{
		Out(Script.Option.speed);

		var o = new ScriptOptions();
		Out(o.speed);
		Moo(new ScriptOptions() { speed=6, slowKeys=true });
	}

	public static void Moo(ScriptOptions o)
	{
		Out(o.speed);
	}
}

partial class Test
{
	static void TestOptions()
	{
		ScriptClass.TestMain();
	}
}
