out
__Handle mutex=CreateMutex(0 1 0)
 int mutex=CreateMutex(0 1 0)
out mutex
mac "sub.Thread" "" mutex
1
 ReleaseMutex mutex

#sub Thread
function mutex
_i=wait(0 H mutex)
out _s.dllerror
out _i
