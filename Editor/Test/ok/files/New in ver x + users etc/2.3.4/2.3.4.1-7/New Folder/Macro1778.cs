str f.expandpath("$desktop$\qm test bmp.bmp")
CaptureImageOnScreen(0 0 ScreenWidth ScreenHeight f)

 run f

str sFile="$desktop$\qm test png.png"

 resize
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.LoadBitmap(f)

 save as jpeg
g.SaveFormat=GflAx.AX_PNG
g.SaveBitmap(_s.expandpath(sFile))

 ---------------------

 show the image
run sFile
