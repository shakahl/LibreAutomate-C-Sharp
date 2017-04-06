 int w=win("" "QM_Editor")
 int w=win("Untitled - Notepad" "Notepad")
int w=win("Document1 - Microsoft Word" "OpusApp")
 int w=win("Calculator" "ApplicationFrameWindow") ;;black
 w=child("Calculator" "Windows.UI.Core.CoreWindow" w) ;; 'Calculator'
 int w=win("Downloads" "CabinetWClass")
 int w=win("Google Maps - Internet Explorer" "IEFrame") ;;mostly white
 w=child("" "Internet Explorer_Server" w 0x0) ;; 'https://www.google.com/maps...'
 int w=win("▶ Full Documentary | Space Documentary National Geographic Death of the Sun - YouTube - Mozilla Firefox" "MozillaWindowClass")
 int w=GetDesktopWindow ;;works, but slow, the same as screen DC
 int w=GetShellWindow ;;desktop, fast. All monitors. With wallpapars, but without icons. Then desktop was WorkerW, not GetShellWindow.
 int w=win("DebugView" "dbgviewClass")

if(!w) end ERR_WINDOW
int wid hei
PF
 int dc=GetWindowDC(w); GetWinXY w 0 0 wid hei
int dc=GetDC(w); RECT r; GetClientRect w &r; wid=r.right; hei=r.bottom
__MemBmp b
if(!b.Create(wid hei dc)) ret
ReleaseDC w dc
PN;PO

if(OpenClipboard(_hwndqm)) EmptyClipboard; SetClipboardData(CF_BITMAP b.Detach); CloseClipboard
int w1=win("Paint" "MSPaintApp")
act w1
key Cv
