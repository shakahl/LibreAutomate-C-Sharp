\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

_monitor=2
if(!ShowDialog(dd &sub.DlgProc 0 0 0 0 0 0 -1 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 500 0
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		sub.Dialog2(hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub Dialog2
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 118 52 "Dialog"
 1 Button 0x54030001 0x4 12 32 48 14 "OK"
 2 Button 0x54030000 0x4 64 32 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

_monitor=0
 if(!ShowDialog(dd 0 0 hwndOwner)) ret
if(!ShowDialog(dd 0 0 hwndOwner 64 0 0 0 283 1080)) ret
 if(!ShowDialog(dd 0 0 hwndOwner 64 0 0 0 0 0)) ret

ret 1
