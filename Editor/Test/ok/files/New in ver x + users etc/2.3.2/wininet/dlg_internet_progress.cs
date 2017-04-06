\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_internet_progress" &dlg_internet_progress)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 10 12 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030207 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		goto g1
ret
 messages2
sel wParam
	case 3
	goto g1
	SetTimer hDlg 1 5000 0
	
	case IDCANCEL
ret 1
 g1
str s="$temp$\test.txt"
IntGetFile("http://www.quickmacros.com/test/test.txt" s 16 0 hDlg)
