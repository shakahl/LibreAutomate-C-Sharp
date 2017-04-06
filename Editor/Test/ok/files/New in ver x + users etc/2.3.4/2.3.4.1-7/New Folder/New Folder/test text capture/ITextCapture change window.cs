out
 int w=win("Word" "OpusApp")
 int c=child("Menu Bar" "MsoCommandBar" w)
 int c1=child("Microsoft Word Document" "_WwG" w)
int w=win("Calculator" "CalcFrame")
int c=id(81 w)
int c1=id(139 w)
int w1=win("Untitled - Notepad" "Notepad")
int c2=id(1025 w1)

int* wa=&w

SetProp(_hwndqm "qmtc_debug_output" 1)

WTI* a; int i n
ITextCapture tc=CreateTextCapture
 tc.Begin(w)

for i 0 5
	n=tc.Capture(a wa[i])
	 WT_ResultsOut a n "results"
	WT_ResultsVisual a n wa[i] 0
	0.2
