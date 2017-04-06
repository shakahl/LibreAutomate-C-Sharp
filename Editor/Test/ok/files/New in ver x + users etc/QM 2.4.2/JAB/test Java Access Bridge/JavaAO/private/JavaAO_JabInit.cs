 /JavaAO help

int+ ___jab_init
if(___jab_init) ret
lock
if(___jab_init) ret

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

_i=&JAB.initializeAccessBridge; err end "Java Access Bridge (JAB) not installed. You may need to update Java on your PC, it installs JAB."

dll- user32 #ChangeWindowMessageFilter message dwFlag
if _winnt>=6
	ChangeWindowMessageFilter(RegisterWindowMessage("AccessBridge-FromJava-Hello") 1) ;;received by the hidden "Access Bridge status" dialog once after initializeAccessBridge
	ChangeWindowMessageFilter(RegisterWindowMessage("AccessBridge-FromWindows-Hello") 1) ;;received by the dialog for each JAB-enabled Java window. initializeAccessBridge posts this message to all windows. wParam is HWND of the poster dialog.

mac "sub.Thread"
wait 0 V ___jab_init


#sub Thread
JAB.initializeAccessBridge
opt waitmsg 1; 0.1
___jab_init=1
MessageLoop ;;the hidden dialog must be running all the time, else isjavawindow etc would not recognize new Java windows
___jab_init=0

 notes:
 Some tested functions worked when initializeAccessBridge called once in process, but eg callbacks then work only in first thread.
 initializeAccessBridge creates a hidden dialog that is used for IPC. It posts and receives messages asynchronously, therefore we must wait a while and process messages. 
 I did not find how to know when already initialized. Could wait for an undocumented "AccessBridge-FromWindows-Hello" message in message loop, but it may be unreliable etc. In most cases works even with only wait 0, but not when 100% CPU. Just wait 0.1 s.
 initializeAccessBridge always returns 1, even if JAB disabled.