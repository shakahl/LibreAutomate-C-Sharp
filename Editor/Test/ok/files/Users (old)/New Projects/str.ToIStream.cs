function! IStream&is

 Creates IStream from this variable.
 Returns 1 (success) or 0 (failure).


is=0
__GlobalMem hg=GlobalAlloc(GMEM_MOVEABLE this.len); if(!hg) ret
if(this.len)
	byte* b=GlobalLock(hg); if(!b) ret
	memcpy(b this this.len)
	GlobalUnlock(hg)
if(CreateStreamOnHGlobal(hg 1 &is)) ret
hg.handle=0
ret 1
