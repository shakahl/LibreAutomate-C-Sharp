function hwnd

 Unhides the taskbar button and the window.


hid- hwnd; err ret

int fl=GetProp(hwnd "qm_tb_flags")
if(fl&1)
	SetWindowLong(hwnd GWL_HWNDPARENT GetDesktopWindow) ;;without this does not add the button if owner is not desktop window
	SetWindowLong(hwnd GWL_HWNDPARENT 0)
if(fl&2)
	SetWinStyle hwnd WS_EX_APPWINDOW 5
RemoveProp hwnd "qm_tb_flags"

_i=IsHiddenButton(hwnd)
if(_i) a.remove(_i-1)
