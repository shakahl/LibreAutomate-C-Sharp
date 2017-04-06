out
RTL_SRWLOCK+ g_srw
InitializeSRWLock &g_srw

out "main before"
AcquireSRWLockExclusive &g_srw
out "main in"
rep(3) mac "sub.Thread"
3
ReleaseSRWLockExclusive &g_srw
out "main after"


#sub Thread

 out "Thread before"
 AcquireSRWLockExclusive &g_srw
 out "Thread in"
 1
 ReleaseSRWLockExclusive &g_srw
 out "Thread after"

out "Thread before"
AcquireSRWLockShared &g_srw
out "Thread in"
1
ReleaseSRWLockShared &g_srw
out "Thread after"
