 EXAMPLE
CsScript x.AddCode("") ;;if "", gets text that follows #ret line in this macro
IDispatch o=x.CreateObject("Test")
int M=o.Add(2 3)
o.Prop=100
int P=o.Prop
out M
out P
out o.Overload(10)
out o.Overload_2("string")
int v; o.PassByRef(&v); out v
str s; BSTR _b=s; o.PassByRefString(&_b); s=_b; out s

#ret
//C# code
using System;
public class Test
{
	public int Add(int a, int b) { return a+b; }
	public int Prop { set; get; }
	public int Overload(int x) { return x; }
	public int Overload(string x) { return x.Length; }
	public void PassByRef(ref int x) { x=5000; }
	public void PassByRefString(ref string x) { x="TEST"; }
}
