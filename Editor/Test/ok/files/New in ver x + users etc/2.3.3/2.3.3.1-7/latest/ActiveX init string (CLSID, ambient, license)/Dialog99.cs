\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int dlFlags
dlFlags=0x40004400 ;;don't download images, sounds, videos, ActiveX; DLCTL_PRAGMA_NO_CACHE (does not work), DLCTL_SILENT (not necessary if wb.Silent=1). Not used DLCTL_DOWNLOADONLY because does not work.
 dlFlags=0x00000010
if(0) dlFlags|0x380 ;;DLCTL_NO_SCRIPTS, DLCTL_NO_JAVA, DLCTL_NO_RUNACTIVEXCTLS

str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 905 433 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 430 430 "SHDocVw.WebBrowser"
 4 ActiveX 0x54030000 0x0 436 0 468 430 "SHDocVw.WebBrowser ambient:{&WebBrowser_AmbientProc},{dlFlags}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""
 4 ActiveX 0x54030000 0x0 436 0 468 430 "SHDocVw.WebBrowser"

str controls = "3 4"
str ax3SHD ax4SHD
ax3SHD="http://www.google.com"
if(!ShowDialog(dd &Dialog99 &controls)) ret

ret
 messages
sel message
	case WM_INITDIALOG
	 SHDocVw.WebBrowser we3
	 we3._getcontrol(id(3 hDlg))
	IntGetFile "http://www.google.com" _s
	HtmlToWebBrowserControl id(3 hDlg) _s
	
	SHDocVw.WebBrowser we4
	we4._getcontrol(id(4 hDlg))
	we4.Navigate("http://www.google.com")
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
