\Dialog_Editor

 Opens several web pages in tabs.
 Change the str pagesCSV=... below.

function# hDlg message wParam lParam
if(hDlg) goto messages

out
if(!ShowDialog("" &dlg_tabbed_web_browser2 0 0 0 0 0 0 0 0 0 "dlg_tabbed_web_browser2")) ret

 BEGIN DIALOG
 0 "" 0x10CF0A48 0x100 0 0 433 265 "QM tabbed web browser"
 3 SysTabControl32 0x54000040 0x0 0 0 434 268 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "" "" ""

ret
 messages

type TWB_TAB ~path ~name hwndwb
ARRAY(TWB_TAB)- ta
int- iTab
SHDocVw.WebBrowser wbDTWB2
int i htb
RECT rct

sel message
	case WM_INITDIALOG
	 specify tabs in CSV format: title, URL.
	str pagesCSV=
	 Google, www.google.com
	 Bing, www.bing.com
	 Yahoo, www.yahoo.com
	
	 add tabs
	ICsv csv=CreateCsv(1); csv.FromString(pagesCSV)
	htb=id(3 hDlg)
	TCITEMW ti.mask=TCIF_TEXT
	for i 0 csv.RowCount
		TWB_TAB& r=ta[]
		r.path=csv.Cell(i 1)
		ti.pszText=@csv.Cell(i 0)
		SendMessage htb TCM_INSERTITEMW i &ti
	
	iTab=-1
	SelectTab htb -1 ;;none
	SelectTab htb 0 ;;disable this line if don't want to load page now
	
	case WM_APP+1
	 A macro can send this message to select a tab and load a page.
	 wParam is tab index, lParam is URL; if lParam 0, just selects tab.
	 Returns current tab index (before changing).
	 Don't send from other process.
	 EXAMPLES
	 int w=win("QM tabbed web browser" "#32770")
	 SendMessage w WM_APP+1 2 0 ;;select tab 2
	 SendMessage w WM_APP+1 1 "www.quickmacros.com" ;;select tab 1 and load URL
	
	_i=iTab
	if wParam>=0 and wParam<ta.len
		if(wParam!=iTab) SelectTab id(3 hDlg) wParam
		if lParam
			lpstr URL=+lParam
			wbDTWB2._getcontrol(ta[wParam].hwndwb)
			wbDTWB2.Navigate(@URL)
	ret _i
	
	case WM_DESTROY
	
	case WM_SIZE
	htb=id(3 hDlg)
	GetClientRect hDlg &rct; MoveWindow htb rct.left rct.top rct.right-rct.left rct.bottom-rct.top 1
	TabControlGetClientRect htb hDlg rct
	for(i 0 ta.len) if(ta[i].hwndwb) MoveWindow ta[i].hwndwb rct.left rct.top rct.right-rct.left rct.bottom-rct.top 0
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case [101,102,103,104]
	err-
	int hax=ta[iTab].hwndwb
	wbDTWB2._getcontrol(hax)
	sel wParam
		case 101 wbDTWB2.GoBack
		case 102 wbDTWB2.Refresh
		case 103 wbDTWB2.Navigate(ta[iTab].path) ;;Home. Load the start page of this tab.
		case 104 if(inp(_s "URL:" "Go to" "" 0 "" 0 hDlg)) wbDTWB2.Navigate(_s)
	err+
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case TCN_SELCHANGE ;;a tab selected
	htb=nh.hwndFrom
	for(i 0 ta.len) if(ta[i].hwndwb) hid ta[i].hwndwb ;;hide all
	i=SendMessage(htb TCM_GETCURSEL 0 0); if(i<0 or i>=ta.len) ret
	iTab=i
	&r=ta[i]
	if(r.hwndwb)
		hid- r.hwndwb ;;if the control already created, unhide
		BringWindowToTop r.hwndwb
	else ;;create control and open document
		TabControlGetClientRect htb hDlg rct
		r.hwndwb=CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" 0 rct.left rct.top rct.right-rct.left rct.bottom-rct.top hDlg 100+i)
		BringWindowToTop r.hwndwb
		wbDTWB2._getcontrol(r.hwndwb)
		wbDTWB2._setevents("wbDTWB2_DWebBrowserEvents2")
		wbDTWB2.Navigate(r.path)

 BEGIN MENU
 &Back : 101
 &Reload : 102
 &Home : 103
 &GoTo... :104
 END MENU
