/exe
out
str code=
 using System;
 public class Class1
 {
     static private void Main(string[] a)
     {
         int i;
         for(i=0; i<a.Length; i++) Console.Write(a[i]);
     }
     static public string StaticFunc(string s)
     {
         Console.Write(s);
         return "something";
     }
     public int Func(int a, int b)
     {
         return a+b;
     }
 }

 Use CsExec when just want to execute Main, like to run a program compiled from this code.
CsExec code "called Main"

 Use CsFunc when just want to call single static function once. Cannot call multiple functions and non-static functions. Calling multiple times is slow.
str R=CsFunc(code "called StaticFunc using CsFunc")
out R

 Use CsScript class in all other cases.
CsScript x.AddCode(code)
 call static function
str R2=x.Call("StaticFunc" "called StaticFunc using CsScript.Call")
 create object and call non-static function
IDispatch obj=x.CreateObject("Class1")
out obj.Func(2 3)


 BEGIN PROJECT
 main_function  Macro2000
 exe_file  $my qm$\Macro2000.qmm
 flags  6
 guid  {DA5D3A08-91AC-4FFC-9A88-C36D4FDBBD2D}
 END PROJECT
