out
#compile "__FileVersionInfo"
FileVersionInfo x

Dir d
 foreach(d "$System$\*.exe" FE_Dir)
foreach(d "$pf$\*.exe" FE_Dir 8)
	str path=d.FileName(1)
	out path
	if(!x.Init(path)) out "<><c 0xff>no version-info</c>"; continue
	VS_FIXEDFILEINFO ffi
	if(x.GetFixed(ffi)) out F"File version: {ffi.dwFileVersionMS>>16}.{ffi.dwFileVersionMS&0xffff}.{ffi.dwFileVersionLS>>16}.{ffi.dwFileVersionLS&0xffff}"
	else out "<><c 0xff0000>no fixed</c>"
	str s
	if(x.GetString(s "FileDescription")) out F"File description: {s}"
	else out "<><c 0x8000>no string</c>"
