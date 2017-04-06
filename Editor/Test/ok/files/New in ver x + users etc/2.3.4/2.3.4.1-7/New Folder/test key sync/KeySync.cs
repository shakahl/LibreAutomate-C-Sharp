 /Macro1689

Q &q
int tidMe(GetCurrentThreadId) tidFore(GetWindowThreadProcessId(win 0))
Q &qq
if(!AttachThreadInput(tidMe tidFore 1)) out "AttachThreadInput failed"; ret
Q &qqq

int vk=VK_F24
 vk=16
vk=0x40

 __RegisterHotKey hk
 hk.Register(0 1 0 VK_F24)

 outx GetKeyState(vk)
int ks=GetKeyState(vk)&1

_key3 key((vk))
 PostThreadMessage g_tid_key WM_KEYDOWN vk 0

 SetThreadPriority GetCurrentThread THREAD_PRIORITY_IDLE
int i
for i 0 100
	if(GetKeyState(vk)&1!=ks) break
	Sleep 1
out i

 outx GetKeyState(vk)

Q &qqqq
AttachThreadInput(tidMe tidFore 0)
Q &qqqqq
outq
