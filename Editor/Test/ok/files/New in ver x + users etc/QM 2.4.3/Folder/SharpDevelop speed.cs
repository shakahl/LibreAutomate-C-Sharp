key "/" CF5
int t1=timeGetTime
rep
	0.01
	int w=win("" "ConsoleWindowClass")
	if(w && IsWindowVisible(w)) break
int t2=timeGetTime
rep
	0.01
	if(!IsWindow(w)) ret
	_s.GetConsoleText(w)
	if(_s.len) break
int t3=timeGetTime

OnScreenDisplay F"{t2-t1} {t3-t2}"
