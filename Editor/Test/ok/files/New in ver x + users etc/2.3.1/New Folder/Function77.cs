\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &Function77)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 265 163 "Form"
 9002 ComboBox 0x54230A42 0x0 58 88 96 213 ""
 1001 Static 0x44020000 0x4 94 60 48 13 "Page0"
 1101 Static 0x44020000 0x4 104 62 48 13 "Page1"
 1201 Static 0x44020000 0x4 110 70 48 13 "Page2"
 1 Button 0x54030001 0x4 142 146 48 14 "OK"
 2 Button 0x54030000 0x4 192 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 242 146 18 14 "?"
 3 SysTabControl32 0x54000040 0x0 0 2 266 132 ""
 5 Static 0x54000010 0x20004 4 138 256 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x203000A "" "0" ""

type TCITEM mask dwState dwStateMask $pszText cchTextMax iImage lParam

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	
	int htb=id(3 hDlg)
	TCITEM ti.mask=WINAPI.TCIF_TEXT
	ti.pszText="A"
	SendMessage htb WINAPI.TCM_INSERTITEMA 0 &ti
	ti.pszText="B"
	SendMessage htb WINAPI.TCM_INSERTITEMA 1 &ti
	ti.pszText="C"
	SendMessage htb WINAPI.TCM_INSERTITEMA 2 &ti
	
	goto g11
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case WINAPI.TCN_SELCHANGE
	_i=SendMessage(nh.hwndFrom WINAPI.TCM_GETCURSEL 0 0)
	 g11
	DT_Page hDlg _i "(0 80) (1 80)"
