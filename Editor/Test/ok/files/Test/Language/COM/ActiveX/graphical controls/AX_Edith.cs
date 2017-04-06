\Dialog_Editor
function# hDlg message wParam lParam
 SHOWS BUT READONLY, AND FILE IS NOT LOADED. In VB too.
if(hDlg) goto messages
typelib EDITHLib {8B665013-5087-408D-BD96-344862B06140} 1.0

if(!ShowDialog("AX_Edith" &AX_Edith)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 4 6 200 100 "EDITHLib.EdithCtrl"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	EDITHLib.EdithCtrl c._create
	c.OpenFile(_s.expandpath("$desktop$\test.txt"))
	c.ReadOnly=0
	 out c.ReadOnly
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
