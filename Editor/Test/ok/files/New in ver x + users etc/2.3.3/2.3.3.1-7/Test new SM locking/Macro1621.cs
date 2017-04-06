__Handle h=CreateMutex(0 0 0)

Q &q
InterlockedIncrement(&_i)
InterlockedDecrement(&_i)
Q &qq
WaitForSingleObject(h 1000)
ReleaseMutex h
Q &qqq
outq

