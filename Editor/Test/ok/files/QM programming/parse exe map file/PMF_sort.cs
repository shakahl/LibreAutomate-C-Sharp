 /
function# param PMF&a PMF&b

sel param
	case 0 ;;by address
	ret a.a2-b.a2
	
	case 1 ;;by size
	if(a.isfunc!=b.isfunc) ret b.isfunc-a.isfunc
	ret a.size-b.size
