\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
if(!ShowDialog("dlg_controls_centered" &dlg_controls_centered &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 217 129 "Dialog"
 3 Button 0x54032000 0x0 54 24 50 16 "Button"
 4 Edit 0x54030080 0x200 116 86 96 14 ""
 5 Button 0x54032000 0x0 64 110 48 14 "Button"
 6 Button 0x54032000 0x0 114 110 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages

DT_AutoCenterControls hDlg message "3 4 5 6"

sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
