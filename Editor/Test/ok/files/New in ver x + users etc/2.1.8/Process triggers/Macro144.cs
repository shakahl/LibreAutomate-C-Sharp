spe
rep 50
	run "c:\windows\notepad.exe"
 1
out "----"
 ret
 1
int h ph
rep
	h=win("" "Notepad"); if(!h) break
	if(h=ph) 0.001
	else
		ph=h
		clo h
1
out "--------"
	