 /exe
 out

str code1=
 using System;
 namespace Nam1{
 class Class1
 {
 static public void Func1()
 {
 Console.Write("B");
 }
 }
 }

str code2=
 using System;
 using Nam1;
 class Test{
 static public void Main(string[] a)
 {
 Class1.Func1();
 }
 }

CsScript x
x.AddCode(code1)

x.Exec(code2)

 BEGIN PROJECT
 main_function  test CsScript use other assembly
 exe_file  $my qm$\test CsScript use other assembly.qmm
 flags  6
 guid  {49F17203-03A2-4BD0-803A-959A38914E12}
 END PROJECT
