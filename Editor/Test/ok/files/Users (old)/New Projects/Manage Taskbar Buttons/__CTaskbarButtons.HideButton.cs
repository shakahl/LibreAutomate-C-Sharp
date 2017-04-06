function hwnd [flags] ;;flags: 1 also hide window, 2 also remove from Alt+Tab

 Hides the taskbar button and optionally the window.


if(!IsWindow(hwnd)) ret

int fl h
if(!GetWindow(hwnd GW_OWNER))
	fl|1
	if(flags&2=0) h=win("Hidden taskbar buttons" "QM_Tray_Class")
	if(!h) h=GetDesktopWindow
	SetWindowLong(hwnd GWL_HWNDPARENT h)
if(GetWinStyle(hwnd 1)&WS_EX_APPWINDOW)
	fl|2
	SetWinStyle hwnd WS_EX_APPWINDOW 6
if(fl) SetProp(hwnd "qm_tb_flags" fl)

if(flags&1) hid hwnd; err

if(!IsHiddenButton(hwnd)) a[a.redim(-1)]=hwnd
