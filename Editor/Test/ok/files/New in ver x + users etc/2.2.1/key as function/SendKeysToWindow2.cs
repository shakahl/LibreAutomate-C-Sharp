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


int i lparam m up alt ati
str sk
ifk(A) alt=1

for i 0 a.len
	KEYEVENT k=a[i]
	if(k.flags&0x80) i+1; continue ;;wait
	sel(k.vk)
		case [16,17,18]
		int th1=GetCurrentThreadId
		int th2=GetWindowThreadProcessId(hwnd &i)
		ati=AttachThreadInput(th1 th2 1)
		sk.all(256)
		0.01
		break

for i 0 a.len
	k=a[i]
	
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
	
	if(k.vk=VK_RETURN) 0.01 ;;some controls, eg PSPad, would insert new line too early
	
	PostMessage hwnd m k.vk lparam
	0.003
	
	sel(k.vk)
		case [16,17,18]
		0.01 ;;wait until processes prev keys
		GetKeyboardState sk
		sk[k.vk]=iif(up 0 0x80)
		SetKeyboardState sk
	
	if(k.vk=VK_MENU) alt=!up

if(ati) ati=AttachThreadInput(th1 th2 0)
