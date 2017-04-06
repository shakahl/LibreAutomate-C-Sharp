 /
function# WTI&t cbParam

out cbParam
out t.txt
RECT r=t.rv; MapWindowPoints t.hwnd 0 +&r 2 ;;from t.hwnd client to screen
OnScreenRect 1 &r; 1; OnScreenRect 2 &r
 ret 1
