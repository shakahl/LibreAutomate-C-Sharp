/exe
 out

str code=
 using System;
 
 public class Test
 {
 //public static int Add(int a, int b) { Console.Write("test"); return a+b; }
 public static int Add(int a, int b) { return a+b; }
 public static int Add(int a) { return a; }
 }
 
 namespace NS
 {
 public class Test
 {
 //public static int Add(int a, int b) { Console.Write("test2"); return a+b; }
 public static int Add(int a, int b) { return a+b+100; }
 }
 }
 
 namespace NS1 { namespace NS2
 {
 public class Test
 {
 //public static int Add(int a, int b) { Console.Write("test3"); return a+b; }
 public static int Add(int a, int b) { return a+b+1000; }
 }
 }}

PF
CsScript x.Init
 x.SetOptions("debugConfig=true")
PN
x.AddCode(code 0)
PN

rep 1
	_i=x.Call("Add" 10 5)
	 _i=x.Call("*" 10 5)
	 _i=x.Call("*" 10)
	 _i=x.Call("Test.Add" 10 5)
	 _i=x.Call("NS.Test.Add" 10 5)
	 _i=x.Call("NS1.NS2.Test.Add" 10 5)
	 _i=x.Call("Test.Add" 10 5)
	 _i=x.Call("Test.*" 10 5)
	 _i=x.Call("Test.*" 10)
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript Call namespace
 exe_file  $my qm$\test CsScript Call namespace.qmm
 flags  6
 guid  {84BBA381-2C2E-49DE-9E39-C8A49478EDD9}
 END PROJECT
