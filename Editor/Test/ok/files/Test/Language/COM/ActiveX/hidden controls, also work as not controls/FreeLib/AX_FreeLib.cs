\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib FreeLibSrc "%com%\UI\FreeLib (misc)\FreeLib.ocx"

if(!ShowDialog("AX_FreeLib" &AX_FreeLib)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 6 8 21 20 "FreeLibSrc.FreeLib"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	FreeLibSrc.FreeLib f._getcontrol(id(3 hDlg))
	ChDir "%com%\UI\FreeLib (misc)"
	out f.CPUCount
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
