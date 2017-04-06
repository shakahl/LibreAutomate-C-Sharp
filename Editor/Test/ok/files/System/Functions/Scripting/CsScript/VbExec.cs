 /
function# $code [$cmdLine]

 Compiles VB.NET script and calls its Main function.
 Returns: Main functions's return value (0 if void).

 code - the script.
   Can be string, file or macro (like "macro:MacroName"), like with <help>CsScript.AddCode</help>.
   If "", gets caller's text that follows #ret line (see example).
 cmdLine - command line arguments.

 REMARKS
 Executes in this process, does not create an exe file.
 The script must contain Main function or sub. It can optionally have single parameter of String[] type and return Integer.

 See also: <VbFunc>, <CsScript.Exec>, <CsScript help>.

 Errors: <.>

 EXAMPLE
 VbExec ""
 #ret
 Imports System
 Imports System.Windows.Forms
 Module Module1
 Sub Main()
  Console.WriteLine("VB")
  MessageBox.Show("VB")
 End Sub
 End Module


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

CsScript x.SetOptions("language=VB")
ret x.Exec(code cmdLine)
