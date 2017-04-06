 /CtoQM
function# $s str&sout &ptr [typedef] [str&cb]

 Unaliases (DWORD to #, etc).

lpstr s2
int r i cs

rep
	if(!typedef)
		sel s
			case "void" sout=iif(ptr "!" ""); ret 1
			case "char" if(ptr) ptr-1; sout="$"; ret 1
	
	s2=m_mtd.Get(s)
	if(!s2 or !strcmp(s2 s)) break
	cs=__iscsym(s2[0])
	if(cs and m_mt.Get(s) and strcmp(s2 "CURRENCY")) break
		 out "%s %s" s s2

	r|1
	sout=s2
	i=sout.len; sout.rtrim("*"); ptr+i-sout.len
	if(!cs)
		if(&cb and sout="#")
			s2=m_mfcb.Get(s)
			if(s2) cb=s2; r|2
		break
	s=sout

if(r&1 and cs)
	sel sout
		case "RTL_CRITICAL_SECTION" sout="CRITICAL_SECTION"
		case "uSTGMEDIUM" sout="STGMEDIUM"

ret r
