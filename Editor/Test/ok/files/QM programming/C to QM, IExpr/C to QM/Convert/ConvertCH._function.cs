 /CtoQM
function# $s

 Converts dll function.

int i hr db
str q ft dln truename comm
ARRAY(str) a

if(matchw(s "typedef *")) ret _callback(s+8)

if(findrx(s "^(?:struct |enum |union |_struct_from_class )?(\w+)?( |\*+)(\w+)\((.*)\);$" 0 0 a)<0)
	if(findrx(s "\b(operator|__fastcall|_getdllprocaddr)\b")>=0) ret
	if(findrx(s "^()(\**)(\w+)\((.*)\);$" 0 0 a)=0) ft="#" ;;func type not specified, use int
	else ret 63
str& name=a[3]

if(m_compact) if(IsFuncNotUseful(name)) ret

if(GetDllName(name dln truename))
	if(truename.len) truename-"["; truename+"]";; db=1
	dln.lcase
else
	 sel(name 2)
		 case ["*_UserSize*","*_UserMarshal*","*_UserUnmarshal*","*_UserFree*","MIDL_*","I_*","Rpc*","Ndr*"]
		 if(m_compact) ret
		  case else db=1
		 case else
		 out s
	if(m_compact and m_mfdn.Count) ret
	 NOTE: make sure that using newest dll names file

if(!ft.len) ConvType(a[1] a[2] ft 1 name)
q.format("dll %s %s%s%s" iif(dln.len dln "???") truename ft name)

if(!FuncArgs(a[4] q name comm name))
	sel(s 2)
		case "*(MkErr(*));" ;;error code created using inline function
		 out "z('' %s #'', %s);" name name ;;MSVC fails to compile
		ret
		case "*(*);*(*);*" ;;several functions in single line
		for i 0 tok(s a -1 ";")
			hr=_function(q.from(a[i] ";")); if(hr) ret hr
			ret
		case ["*signal(int*","*srv_errhandle(int*"] ret
	ret 64

AddToMap(m_mf name q comm)

 db=1
if db
	 out s
	out q
	 out name
	
