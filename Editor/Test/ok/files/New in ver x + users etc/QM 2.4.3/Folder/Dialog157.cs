\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x40000 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 128)) ret


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
 0 "" 0x90C80AC8 0x40000 0 0 150 58 "Dialog"
 1 Button 0x54030001 0x4 24 20 48 14 "OK"
 2 Button 0x54030000 0x4 80 20 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

 if(!ShowDialog(dd &sub.DlgProc2 0 hwndOwner)) ret
int w=ShowDialog(dd &sub.DlgProc2 0 hwndOwner 1)
opt waitmsg 1
wait 0 -WC w
clo hwndOwner

ret 1


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 EnableWindow GetParent(hDlg) 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
