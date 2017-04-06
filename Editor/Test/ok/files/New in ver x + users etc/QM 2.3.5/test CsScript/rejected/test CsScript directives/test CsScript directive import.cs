/exe

 Problem with memory caching: ignores changes in imported files. Should use MD5 of all. Now, if locked in one process, other process fails to create.

 SetCurDir "c:\windows"

_s="using System; namespace MathUtils { class Calculator { static public int Add(int a, int b) { return a+b; } } }"
 _s.setfile("q:\my qm\math.cs")
_s="using System; using System.Windows.Forms; namespace WinForms { class Test { static public void Ms(string s) { MessageBox.Show(s); } } }"
 _s.setfile("q:\my qm\math2.cs")

 _s.setfile("$qm$\math.cs")

str code=
 using System;
 
 class Script
 {
     static public void Main()
     {
         Console.WriteLine(MathUtils.Calculator.Add(1,5));       //bb          f
         //WinForms.Test.Ms("test");
     }
 }

 usingNamespaces=
 references=System.Windows.Forms;
 noFileCache=true
str options=
 searchDirs=$my qm$
 compileFiles=math.cs; math2.cs
 references=System.Core; System.Data.Linq ;

PF
CsScript x
x.SetOptions(options)
x.AddCode(code)
PN
x.Call("Main")
 PN
PO

 BEGIN PROJECT
 main_function  test CsScript directive import
 exe_file  $my qm$\test CsScript directive import.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {F2CE4F3B-AC87-45C6-B435-17B982E0CDA3}
 END PROJECT
