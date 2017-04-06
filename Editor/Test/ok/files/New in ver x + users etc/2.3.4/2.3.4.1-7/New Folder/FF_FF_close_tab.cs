 /
function# iid FILTER&f

outw win ;;active window
outw f.hwnd ;;normally should be the same as above

if(wintest(f.hwnd "" "MozillaWindowClass")) ret iid ;;change window name and class
ret -2
