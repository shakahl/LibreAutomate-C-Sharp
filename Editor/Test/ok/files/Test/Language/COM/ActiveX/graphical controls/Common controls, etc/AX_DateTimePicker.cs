\Dialog_Editor
typelib MSComCtl2 {86CF1D34-0C5F-11D2-A9FC-0000F8754DA1} 2.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_DateTimePicker" &AX_DateTimePicker)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 4 Button 0x54030000 0x4 120 116 48 14 "Button"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 4 6 100 28 "MSComCtl2.DTPicker {20DD1B9E-87C4-11D1-8BE3-0000F8754DA1}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MSComCtl2.DTPicker dt3
	dt3._getcontrol(id(3 hDlg))
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	dt3._getcontrol(id(3 hDlg))
	out dt3._Value
	case IDCANCEL
ret 1
