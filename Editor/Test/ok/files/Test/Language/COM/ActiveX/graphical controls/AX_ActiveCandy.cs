\Dialog_Editor
typelib ActiveCandy3 {0F12C15B-95AC-4475-BC66-4111B805F39E} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_ActiveCandy" &AX_ActiveCandy)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 96 48 "ActiveCandy3.CandyCheck {0DB8B15E-1452-4A98-A485-24DB16BC5D38}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ActiveCandy3.CandyCheck ca3
	ca3._getcontrol(id(3 hDlg))
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
