\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("eb_dialog_main" &eb_dialog_main)) ret ;;this code can be in other function, not necessary here

 Windows declarations
def TCIF_TEXT 0x0001
def TCM_FIRST 0x1300 
def TCM_INSERTITEMA (TCM_FIRST + 7)
def TCM_ADJUSTRECT (TCM_FIRST + 40)
def TCM_SETCURSEL (TCM_FIRST + 12)
def TCM_GETCURSEL (TCM_FIRST + 11)
def TCM_DELETEITEM (TCM_FIRST + 8)
def TCM_SETCURSEL (TCM_FIRST + 12)
def TCN_FIRST (0U-550U) 
def TCN_SELCHANGE (TCN_FIRST - 1)
def TCM_GETCURSEL (TCM_FIRST + 11)
type TCITEM mask dwState dwStateMask $pszText cchTextMax iImage lParam
 Dialog data declarations
type EB_TAB_DATA hdlg str'tabname str'document ;;tab data. Add more members if needed (then restart QM).
type EB_DATA ARRAY(EB_TAB_DATA)a ;;dialog data. Add more members if needed.
EB_DATA _d

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 269 178 "EB"
 9 Static 0x54000000 0x0 2 0 22 12 "Name"
 8 Edit 0x54030080 0x200 26 0 64 14 ""
 6 Button 0x54032000 0x0 94 0 48 14 "Add"
 7 Button 0x54032000 0x0 144 0 48 14 "Delete"
 1 Button 0x54030001 0x4 6 162 48 14 "OK"
 2 Button 0x54030000 0x4 60 162 48 14 "Cancel"
 4 Button 0x54032000 0x4 112 162 18 14 "?"
 3 SysTabControl32 0x54010040 0x0 2 18 266 132 ""
 5 Static 0x54000010 0x20004 4 156 254 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010806 "" " "

ret
 messages
EB_DATA* d ;;dialog data
if(message=WM_INITDIALOG) DT_Init(hDlg lParam); SetProp hDlg "d" d._new
d=+GetProp(hDlg "d")
int i j htab(id(3 hDlg))
sel message
	case WM_INITDIALOG
	ret 1
	case WM_DESTROY
	DT_DeleteData(hDlg)
	d._delete; RemoveProp hDlg "d"
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 6 ;;Add
	 add array item
	i=d.a.redim(-1)
	EB_TAB_DATA& t=d.a[i]
	t.tabname.getwintext(id(8 hDlg)) ;;example
	t.document=t.tabname ;;example
	 add tab
	TCITEM ti.mask=TCIF_TEXT
	ti.pszText=t.tabname
	SendMessage htab TCM_INSERTITEMA 10000 &ti
	 add child dialog
	t.hdlg=ShowDialog("eb_dialog_doc" &eb_dialog_doc 0 hDlg 0 WS_CHILD)
	RECT r
	GetWindowRect htab &r; MapWindowPoints 0 hDlg +&r 2
	SendMessage htab TCM_ADJUSTRECT 0 &r
	SetWindowPos t.hdlg 0 r.left r.top r.right-r.left r.bottom-r.top 0
	SendMessage htab TCM_SETCURSEL i 0
	goto select
	
	case 7 ;;Delete
	i=SendMessage(htab TCM_GETCURSEL 0 0); if(i<0) ret
	SendMessage htab TCM_DELETEITEM i 0
	&t=d.a[i]
	DestroyWindow t.hdlg
	d.a.remove(i)
	if(d.a.len)
		if(i) i-1
		SendMessage htab TCM_SETCURSEL i 0
		goto select
	
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case TCN_SELCHANGE
	i=SendMessage(htab TCM_GETCURSEL 0 0); if(i<0) ret
	 select
	for(j 0 d.a.len) hid d.a[j].hdlg
	&t=d.a[i]
	hid- t.hdlg; act t.hdlg
	t.document.setwintext(id(3 t.hdlg)) ;;example
	_s.from("EB - " t.tabname); _s.setwintext(hDlg) ;;example
	
