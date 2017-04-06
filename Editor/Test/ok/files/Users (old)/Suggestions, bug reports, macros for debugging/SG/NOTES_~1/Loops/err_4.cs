  err after if statements
 ========================

if(wait(1 KF)=0x1B) mac "ok";
err
	out "passed"
 NOTE: wont pass control to err statement - it determines that the next statement is "out"
  	but isnt the "out" just a continuation of the if statement?


  Rewritten, the following works ok though
 int k=1
 k = wait(1 KF); err
 if k=VK_ESCAPE; out "ok"
 out k
 out "passed"

