\Dialog_Editor
 ERROR "Object doesn't support this action" on Enable, after COcxAmbient::GetIDsOfNames
 VB also does not work
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib CDNotification "%com%\UI\CDnotify\vb6\CDNotify6.ocx"

if(!ShowDialog("AX_CDNotify" &AX_CDNotify)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 6 8 21 20 "CDNotification.CDNotify"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	CDNotification.CDNotify c._getcontrol(id(3 hDlg))
	c._setevents("c___CDNotify")
	c.Enabled=1
	 out c.Enabled
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

err+ out _error.description
