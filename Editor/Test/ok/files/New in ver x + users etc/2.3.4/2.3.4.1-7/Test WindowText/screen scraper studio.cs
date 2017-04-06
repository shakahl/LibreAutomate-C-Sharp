out
WT_ResultsVisualClose

int w=child(mouse); if(!w) w=win(mouse)
 int w=win("dlg_apihook" "#32770" "apihook"); if(!w) mac "dlg_apihook"; 1; w=win("dlg_apihook" "#32770" "apihook")
 int w=_hwndqm
 int w=win("Notepad")
 int w=win("" "CabinetWClass")
 int w=win("" "ExploreWClass") ;;XP
 int w=win("Document1 - Microsoft Word" "OpusApp")
 int w=win("Untitled 1 - OpenOffice.org Writer" "SALFRAME")
 int w=win("Microsoft Visual Studio" "wndclass_desked_gsk")
 int w=win("QM Help" "HH Parent")
 int w=win("Microsoft Document Explorer" "wndclass_desked_gsk")
 w=child("" "Internet Explorer_Server" w)
 int w=win("cmd.exe" "ConsoleWindowClass")
 int w=win("Foxit Reader" "classFoxitReader")
 int w=win("" "Mozilla*WindowClass" "" 0x804)
 int w=win("DLLReg" "wndclass_desked_gsk")
 w=child("" "VsTextEditPane" w 0 0 0 2)
 outw w
if(!w) mes- "w=0"

typelib TCaptureXLib {92657C70-D31B-4930-9014-379E3F6FB91A} 1.0
TCaptureXLib.TextCaptureX t._create
t.ExtractHighlightInfo=-1
 t.
t.CaptureWindow(w)
 out "----"


TCaptureXLib.THighlightInfo k=t.HighlightInfo
int i n=k.Count
ARRAY(WTI) ta.create(n)
for i 0 n
	BSTR b
	RECT r
	k.Get(i b r.left r.top r.right r.bottom)
	_s=b
	out "{%i %i %i %i} %s" r.left r.top r.right r.bottom _s
	 ta[i].ts=b
	ta[i].rt=r
	ta[i].rv=r

WT_ResultsVisual &ta[0] n w 0

