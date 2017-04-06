out
str db.dospath("C:\Documents and Settings\G\Desktop\LibDump.EXE")
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\User32.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\shell32.Lib"
str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\shlwapi.Lib"
 str lib="C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\winspool.Lib"
 str lib="C:\windows\system32\User32.dll"
 str lib="C:\windows\system32\shell32.dll"
lib.dospath

run db lib
ret

str s.format("%s %s" db lib)

Wsh.WshShell objShell._create
Wsh.WshExec objScriptExec = objShell.Exec(s)
s=objScriptExec.StdOut.ReadAll
out s
out s.len
ret

str ss
if(lib.endi(".lib"))
	if(findrx(s "(?s)Exports\s+ordinal\s+name[][](.+?)[][]" 0 0 ss 1)<0) ret
	 out ss
	ss.replacerx("^.{18}_?(\w+).*$" "$1" 8)
else ;;dll
	if(findrx(s "(?s)ordinal\s+hint\s+RVA\s+name[][](.+?[])[]" 0 0 ss 1)<0) ret
	 out ss
	ss.replacerx("^.{26}(\w+).*" "$1" 8)
	ss.replacerx("^.{26}\[NONAME\].*[]" "" 8)
out ss
out numlines(ss)
