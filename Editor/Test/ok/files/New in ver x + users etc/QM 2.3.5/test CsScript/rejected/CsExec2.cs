function# $code [$cmdLine]

 Compiles and executes C# script containing Main function.
 Returns: Main functions's return value (0 if void).

 code - C# source code. Can be string, file or macro, like with <help>CsScript.AddCode</help>.
 cmdLine - command line arguments.

 REMARKS
 Calls Main function that is in the script. It should be like:
   static public void Main(string[] args)) { ... }
   static public int Main(string[] args)) { ... }
   static public void Main() { ... }
   static public int Main() { ... }
 Executes in this process, does not create an exe file.

 See also: <CsScript help>.


CsScript x.Exec(code cmdLine)
