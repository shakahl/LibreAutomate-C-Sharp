 /
function monitor marginLeft marginTop marginRigt marginBottom [flags] ;;flags: 1 resize maximized windows

 Sets work area. It is the area where windows are maximized.

 monitor - 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle.
 marginX - margin width in pixels.


RECT r
int hmon=MonitorFromIndex(monitor 0 &r)
r.left+marginLeft
r.top+marginTop
r.right-marginRigt
r.bottom-marginBottom

if(!SystemParametersInfo(SPI_SETWORKAREA 0 &r SPIF_SENDCHANGE)) ret

if(flags&1)
	ARRAY(int) a; int i
	win "" "" "" 0 0 0 a
	for i 0 a.len
		int h=a[i]
		if(!max(h) or MonitorFromWindow(h 0)!=hmon) continue
		res h; max h
