 Break.1) Inconsistancy in break statement handling
 ==================================================

 If any statements come AFTER inner loop, its OK (only breaks inner loop)
 But if no statement comes after inner loop, then outer loop breaks along with inner loop (not what is expected)
 Run this macro to see


int i j c

out "============================= With nothing after inner ''rep'' loop"
for i 1 3
	out "i %i" i
	c=0
	rep
		c+1
		out "i.c %i.%i" i c
		if c>4; break
	 <no statement here - results in bug>

out "============================= With statement following inner ''rep'' loop"
c=0
for i 1 3
	out "i %i" i
	rep
		c+1
		out "i.c %i.%i" i c
		if c>4; break
	c=0
out "============================= With statement following inner ''for'' loop"

c=0
for i 1 3
	for c 1 10
		out "c %i" c
		if c=5; break
	out "%i Outer loop" i

out "============================== With nothing after inner ''for'' loop"
	
c=0
for i 1 3
	out "%i Outer loop" i
	for c 1 10
		out "c %i" c
		if c=5; break




 Break.2) Break in function breaks from function: confusing...
 =============================================================
 Using "break", in function, outside of any loop at all, breaks out of function,
  which is confusing, especially to novices (like me once) who may think that "break"
  can break out of an "if" or "sel" statement, which results in a frustrating time to debug
  when a function prematurely ends with use of "break"
 So, i propose that only "ret" or "end" should be used to break out of function,
  with "break" being allowed only for loops.

 Update: I moved a certain a loop block into a function, and left the "continue" statements
 in place, which causes a return from the function. It works great. Whether its good programming
 practice or not i dont know.

