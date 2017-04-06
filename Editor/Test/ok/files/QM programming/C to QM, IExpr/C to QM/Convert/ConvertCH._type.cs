 /CtoQM
function# $s typedef

 Called for everything that looks like a type declaration (struct, union, enum, _struct_from_class).
 Calls _struct or _interface or _enum. Also may call _typedef.

if(find(s "]={")>0)
	 out s
	ret

int hr i
ARRAY(str) a
if(findrx(s "^(struct|union|enum|_struct_from_class) *(.*?)?\{(?:public:)?(.*)\}(.+?)$" 0 0 a))
	 ret 11
	ret -1 ;;all GUID
str& s1=a[1]
str& s2=a[2]
str& s3=a[3]
int enum=s1[0]='e'

if(typedef)
	if(s2.beg("__declspec(")) s2.replacerx("^__declspec\(.+\)(\w+)" "$1");; out s2
	i=findc(s2 ':'); if(i>0) str sb=s2+i; s2.fix(i) ;;*1
	
	hr=_typedef(s s2 a[4] s2 enum); if(hr) ret hr
	
	if(sb.len) s2+sb ;;*1
	 out s2
else if(!s2.len and !enum) ret 12

sel s1[0]
	case 's'
	if(!s2.len or find(s3 ")=0")>0 or (!s3.len and findc(s2 ':')>0)) hr=_interface(s2 s3)
	else hr=_struct(s2 s3 0)
	case 'u' hr=_struct(s2 s3 1)
	case 'e' hr=_enum(s2 s3)
	case '_' hr=_struct(s2 s3 0)

ret hr

 ;;*1 - quick bug fix where base type would be removed.
