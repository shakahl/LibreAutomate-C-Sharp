BSTR b="Text"

 create big memory DC
__MemBmp m.Create(1000 100)

 set font
__Font font.Create("Verdana" 14 2)
SelectObject m.dc font

 calculate text rectangle
RECT r; DrawTextW m.dc b b.len &r DT_CALCRECT|DT_EXPANDTABS|DT_NOPREFIX

 draw background and rectangle
OffsetRect &r 4 2
InflateRect &r 4 2
__GdiHandle brush=CreateSolidBrush(0xC0FFFF); SelectObject m.dc brush
__GdiHandle pen=CreatePen(0 2 0xff0000); SelectObject m.dc pen
Rectangle m.dc 1 1 r.right r.bottom

 draw text
SetBkMode m.dc TRANSPARENT
SetTextColor m.dc 0xff0000
InflateRect &r -4 -2
DrawTextW m.dc b b.len &r DT_EXPANDTABS|DT_NOPREFIX

 copy the rectangle to another memory DC (crop)
InflateRect &r 4 2
__MemBmp m2.Create(r.right r.bottom m.dc)

 bitmap to clipboard
if(OpenClipboard(_hwndqm)) EmptyClipboard; SetClipboardData(CF_BITMAP m2.Detach); CloseClipboard
