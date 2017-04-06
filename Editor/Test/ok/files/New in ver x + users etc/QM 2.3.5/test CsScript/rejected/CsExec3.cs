function# $code [$cmdLine]

 Compiles and executes C# script containing Main function.
 Returns: Main functions's return value (0 if void).

 code - C# source code. Can be string, file or macro (like "macro:MacroName"), like with <help>CsScript.AddCode</help>.
 cmdLine - command line arguments.

 REMARKS
 Calls Main function that is in the script. It should be like:
   static public void Main(string[] args)) { ... }
   static public int Main(string[] args)) { ... }
   static public void Main() { ... }
   static public int Main() { ... }
 Executes in this process, does not create an exe file.

 See also: <CsScript help>.

 EXAMPLE
 str code=
  using System;
  using System.Windows.Forms; //for MessageBox
  public class Test
  {
    static public int Main(string[] p)
    {
       int i; for(i=0; i<p.Length; i++) Console.WriteLine(p[i]); //display in QM output
       MessageBox.Show("Passed " + p.Length + " arguments.", "CsExec");
       return 100;
    }
  }
 str variable="a b c"
 int R=CsExec(code F"/test ''{variable}''")
 out R


CsScript x.AddCode(code)

ARRAY(str) as
if(!empty(cmdLine)) ExeParseCommandLine cmdLine as
ARRAY(BSTR) ab.create(as.len)
for(_i 0 as.len) ab[_i]=as[_i]

int R=x.Call("*.Main" ab)
err
	if(_hresult!0x80131600) end _error
	R=x.Call("*.Main") ;;support Main() without string[] args
	err
		if(_hresult!0x80131600) end _error
		end F"{ERR_BADARG}. The script must contain Main function. It must be static public."

ret R

err+ end _error

 info: could add flag to run in separate process (create exe), but then problems with antivirus. If need, try rundll32...
