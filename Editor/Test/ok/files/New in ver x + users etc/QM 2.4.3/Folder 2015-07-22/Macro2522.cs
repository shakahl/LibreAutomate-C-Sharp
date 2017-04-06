out
 out GetKeyState(VK_SCROLL)&1
 int w=win("Windows Feedback" "ApplicationFrameWindow" "" 0x400)
 outw w

 opt hidden 1
 out res("Windows Feedback")

dll "qm.exe" #__GetAppFrameWindowType hwnd [*hwndChild]

ARRAY(int) a
win "" "ApplicationFrameWindow" "" 0 "" a
int i
for i 0 a.len
	out "---"
	int w=a[i]
	int c t=__GetAppFrameWindowType(w &c)
	outw w
	outw c
	out "type=%i" t
