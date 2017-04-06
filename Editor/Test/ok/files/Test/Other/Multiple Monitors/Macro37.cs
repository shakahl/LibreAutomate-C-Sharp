#compile "multimonapi"

 POINT p
 GetCursorPos(&p)
 out MonitorFromPoint(p 0)

int h=win("Notepad")
out MonitorFromWindow(h 0)
