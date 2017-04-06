\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=""
if(!ShowDialog("dlg_web_browser_autosize" &dlg_web_browser_autosize &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 825 328 "Dialog"
 3 ActiveX 0x54030000 0x0 0 14 364 314 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 0 48 14 "google"
 5 Button 0x54032000 0x0 48 0 48 14 "ms"
 6 Button 0x54032000 0x0 96 0 48 14 "qm"
 7 Button 0x54032000 0x0 144 0 48 14 "qm help"
 8 Button 0x54032000 0x0 192 0 48 14 "qm forum"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

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
	web_browser_load_page_and_autosize id(3 hDlg) "www.google.com"
	case 5
	web_browser_load_page_and_autosize id(3 hDlg) "www.microsoft.com"
	case 6
	web_browser_load_page_and_autosize id(3 hDlg) "www.quickmacros.com"
	case 7
	web_browser_load_page_and_autosize id(3 hDlg) "www.quickmacros.com/help"
	case 8
	web_browser_load_page_and_autosize id(3 hDlg) "www.quickmacros.com/forum"
ret 1
