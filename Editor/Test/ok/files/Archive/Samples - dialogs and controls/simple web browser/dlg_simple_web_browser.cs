\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 565 386 "My Web Browser"
 3 ActiveX 0x54030000 0x0 4 22 558 358 "SHDocVw.WebBrowser"
 4 Edit 0x54030080 0x200 4 4 450 14 ""
 5 Button 0x54032000 0x0 458 4 48 14 "GO"
 6 Button 0x54032000 0x0 512 4 48 14 "BACK"
 END DIALOG
 DIALOG EDITOR: "" 0x2020103 "" "" ""

str controls = "3 4"
str ax3SHD e4
if(!ShowDialog("dlg_simple_web_browser" &dlg_simple_web_browser &controls)) ret

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser c
	c._getcontrol(id(3 hDlg))
	
	int- t_hdlg; t_hdlg=hDlg
	c._setevents("c_DWebBrowserEvents2")
	
	c.Navigate("www.google.com"); err
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
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
