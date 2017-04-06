out
str db="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\bin\dumpbin.exe"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\User32.Lib"
str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\kernel32.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\shellapi.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\msi.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\shell32.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\urlmon.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\shlwapi.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\winspool.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\Lib\msvcrt.Lib"
 str lib="C:\windows\system32\User32.dll"
 str lib="C:\windows\system32\shell32.dll"
 str lib="C:\windows\system32\msvcp50.dll"
 lib.dospath

 str s.format("''%s'' /EXPORTS ''%s''" db lib)
 str s.format("''%s'' /ALL ''%s''" db lib)
 str s.format("''%s'' /ARCHIVEMEMBERS ''%s''" db lib)
 str s.format("''%s'' /HEADERS ''%s''" db lib)
str s.format("''%s'' /HEADERS /EXPORTS ''%s''" db lib)

Wsh.WshShell objShell._create
Wsh.WshExec objExec = objShell.Exec(s)
s=objExec.StdOut.ReadAll
 out s
 out s.len
 ret
str ss
if(lib.endi(".lib"))
	if(findrx(s "(?s)Exports\s+ordinal\s+name[][](.+?[])[]" 0 0 ss 1)<0) ret
	 out ss
	ss.replacerx("^.{18}[_\?]?([A-Z_]\w*)(?:@\d+|@@\S+|)(?: .*)?(?=[])" "$1" 9)
else ;;dll
	if(findrx(s "(?s)ordinal\s+hint\s+RVA\s+name[][](.+?[])[]" 0 0 ss 1)<0) ret
	 out ss
	ss.replacerx("^.{26}(\??[A-Z_]\w*(?:@\d+|@@\S+|))(?: .*)?(?=[])" "$1" 8)
	ss.replacerx("^.{26}\[NONAME\].*[]" "" 8)
out ss
 out numlines(ss)
