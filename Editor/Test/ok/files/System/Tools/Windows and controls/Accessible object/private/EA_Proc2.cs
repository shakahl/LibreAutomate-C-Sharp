 /
function# Acc&a level ___EA_ENUM&e

___EA- dA
int i=dA.ar.len

if(level>=e.ha.len) e.ha.redim(level+1); e.ha[level]=dA.ar[i-1].htvi
else if(level<e.ha.len-1) e.ha.redim(level+1)

___EA_ARRAY& ar=dA.ar[]
ar.a=a
ar.htvi=TvAdd(e.tv e.ha[level] +LPSTR_TEXTCALLBACK i)

0 ;;now working flag 1 can be set to 0 to stop
ret dA.working&1
