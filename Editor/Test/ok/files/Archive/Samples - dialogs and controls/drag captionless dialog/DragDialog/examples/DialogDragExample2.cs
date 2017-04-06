\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("DialogDragExample2" &DialogDragExample2)) ret

 BEGIN DIALOG
 0 "" 0x90400AC8 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 10 54 42 "Drag Me"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DialogDragSubclassControl id(3 hDlg) ;;subclass button 3
	case WM_DESTROY
	case WM_COMMAND goto messages2
	 case [WM_LBUTTONDOWN,WM_LBUTTONUP,WM_MOUSEMOVE,WM_CANCELMODE] DragDialog hDlg message ;;this also can be used
ret
 messages2
sel wParam
	case 3 out "clicked"
	case IDOK
	case IDCANCEL
ret 1
