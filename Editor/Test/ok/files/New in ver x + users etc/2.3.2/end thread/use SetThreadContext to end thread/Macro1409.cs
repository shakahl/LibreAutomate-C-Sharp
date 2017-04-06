out
int h=mac("Function181")
2
 shutdown -6 0 "Function181"; ret

__Tcc x.Compile("" "exc")
 call x.f

int sc=SuspendThread(h)
 out sc
if(sc=-1) ret

CONTEXT c
c.ContextFlags=CONTEXT_CONTROL
if GetThreadContext(h &c)
	c.Eip=x.f
	out SetThreadContext(h &c)

ResumeThread h

QMTHREAD qt
GetQmThreadInfo h &qt
 out qt.threadid

 try to wake up the thread somehow when it is waiting
 PostThreadMessage qt.threadid WM_QUIT 0 0
 QueueUserAPC &APCProc h 7 ;;works only if the thread is waiting alertable using one of WaitXEx functions

#ret
#include <windows.h>
void exc()
{
//printf("fff");
RaiseException(11111, EXCEPTION_NONCONTINUABLE, 0, 0);
}
