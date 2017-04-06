 Err.1) Statements not exectuting after err/err+ that is outside a "for" loop
 ============================================================================

 If we put err+ at end of macro to handle all errors
	 err statements do not get executed as expected,
	 but loop continues as normal after error 
	 (but we want take action based on the error...,
	 in my application i want minimize a window that suddenly comes up sometimes
		 and interferes with pixel statement (more on this later))
 If we put no error statement, macro ends with error (ok)
 If we put err+ statement within the loop, it works as expected (ok)

 As it stands, to handle all errors in a loop or nested loop
 	Gotta have err statement in the inner loop to handle inner loop errors
    This starts to be very cumbersome and can be a debugging headache when macro doesnt work as expected

 following macro illuscrates
out "Statements in err+ do not get executed, but loop continues after runtime error if err+ is present"
int i
for i 1 3
	out "Before error"
	act win("NoWindow")
	out "After error"
	 0.5
	 err+
		 out "Error statement inside loop"
	

err+
	out "Error statement outside loop"
	


 Err.2) Speed: Finding window takes LOT longer in macro than in function
 =======================================================================

 Another point: this particular macro - where it has to find a non-existant window - 
	 runs LOT faster - mabey 1000x faster -
	 when its property is set to "function" vs. when its "macro"
	 Try it, change property to "function" and compare the speed
	
	
 See also Err_3 macro:
	 Instead of non-existant window, we try to find out of range pixel
	 Speed sufferes there too, and changing property to function 
	 	has absolutely no effect there, whether err or err+ is out or inside loop
