dll "qm.exe" TestSM size hwnd

 int h=_hwndqm
 int h=win("app")

TestSM 1000 0
#ret

ARRAY(int) a
win "" "" "" 0 0 0 a
int i
for i 0 a.len
	int h=a[i]
	outw h
	PostMessage(h, WM_APP+8463, 0, 0)
	0.1