
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040303 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 _hwndqm)) ret


#sub DlgProc
function# hDlg message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	out
	case WM_POWERBROADCAST
	DateTime t.FromComputerTime
	out "%i at %s" wParam t.ToStr(2|4|8)
	if wParam=PBT_APMRESUMEAUTOMATIC
		mac "sub.Thread"
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub Thread
int t0=timeGetTime
GetCPU
 GetDiskUsage
rep 20
	0.5
	 out "%g  cpu=%i disk=%i" timeGetTime-t0/1000.0 GetCPU GetDiskUsage
	outw win "" _s
	out "%g  cpu=%i  win=%s" timeGetTime-t0/1000.0 GetCPU _s


#sub Thread2
1
PF
out SendMessageTimeout(HWND_BROADCAST RegisterWindowMessage("QM_sleep") 0 0 SMTO_ABORTIFHUNG 30000 &_i)
PN
PO

 sleep: speed: 1671319  
 hiber: