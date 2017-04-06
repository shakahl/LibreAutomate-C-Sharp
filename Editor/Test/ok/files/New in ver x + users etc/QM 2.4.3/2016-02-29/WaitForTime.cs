 /
function DATE't [flags] ;;flags: 1 resume from sleep, 2 sleep now, 4 hibernate now, 8 turn on display, 16 t is UTC

 Waits for the specified date/time.


DateTime u
if t<1 ;;only time
	DATE k.getclock
	_i=k.date; t=t+_i
	if(t<k) t=t+1
 else ;;date and time

out t

FILETIME ft; t.tofiletime(ft)
if(flags&16=0) LocalFileTimeToFileTime &ft &ft

__Handle h=CreateWaitableTimer(0 0 0)
if(!SetWaitableTimer(h +&ft 0 0 0 flags&1)) end ERR_FAILED 16 ;;the 1 tells to wake up computer

if(flags&6) shutdown iif(flags&2 5 4)

wait 0 H h ;;wait for the timer
rep(30) 1; if(IsLoggedOn=1) break ;;wait for default desktop
1

 tell Windows to not sleep while the macro is running, and optionally to turn on display
SetThreadExecutionState ES_CONTINUOUS|iif(flags&8 ES_DISPLAY_REQUIRED|ES_SYSTEM_REQUIRED 0)
