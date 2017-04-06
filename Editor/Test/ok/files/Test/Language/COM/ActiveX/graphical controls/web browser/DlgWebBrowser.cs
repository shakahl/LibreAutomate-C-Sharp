\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str ax4SHD
if(!ShowDialog("DlgWebBrowser" &DlgWebBrowser &controls)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 393 311 "Form"
 3 Button 0x54032000 0x4 4 295 48 14 "Button"
 4 ActiveX 0x54000000 0x4 0 0 392 292 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 118 295 48 14 "OK"
 2 Button 0x54030000 0x4 168 295 48 14 "Cancel"
 5 Button 0x54032000 0x4 58 295 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we4._getcontrol(id(4 hDlg))
	we4._setevents("we4_DWebBrowserEvents2")
	 IntGoOnline 0
	 we4.Navigate("about:blank")
	we4.Navigate("http://www.google.com/")
	 we4.Navigate("E:\MyProjects\app\web\free.html")
	
	 IDispatch p._getcontrol(id(4 hDlg))
	
	ret 1
	case WM_SIZE
	RECT r; GetClientRect(hDlg &r)
	siz r.right r.bottom-40 id(4 hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 hid(id(4 hDlg))
	case 5 hid-(id(4 hDlg))
	case IDOK
	case IDCANCEL
ret 1
