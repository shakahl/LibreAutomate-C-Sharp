 /
 Each macro (key or mouse triggered) that has this filter
 function, will run only if Notepad window is active.

function# iid FILTER&f

if(wintest(f.hwnd "" "Notepad")) ret iid
