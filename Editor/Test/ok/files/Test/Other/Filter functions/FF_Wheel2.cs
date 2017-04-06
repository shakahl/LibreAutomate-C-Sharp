 /
function# iid FILTER&f

if(!f.hwnd2) ret
 if(!wintest(f.hwnd "Word" "OpusApp")) ret
if(!child("Hex" "Button" f.hwnd)) ret
ret iid
