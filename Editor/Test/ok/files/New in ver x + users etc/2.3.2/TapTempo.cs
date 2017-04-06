\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "5"
str e5
if(!ShowDialog("TapTempo" &TapTempo &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 48 "Tap Tempo"
 3 Button 0x54032000 0x0 82 2 48 14 "Restart"
 4 Static 0x54000000 0x0 0 2 78 13 ""
 6 Static 0x54000000 0x0 2 32 76 12 "Number of last taps"
 5 Edit 0x54032000 0x200 82 30 32 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

ret
 messages

ARRAY(int)-- a

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	a=0
	SetTimer
	
	case IDOK
	case IDCANCEL
ret 1
