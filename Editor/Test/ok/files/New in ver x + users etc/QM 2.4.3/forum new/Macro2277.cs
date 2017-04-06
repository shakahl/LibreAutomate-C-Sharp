str cl="a b c"
ARRAY(str) a
sub.ParseCommandLine(cl a)

int i
for(i 0 a.len) out a[i]

#sub ParseCommandLine
function $cmdLine ARRAY(str)&a

 Parses command line.
 Uses the same rules as most programs. <link "http://www.google.com/search?q=site%3amicrosoft.com%20Parsing%20C%2b%2b%20Command-Line%20Arguments">Read more</link>.

 cmdLine - command line.
   Can be path followed by command line. Then a[0] will be the path.
 a - receives command line arguments.


a=0
if(empty(cmdLine)) ret
int i n
word** w=CommandLineToArgvW(@cmdLine &n)
for(i 0 n) a[].ansi(w[i])

LocalFree w
