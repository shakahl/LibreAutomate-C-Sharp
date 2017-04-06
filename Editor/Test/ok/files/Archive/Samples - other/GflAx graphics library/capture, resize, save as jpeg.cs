 save to
str sFile="$desktop$\test.jpg"
 resize to
int width(100) height(100)

 ---------------------

 capture image from screen
str sTempFile.expandpath("$temp$\qm_image.bmp")
if(!CaptureImageOrColor(0 32 0 sTempFile)) ret

 resize
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.LoadBitmap(sTempFile)
g.Resize(width height)

 save as jpeg
g.SaveFormat=GflAx.AX_JPEG
g.SaveBitmap(_s.expandpath(sFile))

 ---------------------

 show the image
run sFile
