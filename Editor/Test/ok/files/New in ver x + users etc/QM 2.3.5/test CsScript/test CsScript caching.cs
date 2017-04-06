 /exe
 out

str code=
 using System;
 class Test{
 static public void Main(string[] a)
 {
 Console.Write("B");
 }
 }

CsScript x.Exec(code "1 0")

 BEGIN PROJECT
 main_function  test CsScript caching
 exe_file  $my qm$\test CsScript caching.qmm
 flags  6
 guid  {7FC479C3-81C5-4B45-A218-265285D37D00}
 END PROJECT
