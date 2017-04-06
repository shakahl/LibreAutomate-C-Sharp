#sub FindControl c
function# hwnd

if(hwnd=_hform) ret 0
int i cid=GetDlgCtrlID(hwnd)

for(i 0 _ac.len) if(_ac[i].cid=cid) ret i
ret -1


#sub GetControl c
function'___DE_CONTROL* hwnd

ret &_ac[sub.FindControl(hwnd)]


#sub ControlFromPoint c
function# POINT'p [flags] ;;flags: 1 screen coord
 Returns form control or form or 0.

if(flags&1) MapWindowPoints 0 _hwnd &p 1
int h=ChildWindowFromPoint(_hwnd p.x p.y); if(h!_hform and h!_hmark) ret
MapWindowPoints _hwnd _hform &p 1; h=ChildWindowFromPoint(_hform p.x p.y)
if(!h) h=_hform

ret h


#sub SetMark c
RECT r; GetWindowRect(_hsel &r); MapWindowPoints(0 _hwnd +&r 2)
SetWindowPos(_hmark 0 r.left r.top 4 4 SWP_SHOWWINDOW|SWP_NOCOPYBITS)


#sub AutoSizeEditor c
 Resizes editor if form is bigger.

RECT r1 r2
GetClientRect _hwnd &r1
GetWindowRect _hform &r2; MapWindowPoints 0 _hwnd +&r2 2
int xp(r2.right-r1.right) yp(r2.bottom-r1.bottom)
if xp>0 or yp>0
	GetWindowRect _hwnd &r1
	SetWindowPos(_hwnd 0 0 0 r1.right-r1.left+iif(xp>0 xp 0) r1.bottom-r1.top+iif(yp>0 yp 0) SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE)
