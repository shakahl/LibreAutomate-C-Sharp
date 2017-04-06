\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="http://www.google.com"
if(!ShowDialog("DialogDragExample3" &DialogDragExample3 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90400AC8 0x180 0 0 223 147 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 136 "SHDocVw.WebBrowser"
 4 Static 0x54000000 0x0 0 138 208 12 "Drag the web browser control. Press Esc to close."
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 subclass web browser
	int hctrl=child("" "Internet Explorer_Server" id(3 hDlg)); if(!hctrl) out "Warning: Web browser control not initialized. Assign an url or ''about:blank'' to the control variable before ShowDialog."; ret
	DialogDragSubclassControl hctrl
	case WM_DESTROY
	case WM_COMMAND goto messages2
	 case [WM_LBUTTONDOWN,WM_LBUTTONUP,WM_MOUSEMOVE,WM_CANCELMODE] DragDialog hDlg message ;;this also can be used
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
