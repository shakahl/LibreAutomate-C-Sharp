\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 #exe addactivex "SHDocVw.WebBrowser"

str controls = "3 4"
str ax3SHD ax4SHD
ax3SHD="http://www.quickmacros.com"
ax4SHD="http://www.google.com"
if(!ShowDialog("exe_ActiveX" &exe_ActiveX &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 254 "Form"
 3 ActiveX 0x54000000 0x0 0 0 224 114 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 122 238 48 14 "OK"
 2 Button 0x54030000 0x4 172 238 48 14 "Cancel"
 4 ActiveX 0x54000000 0x0 0 120 224 114 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
if(message=WM_INITDIALOG) DT_Init(hDlg lParam)
 int param=DT_GetParam(hDlg)

sel message
	case WM_INITDIALOG
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
