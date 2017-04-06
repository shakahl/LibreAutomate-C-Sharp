 captures text and shows visual results of all visible non-popup windows

out
ARRAY(int) a
win "" "" "" 4 0 0 a
int i
WindowText x
for i 0 a.len
	outw a[i]
	act a[i]
	x.Init(a[i])
	x.Capture
	WT_ResultsVisual x.a x.n a[i] 0
