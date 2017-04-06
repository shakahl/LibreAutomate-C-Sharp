 /dlg_apihook
function hdc CLogicalCoord&c

 SetBkMode hdc TRANSPARENT
int R showR

__Font f1 f2 f3 f4
f1.Create("Tahoma" 10*c.ex 0)
f2.Create("Tahoma" 10*c.ex 15)
 f2.Create("modern" 10*c.ex 0)
 f2.Create("Courier New" 10*c.ex 0)
 f2.handle=CreateFont(-10*c.ex 0 0 0 FW_NORMAL 0 0 0 ANSI_CHARSET 0 0 0 0 "Tahoma")
BSTR g.alloc(100)

 BSTR s="Text Ԏݭῼ"
str s="Text ąčę"; s.ConvertEncoding(_unicode 0)

SelectObject hdc f1
GetGlyphIndices(hdc s s.len g 0)
outb g.pstr s.len*2
rep(3) R=ExtTextOut(hdc c.x c.y ETO_GLYPH_INDEX 0 +g.pstr s.len 0); c.Offset(0 30)

SelectObject hdc f2
GetGlyphIndices(hdc s s.len g 0)
outb g.pstr s.len*2
rep(3) R=ExtTextOut(hdc c.x c.y ETO_GLYPH_INDEX 0 +g.pstr s.len 0); c.Offset(0 30)

 37 00 48 00 5B 00 57 00 
