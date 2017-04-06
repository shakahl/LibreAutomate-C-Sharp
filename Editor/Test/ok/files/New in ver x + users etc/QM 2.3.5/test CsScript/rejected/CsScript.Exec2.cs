function $code [BSTR'a1] [BSTR'a2] [BSTR'a3] [BSTR'a4] [BSTR'a5] [BSTR'a6] [BSTR'a7] [BSTR'a8] [BSTR'a9] [BSTR'a10]

 Compiles and executes C# script.

 code - C# source code.
 a1-a10 - command line arguments to pass to Main.

 REMARKS
 Calls Main function that is in the script. It should be like: static public void Main(string[] args)) { ... }.
 This is the easiest to use function, and the most limited. Can be used when you just need to execute script like it would be an exe file.
 Assembly caching is the same as with AddCode.


Init

int vt=VT_BSTR
SAFEARRAY sa.cbElements=4; sa.cDims=1
sa.fFeatures=FADF_BSTR|FADF_HAVEVARTYPE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
x.Exec(code _s.encrypt(2|8 code) &sa)

err+ end _error
