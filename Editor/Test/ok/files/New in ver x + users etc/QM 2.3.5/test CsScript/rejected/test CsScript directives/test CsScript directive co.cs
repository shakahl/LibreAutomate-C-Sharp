 /exe

str code=
 //css_co /d:TRACE
 using System;
 using System.Diagnostics;
 
 class Script
 {
     static public void Main()
     {
        Trace.WriteLine(5);
     }
 }

PF
CsScript x
 x.SetOptions("debugConfig=true")
 x.SetOptions("debugConfig=")
x.AddCode(code)
PN
x.Call("Main")
PN
PO


 BEGIN PROJECT
 main_function  test CsScript directive co
 exe_file  $my qm$\test CsScript directive co.qmm
 flags  6
 guid  {0C10260C-32FA-43FE-8C39-C508A35F0CDA}
 END PROJECT
