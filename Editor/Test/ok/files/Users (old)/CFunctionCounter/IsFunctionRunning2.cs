 /
function# $functionName

 Returns the number of currently running instances of a user-defined function.
 Unlike IsFunctionRunning, this function can count all functions, not only thread main functions.
 It counts only functions that at the beginning (or after the function statement) have this code:
 CFunctionCounter fc.Enter

 EXAMPLE
 out IsFunctionRunning2("Function40")

  Function40:
  /
 function# a b
 CFunctionCounter fc.Enter
 ...


lock lock_CFunctionCounter

ARRAY(POINT)+ __cfc_a
int i iid=qmitem(functionName)
for(i 0 __cfc_a.len) if(__cfc_a[i].x=iid) ret __cfc_a[i].y
