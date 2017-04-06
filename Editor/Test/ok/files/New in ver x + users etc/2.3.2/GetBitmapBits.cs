 this gets RGB colors in format 0xAARRGGBB. Note that in QM is used format 0xAABBGGRR (COLORREF type in Windows programming). Don't remember how in file.
 Not sure but maybe will not work if display color depth is not 32bit. Then use GetDIBits instead of GetBitmapBits.

int hb
if(!CaptureImageOrColor(&hb 0)) ret

__MemBmp mb.Attach(hb)

BITMAP b
GetObject hb sizeof(BITMAP) &b

int n=b.bmHeight*b.bmWidthBytes
str s.all(n)
if(!GetBitmapBits(mb.bm n s)) end ES_FAILED

 outb s n
