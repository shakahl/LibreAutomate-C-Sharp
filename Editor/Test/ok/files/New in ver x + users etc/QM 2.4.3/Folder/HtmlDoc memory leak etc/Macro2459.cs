/exe 4
out
int w=win("Internet Explorer" "IEFrame")
Htm el=htm("HTML" "" "" w 0 0 32)
_s=el.el.outerHTML
_s.setfile("$temp$\html.htm")
 rep 10
	sub.Thread _s
	 wait 0 H mac("sub.Thread" "" _s)
 mes 1


#sub Thread
function $HTML

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 396 322 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 398 322 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3"
str ax3SHD
 ax3SHD="http://www.msn.com/?ocid=iehp"
 ax3SHD="http://www.quickmacros.com/forum/index.php?sid=ac47a404aa01920a1c5071aec94a729a"
rep 10
	ShowDialog(dd &sub.DlgProc &controls)


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	opt waitmsg 1
	 we3.Navigate(_s.expandpath("$temp$\html.htm"))
	we3.Navigate("http://www.msn.com/?ocid=iehp")
	rep
		0.01
		if(!we3.Busy) break
		 out "busy"
	
	 we3.Navigate("about:blank"); 0.1
	
	clo hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
