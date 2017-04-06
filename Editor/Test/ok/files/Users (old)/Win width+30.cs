 trigger CWR
int x y cx cy w=win
if(IsZoomed(w) or IsIconic(w)) ret
GetWinXY w &x &y &cx &cy

rep
	cx+30
	siz cx 0 w 2
	wait 2 KF R; err ret
