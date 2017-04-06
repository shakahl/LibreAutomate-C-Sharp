BSTR b="abc[161]"
 BSTR b="abcڲ"
b[3]=0x6b2
str s
 s=b
 s.ansi(b.pstr)
 s.ansi(b.pstr CP_ACP)
s.ansi(b.pstr CP_UTF8)
out s
outb s s.len 1
out s.len

 s.unicode(s 0)
s.unicode(s CP_UTF8)
outb s s.len 1
out s.len

