 /dialog_QM_Tools
function# hwnd message wParam lParam

__ToolsControl* x
if(message=WM_NCCREATE) SetWindowLong hwnd 0 x._new
else x=+GetWindowLong(hwnd 0); if(!x) ret DefWindowProcW(hwnd message wParam lParam)
int R=x.WndProc(hwnd message wParam lParam)
if(message=WM_NCDESTROY) SetWindowLong hwnd 0 x._delete
ret R
