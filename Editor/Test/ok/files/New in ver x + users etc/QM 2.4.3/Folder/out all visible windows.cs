out
int flags=4 ;;flags: 4 get popup if main window is disabled
 ;;TODO: flag to show only current Windows 10 desktop windows
 opt hidden 1
ARRAY(int) a
win "" "" "" 0 "" a
int i topmost(1)
for i 0 a.len
	int w(a[i]) style(GetWinStyle(w)) exstyle(GetWinStyle(w 1))
	if exstyle&WS_EX_APPWINDOW=0
		if(exstyle&(WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE)) continue
		if(GetWindow(w GW_OWNER)) continue
	if _winver>=0x602 and exstyle&WS_EX_NOREDIRECTIONBITMAP ;;Windows 8
		if(_winver<0xA00 and exstyle&WS_EX_TOPMOST && sub.IsExplorerWindow(w)) continue
		sel _s.getwinclass(w) 1
			case "Windows.UI.Core.CoreWindow"
			if(_winver>=0xA00) continue
			 if(exstyle&WS_EX_TOPMOST=0) continue
			 OutWinProps w
			case "ApplicationFrameWindow"
			if _winver>=0xA00
				 PF
				_s.getwintext(w); if(!_s.len) continue
				 if(!FindWindowEx(w 0 "Windows.UI.Core.CoreWindow" 0)) continue
				 PN;PO
	
	if(topmost and exstyle&WS_EX_TOPMOST=0) topmost=0; out "---- NON-TOPMOST WINDOWS ----"
	if(flags&4 and !IsWindowEnabled(w)) w=GetLastActivePopup(w) ;;test also: GetWindow(w GW_ENABLEDPOPUP)
	outw2 w
	
	if _winver>=0x602
		if(!DwmGetWindowAttribute(w 14 &_i 4) and _i) out "<cloaked>"




#sub IsExplorerWindow
function w
 Returns 1 if w belongs to the shell process and does not have a caption.

if(GetWinStyle(w)&WS_CAPTION) ret
int+ g_hShell g_pidShell; if(!IsWindow(g_hShell)) g_hShell=GetShellWindow; GetWindowThreadProcessId(g_hShell &g_pidShell)
int pid; GetWindowThreadProcessId(w &pid)
ret pid=g_pidShell
