 /exe
 out

str code=
 using System;
 using System.Diagnostics;
 class Test{
 static public void Main(string[] a)
 {
    Debugger.Launch(); if(Debugger.IsAttached) Debugger.Break();
 //Debugger.Break();
 //Debug.Assert(a.Length>100);
 Console.Write("A");
 //Console.Write(a[5]); //error 
 Console.Write("B");
 }
 }

 #define TRACE
 #define DEBUG
 code=
 using System;
 using System.Diagnostics;
 class Test{
 static public void Main(string[] a)
 {
 //csscript.Host.RedirectDebugOutput(); //error
 //Trace.Listeners.Add(new ConsoleTraceListener());
                Console.Write("console");
                Trace.Write("trace");
                Debug.Write("debug");
 }
 }

str sf="$temp$\debug.cs"
code.setfile(sf)

CsScript x
 x.SetOptions("debugConfig=true")
 x.SetOptions("compilerOptions=/d:TRACE;DEBUG")
 x.SetOptions("debugConfig=true[]inMemoryAsm=")
x.SetOptions("debugConfig=true[]noFileCache=true")
 x.Exec(code "1 0")
x.Exec(sf "1 0")

 BEGIN PROJECT
 main_function  test CsScript debug
 exe_file  $my qm$\test CsScript debug.qmm
 flags  6
 guid  {DE80EDD7-111E-4D4A-8221-1CC373127DBA}
 END PROJECT

 info:
 To break on errors, VS must be set to break on first-chance exceptions (when thrown), because QM handles exceptions. VS menu -> Debug -> Exceptions -> CLR -> check all except System.Reflection.TargetInvocationException.
