function $code [flags] ;;flags: 1 classless function

 Compiles C# or VB.NET code into assembly and loads the assembly.
 Then you can use <help>CsScript.Call</help> to call static functions, <help>CsScript.CreateObject</help> to call non-static functions.

 code - source code. Can be:
   String variable.
   File. Examples: "c:\file.cs", "$my qm$\file.cs". Does not support exe resource id; instead add compiled assembly and use <help>CsScript.Load</help>.
   Macro, like "macro:MacroName". Gets whole code, or code that follows #ret line. Ignores first line if it looks like macro options ( /...).
   If "", gets caller's text that follows #ret line.
   Tip: It is easier to debug the script if it is in macro or file, because then adds links to jump to compiler errors and warnings.
 flags:
   1 - classless function. Code contains a single function without a class. Can be preceded by 'using' lines (don't need 'using System;') and comments. May be without 'public' and/or 'static'. See examples.
   0x10000 - autodetect classless function. Simply searches for word "class" in whole script (including comments, strings, etc), therefore may not detect in some cases.

 REMARKS
 Does not append code to a previously used assembly. Uses separate assembly for each code/options version.
 By default compiles as C#. For VB call SetOptions("language=VB") before; then cannot be classless function, and flags 1 and 0x10000 are ignored.

 Errors: <.>

 See also: <CsFunc>, <VbFunc>

 EXAMPLES
 str cs=
  using System;
  public class Class1
  {
    public static void Func(string s) { Console.Write(s); }
  }
 CsScript x.AddCode(cs) ;;use code in a variable
 x.Call("Func" "test")

 CsScript x2.AddCode("$my qm$\file.cs") ;;get code from a file

 CsScript x3.AddCode("macro:MacroName") ;;get code from a macro

 CsScript x4.AddCode("") ;;get code from this macro
 x4.Call("Func" "test")
 #ret
 //C# code
 using System;
 public class Class1
 {
 	public static void Func(string s) { Console.Write(s); }
 }


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

Init

str ss sourceFile
code=_GetCode(code ss sourceFile flags)
x.AddCode(code flags sourceFile)
