 /
function ARRAY(KEYEVENT)'a waitMsAfterKeyDown waitMsAfterKeyUp

 Sends keys, waiting waitMsAfterKeyDown milliseconds after each key down event and waitMsAfterKeyUp milliseconds after each key up event.
 Unlike QM key() function, does not use synchronization, does not depend on spe etc.

 EXAMPLE
 KeyWithPreciseSpeed key("1234567890") 20 100


int i
for i 0 a.len
	KEYEVENT k=a[i]
	if(k.flags=0x80) i+1; continue
	INPUT x.type=INPUT_KEYBOARD
	KEYBDINPUT& r=x.ki
	r.wVk=k.vk; r.wScan=k.sc; r.dwFlags=k.flags
	SendInput 1 &x sizeof(x)
	double wt=iif(k.flags&KEYEVENTF_KEYUP waitMsAfterKeyUp waitMsAfterKeyDown)
	wait wt/1000
