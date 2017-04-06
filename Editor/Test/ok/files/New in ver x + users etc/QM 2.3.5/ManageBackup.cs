 /test backup
function str&sf t td

out "<><Z 0x80C080>-------- %i</Z>" td

int i ib
sf.set('?' sf.len-8 4)
str s1.getpath(sf "") s2.getfilename(sf 1)
ARRAY(str) af
GetFilesInFolder af s1 s2
if(af.len<2) ret
af.sort(0)
 out af; ret
ARRAY(POINT) at.create(af.len)
for(i 0 at.len) at[i].x=t-val(af[i]+sf.len-8)

 lpstr schedule="25 50 100 200 400"
lpstr schedule="25 100 400"
type BACKUP age
ARRAY(BACKUP) ab
rep() i=val(schedule 0 _i); if(!_i) break; else ab[].age=i; schedule+_i

int n(at.len-1) age ageMin agePrev
for ib ab.len-1 -1 -1
	BACKUP& b=ab[ib]
	age=b.age
	ageMin=iif(ib ab[ib-1].age 0)
	
	for(i 0 n) if(at[i].x<=age) break
	if(i=n) i-1; if(!af[i][0]) break
	else if(at[i].x<=ageMin)
		int addOlder=0
		if(i and af[i-1][0]) if(!agePrev or agePrev-at[i-1].x>=age/2) addOlder=1
		if(addOlder) i-1; out "more, %i" at[i].x
		else continue
	af[i][0]=0; at[i].y=age; agePrev=at[i].x

for i 0 n
	if(af[i][0]) del- af[i]; out "<><c 0xff>%i</c>" at[i].x
	else out "%i %i %s" at[i].x at[i].y af[i]+s1.len+1
