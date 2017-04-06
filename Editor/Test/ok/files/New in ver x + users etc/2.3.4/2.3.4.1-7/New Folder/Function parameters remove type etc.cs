 str s="PolyTextOutW(HDC hdc, CONST POLYTEXTW * ppt, int nstrings)"
 str s="GdipDrawString(GpGraphics *graphics, GDIPCONST WCHAR *string, INT length, GDIPCONST GpFont *font, GDIPCONST RectF *layoutRect, GDIPCONST GpStringFormat *stringFormat, GDIPCONST GpBrush *brush)"
key SE
str s.getsel
if(!s.len) ret

 s.CompactFunction

s.replacerx("[^\(,]+[ \*]+(\w+[,\)])" " $1")
s.findreplace("( " "(")
 out s; ret
wait 0 ML
0.2

s.setsel
