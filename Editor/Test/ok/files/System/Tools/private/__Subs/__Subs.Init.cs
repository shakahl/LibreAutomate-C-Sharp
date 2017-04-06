function# [iid] [$txt] [defType] ;;defType: 0 dialog definitions, 1 menu definitions

 Gets text into sText. Gets sub-functions and dialog definitions (or menu, depeneding on defType) into a.
 Returns n definitions found.
 If txt, uses txt. Else if iid, gets iid text. Else gets code editor text.

if(txt) sText=txt
else sText.getmacro(iif(iid iid qmitem)); err sText=""

int nDef i
ARRAY(CHARRANGE) as
if(!sText.len) as.create(3 1)
else findrx(sText "(?ms)(?:\A|^#sub[ \t]+(\w+)[^[]]*[])(.*?)(?=[]#sub\b|\z)" 0 4 as)
a.create(as.len)

lpstr srx
sel defType
	case 0 srx=__S_RX_DDDE
	case 1 srx=__S_RX_MD

for i 0 a.len
	__Sub& r=a[i]
	CHARRANGE& c0(as[0 i]) c1(as[1 i])
	r.subOffset=c0.cpMin; r.subLen=c0.cpMax-c0.cpMin
	if i
		r.name.get(sText c1.cpMin c1.cpMax-c1.cpMin)
		r.codeOffset=as[2 i].cpMin
		r.codeLen=as[2 i].cpMax-r.codeOffset
	else r.codeLen=r.subLen
	
	 find dialog/menu definition in this sub
	FINDRX rx.ifrom=c0.cpMin; rx.ito=c0.cpMax
	ARRAY(CHARRANGE) k
	r.dd.offset=findrx(sText srx rx 0 k)
	if r.dd.offset>=0
		r.dd.len=k[0].cpMax-k[0].cpMin
		r.dd.innerOffset=k[1].cpMin
		r.dd.innerLen=k[1].cpMax-k[1].cpMin
		if(k.len>2) if(k[2].cpMin>0) r.dd.optionsOffset=k[2].cpMin
		nDef+1

ret nDef
