\Dialog_Editor
typelib REALDIGITSLib {58635701-4313-11D1-9D7F-CD6975009A1F} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_RealDigits" &AX_RealDigits)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 2 4 75 13 "REALDIGITSLib.RD {58635704-4313-11D1-9D7F-CD6975009A1F}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	REALDIGITSLib.RD rd3
	rd3._getcontrol(id(3 hDlg))
	rd3.Digits="1234567890"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
