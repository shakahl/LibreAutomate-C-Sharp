function'GDIP.GpBitmap* hIcon

 Create this bitmap from icon handle.
 Note: Don't use this function. The bitmap will be without alpha etc (GDI+ bugs).


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCreateBitmapFromHICON(hIcon +&m_i)
ret +m_i
