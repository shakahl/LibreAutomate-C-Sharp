
str fBmp="$temp$\qm_tesseract.bmp"
str fTif="$temp$\qm_tesseract.tif"
fBmp.expandpath
fTif.expandpath

 capture bitmap (optional)
if(!CaptureImageOrColor(0 0 _hwndqm fBmp)) ret

 bmp -> tif
Q &q
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.LoadBitmap(fBmp)
 g.saturation(-100)
 g.Sharpen(50)
 g.ChangeColorDepth(1 0 1)
 g.Resize(g.width*scale g.height*scale)
g.SaveFormatName="tiff"
g.SaveBitmap(fTif)

 run fTif
 ret

Q &qq
 convert to text
typelib MODI {A5EDEDF4-2BBC-45F3-822B-E60C278A1A79} 11.0
MODI.Document doc._create

doc.Create(fTif)
doc.OCR(MODI.miLANG_ENGLISH -1 0)

MODI.Image im=doc.Images.Item(0)
str s=im.Layout.Text
Q &qqq

 show results
 out
out s
outq
