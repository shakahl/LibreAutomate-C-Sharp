function# [!*mem] [memSize]

 Creates stream in memory and optionally copies data to it.
 Returns memory handle. Allocates it with GlobalAlloc. Also you can use function GetHGlobalFromStream to get the handle.


is=0
int hg; byte* b; int hr

hg=GlobalAlloc(GMEM_MOVEABLE memSize)
if(!hg) end "" 16 E_OUTOFMEMORY

if(memSize and mem)
	b=GlobalLock(hg)
	if(b) memcpy(b mem memSize); GlobalUnlock(hg)
	else hr=E_OUTOFMEMORY

if(!hr) hr=CreateStreamOnHGlobal(hg 1 &is)

if(hr)
	GlobalFree(hg)
	end "" 16 hr

ret hg
