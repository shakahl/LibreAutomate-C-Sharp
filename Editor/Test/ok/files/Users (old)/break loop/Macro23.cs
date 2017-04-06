 Example 1
 F12 breaks loop
rep
	ifk(F12) break
	 ...

 Example 2
 Use global variable
int+ breakloop=0
rep
	if(breakloop) break
	 ...
