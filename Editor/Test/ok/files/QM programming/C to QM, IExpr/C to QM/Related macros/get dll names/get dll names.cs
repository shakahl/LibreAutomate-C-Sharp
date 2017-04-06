out

str lib=
 $program files$\Microsoft SDKs\Windows\v7.0\Lib\*.lib
 $system$\msvcrt.dll
 $system$\msvcr71.dll
 $system$\ntdll.dll

 $system$\*.dll

str dmap=
 winspool.lib winspool.drv

 lib="$program files$\Microsoft SDKs\Windows\v7.0\Lib\user32.lib"

str s
CH_GetDllNames "C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\bin\dumpbin.exe" lib s 2 dmap
 out s
s.setfile("$qm$\winapiqmaz_fdn.txt")

 Found 519 matching files. Extracted 40196 functions from 446 files.
