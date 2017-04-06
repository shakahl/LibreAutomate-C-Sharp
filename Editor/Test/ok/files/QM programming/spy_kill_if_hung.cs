int h=val(_command)
int n
rep
	1
	if(!IsWindow(h)) break
	if(IsHungAppWindow(h))
		out "hung"
		n+1
		if(n>5)
			 clo h
			 5
			 if(!IsWindow(h)) break
			ShutDownProcess h 2
			break
 out "Spy++ closed"
