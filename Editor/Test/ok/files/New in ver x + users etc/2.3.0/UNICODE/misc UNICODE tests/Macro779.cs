BSTR b.alloc(300)
int n=GetWindowTextW(_hwndqm b 300)
 out n
 out b.len
str s
 s=b
s=b.pstr
 s.ansi(b)
 s.ansi(b.pstr)
 s.ansi(b -1 n)
 s.ansi(b.pstr -1 n)
out s
out s.len

