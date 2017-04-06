 /
function! hwnd

 Returns 1 if the window is a .NET window or control and you can get its name property. Else returns 0.

 hwnd - control or window handle.


ret SendMessageTimeout(hwnd RegisterWindowMessage("WM_GETCONTROLNAME") 0 0 SMTO_ABORTIFHUNG 10000 &_i) and _i>0
