/exe
out

str lib=
 $system$\*.dll

str s sErrors
 CH_GetDllNames "C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\dumpbin.exe" lib s 0

GetDllFunctionNames lib s sErrors 7|0x300
out s
 if(sErrors.len) out "[]ERRORS:[]%s" sErrors
 s.setfile("$qm$\winapiqmaz_fdn.txt")

 str ss
 CH_MustBeA s ss
 ss.setfile("$qm$\winapiqmaz_funca.txt")
