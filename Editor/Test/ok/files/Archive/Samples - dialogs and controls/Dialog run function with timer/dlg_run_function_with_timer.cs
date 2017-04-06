\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_run_function_with_timer" &dlg_run_function_with_timer 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 6 32 48 14 "Start"
 4 Button 0x54032000 0x0 58 32 48 14 "Stop"
 5 Static 0x54000000 0x0 6 54 100 24 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
int-- t0
sel message
	case WM_INITDIALOG
	case WM_DESTROY goto g2
	case WM_COMMAND goto messages2
	case WM_TIMER
	int s(GetTickCount-t0/1000) m h
	m=s/60; s%60
	h=m/60; m%60
	 g1
	_s.format("%02i:%02i:%02i" h m s); _s.setwintext(id(5 hDlg))
ret
 messages2
sel wParam
	case 3 ;;Start
	str-- func
	func="drfwt_function" ;;change this
	mac func; err ret
	t0=GetTickCount
	SetTimer hDlg 1 100 0
	goto g1
	
	case 4 ;;Stop
	 g2
	KillTimer hDlg 1
	shutdown -6 0 func; err
	ret
	
	case IDOK
	case IDCANCEL
ret 1
