out
str s
s.all(20)
 s="abė"
s="ab◊"
 s.len=0
 s.len=6
 s.len=0
 s.nc=2
 s.nc=3
 s[2]=0
 s[3]=0
 s[4]=0

 s.fix(-1)
 s.fix(-1 4)

 s.fix(3)
 s.fix(4 4)
 s.fix(6 2)
 s.fix(3 2|4)

out s
outb s s.len 1
