function# ^waitmax hwnd [flags] ;;flags: 1 client

if(waitmax<0 or waitmax>2000000) end ERR_BADARG
if(!m_a.len) end ERR_INIT
opt waitmsg -1

int wt(waitmax*1000) t1(GetTickCount) i c
rep
	0.1
	
	if(!IsWindow(hwnd)) end "invalid window handle"
	RECT r
	if(flags&1) GetClientRect hwnd &r; MapWindowPoints hwnd 0 +&r 2
	else GetWindowRect hwnd &r
	
	ARRAY(int) a
	if(!GetRectPixels(r a)) end ES_FAILED
	for i 0 m_a.len
		WFPDATA& d=m_a[i]
		c=ColorARGBtoBGR(a[d.x d.y]); err continue
		if(c=d.c) ret i
	
	if(wt>0 and GetTickCount-t1>=wt) end "wait timeout"

