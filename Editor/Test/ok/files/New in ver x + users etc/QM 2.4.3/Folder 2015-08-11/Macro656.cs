\Dialog_Editor
sub.Dialog2


#sub Dialog2
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ActiveX 0x54030000 0x0 8 8 96 48 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc2 0 hwndOwner)) ret

ret 1


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3._setevents("sub.we3")
	we3.Navigate("http://www.quickmacros.com")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub we3_DocumentComplete
function IDispatch'pDisp `&URL ;;SHDocVw.IWebBrowser2'
out URL
 min 0



#sub we3_NavigateError
function IDispatch'pDisp `&URL `&Frame `&StatusCode @&Cancel ;;SHDocVw.IWebBrowser2'we3


#sub we3_FileDownload
function @ActiveDocument @&Cancel ;;SHDocVw.IWebBrowser2'we3
