 /
function'long $timeSpanStr

 Creates time span from string.
 Error if timeSpanStr is invalid.

 timeSpanStr - string in one of the following formats.
   d
   d h
   d h:m
   d h:m:s
   d h:m:s.f
   h:m
   h:m:s
   h:m:s.f
   m:s.f
   s.f
   
   Here d h m s - days, hours, minutes, seconds. .f - fraction of second (.001 is 1 ms, .1 is 100 ms, .0000001 is 0.1 mcs).
   There is no m:s format. Use 0:m:s or m:s:0
   Whatever format is, the first part can be any positive or negative value. Other parts must be valid values (eg s 0-59).

 Added in: QM 2.3.2.


#opt nowarnings 1

long D H M S X; double F
long A1 A2 A3 A4 A5; long* a=&A1
int i j minus nSp nCol nPer
lpstr s(timeSpanStr) e

for i 0 4
	X=__Val(s &e 4); if(e=s) end ERR_BADARG
	if(X<0) if(i) end ERR_BADARG; else X=-X; minus=1
	a[i]=X
	sel e[0]
		case 0 break
		
		case ' '
		j=findcn(e " "); if(j<0) break
		if(i) end ERR_BADARG
		nSp=1; s=e+j
		
		case ':'
		if(nCol=2) end ERR_BADARG
		nCol+1; s=e+1
		
		case '.'
		F=val(e 2 j)
		j=findcn(e+j " "); if(j>=0) end ERR_BADARG
		if(F>=1.0) end ERR_BADARG
		F*10000000
		nPer=1; break
		
		case else
		end ERR_BADARG

i+1
if(i=1) a=iif(nPer &S &D)
else if(nSp) a=&D
else if(!nPer or i=3) a=&H
else a=&M
memcpy a &A1 i*8

if((H>=24 and a!&H) or (M>=60 and a!&M) or (S>=60 and a!&S)) end ERR_BADARG

X=TimeSpanFromParts(D H M S)
X+F
if(minus) X=-X
ret X
