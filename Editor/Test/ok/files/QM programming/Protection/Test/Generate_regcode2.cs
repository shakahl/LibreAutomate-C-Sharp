 /Test keygen

 This was used when creating/debugging php keygen (regsoft.php).

function str'name
 if(name.len=0) ret
 name="Bill Smith"
 name ="Gintaras Didzgalvis"
 name="J. Lee"
 name="Bcdefg"
 name="Ąčęėįk"
 name="abcd efgh"

dll msvcrt #_ultoa i $buf j

str a.all(16 2) ename.all(name.len*3); name.all(name.len*3 1)
int i j k ln tick=GetTickCount
 prepare name
  escape
for(i 0 name.len)
	if(name[i]>33 and name[i]<127 and name[i]!='%' and name[i]!=34 and name[i]!=91) ename[j]=name[i]
	else ename[j]='%'; j+1; _ultoa(name[i], ename+j, 16); j+1
	j+1
ename.fix(j)
if(j<10 and j&1) ename+"%20"; j+3 ;;fix bug
  pre-mix
ln=j/2
for(i 0 j&-2) name[i]=iif(i&1 ename[i>>1+ln] ename[i>>1])
if(j&1) name[j-1]=ename[j-1]
  fix(10)
name.fix(j); ln=iif(name.len>10 10 name.len)
 generate random string and set name
srand(tick)
str s.format("%.10s%04X%04x%04X%04x" name rand rand rand rand)

 s="abcdefghij1234567812345678"

 randomize again
k=s[13]&7
for(i 0 13) s[i]+i-k
s[14]='A' ;;version
s[15]=ln+65 ;;length
s.fix(16)

 out s

 XOR/mix bits
k=157
for(i 0 16) s[i]^k; k=s[i]

 out s

 create mix table
str fm.all(32 2)
word* wp=fm
int pc=25000+381
for(i 0 16) wp[i]=pc
str mix.format(fm 14 6 9 0 12 2 5 11 15 3 7 1 10 8 13 4)
 mix characters
for(i 0 16) a[mix[i]]=s[i]

 out a

 mix bits
int* p=a
k=p[3]&7
for(i 0 4)
	j=p[i]&7
	p[i]>>3
	p[i]|(k<<29)
	k=j

 out a

 make readable
s.all(32 2)
for(i 0 16)
	s[i<<1]=a[i]&0xF+65
	s[i<<1+1]=a[i]>>4+75

 out s

 mix characters
a.all(32 2)
for(i 0 16)
	a[mix[i]]=s[i+16]
	a[mix[i]+16]=s[i]
 append name
a+"="
a+ename

 _____________________________

 TEST
rset a "QMX" "Software\GinDi"
out a
 0.1
