out
int w=win("" "QM_HSM")
__Handle ev=CreateEvent(0 0 0 "Global\QM test event")
 __Handle ev=OpenEvent(EVENT_MODIFY_STATE 0 "Global\QM test event")
__Handle wt=CreateWaitableTimer(0 0 "Global\QM test timer")
 __Handle wt=OpenWaitableTimer(TIMER_MODIFY_STATE 0 "Global\QM test timer") ;;fails, don't know why
 out _s.dllerror
out w
out ev
out wt

 SendMessage w WM_USER 0 0
PostMessage w WM_USER 0 0
SetEvent ev

long t=-10000000
SetWaitableTimer(wt +&t 0 0 0 0)
