\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x94CF0A4C 0x10100 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 0 "" 0x94CF0A4C 0x10108 0 0 224 136 "Dialog"

if(!ShowDialog(dd &sub.DlgProc)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 SetWinStyle hDlg WS_EX_TOPMOST 4|2
	
	 SetTimer hDlg 1 1000 0
	PostMessage hDlg WM_APP 0 0
	
	 case WM_TIMER
	 KillTimer hDlg wParam
	case WM_APP
	Zorder hDlg HWND_TOPMOST
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
