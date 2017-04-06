double Seconds=5.5

int ms0(Seconds*1000) ms(ms0)
int t0=timeGetTime ;;get ms, same as GetTickCount but more precise
rep
	str s.format("%i.%03i" ms/1000 ms%1000)
	OnScreenDisplay s 1 0 0 0 0 0 5 "cd472"
	0.01
	ms=ms0-(timeGetTime-t0)
	if(ms<=0) break
OsdHide "cd472"
