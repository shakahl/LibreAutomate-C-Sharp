ARRAY(INPUT) a.create(10)
int i
for i 0 a.len
	a[i].type=INPUT_KEYBOARD
	KEYBDINPUT& k=a[i].ki
	k.wVk='A'+(i/2)
	if(i&1) k.dwFlags=KEYEVENTF_KEYUP

rep 1000
	SendInput a.len &a[0] sizeof(INPUT)
