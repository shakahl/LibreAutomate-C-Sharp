\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 4 4 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

out ShowDialog(dd &sub.DlgProc 0)


#sub DlgProc
function# hDlg message wParam lParam

int- ods
 if(ods) OutWinMsg message wParam lParam _s; OutputDebugString _s
 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_LBUTTONUP
	 PostQuitMessage 0; ret
	ods=1
	act "jjkjkj"
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	out
	 opt end 1
	 act "jjkjkj"
	
	shutdown -7 1
	 shutdown -7
	
	case IDOK
	case IDCANCEL
ret 1
