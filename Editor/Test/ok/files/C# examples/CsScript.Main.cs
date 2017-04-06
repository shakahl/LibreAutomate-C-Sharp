function# [$cmdLine]

 Executes C# or VB.NET script. Calls its Main function.
 Returns: Main functions's return value (0 if void).

 cmdLine - command line arguments.

 REMARKS
 Code must be added with AddCode.
 The script must contain static Main function. It can optionally have single parameter of string[] type, and can return void or int.
 By default compiles as C#. For VB call SetOptions("language=VB") before; then cannot be classless function.

 Errors: <.>


opt noerrorshere 1
opt nowarningshere 1

ARRAY(str) a
if(empty(cmdLine)) a.create(0); else ExeParseCommandLine cmdLine a

ret Call("" a)
