/exe
if(WT_ResultsVisualClose) ret
if(getopt(nthreads)>1) ret
out

int w=child(mouse); if(!w) w=win(mouse)
 int w=win("dlg_apihook" "#32770" "apihook"); if(!w) mac "dlg_apihook"; 1; w=win("dlg_apihook" "#32770" "apihook")
if(!w) mes- "w=0"

Q &q
WTI* a
int tf noPreview(0) nRep(1) useCOM(0) i n

 tf|WT_SPLITMULTILINE
 tf|WT_JOIN
 tf|WT_JOINMORE
 tf|WT_NOCHILDREN
 tf|WT_VISIBLE
 tf|WT_REDRAW
 tf|WT_SORT
 tf|WT_SINGLE_COORD_SYSTEM
tf|WT_GETBKCOLOR
SetProp(_hwndqm "qmtc_debug_output" 1)
 g1
if useCOM
	ITextCapture tc=CreateTextCapture
	tc.Begin(w)
	 tc.Codepage=-1
else
	WindowText x
	x.Init(w 0 tf)
	 RECT r; SetRect &r 100 50 350 150; x.Init(w r tf)

for i 0 1+((nRep-1)*noPreview)
	if useCOM
		n=tc.Capture(a w tf)
	else
		if(i) 0.2
		x.Capture
		a=x.a; n=x.n
	 0.5
if(useCOM) tc.End; else x.End
Q &qq
outq

 out n
WT_ResultsOut a n "results"
if(!noPreview) WT_ResultsVisual a n w tf
 int nbAll(HeapSize(GetProcessHeap 0 a)) nbTA(n*sizeof(WTI)) nbStr(nbAll-nbTA) nbMax(2*1024*1024); out "size (all): %i,  WTI array: %i (%i %%),  strings: %i (%i %%)" nbAll nbTA MulDiv(nbTA 100 nbMax) nbStr MulDiv(nbStr 100 nbMax)

 if(tf&(WT_JOIN|WT_JOINMORE)=0) tf|WT_JOIN; 0.1; goto g1
 else if(tf&WT_JOINMORE=0) tf~WT_JOIN; tf|WT_JOINMORE; 0.1; goto g1

#if !EXE
if(useCOM) tc=0; else x.End(1)
UnloadDll "$qm$\qmtc32.dll"
#endif

 BEGIN PROJECT
 main_function  DAH_RemoteCaptureWithDll
 exe_file  $qm$\DAH_RemoteCaptureWithDll.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {566C34A3-9522-4DAC-8532-082F088D50C8}
 END PROJECT
