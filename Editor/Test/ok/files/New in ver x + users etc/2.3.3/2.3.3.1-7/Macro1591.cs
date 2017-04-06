str s="c:\program files\some program\some folder\some file.txt"

 out PathCompactPathEx(s s 30 0)
 out s.lpstr

_s.all(300)
out PathCompactPathEx(_s s 17 0)
out _s.lpstr
