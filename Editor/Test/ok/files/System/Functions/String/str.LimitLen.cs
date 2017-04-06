function! nChar [flags] ;;flags: 1 add "...", 2 path

 Limits string length if it is longer than specified number of characters.
 Returns: 1 if limited, 0 if don't need to limit.

 nChar - max number of characters the string should have.
 flags:
   1 - if limits, replace last 3 characters with "...".
   2 - limit as file path. If limits, replaces part of string before filename with "...".

 REMARKS
 Unlike fix(), this function limits to a number of characters, not bytes.
 In Unicode mode, non-ASCII characters have more than 1 byte, therefore fix() often cannot be used.

 Added in: QM 2.3.3.


int n=nChar; if(n<0) end ERR_BADARG
BSTR b b0=this
if(b0.len<=n) ret

b.alloc(n)

if flags&2 and PathCompactPathExW(b b0 n+1 0)
else
	int e=(flags&1 and n>=4)
	if(e) n-3
	if(n and b0[n-1]=13) n-1
	memcpy b.pstr b0.pstr n*2
	if(e) memcpy(b.pstr+(n*2) L"..." 6); n+3

this.ansi(b -1 n)
ret 1
