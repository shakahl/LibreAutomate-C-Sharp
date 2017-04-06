 /exe
 out

str code=
 using System;
 
 public class Test
 {
 public static int Add(int a, int b) { return a+b; }
 public static int Prop { get; set; }
 }

PF
CsScript x.Init
 x.SetOptions("debugConfig=true")
PN
x.AddCode(code)
PN

rep 7
	_i=x.Call("Add" 10 5)
	x.Call("set_Prop" 10)
	_i=x.Call("get_Prop")
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript Call prop
 exe_file  $my qm$\test CsScript Call prop.qmm
 flags  6
 guid  {6A328EE0-00ED-40DD-8473-6F634E4A384D}
 END PROJECT
