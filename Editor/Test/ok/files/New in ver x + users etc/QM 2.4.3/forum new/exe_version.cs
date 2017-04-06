 /exe
#compile "__FileVersionInfo"
FileVersionInfo x
 load file
if(!x.Init(ExeFullPath)) end "no version info"
 get fixed part
VS_FIXEDFILEINFO ffi
if(x.GetFixed(ffi)) out F"File version: {ffi.dwFileVersionMS>>16}.{ffi.dwFileVersionMS&0xffff}.{ffi.dwFileVersionLS>>16}.{ffi.dwFileVersionLS&0xffff}"

 BEGIN PROJECT
 exe_file  $my qm$\exe_version.exe
 version  0.0.0.2
 flags  6
 END PROJECT
