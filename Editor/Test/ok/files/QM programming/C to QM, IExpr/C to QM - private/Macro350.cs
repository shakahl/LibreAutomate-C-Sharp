out
str s="$program files$\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\user32.lib"

str ss
int n=LibGetFunctions(s ss)
if(n<1) ret
out n
out ss
