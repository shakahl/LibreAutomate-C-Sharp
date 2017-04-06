\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C802C8 0x0 0 0 224 136 "Dialog1"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 0x100)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	sub.Dialog2 hDlg
	 SetTimer hDlg 1 300 0
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		out SetWindowPos(hDlg _hwndqm 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)
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
 0 "" 0x90C802C8 0x0 0 20 124 36 "Dialog2"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd 0 0 hwndOwner 0x101)) ret

ret 1
