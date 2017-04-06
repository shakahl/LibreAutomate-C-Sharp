 /
 Allows starting macro only in certain window.

function# iid FILTER&f

if(wintest(f.hwnd "WindowName" "WindowClass")) ret iid ;;change window name and class
ret -2
