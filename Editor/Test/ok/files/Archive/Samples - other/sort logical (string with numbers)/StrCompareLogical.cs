 /
function# $s1 $s2

lpstr sn="0123456789"
if(findcs(s1 sn)<0 or findcs(s2 sn)<0) ret StrCompare(s1 s2 1)

str ss1(s1) ss2(s2)
REPLACERX r.frepl=&SCL_RxProc
ss1.replacerx("\d+" r)
ss2.replacerx("\d+" r)
 out ss1
ret StrCompare(ss1 ss2 1)
