 /
function# hwnd partIndex str&s

 Gets text of standard status bar control.
 If successful, returns text length, else 0.

 hwnd - control handle.
 partIndex - 0-based part index.
 s - receives text.

 REMARKS
 Cannot get text from owner-draw parts.

 EXAMPLE
 int hwnd=child("" "msctls_statusbar32" "Internet Explorer")
 str s
 GetStatusBarText hwnd 0 s
 out s


if(!hwnd) end ERR_HWND

if(SendMessage(hwnd SB_GETTEXTLENGTHW partIndex 0)&0xffff=0) s.all; ret

Acc a=acc(hwnd); a.Navigate("first")
a.elem=partIndex+1
s=a.Name
err+ s.all
ret s.len
