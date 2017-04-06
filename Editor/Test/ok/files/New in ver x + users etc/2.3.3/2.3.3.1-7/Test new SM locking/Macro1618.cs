out
int c=10
__Handle hs=CreateSemaphore(0 c c "QM test sem")
 out hs

Q &q
rep() if(WaitForSingleObject(hs 0)) break
Q &qq
ReleaseSemaphore(hs c 0)
Q &qqq
outq

 rep 2
	 out WaitForSingleObject(hs 0)

 out ReleaseSemaphore(hs 2 &_i)
 out _i
