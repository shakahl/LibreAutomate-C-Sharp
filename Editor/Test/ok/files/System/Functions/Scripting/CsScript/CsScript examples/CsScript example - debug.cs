 /exe 1

 Shows how to debug C# script in Visual Studio (not Express) or other .NET debugger when using CsScript class.

str code=
 using System;
 using System.Diagnostics;
 class Test{
 public static void Main(string[] a)
 {
 Debugger.Launch(); if(Debugger.IsAttached) Debugger.Break();
 //Debugger.Break();
 //Debug.Assert(a.Length>100);
 Debug.Write("A");
 //Debug.Write(a[5]); //error 
 Debug.Write("B");
 }
 }
str sf="$temp qm$\debug.cs"
code.setfile(sf)

CsScript x
x.SetOptions("debugConfig=true")
x.Exec(sf "1 0")
