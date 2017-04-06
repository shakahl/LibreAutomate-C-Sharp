 /
function! str&s

 Gets directory of latest .NET framework runtime installed on this computer.
 Returns: 1 success, 0 failed (computer without .NET).
 It is where .NET tools are installed, eg regasm.exe.

 Added in: QM 2.3.5.


ARRAY(str) a
GetFilesInFolder a "C:\Windows\Microsoft.NET\Framework" "^v\d+\." 0x10001
a.sort(8|1)
int i
for i 0 a.len
	if(FileExists(F"{a[i]}\regasm.exe")) s=a[i]; ret 1

 GetCORSystemDirectory does not see v4 etc, and not very faster, first time much slower (loads mscoree.dll)
