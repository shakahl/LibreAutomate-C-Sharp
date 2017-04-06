\Dialog_Editor
typelib MSComDlg {F9043C88-F6F2-101A-A3C9-08002B2F49FB} 1.2
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_CommonDialogs" &AX_CommonDialogs)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 87 96 48 "MSComDlg.CommonDialog {F9043C85-F6F2-101A-A3C9-08002B2F49FB}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MSComDlg.CommonDialog co3
	co3._getcontrol(id(3 hDlg))
	co3.ShowOpen
	out co3.FileName
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
