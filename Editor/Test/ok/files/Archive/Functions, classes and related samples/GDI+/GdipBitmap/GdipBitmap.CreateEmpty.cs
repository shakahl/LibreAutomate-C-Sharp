function'GDIP.GpBitmap* width height [format]

 Creates empty bitmap of specified dimensions.

 format - one of GDIP.PixelFormatX constants. If omitted, uses GDIP.PixelFormat32bppARGB.


if(!GdipInit) ret
Delete

if(getopt(nargs)<3) format=GDIP.PixelFormat32bppARGB

_hresult=GDIP.GdipCreateBitmapFromScan0(width height 0 format 0 +&m_i)
ret +m_i
