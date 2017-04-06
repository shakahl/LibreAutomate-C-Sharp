 /
function# iid QMITEM&q level str&s

sel(q.itype)
	case 1 ;;function
	s.addline(q.name)
	
	case 6 ;;member
	str st sn
	if tok(q.name &st 2 ".")=2
		s.formata("%s _%s.%s[]" st st sn)
	
	case 5 ;;folder
	sel(q.name 1) case ["private"] ret 1
