\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 441 374 "Form"
 3 ActiveX 0x54000000 0x0 0 20 440 352 "SHDocVw.WebBrowser"
 4 ComboBox 0x54230243 0x0 2 4 96 213 ""
 5 Button 0x54032000 0x0 116 2 64 14 "Open selected"
 6 Edit 0x44000080 0x200 236 2 96 14 ""
 7 Button 0x54032000 0x0 182 2 48 14 "Up"
 1 Button 0x54030001 0x0 338 2 48 14 "OK"
 2 Button 0x54030000 0x0 388 2 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "" "" "" ""

str controls = "3 4 6"
str ax3SHD cb4 e6
ax3SHD="$personal$"
cb4="FVM_ICON[]FVM_SMALLICON[]FVM_LIST[]FVM_DETAILS[]FVM_THUMBNAIL[]FVM_TILE[]FVM_THUMBSTRIP"; if(_winver>=0x601) cb4+"[]FVM_CONTENT"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret
out e6


#sub DlgProc
function# hDlg message wParam lParam
SHDocVw.WebBrowser we3
Shell32.ShellFolderView fv
Shell32.FolderItem fi
sel message
	case WM_INITDIALOG
	we3._getcontrol(id(3 hDlg))
	fv=we3.Document
	fv.CurrentViewMode=4
	
	int c=child("" "SysHeader32" id(3 hDlg))
	PostMessage c WM_SETFOCUS 0 0
	PostMessage c WM_KEYDOWN VK_RIGHT 0
	PostMessage c WM_KEYDOWN VK_SPACE 0
	PostMessage c WM_KILLFOCUS 0 0
	
	 Acc ach.Find(id(3 hDlg) "COLUMNHEADER" "Date modified" "class=SysHeader32" 0x1015)
	 ach.Mouse(1)
	 mou
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
ret 1
