\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 565 386 "My Web Browser"
 4 SysTabControl32 0x54000040 0x0 2 20 302 362 ""
 5 Button 0x54032000 0x0 458 4 48 14 "GO"
 6 Button 0x54032000 0x0 512 4 48 14 "BACK"
 8 QM_Splitter 0x54030000 0x0 300 12 10 362 ""
 3 ActiveX 0x56030000 0x0 310 20 248 362 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

str controls = "3"
str ax3SHD 

if(!ShowDialog("MultiTabBrowserWithSplitters" &MultiTabBrowserWithSplitters &controls)) ret

ret
 messages
DlgSplitter ds.Init(hDlg 8)
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser c
	c._getcontrol(id(3 hDlg)) 
	 int- t_hdlg; t_hdlg=hDlg
	 c._setevents("c_DWebBrowserEvents2") 
	c.Navigate("www.google.com"); err


	int+ htb=id(4 hDlg) 
	TCITEM ti.mask=WINAPI.TCIF_TEXT 
	ti.pszText="Google"
	SendMessage htb WINAPI.TCM_INSERTITEMA 0 &ti 
	ti.pszText="Yahoo" 
	SendMessage htb WINAPI.TCM_INSERTITEMA 1 &ti
	ti.pszText="QuickMacros.Com" 
	SendMessage htb WINAPI.TCM_INSERTITEMA 2 &ti
	SelectTab htb 2 ;;none
	DT_Page hDlg _i
	
	case WM_DESTROY
		rset ds.GetPos "splitter" "\test\MultiTabBrowserWithSplitters" ;;save splitter pos
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
c._getcontrol(id(3 hDlg))
sel wParam
	case 5 ;;Go
	str a.getwintext(id(4 hDlg))
	c.Navigate(a); err
	
	case 6
	c.GoBack; err

	case IDOK
	but 5 hDlg ;;on Enter press GO
	ret 0 ;;disable closing on Enter
	case IDCANCEL
	ifk(Z) ret 0 ;;disable closing on Esc
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case WINAPI.TCN_SELCHANGE
	_i=SendMessage(nh.hwndFrom WINAPI.TCM_GETCURSEL 0 0)
	out _i
	 int htb=id(4 hDlg)
	RECT rct; GetClientRect htb &rct
	SendMessage htb TCM_ADJUSTRECT 0 &rct
	MapWindowPoints htb hDlg +&rct 2
	 hid id(103 hDlg); err
	DestroyWindow id(103 hDlg)
	int t=CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" WS_CLIPCHILDREN rct.left rct.top rct.right-rct.left rct.bottom-rct.top hDlg 100+3)
	ds.AttachControls(1)
	BringWindowToTop t
	sel _i
		case 0; _s="http://www.Google.com"; _s.setwintext(t)
		case 1; _s="http://www.yahoo.com/"; _s.setwintext(t)
		case 2; _s="http://www.quickmacros.com/"; _s.setwintext(t) 
	DT_Page hDlg _i