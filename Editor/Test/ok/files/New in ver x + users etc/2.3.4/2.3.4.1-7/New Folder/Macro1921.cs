str s.flags=1
s.all(1000)
s="notepad.exe"
 s="C:\Windows\system32\notepad.exe"


Q &q
_s.searchpath(s)
 SearchPath 0 s 0 MAX_PATH s 0
 out PathFindOnPath(s 0)
 lpstr s1("q:\my qm") s2("q:\app")
 out PathFindOnPath(s &s1)
Q &qq; outq
out _s
 out s.lpstr
