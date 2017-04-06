 /
 Allows starting macro only in certain window.

function# iid FILTER&f

 out _s.getmacro(iid 1)
 outw f.hwnd
 _s.getstruct(f 1)
 out _s

outTsmTrigger f

if(wintest(f.hwnd "Notepad" "Notepad")) ret iid ;;change window name and class
ret -2

#ret
