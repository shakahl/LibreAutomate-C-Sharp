dll user32 @*CharLowerW @*lpsz

out
str s s1 s2
int i
BSTR w1.alloc(10) w2.alloc(10)
w1[1]=0; w2[1]=0
for i 0x80 0x10000
	int lo=CharLowerW(+i)
	if(lo=i) continue
	w1[0]=i; w2[0]=lo
	s1.ansi(w1.pstr)
	s2.ansi(w2.pstr)
	if(s2[0]=s1[0]) continue
	s.format("0x%X 0x%X %s %s" i lo s1 s2)
	out s
	