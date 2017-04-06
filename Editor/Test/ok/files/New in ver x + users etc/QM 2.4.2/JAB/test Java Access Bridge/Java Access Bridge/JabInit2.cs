
 Initializes Java Access Bridge (JAB).

 REMARKS
 Call this function once in each thread that uses JAB. Not error to call multiple times. It calls initializeAccessBridge. Don't need to call shutdownAccessBridge.

 Also declares ref JAB that contains all JAB API, and JOBJECT64 type. To make all that available when creating macros, compile this function (compile a macro that calls this function, or add #compile JabInit in function init2 or somewhere).


#if _win64
if(!_win64) end "This program cannot run on 32-bit Windows."
SetEnvVar "jab_dll" "WindowsAccessBridge-32"
type JOBJECT64 :long
#else
if(_win64) end "This program cannot run on 64-bit Windows."
SetEnvVar "jab_dll" "WindowsAccessBridge"
type JOBJECT64 :int
#endif

ref JAB "__jab_api" 1
dll- user32 #ChangeWindowMessageFilter message dwFlag

int-- t_init; if(t_init) ret
JAB.initializeAccessBridge; err end "Java Access Bridge (JAB) not installed. You may need to update Java on your PC, it installs JAB."

int+ ___jab_init
if !___jab_init
	if(_winnt>=6) ChangeWindowMessageFilter(RegisterWindowMessage("AccessBridge-FromJava-Hello") 1)
	opt waitmsg 1; 0.1
	___jab_init=1
else 0 ;;works without this, but...

t_init=1

 notes:
 Some tested functions worked when initializeAccessBridge called once in process, but eg callbacks then work only in first thread.
 initializeAccessBridge creates a hidden dialog that is used for IPC. It posts and receives messages asynchronously, therefore we must wait a while and process messages. 
 I did not find how to know when already initialized. In most cases works even with only wait 0, but not when 100% CPU. Just wait 0.1 s.
 initializeAccessBridge always returns 1, even if JAB disabled.
