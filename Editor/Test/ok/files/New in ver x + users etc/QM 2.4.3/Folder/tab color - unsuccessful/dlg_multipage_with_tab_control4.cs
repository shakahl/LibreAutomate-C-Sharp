 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 265 163 "Form"
 1001 Button 0x54032000 0x0 28 28 48 14 "Button"
 1002 Edit 0x54030080 0x200 28 48 96 12 ""
 1003 Button 0x50012003 0x0 28 68 48 10 "Check"
 1004 Static 0x54000000 0x0 28 84 48 13 "Text"
 1101 Static 0x44020000 0x4 104 62 48 13 "Page1"
 1201 Static 0x44020000 0x4 110 70 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 3 SysTabControl32 0x54000040 0x0 16 8 232 120 ""
 5 Static 0x54000010 0x20004 4 138 257 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "" "0" "" ""
 1001 Static 0x44020000 0x4 24 28 216 93 ""

if(!ShowDialog(dd &sub.DlgProc)) ret
 6 SysTabControl32 0x50000040 0x0 16 8 232 120 ""
 99 #32770 0x50030000 0x0 20 28 222 96 ""


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	
	 EnableThemeDialogTexture hDlg ETDT_ENABLETAB
	 EnableThemeDialogTexture id(99 hDlg) ETDT_ENABLETAB
	DT_SetBackgroundColor hDlg 0 0xe08080
	 DT_SetBackgroundImage hDlg "$my qm$\Copy.jpg"
	
	 SetParent id(1003 hDlg) id(3 hDlg)
	
	int htb=id(3 hDlg)
	TCITEM ti.mask=WINAPI.TCIF_TEXT
	ti.pszText="A"
	SendMessage htb WINAPI.TCM_INSERTITEMA 0 &ti
	ti.pszText="B"
	SendMessage htb WINAPI.TCM_INSERTITEMA 1 &ti
	ti.pszText="C"
	SendMessage htb WINAPI.TCM_INSERTITEMA 2 &ti
	
	DT_Page hDlg _i
	
	 SetWinStyle id(3 hDlg) 0x50000040
	 SetWinStyle id(6 hDlg) 0x50000040
	
	 int hTab=id(3 hDlg)
	 int hDlg0=sub.Dialog2(hDlg)
	
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


#sub Dialog2
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x50000048 0x0 20 26 130 94 "Dialog"
 3 Edit 0x54030080 0x200 16 24 96 12 ""
 4 Button 0x54032000 0x0 16 40 48 14 "Button"
 5 Button 0x54012003 0x0 16 60 48 10 "Check"
 6 Static 0x54000000 0x0 16 8 48 12 "Text"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3 5"
str e3 c5Che
ret ShowDialog(dd &sub.DlgProc2 &controls hwndOwner 0x101)


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	EnableThemeDialogTexture hDlg ETDT_ENABLETAB
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
