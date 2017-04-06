 /
function r $errstr [noerr]

#compile rasapi

str s ss
if(r=2) s="Cancel"; else RasGetErrorString(r s.all(300) 300)
ss.format("%s: %s" errstr s)
if(noerr) out ss
else end ss 3
