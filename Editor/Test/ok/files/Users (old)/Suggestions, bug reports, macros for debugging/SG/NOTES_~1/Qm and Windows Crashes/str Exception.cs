 
str s="a"
rep 10; s+s
	
 This produces an exception error and crashes Qm with an illegal operation error.
 Above gives no error output in Qm, but first time,
 had the following at end of "Ascii Codes Reference" macro:

s="[168]"
rep 20; s+s
out s

 Then it gave me the following two errors in Qm output as it crashed
  with an "Illegal operation" 

 Exception (RT) in Ascii Code Reference
 Exception (RT) in Ascii Code Reference: exception in destructors of local variables
-----
 Then, to test the bug further, i ran:
s="a"
s+s
out s
s+s
out s
s+s
out s
s+s
out s
s+s
out s
s+s
out s
s+s
out s
 I believe it was 6 "s+s" statements, since it crashes immediately
 at the 7th, but it worked -
 gave me the desired output, but then Qm crashed about 30 seconds later
