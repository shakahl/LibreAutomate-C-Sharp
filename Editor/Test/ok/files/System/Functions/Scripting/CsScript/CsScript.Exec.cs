function# $code [$cmdLine]

 Compiles and executes C# or VB.NET script. Calls its Main function.
 Returns: Main functions's return value (0 if void).

 code - source code. Can be string, file or macro, like with <help>CsScript.AddCode</help>.
 cmdLine - command line arguments.

 REMARKS
 The script must contain static Main function. It can optionally have single parameter of string[] type, and can return void or int.
 By default compiles as C#. For VB call SetOptions("language=VB") before; then cannot be classless function.

 See also: <CsExec>, <VbExec>.

 Errors: <.>


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

AddCode(code 0x10000)

ARRAY(str) a
if(empty(cmdLine)) a.create(0); else ExeParseCommandLine cmdLine a

ret Call("" a)
