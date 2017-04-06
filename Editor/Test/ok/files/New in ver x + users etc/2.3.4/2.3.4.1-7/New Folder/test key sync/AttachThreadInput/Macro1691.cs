out
int w=win("Test TSM" "#32770")
act w

int tidMe(GetCurrentThreadId) tidFore(GetWindowThreadProcessId(win 0))
AttachThreadInput(tidMe tidFore 1)

int i
for i VK_F12 VK_F24
	_key3 key((i))
	0.1
	int t=GetKeyState(i)&1
	_key3 key((i))
	0.1
	if(GetKeyState(i)&1 != t) out i

AttachThreadInput(tidMe tidFore 0)
