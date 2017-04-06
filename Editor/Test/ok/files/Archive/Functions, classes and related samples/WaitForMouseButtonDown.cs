 /
function ^waitmax button ;;waitmax: 0 is infinite.  button: 1 left, 2 right, 4 middle, 5 6 X


if(waitmax<0 or waitmax>2000000) end ES_BADARG
opt waitmsg -1

int wt(waitmax*1000) t1(GetTickCount)
rep
	0.01
	
	ifk((button)) ret
	
	if(wt>0 and GetTickCount-t1>=wt) end "wait timeout"
