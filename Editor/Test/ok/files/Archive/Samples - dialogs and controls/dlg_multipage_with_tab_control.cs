\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 265 163 "Form"
 1001 Static 0x44020000 0x4 94 60 48 13 "Page0"
 1101 Static 0x44020000 0x4 104 62 48 13 "Page1"
 1201 Static 0x44020000 0x4 110 70 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 3 SysTabControl32 0x54000040 0x0 0 2 266 132 ""
 5 Static 0x54000010 0x20004 4 138 257 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "" " "

if(!ShowDialog(dd &sub.DlgProc)) ret


#sub DlgProc
function# hDlg message wParam lParam

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
	
	DT_Page hDlg 0
	
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
sel nh.code
	case WINAPI.TCN_SELCHANGE
	_i=SendMessage(nh.hwndFrom WINAPI.TCM_GETCURSEL 0 0)
	DT_Page hDlg _i
