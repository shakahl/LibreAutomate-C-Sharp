 /
function hwnd hbitmap

 Displays bitmap in static control.
 Differences from STM_SETIMAGE: 1. Copies the bitmap. 2. Correctly displays alpha bitmaps.

 hwnd - static control handle.
 hbitmap - bitmap handle. The caller can delete it immediately.


type SICSBDATA wndproc __MemBmp'mb

SICSBDATA* p=+GetProp(hwnd "sicsbdata")
if(!p)
	p._new
	p.wndproc=SubclassWindow(hwnd &__SICSB_WndProc)
	SetProp(hwnd "sicsbdata" p)

p.mb.Attach(CopyImage(hbitmap IMAGE_BITMAP 0 0 0))
InvalidateRect hwnd 0 1

if(GetWinStyle(hwnd)&SS_REALSIZEIMAGE)
	BITMAP b; GetObject p.mb.bm sizeof(BITMAP) &b
	siz b.bmWidth b.bmHeight hwnd
