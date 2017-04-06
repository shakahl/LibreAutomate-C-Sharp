 /
function! hwnd long&t

def THREAD_QUERY_X_INFORMATION iif(_winnt<6 THREAD_QUERY_INFORMATION THREAD_QUERY_LIMITED_INFORMATION)

int tid=GetWindowThreadProcessId(hwnd 0); if(!tid) ret
__Handle ht=OpenThread(THREAD_QUERY_X_INFORMATION 0 tid); if(!ht) ret

long tce tkernel tuser
if(!GetThreadTimes(ht +&tce +&tce +&tkernel +&tuser)) ret
t=tkernel+tuser
 out F"{tkernel} {tuser}"
ret 1
