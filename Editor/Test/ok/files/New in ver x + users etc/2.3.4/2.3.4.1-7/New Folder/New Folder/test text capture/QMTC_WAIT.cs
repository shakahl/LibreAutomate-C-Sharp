if(WT_ResultsVisualClose) ret
out
 int w=win("Internet Explorer" "IEFrame")
 w=child("" "Internet Explorer_Server" w)
 int w=win("Firefox" "Mozilla*WindowClass" "" 0x804)
 int w=win("Chrome" "Chrome_WidgetWin_1")
 int c=child("" "Chrome_RenderWidgetHostHWND" w)
int w=win("Opera" "OperaWindowClass")
act w

SetProp(_hwndqm "qmtc_debug_output" 1)
WTI* a
ITextCapture tc
tc=CreateTextCapture
tc.Begin(w)
int n tf
n=tc.Capture(a 0 tf|WT_WAIT)
key F5; 2
n=tc.Capture(a 0 tf|WT_WAIT_END)
tc.End
if(!n) ret
WT_ResultsOut a n "results"
WT_ResultsVisual a n w 0
