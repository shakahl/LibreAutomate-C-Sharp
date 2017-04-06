int hwnd=TriggerWindow
if(!hwnd) hwnd=win("Dialog Editor" "QM_DE_class")
int c=child("" "#32770" hwnd 0x0 "style=0x8000000 0x8000000")
POINT p pp; int mbPressed
rep
	0.05
	if(hwnd!=win) if(IsWindow(hwnd)) continue; else ret
	ifk- (4); mbPressed=0; continue
	xm p
	if !mbPressed
		mbPressed=1
	else
		if(!memcmp(&p &pp sizeof(p))) continue
		ScrollWindowEx(c p.x-pp.x p.y-pp.y 0 0 0 0 SW_SCROLLCHILDREN)
		InvalidateRect c 0 1
	pp=p
