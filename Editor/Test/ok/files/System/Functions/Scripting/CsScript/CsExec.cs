 /
function# $code [$cmdLine]

 Compiles C# script and calls its Main function.
 Returns: Main functions's return value (0 if void).

 code - the script.
   Can be string, file or macro (like "macro:MacroName"), like with <help>CsScript.AddCode</help>.
   If "", gets caller's text that follows #ret line (see example).
 cmdLine - command line arguments.

 REMARKS
 Executes in this process, does not create an exe file.
 The script must contain static Main function. It can optionally have single parameter of string[] type, and can return void or int.

 See also: <CsScript.Exec>, <CsFunc>, <CsScript help>.

 Errors: <.>

 EXAMPLE
 //QM code
 str variable="a b c"
 int R=CsExec("" F"/test ''{variable}''")
 out R
 #ret
 //C# code
 using System;
 using System.Windows.Forms;
 class Test
 {
 	static int Main(string[] p)
 	{
 		int i; for(i=0; i<p.Length; i++) Console.WriteLine(p[i]); //display in QM output
 		MessageBox.Show("Passed " + p.Length + " arguments.", "CsExec");
 		return 100;
 	}
 }


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

CsScript x
ret x.Exec(code cmdLine)
