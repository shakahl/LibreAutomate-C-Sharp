 This macro calls sample functions.

 declare variables
int r i
str date
 show list box
r=ListDialog("Sample_AddAB[]Sample_GetCurrentDate[][]Read more about functions" "Caller")
 call selected function
sel r
	case 1
	i=Sample_AddAB(10 5)
	mes i "Caller" "i"
	
	case 2
	Sample_GetCurrentDate date
	mes date "Caller" "i"
	
	case 4
	QmHelp "IDH_FUNCTIONTIPS"
