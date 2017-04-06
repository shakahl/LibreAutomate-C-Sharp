function'GDIP.GpBitmap* GdipBitmap&sourceBitmap x y width height [format]

 Create this bitmap from part of another bitmap.

 format - one of GDIP.PixelFormatX constants.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCloneBitmapAreaI(x y width height format +sourceBitmap +&m_i)
ret +m_i
