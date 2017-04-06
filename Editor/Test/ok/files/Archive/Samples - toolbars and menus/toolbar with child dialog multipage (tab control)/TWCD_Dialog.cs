 \Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x10000448 0x0 0 0 267 135 "Form"
 1102 Button 0x54012003 0x0 104 80 48 12 "Check"
 1002 Edit 0x54030080 0x200 144 60 96 14 ""
 1001 Static 0x44020000 0x4 94 60 48 13 "Page0"
 1101 Static 0x44020000 0x4 104 62 48 13 "Page1"
 1201 Static 0x44020000 0x4 110 70 48 13 "Page2"
 3 SysTabControl32 0x54000040 0x0 0 2 266 132 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010804 "" " "

type TCITEM mask dwState dwStateMask $pszText cchTextMax iImage lParam

 messages
sel message
	case WM_INITDIALOG
	
	int htb=id(3 hDlg)
	TCITEM ti.mask=WINAPI.TCIF_TEXT
	ti.pszText="A"
	SendMessage htb WINAPI.TCM_INSERTITEMA 0 &ti
	ti.pszText="B"
	SendMessage htb WINAPI.TCM_INSERTITEMA 1 &ti
	ti.pszText="C"
	SendMessage htb WINAPI.TCM_INSERTITEMA 2 &ti
	
	goto g11
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 4
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case WINAPI.TCN_SELCHANGE
	_i=SendMessage(nh.hwndFrom WINAPI.TCM_GETCURSEL 0 0)
	 g11
	DT_Page hDlg _i
