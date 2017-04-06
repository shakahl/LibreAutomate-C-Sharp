
str fBmp="$temp$\qm_modi.bmp"
fBmp.expandpath

 capture bitmap (optional)
if(!CaptureImageOrColor(0 0 _hwndqm fBmp)) ret

Q &q
 convert to text
typelib MODI {A5EDEDF4-2BBC-45F3-822B-E60C278A1A79} 11.0
MODI.Document doc._create

doc.Create(fBmp)
doc.OCR(MODI.miLANG_ENGLISH -1 0)

MODI.Image im=doc.Images.Item(0)
str s=im.Layout.Text
Q &qqq

 show results
 out
out s
outq
