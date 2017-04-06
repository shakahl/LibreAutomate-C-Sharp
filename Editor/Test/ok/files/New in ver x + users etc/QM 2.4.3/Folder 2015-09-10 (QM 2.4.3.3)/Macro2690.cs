dll "qm.exe" !IsDesktopThreadWindow hwnd

ARRAY(int) a
win "" "" "" 0 "" a
int i
for i 0 a.len
	if(IsDesktopThreadWindow(a[i])) outw a[i]; RECT r; GetWindowRect a[i] &r; outRECT r
	