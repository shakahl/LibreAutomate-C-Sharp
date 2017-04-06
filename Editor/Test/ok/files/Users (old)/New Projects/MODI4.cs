
str fBmp="$temp$\qm_modi.bmp"
fBmp.expandpath

 capture bitmap (optional)
 if(!CaptureImageOrColor(0 0 _hwndqm fBmp)) ret

#compile "____UseComUnregistered"
__UseComUnregistered ucu.Activate("MDIVWCTL.X.manifest")

 convert to text
typelib MODI "$qm$\MODI\MDIVWCTL.DLL"
MODI.Document doc._create

doc.Create(fBmp)
doc.OCR(MODI.miLANG_ENGLISH -1 0)

MODI.Image im=doc.Images.Item(0)
str s=im.Layout.Text

 show results
 out
out s
