\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog1"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	PostMessage hDlg WM_APP 0 0
	case WM_APP
	sub.Dialog2 hDlg
	
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
 0 "" 0x90C80AC8 0x0 0 0 152 70 "Dialog2"
 1 Button 0x54030001 0x4 20 28 48 14 "OK"
 2 Button 0x54030000 0x4 72 28 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc2 0 hwndOwner 128)) ret

ret 1


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SetForegroundWindow hDlg
	PostMessage hDlg WM_APP 0 0
	case WM_APP
	outw GetLastActivePopup(GetParent(hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
