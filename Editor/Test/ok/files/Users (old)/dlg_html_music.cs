\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=
 <html>
 <head>
 <embed src="http://www.quickmacros.com/test/test.mp3">
 </head>
 <body>
 -
 </body>
 </html>
if(!ShowDialog("dlg_html_music" &dlg_html_music &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 3 ActiveX 0x54030000 0x0 0 22 222 114 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
str s
sel wParam
	case IDOK
	case IDCANCEL
ret 1
