function'BSTR [format] ;;format: 0 multiline, 1 date, 2 b.add()

sel format
	case 0 ret "one[]two[]three"
	
	case 1 ret "2000.09.09 05:05:05"
	
	case 2
	BSTR b
	ret b.add("ddddddd" "nnnnnnnn")
	
