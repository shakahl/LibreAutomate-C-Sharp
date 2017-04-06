 /OSD example
function hwnd hdc cx cy param

int hpen oldpen hbrush oldbrush oldfont

 create/select pen and draw rectangle
hpen=CreatePen(0 10 0xff0000); oldpen=SelectObject(hdc hpen)
 Rectangle hdc 5 5 cx-10 cy-10 ;;simple
RoundRect hdc 5 5 cx-10 cy-10 20 20 ;;rounded corners
DeleteObject SelectObject(hdc oldpen)

 create/select brush and pen, and draw ellipse
hbrush=CreateSolidBrush(0x00ff00); oldbrush=SelectObject(hdc hbrush)
hpen=CreatePen(0 3 0x00ff00); oldpen=SelectObject(hdc hpen)
Ellipse hdc 50 50 cx-50 cy-50
DeleteObject SelectObject(hdc oldbrush)
DeleteObject SelectObject(hdc oldpen)

 draw icon
int hicon=GetFileIcon("$qm$" 0 1)
DrawIconEx hdc cx/2-16 40 hicon 32 32 0 0 3
DestroyIcon hicon

 draw text
str txt=iif(_unicode "Some Text ąč ﯔﮥ" "Some Text")
 create font
#ifdef __Font
__Font hfont.Create("Tahoma" 16 0)
oldfont=SelectObject(hdc hfont)
#endif
 set text color and transparent background
SetTextColor(hdc 0x00ffff)
SetBkMode(hdc TRANSPARENT)
 set rectangle
RECT r
DrawTextW hdc @txt -1 &r DT_CALCRECT|DT_NOPREFIX
OffsetRect &r 70 80
 draw
DrawTextW hdc @txt -1 &r DT_NOPREFIX

#ifdef __Font
SelectObject hdc oldfont
#endif
