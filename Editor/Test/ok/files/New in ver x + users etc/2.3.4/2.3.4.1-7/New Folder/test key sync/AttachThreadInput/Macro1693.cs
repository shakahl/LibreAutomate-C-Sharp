out
 int w=win("Test TSM" "#32770")
int w=win("Untitled - Notepad" "Notepad")
act w
int i

int tidMe(GetCurrentThreadId) tidFore(GetWindowThreadProcessId(win 0))
AttachThreadInput(tidMe tidFore 1)

_s.all(256 2 0)
SetKeyboardState _s

 for i 8 VK_F24+1
	 sel(i) case [91,92,VK_CAPITAL,VK_SCROLL,VK_NUMLOCK,VK_F1,VK_F3,VK_F6,VK_F10,VK_MENU,VK_APPS] continue
	 str sk=""; FormatKeyString i 0 &sk
	 out "%i %s" _s[i] sk

for i 8 VK_F24+1
	sel(i) case [91,92,VK_CAPITAL,VK_SCROLL,VK_NUMLOCK,VK_F1,VK_F3,VK_F6,VK_F10,VK_MENU,VK_APPS] continue
	_key3 key((i))
	 _key3 key((i))
1

GetKeyboardState _s
for i 8 VK_F24+1
	sel(i) case [91,92,VK_CAPITAL,VK_SCROLL,VK_NUMLOCK,VK_F1,VK_F3,VK_F6,VK_F10,VK_MENU,VK_APPS] continue
	str sk=""; FormatKeyString i 0 &sk
	out "%i %s" _s[i] sk

AttachThreadInput(tidMe tidFore 0)
