 parameters
str tb="Toolbar24"
double xwait=3
 ____________________

int h=mac(tb)
mov -3 0 h 2
double c=xwait
rep
	if(win(mouse)=h) c=xwait
	else 0.1; c-0.1; if(c<=0) break
mov -3000 0 h 2


 int h=mac(tb)
 mov -3 0 h 2
 double c=xwait
 rep
	 if(win(mouse)=h) c=xwait
	 else 0.1; c-0.1; if(c<=0) break
 mov -3000 0 h 2
