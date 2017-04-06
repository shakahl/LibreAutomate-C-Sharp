\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 typelib ARGradientControl {94F7E282-F78A-11D1-9587-0000B43369D3} 1.1
 typelib ARGradientControl "Q:\Tools, source, ocx\Components\UI\ARGradient.ocx"

if(!ShowDialog("dialog_activex_unregistered" &dialog_activex_unregistered)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 4 10 96 48 "ARGradientControl.ARGradient {94F7E278-F78A-11D1-9587-0000B43369D3} ''dll:%com%\UI\ARGradient.ocx''"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

 3 ActiveX 0x54030000 0x0 4 10 96 48 "ARGradientControl.ARGradient {94F7E278-F78A-11D1-9587-0000B43369D3}"

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
