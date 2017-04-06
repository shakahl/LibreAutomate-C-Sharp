\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("draw_on_control" &draw_on_control)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000000 0x0 10 10 66 46 "Text"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_DrawOnControl id(3 hDlg) &draw_on_control_proc
	 DT_DrawOnControl hDlg &draw_on_control_proc
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
