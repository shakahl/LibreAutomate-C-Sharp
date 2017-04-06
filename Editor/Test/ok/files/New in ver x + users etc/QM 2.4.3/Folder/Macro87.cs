 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST

ARRAY(INPUT) a.create(4)
int i
for(i 0 a.len) a[i].type=INPUT_KEYBOARD; if(i>=2) a[i].ki.dwFlags=KEYEVENTF_KEYUP
a[0].ki.wVk=VK_MENU; a[3].ki.wVk=VK_MENU
a[1].ki.wVk=VK_TAB; a[2].ki.wVk=VK_TAB

rep 2
	0.5
	SendInput 4 &a[0] sizeof(INPUT)
	 SendInput 1 &a[1] sizeof(INPUT)
	
	 0.01
	
	 SendInput 2 &a[2] sizeof(INPUT)
	 SendInput 1 &a[3] sizeof(INPUT)
