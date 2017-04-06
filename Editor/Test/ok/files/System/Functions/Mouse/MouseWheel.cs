 /
function ^nticks [hwnd] ;;nticks: >0 forward, <0 backward.

 Mouse wheel.

 nticks - number of wheel ticks forward (positive) or backward (negative).
 hwnd - handle of a child window. If used, sends mouse wheel message directly to the child window.

 REMARKS
 The auto delay (spe) is applied, except when hwnd used.


if(hwnd)
	int i
	ifk(C) i|MK_CONTROL
	ifk(S) i|MK_SHIFT
	ifk((1)) i|MK_LBUTTON
	ifk((2)) i|MK_RBUTTON
	ifk((4)) i|MK_MBUTTON
	ifk((5)) i|MK_XBUTTON1
	ifk((6)) i|MK_XBUTTON2
	SendMessage hwnd WM_MOUSEWHEEL nticks*120<<16|i ym<<16|xm
else
	INPUT in.mi.dwFlags=MOUSEEVENTF_WHEEL
	in.mi.mouseData=nticks*120
	in.mi.dwExtraInfo=1354291109
	SendInput 1 &in sizeof(INPUT)
	wait -2
