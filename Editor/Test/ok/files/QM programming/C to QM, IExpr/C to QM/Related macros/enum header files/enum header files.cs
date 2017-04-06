out
 str f1="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Include\*.h"
str f1="C:\" ;;use this to display all
str f2="$program files$\Microsoft SDKs\Windows\v7.0\Include\*.h"

 CompareFolders f1 f2 2 "#include ''%s''"
CompareFolders f1 f2 0 "#include <%s>"
