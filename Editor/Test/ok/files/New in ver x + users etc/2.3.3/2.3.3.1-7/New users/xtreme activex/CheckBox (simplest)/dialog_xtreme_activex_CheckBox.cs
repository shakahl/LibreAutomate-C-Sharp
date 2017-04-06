\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib XtremeSuiteControls {A8E5842E-102B-4289-9D57-3B3F5B5E15D3} 15.0

if(!ShowDialog("dialog_xtreme_activex_CheckBox" &dialog_xtreme_activex_CheckBox)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 2 4 94 13 "XtremeSuiteControls.CheckBox {DA45B02D-8341-4AF7-AF5F-D38E6F547876}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	XtremeSuiteControls.CheckBox ch3
	ch3._getcontrol(id(3 hDlg))
	ch3._setevents("ch3__DCheckBoxEvents")
	ch3.Caption="eeeeeee"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
