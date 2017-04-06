 /OSK_Main
function hwnd hdc cx cy param

OSKVAR- v

int oldfont=SelectObject(v.mb.dc v.font)
RECT r.right=cx; r.bottom=cy; FillRect v.mb.dc &r COLOR_BTNFACE+1
SetBkMode(v.mb.dc TRANSPARENT)

int i pressed
for i 0 v.a.len
	OSKKEY& k=v.a[i]
	pressed=RealGetKeyState(k.vk)
	SetTextColor(v.mb.dc iif(pressed||k.pressed v.color 0))
	OSK_DrawKey v.mb.dc i
	if(pressed) k.pressed=1

SelectObject v.mb.dc oldfont

BitBlt hdc 0 0 cx cy v.mb.dc 0 0 SRCCOPY
