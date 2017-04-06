
 There are 3 ways of calling C# functions:
 1. Static functions are called using Call. It's easiest. Internally is used .NET Reflection. It's quite slow, about 10 microseconds.
 2. Non-static functions can be called using an IDispatch variable. Usually it's easy too. In both cases (1 and 2), arguments and return values are wrapped in VARIANT (object in C#); type/format usually is converted automatically. The speed is similar.
 3. Non-static functions can be called using a defined COM interface. It's 10-30 times faster. Also it allows to pass arguments of types not supported by VARIANT. However requires more code and knowledge. Define COM interface for your C# class. Do it in C# and in QM. In QM need to declare correct argument types (BSTR for string, VARIANT for object, etc). In C# may need to use MarshalAs attribute. It is documented in MSDN (.NET COM interop) and elsewhere on the internet.
 In any case, the speed also depends on the number and type of parameters and the return value, because need some time to convert string format etc.

 EXAMPLE
//QM code
interface# IFast :IDispatch
	#Add(a b)
	[p]Prop(x)
	[g]#Prop
	#Overload(x)
	#Overload_2(BSTR's)
	{C7D9B459-0108-4F4A-9483-A7F60F18AF0A}
 note: the interface must have a GUID. To create a unique GUID: _s.Guid; out _s

PerfFirst ;;just to measure times
CsScript x.AddCode("")
PerfNext
IFast o=x.CreateObject("Examples.Fast")
PerfNext
rep 1000
	int R=o.Add(2 3)
	 int R=o.Overload(5)
	 int R=o.Overload_2("test")
PerfNext
o.Prop=100
int P=o.Prop
PerfNext
PerfOut
out R
out P

#ret
//C# code
using System;
using System.Runtime.InteropServices;
namespace Examples
{
[Guid("C7D9B459-0108-4F4A-9483-A7F60F18AF0A")]
public interface IFast
{
	int Add(int a, int b);
	int Prop { set; get; }
	int Overload(int x);
	int Overload(string x);
}
public class Fast : IFast
{
	public int Add(int a, int b) { return a+b; }
	public int Prop { set; get; }
	public int Overload(int x) { return x; }
	public int Overload(string x) { return x.Length; }
}
}
