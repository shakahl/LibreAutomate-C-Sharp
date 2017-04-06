function` retType ;;retType: 0 int, 1 BSTR (multistring), 2 BSTR (date), 3 ARRAY(BSTR), 4 IXml

sel retType
	case 0
	ret 1
	
	case 1 ret "one[]two[]three"
	case 2 ret "2000.09.09 05:05:05"
	
	case 3
	ARRAY(str) a="one[]two"
	ret a
	
	case 4
	IXml x._create
	ret x
