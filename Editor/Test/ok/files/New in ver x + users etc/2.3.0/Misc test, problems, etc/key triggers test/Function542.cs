if(getopt(nthreads)>1) ret
int+ g_ttest

rep
	 press key (254)
	keybd_event 254 0 0 0
	keybd_event 254 0 2 0
	1
	
	 ignore if a modifier key is pressed because then the trigger don't work
	if(GetMod)
		g_ttest=GetTickCount
		continue
	
	 how much time since triggers worked last time?
	int td=GetTickCount-g_ttest
	
	if(td>5000) ;;5 s
		bee+ "$windows$\Media\ringout.wav"
		str s.format("Triggers stopped working at %s" _s.time("%X" "" -td/1000))
		out s
		s+"[][]This may be false positive. Click Retry to check again, or Cancel to stop checking. If after you click Retry this message box will appear again after 1 second, triggers actually don't work or QM is disabled."
		if(mes(s "" "RCx")='C') ret
