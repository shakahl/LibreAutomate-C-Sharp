 /CtoQM
function# str&s

 Adds #define macro to map.
 Simple macros are added to m_mc, function-style macros to m_mcf.
 Does not expand.

int db dupl
ARRAY(str) a
lpstr sad
str& name r

if(findrx(s m_rx.define 0 0 a)<0)
	if(findrx(s m_rx.definef 0 0 a)<0) ret 1
	 function-style macro
	&name=a[1]; &r=a[2]
	if(!AddToMap(m_mcf name r "" 1))
		sad=m_mcf.Get(name)
		if(r=sad) ret
		dupl=1
else ;;simple macro
	&name=a[1]; &r=a[2]
	
	if(!r.lpstr) r=""
	else if(name="_VARIANT_BOOL") r="VARIANT_BOOL"
	
	sad=m_mc.Get(name)
	if(sad)
		if(r=sad) ret
		if(find(name "_SIZE")>0) ret ;;added in AddDef
		dupl=2
	AddToMap(m_mc name r)

if(db) out "%s[]%s %s" s name r

if(dupl)
	str s1(sad) s2(r)
	if(dupl=2)
		if(s1.end("L") or s1.end("U")) if(s2.len=s1.len-1 and s1.beg(s2)) ret
		else if(s2.end("L") or s2.end("U")) if(s1.len=s2.len-1 and s2.beg(s1)) ret
		
	s1.findreplace(" ")
	s2.findreplace(" ")
	if(s1=s2) ret
	
	out "Warning in %s: %s is already defined. To change, use #undef. Leaving old value.[][9]old: %s, new: %s" _s.getfilename(m_file 1) name sad r
