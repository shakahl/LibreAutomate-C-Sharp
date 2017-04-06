int ht1=mac("Function270" "" "thread 1" 300 100)
int ht2=mac("Function270" "" "thread 2" 500 100)
1
sel list("1[]2" "End thread")
	case 1 EndThread "" ht1
	case 2 EndThread "" ht2
