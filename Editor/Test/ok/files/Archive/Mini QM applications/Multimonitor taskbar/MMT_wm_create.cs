 /MMT_Main
function hwnd

MMTVAR- v

v.hwnd=hwnd

 create child windows
int style=TBSTYLE_LIST|TBSTYLE_TOOLTIPS|WS_CLIPSIBLINGS|CCS_NODIVIDER|CCS_NORESIZE
v.htb=CreateControl(0 "ToolbarWindow32" "" style 0 0 1000 30 hwnd 3)
SetWindowTheme v.htb L"" L""
 SetWindowTheme v.htb L"" L"Taskbar"

 register appbar, set pos, show
APPBARDATA ab.cbSize=sizeof(ab); ab.hWnd=hwnd
ab.uCallbackMessage=WM_APP
SHAppBarMessage(ABM_NEW &ab)
MMT_SetPos
ShowWindow hwnd SW_SHOWNOACTIVATE

 add buttons
MMT_Buttons

 set timer to autorefresh
SetTimer hwnd 1 500 0
