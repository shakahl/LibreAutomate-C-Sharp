 \
function# iid FILTER&f

 This filter function can be used with keyboard triggers when the macro wants to know whether the trigger key was pressed one or two times.
 Assign this filter function to the macro (Properties / Keyboard / FF).
 The macro receives a single int argument that is 0 on single tap, 1 on double tap.
 Example macro:

 function isDoubleTrigger
 if isDoubleTrigger
 	out "double"
 else
 	out "single"

 The macro runs on double tap OR on single tap. It does not run on single tap if double tap detected.
 On single tap, the macro starts with some delay. Need it to detect double tap.
 In QM 2.3.3, does not work with modifier keys (Ctrt, Shift etc). It is a bug in this QM version. Works only with single key triggers.
 Does not work with mouse triggers.


int K=FF_Keyboard2(iid f); if(K!iid) ret K

double detectDoubleTapTriggerTime=0.4 ;;0.4 s. You can change this.
 Don't change code below, unless you understand how it works and want to change something.

 __________________________________________________________________

type __TRIGGERSD t __Handle'ev
__TRIGGERSD+ __g_tsd ;;global variable that stores previous trigger time etc
int timeoutMS=detectDoubleTapTriggerTime*1000 ;;convert to milliseconds

if &f ;;this function runs as a filter function when the user pressed a trigger key
	if(f.ttype!1) out "FF_KeyTriggerSingleDouble can be used only with keyboard triggers."; ret
	
	if(!__g_tsd.ev) __g_tsd.ev=CreateEvent(0 0 0 0)
	
	int t=GetTickCount ;;time now
	int td=t-InterlockedExchange(&__g_tsd.t t) ;;get time from previous time, and set __g_tsd.t=t
	 out td
	
	if td>=timeoutMS ;;more than 0.4 s since previous trigger key pressed. Start thread that waits 0.4 s and then launches macro with isDoubleTrigger=0.
		mac "FF_KeyTriggerSingleDouble" "" iid
	else ;;detected double-key trigger. Launch the macro now with isDoubleTrigger=1.
		SetEvent __g_tsd.ev ;;end the waiting thread
		__g_tsd.t=0 ;;prevent detecting double tap on triple tap
		mac iid "" 1 ;;run macro with isDoubleTrigger=1
	ret -1 ;; don't run any macros but eat the key if need
else ;;this function is started by mac 
	wait detectDoubleTapTriggerTime H __g_tsd.ev
	err ;;timeout
		if(GetTickCount-InterlockedExchange(&__g_tsd.t 0)<timeoutMS/2) ret ;;thread synchronization, to prevent starting 'double-tap' and 'single-tap' macros simultaneously if second tap is at ambiguous time
		mac iid ;;run macro with isDoubleTrigger=0
	 if not timeout, setevent was called to end this thread
