 /
function# hwnd

dll gdi32 #GetTextExtentPoint32 hDC $lpsz cbString SIZE*lpSize

int st=GetWindowLong(hwnd GWL_STYLE)
if(st&WS_CAPTION != WS_CAPTION) ret
str s.getwintext(hwnd); if(!s.len) ret
int dc=GetWindowDC(hwnd)
SIZE si
int r=GetTextExtentPoint32(dc s s.len &si)
ReleaseDC(hwnd dc)
if(r) ret si.cx
