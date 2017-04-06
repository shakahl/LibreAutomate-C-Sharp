function [off]

 Installs/uninstalls hooks that will call WE_CreatedWindowProc whenever an object is created.
 This function does not need to be modified.

int-- hook
if(off) if(hook) UnhookWinEvent(hook); hook=0
else
	if(getopt(nthreads)>1) mes "Already running. If in QM, use Threads dialog to end thread. If in exe, thread will end automatically." "" "i"; ret
	hook=SetWinEventHook(EVENT_OBJECT_CREATE EVENT_OBJECT_CREATE 0 &WE_CreatedWindowProc 0 0 WINEVENT_OUTOFCONTEXT)
	atend WE_CreatedWindowInstall 1
	opt waitmsg 1
	wait -1
