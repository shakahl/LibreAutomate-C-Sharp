LOGFONTW lf
__Hdc dc.Init
EnumFontFamiliesExW(dc &lf &sub.EnumFontProc 0 0)


#sub EnumFontProc
function# ENUMLOGFONTEXW*lpelfe NEWTEXTMETRICEXW*lpntme FontType lParam
str s.ansi(&lpelfe.elfLogFont.lfFaceName)
out s
ret 1
