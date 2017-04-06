
 A hidden dialog that receives Windows user session notifications.


if(_winver<0x501) ret ;;not on Windows 2000

str dd=
 BEGIN DIALOG
 0 "" 0x80C800C8 0x0 0 0 224 136 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 128)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	WTSRegisterSessionNotification hDlg NOTIFY_FOR_THIS_SESSION
	
	case WM_DESTROY
	WTSUnRegisterSessionNotification hDlg
	
	case WM_WTSSESSION_CHANGE
	sel wParam
		case WTS_CONSOLE_CONNECT
		out "My session switched on"
		case WTS_CONSOLE_DISCONNECT
		out "My session switched off"
	
	case WM_COMMAND ret 1
