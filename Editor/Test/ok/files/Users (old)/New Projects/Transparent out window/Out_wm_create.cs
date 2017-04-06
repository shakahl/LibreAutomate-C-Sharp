 /Out_Main
function hwnd
OUT_VAR- v
v.hwnd=hwnd
RECT r; GetClientRect(hwnd &r)
v.hwndedit=CreateControl(WS_EX_CLIENTEDGE "Edit" "" WS_VSCROLL|ES_AUTOVSCROLL|ES_MULTILINE 0 0 r.right r.bottom hwnd 1)
SetWindowPos hwnd HWND_TOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_NOZORDER|SWP_SHOWWINDOW

#opt err 1
type BLENDFUNCTION !BlendOp !BlendFlags !SourceConstantAlpha !AlphaFormat
SetLayeredWindowAttributes(hwnd 0 200 2)
 SetLayeredWindowAttributes(hwnd 0xffffff 255 3)
