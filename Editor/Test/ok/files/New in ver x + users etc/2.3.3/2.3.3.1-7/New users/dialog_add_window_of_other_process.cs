\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_add_window_of_other_process" &dialog_add_window_of_other_process)) ret

 BEGIN DIALOG
 0 "" 0x92C80AC8 0x0 0 0 345 238 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int w
	run "$system$\calc.exe" "" "" "" 0x2800 win("Calculator" "CalcFrame") w
	SetParent w hDlg
	SetWinStyle w WS_CAPTION 2
	mov 0 0 w
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
