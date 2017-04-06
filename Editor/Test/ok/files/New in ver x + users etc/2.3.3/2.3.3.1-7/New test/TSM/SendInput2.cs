ARRAY(INPUT) a.create(4)
int i
for i 0 a.len
	a[i].type=INPUT_KEYBOARD
	KEYBDINPUT& k=a[i].ki
	k.wVk=VK_RETURN
	if(i&1) k.dwFlags=KEYEVENTF_KEYUP

rep 1000
	SendInput a.len &a[0] sizeof(INPUT)
