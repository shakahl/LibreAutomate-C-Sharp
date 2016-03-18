using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using System.Reflection;
using System.Diagnostics;

using Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using static Catkeys.Automation.Input;
using Catkeys.Triggers;
using Catkeys.Util; using Util = Catkeys.Util;
using Catkeys.Winapi;

//public class Macro
//{
//	public int speed=100;
//	public bool slowKeys=false, waitMsg;

//	public void Key(string keys)
//	{
//	}

//	public void Click(int x, int y, int w)
//	{
//	}

//	public void Click(int x, int y)
//	{
//	}

//	public void Click()
//	{
//	}

//	public class MouseMethods
//	{
//		Macro _m;
//		public MouseMethods(Macro m) { _m=m; }

//		public void Click()
//		{
//		}
//	}

//	//public readonly MouseMethods Mouse;

//	MouseMethods _Mouse;
//	public MouseMethods Mouse { get { return _Mouse ?? (_Mouse=new MouseMethods(this)); } }

//	//public Macro()
//	//{
//	//}

//}

//public static class MacroExt
//{
//	public static void UserMethod(this Macro m)
//	{
//		Out(m.speed);
//	}
//}


class ScriptClass //:ScriptBase
{
	static void Main()
	{
		char[] a4 = { 'A', 'B', 'C' };

		Console.WriteLine(a4);
	}

	public void Script()
	{
	}
}

//struct Point { public int x, y; };

//class Acc
//{
//	void Test()
//	{
//		Out(ScriptBase.Option.speed);


//		Point p = new Point() { x=1, y=2 };
//		var o = new ScriptOptions() { speed=10 };
//	}
//}
