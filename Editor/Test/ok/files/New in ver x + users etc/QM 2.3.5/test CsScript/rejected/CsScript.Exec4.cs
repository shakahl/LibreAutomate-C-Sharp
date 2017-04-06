function# $code [$cmdLine] [flags] ;;flags: 1 run in separate process, 0x200 console

 Compiles and executes C# script containing Main function.

 code - C# source code. Can be string, file or macro, like with <help>CsScript.AddCode</help>.
 cmdLine - command line arguments.

 REMARKS
 Calls Main function that is in the script. It should be like: static public void Main(string[] args)) { ... }. The return value can be void or int.
 This is the easiest to use function, and the most limited. Can be used when you just need to execute script like it would be an exe file.
 Executes in this process (does not create an exe file).


 if findrx(code "\bpublic\s[\s\w]+\sMain\b"

int R
if flags&1
	str sf=F"$temp$\CSSCRIPT\{_s.encrypt(2|8 code)}.exe"
	Compile(code sf flags&0x200)
	str cl=sf; if(!empty(cmdLine)) cl+" "; cl+cmdLine
	R=RunConsole2(cl)
	del- sf; err
else
	AddCode(code)
	
	if !empty(cmdLine)
		ARRAY(str) as
		ExeParseCommandLine cmdLine as
	ARRAY(BSTR) ab.create(as.len)
	for(_i 0 as.len) ab[_i]=as[_i]
	
	R=Call("*.Main" ab)
	err
		if(_hresult!0x80131600) end _error
		R=Call("*.Main")

ret R

err+ end _error
