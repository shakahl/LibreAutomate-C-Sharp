\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Web browser control is defined in SHDocVw type library, which is already declared, so we don't have to declare it again.

str controls = "3"
str ax3SHD

 To initialize web browser control, you can assign an URL or path or HTML to
 the variable, like in the commented examples below. Or, later use setwintext.
 Or you can use Navigate, like in the code below.

 ax3SHD="" ;;load empty page
 ax3SHD="http://www.google.com" ;;open an URL
 ax3SHD="<p>text <b>bold</b></p>" ;;load HTML (must begin with "<")
 IntGetFile "http://www.quickmacros.com" ax3SHD; err ret ;;at first download HTML from the web

if(!ShowDialog("Dialog_with_web_browser" &Dialog_with_web_browser &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A48 0x100 0 0 277 200 "Form"
 3 ActiveX 0x54000000 0x4 12 20 236 154 "SHDocVw.WebBrowser"
 4 Button 0x54032001 0x4 0 0 48 14 "Back"
 5 Button 0x54032000 0x4 48 0 48 14 "Forward"
 6 Button 0x54032000 0x0 96 0 48 14 "Stop"
 7 Button 0x54032000 0x0 144 0 48 14 "Refresh"
 8 Button 0x54032000 0x0 192 0 48 14 "Home"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""


ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3._getcontrol(id(3 hDlg))
	we3._setevents("we3_DWebBrowserEvents2")
	we3.Silent=TRUE ;;prevent script error messages
	we3.Navigate("http://www.google.com")
	
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
