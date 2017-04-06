\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
 ax3SHD=""
 ax3SHD="http://www.google.com"
 ax3SHD="<html><body>normal <b>bold</b></body></html>"
 ax3SHD="<body>normal <b>bold</b></body>"
 ax3SHD="<i>italic</i> <b>bold</b>"
ax3SHD="<p>text <b>bold</b></p>" ;;load HTML (must begin with "<")
 ax3SHD="<!DOCTYPE HTML PUBLIC ''-//W3C//DTD HTML 4.0 Transitional//EN''><i>italic</i> <b>bold</b>"
 IntGetFile "http://www.quickmacros.com" ax3SHD
if(!ShowDialog("" &dialog_with_web_browser2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 224 114 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 121 48 14 "Button"
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
	case 4
	 SetWindowText id(3 hDlg) "http://www.google.com"
	 SetWindowText id(3 hDlg) "<body>normal <b>bold</b></body>"
	SetWindowText id(3 hDlg) "<i>italic</i> <b>bold</b>"
	 SHDocVw.WebBrowser b._getcontrol(id(3 hDlg))
	 b.Navigate("http://www.google.com")
	case IDOK
	case IDCANCEL
ret 1

