out
ARRAY(int) a.create(10)
int i
for(i a.lbound a.ubound+1) a[i]=i
a.lock
 a.psa.cLocks=1
 a[5]=5
 a.redim(11)
 a[]=5
a.remove(3); err out _error.description
a.unlock
for(i a.lbound a.ubound+1) out a[i]
