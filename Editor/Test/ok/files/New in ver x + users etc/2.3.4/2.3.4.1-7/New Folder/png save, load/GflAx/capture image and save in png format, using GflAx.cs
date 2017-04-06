 file
str sFile="$desktop$\qm test png.png"

 ________________________

 capture to memory
__GdiHandle hb
CaptureImageOnScreen(0 0 ScreenWidth ScreenHeight "" hb)

 save as png
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.SetPicture(BitmapHandleToIPicture(hb))
g.SaveFormat=GflAx.AX_PNG ;;use other constants to save in other formats
g.SaveBitmap(_s.expandpath(sFile))

 ________________________

 show the image
run sFile
