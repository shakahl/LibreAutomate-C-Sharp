 First wait command in rep loop not executing
 ============================================dd

 "wait", when its the first wait in a rep loop,
 fails to execute on first iteration, but its ok on subsequent loops:  
rep 
	out "Top"
	wait(20 K) 
	out "Bottom "
	wait 1  
