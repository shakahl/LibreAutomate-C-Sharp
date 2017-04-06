\Dialog_Editor
typelib TWSLib {0A77CCF5-052C-11D6-B0EC-00B0D074179C} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("test_TWS" &test_TWS 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 87 96 48 "TWSLib.Tws {0A77CCF8-052C-11D6-B0EC-00B0D074179C}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

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
