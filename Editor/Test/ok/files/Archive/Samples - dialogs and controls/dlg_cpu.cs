\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_cpu 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "CPU usage"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 msctls_progress32 0x54030004 0x0 10 10 14 86 ""
 4 Static 0x54000000 0x0 10 100 30 13 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	GetCPU
	SetTimer hDlg 1 1000 0
	
	case WM_TIMER
	sel wParam
		case 1
		int i=GetCPU
		SendMessage id(3 hDlg) PBM_SETPOS i 0
		_s.from(i " %"); _s.setwintext(id(4 hDlg))
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
