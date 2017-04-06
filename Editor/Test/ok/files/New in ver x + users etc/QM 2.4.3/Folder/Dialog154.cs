\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 0 0 48 14 "topmost"
 4 Button 0x54032000 0x0 52 0 48 14 "notopmost"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	SetWindowPos(hDlg HWND_TOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE)
	case 4
	rep 2
		SetWindowPos(hDlg HWND_NOTOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_NOOWNERZORDER)
		 0.001
ret 1
