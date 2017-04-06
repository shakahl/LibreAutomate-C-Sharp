 int w=win("Document - WordPad" "WordPadClass")
 act w

 SetForegroundWindow GetDesktopWindow
 outw win

out
ARRAY(int) a
GetMainWindows a 4
int i
for i 0 a.len
	outw2 a[i]

 outw2 FirstWindowInMonitor(2)
