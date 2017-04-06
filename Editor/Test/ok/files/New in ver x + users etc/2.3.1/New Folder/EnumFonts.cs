out

ARRAY(ENUMLOGFONTEX) a

LOGFONT logfont

logfont.lfCharSet = SYMBOL_CHARSET
 logfont.lfCharSet = DEFAULT_CHARSET
 logfont.lfPitchAndFamily = FIXED_PITCH|FF_DONTCARE

int hdc = GetDC(0)

EnumFontFamiliesEx(hdc &logfont &FontNameProc &a 0)

ReleaseDC(0 hdc)

int i
for i 0 a.len
	ENUMLOGFONTEX& r=a[i]
	lpstr s=&r.elfLogFont.lfFaceName; out s
	 lpstr s=&r.elfFullName; out s
out a.len
