str fTes="Q:\Downloads\tesseract-ocr\tesseract old.exe" ;;change this
int scale=2 ;;try to change this if recognition is poor. Tesseract is very sensitive to text size. Usually with 2 works best.

 Tesseract does not recognize small text, eg normal size text from a web page. Need to make eg 2 times bigger.
 However resizing gives some distortion, and probably it is the reason of tesseract OCR errors.
 Probably another reason is because small text captured from screen is not perfect, ie not like printed.

 ---------------------

str fBmp="$temp$\qm_tesseract.bmp"
str fTif="$temp$\qm_tesseract.tif"
str fTxt="$temp$\qm_tesseract.txt"
fTes.expandpath
fBmp.expandpath
fTif.expandpath
fTxt.expandpath

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
if(scale!1) g.Resize(g.width*scale g.height*scale) ;;better quality and OCR than CopyImage
g.SaveFormatName="tiff"
g.SaveBitmap(fTif)

 run "$program files$\IrfanView\i_view32.exe" fTif; ret

Q &qq
 convert to text
if(fTxt.endi(".txt")) fTxt.fix(fTxt.len-4) ;;tesseract always adds ".txt"
str cl.format("%s ''%s'' ''%s''" fTes fTif fTxt) so
if(RunConsole2(cl so)) end so
Q &qqq

 show results
fTxt+".txt"
_s.getfile(fTxt)
out
out _s
outq
