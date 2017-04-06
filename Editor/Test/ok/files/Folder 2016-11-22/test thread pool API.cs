out
 ref WINAPI2
 out GetCurrentThreadId


byte* tp=CreateThreadpool(0)
int nThreads=4
SetThreadpoolThreadMaximum(tp nThreads)
 SetThreadpoolThreadMinimum(tp nThreads)

TP_CALLBACK_ENVIRON_V3 env; sub.InitializeThreadpoolEnvironment(&env)
env.Pool=tp ;; SetThreadpoolCallbackPool(&env tp)
env.CleanupGroup=CreateThreadpoolCleanupGroup

int i n
PF
byte* work=CreateThreadpoolWork(&sub.WorkCallback &n &env)
for i 0 40
	SubmitThreadpoolWork(work)
	 if(!TrySubmitThreadpoolCallback(&sub.SimpleCallback +i &env)) end "failed" 16
PN;PO

0.2
 1
PF
 WaitForThreadpoolWorkCallbacks(work 1)
 CloseThreadpoolCleanupGroupMembers(env.CleanupGroup 1 0)
PN;PO

 1
CloseThreadpoolWork(work)

mes "main"
CloseThreadpoolCleanupGroup(env.CleanupGroup)
CloseThreadpool(tp)


#sub WorkCallback
function !*inst &n !*work

lock
n+1
_i=n
lock-
out F"{GetCurrentThreadId} {_i}"
wait RandomNumber/4+0.01


#sub SimpleCallback
function !*inst param

out F"{GetCurrentThreadId} {param}"
wait RandomNumber/4+0.01


#sub InitializeThreadpoolEnvironment
function TP_CALLBACK_ENVIRON_V3&env
env.Version=3
env.CallbackPriority=TP_CALLBACK_PRIORITY_NORMAL
env.Size=sizeof(TP_CALLBACK_ENVIRON_V3)

 int-- t_sta
 if(t_sta=0)
	 t_sta=1
	 int at atq
	 out CoGetApartmentType(&at &atq)
	 out F"{at} {atq}"
	 out CoInitializeEx(0 COINIT_APARTMENTTHREADED|COINIT_DISABLE_OLE1DDE)
