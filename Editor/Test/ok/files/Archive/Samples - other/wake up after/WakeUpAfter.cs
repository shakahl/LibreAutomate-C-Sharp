 \
function# wakeUpAfterSeconds [flags] ;;flags: 1 turn on display, 2 sleep now, 4 hibernate now

 Waits wakeUpAfterSeconds seconds and then temporarily wakes up the computer
 from sleep (suspend, standby) or hibernate. When the caller macro ends, but
 not earlier than after 2 minutes, Windows again puts the computer into the
 sleep/hibernate state. However it doesn't if you use the physical mouse or
 keyboard. On some systems it also doesn't if a macro uses mouse/keyboard.
 This function must be called BEFORE putting the computer into the sleep state.
 Returns 1 on success, 0 on failure (eg if it is not supported by the hardware).

 EXAMPLE
  put computer into sleep state, wait 1 minute, wake up, turn on display,
  tell Windows to sleep again when the macro ends or after 2 minutes (which
  is longer) unless you use mouse/keyboard during that time
 WakeUpAfter 1*60 1|2


long t=-wakeUpAfterSeconds*10000000 ;;negative - from now, positive - absolute
__Handle h=CreateWaitableTimer(0 0 0)
if(!SetWaitableTimer(h +&t 0 0 0 1)) ret ;;the 1 tells to wake up computer
if(flags&6) shutdown iif(flags&2 5 4)
wait 0 H h ;;wait for the timer
rep(30) 1; if(IsLoggedOn=1) break ;;wait for default desktop
1

 tell Windows to not sleep while the macro is running, and optionally to turn on display
SetThreadExecutionState ES_CONTINUOUS|iif(flags&1 ES_DISPLAY_REQUIRED|ES_SYSTEM_REQUIRED 0)

ret 1
