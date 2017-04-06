int h=win("Notepad" "Notepad")
 MoveWindowToMonitor h 1
 MoveWindowToMonitor h 2 0 300 300
 MoveWindowToMonitor h 1 1 -100 -100
 MoveWindowToMonitor h 1 2 -300 -100
MoveWindowToMonitor h 1 1

 EnsureWindowInScreen h
 EnsureWindowInScreen h 1
