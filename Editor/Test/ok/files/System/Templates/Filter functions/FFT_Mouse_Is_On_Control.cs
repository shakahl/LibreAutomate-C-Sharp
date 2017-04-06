 /
 Allows starting macro when mouse pointer is on certain control.

function# iid FILTER&f

 f.hwnd2=child(mouse) ;;enable this if using with triggers other than mouse
if(!f.hwnd2) ret -2
if(!wintest(f.hwnd "WindowName" "WindowClass")) ret -2 ;;change window name and class

 if want to use control id
if(GetWinId(f.hwnd2)=15) ret iid ;;change control id (15)

 or, if want to use control text and class
 if(childtest(f.hwnd2 "ControlText" "ControlClass")) ret iid ;;change control text and class

ret -2
