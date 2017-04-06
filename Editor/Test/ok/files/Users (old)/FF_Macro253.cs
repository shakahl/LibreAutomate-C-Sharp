 /
function# iid FILTER&f

if(!f.hwnd2) ret
if(!wintest(f.hwnd "" "Shell_TrayWnd")) ret
if(!childtest(f.hwnd2 "Quick Launch" "ToolbarWindow32" f.hwnd)) ret

Acc a=acc(mouse); err ret
if(!a.elem) ret ;;not on button
str s=a.Name
mac "Macro253" s
ret -1
