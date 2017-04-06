 SetWinStyle w WS_EX_TOOLWINDOW

int w1=win("Untitled - Notepad" "Notepad")
int w2=win("Document - WordPad" "WordPadClass")
 clo w2; ret
SetWinStyle w2 WS_EX_TOOLWINDOW 5
1
SetWindowLong(w2 GWL_HWNDPARENT w1)
 ret
1
SetWinStyle w2 WS_EX_TOOLWINDOW 6
1
SetWindowLong(w2 GWL_HWNDPARENT 0)
 1
 SetWinStyle w2 WS_EX_TOOLWINDOW 6
1
act w2
 min w2
 res w2
