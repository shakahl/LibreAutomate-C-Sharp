out
 RealGetNextWindow(0 0)
rep 10
	int w=RealGetNextWindow(w 8|4)
	if(!w) break
	outw2 w

 RealGetNextWindow(win("WordPad") 1)
 outw2 RealGetNextWindow(win 1|8)
 outw2 RealGetNextWindow(0 0|8)
