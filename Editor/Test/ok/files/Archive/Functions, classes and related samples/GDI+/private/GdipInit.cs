 /
function#

 Initializes GDI+ (calls GdiplusStartup).
 Returns: 1 success, 0 failed.
 Fails on Windows 2000 where gdiplus.dll unavailable.
 Automatically uninitializes when process exits (calls GdiplusShutdown).
 Can be called multiple times. If already initialized, just returns GDI+ token.
 Don't need to call this function when using GdipX classes.

 EXAMPLE
 if(!GdipInit) ret


#if QMVER>=0x02040101
ret InitWindowsDll(1)
#else
type __GdipShutdown __token
__GdipShutdown+ __gdip_token

_hresult=0
if(!__gdip_token)
	lock
	if(!__gdip_token)
		GDIP.GdiplusStartupInput gi.GdiplusVersion=1
		_hresult=GDIP.GdiplusStartup(&__gdip_token &gi 0)
		if(_hresult) ret

err+ _hresult=-1; ret
ret __gdip_token!0
#endif
