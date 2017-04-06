 /
 Allows starting macro when mouse pointer is on certain control.

function# iid FILTER&f

 zw f.hwnd2


ret iid

 int controlid = 0 ;;change control id
 
 if(!f.hwnd2) ret
 if(!wintest(f.hwnd "WindowName" "WindowClass")) ret ;;change window name and class
 if(GetWinId(f.hwnd2)=controlid) ret iid
