 /CtoQM
function# str&sn str&sm

 Converts interface (struct that looks like interface).

int db
str q ft sb comm; int i hr nothr abeg notb
ARRAY(str) a af aa

if(findrx(sn "^(?:__declspec\(uuid\(''\{?(.+?)\}?''\)\))?(\w+):public (\w+)$" 0 0 a)<0)
	if(findrx(sn "^(?:__declspec\(uuid\(''\{?(.+?)\}?''\)\))?(\w+)$" 0 0 a)<0) ret 50 ;;else no base specified
	if(sn.end(")IUnknown")) ret
	if(findrx(sm "\bQueryInterface\(.+AddRef\(.+Release\(")<0) ret ;;class, else base members specified explicitly
	a[a.redim(-1)]="IUnknown"
str& name=a[2]

q.format("interface %s :%s" name a[3])
for i 0 tok(sm af -1 ";")
	if(findrx(af[i] "^(\w+)( |\*+)(\w+)\((.*)\)(?:=0)?$" 0 0 aa)<0)
		if(findrx(af[i] "^()(\**)(\w+)\((.*)\)(?:=0)?$" 0 0 aa)<0) ret 52
		aa[1]="int"
	
	if(!notb and IsBaseMember(name aa[3] a[3] sb))
		 out "%s %s %s" name aa[3] a[3]
		continue
	notb=1
	
	hr=aa[1]="HRESULT" and !aa[2].beg("*")
	if(hr)
		ft="[h]"
	else
		nothr=1
		ConvType(aa[1] aa[2] ft 1 name)
	
	q.formata("[][9]%s%s" ft aa[3])
	
	abeg=q.len
	 g2
	if(!FuncArgs(aa[4] q 0 comm name))
		if(aa[4].replacerx("\bHRESULT\(\(HRESULT\)\w+\)" "HRESULT x")) q.fix(abeg); goto g2 ;;argument name expanded as macro, eg HRESULT((HRESULT)0xC7FF07D2L)
		ret 53
	q.set("(" abeg); q+")"
	 don't move retval arg type to func ret type, because it can be IN or IN/OUT
	
 g1
if(!nothr) q.findreplace("[h]"); q.insert("#" 9)
if(a[1].len)
	q.formata("[][9]{%s}" a[1])
	 also define IID_IInterface
	str sgn.from("IID_" name) sg.format("uuidof(''{%s}'')" a[1])
	AddToMap(m_mg sgn sg)
 else db=1

AddToMap(m_mi name q comm)
if(name.end("A")) ;;probably ANSI version. Also declare without A.
	name.fix(name.len-1)
	q.remove(10+name.len+(q[9]!=32) 1)
	AddToMap(m_mi name q comm)

if db
	out sm
	out q
