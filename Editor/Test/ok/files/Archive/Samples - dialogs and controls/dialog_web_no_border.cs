\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=
 <html>
 <head></head>
 <body style="border: 0px; margin: 2px; overflow: hidden;">
 <p>test</p>
 </body>
 </html>

if(!ShowDialog("dialog_web_no_border" &dialog_web_no_border &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 204 146 "Form"
 3 ActiveX 0x54000000 0x0 14 12 104 93 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
