out
 opt waitmsg 1

 100
 wait -1

int+ g_test; g_test=0
QMTHREAD qt; GetQmThreadInfo 0 &qt
int ht=mac("Function266" "" qt.threadhandle)
 rep(2) QueueUserAPC(&ApcProc2 GetCurrentThread 0)
 wait 0 H ht
wait 0 V g_test
 
out "ok"


#ret
