out
int eh=AddVectoredExceptionHandler(0 &sub.VectoredHandler)
out 1
 RaiseException 8654 0 0 0
lpstr+ g_s
out strlen(g_s)

err+ out _error.description
RemoveVectoredExceptionHandler(+eh)
out 3


#sub VectoredHandler
function# EXCEPTION_POINTERS*ExceptionInfo

out 2

g_s=malloc(10); memcpy g_s "abc" 4
Sleep(500); ret -1
