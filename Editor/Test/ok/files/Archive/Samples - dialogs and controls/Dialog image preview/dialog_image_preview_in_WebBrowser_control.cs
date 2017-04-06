\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90CC0A48 0x10100 0 0 226 140 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 226 140 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3"
str ax3SHD

 ax3SHD="$desktop$\Example_1.jpg"
if(!OpenSaveDialog(0 ax3SHD)) ret

if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_SetAutoSizeControls hDlg "3s"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1