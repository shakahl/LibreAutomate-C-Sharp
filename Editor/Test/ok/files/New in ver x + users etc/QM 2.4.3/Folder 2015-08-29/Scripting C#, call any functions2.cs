CsScript x
x.AddCode("")
int R=x.Call("Class1.TestFunction" "test" 0)
out R

IDispatch obj=x.CreateObject("Class1")
out obj.Func1(3 4)
obj.Prop=100
out obj.Prop
out obj.Overload(10)
out obj.Overload_2("string")


#ret
//C# code
using System;

public class Class1
{
static public int TestFunction(string s, int i)
{
//throw new Exception();
return i/i;
	Console.Write(s); Console.Write(i); //display in QM output
	return i*2;
}

public int Func1(int a, int b)
{
Console.Write("Func1");
//throw new Exception();
	return a+b;
}

public int Prop { set; get; }
public int Overload(int x) { return x; }
public int Overload(string x) { return x.Length; }
}
