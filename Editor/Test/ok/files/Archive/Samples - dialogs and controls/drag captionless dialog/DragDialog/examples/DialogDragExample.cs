\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("DialogDragExample" &DialogDragExample)) ret

 BEGIN DIALOG
 0 "" 0x90400AC8 0x100 0 0 223 146 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 6 6 96 14 ""
 4 Static 0x54000000 0x0 80 60 48 13 "Drag"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case [WM_LBUTTONDOWN,WM_LBUTTONUP,WM_MOUSEMOVE,WM_CANCELMODE] DragDialog hDlg message
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
