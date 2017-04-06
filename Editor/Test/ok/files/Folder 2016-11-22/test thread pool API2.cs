out
 ref WINAPI2
 out GetCurrentThreadId


byte* tp=CreateThreadpool(0)
int nThreads=4
SetThreadpoolThreadMaximum(tp nThreads)
 SetThreadpoolThreadMinimum(tp nThreads)

ARRAY(TP_CALLBACK_ENVIRON_V3) env.create(2)
int i
for i 0 env.len
	sub.InitializeThreadpoolEnvironment(&env[i])
	env[i].Pool=tp ;; SetThreadpoolCallbackPool(&env[i] tp)
	env[i].CleanupGroup=CreateThreadpoolCleanupGroup

PF
for i 0 20
	if(!TrySubmitThreadpoolCallback(&sub.SimpleCallback +i &env[0])) end "failed" 16
PN;PO
0.1
PF
for i 0 20
	if(!TrySubmitThreadpoolCallback(&sub.SimpleCallback +(i+1000000) &env[1])) end "failed" 16
PN;PO

if 1
	0.1
	 1
	PF
	CloseThreadpoolCleanupGroupMembers(env[0].CleanupGroup 1 0)
	PN;PO
	0.1
	 1
	PF
	CloseThreadpoolCleanupGroupMembers(env[1].CleanupGroup 1 0)
	PN;PO

mes "main"
for(i 0 2) CloseThreadpoolCleanupGroup(env[i].CleanupGroup)
CloseThreadpool(tp)


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
