function'GDIP.GpBitmap* $imageFile

 Creates this bitmap from image file.
 Supported formats: BMP, GIF, JPEG, PNG, TIFF, ICON, WMF, EMF, EXIF.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCreateBitmapFromFile(@_s.expandpath(imageFile) +&m_i)
ret +m_i
