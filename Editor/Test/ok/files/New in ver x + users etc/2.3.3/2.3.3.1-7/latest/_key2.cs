 /
function ARRAY(KEYEVENT)k

 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL

INPUT in.type=INPUT_KEYBOARD
KEYBDINPUT& r=in.ki
int i j
for i 0 k.len
	KEYEVENT& rk=k[i]
	if(rk.flags&0x80) i+1; continue
	r.wVk=rk.vk
	r.wScan=rk.sc
	r.dwFlags=rk.flags
	SendInput 1 &in sizeof(INPUT)
	 keybd_event rk.vk rk.sc rk.flags 0
	 0.01
	j+1; if(j=100) 0.01; j=0
	