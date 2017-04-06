str s

int t=GetTickCount
rep
	s.getfile("file")
	err if(GetTickCount-t<30000) 0.1; continue; else end _error
	break
