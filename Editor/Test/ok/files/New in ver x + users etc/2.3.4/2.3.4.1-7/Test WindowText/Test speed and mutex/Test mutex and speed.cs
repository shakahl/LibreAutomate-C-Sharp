out
int w=win("dlg_apihook" "#32770" "apihook"); if(!w) mac "dlg_apihook"; 1; w=win("dlg_apihook" "#32770" "apihook")
 int w=win("Untitled - Notepad" "Notepad")
 int w=win("Task Scheduler" "MMCMainFrame")
 int w=win("Microsoft Document Explorer" "wndclass_desked_gsk")
 int w=win("Document1 - Microsoft Word" "OpusApp")
 int w=win("Google - Windows Internet Explorer" "IEFrame")

Q &q
rep 1
	 mac "QMTC_call_CaptureWindowText" "" w
	QMTC_call_CaptureWindowText w
Q &qq; outq
