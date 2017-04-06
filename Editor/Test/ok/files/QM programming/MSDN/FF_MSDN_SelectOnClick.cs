 /
 Allows starting macro when mouse pointer is on certain control.

function# iid FILTER&f

int controlid = 1001 ;;change control id

if(!f.hwnd2) ret
if(GetWinId(f.hwnd2)=controlid) ret iid
