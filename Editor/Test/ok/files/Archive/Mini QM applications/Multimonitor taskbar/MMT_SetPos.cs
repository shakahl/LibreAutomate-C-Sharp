 /MMT_Main

MMTVAR- v

int height=SendMessage(v.htb TB_GETBUTTONSIZE 0 0)>>16+6
APPBARDATA ab.cbSize=sizeof(ab); ab.hWnd=v.hwnd
ab.uEdge=ABE_BOTTOM
MonitorFromIndex v.monitor 0 &ab.rc
SHAppBarMessage(ABM_QUERYPOS &ab)
ab.rc.top=ab.rc.bottom-height
SHAppBarMessage(ABM_SETPOS &ab)
SetWindowPos v.hwnd 0 ab.rc.left ab.rc.top ab.rc.right-ab.rc.left ab.rc.bottom-ab.rc.top SWP_NOACTIVATE 
