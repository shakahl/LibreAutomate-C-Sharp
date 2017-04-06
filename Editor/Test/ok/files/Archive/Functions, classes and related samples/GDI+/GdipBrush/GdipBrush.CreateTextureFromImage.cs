function'GDIP.GpBrush* GDIP.GpImage*image [wrapMode] ;;wrapMode one of GDIP.WrapModeX constants, default WrapModeTile

 Creates this brush as texture brush.

 image can be GpImage* or GdipImage or GdipBitmap.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCreateTexture(image wrapMode +&m_b)
ret m_b
