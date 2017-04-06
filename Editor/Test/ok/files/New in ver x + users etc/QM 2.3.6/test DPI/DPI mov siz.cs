 int w=win("Microsoft Spy++ - [Windows 1]" "Afx:*" "" 0x4)
 mov 1129 570 w
 siz 549 370 w

 int w=win("Untitled - Notepad" "Notepad")
int w=win("DebugView" "dbgviewClass")
act w
 mov+ 100 200 300 400 w

 CenterWindow w
 EnsureWindowInScreen w
 SaveMultiWinPos "test" "DebugView"; 5; RestoreMultiWinPos "test"
