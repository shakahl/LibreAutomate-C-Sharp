int timeLimit=3 ;;seconds

rep 1
	int ht
	ht=mac("sub.FUNCTION_1")
	wait timeLimit H ht; err EndThread "" ht
	ht=mac("sub.FUNCTION_2")
	wait timeLimit H ht; err EndThread "" ht


#sub FUNCTION_1
mes 1


#sub FUNCTION_2
mes 2
