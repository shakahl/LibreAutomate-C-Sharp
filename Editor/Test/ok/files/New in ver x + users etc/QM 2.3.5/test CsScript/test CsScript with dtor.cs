/exe
str code=
 using System;
 public class Ak
 {
 //~Ak() { int i=5+2; }
 ~Ak() { Console.Write("dtor"); }
 public int B() { Console.Write("B"); return 5; }
 }
CsScript x.AddCode(code)
 IDispatch d=x.CreateObject("A")
IDispatch d=x.CreateObject("Ak")
out d.B
out x.x.CreateObject("Ak").B

 BEGIN PROJECT
 main_function  test CsScript with dtor
 exe_file  $my qm$\Macro1447.qmm
 flags  6
 guid  {9883FDF7-7881-4D29-A220-0450242A9B32}
 END PROJECT
