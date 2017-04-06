out
type KEYDEBUG !vk !inj !wkey !kdown ti
int* p=share
int n=p[0]
if(!n) ret
KEYDEBUG* k=+(p+4)
int i
for i 0 n
	KEYDEBUG& r=k[i]
	str s.fix(0); FormatKeyString r.vk 0 &s
	out "%.*s%s inj=%i wkey=%i %i" !r.kdown "[9]" s r.inj r.wkey r.ti-k[0].ti
p[0]=0

#ret
