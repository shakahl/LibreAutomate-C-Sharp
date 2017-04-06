\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=iif(_command _command "")
if(!ShowDialog("PhpDebugDlg" &PhpDebugDlg &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 343 214 "PhpDebug"
 2 Button 0x54030000 0x4 144 198 48 14 "Close"
 3 ActiveX 0x54030000 0x0 0 0 344 194 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""
 
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

