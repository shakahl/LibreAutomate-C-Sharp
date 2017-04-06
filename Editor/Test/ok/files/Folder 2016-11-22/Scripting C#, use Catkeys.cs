CsScript x.SetOptions("searchDirs=%catkeys%\tasks")
x.Exec("")
 CsExec("")


#ret
//C# code
using System;
using Catkeys;

class Test
{
static void Main()
{
   //POINT p;
//Wnd w;
	//Output.Write(Wnd.Find("*Quick*"));
	var m=new CatMenu();
	m["one"]=o=>Output.Write(o);
	m.Show();
}
}
