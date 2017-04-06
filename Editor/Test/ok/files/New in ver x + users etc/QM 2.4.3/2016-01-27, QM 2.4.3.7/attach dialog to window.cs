type TB5016752 hdlg howner
ARRAY(TB5016752) a

int hhFore=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &sub.HookProcFore 0 0 WINEVENT_OUTOFCONTEXT|WINEVENT_SKIPOWNTHREAD)
if(!hhFore) end F"{ERR_FAILED}. {_s.dllerror}"
int hhLoc=SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE EVENT_OBJECT_LOCATIONCHANGE 0 &sub.HookProcLoc 0 0 WINEVENT_OUTOFCONTEXT|WINEVENT_SKIPOWNTHREAD)
if(!hhLoc) end F"{ERR_FAILED}. {_s.dllerror}"
int idTimer=SetTimer(0 1 500 0)

MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	sel m.message
		case WM_TIMER
		if(m.wParam=idTimer) sub.Timer1
		continue
		
		case WM_APP
		if IsWindow(m.wParam)
			int h=sub.Dialog(m.wParam)
			if h
				TB5016752& r=a[]; r.howner=m.wParam; r.hdlg=h
				sub.Attach(a.len-1)
		continue
	
	TranslateMessage &m; DispatchMessage &m

UnhookWinEvent hhFore
UnhookWinEvent hhLoc
int i
for(i a.len-1 -1 -1) clo a[i].hdlg


#sub HookProcFore v
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

if(!hwnd or idObject or idChild) ret
str sc.getwinclass(hwnd); err ret
sel sc 1
	case "Notepad"
	int i
	for(i 0 a.len) if(hwnd=a[i].howner) ret
	PostMessage 0 WM_APP hwnd 0


#sub HookProcLoc v
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

 This code runs whenever a window
 or mouse pointer, caret, control, etc
 has locationchange.

 outw hwnd
if(!hwnd) ret ;;probably mouse
 out idObject
if(idObject!=OBJID_WINDOW) ret ;;eg caret
 out idChild
if(idChild) ret
int style=GetWinStyle(hwnd); if(style&WS_CHILD) ret

int i
for(i 0 a.len) if(hwnd=a[i].howner) sub.Attach(i); break


#sub Attach v
function i

 out "App Window has moved"
TB5016752& r=a[i]
int x y cx cy
GetWinXY(r.howner x y cx cy)
mov+ x+50 y+5 cx-225 0 r.hdlg 0x200


#sub Timer1 v
int i
for i 0 a.len
	if !IsWindow(a[i].howner)
		clo a[i].hdlg; err


#sub Dialog v
function# hwndOwner

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 32 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040303 "*" "" "" ""

int h=ShowDialog(dd &sub.DlgProc 0 hwndOwner 1)
act hwndOwner; err
ret h


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	int i
	for(i a.len-1 -1 -1) if(hDlg=a[i].hdlg) a.remove(i); break
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
