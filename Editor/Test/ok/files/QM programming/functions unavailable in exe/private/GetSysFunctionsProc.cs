 /
function# iid QMITEM&q level str&s

sel(q.itype)
	case [1,6] ;;function, member
	sel(q.name) case ["Scripting_Link"] ret 1
	s.formata("#compile %s[]" q.name)
	
	case 5 ;;folder
	sel(q.name 1) case ["unavailable in exe","obsolete"] ret 1
