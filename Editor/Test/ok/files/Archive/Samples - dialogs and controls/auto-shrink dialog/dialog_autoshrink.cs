\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="http://www.quickmacros.com"
if(!ShowDialog("dialog_autoshrink" &dialog_autoshrink &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 14 224 100 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages

DT_AutoShrink hDlg message wParam lParam

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
