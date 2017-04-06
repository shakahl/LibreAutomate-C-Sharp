out
 int w=child(mouse); if(!w) w=win(mouse)
int w=win("dlg_apihook" "#32770")

WTI* a
int tf i j n nRep=4

 tf|WT_SPLITMULTILINE
 tf|WT_JOIN
 tf|WT_JOINMORE
 tf|WT_NOCHILDREN
 tf|WT_VISIBLE
 tf|WT_REDRAW
SetProp(_hwndqm "qmtc_debug_output" 1)

ITextCapture tc
tc=CreateTextCapture
tc.Begin(w)

for i 0 nRep
	out "<><Z 0xe080>---- %i ----</Z>" i
	if i
		RedrawWindow w 0 0 RDW_ERASE|RDW_INVALIDATE
		 min w; res w; 0.5
		0.1
	
	Q &q
	int _tf=tf; if(i=0) _tf|WT_WAIT_BEGIN; else if(i=nRep-1) _tf|WT_WAIT_END; else _tf|WT_WAIT
	n=tc.Capture(a 0 _tf)
	 Q &qq; outq
	
	WT_ResultsOut a n

tc.End

#if !EXE
tc=0
UnloadDll "$qm$\qmtc32.dll"
#endif
