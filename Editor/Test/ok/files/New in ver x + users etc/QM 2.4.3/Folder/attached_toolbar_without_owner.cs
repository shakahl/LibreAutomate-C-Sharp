if(getopt(nthreads)>1) CloseWindowsOf "" "QM_tb_test"; 0.2; ifk((2)) ret
out

int hhReo hhMin hhDestr hhFore
 int pid tid=GetWindowThreadProcessId(GetDesktopWindow &pid) ;;does not work
 hhReo=SetWinEventHook(EVENT_OBJECT_REORDER EVENT_OBJECT_REORDER 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT|WINEVENT_SKIPOWNTHREAD)
hhReo=SetWinEventHook(EVENT_OBJECT_REORDER EVENT_OBJECT_REORDER 0 &sub.Hook_SetWinEventHook_Reo 0 0 WINEVENT_OUTOFCONTEXT); if(!hhReo) end F"{ERR_FAILED}. {_s.dllerror}"
 int hhFore=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &sub.Hook_SetWinEventHook_Fore 0 0 WINEVENT_OUTOFCONTEXT); if(!hhFore) end F"{ERR_FAILED}. {_s.dllerror}"
hhMin=SetWinEventHook(EVENT_SYSTEM_MINIMIZESTART EVENT_SYSTEM_MINIMIZEEND 0 &sub.Hook_SetWinEventHook_Min 0 0 WINEVENT_OUTOFCONTEXT); if(!hhMin) end F"{ERR_FAILED}. {_s.dllerror}"
hhDestr=SetWinEventHook(EVENT_OBJECT_DESTROY EVENT_OBJECT_DESTROY 0 &sub.Hook_SetWinEventHook_Destr 0 0 WINEVENT_OUTOFCONTEXT); if(!hhDestr) end F"{ERR_FAILED}. {_s.dllerror}"

int- t_w t_wDestr
 t_w=_hwndqm
t_w=win("Untitled - Notepad" "Notepad")
 t_w=win("Calculator" "ApplicationFrameWindow")
t_wDestr=t_w
if(wintest(t_w "" "ApplicationFrameWindow")) t_wDestr=child("" "Windows.UI.Core.CoreWindow" t_w)

__RegisterWindowClass+ g_wc_tb; if(!g_wc_tb.atom) g_wc_tb.Register("QM_tb_test" &sub.WndProc 4)
int- t_stop
RECT r; GetWindowRect t_w &r
ARRAY(int)- t_a
int i L T S; str TX
for i 0 3
	L=r.left+50+(i*50); T=r.top-16; S=WS_POPUP|WS_CLIPSIBLINGS|WS_DLGFRAME; TX.from("TB " i)
	_i=CreateWindowExW(WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE +g_wc_tb.atom @TX S L T 100 30 0 0 _hinst 0)
	 _i=CreateWindowExW(0 +g_wc_tb.atom @TX S L T 100 30 0 0 _hinst 0)
	 _i=CreateWindowExW(WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE +g_wc_tb.atom @TX S L T 100 30 _hwndqm 0 _hinst 0)
	t_a[]=_i
	Zorder _i GetWindow(t_w GW_HWNDPREV) SWP_NOACTIVATE
	hid- _i

opt waitmsg 1
 mes 1
rep
	wait 1 V t_stop; err
	if(t_stop) break
	 sub.EnsureOverOwner

sub.DestroyAll
0
UnhookWinEvent hhReo
 UnhookWinEvent hhFore
UnhookWinEvent hhMin
UnhookWinEvent hhDestr


#sub EnsureOverOwner
function
int- t_w; int w=t_w
if(!IsWindow(w)) sub.DestroyAll; ret
ARRAY(int)- t_a
ARRAY(int) az=t_a
int i nOver h=w

PF
rep
	h=GetWindow(h GW_HWNDPREV); if(!h) break
	for(i 0 az.len)
		if(h=az[i]) az[i]=0; nOver+1; if(nOver<az.len) break; else goto g1
 g1
PN
 out "%i %i" nOver az.len
if(nOver<az.len)
	int hp=GetWindow(w GW_HWNDPREV)
	outw2 hp
	 for(i 0 az.len)
	for(i az.len-1 -1 -1)
		if(!az[i]) continue
		Zorder az[i] hp SWP_NOACTIVATE
PN
 opt waitmsg 1; 0.3
int- t_hidden t_cloaked; int hide cloak
if(!IsWindowVisible(w) or IsIconic(w)) if(!t_hidden) t_hidden=1; hide=1
else if(t_hidden) t_hidden=0; hide=-1
if hide
	int sw=iif(hide>0 SW_HIDE SW_SHOWNOACTIVATE)
	for(i 0 t_a.len) ShowWindow t_a[i] sw; ShowOwnedPopups t_a[i] hide<0
PN
if(IsWindowCloaked(w)) if(!t_cloaked) t_cloaked=1; cloak=1
else if(t_cloaked) t_cloaked=0; cloak=-1
if cloak
	for(i 0 t_a.len) DwmSetWindowAttribute t_a[i] 13 &t_cloaked 4

PN;PO

#sub WndProc
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

int- t_stop
sel message
	case WM_CREATE
	 PostMessage hWnd WM_APP 0 0
	 SetTimer hWnd 1 1000 0
	
	case WM_APP
	 case WM_TIMER
	 sel wParam
		 case 1
		 KillTimer hWnd 1
		 SetWinStyle hWnd WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE 4
		 SetWinStyle hWnd WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE 4
		SetWindowLong(hWnd GWL_HWNDPARENT 0)
		 outw GetWindow(hWnd GW_OWNER)
		 out "set"
	
	case WM_DESTROY
	t_stop=1

int R=DefWindowProcW(hWnd message wParam lParam)
ret R


#sub Hook_SetWinEventHook_Reo
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

 out "%i %i" dwmsEventTime GetTickCount
 outw2 hwnd ;;debug
 out "%i %i %i" event idObject idChild
if(idObject!OBJID_CLIENT or idChild) ret
if(hwnd!GetDesktopWindow) ret

 prevent recursion. Don't need this, because WINEVENT_SKIPOWNTHREAD somehow makes to skip event when zordered a window of this thread. But in QM then would not detect zordered QM thread windows.
int-- t_tick; int tick=GetTickCount
if(tick-t_tick<=50) ret; else t_tick=tick

sub.EnsureOverOwner


#sub Hook_SetWinEventHook_Min
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

 outw2 hwnd ;;debug
 out "%i %i %i" event idObject idChild
if(idObject or idChild) ret

sub.EnsureOverOwner


#sub Hook_SetWinEventHook_Destr
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

int- t_wDestr
if(idObject or idChild) ret
if(hwnd=t_wDestr) sub.DestroyAll


#sub DestroyAll
ARRAY(int)- t_a
int i
for(i 0 t_a.len) DestroyWindow(t_a[i])
t_a=0
