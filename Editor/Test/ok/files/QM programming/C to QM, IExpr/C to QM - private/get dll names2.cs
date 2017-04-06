out

str lib=
 $program files$\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK\Lib\*.lib
 $program files$\SDK\Lib\*.lib
 $system$\msvcrt.dll
 $system$\msvcr71.dll

 $system$\*.dll


str dmap=
 winspool.lib winspool.drv

str s
CH_GetDllNames "C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\bin\dumpbin.exe" lib s 0 dmap
 out s
s.setfile("$qm$\winapiqmaz_fdn.txt")

str ss
CH_MustBeA s ss
ss.setfile("$qm$\winapiqmaz_funca.txt")
