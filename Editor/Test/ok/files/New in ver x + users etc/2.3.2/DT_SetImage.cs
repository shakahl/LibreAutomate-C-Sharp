 /
function hwnd ~image

 Displays image in static control.

 hwnd - static control handle.
 image - image file or bitmap handle.
    File can be bmp, gif, jpg, png, and some other. On Windows 2000 without gdiplus.dll can be only bmp, gif and jpg.
    If it is handle, the caller can delete it immediately.


type DTSIDATA wndproc str'image __MemBmp'mb __Stream'is

DTSIDATA* p=+GetProp(hwnd "dtsidata")
if(!p)
	p._new
	p.wndproc=SubclassWindow(hwnd &__DTSI_WndProc)
	SetProp(hwnd "dtsidata" p)

int hb i j
i=val(image 0 j)
if(i and j=image.len) hb=CopyImage(i IMAGE_BITMAP 0 0 0)
else if(SelStr(9 image ".bmp" ".gif" ".jpg" ".jpeg")) hb=LoadPictureFile(image)
else p.is.CreateOnFile(image STGM_READ)

if(hb) p.mb.Attach(hb)

InvalidateRect hwnd 0 1

if(hb and wintest(hwnd "" "Static") and GetWinStyle(hwnd)&SS_REALSIZEIMAGE)
	BITMAP b; GetObject p.mb.bm sizeof(BITMAP) &b
	siz b.bmWidth b.bmHeight hwnd
