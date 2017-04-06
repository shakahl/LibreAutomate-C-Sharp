 \
function nCode wParam MSLLHOOKSTRUCT*m
if(getopt(nargs)) goto gHook ;;called as hook procedure. Else runs on Alt+mouse left button trigger.
 ___________________________________________

if(getopt(nthreads)>1) ret

int-- t_hd=win(mouse)
if(GetWinStyle(t_hd)&WS_DLGFRAME=0) ret ;;desktop, tooltip, fullscreen, etc. However also may skip fully skinned and some other exotic windows.
if(max(t_hd)) res t_hd
 act t_hd

POINT-- t_p ;;stores previous mouse pos
GetCursorPos &t_p

int hhM=SetWindowsHookEx(WH_MOUSE_LL &AltDragWindow _hinst 0)
int hhK=SetWindowsHookEx(WH_KEYBOARD_LL &AltDragWindow _hinst 0) ;;need to neutralize Alt, ie press Ctrl when releasing Alt, before or after releasing mouse button. Can use same hook procedure because wParam is different (it is a mouse or keyboard message).
opt waitmsg 1
wait -1 -V t_hd ;;wait until t_hd is 0. The hook procedure will make it 0 when finished.
UnhookWindowsHookEx hhM
UnhookWindowsHookEx hhK
ret
 ___________________________________________

 gHook
if(nCode<0) goto gRet

sel wParam
	case WM_MOUSEMOVE
	 check conditions. Not everything is necessary, but safer.
	if(!IsWindow(t_hd)) t_hd=0; goto gRet
	ifk- (1)
		ifk-(A) t_hd=0
		goto gRet
	
	 move window
	int x y; GetWinXY t_hd x y
	mov x+(m.pt.x-t_p.x) y+(m.pt.y-t_p.y) t_hd
	t_p=m.pt
	
	case WM_LBUTTONUP
	ifk-(A) t_hd=0 ;;if Alt released, stop. Else wait for Alt up.
	
	case WM_LBUTTONDOWN ;;mouse down again without releasing Alt
	t_p=m.pt
	
	case WM_KEYUP ;;this is from keyboard hook
	KBDLLHOOKSTRUCT* k=+m
	if(k.flags&LLKHF_INJECTED) goto gRet
	sel k.vkCode
		case [VK_LMENU,VK_RMENU] ;;Alt up
		ifk-((1)) t_hd=0 ;;if mouse released, stop
		opt keysync 1; key+ C; key- AC ;;neutralize Alt. Not necessary with normal menus, but eg in Firefox and IE would show menu bar that normally is hidden. In IE still shows menu briefly, but the A hides it.

 gRet
ret CallNextHookEx(0 nCode wParam m)
