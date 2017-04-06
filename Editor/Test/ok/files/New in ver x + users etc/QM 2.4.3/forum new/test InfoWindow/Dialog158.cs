 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 8 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

int- t_iw
sel message
	case WM_INITDIALOG
	 t_iw=InfoWindow(0 0 1)
	t_iw=InfoWindow(0 0 0)
	SetTimer hDlg 1 1000 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_TIMER goto timers
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 timers
sel wParam
	case 1
	if(!IsWindow(t_iw)) t_iw=0; KillTimer hDlg wParam; ret
	_s.RandomString(3 3)
	_s.setwintext(t_iw)
ret 1
