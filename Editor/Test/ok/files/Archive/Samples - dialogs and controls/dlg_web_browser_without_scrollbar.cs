/Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "8"
str ax8SHD
ax8SHD=
 <html>
 <style> BODY { margin: 15px 0px 0px 20px; overflow: auto; } </style>
 <body><img src="http://www.quickmacros.com/forum/images/smilies/icon_lol.gif"></body>
 </html>
if(!ShowDialog("dlg_web_browser_without_scrollbar" &dlg_web_browser_without_scrollbar &controls)) ret

 BEGIN DIALOG
 0 "" 0x90880A48 0x100 0 0 53 60 "Dialog"
 2 Button 0x54032000 0x0 6 42 42 14 "close"
 8 ActiveX 0x54030000 0x2020 6 6 42 32 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""
ret
 messages
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
ret 1