out
out FindInDll("$system$\ntdll.dll" "The instruction at" 1)
ret

str s
 s="access violation"
 s="illegal instruction"
s="stack overflow"

Dir d
foreach(d "$System$\*.dll" FE_Dir)
	str sPath=d.FileName(1)
	if(FindInDll(sPath s)) out d.FileName
	

 dbgeng.dll
 ntdll.dll
 wow32.dll
