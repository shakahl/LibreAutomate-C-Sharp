\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=""
hDlg=ShowDialog("" &dlg_translate1 &controls 0 1)
opt waitmsg 1
wait 0 -WC hDlg

 to make invisible, change 0x90C800C8 to 0x80C800C8

 BEGIN DIALOG
 0 "" 0x80C800C8 0x0 0 0 491 333 "dlg_translate1"
 3 ActiveX 0x54030000 0x0 0 0 492 332 "SHDocVw.WebBrowser"
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
	case IDOK
	case IDCANCEL
ret 1
