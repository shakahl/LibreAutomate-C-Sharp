int w=win("" "Shell_TrayWnd")
int c=child("Notification Area" "ToolbarWindow32" w)
if(!c) end "tray control not found. Maybe text is different, for example not English."
RECT r; GetClientRect c &r
int x y
for y r.bottom-8 0 -10
	for x r.right-4 0 -4
		PostMessage c WM_MOUSEMOVE 0 MakeInt(x y)

 Tested on Windows 7 and XP.
 This does not remove hidden icons. It would be difficult.
