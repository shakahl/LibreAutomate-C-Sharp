function! hwnd $folder [flags] [events] [message] ;;flags: 1 recursive

 Registers window to receive shell notifications.
 Returns 1 if successful, 0 if not (eg folder does not exist).

 hwnd - window handle.
 folder - folder to monitor.
 flags - 1 monitor subfolders too.
 events - same as wEventId of SHChangeNotify. Documented in MSDN library. Default is SHCNE_ALLEVENTS (all events).
 message - message that will be used for notifications. Default is WM_USER+145.


Unregister

SHChangeNotifyEntry e
e.fRecursive=flags&1
e.pidl=PidlFromStr(_s.expandpath(folder)); if(!e.pidl) ret

int fSources=SHCNRF_InterruptLevel|SHCNRF_ShellLevel; if(flags&1) fSources|SHCNRF_RecursiveInterrupt
if(!events) events=SHCNE_ALLEVENTS
if(!message) message=WM_USER+145

m_snid=SHChangeNotifyRegister(hwnd fSources events message 1 &e)
ret m_snid!0
