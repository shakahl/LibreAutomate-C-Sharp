 QM regcode generator.
 Used to generate free and temporary regcodes.
 PayPal and ShareIt use pp_ipn.php and shareit.php.

function~ str'name [flags] [ver] [tempWeeks] ;;flags: 1 temporary, 2 pro;  ver: 0 QM 2.0.x, 1 QM 2.1.x, 2 QM 2.2.x

int test=!getopt(itemid 1)

if(test)
	name="Bill Smith"
	 name ="Gintaras Didzgalvis"
	 name="J. Lee"
	 name="Bcdefg"
	 name="Ąčęėįk"
	 name="abcd efgh"
	 name="Hung Le"
	ver=2
	flags=1

if(name.len=0) ret

dll msvcrt #_ultoa i $buf j

if(!_unicode) end "must be Unicode mode"
 name.ConvertEncoding(_unicode 28591) ;;used to be ISO-8859-1. Now all (DB, share-t, PayPal) are set to use UTF-8.

if flags&1
	if(!tempWeeks) tempWeeks=1
	name+_s.time(", until %m/%d/%Y" "" 60*60*24*7*tempWeeks) ;;temporary regcode

str a.all(16 2) sep ename
int i j k ln tick=GetTickCount

  escape
ename=name; ename.escape(9)
j=ename.len

 prepare name
if(ver<2) ;;QM ver <2.2.0
	sep="="
	 fix bug
	if(j<10 and j&1) ename+"%20"; j+3
	  pre-mix
	ln=j/2
	name.all(name.len*3)
	for(i 0 j&-2) name[i]=iif(i&1 ename[i>>1+ln] ename[i>>1])
	if(j&1) name[j-1]=ename[j-1]
	  fix(10)
	name.fix(j)
else
	sep="-"
	  crc
	name.format("%010u" Crc32(ename j))
ln=iif(name.len>10 10 name.len)

 generate random string and set name
srand(tick)
str s.format("%.10s%04X%04x%04X%04x" name rand rand rand rand)
 randomize again
k=s[13]&7; for(i 0 13) s[i]+i-k
 set misc info
s[14]='A' ;;version
if(flags&1) s[14]+32 ;;temp
else if(flags&2) s[14]+16 ;;pro
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
a+sep
a+ename
 _____________________________

rset a "QMX" "Software\GinDi"
if(test) out a
ret a
