int h=win("Quick")
int x y cx cy
GetWinXY h x y cx cy
if(cx>=500)
	x+100
	cx-200
	MoveWindow h x y cx cy 1
