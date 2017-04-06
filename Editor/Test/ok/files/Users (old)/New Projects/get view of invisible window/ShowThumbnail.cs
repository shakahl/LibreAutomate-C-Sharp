 /dlg_window_thumbnail
function dc hwnd2

 Does not work.
 Works only in same process, and even then draws incorrectly.


SendMessage hwnd2 WM_PRINT dc PRF_CHILDREN|PRF_CLIENT|PRF_ERASEBKGND|PRF_NONCLIENT

 SendMessage hwnd2 WM_PRINTCLIENT dc PRF_CHILDREN|PRF_CLIENT|PRF_ERASEBKGND
