 /
function! hwnd

ret SendMessageTimeout(hwnd 0 0 0 0 1000 &_i)
