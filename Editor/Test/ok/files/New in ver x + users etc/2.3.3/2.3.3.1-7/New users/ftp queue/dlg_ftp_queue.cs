\Dialog_Editor

 Shows dialog where user can select files and ftp.
 Saves the list in "$my qm$\ftp queue.csv".

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
qmg3x.getfile("$my qm$\ftp queue.csv"); err
if(!ShowDialog("dlg_ftp_queue" &dlg_ftp_queue &controls)) ret
qmg3x.setfile("$my qm$\ftp queue.csv")

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 114 "0x0,0,0,0,0x0[]File,,16,[]FTP,,1,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
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
	GRID.QM_NMLVDATA* cd=+nh
	sel nh.code
		case GRID.LVN_QG_BUTTONCLICK ;;when user clicks button
		if cd.subitem=0
			if(OpenSaveDialog(0 _s)) _s.setwintext(cd.hctrl)
		
		case GRID.LVN_QG_COMBOFILL ;;when user clicks combo box arrow
		if cd.subitem=1
			TO_CBFill cd.hcb "ftp.one.com[]ftp.two.com[]ftp.three.com"
