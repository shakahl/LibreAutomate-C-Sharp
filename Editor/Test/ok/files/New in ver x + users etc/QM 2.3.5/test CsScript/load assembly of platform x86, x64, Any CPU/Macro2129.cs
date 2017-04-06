CsScript x
 x.SetOptions("searchDirs=$my qm$\test\ClassLibrary1\ClassLibrary1\bin\x86\Debug[]references=ClassLibrary1.dll")
x.SetOptions("searchDirs=$my qm$\test\ClassLibrary1\ClassLibrary1\bin\x64\Debug[]references=ClassLibrary1.dll")
 x.SetOptions("searchDirs=$my qm$\test\ClassLibrary1\ClassLibrary1\bin\Debug[]references=ClassLibrary1.dll")
x.Exec("")

 note: restart QM when switching platform, or may use the already loaded assembly instead

#ret
using System;
using ClassLibrary1;
public class A{static void Main(){Console.Write(ClassLibrary1.Class1.Test());}}
