\Dialog_Editor
 Failed to create if before that have been tested many other controls.
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib HLink "%com%\ui\HLink\HLink.ocx"
 HLink.HLink h._create
if(!ShowDialog("DlgHLink" &DlgHLink 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 14 6 38 12 "HLink.HLink"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	HLink.HLink hl3._getcontrol(id(3 hDlg))
	hl3._setevents("hl3___HLink")
	hl3.URL="http://www.quickmacros.com"
	
	ret 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
ret 1
