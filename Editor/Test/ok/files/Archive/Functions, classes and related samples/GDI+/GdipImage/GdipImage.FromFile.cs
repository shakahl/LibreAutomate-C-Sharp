function'GDIP.GpImage* $imageFile

 Creates this image from file.
 Supported formats: BMP, GIF, JPEG, PNG, TIFF, ICON, WMF, EMF, EXIF.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipLoadImageFromFile(@_s.expandpath(imageFile) +&m_i)
ret m_i
