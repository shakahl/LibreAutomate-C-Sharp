str s="one"
sel s 3
	case "*N*" out 1
	case "*n*" out 2
	case "One" out 3
	case "one" out 4
	case else out 5
