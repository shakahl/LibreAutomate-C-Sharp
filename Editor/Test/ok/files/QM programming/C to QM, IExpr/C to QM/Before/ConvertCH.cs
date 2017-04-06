 /CtoQM
m_aems.create(500) ;;used with constant expand recursion. Need so big because elements of some large enums are defined as CONSTNEXT=CONSTPREV+1 (nonsense).
m_s.flags=3

m_mall=CreateStringMap(0)
m_mh=CreateStringMap(1)
m_mf=CreateStringMap(0)
m_mfcb=CreateStringMap(0)
m_mt=CreateStringMap(0)
m_mtd=CreateStringMap(0)
m_mi=CreateStringMap(0)
m_mg=CreateStringMap(0)
m_mc=CreateStringMap(0)
m_mcf=CreateStringMap(0)
m_mtag=CreateStringMap(0)
m_mcomm=CreateStringMap(0)
m_mfdn=CreateStringMap(2)
m_mfan=CreateStringMap(2)
m_mut=CreateStringMap(8)
m_mpch=CreateStringMap(2)
m_mo=CreateStringMap(2)

m_expr=CreateExpr
m_expr.SetIdMap(m_mc)
m_expr.SetIdFunc(&sub.UnkId &this)
m_expr2=CreateExpr
m_expr2.SetIdMap(m_mc)
m_expr2.SetIdFunc(&sub.UnkId2 &this)

str* sp=&m_rx
str s
lpstr slist=
 ^#define +([\w\$]+)(?: +(.+))?$
 ^#define +([\w\$]+)(\(.*?\)(?: *(?:.+))?)$
 (?i)\b([A-Z_]\w*) *|["']
  +(?=\W)
 (?<=\W) +
 (?i)(?<![\w\)])\(([A-Z_]\w*\**)\)(?=\(*-?~?\(*[\w'])
 (?i)\b(\d+U?|0x[0-9A-F]+U?)L\b
 (?<!\w)\((\d+)\)
 (?i)(?<!\w)\((0x[\dA-F]+)\)
foreach(s slist) findrx("" s 0 128 sp[0]); sp+sizeof(str)


#sub UnkId
function# $ID &r flags ConvertCH&ch

 Callback function of m_expr, which is used to calculate #if expressions.
 Called on unknown identifier. Calculates sizeof("string"), defined(macro), etc.

 out ID
str s
if(flags&1)
	sel ID 2
		case "defined*(*)"
		if(findrx(ID "^defined *\( *(\w+) *\)" 0 0 s 1)<0) end "defined() error" 1
		 out s
		lpstr ss=ch.m_mc.Get(s); if(!ss) ss=ch.m_mcf.Get(s)
		if(!ss) r=0; else r=1
		ret 1
		
		case "sizeof*(*)"
		 out ID
		if(findrx(ID "^sizeof *\( *(L?''.*'') *\)" 0 0 s 1)<0) ret ;;not string
		if(!EscapeCString(&s 1)) end s 1
		r=s.len-1; if(s[0]='L') r=r-1*2
		ret 1
else
	 out ID
	r=0; ret 1 ;;#if SOMETHING_NOT_DEFINED  is equal to #if SOMETHING_IS_0


#sub UnkId2
function# $ID &r flags ConvertCH&ch

 Callback function of m_expr2, which is used to calculate expressions in constants, but not used with #if.
 Called on unknown identifier, and calculates sizeof("string") and sizeof(type).

 out ID
str s
if(flags&1)
	sel ID 2
		case "sizeof*(*)"
		 out ID
		if(findrx(ID "^sizeof *\( *(L?''.*'') *\)" 0 0 s 1)>=0) ;;string
			if(!EscapeCString(&s 1)) end s 1
			r=s.len-1; if(s[0]='L') r=r-1*2
			ret 1
		else if(findrx(ID "^sizeof *\( *(\w+) *\)" 0 0 s 1)>=0) ;;type
			s=ch.m_mtd.Get(s); if(!s.len) ret
			sel(s[s.len-1])
				case ['#','$','*'] r=4
				case '@' r=2
				case '!' r=1
				case ['%','^'] r=8
				case else ret
			 out "%s %i" ID r
			ret 1
else
	 out ID
	ret

 ret 1
