\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_transparent" &dlg_transparent)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030008 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__GdiHandle-- hb=CreateSolidBrush(0xff00)
	Transparent hDlg 255 0xff00
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CTLCOLORDLG
	ret hb
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
