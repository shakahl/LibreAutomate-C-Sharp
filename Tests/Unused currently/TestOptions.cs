using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Au;
using static Au.NoClass;


class ScriptClass :Script
{
	//static ScriptClass() { AuScriptOptions.Default.speed=50; }

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
		//DebugPrintFunc();
		//Print(unu);
		//AuScriptOptions.ThreadDefault.speed=50;

		Print(Options.MouseClickSleep);
		Options.MouseClickSleep = 10;
		//Print(Option.speed);
		TestStatic();
		Options.MouseClickSleep = 20;
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
		Print(Options.MouseClickSleep);
	}

	public static void TestStatic()
	{
		Print(Options.MouseClickSleep);
	}
}

class AnotherClass
{
	public static void TestAnother()
	{
		Print(Options.MouseClickSleep);

		var o = new AuScriptOptions();
		Print(o.MouseClickSleep);
		Moo(new AuScriptOptions() { MouseClickSleep = 6, MouseMoveSpeed = 100 });
	}

	public static void Moo(AuScriptOptions o)
	{
		Print(o.MouseClickSleep);
	}
}

partial class Test
{
	static void TestOptions()
	{
		//ScriptClass.TestMain();

		//Print(Options.MouseClickSleep);
		//using(AuScriptOptions.Temp()) {
		//	Options.MouseClickSleep = 1000;
		//	Print(Options.MouseClickSleep);
		//}
		//Print(Options.MouseClickSleep);

		//Print(Options.MouseClickSleep);
		//using(var r=AuScriptOptions.Temp()) {
		//	Options.MouseClickSleep = 1000;
		//	Print(Options.MouseClickSleep);
		//	r.Dispose();
		//	Print(Options.MouseClickSleep);
		//	Options.MouseClickSleep = 1;
		//}
		//Print(Options.MouseClickSleep);

		Print(Options.MouseClickSleep);
		using(AuScriptOptions.Temp) {
			Options.MouseClickSleep = 1000;
			Print(Options.MouseClickSleep);
			using(AuScriptOptions.Temp) {
				Options.MouseClickSleep = 1;
				Print(Options.MouseClickSleep);
			}
			Print(Options.MouseClickSleep);
		}
		Print(Options.MouseClickSleep);
	}
}
