0.1
__Handle h1 h2
Q &q
DuplicateHandle(GetCurrentProcess GetCurrentThread GetCurrentProcess &h1 0 0 DUPLICATE_SAME_ACCESS)
Q &qq
 h2=OpenThread(SYNCHRONIZE 0 GetCurrentThreadId)
h2=OpenThread(THREAD_ALL_ACCESS 0 GetCurrentThreadId)
Q &qqq
outq
out h1
out h2
