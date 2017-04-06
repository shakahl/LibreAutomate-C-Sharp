\Dialog_Editor
 WORKS
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib IcyInsideX "%com%\UI\ICY (sysinfo)\IcyInsideX.ocx"

if(!ShowDialog("AX_ICY" &AX_ICY)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 4 6 72 64 "IcyInsideX.IcyInside"
 4 Button 0x54032000 0x0 0 121 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	IcyInsideX.IcyInside i._getcontrol(id(3 hDlg))
	BSTR b="C:"
	i.DriveName=&b
	out i.IcyInspector
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 4 hid- id(3 hDlg)
ret 1
