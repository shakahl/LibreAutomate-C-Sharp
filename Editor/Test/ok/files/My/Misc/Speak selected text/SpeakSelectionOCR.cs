 Speaks selected text using OCR. Use this macro for text that cannot be normally selected, eg in some pdf documents.
 Requires MS Office. Uses Office's MODI component. If it is not installed, should install automatically.

 How to use: Press Ctrl+Alt+S and select text. To select text, draw rectangle with mouse.
 You can use hot keys to control it. Shows them when called first time.

 ________________________

 Capture image.

str f.expandpath("$temp qm$\speak_ocr.bmp")
RECT r
if(!CaptureImageOrColor(0 16|32 0 f r)) ret
OnScreenRect 1 &r

 OCR.

typelib MODI {A5EDEDF4-2BBC-45F3-822B-E60C278A1A79} 11.0

MODI.Document doc._create
doc.Create(f) ;;load the file

doc.OCR(MODI.miLANG_ENGLISH -1 0); err OnScreenDisplay "OCR failed."; ret

MODI.Image im=doc.Images.Item(0) ;;first page
str s=im.Layout.Text

 Results.

SpeakTextWithHotKeys s -1
OnScreenRect 2 &r
