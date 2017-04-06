 /
function ARRAY(KEYEVENT)k

 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL

int i
ARRAY(INPUT) a.create(k.len)
for i 0 k.len
	KEYEVENT& rk=k[i]
	if(rk.flags&0x80) i+1; continue
	a[i].type=INPUT_KEYBOARD
	KEYBDINPUT& r=a[i].ki
	r.wVk=rk.vk
	r.wScan=rk.sc
	r.dwFlags=rk.flags

SendInput a.len &a[0] sizeof(INPUT)
