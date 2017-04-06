function $code [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Compiles and executes C# script containing Main function.

 code - C# source code. Can be string, file or macro, like with <help>CsScript.AddCode</help>.
 a1-a10 - command line arguments to pass to Main.

 REMARKS
 Calls Main function that is in the script. It should be like: static public void Main(string[] args)) { ... }.
 This is the easiest to use function, and the most limited. Can be used when you just need to execute script like it would be an exe file.
 Executes in this process (does not create an exe file).


AddCode(code)

int i n=getopt(nargs)-1; VARIANT* p=&a1
ARRAY(BSTR) a.create(n)
for(i 0 n) a[i]=p[i]

Call("*.Main" a)

err+ end _error
