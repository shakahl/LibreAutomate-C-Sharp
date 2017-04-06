function'GDIP.GpGraphics* GDIP.GpImage*image

 Creates this graphics and sets to draw in the image.

 image can be GpImage* or GdipImage or GdipBitmap.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipGetImageGraphicsContext(image &m_g)
SetDefProp
ret m_g
