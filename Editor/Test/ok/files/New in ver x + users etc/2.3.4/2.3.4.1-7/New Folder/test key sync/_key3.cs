 /
function ARRAY(KEYEVENT)k

 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL

int i
ARRAY(INPUT) a.create(k.len)
for i 0 k.len
	KEYEVENT& rk=k[i]
	 out rk.vk
	if(rk.flags&0x80) i+1; continue
	a[i].type=INPUT_KEYBOARD
	KEYBDINPUT& r=a[i].ki
	r.wVk=rk.vk
	r.wScan=rk.sc
	r.dwFlags=rk.flags
	 if(r.wVk=VK_F20) r.wVk=0; r.wScan=0


 int tidMe(GetCurrentThreadId) tidFore(GetWindowThreadProcessId(win 0))
 if(!AttachThreadInput(tidMe tidFore 1)) out "AttachThreadInput failed"

SendInput a.len &a[0] sizeof(INPUT)

 0.1

 int tidMe(GetCurrentThreadId) tidFore(GetWindowThreadProcessId(win 0))
 if(!AttachThreadInput(tidMe tidFore 1)) out "AttachThreadInput failed"
 
 outx GetKeyState(VK_F11)
  _s.all(256); GetKeyboardState _s; outx _s[VK_F20]
  outx GetKeyState(VK_SHIFT)
 
 AttachThreadInput(tidMe tidFore 0)
