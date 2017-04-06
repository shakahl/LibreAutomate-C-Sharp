 int w=win("Untitled - Notepad" "Notepad")
int w=win("temp.txt - Notepad" "Notepad")
 mac "Toolbar41" w
 min w
 1
 max w
 1
 res w

  ont w
 Zorder w HWND_TOPMOST
  ret
 3
  ont- w
 Zorder w HWND_NOTOPMOST
  BringWindowToTop w
  Zorder w HWND_BOTTOM
 
  Zorder w 0
