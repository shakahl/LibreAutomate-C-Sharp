 int w=win("DebugView" "dbgviewClass")
 int c=id(1000 w) ;;list
 int w=win("Untitled - Notepad" "Notepad")
 int c=id(15 w) ;;editable text
 mov 0.5 0.5 c
 siz 0.5 0.5 c
 #ret

int w=win("DebugView" "dbgviewClass")
 int c=child("" "" w 0x0 "id=1000[]xy=300 150") ;;list
int c=child("" "" w 0x0 "id=1000[]xy=0.7 0.7") ;;list

 int w=win("Untitled - Notepad" "Notepad")
 int c=child("" "" w 0x0 "id=15[]xy=312 182") ;;editable text


RECT r
DpiGetWindowRect c &r
OnScreenRect 0 r
1
