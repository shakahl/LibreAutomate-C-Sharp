\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str e3
int h=ShowDialog("Dialog94" &Dialog94 &controls _hwndqm 1)

MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	 if(m.message=WM_KEYDOWN and m.wParam>='A' and m.wParam<='Z') m.wParam+1
	
	 OutWinMsg m.message m.wParam m.lParam
	 if(m.message=WM_KEYDOWN and Function194(m)<0) continue
	 if(m.message=WM_KEYDOWN) Function194(m)
	sel(m.message) case [WM_KEYDOWN,WM_KEYUP,WM_CHAR] OutWinMsg m.message m.wParam m.lParam
	 sel(m.message) case [WM_KEYDOWN,WM_KEYUP,WM_CHAR] outx m.lParam
	 if(m.message=WM_KEYDOWN)
		  out GetTickCount-GetMessageTime
		 out GetTickCount-m.time
	
	if(m.message=WM_KEYDOWN and m.wParam='A' and GetMod=2) SendMessage(id(3 h) EM_SETSEL 0 -1); continue
	
	 if(m.message=WM_KEYDOWN) out "before: 0x%X" GetQueueStatus(QS_ALLPOSTMESSAGE)
	TranslateMessage &m
	 if(m.message=WM_KEYDOWN) out "after: 0x%X" GetQueueStatus(QS_ALLPOSTMESSAGE)
	 if(m.message=WM_KEYDOWN) out "after: 0x%X" GetQueueStatus(QS_ALLPOSTMESSAGE)
	DispatchMessage &m

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	PostQuitMessage 0
	case WM_COMMAND goto messages2
	case WM_LBUTTONUP
	 TerminateThread GetCurrentThread 0
	 ExitThread 0
	 outw GetFocus
	SetFocus 0
	 outw GetForegroundWindow
	 SetActiveWindow 0
	
	case WM_CONTEXTMENU
	ShowMenu "one[]two" hDlg
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x543310C4 0x200 0 0 102 134 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "" "" ""
