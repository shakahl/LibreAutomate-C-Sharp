out
ARRAY(int) a
int w1=win(" - Mozilla Firefox" "MozillaUIWindowClass")
child("" "MozillaContentWindowClass" w1 0 0 0 a)
int i
for i 0 a.len
	outw a[i]
	RECT r; GetWindowRect a[i] &r; zRECT r
