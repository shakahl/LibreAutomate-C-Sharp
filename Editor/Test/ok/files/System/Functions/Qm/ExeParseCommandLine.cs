 /
function $cmdLine ARRAY(str)&a

 Parses command line.
 Uses the same rules as most programs. <google "site:microsoft.com Parsing C++ Command-Line Arguments">Read more</google>.
 This function can be used in exe or not.

 cmdLine - command line.
   Can be path followed by command line. Then a[0] will be the path. If path contains spaces, must be enclosed in ".
 a - receives command line arguments.

 Added in: QM 2.3.3.

 EXAMPLE
 ARRAY(str) a; int i
 ExeParseCommandLine _command a
 for i 0 a.len
	 out a[i]
	  int k=val(a[i]) ;;convert to number, if need


a=0
if(empty(cmdLine)) ret
int i n
word** w=CommandLineToArgvW(@cmdLine &n)
if(!w) ret
for(i 0 n) a[].ansi(w[i])
LocalFree w
