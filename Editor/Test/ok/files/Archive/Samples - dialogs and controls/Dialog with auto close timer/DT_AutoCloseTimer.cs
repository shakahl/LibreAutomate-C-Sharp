 /
function &hDlg idStatic timeout idButton

 Adds a countdown timer and closes the dialog when the time expires.
 To stop the timer, let the user click somewhere in the dialog.
 Call this function before sel message in dialog procedure.
 Example: see dlg_timer_auto_close

 hDlg - must be hDlg.
 idStatic - id of static control where to display the remaining time.
 timeout - number of seconds after which to close the dialog.
 idButton - id of default button. On timeout will click it.


int message wParam lParam; memcpy &message &hDlg+4 12
sel message
	case WM_INITDIALOG
	_s=timeout; _s.setwintext(id(idStatic hDlg))
	SetTimer hDlg 8004 1000 0
	SendMessage hDlg DM_SETDEFID idButton 0; SetFocus id(idButton hDlg)
	
	case WM_TIMER
	sel wParam
		case 8004
		int t=val(_s.getwintext(id(idStatic hDlg)))
		t-1
		_s=t; _s.setwintext(id(idStatic hDlg))
		if(t<1) KillTimer(hDlg 8004); but idButton hDlg
	
	case WM_SETCURSOR
	sel(lParam>>16)
		case [WM_LBUTTONDOWN,WM_NCLBUTTONDOWN]
		KillTimer hDlg 8004

err+
