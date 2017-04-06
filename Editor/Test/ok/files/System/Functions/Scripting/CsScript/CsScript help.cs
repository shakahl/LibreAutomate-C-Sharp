 Executes C# or VB.NET script. It is standard C# or VB.NET language code in a string or file.
 Compiles code, loads assembly, calls its functions and creates objects.
 Also can create new or use existing assemblies (.NET dll or exe files).

 REMARKS
 Requires .NET framework version 4.x or 3.5. Loads 4.x if installed, else 3.5. See also <help>RtOptions</help> net_clr_version.
 Windows 7 always has .NET 3.5. Windows 8 always has 4.0 or 4.5, and installs 3.5 if need. On older OS may need to download/install.
 Not supported on Windows older than XP SP2 or 2003 SP1.

 You can instead use global functions: <help>CsExec</help>, <help>CsFunc</help>, <help>VbExec</help>, <help>VbFunc</help>. Use this class when need more flexibility.

 In C#/VB code you can use Console.Write to send text to QM output.
 To enable Trace.Write and Debug.Write, call <help>CsScript.SetOptions</help> with debugConfig=true or compilerOptions=/d:TRACE;DEBUG.

 Compiling C#/VB code is quite slow. It invokes csc.exe or vbc.exe (.NET compilers).
 To avoid compiling each time, the class uses caching. Uses 2-level cache: in memory (as loaded assembly) and as assembly dll file in "$temp qm$\CsScript folder (optional). If the code is already compiled (with same options) and/or loaded, uses the cache file or the loaded assembly. Else compiles the code and creates a file in the cache folder. Before loading cache file also checks whether other used files have been modified; however doesn't if the assembly is already loaded.
 Other factors that may significantly slow down assembly loading: Referenced private (non-GAC) assemblies. Antivirus program.
 Once compiled, the code runs at the same speed as a .NET program. JIT-compiling adds some delays, but then the code is almost as fast as compiled C++ code. Calling C#/VB functions from QM is significantly slower than from C#, unless you <help "CsScript example - use interface">use interfaces</help>.

 Loaded assemblies cannot be unloaded until current process ends.
 It may cause memory leaks while developing/debugging the script, because each version of script code/options creates and loads new assembly. You can run the macro in separate process to avoid memory leaks in QM process.
 In most cases, loaded assemblies cannot use each other. To use other assembly in a script, you can create assembly file with <help>CsScript.Compile</help> and make sure that it will be found (see <help>CsScript.SetOptions</help>).
 Creates assemblies for x86 platform. Can use assemblies created for x86 and "Any CPU" platforms. Cannot use x64 assemblies.

 Avoid using quoted strings for C#/VB code because of QM escape sequences.
 For example, in "static void Main(string[] a){var v=a[2];}" the [] is replaced with new line and the [2] with byte 2.
 Instead use multiline string as comments, like in the examples. Or place code in a file or macro. You can place code in same macro after #ret line.

 To disable compiler warnings:
 Call SetOptions with compilerOptions=/nowarn or compilerOptions=/warn:0. The number is warning level, and can be 0 to 4 (default). Reference: <google>csc C# compiler options</google>.
 Or use #pragma warning in script. Reference: <google>C# #pragma warning</google>.

 The class can be used in exe too.
 When compiling script, creates files in the temporary folder. You can change temporary folder location with <help>CsScript.SetOptions</help>.
 To avoid compiling script and creating temporary files:
   On your computer: Create assembly file with <help>CsScript.Compile</help>. Add it to exe, or distribute as separate file together with exe.
   At run time: Load the file with <help>CsScript.Load</help>. Don't use AddCode, Compile, Exec, CsExec, CsFunc, VbExec, VbFunc, don't use C# source code.

 EXAMPLES. See also <open>CsScript examples</open>.
 __________________________________________________

 Use CsExec to execute script containing function Main, like to run program compiled from this code.
str code=
 using System;
 public class Class1
 {
     static void Main(string[] a)
     {
         int i;
         for(i=0; i<a.Length; i++) Console.Write(a[i]);
     }
 }
CsExec code "called Main"

 __________________________________________________

 Use CsFunc to call single static function.
str code2=
 using System;
 public class Class1
 {
     public static string StaticFunc(string s)
     {
         Console.Write(s);
         return "something";
     }
 }
str R=CsFunc(code2 "called StaticFunc using CsFunc")
out R

 __________________________________________________

 Use CsScript class when want to call multiple functions, create objects, set options, call functions faster, create or load .dll or .exe files.
str code3=
 using System;
 public class Class1
 {
     public static string StaticFunc(string s)
     {
         Console.Write(s);
         return "something";
     }
     public int Func1(int a, int b)
     {
         return a+b;
     }
     public string Func2(string a, string b)
     {
         return a+b;
     }
 }
CsScript x.AddCode(code3)
 call static function
str R2=x.Call("Class1.StaticFunc" "called StaticFunc using CsScript.Call")
 create object and call non-static function
IDispatch obj=x.CreateObject("Class1")
out obj.Func1(2 3)
out obj.Func2("2" "3")

 __________________________________________________

 To call functions much faster, need to define interface. Click the "CsScript examples" link to see how. It's not possible for static functions.
 However usually the slowest part is compiling and loading. Caching is used to make it faster. Run this macro several times to see how it works.
