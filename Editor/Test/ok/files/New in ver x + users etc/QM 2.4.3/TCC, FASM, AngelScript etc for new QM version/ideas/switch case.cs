switch x
	case 1:
	code
	case 2:
	code
	goto 1 //goto case 3
	case 3:
	code
	goto -2 //goto case 1
	
	case [4 to 7]
	code
	
	case [8 to] //>=8
	code
	
	case [to 0] //<=0
