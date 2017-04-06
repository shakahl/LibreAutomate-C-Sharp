 dll user32 EndTask hwnd _ force
 int w=win("Untitled - Notepad" "Notepad")
 EndTask w 0 1

int w=win("Untitled - Notepad" "Notepad")
WINDOWINFO x.cbSize=sizeof(x)
if GetWindowInfo(w &x)
	out _s.getstruct(x 1)
else out _s.dllerror
