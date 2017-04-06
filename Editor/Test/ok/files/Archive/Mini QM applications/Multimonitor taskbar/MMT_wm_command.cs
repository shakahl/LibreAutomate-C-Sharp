 /MMT_Main
function wParam

MMTVAR- v

if(wParam<0 or wParam>=v.a.len) ret
act GetLastActivePopup(v.a[wParam].hwnd)
err
