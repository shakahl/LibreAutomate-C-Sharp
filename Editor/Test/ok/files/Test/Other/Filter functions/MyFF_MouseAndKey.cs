 /
function# iid FILTER&f

int t1=GetTickCount

str s; int i
9
s.getwintext(f.hwnd)
s.getwinexe(f.hwnd)
i=GetWinId(f.hwnd2)
s.getwintext(f.hwnd)
s.getwinexe(f.hwnd)
i=GetWinId(f.hwnd2)
s.getwintext(f.hwnd)
s.getwinexe(f.hwnd)
i=GetWinId(f.hwnd2)
s.getwintext(f.hwnd)
s.getwinexe(f.hwnd)
i=GetWinId(f.hwnd2)
 
out "%i(%s) %i(%i) %i %i %i" f.hwnd s f.hwnd2 i f.x f.y f.hit
ifa(win("Kkkkklklkl" "fffffffffff")) out 1
else ifa(win("a" "" "Bouytre")) out 1
else ifa(win("a" "" "Bouytre")) out 1
else ifa(win("a" "" "msdev")) out 1
else ifa(win("a" "" "winword")) out 1
else ifa(win("a" "" "Bouytre")) out 1
else ifa(win("a" "" "Bouytre")) out 1
else ifa(win("a" "" "Bouytre")) out 1
else ifa(win("a" "" "msdev")) out 1
else ifa(win("a" "" "winword")) out 1
else ifa(win("a" "" "Bouytre")) out 1

 out GetTickCount-t1

ret iid
