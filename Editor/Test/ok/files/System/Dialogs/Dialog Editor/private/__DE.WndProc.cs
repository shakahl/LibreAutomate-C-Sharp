 /Dialog_Editor
function# hwnd message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_NCCREATE _hwnd=hwnd
	case WM_CREATE if(!_WmCreate(+lParam)) ret -1
	case WM_CLOSE if(!sub.Close) ret
	case WM_DESTROY PostQuitMessage 0
	case WM_NCDESTROY RemoveProp _hwnd "qm_design_mode"
	case WM_CTLCOLORSTATIC if(lParam=_hmark) ret _brushMark
	case WM_EXITSIZEMOVE subs.AutoSizeEditor ;;ensure whole form is in editor
	case WM_COMMAND _WmCommand(wParam lParam)
	case WM_HOTKEY if(wParam=1 and IsWindowEnabled(_hwnd)) _Capture
	case [WM_KEYDOWN,WM_SYSKEYDOWN] _Key(wParam)
	case [WM_LBUTTONDOWN,WM_RBUTTONDOWN,WM_APP+1] _Drag(message lParam); ret
	case WM_SETCURSOR if(sub.SetCursor(wParam lParam)) ret 1
	case WM_APP ret _ddMacro
	case WM_HELP QmHelp "IDH_DIALOG_EDITOR"

ret DefWindowProcW(hwnd message wParam lParam)


#sub Close c
if(IsIconic(_hwnd)) res _hwnd
if(_save) sel mes("Do you want to save changes?" "Dialog Editor" "YNC!")
	case 'Y' if(!_Save) ret
	case 'C' ret
act _hwndqm; err
ret 1


#sub SetCursor c
function! wParam lParam

_edge=0
if(wParam!_hwnd or lParam&0xffff!HTCLIENT) ret
POINT p; xm p
int h=subs.ControlFromPoint(p 1); if(!h) ret
if(h=_hform) int isForm=6
RECT r; GetWindowRect h r; InflateRect &r -2-isForm -2-isForm
if(r.right-r.left<8+isForm or r.bottom-r.top<8+isForm) ret

int cur edge
if(p.y<r.top) edge|1; else if(p.y>=r.bottom) edge|5
if(p.x<r.left) edge|2; else if(p.x>=r.right) edge|6

sel edge&3
	case 1 cur=IDC_SIZENS
	case 2 cur=IDC_SIZEWE
	case else ret

if(edge<=3 and isForm) ret
_edge=edge

SetCursor LoadCursor(0 +cur)
ret 1
