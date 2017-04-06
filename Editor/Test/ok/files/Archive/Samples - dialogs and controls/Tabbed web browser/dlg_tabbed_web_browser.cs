\Dialog_Editor

 Opens all htm and html files in a folder in separate tabs.
 Change the str files=... line below.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_tabbed_web_browser" &dlg_tabbed_web_browser)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 433 266 "QM tabbed web browser"
 3 SysTabControl32 0x54000040 0x0 10 10 416 250 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""

ret
 messages

type TWB_TAB ~path ~name hwndwb
ARRAY(TWB_TAB)- ta

sel message
	case WM_INITDIALOG
	str files="$qm$\web\*.htm*" ;;change this
	
	 files -> array
	ta=0
	Dir d
	foreach(d files FE_Dir)
		TWB_TAB& r=ta[]
		r.path=d.FileName(1)
		r.name=d.FileName
	ta.sort(0 DTWB_sort)
	
	 array -> tabs
	int htb=id(3 hDlg)
	TCITEM ti.mask=TCIF_TEXT
	int i
	for i 0 ta.len
		ti.pszText=ta[i].name
		SendMessage htb TCM_INSERTITEM i &ti
	
	SelectTab htb -1 ;;none
	
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
	case TCN_SELCHANGE ;;a tab selected
	for(i 0 ta.len) if(ta[i].hwndwb) hid ta[i].hwndwb ;;hide all
	i=SendMessage(nh.hwndFrom TCM_GETCURSEL 0 0)
	if(i<0 or i>=ta.len) ret
	&r=ta[i]
	if(r.hwndwb)
		hid- r.hwndwb ;;if the control already created, unhide
		BringWindowToTop r.hwndwb
	else ;;create control and open document
		htb=id(3 hDlg)
		RECT rct; GetClientRect htb &rct
		SendMessage htb TCM_ADJUSTRECT 0 &rct
		MapWindowPoints htb hDlg +&rct 2
		r.hwndwb=CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" 0 rct.left rct.top rct.right-rct.left rct.bottom-rct.top hDlg 100+i)
		BringWindowToTop r.hwndwb
		r.path.setwintext(r.hwndwb)
