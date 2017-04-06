 create variable that manages compiled C# code
CsScript x

 optionally set some options
 x.SetOptions("debugConfig=true[]references=System.Data;System.XML")

 compile code after #ret
x.AddCode("")

 call static function
int R=x.Call("Class1.TestFunction" "test" 5)
out R

 create object and call non-static functions
IDispatch obj=x.CreateObject("Class1")
out obj.Func1(3 4)
obj.Prop=100
out obj.Prop
out obj.Overload(10)
out obj.Overload_2("string")

 more examples
out "<><open ''CsScript example - create object''>More C# examples - interface, callback, delegate etc</open>"


#ret
//C# code
using System;

public class Class1
{
public static int TestFunction(string s, int i)
{
	Console.Write(s); Console.Write(i); //display in QM output
	return i*2;
}

public int Func1(int a, int b)
{
	return a+b;
}

public int Prop { set; get; }
public int Overload(int x) { return x; }
public int Overload(string x) { return x.Length; }
}
