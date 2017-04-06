 file
str sFile="$desktop$\qm test png gdip.png"

 ________________________

 capture to memory
__GdiHandle hb
CaptureImageOnScreen(0 0 ScreenWidth ScreenHeight "" hb)

 save as png
#compile "__Gdip"
GdipBitmap im
if(!im.FromHBITMAP(hb)) end "error"
if(!im.Save(sFile)) end "error"

 ________________________

 show the image
run sFile


 NOTES
 GDI+ may be unavailable on Windows 2000.
 Png file created with GDI+ is 33% bigger than with GflAx.
