 /
 Allows starting macro only in certain windows.

function# iid FILTER&f

if(wintest(f.hwnd "Window1[]Window2[]Window3" "" "" 16)) ret iid ;;edit the list of window names
ret -2
