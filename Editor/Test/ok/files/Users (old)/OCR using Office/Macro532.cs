 This macro tries to search, but fails.

out
str img="$desktop$\qm ocr.bmp" ;;this is an image containing text
str s

int wid(300) hei(30)

POINT p; xm p
RECT r; r.left=p.x-4; r.top=p.y-8; r.right=r.left+wid; r.bottom=r.top+hei

CaptureRect r.left r.top wid hei img

typelib MODI {A5EDEDF4-2BBC-45F3-822B-E60C278A1A79} 11.0

MODI.Document doc._create
doc.Create(img.expandpath)

doc.OCR(MODI.miLANG_ENGLISH -1 0)

MODI.MiDocSearch se._create
VARIANT pn(1) wi(0) sa(0) bw(0)
se.Initialize(doc "search" pn wi sa bw -1 -1 -1 -1) ;;Error: 0x80004005, Unspecified error

MODI.IMiSelectableItem si
se.Search(0 si)

s=si.Text
out s
