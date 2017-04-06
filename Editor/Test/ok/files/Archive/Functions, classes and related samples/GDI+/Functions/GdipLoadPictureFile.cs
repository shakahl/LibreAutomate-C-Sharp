 /
function# $imageFile [backColor] [flags] ;;backColor: 0xAARRGGBB or ColorARGB(red green blue alpha).  flags: 1 DDB

 Loads image file and returns GDI bitmap handle.
 Returns 0 if failed.
 If the image has transparent areas, uses backColor as background color. Alpha remains.
 The returned bitmap later must be deleted with DeleteObject. Or assign to a __GdiHandle variable, it calls DeleteObject automatically.
 Supports all GDI+ formats: BMP, GIF, JPEG, PNG, TIFF, ICON, WMF, EMF, EXIF.


#compile "__Gdip"
GdipBitmap b
if(!b.FromFile(imageFile)) ret
ret b.GetHBITMAP(backColor flags)
