if(win(mouse)!=_hwndqm) ret
int c=child(mouse); if(c=0) ret
sel GetWinId(c)
	case 2201 ;;output
	out ;;clear
	
	case [2210,2211]
	POINT p; xm p c 1
	wait 0.001 CU 0x1395BEB4C; err ret
	key UCb
