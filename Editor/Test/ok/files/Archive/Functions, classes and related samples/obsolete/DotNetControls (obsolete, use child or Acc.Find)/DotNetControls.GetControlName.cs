function! hwndCtrl str&name

 Gets name property of .NET control.
 Returns 1 if successful and the name is not zero length. Else returns 0.

 hwndCtrl - control handle. Must belong to the same process as the window used with Init.
 name - receives control name property (eg "Button1").


if(!m_hwnd) end ES_INIT
name.len=0
int r
if(!SendMessageTimeout(hwndCtrl m_msg 4096 m_pm.address SMTO_ABORTIFHUNG 10000 &r) or r<1) ret
m_pm.ReadStr(name r*2 0 1)
ret 1
err+
