 /
function str'variable

 Shows variable name and value in QM output.
 Unavailable in exe.

 EXAMPLE
 str s="test str"
 RECT r; r.bottom=888888
 OutVar s
 OutVar r.bottom


str s
if(Statement(1 0 &s)>=0 and s.beg("OutVar"))
	out "%s = %s" s+7 variable
else out variable
