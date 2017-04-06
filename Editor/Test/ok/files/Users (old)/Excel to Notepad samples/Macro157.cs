act "Excel"
spe 0
str s
int i
ARRAY(str) a

rep
	s.getsel
	s.rtrim("[]")
	if(!s.len) break
	a[a.redim(-1)]=s
	key D

act "Notepad"
0.1

for i 0 a.len
	a[i].setsel
	key Y
