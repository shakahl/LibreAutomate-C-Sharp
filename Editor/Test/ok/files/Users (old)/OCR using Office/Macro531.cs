 This macro OCRs text by the mouse pointer.

out
str s

 1. Capture screen area by the mouse pointer and save to a temporary image file.

POINT p; xm p; p.x-4; p.y-8 ;;get mouse pointer position and make some correction
int wid(300) hei(30) ;;width and height of the screen area
str img="$desktop$\qm ocr temp.bmp" ;;temporary image file

CaptureRect p.x p.y wid hei img ;;capture and save

 2. Use MS Office Imaging to convert the image file to text.

typelib MODI {A5EDEDF4-2BBC-45F3-822B-E60C278A1A79} 11.0

MODI.Document doc._create
doc.Create(img.expandpath) ;;load the file

doc.OCR(MODI.miLANG_ENGLISH -1 0); err s="FAILED"; goto g1 ;;convert to text

MODI.Image im=doc.Images.Item(0) ;;first page
s=im.Layout.Text

 3. Show results.

 g1
RECT r; r.left=p.x; r.top=p.y; r.right=r.left+wid; r.bottom=r.top+hei
OnScreenRect 1 &r
OnScreenDisplay s 0 0 0 0 0 0xff 2
OnScreenRect 2 &r
