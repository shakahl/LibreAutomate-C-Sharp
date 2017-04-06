\read_cookie_mac
function# hDlg message wParam lParam
if(hDlg) goto messages

 Web browser control is defined in SHDocVw type library, which is already declared, so we don't have to declare it again.

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 277 200 "Submit writes a cookie"
 3 ActiveX 0x54000000 0x4 12 20 236 154 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x0 104 4 48 14 "OK"
 END DIALOG
 DIALOG EDITOR: "" 0x202000B "open_html_mac" ""


ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3._getcontrol(id(3 hDlg))
	we3._setevents("we3_DWebBrowserEvents2")
	we3.Navigate(site)
	
	case WM_SIZE
	RECT r; GetClientRect(hDlg &r)
	MoveWindow id(3 hDlg) 0 30 r.right r.bottom-30 1
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [4,5,6,7,8]
	err-
	we3._getcontrol(id(3 hDlg))
	sel wParam
		case 4 we3.GoBack
		case 5 we3.GoForward
		case 6 we3.Stop
		case 7 we3.Refresh
		case 8 we3.GoHome
	err+
	
	case IDOK
	case IDCANCEL
ret 1
