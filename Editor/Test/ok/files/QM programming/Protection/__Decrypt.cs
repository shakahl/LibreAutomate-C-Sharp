 When a new keyword added, __Decrypt code (compiled code that is embedded into QM as binary resource) becomes invalid, and QM cannot run.
 To fix it:
   1. Compile QM without protection (PROTECT undefined). It is undefined in Debug configuration.
   2. Compile this macro. When you compile it, resource file (binary) is produced and resource script mod time is updated.
   3. Recompile QM with PROTECT defined (Release configuration).
 Also do it when something changed in compiled code format, eg in some functions changed/added/etc some bytes.
 Also do it when a blacklisted regcode added to this macro.
 Info: in this macro don't use UDFs. Don't use dll functions directly, only through __PROTECT. Don't use intrinsic functions whose address in table initially is 0. 
 Note: it seems that error handling does not work. On error this macro silently ends, even if err used.

 def TEST 1

type __MIX a b c d [0]!x[16]
type PROTECT_USAGE qmVer @nDays @lastDay
type __PROTECT !*kw2 *kw GetSTAFT STtoFT Crc Remind PROTECT_USAGE'u !*pRegOk $username $regcode !rcVersion ~regKey ~fileStream

int i k
str a s.all(32 2) ss
__MIX mix
__PROTECT& pr=+getopt(__qmid 255)

int* p2=pr.kw2+100
a=pr.regcode
if(a.len<34) goto TRIAL
int newversion
sel(a[32]) case '=' newversion=1; case '-' newversion=2

 VERIFY REGCODE
 get name
str name.get(a 32+(newversion!0)); a.fix(32)

 blacklisted regcodes
str sbl=
 LVZVTEVLPCLXLIDKLXZTNCPCNMORMFGC
 PRZPRNVGZIJNGEOIXPPNRBPLNOFZNGEP
 TPLTVBVGPHGZJGOGRPXLXBVGZBBRBGAP
 LLXTVATGVGGZGHOGPVTTNALGPPAXAGAP
foreach(ss sbl) if(a=ss) pr.regcode[0]=1; goto TRIAL
 note: blacklist only shared codes. Don't blacklist when refunded, because then may buy/refund again.

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
pr.rcVersion=s[14]-'A'

if(pr.rcVersion&32) ;;temporary
	SYSTEMTIME stu; long lu lnow
	lpstr s1=name+name.len-14
	stu.wMonth=val(s1)
	stu.wDay=val(s1+5)
	stu.wYear=val(s1+10)
	 out "%i %i %i" stu.wMonth stu.wDay stu.wYear
	call pr.STtoFT &stu &lu
	call pr.GetSTAFT &lnow
	if(lnow>lu) goto TRIAL
	 out "%I64i %I64i" lnow lu

int lnn=name.len
if(newversion=2) str name_.format("%010u" call(pr.Crc name lnn 0)); lnn=10

int ln=s[15]-'A'
if(ln>0 and ln<=10 and ln<=lnn)
	s.fix(ln)
	sel(newversion)
		case 0
		for(i 0 ln) if(s[i]!=name[i]) goto TRIAL
		
		case 1
		lnn/2
		for(i 0 ln&-2) if(s[i]!=name[i>>1+(i&1*lnn)]) goto TRIAL
		
		case 2
		for(i 0 ln) if(s[i]!=name_[i]) goto TRIAL

	if(pr.rcVersion&16) name+="[][]Quick Macros Pro license."
	pr.username=name; name.lpstr=0
	 ENABLE
	for(k 0 4) ;;stealth
		for(i 0 100)
			if(pr.kw2[i]=0) break
			pr.kw[pr.kw2[i]]=p2[i]
			if(k<16) continue ;;stealth
			pr.kw[p2[k]]=pr.kw2[i] ;;stealth
			s[i]>>4 ;;stealth
		 *pr.pRegOk=1+!pr.username
		*pr.pRegOk=1
		ret 1

 TRIAL
pr.username=0

 strings used by QM are hidden in this macro. Copy to pr.
pr.regKey="Software\Microsoft\Windows\CurrentVersion\Explorer\Syncmgr\Session"
pr.fileStream="$temp$:Session.Id"

 long dayNow
 call pr.GetSTAFT &dayNow ;;get current date
 dayNow=dayNow/864000000000-138426 ;;days from 1980-01-01

 get usage data from registry and file
def PU_SIZE sizeof(PROTECT_USAGE)
PROTECT_USAGE pu; int is
if(rget(pu "Id" pr.regKey)=PU_SIZE and pu.qmVer>=pr.u.qmVer) is|1
str fs.getfile(pr.fileStream 0 -1 128) ;;128 - no error. Because error handling does not work.
if(fs.len=PU_SIZE) PROTECT_USAGE& pu2=fs; if(pu2.qmVer>=pr.u.qmVer) is|2
 out "is=%i" is
sel is
	case 0 pu.nDays=0; pu.lastDay=0
	case 2 pu=pu2
	case 3 if(pu2.nDays>pu.nDays) pu=pu2

p2=pr.kw2+100 ;;stealth
pr.u=pu

call pr.Remind ;;ProtectRemind (PostMessage WMU100_THREAD_REG)
if(pr.u.nDays<=30) goto ENABLE

 err+ out _error.description; out _error.line ;;does not work
