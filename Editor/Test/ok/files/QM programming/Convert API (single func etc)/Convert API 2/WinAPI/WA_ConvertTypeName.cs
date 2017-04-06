 /
function# str&s $tc byref where ;;where: 0 member, 1 func, 2 arg

sel tc 1
	case ["Long","&"] s=iif(where=1 "#" "")
	case ["String","$"] s="$"
	case ["Integer","%"] s="@"
	case "Byte" s="!"
	case ["Double","#"] s="^"
	case ["Single","!"] s="FLOAT"
	case "Variant" s="VARIANT"
	case ["Currency","@"] s="Currency"
	case "Decimal" s="DECIMAL"
	case "Date" s="DATE"
	case "POINTAPI" s="POINT"
	case "Rect" s="RECT"
	case "Any" s="!"; byref=1
	case else
	s=tc; if(where=0 and s.begi("String ")) ret 1
	if(s.beg("Struct_MembersOf_")) s.get(s 17)

if(byref and s.beg("I")) _s.ucase(s); if(_s!=s) byref=0 ;;IInterface* -> IInterface

if(byref) s+"*"
else if(s.len and isalpha(s[0])) s+"'"
