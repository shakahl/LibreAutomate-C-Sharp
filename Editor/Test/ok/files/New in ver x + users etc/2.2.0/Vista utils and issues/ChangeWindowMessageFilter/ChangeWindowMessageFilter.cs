dll user32 #ChangeWindowMessageFilter message dwFlag
int i
for i 0 WM_APP*2
	ChangeWindowMessageFilter(i 1)
