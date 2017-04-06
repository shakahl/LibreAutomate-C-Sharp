/exe
 out

str code=
 using System;
 public class Test
 {
 public static int Add(int a, int b)
 {
 //System.IO.File.Delete("c>mmmmmmmmmmhhhhhhh;hhhhh");
 return Div(a, b);
 return a/b;
 //note: UTF-8
 //note: escape sequences
 //note: F-variables
 }
 public static int Div(int a, int b)
 {
 return a/b;
 }
 }

PF
CsScript x.Init
x.SetOptions("debugConfig=true")
 x.SetOptions("debugConfig=true[]compileFiles=$qm$\hello.cs")
PN
x.AddCode(code)
 x.AddCode("macro:cs1")
 x.AddCode("$qm$\hello.cs")
 x.AddCode("macro:test CsScript errors")
 x.AddCode("")
 x.AddCode("using System;")
 x.AddCode("" 1)
 x.AddCode("int Add(int a, int b)[]{ return a/b }" 1)
PN

rep 1
	_i=x.Call("Add" 10 0)
	 _i=x.Call("Add" 1000000000000 0)
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript errors
 exe_file  $my qm$\test CsScript errors.qmm
 flags  6
 guid  {5FBFA8B3-4643-4D6D-8563-DA79E5616C08}
 END PROJECT

 int Add(int a, int b)
 {
 //System.IO.File.Delete("c:\\mmmmmmmmmmhhhhhhh;hhhhh");
 return a/b
 return a/b
 }
#ret
 using System;
 public class Test
 {
 public static int Add(int a, int b)
 {
 //System.Diagnostics.Trace.Write("trace");
 //System.IO.File.Delete("c>mmmmmmmmmmhhhhhhh;hhhhh");
 return a/b
 return a/b
 }
 }
