\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 265 163 "Form"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 3 SysTabControl32 0x54030040 0x0 16 8 232 120 ""
 5 Static 0x54000010 0x20004 4 138 257 1 ""
 6 Static 0x5400000E 0x0 4 20 252 112 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "" "0" "" ""

if(!ShowDialog(dd &sub.DlgProc)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	
	DT_SetBackgroundColor hDlg 1 0x80e0a0 0x80e0e0
	 DT_SetBackgroundImage hDlg "$my qm$\Copy.jpg"
	
	int htb=id(3 hDlg)
	TCITEM ti.mask=WINAPI.TCIF_TEXT
	ti.pszText="A"
	SendMessage htb WINAPI.TCM_INSERTITEMA 0 &ti
	ti.pszText="B"
	SendMessage htb WINAPI.TCM_INSERTITEMA 1 &ti
	ti.pszText="C"
	SendMessage htb WINAPI.TCM_INSERTITEMA 2 &ti
	
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
	DT_Page id(99 hDlg) _i


#sub Dialog2
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x50000448 0x0 20 26 130 94 "Dialog"
 1003 Edit 0x54030080 0x200 16 24 96 12 ""
 1004 Button 0x54032000 0x0 16 40 48 14 "Button"
 1005 Button 0x54012003 0x0 16 60 48 10 "Check"
 1006 Static 0x54000000 0x0 16 8 48 12 "Page0"
 1106 Static 0x54000000 0x0 16 8 48 12 "Page1"
 1206 Static 0x54000000 0x0 16 8 48 12 "Page2"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "0" "" ""

str controls = "1003 1005"
str e1003 c1005Che
ret ShowDialog(dd &sub.DlgProc2 &controls hwndOwner 0x101)


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SetWindowLong hDlg GWL_ID 99
	EnableThemeDialogTexture hDlg ETDT_ENABLETAB
	DT_Page hDlg 0
	SetFocus id(1003 hDlg)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
