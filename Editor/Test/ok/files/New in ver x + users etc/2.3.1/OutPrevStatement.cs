 /
function [$prefix] [statOffset]


 EXAMPLE
 _i=0
 _i=1
 OutPrevStatement "Warning"


str s sn; int i iid
i=Statement(1 iif(statOffset statOffset -1) &s &iid)
if(i<0) ret
sn.getmacro(iid 1)
out "<>%s%s<open ''%s /%i''>%s</open>: %s" prefix iif(empty(prefix) "" " in ") sn i sn s
