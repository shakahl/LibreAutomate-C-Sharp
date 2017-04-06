out
 out GetKeyState(VK_SCROLL)&1

ARRAY(int) a
win "" "ApplicationFrameWindow" "" 0 "" a
int i
for i 0 a.len
	outw a[i]