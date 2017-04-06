\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
if(!ShowDialog("Dialog_folder_view" &Dialog_folder_view)) ret
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 221 154 "Form"
 3 ActiveX 0x54000000 0x0 4 2 212 128 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 140 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3.Navigate("c:\")

	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	we3._getcontrol(id(3 hDlg))
	Shell32.ShellFolderView fv=we3.Document
	out fv.CurrentViewMode
	fv.CurrentViewMode=4
	
	case IDOK
	case IDCANCEL
ret 1
