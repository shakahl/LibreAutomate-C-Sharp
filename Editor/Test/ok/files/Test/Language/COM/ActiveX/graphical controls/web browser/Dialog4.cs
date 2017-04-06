\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
 ax3SHD="http://www.quickmacros.com"
 ax3SHD="C:\Documents and Settings\G\Desktop\doc1.htm"
 ax3SHD="$desktop$\tools"
ax3SHD="$personal$\book1.xls"
if(!ShowDialog("Dialog4" &Dialog4 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 367 247 "Form"
 3 ActiveX 0x54000000 0x4 0 0 368 246 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
