 /Macro2645
 \Dialog_Editor
 out GetWindowThreadProcessId(_hwndqm 0)
 out GetCurrentThreadId

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

 out ShowDialog(dd &sub.DlgProc 0)
int d=ShowDialog(dd &sub.DlgProc 0 0 1)
 opt waitmsg 1
 wait 0 -WC d

 out call(&sub.Call)

#sub Call

min 0
ret 1


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 min 0
	case WM_LBUTTONUP
	min 0
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
