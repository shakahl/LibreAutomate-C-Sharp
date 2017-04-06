out
__Handle h=CreateMutex(0 0 0)
 __Handle h=CreateMutex(0 0 "jhsdfhjdshfjdhfj")
CRITICAL_SECTION cs; InitializeCriticalSection &cs

Q &q
rep(1000) lock; lock-
Q &qq
rep(1000) lock _mmmkio "qmtestmutex"; lock- _mmmkio
Q &qqq
rep(1000) WaitForSingleObject(h INFINITE); ReleaseMutex h
Q &qqqq
rep(1000) EnterCriticalSection(&cs); LeaveCriticalSection(&cs)
Q &qqqqq
rep(1000) spe; spe
Q &qqqqqq
outq

DeleteCriticalSection &cs
