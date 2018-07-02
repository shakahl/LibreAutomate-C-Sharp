using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

using Au;
using Au.Types;
using static Au.NoClass;
//using static Au.Input;
//using Au.Triggers;
//using Util = Au.Util;
//using Au.Winapi;

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


class ScriptClass :Script
{
	[STAThread]
	static void Main()
	{
		//var asm = Assembly.GetEntryAssembly();
		//var rm = new System.Resources.ResourceManager("", asm);
		//Console.WriteLine(rm);

		//Print(asm!=null);

		Output.LibUseQM2 = true;
		TestToolCreatedCode();

		//Console.WriteLine(a4);
		//MessageBox.Show("script");
		//Console.ReadKey();
	}

	public void Script()
	{
	}

	static void TestToolCreatedCode()
	{
		//Print(1);
		//AuDialog.Show();

		//var w = +Wnd.Find("Quick Macros - ok*", "QM_Editor");
		//string image = "image:iVBORw0KGgoAAAANSUhEUgAAAA0AAAARCAYAAAAG/yacAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAF+SURBVDhPndG7S4JRGAZw/422/oBoiSAIHOxGGWrxKWiFZSrd/DIkMLpY0NTWYmqlURSGDUmlDmIk3pMKSQoFc9E0PkVoKIJ4SgfxQzRpeM7wvOfHgfcwCoUCGk0un8NLKoGGUeYtA9ImA/+qt3GksatBOLsgCvU1hk4ChxjxDEAa4UGsFfyN7mJhjHs5mHhigzjvhsfrqY+oHIV59wSkSTZGn3tgONaX+rpIF9iGgiKg+hBCpVMgn8+X+proNhaC8lWEle/flxwEotFoecbYCm9gMTAF+42NhlYjJJY+xZhOD8FkNtJmDHmSizVIsECJcHRxUCqtkTOQOQLkO4FZgwQURdGRxXaKubgA6q8xzKSGYXQYoLwXQ57hgO9iwef30UAJFQ+n24nJBy5k6UEI4yzwI0zwHjuwaVivAsWUF2G/toEX7ER/sA0sfwvYu0wkEokqUAxteybrHtpdzWi9bILRvE+7WJmqlestWmh2lpHNZml9ZWr+U738AxXwAyp0+x5o1+HCAAAAAElFTkSuQmCC";
		//WinImage.Wait(5, w, image, WIFlags.WindowDC)
		//	.MouseClick();

		//var w = +Wnd.Find("Quick Macros - ok - [Macro24]", "QM_Editor");
		//string image = "image:iVBORw0KGgoAAAANSUhEUgAAAA8AAAAOCAYAAADwikbvAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAF5SURBVDhPldDbK4NhAMdx/4Y7f4DcSCnlYk4xMXqnDM05xyGEnMuVOyWHYU4RceG4uVhEwxxCyyJvLTfY9ForF6T0ZSvjNTEX35vn16en5wnxeDwEm/Peic5Ygno9kemVSf6FO03NCOY4NIdJNPU3BI9nrVPkWFIotqnQDmQhimJw+OTqmPzdNAoulAhL8Vh2Lb7zP7H0IFGzU0DxtZLcywT0M8P+7U88ZO2jWhKof8qmfqgat9vt337FR1eH1N5paHt9v3lDwG63y3Yf7j3uptFahmnbKBvbbTpanrWU32YwPmeQbd58uPQ6nQ4KqZM0TK9O+IZl2yK6BwHdo0ClvhBJkn7GC8Z5qsQsml/yqLjJxLChp/ZUS6kzDfWmgr39vQDozf9m846ZorN0Sm5TyRYVqG2xqM6j6dF3BaCPZB9m2jKiOogh+SASxX44ypFYHA6HDHxNhr2NL48StRlGxFoohrkx2fa9AOxteGGAzsFWXC5XwPaZhzdYTc8FpkHYZwAAAABJRU5ErkJggg==";
		//var wi = WinImage.Find(w, image);
		//if(wi != null) { wi.MouseMove(); } else { Print("not found"); }

		//var w = +Wnd.Find("Quick Macros - ok - [Macro24]", "QM_Editor");
		//string image = "image:iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABwSURBVChTY/hPIsCpIbL/7X+T9DNQHgJg1QBSPPfg//9VG/7/1wpdAxWFAAwNMMUwDSA+siYUDeiKYRqQNcE14FMM8gsMo9gAEsClGMMGGABJ4lIMAhgaQACmEF0xCGDVAALYFIMATg24AIka/v8HABIU/64J7hqxAAAAAElFTkSuQmCC";
		//var all = new List<WinImage>();
		//WinImage.Find(w, image, also: t => { all.Add(t); return false; });
		//foreach(var wi in all) { wi.MouseMove(); 200.ms(); }

		//var w = +Wnd.Find("Quick Macros - ok - [Macro24]", "QM_Editor");
		//string image = @"C:\Users\G\Documents\func.png";
		//var all = new List<WinImage>();
		//WinImage.Find(w, image, also: t => { all.Add(t); return false; });
		//foreach(var wi in all) { wi.MouseMove(); 100.ms(); }

		var w = +Wnd.Find("Quick Macros - ok - [Macro24]", "QM_Editor");
		var all = new List<WinImage>();
		WinImage.Find(w, 0xFFFFC0, also: t => { all.Add(t); return false; });
		foreach(var wi in all) { wi.MouseMove(); 100.ms(); }


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
