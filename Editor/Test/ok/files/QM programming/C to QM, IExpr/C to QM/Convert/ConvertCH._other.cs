 /CtoQM
function# str&s

 Called for everything that is not a function, type, typedef, interface, enum, etc.
 Mostly converts GUIDs.

ARRAY(str) a
lpstr rx1 rx2 rx3; str s1 s2
rx1="^\w+ (\w+)(?:\[.*?\])?=(L?''.+'');$"
rx2="^\w+ (\w+)=(.+)L?;$"
 rx3="(?:GUID|IID) (\w+)=\{0x(\w+?)L?,0x(\w+),0x(\w+),\{?0x(\w+),0x(\w+),0x(\w+),0x(\w+),0x(\w+),0x(\w+),0x(\w+),0x(\w+)\}?\};$"
rx3="\w+ (\w+)=\{(\w+?)L?,(\w+),(\w+),\{?(\w+),(\w+),(\w+),(\w+),(\w+),(\w+),(\w+),(\w+)\}?\};$"

if(s.beg("class "))
	if(findrx(s "^class __declspec\(uuid\(''\{?(.+?)\}?''\)\)(\w+);$" 0 0 a)=0) ;;CLSID
		s1.from("CLSID_" a[2]); s2.format("uuidof(''{%s}'')" a[1])
		AddToMap(m_mg s1 s2)
	ret
if(findrx(s "(?:GU|I|CLS)ID (\w+)=\{(.+)\};$")=0) ;;GUID
	if(findrx(s rx3 0 0 a)=0)
		s2.format("uuidof(''{%08X-%04X-%04X-%02X%02X-%02X%02X%02X%02X%02X%02X}'')" val(a[2]) val(a[3]) val(a[4]) val(a[5]) val(a[6]) val(a[7]) val(a[8]) val(a[9]) val(a[10]) val(a[11]) val(a[12]))
	else if(findrx(s "(?U)\w+ (\w+)=(\{(.+,){10}[^,]+\});" 0 0 a)=0) ;;contains expressions
		s2=a[2]
	else ret -1
	 out s
	AddToMap(m_mg a[1] s2)
	ret
if(findrx(s rx1 0 0 a)=0 or findrx(s rx2 0 0 a)=0) ;;const string or integer to def
	 out s
	AddToMap(m_mc a[1] a[2])
	ret
if(findrx(s "^struct __declspec\(uuid\(.+?\)\)\w+;$")=0) ;;uuid assignment, not useful
	 out s
	ret
if(s.beg("DEFINE_GUIDSTRUCT(")) ret ;;nonexpanded macro, same as above
if(findrx(s "^\w+\*+\w+=")=0) ret ;;not useful variable
if(findrx(s "^\w+ \w+\[.*?\]=\{")=0) ret ;;not useful variable
if(findrx(s "^\w+;$")=0) ret ;;Catalog etc

if(s.beg("OUR_GUID_ENTRY(")) ret ;;like unexpanded macro, contains several GUIDS
if(s.beg("struct tagIMMPID_GUIDLIST_ITEM{")) ret ;;struct {...}var=...;
if(s.beg("DECLARE_MAPI_INTERFACE_(ITnef,IUnknown)")) ret ;;nonexpanded macro
if(s.beg("GUID 0x")) ret ;;interface name expanded as macro

ret -1
