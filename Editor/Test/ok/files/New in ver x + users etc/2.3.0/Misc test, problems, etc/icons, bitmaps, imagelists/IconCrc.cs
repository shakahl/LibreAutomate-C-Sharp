 /
function# $icofile [index] [flags]

 Prepares icon to compare with another icon.
 Returns icon data checksum. If checksums of two icons are equal, the icons are visually equal.
 Error if fails. Error if display colors is not 32-bit.
 All arguments are the same as with GetFileIcon.


if(ScreenColors!=32) end "This function requires 32-bit display colors. You can set it in Control Panel."

__Hicon hi; ICONINFO ii
hi=GetFileIcon(icofile index flags)
if(!hi or !GetIconInfo(hi &ii)) end ES_FAILED
__GdiHandle b1(ii.hbmColor) b2(ii.hbmMask) ;;caller deletes bitmaps

RECT r; int& cx(r.right) cy(r.bottom)
cx=16<<(flags&1); cy=cx

__MemBmp mb.Create(cx cy)
FillRect mb.dc &r GetStockObject(WHITE_BRUSH)
if(!DrawIconEx(mb.dc 0 0 hi cx cy 0 0 DI_NORMAL)) end ES_FAILED

int n=cx*cy*4
str s.all(n)
if(!GetBitmapBits(mb.bm n s)) end ES_FAILED

ret Crc32(s n)
