 When new keyword is added, __Decrypt code (compiled code is embedded into QM,
 as binary resource) becomes invalid, and QM cannot run.
 To fix it, compile QM without protection (PROTECT undefined),
 compile this macro (when you compile it, resource file (binary) is produced
 and resource script mod time is updated), then recompile QM.


 def TEST 1

type __MIX a b c d [0]!x[16]
type __PROTECT !*kw2 *kw GetST STtoFT PostM daysleft $username $regcode SYSTEMTIME'first_run !ver

#ifdef TEST
int t1=perf
#endif

int i k
str a s.all(32 2)
__MIX mix
__PROTECT& pr=+getopt(__qmid 255)
int* p2=pr.kw2+100
a=pr.regcode
if(a.len<34) goto TRIAL
int newversion=a[32]='='

#ifdef TEST
rget a "QMX" "Software\GinDi"
#endif

 VERIFY REGCODE
 get name
str name.get(a 32+newversion); a.fix(32)
 create unmix table
 14 6 9 0 12 2 5 11 15 3 7 1 10 8 13 4
mix.a=0x0009060E; mix.b=0x0B05020C; mix.c=0x0107030F; mix.d=0x040D080A
 unmix
for(i 0 16)
	s[i+16]=a[mix.x[i]]
	s[i]=a[mix.x[i]+16]
 to binary
for(i 0 16) a[i]=(s[i<<1]-65)|(s[i<<1+1]-75<<4)
a.fix(16)
 unmix bits
int* p=a
k=p[0]
for(i 0 4)
	p[i]<<3
	if(i<3) p[i]|(p[i+1]>>29)
p[3]|(k>>29)
p2=pr.kw2+100 ;;stealth
 unmix characters
for(i 0 16) s[i]=a[mix.x[i]]
s.fix(16)
 unXOR/unmix bits
for(i 15 0 -1) s[i]^s[i-1]
s[0]^157
 unrandomize
k=s[13]&7
for(i 0 13) s[i]-i-k

 compare
pr.ver=s[14]-'A'

if(pr.ver&32) ;;temporary
	SYSTEMTIME stu stnow; long lu lnow
	lpstr s1=name+name.len-14
	stu.wMonth=val(s1)
	stu.wDay=val(s1+5)
	stu.wYear=val(s1+10)
	 out "%i %i %i" stu.wMonth stu.wDay stu.wYear
	call pr.GetST &stnow
	call pr.STtoFT &stnow &lnow
	call pr.STtoFT &stu &lu
	if(lnow>lu) goto TRIAL
	 out "%I64i %I64i" lnow lu

int ln=s[15]-'A'
if(ln>0 and ln<=10 and ln<=name.len)
	s.fix(ln)
	if(newversion)
		int half=name.len/2
		for(i 0 ln&1) if(s[i]!=name[i>>1+(i&1*half)]) goto TRIAL
	else
		for(i 0 ln) if(s[i]!=name[i]) goto TRIAL

	if(pr.ver&16) name+="[][]Quick Macros Pro license."
	pr.username=name; name.lpstr=0
	 ENABLE
	for(k 0 4) ;;stealth
		for(i 0 100)
			if(pr.kw2[i]=0) break
			pr.kw[pr.kw2[i]]=p2[i]
			if(k<16) continue ;;stealth
			pr.kw[p2[k]]=pr.kw2[i] ;;stealth
			s[i]>>4 ;;stealth
#ifdef TEST
		 int t2=perf
		 out t2-t1
#endif
		ret 1



 TRIAL
 get date
SYSTEMTIME st
SYSTEMTIME& st2=pr.first_run
long ft ft2
call pr.GetST &st ;;GetSystemTime

str rkdate.decrypt(8 "434C5349445C7B43344231314139352D423834322D324530372D303246392D3437433146323741334436327D5C496E50726F635365727665723332") ;;"CLSID\{C4B11A95-B842-2E07-02F9-47C1F27A3D62}\InProcServer32"
int version='C' ;;current version
pr.daysleft=0
pr.username=0

 get firstrun date from registry
if(rget(s "" rkdate 0x80000000 "")=0) goto SET DATE
if(s.len!=11 or s[2]<version) goto SET DATE;;if new version then reset trial period 
s[3]-70; s[4]-66
st2.wDay=s[3]<<1
st2.wMonth=s[4]
if(st2.wMonth>12) st2.wDay|1; st2.wMonth-12
st2.wYear=2000 + (s[5]-48*10) + (s[6]-48)
if(st2.wYear>2000) st2.wYear-30

#ifdef TEST
out "y=%i m=%i d=%i v=%i" st2.wYear st2.wMonth st2.wDay s[0]
#endif
 compare dates
p2=pr.kw2+100 ;;stealth
call pr.STtoFT &st &ft ;;SystemTimeToFileTime
call pr.STtoFT &st2 &ft2 ;;SystemTimeToFileTime
int d=ft-ft2/10000000/86400-1
if(d=-1) d=0
if(d>=0 and d<=45)
	pr.daysleft=45-d

 REMIND
call pr.PostM win("" "QM_Editor") 1124 0 0 ;;PostMessage
if(pr.daysleft) goto ENABLE
end

 SET DATE
pr.daysleft=45
s="MSA  00.DLL" ;;A is version. When it grows, reenable trial period.
s[2]=version
s[3]=st.wDay>>1+70
s[4]=st.wMonth
if(st.wDay&1) s[4]+12
s[4]+66
p2=pr.kw2+100 ;;stealth
if(st.wYear>2000 && st.wYear<2070)
	str sss.format("%0.2i" st.wYear-2000+30)
	s.set(sss 5 2)
int regmon=win("" "RegmonClass")
if(regmon) clo+ regmon; 1
rset s "" rkdate 0x80000000
#ifdef TEST
out s
#endif
goto ENABLE

 FOOLING CODE
if(st2.wMonth>12) st2.wDay|1; st2.wMonth-12
st2.wYear=2000 + (s[5]-48*10) + (s[6]-48)
if(st2.wYear>2000) st2.wYear-30
