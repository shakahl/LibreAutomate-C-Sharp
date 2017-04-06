 /
function ^nticks [hwnd] ;;nticks: >0 to right, <0 to left.

 Mouse horizontal wheel.

 nticks - number of wheel ticks to scroll to the right (positive) or left (negative).
 hwnd - handle of a child window. If used, sends mouse wheel message directly to the child window.

 REMARKS
 Does not work on Windows 2000, XP, 2003. Works only with windows that support horizontal scrolling message WM_MOUSEHWHEEL added in Windows Vista.
 The auto delay (spe) is applied, except when hwnd used.


def WM_MOUSEHWHEEL 0x020E
def MOUSEEVENTF_HWHEEL 0x01000

if(hwnd)
	int i
	ifk(C) i|MK_CONTROL
	ifk(S) i|MK_SHIFT
	ifk((1)) i|MK_LBUTTON
	ifk((2)) i|MK_RBUTTON
	ifk((4)) i|MK_MBUTTON
	ifk((5)) i|MK_XBUTTON1
	ifk((6)) i|MK_XBUTTON2
	SendMessage hwnd WM_MOUSEHWHEEL nticks*120<<16|i ym<<16|xm
else
	INPUT in.mi.dwFlags=MOUSEEVENTF_HWHEEL
	in.mi.mouseData=nticks*120
	in.mi.dwExtraInfo=1354291109
	SendInput 1 &in sizeof(INPUT)
	wait -2
