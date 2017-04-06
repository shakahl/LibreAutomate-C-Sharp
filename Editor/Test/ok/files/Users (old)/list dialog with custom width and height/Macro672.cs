sel list3("one[]two[]three" "text[]ytext" "cap" 0 -2 -60 100 200)
 sel list3("one[]two[]three")
	case 1
	out 1
	case 2
	out 2
	case 3
	out 3
	case else
	ret

