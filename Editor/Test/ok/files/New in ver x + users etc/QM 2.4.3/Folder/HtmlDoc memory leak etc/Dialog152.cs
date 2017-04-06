\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 352 294 "Dialog"
 3 ActiveX 0x54030000 0x0 0 16 352 278 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 0 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3"
str ax3SHD
ax3SHD="http://www.msn.com/?ocid=iehp"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	opt waitmsg 1
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3.Navigate("about:blank"); 0.1
	 we3.Navigate(_s.expandpath("$temp$\html.htm"))
	we3.Navigate("http://www.msn.com/?ocid=iehp")
	 rep
		 0.01
		 if(!we3.Busy) break
ret 1
