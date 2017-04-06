 /CtoQM
function# $s $s1 $s2 str&struct [enum]

 Adds typedef to m_mtd.

int db
 db=1

ARRAY(str) a; str st cb ss; int i j ptr
if(tok(s2 a -1 ",;")<1)
	if(&struct) ret
	ret 1

if(!&struct)
	 untag target (tagX to X)
	s2=m_mtag.Get(s1)
	if(s2) s1=s2; goto g1
	 unalias target (DWORD to int, etc)
	if(Unalias(s1 st ptr 1 cb))
		st.set('*' st.len ptr)
		 out "%s %s" s1 st
		 if(st.beg(s1)) out "%s %s" s1 st
		s1=st
else if(enum) struct="#"; s1=struct
else
	if(!a[0].beg("*"))
		if(len(s1)) AddToMap(m_mtag s1 a[0] "" 1)
		struct=a[0]; a.remove(0); s1=struct
	else if(!len(s1)) struct.gett(a[0] 0); struct-"struct_"; s1=struct;; out s1
 g1
st=s1

 for each alias
for(i 0 a.len)
	s1=st
	str& alias=a[i]
	if(alias[0]='&') alias[0]='*'
	for(ptr 0 alias.len) if(alias[ptr]!='*') break
	if(ptr)
		s1=_s.fromn(st -1 alias ptr)
		alias.get(alias ptr)
		 sel(s1 2) case ["!*","void*"] db=1 ;;.
	
	if(!&struct)
		if(s1[len(s1)-1]='*')
			if(!strncmp(s1 "void*" 5)) j=5
			else if(!strncmp(s1 "!*" 2)) j=2
			else
				j=0
				if(alias[0]='H') if(!findrx(s1 "_\w+\*$")) s1="#";; db=1
			if(j)
				if(i=0 and (alias.beg("H") or matchw(alias "*HANDLE*"))) st.from("#" s1+j); s1=st;; db=1
				 else db=1 ;;char* to $, void* to !*. Add in AddTypes.
	
	if(!AddToMap(m_mtd alias s1 "" 1)) continue
	
	if(cb.len) AddToMap(m_mfcb alias cb)
	
	if(db) out "%-30s  ->  %s %s" s alias s1
