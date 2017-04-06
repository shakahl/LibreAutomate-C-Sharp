 /test dialog image preview
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 446 258 "Dialog"
 3 SysListView32 0x54004003 0x0 0 0 176 238 ""
 1 Button 0x54030001 0x4 4 242 48 14 "OK"
 2 Button 0x54030000 0x4 54 242 48 14 "Cancel"
 4 ActiveX 0x54030000 0x0 182 0 264 238 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
DIPDATA- d
sel message
	case WM_INITDIALOG
	d.hDlg=hDlg; d.hlv=id(3 hDlg); d.hwb=id(4 hDlg)
	DIP_AddItems
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	sel nh.code
		case LVN_ITEMCHANGED
		NMITEMACTIVATE* na=+nh
		if(na.uNewState&LVIS_SELECTED and na.uOldState&LVIS_SELECTED=0) ;;listview item selected
			DIP_Preview na.iItem


 info: i tried to use ShellFolderView, but it does not give enough control, eg cannot filter file types
