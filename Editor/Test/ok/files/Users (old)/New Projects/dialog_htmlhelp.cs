\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 info: also tried with hhctrl, does not work.

str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 148 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 134 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 134 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""
 3 ActiveX 0x54030000 0x0 0 0 224 134 "SHDocVw.WebBrowser ambient:{&WebBrowser_AmbientProc},{0x4000}"

str controls = "3"
str ax3SHD
 ax3SHD="mk:@MSITStore:Q:\app\QM2Help.chm::/Commands/IDP_LEF.html"
 ax3SHD="mk:@MSITStore:Q:\app\QM2Help.chm::/QM_Help/IDH_INTERFACE.html"
ax3SHD="ms-its:Q:\app\QM2Help.chm::/QM_Help/IDH_INTERFACE.html" ;;same as mk:@MSITStore
 ax3SHD="http://www.quickmacros.com/test/test.html"
if(!ShowDialog(dd &dialog_htmlhelp &controls)) ret

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	 we3.Navigate("mk:@MSITStore:Q:\app\QM2Help.chm::/QM_Help/IDH_INTERFACE.html")
	 we3.Navigate("http://www.quickmacros.com")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	we3._getcontrol(id(3 hDlg))
	MSHTML.IHTMLDocument2 d=we3.Document
	 d.clear
	 d.close
	 d.open("text/html")
	 ret
	 ARRAY(VARIANT) a.create(1)
	 a[0]="<h3>Parameters</h3>"
	 d.write(a)
	d.body.innerHTML="<h3>Parameters</h3>"
	
	case IDCANCEL
ret 1
