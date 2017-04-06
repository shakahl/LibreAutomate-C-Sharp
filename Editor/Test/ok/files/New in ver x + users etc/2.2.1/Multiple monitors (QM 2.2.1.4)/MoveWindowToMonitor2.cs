int h=win("Notepad")
 int h=win("Find")
act h
out MoveWindowToMonitor(h 2)
 out MoveWindowToMonitor(h MonitorFromWindow(_hwndqm 0) 32)
 out MoveWindowToMonitor(h 0 0 100 300)
 out MoveWindowToMonitor(h 1 2)
