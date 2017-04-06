 int w=win("3D Builder" "ApplicationFrameWindow")
 int w=win("3D Builder" "ApplicationFrameWindow" "" 0x0 "cClass=Windows.UI.Core.CoreWindow")
 outw w
 if IsWindowCloaked(w)
	 out 1

ARRAY(int) a
GetMainWindows(a 4)
 sample code, shows how to use the array
out
int i hwnd
for i 0 a.len
	hwnd=a[i]
	outw hwnd
	out IsWindowCloaked(hwnd)
