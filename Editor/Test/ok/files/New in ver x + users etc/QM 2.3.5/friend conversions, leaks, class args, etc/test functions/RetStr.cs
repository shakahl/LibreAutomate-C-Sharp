function~ [format] ;;format: 0 multiline, 1 date

sel format
	case 0 ret "one[]two[]three"
	
	case 1 ret "2000.09.09 05:05:05"
