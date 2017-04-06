int w=win("Visual Studio" "wndclass_desked_gsk")

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x80 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 16 48 14 "owned"
 4 Button 0x54032000 0x0 60 16 48 14 "free"
 5 Button 0x54032000 0x4 112 16 48 14 "child-owned"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

w=0
if(!ShowDialog(dd &sub.DlgProc 0 w)) ret


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 SetWindowLong(hDlg GWL_HWNDPARENT 0)
	 SetTimer hDlg 1 1000 0
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg 1
		outw SetWindowLong(hDlg GWL_HWNDPARENT 0)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	SetWindowLong(hDlg GWL_HWNDPARENT w)
	case 4
	SetWindowLong(hDlg GWL_HWNDPARENT 0)
	case 5
	int c=child("" "QM_toolbar_owner" w)
	outw c
	SetWindowLong(hDlg GWL_HWNDPARENT c)
ret 1
