 Err.3)  Speed: Takes LONG time to report out-of-bound pixel error in for loop
 =============================================================================

 Why so long to execute?
 Compare following two "for z" loops in exectuion speeds

dll msvcrt clock
type n8 int'x[9]
n8 x y
int i z p cw(win("Quick")) start
for i 1 9
	x[i]=10000;	y[i]=10000
	 Note: in my application these will vary - some points will be in window range, some out of range
	 Purpose of the following loops: to execute later statements in loop if point x[i] y[i] is in range	
	

 *slow error handling* - when letting QM handle it with err statement
start=clock()
for i 1 9
	p=pixel(x[i] y[i] cw); err out "%i %s" i _error.description; continue
	 BUG: it takes over 1000 times longer to finish the loop with this err statement!!
out clock()-start

 *fast error handling* - when we do the error checking using "if"
start=clock()
for i 1 9
	if(x[i]>600 or y[i]>600) out "%i Out of range" i; continue
	else p=pixel(x[i] y[i] cw);
out clock()-start


 Conclusion: avoid using "err" for simple conditions, use "if" instead where you can

 But is there a bug in QM that's causing it to take a long time to determine
 that a certain point is out-of-range? And, as in err.1) in the case of macros
 (not functions), to determine that a window does not exist?