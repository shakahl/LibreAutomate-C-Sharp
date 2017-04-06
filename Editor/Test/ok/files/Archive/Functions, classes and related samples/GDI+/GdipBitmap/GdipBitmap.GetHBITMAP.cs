function# [backColor] [flags] ;;backColor: 0xAARRGGBB or ColorARGB(red green blue alpha).  flags: 1 DDB

 Creates GDI bitmap from this GDI+ bitmap.
 Returns GDI bitmap handle, or 0 if failed.
 The bitmap later must be deleted with DeleteObject.
 If the image has transparent areas, uses backColor as background color. Alpha channel remains, therefore the color is used only by functions that don't support alpha, such as BitBlt.
 The bitmap has format used by GDI+. It is DIB. Use flag 1 if need DDB.


_hresult=GDIP.GdipCreateHBITMAPFromBitmap(+m_i &_i backColor)
if(flags&1) ret CopyImage(_i 0 0 0 LR_COPYDELETEORG)
ret _i
