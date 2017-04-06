int i=14
sel i
	case [WM_SETTEXT-9] out "single"
	case [1,WM_SETTEXT+2,4,2,7] out "multi"
	case else out "else"
