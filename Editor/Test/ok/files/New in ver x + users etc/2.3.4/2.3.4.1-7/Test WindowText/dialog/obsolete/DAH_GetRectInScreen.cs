 /dlg_apihook
function# hdc RECT&r RECT&ro

ro=r

int hwnd=WindowFromDC(hdc)
if(!hwnd) ret

RECT rc rw rws rb

GetClientRect(hwnd &rc);; zRECT rc
GetWindowRect(hwnd &rws); rw=rws; OffsetRect &rw -rw.left -rw.top;; zRECT rw

if !EqualRect(&rc &rw)
	GetClipBox(hdc &rb);; zRECT rb
	if(EqualRect(&rb &rw)) ;;window DC, else client DC
		MapWindowPoints hwnd 0 +&rc 2
		OffsetRect &ro rws.left-rc.left rws.top-rc.top

 now we have client coordinates; convert to screen if need
if(1) MapWindowPoints hwnd 0 +&ro 2

ret hwnd


 POINT p
 GetDCOrgEx(hdc &p); out "%i %i   ot=%i" p.x p.y GetObjectType(hdc)
