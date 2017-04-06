\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 154 108 "Dialog"
 4 SysPager 0x54000000 0x0 0 0 8 8 ""
 1 Button 0x54030001 0x4 8 88 48 14 "OK"
 2 Button 0x54030000 0x4 60 88 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 _hwndqm)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	int hPager=id(4 hDlg)
	RECT rc; GetClientRect hDlg &rc; siz rc.right rc.bottom-40 hPager
	int hChildDlg=sub.Dialog2(hPager)
	SendMessage hPager PGM_SETCHILD 0 hChildDlg
	
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
	case 4
	sel nh.code
		case PGN_CALCSIZE
		RECT r2; GetClientRect id(0 nh.hwndFrom) &r2
		NMPGCALCSIZE& pcs=+nh
		pcs.iWidth=r2.right
		pcs.iHeight=r2.bottom
		

#sub Dialog2
function# hwndParent

str dd=
 BEGIN DIALOG
 0 "" 0x50000048 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 8 104 56 "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
 4 Button 0x54032000 0x0 112 64 104 64 "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ"
 5 Button 0x54012003 0x0 140 20 48 10 "Check"
 6 Edit 0x54030080 0x200 8 92 96 13 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "5 6"
str c5Che e6
ret ShowDialog(dd 0 &controls hwndParent 1)
