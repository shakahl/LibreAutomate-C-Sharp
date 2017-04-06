out
int i
str s.all(300)
for i 0xc000 0x10000
	if(!GlobalGetAtomName(i s 300)) continue
	out s.lpstr
