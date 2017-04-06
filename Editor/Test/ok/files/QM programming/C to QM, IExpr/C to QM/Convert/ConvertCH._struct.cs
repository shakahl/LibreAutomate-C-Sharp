 /CtoQM
function# str&sn str&sm !union [!_nested]

 Converts struct or union.

 out "'%s' '%s'" sn sm

int db=0
str sss=sm

int hr i nested ve
str q at s ss base se comm ntype
ARRAY(str) a aa an

if(findt(sn 1)>=0)
	if(findrx(sn "^(\w+):(?:public )?(\w+)$" 0 0 a)=0) ;;base
		sn=a[1]
		ConvType(a[2] "" ss 0 sn)
		base.format(" :%s_base" ss)
		 out "%s %s" sn base
	else ret 21

if(sn.end("__") and sm="int unused;") ;;handle
	AddToMap(m_mtd sn.fix(sn.len-2) "#" "" 1)
	ret

rep ;;sizeof("string")
	if(findc(sm 34)<0) break
	i=findrx(sm "\bsizeof\(''(.*)''\)" 0 0 a); if(i<0) ret 22
	s=len(a[1])+1
	sm.replace(s i a[0].len)

if(findc(sm '(')>=0)
	if(sn="NS_CONTEXT_ATTRIBUTES") sm.replacerx("struct (\w+)\(\*(\w+)\)\[\]" "$1*$2") ;;error in definition
	 else if(sm.replacerx("\w+\**\([^\)]*?(\w+)?\)\([^\)]*\)" "int fa_$1")) ;;(*functiontype)(args)
	else if(sm.replacerx("\w+\**\(\*(\w+)?\)\([^\)]*\)" "int fa_$1")) ;;(*functiontype)(args)
	else if(findrx(sm "\w+\(.*?\)[;\{]")>=0) if(!(m_needclasses and RemoveClassFunctions(sm))) ret ;;member function
	else if(findrx(sm "\[.*\(.*\]")>0) ;;ok if in [ ]
	else if(sn="MSP_EVENT_INFO") sm.findreplace("(HRESULT)0xC7FF07D2" " hrError") ;;conflicts macro name and member name
	else ret 23

if(findc(sm ':')>=0)
	sm.replacerx("\b(public|private|protected):")
	for i 1 1000 ;;bitfield
		if(findc(sm ':')<0) break
		if(sm.replacerx("(\w+) ([\w ]+:\d+[,;])+" s.format("$1 bf%i;" i) 4)<0) ret 24

if(m_needclasses and find(sm "friend")>=0) sm.replacerx("\bfriend .+?;")

rep ;;TYPE m1, m2, ...; -> TYPE m1; TYPE m2; ...
	if(findc(sm ',')<0) break
	 if(!sm.replacerx("(\w+) (\w+)," "$1 $2;$1 "))
	if(!sm.replacerx("(\w+)( |\*+)(\w+)," "$1$2$3;$1 "))
		if(sn="IPX_PATTERN") ret ;;struct{...}a,b;
		ret 25
	sm.findreplace(" *" "*")

rep ;;nested type
	i=findc(sm '{'); if(i<0) break
	if(i and sm[i-1]=')') ret ;;member function
	nested+1
	i=findrx(sm "\b(union|struct|enum)(?: \w+)?(\{((?>[^{}]+)|(?2))*\})(\w+(?:\[.*?\])?)?;" 0 0 a); if(i<0) ret 26
	
	if(a[1].beg("e")) ntype="int" ;;enum
	else
		ntype.format("__%s%i" sn nested)
		a[2].get(a[2] 1 (a[2].len-2))
		hr=_struct(ntype a[2] a[1].beg("u") 1); if(hr) ret hr
		if(!a[4].len) an[an.redim(-1)]=ntype
	
	if(!a[4].len) a[4].from("_" nested)
	s.format("%s %s;" ntype a[4])
	sm.replace(s i a[0].len)
	 db=1

sm.replacerx("\[\]" "[91]1]") ;;[] to [1]

if(m_crt and find(sn "stat")>=0) ;;type name same as function name
	 out sn
	ss=sn; ss.ucase
	AddToMap(m_mtd sn ss "" 1)
	sn=ss

if(findrx(sm "[^; 0-9A-Z_a-z\*\[\]]")>=0)
	if(findrx(sm "\[.+?[^; 0-9A-Z_a-z\*\[\]].*\]")<0) ;;ok if in [ ]
		ret 28

q.format("type %s%s" sn base)
for i 0 tok(sm a -1 ";")
	if(findrx(a[i] "^([A-Z_]\w*)(\*+| )([A-Z_]\w*)(\[.*?\])?$" 0 1 aa))
		if(findrx(a[i] "^()(\**)([A-Z_]\w*)(\[.*?\])?$" 0 1 aa)=0) aa[1]="int"
		else ret 29
	
	ConvType(aa[1] aa[2] at 0 sn)
	
	if(aa[4].len) ;;array
		se.get(aa[4] 1 (aa[4].len-2))
		
		if(find(se "][")>0) ;;multidim -> singledim
			se.findreplace("[" "("); se.findreplace("]" ")"); se.findreplace(")(" ")*("); se-"("; se+")"
		
		if(findrx(se "[^\d]")>=0) ;;calculate expression
			ve=m_expr2.EvalC(se); err ve=-1
			if(ve>0) aa[4].from("[" ve "]")
		else ve=val(se)
		
		if(!ve) ;;zero-sized array
			comm.from(" ;;" sm)
			 db=1
			break
	
	if(union and i) q.formata(" [91]]%s%s%s" at aa[3] aa[4])
	else q.formata(" %s%s%s" at aa[3] aa[4])

 pack?
if(m_pack!8 and !_nested and !union) q.formata(" [pack%i]" m_pack)

 replace nested types
for i 0 an.len
	if(!m_mt.Get2(an[i] s)) continue
	ss.format("%s'\w+(\[1\])?(?= |$)" an[i])
	s.gett(s 2 " " 2); s-"{"; s+"}"
	if(!q.replacerx(ss s 4)) continue
	m_mt.Remove(an[i])
	rep() if(!m_mut.Get2(an[i] s)) break; else m_mut.Remove(an[i]); AddToMap(m_mut sn s "" 1);; out "%s, in %s" s an[i]
	 db=1

AddToMap(m_mt sn q comm)
if(sn.end("A") and !sn.end("DATA") and !sn.end("PARA")) ;;also declare without A
	sn.fix(sn.len-1)
	q.remove(5+sn.len 1)
	AddToMap(m_mt sn q comm)
else if(sn="tagPROPVARIANT")
	sn="PROPVARIANT"
	q.findreplace("tagPROPVARIANT" "PROPVARIANT" 2)
	AddToMap(m_mt sn q comm)
	 db=1

if db
	 out "  %s %s" sn sss
	out "%s %s" q comm

 NOTES
 Possible incorrect alignment because converts without regard to __declspec(align). tested: in Win7 SDK only several not useful structs use this (align(16)).
