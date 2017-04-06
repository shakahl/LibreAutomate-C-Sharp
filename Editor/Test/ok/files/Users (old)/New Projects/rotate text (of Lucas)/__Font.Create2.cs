function# $font [size] [style] [rotate];;style: 1 bold, 2 italic, 4 underline, 8 strikeout

 Creates font.
 If font is "", uses "Tahoma".


LOGFONT lf
if size
	int hdc=GetDC(0)
	lf.lfHeight=-MulDiv(size GetDeviceCaps(hdc LOGPIXELSY) 72)
	ReleaseDC 0 hdc
lf.lfWeight=iif(style&1 FW_BOLD FW_NORMAL) 
lf.lfItalic=style&2
lf.lfUnderline=style&4
lf.lfStrikeOut=style&8
lf.lfCharSet=1
lf.lfEscapement=rotate
lf.lfOrientation=rotate
if(empty(font)) font="Tahoma"
lstrcpyn(&lf.lfFaceName font 32)

if(handle) DeleteObject handle
handle=CreateFontIndirect(&lf)