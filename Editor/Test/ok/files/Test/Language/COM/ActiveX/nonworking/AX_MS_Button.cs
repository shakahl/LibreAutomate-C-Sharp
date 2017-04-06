\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib MSComctlLib {831FDD16-0C5C-11D2-A9FC-0000F8754DA1} 2.0

if(!ShowDialog("AX_MS_Button" &AX_MS_Button)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 6 8 46 16 "MSComctlLib.Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
