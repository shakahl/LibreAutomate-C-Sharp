 out
ARRAY(int) a
win "" "" "" 0 0 0 a
int i h
for i 0 a.len
	h=a[i]
	if(!IsWindowInTaskbar(h) or !IsWindowEnabled(h)) continue
	 outw h
	PostMessage h WM_SYSCOMMAND SC_CLOSE 0
