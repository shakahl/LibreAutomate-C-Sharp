 Gets a string or/and fixed part from version-info resource of an exe or dll file.

 EXAMPLES

#compile "__FileVersionInfo"
FileVersionInfo x
 load file
if(!x.Init("$system$\notepad.exe")) end "file not found or does not contain version info"
 get a string
str s
if(x.GetString(s "FileDescription")) out F"File description: {s}"
if(x.GetString(s "CompanyName")) out F"Company name: {s}"
 get fixed part
VS_FIXEDFILEINFO ffi
if(x.GetFixed(ffi)) out F"File version: {ffi.dwFileVersionMS>>16}.{ffi.dwFileVersionMS&0xffff}.{ffi.dwFileVersionLS>>16}.{ffi.dwFileVersionLS&0xffff}"
