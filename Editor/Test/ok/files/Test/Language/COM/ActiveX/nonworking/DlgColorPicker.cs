\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 When clicked, creates new control, and therefore does not work on QM.

typelib MBColorPicker {85FD608E-54A8-11D4-8ED4-00E07D815373} 1.0

if(!ShowDialog("DlgColorPicker" &DlgColorPicker 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x4 4 8 96 16 "MBColorPicker.ColorPicker"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	MBColorPicker.ColorPicker co3._getcontrol(id(3 hDlg))
	 co3.ShowPalette
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
