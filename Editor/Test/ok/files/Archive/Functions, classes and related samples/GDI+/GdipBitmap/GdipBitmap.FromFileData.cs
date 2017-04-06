function'GDIP.GpBitmap* str&imageFileData

 Creates this bitmap from image data (image file in memory).
 Supported formats: BMP, GIF, JPEG, PNG, TIFF, ICON, WMF, EMF, EXIF.


if(!GdipInit) ret
Delete

__Stream x.CreateOnHglobal(imageFileData imageFileData.len); err end _error
_hresult=GDIP.GdipCreateBitmapFromStream(x +&m_i)
ret +m_i
