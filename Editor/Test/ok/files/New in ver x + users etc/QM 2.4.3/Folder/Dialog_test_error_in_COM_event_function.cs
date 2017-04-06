\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 338 204 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 338 180 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 4 Button 0x54032000 0x0 4 188 48 14 "Button"
 1 Button 0x54030001 0x4 108 188 48 14 "OK"
 2 Button 0x54030000 0x4 164 188 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3"
str ax3SHD
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

SHDocVw.WebBrowser we3
we3._getcontrol(id(3 hDlg))
sel message
	case WM_INITDIALOG
	we3._setevents("sub.we3")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	we3.Navigate("http://www.quickmacros.com")
	case IDCANCEL
ret 1


#sub we3_NavigateComplete2
function IDispatch'pDisp `&URL ;;SHDocVw.IWebBrowser2'we3
out 1
child("" "h" _i)
