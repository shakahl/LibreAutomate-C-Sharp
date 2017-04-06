\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
if(!ShowDialog("dlg_tools" &dlg_tools &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 241 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 224 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 226 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser tw
	tw._getcontrol(id(3 hDlg))
	tw._setevents("tw_DWebBrowserEvents2")
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	str s=
	 <p><a href='http://www.google.com'>google</a></p>
	 
	 <p><a href='qm:test'>qm:test</a></p>
	 
	 <form action='qm:test' method='get'>
	 <input type='submit' value='qm:test'>
	 </form>
	 
	 <p><a href='http://test'>http://test</a></p>
	 
	 <p><a href='javascript:alert(1);'>javascript</a></p>
	 
	 <form action='http://www.google.com/x.php' method='get'>
	 <input type='text' value='ttttt'>
	 <input type='submit' value='google'>
	 </form>
	 
	 <form action='http://test' method='get'>
	 <input type='text' value='ttttt'>
	 <input type='submit' value='http://test'>
	 </form>
	s.setwintext(id(3 hDlg))
	case IDOK
	case IDCANCEL
ret 1
