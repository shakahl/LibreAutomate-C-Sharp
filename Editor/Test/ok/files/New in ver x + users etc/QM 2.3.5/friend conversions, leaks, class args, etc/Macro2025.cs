str code=
 using System;
 public class Class1
 {
 public void Met(string[] a) { Console.Write(a[0]); }
 public void MetS(string a) { Console.Write(a); }
 public static void St(string x) { Console.Write(x); }
 }

CsScript x.AddCode(code)

IDispatch d=x.CreateObject("Class1")
ARRAY(BSTR) a="one[]two"
d.Met(a)
BSTR b="test"
d.MetS(b)

ARRAY(VARIANT) aa.create(1); aa[0]="test2"
IDispatch y=x.x
y.Call("St" aa)
