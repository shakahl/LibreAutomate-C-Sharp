out
__Handle h1(CreateEvent(0 0 0 0)) h2(CreateEvent(0 0 0 0))
int ht=mac("Function258" "" h1 h2)
0.5
out QueueUserAPC(&ApcProc2 ht 5)
1
out "se1"
SetEvent h1
1
out "se2"
SetEvent h2
