function! IStream&is

 Loads this variable from IStream.
 Returns 1 (success) or 0 (failure).


int hg n ok
ok=!GetHGlobalFromStream(is &hg)
if(ok)
	n=GlobalSize(hg)
	if(n)
		byte* b=GlobalLock(hg)
		if(!b) ok=0
		else
			this.all(n 2)
			memcpy(this b n)
			GlobalUnlock(hg)
if(!n or !ok) this.fix(0)
ret ok
