\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

ARRAY(str)- t_ai
int- t_i
GetFilesInFolder t_ai "$user profile$\Pictures\Screensaver" "*.jpg"

_s=
 <html>
 <style> BODY { margin: 2px 0px 0px 1px; overflow: auto; } </style>
 <body><a href="" target="_blank"><img src="%s" alt=""></a></body>
 </html>

str controls = "3"
str ax3SHD
ax3SHD.format(_s iif(t_ai.len t_ai[0] ""))

str Function8Layout=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 643 455 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 644 422 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 298 424 50 30 "Next"
 END DIALOG
 DIALOG EDITOR: "" 0x203000B "" "" ""
if(!ShowDialog(Function8Layout &Function82 &controls)) ret
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
	t_i+1; if(t_i>=t_ai.len) ret
	Htm el=htm("IMG" "" "" hDlg 0 0 0x20)
	el.el.setAttribute("src" t_ai[t_i] 1)
ret 1