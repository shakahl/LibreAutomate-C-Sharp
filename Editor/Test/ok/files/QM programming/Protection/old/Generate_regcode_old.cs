function str'n ;;name
if(!n.len) n.getclip
if(n.len=0) ret
 n="Bill Smith"
 n ="Gintaras Didzgalvis"
 n="J. Lee"
 n="B"
 n="Ąčęėįk"

dll msvcrt #_ultoa i $buf j

str a.all(16 2) name.all(n.len*3)
int i j k ln tick=GetTickCount
 prepare name
for(i 0 n.len)
	if(n[i]>33 and n[i]<127 and n[i]!='%' and n[i]!=34 and n[i]!=91) name[j]=n[i]
	else name[j]='%'; j+1; _ultoa(n[i], name+j, 16); j+1
	j+1
name.fix(j); ln=iif(name.len>10 10 name.len)
 generate random string and set name
srand(tick)
str s.format("%.10s%04X%04x%04X%04x" name rand rand rand rand)
 randomize again
k=s[13]&7
for(i 0 13) s[i]+i-k
s[14]='A' ;;version
s[15]=ln+65 ;;length
s.fix(16)
 XOR/mix bits
k=157
for(i 0 16) s[i]^k; k=s[i]
 create mix table
str fm.all(32 2)
word* wp=fm
int pc=25000+381
for(i 0 16) wp[i]=pc
str mix.format(fm 14 6 9 0 12 2 5 11 15 3 7 1 10 8 13 4)
 mix characters
for(i 0 16) a[mix[i]]=s[i]
 mix bits
int* p=a
k=p[3]&7
for(i 0 4)
	j=p[i]&7
	p[i]>>3
	p[i]|(k<<29)
	k=j
 make readable
s.all(32 2)
for(i 0 16)
	s[i<<1]=a[i]&0xF+65
	s[i<<1+1]=a[i]>>4+75
 mix characters
a.all(32 2)
for(i 0 16)
	a[mix[i]]=s[i+16]
	a[mix[i]+16]=s[i]
 append name
a+name
 _____________________________

 TEST
rset a "QMX" "Software\GinDi"
outp a
0.1
