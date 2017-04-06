function ^period ^timeout

 Use with foreach to wait for a condition.
 In each loop (except the first) waits period seconds.
 Error if still called after timeout seconds + the loop code execution time.

 EXAMPLE
 foreach(0.5 60 WdWait) if(condition) break


double waited
if(!waited) waited=1E-9; ret 1
if(waited>timeout) end ERR_TIMEOUT
opt waitmsg -1
wait period
waited+period
ret 1
