 /
function hwnd ARRAY(KEYEVENT)'a

 Sends keys to a child window using WM_KEYDOWN/WM_KEYUP messages.
 The window can be inactive.
 The function actually presses modifier keys (Shift, Ctrl, Alt), although very briefly.

 hwnd - child window handle.
 a - the key function.

 EXAMPLE
 int hwnd=id(15 "Notepad")
 SendKeysToWindow hwnd key("Line1[]Line2[]")


int i lparam m up alt
ifk(A) alt=1

for i 0 a.len
	KEYEVENT k=a[i]
	
	if(k.flags&0x80) ;;wait
		i+1
		opt waitmsg -1
		wait (a[i].wt/1000.0)
		continue
	
	lparam=k.sc<<16|1
	if(k.flags&KEYEVENTF_EXTENDEDKEY) lparam|0x01000000
	if(k.flags&KEYEVENTF_KEYUP) lparam|0xC0000000; up=1; else up=0
	
	if(alt) m=iif(up WM_SYSKEYUP WM_SYSKEYDOWN); lparam|0x20000000
	else m=iif(up WM_KEYUP WM_KEYDOWN)
	
	 use key for Ctrl/Shift/Alt
	sel(k.vk)
		case [16,17,18]
		 out "%i %i" i k.vk
		 if(up) key- (k.vk); else key+ (k.vk)
		keybd_event k.vk k.sc k.flags 0
		 0.1
	
	 out "%i 0x%08X" k.vk lparam
	 out "%i %i" k.vk RealGetKeyState(k.vk)
	PostMessage hwnd m k.vk lparam
	0.01
	
	if(k.vk=VK_MENU) alt=!up
