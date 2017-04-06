 shows what is in winapiqm.txt but not in winapiqm2.txt

ClearOutput
str s sf
ARRAY(str) a1 a2
ARRAY(str)& a
int f2 t
str& sr

sf="$qm$\winapiqm.txt"
&a=&a1
 g1
sf.getfile(sf)
foreach s sf
	sel s 2
		case ["def *","type *"] t=1
		case ["dll *"] t=2
		case else continue
	&sr=a[a.redim(-1)]
	sr.gett(s t " ")
	sr.ltrim("#$@!%^")
if(!f2) f2=1; &a=&a2; sf="$qm$\winapiqm2.txt"; goto g1
 ret
 out a1.len
 out a2.len
int i j
for i 0 a1.len
	for j 0 a2.len
		if(a2[j]=a1[i]) break
	if(j=a2.len)
		out a1[i]
