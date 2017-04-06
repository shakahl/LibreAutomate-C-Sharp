 It is possible to display anything with function OnScreenDisplay, using its paramater pictfile. We can even create the file at run time.
 This macro displays horizontally centered text.

str text=
 This
 is multiline
 text
 that's
 centered

 font, text color and background color
__Font font.Create("Segoe UI" 20)
int textColor=0xff0080
int brush=GetSysColorBrush(COLOR_BTNFACE) ;;transparent
 __GdiHandle brush=CreateSolidBrush(0xc0ffff) ;;color

 measure
RECT r
__MemBmp m.Create(1 1)
_i=SelectObject(m.dc font)
DrawTextW(m.dc @text -1 &r DT_CALCRECT|DT_CENTER)
SelectObject(m.dc _i)

 draw
m.Create(r.right r.bottom)
FillRect m.dc &r brush; SetBkMode m.dc TRANSPARENT
SetTextColor m.dc textColor
_i=SelectObject(m.dc font)
DrawTextW(m.dc @text -1 &r DT_CENTER)
SelectObject(m.dc _i)

 save in a temp bmp file
__TempFile tf.Init(".bmp")
SaveBitmap(m.bm tf)
m.Delete

 OSD with the bmp file
OnScreenDisplay "" 5 0 0 "" 0 0 128 "" 0 0 tf
