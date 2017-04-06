 /exe

 SetCurDir "c:\windows"

_s=
 using System;
 namespace MathUtils
 {
     class Calculator
     {
         static public int Add(int a, int b)
         {
             return a+b;
         }
     }
 }

_s.setfile("$my qm$\math.cs")

str code=
 //,css_searchdir q:\my qm
 //css_import math;
 using System;
 
 class Script
 {
     static public void Main()
     {
         Console.WriteLine(MathUtils.Calculator.Add(1,7));
     }
 }
code.setfile("$my qm$\main.cs")

PF
CsScript x.AddCode("$my qm$\main.cs")
PN
x.Call("Main")
PN
PO

 BEGIN PROJECT
 main_function  test CsScript directive import2
 exe_file  $my qm$\test CsScript directive import2.qmm
 flags  6
 guid  {0FDB4445-275A-4A05-9E3E-C2035E355590}
 END PROJECT
