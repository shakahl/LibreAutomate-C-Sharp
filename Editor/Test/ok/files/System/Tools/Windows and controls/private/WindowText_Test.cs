 Captures and shows text of window or control from mouse.

if(WT_ResultsVisualClose) ret ;;if the visual results window exists, close it. Or you can click it to close.
out
int w=child(mouse 1) ;;get control or window from mouse

 With Debug version of text capturing dll, you can use this to enable debug info:
 SetProp(_hwndqm "qmtc_debug_output" 1) ;;set to 0 to disable again

int flags
 flags|WT_SPLITMULTILINE
flags|WT_JOIN
 flags|WT_JOINMORE
 flags|WT_NOCHILDREN
 flags|WT_VISIBLE
 flags|WT_REDRAW
 flags|WT_SORT
 flags|WT_SINGLE_COORD_SYSTEM
 flags|WT_GETBKCOLOR

 capture text
WindowText x
x.Init(w 0 flags)
 RECT r; SetRect &r 50 50 350 150; x.Init(w r flags)
x.Capture
x.End

 results
if(!x.n) ret
WT_ResultsOut x.a x.n "---- Captured text items ----" ;;show results in QM output
WT_ResultsVisual x.a x.n w flags ;;show results as rectangles over the window
